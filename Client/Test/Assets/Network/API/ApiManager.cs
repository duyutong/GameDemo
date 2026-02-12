using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network.Transport.WebSocket;
using UnityEngine.InputSystem.Interactions;
namespace Network.API
{
    public class ApiManager
    {
        private static Dictionary<string, Type> patternToType = new Dictionary<string, Type>()
        {
            #region patternToType
            { "/chat", typeof(ChatApi) },
            { "/gamePlay", typeof(GamePlayApi) },
            { "/playerMovement", typeof(PlayerMovementApi) },
            #endregion patternToType
        };
        private static Dictionary<Type, UdpMessageApi> udpApiDic = new Dictionary<Type, UdpMessageApi>();
        private static Dictionary<Type, HttpMessageApi> httpApiDict = new Dictionary<Type, HttpMessageApi>();
        private static Dictionary<Type, WebSocketMessageApi> webSoketApiDict = new Dictionary<Type, WebSocketMessageApi>();

        public static TApi GetUdpApi<TApi>() where TApi : UdpMessageApi
        {
            Type type = typeof(TApi);
            if (!udpApiDic.ContainsKey(type))
            {
                TApi apiInstance = (TApi)Activator.CreateInstance(type);
                udpApiDic[type] = apiInstance;
            }
            return (TApi)udpApiDic[type];
        }
        public static TApi GetHttpApi<TApi>() where TApi : HttpMessageApi
        {
            Type type = typeof(TApi);
            if (!httpApiDict.ContainsKey(type))
            {
                TApi apiInstance = (TApi)Activator.CreateInstance(type);
                httpApiDict[type] = apiInstance;
            }
            return (TApi)httpApiDict[type];
        }
        public static TApi GetWebSoketApi<TApi>() where TApi : WebSocketMessageApi
        {
            Type type = typeof(TApi);
            if (!webSoketApiDict.ContainsKey(type))
            {
                TApi apiInstance = (TApi)Activator.CreateInstance(type);
                webSoketApiDict[type] = apiInstance;
            }
            return (TApi)webSoketApiDict[type];
        }
        public static void HandleUdpMessage(string pattern, string msg)
        {
            if (string.IsNullOrEmpty(pattern)) return;

            if (patternToType.TryGetValue(pattern, out var type))
            {
                if (!udpApiDic.ContainsKey(type))
                {
                    var apiInstance = (UdpMessageApi)Activator.CreateInstance(type);
                    if (apiInstance == null) goto Faile;
                    udpApiDic[type] = apiInstance;
                }
                udpApiDic[type].OnDataRecieved(pattern, msg);
                return;
            }
        Faile:
            {
                Debug.LogWarning($"未找到 pattern = {pattern} 的 API 类");
                return;
            }
        }
        public static void HandleMessage(string pattern, string msg)
        {
            if (string.IsNullOrEmpty(pattern)) return;

            if (patternToType.TryGetValue(pattern, out var type))
            {
                if (!webSoketApiDict.ContainsKey(type))
                {
                    var apiInstance = (WebSocketMessageApi)Activator.CreateInstance(type);
                    if (apiInstance == null) goto Faile;
                    webSoketApiDict[type] = apiInstance;
                }
                webSoketApiDict[type].OnDataRecieved(pattern, msg);
                return;
            }
        Faile:
            {
                Debug.LogWarning($"未找到 pattern = {pattern} 的 API 类");
                return;
            }
        }
    }
}
