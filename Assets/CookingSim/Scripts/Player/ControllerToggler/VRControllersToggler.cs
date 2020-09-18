using System.Collections;
using UnityEngine;

namespace XRAccelerator.Player
{
    public class VRControllersToggler : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Right XRControllers toggle configurations.")]
        private VRControllerToggler rightController;
        [SerializeField]
        [Tooltip("Left XRControllers toggle configurations.")]
        private VRControllerToggler leftController;

        private void Update()
        {
            leftController.Update();
            rightController.Update();
        }

        private IEnumerator Initialize()
        {
            // Model setup is done by XRController in the Update method during the first frame
            yield return null;

            leftController.Initialize();
            rightController.Initialize();
        }

        private void Start()
        {
            StartCoroutine(Initialize());
        }
    }
}