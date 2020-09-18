using System;
using System.Collections.Generic;
using UnityEngine;

namespace XRAccelerator.Player
{
    [Serializable]
    public class VRControllerToggler
    {
        [SerializeField]
        [Tooltip("Add all possible XRControllers that uses the same ControllerNode; This is an ordered list, so the first entry interactions has a higher priority.")]
        private List<ControllerActivationState> controllerActivationPriority;

        private int activeControllerIndex;
        private ControllerActivationState ActiveController => controllerActivationPriority[activeControllerIndex];

        private void ChangeActiveController(int newStateIndex)
        {
            if (newStateIndex == activeControllerIndex)
            {
                return;
            }

            ActiveController.DisableController();
            activeControllerIndex = newStateIndex;
            ActiveController.EnableController();
        }

        public void Update()
        {
            if (ActiveController.IsLockingControl)
                return;

            for (var index = 0; index < controllerActivationPriority.Count; index++)
            {
                var controllerState = controllerActivationPriority[index];
                if (controllerState.IsRequestingControl)
                {
                    ChangeActiveController(index);
                    return;
                }
            }
        }

        public void Initialize()
        {
            foreach (var controllerState in controllerActivationPriority)
            {
                controllerState.Initialize();
                controllerState.DisableController();
            }

            activeControllerIndex = 0;
            ActiveController.EnableController();
        }
    }
}