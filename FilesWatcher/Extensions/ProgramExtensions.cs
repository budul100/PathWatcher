using Commons.Models;
using Microsoft.Extensions.Hosting;

namespace FilesWatcher.Extensions
{
    internal static class ProgramExtensions
    {
        #region Public Methods

        public static IHostBuilder GetHostBuilder(this IHostBuilder defaultBuilder, Options options)
        {
            return options.RunAsService
                ? defaultBuilder.UseWindowsService()
                : defaultBuilder.UseConsoleLifetime();
        }

        #endregion Public Methods
    }
}