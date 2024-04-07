using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    public class SetAnimationState : MonoBehaviour
    {
        public Animator animator;

        // Start is called before the first frame update
        void Start()
        {
            animator.SetInteger("State", (int)AnimationState.Locomotion);
        }

        // Update is called once per frame
        void Update() { }
    }
}
