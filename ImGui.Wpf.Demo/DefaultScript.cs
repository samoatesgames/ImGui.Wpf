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
            await ShowMessage(dispatcher, $"You clicked save!\n{m_textContents}");
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

        m_isChecked = await imGui.RadioButton("Radio Checked", m_isChecked);
        m_isChecked = !await imGui.RadioButton("Radio UnChecked", !m_isChecked);

        await imGui.ProgressBar(m_sliderValue, 0.0, 1.0);

        await LayoutExample(dispatcher, imGui);
    }

    private async Task LayoutExample(Dispatcher dispatcher, ImGuiWpf imGui)
    {
        using (await imGui.BeginHorizontal())
        {
            if (await imGui.Button("Left"))
            {
                await ShowMessage(dispatcher, "You clicked left.");
            }

            if (await imGui.Button("Right"))
            {
                await ShowMessage(dispatcher, "You clicked right.");
            }

            using (await imGui.BeginVertical())
            {
                if (await imGui.Button("A"))
                {
                    await ShowMessage(dispatcher, "You clicked A.");
                }

                if (await imGui.Button("B"))
                {
                    await ShowMessage(dispatcher, "You clicked B.");
                }

                if (await imGui.Button("C"))
                {
                    await ShowMessage(dispatcher, "You clicked C.");
                }
            }

            using (await imGui.BeginHorizontal())
            {
                if (await imGui.Button("1"))
                {
                    await ShowMessage(dispatcher, "You clicked 1.");
                }

                if (await imGui.Button("2"))
                {
                    await ShowMessage(dispatcher, "You clicked 2.");
                }

                if (await imGui.Button("3"))
                {
                    await ShowMessage(dispatcher, "You clicked 3.");
                }
            }
        }
    }

    private async Task ShowMessage(Dispatcher dispatcher, string message)
    {
        await dispatcher.InvokeAsync(() =>
        {
            ModernDialog.ShowMessage(
                message,
                "",
                MessageBoxButton.OK
            );
        });
    }
}