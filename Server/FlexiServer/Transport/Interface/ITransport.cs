using FlexiServer.Core;

namespace FlexiServer.Transport.Interface
{
    public interface ITransport
    {
        /// <summary> 发送消息 </summary>
        void SendMessage(string clientKey, string message);

        /// <summary> 注册消息接收回调 </summary>
        void SetMessageReceivedListener(Action<SClientConnectData,string,string> receivedCall);

        /// <summary> 启动传输服务 </summary>
        void Start();

        /// <summary> 停止传输服务 </summary>
        void Stop();
        void SetConnectionStateChangedListener(Action<SClientConnectData, EPlayerConnectionState> onConnectionStateChanged);
    }

}
