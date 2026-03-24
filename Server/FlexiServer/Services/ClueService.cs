using FlexiServer.Core;
using FlexiServer.Models;
using FlexiServer.Models.Common;
using FlexiServer.Transport;
using FlexiServer.Transport.Http;
namespace FlexiServer.Services
{
    [ProcessFeature("Clue")]
    public class ClueService
    {
        #region AutoContext
        
        public async Task<ClueIdentifyResponse> ClueIdentify(HttpMessage msg)
        {
            if (msg == null || msg.Data == null) throw new ServerException(ErrorCode.None, "Data is Null");

            var req = msg.Data.ConvertData<ClueIdentifyResponse>();
            if (req == null) throw new ServerException(ErrorCode.None, "ClueIdentifyRequest is Null");
            
            ClueIdentifyResponse res = new ClueIdentifyResponse();
            return res;
        }
        #endregion HttpFuncStr
    }
}