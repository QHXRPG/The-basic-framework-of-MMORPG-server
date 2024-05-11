
@echo off

set foo="%cd%\out\csharp"

if not exist %foo% (
    md %foo%
)

::遍历文件
for %%i in (proto/*.proto) do ( 
    %cd%/bin/protoc -I=proto --csharp_out=out/csharp %%i
    echo %%i Done
)

copy /y out\csharp\Message.cs E:\build_game\MMO-server\mmoServer\Common\Proto\Message.cs
copy /y out\csharp\Message.cs "E:\unityProject\My project\Assets\Plugins\Summer\Proto"


echo "ok"
pause