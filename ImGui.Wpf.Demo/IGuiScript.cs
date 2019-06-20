using System.Threading.Tasks;
using System.Windows.Threading;

namespace ImGui.Wpf.Demo
{
    public interface IGuiScript
    {
        Task OnGui(Dispatcher dispatcher, ImGuiWpf imGui);
    }
}
