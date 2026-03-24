using ConfigData;
using FlexiServer.Core;
using FlexiServer.Sandbox.Interface;
using System.Collections.Concurrent;
using System.Security.AccessControl;

namespace FlexiServer.Sandbox
{
    public class GamePlayItemSandbox() : SandboxBase, ISandboxPlayer
    {
        private const int Bag_Maximum = 12;     //背包一共12个槽位
        private const int Item_Minimum = 10;    //该局游戏中玩家只能同时拥有10个物品
        private class PlayerInfo
        {
            public string account = string.Empty;
            public string clientId = string.Empty;
            public int[] bagSlot = [];
            // other
        }
        private ConcurrentDictionary<string, string> ClientIdToAccountMap = new();
        private ConcurrentDictionary<string, string> AccountToClientIdMap = new();
        private ConcurrentDictionary<int, string> itemOwnerMap = new();
        private ConcurrentDictionary<string, PlayerInfo> playerMap = new();//Key:account

        public void GrantItemToPlayer(string account, out int itemId, out int bagSlotIndex)
        {
            itemId = 0;
            bagSlotIndex = 0;
            
            if (!playerMap.TryGetValue(account, out PlayerInfo? info)) return;
            
            int nonZeroCount = info.bagSlot.Count(x => x > 0);
            if (nonZeroCount >= Item_Minimum) return;   //该玩家已经达到可获取物品的上限了

            int emptySolt = info.bagSlot.Select((value, index) => new { value, index })
                    .FirstOrDefault(x => x.value == 0)?.index ?? -1;

            var candidates = itemOwnerMap
                .Where(kv => string.IsNullOrEmpty(kv.Value))
                .Select(kv => kv.Key)
                .ToArray();

            if(candidates.Length == 0) return;  //该地区线索已经发完了
            
            int randomKey = candidates[Random.Shared.Next(candidates.Length)];
            itemOwnerMap[randomKey] = account;

            bagSlotIndex = emptySolt;
            itemId = randomKey;
            info.bagSlot[bagSlotIndex] = itemId;
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
                playerMap.Remove(account,out var _);
            }
            if (playerMap.Count == 0) Release();
        }
        public void AddPlayer(string clientId, string account)
        {
            bool setC2A = ClientIdToAccountMap.TryAdd(clientId, account);
            bool setA2C = AccountToClientIdMap.TryAdd(account, clientId);

            if (setA2C && setC2A)
            {
                PlayerInfo playerInfo = new PlayerInfo();
                playerInfo.clientId = clientId;
                playerInfo.account = account;
                playerInfo.bagSlot = new int[Item_Minimum];
                playerMap.TryAdd(account, playerInfo);
            }
        }
        public override void OnInit()
        {
            if (itemOwnerMap.IsEmpty)
            {
                // 如果有规则，在这里写Select函数
                // 目前是把表里配置的全都取出来的逻辑
                List<int> allIds = [.. ConfigLoader.GetConfigDatas<ItemConfig>(100).Select(item => item.ID)];
                foreach (var id in allIds) itemOwnerMap.TryAdd(id, string.Empty);
            }
        }
        public override void OnDestroy()
        {
            ClientIdToAccountMap.Clear();
            AccountToClientIdMap.Clear();

            itemOwnerMap.Clear();
            playerMap.Clear();
        }
        public override void OnReset()
        {
            ClientIdToAccountMap.Clear();
            AccountToClientIdMap.Clear();

            itemOwnerMap.Clear();
            playerMap.Clear();
        }

        public override void OnUpdate()
        {

        }
    }
}
