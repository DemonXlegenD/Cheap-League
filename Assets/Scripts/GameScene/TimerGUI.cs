using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerGUI : MonoBehaviour
{

    private TextMeshProUGUI timerText;
    // Start is called before the first frame update
    private void Awake()
    {
        timerText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update()
    {

    }
    public void ChangeScoreValue(string timerValue)
    {
        timerText.text = timerValue;
    }
}