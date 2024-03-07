using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallPhysics : MonoBehaviour
{
    [SerializeField] private float magnitudeForce = 10f;
    [SerializeField] private float friction = 0.2f;
    [SerializeField] private float deceleration = 0.1f;
    [SerializeField] private float initialMass = 50f;
    [SerializeField] private float bounciness = 10f;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = initialMass;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.velocity *= (1 - deceleration * Time.deltaTime);
        magnitudeForce *= (1 - friction * Time.deltaTime);
        rb.AddForce(Vector3.up * (2*Physics.gravity.y/3) * rb.mass);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();


            if (playerRb != null)
            {
                Debug.Log("hit player");
                Vector3 forceDirection = collision.contacts[0].point - playerRb.centerOfMass;
                Vector3 relativeVelocity = playerRb.GetRelativePointVelocity(forceDirection);

                //rb.AddForce(forceDirection * Vector3.Dot(forceDirection.normalized, relativeVelocity) * magnitudeForce, ForceMode.Impulse);
                rb.AddForce(forceDirection.normalized, ForceMode.Impulse);
            }
        } else
        {
            Vector3 normalForce = collision.contacts[0].normal;
            //rb.AddForce(normalForce * magnitudeForce * bounciness * rb.mass, ForceMode.Impulse);
        }

        GetComponent<AudioSource>().Play();
    }
}
