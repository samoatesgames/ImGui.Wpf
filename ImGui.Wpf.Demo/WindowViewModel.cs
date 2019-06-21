using CSScriptLibrary;
using ImGui.Wpf.Demo.Annotations;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Expression.Interactivity.Core;

namespace ImGui.Wpf.Demo
{
    public class WindowViewModel : INotifyPropertyChanged
    {
        private readonly StackPanel m_previewPanel;

        private string m_script;
        private IGuiScript m_activeScript;

        public string Script
        {
            get => m_script;
            set
            {
                m_script = value;
                OnPropertyChanged();
            }
        }

        public ICommand UpdateScript { get; }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public WindowViewModel(StackPanel previewPanel)
        {
            UpdateScript = new ActionCommand(_ =>
            {
                m_activeScript = null;
                Task.Run(async () => await ReloadScript());
            });

            m_previewPanel = previewPanel;

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "ImGui.Wpf.Demo.DefaultScript.cs";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    Script = reader.ReadToEnd();
                }
            }

            var dispatcher = previewPanel.Dispatcher;
            Task.Run(async () => await UpdatePreview(dispatcher));
        }

        private async Task ReloadScript()
        {
            try
            {
                m_activeScript = await CSScript.Evaluator
                    .LoadCodeAsync<IGuiScript>(Script);
            }
            catch (Exception)
            {
                m_activeScript = null;
            }
        }

        private async Task UpdatePreview(Dispatcher dispatcher)
        {
            await ReloadScript();

            using (var imGui = await ImGuiWpf.BeginPanel(m_previewPanel))
            {
                while (true)
                {
                    await imGui.BeginFrame();

                    if (m_activeScript != null)
                    {
                        await m_activeScript.OnGui(dispatcher, imGui);
                    }

                    await imGui.EndFrame();

                    await Task.Delay(17);
                }
            }
        }
    }
}
