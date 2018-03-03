@SET COMPILER_HOME=%CD%
@PATH %COMPILER_HOME%\bin\Release;%PATH%
ECHO "请使用run.bat <.csp文件>命令编译并执行.csp文件"
complier.exe %*
dotnet run --project %*
PAUSE