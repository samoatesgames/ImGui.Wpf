using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ImGui.Wpf
{
    public static class ImProgressBarExtension
    {
        public static async Task ProgressBar(this ImGuiWpf imGui, double value, double minimum, double maximum)
        {
            await imGui.HandleControl<Controls.ImProgressBar>(new object[] { value, minimum, maximum });
        }
    }
}

namespace ImGui.Wpf.Controls
{
    internal class ImProgressBar : IImGuiControl
    {
        private readonly Grid m_grid;
        private readonly TextBlock m_textBlock;
        private readonly ProgressBar m_progressBar;
        public FrameworkElement WindowsControl => m_grid;

        public ImProgressBar()
        {
            m_grid = new Grid
            {
                Height = 20
            };

            m_textBlock = new TextBlock
            {
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            m_progressBar = new ProgressBar();

            m_grid.Children.Add(m_progressBar);
            m_grid.Children.Add(m_textBlock);
        }

        public void Update(object[] data)
        {
            m_progressBar.Value = (double) data[0];
            m_progressBar.Minimum = (double) data[1];
            m_progressBar.Maximum = (double) data[2];

            var progress = (m_progressBar.Value - m_progressBar.Minimum) / (m_progressBar.Maximum - m_progressBar.Minimum);
            m_textBlock.Text = $"{progress * 100.0:F0}%";
        }

        public void ApplyStyle(IImGuiStyle style)
        {
            m_progressBar.Padding = style.Padding;
            m_progressBar.Margin = style.Margin;
        }

        public TResult GetState<TResult>(string stateName)
        {
            return default(TResult);
        }
    }
}
