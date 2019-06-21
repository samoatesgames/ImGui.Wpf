using System.Windows;
using System.Windows.Controls;

namespace ImGui.Wpf.Controls
{
    internal class ImTextBox : IImGuiControl
    {
        private readonly DockPanel m_dockPanel;
        private readonly TextBlock m_textBlock;
        private readonly TextBox m_textBox;

        public FrameworkElement WindowsControl => m_dockPanel;

        private string m_lastKnownText;
        private string m_textBoxText;

        public ImTextBox()
        {
            m_dockPanel = new DockPanel();

            m_textBlock = new TextBlock();

            m_textBox = new TextBox();
            m_textBox.TextChanged += OnTextChanged;

            m_dockPanel.Children.Add(m_textBlock);
            m_dockPanel.Children.Add(m_textBox);
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            m_textBoxText = m_textBox.Text;
        }

        public void Update(object[] data)
        {
            m_textBlock.Text = (string) data[0];

            var inputText = (string)data[1];
            if (inputText == null || inputText.Equals(m_lastKnownText))
            {
                return;
            }

            m_lastKnownText = m_textBoxText = inputText;
            m_textBox.Text = m_textBoxText;
        }

        public void ApplyStyle(IImGuiStyle style)
        {
            m_textBlock.Padding = style.Padding;
            m_textBlock.Margin = style.Margin;
            m_textBox.Padding = style.Padding;
            m_textBox.Margin = style.Margin;
        }

        public TResult GetState<TResult>(string stateName)
        {
            if (stateName == "Text")
            {
                m_lastKnownText = m_textBoxText;
                return ImGuiWpfExtensions.Cast<TResult>(m_textBoxText);
            }

            return default(TResult);
        }
    }
}
