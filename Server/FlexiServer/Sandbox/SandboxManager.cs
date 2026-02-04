using FlexiServer.Core;
using FlexiServer.Core.Frame;
using FlexiServer.Core.Tick;
using FlexiServer.Sandbox.Interface;
using FlexiServer.Transport;
namespace FlexiServer.Sandbox
{
    public class SandboxManager(TickManager tickMgr,FrameManager frameMgr,TransportManager transportMgr)
    {
        public static float DeltaTime { get { return _deltaTime; } }
        private static float _deltaTime;

        private SandBoxPool pool = new();
        
        private TickHandle? tickHandle;

        private Dictionary<Type, List<SandboxBase>> currDic = [];
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
                list.ForEach(item => { item.Destroy(); });

            tickHandle?.Stop();
            pool.ClearAll();
        }
        public List<SandboxBase>? GetSandBox(Func<SandboxBase, bool> select)
        {
            var list = currDic.Values.SelectMany(x => x).ToList();
            var result = list.Where(select).ToList();
            return result.Count > 0 ? result : null;
        }
        public TSandBox? GetOrCreateSandBox<TSandBox>(Func<TSandBox, bool> select, Action<TSandBox>? init = null) where TSandBox : SandboxBase
        {
            Type sandBoxType = typeof(TSandBox);
            TSandBox? sandBox;
            if (!currDic.TryGetValue(sandBoxType, out var list))
            {
                list = new List<SandboxBase>();
                sandBox = (TSandBox?)pool.GetSandBox(sandBoxType);
                if (sandBox != null)
                {
                    if (init != null) sandBox.Init(init);
                    if (select.Invoke(sandBox)) list.Add(sandBox);
                    currDic[sandBoxType] = list;
                }
                return sandBox;
            }
            if (list.Count > 0)
            {
                sandBox = list.OfType<TSandBox>().FirstOrDefault(select);
                if (sandBox != null) return sandBox;
                
                sandBox ??= (TSandBox?)pool.GetSandBox(sandBoxType);
                if (sandBox != null)
                {
                    if (init != null) sandBox.Init(init);
                    if (select.Invoke(sandBox)) currDic[sandBoxType].Add(sandBox);
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
                sandBox.Update();
            }
        }
    }
}