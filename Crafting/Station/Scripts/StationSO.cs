using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GOAP
{
    [CreateAssetMenu(menuName = "GOAP/Crafting/Station")]
    public class StationSO : ScriptableObject
    {
        public string stationName;
        public List<StatEffect> statEffects;

        public GameObject StationPrefab;
    }
}
