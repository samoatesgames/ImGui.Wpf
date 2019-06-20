using System.Windows;

namespace ImGui.Wpf
{
    public interface IImGuiStyle
    {
        Thickness Margin { get; }
        Thickness Padding { get; }
    }
}
