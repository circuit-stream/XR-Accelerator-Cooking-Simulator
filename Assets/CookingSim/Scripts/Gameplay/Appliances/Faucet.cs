using System.Collections.Generic;
using UnityEngine;
using XRAccelerator.Configs;
using XRAccelerator.Gameplay;

namespace CookingSim.Scripts.Gameplay.Appliances
{
    public class Faucet : Appliance
    {
        [SerializeField]
        [Tooltip("The switch that will enable the faucet")]
        private RotarySwitch faucetSwitch;

        [SerializeField]
        [Tooltip("LiquidPourOrigin component reference, responsible for the liquid pouring visuals")]
        private LiquidPourOrigin liquidPourOrigin;

        [SerializeField]
        [Tooltip("The liquid ingredient that will be created by the faucet")]
        private LiquidIngredientConfig liquidIngredientConfig;

        private void EnableAppliance()
        {
            Debug.Assert(!isApplianceEnabled, "Trying to enable appliance that is already enabled.", gameObject);

            liquidPourOrigin.AddIngredientsToPour(new List<IngredientAmount>
            {
                new IngredientAmount {Ingredient = liquidIngredientConfig, Amount = 1000000000}
            });

            isApplianceEnabled = true;
        }

        private void DisableAppliance()
        {
            Debug.Assert(isApplianceEnabled, "Trying to disable appliance that is already disabled.", gameObject);

            liquidPourOrigin.EndPour();
            isApplianceEnabled = false;
        }

        private void OnSwitchStateChange(int index)
        {
            Debug.Assert(index < 2 && index >= 0, "Faucet rotary switch has more states that the faucet handles", gameObject);

            if (index != 0)
            {
                EnableAppliance();
            }
            else
            {
                DisableAppliance();
            }
        }

        private void Start()
        {
            liquidPourOrigin.RegisterParticleColliders();
        }

        protected override void Awake()
        {
            base.Awake();

            faucetSwitch.StateChanged += OnSwitchStateChange;
        }
    }
}