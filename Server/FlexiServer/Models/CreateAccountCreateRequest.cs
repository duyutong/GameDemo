using FlexiServer.Models;
using FlexiServer.Models.Common;
using System.Numerics;
using static EnumDefinitions;
namespace FlexiServer.Models
{
    public class CreateAccountCreateRequest
    {
        #region AutoContext
        
        public string Account { get; set; }
        public string Password { get; set; }
        #endregion Variable
    }
}
