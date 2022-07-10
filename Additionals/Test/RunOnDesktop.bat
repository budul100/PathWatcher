@ECHO OFF
PUSHD ..\..\

%cd%\_Release\FilesWatcher.exe -p %USERPROFILE%\Desktop -t -u %USERPROFILE%\Desktop\Success -i %USERPROFILE%\Desktop\Fail -c "C:\Program Files\ConEmu\ConEmu64.exe" -- -run echo $d\$f

POPD
PAUSE