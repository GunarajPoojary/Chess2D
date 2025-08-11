using Chess2D.Piece;
using UnityEngine;

namespace Chess2D.Board
{
    public interface IBoardStateAccess
    {
        bool ContainsOpponentPieceAt(Vector2Int boardPosition, bool isPlayerPieceSelected);
        void SetOccupiedPieceAt(ChessPiece occupiedPiece, Vector2Int boardPosition);
    }

    public interface IBoardReader : IBoardStateAccess
    {
        bool TryGetOccupiedPieceAt(Vector2Int boardPosition, out ChessPiece occupiedPiece);
        ChessPiece GetOccupiedPieceAt(Vector2Int boardPosition);
    }

    public interface IBoardController : IBoardStateAccess
    {
        bool TryGetPlayerPieceAt(Vector2Int boardPosition, out ChessPiece playerPiece);
        bool IsMoveValidAt(Vector2Int boardPosition);
        void MarkMoveValidAt(Vector2Int boardPosition);
        void UnMarkMoveValidAt(Vector2Int boardPosition);
    }

    public class ChessBoard : IBoardUtility, IBoardController, IBoardReader
    {
        private const int BOARD_SIZE = 8;
        private readonly BoardData _boardData;
        private readonly BoardRenderer _boardRenderer;
        private readonly ChessPiece[,] _pieceGrid = new ChessPiece[BOARD_SIZE, BOARD_SIZE];

        public ChessBoard(GameObject tilePrefab, Color darkTileColor, Color lightTileColor, Transform boardTransform)
        {
            _boardData = new BoardData(BOARD_SIZE);

            _boardRenderer = new BoardRenderer(
                BOARD_SIZE,
                darkTileColor,
                lightTileColor,
                tilePrefab,
                boardTransform);
        }

        public void InitializePieces(bool isPlayerDarkColored, PieceFactory pieceFactory, Transform playerPieceTransform, Transform aiPieceTransform)
        {
            Color playerColor = isPlayerDarkColored ? Color.blue : Color.white;
            Color aiColor = isPlayerDarkColored ? Color.white : Color.blue;

            int playerBackRow = 0;
            int playerFrontRow = 1;
            int aiBackRow = 7;
            int aiFrontRow = 6;

            PieceType[] backRowSetup =
            {
                PieceType.Rook, PieceType.Knight, PieceType.Bishop,
                PieceType.Queen, PieceType.King, PieceType.Bishop,
                PieceType.Knight, PieceType.Rook
            };

            for (int col = 0; col < BOARD_SIZE; col++)
            {
                // Pawns
                CreateAndPlacePiece(pieceFactory, new PieceData(PieceType.Pawn, true), new Vector2Int(col, playerFrontRow), playerPieceTransform, playerColor);
                CreateAndPlacePiece(pieceFactory, new PieceData(PieceType.Pawn, false), new Vector2Int(col, aiFrontRow), aiPieceTransform, aiColor);

                // Back row pieces
                CreateAndPlacePiece(pieceFactory, new PieceData(backRowSetup[col], true), new Vector2Int(col, playerBackRow), playerPieceTransform, playerColor);
                CreateAndPlacePiece(pieceFactory, new PieceData(backRowSetup[col], false), new Vector2Int(col, aiBackRow), aiPieceTransform, aiColor);
            }
        }

        private void CreateAndPlacePiece(PieceFactory pieceFactory, PieceData pieceData, Vector2Int gridIndex, Transform container, Color color)
        {
            ChessPiece piece = pieceFactory.CreatePiece(pieceData);
            piece.SetPiecePosition(gridIndex);
            piece.Transform.SetParent(container);
            piece.SetPieceColor(color);
            _pieceGrid[gridIndex.y, gridIndex.x] = piece;
        }

        public bool TryGetPlayerPieceAt(Vector2Int boardPosition, out ChessPiece playerPiece)
        {
            playerPiece = _pieceGrid[boardPosition.y, boardPosition.x];

            return playerPiece != null && playerPiece.IsPlayer == true;
        }

        #region IBoardUtility Methods
        public bool IsTileEmptyAt(Vector2Int boardPosition) => _pieceGrid[boardPosition.y, boardPosition.x] == null;
        public bool ContainsOpponentPieceAt(Vector2Int boardPosition, bool isPlayerPieceSelected) => !IsTileEmptyAt(boardPosition) && _pieceGrid[boardPosition.y, boardPosition.x].IsPlayer ^ isPlayerPieceSelected;

        public bool ContainsAllyPieceAt(Vector2Int boardPosition, bool isPlayerPieceSelected) => !IsTileEmptyAt(boardPosition) && _pieceGrid[boardPosition.y, boardPosition.x].IsPlayer == isPlayerPieceSelected;
        #endregion

        public void SetBoardTheme(Color darkTileColor, Color lightTileColor) => _boardRenderer.SetBoardTheme(darkTileColor, lightTileColor);

        public void SetOccupiedPieceAt(ChessPiece occupiedPiece, Vector2Int boardPosition) => _pieceGrid[boardPosition.y, boardPosition.x] = occupiedPiece;

        public bool IsMoveValidAt(Vector2Int boardPosition) => _boardData.IsMoveValidAt(boardPosition);
        public void MarkMoveValidAt(Vector2Int boardPosition) => _boardData.MarkMoveValidAt(boardPosition);
        public void UnMarkMoveValidAt(Vector2Int boardPosition) => _boardData.UnMarkMoveValidAt(boardPosition);

        public bool TryCapturePieceAt(Vector2Int boardPosition, bool isPlayerPieceSelected, out ChessPiece capturedPiece)
        {
            if (ContainsOpponentPieceAt(boardPosition, isPlayerPieceSelected))
            {
                capturedPiece = _pieceGrid[boardPosition.y, boardPosition.x];

                if (capturedPiece != null)
                {
                    capturedPiece.SetInActive();
                    SetOccupiedPieceAt(null, boardPosition);
                }

                return true;
            }

            capturedPiece = null;

            return false;
        }

        public bool TryGetOccupiedPieceAt(Vector2Int boardPosition, out ChessPiece occupiedPiece)
        {
            occupiedPiece = _pieceGrid[boardPosition.y, boardPosition.x];

            return occupiedPiece != null;
        }

        public ChessPiece GetOccupiedPieceAt(Vector2Int boardPosition) => _pieceGrid[boardPosition.y, boardPosition.x];
    }
}