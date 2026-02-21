using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OpponentDifficultySlider : MonoBehaviour
{
    [SerializeField] private OpponentAIController opponentAI;
    [SerializeField] private Slider difficultySlider;

    private void OnEnable()
    {
        if (difficultySlider == null)
        {
            return;
        }

        difficultySlider.minValue = 0f;
        difficultySlider.maxValue = 2f;
        difficultySlider.wholeNumbers = true;
        difficultySlider.onValueChanged.AddListener(HandleSliderChanged);
        HandleSliderChanged(difficultySlider.value);
    }

    private void OnDisable()
    {
        if (difficultySlider == null)
        {
            return;
        }

        difficultySlider.onValueChanged.RemoveListener(HandleSliderChanged);
    }

    private void HandleSliderChanged(float value)
    {
        int index = Mathf.RoundToInt(value);
        if (difficultySlider != null && !Mathf.Approximately(difficultySlider.value, index))
        {
            difficultySlider.value = index;
        }

        if (opponentAI != null)
        {
            opponentAI.SetDifficultyIndex(index);
        }
    }
}
