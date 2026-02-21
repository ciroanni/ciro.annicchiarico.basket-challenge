using UnityEngine;
using UnityEngine.UI;

public class SwipePowerBar : MonoBehaviour
{
    [SerializeField] private InputController inputController;
    [SerializeField] private BallController ballController;
    [SerializeField] private Slider powerSlider;

    private void Awake()
    {
        if (powerSlider == null)
        {
            powerSlider = GetComponent<Slider>();
        }

        if (powerSlider != null)
        {
            powerSlider.minValue = 0f;
            powerSlider.maxValue = 1f;
            powerSlider.value = 0f;
        }
    }

    private void OnEnable()
    {
        if (inputController != null)
        {
            inputController.OnSwipeProgress += HandleSwipeProgress;
            inputController.OnSwipeStart += HandleSwipeStart;
            inputController.OnSwipeEnd += HandleSwipeEnd;
        }
    }

    private void OnDisable()
    {
        if (inputController != null)
        {
            inputController.OnSwipeProgress -= HandleSwipeProgress;
            inputController.OnSwipeStart -= HandleSwipeStart;
            inputController.OnSwipeEnd -= HandleSwipeEnd;
        }
    }

    private void HandleSwipeStart()
    {
        SetValue(0f);
    }

    private void HandleSwipeEnd()
    {
        SetValue(0f);
    }

    private void HandleSwipeProgress(Vector2 swipeDelta, float swipeDuration)
    {
        if (powerSlider == null || ballController == null)
        {
            return;
        }

        float swipeLength = swipeDelta.magnitude;
        float swipeSpeed = swipeLength / Mathf.Max(swipeDuration, 0.01f);
        float lengthT = Mathf.InverseLerp(ballController.MinSwipeLength, ballController.MaxSwipeLength, swipeLength);
        float speedT = Mathf.InverseLerp(ballController.MinSwipeSpeed, ballController.MaxSwipeSpeed, swipeSpeed);
        float t = Mathf.Clamp01((lengthT + speedT) * 0.5f);
        SetValue(t);
    }

    private void SetValue(float value)
    {
        if (powerSlider != null)
        {
            powerSlider.value = value;
        }
    }
}
