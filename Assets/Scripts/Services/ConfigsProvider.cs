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
            // TODO Arthur: Any other necessary initialization
            FetchConfigs();
        }

        public List<RecipeConfig> GetRecipesForAppliance(ApplianceType applianceType)
        {
            // TODO Arthur: return all recipes for target appliance
            throw new NotImplementedException();
        }

        public List<LevelConfig> GetAllLevelConfigs()
        {
            // TODO Arthur: return all LevelConfigs
            throw new NotImplementedException();
        }

        private void FetchConfigs()
        {
            // TODO Arthur: Load RecipeConfigs / IngredientConfigs / ...
            throw new NotImplementedException();
            
            // TODO Arthur Optional: Use AssetBundles / Addressables
        }
    }
}