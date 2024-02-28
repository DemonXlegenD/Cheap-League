using System.Collections.Generic;
using UnityEngine;

public class ScoreController : MonoBehaviour
{
    [SerializeField] private List<Score> m_ScoresList = new List<Score>();
    [SerializeField] private Dictionary<GameObject, Score> m_ScoresDict = new Dictionary<GameObject, Score>(); //Le GameObject attendu est le player ici

    public void Awake()
    {
    }

    #region Getter
    public Score GetScoreByIndex(int index)
    {
        if (index >= m_ScoresList.Count) return null;
        return m_ScoresList[index];
    }

    public Score GetScoreByPlayer(GameObject player)
    {
        if (player == null) return null;
        if (m_ScoresDict.TryGetValue(player, out Score score)) return score;
        return null;
    }

    public List<Score> GetScores() { return m_ScoresList; }
    #endregion

    public void PlayerScore(GameObject player, int addAmountToScore = 1)
    {
        if (player == null) return;
        m_ScoresDict[player].IncreaseScore(addAmountToScore);
    }


    #region ResetScores
    public void ResetScores() { foreach (var _score in m_ScoresList) _score.ResetScore(); }

    public void ResetScoreOfPlayer(GameObject player)
    {
        if (player == null) return;
        m_ScoresDict[player].ResetScore();
    }
    #endregion

    public void OnDestroy()
    {
        m_ScoresList.Clear();
        m_ScoresDict.Clear();
    }
}
