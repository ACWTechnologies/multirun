using MultiRun.Launcher;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultiRun
{
    internal class Convert
    {
        public static ProcessStartInformation[] JsonToItemArray(string jsonContents)
        {
            var items = new List<ProcessStartInformation>();
            items.AddRange(JsonConvert.DeserializeObject<List<ProcessStartInformation>>(jsonContents, new JsonSerializerSettings { MissingMemberHandling = MissingMemberHandling.Error }));
            return items.ToArray();
        }

        public static string StringArrayToString(IEnumerable<string> array)
        {
            return string.Join(Environment.NewLine, array);
        }

        public static string ArgArrayToString(IEnumerable<string> args)
        {
            var sb = new StringBuilder();
            foreach (string arg in args)
            {
                sb.Append($"\"{arg.Trim()}\" ");
            }
            return sb.ToString().Trim();
        }
    }
}