using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GoalGameSceneHandler : MonoBehaviour
{
    GameManager gameManager;
    [SerializeField] private TeamManager teamManager;
    [SerializeField] private ScoreController scoreController;
    [SerializeField] private Timer timer;
    [SerializeField] private EndlessTerrain endlessTerrain;
    public UnityEvent m_OnGoalScored;

    public GameObject ball;
    public GameObject player1;
    public GameObject player2;
    public GameObject OrangeTeam;
    public GameObject BlueTeam;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        endlessTerrain = FindAnyObjectByType<EndlessTerrain>();
        if (m_OnGoalScored == null)
            m_OnGoalScored = new UnityEvent();
        scoreController = FindAnyObjectByType<ScoreController>();
        timer.SetIsPaused(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (timer.GetIsEnded())
        {
            Team currentTeam = teamManager.TeamEndTime();
            if (currentTeam == null)
            {
                teamManager.ShowWin("DRAW : NO TEAM", Color.red);
            }
            else
            {
                teamManager.ShowWin(currentTeam.name, currentTeam.GetTeamColorR());
            }
            StartCoroutine(Win());
        }
    }

    private void AddP2()
    {
        BlueTeam.GetComponent<Team>().AddPlayer(player2);
        endlessTerrain.AddViewer(player2.transform);

    }

    public void OnPlayerJoin(PlayerInput playerInput)
    {
        if (playerInput.gameObject != player1)
        {
            playerInput.gameObject.name = "Player2";
            playerInput.transform.position = new Vector3(0, 7f, 35f);
            playerInput.transform.rotation = Quaternion.Euler(0, 180f, 0);
            player2 = playerInput.gameObject;
            AddP2();
        }
        timer.SetIsPaused(false);
        timer.StartTimer();

        StartCoroutine(OnTheStart());

        teamManager.ShowInformation("Start", Color.red);
        ball.transform.position = new Vector3(0, 7.0f, 0);
        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        player1.transform.position = new Vector3(0, 7, -35);
        player1.transform.rotation = Quaternion.Euler(0, 0, 0);
        player1.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player1.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        if (player2 != null)
        {
            player2.transform.position = new Vector3(0, 7, 35);
            player2.transform.rotation = Quaternion.Euler(0, 180, 0);
            player2.GetComponent<Rigidbody>().velocity = Vector3.zero;
            player2.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
    }
    IEnumerator OnTheStart()
    {

        yield return new WaitForSeconds(3.5f);
    }



    IEnumerator Win()
    {
        yield return new WaitForSeconds(5f);
        gameManager.ChangeScene("Menu Scene");
    }

    #region Getter

    public TeamManager GetTeamManager() { return teamManager; }
    public ScoreController GetScoreController() { return scoreController; }
    public GameObject GetBall() { return ball; }
    #endregion
}
