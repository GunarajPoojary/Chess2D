using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class SpriteToggle : MonoBehaviour
{
    [SerializeField] private Image _iconImage;
    [SerializeField] private Sprite _onSprite;
    [SerializeField] private Sprite _offSprite;

    private Toggle _toggle;

    private void Awake()
    {
        _toggle = GetComponent<Toggle>();
        
        UpdateIcon(_toggle.isOn);

        _toggle.onValueChanged.AddListener(UpdateIcon);
    }

    private void UpdateIcon(bool isOn) => _iconImage.sprite = isOn ? _onSprite : _offSprite;
}