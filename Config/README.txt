PathLibrary_Config.json

PathLibrary_Config.json 文件用于配置项目中的文件路径。通过修改该文件，可以在不更改代码的情况下，灵活调整系统读取和写入各类数据的位置。

示例 PathLibrary_Config.json：

{
"excelPath": "ExcelData",
"jsonPath": "Assets/AddressableAssets/Config/Json",
"binaryPath": "Assets/AddressableAssets/Config/Binary",
"csharpPath": "Assets/ConfigData/CSharpClass",
"loaderPath": "Assets/ConfigData/ConfigLoader.cs"
}

使用说明：

所有路径均为相对于项目根目录的相对路径。

修改 PathLibrary.json 中的任何路径，都会改变相关文件的读取或写入位置。

在运行配置生成流程之前，确保指定的目录已存在，或提前创建好。

如果路径指向 Assets 目录内部，在 Unity 中使用时，确保其符合 Unity 的资源系统规范。

===== 配置表格式要求 =====

本项目中的 Excel 配置表必须遵循以下规则，以确保 JSON、Binary 以及 C# 类能够正确生成。

===== 1. 表头行（前 3 行） =====

第 1 行：变量名
例如：ID、Icon 等。
变量名必须符合 C# 命名规范（字母、数字、下划线，区分大小写）。

第 2 行：变量类型
必须符合 C# 语法规范。
例如：int、string、List<int>、List<List<T>>、Dictionary<T,T>。

第 3 行：变量说明
该行内容将自动转换为生成的 C# 代码中的注释。
支持中文和英文描述。

===== 2. 数据行（从第 4 行开始） =====

每一行对应一条数据记录。
每一列对应表头中定义的变量。
每一列的数据类型必须与第 2 行中声明的类型一致。
类型不匹配可能导致代码生成失败。

===== 3. 字符串与集合分隔规则 =====

List<T>：使用英文逗号（,）分隔元素。
示例：1,2,3,4,5

List<List<T>>：使用分号（;）分隔外层列表，使用逗号（,）分隔内层元素。
示例：1,2;3,4;5,6

Dictionary<T,T>：使用等号（=）分隔键和值，使用逗号（,）分隔不同键值对。
示例：1=10001,2=10002

===== 4. 注意事项 =====

所有变量名、变量类型和数据内容必须严格遵循 C# 语法。

数据行必须从第 4 行开始，否则生成工具将无法正确识别条目