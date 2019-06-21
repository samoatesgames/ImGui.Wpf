using System.Windows;

namespace ImGui.Wpf
{
    public interface IImGuiControl
    {
        FrameworkElement WindowsControl { get; }

        void Update(object[] data);

        void ApplyStyle(IImGuiStyle style);

        TResult GetState<TResult>(string stateName);
    }
}
