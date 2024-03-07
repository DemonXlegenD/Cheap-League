using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class MainHandler : MonoBehaviour
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

    public void OnPlayerJoin(PlayerInput playerInput)
    {
        Debug.Log(playerInput.gameObject.name);
        if (playerInput.gameObject != player1)
        {
            playerInput.gameObject.name = "Player2";
            player2 = playerInput.gameObject;
        }
    }

    void OnGoalScored()
    {
        ball.transform.position = new Vector3(0, 1.0f, 0);
        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        player1.transform.position = new Vector3(0, 2, -35);
        player1.transform.rotation = Quaternion.Euler(0, 0, 0);
        player1.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player1.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        if (player2 != null)
        {
            player2.transform.position = new Vector3(0, 2, 35);
            player2.transform.rotation = Quaternion.Euler(0, 180, 0);
            player2.GetComponent<Rigidbody>().velocity = Vector3.zero;
            player2.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }

        Debug.Log("Scored !!!");

    }

    #region Getter

    public TeamManager GetTeamManager() { return teamManager; }
    public ScoreController GetScoreController() { return scoreController; }
    public GameObject GetBall() { return ball; }
    #endregion
}
