using FlexiServer.Models;
using FlexiServer.Models.Common;
using System.Numerics;
using static EnumDefinitions;
namespace FlexiServer.Models
{
    public class LoginHttpLoginResponse
    {
        #region AutoContext
        
        public string Account { get; set; }
        public string Token { get; set; }
        public List<ProcessInfo> ProcessInfos { get; set; }
        public int Code { get; set; }
        #endregion Variable
    }
}
