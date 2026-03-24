using FlexiServer.Core;
using FlexiServer.Core.Frame;
using FlexiServer.Models.Common;
using FlexiServer.Sandbox.Interface;
using FlexiServer.Sandbox.Util;
using FlexiServer.Services;
using System;
using System.Collections.Concurrent;
using System.Numerics;

namespace FlexiServer.Sandbox
{
    public class GamePlayMovementSandbox : SandboxBase, ISandboxPlayer
    {
        private class PlayerInfo
        {
            public string account = string.Empty;
            public string clientId = string.Empty;
            public MovementInfo currMove = new();
            public MovementInfo targetMove = new();
            public int lastFramerate = 0;
            // other
        }
        private readonly ConcurrentDictionary<string, string> ClientIdToAccountMap = new();
        private readonly ConcurrentDictionary<string, string> AccountToClientIdMap = new();
        private readonly ConcurrentDictionary<string, PlayerInfo> playerMap = new();
        public List<MovementInfo> GetMoveInfos(int serverCurrentFrame)
        {
            var list = new List<MovementInfo>();
            foreach (var player in playerMap.Values) list.Add(player.currMove);

            return list;
        }
        public void RefreshMovementState(MovementInfo? movementInfo)
        {
            if (movementInfo == null) return;

            string account = movementInfo.Account;
            if (!playerMap.TryGetValue(account, out PlayerInfo? info)) return;

            info.currMove = movementInfo;
            info.targetMove = movementInfo;
            LastSandboxFramerate = int.Max(info.lastFramerate, LastSandboxFramerate);
        }
        public void RefreshMovement(int frame, MovementInfo? movementInfo)
        {
            if (movementInfo == null) return;

            string account = movementInfo.Account;
            if (!playerMap.TryGetValue(account, out PlayerInfo? info)) return;
            if (info.lastFramerate > frame) return;

            LastSandboxFramerate = int.Max(frame, LastSandboxFramerate);
            info.lastFramerate = int.Max(info.lastFramerate, frame);
            info.targetMove = movementInfo;
            info.currMove.EOpState = EnumDefinitions.EOperationState.InProgress;
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
        public List<string> GetPlayerAccounts(Func<string, bool>? select = null)
        {
            List<string> accounts = new List<string>();
            foreach (var account in AccountToClientIdMap.Keys)
            {
                if (select != null && select.Invoke(account)) accounts.Add(account);
                else accounts.Add(account);
            }
            return accounts;
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
                playerMap.TryAdd(account, playerInfo);
            }
        }

        public bool ContainsPlayer(string account)
        {
            return AccountToClientIdMap.TryGetValue(account, out var _);
        }

        public override void OnDestroy()
        {
            ClientIdToAccountMap.Clear();
            AccountToClientIdMap.Clear();

            playerMap.Clear();
        }

        public void OnPlayerConnectionStateChanged(string clientId, string account, EPlayerConnectionState state)
        {
            Console.WriteLine($"[GamePlayMovementSandbox]  OnPlayerConnectionStateChanged {state}");
            if (state == EPlayerConnectionState.Closed)
            {
                ClientIdToAccountMap.TryRemove(clientId, out var _);
                AccountToClientIdMap.TryRemove(account, out var _);
                playerMap.Remove(account, out var _);
            }
            if (playerMap.Count == 0) Release();
        }

        public override void OnReset()
        {
            ClientIdToAccountMap.Clear();
            AccountToClientIdMap.Clear();

            playerMap.Clear();
        }
        public override void OnUpdate()
        {
            foreach (PlayerInfo player in playerMap.Values)
            {
                Vector2 posA = new(player.currMove.X, player.currMove.Y);
                Vector2 posB = new(player.targetMove.X, player.targetMove.Y);
                float lerpT = player.targetMove.MoveLerpSpeed * SandboxManager.DeltaTime;
                Vector2 result = SandboxUtil.Lerp(posA, posB, lerpT);
                player.currMove.X= result.X; 
                player.currMove.Y = result.Y;
            }
        }
    }
}
