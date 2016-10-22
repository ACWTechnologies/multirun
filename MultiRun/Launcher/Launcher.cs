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
using NLog;

namespace MultiRun.Launcher
{
    internal static class Launcher
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static CancellationTokenSource _cts;
        private static CancellationToken _ct;
        private static NotifyIcon _trayIcon;

        public static void Begin(string[] args)
        {
            using (_trayIcon = new NotifyIcon
            {
                Text = "MultiRun Launcher",
                Icon = new Icon("MultiRun-Icon.ico", 40, 40),
                Visible = true
            })
            {
                _trayIcon.Click += TrayIcon_Click;
                _trayIcon.BalloonTipClicked += TrayIcon_BalloonTipClicked;

                _cts = new CancellationTokenSource();
                _ct = _cts.Token;

                var allTasks = new List<Task>();
                foreach (string tempArg in args)
                {
                    try
                    {
                        string arg = Validator.AbsoluteToLocalPath(tempArg);
                        if (arg.IsValidFile())
                        {
                            try
                            {
                                switch (arg.GetFileType())
                                {
                                    case Reader.FileType.Plain:
                                        allTasks.Add(BeginPlain(Reader.GetPlainContents(arg)));
                                        break;

                                    case Reader.FileType.Json:
                                        allTasks.Add(BeginJson(Reader.GetJsonContents(arg)));
                                        break;

                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                            }
                            catch (Exception ex)
                            {
                                Logger.Error(ex);
                                System.Windows.MessageBox.Show($"An exception occurred whilst attempting to read the MultiRun file.\nException message: {ex.Message}", "MultiRun Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
                            }
                        }
                        else
                        {
                            Logger.Warn($"Began Launcher with invalid file argument: '{arg}' is not a valid file.");
                            System.Windows.MessageBox.Show($"'{arg}' is not a valid file.", "MultiRun Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
                        }
                    }
                    catch (UriFormatException ex) // For Validator.AbsoluteToLocalPath();
                    {
                        Logger.Error(ex);
                        System.Windows.MessageBox.Show($"The file URI is invalid.\nException message: {ex.Message}", "MultiRun Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                        System.Windows.MessageBox.Show($"An exception occurred whilst attempting to read the file.\nException message: {ex.Message}", "MultiRun Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
                    }
                }
                try
                {
                    Task.WhenAll(allTasks).Wait(_ct);
                }
                catch (OperationCanceledException)
                {
                    _trayIcon.ShowBalloonTip(3000, "MultiRun", "The launcher operation was cancelled.", ToolTipIcon.None);
                    _trayIcon.Click -= TrayIcon_Click;
                    _trayIcon.BalloonTipClicked -= TrayIcon_BalloonTipClicked;
                    _trayIcon.Visible = false;
                    Logger.Warn("The MultiRun Launcher operation was canceled.");
                }
            }
        }

        private static Task BeginPlain(IEnumerable<string> plainContents)
        {
            return Launch(plainContents);
        }

        private static Task BeginJson(string jsonContents)
        {
            return Launch(Convert.JsonToItemArray(jsonContents));
        }

        private static Task Launch(IEnumerable<string> items)
        {
            return Task.WhenAll(
                items.Select(item => Task.Factory.StartNew(() =>
                {
                    try
                    {
                        Process.Start(item);
                    }
                    catch (Win32Exception ex)
                    {
                        Logger.Error(ex);
                        System.Windows.MessageBox.Show($"The path '{item}' could not be found.", "MultiRun Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                        System.Windows.MessageBox.Show($"An error occurred while starting '{item}'. Exception message: '{ex.Message}'", "MultiRun Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
                    }
                }))
            );
        }

        private static Task Launch(IEnumerable<ProcessStartInformation> items)
        {
            var tasks = new List<Task>();
            foreach (ProcessStartInformation item in items)
            {
                if (!string.IsNullOrWhiteSpace(item.Verb) && !item.Verbs.Contains(item.Verb))
                {
                    throw new ArgumentException("Invalid Verb");
                }

                var tempItem = new ProcessStartInfo()
                {
                    FileName = item.FullPath,
                    Arguments = Convert.ArgArrayToString(item.Arguments),
                    Verb = item.Verb,
                    WindowStyle = item.GetProcessWindowStyle
                };

                if (item.Delay > 0)
                {
                    tasks.Add(Task.Delay(TimeSpan.FromSeconds(item.Delay)).ContinueWith(_ =>
                    {
                        try
                        {
                            Process.Start(tempItem);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);
                            System.Windows.MessageBox.Show($"An error occurred while starting '{tempItem.FileName}'. Exception message: '{ex.Message}'", "MultiRun Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
                        }
                    }));
                }
                else
                {
                    tasks.Add(Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            Process.Start(tempItem);
                        }
                        catch (Exception ex)
                        {
                            Logger.Error(ex);
                            System.Windows.MessageBox.Show($"An error occurred while starting '{tempItem.FileName}'. Exception message: '{ex.Message}'", "MultiRun Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK, System.Windows.MessageBoxOptions.DefaultDesktopOnly);
                        }
                    }));
                }
            }
            return Task.WhenAll(tasks);
        }

        private static void TrayIcon_Click(object sender, EventArgs e)
        {
            _trayIcon.ShowBalloonTip(3000, "MultiRun", "MultiRun is still launching one or more items. Click here to cancel the launcher operation.", ToolTipIcon.None);
        }

        private static void TrayIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            _cts?.Cancel();
        }
    }
}