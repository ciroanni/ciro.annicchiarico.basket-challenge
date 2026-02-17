using UnityEngine;

public class BallFollowCamera : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private BallController ball;
    [SerializeField] private Vector3 followOffset = new Vector3(0f, 5f, -8f);
    [SerializeField] private Vector3 lookAtOffset = new Vector3(0f, 1f, 0f);
    [SerializeField] private float positionSmoothTime = 0.15f;

    private Vector3 velocity;
    private bool followActive;

    private void OnEnable()
    {
        if (ball != null)
        {
            ball.OnShotLaunched += HandleShotLaunched;
            ball.OnShotResolved += HandleShotResolved;
            followActive = ball.IsInFlight;
        }
    }

    private void OnDisable()
    {
        if (ball != null)
        {
            ball.OnShotLaunched -= HandleShotLaunched;
            ball.OnShotResolved -= HandleShotResolved;
        }
    }

    private void LateUpdate()
    {
        if (target == null)
        {
            return;
        }

        if (followActive)
        {
            FollowTarget();
        }

        LookAtTarget();

    }
    private void FollowTarget()
    {
        Vector3 desiredPosition = target.TransformPoint(followOffset);

        transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, positionSmoothTime);

    }

    private void LookAtTarget()
    {
        Vector3 lookPoint = target.position + lookAtOffset;
        transform.LookAt(lookPoint, Vector3.up);
    }

    private void HandleShotLaunched()
    {
        followActive = true;
    }

    private void HandleShotResolved()
    {
        followActive = false;
    }


}
