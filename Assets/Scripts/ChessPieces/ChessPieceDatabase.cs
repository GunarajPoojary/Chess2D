using UnityEngine;

namespace Chess2D.Piece
{
    public class ChessPieceDatabase<T> : ScriptableObject where T : Object
    {
        [field: SerializeField] public PieceSet<T> WhitePieceSet { get; private set; }
        [field: SerializeField] public PieceSet<T> BlackPieceSet { get; private set; }
    }
}