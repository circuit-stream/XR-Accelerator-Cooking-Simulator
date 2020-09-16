using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace XRAccelerator.Player
{
    public class VRControllerToggler : MonoBehaviour
    {
        [Serializable]
        private class ControllerState
        {
            [SerializeField]
            private bool requestControlOnHover = true;
            [Header("References")]
            [SerializeField]
            private XRController xrController;
            [SerializeField]
            private XRBaseControllerInteractor xrControllerInteractor;

            public bool IsRequestingControl => (requestControlOnHover && isHovering) || IsPressingSelect();
            public bool IsLockingControl => isSelecting;

            private bool isHovering;
            private bool isSelecting;

            public void EnterState()
            {
                xrController.hideControllerModel = false;
                xrController.enableInputActions = true;
            }

            public void ExitState()
            {
                xrController.hideControllerModel = true;
                xrController.enableInputActions = false;
            }

            private bool IsPressingSelect()
            {
                xrController.inputDevice.IsPressed(xrController.selectUsage, out var pressed, xrController.axisToPressThreshold);
                return pressed;
            }

            private void OnHoverEnter(XRBaseInteractable interactable)
            {
                isHovering = true;
            }

            private void OnHoverExit(XRBaseInteractable interactable)
            {
                isHovering = false;
            }

            private void OnSelectEnter(XRBaseInteractable interactable)
            {
                isSelecting = true;
            }

            private void OnSelectExit(XRBaseInteractable interactable)
            {
                isSelecting = false;
            }

            public void Initialize()
            {
                xrControllerInteractor.onHoverEnter.AddListener(OnHoverEnter);
                xrControllerInteractor.onHoverExit.AddListener(OnHoverExit);
                xrControllerInteractor.onSelectEnter.AddListener(OnSelectEnter);
                xrControllerInteractor.onSelectExit.AddListener(OnSelectExit);
            }
        }

        [SerializeField]
        [Tooltip("Add states in priority order")]
        private List<ControllerState> controllerStates;

        private int currentControllerStateIndex;
        private ControllerState CurrentControllerState => controllerStates[currentControllerStateIndex];

        private void EnterState(int newStateIndex)
        {
            if (newStateIndex == currentControllerStateIndex)
            {
                return;
            }

            CurrentControllerState.ExitState();
            currentControllerStateIndex = newStateIndex;
            CurrentControllerState.EnterState();
        }

        private void Update()
        {
            if (CurrentControllerState.IsLockingControl)
                return;

            for (var index = 0; index < controllerStates.Count; index++)
            {
                var controllerState = controllerStates[index];
                if (controllerState.IsRequestingControl)
                {
                    EnterState(index);
                    return;
                }
            }
        }

        private IEnumerator Initialize()
        {
            // Model setup is done by XRController in the Update method during the first frame
            yield return null;

            foreach (var controllerState in controllerStates)
            {
                controllerState.Initialize();
                controllerState.ExitState();
            }

            currentControllerStateIndex = 0;
            CurrentControllerState.EnterState();
        }

        private void Start()
        {
            StartCoroutine(Initialize());
        }
    }
}