using System;
using System.Collections.Generic;
using UnityEngine;

namespace XRAccelerator.Player
{
    [Serializable]
    public class VRControllerToggler
    {
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

        public void Update()
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

        public void Initialize()
        {
            foreach (var controllerState in controllerStates)
            {
                controllerState.Initialize();
                controllerState.ExitState();
            }

            currentControllerStateIndex = 0;
            CurrentControllerState.EnterState();
        }
    }
}