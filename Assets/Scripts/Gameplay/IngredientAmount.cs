using System;
using XRAccelerator.Configs;

namespace XRAccelerator.Gameplay
{
    [Serializable]
    public struct IngredientAmount
    {
        public IngredientConfig Ingredient;
        public int Amount;
    }
}