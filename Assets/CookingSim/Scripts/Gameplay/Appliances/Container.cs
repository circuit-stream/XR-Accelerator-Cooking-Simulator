using System.Collections.Generic;
using UnityEngine;
using XRAccelerator.Configs;
using XRAccelerator.Services;

namespace XRAccelerator.Gameplay
{
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
        [SerializeField]
        [Tooltip("What ingredient to create on a failed activation")]
        private IngredientConfig failedRecipeIngredient;

        protected readonly List<IngredientAmount> CurrentIngredients = new List<IngredientAmount>();
        protected readonly List<IngredientGraphics> CurrentIngredientGraphics = new List<IngredientGraphics>();
        protected RecipeConfig CurrentRecipeConfig;

        private float currentLiquidVolume;

        public void AddLiquidIngredient(List<IngredientAmount> addedIngredients)
        {
            var newlyAddedVolume = IngredientAmount.TotalListAmount(addedIngredients);
            currentLiquidVolume += newlyAddedVolume;

            OnIngredientsEnter(addedIngredients);
            liquidContainer.AddLiquid(newlyAddedVolume);
        }

        protected virtual void ExecuteRecipe()
        {
            var currentAmount = IngredientAmount.TotalListAmount(CurrentIngredients);
            if (currentAmount == 0)
            {
                return;
            }

            DestroyCurrentIngredients();
            CreateRecipeIngredient(currentAmount);
        }

        private void CreateRecipeIngredient(float amount)
        {
            IngredientConfig newIngredientConfig = GetOutputIngredientConfig();

            if (newIngredientConfig is LiquidIngredientConfig)
            {
                CreateLiquidIngredient((LiquidIngredientConfig)newIngredientConfig, amount);
            }
            else
            {
                CreateIngredientGraphics(newIngredientConfig.IngredientPrefab, amount);
            }
        }

        private void CreateLiquidIngredient(LiquidIngredientConfig config, float amount)
        {
            AddLiquidIngredient(new List<IngredientAmount>
            {
                new IngredientAmount {Ingredient = config, Amount = amount}
            });
        }

        private void CreateIngredientGraphics(IngredientGraphics prefab, float amount)
        {
            var ingredientGraphics = Instantiate(prefab, transform.position, Quaternion.identity);
            ingredientGraphics.SetCurrentIngredientAmount(amount);
        }

        protected virtual bool WasRecipeSuccessful()
        {
            return CurrentRecipeConfig != null;
        }

        private IngredientConfig GetOutputIngredientConfig()
        {
            if (!WasRecipeSuccessful())
            {
                return failedRecipeIngredient;
            }

            return CurrentRecipeConfig.OutputIngredient;
        }

        private void DestroyCurrentIngredients()
        {
            // Destroy solids
            foreach (var ingredientGraphics in CurrentIngredientGraphics)
            {
                Destroy(ingredientGraphics.gameObject);
            }

            // Empty container
            currentLiquidVolume = 0;
            liquidContainer.Empty();

            CurrentIngredientGraphics.Clear();
            CurrentIngredients.Clear();
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