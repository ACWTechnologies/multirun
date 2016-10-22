using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace MultiRun.Editor
{
    internal static class Serializer
    {
        public static string[] JsonSerialize(IEnumerable<Launcher.ProcessStartInformation> items)
        {
            if (items == null) { throw new ArgumentNullException(nameof(items)); }

            string json = JsonConvert.SerializeObject(items, Formatting.Indented);
            return json.AddTypeMetadata(Reader.FileType.Json);
        }

        public static string[] PlainSerialize(IEnumerable<Launcher.ProcessStartInformation> items)
        {
            if (items == null) { throw new ArgumentNullException(nameof(items)); }

            List<string> result = items
                .Select(item => item.FullPath)
                .ToList();
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
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }

            list.Insert(0, prefix);
            return list.ToArray();
        }
    }
}