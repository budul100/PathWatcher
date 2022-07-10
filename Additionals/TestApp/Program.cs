using System;
using System.IO;
using System.Linq;

namespace TestApp
{
    internal static class Program
    {
        #region Private Fields

        private const int ExitCodeError = 1;
        private const int ExitCodeSuccess = 0;

        private const string FileName = "TestApp.log";

        #endregion Private Fields

        #region Private Methods

        private static int Main(string[] args)
        {
            if (args?.Any(a => a.Contains("error")) ?? false)
            {
                return ExitCodeError;
            }
            else
            {
                var path = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    FileName);

                File.AppendAllLines(
                    path: path,
                    contents: args);

                return ExitCodeSuccess;
            }
        }

        #endregion Private Methods
    }
}