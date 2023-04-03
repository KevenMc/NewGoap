using UnityEngine;
using System.Collections.Generic;

namespace GOAP {
    
    [CreateAssetMenu(fileName = "New Item", menuName = "GOAP/Item")]
    public class ItemSO : ScriptableObject {
        
        [System.Serializable]
        public class StatEffect {
            public StatType statType;
            public float value;
        }

        public string itemName;
        public List<StatEffect> statEffects = new List<StatEffect>();
    }
}
