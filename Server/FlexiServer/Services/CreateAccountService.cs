using FlexiServer.Core;
using FlexiServer.Core.Frame;
using FlexiServer.Infrastructure.Database;
using FlexiServer.Models;
using FlexiServer.Models.Common;
using FlexiServer.Transport;
using FlexiServer.Transport.Http;
using static FlexiServer.Services.LoginService;
namespace FlexiServer.Services
{
    [ProcessFeature("CreateAccount")]
    public class CreateAccountService(TokenManager tokenService, Database db,IConfiguration config)
    {
        #region AutoContext
        public async Task<CreateAccountCreateResponse> CreateAccountCreate(HttpMessage<CreateAccountCreateRequest> msg)
        {
            CreateAccountCreateRequest? req = msg.Data;
            if (req == null) throw new ServerException(ErrorCode.None, "CreateAccountCreateRequest is Null");

            var processes = config
                   .GetSection("Processes")
                   .Get<Dictionary<string, ProcessInfo>>();

            var token = await tokenService.GenerateToken(req.Account,TokenManager.DefaultValidTime);
            CreateAccountCreateResponse res = new CreateAccountCreateResponse();
            res.Token = token;
            res.Account = req.Account;
            res.ProcessInfos = processes.Values.ToList();

            byte[] hash;
            byte[] salt;
            PasswordHasher.CreateHash(req.Password, out hash, out salt);

            try 
            {
                db.Accounts.Insert(new AccountInfo
                {
                    Account = req.Account,
                    PasswordHash = hash,
                    PasswordSalt = salt,
                });
            }
            catch (LiteDB.LiteException ex) 
            {
                throw new ServerException(ex.ErrorCode, "Account already exists");
            }

            return res;
        }
        #endregion HttpFuncStr
    }
}