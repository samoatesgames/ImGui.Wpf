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

    private bool m_isChecked = false;

    private string[] m_fruit = new[] { "Apple", "Banana", "Tomato" };
    private string m_selectedFruit = "Banana";

    public async Task OnGui(Dispatcher dispatcher, ImGuiWpf imGui)
    {
        await imGui.Text("Hello World {0:F}", m_sliderValue);
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

        m_isChecked = await imGui.CheckBox("Check Me!", m_isChecked);
        if (m_isChecked)
        {
            await imGui.Label("You checked me!");
        }

        m_selectedFruit = await imGui.ComboBox("Fruit:", m_selectedFruit, m_fruit);

        m_textContents = await imGui.InputText("Input Text:", m_textContents);
        m_sliderValue = await imGui.Slider("Slider:", m_sliderValue, 0.0, 1.0);

        m_isChecked = await imGui.ToggleButton("Toggle Me", m_isChecked);

        m_selectedFruit = await imGui.ListBox("Fruit:", m_selectedFruit, m_fruit);
    }
}