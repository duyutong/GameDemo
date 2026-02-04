namespace Network.Core.Frame
{
    public class FrameMessage
    {
        public int InputFrame { get; set; } = 0;
        public string ClientId { get; set; } = "";
        public string Pattern { get; set; } = "";
        public string Command { get; set; } = "";
    }
}
