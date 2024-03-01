using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece : MonoBehaviour
{
    // Row and column position of the piece on the chessboard.
    public int row, column;

    // Reference to the green and red highlighter prefabs.
    [SerializeField] protected GameObject greenHighlighter, redHighlighter;

    // Abstract method to calculate legal moves for the piece.
    public abstract void LegalMove(int row, int column);

    // Start is called before the first frame update.
    void Start()
    {
        // Initialize the row and column based on the piece's position.
        row = Convert.ToInt32(transform.position.x);
        column = Convert.ToInt32(transform.position.y);
    }

    // Method to handle legal moves for a single step.
    protected void SingleStep(int targetRow, int targetColumn, string color1, string color2)
    {
        // Local function to check if a cell is occupied by a chess piece.
        bool IsCellOccupied(int row, int column)
        {
            foreach (Piece c in Board.Instance.pieces)
            {
                if (transform.name.Contains("King"))
                {
                    if (c.transform.position.x == row && c.transform.position.y == column && c.name.Contains(color1))
                        return true;
                    else if (c.transform.position.x == targetRow && c.transform.position.y == targetColumn && c.name.Contains(color2))
                    {
                        HighLightCell(targetRow, targetColumn, redHighlighter);
                        return true;
                    }
                }
                else if (transform.name.Contains("Knight"))
                {
                    if (c.transform.position.x == row && c.transform.position.y == column && c.name.Contains(color1))
                        return true;
                    else if (c.transform.position.x == targetRow && c.transform.position.y == targetColumn && c.name.Contains(color2))
                    {
                        HighLightCell(targetRow, targetColumn, redHighlighter);
                        return true;
                    }
                }
                else
                {
                    if (c.transform.position.x == row && c.transform.position.y == column && c.name.Contains(color1))
                        return true;
                    else if (c.transform.position.x == row && c.transform.position.y == column && c.name.Contains(color2))
                        return true;
                }
            }
            return false;
        }

        // Check cells surrounding the target for potential captures.
        foreach (Piece c in Board.Instance.pieces)
        {
            for (int i = -1; i <= 1; i++)
            {
                if (i == 0) continue;
                if (c.transform.position.x == targetRow + i && c.transform.position.y == targetColumn && c.name.Contains(color2) && transform.name.Contains("Pawn"))
                {
                    HighLightCell(targetRow + i, targetColumn, redHighlighter);
                }
            }
        }

        // If the target cell is not occupied, highlight it.
        if (!IsCellOccupied(targetRow, targetColumn))
        {
            HighLightCell(targetRow, targetColumn, greenHighlighter);
        }
    }

    // Method to highlight cells in straight or diagonal directions.
    protected void StraightOrDiagonalMoves(int row, int column, int rowDirection, int columnDirection, string color1, string color2)
    {
        for (int i = 1; i < 8; i++)
        {
            int targetRow = row + i * rowDirection;
            int targetColumn = column + i * columnDirection;

            // Check if the cell is occupied by another piece.
            foreach (Piece c in Board.Instance.pieces)
            {
                if (c.transform.position.x == targetRow && c.transform.position.y == targetColumn && c.name.Contains(color1))
                    return;
                else if (c.transform.position.x == targetRow && c.transform.position.y == targetColumn && c.name.Contains(color2))
                {
                    HighLightCell(targetRow, targetColumn, redHighlighter);
                    return;
                }
            }

            // Highlight the unoccupied cell.
            HighLightCell(targetRow, targetColumn, greenHighlighter);
        }
    }

    // Method to instantiate a highlighter at a given cell position.
    public void HighLightCell(int row, int column, GameObject highlighter)
    {
        Vector2 cell = new Vector2(row, column);
        if (cell.x >= 0 && cell.x < 8 && cell.y >= 0 && cell.y < 8) 
            Instantiate(highlighter, cell, Quaternion.identity);
    }
}
