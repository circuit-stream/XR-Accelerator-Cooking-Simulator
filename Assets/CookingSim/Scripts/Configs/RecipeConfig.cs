using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XRAccelerator.Enums;
using XRAccelerator.Gameplay;

namespace XRAccelerator.Configs
{
    [CreateAssetMenu(fileName = "New Recipe Config", menuName = "Configs/Recipe", order = 0)]
    public class RecipeConfig : ScriptableObject
    {
        [Serializable]
        public class IngredientRequirement
        {
            [SerializeField]
            [Tooltip("Which types of ingredients can be used.")]
            public List<IngredientType> IngredientTypes;

            [SerializeField]
            [Tooltip("[Optional] Specific ingredients required, in case you can't just narrow by IngredientType.\nOnly one entry can be used per recipe execution.")]
            public List<IngredientConfig> Ingredients;

            [SerializeField]
            [Tooltip("The minimum amount of these ingredients for the recipe.")]
            public float MinAmount;

            [SerializeField]
            [Tooltip("The maximum amount of these ingredients for the recipe.")]
            public float MaxAmount;
        }

        [SerializeField]
        [Tooltip("The required ingredients to perform this recipe.")]
        public List<IngredientRequirement> SourceIngredients;

        [SerializeField]
        [Tooltip("Which appliance can execute this recipe.")]
        public ApplianceType ApplianceType;

        [SerializeField]
        [Tooltip("The result ingredient from this recipe.")]
        public IngredientConfig OutputIngredient;

        public bool DoesIngredientsSatisfyRecipe(List<IngredientAmount> ingredients)
        {
            foreach (var sourceIngredient in SourceIngredients)
            {
                // Validate specific ingredient requirement
                if (sourceIngredient.Ingredients != null && sourceIngredient.Ingredients.Count > 0)
                {
                    var ingredientAmount = ingredients.Find(
                        possibleIngredient => sourceIngredient.Ingredients.Contains(possibleIngredient.Ingredient));

                    if (ingredientAmount == null || !IngredientSatisfyQuantity(sourceIngredient, ingredientAmount.Amount))
                    {
                        return false;
                    }
                }

                // Validate ingredient type requirement
                else
                {
                    var totalIngredientTypeAmount = ingredients
                        .Select(possibleIngredient =>
                            SharesIngredientType(possibleIngredient.Ingredient.IngredientTypes, sourceIngredient.IngredientTypes)
                                ? possibleIngredient.Amount
                                : 0)
                        .Sum();

                    if (!IngredientSatisfyQuantity(sourceIngredient, totalIngredientTypeAmount))
                    {
                        return false;
                    }
                }
            }

            // Make sure ingredients have no extra ingredient
            foreach (var ingredientAmount in ingredients)
            {
                var foundIngredient = SourceIngredients.Find(
                    possibleIngredient =>
                    {
                        var sharesType = SharesIngredientType(possibleIngredient.IngredientTypes,
                            ingredientAmount.Ingredient.IngredientTypes);
                        var requiresConfig = possibleIngredient.Ingredients.Contains(ingredientAmount.Ingredient);

                        return requiresConfig || sharesType;
                    });

                if (foundIngredient == null)
                {
                    return false;
                }
            }

            return true;
        }

        private bool IngredientSatisfyQuantity(IngredientRequirement requirement, float ingredientAmount)
        {
            return requirement.MaxAmount > ingredientAmount ||
                   requirement.MinAmount <= ingredientAmount;
        }

        private bool SharesIngredientType(List<IngredientType> listA, List<IngredientType> listB)
        {
            foreach (var ingredientTypeA in listA)
            {
                if (listB.Contains(ingredientTypeA))
                {
                    return true;
                }
            }

            return false;
        }
    }
}