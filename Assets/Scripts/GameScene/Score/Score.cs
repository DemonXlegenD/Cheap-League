using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [SerializeField, Min(0)] private int maxScore = 10;
    [SerializeField, Min(0)] private int currentScore = 0;

    [SerializeField] private List<ScoreGUI> scoreGuiList = new List<ScoreGUI>();

    public void Start()
    {
        ResetScore();
    }

    public bool IsCurrentScoreMaxed() { return currentScore == maxScore; }

    public void IncreaseScore(int amount = 1)
    {


        if (IsCurrentScoreMaxed())
        {
            Debug.Log("U WIN");
        }
        else
        {
            currentScore += amount;
        }
        UpdateScoreUI();
    }

    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        foreach (var scoreGui in scoreGuiList)
        {
            scoreGui.ChangeScoreValue(currentScore.ToString());
        }
    }

    #region Getter

    public int GetCurrentScore() { return currentScore; }

    public int GetMaxScore() { return maxScore; }

    public bool IsScoreAtteint() { return maxScore == currentScore; }

    #endregion
}