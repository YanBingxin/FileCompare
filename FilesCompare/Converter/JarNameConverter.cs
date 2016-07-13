using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace FilesCompare.Converter
{
    public class JarNameConverter : IMultiValueConverter
    {

        #region IMultiValueConverter 成员
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (string.IsNullOrEmpty(values[3].ToString()))
                {
                    return string.Empty;
                }
                string jarName = values[1].ToString().Replace(values[0].ToString(), "..");
                return string.IsNullOrEmpty(jarName) ? values[2].ToString().Replace(values[0].ToString(), "..").Replace(values[3].ToString(), "") : "[" + jarName + "]";
            }
            catch
            {
                return string.Empty;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
