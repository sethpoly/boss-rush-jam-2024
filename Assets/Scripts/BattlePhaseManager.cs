using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BattlePhaseManager : MonoBehaviour
{
    public LevelLoader levelLoader;
    public int minutes;
    public int seconds;
    private SimpleTimer battlePhaseTimer;
    

    void Awake()
    {
        OnBattlePhaseStart();
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
        battlePhaseTimer.Init(minutes, seconds, OnBattleTimerExpired, true);
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
}
