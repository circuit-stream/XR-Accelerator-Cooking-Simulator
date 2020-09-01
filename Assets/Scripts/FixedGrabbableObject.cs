using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRAccelerator
{
    public class FixedGrabbableObject : MonoBehaviour
    {
        [SerializeField]
        private GameObject grabbableObject;
            
        private Rigidbody connectedBody;
        private Transform connectedTransform;
        private bool enablePreprocessing;
        private float breakForce;

        private XRGrabInteractable xrGrabInteractable;
        
        private void Awake()
        {
            var fixedJoint = GetComponent<FixedJoint>();
            connectedBody = fixedJoint.connectedBody;
            enablePreprocessing = fixedJoint.enablePreprocessing;
            breakForce = fixedJoint.breakForce;

            connectedTransform = connectedBody.transform;
        }
        
        private void OnJointBreak(float _)
        {
            Debug.Log("A joint has just been broken!, force: " + breakForce);
            
            Destroy(grabbableObject.GetComponent<XRGrabInteractable>());
            connectedTransform.parent = transform;
            connectedTransform.localPosition = Vector3.zero;
            connectedTransform.localScale = Vector3.one;
            Invoke(nameof(ReenableGrab), 0.1f);
        }

        private void ReenableGrab()
        {
            var fixedJoint = gameObject.AddComponent<FixedJoint>();
            fixedJoint.connectedBody = connectedBody;
            fixedJoint.breakForce = breakForce;
            fixedJoint.enablePreprocessing = enablePreprocessing;
            
            grabbableObject.AddComponent<XRGrabInteractable>();
            var grabInteractable = grabbableObject.GetComponent<XRGrabInteractable>();
            grabInteractable.throwOnDetach = false;
        }
    }
}