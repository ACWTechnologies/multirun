using System;
using System.Collections.Generic;

namespace MultiRun.Editor
{
    internal static class FileHelper
    {
        public static readonly Dictionary<Reader.FileType, string> FileFilters = new Dictionary<Reader.FileType, string>()
        {
            { Reader.FileType.Json, "MultiRun Json File (*.mr)|*.mr" },
            { Reader.FileType.Plain, "MultiRun Plain File (*.mr)|*.mr" }
        };

        /// <summary>
        /// Allows the user to browse for a folder.
        /// </summary>
        /// <param name="description">The descriptive text displayed above the tree view control in the dialog box.</param>
        /// <param name="rootFolder">The root folder where the browsing starts from.</param>
        /// <param name="showNewFolderButton">A value indicating whether the New Folder button appears in the folder browser dialog box.</param>
        /// <returns>The path of the folder, or null if browse cancelled.</returns>
        public static string BrowseForFolder(string description = "", Environment.SpecialFolder rootFolder = Environment.SpecialFolder.Desktop, bool showNewFolderButton = true)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.Description = description;
            dialog.RootFolder = rootFolder;
            dialog.ShowNewFolderButton = showNewFolderButton;

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return dialog.SelectedPath;
            }
            return null;
        }

        public static string BrowseForFileOpen(bool multiselect = false, string filter = null)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Multiselect = multiselect;
            dialog.Filter = filter;

            if (dialog.ShowDialog() ?? false)
            {
                return dialog.FileName;
            }
            return null;
        }

        public static string BrowseForFileSave(string filter = null)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.Filter = filter;

            if (dialog.ShowDialog() ?? false)
            {
                return dialog.FileName;
            }
            return null;
        }

        public static Microsoft.Win32.SaveFileDialog BrowseForFileSaveDialog(string filter = null)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.Filter = filter;

            if (dialog.ShowDialog() ?? false)
            {
                return dialog;
            }
            return null;
        }
    }
}