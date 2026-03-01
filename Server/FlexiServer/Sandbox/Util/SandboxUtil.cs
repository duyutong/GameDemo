using System.Numerics;

namespace FlexiServer.Sandbox.Util
{
    public static class SandboxUtil
    {
        public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
        {
            t = Math.Clamp(t, 0, 1);
            Vector3 result = default;
            result.X = a.X + (b.X - a.X) * t;
            result.Y = a.Y + (b.Y - a.Y) * t;
            result.Z = a.Z + (b.Z - a.Z) * t;
            return result;
        }
    }
}
