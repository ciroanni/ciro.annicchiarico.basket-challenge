using TMPro;
using UnityEngine;

public class GameUIController : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject gameplayUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameStateController stateController;

    private void OnEnable()
    {
        if (stateController != null)
        {
            stateController.OnStateChanged += HandleStateChanged;
            stateController.OnTimeChanged += HandleTimeChanged;
        }
    }

    private void Start()
    {
        if (stateController != null)
        {
            HandleStateChanged(stateController.CurrentState);
        }
    }

    private void OnDisable()
    {
        if (stateController != null)
        {
            stateController.OnStateChanged -= HandleStateChanged;
            stateController.OnTimeChanged -= HandleTimeChanged;
        }
    }

    private void HandleStateChanged(GameStateController.GameState state)
    {
        if (mainMenuUI != null)
        {
            mainMenuUI.SetActive(state == GameStateController.GameState.Menu);
        }

        if (gameplayUI != null)
        {
            gameplayUI.SetActive(state == GameStateController.GameState.Playing);
        }

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(state == GameStateController.GameState.GameOver);
        }
    }

    private void HandleTimeChanged(float timeRemaining)
    {
        if (timerText != null)
        {
            timerText.text = $"{timeRemaining:F2}s";
        }
    }
}
