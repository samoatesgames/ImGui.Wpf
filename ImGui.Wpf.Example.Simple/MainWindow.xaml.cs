using System.Threading.Tasks;
using System.Windows;

namespace ImGui.Wpf.Example.Simple
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            Task.Run(async () => await UpdateImGui());
        }

        private async Task UpdateImGui()
        {
            ImGuiWpf.SetOwner(RootPanel);

            var buffer = "Quick Brown Fox";
            var sliderValue = 0.5;

            while (true)
            {
                await ImGuiWpf.BeginFrame();

                await ImGuiWpf.Text("Hello World {0}", 123);
                if (await ImGuiWpf.Button("Save"))
                {
                    MessageBox.Show($"You clicked save!\n{buffer}");
                }
                buffer = await ImGuiWpf.InputText("Input Text:", buffer);
                sliderValue = await ImGuiWpf.Slider("Slider:", sliderValue, 0.0, 1.0);

                await ImGuiWpf.EndFrame();

                await Task.Delay(100);
            }
        }
    }
}
