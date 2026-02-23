using UnityEngine;
using UnityEngine.UI;

public class FireballController : MonoBehaviour
{
    [SerializeField] private PointTrigger pointTrigger;
    [SerializeField] private BallController ballController;
    [SerializeField] private GameStateController stateController;

    [Header("Fireball Settings")]
    [SerializeField] private int streakThreshold = 3;
    [SerializeField] private float fireballDuration = 6f;
    [SerializeField] private int fireballScoreMultiplier = 2;

    [Header("UI")]
    [SerializeField] private Slider fireballSlider;

    private Rigidbody ballRigidbody;
    private int streakCount;
    private bool isFireballActive;
    private float fireballTimer;

    public bool IsFireballActive => isFireballActive;

    public int GetScoreMultiplier()
    {
        if (!isFireballActive)
        {
            return 1;
        }

        return Mathf.Max(1, fireballScoreMultiplier);
    }

    private void Awake()
    {
        if (ballController == null)
        {
            ballController = GetComponent<BallController>();
        }

        if (ballRigidbody == null)
        {
            ballRigidbody = GetComponent<Rigidbody>();
        }

        ConfigureSlider();
        UpdateMeter();
    }

    private void OnEnable()
    {
        if (pointTrigger != null)
        {
            pointTrigger.OnShotScored += HandleShotScored;
        }

        if (ballController != null)
        {
            ballController.OnShotMissed += HandleShotMissed;
        }

        if (stateController != null)
        {
            stateController.OnGameStart += HandleGameStart;
        }
    }

    private void OnDisable()
    {
        if (pointTrigger != null)
        {
            pointTrigger.OnShotScored -= HandleShotScored;
        }

        if (ballController != null)
        {
            ballController.OnShotMissed -= HandleShotMissed;
        }

        if (stateController != null)
        {
            stateController.OnGameStart -= HandleGameStart;
        }
    }

    private void Update()
    {
        if (!isFireballActive)
        {
            return;
        }

        fireballTimer -= Time.deltaTime;
        if (fireballTimer <= 0f)
        {
            ResetFireball();
            return;
        }

        UpdateMeter();
    }

    private void HandleShotScored(Rigidbody scoredBody)
    {
        if (ballRigidbody == null || scoredBody != ballRigidbody)
        {
            return;
        }

        if (isFireballActive)
        {
            return;
        }

        if (streakThreshold <= 0)
        {
            ActivateFireball();
            return;
        }

        streakCount = Mathf.Clamp(streakCount + 1, 0, streakThreshold);
        if (streakCount >= streakThreshold)
        {
            ActivateFireball();
        }
        else
        {
            UpdateMeter();
        }
    }

    private void HandleShotMissed()
    {
        ResetFireball();
    }

    private void HandleGameStart()
    {
        ResetFireball();
    }

    private void ActivateFireball()
    {
        isFireballActive = true;
        fireballTimer = Mathf.Max(0f, fireballDuration);
        UpdateMeter();
    }

    private void ResetFireball()
    {
        isFireballActive = false;
        fireballTimer = 0f;
        streakCount = 0;
        UpdateMeter();
    }

    private void ConfigureSlider()
    {
        if (fireballSlider == null)
        {
            return;
        }

        fireballSlider.minValue = 0f;
        fireballSlider.maxValue = 1f;
        fireballSlider.value = 0f;
    }

    private void UpdateMeter()
    {
        if (fireballSlider == null)
        {
            return;
        }

        if (isFireballActive)
        {
            float normalized = fireballDuration > 0f ? fireballTimer / fireballDuration : 0f;
            fireballSlider.value = Mathf.Clamp01(normalized);
        }
        else
        {
            float normalized = streakThreshold > 0 ? (float)streakCount / streakThreshold : 0f;
            fireballSlider.value = Mathf.Clamp01(normalized);
        }
    }
}