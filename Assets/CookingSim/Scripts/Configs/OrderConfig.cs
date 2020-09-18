using UnityEngine;
using UnityEngine.Serialization;

namespace XRAccelerator.Configs
{
    [CreateAssetMenu(fileName = "New Order Config", menuName = "Configs/Order", order = 0)]
    public class OrderConfig : ScriptableObject
    {
        public IngredientConfig TargetIngredient;
        public float MaxOrderTime;
    }
}