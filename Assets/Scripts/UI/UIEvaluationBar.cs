using UnityEngine;
using UnityEngine.UI;

namespace Chess2D.UI
{
    public class UIEvaluationBar : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private AI.AIController _aiController;
        [SerializeField] private float _maxEval = 50f;

        private void Update()
        {
            // Get board evaluation from AI
            float eval = 0;//_aiController.GetCurrentEvaluation();

            // Clamp so it fits the slider range
            eval = Mathf.Clamp(eval, -_maxEval, _maxEval);

            // Update slider value
            _slider.value = eval;
        }

        public void ShowWinner()
        {
            // if (eval > 0.5f)
            //     winnerText.text = "AI is Winning";
            // else if (eval < -0.5f)
            //     winnerText.text = "Player is Winning";
            // else
            //     winnerText.text = "Even";
        }
    }
}