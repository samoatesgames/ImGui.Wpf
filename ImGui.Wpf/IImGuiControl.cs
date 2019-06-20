using System.Windows;

namespace ImGui.Wpf
{
    public interface IImGuiControl
    {
        FrameworkElement WindowsControl { get; }

        void Update(IImGuiStyle style, object[] data);
        TResult GetState<TResult>(string stateName);
    }
}
