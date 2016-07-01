using FilesCompare.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using FilesCompare.CompareHelper;

namespace FilesCompare.Model
{

    /// <summary>
    /// ID:文件节点
    /// Describe:文件节点
    /// Author:ybx
    /// Date:2016-6-17 17:57:22
    /// </summary>
    public class FNode : NotifyObject
    {
        private string _fFullName = string.Empty;
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FFullName
        {
            get
            {
                return _fFullName;
            }
            set
            {
                _fFullName = value;
                this.RaisePropertyChanged("FFullName");
            }
        }

        private string _fName = string.Empty;
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FName
        {
            get
            {
                return _fName;
            }
            set
            {
                _fName = value;
                this.RaisePropertyChanged("FName");
            }
        }

        private string _fMD5 = string.Empty;
        /// <summary>
        /// MD5
        /// </summary>
        public string FMD5
        {
            get
            {
                //return Child == null ? MD5Helper.GetMD5(FFullName) : string.Empty;
                return MD5Helper.GetMD5(FFullName);
            }
            //get
            //{
            //    return _fMD5;
            //}
            //set
            //{
            //    _fMD5 = value;
            //    this.RaisePropertyChanged("FMD5");
            //}
        }

        /// <summary>
        /// 是否为文件
        /// </summary>
        private bool? _isFile = false;
        /// <summary>
        /// 获取或设置是否为文件
        /// </summary>
        public bool? IsFile
        {
            get
            {
                return _isFile;
            }
            set
            {
                _isFile = value;
                RaisePropertyChanged("IsFile");
            }
        }

        /// <summary>
        /// 差异类型 修改：true；多,少：false
        /// </summary>
        private bool? _difTag = null;
        /// <summary>
        /// 获取或设置是否为文件
        /// </summary>
        public bool? DifTag
        {
            get
            {
                return _difTag;
            }
            set
            {
                _difTag = value;
                RaisePropertyChanged("DifTag");
            }
        }

        private ObservableCollection<FNode> _child;
        /// <summary>
        /// 下级点
        /// </summary>
        public ObservableCollection<FNode> Child
        {
            get
            {
                return _child;
            }
            set
            {
                _child = value;
                this.RaisePropertyChanged("Child");
            }
        }
    }
}
