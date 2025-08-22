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
            [Tooltip("���~�����̃r�b�g�}�b�v�itrue=�u���b�N�L�j")]
            public Vector2Int size;
            public List<bool> bitmap = new(); // size.x*size.y ��
        }

        [Header("12�`��iDay1�͋��OK�ADay2�Ŗ��߂�j")]
        public List<BlockShape> shapes = new();
    }
}