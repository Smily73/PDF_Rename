using PDFRename.Models;
using System.Globalization;
using System.Windows.Data;

namespace PDFRename
{
    public class ProcessingModeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is ProcessingMode mode)
            {
                return mode switch
                {
                    ProcessingMode.AutomaticRename => "Automatisch umbenennen",
                    ProcessingMode.SimulationMode => "Simulations-Modus",
                    ProcessingMode.EditBeforeRename => "Namen vorher bearbeiten",
                    _ => "Unbekannt"
                };
            }
            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
