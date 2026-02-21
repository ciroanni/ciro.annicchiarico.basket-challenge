using TMPro;
using UnityEngine;

public class ScoreFlyerSpawner : MonoBehaviour
{
    [SerializeField] private PointTrigger pointTrigger;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Canvas parentCanvas;
    [SerializeField] private ScoreFlyer flyerPrefab;
    [SerializeField] private Color perfectColor = new Color(1f, 0.9f, 0.3f, 1f);
    [SerializeField] private Color goodColor = new Color(0.6f, 1f, 0.6f, 1f);
    [SerializeField] private Color bonusColor = new Color(0.4f, 0.8f, 1f, 1f);
    [SerializeField] private Vector3 stackOffset = new Vector3(0f, 0.25f, 0f);
    [SerializeField] private float stackWindow = 0.15f;

    private float lastSpawnTime;
    private int stackIndex;

    private void OnEnable()
    {
        if (pointTrigger != null)
        {
            pointTrigger.OnShotScoredResult += HandleShotScored;
            pointTrigger.OnBonusScored += HandleBonusScored;
        }
    }

    private void OnDisable()
    {
        if (pointTrigger != null)
        {
            pointTrigger.OnShotScoredResult -= HandleShotScored;
            pointTrigger.OnBonusScored -= HandleBonusScored;
        }
    }

    private void HandleShotScored(ShotEvaluator.ShotResult result, ShotContext.ShooterType shooter)
    {
        if (shooter == ShotContext.ShooterType.Opponent)
        {
            return;
        }

        if (result.Points <= 0)
        {
            return;
        }

        Color color = result.Label == "Perfect" ? perfectColor : goodColor;
        SpawnFlyer(result.Label, color);
    }

    private void HandleBonusScored(int bonusPoints, ShotContext.ShooterType shooter)
    {
        if (shooter == ShotContext.ShooterType.Opponent)
        {
            return;
        }

        if (bonusPoints <= 0)
        {
            return;
        }

        SpawnFlyer("Bonus +" + bonusPoints, bonusColor);
    }

    private void SpawnFlyer(string text, Color color)
    {
        if (flyerPrefab == null)
        {
            return;
        }

        Transform target = spawnPoint != null ? spawnPoint : transform;
        UpdateStackIndex();
        Vector3 spawnPosition = target.position + stackOffset * stackIndex;
        ScoreFlyer instance = Instantiate(flyerPrefab, spawnPosition, target.rotation, null);
        instance.transform.SetParent(parentCanvas.transform, true);
        instance.Setup(text, color);
    }

    private void UpdateStackIndex()
    {
        if (Time.time - lastSpawnTime > stackWindow)
        {
            stackIndex = 0;
        }
        else
        {
            stackIndex++;
        }

        lastSpawnTime = Time.time;
    }
}
