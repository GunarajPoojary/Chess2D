using Chess2D.Events;
using UnityEngine;

namespace Chess2D.Highlight
{
    public class HighlighterManager : MonoBehaviour
    {
        [SerializeField] private Highlighter _highlighterPrefab;
        [SerializeField] private GameEvents _gameEvents;

        private readonly Highlighter[,] _highlighters = new Highlighter[8, 8];

        private void Awake() => InitHighlighters();

        private void OnEnable()
        {
            _gameEvents.HighlightEvent.OnEventRaised += Highlight;
            _gameEvents.UnHighlightEvent.OnEventRaised += UnHighlight;
        }

        private void OnDisable()
        {
            _gameEvents.HighlightEvent.OnEventRaised -= Highlight;
            _gameEvents.UnHighlightEvent.OnEventRaised -= UnHighlight;
        }

        public void InitHighlighters()
        {
            for (int row = 0; row < 8; row++)
            {
                for (int col = 0; col < 8; col++)
                {
                    Highlighter highlighter = Instantiate(_highlighterPrefab, new Vector3(col, row), Quaternion.identity, transform);

                    highlighter.transform.name = $"Highlighter_C{col}_R{row}";

                    highlighter.gameObject.SetActive(false);

                    _highlighters[row, col] = highlighter;
                }
            }
        }

        public void Highlight((Vector2Int worldPosition, HighlightType type) highlightData)
        {
            Vector2Int pos = highlightData.worldPosition;
            HighlightType type = highlightData.type;

            Highlighter highlighter = _highlighters[pos.y, pos.x];
            highlighter.gameObject.SetActive(true);
            highlighter.Highlight(type);
        }

        public void UnHighlight(Vector2Int worldPosition)
        {
            Highlighter highlighter = _highlighters[worldPosition.y, worldPosition.x];

            highlighter.gameObject.SetActive(false);
        }
    }
}