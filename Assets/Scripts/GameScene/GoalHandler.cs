using UnityEngine;

public class GoalHandler : MonoBehaviour
{
    private MainHandler mainHandler;
    [SerializeField] private ColorNameTeam colorTeam;
    [SerializeField] private Color color;
    [SerializeField] private string teamName;

    void Start()
    {
        mainHandler = GameObject.Find("Workspace").GetComponent<MainHandler>();
        if (colorTeam == ColorNameTeam.Orange)
        {
            color = new Color(255, 30, 0, 255);
            teamName = "ORANGETEAM";
        }
        else if (colorTeam == ColorNameTeam.Blue)
        {
            color = new Color(0, 0, 255, 255);
            teamName = "BLUETEAM";
        }
        else
        {
            color = new Color(0, 0, 0, 255);
            teamName = "WHITETEAM";
        }
    }

    void OnTriggerEnter(Collider infoCollision)
    {
        if (infoCollision.gameObject.tag == "Ball")
        {
            mainHandler.GetTeamManager().GetTeamByColor(colorTeam).GetComponent<Team>().TeamHasScored();
            mainHandler.GetTeamManager().ShowInformation(teamName + " SCORED", color);
            mainHandler.m_OnGoalScored.Invoke();
            this.GetComponent<ParticleSystem>().Play();
            this.GetComponent<AudioSource>().Play();
            this.GetComponent<AudioSource>().time = 0.7f;
        }
    }
}