using System;
using System.Windows.Controls;

namespace ImGui.Wpf.Layouts
{
    public class ImVerticalLayout : ImLayout
    {
        public override Panel Panel { get; }

        public ImVerticalLayout(Action onDispose) : base(onDispose)
        {
            Panel = new StackPanel();
        }
    }
}
