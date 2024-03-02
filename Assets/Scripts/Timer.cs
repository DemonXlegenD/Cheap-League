using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static UnityEngine.Rendering.DebugUI;

public class Timer : MonoBehaviour
{
    [SerializeField] private UnityEvent OnStartTimer;
    [SerializeField] private UnityEvent OnPauseTimer;
    [SerializeField] private UnityEvent OnEndTimer;

    //Each timer is in seconds
    [SerializeField] private float MaxTimer = 90;
    [SerializeField] private float CurrentTimer;

    private bool IsStarted = false;
    private bool IsPaused = false;
    private bool IsEnded= false;

    // Start is called before the first frame update
    void Start()
    {
        ResetTimer();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsStarted && !IsPaused)
        {
            CurrentTimer -= Time.deltaTime;   
        }

        if (CurrentTimer <= 0)
        {
            IsEnded = true;
            IsStarted = false;
            IsPaused = false;
        }
    }

    public void ResetTimer()
    {
        IsStarted = false;
        IsPaused = false;
        IsEnded = false;
        CurrentTimer = MaxTimer;
    }


    #region Getter

    public float GetMaxTimer() { return MaxTimer; } 
    public float GetCurrentTimer() { return CurrentTimer; } 

    public bool GetIsStarted() { return IsStarted; }
    public bool GetIsPaused() { return IsPaused; }
    public bool GetIsEnded() { return IsEnded; }

    #endregion

    #region Setter

    public void SetMaxTimer(float value) { MaxTimer = value; }
    public void SetCurrentTimer(float value) { CurrentTimer = value; }

    public void SetIsStarted(bool value) { IsStarted = value; }
    public void SetIsPaused(bool value) { IsPaused = value; }
    public void SetIsEnded(bool value) { IsEnded = value; }

    #endregion
}
