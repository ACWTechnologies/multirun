using System;
using System.Collections.ObjectModel;
using System.Deployment.Application;
using System.Text.RegularExpressions;
using System.Windows;
using NLog;

namespace MultiRun.Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private ObservableCollection<Launcher.ProcessStartInformation> Items = new ObservableCollection<Launcher.ProcessStartInformation>();
        private bool _unsavedChanges = false;

        public MainWindow()
        {
            InitializeComponent();
            Items.CollectionChanged += Items_CollectionChanged;
            listBox_items.ItemsSource = Items;
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _unsavedChanges = true;
        }

        #region Delay textbox validation

        private void textBox_delay_PreviewTextInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            e.Handled = !IsDelayTextAllowed(e.Text);
        }

        private void textBox_delay_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                var text = (string)e.DataObject.GetData(typeof(string));
                if (!IsDelayTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }

        private static bool IsDelayTextAllowed(string text)
        {
            // Regex that matches disallowed text; anything that isn't the numbers 0-9
            var regex = new Regex("[^0-9]+");
            return !regex.IsMatch(text);
        }

        #endregion Delay textbox validation

        private void button_addArgument_Click(object sender, RoutedEventArgs e)
        {
            AddArgument();
        }

        private void button_removeArgument_Click(object sender, RoutedEventArgs e)
        {
            int index = listBox_arguments.SelectedIndex;
            if (index > -1)
            {
                CurrentlySelectedItem.Arguments.Remove((string)listBox_arguments.SelectedItem);
                listBox_arguments.SelectedIndex = index - 1;
            }
        }

        public static Launcher.ProcessStartInformation CurrentlySelectedItem => (Launcher.ProcessStartInformation)(Application.Current.MainWindow as MainWindow)?.listBox_items.SelectedItem;

        private void button_save_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void button_browseFile_Click(object sender, RoutedEventArgs e)
        {
            string path = FileHelper.BrowseForFileOpen();
            if (path != null) { CurrentlySelectedItem.FullPath = path; }
        }

        private void button_addItem_Click(object sender, RoutedEventArgs e)
        {
            var newItem = new Launcher.ProcessStartInformation(null);
            Items.Add(newItem);
            listBox_items.SelectedItem = newItem;
        }

        private void button_removeItem_Click(object sender, RoutedEventArgs e)
        {
            int index = listBox_items.SelectedIndex;
            if (index > -1)
            {
                Items.Remove(CurrentlySelectedItem);
                listBox_items.SelectedIndex = index - 1;
            }
        }

        private void window_editor_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (_unsavedChanges && Items.Count > 0)
            {
                MessageBoxResult result = MessageBox.Show("You have unsaved changes. If you close now, your most recent changes will not be saved. Are you sure you want to close?", "MultiRun Close", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes) { e.Cancel = true; }
            }
        }

        private void textBox_newArgument_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                AddArgument();
            }
        }

        /// <summary>
        /// Get the current version of the network deployed application, formatted as 'major.minor[.build[.revision]]'.
        /// </summary>
        private static string GetCurrentVersion()
        {
            return ApplicationDeployment.IsNetworkDeployed ? ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString() : "Unavailable";
        }

        private void window_editor_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files == null) { return; }

                if (files.Length > 1) { MessageBox.Show($"Only one MR file can be opened at a time, you dragged in {files.Length}.", "MultiRun Open", MessageBoxButton.OK, MessageBoxImage.Warning); }
                else
                {
                    try
                    {
                        Open(files[0]);
                    }
                    catch (Exception ex) when (ex is InvalidPathException || ex is InvalidFileException)
                    {
                        Logger.Warn(ex);
                        MessageBox.Show(ex.Message, "MultiRun Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    catch (ArgumentException ex)
                    {
                        Logger.Error(ex);
                        MessageBox.Show($"An exception occurred whilst attempting to open the file you dragged in.\nException message: {ex.Message}", "MultiRun Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }
    }
}