using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PDFRename.ViewModels
{
    public partial class EditFileNameViewModel : ObservableObject
    {
        [ObservableProperty]
        private string fileName = string.Empty;

        [ObservableProperty]
        private string originalFileName = string.Empty;

        public string NewFileName => !string.IsNullOrWhiteSpace(FileName) 
            ? (FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) ? FileName : FileName + ".pdf")
            : string.Empty;

        public EditFileNameViewModel(string currentFileName)
        {
            OriginalFileName = currentFileName;
            
            // Remove .pdf extension for editing
            FileName = currentFileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase) 
                ? currentFileName.Substring(0, currentFileName.Length - 4) 
                : currentFileName;
        }

        [RelayCommand]
        private void Accept()
        {
            DialogResult = true;
        }

        [RelayCommand]
        private void Cancel()
        {
            DialogResult = false;
        }

        // Property to communicate with the view
        [ObservableProperty]
        private bool? dialogResult;
    }
}
