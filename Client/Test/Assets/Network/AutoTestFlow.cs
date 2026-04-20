using ConfigData;
using Network;
using Network.API;
using Network.Models.Common;
using Network.Transport.WebSocket;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.U2D;

public class AutoTestFlow : MonoBehaviour
{
    public NetworkManager network;
    public bool isAutoLogin;
    public BTRuntimeComponent bTRuntimeComp;

    private AccountInfo defaultAccount { get; } = new AccountInfo() { Account = "DEF", Password = "123" };
    public void Start()
    {
        LoadGlobalSetting();
    }
    public void LoadGlobalSetting()
    {
        GlobalSetting.LoadSetting(() =>
        {
            if (isAutoLogin) bTRuntimeComp.SendMsgToBTRuntime("AutoLogin_Start");
        });
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
        PlayerGameInfo playerGameInfo = new PlayerGameInfo();
        playerGameInfo.Account = NetworkManager.Instance.Account;

        GamePlayApi gamePlayApi = ApiManager.GetWebSoketApi<GamePlayApi>();
        gamePlayApi.SendWebSocketMessage(NetworkEventPaths.GamePlay_JoinGame, playerGameInfo);
    }
    public void TestAction3()
    {
        GamePlayApi gamePlayApi = ApiManager.GetWebSoketApi<GamePlayApi>();
        gamePlayApi.SendWebSocketMessage<string>(NetworkEventPaths.GamePlay_StartGame, null);

        GameFramework.UIMgr.OpenWindow<UIWindow_ExplorationUI>();
    }
}
