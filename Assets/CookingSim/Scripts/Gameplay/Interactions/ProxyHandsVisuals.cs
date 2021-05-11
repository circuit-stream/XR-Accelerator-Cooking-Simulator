using System;
using UnityEngine;
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
        private ActionBasedController currentController;

        public void EnableProxyHandVisual(ActionBasedController controller, XRBaseInteractor interactor)
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
            var isRightController = currentController.activateAction.action.actionMap.name.Contains("Right");
            return isRightController
                ? rightHandProxyHandVisuals
                : leftHandProxyHandVisuals;
        }
    }
}