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
        private Collider connectedCollider;
        
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
            connectedCollider = connectedBody.GetComponent<Collider>();
        }
        
        private void OnJointBreak(float _)
        {
            Debug.Log("A joint has just been broken!, force: " + breakForce);
            
            Destroy(grabbableObject.GetComponent<XRGrabInteractable>());
            connectedCollider.enabled = false;
            Invoke(nameof(ReenableGrab), 0.1f);
        }

        private void ReenableGrab()
        {
            connectedTransform.parent = transform.parent;
            connectedTransform.localPosition = transform.localPosition;
            connectedTransform.localRotation = transform.localRotation;
            
            var fixedJoint = gameObject.AddComponent<FixedJoint>();
            fixedJoint.connectedBody = connectedBody;
            fixedJoint.breakForce = breakForce;
            fixedJoint.enablePreprocessing = enablePreprocessing;
            
            grabbableObject.AddComponent<XRGrabInteractable>();
            var grabInteractable = grabbableObject.GetComponent<XRGrabInteractable>();
            grabInteractable.throwOnDetach = false;
            
            connectedCollider.enabled = true;
        }
    }
}