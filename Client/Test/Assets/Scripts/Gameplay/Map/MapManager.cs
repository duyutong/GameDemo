using Network;
using Network.API;
using Network.Models;
using Network.Transport.WebSocket;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;
using static EnumDefinitions;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { private set; get; }
    public CinemachineCamera cmCam;
    public List<MapGeneratorData> maps;

    private Dictionary<EMapLayerType, MapGenerator> mapGenerators = new();
    private Dictionary<EMapLayerType, MapGeneratorData> mapGeneratorDatas = new();

    private MapApi mapApi=>ApiManager.GetHttpApi<MapApi>();
    private GamePlayApi gamePlayApi => ApiManager.GetWebSoketApi<GamePlayApi>();
    public MapManager()
    {
        Instance = this;
    }
    public void Start()
    {
        gamePlayApi.AddListener(NetworkEventPaths.GamePlay_StartGame, OnStartGame);
    }
    private void OnDestroy() 
    {
        gamePlayApi.RemoveListener(NetworkEventPaths.GamePlay_StartGame, OnStartGame);
    }
    private void OnStartGame(WebSocketResult result)
    {
        if (result.Code != 200) return;
        if (result.Data == null) return;

        GenerateMaps();
    }

    private void GenerateMaps() 
    {
        mapGenerators.Clear();
        IniMapGenerators();

        MapGenerateRequest req = new();
        req.MapLayer = mapGenerators.Keys.ToList();
        mapApi.MapGenerate(req, OnMapGenerate);
    }

    private void OnMapGenerate(bool succ, MapGenerateResponse response)
    {
        if (!succ) return;

        foreach (var mapGenInfo in response.MapGenInfos) 
        {
            EMapLayerType eMapLayerType = mapGenInfo.MapLayer;
            mapGenerators.TryGetValue(eMapLayerType, out var generator);
            if (generator == null) return;

            int seed = mapGenInfo.Seed;
            generator.GenerateMap(seed);
        }
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
    public void ShowAllSpawn(EMapLayerType eMapLayer) 
    {
        if (!mapGenerators.ContainsKey(eMapLayer)) IniMapGenerators();
        MapGenerator generator = GetMapByLayer(eMapLayer);
        generator?.ShowAllSpawn();
    }
    public void ClearMap(EMapLayerType eMapLayer)
    {
        if (!mapGenerators.ContainsKey(eMapLayer)) IniMapGenerators();
        MapGenerator generator = GetMapByLayer(eMapLayer);
        generator?.ClearMap();
    }
    public void GenerateMap(MapGeneratorData generatorData)
    {
        EMapLayerType layer = generatorData.layer;
        MapGenerator generator = GetMapByLayer(layer);
        if (generator == null)
        {
            generator = new(generatorData);
            mapGenerators.AddOrReplace(layer, generator);
        }
        generator.GenerateMap();
    }
    public MapGenerator GetMapByLayer(EMapLayerType eMapLayer)
    {
        mapGenerators.TryGetValue(eMapLayer, out MapGenerator generator);
        if (generator != null) return generator;
        return null;
    }
    public MapGeneratorData GetMapDataByNameByLayer(EMapLayerType eMapLayer)
    {
        mapGeneratorDatas.TryGetValue(eMapLayer, out MapGeneratorData data);
        if (data != null) return data;

        MapGenerator generator = GetMapByLayer(eMapLayer);
        if (generator != null)
        {
            MapGeneratorData generatorData = generator.GeneratorData;
            mapGeneratorDatas.AddOrReplace(eMapLayer, generatorData);
            return generatorData;
        }

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
