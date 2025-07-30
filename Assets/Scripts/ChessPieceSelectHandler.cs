using System.Collections.Generic;
using UnityEngine;

namespace Chess2D
{
    public class PlayerPieceSelectionHandler : MonoBehaviour
    {
        private readonly Dictionary<Vector3, ISelectable> _pieceMap = new();
        private Camera _mainCamera;

        private void Awake()
        {
            _mainCamera = Camera.main;
        }

        public void SetupSelectables(Dictionary<ChessPieceController, Vector3> playerPieces)
        {
            foreach (KeyValuePair<ChessPieceController, Vector3> piece in playerPieces)
                _pieceMap.Add(piece.Value, piece.Key);
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

                if (_pieceMap.TryGetValue(new Vector3(
                        (int)mouseWorldPos.x,
                        (int)mouseWorldPos.y,
                        0), out ISelectable piece))
                { piece.Select(); }
            }
        }

        public void UpdatePiecePosition(ISelectable piece, Vector3 oldPosition, Vector3 newPosition)
        {
            if (_pieceMap.Remove(oldPosition))
                _pieceMap[newPosition] = piece;
        }
    }
}