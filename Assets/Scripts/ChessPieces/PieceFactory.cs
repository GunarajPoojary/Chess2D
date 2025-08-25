using UnityEngine;

namespace Chess2D.Piece
{
    public class PieceFactory<T> where T : Object
    {
        private readonly PieceSet<T> _pieceSet;

        public PieceFactory(PieceSet<T> pieceSet)
        {
            _pieceSet = pieceSet;
        }

        public T GetPiece(PieceType type)
        {
            T piece = null;

            switch (type)
            {
                case PieceType.Pawn: piece = _pieceSet.Pawn; break;
                case PieceType.Rook: piece = _pieceSet.Rook; break;
                case PieceType.Knight: piece = _pieceSet.Knight; break;
                case PieceType.Bishop: piece = _pieceSet.Bishop; break;
                case PieceType.Queen: piece = _pieceSet.Queen; break;
                case PieceType.King: piece = _pieceSet.King; break;
            }

            return Object.Instantiate(piece);
        }
    }
}