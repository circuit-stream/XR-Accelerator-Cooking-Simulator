using System;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRAccelerator.Gameplay
{
    [Serializable]
    public class ProxyHandsVisuals
    {
        [SerializeField]
        [Tooltip("A reference to the right hand ProxyHandVisual component.\nSet this if you want to display a geometric matched hand when the proxy is grabbed")]
        private ProxyHandVisuals rightHandProxyHandVisuals;

        [SerializeField]
        [Tooltip("A reference to the left hand ProxyHandVisual component.\nSet this if you want to display a geometric matched hand when the proxy is grabbed")]
        private ProxyHandVisuals leftHandProxyHandVisuals;

        private bool HasProxyVisuals => rightHandProxyHandVisuals != null;

        private XRBaseInteractor currentInteractor;
        private XRController currentController;

        public void EnableProxyHandVisual(XRController controller, XRBaseInteractor interactor)
        {
            if (!HasProxyVisuals)
            {
                return;
            }

            currentInteractor = interactor;
            currentController = controller;

            GetMatchingControllerProxy().Enable();
            currentController.hideControllerModel = true;
        }

        public void DisableProxyHandVisual()
        {
            if (!HasProxyVisuals)
            {
                return;
            }

            GetMatchingControllerProxy().Disable();
            currentController.hideControllerModel = false;
        }

        public void Setup()
        {
            if (HasProxyVisuals)
            {
                rightHandProxyHandVisuals.Disable();
                leftHandProxyHandVisuals.Disable();
            }
        }

        private ProxyHandVisuals GetMatchingControllerProxy()
        {
            return currentController.controllerNode == XRNode.RightHand
                ? rightHandProxyHandVisuals
                : leftHandProxyHandVisuals;
        }
    }
}