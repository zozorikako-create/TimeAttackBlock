using UnityEngine;

namespace TimeAttackBlock
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "TimeAttackBlock/Configs/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [Header("���Ԑݒ�")]
        public float initialTime = 100f;   // ��������
        public float maxTime = 100f;       // �������
        public bool allowTimeExtension = false;

        [Header("���ԉ񕜁iindex=���C�����j")]
        // 0�s�͖��g�p�A1: +2, 2: +5, 3: +9, 4: +14, 5+: +20
        public int[] lineTimeBonus = new int[] { 0, 2, 5, 9, 14, 20 };
    }
}