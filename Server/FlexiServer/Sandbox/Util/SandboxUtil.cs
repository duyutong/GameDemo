using System.Numerics;

namespace FlexiServer.Sandbox.Util
{
    public static class SandboxUtil
    {
        public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
        {
            t = Math.Clamp(t, 0, 1);
            Vector2 result = default;
            result.X = a.X + (b.X - a.X) * t;
            result.Y = a.Y + (b.Y - a.Y) * t;
            return result;
        }
    }
}
