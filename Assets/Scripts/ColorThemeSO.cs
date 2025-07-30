using UnityEngine;

namespace Chess2D
{
    [CreateAssetMenu(fileName = "ChessPieceTheme", menuName = "Custom/Chess Piece Theme")]
    public class ColorThemeSO : ScriptableObject
    {
        [Header("Tiles Color")]
        public Color lightTileColor;
        public Color darkTileColor;
        
        [Header("Light color pieces")]
        public Sprite lightPawn;
        public Sprite lightRook;
        public Sprite lightKnight;
        public Sprite lightBishop;
        public Sprite lightQueen;
        public Sprite lightKing;

        [Header("Dark color pieces")]
        public Sprite darkPawn;
        public Sprite darkRook;
        public Sprite darkKnight;
        public Sprite darkBishop;
        public Sprite darkQueen;
        public Sprite darkKing;
    }
}