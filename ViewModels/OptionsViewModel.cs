using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PDFRename.Models;
using System.Collections.ObjectModel;

namespace PDFRename.ViewModels
{
    public partial class OptionsViewModel : ObservableObject
    {
        [ObservableProperty]
        private RenameOptions options;

        [ObservableProperty]
        private WordReplacement? selectedReplacement;

        public OptionsViewModel(RenameOptions originalOptions)
        {
            // Kopie der ursprünglichen Optionen erstellen
            Options = originalOptions.Clone();
        }

        [RelayCommand]
        private void AddReplacement()
        {
            var newReplacement = new WordReplacement { From = "", To = "" };
            Options.WordReplacements.Add(newReplacement);
            SelectedReplacement = newReplacement;
        }

        [RelayCommand]
        private void RemoveReplacement(WordReplacement? replacement)
        {
            if (replacement != null && Options.WordReplacements.Contains(replacement))
            {
                Options.WordReplacements.Remove(replacement);
                
                // Nächstes Element auswählen oder null wenn keine Elemente mehr vorhanden
                if (Options.WordReplacements.Count > 0)
                {
                    var index = Math.Min(Options.WordReplacements.Count - 1, 
                                       Options.WordReplacements.Count - 1);
                    SelectedReplacement = Options.WordReplacements[index];
                }
                else
                {
                    SelectedReplacement = null;
                }
            }
        }

        [RelayCommand]
        private void MoveReplacementUp(WordReplacement? replacement)
        {
            if (replacement == null) return;

            var index = Options.WordReplacements.IndexOf(replacement);
            if (index > 0)
            {
                Options.WordReplacements.Move(index, index - 1);
            }
        }

        [RelayCommand]
        private void MoveReplacementDown(WordReplacement? replacement)
        {
            if (replacement == null) return;

            var index = Options.WordReplacements.IndexOf(replacement);
            if (index >= 0 && index < Options.WordReplacements.Count - 1)
            {
                Options.WordReplacements.Move(index, index + 1);
            }
        }

        [RelayCommand]
        private void ResetToDefaults()
        {
            Options.WordReplacements.Clear();
            Options.WordReplacements.Add(new WordReplacement { From = "_", To = " " });
            Options.WordReplacements.Add(new WordReplacement { From = "Volume", To = "Vol." });
            Options.WordReplacements.Add(new WordReplacement { From = "Chapter", To = "Ch." });
            Options.WordReplacements.Add(new WordReplacement { From = "Edition", To = "Ed." });
            
            Options.PrefixText = "Make - ";
            Options.EnablePrefixReplacement = true;
            Options.PrefixSearchPattern = "Make ";
        }
    }
}
