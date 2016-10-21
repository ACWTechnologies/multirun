using System.IO;

namespace MultiRun
{
    internal static class Reader
    {
        public enum FileType { Plain, Json }

        public static string[] GetPlainContents(string path, bool clean = true)
        {
            string[] plainContents = File.ReadAllLines(path);
            return clean ? Launcher.Validator.CleanPlainContents(plainContents) : plainContents;
        }

        public static string GetJsonContents(string path, bool clean = true)
        {
            string[] jsonContentsTemp = File.ReadAllLines(path);
            return clean ? Launcher.Validator.CleanJsonContents(jsonContentsTemp) : Convert.StringArrayToString(jsonContentsTemp);
        }
    }
}