using UnityEngine;

namespace XRAccelerator.Configs
{
    [CreateAssetMenu(fileName = "New Pot Recipe Config", menuName = "Configs/Pot Recipe", order = 0)]
    public class PotRecipeConfig : RecipeConfig
    {
        [Header("Pot Recipe specific")]
        [SerializeField]
        [Tooltip("How long the player should stir for.")]
        public float StirringTime;

        [SerializeField]
        [Tooltip("How long the pot should stay in the fire.")]
        public float CookTime = 5;
    }
}