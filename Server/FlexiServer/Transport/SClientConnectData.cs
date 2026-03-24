using FlexiServer.Core;

namespace FlexiServer.Transport
{
    public struct SClientConnectData
    {
        public SClientConnectData()
        {
        }

        public string ClientId { get; set; } = "";
        public string Account { get; set; } = "";
    }
}
