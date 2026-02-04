namespace FlexiServer.Core
{
    public enum EPlayerConnectionState
    {
        /// <summary>
        /// 未建立连接（从未连接 / 已彻底释放）
        /// </summary>
        Disconnected = 0,

        /// <summary>
        /// 已建立底层连接（TCP / WS 已连上）
        /// </summary>
        Connected = 1,

        /// <summary>
        /// 心跳超时，判定为掉线
        /// （连接可能尚未物理关闭）
        /// </summary>
        HeartbeatTimeout = 2,

        /// <summary>
        /// 主动断开（客户端正常退出）
        /// </summary>
        Closed = 3
    }

}
