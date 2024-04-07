using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    public class Station : MonoBehaviour
    {
        public StationSO stationData;
        public Vector3 location;

        private void OnEnable()
        {
            location = transform.position;
        }
    }
}
