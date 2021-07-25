using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace ImGui.Wpf
{
    public static class ImToggleButtonExtension
    {
        public static async Task<bool> ToggleButton(this ImGuiWpf imGui, string text, bool isChecked)
        {
            var toggleButton = await imGui.HandleControl<Controls.ImToggleButton>(new object[] { text, isChecked });
            return toggleButton.GetState<bool>("Checked");
        }
    }
}

namespace ImGui.Wpf.Controls
{
    internal class ImToggleButton : IImGuiControl
    {
        private readonly ToggleButton m_button;
        public FrameworkElement WindowsControl => m_button;

        private bool? m_isChecked;
        private bool? m_lastKnownChecked;

        public ImToggleButton()
        {
            m_button = new ToggleButton();
            m_button.Checked += OnChecked;
            m_button.Unchecked += OnUnchecked;
        }

        private void OnChecked(object sender, RoutedEventArgs e)
        {
            m_isChecked = true;
        }

        private void OnUnchecked(object sender, RoutedEventArgs e)
        {
            m_isChecked = false;
        }

        public void Update(object[] data)
        {
            m_button.Content = (string)data[0];

            var checkState = (bool?)data[1];
            if (checkState == m_lastKnownChecked)
            {
                return;
            }

            m_lastKnownChecked = m_isChecked = checkState;
            m_button.IsChecked = m_isChecked;
        }

        public void ApplyStyle(IImGuiStyle style)
        {
            m_button.Padding = style.Padding;
            m_button.Margin = style.Margin;
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
