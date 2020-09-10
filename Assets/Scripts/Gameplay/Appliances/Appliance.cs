using System;
using System.Collections.Generic;
using UnityEngine;
using XRAccelerator.Configs;
using XRAccelerator.Enums;

namespace XRAccelerator.Gameplay
{
    public abstract class Appliance : MonoBehaviour
    {
        [SerializeField]
        protected ApplianceType ApplianceType;

        protected List<RecipeConfig> possibleRecipes;

        protected void Awake()
        {
            // TODO: Get possibleRecipes from ConfigsProvider
            throw new NotImplementedException();
        }
    }
}