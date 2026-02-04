using FlexiServer.Core;

namespace FlexiServer.Sandbox.Interface
{
    public interface ISandboxPlayer
    {
        bool ContainsPlayer(string account);
        void OnPlayerConnectionStateChanged(
            string clientId,
            string account,
            EPlayerConnectionState state
        );
    }
}
