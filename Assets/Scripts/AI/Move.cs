using Chess2D.Piece;
using UnityEngine;

namespace Chess2D.AI
{
    public class Move
    {
        public Vector2Int from;
        public Vector2Int to;
        public ChessPiece movedPiece;
        public bool ContainsCapturablePiece;
    }
}