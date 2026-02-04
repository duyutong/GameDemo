using FlexiServer.Core;
using FlexiServer.Core.Frame;
using FlexiServer.Infrastructure.Database;
using FlexiServer.Models;
using FlexiServer.Models.Common;
using FlexiServer.Sandbox;
using FlexiServer.Transport;
using FlexiServer.Transport.Http;
using FlexiServer.Transport.Web;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace FlexiServer.Services
{
    [ProcessFeature("Login")]
    public class LoginService(TokenManager tokenService, SandboxManager sandboxManager, Database db, IConfiguration config)
    {
        #region AutoContext
        public async Task<LoginHttpLoginResponse> LoginHttpLogin(HttpMessage<LoginHttpLoginRequest> msg)
        {
            LoginHttpLoginRequest? req = msg.Data;
            return req == null ? throw new ServerException(ErrorCode.None, "LoginRequest is Null")
                : await RequestHandler(req);
        }
        public async Task<LoginValidateResponse> LoginValidate(HttpMessage<LoginValidateRequest> msg)
        {
            LoginValidateRequest? req = msg.Data;
            if (req == null) throw new ServerException(ErrorCode.None, "LoginValidateRequest is Null");

            string account = req.Account ?? throw new ServerException(ErrorCode.None, "Account is Null");
            var info = db.Accounts.FindOne(x => x.Account == account);

            LoginValidateResponse res = new LoginValidateResponse();
            res.IsValidate = info != null;
            return res;
        }
        #endregion HttpFuncStr
        private async Task<LoginHttpLoginResponse> RequestHandler(LoginHttpLoginRequest req)
        {
            CheckAccount(req.Account, req.Password, out int code, out string message);
            if (code == 200)
            {
                var processes = config
                    .GetSection("Processes")
                    .Get<Dictionary<string, ProcessInfo>>();

                var token = await tokenService.GenerateToken(req.Account, TokenManager.DefaultValidTime);
                LoginHttpLoginResponse res = new LoginHttpLoginResponse();
                res.Code = code;
                res.Account = req.Account;
                res.Token = token;
                res.ProcessInfos = processes.Values.ToList();
                return res;
            }
            else 
            {
                LoginHttpLoginResponse res = new LoginHttpLoginResponse();
                res.Code = code;
                res.Account = req.Account;
                res.Token = string.Empty;
                res.ProcessInfos = new List<ProcessInfo>();
                return res;
            }
        }
        private void CheckAccount(string account, string password, out int code, out string message)
        {
            if (string.IsNullOrEmpty(account))
            {
                code = (int)ErrorCode.InvalidAccount;
                message = "Account cannot be empty";
                return;
            }

            if (string.IsNullOrEmpty(password))
            {
                code = (int)ErrorCode.InvalidPassword;
                message = "Password cannot be empty";
                return;
            }

            var info = db.Accounts.FindOne(x => x.Account == account);
            if (info == null)
            {
                code = (int)ErrorCode.AccountNotExists;
                message = "Account not exists";
                return;
            }

            bool ok = PasswordHasher.Verify(password, info.PasswordHash, info.PasswordSalt);
            if (!ok)
            {
                code = (int)ErrorCode.InvalidPassword;
                message = "Incorrect password";
                return;
            }

            code = 200;
            message = "Success";
        }
        internal class PasswordHasher
        {
            private const int SaltSize = 16;
            private const int HashSize = 32;
            private const int Iterations = 100_000;

            public static void CreateHash(
                string password,
                out byte[] hash,
                out byte[] salt)
            {
                using var pbkdf2 = new Rfc2898DeriveBytes(
                    password,
                    SaltSize,
                    Iterations,
                    HashAlgorithmName.SHA256);

                salt = pbkdf2.Salt;
                hash = pbkdf2.GetBytes(HashSize);
            }

            public static bool Verify(
                string password,
                byte[] storedHash,
                byte[] storedSalt)
            {
                using var pbkdf2 = new Rfc2898DeriveBytes(
                    password,
                    storedSalt,
                    Iterations,
                    HashAlgorithmName.SHA256);

                var hash = pbkdf2.GetBytes(HashSize);
                return CryptographicOperations.FixedTimeEquals(hash, storedHash);
            }
        }
    }
}
