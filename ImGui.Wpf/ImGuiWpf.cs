using ImGui.Wpf.Factories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using ImGui.Wpf.Controls;
using ImGui.Wpf.Styles;

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
            return (TType) input;
        }
    }

    public class ImGuiWpf : IDisposable
    {
        private readonly Panel m_controlOwner;
        private readonly IImGuiStyle m_style = new DefaultStyle();

        private int m_controlId;
        private readonly Dictionary<int, IImGuiControl> m_idToControlMap = new Dictionary<int, IImGuiControl>();
        private readonly Dictionary<Type, IImGuiControlFactory> m_controlFactories = new Dictionary<Type, IImGuiControlFactory>();

        private ImGuiWpf()
        {
            RegisterControl<ImButton>();
            RegisterControl<ImCheckBox>();
            RegisterControl<ImComboBox>();
            RegisterControl<ImLabel>();
            RegisterControl<ImListBox>();
            RegisterControl<ImProgressBar>();
            RegisterControl<ImSlider>();
            RegisterControl<ImTextBlock>();
            RegisterControl<ImTextBox>();
            RegisterControl<ImToggleButton>();
        }

        private ImGuiWpf(Panel owner) : this()
        {
            m_controlOwner = owner;
        }

        public void RegisterControl<TControl>(IImGuiControlFactory factory) where TControl : IImGuiControl
        {
            m_controlFactories[typeof(TControl)] = factory;
        }

        public void RegisterControl<TControl>() where TControl : IImGuiControl
        {
            m_controlFactories[typeof(TControl)] = new ImGenericFactory<TControl>();
        }

        public static async Task<ImGuiWpf> BeginPanel(Panel owner)
        {
            await Task.CompletedTask;
            return new ImGuiWpf(owner);
        }

        public static async Task<ImGuiWpf> BeginPanel()
        {
            return await BeginPanel(Application.Current.MainWindow);
        }

        public static async Task<ImGuiWpf> BeginPanel<TOwner>(TOwner owner) where TOwner : FrameworkElement, IAddChild
        {
            var panel = owner as Panel;
            if (panel == null)
            {
                var frameworkElement = (FrameworkElement) owner;

                StackPanel newPanel = null;
                await frameworkElement.Dispatcher.InvokeAsync(() =>
                {
                    newPanel = new StackPanel();
                    owner.AddChild(newPanel);
                });

                return await BeginPanel(newPanel);
            }

            return await BeginPanel(panel);
        }

        public async void Dispose()
        {
            await InvalidateTree(0);
        }

        public async Task BeginFrame()
        {
            m_controlId = 0;
            await Task.CompletedTask;
        }

        public async Task EndFrame()
        {
            await InvalidateTree(m_controlId);
        }

        private async Task InvalidateTree(int fromId)
        {
            var removedIds = new HashSet<int>();
            while (m_idToControlMap.TryGetValue(fromId++, out var orphanedElement))
            {
                removedIds.Add(fromId - 1);

                var controlToRemove = orphanedElement.WindowsControl;
                await m_controlOwner.Dispatcher.InvokeAsync(() =>
                {
                    m_controlOwner.Children.Remove(controlToRemove);
                });
            }

            foreach (var id in removedIds)
            {
                m_idToControlMap.Remove(id);
            }
        }
        
        private bool TryGetExistingControl<TControl>(int id, out IImGuiControl control)
        {
            if (!m_idToControlMap.TryGetValue(id, out var existingControl))
            {
                control = null;
                return false;
            }

            if (!(existingControl is TControl))
            {
                control = null;
                return false;
            }

            control = existingControl;
            return true;
        }

        private async Task<TControl> HandleControl<TControl>(object[] data) where TControl : IImGuiControl
        {
            var factory = m_controlFactories[typeof(TControl)];
            var controlId = m_controlId++;

            if (!TryGetExistingControl<TControl>(controlId, out var control))
            {
                await InvalidateTree(controlId);
            }
            
            await m_controlOwner.Dispatcher.InvokeAsync(() =>
            {
                if (control == null)
                {
                    control = factory.CreateNew();
                    m_controlOwner.Children.Add(control.WindowsControl);
                }

                control.ApplyStyle(m_style);
                control.Update(data);

                m_idToControlMap[controlId] = control;
            });

            return (TControl)control;
        }

        public async Task<bool> Button(string text)
        {
            var button = await HandleControl<ImButton>(new object[] { text });
            return button.GetState<bool>("Clicked");
        }

        public async Task<TType> ComboBox<TType>(string title, TType selected, params TType[] items)
        {
            var comboBox = await HandleControl<ImComboBox>(new object[] { title, selected, items });
            return comboBox.GetState<TType>("Selected");
        }

        public async Task<bool> CheckBox(string text, bool isChecked)
        {
            var checkBox = await HandleControl<ImCheckBox>(new object[] { text, isChecked });
            return checkBox.GetState<bool?>("Checked") ?? false;
        }

        public async Task<string> InputText(string title, string contents)
        {
            return await TextBox(title, contents);
        }

        public async Task Label(string message, params object[] args)
        {
            await HandleControl<ImLabel>(new object[] { message, args });
        }

        public async Task<TType> ListBox<TType>(string title, TType selected, params TType[] items)
        {
            var listBox = await HandleControl<ImListBox>(new object[] { title, selected, items });
            return listBox.GetState<TType>("Selected");
        }

        public async Task ProgressBar(double value, double minimum, double maximum)
        {
            await HandleControl<ImProgressBar>(new object[] { value, minimum, maximum });
        }

        public async Task<double> Slider(string title, double value, double minimum, double maximum)
        {
            var slider = await HandleControl<ImSlider>(new object[] { title, value, minimum, maximum });
            return slider.GetState<double>("Value");
        }

        public async Task Text(string message, params object[] args)
        {
            await TextBlock(message, args);
        }

        public async Task TextBlock(string message, params object[] args)
        {
            await HandleControl<ImTextBlock>(new object[] { message, args });
        }

        public async Task<string> TextBox(string title, string contents)
        {
            var textBox = await HandleControl<ImTextBox>(new object[] { title, contents });
            return textBox.GetState<string>("Text");
        }

        public async Task<bool> Toggle(string text, bool isChecked)
        {
            return await CheckBox(text, isChecked);
        }

        public async Task<bool> ToggleButton(string text, bool isChecked)
        {
            var toggleButton = await HandleControl<ImToggleButton>(new object[] { text, isChecked });
            return toggleButton.GetState<bool>("Checked");
        }
    }
}
