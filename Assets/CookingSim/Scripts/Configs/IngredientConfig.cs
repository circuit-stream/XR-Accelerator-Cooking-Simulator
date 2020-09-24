using System.Collections.Generic;
using UnityEngine;
using XRAccelerator.Enums;
using XRAccelerator.Gameplay;

namespace XRAccelerator.Configs
{
    [CreateAssetMenu(fileName = "new Ingredient Config", menuName = "Configs/Ingredient", order = 0)]
    public class IngredientConfig : ScriptableObject
    {
        [SerializeField]
        [Tooltip("The type of this ingredient")]
        public List<IngredientType> IngredientTypes;

        [SerializeField]
        [Tooltip("The prefab that will be created when a recipe generates this ingredient.")]
        public IngredientGraphics IngredientPrefab;
    }
}