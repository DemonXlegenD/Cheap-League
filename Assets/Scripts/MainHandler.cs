using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class MainHandler : MonoBehaviour
{
    GameManager gameManager;
    [SerializeField] private TeamManager teamManager;
    [SerializeField] private ScoreController scoreController;
    [SerializeField] private Timer timer;
    public UnityEvent m_OnGoalScored;

    public GameObject ball;
    public GameObject player1;
    public GameObject player2;
    public GameObject BlueTeam;
    public GameObject OrangeTeam;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameManager.Instance;
        if (m_OnGoalScored == null)
            m_OnGoalScored = new UnityEvent();
        scoreController = FindAnyObjectByType<ScoreController>();
        m_OnGoalScored.AddListener(OnGoalScored);
        timer.SetIsPaused(true);
    }
    private void AddP2()
    {
        BlueTeam.GetComponent<Team>().AddPlayer(player2);
    }
    // Update is called once per frame
    void Update()
    {
        Debug.Log(timer.GetIsEnded());
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

    public void OnPlayerJoin(PlayerInput playerInput)
    {
        Debug.Log(playerInput.gameObject.name);
        if (playerInput.gameObject != player1)
        {
            playerInput.gameObject.name = "Player2";
            player2 = playerInput.gameObject;
            AddP2();
        }
        timer.SetIsPaused(false);
        timer.StartTimer();

        StartCoroutine(OnTheStart());

        teamManager.ShowInformation("Start", Color.red);
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
    }

    public void OnGoalScored()
    {
        StartCoroutine(Scored());
    }
    IEnumerator OnTheStart()
    {

        yield return new WaitForSeconds(3.5f);
    }


    IEnumerator Scored()
    {

        yield return new WaitForSeconds(3.5f);

        Team team = teamManager.TeamWon();
        if (team != null)
        {
            teamManager.ShowWin(team.name, team.GetTeamColorR());
            StartCoroutine(Win());
        }
        else
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
        }
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
