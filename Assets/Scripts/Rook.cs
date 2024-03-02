using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece, IHighlight
{
    // Method to highlight legal moves for the rook.
    public void Highlight()
    {
        // Get the current position of the rook.
        row = Convert.ToInt32(transform.position.x);
        column = Convert.ToInt32(transform.position.y);

        // Determine legal moves for the rook.
        LegalMove(row, column);
    }

    // Method to calculate legal moves for the rook.
    public override void LegalMove(int row, int column)
    {
        /* The rook can move horizontally and vertically, so we call the StraightMoves method
           for up, down, left, and right directions. */

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                // Skip the current position and diagonals.
                if ((i == 0 && j == 0) || (i == 1 && j == 1) || (i == -1 && j == -1) || (i == -1 && j == 1) || (i == 1 && j == -1))
                    continue;

                // Check and highlight the move based on the rook's color.
                if (transform.name.Contains(boardInstance.playerColor))
                    StraightOrDiagonalMoves(row, column, i, j, boardInstance.playerColor, boardInstance.opponentColor);
                else if (transform.name.Contains(boardInstance.opponentColor))
                    StraightOrDiagonalMoves(row, column, i, j, boardInstance.opponentColor, boardInstance.playerColor);
            }
        }
    }
}
