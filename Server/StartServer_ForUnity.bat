@echo off
set EXE_NAME=FlexiServer.exe
set EXE_PATH=%~dp0FlexiServer\bin\Debug\net8.0\FlexiServer.exe

echo ==========================================
echo [Unity] Starting FlexiServer for Unity...
echo ==========================================

REM 检查进程是否已经存在
tasklist /FI "IMAGENAME eq %EXE_NAME%" | find /I "%EXE_NAME%" >nul
if NOT errorlevel 1 (
    echo [Unity] Server is already running. Skip start.
    exit /b 0
)

echo [Unity] Server is not running. Starting now...
cd /d "%~dp0FlexiServer\bin\Debug\net8.0"
start "FlexiServer Window" cmd /k "echo ========================================== & echo [Unity] Starting FlexiServer for Unity... & echo ========================================== & "%EXE_PATH%""