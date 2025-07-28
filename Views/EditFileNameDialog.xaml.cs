using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using PDFRename.ViewModels;

namespace PDFRename.Views
{
    public partial class EditFileNameDialog : Window
    {
        private EditFileNameViewModel _viewModel;

        public string NewFileName => _viewModel?.NewFileName ?? string.Empty;

        public EditFileNameDialog(string currentFileName)
        {
            InitializeComponent();
            
            _viewModel = new EditFileNameViewModel(currentFileName);
            DataContext = _viewModel;
            
            // Subscribe to ViewModel property changes
            _viewModel.PropertyChanged += ViewModel_PropertyChanged;
            
            // Set owner to main window for proper centering
            if (Application.Current.MainWindow != null)
            {
                this.Owner = Application.Current.MainWindow;
            }
            
            // Focus and select text after window is loaded
            this.Loaded += (s, e) =>
            {
                FileNameTextBox.Focus();
                FileNameTextBox.SelectAll();
            };
        }

        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(EditFileNameViewModel.DialogResult))
            {
                if (_viewModel.DialogResult.HasValue)
                {
                    DialogResult = _viewModel.DialogResult;
                }
            }
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 1)
            {
                this.DragMove();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void FileNameTextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                _viewModel.AcceptCommand.Execute(null);
            }
            else if (e.Key == System.Windows.Input.Key.Escape)
            {
                _viewModel.CancelCommand.Execute(null);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= ViewModel_PropertyChanged;
            }
            base.OnClosed(e);
        }
    }
}
