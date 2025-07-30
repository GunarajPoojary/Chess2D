using UnityEngine;

namespace Chess2D
{
    public interface ISelectStrategy
    {
        void HandleSelection();
    }

    public class PlayerPieceController : ISelectStrategy
    {
        public void HandleSelection()
        {
            Debug.Log("Select a piece");
        }
    }

    public class AIPieceController : ISelectStrategy
    {
        public void HandleSelection()
        {
            Debug.Log("Select a random piece");
        }
    }
}