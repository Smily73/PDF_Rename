using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using PDFRename.Models;

namespace PDFRename.Services
{
    public interface IOptionsStorageService
    {
        RenameOptions LoadOptions();
        void SaveOptions(RenameOptions options);
        string GetOptionsFilePath();
    }

    public class OptionsStorageService : IOptionsStorageService
    {
        private const string OptionsFileName = "PDFRename.options.json";
        private readonly string _optionsFilePath;
        private readonly JsonSerializerOptions _jsonOptions;

        public OptionsStorageService()
        {
            // Try application directory first
            string appDirectory = AppContext.BaseDirectory;
            string appDirectoryPath = Path.Combine(appDirectory, OptionsFileName);

            if (IsDirectoryWritable(appDirectory))
            {
                _optionsFilePath = appDirectoryPath;
            }
            else
            {
                // Fallback to user directory
                string userDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string appDataDirectory = Path.Combine(userDirectory, "PDFRename");

                // Create directory if it doesn't exist
                if (!Directory.Exists(appDataDirectory))
                {
                    Directory.CreateDirectory(appDataDirectory);
                }

                _optionsFilePath = Path.Combine(appDataDirectory, OptionsFileName);
            }

            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };
        }

        public string GetOptionsFilePath() => _optionsFilePath;

        public RenameOptions LoadOptions()
        {
            try
            {
                if (File.Exists(_optionsFilePath))
                {
                    string json = File.ReadAllText(_optionsFilePath);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        var options = JsonSerializer.Deserialize<RenameOptions>(json, _jsonOptions);
                        if (options != null && options.WordReplacements != null)
                        {
                            return options;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error when a debugger is attached
                System.Diagnostics.Debug.WriteLine($"Error loading options: {ex.Message}");

                // Delete corrupted file on severe error
                try
                {
                    if (File.Exists(_optionsFilePath))
                    {
                        File.Delete(_optionsFilePath);
                    }
                }
                catch
                {
                    // Ignore deletion errors
                }
            }

            return CreateDefaultOptions();
        }

        public void SaveOptions(RenameOptions options)
        {
            try
            {
                string json = JsonSerializer.Serialize(options, _jsonOptions);
                File.WriteAllText(_optionsFilePath, json);
            }
            catch (Exception ex)
            {
                // Log error when a debugger is attached
                System.Diagnostics.Debug.WriteLine($"Error saving options: {ex.Message}");
            }
        }

        private static bool IsDirectoryWritable(string directoryPath)
        {
            try
            {
                string testFile = Path.Combine(directoryPath, Path.GetRandomFileName());
                File.WriteAllText(testFile, "test");
                File.Delete(testFile);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static RenameOptions CreateDefaultOptions()
        {
            return new RenameOptions
            {
                WordReplacements = new ObservableCollection<WordReplacement>
                {
                    new() { From = "ä", To = "ae" },
                    new() { From = "ö", To = "oe" },
                    new() { From = "ü", To = "ue" },
                    new() { From = "ß", To = "ss" },
                    new() { From = "Ä", To = "Ae" },
                    new() { From = "Ö", To = "Oe" },
                    new() { From = "Ü", To = "Ue" }
                },
                EnablePrefixReplacement = false,
                PrefixText = "Make",
                PrefixSearchPattern = "Make"
            };
        }
        
    }
}
