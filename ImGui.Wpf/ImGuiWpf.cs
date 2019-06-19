using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ImGui.Wpf
{
    public static class ImGuiWpf
    {
        private static StackPanel s_activeOwner;

        private static int s_controlId;
        private static readonly Dictionary<int, FrameworkElement> s_idToControlMap = new Dictionary<int, FrameworkElement>();
        private static readonly Dictionary<int, object> s_idToControlStateMap = new Dictionary<int, object>();

        public static void Deconstruct<T1, T2>(this Tuple<T1, T2> tuple, out T1 item1, out T2 item2)
        {
            item1 = tuple.Item1;
            item2 = tuple.Item2;
        }

        public static async Task BeginFrame()
        {
            s_controlId = 0;
            await Task.CompletedTask;
        }

        public static async Task EndFrame()
        {
            await InvalidateTree(s_controlId);
        }

        private static async Task InvalidateTree(int fromId)
        {
            var removedIds = new HashSet<int>();
            while (s_idToControlMap.TryGetValue(fromId++, out var orphanedElement))
            {
                removedIds.Add(fromId - 1);
                await s_activeOwner.Dispatcher.InvokeAsync(() =>
                {
                    s_activeOwner.Children.Remove(orphanedElement);
                });
            }

            foreach (var id in removedIds)
            {
                s_idToControlMap.Remove(id);
            }
        }

        public static void SetOwner(StackPanel owner)
        {
            s_activeOwner = owner;
        }

        private static async Task<Tuple<TControl, bool>> GetOrCreateControl<TControl>(int id) where TControl : FrameworkElement
        {
            if (!s_idToControlMap.TryGetValue(id, out var existingControl))
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

        private static TState GetControlState<TState>(int id, TState defaultState)
        {
            if (!s_idToControlStateMap.TryGetValue(id, out var state))
            {
                return defaultState;
            }

            if (!(state is TState))
            {
                return defaultState;
            }

            return (TState) state;
        }

        public static async Task Text(string message, params object[] args)
        {
            var controlId = s_controlId++;
            var formatted = string.Format(message, args);

            await s_activeOwner.Dispatcher.InvokeAsync(async () =>
            {
                var (control, created) = await GetOrCreateControl<TextBlock>(controlId);
                control.Text = formatted;
                control.Margin = new Thickness(2);
                control.Padding = new Thickness(2);

                if (created)
                {
                    s_activeOwner.Children.Add(control);
                }

                s_idToControlMap[controlId] = control;
            });
        }

        public static async Task<bool> Button(string text)
        {
            var controlId = s_controlId++;

            var state = GetControlState<bool?>(controlId, null);

            await s_activeOwner.Dispatcher.InvokeAsync(async () =>
            {
                var (control, created) = await GetOrCreateControl<Button>(controlId);
                control.Content = text;
                control.Margin = new Thickness(2);
                control.Padding = new Thickness(2);

                if (state != null)
                {
                    s_idToControlStateMap[controlId] = false;
                }

                if (created)
                {
                    control.Click += (s, e) => { s_idToControlStateMap[controlId] = true; };
                    s_activeOwner.Children.Add(control);
                }

                s_idToControlMap[controlId] = control;
            });

            if (state == true)
            {
                s_idToControlStateMap[controlId] = null;
                return true;
            }

            return state ?? false;
        }

        public static async Task<string> InputText(string title, string contents)
        {
            var controlId = s_controlId++;

            contents = GetControlState(controlId, contents);

            await s_activeOwner.Dispatcher.InvokeAsync(async () =>
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

                    control.TextChanged += (s, e) => { s_idToControlStateMap[controlId] = control.Text; };
                    s_activeOwner.Children.Add(dock);
                }

                s_idToControlMap[controlId] = dock;
            });

            return contents;
        }

        public static async Task<double> Slider(string title, double value, double minValue, double maxValue)
        {
            var controlId = s_controlId++;

            value = GetControlState(controlId, value);

            await s_activeOwner.Dispatcher.InvokeAsync(async () =>
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
                        s_idToControlStateMap[controlId] = slider.Value;
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

                    s_activeOwner.Children.Add(dock);
                }

                s_idToControlMap[controlId] = dock;
            });

            return value;
        }
    }
}
