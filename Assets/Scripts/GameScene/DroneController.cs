using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneController : MonoBehaviour
{
    [SerializeField] private GameObject ball;
    [SerializeField] private Vector3 minRegion;
    [SerializeField] private Vector3 maxRegion;
    [SerializeField] private float ballDetectionDistance = 15f;
    [SerializeField] private float droneHeight = 10f;
    [SerializeField] private float droneSpeed = 15f;

    private Rigidbody rb;
    private Vector3 newPos;
    private bool ballAttached = false;
    private float attachDebounce = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        transform.position = new Vector3(Random.Range(minRegion.x, maxRegion.x), droneHeight, Random.Range(minRegion.z, maxRegion.z));
    }

    void FixedUpdate()
    {
        HandleDrone();
    }

    private void HandleDrone()
    {
        if (newPos == null)
        {
            newPos = GetRandomPos();
        }

        if (!ballAttached)
        {
            if (attachDebounce > 0f)
            {
                attachDebounce -= Time.deltaTime;
                if (attachDebounce < 0f)
                {
                    attachDebounce = 0f;
                }
            }

            if ((transform.position - ball.transform.position).magnitude < ballDetectionDistance && attachDebounce == 0f)
            {
                //Debug.Log("ball detected!");
                rb.AddForce((transform.position - ball.transform.position + new Vector3(0, -ball.transform.localScale.y / 2, 0)).normalized * -droneSpeed * 3);
                if ((transform.position - ball.transform.position).magnitude < 5)
                {
                    ballAttached = true;
                    newPos = GetRandomPos();
                    //Debug.Log("Ball stolen");
                }
            }
            else
            {
                if ((transform.position - newPos).magnitude < 5)
                {
                   // Debug.Log("position reached !");
                    newPos = GetRandomPos();
                }
                else
                {
                    //Debug.Log("trying to reach position. distance: " + (transform.position - newPos).magnitude);
                    rb.AddForce((transform.position - newPos).normalized * -droneSpeed);
                }
            }
        } else
        {
            rb.AddForce((transform.position - newPos).normalized * -droneSpeed);
            if ((transform.position - newPos).magnitude < 5)
            {
                attachDebounce = 5f;
                ballAttached = false;
            } else
            {
                ball.transform.position = transform.position - new Vector3(0, ball.transform.localScale.y / 2, 0);
            }
        }
    }

    private Vector3 GetRandomPos()
    {
        return new Vector3(Random.Range(minRegion.x, maxRegion.x), Random.Range(minRegion.y, maxRegion.y) + droneHeight, Random.Range(minRegion.z, maxRegion.z));
    }
}
