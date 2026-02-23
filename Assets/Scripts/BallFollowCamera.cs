using UnityEngine;

public class BallFollowCamera : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform ballTarget;
    [SerializeField] private Transform hoopTarget;
    [SerializeField] private BallController ball;
    [SerializeField] private Vector3 playerOffset = new Vector3(0f, 5f, -8f);
    [SerializeField] private Vector3 ballOffset = new Vector3(0f, 5f, -8f);
    [SerializeField] private Vector3 lookAtOffset = new Vector3(0f, 1f, 0f);
    [SerializeField] private float positionSmoothTime = 0.15f;
    [SerializeField] private float lookSmoothTime = 0.12f;

    private Vector3 velocity;
    private Vector3 lookVelocity;
    private Vector3 currentLookPoint;
    private bool lookInitialized;
    private bool followActive;
    private bool snapPending;

    private void OnEnable()
    {
        if (ball != null)
        {
            ball.OnResetBall += HandleShotResolved;
            ball.OnShotLaunched += HandleStartFollowing;
            ball.OnShotResolved += HandleShotResolved;
            ball.StopFollowing += HandleShotResolved;
            ball.OnPlayerTeleported += HandlePlayerTeleported;
            followActive = ball.IsInFlight;
        }
    }

    private void OnDisable()
    {
        if (ball != null)
        {
            ball.OnResetBall -= HandleShotResolved;
            ball.OnShotLaunched -= HandleStartFollowing;
            ball.OnShotResolved -= HandleShotResolved;
            ball.StopFollowing -= HandleShotResolved;
            ball.OnPlayerTeleported -= HandlePlayerTeleported;
        }
    }

    private void LateUpdate()
    {
        if (player == null || ballTarget == null)
        {
            return;
        }

        if (followActive)
        {
            FollowTarget();
            SmoothLookAt(GetLookPoint(ballTarget.position));
        }
        else if (snapPending)
        {
            SnapToPlayer();
            snapPending = false;
        }
    }

    private void FollowTarget()
    {
        Vector3 desiredPosition = new Vector3(
            player.position.x + ballOffset.x,
            ballTarget.position.y + ballOffset.y,
            ballTarget.position.z + ballOffset.z
        );
        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, positionSmoothTime);
    }

    private void SmoothLookAt(Vector3 targetLookPoint)
    {
        if (!lookInitialized)
        {
            currentLookPoint = targetLookPoint;
            lookInitialized = true;
        }

        currentLookPoint = Vector3.SmoothDamp(currentLookPoint, targetLookPoint, ref lookVelocity, lookSmoothTime);
        transform.LookAt(currentLookPoint, Vector3.up);
    }

    private void SnapToPlayer()
    {
        Vector3 desiredPosition = player.TransformPoint(playerOffset);
        transform.position = desiredPosition;
        velocity = Vector3.zero;
        Vector3 lookPoint = GetLookPoint(player.position);
        currentLookPoint = lookPoint;
        lookVelocity = Vector3.zero;
        lookInitialized = true;
        transform.LookAt(lookPoint, Vector3.up);
    }

    private Vector3 GetLookPoint(Vector3 mainTargetPosition)
    {
        if (hoopTarget != null)
        {
            Vector3 midpoint = (mainTargetPosition + hoopTarget.position) * 0.5f;
            return midpoint + lookAtOffset;
        }

        return mainTargetPosition + lookAtOffset;
    }

    private void HandleStartFollowing()
    {
        followActive = true;
    }

    private void HandleShotResolved()
    {
        followActive = false;
    }

    private void HandlePlayerTeleported()
    {
        snapPending = true;
    }


}
