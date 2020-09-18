using UnityEngine;
using XRAccelerator.Gameplay;

namespace XRAccelerator.Configs
{
    [CreateAssetMenu(fileName = "new Ingredient Config", menuName = "Configs/Ingredient", order = 0)]
    public class IngredientConfig : ScriptableObject
    {
        public IngredientGraphics IngredientPrefab;
    }
}