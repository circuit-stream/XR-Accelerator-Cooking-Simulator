using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XRAccelerator.Gameplay;

namespace XRAccelerator.Configs
{
    [CreateAssetMenu(fileName = "new Liquid Ingredient Config", menuName = "Configs/Liquid Ingredient", order = 0)]
    public class LiquidIngredientConfig : IngredientConfig
    {
        public static List<IngredientAmount> GetLiquidIngredientsForVolume(List<IngredientAmount> list, float liquidVolume, float currentLiquidVolume = -1)
        {
            if (currentLiquidVolume < 0)
            {
                currentLiquidVolume = list.Select(entry => entry.Amount).Sum();
            }

            var newList = new List<IngredientAmount>();

            foreach (var ingredient in list)
            {
                if (ingredient.Ingredient is LiquidIngredientConfig)
                {
                    newList.Add(new IngredientAmount
                    {
                        Ingredient = ingredient.Ingredient,
                        Amount = liquidVolume * ingredient.Amount / currentLiquidVolume
                    });
                }
            }

            return newList;
        }
    }
}