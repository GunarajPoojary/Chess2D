using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece, IHighlight
{
    // Method to highlight legal moves for the bishop.
    public void Highlight()
    {
        // Get the current position of the bishop.
        row = Convert.ToInt32(transform.position.x);
        column = Convert.ToInt32(transform.position.y);

        // Determine legal moves for the bishop.
        LegalMove(row, column);
    }

    // Method to calculate legal moves for the bishop.
    public override void LegalMove(int row, int column)
    {
        // The bishop can move diagonally.

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                // Skip the current position and non-diagonal directions.
                if ((i == 0 && j == 0) || (i == 0 && j == 1) || (i == 0 && j == -1) || (i == -1 && j == 0) || (i == 1 && j == 0))
                    continue;

                // Check and highlight the move based on the bishop's color.
                if (transform.name.Contains(Board.Instance.playerColor))
                    StraightOrDiagonalMoves(row, column, i, j, Board.Instance.playerColor, Board.Instance.opponentColor);
                else if (transform.name.Contains(Board.Instance.opponentColor))
                    StraightOrDiagonalMoves(row, column, i, j, Board.Instance.opponentColor, Board.Instance.playerColor);
            }
        }
    }
}
