using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XRAccelerator.Configs;
using XRAccelerator.Services;

namespace XRAccelerator.Gameplay
{
    [RequireComponent(typeof(LiquidContainer))]
    [RequireComponent(typeof(LiquidPourOrigin))]
    public abstract class Container : Appliance
    {
        [SerializeField]
        [Tooltip("LiquidContainer component reference, responsible for the liquid visuals")]
        private LiquidContainer liquidContainer;
        [SerializeField]
        [Tooltip("LiquidPourOrigin component reference, responsible for the liquid pouring visuals")]
        private LiquidPourOrigin liquidPourOrigin;
        [SerializeField]
        [Tooltip("The collider that detects liquid collision and adds it to the container.\nMust be a trigger collider and on Container layer")]
        private Collider liquidCollider;

        protected readonly List<IngredientAmount> CurrentIngredients = new List<IngredientAmount>();
        protected readonly List<IngredientGraphics> CurrentIngredientGraphics = new List<IngredientGraphics>();
        protected RecipeConfig CurrentRecipeConfig;

        private float currentLiquidVolume;

        public void AddLiquidIngredient(List<IngredientAmount> addedIngredients)
        {
            var newlyAddedVolume = addedIngredients.Select(entry => entry.Amount).Sum();
            currentLiquidVolume += newlyAddedVolume;

            OnIngredientsEnter(addedIngredients);
            liquidContainer.AddLiquid(newlyAddedVolume);
        }

        protected virtual void OnIngredientsEnter(List<IngredientAmount> addedIngredients)
        {
            // TODO Arthur: Change container weight

            AddIngredients(addedIngredients);
            SetCurrentRecipe();
        }

        protected virtual void OnIngredientsExit(List<IngredientAmount> removedIngredients)
        {
            RemoveIngredients(removedIngredients);
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            var ingredientGraphics = other.gameObject.GetComponent<IngredientGraphics>();
            if (ingredientGraphics != null)
            {
                CurrentIngredientGraphics.Add(ingredientGraphics);
                OnIngredientsEnter(ingredientGraphics.CurrentIngredients);
            }
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            var ingredientGraphics = other.gameObject.GetComponent<IngredientGraphics>();
            if (ingredientGraphics != null)
            {
                CurrentIngredientGraphics.Remove(ingredientGraphics);
                OnIngredientsExit(ingredientGraphics.CurrentIngredients);
            }
        }

        private void Spill(float volumeSpilled)
        {
            List<IngredientAmount> spilledIngredients =
                LiquidIngredientConfig.GetLiquidIngredientsForVolume(CurrentIngredients, volumeSpilled, currentLiquidVolume);
            RemoveIngredients(spilledIngredients);

            liquidPourOrigin.AddIngredientsToPour(spilledIngredients);
        }

        private List<IngredientAmount> GetLiquidIngredientsForVolume(float liquidVolume)
        {
            var newList = new List<IngredientAmount>();

            foreach (var ingredient in CurrentIngredients)
            {
                if (ingredient.Ingredient is LiquidIngredientConfig)
                {
                    newList.Add(new IngredientAmount
                    {
                        Ingredient = ingredient.Ingredient,
                        Amount = ingredient.Amount * liquidVolume / currentLiquidVolume
                    });
                }
            }

            return newList;
        }
        private void AddIngredients(List<IngredientAmount> addedIngredients)
        {
            IngredientAmount.AddToIngredientsList(CurrentIngredients, addedIngredients);
        }

        private void RemoveIngredients(List<IngredientAmount> removedIngredients)
        {
            foreach (var removedIngredient in removedIngredients)
            {
                var oldIngredient = CurrentIngredients.Find(ingredientEntry =>
                    ingredientEntry.Ingredient == removedIngredient.Ingredient);

                Debug.Assert(oldIngredient != null, $"Trying to remove inexistent ingredient: {removedIngredient.Ingredient.name}");

                oldIngredient.Amount -= removedIngredient.Amount;

                if (oldIngredient.Ingredient is LiquidIngredientConfig)
                {
                    currentLiquidVolume -= removedIngredient.Amount;
                }

                if (oldIngredient.Amount <= 0)
                {
                    CurrentIngredients.Remove(oldIngredient);
                }
            }
        }

        private void SetCurrentRecipe()
        {
            CurrentRecipeConfig = GetRecipeForIngredients(CurrentIngredients);
        }

        private void Start()
        {
            liquidPourOrigin.RegisterParticleColliders(liquidCollider);
            liquidPourOrigin.TrackContainer(liquidContainer);
        }

        protected override void Awake()
        {
            base.Awake();

            liquidContainer.Spilled += Spill;

            ServiceLocator.GetService<ContainerCollidersProvider>().RegisterContainerCollider(liquidCollider);
        }
    }
}