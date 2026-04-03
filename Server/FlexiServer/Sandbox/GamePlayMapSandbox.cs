using FlexiServer.Core;
using FlexiServer.Models.Common;
using FlexiServer.Sandbox.Interface;
using System.Collections.Concurrent;
using static EnumDefinitions;

namespace FlexiServer.Sandbox
{
    public class GamePlayMapSandbox : SandboxBase, ISandboxPlayer
    {
        private class PlayerInfo
        {
            public string account = string.Empty;
            public string clientId = string.Empty;
        }
        private ConcurrentDictionary<string, string> ClientIdToAccountMap = new();
        private ConcurrentDictionary<string, string> AccountToClientIdMap = new();
        private ConcurrentDictionary<string, PlayerInfo> playerDic = new();//Key:account

        private ConcurrentDictionary<EMapLayerType, int> mapDic = new();
        public void AddPlayer(string clientId, string account)
        {
            bool setA2C = AccountToClientIdMap.TryAdd(account, clientId);
            ClientIdToAccountMap.AddOrUpdate(clientId,key => account,(key,oldValue)=> account);
            

            if (setA2C)
            {
                PlayerInfo playerInfo = new PlayerInfo();
                playerInfo.clientId = clientId;
                playerInfo.account = account;
                playerDic.TryAdd(account, playerInfo);
            }
        }
        public List<string> GetPlayerClients(Func<string, bool>? select = null)
        {
            List<string> clients = new List<string>();
            foreach (var client in ClientIdToAccountMap.Keys)
            {
                if (select != null && select.Invoke(client)) clients.Add(client);
                else clients.Add(client);
            }
            return clients;
        }
        public bool ContainsPlayer(string account)
        {
            return AccountToClientIdMap.TryGetValue(account, out var _);
        }
        public void OnPlayerConnectionStateChanged(string clientId, string account, EPlayerConnectionState state)
        {
            // 如果玩家断开连接，那么就从字典中移除该玩家
            // 具体怎么做需要看Gamplay，现在暂时这么处理
            if (state == EPlayerConnectionState.Closed)
            {
                ClientIdToAccountMap.TryRemove(clientId, out var _);
                AccountToClientIdMap.TryRemove(account, out var _);
                playerDic.Remove(account, out var _);
            }
            if (playerDic.Count == 0) Release();
        }
        public void GenerateMap(List<EMapLayerType> layers, out List<MapGeneratorInfo> result)
        {
            result = [];
            if (!mapDic.IsEmpty) result = [.. mapDic.Select(kv => new MapGeneratorInfo() { MapLayer = kv.Key, Seed = kv.Value })];

            foreach (var layer in layers)
            {
                if (!mapDic.ContainsKey(layer))
                {
                    int seed = new Random().Next();
                    mapDic.TryAdd(layer, seed);
                    result.Add(new MapGeneratorInfo() { MapLayer = layer, Seed = seed });
                }
            }
        }
        public override void OnDestroy()
        {
            ClientIdToAccountMap.Clear();
            AccountToClientIdMap.Clear();
            playerDic.Clear();
            mapDic.Clear();
        }
        public override void OnReset()
        {
            ClientIdToAccountMap.Clear();
            AccountToClientIdMap.Clear();
            playerDic.Clear();
            mapDic.Clear();
        }
    }
}
