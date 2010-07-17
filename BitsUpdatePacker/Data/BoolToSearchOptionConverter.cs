using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.IO;

namespace BitsUpdatePacker.Data
{
    [ValueConversion(typeof(object), typeof(Version))]
    public sealed class BoolToSearchOptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (SearchOption)value == SearchOption.AllDirectories;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (bool)value ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
        }
    }
}
