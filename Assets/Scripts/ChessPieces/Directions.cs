using UnityEngine;

namespace Chess2D.Piece
{
    public static class Directions
    {
        public static readonly Vector2Int Up = Vector2Int.up;
        public static readonly Vector2Int Down = Vector2Int.down;
        public static readonly Vector2Int Left = Vector2Int.left;
        public static readonly Vector2Int Right = Vector2Int.right;

        public static readonly Vector2Int UpLeft = Vector2Int.up + Vector2Int.left;
        public static readonly Vector2Int UpRight = Vector2Int.up + Vector2Int.right;
        public static readonly Vector2Int DownLeft = Vector2Int.down + Vector2Int.left;
        public static readonly Vector2Int DownRight = Vector2Int.down + Vector2Int.right;

        public static readonly Vector2Int[] Diagonals =
        {
            UpLeft, UpRight, DownLeft, DownRight
        };

        public static readonly Vector2Int[] Orthogonals =
        {
            Up, Down, Left, Right
        };

        public static readonly Vector2Int[] EightDirections =
        {
            UpLeft, UpRight, DownLeft, DownRight,
            Up, Down, Left, Right
        };

        public static readonly Vector2Int[] KnightMoves =
        {
            new(1, 2), new(2, 1),
            new(2, -1), new(1, -2),
            new(-1, -2), new(-2, -1),
            new(-2, 1), new(-1, 2)
        };

        public static readonly Vector2Int[] KingMoves = EightDirections;
    }
}