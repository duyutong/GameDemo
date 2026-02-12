using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;

namespace Network.Transport.Udp
{
    public class UdpTransport
    {
        private Dictionary<int, HashSet<string>> portDic = new();
        private Dictionary<int, UdpSession> sessionDic = new();
        public void RegistService(string host, int port, List<string> modules)
        {
            if (!sessionDic.ContainsKey(port)) sessionDic[port] = new UdpSession(host, port);
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
        public void SendMessage(string pattern,string msg) 
        {
            int port = GetPortByPattern(pattern);
            if (!sessionDic.ContainsKey(port)) return;

            UdpSession session = sessionDic[port];
            session.SendMessageAsync(msg);
        }
        public async Task Connect(int port) 
        {
            if (!sessionDic.ContainsKey(port)) return;

            UdpSession session = sessionDic[port];
            await session.Connect();

        }
        public void Stop()
        {
            foreach (var kvp in sessionDic) kvp.Value.DisconnectAsync();
        }
    }
}