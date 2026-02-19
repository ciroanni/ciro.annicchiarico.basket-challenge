using System;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField] private Rigidbody ballRigidbody;
    [SerializeField] private TrajectoryCalculator trajectoryCalculator;
    [SerializeField] private GameStateController stateController;
    [SerializeField] private ShotContext shotContext;
    [SerializeField] private Transform playerRoot;
    [SerializeField] private Transform ballHoldPoint;
    [SerializeField] private PointTrigger pointTrigger;
    [SerializeField] private Transform hoopLookTarget;
    [SerializeField] private Transform[] shootingPositions;
    [SerializeField] private float resultTimeout = 3f;
    [SerializeField] private float returnDelay = 2f;
    [Header("Swipe Settings")]
    [SerializeField] private float minSwipeLength = 60f;
    [SerializeField] private float maxSwipeLength = 450f;
    [SerializeField] private float minSwipeSpeed = 300f;
    [SerializeField] private float maxSwipeSpeed = 2000f;
    [SerializeField] private float minPowerMultiplier = 0.7f;
    [SerializeField] private float maxPowerMultiplier = 1.3f;

    private Vector3 startPosition;
    private int currentPositionIndex;
    private bool isInFlight;
    private bool awaitingResult;
    private bool scoredThisShot;
    private Coroutine resultCoroutine;
    private Coroutine returnCoroutine;

    public bool IsInFlight => isInFlight;
    public event Action OnShotLaunched;
    public event Action OnShotResolved;
    public event Action StopFollowing; // to notify camera to stop following to avoid visual bug

    private void Awake()
    {
        if (ballRigidbody == null)
        {
            ballRigidbody = GetComponent<Rigidbody>();
        }

        if (shotContext == null)
        {
            shotContext = GetComponent<ShotContext>();
        }
        startPosition = transform.position;
        AttachToHand();
    }

    private void OnEnable()
    {
        if (stateController != null)
        {
            stateController.OnGameStart += ResetBall;
        }

        if (pointTrigger != null)
        {
            pointTrigger.OnShotScored += HandleShotScored;
        }
    }

    private void OnDisable()
    {
        if (stateController != null)
        {
            stateController.OnGameStart -= ResetBall;
        }

        if (pointTrigger != null)
        {
            pointTrigger.OnShotScored -= HandleShotScored;
        }
    }

    public void ThrowPerfectBall()
    {
        ThrowWithDefaultSwipe(TrajectoryCalculator.ShotType.Perfect);
    }

    public void ThrowNotPerfectBall()
    {
        ThrowWithDefaultSwipe(TrajectoryCalculator.ShotType.NotPerfect);
    }

    public void ThrowMissBall()
    {
        ThrowWithDefaultSwipe(TrajectoryCalculator.ShotType.Miss);
    }

    public void ThrowFromSwipe(Vector2 swipeDelta, float swipeDuration)
    {
        if (trajectoryCalculator == null || ballRigidbody == null || isInFlight)
        {
            return;
        }

        float swipeLength = swipeDelta.magnitude;
        float swipeSpeed = swipeLength / Mathf.Max(swipeDuration, 0.01f);
        float multiplier = GetPowerMultiplier(swipeLength, swipeSpeed);
        Vector3 idealVelocity = trajectoryCalculator.CalculateIdealVelocityToHoop(transform.position);
        if (idealVelocity == Vector3.zero)
        {
            return;
        }

        LaunchBall(idealVelocity * multiplier);
    }

    public void ResetBall()
    {
        if (ballRigidbody != null)
        {
            ballRigidbody.velocity = Vector3.zero;
            ballRigidbody.angularVelocity = Vector3.zero;
        }

        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
            returnCoroutine = null;
        }

        if (resultCoroutine != null)
        {
            StopCoroutine(resultCoroutine);
            resultCoroutine = null;
        }

        isInFlight = false;
        awaitingResult = false;
        scoredThisShot = false;
        OnShotLaunched?.Invoke(); // just to notify camere to restart following
        AdvancePosition();
        MovePlayerToCurrentPosition();
        AttachToHand();
    }

    private void LaunchBall(Vector3 velocity)
    {
        isInFlight = true;
        awaitingResult = true;
        scoredThisShot = false;
        OnShotLaunched?.Invoke();
        shotContext?.BeginShot();
        transform.SetParent(null);
        ballRigidbody.isKinematic = false;
        ballRigidbody.velocity = Vector3.zero;
        ballRigidbody.angularVelocity = Vector3.zero;
        ballRigidbody.AddForce(velocity, ForceMode.VelocityChange);

        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
        }

        if (resultCoroutine != null)
        {
            StopCoroutine(resultCoroutine);
        }

        resultCoroutine = StartCoroutine(WaitForResultTimeout());
    }

    private System.Collections.IEnumerator WaitForResultTimeout()
    {
        yield return new WaitForSeconds(resultTimeout);
        if (awaitingResult && isInFlight)
        {
            HandleShotResult();
        }
    }

    private System.Collections.IEnumerator ReturnToHandAfterDelay()
    {
        yield return new WaitForSeconds(returnDelay);
        isInFlight = false;
        OnShotLaunched?.Invoke();
        if (scoredThisShot)
        {
            AdvancePosition();
        }
        MovePlayerToCurrentPosition();
        AttachToHand();
        returnCoroutine = null;
    }

    private void AttachToHand()
    {
        if (ballRigidbody != null)
        {
            ballRigidbody.velocity = Vector3.zero;
            ballRigidbody.angularVelocity = Vector3.zero;
            ballRigidbody.isKinematic = true;
        }

        if (ballHoldPoint != null)
        {
            transform.SetParent(ballHoldPoint, false);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
        else
        {
            transform.SetParent(null);
            transform.position = startPosition;
        }
    }

    private void AdvancePosition()
    {
        currentPositionIndex = GetRandomPositionIndex(currentPositionIndex);
    }

    private float GetPowerMultiplier(float swipeLength, float swipeSpeed)
    {
        float lengthT = Mathf.InverseLerp(minSwipeLength, maxSwipeLength, swipeLength);
        float speedT = Mathf.InverseLerp(minSwipeSpeed, maxSwipeSpeed, swipeSpeed);
        float t = Mathf.Clamp01((lengthT + speedT) * 0.5f);
        return Mathf.Lerp(minPowerMultiplier, maxPowerMultiplier, t);
    }

    private void ThrowWithDefaultSwipe(TrajectoryCalculator.ShotType shotType)
    {
        if(trajectoryCalculator == null || ballRigidbody == null || isInFlight)
        {
            return;
        }

        LaunchBall(trajectoryCalculator.CalculateShotVelocity(transform.position, shotType));
    }

    private int GetRandomPositionIndex(int excludeIndex)
    {
        if (shootingPositions == null || shootingPositions.Length == 0)
        {
            return 0;
        }

        if (shootingPositions.Length == 1)
        {
            return 0;
        }

        int index = excludeIndex;
        while (index == excludeIndex)
        {
            index = UnityEngine.Random.Range(0, shootingPositions.Length);
        }

        return index;
    }

    private void MovePlayerToCurrentPosition()
    {
        if (playerRoot == null || shootingPositions == null || shootingPositions.Length == 0)
        {
            return;
        }

        Transform target = shootingPositions[currentPositionIndex];
        if (target == null)
        {
            return;
        }

        Quaternion rotation = target.rotation;
        if (hoopLookTarget != null)
        {
            Vector3 lookDirection = hoopLookTarget.position - target.position;
            lookDirection.y = 0f;
            if (lookDirection.sqrMagnitude > 0.001f)
            {
                rotation = Quaternion.LookRotation(lookDirection.normalized, Vector3.up);
            }
        }

        playerRoot.SetPositionAndRotation(target.position, rotation);
    }

    private void HandleShotScored(Rigidbody scoredBody)
    {
        if (ballRigidbody == null || scoredBody != ballRigidbody)
        {
            return;
        }

        scoredThisShot = true;
        HandleShotResult();
    }

    private void HandleShotResult()
    {
        if (!awaitingResult)
        {
            return;
        }

        awaitingResult = false;
        OnShotResolved?.Invoke();
        if (resultCoroutine != null)
        {
            StopCoroutine(resultCoroutine);
            resultCoroutine = null;
        }

        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
        }

        returnCoroutine = StartCoroutine(ReturnToHandAfterDelay());
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Rim") || collision.gameObject.CompareTag("Backboard") || collision.gameObject.CompareTag("Ground"))
        {
            StopFollowing?.Invoke();
        }
    }
}
