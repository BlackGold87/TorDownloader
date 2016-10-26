using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace NewTor.Converter
{
    public class FileSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                var byteCount = System.Convert.ToInt64(value);
                string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" };

                if (byteCount == 0)
                    return "0" + suf[0];

                var bytes = Math.Abs(byteCount);
                var place = System.Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
                var num = Math.Round(bytes / Math.Pow(1024, place), 1);

                return (Math.Sign(byteCount) * num) + suf[place];
            }
            catch { }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
