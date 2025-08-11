using Chess2D.Piece;
using UnityEngine;

namespace Chess2D.ScriptableObjects
{
    [CreateAssetMenu(fileName = "PieceDatabase", menuName = "Custom/Piece Database")]
    public class PieceRendererDatabase : ScriptableObject
    {
        [field: SerializeField] public PieceRenderer Pawn { get; private set; }
        [field: SerializeField] public PieceRenderer Rook { get; private set; }
        [field: SerializeField] public PieceRenderer Knight { get; private set; }
        [field: SerializeField] public PieceRenderer Bishop { get; private set; }
        [field: SerializeField] public PieceRenderer Queen { get; private set; }
        [field: SerializeField] public PieceRenderer King { get; private set; }
    }
}