using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRAccelerator.Gameplay
{
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

        [NonSerialized]
        public Action<int> StateChanged;

        private Quaternion? startingControllerRotation;
        private Quaternion? startingRotation;
        private Transform currentControllerTransform;
        private int previousIndex;

        private bool isTweening;
        private float elapsedTweenTime;
        private bool IsInteracting => currentControllerTransform != null;

        private Transform _transform;

        /// <summary>
        /// Return the number of states
        /// </summary>
        public int NumberOfStates => numberOfStates;

        /// <summary>
        /// Whether or not the button is pressed
        /// </summary>
        public int CurrentStateIndex { get; private set; } = 0;

        /// <summary>
        /// Increase the state index to the upper state
        /// </summary>
        public void IncreaseStateIndex()
        {
            ChangeState(CurrentStateIndex + 1);
        }

        /// <summary>
        /// Decrease the state index to the lower state
        /// </summary>
        public void DecreaseStateIndex()
        {
            ChangeState(CurrentStateIndex - 1);
        }

        /// <summary>
        /// Jump to a state index
        /// </summary>
        /// <param name="index"></param>
        /// <param name="triggerEvents"></param>
        public void JumpToIndex(int index, bool triggerEvents = true)
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
        }

        private void OnEndInteraction(XRBaseInteractor interactor)
        {
            currentControllerTransform = null;
            isTweening = true;

            float previousAngle = angleBetweenStates * previousIndex;
            elapsedTweenTime = Mathf.Abs(GetAngleFromDesiredAxis(_transform.localRotation) - previousAngle) / angleBetweenStates * tweenSpeed;
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
                axis = _transform.forward;
            }
            else if (rotationAxis == RotationAxis.Z)
            {
                axis = _transform.right;
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

            JumpToIndex(indexOfStartingState - 1, false);
            _transform = transform;

            onSelectEnter.AddListener(OnBeginInteraction);
            onSelectExit.AddListener(OnEndInteraction);

            base.Awake();
        }

        #region Debug
#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            float angle = -Mathf.Deg2Rad * angleBetweenStates;

            Matrix4x4 rotationMatrix = Matrix4x4.TRS(_transform.position, _transform.parent.rotation, _transform.lossyScale);
            Gizmos.matrix = rotationMatrix;

            // draw the state points
            for (int i = 0; i < numberOfStates; i++)
            {
                // adj = cos@ * hyp
                float x = Mathf.Cos(angle * i) * k_debugRadius;
                // opp = sin@ * hyp
                float y = Mathf.Sin(angle * i) * k_debugRadius;

                // create target position (y up)
                Vector3 target = new Vector3(x, 0f, y);

                Gizmos.DrawRay(Vector3.zero, target* 10);
                Gizmos.DrawRay(Vector3.down, target * 10);

            }

            Gizmos.DrawSphere(Vector3.zero, 0.01f);
            Gizmos.DrawLine(Vector3.zero, Vector3.down);
            Gizmos.DrawSphere(Vector3.down * 0.1f, 0.01f);
            Gizmos.DrawSphere(Vector3.down * 0.2f, 0.01f);
            Gizmos.DrawSphere(Vector3.down * 0.3f, 0.01f);
            Gizmos.DrawSphere(Vector3.down * 0.4f, 0.01f);
            Gizmos.DrawSphere(Vector3.down * 0.5f, 0.01f);
            Gizmos.DrawSphere(Vector3.down * 0.6f, 0.01f);
            Gizmos.DrawSphere(Vector3.down * 0.7f, 0.01f);
            Gizmos.DrawSphere(Vector3.down * 0.8f, 0.01f);
            Gizmos.DrawSphere(Vector3.down * 0.9f, 0.01f);
            Gizmos.DrawSphere(Vector3.down, 0.01f);
        }

#endif
        #endregion
    }
}