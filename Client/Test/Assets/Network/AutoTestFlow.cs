using Network;
using Network.API;
using Network.Models.Common;
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
        GamePlayApi.SendWebSocketMessage(NetworkEventPaths.GamePlay_StartGame, string.Empty);
    }
    public void TestAction2()
    {
        GamePlayApi.SendWebSocketMessage(NetworkEventPaths.GamePlay_JoinGame, string.Empty);
    }
    public void TestAction3()
    {
        GameFramework.UIMgr.OpenWindow<UIWindow_Bag>(null);
    }
}
