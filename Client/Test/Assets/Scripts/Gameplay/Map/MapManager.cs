using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { private set; get; }
    public CinemachineCamera cmCam;
    public List<MapGeneratorData> maps;

    private Dictionary<EMapLayer, MapGenerator> mapGenerators = new();

    public MapManager()
    {
        Instance = this;
    }
    private void Start()
    {
        mapGenerators.Clear();
        IniMapGenerators();
        GenerateMaps();
    }
    private void GenerateMaps() 
    {
        foreach (var generator in mapGenerators) 
            generator.Value.GenerateMap();
    }
    public void IniMapGenerators()
    {
        foreach (var generator in mapGenerators) generator.Value.ClearMap();

        foreach (var map in maps)
        {
            MapGenerator generator = new MapGenerator(map);
            generator.ClearMap();
            mapGenerators.AddOrReplace(map.layer, generator);
        }
    }
    public void ClearMap(EMapLayer eMapLayer)
    {
        MapGenerator generator = GetMapByNameByLayer(eMapLayer);
        generator?.ClearMap();
    }
    public void GenerateMap(MapGeneratorData generatorData)
    {
        EMapLayer layer = generatorData.layer;
        MapGenerator generator = GetMapByNameByLayer(layer);
        if (generator == null)
        {
            generator = new(generatorData);
            mapGenerators.AddOrReplace(layer, generator);
        }
        generator.GenerateMap();
    }
    private MapGenerator GetMapByNameByLayer(EMapLayer eMapLayer)
    {
        mapGenerators.TryGetValue(eMapLayer, out MapGenerator generator);
        if (generator != null) return generator;
        return null;
    }
    public MapGeneratorData GetMapDataByNameByLayer(EMapLayer eMapLayer)
    {
        mapGenerators.TryGetValue(eMapLayer, out MapGenerator generator);
        if (generator != null) return generator.GeneratorData;
        return null;
    }
    public object GetSpawnObjAt(Vector2Int mapIndex)
    {
        foreach (var generator in mapGenerators)
        {
            MapSpawnObj result = generator.Value.GetSpawnObjAt(mapIndex);
            if (result != null) return result;
        }
        return null;
    }

    public bool IsEmptyTile(Vector2 checkPos)
    {
        bool result = true;
        foreach (var generator in mapGenerators)
        {
            bool isEmpty = generator.Value.IsEmptyTile(checkPos);
            if (!isEmpty) return false;
        }
        return result;
    }
    private void Update()
    {
        foreach (var generator in mapGenerators) 
        {
            if (!generator.Value.InitFinshi) continue;
            generator.Value.Update();
        }
            
    }
}
public enum EMapLayer
{
    BaseDround
}
