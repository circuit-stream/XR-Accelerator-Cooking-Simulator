using System.Collections.Generic;
using UnityEngine;
using XRAccelerator.Enums;

namespace XRAccelerator.Gameplay
{
    public class HandVisuals : MonoBehaviour
    {
        #region constants

        protected static readonly int LockedPoseHash = Animator.StringToHash("LockedPose");
        protected static readonly int ControllerSelectValueHash = Animator.StringToHash("ControllerSelectValue");
        protected static readonly int ControllerActivateValueHash = Animator.StringToHash("ControllerActivateValue");
        protected static readonly int HoveringInteractableHash = Animator.StringToHash("HoveringInteractable");
        protected static readonly int SelectingInteractableHash = Animator.StringToHash("SelectingInteractable");

        // TODO Arthur: Set ActivatingInteractableHash
        protected static readonly int ActivatingInteractableHash = Animator.StringToHash("ActivatingInteractable");

        private static readonly Dictionary<VRControllerInteractionType, int> InteractionTypeToLayerIndex =
            new Dictionary<VRControllerInteractionType, int>
            {
                {VRControllerInteractionType.DirectContact, 1},
                {VRControllerInteractionType.Ray, 2}
            };

        #endregion

        [SerializeField]
        [Tooltip("What kind of interaction this hand performs? This sets which animator layer we use.")]
        protected VRControllerInteractionType interactionType;
        [SerializeField]
        [Tooltip("The hand animator")]
        protected Animator animator;

        public void LockPose(HandPose handPose)
        {
            animator.SetInteger(LockedPoseHash, (int)handPose);
        }

        public void UnlockPose()
        {
            LockPose(HandPose.NoPose);
        }

        protected void SetInteractionType(VRControllerInteractionType newInteractionType)
        {
            Debug.Assert(InteractionTypeToLayerIndex.ContainsKey(newInteractionType), "Using unsupported VRControllerInteractionType");

            interactionType = newInteractionType;

            foreach (var pair in InteractionTypeToLayerIndex)
            {
                animator.SetLayerWeight(pair.Value, pair.Key == interactionType ? 1 : 0);
            }
        }
    }
}