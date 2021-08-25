using ImGui.Wpf.Controls;
using ImGui.Wpf.Factories;
using ImGui.Wpf.Layouts;
using ImGui.Wpf.Styles;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Threading;

namespace ImGui.Wpf
{
    public class ImGuiWpf : IDisposable
    {
        private readonly ImLayout m_rootLayout;
        private readonly Dispatcher m_dispatcher;
        private readonly IImGuiStyle m_style = new DefaultStyle();

        private int m_controlId;
        private readonly Dictionary<int, IImGuiBase> m_idToControlMap = new Dictionary<int, IImGuiBase>();
        private readonly Dictionary<Type, IImGuiControlFactory> m_controlFactories = new Dictionary<Type, IImGuiControlFactory>();

        private readonly Stack<ImLayout> m_layoutStack = new Stack<ImLayout>();

        private ImGuiWpf()
        {
            RegisterDefaultControls();
        }

        private ImGuiWpf(Panel owner) : this()
        {
            m_dispatcher = owner.Dispatcher;

            m_rootLayout = new ImVerticalLayout(owner, null);
            m_layoutStack.Push(m_rootLayout);
        }

        private void RegisterDefaultControls()
        {
            RegisterControl<ImButton>();
            RegisterControl<ImCheckBox>();
            RegisterControl<ImComboBox>();
            RegisterControl<ImLabel>();
            RegisterControl<ImImage>();
            RegisterControl<ImListBox>();
            RegisterControl<ImProgressBar>();
            RegisterControl<ImRadioButton>();
            RegisterControl<ImSlider>();
            RegisterControl<ImTextBlock>();
            RegisterControl<ImTextBox>();
            RegisterControl<ImToggleButton>();
        }

        public void RegisterControl<TControl>(IImGuiControlFactory factory) where TControl : IImGuiControl
        {
            m_controlFactories[typeof(TControl)] = factory;
        }

        public void RegisterControl<TControl>() where TControl : IImGuiControl
        {
            m_controlFactories[typeof(TControl)] = new ImGenericFactory<TControl>();
        }

        public static async Task<ImGuiWpf> BeginUi(Panel owner)
        {
            await Task.CompletedTask;
            return new ImGuiWpf(owner);
        }

        public static async Task<ImGuiWpf> BeginUi()
        {
            return await BeginUi(Application.Current.MainWindow);
        }

        public static async Task<ImGuiWpf> BeginUi<TOwner>(TOwner owner) where TOwner : FrameworkElement, IAddChild
        {
            if (owner is Panel panel)
            {
                return await BeginUi(panel);
            }

            var frameworkElement = (FrameworkElement)owner;

            StackPanel newPanel = null;
            await frameworkElement.Dispatcher.InvokeAsync(() =>
            {
                newPanel = new StackPanel();
                owner.AddChild(newPanel);
            });

            return await BeginUi(newPanel);
        }

        public async void Dispose()
        {
            await InvalidateTree(0);
        }

        public void BeginFrame()
        {
            m_controlId = 0;
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
                await m_dispatcher.InvokeAsync(() =>
                {
                    if (controlToRemove.Parent is Panel parent)
                    {
                        parent.Children.Remove(controlToRemove);

                        var panel = parent;
                        while (panel?.Children.Count == 0)
                        {
                            parent = panel.Parent as Panel;
                            if (panel == m_rootLayout.WindowsControl)
                            {
                                break;
                            }
                            
                            parent?.Children.Remove(panel);
                            panel = parent;
                        }
                    }
                });
            }

            foreach (var id in removedIds)
            {
                m_idToControlMap.Remove(id);
            }
        }

        private bool TryGetExistingControl<TControl>(int id, out TControl control) where TControl : IImGuiBase
        {
            if (!m_idToControlMap.TryGetValue(id, out var existingControl))
            {
                control = default;
                return false;
            }

            if (!(existingControl is TControl typedControl))
            {
                control = default;
                return false;
            }

            control = typedControl;
            return true;
        }

        // Layout

        private void PopLayout()
        {
            m_layoutStack.Pop();
        }

        public async Task<ImLayout> BeginHorizontal()
        {
            var controlId = m_controlId++;

            if (TryGetExistingControl<ImHorizontalLayout>(controlId, out var layout))
            {
                m_layoutStack.Push(layout);
                return layout;
            }

            await InvalidateTree(controlId);

            await m_dispatcher.InvokeAsync(() =>
            {
                layout = new ImHorizontalLayout(PopLayout);

                var owner = m_layoutStack.Peek();
                owner.AddChild(layout);

                m_layoutStack.Push(layout);

                m_idToControlMap[controlId] = layout;
            });

            return layout;
        }

        public async Task<ImLayout> BeginHorizontal(params string[] columnWidths)
        {
            var controlId = m_controlId++;

            if (TryGetExistingControl<ImHorizontalLayout>(controlId, out var layout))
            {
                if (layout.HasColumnWidths(columnWidths))
                {
                    m_layoutStack.Push(layout);
                    return layout;
                }
            }

            await InvalidateTree(controlId);

            await m_dispatcher.InvokeAsync(() =>
            {
                layout = new ImHorizontalLayout(columnWidths, PopLayout);

                var owner = m_layoutStack.Peek();
                owner.AddChild(layout);

                m_layoutStack.Push(layout);

                m_idToControlMap[controlId] = layout;
            });

            return layout;
        }

        public async Task<ImLayout> BeginVertical()
        {
            var controlId = m_controlId++;

            if (TryGetExistingControl<ImVerticalLayout>(controlId, out var layout))
            {
                m_layoutStack.Push(layout);
                return layout;
            }

            await InvalidateTree(controlId);

            await m_dispatcher.InvokeAsync(() =>
            {
                layout = new ImVerticalLayout(PopLayout);

                var owner = m_layoutStack.Peek();
                owner.AddChild(layout);

                m_layoutStack.Push(layout);

                m_idToControlMap[controlId] = layout;
            });

            return layout;
        }

        // Controls

        public async Task<TControl> HandleControl<TControl>(object[] data) where TControl : class, IImGuiControl
        {
            if (!(m_controlFactories.TryGetValue(typeof(TControl), out var factory)))
            {
                factory = new ImGenericFactory<TControl>();
                m_controlFactories[typeof(TControl)] = factory;
            }

            var controlId = m_controlId++;

            if (!TryGetExistingControl<TControl>(controlId, out var control))
            {
                await InvalidateTree(controlId);
            }
            
            await m_dispatcher.InvokeAsync(() =>
            {
                if (control == null)
                {
                    control = factory.CreateNew() as TControl;
                    if (control == null)
                    {
                        return;
                    }

                    var owner = m_layoutStack.Peek();
                    owner.AddChild(control);
                }

                control.ApplyStyle(m_style);
                control.Update(data);

                m_idToControlMap[controlId] = control;
            });

            return control;
        }
    }
}
