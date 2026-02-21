using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField] private GameStateController stateController;

    public event Action<int> OnScoreChanged;
    public event Action<int> OnOpponentScoreChanged;
    public event Action OnScoreReset;
    public event Action OnScoresReset;

    private int score;
    private int opponentScore;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnEnable()
    {
        if (stateController != null)
        {
            stateController.OnGameStart += HandleGameStart;
        }
    }

    private void OnDisable()
    {
        if (stateController != null)
        {
            stateController.OnGameStart -= HandleGameStart;
        }
    }

    private void HandleGameStart()
    {
        ResetScores();
    }

    public void AddPoints(ShotContext.ShooterType shooter, int points)
    {
        if (shooter == ShotContext.ShooterType.Opponent)
        {
            opponentScore += points;
            OnOpponentScoreChanged?.Invoke(opponentScore);
        }
        else
        {
            score += points;
            OnScoreChanged?.Invoke(score);
        }
    }

    public void AddPoints(int points)
    {
        AddPoints(ShotContext.ShooterType.Player, points);
    }

    public void ResetScore()
    {
        score = 0;
        OnScoreChanged?.Invoke(score);
        OnScoreReset?.Invoke();
    }

    public void ResetScores()
    {
        score = 0;
        opponentScore = 0;
        OnScoreChanged?.Invoke(score);
        OnOpponentScoreChanged?.Invoke(opponentScore);
        OnScoreReset?.Invoke();
        OnScoresReset?.Invoke();
    }

    public int GetScore()
    {
        return score;
    }

    public int GetOpponentScore()
    {
        return opponentScore;
    }
}
