using ConfigData;
using FlexiServer.Core;
using FlexiServer.Models;
using FlexiServer.Models.Common;
using FlexiServer.Transport.Http;
namespace FlexiServer.Services
{
    [ProcessFeature("Bag")]
    public class BagService
    {
        HashSet<int> allIds = new HashSet<int>();
        #region AutoContext
        public async Task<BagAcquireItemResponse> BagAcquireItem(HttpMessage<BagAcquireItemRequest> msg)
        {
            BagAcquireItemRequest? req = msg.Data;
            if (req == null) throw new ServerException(ErrorCode.None, "BagAcquireItemRequest is Null");

            if (allIds.Count == 0)
            {
                allIds = ConfigLoader.GetConfigDatas<ItemConfig>(100)
                         .Select(item => item.ID)
                         .ToHashSet();
            }

            int randomId = allIds.Count > 0
               ? allIds.ElementAt(Random.Shared.Next(allIds.Count))
               : default; // 列表为空返回默认值 0

            BagAcquireItemResponse res = new BagAcquireItemResponse();
            res.Item = new ItemInfo();
            res.Item.Id = randomId;
           
            return res;
        }
        #endregion HttpFuncStr
    }
}