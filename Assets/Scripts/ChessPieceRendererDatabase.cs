using Chess2D.Piece;
using UnityEngine;

namespace Chess2D.ScriptableObjects
{
    [CreateAssetMenu(fileName = "ChessPieceDatabase", menuName = "Custom/ChessPiece Database")]
    public class ChessPieceRendererDatabase : ChessPieceDatabase<PieceRenderer> { }
}