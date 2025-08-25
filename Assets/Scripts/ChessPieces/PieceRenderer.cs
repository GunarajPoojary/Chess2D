using UnityEngine;

namespace Chess2D.Piece
{
    public class PieceRenderer : MonoBehaviour
    {
        public void SetWorldPosition(Vector3Int position) => transform.position = position;
        public void SetInActive() => gameObject.SetActive(false);
        internal void SetActive() => gameObject.SetActive(true);
    }
}