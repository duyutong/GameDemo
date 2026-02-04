using UnityEngine.Pool;

namespace Network.Core.Frame
{
    public class FrameMessagePool
    {
        private static readonly ObjectPool<FrameMessage> _pool = new(OnGet,OnRelease);
        public static FrameMessage OnGet()
        {
            var f = _pool.Get();
            f.InputFrame = 0;
            f.ClientId = "";
            f.Command = "";
            return f;
        }

        public static void OnRelease(FrameMessage f)
        {
            _pool.Release(f);
        }
    }
}
