﻿/*
 * 2016年10月11日10:06:56
 * 参考http://blog.csdn.net/fwj380891124/article/details/8018100的博客
 * 改进了无法正常使用的bug
 * 1.修复了鼠标消息错误映射到标题栏，导致正常消息被捕获bug
 * 2.增加标题栏高度，右边距，可以正确映射标题栏
 * 3.修复了窗口最大化后无法正常使用的bug
 * 4.整合为自定义窗口类，可直接继承使用。
 * 2016年10月11日10:14:51
 * ybx
 * */
using System;
using System.Windows;
using System.Windows.Interop;

namespace BxWindow
{
    public abstract class BxWindow : Window
    {
        private const int WM_NCHITTEST = 0x0084;
        private readonly int agWidth = 8; //拐角宽度  
        private readonly int bThickness = 2; // 边框宽度  
        private Point mousePoint = new Point(); //鼠标坐标  
        public bool IsWindowStateNormal { get { return this.WindowState == WindowState.Normal; } }//窗口是否最大化

        /// <summary>
        /// 标题栏高度
        /// </summary>
        public abstract double TitleHeight { get; }
        /// <summary>
        /// 标题栏右边距
        /// </summary>
        public abstract double TitleRightMargin { get; }
        #region 边框自定义调整大小
        /// <summary>
        /// 将窗口默认边框隐藏
        /// </summary>
        public BxWindow()
        {
            this.AllowsTransparency = true;
            this.WindowStyle = WindowStyle.None;
        }
        /// <summary>
        /// 加载鼠标钩子
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            if (hwndSource != null)
            {
                hwndSource.AddHook(new HwndSourceHook(this.WndProc));
            }
        }
        
        public enum HitTest : int
        {
            HTERROR = -2,
            HTTRANSPARENT = -1,
            HTNOWHERE = 0,
            HTCLIENT = 1,
            HTCAPTION = 2,
            HTSYSMENU = 3,
            HTGROWBOX = 4,
            HTSIZE = HTGROWBOX,
            HTMENU = 5,
            HTHSCROLL = 6,
            HTVSCROLL = 7,
            HTMINBUTTON = 8,
            HTMAXBUTTON = 9,
            HTLEFT = 10,
            HTRIGHT = 11,
            HTTOP = 12,
            HTTOPLEFT = 13,
            HTTOPRIGHT = 14,
            HTBOTTOM = 15,
            HTBOTTOMLEFT = 16,
            HTBOTTOMRIGHT = 17,
            HTBORDER = 18,
            HTREDUCE = HTMINBUTTON,
            HTZOOM = HTMAXBUTTON,
            HTSIZEFIRST = HTLEFT,
            HTSIZELAST = HTBOTTOMRIGHT,
            HTOBJECT = 19,
            HTCLOSE = 20,
            HTHELP = 21,
        }
        protected virtual IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case WM_NCHITTEST:
                    {
                        this.mousePoint.X = (lParam.ToInt32() & 0xFFFF);
                        this.mousePoint.Y = (lParam.ToInt32() >> 16);

                        #region 定位鼠标位置并映射到相应目标功能位置
                        // 窗口左上角  
                        if (this.mousePoint.Y - this.Top <= this.agWidth
                                         && this.mousePoint.X - this.Left <= this.agWidth && IsWindowStateNormal)
                        {
                            handled = true;
                            return new IntPtr((int)HitTest.HTTOPLEFT);
                        }
                        // 窗口左下角　　  
                        else if (this.ActualHeight + this.Top - this.mousePoint.Y <= this.agWidth
                                         && this.mousePoint.X - this.Left <= this.agWidth && IsWindowStateNormal)
                        {
                            handled = true;
                            return new IntPtr((int)HitTest.HTBOTTOMLEFT);
                        }
                        // 窗口右上角  
                        else if (this.mousePoint.Y - this.Top <= this.agWidth
                         && this.ActualWidth + this.Left - this.mousePoint.X <= this.agWidth && IsWindowStateNormal)
                        {
                            handled = true;
                            return new IntPtr((int)HitTest.HTTOPRIGHT);
                        }
                        // 窗口右下角  
                        else if (this.ActualWidth + this.Left - this.mousePoint.X <= this.agWidth
                         && this.ActualHeight + this.Top - this.mousePoint.Y <= this.agWidth && IsWindowStateNormal)
                        {
                            handled = true;
                            return new IntPtr((int)HitTest.HTBOTTOMRIGHT);
                        }
                        // 窗口左侧  
                        else if (this.mousePoint.X - this.Left <= this.bThickness && IsWindowStateNormal)
                        {
                            handled = true;
                            return new IntPtr((int)HitTest.HTLEFT);
                        }
                        // 窗口右侧  
                        else if (this.ActualWidth + this.Left - this.mousePoint.X <= this.bThickness && IsWindowStateNormal)
                        {
                            handled = true;
                            return new IntPtr((int)HitTest.HTRIGHT);
                        }
                        // 窗口上方  
                        else if (this.mousePoint.Y - this.Top <= this.bThickness && IsWindowStateNormal)
                        {
                            handled = true;
                            return new IntPtr((int)HitTest.HTTOP);
                        }
                        // 窗口下方  
                        else if (this.ActualHeight + this.Top - this.mousePoint.Y <= this.bThickness && IsWindowStateNormal)
                        {
                            handled = true;
                            return new IntPtr((int)HitTest.HTBOTTOM);
                        }//标题栏位置操作映射
                        else if (!IsWindowStateNormal && this.mousePoint.Y < this.TitleHeight && this.mousePoint.X < this.ActualWidth - this.TitleRightMargin || IsWindowStateNormal && this.mousePoint.Y > this.Top && this.mousePoint.Y < this.Top + this.TitleHeight && this.mousePoint.X < this.Left + this.ActualWidth - this.TitleRightMargin)
                        {
                            handled = true;
                            return new IntPtr((int)HitTest.HTCAPTION);
                        }
                        else //其余消息不处理
                        {
                            return IntPtr.Zero;
                        }
                        #endregion
                    }
                default:
                    return IntPtr.Zero;

            }
        }
        #endregion
    }
}
