using UnityEngine;

namespace TimeAttackBlock.Gameplay
{
    public class Board : MonoBehaviour
    {
        // 盤面の大きさ（10x10）
        private const int GridSize = 10;

        // 将来ここにマスを入れて管理する
        private GameObject[,] grid = new GameObject[GridSize, GridSize];

        /// <summary>
        /// 盤面を作る処理（今は中身なし）
        /// </summary>
        public void CreateGrid()
        {
            // TODO: Day3以降でマスを生成する処理を書く
            Debug.Log("CreateGrid() called - grid will be generated here.");
        }
    }
}