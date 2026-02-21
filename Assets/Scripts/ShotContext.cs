using UnityEngine;

public class ShotContext : MonoBehaviour
{
    public enum ShooterType
    {
        Player,
        Opponent
    }

    [SerializeField] private string rimTag = "Rim";
    [SerializeField] private string backboardTag = "Backboard";
    [SerializeField] private ShooterType shooter = ShooterType.Player;

    public bool TouchedRim { get; private set; }
    public bool TouchedBackboard { get; private set; }
    public ShooterType Shooter => shooter;

    public void BeginShot()
    {
        TouchedRim = false;
        TouchedBackboard = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        HandleContact(collision.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleContact(other.gameObject);
    }

    private void HandleContact(GameObject other)
    {
        if (!string.IsNullOrEmpty(rimTag) && other.CompareTag(rimTag))
        {
            TouchedRim = true;
        }

        if (!string.IsNullOrEmpty(backboardTag) && other.CompareTag(backboardTag))
        {
            TouchedBackboard = true;
        }
    }
}
