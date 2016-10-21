using System;
using System.IO;
using System.Windows;

namespace MultiRun.Editor
{
    public partial class MainWindow
    {
        private void Open()
        {
            string mrFile = FileHelper.BrowseForFileOpen(false, "MultiRun File (*.mr)|*.mr");
            if (mrFile != null)
            {
                if (Items.Count > 0)
                {
                    var result = MessageBox.Show("Are you sure you want to load a new MultiRun file, the current file will not be saved. If you wish to save the current file, select 'no' first.", "Open File", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result != MessageBoxResult.Yes) { return; }
                }

                switch (Launcher.Validator.GetFileType(mrFile))
                {
                    case Reader.FileType.Json:

                        string jsonContents = Reader.GetJsonContents(mrFile, true);
                        try
                        {
                            Launcher.ProcessStartInformation[] PSIs = Convert.JsonToPSIArray(jsonContents);
                            Items.Clear();
                            foreach (var item in PSIs) { Items.Add(item); }
                            unsavedChanges = false;
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex);
                            MessageBox.Show("An exception occurred whilst attempting to read the file.\nException message: " + ex.Message, "MultiRun Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        break;

                    case Reader.FileType.Plain:
                        string[] plainContents = Reader.GetPlainContents(mrFile, true);
                        Items.Clear();
                        foreach (var item in plainContents) { Items.Add(new Launcher.ProcessStartInformation(item)); }
                        unsavedChanges = false;
                        break;
                }
            }
        }

        private bool ReadyForSave()
        {
            if (Items.Count > 0)
            {
                if (Launcher.ProcessStartInformation.AreValid(Items))
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
            if (ReadyForSave())
            {
                var dialog = FileHelper.BrowseForFileSaveDialog(string.Join("|", FileHelper.FileFilters[Reader.FileType.Json], FileHelper.FileFilters[Reader.FileType.Plain]));
                if (dialog?.FileName != null)
                {
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
                    unsavedChanges = false;
                }
            }
        }

        private void SaveAs(Reader.FileType type)
        {
            if (ReadyForSave())
            {
                var dialog = FileHelper.BrowseForFileSaveDialog(FileHelper.FileFilters[type]);
                if (dialog.FileName != null)
                {
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
                    unsavedChanges = false;
                }
            }
        }

        private new void Close()
        {
            base.Close();
        }

        private void ClearAll()
        {
            if (Items.Count > 0)
            {
                var result = MessageBox.Show("Are you sure you want to clear all of the current items? This will remove every item and cannot be undone.", "Clear All?", MessageBoxButton.YesNo);
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
            if (!string.IsNullOrWhiteSpace(textBox_newArgument.Text))
            {
                CurrentlySelectedItem.Arguments.Add(textBox_newArgument.Text);
                textBox_newArgument.Text = string.Empty;
                textBox_newArgument.Focus();
            }
        }
    }
}