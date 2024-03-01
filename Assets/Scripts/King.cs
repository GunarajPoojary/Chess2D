using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// King class inherits from Piece class and implements IHighlight interface.
public class King : Piece, IHighlight
{
    // Method to highlight legal moves for the king.
    public void Highlight()
    {
        // Get the current position of the king.
        row = Convert.ToInt32(transform.position.x);
        column = Convert.ToInt32(transform.position.y);

        // Determine legal moves for the king.
        LegalMove(row, column);
    }

    // Method to calculate legal moves for the king.
    public override void LegalMove(int row, int column)
    {
        // Loop through surrounding cells around the king.
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                // Skip the current cell where the king is placed.
                if (row == 0 && column == 0) continue;

                // Calculate the target row and column for potential move.
                int targetRow = row + i;
                int targetColumn = column + j;

                // Determine legal moves based on the player's color.
                if (transform.name.Contains(Board.Instance.playerColor))
                    SingleStep(targetRow, targetColumn, Board.Instance.playerColor, Board.Instance.opponentColor);
                else if (transform.name.Contains(Board.Instance.opponentColor))
                    SingleStep(targetRow, targetColumn, Board.Instance.opponentColor, Board.Instance.playerColor);
            }
        }
    }

    // Update is called once per frame.
    private void Update()
    {
        // Check if the king has reached the opponent's back rank for victory.
        if (transform.name.Contains(Board.Instance.playerColor) && transform.position.x > 7)
        {
            // Player 1 wins if their king reaches the opponent's back rank.
            GameManager.Instance.Player_1Win(Board.Instance.opponentColor);
        }
        else if (transform.name.Contains(Board.Instance.opponentColor) && transform.position.x > 7)
        {
            // Player 2 wins if their king reaches the opponent's back rank.
            GameManager.Instance.Player_1Win(Board.Instance.playerColor);
        }
    }
}
