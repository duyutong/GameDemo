using FlexiServer.Services.Interface;
using FlexiServer.Transport;
using System.Collections.Concurrent;

namespace FlexiServer.Services
{
    public class ServiceManager(TransportManager transportMgr)
    {
        private readonly ConcurrentDictionary<string, IService> services = new();
        public void Initialize()
        {
            transportMgr.AddClientMsgHandler(OnClientMessageReceived);
        }
        public void Shutdown()
        {
            transportMgr.RemoveClientMsgHandler(OnClientMessageReceived);
        }
        private void OnClientMessageReceived(string pattern, string clientId, string account, string msg)
        {
            IService? service = GetService(pattern);
            if (service == null) return;

            service.OnDataRecieved(clientId, account, msg);
        }

        public void RegisterService(IService? service)
        {
            if(service == null)return;
            services[service.Pattern] = service;
        }
        private IService? GetService(string pattern)
        {
            if (services.ContainsKey(pattern)) return services[pattern];
            else return null;
        }
    }
}
