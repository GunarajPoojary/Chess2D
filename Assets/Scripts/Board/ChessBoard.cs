using Chess2D.Events;
using Chess2D.Piece;
using UnityEngine;

namespace Chess2D.Board
{
    public interface IBoardQuery
    {
        bool IsTileEmptyAt(Vector2Int boardPosition);
        bool ContainsAllyPieceAt(Vector2Int boardPosition, bool isPlayer);
        bool ContainsOpponentPieceAt(Vector2Int boardPosition, bool isPlayer);
        bool TryGetOccupiedPieceAt(Vector2Int boardPosition, out ChessPiece occupiedPiece);
        ChessPiece GetOccupiedPieceAt(Vector2Int boardPosition);
        bool TryGetPlayerPieceAt(Vector2Int boardPosition, out ChessPiece playerPiece);
    }

    public interface IBoardCommand
    {
        void SetOccupiedPieceAt(ChessPiece occupiedPiece, Vector2Int boardPosition);
        bool TryCapturePieceAt(Vector2Int boardPosition, bool isPlayerPieceSelected, out ChessPiece capturedPiece);
    }

    public interface IBoard : IBoardQuery, IBoardCommand { }

    public class ChessBoard : IBoard
    {
        private const int BOARD_SIZE = 8;
        private readonly ChessPiece[,] _pieceGrid = new ChessPiece[BOARD_SIZE, BOARD_SIZE];
        private readonly EventChannel<ChessPiece> _onIntializeChessPieceEvent = default;

        public ChessBoard(EventChannel<ChessPiece> onIntializeChessPieceEvent)
        {
            _onIntializeChessPieceEvent = onIntializeChessPieceEvent;
        }

        public void InitializeBoard(MoveStrategyFactory moveStrategyFactory, PieceFactory<PieceRenderer> playerPieceFactory, PieceFactory<PieceRenderer> aiPieceFactory, Transform playerPieceTransform, Transform aiPieceTransform)
        {
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
                CreateAndPlacePiece(moveStrategyFactory, playerPieceFactory, new PieceData(PieceType.Pawn, true), new Vector2Int(col, playerFrontRow), playerPieceTransform);
                CreateAndPlacePiece(moveStrategyFactory, aiPieceFactory, new PieceData(PieceType.Pawn, false), new Vector2Int(col, aiFrontRow), aiPieceTransform);

                // Back row pieces
                CreateAndPlacePiece(moveStrategyFactory, playerPieceFactory, new PieceData(backRowSetup[col], true), new Vector2Int(col, playerBackRow), playerPieceTransform);
                CreateAndPlacePiece(moveStrategyFactory, aiPieceFactory, new PieceData(backRowSetup[col], false), new Vector2Int(col, aiBackRow), aiPieceTransform);
            }
        }

        private void CreateAndPlacePiece(MoveStrategyFactory moveStrategyFactory, PieceFactory<PieceRenderer> pieceFactory, PieceData pieceData, Vector2Int gridIndex, Transform container)
        {
            PieceRenderer pieceRenderer = pieceFactory.GetPiece(pieceData.Type);

            ChessPiece piece = new(moveStrategyFactory.GetPieceMoveStrategy(pieceData), pieceData, pieceRenderer);

            piece.SetPiecePosition(gridIndex);
            piece.Transform.SetParent(container);

            _pieceGrid[gridIndex.y, gridIndex.x] = piece;

            _onIntializeChessPieceEvent.RaiseEvent(piece);
        }

        public bool TryGetPlayerPieceAt(Vector2Int boardPosition, out ChessPiece playerPiece)
        {
            playerPiece = _pieceGrid[boardPosition.y, boardPosition.x];

            return playerPiece != null && playerPiece.IsPlayer == true;
        }

        public bool IsTileEmptyAt(Vector2Int boardPosition) => _pieceGrid[boardPosition.y, boardPosition.x] == null;
        public bool ContainsOpponentPieceAt(Vector2Int boardPosition, bool isPlayerPieceSelected) => !IsTileEmptyAt(boardPosition) && _pieceGrid[boardPosition.y, boardPosition.x].IsPlayer ^ isPlayerPieceSelected;

        public bool ContainsAllyPieceAt(Vector2Int boardPosition, bool isPlayerPieceSelected) => !IsTileEmptyAt(boardPosition) && _pieceGrid[boardPosition.y, boardPosition.x].IsPlayer == isPlayerPieceSelected;

        public void SetOccupiedPieceAt(ChessPiece occupiedPiece, Vector2Int boardPosition) => _pieceGrid[boardPosition.y, boardPosition.x] = occupiedPiece;

        public bool TryCapturePieceAt(Vector2Int boardPosition, bool isPlayerPieceSelected, out ChessPiece capturedPiece)
        {
            capturedPiece = null;

            if (ContainsOpponentPieceAt(boardPosition, isPlayerPieceSelected))
            {
                capturedPiece = _pieceGrid[boardPosition.y, boardPosition.x];

                CapturePieceAt(boardPosition, capturedPiece);
            }

            return capturedPiece != null;
        }

        private void CapturePieceAt(Vector2Int boardPosition, ChessPiece chessPiece)
        {
            if (chessPiece != null)
            {
                chessPiece.SetInActive();
                SetOccupiedPieceAt(null, boardPosition);
            }
        }

        public bool TryGetOccupiedPieceAt(Vector2Int boardPosition, out ChessPiece occupiedPiece)
        {
            occupiedPiece = _pieceGrid[boardPosition.y, boardPosition.x];

            return occupiedPiece != null;
        }

        public ChessPiece GetOccupiedPieceAt(Vector2Int boardPosition) => _pieceGrid[boardPosition.y, boardPosition.x];
    }
}