using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Chess2D.UI
{
    public class TutorialManager : MonoBehaviour
    {
        [SerializeField] private GameObject tutorialPanel;
        [SerializeField] private TextMeshProUGUI tutorialText;
        [SerializeField] private Button nextButton;

        [TextArea(2, 5)]
        [SerializeField] private string[] tutorialSteps;

        private int currentStep = 0;

        void Start()
        {
            tutorialPanel.SetActive(true);
            ShowStep(0);
            nextButton.onClick.AddListener(NextStep);
        }

        private void ShowStep(int stepIndex)
        {
            if (stepIndex < tutorialSteps.Length)
            {
                tutorialText.text = tutorialSteps[stepIndex];
            }
        }

        private void NextStep()
        {
            currentStep++;

            if (currentStep >= tutorialSteps.Length)
            {
                tutorialPanel.SetActive(false); // Hide when done
            }
            else
            {
                ShowStep(currentStep);
            }
        }
    }
}