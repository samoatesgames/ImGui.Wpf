using System;
using System.Windows;
using System.Windows.Controls;

namespace ImGui.Wpf.Layouts
{
    public abstract class ImLayout : IImGuiBase, IDisposable
    {
        private readonly Action m_onDispose;

        protected ImLayout(Action onDispose)
        {
            m_onDispose = onDispose;
        }

        public FrameworkElement WindowsControl => GetPanel();
        internal abstract Panel GetPanel();
        public abstract void AddChild(IImGuiControl control);
        public abstract void AddChild(ImLayout layout);

        public void Dispose()
        {
            m_onDispose?.Invoke();
        }
    }
}
