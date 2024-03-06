using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public LayerMask groundLayer;
    public float spawnRadius = 5f;
    public float minDistanceBetweenObstacles = 2f;

    public GameObject[] obstaclePrefabs;
    public GameObject parentGameObject;
    public Collider[] forbiddenZones;
    public float bufferDistance = 2f;

    public int maxTries = 10;

    void Start()
    {
    }

    public GameObject SpawnObstacles(Vector2 chunkMin, Vector2 chunkMax)
    {
        int tries = 0;

        while (tries < maxTries)
        {
            //Ca ne marche pas, peut etre à cause de la génération procédurale... le fait que ca soit un mesh?
            /*Vector2 randomPoint = GenerateRandomPoint(chunkMin, chunkMax);
            RaycastHit2D hit = Physics2D.Raycast(randomPoint, Vector2.down, Mathf.Infinity, groundLayer);
            Debug.Log(randomPoint);
            if (hit.collider != null)
            {
                bool canPlace = CheckObstaclePlacement(hit.point);

                if (canPlace)
                {

                    GameObject newObstacle = Instantiate(obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)], hit.point, Quaternion.identity);
                    if (parentGameObject != null)
                    {
                        newObstacle.transform.parent = parentGameObject.transform;
                    }
                    return newObstacle;
                }
            }*/

                Vector2 randomPoint = GenerateRandomPoint(chunkMin, chunkMax);
            if (!IsInForbiddenZone(randomPoint))
            {
                bool canPlace = CheckObstaclePlacement(new Vector2(randomPoint.x, 20));

                if (canPlace)
                {

                    GameObject newObstacle = Instantiate(obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)], new Vector3(randomPoint.x, -2, randomPoint.y), Quaternion.identity);
                    if (parentGameObject != null)
                    {
                        newObstacle.transform.parent = parentGameObject.transform;
                    }
                    return newObstacle;
                }
            }
            


            tries++;
        }
        return null;
    }

    Vector2 GenerateRandomPoint(Vector2 chunkMin, Vector2 chunkMax)
    {
        Vector2 randomPoint = Vector2.zero;
        bool pointFound = false;

        while (!pointFound)
        {
            randomPoint = new Vector2(Random.Range(chunkMin.x, chunkMax.x), Random.Range(chunkMin.y, chunkMax.y));

            if (Physics2D.OverlapCircle(randomPoint, minDistanceBetweenObstacles) == null)
            {
                pointFound = true;
            }
        }

        return randomPoint;
    }

    bool CheckObstaclePlacement(Vector2 position)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, spawnRadius);

        // Vérifier si l'emplacement est déjà occupé par un obstacle
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Obstacle"))
            {
                return false;
            }
        }

        // Vérifier s'il y a des obstacles proches
        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(position, minDistanceBetweenObstacles);
        if (nearbyColliders.Length > 1) // Compte le collider de l'obstacle que nous allons placer
        {
            return false;
        }

        // Vous pouvez ajouter d'autres vérifications ici, par exemple, s'il est trop près du joueur, etc.

        return true;
    }

    bool IsInForbiddenZone(Vector3 position)
    {
        foreach (Collider zone in forbiddenZones)
        {
            Bounds zoneBounds = zone.bounds;
            zoneBounds.Expand(bufferDistance);
            if (zone.bounds.Contains(position))
            {
                return true;
            }
        }
        return false;
    }

}