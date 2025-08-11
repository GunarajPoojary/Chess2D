using UnityEngine;

namespace Chess2D.Piece
{
    public class PieceRenderer : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _fillRenderer;

        private void Awake()
        {
            if (_fillRenderer == null)
                Debug.LogError("Forgot to assign sprite renderer component for the chess piece prefab");
        }

        public void SetColor(Color color) => _fillRenderer.color = color;
        public void SetWorldPosition(Vector3Int position) => transform.position = position;
        public void SetInActive() => gameObject.SetActive(false);
        internal void SetActive() => gameObject.SetActive(true);
    }
}