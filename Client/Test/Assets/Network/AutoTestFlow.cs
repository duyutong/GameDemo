using Network;
using Network.API;
using Network.Models.Common;
using Network.Transport.WebSocket;
using System;
using UnityEngine;

public class AutoTestFlow:MonoBehaviour
{
    public NetworkManager network;
    public bool isAutoLogin;
    public BTRuntimeComponent bTRuntimeComp;

    private GamePlayApi GamePlayApi => ApiManager.GetWebSoketApi<GamePlayApi>();
    private AccountInfo defaultAccount { get; } = new AccountInfo() { Account = "ABC", Password = "123" };
    public void Start()
    {
        if (isAutoLogin) bTRuntimeComp.SendMsgToBTRuntime("AutoLogin_Start");
    }
    public void AutoLogin()
    {
        network.SetLoginInfo(defaultAccount.Account, defaultAccount.Password);
        network.HttpLogin();
    }
    public void WebSocketConnect() 
    {
        network.WebSocketConnect("Debug");
    }
    public void TestAction1() 
    {
        network.UpdConnect("Debug");
    }
    public void TestAction2()
    {
        PlayerMovementApi playerMovementApi = ApiManager.GetUdpApi<PlayerMovementApi>();
        playerMovementApi.SendUdpMessage<string>(NetworkEventPaths.PlayerMovement_MoveInGame, "PlayerMovement");
    }
    public void TestAction3()
    {
    }
}
