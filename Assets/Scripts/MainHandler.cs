using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MainHandler : MonoBehaviour
{
    public UnityEvent m_OnGoalScored;

    [SerializeField] private TeamManager teamManager;
    [SerializeField] private ScoreController scoreController;
    [SerializeField] private GameObject ball;

    // Start is called before the first frame update
    void Start()
    {
        if (m_OnGoalScored == null)
            m_OnGoalScored = new UnityEvent();
        scoreController = new ScoreController();
        m_OnGoalScored.AddListener(OnGoalScored);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnGoalScored()
    {
        ball.transform.position = new Vector3(0, 1.0f, 0);
        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        Debug.Log("Scored !!!");
    }

    #region Getter

    public TeamManager GetTeamManager() { return teamManager; }
    public ScoreController GetScoreController() { return scoreController; }
    public GameObject GetBall() { return ball; }
    #endregion
}
