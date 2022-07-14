using Commons.Models;
using System.IO;
using System.Linq;
using System.Text;

namespace PathWatcher.Extensions
{
    public static class OptionsExtensions
    {
        #region Private Fields

        private const char Space = ' ';

        #endregion Private Fields

        #region Public Methods

        public static string GetArguments(this Options options)
        {
            var result = new StringBuilder();

            if (options.ExecuteArgs?.Any() ?? false)
            {
                foreach (var executeArg in options.ExecuteArgs)
                {
                    if (!string.IsNullOrWhiteSpace(executeArg))
                    {
                        if (result.Length > 0)
                        {
                            result.Append(Space);
                        }

                        result.Append(executeArg.Trim());
                    }
                }
            }

            return result.ToString();
        }

        public static string GetCommand(this Options options)
        {
            var result = Path.GetFullPath(options.ExecuteCommand);

            return "\"" + result + "\"";
        }

        #endregion Public Methods
    }
}