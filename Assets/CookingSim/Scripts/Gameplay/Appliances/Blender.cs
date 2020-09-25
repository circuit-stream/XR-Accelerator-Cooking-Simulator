using UnityEngine;
using XRAccelerator.Gameplay;

namespace CookingSim.Scripts.Gameplay.Appliances
{
    public class Blender : Container
    {
        // TODO Arthur Optional: blend time per recipe
        private const float blendTime = 5;

        [SerializeField]
        [Tooltip("The switch that will enable the faucet")]
        private RotarySwitch blenderSwitch;

        [SerializeField]
        [Tooltip("Reference to the blender animator")]
        private Animator animator;

        private float applianceEnabledTime;

        protected override void ExecuteRecipe()
        {
            base.ExecuteRecipe();

            applianceEnabledTime = 0;
        }

        private void Update()
        {
            if (!isApplianceEnabled)
            {
                return;
            }

            applianceEnabledTime += Time.deltaTime;

            if (applianceEnabledTime > blendTime)
            {
                ExecuteRecipe();
            }
        }

        private void EnableAppliance()
        {
            // TODO: Check if lid and glass is in place

            isApplianceEnabled = true;
            applianceEnabledTime = 0;

            // TODO: animator / timer
        }

        private void DisableAppliance()
        {
            isApplianceEnabled = false;
        }

        private void OnSwitchStateChange(int index)
        {
            if (index > 0)
            {
                // TODO Arthur: Handle blender speeds
                EnableAppliance();
            }
            else
            {
                DisableAppliance();
            }
        }

        protected override void Awake()
        {
            base.Awake();

            blenderSwitch.StateChanged += OnSwitchStateChange;
        }
    }
}