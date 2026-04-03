using FlexiServer.Core;
using FlexiServer.Models;
using FlexiServer.Models.Common;
using FlexiServer.Sandbox;
using FlexiServer.Transport;
using FlexiServer.Transport.Http;
namespace FlexiServer.Services
{
    [ProcessFeature("Map")]
    public class MapService(SandboxManager sandboxManager)
    {
        #region AutoContext
        
        public async Task<MapGenerateResponse> MapGenerate(HttpMessage msg)
        {
            if (msg == null || msg.Data == null) throw new ServerException(ErrorCode.None, "Data is Null");

            MapGenerateRequest? req = msg.Data.ConvertData<MapGenerateRequest>();
            if (req == null) throw new ServerException(ErrorCode.None, "MapGenerateRequest is Null");

            var sandbox = sandboxManager.GetSandbox<GamePlayMapSandbox>(
                (_sandbox) => { return _sandbox.ContainsPlayer(msg.Account); })
                ?? throw new ServerException(ErrorCode.None, "The match does not exist.");

            sandbox.GenerateMap(req.MapLayer, out List<MapGeneratorInfo> result);
            MapGenerateResponse res = new();
            res.MapGenInfos = result;
            return res;
        }
        #endregion HttpFuncStr
    }
}