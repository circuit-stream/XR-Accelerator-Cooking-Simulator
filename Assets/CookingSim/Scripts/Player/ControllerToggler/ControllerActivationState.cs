using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRAccelerator.Player
{
    [Serializable]
    public class ControllerActivationState
    {
        [SerializeField]
        [Tooltip("If you only want to keep this state during it's activation")]
        public bool ReleasesControlWhenInactive;

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

        public bool IsRequestingControl => (requestControlOnHover && isHovering) || IsPressingAction();
        public bool IsLockingControl => isSelecting;

        private InputHelpers.Button ActionButton => xrController.selectUsage == InputHelpers.Button.None
            ? xrController.uiPressUsage
            : xrController.selectUsage;

        public void EnableController()
        {
            xrController.hideControllerModel = false;
            xrController.enableInputActions = true;
            xrControllerInteractor.allowSelect = true;
            xrControllerInteractor.allowHover = true;
        }

        public void DisableController()
        {
            xrController.hideControllerModel = true;
            xrController.enableInputActions = false;
            xrControllerInteractor.allowSelect = false;
            xrControllerInteractor.allowHover = requestControlOnHover;
        }

        public void Initialize()
        {
            xrControllerInteractor = xrController.GetComponent<XRBaseInteractor>();

            // These events don't work for UI Elements.
            xrControllerInteractor.onHoverEnter.AddListener(OnHoverEnter);
            xrControllerInteractor.onHoverExit.AddListener(OnHoverExit);
            xrControllerInteractor.onSelectEnter.AddListener(OnSelectEnter);
            xrControllerInteractor.onSelectExit.AddListener(OnSelectExit);
        }

        private bool IsPressingAction()
        {
            xrController.inputDevice.IsPressed(ActionButton, out var pressed,
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
            Debug.Log($"OnSelectEnter: {xrController}");
            isSelecting = true;
        }

        private async void OnSelectExit(XRBaseInteractable interactable)
        {
            Debug.Log($"OnSelectExit: {xrController}");
            await Task.Delay(100);

            isSelecting = false;
        }

        #endregion
    }
}