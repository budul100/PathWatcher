using CommandLine;
using CommandLine.Text;
using Commons.Interfaces;
using Commons.Models;
using PathWatcher.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PathWatcher
{
    internal static class Program
    {
        #region Private Fields

        private const int ExitCodeHelp = 1;
        private const int ExitCodeSuccess = 0;

        private const string TimeStampFormat = "[yyyy-MM-dd HH:mm:ss] ";

        #endregion Private Fields

        #region Public Methods

        public static int Main(string[] args)
        {
            var result = ExitCodeHelp;

            try
            {
                var hostBuilder = CreateHostBuilder(args);

                if (hostBuilder != default)
                {
                    hostBuilder.Build().Run();
                    result = ExitCodeSuccess;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);

                result = exception.HResult;
            }

            if (result != ExitCodeSuccess)
            {
                PauseConsole();
            }

            return result;
        }

        #endregion Public Methods

        #region Private Methods

        private static void ConfigureLogging(ILoggingBuilder logging, Options options)
        {
            logging.ClearProviders();

            if (!options.RunAsService)
            {
                logging
                    .SetMinimumLevel(options.LogLevel)
                    .AddSimpleConsole(configure =>
                    {
                        configure.SingleLine = true;
                        configure.IncludeScopes = false;
                        configure.TimestampFormat = TimeStampFormat;
                    });
            }

            // Windows event logs only written for warning or error messages!

            logging
                .AddEventLog(configure => configure.SourceName = nameof(PathWatcher));
        }

        private static void ConfigureServices(IServiceCollection services, Options options)
        {
            services.AddSingleton(options);

            services.AddSingleton<IExecutor, ExecutorService.Executor>();
            services.AddSingleton<IWatcher, WatcherService.Watcher>();

            services.AddHostedService<Worker>();
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            var result = default(IHostBuilder);

            var parsing = new Parser(settings => settings.EnableDashDash = true)
                .ParseArguments<Options>(args);

            parsing
                .WithParsed(options => result = GetHostBuilder(
                    args: args,
                    options: options))
                .WithNotParsed(errors => DisplayHelp(
                    parsing: parsing,
                    errors: errors));

            return result;
        }

        private static void DisplayHelp<T>(ParserResult<T> parsing, IEnumerable<Error> errors)
        {
            var helpText = default(HelpText);

            if (errors.IsVersion())
            {
                helpText = HelpText.AutoBuild(parsing);
            }
            else
            {
                helpText = HelpText.AutoBuild(
                    parserResult: parsing,
                    onError: helpText => DisplayOnError(
                        parsing: parsing,
                        helpText: helpText),
                    onExample: example => example);
            }
            Console.WriteLine(helpText);
        }

        private static HelpText DisplayOnError<T>(ParserResult<T> parsing, HelpText helpText)
        {
            helpText.AdditionalNewLineAfterOption = false;

            var result = HelpText.DefaultParsingErrorsHandler(
                parserResult: parsing,
                current: helpText);

            return result;
        }

        private static IHostBuilder GetHostBuilder(string[] args, Options options)
        {
            var result = Host
                .CreateDefaultBuilder(args)
                .GetHostBuilder(options)
                .ConfigureServices((_, services) => ConfigureServices(
                    services: services,
                    options: options))
                .ConfigureLogging((_, logging) => ConfigureLogging(
                    logging: logging,
                    options: options));

            return result;
        }

        [Conditional("DEBUG")]
        private static void PauseConsole()
        {
            Console.WriteLine("\r\nProcess is paused. Continue?");
            Console.ReadLine();
        }

        #endregion Private Methods
    }
}