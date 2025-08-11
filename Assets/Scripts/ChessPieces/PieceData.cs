namespace Chess2D.Piece
{
    public class PieceData
    {
        public PieceType Type { get; }
        public bool IsPlayer { get; }

        public PieceData(PieceType type, bool isPlayer)
        {
            Type = type;
            IsPlayer = isPlayer;
        }
    }
}