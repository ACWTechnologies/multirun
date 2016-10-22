using NLog;
using NLog.Targets;
using System;
using System.Windows;

namespace MultiRun
{
    internal static class EntryPoint
    {
        private static readonly Logger Logger = LogManager.GetLogger("CurrentDomainUnhandledExceptionLogger");
        private static readonly string MultiRunDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\ACW Technologies\MultiRun\";
        private static readonly string LogFileDirectory = MultiRunDirectory + @"Logs\";

        [STAThread]
        public static void Main(string[] args)
        {
            var target = (FileTarget)LogManager.Configuration.FindTargetByName("logfile");
            target.FileName = LogFileDirectory + "${shortdate}.log";

            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += CurrentDomain_UnhandledException;

            if (args != null && args.Length > 0)
            {
                BeginLauncher(args);
            }
            else if (System.Deployment.Application.ApplicationDeployment.IsNetworkDeployed
                && currentDomain.SetupInformation.ActivationArguments.ActivationData != null
                && currentDomain.SetupInformation.ActivationArguments.ActivationData.Length > 0)
            {
                BeginLauncher(currentDomain.SetupInformation.ActivationArguments.ActivationData);
            }
            else
            {
                BeginEditor();
            }
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.Fatal((Exception)e.ExceptionObject);
            MessageBox.Show("An unhandled exception occurred. Message: '" + ((Exception)e.ExceptionObject).Message + "'.", "MultiRun Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private static void BeginLauncher(string[] args)
        {
            Launcher.Launcher.Begin(args);
        }

        private static void BeginEditor()
        {
            App.Main();
        }
    }
}