using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace ImGui.Wpf.Controls
{
    internal class ImComboBox : IImGuiControl
    {
        private readonly DockPanel m_dockPanel;
        private readonly TextBlock m_label;
        private readonly ComboBox m_comboBox;

        public FrameworkElement WindowsControl => m_dockPanel;

        private object[] m_knownItems;

        private object m_lastKnownSelected;
        private object m_selected;

        public ImComboBox()
        {
            m_dockPanel = new DockPanel();

            m_label = new TextBlock();

            m_comboBox = new ComboBox();
            m_comboBox.SelectionChanged += OnSelectionChanged;

            m_dockPanel.Children.Add(m_label);
            m_dockPanel.Children.Add(m_comboBox);
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            m_selected = m_comboBox.SelectedItem;
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
            var title = (string) data[0];
            var selected = data[1];
            var items = (object[])data[2];

            m_label.Text = title;

            var itemsChanged = m_knownItems?.SequenceEqual(items) != true;
            if (itemsChanged)
            {
                m_comboBox.Items.Clear();
                foreach (var item in items)
                {
                    m_comboBox.Items.Add(item);
                }

                m_knownItems = items;
            }

            if (m_lastKnownSelected != selected || itemsChanged)
            {
                m_lastKnownSelected = m_selected = m_knownItems[GetSelectedIndex(selected)];
                m_comboBox.SelectedItem = m_selected;
            }
        }

        public void ApplyStyle(IImGuiStyle style)
        {
            m_label.Padding = style.Padding;
            m_label.Margin = style.Margin;
            m_comboBox.Padding = style.Padding;
            m_comboBox.Margin = style.Margin;
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
