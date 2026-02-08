using Microsoft.Extensions.ObjectPool;
namespace FlexiServer.Core.Frame
{
    public class FrameMessagePool
    {
        private static readonly ObjectPool<FrameMessage> _pool =
        new DefaultObjectPool<FrameMessage>(new DefaultPooledObjectPolicy<FrameMessage>());
        public static FrameMessage Get()
        {
            var f = _pool.Get();
            f.InputFrame = 0;
            f.ClientId = "";
            f.Command = "";
            return f;
        }

        public static void Return(FrameMessage f)
        {
            _pool.Return(f);
        }
    }
}
