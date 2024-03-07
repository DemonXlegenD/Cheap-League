using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{

    const float scale = 2f;

    const float viewerMoveThresholdForChunkUpdate = 12f;
    const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;

    public LODInfo[] detailLevels;
    public static float maxViewDst;

    public Transform[] viewers;
    public Material mapMaterial;

    public static Vector2[] viewersPosition;

    Vector2[] viewersPositionOld;

    static MapGenerator mapGenerator;
    static BallGenerator ballGenerator;
    static ObstacleSpawner ObstacleSpawner;

    int chunkSize;
    int chunksVisibleInViewDst;

    Dictionary<Vector2, TerrainChunk> terrainChunkDictionary = new Dictionary<Vector2, TerrainChunk>();
    static List<TerrainChunk> terrainChunksVisibleLastUpdate = new List<TerrainChunk>();
    static List<GameObject> ballOnTerrainLists = new List<GameObject>();

    private void Start()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();
        ballGenerator = FindObjectOfType<BallGenerator>();
        ObstacleSpawner = FindObjectOfType<ObstacleSpawner>();

        maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
        chunkSize = MapGenerator.mapChunkSize - 1;
        chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / chunkSize);
        viewersPosition = new Vector2[2];
        viewersPositionOld = new Vector2[2];
        UpdateVisibleChunks();
    }

    private void OnDestroy()
    {
        terrainChunkDictionary.Clear();
        terrainChunksVisibleLastUpdate.Clear();
        ballOnTerrainLists.Clear();
    }

    private void Update()
    {
        viewersPosition[0] = new Vector2(viewers[0].position.x, viewers[0].position.z) / 2f;
        viewersPosition[1] = new Vector2(viewers[1].position.x, viewers[1].position.z) / 2f;

        if ((viewersPositionOld[0] - viewersPosition[0]).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate)
        {
            viewersPositionOld[0] = viewersPosition[0];
            UpdateVisibleChunks();
        }
        else if ((viewersPositionOld[1] - viewersPosition[1]).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate)
        {
            viewersPositionOld[1] = viewersPosition[1];
            UpdateVisibleChunks();
        }
    }


    void UpdateVisibleChunks()
    {

        for (int i = 0; i < terrainChunksVisibleLastUpdate.Count; i++)
        {
            terrainChunksVisibleLastUpdate[i].SetVisible(false);
        }
        terrainChunksVisibleLastUpdate.Clear();

        for (int j = 0; j < viewers.Length; j++)
        {
            int currentChunkCoordX = Mathf.RoundToInt(viewersPosition[j].x / chunkSize);
            int currentChunkCoordY = Mathf.RoundToInt(viewersPosition[j].y / chunkSize);

            for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++)
            {
                for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
                {
                    Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                    if (terrainChunkDictionary.ContainsKey(viewedChunkCoord))
                    {
                        terrainChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                    }
                    else
                    {
                        TerrainChunk newT = new TerrainChunk(viewedChunkCoord, chunkSize, detailLevels, transform, mapMaterial);
                        terrainChunkDictionary.Add(viewedChunkCoord, newT);
                        if (!Vector2.Equals(viewedChunkCoord, Vector2.zero))
                        {
                           /* int numberBalls = Random.Range(1, 6);

                            for (int i = 0; i < numberBalls; i++)
                            {
                                int randomPosX = Random.Range(((int)viewedChunkCoord.x * chunkSize) - (chunkSize / 2), ((int)viewedChunkCoord.x * chunkSize) + (chunkSize / 2));
                                int randomPosZ = Random.Range(((int)viewedChunkCoord.y * chunkSize) - (chunkSize / 2), ((int)viewedChunkCoord.y * chunkSize) + (chunkSize / 2));
                                int randomPosY = Random.Range(2, 31);
                                ballOnTerrainLists.Add(ballGenerator.GenerateBalls(new Vector3(randomPosX, randomPosY, randomPosZ)));
                            }*/

                        }
                    }
                }
            }
        }

    }

    public class TerrainChunk
    {

        List<GameObject> obstacles = new List<GameObject>();
        GameObject meshObject;
        public int size;
        public Vector2 coord;
        public Vector2 position;
        public Bounds bounds;

        MeshRenderer meshRenderer;
        MeshFilter meshFilter;
        MeshCollider meshCollider;

        LODInfo[] detailsLevels;
        LODMesh[] lodMeshes;
        LODMesh collisionLODMesh;

        MapData mapData;
        bool mapDataReceived;
        int previousLODIndex = -1;

        public TerrainChunk(Vector2 coord, int size, LODInfo[] detailsLevels, Transform parent, Material material)
        {
            this.detailsLevels = detailsLevels;
            this.size = size;
            this.coord = coord;
            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);

            meshObject = new GameObject("Terrain Chunk");
            meshRenderer = meshObject.AddComponent<MeshRenderer>();
            meshFilter = meshObject.AddComponent<MeshFilter>();
            meshCollider = meshObject.AddComponent<MeshCollider>();
            meshRenderer.material = material;

            meshObject.transform.position = positionV3 * scale;
            meshObject.transform.parent = parent;
            meshObject.transform.localScale = Vector3.one * scale;
            SetVisible(false);

            lodMeshes = new LODMesh[detailsLevels.Length];
            for (int i = 0; i < detailsLevels.Length; i++)
            {
                lodMeshes[i] = new LODMesh(detailsLevels[i].lod, UpdateTerrainChunk);
                if (detailsLevels[i].useForCollider)
                {
                    collisionLODMesh = lodMeshes[i];
                }
            }

            mapGenerator.RequestMapData(position, OnMapDataReceived);

        }

        void GenerateObstacles()
        {
            int numberObstacles = Random.Range(10, 20);
            for (int i = 0; i < numberObstacles; i++)
            {
                Vector2 minChunk = (coord - (Vector2.one / 2)) * this.size;
                Vector2 maxChunk = (coord + (Vector2.one / 2)) * this.size;
                GameObject newObstacle = ObstacleSpawner.SpawnObstacles(minChunk, maxChunk);
                if (newObstacle != null) obstacles.Add(newObstacle);
            }
        }

        void OnMapDataReceived(MapData mapData)
        {
            this.mapData = mapData;
            mapDataReceived = true;

            Texture2D texture = TextureGenerator.TextureFromColourMap(mapData.colourMap, MapGenerator.mapChunkSize, MapGenerator.mapChunkSize);
            meshRenderer.material.mainTexture = texture;

            UpdateTerrainChunk();
            GenerateObstacles();
        }

        public void UpdateTerrainChunk()
        {
            if (mapDataReceived)
            {
                float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewersPosition[0]));
                float viewerDstFromNearestEdge2 = Mathf.Sqrt(bounds.SqrDistance(viewersPosition[1]));
                bool visible = viewerDstFromNearestEdge <= maxViewDst || viewerDstFromNearestEdge2 <= maxViewDst;

                if (visible)
                {
                    int lodIndex = 0;

                    for (int i = 0; i < detailsLevels.Length - 1; i++)
                    {
                        if (viewerDstFromNearestEdge > detailsLevels[i].visibleDstThreshold || viewerDstFromNearestEdge2 > detailsLevels[i].visibleDstThreshold)
                        {
                            lodIndex = i + 1;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (lodIndex != previousLODIndex)
                    {
                        LODMesh lodMesh = lodMeshes[lodIndex];
                        if (lodMesh.hasMesh)
                        {
                            previousLODIndex = lodIndex;
                            meshFilter.mesh = lodMesh.mesh;
                            meshCollider.sharedMesh = lodMesh.mesh;
                        }
                        else if (!lodMesh.hasRequestedMesh)
                        {
                            lodMesh.RequestMesh(mapData);
                        }
                    }


                    if (collisionLODMesh.hasMesh)
                    {
                        meshCollider.sharedMesh = collisionLODMesh.mesh;
                    }
                    else if (!collisionLODMesh.hasRequestedMesh)
                    {
                        collisionLODMesh.RequestMesh(mapData);
                    }
                   
                    terrainChunksVisibleLastUpdate.Add(this);
                }

                for (int j = 0; j < ballOnTerrainLists.Count; j++) // Utilisation du for i plutot que foreach pour éviter les problèmes lors de la destruction des balles
                {
                    if (ballOnTerrainLists[j] != null)
                    {
                        if (IsInMeshZone(ballOnTerrainLists[j], meshRenderer.bounds))
                        {
                            ballOnTerrainLists[j].SetActive(visible);
                        }
                    }
                    else
                    {
                        ballOnTerrainLists.Remove(ballOnTerrainLists[j]);
                    }

                }

                for (int j = 0; j < obstacles.Count; j++)
                {
                    if (obstacles[j] != null)
                    {
                        if (IsInMeshZone(obstacles[j], meshRenderer.bounds))
                        {
                            obstacles[j].SetActive(visible);
                        }
                    }
                    else
                    {
                        ballOnTerrainLists.Remove(ballOnTerrainLists[j]);
                    }
                }
                SetVisible(visible);
            }
        }

        public bool IsInMeshZone(GameObject gameObject, Bounds bounds)
        {

            // Récupérer les bounds du GameObject
            Bounds gameObjectBounds = gameObject.GetComponent<Renderer>().bounds;

            // Vérifier si les bounds du GameObject intersectent les bounds du mesh
            return bounds.Intersects(gameObjectBounds);
        }

        public void SetVisible(bool visisble)
        {
            meshObject.SetActive(visisble);
        }

        public bool IsVisible()
        {
            return meshObject.activeSelf;
        }
    }

    class LODMesh
    {

        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasMesh;
        int lod;
        System.Action updateCallback;

        public LODMesh(int lod, System.Action updateCallback)
        {
            this.lod = lod;
            this.updateCallback = updateCallback;
        }

        void OnMeshDataReceived(MeshData meshData)
        {
            mesh = meshData.CreateMesh();
            hasMesh = true;

            updateCallback();
        }

        public void RequestMesh(MapData mapData)
        {
            hasRequestedMesh = true;
            mapGenerator.RequestMeshData(mapData, lod, OnMeshDataReceived);
        }

    }

    [System.Serializable]
    public struct LODInfo
    {
        public int lod;
        public float visibleDstThreshold;
        public bool useForCollider;
    }
}
