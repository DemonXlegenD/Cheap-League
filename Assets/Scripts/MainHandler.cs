using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MainHandler : MonoBehaviour
{
    public UnityEvent m_OnGoalScored;

    public ScoreController scoreController;
    public GameObject ball;

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
        player1.transform.position = new Vector3(0, 2, -35);
        player1.transform.rotation = Quaternion.Euler(0, 0, 0);
        player1.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player1.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        player2.transform.position = new Vector3(0, 2, 35);
        player2.transform.rotation = Quaternion.Euler(0, 180, 0);
        player2.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player2.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        Debug.Log("Scored !!!");

    }
}
