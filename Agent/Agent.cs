using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace GOAP
{
    public class Agent : MonoBehaviour
    {
        public StatHandler statHandler;
        public Inventory inventory;
        public KnowledgeHandler knowledgeHandler;
        public string currentGoal;
        public float distanceToArrive = 1f;

        public StatHandler GetStatHandler()
        {
            if (statHandler == null)
            {
                statHandler = GetComponent<StatHandler>();
            }
            return statHandler;
        }

        public Stat GetCurrentStatGoal()
        {
            return GetStatHandler().currentGoals.FirstOrDefault();
        }
    }
}
