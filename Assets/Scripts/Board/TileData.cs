namespace Chess2D.Board
{
    /// <summary>
    /// Represents the state of a single tile on the chessboard.
    /// Stores information about whether it is occupied and if it is currently a valid move target.
    /// </summary>
    public class TileData
    {
        /// <summary>
        /// Indicates whether this tile is currently marked as valid for a move.
        /// Typically set during legal move calculation and cleared after a move is made or cancelled.
        /// </summary>
        public bool IsValidForMove { get; private set; }

        /// <summary>
        /// Marks this tile as a valid move destination for the currently selected piece.
        /// </summary>
        public void MarkValidForMove() => IsValidForMove = true;

        /// <summary>
        /// Removes the "valid move" mark from this tile, making it a normal tile again.
        /// </summary>
        public void UnMarkValidForMove() => IsValidForMove = false;
    }
}