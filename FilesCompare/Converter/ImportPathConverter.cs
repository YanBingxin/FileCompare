using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace FilesCompare.Converter
{
    class ImportPathConverter : IValueConverter
    {
        #region IValueConverter 成员

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string path = value.ToString();
            if (path.Contains(@"\新\"))
                return path.Substring(path.IndexOf(@"\新\") + @"\新\".Length);
            if (path.Contains(@"\旧\"))
                return path.Substring(path.IndexOf(@"\旧\") + @"\旧\".Length);
            return path;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
