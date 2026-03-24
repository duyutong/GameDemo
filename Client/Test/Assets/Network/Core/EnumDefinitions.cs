using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

public class EnumDefinitions
{
    // 所有枚举必须要使用 E{枚举名}State 或者 E{枚举名}Type的命名规则
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
        Survival = 3,
        Unidentified = 4,
    }
    public enum EClueType
    {
        Useless = 0,
        True = 1,
        False = 2
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


