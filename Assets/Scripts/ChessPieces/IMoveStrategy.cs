using System;
using Chess2D.Board;
using Chess2D.Board.Utilities;
using UnityEngine;

namespace Chess2D.Piece
{
    public interface IMoveStrategy
    {
        void CalculateLegalMoves(
            bool isPlayerPieceSelected,
            Vector2Int currentTile,
            Action<Vector2Int> onGetLegalMoveAction);
    }

    public interface IMoveStrategySwitch
    {
        void SwitchStrategy();
    }

    public class PawnMove : IMoveStrategy, IMoveStrategySwitch
    {
        private readonly IMoveStrategy _nonCaptureSingleStepMoveStrategy;
        private readonly IMoveStrategy _nonCaptureDoubleStepMoveStrategy;
        private readonly IMoveStrategy _captureStrategy;
        private bool _hasMoved = false;
        private IMoveStrategy _nonCaptureMoveStrategy;

        public PawnMove(bool isPlayer, IBoardUtility boardUtility)
        {
            _nonCaptureDoubleStepMoveStrategy = isPlayer
                ? new DoubleStepMove(boardUtility, Directions.Up)
                : new DoubleStepMove(boardUtility, Directions.Down);

            _nonCaptureSingleStepMoveStrategy = isPlayer
                ? new SingleStepMove(boardUtility, Directions.Up)
                : new SingleStepMove(boardUtility, Directions.Down);

            _nonCaptureMoveStrategy = _nonCaptureDoubleStepMoveStrategy;

            _captureStrategy = isPlayer
                ? new DiagonalCaptureMove(boardUtility, new[] { Directions.UpLeft, Directions.UpRight })
                : new DiagonalCaptureMove(boardUtility, new[] { Directions.DownLeft, Directions.DownRight });
        }

        public void CalculateLegalMoves(bool isPlayerPieceSelected, Vector2Int currentTile, Action<Vector2Int> onGetLegalMoveAction)
        {
            _nonCaptureMoveStrategy.CalculateLegalMoves(isPlayerPieceSelected, currentTile, onGetLegalMoveAction);
            _captureStrategy.CalculateLegalMoves(isPlayerPieceSelected, currentTile, onGetLegalMoveAction);
        }

        public void SwitchStrategy()
        {
            if (!_hasMoved)
            {
                _hasMoved = true;
                _nonCaptureMoveStrategy = _nonCaptureSingleStepMoveStrategy;
            }
        }
    }

    public class QueenMove : IMoveStrategy
    {
        private readonly IMoveStrategy _eightDirectionalMultiStepMovement;

        public QueenMove(IBoardUtility boardUtility)
        {
            _eightDirectionalMultiStepMovement = new MultiStepMove(boardUtility, Directions.EightDirections);
        }

        public void CalculateLegalMoves(bool isPlayerPieceSelected, Vector2Int currentTile, Action<Vector2Int> onGetLegalMoveAction) =>
            _eightDirectionalMultiStepMovement.CalculateLegalMoves(isPlayerPieceSelected, currentTile, onGetLegalMoveAction);
    }

    public class KnightMove : IMoveStrategy
    {
        private readonly IBoardUtility _boardUtility;

        public KnightMove(IBoardUtility boardUtility)
        {
            _boardUtility = boardUtility;
        }

        public void CalculateLegalMoves(bool isPlayerPieceSelected, Vector2Int currentTile, Action<Vector2Int> onGetLegalMoveAction)
        {
            foreach (Vector2Int jump in Directions.KnightMoves)
            {
                Vector2Int to = currentTile + jump;

                if (BoardUtilities.IsWithinBoard(to) && !_boardUtility.ContainsAllyPieceAt(to, isPlayerPieceSelected))
                    onGetLegalMoveAction?.Invoke(to);
            }
        }
    }

    public class KingMove : IMoveStrategy
    {
        private readonly IBoardUtility _boardUtility;

        public KingMove(IBoardUtility boardUtility)
        {
            _boardUtility = boardUtility;
        }

        public void CalculateLegalMoves(bool isPlayerPieceSelected, Vector2Int currentTile, Action<Vector2Int> onGetLegalMoveAction)
        {
            foreach (Vector2Int dir in Directions.KingMoves)
            {
                Vector2Int to = currentTile + dir;

                if (BoardUtilities.IsWithinBoard(to) && !_boardUtility.ContainsAllyPieceAt(to, isPlayerPieceSelected))
                    onGetLegalMoveAction?.Invoke(to);
            }
        }
    }

    public class RookMove : IMoveStrategy
    {
        private readonly IMoveStrategy _orthogonalMultiStepMovement;

