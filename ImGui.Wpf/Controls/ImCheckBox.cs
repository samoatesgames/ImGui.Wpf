using System.Windows;
using System.Windows.Controls;

namespace ImGui.Wpf.Controls
{
    internal class ImCheckBox : IImGuiControl
    {
        private readonly CheckBox m_checkBox;
        public FrameworkElement WindowsControl => m_checkBox;

        private bool? m_isChecked;
        private bool? m_lastKnownChecked;

        public ImCheckBox()
        {
            m_checkBox = new CheckBox
            {
                IsThreeState = false,
                VerticalContentAlignment = VerticalAlignment.Center
            };

            m_checkBox.Checked += OnChecked;
            m_checkBox.Unchecked += OnUnchecked;
        }

        private void OnUnchecked(object sender, RoutedEventArgs e)
        {
            m_isChecked = false;
        }

        private void OnChecked(object sender, RoutedEventArgs e)
        {
            m_isChecked = true;
        }

        public void Update(object[] data)
        {
            m_checkBox.Content = (string)data[0];

            var checkState = (bool?) data[1];
            if (checkState == m_lastKnownChecked)
            {
                return;
            }

            m_lastKnownChecked = m_isChecked = checkState;
            m_checkBox.IsChecked = m_isChecked;
        }

        public void ApplyStyle(IImGuiStyle style)
        {
            m_checkBox.Padding = style.Padding;
            m_checkBox.Margin = style.Margin;
        }

        public TResult GetState<TResult>(string stateName)
        {
            if (stateName == "Checked")
            {
                m_lastKnownChecked = m_isChecked;
                return ImGuiWpfExtensions.Cast<TResult>(m_isChecked);
            }

            return default(TResult);
        }
    }
}
