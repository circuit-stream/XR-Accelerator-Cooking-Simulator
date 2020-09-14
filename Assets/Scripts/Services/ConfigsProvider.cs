using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using XRAccelerator.Configs;
using XRAccelerator.Enums;

namespace XRAccelerator.Services
{
    public class ConfigsProvider
    {
        private RecipeConfig[] recipeConfigs;

        public ConfigsProvider()
        {
            FetchConfigs();
        }

        public List<RecipeConfig> GetRecipesForAppliance(ApplianceType applianceType)
        {
            return recipeConfigs.Where(recipeConfig => recipeConfig.ApplianceType == applianceType).ToList();
        }

        public List<LevelConfig> GetAllLevelConfigs()
        {
            // TODO Arthur: return all LevelConfigs
            throw new NotImplementedException();
        }

        private void FetchConfigs()
        {
            // TODO Arthur: Load RecipeConfigs / IngredientConfigs / ...
            recipeConfigs = Resources.LoadAll<RecipeConfig>("RecipeConfigs");

            // TODO Arthur Optional: Use AssetBundles / Addressables
        }
    }
}