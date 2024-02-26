using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalHandler : MonoBehaviour
{

    public GameObject goal;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("OK JSUIS PRET TIRER LE BALLON DEDANS LO");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider infoCollision)
    {
        if (infoCollision.gameObject.tag == "Ball")
        {
            Debug.Log("IL A MARQUE TROPO FROOT");
        }
    }
}
