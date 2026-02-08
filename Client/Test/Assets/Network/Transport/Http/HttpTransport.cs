using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;

namespace Network.Transport.Http
{
    public class HttpTransport
    {
        private Dictionary<int, HashSet<string>> portDic = new();
        private Dictionary<int, HttpSession> sessionDic = new();
        public async Task<HttpResult<TRes>> PostAsync<TReq, TRes>(string path, TReq req)
        {
            int port = GetPortByPattern(path);
            if (!sessionDic.ContainsKey(port)) return null;

            HttpSession session = sessionDic[port];
            return await session.PostAsync<TReq, TRes>(path, req);
        }
        public void RegistService(string host, int port, List<string> modules)
        {
            if (!sessionDic.ContainsKey(port)) sessionDic[port] = new HttpSession(host, port);

            if (modules != null) 
            {
                if (portDic.ContainsKey(port)) portDic[port].AddRange(modules);
                else portDic[port] = new HashSet<string>(modules);
            }
        }
        private int GetPortByPattern(string pattern)
        {
            string moduleName = pattern.Split('/')[1].ToLower();
            var result = portDic.FirstOrDefault(kvp => kvp.Value.Any(module =>
            pattern.Contains(module, StringComparison.OrdinalIgnoreCase)));

            if (result.Value != null && result.Value.Any()) return result.Key;
            return NetworkManager.Instance.port;
        }
    }

}