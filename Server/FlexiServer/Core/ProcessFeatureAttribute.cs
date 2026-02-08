namespace FlexiServer.Core
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public sealed class ProcessFeatureAttribute : Attribute
    {
        public string Module { get; }
        public ProcessFeatureAttribute(string module)
        {
            Module = module;
        }
    }

}
