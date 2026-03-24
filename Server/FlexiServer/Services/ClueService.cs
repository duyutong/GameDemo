using FlexiServer.Core;
using FlexiServer.Models;
using FlexiServer.Models.Common;
using FlexiServer.Transport.Http;
namespace FlexiServer.Services
{
    [ProcessFeature("Clue")]
    public class ClueService
    {
        #region AutoContext
        
        public async Task<ClueIdentifyResponse> ClueIdentify(HttpMessage<ClueIdentifyRequest> msg)
        {
            ClueIdentifyRequest? req = msg.Data;
            if (req == null) throw new ServerException(ErrorCode.None, "ClueIdentifyRequest is Null");
            
            ClueIdentifyResponse res = new ClueIdentifyResponse();
            return res;
        }
        #endregion HttpFuncStr
    }
}