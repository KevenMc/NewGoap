using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveTo : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public GOAP.ActionPlanner actionPlanner;
    public GOAP.StatHandler statHandler;
    public GOAP.ActionHandler actionHandler;

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
    }
}
