using System;
using UnityEngine;
using Network.Models;
namespace Network.API
{
    public class CreateAccountApi : HttpMessageApi
    {
        #region AutoContext
        public async void CreateAccountCreate(CreateAccountCreateRequest req, Action<bool, CreateAccountCreateResponse> action)
        {
            await PostAsync<CreateAccountCreateRequest, CreateAccountCreateResponse>("/createAccount/create", req, (result) =>
            {
                bool success = result.Code == 200 && result.Data != null;

                if (success) action?.Invoke(success, result.Data);
                else
                {
                    Debug.LogError($"CreateAccountApi CreateAccountCreate failed: Code={result.Code}, Message={result.Message}");
                    action?.Invoke(success, null);
                }
            });
        }
        #endregion HttpFuncStr
    }
}