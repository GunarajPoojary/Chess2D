using Chess2D.Events;
using Chess2D.Piece;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Chess2D.UI
{
    public class UICapturedPieces : MonoBehaviour
    {
        [SerializeField] private GameEvents _gameEvents;

        private readonly Dictionary<ChessPiece, Image> _capturedPieceMap = new();
        private PieceFactory<Image> _playerPieceFactory;
        private PieceFactory<Image> _aiPieceFactory;
        [SerializeField] private Transform _aiPieceTransform;
        [SerializeField] private Transform _playerPieceTransform;

        private void OnEnable()
        {
            _gameEvents.PieceCaptureEvent.OnEventRaised += OnPieceCaptured;
            _gameEvents.InitializePieceEvent.OnEventRaised += AddPieceUI;
        }

        private void OnDisable()
        {
            _gameEvents.PieceCaptureEvent.OnEventRaised -= OnPieceCaptured;
            _gameEvents.InitializePieceEvent.OnEventRaised -= AddPieceUI;
        }

        public void Initialize(PieceFactory<Image> playerPieceFactory, PieceFactory<Image> aiPieceFactory)
        {
            _playerPieceFactory = playerPieceFactory;
            _aiPieceFactory = aiPieceFactory;
        }

        private void AddPieceUI(ChessPiece piece)
        {
            Image pieceImage = piece.IsPlayer ? _playerPieceFactory.GetPiece(piece.PieceType) : _aiPieceFactory.GetPiece(piece.PieceType);

            pieceImage.transform.SetParent(piece.IsPlayer ? _playerPieceTransform : _aiPieceTransform);

            pieceImage.gameObject.SetActive(false);

            _capturedPieceMap.Add(piece, pieceImage);
        }

        private void OnPieceCaptured(ChessPiece piece)
        {
            if (_capturedPieceMap.TryGetValue(piece, out var pieceImage))
            {
                pieceImage.gameObject.SetActive(true);
            }
        }
    }
}