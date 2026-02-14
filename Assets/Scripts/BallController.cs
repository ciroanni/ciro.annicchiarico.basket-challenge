using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField] private Rigidbody ballRigidbody;
    [SerializeField] private TrajectoryCalculator trajectoryCalculator;
    [SerializeField] private GameStateController stateController;
    private Vector3 startPosition;

    private void Awake()
    {
        if (ballRigidbody == null)
        {
            ballRigidbody = GetComponent<Rigidbody>();
        }
        startPosition = transform.position;
    }

    private void OnEnable()
    {
        if (stateController != null)
        {
            stateController.OnGameStart += ResetBall;
        }
    }

    private void OnDisable()
    {
        if (stateController != null)
        {
            stateController.OnGameStart -= ResetBall;
        }
    }

    public void ThrowPerfectBall()
    {
        if (trajectoryCalculator != null && ballRigidbody != null)
        {
            Vector3 velocity = trajectoryCalculator.CalculateShotVelocity(transform.position, TrajectoryCalculator.ShotType.Perfect);
            ballRigidbody.velocity = Vector3.zero; // Reset velocity before applying new force
            ballRigidbody.angularVelocity = Vector3.zero; // Reset angular velocity before applying new force
            ballRigidbody.AddForce(velocity, ForceMode.VelocityChange);
        }
    }

    public void ThrowNotPerfectBall()
    {
        if (trajectoryCalculator != null && ballRigidbody != null)
        {
            Vector3 velocity = trajectoryCalculator.CalculateShotVelocity(transform.position, TrajectoryCalculator.ShotType.NotPerfect);
            ballRigidbody.velocity = Vector3.zero; // Reset velocity before applying new force
            ballRigidbody.angularVelocity = Vector3.zero; // Reset angular velocity before applying new force
            ballRigidbody.AddForce(velocity, ForceMode.VelocityChange);
        }
    }

    public void ThrowMissBall()
    {
        if (trajectoryCalculator != null && ballRigidbody != null)
        {
            Vector3 velocity = trajectoryCalculator.CalculateShotVelocity(transform.position, TrajectoryCalculator.ShotType.Miss);
            ballRigidbody.velocity = Vector3.zero; // Reset velocity before applying new force
            ballRigidbody.angularVelocity = Vector3.zero; // Reset angular velocity before applying new force
            ballRigidbody.AddForce(velocity, ForceMode.VelocityChange);
        }
    }

    public void ResetBall()
    {
        if (ballRigidbody != null)
        {
            ballRigidbody.velocity = Vector3.zero;
            ballRigidbody.angularVelocity = Vector3.zero;
        }

        transform.position = startPosition;
    }
}
