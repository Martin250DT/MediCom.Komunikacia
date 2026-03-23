using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MediCom.Komunikacia.Converters
{
    public class BooleanToBrushConverter : IValueConverter
    {
        public Brush UserBrush { get; set; }

        public Brush AssistantBrush { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isUser = value is bool flag && flag;
            return isUser ? UserBrush : AssistantBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
