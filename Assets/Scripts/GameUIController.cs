using TMPro;
using UnityEngine;

public class GameUIController : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject gameplayUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameStateController stateController;

    private void OnEnable()
    {
        if (stateController != null)
        {
            stateController.OnStateChanged += HandleStateChanged;
            stateController.OnTimeChanged += HandleTimeChanged;
        }

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged += HandleScoreChanged;
        }
    }

    private void Start()
    {
        if (stateController != null)
        {
            HandleStateChanged(stateController.CurrentState);
        }

        if (ScoreManager.Instance != null)
        {
            HandleScoreChanged(ScoreManager.Instance.GetScore());
        }
    }

    private void OnDisable()
    {
        if (stateController != null)
        {
            stateController.OnStateChanged -= HandleStateChanged;
            stateController.OnTimeChanged -= HandleTimeChanged;
        }

        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged -= HandleScoreChanged;
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

        if (state == GameStateController.GameState.GameOver)
        {
            UpdateFinalScore();
        }

    }

    private void HandleTimeChanged(float timeRemaining)
    {
        if (timerText != null)
        {
            timerText.text = $"{timeRemaining:F2}s";
        }
    }

    private void HandleScoreChanged(int newScore)
    {
        if (scoreText != null)
        {
            scoreText.text = newScore.ToString();
        }
    }

    private void UpdateFinalScore()
    {
        if (finalScoreText == null || ScoreManager.Instance == null)
        {
            return;
        }

        finalScoreText.text = ScoreManager.Instance.GetScore().ToString();
    }
}
