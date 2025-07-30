using System.Collections.Generic;
using Chess2D.Utilities;
using UnityEngine;

namespace Chess2D
{
    public class MoveStrategyFactory
    {
        public IMoveStrategy Create(PieceType type)
        {
            IMoveStrategy moveStrategy = null;
            
            switch (type)
            {
                case PieceType.Pawn:
                    moveStrategy = new PawnMovement();
                    break;
                case PieceType.Rook:
                    moveStrategy = new RookMovement();
                    break;
                case PieceType.Knight:
                    moveStrategy = new KnightMovement();
                    break;
                case PieceType.Bishop:
                    moveStrategy = new BishopMovement();
                    break;
                case PieceType.Queen:
                    moveStrategy = new QueenMovement();
                    break;
                case PieceType.King:
                    moveStrategy = new KingMovement();
                    break;
            }

            return moveStrategy;
        }
    }
    
    public interface IMoveStrategy
    {
        List<Move> GetLegalMoves(Vector2Int currentTile);
    }

    public struct Move
    {
        public Vector2Int from;
        public Vector2Int to;

        public Move(Vector2Int from, Vector2Int to)
        {
            this.from = from;
            this.to = to;
        }
    }

    public class QueenMovement : IMoveStrategy
    {
        private readonly IMoveStrategy _diagonalMultiStepMovement = new DiagonalMultiStepMovement();
        private readonly IMoveStrategy _orthogonalMultiStepMovement = new OrthogonalMultiStepMovement();

        public List<Move> GetLegalMoves(Vector2Int currentTile)
        {
            List<Move> moves = new();

            moves.AddRange(_diagonalMultiStepMovement.GetLegalMoves(currentTile));
            moves.AddRange(_orthogonalMultiStepMovement.GetLegalMoves(currentTile));

            return moves;
        }
    }

    public class PawnMovement : IMoveStrategy
    {
        private IMoveStrategy _movementStrategy;
        private readonly IMoveStrategy _singleForwardMovement = new SingleUpwardMovement();
        private readonly IMoveStrategy _doubleForwardMovement = new DoubleDownwardMovement();

        public PawnMovement()
        {
            _movementStrategy = _doubleForwardMovement;
        }

        public List<Move> GetLegalMoves(Vector2Int currentTile) => _movementStrategy.GetLegalMoves(currentTile);

        public void SwitchToSingleForwardMovement() => _movementStrategy = _singleForwardMovement;
    }

    public class KnightMovement : IMoveStrategy
    {
        private readonly IMoveStrategy _lShapedMovement = new LShapedMovement();

        public List<Move> GetLegalMoves(Vector2Int currentTile) => _lShapedMovement.GetLegalMoves(currentTile);
    }

    public class KingMovement : IMoveStrategy
    {
        private readonly IMoveStrategy _eightWaySingleStepMovement = new EightWaySingleStepMovement();

        public List<Move> GetLegalMoves(Vector2Int currentTile) => _eightWaySingleStepMovement.GetLegalMoves(currentTile);
    }

    public class RookMovement : IMoveStrategy
    {
        private readonly IMoveStrategy _orthogonalMultiStepMovement = new OrthogonalMultiStepMovement();

        public List<Move> GetLegalMoves(Vector2Int currentTile) => _orthogonalMultiStepMovement.GetLegalMoves(currentTile);
    }

    public class BishopMovement : IMoveStrategy
    {
        private readonly IMoveStrategy _diagonalMultiStepMovement = new DiagonalMultiStepMovement();

        public List<Move> GetLegalMoves(Vector2Int currentTile) => _diagonalMultiStepMovement.GetLegalMoves(currentTile);
    }

    /// <summary>
    /// Moves one tile in any direction (King-style)
    /// </summary>
    public class EightWaySingleStepMovement : IMoveStrategy
    {
        private static readonly Vector2Int[] directions =
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right,
            Vector2Int.up + Vector2Int.left, Vector2Int.up + Vector2Int.right,
            Vector2Int.down + Vector2Int.left, Vector2Int.down + Vector2Int.right
        };

