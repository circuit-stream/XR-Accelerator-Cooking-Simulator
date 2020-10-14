using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRAccelerator.Gameplay
{
    [RequireComponent(typeof(Collider))]
    public class Lever : XRSimpleInteractable
    {
        private Transform currentControllerTransform;
        private bool IsInteracting => currentControllerTransform != null;

        [SerializeField] 
        [Tooltip("The Transform that is the pivot of the lever.")]
        private Transform leverPivot;

        [SerializeField] 
        [Tooltip("The rotating part of the lever.")]
        private Transform movableTransform;

        private enum Axis
        {
            X,
            Y,
            Z
        }

        [SerializeField]
        [Tooltip("Based off of the orientation of the local orientation of the \"LeverPivot\"")]
        private Axis pivotLocalRotationAxis = Axis.X;

        [SerializeField] 
        [Tooltip("Based off of the orientation of the local orientation of the \"LeverPivot\"")]
        private bool invertAxis = false;

        [SerializeField] 
        [Tooltip("The Deactivated Position of the lever")]
        private Vector3 localStartDirection;

        [SerializeField] 
        [Tooltip("The Activated Position of the lever")]
        private Vector3 localEndDirection;

        [SerializeField] 
        [Tooltip("If no limits, the lever can spin in circles")]
        private bool hasLimits = true;

        [SerializeField]
        [Tooltip("Reverse the rotation direction between the start and end directions")]
        private bool reverseRotation = false;

        [SerializeField] 
        [Tooltip("Whether the lever snaps to start or end position")]
        private bool snapOnRelease = false;

        [SerializeField] 
        [Tooltip("At what point the lever snaps")]
        private float snapToPercent = 0.5f;

        public UnityEvent onActivated;
        public UnityEvent onDeactivated;

        [Serializable]
        public class FloatEvent : UnityEvent<float>
        {
        }

        public FloatEvent LeverChange;
        public UnityEvent StartInteracting;
        public UnityEvent EndInteracting;

        private float localMaxAngle = 90f;
        private Vector3 initialHandOffset;

        public float PercentOpen { get; private set; }
        public bool IsActivated { get; private set; }

        public bool IsAtMax => Mathf.Approximately(PercentOpen, 1f);

        public bool IsAtMin => Mathf.Approximately(PercentOpen, 0f);


        Vector3 localRotationAxis
        {
            get
            {
                switch (pivotLocalRotationAxis)
                {
                    case Axis.X:
                        return invertAxis ? -Vector3.right : Vector3.right;
                    case Axis.Y:
                        return invertAxis ? -Vector3.up : Vector3.up;
                    default:
                        return invertAxis ? -Vector3.forward : Vector3.forward;
                }
            }
        }

        private Quaternion startRotation;

        protected override void Awake()
        {
            if (leverPivot == null)
            {
                Debug.LogError("Please Set the initial drawer position transform", this.gameObject);
            }

            if (movableTransform == null)
            {
                Debug.LogError("Please Set the initial movable drawer position transform", this.gameObject);
            }

            if (localRotationAxis == Vector3.zero)
            {
                Debug.LogError("Please set a rotational axis", this.gameObject);
            }

            Initialize();
            
            onSelectEnter.AddListener(OnBeginInteraction);
            onSelectExit.AddListener(OnEndInteraction);
            base.Awake();
        }

        private void Initialize()
        {
            if (hasLimits)
            {
                localMaxAngle = Vector3.Angle(localStartDirection, localEndDirection);
                if (reverseRotation)
                {
                    localMaxAngle = -localMaxAngle;
                }
            }

            startRotation = movableTransform.localRotation;
        }

        public void Interacting()
        {
            Vector3 leverDirection = ConvertWorldPointToAxisDirection(currentControllerTransform.transform.position);

            Vector3 startDirection = leverPivot.TransformDirection(localStartDirection);

            Vector3 perendicularAxisToDetermineRoationAngle = Vector3.Cross(startDirection, leverDirection);

            float currentAngle = Vector3.Angle(startDirection, leverDirection);
            if (Vector3.Dot(perendicularAxisToDetermineRoationAngle, leverPivot.TransformDirection(localRotationAxis)) <
                0)
            {
                //TODO: fix this for any limits > 180
                currentAngle = -currentAngle;
            }

            if (hasLimits)
            {
                if (localMaxAngle > 0)
                {
                    SetOpenPercent(Mathf.Clamp(currentAngle, 0, localMaxAngle) / localMaxAngle);
                }
                else
                {
                    SetOpenPercent(Mathf.Clamp(currentAngle, localMaxAngle, 0) / localMaxAngle);
                }
            }
            else
            {
                SetOpenPercent(Mathf.Abs(Mathf.Clamp(currentAngle, -360, 360) / 360));
            }
        }

        private Vector3 ConvertWorldPointToAxisDirection(Vector3 point)
        {
            Vector3 pivotAxis = leverPivot.TransformDirection(localRotationAxis.normalized);
            return Vector3.ProjectOnPlane((point) - leverPivot.position, pivotAxis).normalized;
        }

        private void SetOpenPercent(float percent)
        {
            PercentOpen = Mathf.Clamp01(percent);
            Vector3 pivotAxis = leverPivot.TransformDirection(localRotationAxis.normalized);
            movableTransform.localRotation = startRotation;
            movableTransform.RotateAround(leverPivot.transform.position, pivotAxis, PercentOpen * localMaxAngle);
            LeverChange.Invoke(PercentOpen);

            if (IsAtMax && !IsActivated)
            {
                IsActivated = true;
                onActivated.Invoke();
            }
            else if (IsAtMin && IsActivated)
            {
                IsActivated = false;
                onDeactivated.Invoke();
            }
        }

        public void NotInteracting()
        {
        }

        private void OnBeginInteraction(XRBaseInteractor interactor)
        {
            currentControllerTransform = interactor.transform;
            StartInteracting?.Invoke();
        }

        private void OnEndInteraction(XRBaseInteractor interactor)
        {
            if (snapOnRelease)
            {
                if (!IsAtMin && !IsAtMax)
                {
                    if (PercentOpen - snapToPercent <= 0f)
                    {
                        SetOpenPercent(0f);
                    }
                    else if (PercentOpen + snapToPercent >= 1f)
                    {
                        SetOpenPercent(1f);
                    }
                }
            }

            EndInteracting?.Invoke();
            currentControllerTransform = null;
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
    }
}