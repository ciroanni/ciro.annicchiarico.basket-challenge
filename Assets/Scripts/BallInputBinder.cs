using System;
using UnityEngine;

public class BallInputBinder : MonoBehaviour
{
    [SerializeField] private BallController ballController;
    [SerializeField] private InputController inputController;

    private void OnEnable()
    {
        if (inputController != null)
        {
            inputController.OnPerfectThrow += HandlePerfectThrow;
            inputController.OnNotPerfectThrow += HandleNotPerfectThrow;
            inputController.OnMissThrow += HandleMissThrow;
            inputController.OnSwipe += HandleSwipe;
            inputController.OnReset += HandleReset;
        }
    }

    private void OnDisable()
    {
        if (inputController != null)
        {
            inputController.OnPerfectThrow -= HandlePerfectThrow;
            inputController.OnNotPerfectThrow -= HandleNotPerfectThrow;
            inputController.OnMissThrow -= HandleMissThrow;
            inputController.OnSwipe -= HandleSwipe;
            inputController.OnReset -= HandleReset;
        }
    }

    private void HandlePerfectThrow()
    {
        if (ballController == null)
        {
            return;
        }

        ballController.ThrowPerfectBall();
    }

    private void HandleMissThrow()
    {
        if (ballController == null)
        {
            return;
        }

        ballController.ThrowMissBall();
    }

    private void HandleNotPerfectThrow()
    {
        if (ballController == null)
        {
            return;
        }

        ballController.ThrowNotPerfectBall();
    }

    private void HandleSwipe(Vector2 swipeDelta, float swipeDuration)
    {
        if (ballController == null)
        {
            return;
        }

        ballController.ThrowFromSwipe(swipeDelta, swipeDuration);
    }

    private void HandleReset()
    {
        if (ballController != null)
        {
            ballController.ResetBall();
        }
    }
}