        public List<Move> GetLegalMoves(Vector2Int currentTile)
        {
            List<Move> moves = new();

            foreach (Vector2Int dir in directions)
            {
                Vector2Int to = currentTile + dir;

                if (BoardUtilities.IsWithinBoard(to))
                    moves.Add(new Move(currentTile, to));
            }
            return moves;
        }
    }

    /// <summary>
    /// Moves multiple tiles in straight lines (Rook-style)
    /// </summary>
    public class OrthogonalMultiStepMovement : IMoveStrategy
    {
        private static readonly Vector2Int[] directions =
        {
            Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right
        };

        public List<Move> GetLegalMoves(Vector2Int currentTile)
        {
            List<Move> moves = new();
            foreach (Vector2Int dir in directions)
            {
                Vector2Int to = currentTile + dir;
                while (BoardUtilities.IsWithinBoard(to))
                {
                    moves.Add(new Move(currentTile, to));
                    to += dir;
                }
            }
            return moves;
        }
    }

    /// <summary>
    /// Moves multiple tiles diagonally (Bishop-style)
    /// </summary>
    public class DiagonalMultiStepMovement : IMoveStrategy
    {
        private static readonly Vector2Int[] directions =
        {
            Vector2Int.up + Vector2Int.left,
            Vector2Int.up + Vector2Int.right,
            Vector2Int.down + Vector2Int.left,
            Vector2Int.down + Vector2Int.right
        };

        public List<Move> GetLegalMoves(Vector2Int currentTile)
        {
            List<Move> moves = new();
            foreach (Vector2Int dir in directions)
            {
                Vector2Int to = currentTile + dir;

                while (BoardUtilities.IsWithinBoard(to))
                {
                    moves.Add(new Move(currentTile, to));
                    to += dir;
                }
            }
            return moves;
        }
    }

    /// <summary>
    /// Moves in an L-shape (Knight-style)
    /// </summary>
    public class LShapedMovement : IMoveStrategy
    {
        private static readonly Vector2Int[] jumps =
        {
            new Vector2Int(1, 2), new Vector2Int(2, 1),
            new Vector2Int(2, -1), new Vector2Int(1, -2),
            new Vector2Int(-1, -2), new Vector2Int(-2, -1),
            new Vector2Int(-2, 1), new Vector2Int(-1, 2)
        };

        public List<Move> GetLegalMoves(Vector2Int currentTile)
        {
            List<Move> moves = new();

            foreach (Vector2Int jump in jumps)
            {
                Vector2Int to = currentTile + jump;

                if (BoardUtilities.IsWithinBoard(to))
                    moves.Add(new Move(currentTile, to));
            }
            return moves;
        }
    }

    /// <summary>
    /// Moves one tile upward
    /// </summary>
    public class SingleUpwardMovement : IMoveStrategy
    {
        private readonly Vector2Int _upward = Vector2Int.up;

        public List<Move> GetLegalMoves(Vector2Int currentTile)
        {
            List<Move> moves = new();

            Vector2Int to = currentTile + _upward;

            if (BoardUtilities.IsWithinBoard(to))
                moves.Add(new Move(currentTile, to));

            return moves;
        }
    }

    /// <summary>
    /// Moves one tile downward
    /// </summary>
    public class SingleDownwardMovement : IMoveStrategy
    {
        private readonly Vector2Int _downward = Vector2Int.down;

        public List<Move> GetLegalMoves(Vector2Int currentTile)
        {
            List<Move> moves = new();

            Vector2Int to = currentTile + _downward;

            if (BoardUtilities.IsWithinBoard(to))
                moves.Add(new Move(currentTile, to));

            return moves;
        }
    }

    /// <summary>
    /// Moves two tiles upward
    /// </summary>
    public class DoubleUpwardMovement : IMoveStrategy
    {
        private readonly Vector2Int _upward = Vector2Int.up;

        public List<Move> GetLegalMoves(Vector2Int currentTile)
        {
            List<Move> moves = new();

            Vector2Int to = currentTile + _upward * 2;

            if (BoardUtilities.IsWithinBoard(to))
                moves.Add(new Move(currentTile, to));

            return moves;
        }
    }

    /// <summary>
    /// Moves two tiles downward
    /// </summary>
    public class DoubleDownwardMovement : IMoveStrategy
    {
        private readonly Vector2Int _upward = Vector2Int.down;

        public List<Move> GetLegalMoves(Vector2Int currentTile)
        {
            List<Move> moves = new();

            Vector2Int to = currentTile + _upward * 2;

            if (BoardUtilities.IsWithinBoard(to))
                moves.Add(new Move(currentTile, to));

            return moves;
        }
    }

    /// <summary>
    /// Diagonal capture for pawns
    /// </summary>
    public class DiagonalSingleStepUpwardCaptureMovement : IMoveStrategy
    {
        private readonly Vector2Int[] _diagonals = new[] { Vector2Int.up + Vector2Int.left, Vector2Int.up + Vector2Int.right };

        public List<Move> GetLegalMoves(Vector2Int currentTile)
        {
            List<Move> moves = new();

            foreach (Vector2Int dir in _diagonals)
            {
                Vector2Int to = currentTile + dir;

                if (BoardUtilities.IsWithinBoard(to))
                    moves.Add(new Move(currentTile, to));
            }

            return moves;
        }

        /// <summary>
        /// Diagonal capture for pawns
        /// </summary>
        public class DiagonalSingleStepDownwardCaptureMovement : IMoveStrategy
        {
            private readonly Vector2Int[] _diagonals = new[] { Vector2Int.down + Vector2Int.left, Vector2Int.down + Vector2Int.right };

            public List<Move> GetLegalMoves(Vector2Int currentTile)
            {
                List<Move> moves = new();

                foreach (Vector2Int dir in _diagonals)
                {
                    Vector2Int to = currentTile + dir;

                    if (BoardUtilities.IsWithinBoard(to))
                        moves.Add(new Move(currentTile, to));
                }

                return moves;
            }
        }
    }
}