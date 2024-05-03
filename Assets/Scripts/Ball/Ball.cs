using UnityEngine;

public class Ball : MonoBehaviour
{
    void Update()
    {
        if(transform.position.y < -200)
        {
            Destroy(gameObject);
        }
    }
}
