using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using XRAccelerator.Enums;

namespace XRAccelerator.Player
{
    public class VRHandAnimationController : MonoBehaviour
    {
        #region constants

        private static readonly int LockedPoseHash = Animator.StringToHash("LockedPose");
        private static readonly int ControllerSelectValueHash = Animator.StringToHash("ControllerSelectValue");
        private static readonly int ControllerActivateValueHash = Animator.StringToHash("ControllerActivateValue");
        private static readonly int HoveringInteractableHash = Animator.StringToHash("HoveringInteractable");
        private static readonly int SelectingInteractableHash = Animator.StringToHash("SelectingInteractable");

        // TODO Arthur: Set ActivatingInteractableHash
        private static readonly int ActivatingInteractableHash = Animator.StringToHash("ActivatingInteractable");

        private static readonly Dictionary<InputHelpers.Button, InputFeatureUsage<float>> ButtonToFeatureParser =
            new Dictionary<InputHelpers.Button, InputFeatureUsage<float>>
            {
                {InputHelpers.Button.Trigger, CommonUsages.trigger},
                {InputHelpers.Button.Grip, CommonUsages.grip},
            };

        private static readonly Dictionary<VRControllerInteractionType, int> InteractionTypeToLayerIndex =
            new Dictionary<VRControllerInteractionType, int>
            {
                {VRControllerInteractionType.DirectContact, 1},
                {VRControllerInteractionType.Ray, 2}
            };

        #endregion

        private VRControllerInteractionType interactionType;

        [SerializeField]
        private XRBaseControllerInteractor xrControllerInteractor;
        [SerializeField]
        private XRController xrController;

        [SerializeField]
        private Animator animator;

        public void SetInteractionType(VRControllerInteractionType newInteractionType)
        {
            Debug.Assert(InteractionTypeToLayerIndex.ContainsKey(newInteractionType), "Using unsupported VRControllerInteractionType");

            interactionType = newInteractionType;

            foreach (var pair in InteractionTypeToLayerIndex)
            {
                animator.SetLayerWeight(pair.Value, pair.Key == interactionType ? 1 : 0);
            }
        }

        public void LockPose(HandPose handPose)
        {
            animator.SetInteger(LockedPoseHash, (int)handPose);
        }

        private void Update()
        {
            SetAnimatorInputValue(xrController.selectUsage, ControllerSelectValueHash);
            SetAnimatorInputValue(xrController.activateUsage, ControllerActivateValueHash);
        }

        private void SetAnimatorInputValue(InputHelpers.Button button, int animationHashName)
        {
            Debug.Assert(ButtonToFeatureParser.ContainsKey(button), "Using unsupported button for hand animation");

            var featureUsage = ButtonToFeatureParser[button];
            var gotValue = xrController.inputDevice.TryGetFeatureValue(featureUsage, out var pressValue);

            if (gotValue)
            {
                animator.SetFloat(animationHashName, pressValue);
            }
        }

        private void Awake()
        {
            SetInteractionType(interactionType);

            xrControllerInteractor.onHoverEnter.AddListener(OnXRControllerHoverEnter);
            xrControllerInteractor.onHoverExit.AddListener(OnXRControllerHoverExit);
            xrControllerInteractor.onSelectEnter.AddListener(OnXRControllerSelectEnter);
            xrControllerInteractor.onSelectExit.AddListener(OnXRControllerSelectExit);
        }

        #region XRController event callbacks

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