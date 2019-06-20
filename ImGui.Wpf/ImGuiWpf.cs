using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace ImGui.Wpf
{
    public static class TupleExtensions
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
    }

    public class ImGuiWpf : IDisposable
    {
        private readonly Panel m_controlOwner;

        private int m_controlId;
        private readonly Dictionary<int, FrameworkElement> m_idToControlMap = new Dictionary<int, FrameworkElement>();
        private readonly Dictionary<int, object> m_idToControlStateMap = new Dictionary<int, object>();
        
        private ImGuiWpf(Panel owner)
        {
            m_controlOwner = owner;
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
                await m_controlOwner.Dispatcher.InvokeAsync(() =>
                {
                    m_controlOwner.Children.Remove(orphanedElement);
                });
            }

            foreach (var id in removedIds)
            {
                m_idToControlMap.Remove(id);
            }
        }

        private async Task<Tuple<TControl, bool>> GetOrCreateControl<TControl>(int id) where TControl : FrameworkElement
        {
            if (!m_idToControlMap.TryGetValue(id, out var existingControl))
            {
                return new Tuple<TControl, bool>(Activator.CreateInstance<TControl>(), true);
            }

            if (!(existingControl is TControl))
            {
                await InvalidateTree(id);
                return new Tuple<TControl, bool>(Activator.CreateInstance<TControl>(), true);
            }

            return new Tuple<TControl, bool>((TControl)existingControl, false);
        }

        private TState GetControlState<TState>(int id, TState defaultState)
        {
            if (!m_idToControlStateMap.TryGetValue(id, out var state))
            {
                return defaultState;
            }

            if (!(state is TState))
            {
                return defaultState;
            }

            return (TState) state;
        }

        public async Task Text(string message, params object[] args)
        {
            var controlId = m_controlId++;
            var formatted = string.Format(message, args);

            await m_controlOwner.Dispatcher.InvokeAsync(async () =>
            {
                var (control, created) = await GetOrCreateControl<TextBlock>(controlId);
                control.Text = formatted;
                control.Margin = new Thickness(2);
                control.Padding = new Thickness(2);
                control.TextWrapping = TextWrapping.Wrap;

                if (created)
                {
                    m_controlOwner.Children.Add(control);
                }

                m_idToControlMap[controlId] = control;
            });
        }

        public async Task<bool> Button(string text)
        {
            var controlId = m_controlId++;

            var state = GetControlState<bool?>(controlId, null);

            await m_controlOwner.Dispatcher.InvokeAsync(async () =>
            {
                var (control, created) = await GetOrCreateControl<Button>(controlId);
                control.Content = text;
                control.Margin = new Thickness(2);
                control.Padding = new Thickness(2);

                if (state != null)
                {
                    m_idToControlStateMap[controlId] = false;
                }

                if (created)
                {
                    control.Click += (s, e) => { m_idToControlStateMap[controlId] = true; };
                    m_controlOwner.Children.Add(control);
                }

                m_idToControlMap[controlId] = control;
            });

            if (state == true)
            {
                m_idToControlStateMap[controlId] = null;
                return true;
            }

            return state ?? false;
        }

        public async Task<string> InputText(string title, string contents)
        {
            var controlId = m_controlId++;

            contents = GetControlState(controlId, contents);

            await m_controlOwner.Dispatcher.InvokeAsync(async () =>
            {
                var (dock, createdDock) = await GetOrCreateControl<DockPanel>(controlId);

                if (createdDock)
                {
                    var textBlock = new TextBlock
                    {
                        Text = title,
                        Margin = new Thickness(2),
                        Padding = new Thickness(2)
                    };
                    dock.Children.Add(textBlock);

                    var control = new TextBox
                    {
                        Text = contents,
                        Margin = new Thickness(2),
                        Padding = new Thickness(2)
                    };
                    dock.Children.Add(control);

                    control.TextChanged += (s, e) => { m_idToControlStateMap[controlId] = control.Text; };
                    m_controlOwner.Children.Add(dock);
                }

                m_idToControlMap[controlId] = dock;
            });

            return contents;
        }

        public async Task<double> Slider(string title, double value, double minValue, double maxValue)
        {
            var controlId = m_controlId++;

            value = GetControlState(controlId, value);

            await m_controlOwner.Dispatcher.InvokeAsync(async () =>
            {
                var (dock, createdDock) = await GetOrCreateControl<DockPanel>(controlId);

                if (createdDock)
                {
                    var textBlock = new TextBlock
                    {
                        Text = title,
                        Margin = new Thickness(2),
                        Padding = new Thickness(2)
                    };
                    DockPanel.SetDock(textBlock, Dock.Left);
                    dock.Children.Add(textBlock);

                    var editBox = new TextBox
                    {
                        Text = value.ToString("F"),
                        Width = 80,
                        Margin = new Thickness(2),
                        Padding = new Thickness(2)
                    };
                    DockPanel.SetDock(editBox, Dock.Right);
                    dock.Children.Add(editBox);

                    var slider = new Slider
                    {
                        Value = value,
                        Minimum = minValue,
                        Maximum = maxValue,
                        Margin = new Thickness(2),
                        Padding = new Thickness(2)
                    };
                    dock.Children.Add(slider);

                    slider.ValueChanged += (s, e) =>
                    {
                        m_idToControlStateMap[controlId] = slider.Value;
                        editBox.Text = slider.Value.ToString("F");
                    };

                    editBox.TextChanged += (s, e) =>
                    {
                        if (double.TryParse(editBox.Text, out var newValue))
                        {
                            slider.Value = Math.Max(minValue, Math.Min(maxValue, newValue));
                            editBox.Text = slider.Value.ToString("F");
                        }
                    };

                    m_controlOwner.Children.Add(dock);
                }

                m_idToControlMap[controlId] = dock;
            });

            return value;
        }
    }
}
