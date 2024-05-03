using UnityEngine;

public class PlayerFix : MonoBehaviour
{
    void Update()
    {
        if (transform.position.y < -10f)
        {
            transform.position = new Vector3(transform.position.x, 6, transform.position.z); ;
        }
    }
}
