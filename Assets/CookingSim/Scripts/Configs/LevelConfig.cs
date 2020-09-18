using System.Collections.Generic;
using UnityEngine;

namespace XRAccelerator.Configs
{
    [CreateAssetMenu(fileName = "New Level Config", menuName = "Configs/Level", order = 0)]
    public class LevelConfig : ScriptableObject
    {
        public List<OrderConfig> PossibleOrders;
        public float LevelTime;
        public int MinActiveOrders;
        public int NewOrderEveryXSeconds;
    }
}