using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using XRAccelerator.Enums;

namespace XRAccelerator.Gameplay
{
    public class InteractionPose : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The hand pose this object should be interacted with")]
        private HandPose interactionPose;

        private void OnGrab(XRBaseInteractor interactor)
        {
            interactor.GetComponentInChildren<HandVisuals>().LockPose(interactionPose);
        }

        private void OnGrabRelease(XRBaseInteractor interactor)
        {
            interactor.GetComponentInChildren<HandVisuals>().UnlockPose();
        }

        private void Start()
        {
            var component = GetComponent<XRBaseInteractable>();
            component.onSelectEntered.AddListener(OnGrab);
            component.onSelectExited.AddListener(OnGrabRelease);
        }
    }
}