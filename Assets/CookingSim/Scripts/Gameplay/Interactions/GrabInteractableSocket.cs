using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRAccelerator.Gameplay
{
    public class GrabInteractableSocket : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The grabInteractable that occupies this socket")]
        private XRGrabInteractable grabInteractable;

        [SerializeField]
        [Tooltip("List of all interactable non trigger colliders")]
        public Collider[] interactableColliders;

        public Action OnAttach;

        private bool isInsideTrigger;
        private bool isInteracting;

        private Vector3 initialPosition;
        private Quaternion initialRotation;
        private Vector3 initialScale;
        private Transform initialParent;
        private Transform interactableTransform;
        private Rigidbody interactableRigidbody;

        public bool IsInteractableAttached { get; private set; } = true;

        public void IgnoreCollisionWith(Collider[] ignoreColliders)
        {
            foreach (var interactableCollider in interactableColliders)
            {
                foreach (var ignoreCollider in ignoreColliders)
                {
                    Physics.IgnoreCollision(interactableCollider, ignoreCollider);
                }
            }
        }

        private void Attach()
        {
            OnAttach?.Invoke();

            interactableTransform.parent = initialParent;
            ResetInteractableTransform();

            IsInteractableAttached = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponentInParent<XRGrabInteractable>() != grabInteractable)
            {
                return;
            }

            isInsideTrigger = true;

            if (!isInteracting)
            {
                Attach();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponentInParent<XRGrabInteractable>() != grabInteractable)
            {
                return;
            }

            isInsideTrigger = false;
        }

        private void OnGrab(XRBaseInteractor interactor)
        {
            isInteracting = true;
            IsInteractableAttached = false;
        }

        private void OnGrabRelease(XRBaseInteractor interactor)
        {
            isInteracting = false;

            if (isInsideTrigger)
            {
                Attach();
            }
        }

        private void ResetInteractableTransform()
        {
            interactableTransform.localRotation = initialRotation;
            interactableTransform.localPosition = initialPosition;
            interactableRigidbody.position = interactableTransform.position;
            interactableRigidbody.rotation = interactableTransform.rotation;
        }

        private void Update()
        {
            if (IsInteractableAttached)
            {
                ResetInteractableTransform();
            }
        }

        private void Awake()
        {
            interactableTransform = grabInteractable.transform;
            interactableRigidbody = grabInteractable.GetComponent<Rigidbody>();

            initialPosition = interactableTransform.localPosition;
            initialRotation = interactableTransform.localRotation;
            initialScale = interactableTransform.localScale;
            initialParent = interactableTransform.parent;

            grabInteractable.onSelectEntered.AddListener(OnGrab);
            grabInteractable.onSelectExited.AddListener(OnGrabRelease);
        }
    }
}