using System;
using System.Threading;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{

    [SerializeField] private enum DrawMode { NoiseMap, ColorMap, Mesh }
    [SerializeField] private DrawMode drawMode;

    public const int mapChunkSize = 241; //240
    [SerializeField, Range(0,6)] private int levelOfDetail;
    [SerializeField] private int octaves;
    [SerializeField] private int seed;

    [SerializeField, Range(0, 1)] private float persistance;
    [SerializeField] private float lacunarity;
    [SerializeField] private float noiseScale;

    [SerializeField] private Vector2 offset;

    [SerializeField] private float meshHeightMultiplier;
    [SerializeField] private AnimationCurve meshHeightCurve;

    [SerializeField] TerrainType[] regions;

    public bool autoUpdate;

    public void DrawpMapInEditor()
    {
        MapData mapData = GenerateMapData();
            
        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap) display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
        else if (drawMode == DrawMode.ColorMap) display.DrawTexture(TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
        else if (drawMode == DrawMode.Mesh) display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail), TextureGenerator.TextureFromColorMap(mapData.colorMap, mapChunkSize, mapChunkSize));
    }

    public void MapDataThread(Action<MapData> callback)
    {
        MapData mapData = GenerateMapData();
    }

    public void RequestMapData(Action<MapData> callback)
    {
        ThreadStart threadStart = delegate
        {
            MapDataThread(callback);
        };

        new Thread(threadStart).Start();
    }
    private MapData GenerateMapData()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset);
        Color[] colorMap = new Color[mapChunkSize * mapChunkSize];

        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        colorMap[y * mapChunkSize + x] = regions[i].color;
                        break;
                    }
                }
            }
        }
        return new MapData(noiseMap, colorMap);
    }

    private void OnValidate()
    {
        if (lacunarity < 1) lacunarity = 1f;
        if (octaves < 0) octaves = 0;
    }


    private struct MapThreadInfo<T>
    {

    }

}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}


[System.Serializable]
public struct MapData
{
    public float[,] heightMap;
    public Color[] colorMap;

    public MapData(float[,] noiseMap, Color[] colorMap)
    {
        this.heightMap = noiseMap;
        this.colorMap = colorMap;
    }
}