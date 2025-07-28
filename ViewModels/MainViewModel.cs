using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PDFRename.Models;
using PDFRename.Services;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;

namespace PDFRename.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly PdfMetadataService _metadataService;
        private readonly FileRenameService _renameService;

        [ObservableProperty]
        private ObservableCollection<PdfFileItem> pdfFiles = new();

        [ObservableProperty]
        private ProcessingMode selectedMode = ProcessingMode.AutomaticRename;

        [ObservableProperty]
        private bool isProcessing = false;

        [ObservableProperty]
        private string statusText = "Bereit";

        [ObservableProperty]
        private int totalFiles = 0;

        [ObservableProperty]
        private int processedFiles = 0;

        public List<ProcessingMode> AvailableModes { get; } = new()
        {
            ProcessingMode.AutomaticRename,
            ProcessingMode.SimulationMode,
            ProcessingMode.EditBeforeRename
        };

        public string ModeDisplayName => SelectedMode switch
        {
            ProcessingMode.AutomaticRename => "Automatisch umbenennen",
            ProcessingMode.SimulationMode => "Simulations-Modus",
            ProcessingMode.EditBeforeRename => "Namen vorher bearbeiten",
            _ => "Unbekannt"
        };

        public bool ShowEditButtons => SelectedMode == ProcessingMode.EditBeforeRename;
        public bool ShowProcessButton => SelectedMode == ProcessingMode.EditBeforeRename;
        public bool IsSimulationMode => SelectedMode == ProcessingMode.SimulationMode;

        public MainViewModel()
        {
            _metadataService = new PdfMetadataService();
            _renameService = new FileRenameService();
        }

        [RelayCommand]
        private async Task ProcessDroppedFiles(string[] filePaths)
        {
            if (IsProcessing) return;

            var pdfFiles = filePaths.Where(f => Path.GetExtension(f).Equals(".pdf", StringComparison.OrdinalIgnoreCase)).ToArray();
            
            if (!pdfFiles.Any())
            {
                MessageBox.Show("Keine PDF-Dateien gefunden.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            IsProcessing = true;
            TotalFiles = pdfFiles.Length;
            ProcessedFiles = 0;
            StatusText = $"Verarbeite {TotalFiles} Dateien...";

            try
            {
                foreach (var filePath in pdfFiles)
                {
                    var fileItem = new PdfFileItem
                    {
                        OriginalFilePath = filePath,
                        OriginalFileName = Path.GetFileName(filePath),
                        CanEdit = ShowEditButtons
                    };

                    PdfFiles.Add(fileItem);

                    if (SelectedMode != ProcessingMode.EditBeforeRename)
                    {
                        await ProcessSingleFile(fileItem);
                    }
                    else
                    {
                        await ReadMetadataOnly(fileItem);
                    }

                    ProcessedFiles++;
                }

                StatusText = $"Verarbeitung abgeschlossen. {ProcessedFiles}/{TotalFiles} Dateien verarbeitet.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler bei der Verarbeitung: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusText = "Verarbeitung mit Fehlern abgeschlossen.";
            }
            finally
            {
                IsProcessing = false;
            }
        }

        private async Task ProcessSingleFile(PdfFileItem fileItem)
        {
            await Task.Run(() =>
            {
                try
                {
                    fileItem.UpdateStatus(FileStatus.ReadingMetadata, "Lese Metadaten...");
                    
                    var title = _metadataService.ExtractTitle(fileItem.OriginalFilePath);
                    fileItem.PdfTitle = title;
                    fileItem.NewFileName = title;

                    fileItem.UpdateStatus(FileStatus.Processing, "Benenne um...");

                    var success = _renameService.RenameFile(fileItem, IsSimulationMode);
                    
                    if (!success && fileItem.Status != FileStatus.Error)
                    {
                        fileItem.UpdateStatus(FileStatus.Error, "Umbenennung fehlgeschlagen");
                    }
                }
                catch (Exception ex)
                {
                    fileItem.UpdateStatus(FileStatus.Error, ex.Message);
                }
            });
        }

        private async Task ReadMetadataOnly(PdfFileItem fileItem)
        {
            await Task.Run(() =>
            {
                try
                {
                    fileItem.UpdateStatus(FileStatus.ReadingMetadata, "Lese Metadaten...");
                    
                    var title = _metadataService.ExtractTitle(fileItem.OriginalFilePath);
                    fileItem.PdfTitle = title;
                    fileItem.NewFileName = title;

                    fileItem.UpdateStatus(FileStatus.Ready, "Bereit zum Bearbeiten");
                }
                catch (Exception ex)
                {
                    fileItem.UpdateStatus(FileStatus.Error, ex.Message);
                }
            });
        }

        [RelayCommand]
        private async Task ProcessSelectedFiles()
        {
            if (IsProcessing) 
                return;

            var selectedFiles = PdfFiles.Where(f => f.IsSelected && f.Status == FileStatus.Ready).ToList();
            
            if (!selectedFiles.Any())
            {
                MessageBox.Show("Keine Dateien zum Verarbeiten ausgewählt oder bereit.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            
            IsProcessing = true;
            TotalFiles = selectedFiles.Count;
            ProcessedFiles = 0;
            StatusText = $"Benenne {TotalFiles} Dateien um...";

            try
            {
                foreach (var fileItem in selectedFiles)
                {
                    await Task.Run(() =>
                    {
                        fileItem.UpdateStatus(FileStatus.Processing, "Benenne um...");
                        
                        var success = _renameService.RenameFile(fileItem, IsSimulationMode);
                        
                        if (!success && fileItem.Status != FileStatus.Error)
                        {
                            fileItem.UpdateStatus(FileStatus.Error, "Umbenennung fehlgeschlagen");
                        }
                    });

                    ProcessedFiles++;
                }

                var successCount = selectedFiles.Count(f => f.Status == FileStatus.Success);
                var skippedCount = selectedFiles.Count(f => f.Status == FileStatus.Skipped);
                var errorCount = selectedFiles.Count(f => f.Status == FileStatus.Error);
                
                StatusText = $"Umbenennung abgeschlossen. {successCount} erfolgreich, {skippedCount} übersprungen, {errorCount} Fehler.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fehler beim Umbenennen: {ex.Message}", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                StatusText = "Umbenennung mit Fehlern abgeschlossen.";
            }
            finally
            {
                IsProcessing = false;
            }
        }

        [RelayCommand]
        private void ClearList()
        {
            if (IsProcessing)
            {
                MessageBox.Show("Kann Liste nicht löschen während der Verarbeitung.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            PdfFiles.Clear();
            StatusText = "Liste geleert";
            TotalFiles = 0;
            ProcessedFiles = 0;
        }

        [RelayCommand]
        private void RemoveFile(PdfFileItem fileItem)
        {
            if (IsProcessing) return;
            
            PdfFiles.Remove(fileItem);
            StatusText = $"Datei entfernt: {fileItem.OriginalFileName}";
        }

        [RelayCommand]
        private void EditFileName(PdfFileItem fileItem)
        {
            var dialog = new Views.EditFileNameDialog(fileItem.NewFileName);
            if (dialog.ShowDialog() == true)
            {
                var newFileName = dialog.NewFileName;
                
                if (_renameService.ValidateNewFileName(newFileName))
                {
                    fileItem.NewFileName = newFileName;
                    
                    // Reset status to Ready if it was an error, so it can be processed again
                    if (fileItem.Status == FileStatus.Error || fileItem.Status == FileStatus.Success)
                    {
                        fileItem.UpdateStatus(FileStatus.Ready, "Bereit zum Bearbeiten");
                    }
                    
                    StatusText = $"Dateiname geändert: {fileItem.OriginalFileName} → {newFileName}";
                }
                else
                {
                    MessageBox.Show("Ungültiger Dateiname. Bitte verwenden Sie keine ungültigen Zeichen.", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        partial void OnSelectedModeChanged(ProcessingMode value)
        {
            OnPropertyChanged(nameof(ModeDisplayName));
            OnPropertyChanged(nameof(ShowEditButtons));
            OnPropertyChanged(nameof(ShowProcessButton));
            OnPropertyChanged(nameof(IsSimulationMode));

            // Update CanEdit property for all items
            foreach (var item in PdfFiles)
            {
                item.CanEdit = ShowEditButtons;
            }
        }
    }
}
