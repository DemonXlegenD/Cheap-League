using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalHandler : MonoBehaviour
{
    private MainHandler mainHandler;
    [SerializeField] private ColorNameTeam colorTeam;

    void Start()
    {
        mainHandler = GameObject.Find("Workspace").GetComponent<MainHandler>();
    }

    void Update()
    {

    }

    void OnTriggerEnter(Collider infoCollision)
    {
        if (infoCollision.gameObject.tag == "Ball")
        {
            Debug.Log(colorTeam + " SCORED !");
            mainHandler.GetTeamManager().GetTeamByColor(colorTeam).GetComponent<Team>().TeamHasScored();
            mainHandler.m_OnGoalScored.Invoke();
            this.GetComponent<ParticleSystem>().Play();
            this.GetComponent<AudioSource>().Play();
            this.GetComponent<AudioSource>().time = 0.7f;
        }
    }
}