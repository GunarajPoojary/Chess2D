using Chess2D.ScriptableObjects;

namespace Chess2D.Piece
{
    public class PieceFactory
    {
        private readonly PieceRendererDatabase _pieceDatabase;
        private readonly MoveStrategyFactory _moveStrategyFactory;

        public PieceFactory(PieceRendererDatabase pieceDatabase, MoveStrategyFactory moveStrategyFactory)
        {
            _pieceDatabase = pieceDatabase;
            _moveStrategyFactory = moveStrategyFactory;
        }

        public ChessPiece CreatePiece(PieceData data)
        {
            ChessPiece piece = data.Type switch
            {
                PieceType.Pawn => GetPiece(_moveStrategyFactory.GetPieceMoveStrategy(data), data, _pieceDatabase.Pawn),
                PieceType.Rook => GetPiece(_moveStrategyFactory.GetPieceMoveStrategy(data), data, _pieceDatabase.Rook),
                PieceType.Knight => GetPiece(_moveStrategyFactory.GetPieceMoveStrategy(data), data, _pieceDatabase.Knight),
                PieceType.Bishop => GetPiece(_moveStrategyFactory.GetPieceMoveStrategy(data), data, _pieceDatabase.Bishop),
                PieceType.Queen => GetPiece(_moveStrategyFactory.GetPieceMoveStrategy(data), data, _pieceDatabase.Queen),
                PieceType.King => GetPiece(_moveStrategyFactory.GetPieceMoveStrategy(data), data, _pieceDatabase.King),
                _ => throw new System.ArgumentOutOfRangeException(nameof(data), $"Unknown piece type: {data}")
            };

            return piece;
        }

        private ChessPiece GetPiece(
            IMoveStrategy moveStrategy,
            PieceData pieceData,
            PieceRenderer pieceRenderer) => new(moveStrategy, pieceData, pieceRenderer);
    }
}