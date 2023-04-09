using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GOAP
{
    public class MovementHandler : MonoBehaviour
    {
        public NavMeshAgent navMeshAgent;
        private Vector3 target;
        public Agent agent;

        public void Init() { }

        public void MoveTo(Vector3 location)
        {
            target = location;
            navMeshAgent.SetDestination(target);
        }

        public bool HasArrivedAtLocation()
        {
            float distance = Vector3.Distance(transform.position, target);

            if (distance < agent.distanceToArrive)
            {
                navMeshAgent.SetDestination(transform.position);
                return true;
            }
            return false;
        }
    }
}
