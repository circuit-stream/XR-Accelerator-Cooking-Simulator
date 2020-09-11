using System;
using System.Collections.Generic;
using UnityEngine;
using XRAccelerator.Configs;

namespace XRAccelerator.Gameplay
{
    public abstract class Container : Appliance
    {
        public List<IngredientAmount> CurrentIngredients;
        public RecipeConfig CurrentRecipeConfig;

        protected void SetCurrentRecipe()
        {
            // TODO Arthur: Go through appliances recipes and find one the uses currentIngredients as source
            throw new NotImplementedException();
        }
        
        protected void Pour()
        {
            // TODO Arthur: Slowly remove liquid ingredients from current Ingredient, instantiate liquid graphics, call OnRemoveIngredient
            throw new NotImplementedException();
        }

        protected void OnAddLiquidIngredient()
        {
            // TODO Arthur: Change the container to show the liquid
            throw new NotImplementedException();
        }

        protected virtual void OnAddIngredient()
        {
            // TODO Arthur: Add to currentIngredient, possibly trigger container specific logic, Call OnAddLiquidIngredient, call SetCurrentRecipe
            throw new NotImplementedException();
        }

        protected virtual void OnRemoveIngredient()
        {
            // TODO Arthur: Remove from currentIngredient, possibly trigger container specific logic
            throw new NotImplementedException();   
        }
        
        protected virtual void OnTriggerEnter(Collider other)
        {
            // TODO Arthur: Check if is Ingredient and call OnAddIngredient
            throw new NotImplementedException();
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            // TODO Arthur: Check if Ingredient and call OnRemoveIngredient
            throw new NotImplementedException();
        }
    }
}