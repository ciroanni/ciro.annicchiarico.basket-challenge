using UnityEngine;

public class ShotContext : MonoBehaviour
{
    public TrajectoryCalculator.ShotType LastShotType { get; private set; } = TrajectoryCalculator.ShotType.Miss;

    public void SetShotType(TrajectoryCalculator.ShotType shotType)
    {
        LastShotType = shotType;
    }
}
