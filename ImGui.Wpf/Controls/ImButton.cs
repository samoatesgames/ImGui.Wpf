using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ImGui.Wpf
{
    public static class ImButtonExtension
    {
        public static async Task<bool> Button(this ImGuiWpf imGui, string text)
        {
            var button = await imGui.HandleControl<Controls.ImButton>(new object[] { text });
            return button.GetState<bool>("Clicked");
        }
    }
}

namespace ImGui.Wpf.Controls
{
    internal class ImButton : IImGuiControl
    {
        private readonly Button m_button;
        public FrameworkElement WindowsControl => m_button;

        private bool m_isClicked;

        public ImButton()
        {
            m_button = new Button();
            m_button.Click += OnButtonClick;
        }

        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            m_isClicked = true;
        }

        public void Update(object[] data)
        {
            m_button.Content = (string)data[0];
        }

        public void ApplyStyle(IImGuiStyle style)
        {
            m_button.Padding = style.Padding;
            m_button.Margin = style.Margin;
        }

        public TResult GetState<TResult>(string stateName)
        {
            if (stateName == "Clicked")
            {
                if (m_isClicked)
                {
                    m_isClicked = false;
                    return ImGuiWpfExtensions.Cast<TResult>(true);
                }

                return ImGuiWpfExtensions.Cast<TResult>(false);
            }

            return default(TResult);
        }
    }
}
