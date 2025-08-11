using UnityEngine;

namespace Chess2D.Piece
{
    public class ChessPiece
    {
        private readonly PieceRenderer _pieceRenderer;
        public IMoveStrategy MoveStrategy { get; private set; }
        public PieceData PieceData { get; }
        public bool IsPlayer => PieceData.IsPlayer;
        public PieceType PieceType => PieceData.Type;
        public Transform Transform => _pieceRenderer.transform;
        public Vector2Int BoardPosition => new(
            (int)_pieceRenderer.transform.position.x,
            (int)_pieceRenderer.transform.position.y);

        public ChessPiece(
            IMoveStrategy moveStrategy,
            PieceData pieceData,
            PieceRenderer pieceRenderer)
        {
            PieceData = pieceData;
            _pieceRenderer = Object.Instantiate(pieceRenderer);

            MoveStrategy = moveStrategy;
        }

        public void SetMoveStrategy(IMoveStrategy moveStrategy) => MoveStrategy = moveStrategy;
        public void SetPieceColor(Color color) => _pieceRenderer.SetColor(color);
        public void SetPiecePosition(Vector2Int position) => _pieceRenderer.SetWorldPosition(new Vector3Int(position.x, position.y));
        public void SetInActive() => _pieceRenderer.SetInActive();
        public void SetActive() => _pieceRenderer.SetActive();
    }
}