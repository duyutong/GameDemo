using Network;
using Network.Transport.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;

namespace Network.API
{
    public class HttpMessageApi
    {
        public async Task<HttpResult<TRes>> PostAsync<TReq, TRes>(string path, TReq req, Action<HttpResult<TRes>> callBack)
        {
            var result = await NetworkManager.Instance.HttpPostAsync<TReq, TRes>(path, req);
            callBack?.Invoke(result);
            return result;
        }
    } 
}
