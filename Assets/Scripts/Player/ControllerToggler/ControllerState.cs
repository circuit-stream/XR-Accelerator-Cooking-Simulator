using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRAccelerator.Player
{
    [Serializable]
    public class ControllerState
    {
        [SerializeField]
        private bool requestControlOnHover = true;

        [Header("References")]
        [SerializeField]
        private XRController xrController;

        private XRBaseInteractor xrControllerInteractor;

        public bool IsRequestingControl => (requestControlOnHover && isHovering) || IsPressingSelect();
        public bool IsLockingControl => isSelecting;

        private bool isHovering;
        private bool isSelecting;

        public void EnterState()
        {
            xrController.hideControllerModel = false;
            xrController.enableInputActions = true;
        }

        public void ExitState()
        {
            xrController.hideControllerModel = true;
            xrController.enableInputActions = false;
        }

        private bool IsPressingSelect()
        {
            xrController.inputDevice.IsPressed(xrController.selectUsage, out var pressed,
                xrController.axisToPressThreshold);
            return pressed;
        }

        private void OnHoverEnter(XRBaseInteractable interactable)
        {
            isHovering = true;
        }

        private void OnHoverExit(XRBaseInteractable interactable)
        {
            isHovering = false;
        }

        private void OnSelectEnter(XRBaseInteractable interactable)
        {
            isSelecting = true;
        }

        private void OnSelectExit(XRBaseInteractable interactable)
        {
            isSelecting = false;
        }

        public void Initialize()
        {
            xrControllerInteractor = xrController.GetComponent<XRBaseInteractor>();
            xrControllerInteractor.onHoverEnter.AddListener(OnHoverEnter);
            xrControllerInteractor.onHoverExit.AddListener(OnHoverExit);
            xrControllerInteractor.onSelectEnter.AddListener(OnSelectEnter);
            xrControllerInteractor.onSelectExit.AddListener(OnSelectExit);
        }
    }
}