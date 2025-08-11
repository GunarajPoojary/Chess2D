using TMPro;
using UnityEngine;

namespace Chess2D.UI
{
    public class UIMoveItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text _moveText;
        private string _playerMove = "";
        private string _aiMove = "";

        public void SetMove(int moveCount, int row, int col, bool isPlayer)
        {
            if (isPlayer)
            {
                _playerMove = ((char)(col + 64)).ToString() + row;
            }
            else
            {
                _aiMove = ((char)(col + 64)).ToString() + row;
            }

            _moveText.text = $"{moveCount}. {_playerMove} {_aiMove}";
        }
    }
}