using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(LineRenderer))]
public class SwipeFeedbackRenderer : MonoBehaviour
{
    [SerializeField] private Camera inputCamera;
    [SerializeField] private float worldZ = 5f;
    [SerializeField] private float minPointDistance = 0.05f;
    [SerializeField] private float maxLineLength = 2f;
    [SerializeField] private bool enableMouseInput = true;
    [SerializeField] private bool enableTouchInput = true;

    private LineRenderer lineRenderer;
    private readonly List<Vector3> points = new List<Vector3>();
    private float currentLength;
    private bool pointerDown;
    private bool pointerStartedOverUI;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        if (inputCamera == null)
        {
            inputCamera = Camera.main;
        }
        ClearLine();
    }

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
    }

    private void HandleTouch()
    {
        Touch touch = Input.GetTouch(0);
        switch (touch.phase)
        {
            case TouchPhase.Began:
                BeginPointer(touch.position, touch.fingerId);
                break;
            case TouchPhase.Moved:
            case TouchPhase.Stationary:
                UpdatePointer(touch.position);
                break;
            case TouchPhase.Ended:
            case TouchPhase.Canceled:
                EndPointer();
                break;
        }
    }

    private void HandleMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            BeginPointer(Input.mousePosition, -1);
        }

        if (Input.GetMouseButton(0))
        {
            UpdatePointer(Input.mousePosition);
        }

        if (Input.GetMouseButtonUp(0))
        {
            EndPointer();
        }
    }

    private void BeginPointer(Vector2 screenPosition, int pointerId)
    {
        pointerDown = true;
        pointerStartedOverUI = EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(pointerId);
        ClearLine();
        if (pointerStartedOverUI)
        {
            return;
        }
        AddPoint(screenPosition);
    }

    private void UpdatePointer(Vector2 screenPosition)
    {
        if (!pointerDown || pointerStartedOverUI)
        {
            return;
        }

        AddPoint(screenPosition);
    }

    private void EndPointer()
    {
        pointerDown = false;
        pointerStartedOverUI = false;
        ClearLine();
    }

    private void AddPoint(Vector2 screenPosition)
    {
        if (inputCamera == null)
        {
            return;
        }

        Vector3 worldPoint = inputCamera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, worldZ));
        if (points.Count > 0)
        {
            float segmentLength = Vector3.Distance(points[points.Count - 1], worldPoint);
            if (segmentLength < minPointDistance)
            {
                return;
            }

            if (maxLineLength > 0f)
            {
                float remaining = maxLineLength - currentLength;
                if (remaining <= 0f)
                {
                    return;
                }

                if (segmentLength > remaining)
                {
                    Vector3 direction = (worldPoint - points[points.Count - 1]).normalized;
                    worldPoint = points[points.Count - 1] + direction * remaining;
                    segmentLength = remaining;
                }

                currentLength += segmentLength;
            }
        }

        points.Add(worldPoint);
        lineRenderer.positionCount = points.Count;
        lineRenderer.SetPositions(points.ToArray());
    }

    private void ClearLine()
    {
        points.Clear();
        lineRenderer.positionCount = 0;
        currentLength = 0f;
    }
}
