using System;
using System.Windows;
using System.Windows.Controls;

namespace ImGui.Wpf
{
    public static class ImGuiWpfExtensions
    {
        public static void Deconstruct<T1, T2>(this Tuple<T1, T2> tuple, out T1 item1, out T2 item2)
        {
            item1 = tuple.Item1;
            item2 = tuple.Item2;
        }

        public static TControl FindChildOfType<TControl>(this Panel panel) where TControl : FrameworkElement
        {
            foreach (var child in panel.Children)
            {
                if (child is TControl control)
                {
                    return control;
                }
            }

            return null;
        }

        public static TType Cast<TType>(object input)
        {
            return (TType)input;
        }
    }
}
