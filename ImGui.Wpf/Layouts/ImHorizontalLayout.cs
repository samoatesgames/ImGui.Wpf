using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ImGui.Wpf.Layouts
{
    public class ImHorizontalLayout : ImLayout
    {
        private readonly Panel m_panel;
        private readonly TypeConverter m_gridLengthConverter = TypeDescriptor.GetConverter(typeof(GridLength));
        private readonly List<Tuple<string, GridLength>> m_columnDefinitions = new List<Tuple<string, GridLength>>();

        public ImHorizontalLayout(Action onDispose) : base(onDispose)
        {
            m_panel = new UniformGrid
            {
                Rows = 1
            };
        }

        public ImHorizontalLayout(string[] columnWidths, Action onDispose) : base(onDispose)
        {
            if (columnWidths == null || columnWidths.Length == 0)
            {
                m_panel = new UniformGrid
                {
                    Rows = 1
                };
                return;
            }

            var fallbackGridLength = (GridLength)m_gridLengthConverter.ConvertFromString("*");

            var grid = new Grid();
            foreach (var columnWidth in columnWidths)
            {
                var gridLength = fallbackGridLength;
                try
                {
                    gridLength = (GridLength)m_gridLengthConverter.ConvertFromString(columnWidth);
                }
                catch { /* ignore */ }

                m_columnDefinitions.Add(new Tuple<string, GridLength>(columnWidth, gridLength));

                grid.ColumnDefinitions.Add(new ColumnDefinition
                {
                    Width = gridLength
                });
            }
            m_panel = grid;
        }

        internal override Panel GetPanel()
        {
            return m_panel;
        }

        public override void AddChild(IImGuiControl control)
        {
            if (m_panel is UniformGrid)
            {
                m_panel.Children.Add(control.WindowsControl);
                return;
            }

            if (!(m_panel is Grid grid))
            {
                return;
            }

            var columnIndex = m_panel.Children.Count;
            grid.Children.Add(control.WindowsControl);
            Grid.SetColumn(control.WindowsControl, columnIndex);
        }

        public override void AddChild(ImLayout layout)
        {
            var newPanel = layout.GetPanel();
            if (m_panel is UniformGrid)
            {
                m_panel.Children.Add(newPanel);
                return;
            }

            if (!(m_panel is Grid grid))
            {
                return;
            }

            var columnIndex = m_panel.Children.Count;
            grid.Children.Add(newPanel);
            Grid.SetColumn(newPanel, columnIndex);
        }

        public bool HasColumnWidths(string[] columnWidths)
        {
            if (columnWidths == null || columnWidths.Length == 0)
            {
                return m_panel is UniformGrid;
            }

            if (!(m_panel is Grid grid))
            {
                return false;
            }

            if (m_columnDefinitions.Count != columnWidths.Length)
            {
                return false;
            }

            for (var columnIndex = 0; columnIndex < grid.ColumnDefinitions.Count; ++columnIndex)
            {
                var columnDef = m_columnDefinitions[columnIndex];
                var newColumnWidth = columnWidths[columnIndex];
                if (columnDef.Item1 != newColumnWidth)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
