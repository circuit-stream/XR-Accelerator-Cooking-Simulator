using System;
using System.Collections.Generic;
using UnityEngine;
using XRAccelerator.Configs;
using XRAccelerator.Enums;
using XRAccelerator.Services;

namespace XRAccelerator.Gameplay
{
    public abstract class Appliance : MonoBehaviour
    {
        [SerializeField]
        protected ApplianceType applianceType;

        protected List<RecipeConfig> possibleRecipes;
        protected bool isApplianceEnabled;

        protected virtual void Awake()
        {
            var configsProvider = ServiceLocator.GetService<ConfigsProvider>();
            possibleRecipes = configsProvider.GetRecipesForAppliance(applianceType);
        }

        protected RecipeConfig GetRecipeForIngredients(List<IngredientAmount> ingredients)
        {
            foreach (var possibleRecipe in possibleRecipes)
            {
                if (possibleRecipe.DoesIngredientsSatisfyRecipe(ingredients))
                {
                    return possibleRecipe;
                }
            }

            return null;
        }
    }
}