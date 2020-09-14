using System;
using System.Collections.Generic;
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
            public IngredientConfig Ingredient;
            public float MinAmount;
            public float MaxAmount;
        }

        [SerializeField]
        public List<IngredientRequirement> SourceIngredients;
        [SerializeField]
        public ApplianceType ApplianceType;
        [SerializeField]
        public IngredientConfig OutputIngredient;

        public bool DoesIngredientsSatisfyRecipe(List<IngredientAmount> ingredients)
        {
            if (ingredients.Count != SourceIngredients.Count)
            {
                return false;
            }

            foreach (var sourceIngredient in SourceIngredients)
            {
                var ingredientAmount = ingredients.Find(possibleIngredient => possibleIngredient.Ingredient == sourceIngredient.Ingredient);

                if (IngredientSatisfyQuantity(sourceIngredient, ingredientAmount))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IngredientSatisfyQuantity(IngredientRequirement requirement, IngredientAmount ingredientAmount)
        {
            return ingredientAmount == null ||
                   requirement.MaxAmount < ingredientAmount.Amount ||
                   requirement.MinAmount > ingredientAmount.Amount;
        }
    }
}