using FlexiServer.Core;

namespace FlexiServer.Transport.Interface
{
    public interface ITransport
    {
        /// <summary> 发送消息 </summary>
        void SendMessage(string clientId, string message);

        /// <summary> 注册消息接收回调 </summary>
        void OnMessageReceived(Action<ClientConnectData,string,string> receivedCall);

        /// <summary> 启动传输服务 </summary>
        void Start();

        /// <summary> 停止传输服务 </summary>
        void Stop();
        void OnConnectionStateChanged(Action<ClientConnectData, EPlayerConnectionState> onConnectionStateChanged);
    }

}
