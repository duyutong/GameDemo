using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class EnumDefinitions
{
    public enum EOperationState
    {
        None,
        Begin,      // 开始
        InProgress, // 持续
        Finish      // 结束
    }
    public enum EChatMsgType 
    {
        Private,
        Public,
        World,
    }
    public enum EItemType
    {
        None = 0,
        Food = 1,
        Clue = 2,
        Survival = 3
    }
    public static List<string> GetEnumNames() 
    {
        // 获取 EnumDefinitions 类的 Type
        Type enumDefType = typeof(EnumDefinitions);

        // 获取该类中所有嵌套类型，并筛选出枚举
        var enumTypes = enumDefType.GetNestedTypes(BindingFlags.Public | BindingFlags.NonPublic)
                                    .Where(t => t.IsEnum);
        List<string> result = new List<string>();
        foreach (var enumType in enumTypes) 
        {
            result.Add(enumType.Name);
        }
        return result;
    }
}


