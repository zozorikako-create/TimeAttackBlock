using UnityEngine;

namespace TimeAttackBlock.Gameplay
{
    /// <summary>
    /// ��D�X���b�g�i1�g���j�B����ւ����͌Â��\����j���B
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