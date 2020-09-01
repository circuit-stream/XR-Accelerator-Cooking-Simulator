using UnityEngine;

namespace XRAccelerator
{
    public class FixedGrabbableObject : MonoBehaviour
    {
        private Rigidbody connectedBody;
        private bool enablePreprocessing;
        private float breakForce;
        
        private void Awake()
        {
            var fixedJoint = GetComponent<FixedJoint>();
            connectedBody = fixedJoint.connectedBody;
            enablePreprocessing = fixedJoint.enablePreprocessing;
            breakForce = fixedJoint.breakForce;
        }
        
        private void OnJointBreak(float _)
        {
            Debug.Log("A joint has just been broken!, force: " + breakForce);
            connectedBody.position = transform.position;
            
            // TODO: release XR Interaction grab
            // Invoke(nameof(ReenableGrab), 1);
        }

        private void ReenableGrab()
        {
            var fixedJoint = gameObject.AddComponent<FixedJoint>();
            fixedJoint.connectedBody = connectedBody;
            fixedJoint.breakForce = breakForce;
            fixedJoint.enablePreprocessing = enablePreprocessing;
        }
    }
}