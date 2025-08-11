using TMPro;
using UnityEngine;

namespace Chess2D.UI
{
    public class UITimer : MonoBehaviour
    {
        [SerializeField] private TMP_Text _timerText;

        public void UpdateTimer(int min, int sec)
        {
            _timerText.text = $"{min:00}:{sec:00}";
        }
    }
}