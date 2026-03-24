using ConfigData;
using FlexiServer.Core;
using FlexiServer.Models;
using FlexiServer.Models.Common;
using FlexiServer.Sandbox;
using FlexiServer.Transport.Http;

namespace FlexiServer.Services
{
    [ProcessFeature("Bag")]
    public class BagService(SandboxManager sandboxManager)
    {
        #region AutoContext
        public async Task<BagAcquireItemResponse> BagAcquireItem(HttpMessage<BagAcquireItemRequest> msg)
        {
            BagAcquireItemRequest? req = msg.Data;
            if (req == null) throw new ServerException(ErrorCode.None, "BagAcquireItemRequest is Null");

            GamePlayItemSandbox? sandbox = sandboxManager.GetSandbox<GamePlayItemSandbox>(
                (_sandbox) => { return _sandbox.ContainsPlayer(msg.Account); })
                ?? throw new ServerException(ErrorCode.None, "The match does not exist.");

            sandbox.GrantItemToPlayer(msg.Account, out int _itemId, out int _bagSoltIndex);
            BagAcquireItemResponse res = new();
            res.Item = new ItemInfo();
            res.Item.Id = _itemId;
            res.BagSlotIndex = _bagSoltIndex;
            return res;
        }
        #endregion HttpFuncStr
    }
}