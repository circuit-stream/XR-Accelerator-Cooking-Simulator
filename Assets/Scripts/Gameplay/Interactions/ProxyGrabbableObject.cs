using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRAccelerator
{
    public class ProxyGrabbableObject : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("How much force should break the FixedJoint thisObject <> grabbableObject")]
        private float breakForce;
        [SerializeField]
        [Tooltip("How much torque should break the FixedJoint thisObject <> grabbableObject")]
        private float breakTorque;

        [Header("Prefab References")]
        [SerializeField]
        [Tooltip("The actual object that will be grabbed by the XRInteractor")]
        private GameObject grabbableObject;
        [SerializeField]
        [Tooltip("A reference to a ProxyHandVisual monobehavior component.\nSet this if you want to display a geometric matched hand when the proxy is grabbed")]
        private ProxyHandVisuals proxyHandVisuals;

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

        private XRBaseInteractor currentInteractor;
        private XRController currentController;

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
            grabInteractable.onSelectExit.AddListener(OnGrabRelease);
        }

        private void Start()
        {
            StartCoroutine(ResetGrabbableTransform());
        }

        private void OnJointBreak(float _)
        {
            DestroyXRGrabInteractableComponent();

            connectedCollider.enabled = false;

            StartCoroutine(ReenableGrab());
        }

        private void EnableProxyHandVisual()
        {
            if (proxyHandVisuals == null)
            {
                return;
            }

            proxyHandVisuals.Enable(currentInteractor.transform);
            currentController.hideControllerModel = true;
        }

        private void DisableProxyHandVisual()
        {
            if (proxyHandVisuals == null)
            {
                return;
            }

            proxyHandVisuals.Disable();
            currentController.hideControllerModel = false;
        }

        private void OnGrab(XRBaseInteractor interactor)
        {
            currentInteractor = interactor;
            currentController = currentInteractor.GetComponent<XRController>();

            CreateJoint();
            EnableProxyHandVisual();
        }

        private void OnGrabRelease(XRBaseInteractor interactor)
        {
            Debug.Log("OnGrabRelease");
            DestroyJoint();
            DisableProxyHandVisual();

            currentInteractor = null;
            currentController = null;
        }

        private IEnumerator ReenableGrab()
        {
            yield return ResetGrabbableTransform();

            ResetGrabbableTransform();
            CreateXRGrabInteractableComponent();
            connectedCollider.enabled = true;
        }

        private IEnumerator ResetGrabbableTransform()
        {
            // Reset position after joint forces are resolved
            yield return new WaitForSeconds(0.1f);

            grabbableTransform.parent = grabbableTransformParent;
            grabbableTransform.localPosition = grabbablePosition;
            grabbableTransform.localRotation = grabbableRotation;
            connectedBody.velocity = Vector3.zero;
            connectedBody.angularVelocity = Vector3.zero;
        }

        private void CreateJoint()
        {
            var fixedJoint = gameObject.AddComponent<FixedJoint>();
            fixedJoint.connectedBody = connectedBody;
            fixedJoint.breakForce = breakForce;
            fixedJoint.breakTorque = breakTorque;
            fixedJoint.enablePreprocessing = true;
        }

        private void DestroyJoint()
        {
            var joint = GetComponent<FixedJoint>();
            if (joint != null)
            {
                Destroy(joint);
            }
        }

        // [XRToolkitWorkaround] These functions only exist because XRToolkit provides no way to forcefully deselect a interactable
        #region XRToolkit workaround

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
            grabInteractable.onSelectExit.AddListener(OnGrabRelease);
        }

        private void DestroyXRGrabInteractableComponent()
        {
            var interactable = grabbableObject.GetComponent<XRGrabInteractable>();
            var interactor = currentInteractor;

            // calling these events like this leaves some of the XRToolkit internal logic out, but so far I got no errors
            // an alternative would be using reflection here.
            currentInteractor.onSelectExit?.Invoke(interactable);
            currentInteractor.onHoverExit?.Invoke(interactable);
            interactable.onSelectExit?.Invoke(interactor);
            interactable.onHoverExit?.Invoke(interactor);

            // Clearing some references to the soon to be destroyed component
            interactable.onSelectExit = null;
            interactable.onSelectEnter = null;
            interactable.onHoverEnter = null;
            interactable.onHoverExit = null;
            interactable.onActivate = null;
            interactable.onDeactivate = null;
            interactable.onFirstHoverEnter = null;
            interactable.onLastHoverExit = null;

            Destroy(interactable);
        }

        #endregion
    }
}