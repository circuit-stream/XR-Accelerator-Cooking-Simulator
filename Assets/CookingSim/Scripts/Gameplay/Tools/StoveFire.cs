using System;
using System.Collections.Generic;
using UnityEngine;

namespace XRAccelerator.Gameplay
{
    public class StoveFire : MonoBehaviour
    {
        public bool IsEnabled { get; private set; }
        public int CurrentHeat => availableHeatTemperatures[heatTemperatureIndex];

        [SerializeField]
        [Tooltip("Reference to the switch correspondent to this fire")]
        private RotarySwitch stoveSwitch;

        [SerializeField]
        [Tooltip("The available heats this stove can reach. Must have the same number of states of his correspondent switch, minus the turned off state.")]
        private List<int> availableHeatTemperatures;

        private int heatTemperatureIndex;

        public void SetHeatTemperatureIndex(int index)
        {
            if (index == 0)
            {
                IsEnabled = false;
                gameObject.SetActive(false);
                return;
            }

            gameObject.SetActive(true);
            IsEnabled = true;
            heatTemperatureIndex = index - 1;
        }

        // TODO Arthur: Remove this when we have the toggle
        private void Awake()
        {
            SetHeatTemperatureIndex(0);
            stoveSwitch.StateChanged += SetHeatTemperatureIndex;
        }
    }
}