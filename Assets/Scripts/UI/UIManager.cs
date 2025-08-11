using UnityEngine;

namespace Chess2D.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private GameObject _winStats;
        [SerializeField] private UIMoveHistory _uiMoveHistory;

        public void ShowWinStats()
        {
            _winStats.SetActive(true);
        }

        private void ChangeBoardColor()
        {
            //  Board.SetBoardTheme(Color.red, Color.yellow);
        }
    }
}