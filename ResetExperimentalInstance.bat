@echo off
echo Resetting Visual Studio 2022 Experimental Instance...
echo.
echo This will clear all extensions and settings from the Experimental Instance.
echo Press Ctrl+C to cancel, or
pause

"%ProgramFiles%\Microsoft Visual Studio\2022\Community\VSSDK\VisualStudioIntegration\Tools\Bin\CreateExpInstance.exe" /Reset /VSInstance=17.0 /RootSuffix=Exp

echo.
echo Experimental Instance has been reset.
echo You can now rebuild and debug your extension.
pause
