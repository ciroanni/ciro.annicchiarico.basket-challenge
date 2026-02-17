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

    public event Action<Vector2, float> OnSwipe;
    public event Action OnReset;

    # region Debug events
    public event Action OnPerfectThrow;
    public event Action OnNotPerfectThrow;
    public event Action OnMissThrow;
    # endregion

    private bool pointerDown;
    private Vector2 pointerStart;
    private float pointerStartTime;
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
                pointerStartTime = Time.time;
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
            pointerStartTime = Time.time;
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
            OnPerfectThrow?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.N))
        {
            if (enableDebugLogs)
            {
                Debug.Log("Keyboard swipe (N)");
            }
            OnNotPerfectThrow?.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            if (enableDebugLogs)
            {
                Debug.Log("Keyboard swipe long (M)");
            }
            OnMissThrow?.Invoke();
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
        float duration = Mathf.Max(Time.time - pointerStartTime, 0.01f);
        pointerDown = false;

        if (delta.magnitude >= minSwipeDistance)
        {
            if (enableDebugLogs)
            {
                Debug.Log("Swipe detected");
            }
            OnSwipe?.Invoke(delta, duration);
        }
    }
}
