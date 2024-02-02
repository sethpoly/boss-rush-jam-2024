using System;
using TMPro;
using UnityEngine;

public class BattlePhaseManager : MonoBehaviour
{
    public LevelLoader levelLoader;
    public int minutes;
    public int seconds;
    public TextMeshProUGUI timerLabel;
    private SimpleTimer battlePhaseTimer;
    

    void Awake()
    {
        OnBattlePhaseStart();
    }

    void Update()
    {
        UpdateTimerLabel();
    }

    /// <summary>
    /// This function is called when the object becomes enabled and active.
    /// </summary>
    void OnEnable()
    {
        OnBattlePhaseResume();
    }

    private void OnBattlePhaseStart()
    {
        Debug.Log("Battle Phase Start");
        battlePhaseTimer = gameObject.AddComponent<SimpleTimer>();
        battlePhaseTimer.Init(minutes, seconds, OnBattleTimerExpired);
    }

    /// <summary>
    /// Resume the battle phase. Resets the timer - cleans up any additional resources
    /// </summary>
    private void OnBattlePhaseResume()
    {
        Debug.Log("Battle Phase Resume");
        ResetBattleTimer();
    }


    public void OnBattleTimerExpired()
    {
        Debug.Log("Battle Timer expired");
        levelLoader.LoadNextPhase();
    }

    private void ResetBattleTimer()
    {
        battlePhaseTimer.ResetTimer(keepActive: true);
    }

    private void UpdateTimerLabel()
    {
        if(battlePhaseTimer == null) return;
        TimeSpan time = TimeSpan.FromSeconds(battlePhaseTimer.TimeLeft);

        string formattedTime = time.ToString(@"\:ss");
        timerLabel.text = formattedTime;
    }
}
