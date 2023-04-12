using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace GOAP
{
    [CreateAssetMenu(fileName = "New Item", menuName = "GOAP/Item")]
    public class ItemSO : ScriptableObject
    {
        [System.Serializable]
        public class StatEffect
        {
            public StatType statType;
            public float value;
        }

        public string itemName;
        public float itemUseCost;
        public GameObject itemPrefab;
        public AnimationClip useItemAnimation;
        public List<StatEffect> statEffects = new List<StatEffect>();
    }
}
