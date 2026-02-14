using UnityEngine;

public class TrajectoryCalculator : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] private Transform hoopCenter;
    [SerializeField] private Transform backboardCenter;

    public enum ShotType
    {
        Perfect,
        NotPerfect,
        Miss
    }


    public Vector3 CalculateShotVelocity(Vector3 startPoint, ShotType shotType)
    {
        Vector3 targetPoint = Vector3.zero;

        switch (shotType)
        {
            case ShotType.Perfect:
                bool useBackboard = Random.value > 0.5f; // randomly decide to use backboard or not
                Debug.Log(useBackboard ? "Perfect shot using backboard!" : "Perfect shot directly to hoop!");
                targetPoint = useBackboard ? backboardCenter.position : hoopCenter.position;
                break;
            case ShotType.NotPerfect:
                //hit the rim but not perfectly, so we can add some random offset to the hoop center
                Vector2 randomRimOffset = Random.insideUnitCircle.normalized * 0.35f; // 35cm offset in a random direction
                targetPoint = hoopCenter.position + new Vector3(randomRimOffset.x, 0, randomRimOffset.y);
                break;
            case ShotType.Miss:
                // miss the hoop entirely
                Vector2 randomOffset = Random.insideUnitCircle.normalized * 2f; // random offset within a radius of 2 units
                targetPoint = hoopCenter.position + new Vector3(randomOffset.x, 0, randomOffset.y);
                break;
        }

        return CalculateArcVelocity(startPoint, targetPoint);
    }

    public Vector3 CalculateArcVelocity(Vector3 start, Vector3 target)
    {
        // horizontal distance (XZ plane)
        Vector3 startXZ = new Vector3(start.x, 0, start.z);
        Vector3 targetXZ = new Vector3(target.x, 0, target.z);
        float distance = Vector3.Distance(startXZ, targetXZ);

        // desired height offset for the arc (the higher the distance, the higher the arc)
        float heightOffset = 2.0f + (distance * 0.5f);
        // calculate the maximum height of the arc
        float maxY = Mathf.Max(start.y, target.y) + heightOffset;

        float gravity = Mathf.Abs(Physics.gravity.y);
        Vector3 displacementXZ = new Vector3(target.x - start.x, 0, target.z - start.z);

        // vertical velocity Vy -> v = sqrt(2 * g * h)
        float verticalVelocity = Mathf.Sqrt(2 * gravity * (maxY - start.y));
        Vector3 velocityY = Vector3.up * verticalVelocity;

        // time to reach max height and then fall down to target
        // t = sqrt(2 * h / g)
        float timeUp = Mathf.Sqrt(2 * (maxY - start.y) / gravity);
        float timeDown = Mathf.Sqrt(2 * (maxY - target.y) / gravity);
        float totalTime = timeUp + timeDown;

        // horizontal velocity Vxz -> v = d / t
        Vector3 velocityXZ = displacementXZ / totalTime;

        return velocityXZ + velocityY;
    }
}
