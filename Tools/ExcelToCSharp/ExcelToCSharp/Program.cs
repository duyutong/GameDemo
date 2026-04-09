
// See https://aka.ms/new-console-template for more information
using System.Text;

var argDict = args.Select(a => a.Split('=', 2)).Where(p => p.Length == 2).ToDictionary(p => p[0], p => p[1]);
string dataType = argDict.GetValueOrDefault("dataType", "Json");
string libraryPath = argDict.GetValueOrDefault("libraryPath", Path.Combine(Directory.GetCurrentDirectory(), "PathLibrary_Config.json"));
ExcelToCSharp.SetLibraryPath(libraryPath);

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine($"=====DataType=====");
Console.ForegroundColor = ConsoleColor.DarkCyan;
Console.WriteLine($"<<<<< {dataType} >>>>>");
Console.ResetColor();

Console.WriteLine($"=====ToolPath=====");
Console.ForegroundColor = ConsoleColor.DarkCyan;
Console.WriteLine($"<<<<< {Directory.GetCurrentDirectory()} >>>>>");
Console.ResetColor();

Console.WriteLine($"=====LibraryPath=====");
Console.ForegroundColor = ConsoleColor.DarkCyan;
Console.WriteLine($"<<<<< {libraryPath} >>>>>");
Console.ResetColor();

Console.WriteLine(Environment.NewLine);
Console.WriteLine($"=====输出结果=====");

if (dataType == "Json")
    ExcelToCSharp.RefreshAllByJson();
else
    ExcelToCSharp.RefreshAllByPB();