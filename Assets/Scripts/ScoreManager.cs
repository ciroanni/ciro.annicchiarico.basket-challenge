using System;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    [SerializeField] private GameStateController stateController;

    public event Action<int> OnScoreChanged;

    private int score;

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
        ResetScore();
    }

    public void AddPoints(int points)
    {
        score += points;
        OnScoreChanged?.Invoke(score);
    }

    public void ResetScore()
    {
        score = 0;
        OnScoreChanged?.Invoke(score);
    }

    public int GetScore()
    {
        return score;
    }
}
