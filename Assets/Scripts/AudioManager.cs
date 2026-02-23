using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private GameStateController stateController;
    [SerializeField] private PointTrigger pointTrigger;

    [Header("Clips")]
    [SerializeField] private AudioClip timerExpiredClip;
    [SerializeField] private AudioClip scoreClip;
    [SerializeField] private AudioClip bonusScoreClip;
    [SerializeField] private AudioClip winClip;
    [SerializeField] private AudioClip loseClip;
    [SerializeField] private AudioClip drawClip;
    [SerializeField] private AudioClip uiClickClip;

    [Header("Volumes")]
    [Range(0f, 1f)][SerializeField] private float timerVolume = 1f;
    [Range(0f, 1f)][SerializeField] private float scoreVolume = 1f;
    [Range(0f, 1f)][SerializeField] private float bonusVolume = 1f;
    [Range(0f, 1f)][SerializeField] private float winLoseVolume = 1f;
    [Range(0f, 1f)][SerializeField] private float uiClickVolume = 0.7f;

    private readonly List<Button> boundButtons = new List<Button>();

    private void OnEnable()
    {
        if (stateController != null)
        {
            stateController.OnTimerExpired += HandleTimerExpired;
            stateController.OnGameEnd += HandleGameEnd;
        }

        if (pointTrigger != null)
        {
            pointTrigger.OnShotScoredResult += HandleShotScored;
            pointTrigger.OnBonusScored += HandleBonusScored;
        }

        BindUiButtons();
    }

    private void OnDisable()
    {
        if (stateController != null)
        {
            stateController.OnTimerExpired -= HandleTimerExpired;
            stateController.OnGameEnd -= HandleGameEnd;
        }

        if (pointTrigger != null)
        {
            pointTrigger.OnShotScoredResult -= HandleShotScored;
            pointTrigger.OnBonusScored -= HandleBonusScored;
        }
        
        UnbindUiButtons();
    }

    private void BindUiButtons()
    {
        boundButtons.Clear();
        Button[] buttons = FindObjectsOfType<Button>(true);
        foreach (Button button in buttons)
        {
            if (button == null)
            {
                continue;
            }

            button.onClick.AddListener(HandleUiClick);
            boundButtons.Add(button);
        }
    }

    private void UnbindUiButtons()
    {
        foreach (Button button in boundButtons)
        {
            if (button == null)
            {
                continue;
            }

            button.onClick.RemoveListener(HandleUiClick);
        }

        boundButtons.Clear();
    }

    private void HandleTimerExpired()
    {
        PlayOneShot(timerExpiredClip, timerVolume);
    }

    private void HandleShotScored(ShotEvaluator.ShotResult result, ShotContext.ShooterType shooter)
    {
        if (shooter != ShotContext.ShooterType.Player)
        {
            return;
        }

        if (result.Points <= 0)
        {
            return;
        }

        PlayOneShot(scoreClip, scoreVolume);
    }

    private void HandleBonusScored(int bonusPoints, ShotContext.ShooterType shooter)
    {
        if (shooter != ShotContext.ShooterType.Player)
        {
            return;
        }

        if (bonusPoints <= 0)
        {
            return;
        }

        PlayOneShot(bonusScoreClip, bonusVolume);
    }

    private void HandleGameEnd()
    {
        if (ScoreManager.Instance == null)
        {
            return;
        }

        int playerScore = ScoreManager.Instance.GetScore();
        int opponentScore = ScoreManager.Instance.GetOpponentScore();

        if (playerScore > opponentScore)
        {
            PlayOneShot(winClip, winLoseVolume);
        }
        else if (playerScore < opponentScore)
        {
            PlayOneShot(loseClip, winLoseVolume);
        }
        else
        {
            PlayOneShot(drawClip, winLoseVolume);
        }
    }

    private void HandleUiClick()
    {
        PlayOneShot(uiClickClip, uiClickVolume);
    }

    private void PlayOneShot(AudioClip clip, float volume)
    {
        if (clip == null)
        {
            return;
        }

        GameObject tempObject = new GameObject("SFX_" + clip.name);
        tempObject.transform.SetParent(transform, false);
        AudioSource tempSource = tempObject.AddComponent<AudioSource>();
        tempSource.playOnAwake = false;
        tempSource.clip = clip;
        tempSource.volume = volume;
        tempSource.spatialBlend = 0f;
        tempSource.Play();
        Destroy(tempObject, clip.length);
    }
}
