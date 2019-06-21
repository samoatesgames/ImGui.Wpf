using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ImGui.Wpf.Controls
{
    internal class ImListBox : IImGuiControl
    {
        private readonly DockPanel m_dockPanel;
        private readonly TextBlock m_label;
        private readonly ListBox m_listBox;

        public FrameworkElement WindowsControl => m_dockPanel;

        private object[] m_knownItems;

        private object m_lastKnownSelected;
        private object m_selected;

        public ImListBox()
        {
            m_dockPanel = new DockPanel();

            m_label = new TextBlock();

            m_listBox = new ListBox();
            m_listBox.SelectionChanged += OnSelectionChanged;

            m_dockPanel.Children.Add(m_label);
            m_dockPanel.Children.Add(m_listBox);
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            m_selected = m_listBox.SelectedItem;
        }

        private int GetSelectedIndex(object item)
        {
            if (m_knownItems == null)
            {
                return 0;
            }

            var index = 0;
            foreach (var knownItem in m_knownItems)
            {
                if (knownItem.Equals(item))
                {
                    return index;
                }
                index++;
            }

            return 0;
        }

        public void Update(object[] data)
        {
            var title = (string)data[0];
            var selected = data[1];
            var items = (object[])data[2];

            m_label.Text = title;

            var itemsChanged = m_knownItems?.SequenceEqual(items) != true;
            if (itemsChanged)
            {
                m_listBox.Items.Clear();
                foreach (var item in items)
                {
                    m_listBox.Items.Add(item);
                }

                m_knownItems = items;
            }

            if (m_lastKnownSelected != selected || itemsChanged)
            {
                m_lastKnownSelected = m_selected = m_knownItems[GetSelectedIndex(selected)];
                m_listBox.SelectedItem = m_selected;
            }
        }

        public void ApplyStyle(IImGuiStyle style)
        {
            m_label.Padding = style.Padding;
            m_label.Margin = style.Margin;
            m_listBox.Padding = style.Padding;
            m_listBox.Margin = style.Margin;
        }

        public TResult GetState<TResult>(string stateName)
        {
            if (stateName == "Selected")
            {
                m_lastKnownSelected = m_selected;
                return ImGuiWpfExtensions.Cast<TResult>(m_selected);
            }

            return default(TResult);
        }
    }
}
