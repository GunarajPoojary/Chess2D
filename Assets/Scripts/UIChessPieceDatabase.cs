using Chess2D.Piece;
using UnityEngine;

namespace Chess2D.ScriptableObjects
{
    [CreateAssetMenu(fileName = "UIChessPieceDatabase", menuName = "Custom/UI Chess Piece Database")]
    public class UIChessPieceDatabase : ChessPieceDatabase<UnityEngine.UI.Image> { }
}