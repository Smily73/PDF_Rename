using PDFRename.Models;
using PDFRename.ViewModels;
using System.Windows;

namespace PDFRename.Views
{
    public partial class OptionsDialog : Window
    {
        public OptionsViewModel ViewModel { get; }
        public bool WasAccepted { get; private set; }

        public OptionsDialog(RenameOptions options)
        {
            InitializeComponent();
            ViewModel = new OptionsViewModel(options);
            DataContext = ViewModel;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            WasAccepted = true;
            this.DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            WasAccepted = false;
            this.DialogResult = false;
            Close();
        }
    }
}
