using System;
using UnityEngine;

namespace XRAccelerator.Gameplay
{
    public class StewPan : Container
    {
        // TODO Arthur Optional: Heat temperature, stirring velocity, cook time per recipe
        private const float cookTime = 5;
        private const float minStirringTime = 0;

        [SerializeField]
        private IngredientGraphics burntIngredientPrefab;

        private float applianceEnabledTime;
        private float stirringTime;

        private void EnableAppliance()
        {
            isApplianceEnabled = true;
            applianceEnabledTime = 0;
        }

        private void DisableAppliance()
        {
            isApplianceEnabled = false;
        }

        private void ExecuteRecipe()
        {
            DestroyCurrentIngredients();
            CreateIngredient(GetOutputIngredient());

            applianceEnabledTime = 0;
            stirringTime = 0;
        }

        private IngredientGraphics GetOutputIngredient()
        {
            if (stirringTime < minStirringTime || CurrentRecipeConfig == null)
            {
                // TODO Arthur Optional: Have different prefab for burnt / wrong ingredients
                return burntIngredientPrefab;
            }

            return CurrentRecipeConfig.OutputIngredient.IngredientPrefab;
        }

        private void DestroyCurrentIngredients()
        {
            foreach (var ingredientGraphics in CurrentIngredientGraphics)
            {
                Destroy(ingredientGraphics.gameObject);
            }

            CurrentIngredientGraphics.Clear();
            CurrentIngredients.Clear();
        }

        private void CreateIngredient(IngredientGraphics prefab)
        {
            Instantiate(prefab, transform.position, Quaternion.identity);
        }

        protected override void OnTriggerEnter(Collider other)
        {
            var stoveFire = other.gameObject.GetComponent<StoveFire>();
            if (stoveFire != null)
            {
                EnableAppliance();
                return;
            }

            // TODO Arthur: If spoon Enable Stirring

            base.OnTriggerEnter(other);
        }

        protected override void OnTriggerExit(Collider other)
        {
            var stoveFire = other.gameObject.GetComponent<StoveFire>();
            if (stoveFire != null)
            {
                DisableAppliance();
                return;
            }

            base.OnTriggerExit(other);

            // TODO Arthur: If spoon Disable Stirring
        }

        private void Update()
        {
            if (!isApplianceEnabled)
                return;

            applianceEnabledTime += Time.deltaTime;

            if (applianceEnabledTime > cookTime)
            {
                ExecuteRecipe();
            }
        }
    }
}