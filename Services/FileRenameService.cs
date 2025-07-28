using PDFRename.Models;
using System.IO;

namespace PDFRename.Services
{
    public class FileRenameService
    {
        public bool RenameFile(PdfFileItem fileItem, bool isSimulation = false)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileItem.NewFileName))
                {
                    fileItem.UpdateStatus(FileStatus.Error, "Neuer Dateiname ist leer");
                    return false;
                }

                // Check if new filename is the same as original
                if (string.Equals(fileItem.OriginalFileName, fileItem.NewFileName, StringComparison.OrdinalIgnoreCase))
                {
                    fileItem.UpdateStatus(FileStatus.Skipped, "Dateiname ist bereits korrekt");
                    return true;
                }

                var newFilePath = fileItem.NewFilePath;

                // Check if target file already exists
                if (File.Exists(newFilePath) && !string.Equals(fileItem.OriginalFilePath, newFilePath, StringComparison.OrdinalIgnoreCase))
                {
                    fileItem.UpdateStatus(FileStatus.Error, "Zieldatei existiert bereits");
                    return false;
                }

                // Check if source file exists
                if (!File.Exists(fileItem.OriginalFilePath))
                {
                    fileItem.UpdateStatus(FileStatus.Error, "Quelldatei nicht gefunden");
                    return false;
                }

                if (isSimulation)
                {
                    fileItem.UpdateStatus(FileStatus.Success, "Simulation erfolgreich");
                    return true;
                }

                // Perform actual rename
                File.Move(fileItem.OriginalFilePath, newFilePath);
                
                // Update the original path to the new path
                fileItem.OriginalFilePath = newFilePath;
                fileItem.OriginalFileName = fileItem.NewFileName;
                
                fileItem.UpdateStatus(FileStatus.Success, "Erfolgreich umbenannt");
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                fileItem.UpdateStatus(FileStatus.Error, "Keine Berechtigung zum Umbenennen");
                return false;
            }
            catch (IOException ex)
            {
                fileItem.UpdateStatus(FileStatus.Error, $"IO-Fehler: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                fileItem.UpdateStatus(FileStatus.Error, $"Unbekannter Fehler: {ex.Message}");
                return false;
            }
        }

        public bool ValidateNewFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return false;

            var invalidChars = Path.GetInvalidFileNameChars();
            return !fileName.Any(c => invalidChars.Contains(c));
        }
    }
}
