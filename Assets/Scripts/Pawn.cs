using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece, IHighlight
{
    // Method to highlight legal moves for the pawn.
    public void Highlight()
    {
        // Get the current position of the pawn.
        row = Convert.ToInt32(transform.position.x);
        column = Convert.ToInt32(transform.position.y);

        // Determine legal moves for the pawn.
        LegalMove(row, column);
    }

    // Method to calculate legal moves for the pawn.
    public override void LegalMove(int row, int column)
    {
        int targetRow = row;

        // Determine the target column based on the pawn's color and game state.
        if (transform.name.Contains(Board.Instance.playerColor))
        {
            int targetColumn = column + 1;
            SingleStep(targetRow, targetColumn, Board.Instance.playerColor, Board.Instance.opponentColor);
        }
        else if (transform.name.Contains(Board.Instance.opponentColor))
        {
            int targetColumn = column - 1;
            SingleStep(targetRow, targetColumn, Board.Instance.opponentColor, Board.Instance.playerColor);
        }
    }
}
