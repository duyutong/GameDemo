namespace FlexiServer.Services
{
    public enum ErrorCode
    {
        None = 0,
        InvalidAccount = 1001,
        InvalidPassword = 1002,
        AccountNotExists = 1003,
        PermissionDenied = 2001,
    }
    public class ServerException : Exception
    {
        public int Code { get; }

        public ServerException(int code, string? message = null) : base(message) 
        {
            Code = code;
        }
        public ServerException(ErrorCode code, string? message = null): base(message)
        {
            Code = (int)code;
        }
    }

}
