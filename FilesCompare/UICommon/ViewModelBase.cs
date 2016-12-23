using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace NV.DRF.UICommon
{
    /// <summary>
    /// ID:ViewModel基础类
    /// Describe:提供额外的熟悉Set方法
    /// Author:ybx
    /// Date:2016-11-21 16:55:09
    /// </summary>
    public class ViewModelBase : ObservableModel
    {
        /// <summary>
        /// 针对ViewModel的属性Set方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public T Set<T>(Expression<Func<T>> func, ref T property, T value)
        {
            property = value;
            RaisePropertyChanged(func);
            return property;
        }
    }
}
