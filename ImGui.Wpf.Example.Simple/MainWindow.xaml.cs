﻿using ImGui.Wpf.Example.Simple.CustomControl;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ImGui.Wpf.Example.Simple
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();

            Task.Run(async () => await UpdateImDemo(LeftGroup));
            Task.Run(async () => await UpdateImAbout(RightGroup));
        }

        private async Task UpdateImDemo(GroupBox owner)
        {
            var sliderValue = 0.5;
            var buffer = "Hello World";
            var isChecked = false;

            var fruit = new[] {"Apple", "Banana", "Tomato"};
            var comboSelected = fruit[1];

            using (var imGui = await ImGuiWpf.BeginUi(owner))
            {
                while (true)
                {
                    imGui.BeginFrame();

                    await imGui.Text("Hello World {0:F}", sliderValue);
                    if (await imGui.Button("Save"))
                    {
                        MessageBox.Show($"You clicked save!\n{buffer}");
                    }

                    if (await imGui.ExampleCustomControl("I'm a custom control"))
                    {
                        MessageBox.Show($"All my code lives within the applications assembly, yet you can call me via imGui!");
                    }

                    isChecked = await imGui.CheckBox("Check Me!", isChecked);
                    if (isChecked)
                    {
                        await imGui.Label("You checked me!");
                    }

                    comboSelected = await imGui.ComboBox("Fruit:", comboSelected, fruit);

                    buffer = await imGui.InputText("Input Text:", buffer);
                    sliderValue = await imGui.Slider("Slider:", sliderValue, 0.0, 1.0);

                    isChecked = await imGui.ToggleButton("Toggle Me", isChecked);

                    comboSelected = await imGui.ListBox("Fruit:", comboSelected, fruit);

                    isChecked = await imGui.RadioButton("Radio Checked", isChecked);
                    isChecked = !await imGui.RadioButton("Radio UnChecked", !isChecked);

                    await imGui.ProgressBar(sliderValue, 0.0, 1.0);

                    using (await imGui.BeginHorizontal())
                    {
                        if (await imGui.Button("Left"))
                        {
                            MessageBox.Show("You clicked left.");
                        }

                        if (await imGui.Button("Right"))
                        {
                            MessageBox.Show("You clicked right.");
                        }

                        using (await imGui.BeginVertical())
                        {
                            if (await imGui.Button("A"))
                            {
                                MessageBox.Show("You clicked A.");
                            }

                            if (await imGui.Button("B"))
                            {
                                MessageBox.Show("You clicked B.");
                            }

                            if (await imGui.Button("C"))
                            {
                                MessageBox.Show("You clicked C.");
                            }
                        }

                        using (await imGui.BeginHorizontal())
                        {
                            if (await imGui.Button("1"))
                            {
                                MessageBox.Show("You clicked 1.");
                            }

                            if (await imGui.Button("2"))
                            {
                                MessageBox.Show("You clicked 2.");
                            }

                            if (await imGui.Button("3"))
                            {
                                MessageBox.Show("You clicked 3.");
                            }
                        }
                    }

                    await imGui.EndFrame();

                    await Task.Delay(20);
                }
            }
        }

        private async Task UpdateImAbout(GroupBox owner)
        {
            using (var imGui = await ImGuiWpf.BeginUi(owner))
            {
                while (true)
                {
                    imGui.BeginFrame();

                    await imGui.Text("This is a simple example of using ImGui.Wpf" +
                                     "The idea of this library is to implement an immediate mode " +
                                     "Gui system usable in WPF." +
                                     "The primary use for this library is to enable runtime script " +
                                     "execution to also implement Gui's.");

                    await imGui.EndFrame();

                    await Task.Delay(100);
                }
            }
        }
    }
}
