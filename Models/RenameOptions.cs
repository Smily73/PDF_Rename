using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace PDFRename.Models
{
    public partial class RenameOptions : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<WordReplacement> wordReplacements = new();

        [ObservableProperty]
        private string prefixText = "Make - ";

        [ObservableProperty]
        private bool enablePrefixReplacement = true;

        [ObservableProperty]
        private string prefixSearchPattern = "Make ";

        public RenameOptions()
        {
            // Standard-Ersetzungen hinzufügen
            WordReplacements.Add(new WordReplacement { From = "_", To = " " });
            WordReplacements.Add(new WordReplacement { From = "Volume", To = "Vol." });
            WordReplacements.Add(new WordReplacement { From = "Chapter", To = "Ch." });
            WordReplacements.Add(new WordReplacement { From = "Edition", To = "Ed." });
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
    }
}
