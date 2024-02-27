using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [SerializeField] private int maxScore = 10;
    [SerializeField] private int currentScore = 0;

    [SerializeField] private GameObject scoreTextGameObject;
    private TextMeshPro scoreText;

    [SerializeField]
    private GameObject player;

    public void Start()
    {
        player = transform.parent.parent.gameObject;
        scoreText = scoreTextGameObject.GetComponent<TextMeshPro>();
        if (player != null)
        {
            Debug.Log("Le GameObject Player a été récupéré avec succès!");
        }
        else
        {
            Debug.LogError("Impossible de trouver le GameObject Player.");
        }
    }

    public void IncreaseScore(int amount = 1)
    {
        currentScore += amount;

        UpdateScoreUI();

        if (currentScore >= maxScore)
        {
            Debug.Log("U WIN");
        }
    }

    public void ResetScore()
    {
        currentScore = 0;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = currentScore.ToString();
        }
    }

    #region Getter

    public int GetCurrentScore() { return currentScore; }

    public int GetMaxScore() { return maxScore; }

    public TextMeshPro GetScoreText() { return scoreText; }

    public GameObject GetPlayer() { return player; }

    #endregion
}