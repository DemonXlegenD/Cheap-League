using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalHandler : MonoBehaviour
{
    private MainHandler mainHandler;
    public string Team;
    // Start is called before the first frame update
    void Start()
    {
        mainHandler = GameObject.Find("Workspace").GetComponent<MainHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider infoCollision)
    {
        if (infoCollision.gameObject.tag == "Ball")
        {
            Debug.Log(Team + " SCORED !");

            mainHandler.m_OnGoalScored.Invoke();
        }
    }
}
