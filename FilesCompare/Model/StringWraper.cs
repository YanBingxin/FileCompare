using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FilesCompare.Common;

namespace FilesCompare.Model
{
    public class StringWraper : NotifyObject
    {

        /// <summary>
        /// 值
        /// </summary>
        private string _value;
        /// <summary>
        /// 获取或设置值
        /// </summary>
        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value == value)
                {
                    return;
                }
                _value = value;
                RaisePropertyChanged("Value");
            }
        }

    }
}
