using Chess2D.Board.Utilities;
using NUnit.Framework;
using UnityEngine;

public class BoardUtilitiesTests
{
    [Test]
    public void InsideBoard_ReturnsTrue()
    {
        Assert.IsTrue(BoardUtilities.IsWithinBoard(new Vector2Int(0, 0)));
        Assert.IsTrue(BoardUtilities.IsWithinBoard(new Vector2Int(7, 7)));
    }

    [Test]
    public void OutsideBoard_ReturnsFalse()
    {
        Assert.IsFalse(BoardUtilities.IsWithinBoard(new Vector2Int(-1, 0)));
        Assert.IsFalse(BoardUtilities.IsWithinBoard(new Vector2Int(8, 8)));
    }
}