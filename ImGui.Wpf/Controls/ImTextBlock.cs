using System.Windows;
using System.Windows.Controls;

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

        public void Update(IImGuiStyle style, object[] data)
        {
            m_textBlock.Padding = style.Padding;
            m_textBlock.Margin = style.Margin;

            m_textBlock.Text = string.Format((string)data[0], (object[])data[1]);
        }

        public TResult GetState<TResult>(string stateName)
        {
            return default(TResult);
        }
    }
}
