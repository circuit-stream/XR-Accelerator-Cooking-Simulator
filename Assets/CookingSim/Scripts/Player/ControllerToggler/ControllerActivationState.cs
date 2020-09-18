using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRAccelerator.Player
{
    [Serializable]
    public class ControllerActivationState
    {
        [SerializeField]
        [Tooltip("If the hover event requests the activation of this controller.")]
        private bool requestControlOnHover = true;

        [Header("References")]
        [SerializeField]
        [Tooltip("The controller to be activated.")]
        private XRController xrController;
        private XRBaseInteractor xrControllerInteractor;

        private bool isHovering;
        private bool isSelecting;

        public bool IsRequestingControl => (requestControlOnHover && isHovering) || IsPressingSelect();
        public bool IsLockingControl => isSelecting;

        public void EnableController()
        {
            xrController.hideControllerModel = false;
            xrController.enableInputActions = true;
            xrControllerInteractor.enableInteractions = true;
        }

        public void DisableController()
        {
            xrController.hideControllerModel = true;
            xrController.enableInputActions = false;
            xrControllerInteractor.enableInteractions = false;
        }

        public void Initialize()
        {
            xrControllerInteractor = xrController.GetComponent<XRBaseInteractor>();
            xrControllerInteractor.onHoverEnter.AddListener(OnHoverEnter);
            xrControllerInteractor.onHoverExit.AddListener(OnHoverExit);
            xrControllerInteractor.onSelectEnter.AddListener(OnSelectEnter);
            xrControllerInteractor.onSelectExit.AddListener(OnSelectExit);
        }

        private bool IsPressingSelect()
        {
            xrController.inputDevice.IsPressed(xrController.selectUsage, out var pressed,
                xrController.axisToPressThreshold);
            return pressed;
        }

        #region XRControllerInteractor callbacks

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

        #endregion
    }
}