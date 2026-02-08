using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;

namespace Network.Transport.WebSocket
{

    public class WebSocketTransport
    {
        private Dictionary<int, HashSet<string>> portDic = new();
        private Dictionary<int, WebSocketSession> sessionDic = new();
        public void RegistService(string host, int port, List<string> modules)
        {
            if (!sessionDic.ContainsKey(port)) sessionDic[port] = new WebSocketSession(host, port);
            if (portDic.ContainsKey(port)) portDic[port].AddRange(modules);
            else portDic[port] = new HashSet<string>(modules);
        }
        private int GetPortByPattern(string pattern)
        {
            string moduleName = pattern.Split('/')[1].ToLower();
            var result = portDic.FirstOrDefault(kvp => kvp.Value.Any(module =>
            pattern.Contains(module, StringComparison.OrdinalIgnoreCase)));

            if (result.Value != null && result.Value.Any()) return result.Key;
            return NetworkManager.Instance.port;
        }
        public void SendMessage(string pattern, string msg)
        {
            int port = GetPortByPattern(pattern);
            if (!sessionDic.ContainsKey(port)) return;

            WebSocketSession session = sessionDic[port];
            session.SendMessageAsync(msg);
        }

        public async Task ConnectAsync(int port,string token)
        {
            if (!sessionDic.ContainsKey(port)) return;

            WebSocketSession session = sessionDic[port];
            await session.ConnectAsync(token);
        }

        public void Stop()
        {
            foreach (var kvp in sessionDic) kvp.Value.DisconnectAsync();
        }
    }
}
