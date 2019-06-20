using System.Windows;
using System.Windows.Controls;

namespace ImGui.Wpf.Controls
{
    internal class ImLabel : IImGuiControl
    {
        private readonly Label m_label;
        public FrameworkElement WindowsControl => m_label;

        public ImLabel()
        {
            m_label = new Label();
        }

        public void Update(IImGuiStyle style, object[] data)
        {
            m_label.Padding = style.Padding;
            m_label.Margin = style.Margin;

            m_label.Content = string.Format((string)data[0], (object[])data[1]);
        }

        public TResult GetState<TResult>(string stateName)
        {
            return default(TResult);
        }
    }
}
