using UnityEngine;

namespace Chess2D
{
    public interface ISelectStrategy
    {
        void HandleSelection();
    }

    public class PlayerPieceController : ISelectStrategy
    {
        private readonly GameObject _moveHighlighter;
        private readonly GameObject _captureHighlighter;

        public PlayerPieceController(GameObject moveHighlighter, GameObject captureHighlighter)
        {
            _moveHighlighter = moveHighlighter;
            _captureHighlighter = captureHighlighter;
        }

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