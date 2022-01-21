
using UnityEngine;

namespace com.SolClovser.StateTree
{
    [System.Serializable]
    public class AnimationTransitionSettings
    {
        [Range(0, 1f)]
        public float fixedTransitionDuration = 0.25f;
        [Range(0, 10f)] 
        public float fixedTimeOffset = 0;
    }
}