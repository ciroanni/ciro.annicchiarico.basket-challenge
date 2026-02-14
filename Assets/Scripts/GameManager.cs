using UnityEngine;
public class GameManager : MonoBehaviour
{
    [SerializeField] private GameStateController stateController;

    private void Start()
    {
        if (stateController != null)
        {
            stateController.ShowMainMenu();
        }
    }

    public void ShowMainMenu()
    {
        if (stateController != null)
        {
            stateController.ShowMainMenu();
        }
    }

    public void StartGame()
    {
        if (stateController != null)
        {
            stateController.StartGame();
        }
    }

    public void EndGame()
    {
        if (stateController != null)
        {
            stateController.EndGame();
        }
    }

    public void RestartGame()
    {
        if (stateController != null)
        {
            stateController.RestartGame();
        }
    }

    
}
