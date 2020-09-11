using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRAccelerator
{
    public class FixedGrabbableObject : MonoBehaviour
    {
        [SerializeField]
        private GameObject grabbableObject;
        [SerializeField]
        private float breakForce;
        [SerializeField]
        private float breakTorque;
        
        private Rigidbody connectedBody;
        private Collider connectedCollider;
        private Transform grabbableTransform;
        private Transform grabbableTransformParent;
        private Vector3 grabbablePosition;
        private Quaternion grabbableRotation;
        

        private XRGrabInteractable xrGrabInteractable;
        private float smoothRotationAmount;
        private float tightenRotation;
        private XRBaseInteractable.MovementType movementType;
        
        
        private void Awake()
        {
            connectedBody = grabbableObject.GetComponent<Rigidbody>();
            connectedCollider = connectedBody.GetComponent<Collider>();
            grabbableTransform = grabbableObject.transform;
            grabbableTransformParent = grabbableTransform.parent;
            grabbablePosition = grabbableTransform.localPosition;
            grabbableRotation = grabbableTransform.localRotation;

            var grabInteractable = grabbableObject.GetComponent<XRGrabInteractable>();
            smoothRotationAmount = grabInteractable.smoothRotationAmount;
            tightenRotation = grabInteractable.tightenRotation;
            movementType = grabInteractable.movementType;
            grabInteractable.onSelectEnter.AddListener(OnGrab);
        }
        
        private void OnJointBreak(float _)
        {
            Debug.Log("A joint has just been broken!, force: " + breakForce);
            
            Destroy(grabbableObject.GetComponent<XRGrabInteractable>());
            connectedCollider.enabled = false;
            
            Invoke(nameof(ReenableGrab), 0.1f);
        }

        private void OnGrab(XRBaseInteractor interactor)
        {
            CreateJoint();
        }

        private void ReenableGrab()
        {
            grabbableTransform.parent = grabbableTransformParent;
            grabbableTransform.localPosition = grabbablePosition;
            grabbableTransform.localRotation = grabbableRotation;
            connectedBody.velocity = Vector3.zero;
            connectedBody.angularVelocity = Vector3.zero;
            
            CreateXRGrabInteractableComponent();
            connectedCollider.enabled = true;
        }

        private void CreateXRGrabInteractableComponent()
        {
            grabbableObject.AddComponent<XRGrabInteractable>();
            var grabInteractable = grabbableObject.GetComponent<XRGrabInteractable>();
            
            grabInteractable.throwOnDetach = false;
            grabInteractable.smoothRotation = true;
            grabInteractable.tightenRotation = tightenRotation;
            grabInteractable.smoothRotationAmount = smoothRotationAmount;
            grabInteractable.movementType = movementType;

            grabInteractable.onSelectEnter.AddListener(OnGrab);
        }
        
        private void CreateJoint()
        {
            var fixedJoint = gameObject.AddComponent<FixedJoint>();
            fixedJoint.connectedBody = connectedBody;
            fixedJoint.breakForce = breakForce;
            fixedJoint.breakTorque = breakTorque;
            fixedJoint.enablePreprocessing = true;
        }
    }
}