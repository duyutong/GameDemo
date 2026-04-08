using ConfigData;
public class ConfigLoaderUtil
{
    public static int ConvertToId(object value)
    {
        if (value == null) return -1;

        // 如果本身就是 int
        if (value is int i) return i;

        //常见数值类型（避免直接 GetHashCode）
        if (value is long l) return (int)l;
        if (value is float f) return (int)f;
        if (value is double d) return (int)d;

        // 字符串：优先尝试转 int
        if (value is string s)
        {
            if (int.TryParse(s, out var result)) return result;
            return s.GetHashCode();
        }

        // 其他类型：统一走 HashCode
        return value.GetHashCode();
    }
}
