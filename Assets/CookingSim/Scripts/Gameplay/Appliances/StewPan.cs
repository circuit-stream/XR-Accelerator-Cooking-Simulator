using System;
using UnityEngine;

namespace XRAccelerator.Gameplay
{
    public class StewPan : Container
    {
        // TODO Arthur Optional: Heat temperature, stirring velocity, cook time per recipe
        private const float cookTime = 5;
        private const float minStirringTime = 2;

        [SerializeField]
        private IngredientGraphics burntIngredientPrefab;

        private bool isOverFire;
        private float applianceEnabledTime;
        private float stirringTime;

        private StirringSpoon currentStirringSpoon;
        private bool IsStirring => currentStirringSpoon != null && currentStirringSpoon.IsStirring;

        private void TryEnableAppliance()
        {
            if (isApplianceEnabled || !isOverFire || CurrentIngredients.Count == 0)
            {
                return;
            }

            isApplianceEnabled = true;
            applianceEnabledTime = 0;
            stirringTime = 0;

            // TODO Arthur: enabled visual feedback
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

        protected override void OnIngredientEnter(IngredientGraphics ingredientGraphics)
        {
            base.OnIngredientEnter(ingredientGraphics);

            TryEnableAppliance();
        }

        protected override void OnIngredientExit(IngredientGraphics ingredientGraphics)
        {
            base.OnIngredientExit(ingredientGraphics);

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
                Debug.Log("StirringSpoon");
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