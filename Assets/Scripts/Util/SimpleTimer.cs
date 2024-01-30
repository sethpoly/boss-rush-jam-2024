using System;
using System.Collections;
using UnityEngine;

public class SimpleTimer : MonoBehaviour
{
    public bool timerEnded;
    private int _timeLeft;
    public int TimeLeft
    {
        get { return _timeLeft; }
        private set { _timeLeft = value; }
    }
    public float TimeLeftClamped
    {
        get { return TimeLeft / targetTime; }
    }

    private int min, sec;
    private int targetTime;
    private Action onTimerEnded;

    // Constructor
    public void Init(int minutes, int seconds, Action onTimerEnded, bool startTimer = false)
    {
        min = minutes;
        sec = seconds;
        this.onTimerEnded = onTimerEnded;
        int targetTime = min * 60 + sec;
        TimeLeft = targetTime;
        if (startTimer)
            StartTimer();
    }

    public void StartTimer()
    {
        StartCoroutine(Clock());
    }

    public void ResetTimer(bool keepActive = false)
    {
        int targetTime = min * 60 + sec;
        TimeLeft = targetTime;
        if (keepActive)
            StartTimer();
    }

    public void SetTime(int minutes, int seconds)
    {
        min = minutes;
        sec = seconds;
    }

    private IEnumerator Clock()
    {
        while (TimeLeft > 0)
        {
            TimeLeft--;
            timerEnded = false;
            yield return new WaitForSeconds(1);
        }
        TimerEnded();
    }

    private void TimerEnded()
    {
        timerEnded = true;
        onTimerEnded.Invoke();
    }
}
