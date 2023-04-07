using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    [CreateAssetMenu(fileName = "New Station", menuName = "GOAP/Station")]
    public class StationSO : ScriptableObject
    {
        public string stationName;
        public string description;
        public StationType stationType;
        public int maxResources = 0;

        public enum StationType
        {
            Stove = 0,
            AppleTree = 1,
        }
    }
}
