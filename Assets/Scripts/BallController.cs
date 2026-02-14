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

    private void Update()
    {
        if (stateController != null && stateController.CurrentState != GameStateController.GameState.Playing)
        {
            return;
        }

        HandleInput();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ThrowPerfectBall();
        } 

        if (Input.GetKeyDown(KeyCode.N))
        {
            ThrowNotPerfectBall();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            ThrowMissBall();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetBall();
        }
    }

    private void ThrowPerfectBall()
    {
        if (trajectoryCalculator != null)
        {
            Vector3 velocity = trajectoryCalculator.CalculateShotVelocity(transform.position, TrajectoryCalculator.ShotType.Perfect);
            ballRigidbody.velocity = Vector3.zero; // Reset velocity before applying new force
            ballRigidbody.angularVelocity = Vector3.zero; // Reset angular velocity before applying new force
            ballRigidbody.AddForce(velocity, ForceMode.VelocityChange);
        }
    }

    private void ThrowNotPerfectBall()
    {
        if (trajectoryCalculator != null)
        {
            Vector3 velocity = trajectoryCalculator.CalculateShotVelocity(transform.position, TrajectoryCalculator.ShotType.NotPerfect);
            ballRigidbody.velocity = Vector3.zero; // Reset velocity before applying new force
            ballRigidbody.angularVelocity = Vector3.zero; // Reset angular velocity before applying new force
            ballRigidbody.AddForce(velocity, ForceMode.VelocityChange);
        }
    }

    private void ThrowMissBall()
    {
        if (trajectoryCalculator != null)
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
