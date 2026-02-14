using UnityEngine;

public class GameInputStateBinder : MonoBehaviour
{
    [SerializeField] private GameStateController stateController;
    [SerializeField] private InputController inputController;

    private void OnEnable()
    {
        if (stateController != null)
        {
            stateController.OnStateChanged += HandleStateChanged;
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
        }
    }

    private void HandleStateChanged(GameStateController.GameState state)
    {
        if (inputController != null)
        {
            inputController.enabled = state == GameStateController.GameState.Playing;
        }
    }
}
