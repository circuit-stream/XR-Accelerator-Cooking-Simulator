using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using XRAccelerator.Enums;
using InputDevice = UnityEngine.XR.InputDevice;

namespace XRAccelerator.Player
{
    // This component should be placed on a direct child gameObject of the modelAttachPoint,
    // with the custom transform offsets as children of this gameObject
    public class CustomControllerModelOffset : MonoBehaviour
    {
        private const string knucklesControllerPartialName = "Knuckles";
        private const string riftControllerPartialName = "Rift";
        private const string oculusControllerPartialName = "Quest";
        private const string viveControllerPartialName = "Vive";

        [Serializable]
        private class ModelOffset
        {
            public SupportedVRControllers vrController;
            public Transform offset;
            public Vector3 colliderOffset;
        }

        [SerializeField]
        private ActionBasedController xrController;

        [SerializeField]
        private Transform modelAttachPoint;

        [SerializeField]
        [Tooltip("[Optional] Reference to the collider used by a directInteractor")]
        private SphereCollider controllerCollider;

        [SerializeField]
        private List<ModelOffset> customModelsOffsets;

        private void Awake()
        {
            InputDevices.deviceConnected += TryApplyOffsets;
        }

        private void TryApplyOffsets(InputDevice inputDevice)
        {
            var currentController = GetConnectedController(inputDevice.name);

            if (currentController == SupportedVRControllers.Unsupported)
                return;

            var modelOffset = customModelsOffsets.Find(entry => entry.vrController == currentController);

            ApplyOffsets(modelOffset);

            InputDevices.deviceConnected -= TryApplyOffsets;
            Destroy(gameObject);
        }

        private void ApplyOffsets(ModelOffset modelOffset)
        {
            if (modelOffset == null)
            {
                return;
            }

            modelAttachPoint.localPosition = modelOffset.offset.localPosition;
            modelAttachPoint.localRotation = modelOffset.offset.localRotation;

            if (controllerCollider != null)
            {
                controllerCollider.center = modelOffset.colliderOffset;
            }
        }

        private SupportedVRControllers GetConnectedController(string controllerName)
        {
            if (CaseInsensitiveContains(controllerName, knucklesControllerPartialName))
            {
                return SupportedVRControllers.Knuckles;
            }
            if (CaseInsensitiveContains(controllerName, riftControllerPartialName))
            {
                return SupportedVRControllers.OculusTouch;
            }
            if (CaseInsensitiveContains(controllerName, oculusControllerPartialName))
            {
                return SupportedVRControllers.OculusTouch;
            }
            if (CaseInsensitiveContains(controllerName, viveControllerPartialName))
            {
                return SupportedVRControllers.ViveWand;
            }

            return SupportedVRControllers.Unsupported;
        }

        private bool CaseInsensitiveContains(string src, string sub)
        {
            return src.IndexOf(sub, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}