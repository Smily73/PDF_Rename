using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.IO;
using System.Text.RegularExpressions;

namespace PDFRename.Services
{
    public class PdfMetadataService
    {
        public string ExtractTitle(string filePath)
        {
            try
            {
                using var document = PdfReader.Open(filePath, PdfDocumentOpenMode.Import);
                var title = document.Info.Title;
                
                if (string.IsNullOrWhiteSpace(title))
                {
                    // Fallback: Use filename without extension
                    title = Path.GetFileNameWithoutExtension(filePath);
                }

                return SanitizeFileName(title);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Fehler beim Lesen der PDF-Metadaten: {ex.Message}", ex);
            }
        }

        public PdfMetadata ExtractAllMetadata(string filePath)
        {
            try
            {
                using var document = PdfReader.Open(filePath, PdfDocumentOpenMode.Import);
                var info = document.Info;

                return new PdfMetadata
                {
                    Title = info.Title ?? string.Empty,
                    Author = info.Author ?? string.Empty,
                    Subject = info.Subject ?? string.Empty,
                    Creator = info.Creator ?? string.Empty,
                    Producer = info.Producer ?? string.Empty,
                    CreationDate = info.CreationDate,
                    ModificationDate = info.ModificationDate
                };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Fehler beim Lesen der PDF-Metadaten: {ex.Message}", ex);
            }
        }

        private string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return "Unbenannt";

            // Remove invalid characters
            var invalidChars = Path.GetInvalidFileNameChars();
            foreach (var invalidChar in invalidChars)
            {
                fileName = fileName.Replace(invalidChar, '_');
            }

            // Replace multiple spaces and underscores with single underscore
            fileName = Regex.Replace(fileName, @"[\s_]+", "_");
            
            // Remove leading/trailing underscores and dots
            fileName = fileName.Trim('_', '.', ' ');
            
            // Ensure it's not empty
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = "Unbenannt";

            // Limit length
            if (fileName.Length > 200)
                fileName = fileName.Substring(0, 200);

            return fileName + ".pdf";
        }
    }

    public class PdfMetadata
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Creator { get; set; } = string.Empty;
        public string Producer { get; set; } = string.Empty;
        public DateTime CreationDate { get; set; }
        public DateTime ModificationDate { get; set; }
    }
}
