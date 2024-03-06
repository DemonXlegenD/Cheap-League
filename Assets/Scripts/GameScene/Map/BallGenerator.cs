using System.Collections.Generic;
using UnityEngine;

public class BallGenerator : MonoBehaviour
{
    public static BallGenerator instance;
    public GameObject ballPrefab; // Le prefab de la balle
    public GameObject parentGameObject;

    void Start()
    {
    }

    public GameObject GenerateBalls(Vector3 position)
    { 
        GameObject newBall = Instantiate(ballPrefab, position, Quaternion.identity);
        if (parentGameObject != null)
        {
            newBall.transform.parent = parentGameObject.transform;
        }
        return newBall;
    }
}

