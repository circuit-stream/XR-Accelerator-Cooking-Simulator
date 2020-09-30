using System;
using System.Collections.Generic;
using UnityEngine;
using XRAccelerator.Configs;

namespace XRAccelerator.Gameplay
{
    public class SoupPot : Container
    {
        // TODO Arthur Optional: Heat temperature, stirring velocity
        private bool isOverFire;
        private float applianceEnabledTime;
        private float stirringTime;

        private StirringSpoon currentStirringSpoon;
        private bool IsStirring => currentStirringSpoon != null && currentStirringSpoon.IsStirring;

        private PotRecipeConfig CurrentPotRecipeConfig => (PotRecipeConfig) CurrentRecipeConfig;
        private float cookTime => CurrentPotRecipeConfig == null ? 10 : CurrentPotRecipeConfig.CookTime;
        private float requiredStirringTime => CurrentPotRecipeConfig == null ? 0 : CurrentPotRecipeConfig.StirringTime;

        private void TryEnableAppliance()
        {
            applianceEnabledTime = 0;
            stirringTime = 0;

            if (isApplianceEnabled || !isOverFire || CurrentIngredients.Count == 0)
            {
                return;
            }

            isApplianceEnabled = true;

            // TODO Arthur: enabled visual feedback
        }

        private void DisableAppliance()
        {
            isApplianceEnabled = false;
        }

        protected override void ExecuteRecipe()
        {
            base.ExecuteRecipe();

            applianceEnabledTime = 0;
            stirringTime = 0;
        }

        protected override bool WasRecipeSuccessful()
        {
            return base.WasRecipeSuccessful() && WasStirringSuccessful();
        }

        private bool WasStirringSuccessful()
        {
            return stirringTime >= requiredStirringTime;
        }

        protected override void OnIngredientsEnter(List<IngredientAmount> addedIngredients)
        {
            base.OnIngredientsEnter(addedIngredients);

            TryEnableAppliance();
        }

        protected override void OnIngredientsExit(List<IngredientAmount> removedIngredients)
        {
            base.OnIngredientsExit(removedIngredients);

            if (CurrentIngredients.Count == 0)
            {
                DisableAppliance();
            }
        }

        protected override void OnTriggerEnter(Collider other)
        {
            var stoveFire = other.gameObject.GetComponent<StoveFire>();
            if (stoveFire != null)
            {
                isOverFire = true;
                TryEnableAppliance();
                return;
            }

            var stirringSpoon = other.gameObject.GetComponent<StirringSpoon>();
            if (stirringSpoon != null)
            {
                currentStirringSpoon = stirringSpoon;
                return;
            }

            base.OnTriggerEnter(other);
        }

        protected override void OnTriggerExit(Collider other)
        {
            var stoveFire = other.gameObject.GetComponent<StoveFire>();
            if (stoveFire != null)
            {
                isOverFire = false;
                DisableAppliance();
                return;
            }

            var stirringSpoon = other.gameObject.GetComponent<StirringSpoon>();
            if (stirringSpoon != null)
            {
                Debug.Assert(stirringSpoon == currentStirringSpoon, "Possible multiple stirring spoons");
                currentStirringSpoon = null;
                return;
            }

            base.OnTriggerExit(other);
        }

        private void Update()
        {
            if (IsStirring)
            {
                liquidContainer.StartStirring();
            }
            else
            {
                liquidContainer.StopStirring();
            }

            if (!isApplianceEnabled)
                return;

            applianceEnabledTime += Time.deltaTime;
            if (IsStirring)
            {
                // TODO Arthur: Stirring visual feedback
                stirringTime += Time.deltaTime;
            }

            if (applianceEnabledTime > cookTime)
            {
                ExecuteRecipe();
            }
        }
    }
}