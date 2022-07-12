using Commons.Models;
using System;
using System.Collections.Generic;
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
            if (!string.IsNullOrWhiteSpace(options.ExecuteArgsText)
                && (options.ExecuteArgsCollection?.Any() ?? false))
            {
                throw new ApplicationException(
                    "There cannot be Arguments given by the -a parameter and the -- element. Please use only one of these elements.");
            }

            var result = !string.IsNullOrWhiteSpace(options.ExecuteArgsText)
                ? options.ExecuteArgsText.Trim()
                : options.ExecuteArgsCollection.GetArguments();

            return result;
        }

        public static string GetCommand(this Options options)
        {
            var result = Path.GetFullPath(options.ExecuteCommand);

            return "\"" + result + "\"";
        }

        #endregion Public Methods

        #region Private Methods

        private static string GetArguments(this IEnumerable<string> values)
        {
            var result = new StringBuilder();

            if (values?.Any() ?? false)
            {
                foreach (var value in values)
                {
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        if (result.Length > 0)
                        {
                            result.Append(Space);
                        }

                        result.Append(value.Trim());
                    }
                }
            }

            return result.ToString();
        }

        #endregion Private Methods
    }
}