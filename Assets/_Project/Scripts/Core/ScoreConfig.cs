using UnityEngine;

namespace TimeAttackBlock
{
    [CreateAssetMenu(fileName = "ScoreConfig", menuName = "TimeAttackBlock/Configs/ScoreConfig")]
    public class ScoreConfig : ScriptableObject
    {
        [Header("���������{�[�i�X�i�s�����W���j")]
        // 1:1.0, 2:1.2, 3:1.5, 4:1.8, 5+:2.2
        public float[] lineComboMultiplier = new float[] { 0f, 1.0f, 1.2f, 1.5f, 1.8f, 2.2f };

        [Header("�`�F�C���{��")]
        [Tooltip("1�����p�����Ƃ� +15%")]
        public float chainStep = 0.15f;
        public float chainCap = 2.0f;
    }
}