using System;
using System.Collections.Generic;
using UnityEngine;

public class PointTrigger : MonoBehaviour
{
    [SerializeField] private ShotEvaluator shotEvaluator;
    [SerializeField] private BackboardBonusController backboardBonus;
    private readonly HashSet<Rigidbody> scoredBodies = new HashSet<Rigidbody>();
    public event Action<Rigidbody> OnShotScored;

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
                if (shotContext != null && !shotContext.TouchedRim)
                {
                    shotType = TrajectoryCalculator.ShotType.Perfect;
                }

                scoredBodies.Add(rb);
                ShotEvaluator.ShotResult result = shotEvaluator.Evaluate(shotType);
                Debug.Log($"Shot scored! Type: {shotType}, Points: {result.Points}");
                ScoreManager.Instance?.AddPoints(result.Points);

                if (shotContext != null && shotContext.TouchedBackboard && backboardBonus != null)
                {
                    if (backboardBonus.TryConsumeBonus(out int bonusPoints))
                    {
                        Debug.Log($"Backboard bonus awarded: {bonusPoints}");
                        ScoreManager.Instance?.AddPoints(bonusPoints);
                    }
                }

                OnShotScored?.Invoke(rb);
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
