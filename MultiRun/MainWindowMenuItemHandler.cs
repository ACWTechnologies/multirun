using System.Diagnostics;
using System.Text;
using System.Windows;

namespace MultiRun.Editor
{
    public partial class MainWindow
    {
        #region File

        private void menuItem_file_open(object sender, RoutedEventArgs e)
        {
            Open();
        }

        private void menuItem_file_save(object sender, RoutedEventArgs e)
        {
            Save();
        }

        private void menuItem_file_saveAs_json(object sender, RoutedEventArgs e)
        {
            SaveAs(Reader.FileType.Json);
        }

        private void menuItem_file_saveAs_plain(object sender, RoutedEventArgs e)
        {
            SaveAs(Reader.FileType.Plain);
        }

        private void menuItem_file_close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        #endregion File

        #region Edit

        private void menuItem_edit_clear_allItems(object sender, RoutedEventArgs e)
        {
            ClearAll();
        }

        private void menuItem_edit_clear_currentItem(object sender, RoutedEventArgs e)
        {
            ClearCurrent();
        }

        #endregion Edit

        #region Help

        private void menuItem_help_viewHelp(object sender, RoutedEventArgs e)
        {
            Process.Start("http://acwtechnologies.co.uk/help/multirun/?sender=mr");
        }

        private void menuItem_help_reportAProblem(object sender, RoutedEventArgs e)
        {
            Process.Start("http://acwtechnologies.co.uk/contact-us/?sender=mr");
        }

        private void menuItem_help_about(object sender, RoutedEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("MultiRun");
            sb.AppendLine("Simplifying the art of starting processes.");
            sb.AppendLine();
            sb.AppendLine("Version: " + GetCurrentVersion());
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("Licensed under a Creative Commons Attribution-NonCommercial 4.0 International license.");
            sb.AppendLine("Copyright © 2016 ACW Technologies");
            MessageBox.Show(sb.ToString(), "About MultiRun", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        #endregion Help
    }
}