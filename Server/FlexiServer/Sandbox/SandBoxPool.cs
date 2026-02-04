namespace FlexiServer.Sandbox
{
    public class SandBoxPool()
    {
        private readonly Dictionary<Type, Stack<SandboxBase>> pool = [];
        public void ClearAll()
        {
            foreach (KeyValuePair<Type, Stack<SandboxBase>> keyValuePair in pool)
            {
                Stack<SandboxBase> stack = keyValuePair.Value;
                while (stack.Count > 0)
                {
                    var item = stack.Pop();
                    item.Destroy();
                }
            }
            pool.Clear();
        }
        public SandboxBase? GetSandBox(Type? sandBoxType)
        {
            if (sandBoxType == null) return null;

            if (!pool.TryGetValue(sandBoxType, out var stack))
            {
                stack = new Stack<SandboxBase>();
                pool[sandBoxType] = stack;
            }
            if (stack.Count > 0) return stack.Pop();
            else
            {
                var obj = Activator.CreateInstance(sandBoxType);
                if (obj != null && (SandboxBase)obj != null) return (SandboxBase)obj;
            }
            return null;
        }
        public TSandBox? GetSandBox<TSandBox>(string? typeName = "") where TSandBox : SandboxBase
        {
            var sandBoxType = typeof(TSandBox);
            if (!pool.TryGetValue(sandBoxType, out var stack))
            {
                stack = new Stack<SandboxBase>();
                pool[sandBoxType] = stack;
            }
            if (stack.Count > 0) return (TSandBox)stack.Pop();
            else
            {
                var obj = Activator.CreateInstance(sandBoxType, typeName ?? typeof(TSandBox).Name);
                if (obj != null && (TSandBox)obj != null) return (TSandBox)obj;
            }
            return null;
        }
        public void Release(SandboxBase instance)
        {
            if (instance == null) return;

            var type = instance.GetType();

            if (!pool.TryGetValue(type, out var stack))
            {
                stack = new Stack<SandboxBase>();
                pool[type] = stack;
            }

            instance.Reset();
            stack.Push(instance);

        }
    }
}
