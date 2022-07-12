using Commons.Extensions;
using Commons.Interfaces;
using Commons.Models;
using Microsoft.Extensions.Logging;
using PathWatcher.Extensions;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ExecutorService
{
    public class Executor
        : IExecutor, IDisposable
    {
        #region Protected Fields

        protected readonly TimeSpan delay;

        #endregion Protected Fields

        #region Private Fields

        private const int DelayInMilliSeconds = 10;
        private const int ExitCodeSuccess = 0;

        private readonly string arguments;
        private readonly string command;
        private readonly ConcurrentQueue<string> executionQueue = new();
        private readonly ILogger<Executor> logger;
        private readonly Options options;
        private readonly TimeSpan timeout;

        private bool isDisposed;

        #endregion Private Fields

        #region Public Constructors

        public Executor(Options options, ILogger<Executor> logger)
        {
            this.options = options;
            this.logger = logger;

            command = options.GetCommand();
            arguments = options.GetArguments();

            timeout = TimeSpan.FromSeconds(options.Timeout);
            delay = TimeSpan.FromMilliseconds(DelayInMilliSeconds);
        }

        #endregion Public Constructors

        #region Public Methods

        public void Add(string path)
        {
            if (path.Exists(options.WatchDirectories)
                && path != options.DirectorySuccess
                && path != options.DirectoryFail)
            {
                executionQueue.Enqueue(path);

                logger.LogInformation(
                    "The following object was added: {path}",
                    path);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task ExecuteAsync(CancellationToken serviceCancellationToken)
        {
            serviceCancellationToken.Register(() => CloseService());

            while (!serviceCancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (!executionQueue.IsEmpty)
                    {
                        executionQueue.TryPeek(out string path);

                        if (path.Exists(options.WatchDirectories))
                        {
                            logger.LogDebug(
                                "The following object is handled now: {path}",
                                path);

                            var process = GetProcess(path);

                            using var processCancellationSource = new CancellationTokenSource(timeout);

                            serviceCancellationToken.Register(() => OnCancellationService(
                                process: process,
                                processCancellationSource: processCancellationSource));

                            try
                            {
                                var exitCode = await RunProcessAsync(
                                    process: process,
                                    processCancellationToken: processCancellationSource.Token);

                                if (exitCode == ExitCodeSuccess)
                                {
                                    PostProcessingSuccess(path);
                                }
                                else
                                {
                                    PostProcessingFail(
                                        sourcePath: path,
                                        exitCode: exitCode);
                                }
                            }
                            catch (TaskCanceledException)
                            {
                                if (!serviceCancellationToken.IsCancellationRequested)
                                {
                                    logger.LogError(
                                        "The process was ended with a timeout of {timeout} seconds: \"{file}\" {arguments}",
                                        options.Timeout,
                                        process.StartInfo.FileName,
                                        process.StartInfo.Arguments);
                                }
                            }
                        }

                        executionQueue.TryDequeue(out _);
                    }
                    else
                    {
                        // Allow other tasks to run

                        await Task.Delay(
                            delay: delay,
                            cancellationToken: serviceCancellationToken);
                    }
                }
                catch (TaskCanceledException)
                { }
                catch (Exception exception)
                {
                    logger.LogError(
                        exception: exception,
                        message: "There was an error while execution.");
                }
            }
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    CloseService();
                }

                isDisposed = true;
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private static void OnCancellationService(Process process, CancellationTokenSource processCancellationSource)
        {
            processCancellationSource.Cancel();
            process?.Dispose();
        }

        private void CloseService()
        {
            executionQueue.Clear();
        }

        private Process GetProcess(string path)
        {
            var currentArguments = arguments.Replace(
                oldValue: Options.WildcardPath,
                newValue: path);

            var directory = Path.GetDirectoryName(path);

            currentArguments = currentArguments.Replace(
                oldValue: Options.WildcardDirectory,
                newValue: directory);

            var file = Path.GetFileName(path);

            currentArguments = currentArguments.Replace(
                oldValue: Options.WildcardFile,
                newValue: file);

            logger.LogDebug(
                "The following command is called: {command} {arguments}",
                command,
                currentArguments);

            var startInfo = new ProcessStartInfo
            {
                Arguments = currentArguments,
                CreateNoWindow = true,
                FileName = command,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Minimized,
            };

            var result = new Process
            {
                StartInfo = startInfo
            };

            return result;
        }

        private void PostProcessingFail(string sourcePath, int exitCode)
        {
            logger.LogError(
                "The following object was handled with failure code {exitCode}: {path}",
                exitCode,
                sourcePath);

            if (!string.IsNullOrWhiteSpace(options.DirectoryFail))
            {
                var directoryFail = options.DirectoryFail
                    .CreateDirectory();

                var targetPath = options.WatchDirectories
                    ? sourcePath.MoveDirectory(directoryFail)
                    : sourcePath.MoveFile(directoryFail);

                logger.LogDebug(
                    "The object {sourcePath} was moved to {targetPath}.",
                    sourcePath,
                    targetPath);
            }
        }

        private void PostProcessingSuccess(string sourcePath)
        {
            logger.LogInformation(
                "The following object was handled successfully: {path}",
                sourcePath);

            if (!string.IsNullOrWhiteSpace(options.DirectorySuccess))
            {
                var directorySuccess = options.DirectorySuccess
                    .CreateDirectory();

                var targetPath = options.WatchDirectories
                    ? sourcePath.MoveDirectory(directorySuccess)
                    : sourcePath.MoveFile(directorySuccess);

                logger.LogDebug(
                    "The object {sourcePath} was moved to {targetPath}.",
                    sourcePath,
                    targetPath);
            }
        }

        private Task<int> RunProcessAsync(Process process, CancellationToken processCancellationToken)
        {
            var tcs = new TaskCompletionSource<int>();
            processCancellationToken.Register(() => tcs.TrySetCanceled());

            process.Exited += (sender, args) =>
            {
                tcs.SetResult(process.ExitCode);
                process?.Dispose();
            };

            process.EnableRaisingEvents = true;

            try
            {
                process.Start();
            }
            catch (Exception exception)
            {
                logger.LogError(
                    exception: exception,
                    message: "There was an error while execution.");

                tcs.SetResult(exception.HResult);
            }

            return tcs.Task;
        }

        #endregion Private Methods
    }
}