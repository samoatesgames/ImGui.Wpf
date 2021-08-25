using System;
using System.Windows.Controls;

namespace ImGui.Wpf.Layouts
{
    public class ImVerticalLayout : ImLayout
    {
        private readonly Panel m_panel;

        public ImVerticalLayout(Action onDispose) : base(onDispose)
        {
            m_panel = new StackPanel();
        }

        public ImVerticalLayout(Panel existingPanel, Action onDispose) : base(onDispose)
        {
            m_panel = existingPanel;
        }

        internal override Panel GetPanel()
        {
            return m_panel;
        }

        public override void AddChild(IImGuiControl control)
        {
            m_panel.Children.Add(control.WindowsControl);
        }

        public override void AddChild(ImLayout layout)
        {
            m_panel.Children.Add(layout.GetPanel());
        }
    }
}
