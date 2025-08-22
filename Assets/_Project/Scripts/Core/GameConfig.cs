using UnityEngine;

namespace TimeAttackBlock
{
    [CreateAssetMenu(fileName = "GameConfig", menuName = "TimeAttackBlock/Configs/GameConfig")]
    public class GameConfig : ScriptableObject
    {
        [Header("時間設定")]
        public float initialTime = 100f;   // 初期時間
        public float maxTime = 100f;       // 上限時間
        public bool allowTimeExtension = false;

        [Header("時間回復（index=ライン数）")]
        // 0行は未使用、1: +2, 2: +5, 3: +9, 4: +14, 5+: +20
        public int[] lineTimeBonus = new int[] { 0, 2, 5, 9, 14, 20 };
    }
}