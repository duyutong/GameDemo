namespace FlexiServer.Services.Interface
{
    public interface  IService
    {
        public abstract string Pattern { get; }
        public abstract void OnDataRecieved(string ClientId,string Acount,string Msg);
    }

}
