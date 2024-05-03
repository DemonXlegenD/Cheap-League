using UnityEngine;

public class BallFix : MonoBehaviour
{
    void Update()
    {
        if (transform.position.y < -10f)
        {
            transform.position = new Vector3(0f, 3f, 0f);
        }
        if (transform.position.z > 194.3f || transform.position.z < -194.3f)
        {
            transform.position = new Vector3(0f, 3f, 0f);
        }
        if (transform.position.x > 137.2f || transform.position.x < -137.2f)
        {
            transform.position = new Vector3(0f, 3f, 0f);
        }
    }
}
