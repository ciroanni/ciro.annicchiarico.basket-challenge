using System.Collections;
using UnityEngine;

public class BackboardBonusController : MonoBehaviour
{
    private enum BonusTier
    {
        Common,
        Rare,
        VeryRare
    }

    [SerializeField] private GameStateController stateController;
    [SerializeField] private Renderer[] backboardRenderers;
    [SerializeField] private Color highlightColor = new Color(1f, 0.85f, 0.2f, 1f);
    [SerializeField] private float blinkInterval = 0.25f;
    [SerializeField] private float activeDuration = 5f;
    [SerializeField] private float minSpawnDelay = 3f;
    [SerializeField] private float maxSpawnDelay = 6f;
    [SerializeField] private float rareChance = 0.2f;
    [SerializeField] private float veryRareChance = 0.05f;
    [SerializeField] private int commonPoints = 4;
    [SerializeField] private int rarePoints = 6;
    [SerializeField] private int veryRarePoints = 8;

    private MaterialPropertyBlock propertyBlock;
    private Coroutine spawnCoroutine;
    private Coroutine blinkCoroutine;
    private Coroutine activeCoroutine;
    private bool bonusActive;
    private BonusTier currentTier;
    private int lastScore;

    public bool IsActive => bonusActive;

    void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();
    }

    private void OnDisable()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged -= HandleScoreChanged;
            ScoreManager.Instance.OnScoreReset -= HandleScoreReset;
        }

        if (stateController != null)
        {
            stateController.OnGameEnd -= HandleGameEnd;
        }

        StopAllCoroutines();
        ClearHighlight();
        bonusActive = false;
    }

    void Start()
    {
        if (ScoreManager.Instance != null)
        {
            lastScore = ScoreManager.Instance.GetScore();
            ScoreManager.Instance.OnScoreChanged += HandleScoreChanged;
            ScoreManager.Instance.OnScoreReset += HandleScoreReset;
        }

        if (stateController != null)
        {
            stateController.OnGameEnd += HandleGameEnd;
        }
    }

    private void HandleScoreChanged(int newScore)
    {
        if (newScore < lastScore)
        {
            lastScore = newScore;
            return;
        }

        lastScore = newScore;
        ScheduleBonusSpawn();
    }

    private void HandleScoreReset()
    {
        lastScore = 0;
    }

    private void HandleGameEnd()
    {
        StopAllCoroutines();
        ClearHighlight();
        bonusActive = false;
    }

    private void ScheduleBonusSpawn()
    {
        if (bonusActive || spawnCoroutine != null)
        {
            return;
        }

        spawnCoroutine = StartCoroutine(SpawnAfterDelay());
    }

    private IEnumerator SpawnAfterDelay()
    {
        float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
        yield return new WaitForSeconds(delay);
        spawnCoroutine = null;
        ActivateBonus();
    }

    private void ActivateBonus()
    {
        currentTier = RollTier();
        bonusActive = true;

        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
        }
        blinkCoroutine = StartCoroutine(BlinkRoutine());

        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
        }
        activeCoroutine = StartCoroutine(ExpireAfterDuration());
    }

    private IEnumerator BlinkRoutine()
    {
        bool highlightOn = false;
        while (bonusActive)
        {
            highlightOn = !highlightOn;
            ApplyHighlight(highlightOn);
            yield return new WaitForSeconds(blinkInterval);
        }

        ClearHighlight();
    }

    private IEnumerator ExpireAfterDuration()
    {
        yield return new WaitForSeconds(activeDuration);
        DeactivateBonus();
    }

    private void DeactivateBonus()
    {
        bonusActive = false;

        if (blinkCoroutine != null)
        {
            StopCoroutine(blinkCoroutine);
            blinkCoroutine = null;
        }

        if (activeCoroutine != null)
        {
            StopCoroutine(activeCoroutine);
            activeCoroutine = null;
        }

        ClearHighlight();
    }

    public bool TryConsumeBonus(out int bonusPoints)
    {
        bonusPoints = 0;
        if (!bonusActive)
        {
            return false;
        }

        bonusPoints = GetTierPoints(currentTier);
        DeactivateBonus();
        return true;
    }

    private BonusTier RollTier()
    {
        float roll = Random.value;
        if (roll < veryRareChance)
        {
            return BonusTier.VeryRare;
        }

        if (roll < veryRareChance + rareChance)
        {
            return BonusTier.Rare;
        }

        return BonusTier.Common;
    }

    private int GetTierPoints(BonusTier tier)
    {
        switch (tier)
        {
            case BonusTier.VeryRare:
                return veryRarePoints;
            case BonusTier.Rare:
                return rarePoints;
            default:
                return commonPoints;
        }
    }

    private void ApplyHighlight(bool enabled)
    {
        if (backboardRenderers == null || backboardRenderers.Length == 0)
        {
            return;
        }

        foreach (Renderer renderer in backboardRenderers)
        {
            if (renderer == null)
            {
                continue;
            }

            if (enabled)
            {
                propertyBlock.Clear();
                if (renderer.sharedMaterial != null && renderer.sharedMaterial.HasProperty("_BaseColor"))
                {
                    propertyBlock.SetColor("_BaseColor", highlightColor);
                    renderer.SetPropertyBlock(propertyBlock);
                }
                else if (renderer.sharedMaterial != null && renderer.sharedMaterial.HasProperty("_Color"))
                {
                    propertyBlock.SetColor("_Color", highlightColor);
                    renderer.SetPropertyBlock(propertyBlock);
                }
                else
                {
                    renderer.enabled = false;
                }
            }
            else
            {
                if (renderer.sharedMaterial != null && (renderer.sharedMaterial.HasProperty("_BaseColor") || renderer.sharedMaterial.HasProperty("_Color")))
                {
                    renderer.SetPropertyBlock(null);
                }
                else
                {
                    renderer.enabled = true;
                }
            }
        }
    }

    private void ClearHighlight()
    {
        ApplyHighlight(false);
    }
}
