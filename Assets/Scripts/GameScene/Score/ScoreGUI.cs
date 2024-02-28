using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreGUI : MonoBehaviour
{

    private ScoreController scoreController;

    private TextMeshPro scoreText;
    // Start is called before the first frame update
    void Start()
    {
        scoreController = FindObjectOfType<ScoreController>();
        scoreText = GetComponentInChildren<TextMeshPro>();

    }

    public void ChangeScoreValue(string scoreValue)
    {
        scoreText.text = scoreValue;
    }
}
