using System;
using System.Collections.Generic;
using System.IO;
using static MultiRun.Reader;

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
                throw new ArgumentException("The file '" + path + "' is not a MultiRun file (.mr)");
            }
            return File.Exists(path);
        }

        public static FileType GetFileType(this string path)
        {
            string[] contents = File.ReadAllLines(path);

            if (contents.Length < 1) { throw new ArgumentException("The specified file has no content."); }

            if (contents[0].StartsWith("@@"))
            {
                string type = contents[0].Remove(0, 2).Trim().ToLower();
                switch (type)
                {
                    case "json":
                        return FileType.Json;

                    case "plain":
                    default:
                        return FileType.Plain;
                }
            }
            else
            {
                return FileType.Plain;
            }
        }

        public static string CleanJsonContents(string[] contents)
        {
            List<string> cleaned = new List<string>();
            foreach (string line in contents)
            {
                string trimmedLine = line.Trim();
                if (trimmedLine.Length == 0 || trimmedLine.StartsWith("@@") || trimmedLine.StartsWith("##")) { continue; }
                else
                {
                    cleaned.Add(trimmedLine);
                }
            }
            return Convert.StringArrayToString(cleaned);
        }

        public static string[] CleanPlainContents(string[] contents)
        {
            List<string> cleaned = new List<string>();
            foreach (string line in contents)
            {
                string trimmedLine = line.Trim();
                if (trimmedLine.Length == 0 || trimmedLine.StartsWith("@@") || trimmedLine.StartsWith("##")) { continue; }
                else
                {
                    cleaned.Add(trimmedLine);
                }
            }
            return cleaned.ToArray();
        }
    }
}