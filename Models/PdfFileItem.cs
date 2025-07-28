using CommunityToolkit.Mvvm.ComponentModel;
using System.IO;

namespace PDFRename.Models
{
    public partial class PdfFileItem : ObservableObject
    {
        [ObservableProperty]
        private string originalFilePath = string.Empty;

        [ObservableProperty]
        private string originalFileName = string.Empty;

        [ObservableProperty]
        private string newFileName = string.Empty;

        [ObservableProperty]
        private string pdfTitle = string.Empty;

        [ObservableProperty]
        private FileStatus status = FileStatus.Pending;

        [ObservableProperty]
        private string statusMessage = string.Empty;

        [ObservableProperty]
        private bool isSelected = true;

        [ObservableProperty]
        private bool canEdit = false;

        [ObservableProperty]
        private bool hasError = false;

        public string DirectoryPath => Path.GetDirectoryName(OriginalFilePath) ?? string.Empty;
        
        public string NewFilePath => Path.Combine(DirectoryPath, NewFileName);

        public void UpdateStatus(FileStatus newStatus, string message = "")
        {
            Status = newStatus;
            StatusMessage = message;
            HasError = newStatus == FileStatus.Error;
        }
    }
}
