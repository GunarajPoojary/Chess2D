using UnityEngine;

public class ChessPiece : MonoBehaviour
{
    public enum PieceType
    {
        Pawn,
        King,
        Queen,
        Knight,
        Bishop,
        Rook
    }

    public enum PieceColor
    {
        White,
        Black
    }

    [SerializeField] PieceColor color;
    public PieceColor pieceColor { get { return color; } }

    [SerializeField] PieceType type;
    public PieceType pieceType { get { return type; } }
}
