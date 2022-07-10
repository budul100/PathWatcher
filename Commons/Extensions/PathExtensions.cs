using System.IO;

namespace Commons.Extensions
{
    public static class PathExtensions
    {
        #region Public Methods

        public static string CreateDirectory(this string path)
        {
            var result = default(string);

            if (!string.IsNullOrWhiteSpace(path))
            {
                result = Path.GetFullPath(path);

                if (!Directory.Exists(result))
                {
                    Directory.CreateDirectory(result);
                }
            }

            return result;
        }

        public static bool Exists(this string path, bool isDirectory)
        {
            var result = (isDirectory && Directory.Exists(path))
                || File.Exists(path);

            return result;
        }

        public static string GetDirectory(this string path)
        {
            var result = path;

            if (!string.IsNullOrWhiteSpace(result)
                && !Directory.Exists(result))
            {
                var full = Path.GetFullPath(result);

                if (Directory.Exists(full))
                {
                    result = full;
                }
            }

            return result;
        }

        public static bool IsDirectory(this string path)
        {
            var result = Directory.Exists(path)
                && (File.GetAttributes(path) & FileAttributes.Directory) == FileAttributes.Directory;

            return result;
        }

        public static bool IsHidden(this string path, bool isDirectory)
        {
            var result = path.Exists(isDirectory)
                && File.GetAttributes(path).HasFlag(FileAttributes.Hidden);

            return result;
        }

        public static string MoveDirectory(this string sourcePath, string destDirectory)
        {
            var result = default(string);

            if (Directory.Exists(sourcePath))
            {
                var sourceName = Path.GetFileName(sourcePath);

                result = Path.Combine(
                    destDirectory,
                    sourceName);

                if (Directory.Exists(result))
                {
                    Directory.Delete(result);
                }

                Directory.Move(
                    sourceDirName: sourcePath,
                    destDirName: result);
            }

            return result;
        }

        public static string MoveFile(this string sourcePath, string destDirectory)
        {
            var result = default(string);

            if (File.Exists(sourcePath))
            {
                var sourceName = Path.GetFileName(sourcePath);

                result = Path.Combine(
                    destDirectory,
                    sourceName);

                if (File.Exists(result))
                {
                    File.Delete(result);
                }

                File.Move(
                    sourceFileName: sourcePath,
                    destFileName: result);
            }

            return result;
        }

        #endregion Public Methods
    }
}