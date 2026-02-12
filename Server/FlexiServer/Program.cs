using FlexiServer.Core;
using FlexiServer.Core.Frame;
using FlexiServer.Core.Tick;
using FlexiServer.Infrastructure.Database;
using FlexiServer.Infrastructure.InternalServices;
using FlexiServer.Models.Common;
using FlexiServer.Sandbox;
using FlexiServer.Services;
using FlexiServer.Transport;
using FlexiServer.Transport.Udp;
using FlexiServer.Transport.Web;

var builder = WebApplication.CreateBuilder(args);
var argDict = args.Select(a => a.Split('=', 2)) .Where(p => p.Length == 2).ToDictionary(p => p[0], p => p[1]);
string role = argDict.GetValueOrDefault("role", "Debug");

var processes = builder.Configuration.GetSection("Processes").Get<Dictionary<string, ProcessInfo>>();
var processInfo = processes != null && processes.ContainsKey(role) ? processes[role] : null;
var port = processInfo != null ? processInfo.Port : 8080;
var modules = processInfo != null ? processInfo.Modules : null;

builder.Services.AddSingleton(sp => new Database(role));
builder.Services.AddSingleton<InternalServiceClient>();

builder.Services.AddSingleton<TransportManager>();
builder.Services.AddSingleton<ServiceManager>();
builder.Services.AddSingleton<TickManager>();
builder.Services.AddSingleton<FrameManager>();
builder.Services.AddSingleton<TokenManager>();
builder.Services.AddSingleton<SandboxManager>();
builder.Services.AddSingleton<WebSocketTransport>();
builder.Services.AddSingleton<UdpTransport>();

// 注册服务
builder.AddSingletonByConfig(modules);

var app = builder.Build();
app.UseWebSockets();

WebSocketTransport? webSocket = app.Services.GetService<WebSocketTransport>();
UdpTransport? udpTransport = app.Services.GetService<UdpTransport>();

app.Services.GetRequiredService<TransportManager>().RgiestTransport(webSocket);
app.Services.GetRequiredService<TransportManager>().RgiestTransport(udpTransport);
app.MapWebSocketEndpoints();

//注册长连接协议接口
app.RegisterSocketServiceByConfig(modules);

//注册协议接口
app.MapPostByConfig(modules);

//开启Udp协议接口
app.StartUdpListen(port);

// 打印注册结果
ServerModuleRegister.ConsoleRegistLog(role);

// 在应用启动时:
app.Lifetime.ApplicationStarted.Register(app.Services.GetRequiredService<TransportManager>().OnApplicationStarted);
app.Lifetime.ApplicationStarted.Register(app.Services.GetRequiredService<ServiceManager>().Initialize);
app.Lifetime.ApplicationStarted.Register(app.Services.GetRequiredService<Database>().InitializeDatabase);
app.Lifetime.ApplicationStarted.Register(app.Services.GetRequiredService<TickManager>().Start);
app.Lifetime.ApplicationStarted.Register(app.Services.GetRequiredService<FrameManager>().StartFrameLoop);
app.Lifetime.ApplicationStarted.Register(app.Services.GetRequiredService<SandboxManager>().StarSandboxUpdateLoop);
// 在应用停止时:
app.Lifetime.ApplicationStopped.Register(app.Services.GetRequiredService<TransportManager>().OnApplicationStopped);
app.Lifetime.ApplicationStopped.Register(app.Services.GetRequiredService<ServiceManager>().Shutdown);
app.Lifetime.ApplicationStopped.Register(app.Services.GetRequiredService<Database>().Dispose);
app.Lifetime.ApplicationStopped.Register(app.Services.GetRequiredService<TickManager>().Stop);
app.Lifetime.ApplicationStopped.Register(app.Services.GetRequiredService<FrameManager>().StopFrameLoop);
app.Lifetime.ApplicationStopped.Register(app.Services.GetRequiredService<SandboxManager>().StopSandboxUpdateLoop);

app.Run($"http://0.0.0.0:{port}");


