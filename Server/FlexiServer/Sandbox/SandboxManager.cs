using FlexiServer.Core;
using FlexiServer.Core.Frame;
using FlexiServer.Core.Tick;
using FlexiServer.Sandbox.Interface;
using FlexiServer.Services;
using FlexiServer.Transport;
using System.Collections.Concurrent;
namespace FlexiServer.Sandbox
{
    public class SandboxManager(TickManager tickMgr, FrameManager frameMgr, TransportManager transportMgr)
    {
        public static float DeltaTime { get { return _deltaTime; } }
        private static float _deltaTime;

        public Action<SandboxBase>? OnManagerInited;
        public Action<SandboxBase>? OnManagerUpdated;

        private SandBoxPool pool = new();
        private TickHandle? tickHandle;
        private ConcurrentDictionary<Type, List<SandboxBase>> currDic = [];
        public void StarSandboxUpdateLoop()
        {
            transportMgr.AddClientConnHandler(OnClientConnectionStateChanged);

            int frameSyncIntervalMs = frameMgr.FrameSyncIntervalMs;
            tickHandle = tickMgr.RegisterTick(frameSyncIntervalMs, UpdateLoop);
        }
        public void StopSandboxUpdateLoop()
        {
            transportMgr.RemoveClientConnHandler(OnClientConnectionStateChanged);

            foreach (var list in currDic.Values)
                list.ForEach(item => { item.OnDestroy(); });

            tickHandle?.Stop();
            pool.ClearAll();
        }
       
        public void OnReleaseSandbox(SandboxBase instance)
        {
            var type = instance.GetType();
            if (currDic.ContainsKey(type)) currDic[type].Remove(instance);
            pool.Release(instance);
        }
        public TSandBox? GetSandbox<TSandBox>(Func<TSandBox, bool>? select = null) where TSandBox : SandboxBase
        {
            if (select != null) return currDic.Values.SelectMany(x => x) .OfType<TSandBox>() .FirstOrDefault(select);
            else return currDic.Values.SelectMany(x => x) .OfType<TSandBox>() .FirstOrDefault();
        }
        public List<SandboxBase>? GetSandboxs(Func<SandboxBase, bool> select)
        {
            var list = currDic.Values.SelectMany(x => x).ToList();
            var result = list.Where(select).ToList();
            return result.Count > 0 ? result : null;
        }
        public TSandbox? GetOrCreateSandbox<TSandbox>(Func<TSandbox, bool>? select = null, Action<TSandbox>? init = null) where TSandbox : SandboxBase
        {
            Type sandBoxType = typeof(TSandbox);
            TSandbox? sandBox;
            int nextId = 0;
            if (!currDic.TryGetValue(sandBoxType, out var list)) 
            {
                list = new List<SandboxBase>();
                currDic[sandBoxType] = list;
            }
            if (currDic[sandBoxType].Count == 0)
            {
                sandBox = (TSandbox?)pool.GetSandbox(sandBoxType);
                if (sandBox != null)
                {
                    sandBox.Init(init);
                    sandBox.OnReleaseAction = OnReleaseSandbox;
                    sandBox.SandboxId = Interlocked.Increment(ref nextId);
                    OnManagerInited?.Invoke(sandBox);

                    if (select == null) currDic[sandBoxType].Add(sandBox);
                    if (select != null && select.Invoke(sandBox)) currDic[sandBoxType].Add(sandBox);
                }
                return sandBox;
            }
            if (currDic[sandBoxType].Count > 0)
            {
                var checkList = currDic[sandBoxType].OfType<TSandbox>();
                sandBox = select != null ? checkList.FirstOrDefault(select) : checkList.First();
                if (sandBox != null) return sandBox;

                sandBox ??= (TSandbox?)pool.GetSandbox(sandBoxType);
                if (sandBox != null)
                {
                    if (init != null) sandBox.Init(init);
                    if (select == null) currDic[sandBoxType].Add(sandBox);
                    if (select != null && select.Invoke(sandBox)) currDic[sandBoxType].Add(sandBox);

                    sandBox.OnReleaseAction = OnReleaseSandbox;
                    sandBox.SandboxId = Interlocked.Increment(ref nextId);

                    OnManagerInited?.Invoke(sandBox);
                }
                return sandBox;
            }
            return null;
        }
        private void OnClientConnectionStateChanged(string clientId, string account, EPlayerConnectionState connectionState)
        {
            var list = GetSandboxs(s => s is ISandboxPlayer p && p.ContainsPlayer(account))
                ?.OfType<ISandboxPlayer>();

            foreach (var sandbox in list ?? [])
            {
                sandbox.OnPlayerConnectionStateChanged(
                    clientId,
                    account,
                    connectionState
                );
            }
        }
        private long lastUpdateTime;
        private void UpdateLoop()
        {
            _deltaTime = (DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - lastUpdateTime) * 0.001f;
            lastUpdateTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            foreach (var sandBox in currDic.Values.SelectMany(x => x))
            {
                sandBox.OnUpdate();
                OnManagerUpdated?.Invoke(sandBox);
            }
        }
    }
}