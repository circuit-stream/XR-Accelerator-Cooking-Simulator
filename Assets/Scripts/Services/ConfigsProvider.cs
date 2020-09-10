using System;
using System.Collections.Generic;
using XRAccelerator.Configs;
using XRAccelerator.Enums;

namespace XRAccelerator.Services
{
    public class ConfigsProvider
    {
        public ConfigsProvider()
        {
            // TODO: Any other necessary initialization
            FetchConfigs();
        }

        public List<RecipeConfig> GetRecipesForAppliance(ApplianceType applianceType)
        {
            // TODO: return all recipes for target appliance
            throw new NotImplementedException();
        }

        public List<LevelConfig> GetAllLevelConfigs()
        {
            // TODO: return all LevelConfigs
            throw new NotImplementedException();
        }

        private void FetchConfigs()
        {
            // TODO: Load RecipeConfigs / IngredientConfigs / ...
            throw new NotImplementedException();
            
            // TODO Optional: Use AssetBundles / Addressables
        }
    }
}