using System.Windows;
using System.Windows.Controls;

namespace ImGui.Wpf.Controls
{
    internal class ImSlider : IImGuiControl
    {
        private readonly DockPanel m_dockPanel;
        private readonly TextBlock m_label;
        private readonly Slider m_slider;
        private readonly TextBox m_editBox;

        public FrameworkElement WindowsControl => m_dockPanel;

        private double? m_lastKnownValue;
        private double m_sliderValue;

        public ImSlider()
        {
            m_dockPanel = new DockPanel();

            m_label = new TextBlock();

            m_slider = new Slider();
            m_slider.ValueChanged += OnValueChanged;

            m_editBox = new TextBox();
            m_editBox.TextChanged += OnTextChanged;

            DockPanel.SetDock(m_label, Dock.Left);
            DockPanel.SetDock(m_editBox, Dock.Right);

            m_dockPanel.Children.Add(m_label);
            m_dockPanel.Children.Add(m_editBox);
            m_dockPanel.Children.Add(m_slider);
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (double.TryParse(m_editBox.Text, out var newValue))
            {
                m_slider.Value = newValue;
                m_sliderValue = newValue;
            }
        }

        private void OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            m_sliderValue = m_slider.Value;
            m_editBox.Text = m_sliderValue.ToString("F");
        }

        public void Update(IImGuiStyle style, object[] data)
        {
            m_label.Padding = style.Padding;
            m_label.Margin = style.Margin;
            m_slider.Padding = style.Padding;
            m_slider.Margin = style.Margin;
            m_editBox.Padding = style.Padding;
            m_editBox.Margin = style.Margin;

            m_label.Text = (string)data[0];

            var value = (double)data[1];
            var minimum = (double)data[2];
            var maximum = (double)data[3];

            m_slider.Minimum = minimum;
            m_slider.Maximum = maximum;

            if (value == m_lastKnownValue)
            {
                return;
            }

            m_lastKnownValue = m_sliderValue = value;
            m_slider.Value = m_sliderValue;
            m_editBox.Text = m_sliderValue.ToString("F");
        }

        public TResult GetState<TResult>(string stateName)
        {
            if (stateName == "Value")
            {
                m_lastKnownValue = m_sliderValue;
                return ImGuiWpfExtensions.Cast<TResult>(m_sliderValue);
            }

            return default(TResult);
        }
    }
}
