using System;
using System.Collections.Generic;
using UnityEngine;
using XRAccelerator.Configs;

namespace XRAccelerator.Gameplay
{
    public class IngredientGraphics : MonoBehaviour
    {
        public readonly List<IngredientAmount> CurrentIngredients = new List<IngredientAmount>();

        public void OnEnterAppliance()
        {
            throw new NotImplementedException();
        }
    }
}