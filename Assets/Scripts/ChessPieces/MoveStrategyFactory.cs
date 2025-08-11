namespace Chess2D.Piece
{
    public class MoveStrategyFactory
    {
        private Board.IBoardUtility _boardUtility;

        public MoveStrategyFactory(Board.IBoardUtility boardUtility)
        {
            _boardUtility = boardUtility;
        }

        public IMoveStrategy GetPieceMoveStrategy(PieceData data)
        {
            IMoveStrategy moveStrategy = null;

            switch (data.Type)
            {
                case PieceType.Pawn:
                    moveStrategy = new PawnMove(data.IsPlayer, _boardUtility);
                    break;
                case PieceType.Rook:
                    moveStrategy = new RookMove(_boardUtility);
                    break;
                case PieceType.Knight:
                    moveStrategy = new KnightMove(_boardUtility);
                    break;
                case PieceType.Bishop:
                    moveStrategy = new BishopMove(_boardUtility);
                    break;
                case PieceType.Queen:
                    moveStrategy = new QueenMove(_boardUtility);
                    break;
                case PieceType.King:
                    moveStrategy = new KingMove(_boardUtility);
                    break;
            }

            return moveStrategy;
        }

        public IMoveStrategy GetPawnSingleStepMoveStrategy(bool isPlayer) => isPlayer
                            ? new SingleStepMove(_boardUtility, Directions.Up)
                            : new SingleStepMove(_boardUtility, Directions.Down);
    }
}