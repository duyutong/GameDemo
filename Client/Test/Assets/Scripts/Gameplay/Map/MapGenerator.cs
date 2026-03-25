using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    public int width;
    public int height;
    public Transform plantRoot;
    public Tilemap tilemap;
    public TileBase tileGround;
    public float lacunarity = 0.1f;
    public int seed;
    public bool useNewSeed = false;

    [Range(0, 1)]
    public float threshold = 0.3f;

    public List<PlantConfig> plantConfigs;

    private float[,] tileNoiseValue;
    private System.Random random;
   
    public void GenerateMap()
    {
        InitMapData();
        SetTileMap();
        GeneratePlant();
    }
    private void GeneratePlant()
    {
        plantRoot.RemoveChildren();

        for (int y = 0; y < height; y++) 
        {
            for (int x = 0; x < width; x++) 
            {
                float noise = tileNoiseValue[x, y];
                bool tile = tileNoiseValue[x, y] <= threshold;
                if (!tile) continue;

                float chance = random.Next(0, 1000) * 0.001f;
                float posX = x - 0.5f * width;
                float posY = y - 0.5f * height;
                Vector2 pos = new(posX, posY);
                foreach (var plant in plantConfigs)
                { 
                   bool isGenPlant = plant.GenPlant(noise, pos, chance);
                    if (isGenPlant) break;
                }
            }
        }
    }
    private void SetTileMap()
    {
        float randomOffset = random.Next(-1000, 1000);
        float nosieMax = float.MinValue;
        float nosieMin = float.MaxValue;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float noise = Mathf.PerlinNoise(x * lacunarity + randomOffset, y * lacunarity + randomOffset);
                if (noise < nosieMin) nosieMin = noise;
                if (noise > nosieMax) nosieMax = noise;
                tileNoiseValue[x, y] = noise;
            }
        }
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float noise = tileNoiseValue[x, y];
                tileNoiseValue[x, y] = Mathf.InverseLerp(nosieMin, nosieMax, noise);
            }
        }
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                bool tile = tileNoiseValue[x, y] <= threshold;
                int posX = Mathf.CeilToInt(x - 0.5f * width);
                int posY = Mathf.CeilToInt(y - 0.5f * height);
                if (tile) tilemap.SetTile(new Vector3Int(posX, posY), tileGround);
            }
        }
    }
    private void InitMapData()
    {
        tileNoiseValue = new float[width, height];
        if(useNewSeed) seed = DateTimeOffset.UtcNow.ToUnixTimeSeconds().GetHashCode();
        random = new(seed);

        foreach (var plant in plantConfigs)
        {
            plant.SetPlantRoot(plantRoot);
            plant.SetGroundThreshold(threshold);
            plant.SetGroundSize(width, height);
        }
    }
    public void ClearTileMap()
    {
        tilemap.ClearAllTiles();
        plantRoot.RemoveChildren();
    }
}
[Serializable]
public class PlantRule
{
    public string name;
    public float min;
    public float max;
}
