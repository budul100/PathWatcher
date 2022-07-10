@ECHO OFF
PUSHD ..\..\

SC CREATE "Files Watcher Desktop" binpath="%cd%\_Release\FilesWatcher.exe -s -p %USERPROFILE%\Desktop -t -u %USERPROFILE%\Desktop\Success -i %USERPROFILE%\Desktop\Fail -c \"%cd%\Additionals\Test\TestApp.exe\" -- $f"

POPD
PAUSE