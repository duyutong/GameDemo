using FlexiServer.Core;

namespace FlexiServer.Sandbox.Interface
{
    public interface ISandboxPlayer
    {
        public void AddPlayer(string clientId, string account);
        bool ContainsPlayer(string account);
        void OnPlayerConnectionStateChanged(
            string clientId,
            string account,
            EPlayerConnectionState state
        );
    }
}
