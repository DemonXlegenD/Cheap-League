using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaHandler : MonoBehaviour
{
    
    private GoalGameSceneHandler gameSceneHandler;
    [SerializeField] private ColorNameTeam colorTeam;
    [SerializeField] private Color color;
    [SerializeField] private string teamName;

    void Start()
    {
        gameSceneHandler = GameObject.Find("Workspace").GetComponent<GoalGameSceneHandler>();
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

    void Update()
    {

    }

    void OnTriggerEnter(Collider infoCollision)
    {
        if (infoCollision.gameObject.tag == "Ball" || infoCollision.gameObject.tag == "SainteBall")
        {
            if(infoCollision.gameObject.tag == "SainteBall") Destroy(infoCollision.gameObject);
            gameSceneHandler.GetTeamManager().GetTeamByColor(colorTeam).GetComponent<Team>().TeamHasScored();
            gameSceneHandler.m_OnGoalScored.Invoke();
        }
    }
}