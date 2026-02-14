using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputController : MonoBehaviour
{
    [SerializeField] private bool enableMouseInput = true;
    [SerializeField] private bool enableTouchInput = true;
    [SerializeField] private bool enableKeyboardDebug = true;
    [SerializeField] private bool enableDebugLogs = true;
    [SerializeField] private float minSwipeDistance = 50f;

    public event Action<Vector2> OnTap;
    public event Action<Vector2> OnSwipe;
    public event Action OnReset;

    private bool pointerDown;
    private Vector2 pointerStart;
    private bool pointerStartedOverUI;

    private void Update()
    {
        if (enableTouchInput && Input.touchCount > 0)
        {
            HandleTouch();
            return;
        }

        if (enableMouseInput)
        {
            HandleMouse();
        }

        if (enableKeyboardDebug)
        {
            HandleKeyboard();
        }
    }

    private void HandleTouch()
    {
        Touch touch = Input.GetTouch(0);
        switch (touch.phase)
        {
            case TouchPhase.Began:
                pointerDown = true;
                pointerStart = touch.position;
                pointerStartedOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touch.fingerId);
                if (enableDebugLogs)
                {
                    Debug.Log("Touch began");
                }
                break;
            case TouchPhase.Ended:
                if (pointerStartedOverUI)
                {
                    pointerDown = false;
                    return;
                }
                if (enableDebugLogs)
                {
                    Debug.Log("Touch ended");
                }
                EvaluateGesture(touch.position);
                break;
            case TouchPhase.Canceled:
                pointerDown = false;
                break;
        }
    }

    private void HandleMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            pointerDown = true;
            pointerStart = Input.mousePosition;
            pointerStartedOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
            if (enableDebugLogs)
            {
                Debug.Log("Mouse down");
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (pointerStartedOverUI)
            {
                pointerDown = false;
                return;
            }
            if (enableDebugLogs)
            {
                Debug.Log("Mouse up");
            }
            EvaluateGesture(Input.mousePosition);
        }
    }

    private void HandleKeyboard()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (enableDebugLogs)
            {
                Debug.Log("Keyboard tap (Space)");
            }
            OnTap?.Invoke(Vector2.zero);
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            if (enableDebugLogs)
            {
                Debug.Log("Keyboard swipe (N)");
            }
            OnSwipe?.Invoke(Vector2.up * minSwipeDistance);
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (enableDebugLogs)
            {
                Debug.Log("Keyboard swipe long (M)");
            }
            OnSwipe?.Invoke(Vector2.up * minSwipeDistance * 2f);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (enableDebugLogs)
            {
                Debug.Log("Keyboard reset (R)");
            }
            OnReset?.Invoke();
        }
    }

    private void EvaluateGesture(Vector2 endPosition)
    {
        if (!pointerDown)
        {
            return;
        }

        Vector2 delta = endPosition - pointerStart;
        pointerDown = false;

        if (delta.magnitude >= minSwipeDistance)
        {
            if (enableDebugLogs)
            {
                Debug.Log("Swipe detected");
            }
            OnSwipe?.Invoke(delta);
        }
        else
        {
            if (enableDebugLogs)
            {
                Debug.Log("Tap detected");
            }
            OnTap?.Invoke(endPosition);
        }
    }
}
