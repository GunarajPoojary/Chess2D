using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece, IHighlight
{
    // Method to highlight legal moves for the knight.
    public void Highlight()
    {
        // Get the current position of the knight.
        row = Convert.ToInt32(transform.position.x);
        column = Convert.ToInt32(transform.position.y);

        // Determine legal moves for the knight.
        LegalMove(row, column);
    }

    // Method to calculate legal moves for the knight.
    public override void LegalMove(int row, int column)
    {
        // Define the relative positions for knight moves
        int[,] knightCells = new int[,] { { 2, 1 }, { 2, -1 }, { -2, 1 }, { -2, -1 },
                                        { 1, 2 }, { -1, 2 }, { 1, -2 }, { -1, -2 } }; /* {i, j} i represent row and j represent column.
                                                                                       This array has 8 rows and 2 column */

        // Iterate through all possible knight moves
        for (int i = 0; i < knightCells.GetLength(0); i++)
        {
            int targetRow = row + knightCells[i, 0]; // Elements are 2, 2, -2, -2, 1, -1, 1, -1.
            int targetColumn = column + knightCells[i, 1]; // Elements are 1, -1, 1, -1, 2, 2, -2, -2.

            // Highlight the cells based on the knight's color
            if (transform.name.Contains(Board.Instance.playerColor))
                SingleStep(targetRow, targetColumn, Board.Instance.playerColor, Board.Instance.opponentColor);
            else if (transform.name.Contains(Board.Instance.opponentColor))
                SingleStep(targetRow, targetColumn, Board.Instance.opponentColor, Board.Instance.playerColor);
        }
    }
}
