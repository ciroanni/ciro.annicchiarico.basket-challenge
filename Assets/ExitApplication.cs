using UnityEngine;
using UnityEngine.UI;

public class ExitApplication : MonoBehaviour
{
    [SerializeField] private Button exitButton;

    private void Awake()
    {
        if (exitButton == null)
        {
            exitButton = GetComponent<Button>();
        }
    }

    private void OnEnable()
    {
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(HandleExitClicked);
        }
    }

    private void OnDisable()
    {
        if (exitButton != null)
        {
            exitButton.onClick.RemoveListener(HandleExitClicked);
        }
    }

    private void HandleExitClicked()
    {
        Application.Quit();
    }
}
