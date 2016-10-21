using NLog;
using System.Collections.ObjectModel;
using System.Deployment.Application;
using System.Text.RegularExpressions;
using System.Windows;

namespace MultiRun.Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        private ObservableCollection<Launcher.ProcessStartInformation> Items = new ObservableCollection<Launcher.ProcessStartInformation>();
        private bool unsavedChanges = false;

        public MainWindow()
        {
            InitializeComponent();
            Items.CollectionChanged += Items_CollectionChanged;
            listBox_items.ItemsSource = Items;
        }

        private void Items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            unsavedChanges = true;
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
                string text = (string)e.DataObject.GetData(typeof(string));
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
            Regex regex = new Regex("[^0-9]+");
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

        private Launcher.ProcessStartInformation CurrentlySelectedItem
        {
            get { return (Launcher.ProcessStartInformation)listBox_items.SelectedItem; }
        }

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
            if (unsavedChanges && Items.Count > 0)
            {
                var result = MessageBox.Show("You have unsaved changes. If you close now, your most recent changes will not be saved. Are you sure you want to close?", "MultiRun Close", MessageBoxButton.YesNo, MessageBoxImage.Question);
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
        private string GetCurrentVersion()
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                // Application is network deployed
                return ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }
            else
            {
                // Application is not network deployed
                return "Unavailable";
            }
        }
    }
}