using System;
using System.Collections.Generic;
using UnityEngine;
using XRAccelerator.Configs;

namespace XRAccelerator.Gameplay
{
    public abstract class Container : Appliance
    {
        protected readonly List<IngredientAmount> CurrentIngredients = new List<IngredientAmount>();
        protected readonly List<IngredientGraphics> CurrentIngredientGraphics = new List<IngredientGraphics>();
        protected RecipeConfig CurrentRecipeConfig;

        public void AddIngredients(List<IngredientAmount> addedIngredients)
        {
            IngredientAmount.AddToIngredientsList(CurrentIngredients, addedIngredients);
        }

        protected void SetCurrentRecipe()
        {
            CurrentRecipeConfig = GetRecipeForIngredients(CurrentIngredients);
        }

        protected void Pour()
        {
            // TODO Arthur: Slowly remove liquid ingredients from current Ingredient, instantiate liquid graphics, call OnRemoveIngredient
            throw new NotImplementedException();
        }

        protected void AddLiquidIngredient()
        {
            // TODO Arthur: Change the container to show the liquid
            throw new NotImplementedException();
        }

        protected virtual void OnIngredientEnter(IngredientGraphics ingredientGraphics)
        {
            AddIngredients(ingredientGraphics.CurrentIngredients);
            CurrentIngredientGraphics.Add(ingredientGraphics);
            SetCurrentRecipe();

            // TODO Arthur: Call OnAddLiquidIngredient
        }

        protected virtual void OnIngredientExit(IngredientGraphics ingredientGraphics)
        {
            RemoveIngredients(ingredientGraphics.CurrentIngredients);
            CurrentIngredientGraphics.Remove(ingredientGraphics);
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            var ingredient = other.gameObject.GetComponent<IngredientGraphics>();
            if (ingredient != null)
            {
                OnIngredientEnter(ingredient);
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            var ingredient = other.gameObject.GetComponent<IngredientGraphics>();
            if (ingredient != null)
            {
                OnIngredientExit(ingredient);
            }
        }

        private void RemoveIngredients(List<IngredientAmount> removedIngredients)
        {
            foreach (var removedIngredient in removedIngredients)
            {
                var oldIngredient = CurrentIngredients.Find(ingredientEntry =>
                    ingredientEntry.Ingredient == removedIngredient.Ingredient);

                Debug.Assert(oldIngredient != null, $"Trying to remove inexistent ingredient: {removedIngredient.Ingredient.name}");

                oldIngredient.Amount -= removedIngredient.Amount;

                if (oldIngredient.Amount <= 0)
                {
                    CurrentIngredients.Remove(oldIngredient);
                }
            }
        }
    }
}