using System.Collections.Generic;
using UnityEngine;

namespace XRAccelerator.Gameplay
{
    public class IngredientGraphics : MonoBehaviour
    {
        [SerializeField]
        public List<IngredientAmount> CurrentIngredients = new List<IngredientAmount>();

        protected float CurrentIngredientsAmount => IngredientAmount.TotalListAmount(CurrentIngredients);

        public void SetCurrentIngredientAmount(float newAmount)
        {
            var factor = newAmount / CurrentIngredientsAmount;

            foreach (var ingredientAmount in CurrentIngredients)
            {
                ingredientAmount.Amount *= factor;
            }
        }
    }
}