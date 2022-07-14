# PathWatcher

PathWatcher monitors a given directory for either new files or new directories. If a new object is added, then a certain application is called. If the execution of the application was successful, then the object can be moved to a certain folder. If it was not successful, then it can be moved into another folder.

The application can either be used from the command line without admin rights, or it can be installed as a Windows service with admin rights.

## Arguments

| Argument                           | Description                                                                                                                                                                                                                                                                                               |
|------------------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| -p, --path                         | Required. Path of the directory which should be watched.                                                                                                                                                                                                                                                  |
| -f, --filter                       | Filter for file names or directory names to be watched only (e.g. "*.xml").                                                                                                                                                                                                                               |
| -d, --directories                  | If set, then the application is watching for directories instead of files.                                                                                                                                                                                                                                |
| -c, --command                      | Required. Command to be executed with an object found by the watcher.                                                                                                                                                                                                                                     |
| -- at line end                     | Arguments to be used with the execution command. All arguments must be put at the end of the line, starting with a double dash element (e.g. "-c example.exe --arg1, arg2"). The following wildcards can be used to be replaced with the found object: $p... full path, $d... directory, $f... file name. |
| -u, --success                      | Path to a directory where the found objects are moved in case of successful execution.                                                                                                                                                                                                                    |
| -i, --fail                         | Path to a directory where the found objects are moved in case of a failed execution.                                                                                                                                                                                                                      |
| -o, --timeout                      | Maximum timeout to wait for the execution command in seconds. The default value is 60 seconds.                                                                                                                                                                                                            |
| -t, --checkatstart                 | If set, then the watch path is checked at the startup of the application for existing files already.                                                                                                                                                                                                      |
| -l, --loglevel                     | The logging level on the console. Can be set as either Error, Warning, Information, or Debug. The default value is Information.                                                                                                                                                                           |
| -s, --service                      | If set, there is no command-line output for running the application as windows service.                                                                                                                                                                                                                   |

## Windows service

The application can also be used as a Windows service.Admin rights are needed to execute the following commands

The installation of service can be done with the following command:

`SC CREATE "[Name of the service]" binpath="\"[Path to PathWatcher.exe]\" -s -p \"[Path to monitored directory]\" -u \"[Directory for success]\" -i \"[Directory for failures]\" -c \"[Path to the executing application]\" -- [Arguments for executing application]"`

The service can be started with the following command:

`SC START "[Name of the service]"`

The service can be stopped with the following command:

`SC STOP "[Name of the service]"`

The service can be uninstalled with the following command:

`SC DELETE "[Name of the service]"`