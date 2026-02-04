using System;
using UnityEngine;
using Network.Models;
namespace Network.API
{
    public class LoginApi : HttpMessageApi
    {
        #region AutoContext
        public async void LoginHttpLogin(LoginHttpLoginRequest req, Action<bool,LoginHttpLoginResponse> action)
        {
            await PostAsync<LoginHttpLoginRequest, LoginHttpLoginResponse>("/login/httpLogin", req, (result) =>
            {
                bool success = result.Code == 200 && result.Data != null;

                if (success) action?.Invoke(success, result.Data);
                else 
                {
                    Debug.LogError($"LoginApi LoginHttpLogin failed: Code={result.Code}, Message={result.Message}");
                    action?.Invoke(success, null); 
                }
            });
        }
        public async void LoginValidate(LoginValidateRequest req, Action<bool,LoginValidateResponse> action)
        {
            await PostAsync<LoginValidateRequest, LoginValidateResponse>("/login/Validate", req, (result) =>
            {
                bool success = result.Code == 200 && result.Data != null;

                if (success) action?.Invoke(success, result.Data);
                else 
                {
                    Debug.LogError($"LoginApi LoginValidate failed: Code={result.Code}, Message={result.Message}");
                    action?.Invoke(success, null); 
                }
            });
        }
         #endregion HttpFuncStr
    }
}