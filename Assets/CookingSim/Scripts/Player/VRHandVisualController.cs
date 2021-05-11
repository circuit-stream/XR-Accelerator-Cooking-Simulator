using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using XRAccelerator.Gameplay;

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

        [SerializeField]
        [Tooltip("Axis action to control the main hand flex")]
        private InputActionProperty mainFlexAction;

        [SerializeField]
        [Tooltip("Axis action to control the secondary hand flex")]
        private InputActionProperty secondaryFlexAction;

        private XRBaseControllerInteractor xrControllerInteractor;
        private ActionBasedController xrController;

        private void Update()
        {
            SetAnimatorInputValue(mainFlexAction.action, ControllerSelectValueHash);
            SetAnimatorInputValue(secondaryFlexAction.action, ControllerActivateValueHash);
        }

        private void SetAnimatorInputValue(InputAction action, int animationHashName)
        {
            if (action != null)
                animator.SetFloat(animationHashName, action.ReadValue<float>());
        }

        private void OnEnable()
        {
            SetInteractionType(interactionType);
        }

        private void Start()
        {
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
                xrController = xrController != null ? xrController : currentTransform.GetComponent<ActionBasedController>();
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

            Transform attachTransform;
            if (xrControllerInteractor is XRRayInteractor)
            {
                var attachName = $"[{xrControllerInteractor.name}] Original Attach";
                attachTransform = xrControllerInteractor.transform.Find(attachName);
            }
            else
                attachTransform = xrControllerInteractor.attachTransform;

            attachTransform.parent = attachPoint.parent;
            attachTransform.localPosition = attachPoint.localPosition;
            attachTransform.localRotation = attachPoint.localRotation;
            attachTransform.name = attachPoint.name;
            Destroy(attachPoint.gameObject);
        }

        private void RegisterControllerEventCallbacks()
        {
            xrControllerInteractor.onHoverEntered.AddListener(OnXRControllerHoverEnter);
            xrControllerInteractor.onHoverExited.AddListener(OnXRControllerHoverExit);
            xrControllerInteractor.onSelectEntered.AddListener(OnXRControllerSelectEnter);
            xrControllerInteractor.onSelectExited.AddListener(OnXRControllerSelectExit);
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