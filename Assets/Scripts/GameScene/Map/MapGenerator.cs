using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private int mapWidth;
    [SerializeField] private int mapHeight;
    [SerializeField] private float noiseScale;

    public void GenerateMap()
    {
        float[,] noiseMap= Noise.GenerateNoiseMap(mapWidth, mapHeight, noiseScale);
    }
}
