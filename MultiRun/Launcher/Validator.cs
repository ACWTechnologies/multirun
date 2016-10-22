using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MultiRun.Launcher
{
    internal static class Validator
    {
        public static string AbsoluteToLocalPath(string absolutePath)
        {
            return new Uri(absolutePath).LocalPath;
        }

        public static bool IsValidFile(this string path)
        {
            if (Path.GetExtension(path) != ".mr")
            {
                throw new ArgumentException($"The file '{path}' is not a MultiRun file (.mr)");
            }
            return File.Exists(path);
        }

        public static Reader.FileType GetFileType(this string path)
        {
            string[] contents = File.ReadAllLines(path);

            if (contents.Length < 1) { throw new ArgumentException("The specified file has no content."); }

            if (contents[0].StartsWith("@@"))
            {
                string type = contents[0].Remove(0, 2).Trim().ToLower();
                switch (type)
                {
                    case "json":
                        return Reader.FileType.Json;

                    default:
                        return Reader.FileType.Plain;
                }
            }
            else
            {
                return Reader.FileType.Plain;
            }
        }

        public static string CleanJsonContents(string[] contents)
        {
            List<string> cleaned = contents
                .Select(line => line.Trim())
                .Where(trimmedLine => !(trimmedLine.Length == 0 || trimmedLine.StartsWith("@@") || trimmedLine.StartsWith("##")))
                .ToList();
            return Convert.StringArrayToString(cleaned);
        }

        public static string[] CleanPlainContents(string[] contents)
        {
            return contents
                .Select(line => line.Trim())
                .Where(trimmedLine => !(trimmedLine.Length == 0 || trimmedLine.StartsWith("@@") || trimmedLine.StartsWith("##")))
                .ToArray();
        }
    }
}