using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private enum DrawMode { NoiseMap, ColorMap }
    [SerializeField] private DrawMode drawMode;

    [SerializeField] private int mapWidth;
    [SerializeField] private int mapHeight;
    [SerializeField] private int octaves;
    [SerializeField] private int seed;

    [SerializeField, Range(0, 1)] private float persistance;
    [SerializeField] private float lacunarity;
    [SerializeField] private float noiseScale;

    [SerializeField] private Vector2 offset;

    [SerializeField] TerrainType[] regions;

    public bool autoUpdate;
    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);
        Color[] colorMap = new Color[mapWidth * mapHeight];

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        colorMap[y * mapWidth + x] = regions[i].color;
                        break;
                    }
                }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap) display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        else if (drawMode == DrawMode.ColorMap) display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));;
    }

    private void OnValidate()
    {
        if (mapWidth < 1) mapWidth = 1;
        if (mapHeight < 1) mapHeight = 1;
        if (lacunarity < 1) lacunarity = 1f;
        if (octaves < 0) octaves = 0;
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}
