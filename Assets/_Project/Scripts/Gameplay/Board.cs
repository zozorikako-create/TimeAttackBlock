using UnityEngine;

namespace TimeAttackBlock.Gameplay
{
    public class Board : MonoBehaviour
    {
        // �Ֆʂ̑傫���i10x10�j
        private const int GridSize = 10;

        // ���������Ƀ}�X�����ĊǗ�����
        private GameObject[,] grid = new GameObject[GridSize, GridSize];

        /// <summary>
        /// �Ֆʂ���鏈���i���͒��g�Ȃ��j
        /// </summary>
        public void CreateGrid()
        {
            // TODO: Day3�ȍ~�Ń}�X�𐶐����鏈��������
            Debug.Log("CreateGrid() called - grid will be generated here.");
        }
    }
}