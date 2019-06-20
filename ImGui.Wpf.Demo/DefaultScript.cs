using FirstFloor.ModernUI.Windows.Controls;
using ImGui.Wpf;
using ImGui.Wpf.Demo;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

public class Demo : IGuiScript
{
    private string m_textContents = "Hello World";
    private double m_sliderValue = 0.5;

    public async Task OnGui(Dispatcher dispatcher, ImGuiWpf imGui)
    {
        await imGui.Text("Hello World {0}", 123);
        if (await imGui.Button("Save"))
        {
            await dispatcher.InvokeAsync(() =>
            {
                ModernDialog.ShowMessage(
                    $"You clicked save!\n{m_textContents}", 
                    "Clicky Click", 
                    MessageBoxButton.OK
                );
            });
        }
        
        m_textContents = await imGui.InputText("Input Text:", m_textContents);
        m_sliderValue = await imGui.Slider("Slider:", m_sliderValue, 0.0, 1.0);
    }
}