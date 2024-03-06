using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GoalGameSceneHandler : MonoBehaviour
{
    [SerializeField] private TeamManager teamManager;
    [SerializeField] private ScoreController scoreController;
    public UnityEvent m_OnGoalScored;

    public GameObject ball;
    public GameObject player1;
    public GameObject player2;

    // Start is called before the first frame update
    void Start()
    {
        if (m_OnGoalScored == null)
            m_OnGoalScored = new UnityEvent();
        scoreController = FindAnyObjectByType<ScoreController>();
        m_OnGoalScored.AddListener(OnGoalScored);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnGoalScored()
    {

        Debug.Log("Scored !!!");

    }

    #region Getter

    public TeamManager GetTeamManager() { return teamManager; }
    public ScoreController GetScoreController() { return scoreController; }
    public GameObject GetBall() { return ball; }
    #endregion
}
