using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace SpaApp
{
    // Конвертер для текста кнопки сравнения
    public class BoolToComparisonTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isInComparison)
            {
                return isInComparison ? "Удалить из сравнения" : "Добавить к сравнению";
            }
            return "Добавить к сравнению";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Конвертер для цвета кнопки сравнения
    public class BoolToComparisonColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isInComparison)
            {
                return isInComparison ? new SolidColorBrush(Colors.OrangeRed) : new SolidColorBrush(Color.FromRgb(0x2D, 0x5A, 0xA1));
            }
            return new SolidColorBrush(Color.FromRgb(0x2D, 0x5A, 0xA1));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}