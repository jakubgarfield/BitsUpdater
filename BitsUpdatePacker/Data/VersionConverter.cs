using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace BitsUpdatePacker.Data
{
    [ValueConversion(typeof(object), typeof(Version))]
    public sealed class VersionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (value ?? String.Empty).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new Version((string)value);
        }
    }
}
