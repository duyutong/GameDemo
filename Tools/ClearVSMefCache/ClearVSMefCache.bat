@echo off
echo Closing Visual Studio...
taskkill /IM devenv.exe /F >nul 2>&1

echo Deleting MEF Component Cache...
rmdir /S /Q "%LocalAppData%\Microsoft\VisualStudio\17.0_50f3da84\ComponentModelCache"

echo Done. Please restart Visual Studio.
pause
