using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using XRAccelerator.Enums;

namespace XRAccelerator.Player
{
    public class VRHandVisualController : HandVisuals
    {
        [SerializeField]
        [Tooltip("A reference to a XRInteractorLineVisual component that is inside the handModel, if it exists.")]
        private XRInteractorLineVisual interactorLineVisual;
        [SerializeField]
        [Tooltip("An optional attachPoint to replace the XRControllerInteractor when the model is instantiated.")]
        private Transform attachPoint;

        private XRBaseControllerInteractor xrControllerInteractor;
        private XRController xrController;

        private void Update()
        {
            SetAnimatorInputValue(xrController.selectUsage, ControllerSelectValueHash);
            SetAnimatorInputValue(xrController.activateUsage, ControllerActivateValueHash);
        }

        private void SetAnimatorInputValue(InputHelpers.Button button, int animationHashName)
        {
            animator.SetFloat(animationHashName, GetButtonPressValue(button));
        }

        private float GetButtonPressValue(InputHelpers.Button button)
        {
            if (button == InputHelpers.Button.None)
            {
                return 0;
            }

            var gotValue = InputDeviceUtils.GetPressValue(xrController.inputDevice, button, out var pressValue);
            return gotValue ? pressValue : 0;
        }

        private void Start()
        {
            SetInteractionType(interactionType);
            SetupXRController();
            SetupInteractorLineVisual();
        }

        // [XRToolkitWorkaround] Rerouting XRBaseInteractable <> XRInteractorLineVisual communication
        // This is required so we can have the XRInteractorLineVisual anywhere in the hierarchy
        private void SetupInteractorLineVisual()
        {
            if (interactorLineVisual == null)
            {
                return;
            }

            var flags = System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance;
            var field = interactorLineVisual.GetType().GetField("m_LineRenderable", flags);
            field.SetValue(interactorLineVisual, xrControllerInteractor);

            xrControllerInteractor.GetComponent<XRCustomReticleProviderProxy>().target = interactorLineVisual;
        }

        #region XRController

        // Finds xrController and xrControllerInteractor component in parents
        // then register some event callbacks
        private void SetupXRController()
        {
            FindControllerComponents();
            MoveControllerAttachPoint();
            RegisterControllerEventCallbacks();
        }

        private void FindControllerComponents()
        {
            var currentTransform = transform.parent;

            while (xrController == null || xrControllerInteractor == null)
            {
                xrController = xrController != null ? xrController : currentTransform.GetComponent<XRController>();
                xrControllerInteractor = xrControllerInteractor != null ? xrControllerInteractor : currentTransform.GetComponent<XRBaseControllerInteractor>();

                currentTransform = currentTransform.parent;

                if (currentTransform == null)
                {
                    break;
                }
            }

            Debug.Assert(xrController != null && xrControllerInteractor != null, "Missing xrController or xrControllerInteractor");
        }

        // [XRToolkitWorkaround] Hack so we can use an attachPoint inside newly instantiated model
        private void MoveControllerAttachPoint()
        {
            if (attachPoint == null)
            {
                return;
            }

            var attachTransform = xrControllerInteractor.attachTransform;
            attachTransform.parent = attachPoint.parent;
            attachTransform.localPosition = attachPoint.localPosition;
            attachTransform.localRotation = attachPoint.localRotation;
            attachTransform.name = attachPoint.name;
            Destroy(attachPoint.gameObject);
        }

        private void RegisterControllerEventCallbacks()
        {
            xrControllerInteractor.onHoverEnter.AddListener(OnXRControllerHoverEnter);
            xrControllerInteractor.onHoverExit.AddListener(OnXRControllerHoverExit);
            xrControllerInteractor.onSelectEnter.AddListener(OnXRControllerSelectEnter);
            xrControllerInteractor.onSelectExit.AddListener(OnXRControllerSelectExit);
        }

        private void OnXRControllerHoverEnter(XRBaseInteractable interactable)
        {
            animator.SetBool(HoveringInteractableHash, true);
        }

        private void OnXRControllerHoverExit(XRBaseInteractable interactable)
        {
            animator.SetBool(HoveringInteractableHash, false);
        }

        private void OnXRControllerSelectEnter(XRBaseInteractable interactable)
        {
            animator.SetBool(SelectingInteractableHash, true);
        }

        private void OnXRControllerSelectExit(XRBaseInteractable interactable)
        {
            animator.SetBool(SelectingInteractableHash, false);
        }

        #endregion
    }
}