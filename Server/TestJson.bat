@echo off
REM 设置环境变量
set dataType=Json
set toolPath=F:\GameDemo\Server\ExcelToCSharp_Tool
set libraryPath=F:\GameDemo\Server\PathLibrary_Config.json

REM 启动工具
"%toolPath%\ExcelToCSharp.exe"

REM 等待用户按任意键关闭（可选）
pause
