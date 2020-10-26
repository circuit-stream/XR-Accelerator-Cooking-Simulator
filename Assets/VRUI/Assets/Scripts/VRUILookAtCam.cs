using UnityEngine;

// VRUI Script For LookAt Camera
namespace XRAccelerator.VRUI
{
    [ExecuteAlways]
    public class VRUILookAtCam : MonoBehaviour
    {
        [SerializeField] 
        private Transform cam;
        private void Update()
        {
            if (!cam)
            {
                return;
                
            }
            transform.LookAt(cam);
            transform.Rotate(new Vector3(0, 180, 0));

        }
    }
}
