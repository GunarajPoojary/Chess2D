using UnityEngine;

namespace Chess2D.Highlight
{
    /// <summary>
    /// Controls tile highlighting using a finite state machine (FSM)
    /// </summary>
    public class Highlighter : MonoBehaviour
    {
        [SerializeField] private HighlightSpritesDatabase _colorThemeSO;
        private SpriteRenderer _renderer;

        private void Awake() => _renderer = GetComponentInChildren<SpriteRenderer>();

        public void Highlight(HighlightType type)
        {
            switch (type)
            {
                case HighlightType.Select:
                    ApplyHighlight(_colorThemeSO.SelectionHighlightSprite);
                    break;

                case HighlightType.EmptyTile:
                    ApplyHighlight(_colorThemeSO.EmptyTileHighlightSprite);
                    break;

                // case HighlightType.Special:
                //     ApplyHighlight(_colorThemeSO.SpecialHighlightSprite);
                //     break;

                case HighlightType.Capture:
                    ApplyHighlight(_colorThemeSO.CaptureHighlightSprite);
                    break;
            }
        }

        private void ApplyHighlight(Sprite sprite) => _renderer.sprite = sprite;
    }
}