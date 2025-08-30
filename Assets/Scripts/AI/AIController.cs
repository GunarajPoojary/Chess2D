using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Chess2D.Events;
using Chess2D.Piece;
using UnityEngine;

namespace Chess2D.AI
{
    public class AIController : MonoBehaviour
    {
        [SerializeField] private GameEvents _gameEvents;
        [SerializeField] private AudioConfig _audioConfig;
        [Range(1, 3)][SerializeField] private int _depth = 1;
        private Board.IBoard _board;
        private CancellationTokenSource _cts;

        private void OnEnable()
        {
            _gameEvents.SwitchTurnToAIEvent.OnEventRaised += MakeAIMove;
            _gameEvents.SwitchTurnToPlayerEvent.OnEventRaised += CancelMove;

            _depth = PlayerPrefs.GetInt("Difficulty", 2);
        }

        private void OnDisable()
        {
            _gameEvents.SwitchTurnToAIEvent.OnEventRaised -= MakeAIMove;
            _gameEvents.SwitchTurnToPlayerEvent.OnEventRaised -= CancelMove;
        }

        private void Start() => _board = GameManager.Instance.Board;

        private void CancelMove(Empty e) => _cts?.Cancel();

        private async void MakeAIMove(Empty empty = null)
        {
            _cts = new CancellationTokenSource();

            Move bestMove = null;
            try
            {
                bestMove = await Task.Run(() =>
                    GetBestMove(_depth, float.NegativeInfinity, float.PositiveInfinity, true, _cts.Token),
                    _cts.Token
                );
            }
            catch (OperationCanceledException)
            {
                Debug.Log("AI move canceled due to time out.");
                return;
            }

            if (bestMove != null)
                ExecuteMove(bestMove);
        }

        private Move GetBestMove(int depth, float alpha, float beta, bool maximizingPlayer, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            List<Move> allMoves = GetAllPossibleMoves(false);

            Move bestMove = null;
            float bestEval = maximizingPlayer ? float.NegativeInfinity : float.PositiveInfinity;

            foreach (var move in allMoves)
            {
                token.ThrowIfCancellationRequested();

                var captured = SimulateMove(move);

                float eval = Minimax(depth - 1, alpha, beta, !maximizingPlayer);

                UndoMove(move, captured);

                if (maximizingPlayer)
                {
                    if (eval > bestEval)
                    {
                        bestEval = eval;
                        bestMove = move;
                    }
                    alpha = Mathf.Max(alpha, eval);
                }
                else
                {
                    if (eval < bestEval)
                    {
                        bestEval = eval;
                        bestMove = move;
                    }
                    beta = Mathf.Min(beta, eval);
                }

                if (beta <= alpha)
                    break;
            }

            return bestMove;
        }

        private float Minimax(int depth, float alpha, float beta, bool maximizingPlayer)
        {
            if (depth == 0)
                return EvaluateBoard();

            List<Move> moves = GetAllPossibleMoves(!maximizingPlayer);

            float bestEval = maximizingPlayer ? float.NegativeInfinity : float.PositiveInfinity;

            foreach (var move in moves)
            {
                var captured = SimulateMove(move);

                float eval = Minimax(depth - 1, alpha, beta, !maximizingPlayer);

                UndoMove(move, captured);

                if (maximizingPlayer)
                {
                    bestEval = Mathf.Max(bestEval, eval);
                    alpha = Mathf.Max(alpha, eval);
                }
                else
                {
                    bestEval = Mathf.Min(bestEval, eval);
                    beta = Mathf.Min(beta, eval);
                }

                if (beta <= alpha)
                    break;
            }

            return bestEval;
        }

        private float EvaluateBoard()
        {
            float eval = 0f;

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (!_board.TryGetOccupiedPieceAt(new Vector2Int(col, row), out var piece)) continue;

                    float value = GetPieceValue(piece.PieceData.Type);
                    eval += piece.PieceData.IsPlayer ? -value : value;
                }
            }

            return eval;
        }

        private float GetPieceValue(PieceType type) => type switch
        {
            PieceType.Pawn => 1f,
            PieceType.Knight => 4f,
            PieceType.Bishop => 3f,
            PieceType.Rook => 5f,
            PieceType.Queen => 9f,
            PieceType.King => 100f,
            _ => 0f
        };

        private List<Move> GetAllPossibleMoves(bool isPlayer)
        {
            List<Move> moves = new();

            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    if (!_board.TryGetOccupiedPieceAt(new Vector2Int(col, row), out var piece)) continue;

                    if (piece.PieceData.IsPlayer == isPlayer)
                    {
                        Vector2Int current = new(col, row);

                        piece.MoveStrategy.CalculateLegalMoves(isPlayer, current, to =>
                        {
                            moves.Add(new Move
                            {
                                from = current,
                                to = to,
                                movedPiece = piece,
                                ContainsCapturablePiece = _board.ContainsOpponentPieceAt(to, isPlayer)
                            });
                        });
                    }
                }
            }

            return moves;
        }

        private ChessPiece SimulateMove(Move move)
        {
            ChessPiece captured = _board.GetOccupiedPieceAt(move.to);

            _board.SetOccupiedPieceAt(null, move.from);
            _board.SetOccupiedPieceAt(move.movedPiece, move.to);

            return captured;
        }

        private void UndoMove(Move move, ChessPiece captured)
        {
            _board.SetOccupiedPieceAt(null, move.to);
            _board.SetOccupiedPieceAt(move.movedPiece, move.from);

            if (captured != null)
                _board.SetOccupiedPieceAt(captured, move.to);
        }

        private void ExecuteMove(Move move)
        {
            move.movedPiece.SetPiecePosition(move.to);

            if (move.ContainsCapturablePiece && _board.TryCapturePieceAt(move.to, false, out var capturedPiece))
            {
                if (capturedPiece != null)
                {
                    _gameEvents.PieceCaptureEvent.RaiseEvent(capturedPiece);
                    _gameEvents.PlayOneShotAudioEvent.RaiseEvent(_audioConfig.CaptureAudio);
                }

                if (capturedPiece.PieceType == PieceType.King)
                        _gameEvents.WinEvent.RaiseEvent(null);
            }
            else
            {
                _gameEvents.PlayOneShotAudioEvent.RaiseEvent(_audioConfig.MoveSelfAudio);
            }

            _board.SetOccupiedPieceAt(null, move.from);
            _board.SetOccupiedPieceAt(move.movedPiece, move.to);

            _gameEvents.AIMadeMoveEvent.RaiseEvent(move.to);
        }
    }
}