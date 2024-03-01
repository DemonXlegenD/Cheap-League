using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scrip : MonoBehaviour
{
    [SerializeField] private float magnitudeForce = 10f;
    [SerializeField] private float friction = 0.2f;
    [SerializeField] private float deceleration = 0.1f;
    [SerializeField] private float initialMass = 1f;
    [SerializeField] private float bounciness = 0.8f;

    void Start()
    {
        GetComponent<Rigidbody>().mass = initialMass;
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Rigidbody>().velocity *= (1 - deceleration * Time.deltaTime);
        magnitudeForce *= (1 - friction * Time.deltaTime);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Rigidbody playerRb = collision.gameObject.GetComponent<Rigidbody>();

            if (playerRb != null)
            {
                Debug.Log("Hit");
                Vector3 forceDirection = transform.position - playerRb.transform.position + Vector3.up;
                GetComponent<Rigidbody>().AddForce(forceDirection.normalized * magnitudeForce, ForceMode.Impulse);
            }
        } else
        {
            Vector3 normalForce = collision.contacts[0].normal;
            GetComponent<Rigidbody>().AddForce(normalForce * magnitudeForce * bounciness, ForceMode.Impulse);
        }
    }
}
