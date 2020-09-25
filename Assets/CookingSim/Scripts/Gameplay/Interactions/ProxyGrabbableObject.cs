using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using XRAccelerator.Services;

namespace XRAccelerator.Gameplay
{
    public class ProxyGrabbableObject : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("How much force should break the FixedJoint proxy <> connectedBody")]
        private float breakForce;
        [SerializeField]
        [Tooltip("How much torque should break the FixedJoint proxy <> connectedBody")]
        private float breakTorque;

        [Header("Prefab References")]
        [SerializeField]
        [Tooltip("The actual object that will be lifted when the user picks up this proxy")]
        private Rigidbody connectedBody;
        [SerializeField]
        [Tooltip("A reference to a ProxyHandVisual monobehavior component.\nSet this if you want to display a geometric matched hand when the proxy is grabbed")]
        private ProxyHandVisuals proxyHandVisuals;

        private Rigidbody proxyBody;
        private Collider proxyCollider;
        private Transform proxyTransform;
        private Transform proxyParent;

        private Vector3 initialPosition;
        private Quaternion initialRotation;
        private float initialSmoothRotationAmount;
        private float initialTightenRotation;
        private XRBaseInteractable.MovementType initialMovementType;

        private XRBaseInteractor currentInteractor;
        private XRController currentController;
        private Joint currentJoint;
        private bool isBeingGrabbed;

        private void OnJointBreak(float _)
        {
            currentJoint = null;
            proxyCollider.enabled = false;
            DestroyXRGrabInteractableComponent();

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
            isBeingGrabbed = true;
            currentInteractor = interactor;
            currentController = currentInteractor.GetComponent<XRController>();

            StartCoroutine(CreateJoint());
            EnableProxyHandVisual();
        }

        private void OnGrabRelease(XRBaseInteractor interactor)
        {
            isBeingGrabbed = false;
            DestroyJoint();
            DisableProxyHandVisual();

            currentInteractor = null;
            currentController = null;
        }

        private IEnumerator ReenableGrab()
        {
            yield return null;

            CreateXRGrabInteractableComponent();
            proxyCollider.enabled = true;
        }

        private void ResetGrabbableTransform()
        {
            proxyTransform.parent = proxyParent;
            proxyTransform.localRotation = initialRotation;
            proxyTransform.localPosition = initialPosition;
            proxyBody.position = proxyTransform.position;
            proxyBody.rotation = proxyTransform.rotation;
            proxyBody.velocity = Vector3.zero;
            proxyBody.angularVelocity = Vector3.zero;
        }

        private IEnumerator CreateJoint()
        {
            // Wait from grabbable attach
            yield return new WaitForSeconds(0.1f);

            currentJoint = gameObject.AddComponent<FixedJoint>();
            currentJoint.connectedBody = connectedBody;
            currentJoint.breakForce = breakForce;
            currentJoint.breakTorque = breakTorque;
            currentJoint.enablePreprocessing = false;
        }

        private void DestroyJoint()
        {
            if (currentJoint != null)
            {
                Destroy(currentJoint);
                currentJoint = null;
            }
        }

        private void OnRigStartLocomotion(LocomotionSystem locomotionSystem)
        {
            if (isBeingGrabbed)
            {
                proxyCollider.enabled = false;
                DestroyXRGrabInteractableComponent();
                StartCoroutine(ReenableGrab());
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                DestroyJoint();
            }

            if (isBeingGrabbed)
            {
                return;
            }

            ResetGrabbableTransform();
        }

        private void Awake()
        {
            proxyBody = GetComponent<Rigidbody>();
            proxyCollider = proxyBody.GetComponent<Collider>();
            var grabInteractable = GetComponent<XRGrabInteractable>();

            proxyTransform = transform;
            proxyParent = proxyTransform.parent;

            initialPosition = proxyTransform.localPosition;
            initialRotation = proxyTransform.localRotation;
            initialSmoothRotationAmount = grabInteractable.smoothRotationAmount;
            initialTightenRotation = grabInteractable.tightenRotation;
            initialMovementType = grabInteractable.movementType;

            grabInteractable.onSelectEnter.AddListener(OnGrab);
            grabInteractable.onSelectExit.AddListener(OnGrabRelease);

            var referencesProvider = ServiceLocator.GetService<ComponentReferencesProvider>();
            foreach (var locomotionProvider in referencesProvider.registeredLocomotionProviders)
            {
                locomotionProvider.startLocomotion += OnRigStartLocomotion;
            }
        }

        // [XRToolkitWorkaround] These functions only exist because XRToolkit provides no way to forcefully deselect a interactable
        #region XRToolkit workaround

        private void CreateXRGrabInteractableComponent()
        {
            var grabInteractable = gameObject.AddComponent<XRGrabInteractable>();

            grabInteractable.retainTransformParent = false; // We set this by hand
            grabInteractable.throwOnDetach = false;
            grabInteractable.smoothRotation = true;

            grabInteractable.tightenRotation = initialTightenRotation;
            grabInteractable.smoothRotationAmount = initialSmoothRotationAmount;
            grabInteractable.movementType = initialMovementType;

            grabInteractable.onSelectEnter.AddListener(OnGrab);
            grabInteractable.onSelectExit.AddListener(OnGrabRelease);
        }

        private void DestroyXRGrabInteractableComponent()
        {
            var interactable = GetComponent<XRGrabInteractable>();
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