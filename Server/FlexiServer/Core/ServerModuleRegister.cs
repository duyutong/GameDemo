using FlexiServer.Services;
using FlexiServer.Services.Interface;
using FlexiServer.Transport.Http;
using System.Reflection;

namespace FlexiServer.Core
{
    public static class ServerModuleRegister
    {
        private static List<string> serviceTypes = new();
        private static List<string> endpointMethods = new();
        public static bool CheckIsRegisteredService(string serviceName)
        {
            return serviceTypes.Contains(serviceName);
        }
        public static void AddSingletonByConfig(this WebApplicationBuilder builder, List<string>? modules)
        {
            if (builder == null) return;
            if (modules == null) return;

            var types = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(a =>
            {
                try { return a.GetTypes(); }
                catch (ReflectionTypeLoadException e) { return e.Types.Where(t => t != null); }
            })
            .Where(t => t != null && t.IsClass && !t.IsAbstract && !string.IsNullOrEmpty(t.Namespace)
            && t.Namespace.StartsWith("FlexiServer.Services"));

            foreach (var type in types)
            {
                if (type == null) continue;

                var moduleAttr = type.GetCustomAttribute<ProcessFeatureAttribute>();
                if (moduleAttr == null) continue;

                if (!modules.Contains(moduleAttr.Module)) continue;

                serviceTypes.Add(moduleAttr.Module);
                builder.Services.AddSingleton(type);
            }
        }
        /// <summary>
        /// 根据指定的模块列表，从当前程序域中查找所有符合条件的 WebSocket 服务类（继承 <see cref="IService"/> 且标记了 <see cref="ProcessFeatureAttribute"/>），
        /// 并将其实例从依赖注入容器中获取后注册到 <see cref="ServiceManager"/>。
        /// </summary>
        /// <param name="app">当前的 <see cref="WebApplication"/> 实例。</param>
        /// <param name="modules">需要注册的模块名称列表，只有服务类的模块在列表中才会被注册。</param>
        public static void RegisterWebSocketServiceByConfig(this WebApplication app, List<string>? modules)
        {
            if (app == null) return;
            if (modules == null) return;

            var types = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a =>
                {
                    try { return a.GetTypes(); }
                    catch (ReflectionTypeLoadException e) { return e.Types.Where(t => t != null); }
                })
                .Where(t => t != null
                            && t.IsClass
                            && !t.IsAbstract
                            && typeof(IService).IsAssignableFrom(t)
                            && !string.IsNullOrEmpty(t.Namespace)
                            && t.Namespace.StartsWith("FlexiServer.Services"));

            foreach (var type in types)
            {
                if (type == null) continue;

                var moduleAttr = type.GetCustomAttribute<ProcessFeatureAttribute>();
                if (moduleAttr == null) continue;

                if (!modules.Contains(moduleAttr.Module)) continue;

                var service = app.Services.GetRequiredService(type);
                app.Services.GetRequiredService<ServiceManager>().RegisterService(service as IService);
            }
        }
        public static void MapPostByConfig(this WebApplication app, List<string>? modules)
        {
            if (app == null) return;
            if (modules == null) return;

            Type type = typeof(MapPostEndpoints);
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static);
            foreach (var method in methods)
            {
                var postAttr = method.GetCustomAttribute<ProcessFeatureAttribute>();
                if (postAttr == null) continue;
                if (!modules.Contains(postAttr.Module)) continue;

                endpointMethods.Add($"Map{postAttr.Module}Endpoints");
                method.Invoke(null, [app]);
            }
        }
        public static void ConsoleRegistLog(string role) 
        {
            PrintModuleHeader(role);

            foreach (var serviceType in serviceTypes)
            {
                PrintServiceRegistered(serviceType);
            }

            foreach (var endpointMethod in endpointMethods)
            {
                PrintEndpointRegistered(endpointMethod);
            }

            PrintSeparator();
        }
        private static void PrintModuleHeader(string moduleName)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\n===== MODULE: {moduleName} =====");
            Console.ResetColor();
        }

        private static void PrintServiceRegistered(string serviceName)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($" - Service: [{serviceName}] Registered");
            Console.ResetColor();
        }

        private static void PrintEndpointRegistered(string endpointName)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($" - Endpoint: [{endpointName}] Registered");
            Console.ResetColor();
        }

        private static void PrintSeparator()
        {
            Console.WriteLine(new string('-', 50));
        }

    }
}
