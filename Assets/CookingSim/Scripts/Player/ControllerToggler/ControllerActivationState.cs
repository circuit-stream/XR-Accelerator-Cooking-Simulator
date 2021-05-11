using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
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

        [SerializeField]
        [Tooltip("If the action that requests the activation of this controller should be the uiPressAction.")]
        private bool useUIPressAction = false;

        [Header("References")]
        [SerializeField]
        [Tooltip("The controller to be activated.")]
        private ActionBasedController xrController;
        private XRBaseInteractor xrControllerInteractor;

        private bool isHovering;
        private bool isSelecting;

        public bool IsRequestingControl => (requestControlOnHover && isHovering) || IsPressingAction();
        public bool IsLockingControl => isSelecting;

        private InputAction ActivationAction => !useUIPressAction
            ? xrController.selectAction.action
            : xrController.uiPressAction.action;

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
            xrControllerInteractor.onHoverEntered.AddListener(OnHoverEnter);
            xrControllerInteractor.onHoverExited.AddListener(OnHoverExit);
            xrControllerInteractor.onSelectEntered.AddListener(OnSelectEnter);
            xrControllerInteractor.onSelectExited.AddListener(OnSelectExit);
        }

        private bool IsPressingAction()
        {
            return ActivationAction.triggered || ActivationAction.phase == InputActionPhase.Performed;
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

        private async void OnSelectExit(XRBaseInteractable interactable)
        {
            await Task.Delay(100);

            isSelecting = false;
        }

        #endregion
    }
}