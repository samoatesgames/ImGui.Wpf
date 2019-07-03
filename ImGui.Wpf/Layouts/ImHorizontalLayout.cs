using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace ImGui.Wpf.Layouts
{
    public class ImHorizontalLayout : ImLayout
    {
        public override Panel Panel { get; }

        public ImHorizontalLayout(Action onDispose) : base(onDispose)
        {
            Panel = new UniformGrid
            {
                Rows = 1
            };
        }
    }
}
