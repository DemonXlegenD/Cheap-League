using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreGUI : MonoBehaviour
{

    private TextMeshProUGUI scoreText;
    // Start is called before the first frame update
    public void Awake()
    {
        scoreText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Update()
    {

    }
    public void ChangeScoreValue(string scoreValue)
    {
        scoreText.text = scoreValue;
    }
}
