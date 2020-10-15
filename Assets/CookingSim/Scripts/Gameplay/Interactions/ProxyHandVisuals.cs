using UnityEngine;
using XRAccelerator.Enums;

namespace XRAccelerator.Gameplay
{
    public class ProxyHandVisuals : HandVisuals
    {
        [SerializeField]
        [Tooltip("The static animator pose the hand will use")]
        private HandPose defaultPose;

        public void Enable()
        {
            gameObject.SetActive(true);
            LockPose(defaultPose);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}