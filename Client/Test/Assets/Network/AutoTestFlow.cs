using Network;
using Network.API;
using Network.Models.Common;
using Network.Transport.WebSocket;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.U2D;

public class AutoTestFlow:MonoBehaviour
{
    public NetworkManager network;
    public bool isAutoLogin;
    public BTRuntimeComponent bTRuntimeComp;

    private AccountInfo defaultAccount { get; } = new AccountInfo() { Account = "DEF", Password = "123" };
    public void Start()
    {
        //PrintAtlasSprites("Assets/AddressableAssets/Art/Atlas/Food.spriteatlasv2");
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
        GamePlayApi gamePlayApi = ApiManager.GetWebSoketApi<GamePlayApi>();
        gamePlayApi.SendWebSocketMessage<string>(NetworkEventPaths.GamePlay_StartGame, null);
    }
    public void TestAction3()
    {
        PlayerGameInfo playerGameInfo = new PlayerGameInfo();
        playerGameInfo.Account = NetworkManager.Instance.Account;

        GamePlayApi gamePlayApi = ApiManager.GetWebSoketApi<GamePlayApi>();
        gamePlayApi.SendWebSocketMessage(NetworkEventPaths.GamePlay_JoinGame, playerGameInfo);

        GameFramework.UIMgr.OpenWindow<UIWindow_Bag>();
    }
    public async Task PrintAtlasSprites(string atlasKey)
    {
        var handle = Addressables.LoadAssetAsync<SpriteAtlas>(atlasKey);
        await handle.Task;

        if (handle.Status != UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("加载图集失败");
            return;
        }

        SpriteAtlas atlas = handle.Result;
        Sprite[] sprites = new Sprite[atlas.spriteCount];
        atlas.GetSprites(sprites);

        foreach (var s in sprites)
            Debug.Log(s.name);
    }
}
