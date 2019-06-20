using System.Windows;

namespace ImGui.Wpf
{
    public interface IImGuiControl
    {
        FrameworkElement WindowsControl { get; }

        void Update(object[] data);
        TResult GetState<TResult>(string stateName);
    }
}
