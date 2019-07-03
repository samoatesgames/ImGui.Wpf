using System;
using System.Windows.Controls;

namespace ImGui.Wpf.Layouts
{
    public abstract class ImLayout : IDisposable
    {
        private readonly Action m_onDispose;

        public abstract Panel Panel { get; }

        protected ImLayout(Action onDispose)
        {
            m_onDispose = onDispose;
        }

        public void Dispose()
        {
            m_onDispose?.Invoke();
        }
    }
}
