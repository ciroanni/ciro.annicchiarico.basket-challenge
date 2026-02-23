using TMPro;
using UnityEngine;

public class GameUIController : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject gameplayUI;
    [SerializeField] private GameObject gameOverUI;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI opponentScoreText;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI finalOpponentScoreText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI finalResultText;
    [SerializeField] private Color winColor = new Color(0.2f, 0.8f, 0.3f, 1f);
    [SerializeField] private Color loseColor = new Color(0.9f, 0.2f, 0.2f, 1f);
    [SerializeField] private Color drawColor = new Color(0.95f, 0.8f, 0.2f, 1f);
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
            ScoreManager.Instance.OnOpponentScoreChanged += HandleOpponentScoreChanged;
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
            HandleOpponentScoreChanged(ScoreManager.Instance.GetOpponentScore());
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
            ScoreManager.Instance.OnOpponentScoreChanged -= HandleOpponentScoreChanged;
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
            UpdateFinalResult();
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

    private void HandleOpponentScoreChanged(int newScore)
    {
        if (opponentScoreText != null)
        {
            opponentScoreText.text = newScore.ToString();
        }
    }

    private void UpdateFinalScore()
    {
        if (finalScoreText == null || ScoreManager.Instance == null)
        {
            return;
        }

        finalScoreText.text = ScoreManager.Instance.GetScore().ToString();

        if (finalOpponentScoreText != null)
        {
            finalOpponentScoreText.text = ScoreManager.Instance.GetOpponentScore().ToString();
        }
    }

    private void UpdateFinalResult()
    {
        if (finalResultText == null || ScoreManager.Instance == null)
        {
            return;
        }

        int playerScore = ScoreManager.Instance.GetScore();
        int opponentScore = ScoreManager.Instance.GetOpponentScore();

        if (playerScore > opponentScore)
        {
            finalResultText.text = "You Win!";
            finalResultText.color = winColor;
            finalScoreText.color = winColor;
            if (finalOpponentScoreText != null)
            {
                finalOpponentScoreText.color = loseColor;
            }
        }
        else if (playerScore < opponentScore)
        {
            finalResultText.text = "You Lose!";
            finalResultText.color = loseColor;
            finalScoreText.color = loseColor;
            if (finalOpponentScoreText != null)
            {
                finalOpponentScoreText.color = winColor;
            }
        }
        else
        {
            finalResultText.text = "Draw!";
            finalResultText.color = drawColor;
            finalScoreText.color = drawColor;
            if (finalOpponentScoreText != null)
            {
                finalOpponentScoreText.color = drawColor;
            }
        }
    }



}