        public RookMove(IBoardUtility boardUtility)
        {
            _orthogonalMultiStepMovement = new MultiStepMove(boardUtility, Directions.Orthogonals);
        }

        public void CalculateLegalMoves(bool isPlayerPieceSelected, Vector2Int currentTile, Action<Vector2Int> onGetLegalMoveAction) =>
            _orthogonalMultiStepMovement.CalculateLegalMoves(isPlayerPieceSelected, currentTile, onGetLegalMoveAction);
    }

    public class BishopMove : IMoveStrategy
    {
        private readonly IMoveStrategy _diagonalMultiStepMovement;

        public BishopMove(IBoardUtility boardUtility)
        {
            _diagonalMultiStepMovement = new MultiStepMove(boardUtility, Directions.Diagonals);
        }

        public void CalculateLegalMoves(bool isPlayerPieceSelected, Vector2Int currentTile, Action<Vector2Int> onGetLegalMoveAction) =>
            _diagonalMultiStepMovement.CalculateLegalMoves(isPlayerPieceSelected, currentTile, onGetLegalMoveAction);
    }

    public class MultiStepMove : IMoveStrategy
    {
        private readonly IBoardUtility _boardUtility;
        private readonly Vector2Int[] _directions;

        public MultiStepMove(IBoardUtility boardUtility, Vector2Int[] directions)
        {
            _boardUtility = boardUtility;
            _directions = directions;
        }

        public void CalculateLegalMoves(bool isPlayerPieceSelected, Vector2Int currentTile, Action<Vector2Int> onGetLegalMoveAction)
        {
            foreach (Vector2Int dir in _directions)
            {
                Vector2Int to = currentTile + dir;

                while (BoardUtilities.IsWithinBoard(to))
                {
                    if (_boardUtility.IsTileEmptyAt(to))
                    {
                        onGetLegalMoveAction?.Invoke(to);
                        to += dir;
                    }
                    else if (_boardUtility.ContainsOpponentPieceAt(to, isPlayerPieceSelected))
                    {
                        onGetLegalMoveAction?.Invoke(to);
                        break;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }
    }

    public class SingleStepMove : IMoveStrategy
    {
        private readonly IBoardUtility _boardUtility;
        private readonly Vector2Int _direction;

        public SingleStepMove(IBoardUtility boardUtility, Vector2Int direction)
        {
            _boardUtility = boardUtility;
            _direction = direction;
        }

        public void CalculateLegalMoves(bool isPlayerPieceSelected, Vector2Int currentTile, Action<Vector2Int> onGetLegalMoveAction)
        {
            Vector2Int to = currentTile + _direction;

            if (BoardUtilities.IsWithinBoard(to) && _boardUtility.IsTileEmptyAt(to))
                onGetLegalMoveAction?.Invoke(to);
        }
    }

    public class DoubleStepMove : IMoveStrategy
    {
        private readonly IBoardUtility _boardUtility;
        private readonly Vector2Int _direction;

        public DoubleStepMove(IBoardUtility boardUtility, Vector2Int direction)
        {
            _boardUtility = boardUtility;
            _direction = direction;
        }

        public void CalculateLegalMoves(bool isPlayerPieceSelected, Vector2Int currentTile, Action<Vector2Int> onGetLegalMoveAction)
        {
            Vector2Int oneStep = currentTile + _direction;
            Vector2Int twoStep = currentTile + _direction * 2;

            if (BoardUtilities.IsWithinBoard(oneStep) && _boardUtility.IsTileEmptyAt(oneStep))
            {
                onGetLegalMoveAction?.Invoke(oneStep);

                if (BoardUtilities.IsWithinBoard(twoStep) && _boardUtility.IsTileEmptyAt(twoStep))
                    onGetLegalMoveAction?.Invoke(twoStep);
            }
        }
    }

    public class DiagonalCaptureMove : IMoveStrategy
    {
        private readonly IBoardUtility _boardUtility;
        private readonly Vector2Int[] _captureDirections;

        public DiagonalCaptureMove(IBoardUtility boardUtility, Vector2Int[] captureDirections)
        {
            _boardUtility = boardUtility;
            _captureDirections = captureDirections;
        }

        public void CalculateLegalMoves(bool isPlayerPieceSelected, Vector2Int currentTile, Action<Vector2Int> onGetLegalMoveAction)
        {
            foreach (var dir in _captureDirections)
            {
                Vector2Int to = currentTile + dir;
                if (BoardUtilities.IsWithinBoard(to) &&
                    !_boardUtility.IsTileEmptyAt(to) &&
                    _boardUtility.ContainsOpponentPieceAt(to, isPlayerPieceSelected))
                {
                    onGetLegalMoveAction?.Invoke(to);
                }
            }
        }
    }
}