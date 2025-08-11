using System.Collections.Generic;
using UnityEngine;

namespace Chess2D.UI
{
    public class UIMoveHistory : MonoBehaviour
    {
        [SerializeField] private UIMoveItem _uiMoveItemPrefab;
        [SerializeField] private Transform _content;
        [SerializeField] private int _poolSize;
        private readonly List<UIMoveItem> _uiMoveItems = new();
        private int _currentMoveCount;

        private void Awake()
        {
            for (int i = 0; i < _poolSize; i++)
            {
                var moveItem = Instantiate(_uiMoveItemPrefab, _content);
                moveItem.gameObject.SetActive(false);
                _uiMoveItems.Add(moveItem);
            }
        }

        public void AddMove(int moveCount, int row, int col, bool isPlayer)
        {
            if (_currentMoveCount == moveCount)
            {
                _uiMoveItems[_currentMoveCount].SetMove(moveCount, row, col, isPlayer);
                return;
            }
            _currentMoveCount = moveCount;

            _uiMoveItems[_currentMoveCount].gameObject.SetActive(true);

            _uiMoveItems[_currentMoveCount].SetMove(moveCount, row, col, isPlayer);
        }
    }
}