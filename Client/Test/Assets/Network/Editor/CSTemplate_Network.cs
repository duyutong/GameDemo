using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSTemplate_Network
{
    public const string NamespaceStr_ModelsCommon = @".Models.Common";
    public const string NamespaceStr_Models = @".Models";
    public const string ModelStr =
@"using FlexiServer.Models;
using FlexiServer.Models.Common;
using static EnumDefinitions;
namespace FlexiServer#NamespaceStr#
{
    [global::ProtoBuf.ProtoContract(Name = @""#ModelName#"")]
    public class #ModelName#
    {
        #region AutoContext
        #Variable#
        #endregion Variable
    }
}
";
    public const string ClientModelStr =
@"using System;
using System.Collections.Generic;
using Network.Models.Common;
using static EnumDefinitions;
namespace Network#NamespaceStr#
{
    [global::ProtoBuf.ProtoContract(Name = @""#ModelName#"")]
    public class #ModelName#
    {
        #region AutoContext
        #Variable#
        #endregion Variable
    }
}
";
    public const string ModelVariableListStr = @"
        [global::ProtoBuf.ProtoMember(#Pos#)]
        public List<#TypeName#> #VariableName# { get; set; }
";

    public const string ModelVariableStr = @"
        [global::ProtoBuf.ProtoMember(#Pos#)]
        public #TypeName# #VariableName# { get; set; }
";

    public const string HttpEndpointsStr =
@"using FlexiServer.Core;
using FlexiServer.Services;
namespace FlexiServer.Transport.Http
{
    public static partial class MapPostEndpoints
    {
        [ProcessFeature(""#ProtocolName_UC#"")]
        public static void Map#ProtocolName_UC#Endpoints(this WebApplication app)
        {
            #region AutoContext
            #MapPostStr#
            #endregion MapPostStr
        }
    }
}";
    public const string MapPostStr = @"
            app.MapPost(""#Pattern#"", async (HttpContext context) =>
            {
                HttpMessage msg = await ReadHttpMessageAsync(context);
                var result = new HttpResult();
                try
                {
                    #ProtocolName_UC#Service service = app.Services.GetRequiredService<#ProtocolName_UC#Service>();
                    var res = await service.#Func#(msg);
                    result.Code = 200;
                    result.Message = ""succ"";
                    result.Data = res.ToBytes();
                }
                catch (ServerException ex)
                {
                    result.Code = ex.Code;                 // 可以自定义不同错误码
                    result.Message = ex.Message;
                }
                ReturnHttpResultTask(context, result);
            });
            ";

    public const string HttpServiceStr =
@"using FlexiServer.Core;
using FlexiServer.Models;
using FlexiServer.Models.Common;
using FlexiServer.Transport.Http;
namespace FlexiServer.Services
{
    [ProcessFeature(""#ProtocolName_UC#"")]
    public class #ProtocolName_UC#Service
    {
        #region AutoContext
        #endregion HttpFuncStr
    }
}";
    public const string HttpFuncStr = @"
        public async Task<#Func#Response> #Func#(HttpMessage msg)
        {
            if (msg == null || msg.Data == null) throw new ServerException(ErrorCode.None, ""Data is Null"");

            #Func#Request? req = msg.Data.ConvertData<#Func#Request>();
            if (req == null) throw new ServerException(ErrorCode.None, ""#Func#Request is Null"");
            
            #Func#Response res = new();
            return res;
        }";
    public const string HttpApiStr =
@"using System;
using UnityEngine;
using Network.Models;
using Assets.Network.Transport;
namespace Network.API
{
    public class #ProtocolName_UC#Api : HttpMessageApi
    {
        #region AutoContext
        #endregion HttpFuncStr
    }
}";
    public const string HttpApiFuncStr =
@"
        public async void #Func#(#Func#Request req, Action<bool,#Func#Response> action)
        {
            await PostAsync<#Func#Request, #Func#Response>(""#Pattern#"", req, (result) =>
            {
                bool success = result.Code == 200 && result.Data != null;

                if (success) action?.Invoke(success, result.Data.ConvertData<#Func#Response>());
                else 
                {
                    Debug.LogError($""#ProtocolName_UC#Api #Func# failed: Code={result.Code}, Message={result.Message}"");
                    action?.Invoke(success, null); 
                }
            });
        }";
    public const string UdpMessageApiStr =
@"using UnityEngine;

namespace Network.API
{
    public class #ProtocolName_UC#Api : UdpMessageApi
    {
        public override string Pattern { get; set; } = ""#Pattern#"";
        public void SendUdpMessage<TSend>(string path, TSend messageObj)
        {
            SendUdpMessage(Pattern, path, messageObj);
        }
        public override void OnDataRecieved(string pattern, UdpResult result)
        {
            // Debug.Log($""[#ProtocolName_UC#Api] OnDataRecieved | ServerFrame: {result.ServerFrame}"");
            base.OnDataRecieved(pattern, result);
        }
    }
}
";
    public const string WebSocketMessageApiStr =
@"using Network;
using Network.Transport.WebSocket;
using System;
using UnityEngine;
namespace Network.API
{
    public class #ProtocolName_UC#Api : WebSocketMessageApi
    {
        public override string Pattern { get; set; } = ""#Pattern#"";

        public void SendWebSocketMessage<TSend>(string path, TSend messageObj)
        {
            base.SendWebSocketMessage(Pattern, path, messageObj);
        }
        public override void OnDataRecieved(string pattern, WebSocketResult result)
        {
            Debug.Log($""[#ProtocolName_UC#Api] OnDataRecieved"");
            base.OnDataRecieved(pattern, result);
        }
    }
}";
    public const string SocketHandlerStr =
@"using FlexiServer.Core;
using FlexiServer.Models.Common;
using FlexiServer.Services.Interface;
using FlexiServer.Transport;
using FlexiServer.Transport.Web;
namespace FlexiServer.Services
{
    [ProcessFeature(""#ProtocolName_UC#"")]
    public class #ProtocolName_UC#Service : IService
    {
        public string Pattern => ""#Pattern#"";
        public void OnDataRecieved(string ClientId, string Account, byte[] Buffer)
        {
            var recievMsg = Buffer.ConvertData<WebSocketMessage>();
            if (recievMsg == null) return;

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(""[#ProtocolName_UC#Service]"");
            Console.ResetColor();

            Console.WriteLine(
                $"" OnDataRecieved | Pattern: {recievMsg.Pattern} | Path: {recievMsg.Path}""
            );
            
            switch (recievMsg.Path)
            {
                #region AutoContext
                #endregion Switch_Handle
                default:
                    break;
            }
        }
        #region AutoContext
        #endregion Function_Handle
    }
}";
    public const string SwitchHandleStr =
@"              
                case NetworkEventPaths.#Pattern_UC#_#Func#:
                    #Func#Handle(ClientId, Account, recievMsg.Path, Buffer);
                    break;
";
    public const string FunctionHandleStr =
        @"
        private void #Func#Handle(string clientId, string account, string path, byte[] buffer)
        {
            
        }";
    public const string SocketApiMappingStr = @"{ ""#Pattern#"", typeof(#ProtocolName_UC#Api) },";
    public const string NetworkEventPathStr = @"public const string #Pattern_UC#_#Func# = ""#Path#"";";
};

