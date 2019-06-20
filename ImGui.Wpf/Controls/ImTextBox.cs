using System.Windows;
using System.Windows.Controls;

namespace ImGui.Wpf.Controls
{
    internal class ImTextBox : IImGuiControl
    {
        private readonly DockPanel m_dockPanel;
        private readonly TextBlock m_label;
        private readonly TextBox m_textBox;

        public FrameworkElement WindowsControl => m_dockPanel;

        private string m_lastKnownText;
        private string m_textBoxText;

        public ImTextBox()
        {
            m_dockPanel = new DockPanel();

            m_label = new TextBlock();

            m_textBox = new TextBox();
            m_textBox.TextChanged += OnTextChanged;

            m_dockPanel.Children.Add(m_label);
            m_dockPanel.Children.Add(m_textBox);
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            m_textBoxText = m_textBox.Text;
        }

        public void Update(object[] data)
        {
            m_label.Text = (string) data[0];

            var inputText = (string)data[1];
            if (inputText == null || inputText.Equals(m_lastKnownText))
            {
                return;
            }

            m_lastKnownText = m_textBoxText = inputText;
            m_textBox.Text = m_textBoxText;
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
