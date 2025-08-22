using UnityEngine;

namespace TimeAttackBlock
{
    [CreateAssetMenu(fileName = "MustFitConfig", menuName = "TimeAttackBlock/Configs/MustFitConfig")]
    public class MustFitConfig : ScriptableObject
    {
        [Header("Must-Fit”­“®ğŒ")]
        public float activationTimeThreshold = 30f;
        public bool requireAllUnplaceable = false;
        public int maxActivationsPerGame = 999;
        [Range(0f, 1f)] public float activationProbability = 1.0f;
    }
}