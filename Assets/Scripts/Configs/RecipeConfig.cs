using System.Collections.Generic;
using UnityEngine;
using XRAccelerator.Enums;

namespace XRAccelerator.Configs
{
    [CreateAssetMenu(fileName = "New Recipe Config", menuName = "Configs/Recipe", order = 0)]
    public class RecipeConfig : ScriptableObject
    {
        public struct IngredientRequirement
        {
            public IngredientConfig Ingredient;
            public int MinAmount;
            public int MaxAmount;
        }

        public List<IngredientRequirement> SourceIngredients;

        public ApplianceType ApplianceType;
        public IngredientConfig OutputIngredient;
    }
}