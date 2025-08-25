using UnityEngine;

namespace Chess2D.Piece
{
    [System.Serializable]
    public class PieceSet<T> where T : Object
    {
        public T Pawn;
        public T Rook;
        public T Knight;
        public T Bishop;
        public T Queen;
        public T King;
    }
}