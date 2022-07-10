using Commons.Extensions;
using Commons.Interfaces;
using Commons.Models;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading;

namespace WatcherService
{
    public class Watcher
        : IWatcher
    {
        #region Private Fields

        private const int WaitTime = 500;
        private const string WatchFilterDefault = "*";
        private const string WatchTypeDirectories = "directories";
        private const string WatchTypeFiles = "files";

        private readonly IExecutor executor;
        private readonly object locker = new();
        private readonly ILogger<Watcher> logger;
        private readonly Options options;

        private FileSystemWatcher watcher;

        #endregion Private Fields

        #region Public Constructors

        public Watcher(IExecutor executor, Options options, ILogger<Watcher> logger)
        {
            this.executor = executor;
            this.options = options;
            this.logger = logger;
        }

        #endregion Public Constructors

        #region Public Methods

        public void Execute(CancellationToken cancellationToken)
        {
            watcher = GetWatcher();
            watcher.Created += OnCreated;

            cancellationToken.Register(() => watcher.EnableRaisingEvents = false);

            watcher.EnableRaisingEvents = true;

            CheckAtStartup();
        }

        #endregion Public Methods

        #region Private Methods

        private void AddPath(string path)
        {
            logger.LogDebug(
                "The following object was created: {path}",
                path);

            if (!path.IsHidden(options.WatchDirectories))
            {
                var isDirectory = path.IsDirectory();

                if (isDirectory ^ !options.WatchDirectories)
                {
                    executor.Add(path);
                }
            }
        }

        private void CheckAtStartup()
        {
            if (options.CheckAtStart)
            {
                var searchPattern = string.IsNullOrWhiteSpace(options.WatchFilter)
                    ? string.Empty
                    : options.WatchFilter;

                if (options.WatchDirectories)
                {
                    var paths = Directory.GetDirectories(
                        path: options.WatchPath,
                        searchPattern: searchPattern,
                        searchOption: SearchOption.TopDirectoryOnly);

                    foreach (var path in paths)
                    {
                        AddPath(path);
                    }
                }
                else
                {
                    var paths = Directory.GetFiles(
                        path: options.WatchPath,
                        searchPattern: searchPattern,
                        searchOption: SearchOption.TopDirectoryOnly);

                    foreach (var path in paths)
                    {
                        AddPath(path);
                    }
                }
            }
        }

        private FileSystemWatcher GetWatcher()
        {
            var watchPath = options.WatchPath.GetDirectory();

            if (!Directory.Exists(watchPath))
            {
                throw new DirectoryNotFoundException(
                    $"There is no directory '{watchPath}'. Please check the path argument.");
            }

            var result = new FileSystemWatcher
            {
                Path = watchPath,
                Filter = options.WatchFilter,
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName
            };

            var watchType = options.WatchDirectories
                ? WatchTypeDirectories
                : WatchTypeFiles;

            if (result.Filter == WatchFilterDefault)
            {
                logger.LogInformation(
                    "The system is watching {watchPath} for {watchType}.",
                    result.Path,
                    watchType);
            }
            else
            {
                logger.LogInformation(
                    "The system is watching {watchPath} for {watchType} with filter {watchFilter}.",
                    result.Path,
                    watchType,
                    result.Filter);
            }

            return result;
        }

        private void OnCreated(object source, FileSystemEventArgs e)
        {
            // Wait for things to calm down.
            Thread.Sleep(WaitTime);

            lock (locker)
            {
                AddPath(e.FullPath);
            }
        }

        #endregion Private Methods
    }
}