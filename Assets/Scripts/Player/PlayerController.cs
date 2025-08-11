using System.Collections.Generic;
using Chess2D.Board.Utilities;
using Chess2D.Events;
using Chess2D.Highlight;
using Chess2D.Piece;
using UnityEngine;

namespace Chess2D.Player
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private GameEvents _gameEvents;
        private Camera _mainCamera;
        private ChessPiece _selectedPiece;
        private readonly List<Vector2Int> _selectedPieceValidMoves = new();
        private Board.ChessBoard _board;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        private void Start()
        {
            _board = GameManager.Instance.Board;
        }

        private void Update()
        {
            Vector3 inputPos = Vector3.zero;

            bool inputDown = false;

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBGL
            inputDown = Input.GetMouseButtonDown(0);
            inputPos = Input.mousePosition;
#else
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            inputPos = touch.position;
            inputDown = touch.phase == TouchPhase.Began;
        }
#endif
            if (inputDown)
            {
                Vector2 mouseWorldPos = _mainCamera.ScreenToWorldPoint(inputPos);

                Vector2Int input = new((int)mouseWorldPos.x, (int)mouseWorldPos.y);

                if (!BoardUtilities.IsWithinBoard(input)) return;

                if (_board.TryGetPlayerPieceAt(input, out ChessPiece piece))
                {
                    // if new piece selected unhighlight previous highlighters
                    if (_selectedPiece != null && _selectedPiece != piece)
                    {
                        _gameEvents.UnHighlightEvent.RaiseEvent(_selectedPiece.BoardPosition);

                        ClearValidMoves();
                    }

                    _selectedPiece = piece;
                    _gameEvents.HighlightEvent.RaiseEvent((input, HighlightType.Select));
                    _selectedPiece.MoveStrategy.CalculateLegalMoves(true, input, OnGetLegalMove);
                }
                else
                {
                    if (_board.IsMoveValidAt(input) && _selectedPiece != null)
                    {
                        ClearValidMoves();

                        Vector2Int previousPosition = _selectedPiece.BoardPosition;

                        _selectedPiece.SetPiecePosition(input);

                        if (_board.TryCapturePieceAt(input, true, out ChessPiece capturedPiece))
                        {
                            if (capturedPiece.PieceType == PieceType.King)
                                _gameEvents.WinEvent.RaiseEvent(null);
                        }

                        _board.SetOccupiedPieceAt(null, previousPosition);

                        _board.SetOccupiedPieceAt(_selectedPiece, input);

                        if (_selectedPiece.MoveStrategy is IMoveStrategySwitch postMoveAware)
                            postMoveAware.SwitchStrategy();

                        _gameEvents.PlayerMadeMove.RaiseEvent(null);

                        _selectedPiece = null;
                    }
                }
            }
        }

        private void ClearValidMoves()
        {
            foreach (Vector2Int position in _selectedPieceValidMoves)
            {
                _gameEvents.UnHighlightEvent.RaiseEvent(position);

                _board.UnMarkMoveValidAt(position);
            }

            _gameEvents.UnHighlightEvent.RaiseEvent(_selectedPiece.BoardPosition);
            _selectedPieceValidMoves.Clear();
        }

        private void OnGetLegalMove(Vector2Int position)
        {
            _gameEvents.HighlightEvent.RaiseEvent((new Vector2Int(
                position.x,
                position.y), _board.ContainsOpponentPieceAt(
                    position,
                    true) ? HighlightType.Capture : HighlightType.EmptyTile));

            _board.MarkMoveValidAt(position);

            _selectedPieceValidMoves.Add(position);
        }
    }
}