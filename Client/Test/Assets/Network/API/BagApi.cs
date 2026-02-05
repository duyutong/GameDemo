using System;
using UnityEngine;
using Network.Models;
namespace Network.API
{
    public class BagApi : HttpMessageApi
    {
        #region AutoContext
        
        public async void BagAcquireItem(BagAcquireItemRequest req, Action<bool,BagAcquireItemResponse> action)
        {
            await PostAsync<BagAcquireItemRequest, BagAcquireItemResponse>("/bag/acquireItem", req, (result) =>
            {
                bool success = result.Code == 200 && result.Data != null;

                if (success) action?.Invoke(success, result.Data);
                else 
                {
                    Debug.LogError($"BagApi BagAcquireItem failed: Code={result.Code}, Message={result.Message}");
                    action?.Invoke(success, null); 
                }
            });
        }
         #endregion HttpFuncStr
    }
}