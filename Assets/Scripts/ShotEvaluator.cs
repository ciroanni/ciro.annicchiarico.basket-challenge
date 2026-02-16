using UnityEngine;

public class ShotEvaluator : MonoBehaviour
{
    public struct ShotResult
    {
        public int Points;
        public string Label;

        public ShotResult(int points, string label)
        {
            Points = points;
            Label = label;
        }
    }

    public ShotResult Evaluate(TrajectoryCalculator.ShotType shotType)
    {
        switch (shotType)
        {
            case TrajectoryCalculator.ShotType.Perfect:
                return new ShotResult(3, "Perfect");
            case TrajectoryCalculator.ShotType.NotPerfect:
                return new ShotResult(2, "Good");
            case TrajectoryCalculator.ShotType.Miss:
                return new ShotResult(0, "Miss");
            default:
                return new ShotResult(0, "Miss");
        }
    }
}
