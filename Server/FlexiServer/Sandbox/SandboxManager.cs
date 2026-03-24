using FlexiServer.Core;
using FlexiServer.Core.Frame;
using FlexiServer.Core.Tick;
using FlexiServer.Sandbox.Interface;
using FlexiServer.Transport;
using System.Collections.Concurrent;
namespace FlexiServer.Sandbox
{
    public class SandboxManager(TickManager tickMgr, FrameManager frameMgr, TransportManager transportMgr)
    {
        public static float DeltaTime { get { return _deltaTime; } }
        private static float _deltaTime;

        private SandBoxPool pool = new();

        private TickHandle? tickHandle;

        private ConcurrentDictionary<Type, List<SandboxBase>> currDic = [];
        public void StarSandboxUpdateLoop()
        {
            transportMgr.AddClientConnHandler(OnClientConnectionStateChanged);

            int frameSyncIntervalMs = frameMgr.FrameSyncIntervalMs;
            _deltaTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
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
        public void ReleaseSandBox(SandboxBase instance)
        {
            var type = instance.GetType();
            if (currDic.ContainsKey(type)) currDic[type].Remove(instance);
            pool.Release(instance);
        }
        public TSandBox? GetSandBox<TSandBox>(Func<TSandBox, bool> select) where TSandBox : SandboxBase
        {
            return currDic.Values.SelectMany(x => x)
                           .OfType<TSandBox>()    // 先过滤类型
                           .FirstOrDefault(select); // 再筛选
        }
        public List<SandboxBase>? GetSandBox(Func<SandboxBase, bool> select)
        {
            var list = currDic.Values.SelectMany(x => x).ToList();
            var result = list.Where(select).ToList();
            return result.Count > 0 ? result : null;
        }
        public TSandBox? GetOrCreateSandBox<TSandBox>(Func<TSandBox, bool>? select = null, Action<TSandBox>? init = null) where TSandBox : SandboxBase
        {
            Type sandBoxType = typeof(TSandBox);
            TSandBox? sandBox;
            if (!currDic.TryGetValue(sandBoxType, out var list)) 
            {
                list = new List<SandboxBase>();
                currDic[sandBoxType] = list;
            }
            if (currDic[sandBoxType].Count == 0)
            {
                sandBox = (TSandBox?)pool.GetSandBox(sandBoxType);
                if (sandBox != null)
                {
                    sandBox.Init(init);
                    sandBox.OnReleaseAction = ReleaseSandBox;

                    if (select == null) currDic[sandBoxType].Add(sandBox);
                    if (select != null && select.Invoke(sandBox)) currDic[sandBoxType].Add(sandBox);
                }
                return sandBox;
            }
            if (currDic[sandBoxType].Count > 0)
            {
                var checkList = currDic[sandBoxType].OfType<TSandBox>();
                sandBox = select != null ? checkList.FirstOrDefault(select) : checkList.First();
                if (sandBox != null) return sandBox;

                sandBox ??= (TSandBox?)pool.GetSandBox(sandBoxType);
                if (sandBox != null)
                {
                    if (init != null) sandBox.Init(init);
                    if (select == null) currDic[sandBoxType].Add(sandBox);
                    if (select != null && select.Invoke(sandBox)) currDic[sandBoxType].Add(sandBox);

                    sandBox.OnReleaseAction = ReleaseSandBox;
                }
                return sandBox;
            }
            return null;
        }
        private void OnClientConnectionStateChanged(string clientId, string account, EPlayerConnectionState connectionState)
        {
            var list = GetSandBox(s => s is ISandboxPlayer p && p.ContainsPlayer(account))
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
            }
        }
    }
}