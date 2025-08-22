using UnityEngine;

namespace TimeAttackBlock
{
    [CreateAssetMenu(fileName = "ScoreConfig", menuName = "TimeAttackBlock/Configs/ScoreConfig")]
    public class ScoreConfig : ScriptableObject
    {
        [Header("同時消しボーナス（行数→係数）")]
        // 1:1.0, 2:1.2, 3:1.5, 4:1.8, 5+:2.2
        public float[] lineComboMultiplier = new float[] { 0f, 1.0f, 1.2f, 1.5f, 1.8f, 2.2f };

        [Header("チェイン倍率")]
        [Tooltip("1消し継続ごとに +15%")]
        public float chainStep = 0.15f;
        public float chainCap = 2.0f;
    }
}