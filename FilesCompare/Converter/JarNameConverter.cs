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
        /// <summary>
        /// 0:比对目录 1:所属压缩文件名 2:文件全名 3文件名
        /// </summary>
        /// <param name="values"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                if (string.IsNullOrEmpty(values[3].ToString()))
                    return string.Empty;

                string jarName = values[1].ToString().Replace(values[0].ToString(), "..");
                if (!string.IsNullOrEmpty(jarName))
                    return "[" + jarName + "]";

                string pathName = values[2].ToString().Replace(values[0].ToString(), "..").Replace(values[3].ToString(), "");
                if (pathName.Contains(".jar"))
                    return pathName = "[" + pathName.Substring(0, pathName.IndexOf(".jar") + ".jar".Length) + "]";
                if (pathName.Contains(".zip"))
                    return pathName = "[" + pathName.Substring(0, pathName.IndexOf(".zip") + ".zip".Length) + "]";
                return pathName;
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
