using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    public class Item : MonoBehaviour
    {
        public Vector3 location;
        public ItemSO itemData;

        private void OnEnable()
        {
            location = transform.position;
        }
    }
}
