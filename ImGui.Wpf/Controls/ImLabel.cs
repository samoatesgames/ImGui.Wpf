using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ImGui.Wpf
{
    public static class ImLabelExtension
    {
        public static async Task Label(this ImGuiWpf imGui, string message, params object[] args)
        {
            await imGui.HandleControl<Controls.ImLabel>(new object[] { message, args });
        }

        public static async Task Break(this ImGuiWpf imGui)
        {
            await imGui.HandleControl<Controls.ImLabel>(new object[] { "", null });
        }

        public static async Task Space(this ImGuiWpf imGui)
        {
            await imGui.HandleControl<Controls.ImLabel>(new object[] { "", null });
        }
    }
}

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

        public void Update(object[] data)
        {
            m_label.Content = string.Format((string)data[0], (object[])data[1]);
        }

        public void ApplyStyle(IImGuiStyle style)
        {
            m_label.Padding = style.Padding;
            m_label.Margin = style.Margin;
        }

        public TResult GetState<TResult>(string stateName)
        {
            return default(TResult);
        }
    }
}
