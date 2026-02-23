using System;
using System.Collections.Generic;
using UnityEngine;

public class PointTrigger : MonoBehaviour
{
    [SerializeField] private ShotEvaluator shotEvaluator;
    [SerializeField] private BackboardBonusController backboardBonus;
    private readonly HashSet<Rigidbody> scoredBodies = new HashSet<Rigidbody>();
    public event Action<Rigidbody> OnShotScored;
    public event Action<ShotEvaluator.ShotResult, ShotContext.ShooterType> OnShotScoredResult;
    public event Action<int, ShotContext.ShooterType> OnBonusScored;

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            if (scoredBodies.Contains(rb))
            {
                Debug.Log($"Already scored for this Rigidbody: {rb.gameObject.name}");
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
                int points = result.Points;
                ShotContext.ShooterType shooter = shotContext != null ? shotContext.Shooter : ShotContext.ShooterType.Player;
                FireballController fireball = rb.GetComponent<FireballController>();
                if (fireball != null)
                {
                    int multiplier = fireball.GetScoreMultiplier();
                    points *= Mathf.Max(1, multiplier);
                }

                ShotEvaluator.ShotResult finalResult = new ShotEvaluator.ShotResult(points, result.Label);
                Debug.Log($"Shot scored! Type: {shotType}, Points: {finalResult.Points}");
                ScoreManager.Instance?.AddPoints(shooter, finalResult.Points);
                OnShotScoredResult?.Invoke(finalResult, shooter);

                if (shotContext != null && shotContext.TouchedBackboard && backboardBonus != null)
                {
                    if (backboardBonus.TryConsumeBonus(out int bonusPoints))
                    {
                        Debug.Log($"Backboard bonus awarded: {bonusPoints}");
                        ScoreManager.Instance?.AddPoints(shooter, bonusPoints);
                        OnBonusScored?.Invoke(bonusPoints, shooter);
                    }
                }

                OnShotScored?.Invoke(rb);
            }
            else
            {
                Debug.Log($"Ball entered trigger but is not moving downwards: {rb.gameObject.name}, Velocity: {rb.velocity}");
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
