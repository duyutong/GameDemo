using FlexiServer.Core;
using FlexiServer.Sandbox.Interface;

namespace FlexiServer.Sandbox
{
    public class TestSandbox : SandboxBase, ISandboxPlayer
    {
        public bool ContainsPlayer(string account)
        {
            Console.WriteLine($"[TestSandbox] ContainsPlayer {account}");
            return true;
        }
        public void OnPlayerConnectionStateChanged(string clientId, string account, EPlayerConnectionState state)
        {
            Console.WriteLine(
                $"[TestSandbox] OnPlayerConnectionStateChanged | ClientId: {clientId} | Account: {account} | State: {state}"
            );
        }
        public override void Destroy()
        {

        }
        public override void Reset()
        {

        }

        public override void Update()
        {

        }
    }
}
