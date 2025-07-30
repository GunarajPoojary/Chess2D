using UnityEngine;

namespace Chess2D
{
    public class ChessPiecePlacementHandler : MonoBehaviour
    {
        [Header("Light color pieces")]
        [SerializeField] private ChessPieceView _lightPawn;
        [SerializeField] private ChessPieceView _lightRook;
        [SerializeField] private ChessPieceView _lightKnight;
        [SerializeField] private ChessPieceView _lightBishop;
        [SerializeField] private ChessPieceView _lightQueen;
        [SerializeField] private ChessPieceView _lightKing;

        [Header("Dark color pieces")]
        [SerializeField] private ChessPieceView _darkPawn;
        [SerializeField] private ChessPieceView _darkRook;
        [SerializeField] private ChessPieceView _darkKnight;
        [SerializeField] private ChessPieceView _darkBishop;
        [SerializeField] private ChessPieceView _darkQueen;
        [SerializeField] private ChessPieceView _darkKing;

        private Transform _lightPiecesContainer;
        private Transform _darkPiecesContainer;
        private readonly int _size = 8;

        private void Awake() => SetupPieces();

        private void SetupPieces()
        {
            _lightPiecesContainer = new GameObject("LightPieceContainer").transform;
            _darkPiecesContainer = new GameObject("DarkPieceContainer").transform;

            for (int col = 0; col < _size; col++)
            {
                PlacePiece(_lightPawn, col, 1, true);
                PlacePiece(_darkPawn, col, 6, false);
            }

            PlaceBackRow(true);
            PlaceBackRow(false);
        }

        private void PlaceBackRow(bool isLight)
        {
            int row = isLight ? 0 : 7;

            PlacePiece(isLight ? _lightRook : _darkRook, 0, row, isLight);
            PlacePiece(isLight ? _lightKnight : _darkKnight, 1, row, isLight);
            PlacePiece(isLight ? _lightBishop : _darkBishop, 2, row, isLight);
            PlacePiece(isLight ? _lightQueen : _darkQueen, 3, row, isLight);
            PlacePiece(isLight ? _lightKing : _darkKing, 4, row, isLight);
            PlacePiece(isLight ? _lightBishop : _darkBishop, 5, row, isLight);
            PlacePiece(isLight ? _lightKnight : _darkKnight, 6, row, isLight);
            PlacePiece(isLight ? _lightRook : _darkRook, 7, row, isLight);
        }

        private void PlacePiece(ChessPieceView prefab, int col, int row, bool isLight)
        {
            ChessPieceView piece = Instantiate(prefab, new Vector3(col, row, 0), Quaternion.identity, isLight ? _lightPiecesContainer : _darkPiecesContainer);
            string color = isLight ? "Light" : "Black";
            piece.transform.name = $"{color}_{piece}(C{col},R{row})";
        }
    }
}