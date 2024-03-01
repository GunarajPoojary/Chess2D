using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece, IHighlight
{
    // Method to highlight legal moves for the queen.
    public void Highlight()
    {
        // Get the current position of the queen.
        row = Convert.ToInt32(transform.position.x);
        column = Convert.ToInt32(transform.position.y);

        // Determine legal moves for the queen.
        LegalMove(row, column);
    }

    // Method to calculate legal moves for the queen.
    public override void LegalMove(int row, int column)
    {
        // Loop through all possible directions: horizontal, vertical, and diagonal.
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                // Skip the queen's current position.
                if (i == transform.position.x && j == transform.position.y) continue;

                // Check and highlight valid moves based on the queen's color.
                if (transform.name.Contains(Board.Instance.playerColor))
                    StraightOrDiagonalMoves(row, column, i, j, Board.Instance.playerColor, Board.Instance.opponentColor);
                else if (transform.name.Contains(Board.Instance.opponentColor))
                    StraightOrDiagonalMoves(row, column, i, j, Board.Instance.opponentColor, Board.Instance.playerColor);
            }
        }
    }
}
