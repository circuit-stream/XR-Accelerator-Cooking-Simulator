using UnityEngine;
using XRAccelerator.Enums;

namespace XRAccelerator.Gameplay
{
    public class ProxyHandVisuals : HandVisuals
    {
        [SerializeField]
        [Tooltip("The static animator pose the hand will use")]
        private HandPose defaultPose;
        [SerializeField]
        [Tooltip("A transform that will rotate to better match the actual controller position")]
        private Transform rotatableTransform;
        [SerializeField]
        [Tooltip("If the rotation of the rotatableTransform should be limited.")]
        private bool limitRotation;
        [SerializeField]
        [Tooltip("max X rotation that the rotatableTransform can reach")]
        private float maxXRotation;
        [SerializeField]
        [Tooltip("min X rotation that the rotatableTransform can reach")]
        private float minXRotation;

        private Transform trackedController;

        public void Enable(Transform trackedController)
        {
            this.trackedController = trackedController;
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            trackedController = null;
            gameObject.SetActive(false);
        }

        private void ClampRotation()
        {
            var rotation = rotatableTransform.localRotation.eulerAngles;
            var xRotation = limitRotation
                ? Mathf.Clamp(rotation.x, minXRotation, maxXRotation)
                : rotation.x;
            rotatableTransform.localRotation = Quaternion.Euler(xRotation, 0, 0);
        }

        private void Update()
        {
            if (rotatableTransform == null)
            {
                return;
            }

            rotatableTransform.rotation = trackedController.rotation;
            ClampRotation();
        }

        private void Awake()
        {
            LockPose(defaultPose);
        }
    }
}