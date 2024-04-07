using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeState : StateMachineBehaviour
{
    [SerializeField]
    private string parameterName = "ChangeState";

    [SerializeField]
    private bool boolValue = false;

    public override void OnStateEnter(
        Animator animator,
        AnimatorStateInfo stateInfo,
        int layerIndex
        )
    {
        animator.SetBool(parameterName, boolValue);
    }
}
