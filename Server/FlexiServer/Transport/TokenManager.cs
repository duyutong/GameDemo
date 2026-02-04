using FlexiServer.Core;
using FlexiServer.Infrastructure.Database;
using FlexiServer.Infrastructure.InternalServices;
using FlexiServer.Models;
using FlexiServer.Services;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
namespace FlexiServer.Transport
{
    public class TokenManager(InternalServiceClient internalService, Database db)
    {
        public static TimeSpan DefaultValidTime => TimeSpan.FromHours(2);
        private string SecretKey => "mR8!Kp3^sQZ@9E#Bv_2H4FJ+0A%wX*Y2";
        public async Task<string> GenerateToken(string account, TimeSpan validTime)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[] { new Claim("account", account) };
            var token = new JwtSecurityToken(
                issuer: "LoginServer",
                audience: "GameServer",
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.Add(validTime),
                signingCredentials: creds);

            #region 注释
            //issuer（签发者）= JWT 由谁签发
            //audience（受众 / 使用方）= JWT 给谁用
            //claims（声明 / 属性）= JWT 携带的身份信息
            //notBefore（生效时间）= token 在此时间前无效
            //expires（过期时间）= token 超过此时间后无效
            //signingCredentials（签名凭证）= 用哪把密钥和哪种算法签名
            #endregion

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<bool> Validate(string token)
        {
            var account = GetAccount(token);
            if (string.IsNullOrEmpty(account)) return false;

            bool isRegistered = ServerModuleRegister.CheckIsRegisteredService("Login");
            if (isRegistered)
            {
                var info = db.Accounts.FindOne(x => x.Account == account);
                bool isExists = info != null;
                return isExists;
            }
            else
            {
                string role = "Login";
                string path = "/login/validate";
                LoginValidateRequest req = new LoginValidateRequest();
                req.Account = account;
                var result = await internalService.PostAsync<LoginValidateRequest, LoginValidateResponse>(role, path, req);
                if (result != null && result.Code == 200 && result.Data != null) return result.Data.IsValidate;
            }
            return false;
        }
        public string? GetAccount(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) return null;

            var handler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(SecretKey);

            try
            {
                var principal = handler.ValidateToken(
                    token,
                    new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),

                        ValidateIssuer = true,
                        ValidIssuer = "LoginServer",

                        ValidateAudience = true,
                        ValidAudience = "GameServer",

                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromSeconds(30),

                        RequireSignedTokens = true
                    },
                    out _);

                return principal.Claims
                    .FirstOrDefault(c => c.Type == "account")
                    ?.Value;
            }
            catch { return null; }
        }

    }
}
