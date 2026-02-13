using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject gameplayUI;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject gameOverUI;

    [SerializeField] private float gameDuration = 60f;

    private float currentTime;

    public enum GameState
    {
        Menu,
        Playing,
        GameOver
    }
    public GameState CurrentState { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        CurrentState = GameState.Menu;
        mainMenuUI.SetActive(true);
        gameplayUI.SetActive(false);
        gameOverUI.SetActive(false);
    }

    public void StartGame()
    {
        CurrentState = GameState.Playing;
        currentTime = gameDuration;
        mainMenuUI.SetActive(false);
        gameplayUI.SetActive(true);
        gameOverUI.SetActive(false);
    }

    public void EndGame()
    {
        CurrentState = GameState.GameOver;
        mainMenuUI.SetActive(false);
        gameplayUI.SetActive(false);
        gameOverUI.SetActive(true);
    }

    public void RestartGame()
    {
        StartGame();
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentState == GameState.Playing)
        {
            currentTime -= Time.deltaTime;
            if (timerText != null)
            {
                timerText.text = $"{currentTime:F2}s";
            }

            if (currentTime <= 0)
            {
                EndGame();
            }

        }
    }
}
