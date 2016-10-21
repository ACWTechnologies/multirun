using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiRun.Editor
{
    internal static class Serializer
    {
        public static string[] JsonSerialize(IEnumerable<Launcher.ProcessStartInformation> items)
        {
            if (items == null) { throw new ArgumentNullException("items"); }

            string json = JsonConvert.SerializeObject(items, Formatting.Indented);
            return json.AddTypeMetadata(Reader.FileType.Json);
        }

        public static string[] PlainSerialize(IEnumerable<Launcher.ProcessStartInformation> items)
        {
            if (items == null) { throw new ArgumentNullException("items"); }

            List<string> result = new List<string>();
            foreach (var item in items)
            {
                result.Add(item.FullPath);
            }
            return result.AddTypeMetadata(Reader.FileType.Plain);
        }

        private static string[] AddTypeMetadata(this string serialized, Reader.FileType type)
        {
            return new string[] { serialized }.AddTypeMetadata(type);
        }

        private static string[] AddTypeMetadata(this IEnumerable<string> serialized, Reader.FileType type)
        {
            List<string> list = serialized.OfType<string>().ToList();
            string prefix = "@@";

            switch (type)
            {
                case Reader.FileType.Json:
                    prefix += "json";
                    break;

                case Reader.FileType.Plain:
                    prefix += "plain";
                    break;
            }

            list.Insert(0, prefix);
            return list.ToArray();
        }
    }
}