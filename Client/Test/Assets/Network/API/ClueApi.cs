using System;
using UnityEngine;
using Network.Models;
namespace Network.API
{
    public class ClueApi : HttpMessageApi
    {
        #region AutoContext
        
        public async void ClueIdentify(ClueIdentifyRequest req, Action<bool,ClueIdentifyResponse> action)
        {
            await PostAsync<ClueIdentifyRequest, ClueIdentifyResponse>("/clue/identify", req, (result) =>
            {
                bool success = result.Code == 200 && result.Data != null;

                if (success) action?.Invoke(success, result.Data);
                else 
                {
                    Debug.LogError($"ClueApi ClueIdentify failed: Code={result.Code}, Message={result.Message}");
                    action?.Invoke(success, null); 
                }
            });
        }
         #endregion HttpFuncStr
    }
}