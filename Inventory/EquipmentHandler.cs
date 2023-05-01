using System;
using System.Collections.Generic;
using UnityEngine;

namespace GOAP
{
    public class EquipmentHandler : MonoBehaviour
    {
        public AnimationSet leftHandAnimSet;
        public AnimationSet rightHandAnimSet;
        public AnimationSet armourAnimSet;
        public Animator animator;
        protected AnimatorOverrideController animatorOverrideController;
        protected AnimationClipOverrides clipOverrides;

        public void Start()
        {
            animator = GetComponent<Animator>();

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
                ApplyOverrideAnimations(leftHandAnimSet);
            }
        }

        private void ApplyOverrideAnimations(AnimationSet animSet)
        {
            foreach (AnimationSet.AnimationOverride animationOverride in animSet.animationOverrides)
            {
                clipOverrides[animationOverride.animationName.ToString()] = animationOverride.clip;
            }
            animatorOverrideController.ApplyOverrides(clipOverrides);
        }

        private void ResetOverrideAnimations(AnimationSet animSet)
        {
            foreach (AnimationSet.AnimationOverride animationOverride in animSet.animationOverrides)
            {
                clipOverrides[animationOverride.animationName.ToString()] = null;
            }
            animatorOverrideController.ApplyOverrides(clipOverrides);
        }
    }
}
