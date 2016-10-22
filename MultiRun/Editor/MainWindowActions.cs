using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using MultiRun.Launcher;

namespace MultiRun.Editor
{
    public partial class MainWindow
    {
        private void Open()
        {
            string mrFile = FileHelper.BrowseForFileOpen(false, "MultiRun File (*.mr)|*.mr");
            if (mrFile != null)
            {
                Open(mrFile);
            }
        }

        private void Open(string path)
        {
            if (!path.EndsWith(".mr")) { throw new InvalidFileException("The file dragged in is not an MR file. It does not have the '.mr' file extension."); }
            if (!File.Exists(path)) { throw new InvalidPathException("The path to the file dragged in does not exist."); }

            if (Items.Count > 0)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to load a new MultiRun file, the current file will not be saved. If you wish to save the current file, select 'no' first.", "Open File", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result != MessageBoxResult.Yes) { return; }
            }

            switch (path.GetFileType())
            {
                case Reader.FileType.Json:

                    string jsonContents = Reader.GetJsonContents(path);
                    try
                    {
                        ProcessStartInformation[] items = Convert.JsonToItemArray(jsonContents);
                        Items.Clear();
                        foreach (ProcessStartInformation item in items) { Items.Add(item); }
                        _unsavedChanges = false;
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex);
                        MessageBox.Show($"An exception occurred whilst attempting to read the file.\nException message: {ex.Message}", "MultiRun Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    break;

                case Reader.FileType.Plain:
                    string[] plainContents = Reader.GetPlainContents(path);
                    Items.Clear();
                    foreach (string item in plainContents) { Items.Add(new ProcessStartInformation(item)); }
                    _unsavedChanges = false;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool ReadyForSave()
        {
            if (Items.Count > 0)
            {
                if (ProcessStartInformation.AreValid(Items))
                {
                    return true;
                }
                else
                {
                    MessageBox.Show("One or more items are not valid. This is likely due to a lack of Full Path.", "MultiRun Save", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }
            }
            else
            {
                MessageBox.Show("There are no items to save.", "MultiRun Save", MessageBoxButton.OK, MessageBoxImage.Information);
                return false;
            }
        }

        private void Save()
        {
            if (!ReadyForSave()) { return; }

            var dialog = FileHelper.BrowseForFileSaveDialog(string.Join("|", FileHelper.FileFilters[Reader.FileType.Json], FileHelper.FileFilters[Reader.FileType.Plain]));
            if (dialog?.FileName == null) { return; }
            string[] serializedContents;
            switch (dialog.FilterIndex)
            {
                case 1:
                    // Json
                    serializedContents = Serializer.JsonSerialize(Items);
                    break;

                case 2:
                    // Plain
                    serializedContents = Serializer.PlainSerialize(Items);
                    break;

                default:
                    return;
            }
            File.WriteAllLines(dialog.FileName, serializedContents);
            _unsavedChanges = false;
        }

        private void SaveAs(Reader.FileType type)
        {
            if (!ReadyForSave()) { return; }

            SaveFileDialog dialog = FileHelper.BrowseForFileSaveDialog(FileHelper.FileFilters[type]);
            if (dialog.FileName == null) { return; }

            string[] serializedContents;
            switch (type)
            {
                case Reader.FileType.Json:
                    serializedContents = Serializer.JsonSerialize(Items);
                    break;

                case Reader.FileType.Plain:
                    serializedContents = Serializer.PlainSerialize(Items);
                    break;

                default:
                    return;
            }

            File.WriteAllLines(dialog.FileName, serializedContents);
            _unsavedChanges = false;
        }

        private new void Close()
        {
            base.Close();
        }

        private void ClearAll()
        {
            if (Items.Count > 0)
            {
                MessageBoxResult result = MessageBox.Show("Are you sure you want to clear all of the current items? This will remove every item and cannot be undone.", "Clear All?", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    Items.Clear();
                }
            }
            else
            {
                MessageBox.Show("There are no items to clear.", "MultiRun Clear", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ClearCurrent()
        {
            if (listBox_items.SelectedIndex > -1)
            {
                CurrentlySelectedItem?.Clear();
            }
            else
            {
                MessageBox.Show("No item is selected to clear.", "MultiRun Clear", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void AddArgument()
        {
            if (string.IsNullOrWhiteSpace(textBox_newArgument.Text)) { return; }

            CurrentlySelectedItem.Arguments.Add(textBox_newArgument.Text);
            textBox_newArgument.Text = string.Empty;
            textBox_newArgument.Focus();
        }
    }
}