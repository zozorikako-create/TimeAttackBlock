using System.Collections.Generic;
using UnityEngine;

namespace TimeAttackBlock
{
    [CreateAssetMenu(fileName = "BlockShapeDatabase", menuName = "TimeAttackBlock/Configs/BlockShapeDatabase")]
    public class BlockShapeDatabase : ScriptableObject
    {
        [System.Serializable]
        public class BlockShape
        {
            public string id;
            [Tooltip("幅×高さのビットマップ（true=ブロック有）")]
            public Vector2Int size;
            public List<bool> bitmap = new(); // size.x*size.y 個
        }

        [Header("12形状（Day1は空でOK、Day2で埋める）")]
        public List<BlockShape> shapes = new();
    }
}