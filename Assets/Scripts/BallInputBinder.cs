using UnityEngine;

public class BallInputBinder : MonoBehaviour
{
    [SerializeField] private BallController ballController;
    [SerializeField] private InputController inputController;
    [SerializeField] private float longSwipeThreshold = 250f;

    private void OnEnable()
    {
        if (inputController != null)
        {
            inputController.OnTap += HandleTap;
            inputController.OnSwipe += HandleSwipe;
            inputController.OnReset += HandleReset;
        }
    }

    private void OnDisable()
    {
        if (inputController != null)
        {
            inputController.OnTap -= HandleTap;
            inputController.OnSwipe -= HandleSwipe;
            inputController.OnReset -= HandleReset;
        }
    }

    private void HandleTap(Vector2 screenPosition)
    {
        if (ballController != null)
        {
            ballController.ThrowPerfectBall();
        }
    }

    private void HandleSwipe(Vector2 swipeDelta)
    {
        if (ballController == null)
        {
            return;
        }

        if (swipeDelta.magnitude >= longSwipeThreshold)
        {
            ballController.ThrowMissBall();
        }
        else
        {
            ballController.ThrowNotPerfectBall();
        }
    }

    private void HandleReset()
    {
        if (ballController != null)
        {
            ballController.ResetBall();
        }
    }
}
