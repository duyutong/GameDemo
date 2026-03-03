using FlexiServer.Core;

namespace FlexiServer.Transport.Interface
{
    public interface ITransport
    {
        /// <summary> 发送消息 </summary>
        void SendMessage<TData>(string clientKey, string pattern, string path, TData data);

        /// <summary> 注册消息接收回调 </summary>
        void SetMessageReceivedListener(Action<SClientConnectData, string, byte[]> receivedCall);

        /// <summary> 启动传输服务 </summary>
        void Start();

        /// <summary> 停止传输服务 </summary>
        void Stop();
        void SetConnectionStateChangedListener(Action<SClientConnectData, EPlayerConnectionState> onConnectionStateChanged);
    }
}
