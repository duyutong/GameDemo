using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public interface IConfigDataHandler
{
    public void SetTablePath(string path);
}
public interface IConfigDataHandler<T> : IConfigDataHandler where T : BaseConfig
{
    public T GetConfigData(int id);
    public List<T> GetConfigData(Func<T, bool> select, int count = 1);
    public Dictionary<int, T> LoadConfigTable();
}

