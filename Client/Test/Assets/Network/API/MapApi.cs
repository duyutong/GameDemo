using System;
using UnityEngine;
using Network.Models;
using Assets.Network.Transport;
namespace Network.API
{
    public class MapApi : HttpMessageApi
    {
        #region AutoContext
        
        public async void MapGenerate(MapGenerateRequest req, Action<bool,MapGenerateResponse> action)
        {
            await PostAsync<MapGenerateRequest, MapGenerateResponse>("/map/generate", req, (result) =>
            {
                bool success = result.Code == 200 && result.Data != null;

                if (success) action?.Invoke(success, result.Data.ConvertData<MapGenerateResponse>());
                else 
                {
                    Debug.LogError($"MapApi MapGenerate failed: Code={result.Code}, Message={result.Message}");
                    action?.Invoke(success, null); 
                }
            });
        }
        #endregion HttpFuncStr
    }
}