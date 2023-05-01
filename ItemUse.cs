using System;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    public class ItemUse : MonoBehaviour
    {
        public Weapon[] weapons;
        public AnimationSet animationSet;
        public Animator animator;

        [System.Serializable]
        public struct Weapon
        {
            public AnimationClip singleAttack;
            public AnimationBase animationBase;
        }

        protected AnimatorOverrideController animatorOverrideController;
        protected int weaponIndex;

        protected AnimationClipOverrides clipOverrides;

        public void Start()
        {
            animator = GetComponent<Animator>();
            weaponIndex = 0;

            animatorOverrideController = new AnimatorOverrideController(
                animator.runtimeAnimatorController
            );
            animator.runtimeAnimatorController = animatorOverrideController;

            clipOverrides = new AnimationClipOverrides(animatorOverrideController.overridesCount);
            animatorOverrideController.GetOverrides(clipOverrides);
        }

        public void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                weaponIndex = (weaponIndex + 1) % weapons.Length;
                foreach (
                    AnimationSet.AnimationOverride animationOverride in animationSet.animationOverrides
                )
                {
                    clipOverrides[animationOverride.animationName.ToString()] =
                        animationOverride.clip;
                }
                animatorOverrideController.ApplyOverrides(clipOverrides);
                // animator.SetTrigger("SingleAttack");/
            }
        }
    }
}
