using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointTrigger : MonoBehaviour
{
    [SerializeField] private ShotEvaluator shotEvaluator;
    private readonly HashSet<Rigidbody> scoredBodies = new HashSet<Rigidbody>();

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            if (scoredBodies.Contains(rb))
            {
                return;
            }

            if (rb.velocity.y < 0) // check if the ball is moving downwards, so im sure it went through the hoop
            {
                if (shotEvaluator == null)
                {
                    Debug.LogWarning("ShotEvaluator missing in scene.");
                    return;
                }

                ShotContext shotContext = rb.GetComponent<ShotContext>();
                TrajectoryCalculator.ShotType shotType = TrajectoryCalculator.ShotType.NotPerfect;
                if (shotContext != null)
                {
                    shotType = shotContext.LastShotType;
                }

                scoredBodies.Add(rb);
                ShotEvaluator.ShotResult result = shotEvaluator.Evaluate(shotType);
                ScoreManager.Instance?.AddPoints(result.Points);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            scoredBodies.Remove(rb);
        }
    }
}
