using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    public class RelationshipHandler : MonoBehaviour
    {
        public List<Agent> knownAgents = new List<Agent>();

        public List<Agent> ReturnDelegateAgents(Stat goal)
        {
            return knownAgents;
        }
    }
}
