using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ImGui.Wpf
{
    public static class ImTextBlockExtension
    {
        public static async Task Text(this ImGuiWpf imGui, string message, params object[] args)
        {
            await imGui.TextBlock(message, args);
        }

        public static async Task TextBlock(this ImGuiWpf imGui, string message, params object[] args)
        {
            await imGui.HandleControl<Controls.ImTextBlock>(new object[] { message, args });
        }
    }
}

namespace ImGui.Wpf.Controls
{
    internal class ImTextBlock : IImGuiControl
    {
        private readonly TextBlock m_textBlock;
        public FrameworkElement WindowsControl => m_textBlock;

        public ImTextBlock()
        {
            m_textBlock = new TextBlock
            {
                TextWrapping = TextWrapping.Wrap
            };
        }

        public void Update(object[] data)
        {
            m_textBlock.Text = string.Format((string)data[0], (object[])data[1]);
        }

        public void ApplyStyle(IImGuiStyle style)
        {
            m_textBlock.Padding = style.Padding;
            m_textBlock.Margin = style.Margin;
        }

        public TResult GetState<TResult>(string stateName)
        {
            return default(TResult);
        }
    }
}
