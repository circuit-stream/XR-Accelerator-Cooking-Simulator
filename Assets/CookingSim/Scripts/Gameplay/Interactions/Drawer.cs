using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRAccelerator.Gameplay
{
    public class Drawer : XRSimpleInteractable
    {
        [SerializeField] [Tooltip("The index of the state to start on (1=start)")]
        private float pullOutDistance = 0.5f;

        [SerializeField] [Tooltip("The initial position of the tray")]
        private Transform initialDrawerPosition;

        [SerializeField] [Tooltip("The transform that is going to be moved while interacting")]
        private Transform movableTransform;

        [SerializeField] [Tooltip("Based off of the orientation of the \"initialPosition\" transform's orientation")]
        private Vector3 localPulloutDirection;

        private Vector3 initialHandOffset;

        public FloatEvent PercentOpenOnRelease;
        public UnityEvent OnOpen;
        public UnityEvent OnClose;
        public UnityEvent OnStartInteracting;
        public UnityEvent OnEndInteracting;

        public float PercentOpen { get; private set; }
        public bool IsFullyOpen { get; private set; }
        public bool IsFullyClosed { get; private set; }

        private Transform currentControllerTransform;
        private bool IsInteracting => currentControllerTransform != null;


        [Serializable]
        public class FloatEvent : UnityEvent<float>
        {
        }


        protected override void Awake()
        {
            if (initialDrawerPosition == null)
            {
                Debug.LogError("Please Set the initial drawer position transform", this.gameObject);
            }

            if (movableTransform == null)
            {
                Debug.LogError("Please Set the initial movable drawer position transform", this.gameObject);
            }

            if (localPulloutDirection == Vector3.zero)
            {
                Debug.LogError("Please set a normal direction for the Drawer pullout", this.gameObject);
            }


            onSelectEnter.AddListener(OnBeginInteraction);
            onSelectExit.AddListener(OnEndInteraction);

            base.Awake();
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


        private void Interacting()
        {
            Vector3 handPosition = currentControllerTransform.position - initialHandOffset;


            Vector3 drawerDirectionWorldNormal =
                initialDrawerPosition.TransformDirection(localPulloutDirection.normalized);


            Vector3 positionOnDrawerNormal = Vector3.Project(handPosition - initialDrawerPosition.position,
                drawerDirectionWorldNormal);


            if (Vector3.Dot(positionOnDrawerNormal, drawerDirectionWorldNormal) > 0)
            {
                SetOpenPercent(positionOnDrawerNormal.magnitude / pullOutDistance);
            }
            else
            {
                SetOpenPercent(0f);
            }
        }

        public void SetOpenPercent(float percent)
        {
            percent = Mathf.Clamp01(percent);
            Vector3 drawerDirectionWorldNormal =
                initialDrawerPosition.TransformDirection(localPulloutDirection.normalized);

            Vector3 clampedPosition = initialDrawerPosition.position +
                                      (drawerDirectionWorldNormal * (pullOutDistance * percent));
            movableTransform.position = clampedPosition;

            PercentOpen = percent;

            if (PercentOpen == 1f && !IsFullyOpen)
            {
                IsFullyOpen = true;
                IsFullyClosed = false;
                OnOpen.Invoke();
            }
            else if (PercentOpen == 0f && !IsFullyClosed)
            {
                IsFullyClosed = true;
                IsFullyOpen = false;
                OnClose.Invoke();
            }
            else
            {
                IsFullyClosed = false;
                IsFullyOpen = false;
            }
        }

        public void NotInteracting()
        {
        }

        public void OnBeginInteraction(XRBaseInteractor interactor)
        {
            currentControllerTransform = interactor.transform;

            initialHandOffset = currentControllerTransform.position -
                                (initialDrawerPosition.position - transform.localPosition);
            OnStartInteracting?.Invoke();
        }

        public void OnEndInteraction(XRBaseInteractor interactor)
        {
            currentControllerTransform = null;
            PercentOpenOnRelease.Invoke(PercentOpen);
            OnEndInteracting?.Invoke();
        }
    }
}