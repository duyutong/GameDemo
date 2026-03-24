using FlexiServer.Core.Frame;
using FlexiServer.Sandbox;
using FlexiServer.Services.Interface;
using FlexiServer.Transport;
using System.Collections.Concurrent;

namespace FlexiServer.Services
{
    public class ServiceManager(TransportManager transportMgr, SandboxManager sandboxManager,FrameManager frameManager)
    {
        private readonly ConcurrentDictionary<string, IService> services = new();
        public void Initialize()
        {
            transportMgr.AddClientMsgHandler(OnClientMessageReceived);
            RegisterHandlers();
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
            if (service == null) return;
            services[service.Pattern] = service;
        }
        private IService? GetService(string pattern)
        {
            if (services.ContainsKey(pattern)) return services[pattern];
            else return null;
        }
        private void RegisterHandlers()
        {
            foreach (var service in services.Values)
            {
                var serviceType = service.GetType();

                foreach (var iface in serviceType.GetInterfaces())
                {
                    // -------- 泛型接口 --------
                    if (iface.IsGenericType)
                    {
                        var genericDef = iface.GetGenericTypeDefinition();

                        if (genericDef == typeof(ISandboxUpdateHandler<>))
                        {
                            var adapter = RegisterSandboxHandler(service, iface, "OnSandboxUpdate");
                            if (adapter != null)
                                sandboxManager.OnManagerUpdated += adapter;
                        }
                        else if (genericDef == typeof(ISandboxInitHandler<>))
                        {
                            var adapter = RegisterSandboxHandler(service, iface, "OnSandboxInit");
                            if (adapter != null)
                                sandboxManager.OnManagerInited += adapter;
                        }
                    }
                    // -------- 非泛型接口 --------
                    else
                    {
                        if (iface == typeof(IFrameResolvedHandler))
                        {
                            var adapter = RegisterFramexHandler(service, iface, "OnFrameResolved");
                            if (adapter != null)
                                frameManager.OnFrameResolved += adapter;
                        }
                    }
                }
            }
        }

        private static Action<int, List<FrameMessage>>? RegisterFramexHandler(IService service, Type iface, string methodName)
        {
            var method = iface.GetMethod(methodName);
            if (method == null) return null;

            var typedHandler = Delegate.CreateDelegate(typeof(Action<int, List<FrameMessage>>), service, method);
            void adapter(int _frame, List<FrameMessage> _list)
            {
                List<FrameMessage> commands = [];
                foreach (var frameMsg in _list)
                {
                    string pattern = frameMsg.Pattern;
                    if (service.Pattern != pattern) continue;
                    commands.Add(frameMsg);
                    typedHandler.DynamicInvoke(_frame, commands);
                }
            }
            return adapter;
        }

        private static Action<SandboxBase>? RegisterSandboxHandler(IService service, Type iface, string methodName)
        {
            var dataType = iface.GetGenericArguments()[0];
            var method = iface.GetMethod(methodName);
            if (method == null) return null;

            var typedHandler = Delegate.CreateDelegate(typeof(Action<>).MakeGenericType(dataType), service, method);
            void adapter(SandboxBase sandbox)
            {
                if (!dataType.IsInstanceOfType(sandbox)) return;
                typedHandler.DynamicInvoke(sandbox);
            }

            return adapter;
        }
    }
}
