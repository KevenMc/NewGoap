using UnityEngine;
using System.Collections.Generic;

namespace GOAP
{
    public enum AnimationBase
    {
        Single_Attack,
        Use_Item
    }

    public class AnimationClipOverrides : List<KeyValuePair<AnimationClip, AnimationClip>>
    {
        public AnimationClipOverrides(int capacity)
            : base(capacity) { }

        public AnimationClip this[string name]
        {
            get { return this.Find(x => x.Key.name.Equals(name)).Value; }
            set
            {
                int index = this.FindIndex(x => x.Key.name.Equals(name));
                if (index != -1)
                    this[index] = new KeyValuePair<AnimationClip, AnimationClip>(
                        this[index].Key,
                        value
                    );
            }
        }
    }

    [CreateAssetMenu(fileName = "New Animation Set", menuName = "GOAP/Animation Set")]
    public class AnimationSet : ScriptableObject
    {
        [System.Serializable]
        public struct AnimationOverride
        {
            public AnimationBase animationName;
            public AnimationClip clip;
        }

        public List<AnimationOverride> animationOverrides = new List<AnimationOverride>();
    }
}
