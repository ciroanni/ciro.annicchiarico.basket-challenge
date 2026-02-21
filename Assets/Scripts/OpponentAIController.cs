using System.Collections;
using UnityEngine;

public class OpponentAIController : MonoBehaviour
{
    public enum DifficultyLevel
    {
        Easy,
        Normal,
        Hard
    }

    [SerializeField] private GameStateController stateController;
    [SerializeField] private BallController ballController;
    [SerializeField] private float minShotInterval = 1.2f;
    [SerializeField] private float maxShotInterval = 2.4f;
    [SerializeField] private DifficultyLevel difficulty = DifficultyLevel.Normal;
    [SerializeField] private AnimationCurve perfectChanceCurve = AnimationCurve.EaseInOut(0f, 0.1f, 1f, 0.6f);
    [SerializeField] private AnimationCurve missChanceCurve = AnimationCurve.EaseInOut(0f, 0.6f, 1f, 0.1f);

    private Coroutine shootingRoutine;

    private void OnEnable()
    {
        if (stateController != null)
        {
            stateController.OnGameStart += HandleGameStart;
            stateController.OnGameEnd += HandleGameEnd;
            stateController.OnStateChanged += HandleStateChanged;
        }
    }

    private void OnDisable()
    {
        if (stateController != null)
        {
            stateController.OnGameStart -= HandleGameStart;
            stateController.OnGameEnd -= HandleGameEnd;
            stateController.OnStateChanged -= HandleStateChanged;
        }

        StopShooting();
    }

    public void SetDifficulty(DifficultyLevel level)
    {
        difficulty = level;
    }

    public void SetDifficultyIndex(int index)
    {
        difficulty = (DifficultyLevel)Mathf.Clamp(index, 0, 2);
    }

    private void HandleGameStart()
    {
        StartShooting();
    }

    private void HandleGameEnd()
    {
        StopShooting();
    }

    private void HandleStateChanged(GameStateController.GameState state)
    {
        if (state == GameStateController.GameState.Playing)
        {
            StartShooting();
        }
        else
        {
            StopShooting();
        }
    }

    private void StartShooting()
    {
        if (shootingRoutine != null)
        {
            return;
        }

        if (ballController == null)
        {
            return;
        }

        shootingRoutine = StartCoroutine(ShootingLoop());
    }

    private void StopShooting()
    {
        if (shootingRoutine == null)
        {
            return;
        }

        StopCoroutine(shootingRoutine);
        shootingRoutine = null;
    }

    private IEnumerator ShootingLoop()
    {
        while (true)
        {
            float delay = Random.Range(minShotInterval, maxShotInterval);
            yield return new WaitForSeconds(delay);

            if (stateController != null && !stateController.IsAcceptingShots)
            {
                continue;
            }

            if (ballController == null || ballController.IsInFlight)
            {
                continue;
            }

            TakeShot();
        }
    }

    private void TakeShot()
    {
        float difficultyValue = GetDifficultyValue();
        float perfectChance = Mathf.Clamp01(perfectChanceCurve.Evaluate(difficultyValue));
        float missChance = Mathf.Clamp01(missChanceCurve.Evaluate(difficultyValue));
        float goodChance = Mathf.Clamp01(1f - perfectChance - missChance);

        float roll = Random.value;
        if (roll <= missChance)
        {
            ballController.ThrowMissBall();
            return;
        }

        if (roll <= missChance + perfectChance)
        {
            ballController.ThrowPerfectBall();
            return;
        }

        if (goodChance > 0f)
        {
            ballController.ThrowNotPerfectBall();
        }
        else
        {
            ballController.ThrowPerfectBall();
        }
    }

    private float GetDifficultyValue()
    {
        switch (difficulty)
        {
            case DifficultyLevel.Easy:
                return 0f;
            case DifficultyLevel.Hard:
                return 1f;
            default:
                return 0.5f;
        }
    }
}
