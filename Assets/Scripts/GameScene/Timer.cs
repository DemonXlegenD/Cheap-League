using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.Rendering.DebugUI;

public class Timer : MonoBehaviour
{
    GameManager gameManager;
    [SerializeField] private List<TimerGUI> timerGuiList = new List<TimerGUI>();

    [SerializeField] private UnityEvent OnStartTimer;
    [SerializeField] private UnityEvent OnPauseTimer;
    [SerializeField] private UnityEvent OnEndTimer;

    //Each timer is in seconds
    [SerializeField] private float MaxTimer = 90;
    [SerializeField] private float CurrentTimer;
    [SerializeField] private string formatTimer;

    private bool IsStarted = false;
    private bool IsPaused = false;
    private bool IsEnded= false;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        ResetTimer();
        StartTimer();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsStarted && !IsPaused)
        {
            CurrentTimer -= Time.deltaTime;
            if (CurrentTimer <= 0 )
            {
                EndTimer();
            }
            UpdateTimerUI();
        }
     
    }

    public void StartTimer()
    {
        IsStarted = true;
        IsPaused = false;
        OnStartTimer?.Invoke();
    }

    public void PauseTimer()
    {
        IsPaused = true;
        OnPauseTimer?.Invoke();
    }

    public void EndTimer()
    {
        IsStarted = false;
        IsPaused = false;
        IsEnded = true;
        CurrentTimer = 0;
        OnEndTimer?.Invoke();
        gameManager.ChangeScene("Menu Scene");
    }


    public void ResetTimer()
    {
        IsStarted = false;
        IsPaused = false;
        IsEnded = false;
        CurrentTimer = MaxTimer;
    }

    private void UpdateTimerUI()
    {
        foreach (var timerGUI in timerGuiList)
        {
            timerGUI.ChangeScoreValue(GetFormattedTime());
        }
    }


    #region Getter

    public float GetMaxTimer() { return MaxTimer; } 
    public float GetCurrentTimer() { return CurrentTimer; } 

    public bool GetIsStarted() { return IsStarted; }
    public bool GetIsPaused() { return IsPaused; }
    public bool GetIsEnded() { return IsEnded; }
    public string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(CurrentTimer / 60);
        int seconds = Mathf.FloorToInt(CurrentTimer % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }


    #endregion

    #region Setter

    public void SetMaxTimer(float value) { MaxTimer = value; }
    public void SetCurrentTimer(float value) { CurrentTimer = value; }

    public void SetIsStarted(bool value) { IsStarted = value; }
    public void SetIsPaused(bool value) { IsPaused = value; }
    public void SetIsEnded(bool value) { IsEnded = value; }

    #endregion
}
