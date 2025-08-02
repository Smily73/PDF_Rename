using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Text.Json.Serialization;

namespace PDFRename.Models
{
    public partial class RenameOptions : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<WordReplacement> wordReplacements = new();

        [ObservableProperty]
        private string prefixText = "Make - ";

        [ObservableProperty]
        private bool enablePrefixReplacement = false;

        [ObservableProperty]
        private string prefixSearchPattern = "Make ";

        public RenameOptions()
        {
            // Default constructor for object initialization and JSON deserialization
        }

        public RenameOptions Clone()
        {
            var clone = new RenameOptions
            {
                PrefixText = this.PrefixText,
                EnablePrefixReplacement = this.EnablePrefixReplacement,
                PrefixSearchPattern = this.PrefixSearchPattern
            };

            clone.WordReplacements.Clear();
            foreach (var replacement in this.WordReplacements)
            {
                clone.WordReplacements.Add(new WordReplacement 
                { 
                    From = replacement.From, 
                    To = replacement.To 
                });
            }

            return clone;
        }
    }

    public partial class WordReplacement : ObservableObject
    {
        [ObservableProperty]
        private string from = string.Empty;

        [ObservableProperty]
        private string to = string.Empty;

        public WordReplacement()
        {
            // Standard-Konstruktor für Object-Initialisierung und JSON-Deserialisierung
        }
    }
}
