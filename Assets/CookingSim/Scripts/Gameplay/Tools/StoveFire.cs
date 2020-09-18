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
        private List<int> availableHeatTemperatures;

        private int heatTemperatureIndex;

        public void SetHeatTemperatureIndex(int index)
        {
            if (index == 0)
            {
                IsEnabled = false;
                return;
            }

            IsEnabled = true;
            heatTemperatureIndex = index - 1;
        }

        // TODO Arthur: Remove this when we have the toggle
        private void Awake()
        {
            SetHeatTemperatureIndex(1);
        }
    }
}