using CommandLine;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Commons.Models
{
    public class Options
    {
        #region Public Fields

        public const string WildcardDirectory = "$d";
        public const string WildcardFile = "$f";
        public const string WildcardPath = "$p";

        #endregion Public Fields

        #region Public Properties

        [Option(
            shortName: 't',
            longName: "checkatstart",
            Required = false,
            HelpText = "Check the watch path at startup already for existing files.")]
        public bool CheckAtStart { get; set; }

        [Option(
            shortName: 'i',
            longName: "fail",
            Required = false,
            HelpText = "Path to a directory where the found objects are moved in case of a failed execution.")]
        public string DirectoryFail { get; set; }

        [Option(
            shortName: 'u',
            longName: "success",
            Required = false,
            HelpText = "Path to a directory where the found objects are moved in case of a successfull execution.")]
        public string DirectorySuccess { get; set; }

        [Value(index: 0,
            Min = 1,
            Required = false,
            HelpText = "Arguments to be used with the execution command. All arguments " +
                "must be put at the end of the line, starting with a double dash element " +
                "(e.g. \"-c example.exe -- arg1, arg2\"). The following wildcards can be used " +
                "be replaced with the found object: " + WildcardPath + "... full path, " +
                WildcardDirectory + "... directory, " + WildcardFile + "... file name.\r\n\r\n" +
                "This option cannot be used with the arguments (-a) option.")]
        public IEnumerable<string> ExecuteArgsCollection { get; set; }

        [Option(
            shortName: 'a',
            longName: "arguments",
            Required = false,
            HelpText = "Arguments to be used with the execution command. The following wildcards " +
                "can be used be replaced with the found object: " + WildcardPath + "... full path, " +
                WildcardDirectory + "... directory, " + WildcardFile + "... file name.\r\n\r\n" +
                "This option cannot be used with the arguments after -- option.")]
        public string ExecuteArgsText { get; set; }

        [Option(
            shortName: 'c',
            longName: "command",
            Required = true,
            HelpText = "Command to be executed with an object found by the watcher.")]
        public string ExecuteCommand { get; set; }

        [Option(
            shortName: 'l',
            longName: "loglevel",
            Required = false,
            HelpText = "The logging level on the console. Can be set as either Error, Warning, " +
                "Information, or Debug. Default value is Information.")]
        public LogLevel LogLevel { get; set; } = LogLevel.Information;

        [Option(
            shortName: 's',
            longName: "service",
            Required = false,
            HelpText = "Run the application as windows service.")]
        public bool RunAsService { get; set; }

        [Option(
            shortName: 'o',
            longName: "timeout",
            Required = false,
            HelpText = "Maximum timeout to wait for the execution command in seconds. The default value is 60 seconds.")]
        public int Timeout { get; set; } = 60;

        [Option(
            shortName: 'd',
            longName: "directories",
            Required = false,
            HelpText = "If set, then the application is watching for directories instead of files.")]
        public bool WatchDirectories { get; set; }

        [Option(
            shortName: 'f',
            longName: "filter",
            Required = false,
            HelpText = "Filter for file names or directory names to be watched only.")]
        public string WatchFilter { get; set; }

        [Option(
            shortName: 'p',
            longName: "path",
            Required = true,
            HelpText = "Path of the directory which should be watched.")]
        public string WatchPath { get; set; }

        #endregion Public Properties
    }
}