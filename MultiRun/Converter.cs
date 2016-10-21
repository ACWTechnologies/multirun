using MultiRun.Launcher;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultiRun
{
    internal class Convert
    {
        public static ProcessStartInformation[] JsonToPSIArray(string jsonContents)
        {
            List<ProcessStartInformation> PSIs = new List<ProcessStartInformation>();
            PSIs.AddRange(JsonConvert.DeserializeObject<List<ProcessStartInformation>>(jsonContents, new JsonSerializerSettings() { MissingMemberHandling = MissingMemberHandling.Error }));
            return PSIs.ToArray();
        }

        public static string StringArrayToString(IEnumerable<string> array)
        {
            return string.Join(Environment.NewLine, array);
        }

        public static string ArgArrayToString(IEnumerable<string> args)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string arg in args)
            {
                sb.Append(string.Format("\"{0}\" ", arg.Trim()));
            }
            return sb.ToString().Trim();
        }
    }
}