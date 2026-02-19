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
    [SerializeField] private BallController ballController;

    public event Action OnGameStart;
    public event Action OnGameEnd;
    public event Action<GameState> OnStateChanged;
    public event Action<float> OnTimeChanged;

    public GameState CurrentState { get; private set; }

    private float currentTime;
    private bool awaitingFinalShot;

    private void OnEnable()
    {
        if (ballController != null)
        {
            ballController.OnShotResolved += HandleShotResolved;
        }
    }

    private void OnDisable()
    {
        if (ballController != null)
        {
            ballController.OnShotResolved -= HandleShotResolved;
        }
    }

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

        if (awaitingFinalShot)
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
            if (ballController != null && ballController.IsInFlight)
            {
                awaitingFinalShot = true;
                return;
            }

            EndGame();
        }
    }

    public void ShowMainMenu()
    {
        awaitingFinalShot = false;
        currentTime = gameDuration;
        SetState(GameState.Menu);
        OnTimeChanged?.Invoke(currentTime);
    }

    public void SetGameDuration(float durationSeconds)
    {
        gameDuration = Mathf.Max(1f, durationSeconds);
        if (CurrentState != GameState.Playing)
        {
            currentTime = gameDuration;
            OnTimeChanged?.Invoke(currentTime);
        }
    }

    public void StartGame()
    {
        awaitingFinalShot = false;
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

    private void HandleShotResolved()
    {
        if (!awaitingFinalShot)
        {
            return;
        }

        awaitingFinalShot = false;
        EndGame();
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
