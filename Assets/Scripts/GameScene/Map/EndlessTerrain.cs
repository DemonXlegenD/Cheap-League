using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{

    [SerializeField] private const float maxViewDist = 500;
    [SerializeField] private Transform viewer;

    public static Vector2 viewerPosition;
    [SerializeField] private int chunkSize;
    [SerializeField] private int chunksVisibleInViewDst;

    Collection<Vector2, TerrainChunk> terrainChunkCollection = new Collection<Vector2, TerrainChunk>();
    List<TerrainChunk> terrainChunkVisibleList = new List<TerrainChunk>();

    private void Start()
    {
        chunkSize = MapGenerator.mapChunkSize - 1;
        chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDist / chunkSize);
    }


    private void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);
        UpdateVisibleChunks();
    }
    private void UpdateVisibleChunks()
    {

        foreach(var chunk in terrainChunkVisibleList)
        {
            chunk.SetVisible(false);
        }
        terrainChunkVisibleList.Clear();

        int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / chunkSize);
        int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / chunkSize);

        for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++)
        {
            for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
            {
                Vector2 viewedChunkCoord = new Vector2(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                if (terrainChunkCollection.HasItem(viewedChunkCoord))
                {
                    TerrainChunk currentTerrain = terrainChunkCollection.GetItemBykey(viewedChunkCoord);
                    currentTerrain.UpdateTerrainChunk();

                    if (currentTerrain.IsVisible())
                    {
                        terrainChunkVisibleList.Add(currentTerrain);
                    }
                }
                else
                {
                    terrainChunkCollection.AddItem(viewedChunkCoord, new TerrainChunk(viewedChunkCoord, chunkSize, transform));
                }
            }
        }
    }

    public class TerrainChunk
    {
        GameObject meshObject;
        Vector2 position;
        Bounds bounds;
        public TerrainChunk(Vector2 coord, int size, Transform parent)
        {
            position = coord * size;
            bounds = new Bounds(position, Vector2.one * size);
            Vector3 positionV3 = new Vector3(position.x, 0, position.y);

            meshObject = GameObject.CreatePrimitive(PrimitiveType.Plane);
            meshObject.transform.position = positionV3;
            meshObject.transform.localScale = Vector3.one * size / 10f; //un plane fait 10 de distance par défaut
            meshObject.transform.parent = parent;
        }

        public void UpdateTerrainChunk()
        {
            float viewerDistanceFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
            bool visible = viewerDistanceFromNearestEdge <= maxViewDist;
            SetVisible(visible);
        }

        public void SetVisible(bool visible)
        {
            meshObject.SetActive(visible);
        }

        public bool IsVisible()
        {
            return meshObject.activeSelf;
        }
    }
}
