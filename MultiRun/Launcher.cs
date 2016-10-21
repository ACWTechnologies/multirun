using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace MultiRun.Launcher
{
    internal static class Launcher
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static CancellationTokenSource cts;
        private static CancellationToken ct;
        private static NotifyIcon trayIcon;

        public static void Begin(string[] args)
        {
            using (trayIcon = new NotifyIcon()
            {
                Text = "MultiRun Launcher",
                Icon = new Icon("MultiRun-Icon.ico", 40, 40),
                Visible = true
            })
            {
                trayIcon.Click += TrayIcon_Click;
                trayIcon.BalloonTipClicked += TrayIcon_BalloonTipClicked;

                cts = new CancellationTokenSource();
                ct = cts.Token;

                List<Task> allTasks = new List<Task>();
                foreach (string tempArg in args)
                {
                    try
                    {
                        string arg = Validator.AbsoluteToLocalPath(tempArg);
                        if (arg.IsValidFile())
                        {
                            try
                            {
                                switch (Validator.GetFileType(arg))
                                {
                                    case Reader.FileType.Plain:
                                        allTasks.Add(BeginPlain(Reader.GetPlainContents(arg)));
                                        break;

                                    case Reader.FileType.Json:
                                        allTasks.Add(BeginJson(Reader.GetJsonContents(arg)));
                                        break;
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.Error(ex);
                                System.Windows.MessageBox.Show("An exception occurred whilst attempting to read the MultiRun file.\nException message: " + ex.Message, "MultiRun Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
                            }
                        }
                        else
                        {
                            logger.Warn("Began Launcher with invalid file argument: '" + arg + "' is not a valid file.");
                            System.Windows.MessageBox.Show("'" + arg + "' is not a valid file.", "MultiRun Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
                        }
                    }
                    catch (UriFormatException ex) // For Validator.AbsoluteToLocalPath();
                    {
                        logger.Error(ex);
                        System.Windows.MessageBox.Show("The file URI is invalid.\nException message: " + ex.Message, "MultiRun Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
                    }
                    catch (Exception ex) // All other exceptions
                    {
                        logger.Error(ex);
                        System.Windows.MessageBox.Show("An exception occurred whilst attempting to read the file.\nException message: " + ex.Message, "MultiRun Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
                    }
                }
                try
                {
                    Task.WhenAll(allTasks).Wait(ct);
                }
                catch (OperationCanceledException)
                {
                    trayIcon.ShowBalloonTip(3000, "MultiRun", "The launcher operation was cancelled.", ToolTipIcon.None);
                    trayIcon.Click -= TrayIcon_Click;
                    trayIcon.BalloonTipClicked -= TrayIcon_BalloonTipClicked;
                    trayIcon.Visible = false;
                    logger.Warn("The MultiRun Launcher operation was canceled.");
                }
            }
        }

        private static Task BeginPlain(string[] plainContents)
        {
            return Launch(plainContents);
        }

        private static Task BeginJson(string jsonContents)
        {
            return Launch(Convert.JsonToPSIArray(jsonContents));
        }

        private static Task Launch(string[] items)
        {
            List<Task> tasks = new List<Task>();
            foreach (string item in items)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    try
                    {
                        Process.Start(item);
                    }
                    catch (Win32Exception ex)
                    {
                        logger.Error(ex);
                        System.Windows.MessageBox.Show(string.Format("The path '{0}' could not be found.", item), "MultiRun Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
                    }
                    catch (Exception ex)
                    {
                        logger.Error(ex);
                        System.Windows.MessageBox.Show(string.Format("An error occurred while starting '{0}'. Exception message: '{1}'", item, ex.Message), "MultiRun Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
                    }
                }));
            }
            return Task.WhenAll(tasks);
        }

        private static Task Launch(ProcessStartInformation[] items)
        {
            List<Task> tasks = new List<Task>();
            foreach (ProcessStartInformation PSI in items)
            {
                if (!string.IsNullOrWhiteSpace(PSI.Verb) && !PSI.Verbs.Contains(PSI.Verb))
                {
                    throw new ArgumentException("Invalid Verb");
                }

                ProcessStartInfo tempPSI = new ProcessStartInfo()
                {
                    FileName = PSI.FullPath,
                    Arguments = Convert.ArgArrayToString(PSI.Arguments),
                    Verb = PSI.Verb,
                    WindowStyle = PSI.GetProcessWindowStyle
                };

                if (PSI.Delay > 0)
                {
                    tasks.Add(Task.Delay(TimeSpan.FromSeconds(PSI.Delay)).ContinueWith(_ =>
                    {
                        try
                        {
                            Process.Start(tempPSI);
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex);
                            System.Windows.MessageBox.Show(string.Format("An error occurred while starting '{0}'. Exception message: '{1}'", tempPSI.FileName, ex.Message), "MultiRun Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
                        }
                    }));
                }
                else
                {
                    tasks.Add(Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            Process.Start(tempPSI);
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex);
                            System.Windows.MessageBox.Show(string.Format("An error occurred while starting '{0}'. Exception message: '{1}'", tempPSI.FileName, ex.Message), "MultiRun Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
                        }
                    }));
                }
            }
            return Task.WhenAll(tasks);
        }

        private static void TrayIcon_Click(object sender, EventArgs e)
        {
            trayIcon.ShowBalloonTip(3000, "MultiRun", "MultiRun is still launching one or more items. Click here to cancel the launcher operation.", ToolTipIcon.None);
        }

        private static void TrayIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            cts?.Cancel();
        }
    }
}