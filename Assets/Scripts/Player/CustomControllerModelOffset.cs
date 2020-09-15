using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using XRAccelerator.Enums;

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
        }

        [SerializeField]
        private XRController xrController;

        [SerializeField]
        private Transform modelAttachPoint;

        [SerializeField]
        private List<ModelOffset> customModelsOffsets;

        private void Awake()
        {
            InputDevices.deviceConnected += TryApplyHandOffset;
        }

        private void TryApplyHandOffset(InputDevice inputDevice)
        {
            if (xrController.inputDevice != inputDevice)
            {
                return;
            }

            var currentController = GetConnectedController();
            var modelOffset = customModelsOffsets.Find(entry => entry.vrController == currentController);

            ApplyHandOffset(modelOffset);

            InputDevices.deviceConnected -= TryApplyHandOffset;
            Destroy(gameObject);
        }

        private void ApplyHandOffset(ModelOffset modelOffset)
        {
            if (modelOffset == null)
            {
                return;
            }

            modelAttachPoint.localPosition = modelOffset.offset.localPosition;
            modelAttachPoint.localRotation = modelOffset.offset.localRotation;
        }

        private SupportedVRControllers GetConnectedController()
        {
            var controllerName = xrController.inputDevice.name;

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

            Debug.LogError("Using unsupported VR controller device!");
            return SupportedVRControllers.Unsupported;
        }

        private bool CaseInsensitiveContains(string src, string sub)
        {
            return src.IndexOf(sub, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}