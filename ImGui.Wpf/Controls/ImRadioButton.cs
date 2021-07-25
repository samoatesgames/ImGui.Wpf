using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ImGui.Wpf
{
    public static class ImRadioButtonExtension
    {
        public static async Task<bool> RadioButton(this ImGuiWpf imGui, string title, bool isChecked)
        {
            var radioButton = await imGui.HandleControl<Controls.ImRadioButton>(new object[] { title, isChecked });
            return radioButton.GetState<bool>("Checked");
        }
    }
}

namespace ImGui.Wpf.Controls
{
    internal class ImRadioButton : IImGuiControl
    {
        private readonly RadioButton m_radioButton;
        public FrameworkElement WindowsControl => m_radioButton;

        private bool? m_isChecked;
        private bool? m_lastKnownChecked;

        public ImRadioButton()
        {
            m_radioButton = new RadioButton
            {
                IsThreeState = false,
                VerticalContentAlignment = VerticalAlignment.Center
            };

            m_radioButton.Checked += OnChecked;
            m_radioButton.Unchecked += OnUnchecked;
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
            m_radioButton.Content = (string)data[0];

            var checkState = (bool?)data[1];
            if (checkState == m_lastKnownChecked)
            {
                return;
            }

            m_lastKnownChecked = m_isChecked = checkState;
            m_radioButton.IsChecked = m_isChecked;
        }

        public void ApplyStyle(IImGuiStyle style)
        {
            m_radioButton.Padding = style.Padding;
            m_radioButton.Margin = style.Margin;
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
