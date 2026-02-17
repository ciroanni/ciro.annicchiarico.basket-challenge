using UnityEngine;

public class ShotContext : MonoBehaviour
{
    [SerializeField] private string rimTag = "Rim";
    [SerializeField] private string backboardTag = "Backboard";

    public bool TouchedRim { get; private set; }
    public bool TouchedBackboard { get; private set; }

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
            Debug.Log("Ball touched the rim" + other.name);
            TouchedRim = true;
        }

        if (!string.IsNullOrEmpty(backboardTag) && other.CompareTag(backboardTag))
        {
            TouchedBackboard = true;
        }
    }
}
