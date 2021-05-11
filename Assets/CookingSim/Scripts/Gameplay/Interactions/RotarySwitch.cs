using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRAccelerator.Gameplay
{

    [Serializable]
    public  class ValueChanged : UnityEvent <float>{ } ;
    public class RotarySwitch : XRSimpleInteractable
    {
        private enum RotationAxis {X, Y, Z};

        private const float k_debugRadius = 0.1f;

        [Header("Rotary Switch Specific")]
        [SerializeField]
        [Tooltip("The number of states to switch between")]
        private int numberOfStates = 2;

        [SerializeField]
        [Tooltip("The index of the state to start on (1=start)")]
        private int indexOfStartingState = 1;

        [SerializeField]
        [Tooltip("The angle between states")]
        private float angleBetweenStates = 45f;

        [SerializeField]
        [Tooltip("The speed at which the rotation settles")]
        private float tweenSpeed = 1f;

        [SerializeField]
        [Tooltip("The speed at which the rotation settles")]
        private RotationAxis rotationAxis;

        [SerializeField]
        [Tooltip("Proxy Hands references")]
        private ProxyHandsVisuals handsVisuals;

        [NonSerialized]
        public Action<int> StateChanged;

        [Tooltip("Notifies the normalized value of the switch")]
        public ValueChanged valueChanged;

        private Quaternion? startingControllerRotation;
        private Quaternion? startingRotation;
        private Transform currentControllerTransform;
        private int previousIndex;

        private bool isTweening;
        private float elapsedTweenTime;
        private bool IsInteracting => currentControllerTransform != null;

        private Transform _transform;

        public int NumberOfStates => numberOfStates;
        public int CurrentStateIndex { get; private set; } = 0;

        public void IncreaseStateIndex()
        {
            ChangeState(CurrentStateIndex + 1);
        }

        public void DecreaseStateIndex()
        {
            ChangeState(CurrentStateIndex - 1);
        }

        public void JumpToIndex(int index, bool triggerEvents = false)
        {
            ChangeState(index, triggerEvents);
        }

        private void Interacting()
        {
            float angle = GetSignedAngleForDesiredAxis(startingControllerRotation.Value, currentControllerTransform.rotation);
            angle += GetAngleFromDesiredAxis(startingRotation.Value);

            angle = AngleClamp(angle, 0f, angleBetweenStates * (numberOfStates - 1));

            _transform.localRotation = CreateRotationOnDesiredAxis(angle);

            // check for new state
            float division = GetAngleFromDesiredAxis(_transform.localRotation) / angleBetweenStates;
            int index = Mathf.RoundToInt(division);

            ChangeState(index);
            valueChanged?.Invoke(division);
        }

        private void NotInteracting()
        {
            // tween rotation to index
            if (isTweening)
            {
                float targetAngle = angleBetweenStates * CurrentStateIndex;
                if (!Mathf.Approximately(GetAngleFromDesiredAxis(_transform.localRotation), targetAngle))
                {
                    float previousAngle = angleBetweenStates * previousIndex;
                    elapsedTweenTime += Time.deltaTime * tweenSpeed;

                    _transform.localRotation = Quaternion.Lerp(
                        CreateRotationOnDesiredAxis(previousAngle),
                        CreateRotationOnDesiredAxis(targetAngle),
                        elapsedTweenTime);
                }
                else
                {
                    _transform.localRotation = CreateRotationOnDesiredAxis(targetAngle);
                    startingControllerRotation = null;
                    isTweening = false;
                }
            }
        }

        private void Update()
        {
            if (IsInteracting)
            {
                Interacting();
            }
            else
            {
                NotInteracting();
            }
        }

        private void OnBeginInteraction(XRBaseInteractor interactor)
        {
            Debug.Assert(!IsInteracting, "Starting concurrent interaction on rotarySwitch", gameObject);

            currentControllerTransform = interactor.transform;
            startingControllerRotation = currentControllerTransform.rotation;
            startingRotation = _transform.localRotation;
            isTweening = false;

            handsVisuals.EnableProxyHandVisual(interactor.GetComponent<ActionBasedController>(), interactor);
        }

        private void OnEndInteraction(XRBaseInteractor interactor)
        {
            currentControllerTransform = null;
            isTweening = true;

            float previousAngle = angleBetweenStates * previousIndex;
            elapsedTweenTime = Mathf.Abs(GetAngleFromDesiredAxis(_transform.localRotation) - previousAngle) / angleBetweenStates * tweenSpeed;
            handsVisuals.DisableProxyHandVisual();
        }

        /// <summary>
        /// Change the current state
        /// </summary>
        /// <param name="index"></param>
        /// <param name="time">the time passed between states to kick off at</param>
        private void ChangeState(int index, bool triggerEvents = true)
        {
            // TODO Arthur: Add instant state change support

            if (index == CurrentStateIndex)
                return;

            Debug.Assert(index < numberOfStates && index >= 0,
                $"Trying to change rotary switch to invalid state index: {index}", gameObject);
            index = Mathf.Clamp(index, 0, numberOfStates);

            previousIndex = CurrentStateIndex;
            CurrentStateIndex = index;

            if (triggerEvents)
            {
                StateChanged?.Invoke(CurrentStateIndex);
            }
        }

        #region Math

        private Quaternion CreateRotationOnDesiredAxis(float angle)
        {
            switch (rotationAxis)
            {
                case RotationAxis.X:
                    return Quaternion.Euler(angle, 0f, 0f);
                case RotationAxis.Y:
                    return Quaternion.Euler(0f, angle, 0f);
                case RotationAxis.Z:
                    return Quaternion.Euler(0f, 0f, angle);
            }

            throw new NotSupportedException();
        }

        private float GetAngleFromDesiredAxis(Quaternion rotation)
        {
            switch (rotationAxis)
            {
                case RotationAxis.X:
                    return rotation.eulerAngles.x;
                case RotationAxis.Y:
                    return rotation.eulerAngles.y;
                case RotationAxis.Z:
                    return rotation.eulerAngles.z;
            }

            throw new NotSupportedException();
        }

        /// <summary>
        /// Get a signed angle for the difference between two quaterions given an axis
        /// </summary>
        /// <param name="A"></param>
        /// <param name="B"></param>
        /// <param name="axis"></param>
        /// <returns></returns>
        private float GetSignedAngleForDesiredAxis(Quaternion A, Quaternion B)
        {
            Vector3 axis = _transform.up;
            if (rotationAxis == RotationAxis.X)
            {
                axis = _transform.right;
            }
            else if (rotationAxis == RotationAxis.Z)
            {
                axis = _transform.forward;
            }

            (B * Quaternion.Inverse(A)).ToAngleAxis(out float angle, out Vector3 angleAxis);

            if (Vector3.Angle(axis, angleAxis) > 90f)
            {
                angle = -angle;
            }

            return Mathf.DeltaAngle(0f, angle);
        }

        /// <summary>
        /// Clamp an angle with relation to 360
        /// </summary>
        /// <param name="angle"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private float AngleClamp(float angle, float min, float max)
        {
            if (angle > 180)
                angle -= 360;
            angle = Mathf.Max(Mathf.Min(angle, max), min);
            if (angle < 0)
                angle += 360;
            return angle;
        }

        #endregion

        protected override void Awake()
        {
            Debug.Assert(indexOfStartingState > 0, "Rotary Switch starting state must be greater than 0", gameObject);
            Debug.Assert(numberOfStates > 1, "Rotary Switch number of states must be greater than 1", gameObject);
            Debug.Assert(angleBetweenStates > 0f, "Rotary Switch angle between states must be greater than 0", gameObject);
            Debug.Assert(numberOfStates > indexOfStartingState, "Rotary Switch starting state must be less than number of states", gameObject);

            handsVisuals.Setup();
            JumpToIndex(indexOfStartingState - 1);
            _transform = transform;

            onSelectEntered.AddListener(OnBeginInteraction);
            onSelectExited.AddListener(OnEndInteraction);

            base.Awake();
        }
    }
}