using UnityEngine;

namespace TimeAttackBlock.Gameplay
{
    /// <summary>
    /// 手札スロット（1枠分）。入れ替え時は古い表示を破棄。
    /// </summary>
    public class HandSlot : MonoBehaviour
    {
        public PieceView currentPiece;

        public bool IsEmpty => currentPiece == null;

        public void SetPiece(PieceView piece)
        {
            Clear();
            currentPiece = piece;
            if (piece != null)
            {
                piece.transform.SetParent(transform, false);
                piece.transform.localPosition = Vector3.zero;
            }
        }

        public void Clear()
        {
            if (currentPiece != null)
            {
                Destroy(currentPiece.gameObject);
                currentPiece = null;
            }
        }
    }
}