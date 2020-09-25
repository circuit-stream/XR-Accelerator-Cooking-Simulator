using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRAccelerator.Gameplay
{
    public class GrabInteractableSocket : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The grabInteractable that occupies this socket")]
        private XRGrabInteractable grabInteractable;

        private bool isInsideTrigger;
        private bool isInteracting;

        private Vector3 initialPosition;
        private Quaternion initialRotation;
        private Vector3 initialScale;
        private Transform initialParent;
        private Transform interactableTransform;
        private Rigidbody interactableRigidbody;

        private void Attach()
        {
            interactableTransform.parent = initialParent;
            interactableTransform.localPosition = initialPosition;
            interactableTransform.localRotation = initialRotation;
            interactableTransform.localScale = initialScale;

            interactableRigidbody.isKinematic = true;
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
        }

        private void OnGrabRelease(XRBaseInteractor interactor)
        {
            isInteracting = false;

            if (isInsideTrigger)
            {
                Attach();
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

            interactableRigidbody.isKinematic = true;

            grabInteractable.onSelectEnter.AddListener(OnGrab);
            grabInteractable.onSelectExit.AddListener(OnGrabRelease);
        }
    }
}