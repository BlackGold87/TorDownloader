using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace NewTor.Converter
{
    public class PercentToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return string.Format("{0:0.00}%", (System.Convert.ToDouble(value) * 100));
            }
            catch { }
            return "0.00%";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string val = value.ToString().Trim('%');
                return System.Convert.ToDouble(val) / 100;
            }
            catch { }
            return 0;
        }
    }

    public class PercentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return System.Convert.ToDouble(value) * 100;
            }
            catch { }
            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return System.Convert.ToInt64(value) / 100;
            }
            catch { }
            return 0;
        }
    }
}
