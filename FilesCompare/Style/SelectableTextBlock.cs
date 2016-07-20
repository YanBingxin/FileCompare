﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Documents;

namespace FilesCompare.Style
{
    class SelectableTextBlock : TextBlock
    {
        TextPointer startpoz;
        TextPointer endpoz;
        MenuItem copyMenu;
        MenuItem selectAllMenu;

        public TextRange Selection { get; private set; }
        public bool HasSelection
        {
            get { return Selection != null && !Selection.IsEmpty; }
        }

        #region SelectionBrush

        public static readonly DependencyProperty SelectionBrushProperty =
            DependencyProperty.Register("SelectionBrush", typeof(Brush), typeof(SelectableTextBlock),
                new FrameworkPropertyMetadata(Brushes.Orange));

        public Brush SelectionBrush
        {
            get { return (Brush)GetValue(SelectionBrushProperty); }
            set { SetValue(SelectionBrushProperty, value); }
        }

        #endregion



        public SelectableTextBlock()
        {
            Focusable = true;
            var contextMenu = new ContextMenu();
            ContextMenu = contextMenu;

            copyMenu = new MenuItem();
            copyMenu.Header = "复制";
            copyMenu.InputGestureText = "Ctrl + C";
            copyMenu.Click += (ss, ee) =>
                {
                    Copy();
                };
            contextMenu.Items.Add(copyMenu);

            selectAllMenu = new MenuItem();
            selectAllMenu.Header = "全选";
            selectAllMenu.InputGestureText = "Ctrl + A";
            selectAllMenu.Click += (ss, ee) =>
                {
                    SelectAll();
                };
            contextMenu.Items.Add(selectAllMenu);

            ContextMenuOpening += contextMenu_ContextMenuOpening;
        }

        void contextMenu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            copyMenu.IsEnabled = HasSelection;
        }
        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            Keyboard.Focus(this);
            ReleaseMouseCapture();
            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            var point = e.GetPosition(this);
            startpoz = GetPositionFromPoint(point, true);
            CaptureMouse();
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (IsMouseCaptured)
            {
                var point = e.GetPosition(this);
                endpoz = GetPositionFromPoint(point, true);

                ClearSelection();
                Selection = new TextRange(startpoz, endpoz);
                Selection.ApplyPropertyValue(TextElement.BackgroundProperty, SelectionBrush);
                CommandManager.InvalidateRequerySuggested();

                OnSelectionChanged(EventArgs.Empty);
            }

            base.OnMouseMove(e);
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            if (HasSelection)
                Selection.ApplyPropertyValue(TextElement.BackgroundProperty, Brushes.Transparent);
            base.OnLostFocus(e);
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (e.Key == Key.C)
                    Copy();
                else if (e.Key == Key.A)
                    SelectAll();
            }

            base.OnKeyUp(e);
        }

        public bool Copy()
        {
            if (HasSelection)
            {
                Clipboard.SetDataObject(Selection.Text);
                return true;
            }
            return false;
        }

        public void ClearSelection()
        {
            var contentRange = new TextRange(ContentStart, ContentEnd);
            contentRange.ApplyPropertyValue(TextElement.BackgroundProperty, null);
            Selection = null;
        }

        public void SelectAll()
        {
            Selection = new TextRange(ContentStart, ContentEnd);
            Selection.ApplyPropertyValue(TextElement.BackgroundProperty, SelectionBrush);
        }

        public event EventHandler SelectionChanged;

        protected virtual void OnSelectionChanged(EventArgs e)
        {
            var handler = this.SelectionChanged;
            if (handler != null)
                handler(this, e);
        }




    }
}
