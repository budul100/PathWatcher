@ECHO OFF
PUSHD ..\..\

SC CREATE "Path Watcher Desktop" binpath="%cd%\_Release\PathWatcher.exe -s -p %USERPROFILE%\Desktop -t -u %USERPROFILE%\Desktop\Success -i %USERPROFILE%\Desktop\Fail -c \"%cd%\Additionals\Test\TestApp.exe\" -- $f"
SC START "Path Watcher Desktop"

POPD
PAUSE