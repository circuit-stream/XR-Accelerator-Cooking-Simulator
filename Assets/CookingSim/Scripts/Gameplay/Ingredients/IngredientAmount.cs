using System;
using System.Collections.Generic;
using System.Linq;
using XRAccelerator.Configs;

namespace XRAccelerator.Gameplay
{
    [Serializable]
    public class IngredientAmount
    {
        public IngredientConfig Ingredient;
        public float Amount;

        public static void AddToIngredientsList(List<IngredientAmount> ingredients, List<IngredientAmount> ingredientsToAdd)
        {
            foreach (var addedIngredient in ingredientsToAdd)
            {
                var oldIngredient = ingredients.Find(ingredientEntry =>
                    ingredientEntry.Ingredient == addedIngredient.Ingredient);

                if (oldIngredient != null)
                {
                    oldIngredient.Amount += addedIngredient.Amount;
                }
                else
                {
                    var newIngredient = new IngredientAmount {Ingredient = addedIngredient.Ingredient, Amount = addedIngredient.Amount};
                    ingredients.Add(newIngredient);
                }
            }
        }

        public static float TotalListAmount(List<IngredientAmount> list)
        {
            return list.Select(entry => entry.Amount).Sum();
        }
    }
}