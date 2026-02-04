using Assets.Network.API.ErrorCode;
using Network.API;
using Network.Core.Frame;
using Network.Core.Tick;
using Network.Models;
using Network.Models.Common;
using Network.Transport.Http;
using Network.Transport.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
namespace Network
{
    public class NetworkManager : MonoBehaviour
    {
        public bool isAutoLogin = false;
        public string host = "127.0.0.1";
        public int port = 8080;
        public bool isDebug = false;

        private HttpTransport httpTransport;
        private WebSocketTransport webSocketTransport;
        public static NetworkManager Instance { get; private set; }
        public string Token { get; set; } = string.Empty;
        public string Account { get; set; } = string.Empty;

        private AccountInfo defaultAccount { get; } = new AccountInfo() { Account = "ABC", Password = "123" };
        private AccountInfo currLoginInfo = new AccountInfo();
        private Dictionary<string, int> rolePortDic = new();
        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;

            httpTransport = new HttpTransport();
            webSocketTransport = new WebSocketTransport();

            TickManager.Instance.StartTickLoop();
            FrameManager.Instance.StartFrameLoop();
        }
        private void Start()
        {
            httpTransport.RegistService(host, port, null);
            if (isAutoLogin) AutoLogin();
        }

        private void AutoLogin()
        {
            SetLoginInfo(defaultAccount.Account, defaultAccount.Password);
            HttpLogin();
        }
        public void SetLoginInfo(string account, string password)
        {
            currLoginInfo.Account = account;
            currLoginInfo.Password = password;
        }
        public void HttpLogin()
        {
            webSocketTransport.Stop();

            LoginApi loginApi = new();
            LoginHttpLoginRequest request = new LoginHttpLoginRequest();
            request.Account = currLoginInfo.Account;
            request.Password = currLoginInfo.Password;
            loginApi.LoginHttpLogin(request, OnHttpLogin);
        }

        private void OnHttpLogin(bool succ, LoginHttpLoginResponse response)
        {
            if (!succ) return;

            if (response.Code != 200) 
            {
                HandleError((ErrorCode)response.Code);
                return;
            }

            Account = response.Account;
            Token = response.Token;

            var processes = response.ProcessInfos;
            foreach (var info in processes)
            {
                // Debug模式只连接Debug进程，非Debug模式不连接Debug进程
                bool isCheck = (isDebug && info.Role == "Debug") || (!isDebug && info.Role != "Debug");
                if (!isCheck) continue;

                string role = info.Role;
                string host = info.Host.Equals("http://localhost") ? this.host : info.Host;
                int port = info.Port;

                rolePortDic.AddOrReplace(role, port);
                httpTransport.RegistService(host, port, info.Modules);
                webSocketTransport.RegistService(host, port, info.Modules);

                //直接连接WebSocket
                // if(info.UseWebSocket) WebSocketConnect(role);
            }
        }
        private void HandleError(ErrorCode errorCode)
        {
            if (errorCode == ErrorCode.AccountNotExists)
            {
                Debug.Log("Account not exists, creating account...");
                CreateAccountApi createAccountApi = ApiManager.GetHttpApi<CreateAccountApi>();
                CreateAccountCreateRequest req = new CreateAccountCreateRequest();
                req.Account = currLoginInfo.Account;
                req.Password = currLoginInfo.Password;
                createAccountApi.CreateAccountCreate(req, OnCreateAccount);
            }
        }
        private void OnCreateAccount(bool arg1, CreateAccountCreateResponse response)
        {
            if (!arg1) return;
            Account = response.Account;
            Token = response.Token;
        }
        private void OnApplicationQuit()
        {
            rolePortDic.Clear();
            webSocketTransport.Stop();
            TickManager.Instance.StopTickLoop();
            FrameManager.Instance.StopFrameLoop();
        }
        public async void WebSocketConnect(string role)
        {
            int port = rolePortDic.ContainsKey(role) ? rolePortDic[role] : this.port;
            await webSocketTransport.ConnectAsync(port, Token);
        }
        public async Task<HttpResult<TRes>> HttpPostAsync<TReq, TRes>(string path, TReq req)
        {
            var result = await httpTransport.PostAsync<TReq, TRes>(path, req);
            return result;
        }

        public void SendWebSocketMessage<T>(string pattern, string path, T messageObj)
        {
            WebSocketMessage<T> wsMessage = new WebSocketMessage<T>();
            wsMessage.Type = EWsMessageType.Normal;
            wsMessage.InputFrame = FrameManager.Instance.LocalCurrentFrame;
            wsMessage.Timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            wsMessage.Pattern = pattern;
            wsMessage.Path = path;
            wsMessage.Data = messageObj;

            string msg = JsonConvert.SerializeObject(wsMessage);
            webSocketTransport.SendMessage(pattern, msg);
        }
    }
}
