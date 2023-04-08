using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveTo : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public GOAP.ActionPlanner actionPlanner;
    public GOAP.StatHandler statHandler;
    public GOAP.PlanHandler planHandler;
    public GOAP.ActionHandler actionHandler;

    // public void Location(Vector3 target)
    // {
    //     // navMeshAgent.SetDestination(target);
    // }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                //   Location(hit.point);
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Right click");
            actionPlanner.SetGoal(statHandler.currentGoals[0]);
        }
    }
}
