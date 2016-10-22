using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using NLog;

namespace MultiRun.Launcher
{
    public sealed class ProcessStartInformation : INotifyPropertyChanged
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private ObservableCollection<string> _arguments;
        private int _delay;
        private string _fullPath;
        private string _verb;
        private ProcessStartWindowStyle _windowStyle;

        [JsonConstructor]
        public ProcessStartInformation(string fileName, ObservableCollection<string> arguments = null, string verb = null, ProcessStartWindowStyle windowStyle = ProcessStartWindowStyle.Normal, int delay = 0)
        {
            FullPath = fileName;
            Arguments = arguments;
            Verb = verb;
            WindowStyle = windowStyle;
            Delay = delay;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public enum ProcessStartWindowStyle { Normal = 0, Hidden = 1, Minimized = 2, Maximized = 3 }

        public ObservableCollection<string> Arguments
        {
            get { return _arguments; }
            set
            {
                _arguments = value ?? new ObservableCollection<string>();
                NotifyPropertyChanged();
            }
        }

        public int Delay
        {
            get { return _delay; }
            set
            {
                _delay = value;
                NotifyPropertyChanged();
            }
        }

        [JsonIgnore]
        public string FileExtension => Path.GetExtension(FullPath);

        [JsonProperty(Required = Required.Always)]
        public string FullPath
        {
            get { return _fullPath; }
            set
            {
                _fullPath = value ?? string.Empty;
                NotifyPropertyChanged();
                
                NotifyPropertyChanged("Verbs");
                if (Verb != null && !Verbs.Contains(Verb))
                {
                    Verb = null;
                }
            }
        }

        [JsonIgnore]
        public ProcessWindowStyle GetProcessWindowStyle => (ProcessWindowStyle)WindowStyle;

        public string Verb
        {
            get { return _verb; }
            set
            {
                if (value != null && value.Contains("[none]")) { value = null; }
                _verb = value ?? string.Empty;
                NotifyPropertyChanged();
            }
        }

        [JsonIgnore]
        public string[] Verbs
        {
            get
            {
                try { return new ProcessStartInfo(FullPath).Verbs; }
                catch (Exception ex)
                {
                    Logger.Warn(ex);
                    return new string[0];
                }
            }
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public ProcessStartWindowStyle WindowStyle
        {
            get { return _windowStyle; }
            set
            {
                _windowStyle = value;
                NotifyPropertyChanged();
            }
        }

        public static bool AreValid(IEnumerable<ProcessStartInformation> items)
        {
            foreach (var item in items)
            {
                if (!item.IsValid()) { return false; }
            }
            return true;
        }

        public void Clear()
        {
            FullPath = null;
            Arguments = null;
            Verb = null;
            WindowStyle = ProcessStartWindowStyle.Normal;
            Delay = 0;
        }

        public bool IsValid()
        {
            if (string.IsNullOrWhiteSpace(FullPath))
            {
                return false;
            }
            return true;
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}