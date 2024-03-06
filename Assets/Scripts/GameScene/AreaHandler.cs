using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaHandler : MonoBehaviour
{
    [SerializeField] private ColorNameTeam colorTeam;
    private GoalGameSceneHandler gameSceneHandler;
    void Start()
    {
        gameSceneHandler = GameObject.Find("Workspace").GetComponent<GoalGameSceneHandler>();
    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider infoCollision)
    {
        if (infoCollision.gameObject.tag == "Ball")
        {
            Destroy(infoCollision.gameObject);
            Debug.Log(colorTeam + " SCORED !");
            gameSceneHandler.GetTeamManager().GetTeamByColor(colorTeam).GetComponent<Team>().TeamHasScored();
            gameSceneHandler.m_OnGoalScored.Invoke();
        }
    }
}