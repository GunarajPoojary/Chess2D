using System.Collections.Generic;
using Chess2D.Events;
using Chess2D.Piece;
using UnityEngine;

namespace Chess2D.UI
{
    public class UICapturedPieces : MonoBehaviour
    {
        [SerializeField] private GameEvents _gameEvents;
        [SerializeField] private Transform _playerCapturedContainer;
        [SerializeField] private Transform _aiCapturedContainer;

        private List<PieceRenderer> _playerCaptured = new();
        private List<PieceRenderer> _aiCaptured = new();

        private void OnEnable()
        {
            _gameEvents.PieceCaptureEvent.OnEventRaised += OnPieceCaptured;
        }

        private void OnDisable()
        {
            _gameEvents.PieceCaptureEvent.OnEventRaised -= OnPieceCaptured;
        }

        private void OnPieceCaptured(PieceRenderer capturedRenderer)
        {
            // Make a small icon copy
            PieceRenderer icon = Instantiate(capturedRenderer, Vector3.zero, Quaternion.identity);
            icon.transform.localScale = Vector3.one * 0.5f; // smaller size
            icon.SetInActive(); // ensure original gameplay object is not active in the scene
            icon.gameObject.SetActive(true);

            // Decide where to show based on who owned the piece
            ChessPiece piece = capturedRenderer.GetComponent<ChessPiece>();
            bool wasPlayerPiece = piece != null && piece.IsPlayer;

            if (wasPlayerPiece)
            {
                icon.transform.SetParent(_aiCapturedContainer, false);
                _playerCaptured.Add(icon);
            }
            else
            {
                icon.transform.SetParent(_playerCapturedContainer, false);
                _aiCaptured.Add(icon);
            }
        }
    }
}