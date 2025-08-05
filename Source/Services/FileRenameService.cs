using PDFRename.Models;
using System.IO;
using System.Text.RegularExpressions;

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

        public string ApplyRenameOptions(string originalTitle, RenameOptions options)
        {
            if (string.IsNullOrWhiteSpace(originalTitle))
                return originalTitle;

            string processedTitle = originalTitle;

            // Wort-Ersetzungen anwenden
            foreach (var replacement in options.WordReplacements)
            {
                if (!string.IsNullOrEmpty(replacement.From) && replacement.To != null)
                {
                    // Case-insensitive replacement - verwende replacement.To direkt ohne Trim
                    processedTitle = Regex.Replace(processedTitle, 
                        Regex.Escape(replacement.From), 
                        replacement.To, 
                        RegexOptions.IgnoreCase);
                }
            }

            // Prefix-Ersetzung anwenden
            if (options.EnablePrefixReplacement && !string.IsNullOrEmpty(options.PrefixText))
            {
                processedTitle = ApplyPrefixReplacement(processedTitle, options);
            }

            return processedTitle;
        }

        private string ApplyPrefixReplacement(string title, RenameOptions options)
        {
            // WICHTIG: Kein Trim() verwenden, um Leerzeichen zu erhalten!
            var prefixText = options.PrefixText ?? string.Empty;
            var searchPattern = options.PrefixSearchPattern ?? string.Empty;

            // Wenn der Titel bereits mit dem gewünschten Prefix beginnt, nichts ändern
            if (title.StartsWith(prefixText, StringComparison.OrdinalIgnoreCase))
            {
                return title;
            }

            // Wenn der Titel mit dem Suchmuster beginnt, ersetzen
            if (!string.IsNullOrEmpty(searchPattern) && 
                title.StartsWith(searchPattern, StringComparison.OrdinalIgnoreCase))
            {
                return prefixText + title.Substring(searchPattern.Length);
            }

            // Ansonsten den Prefix voranstellen
            return prefixText + title;
        }
    }
}
