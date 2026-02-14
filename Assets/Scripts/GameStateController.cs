using System;
using UnityEngine;

public class GameStateController : MonoBehaviour
{
    public enum GameState
    {
        Menu,
        Playing,
        GameOver
    }

    [SerializeField] private float gameDuration = 60f;

    public event Action OnGameStart;
    public event Action OnGameEnd;
    public event Action<GameState> OnStateChanged;
    public event Action<float> OnTimeChanged;

    public GameState CurrentState { get; private set; }

    private float currentTime;

    private void Start()
    {
        currentTime = gameDuration;
        SetState(GameState.Menu);
        OnTimeChanged?.Invoke(currentTime);
    }

    private void Update()
    {
        if (CurrentState != GameState.Playing)
        {
            return;
        }

        currentTime -= Time.deltaTime;
        if (currentTime < 0f)
        {
            currentTime = 0f;
        }

        OnTimeChanged?.Invoke(currentTime);

        if (currentTime <= 0f)
        {
            EndGame();
        }
    }

    public void ShowMainMenu()
    {
        currentTime = gameDuration;
        SetState(GameState.Menu);
        OnTimeChanged?.Invoke(currentTime);
    }

    public void StartGame()
    {
        currentTime = gameDuration;
        SetState(GameState.Playing);
        OnGameStart?.Invoke();
        OnTimeChanged?.Invoke(currentTime);
    }

    public void EndGame()
    {
        SetState(GameState.GameOver);
        OnGameEnd?.Invoke();
    }

    public void RestartGame()
    {
        StartGame();
    }

    private void SetState(GameState newState)
    {
        CurrentState = newState;
        OnStateChanged?.Invoke(CurrentState);
    }
}
