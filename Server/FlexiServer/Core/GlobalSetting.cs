namespace FlexiServer.Core
{

    public static class GlobalSetting
    {
        public static string Role { get; set; }= "Debug";
        public static ETransportFormat Format {
            get
            {
                if (Enum.TryParse<ETransportFormat>(FormatStr, ignoreCase: true,out var result))
                {
                    return result;
                }

                // 非法值兜底
                return ETransportFormat.Json;
            }
        }
        public static string FormatStr {private get; set; }= "Json";
    }
    public enum ETransportFormat
    {
        Json = 0,
        Protobuf = 1
    }
}
