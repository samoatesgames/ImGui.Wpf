using CSScriptLibrary;
using ImGui.Wpf.Demo.Annotations;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Threading;

namespace ImGui.Wpf.Demo
{
    public class WindowViewModel : INotifyPropertyChanged
    {
        private readonly StackPanel m_previewPanel;

        private string m_script;
        private string m_status;
        private IGuiScript m_activeScript;

        public string Script
        {
            get => m_script;
            set
            {
                m_script = value;
                OnPropertyChanged();
                Task.Run(async () => await ReloadScript());
            }
        }

        public string Status
        {
            get => m_status;
            set
            {
                m_status = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public WindowViewModel(StackPanel previewPanel)
        {
            m_previewPanel = previewPanel;
            Status = "Loading...";

            var assembly = Assembly.GetExecutingAssembly();
            var resourceName = "ImGui.Wpf.Demo.DefaultScript.cs";

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream != null)
                {
                    using (var reader = new StreamReader(stream))
                    {
                        Script = reader.ReadToEnd();
                    }
                }
            }

            var dispatcher = previewPanel.Dispatcher;
            Task.Run(async () => await UpdatePreview(dispatcher));
        }

        private async Task ReloadScript()
        {
            Status = "Compiling...";
            try
            {
                m_activeScript = await CSScript.Evaluator
                    .LoadCodeAsync<IGuiScript>(Script);
                Status = "Loaded.";
            }
            catch (Exception e)
            {
                m_activeScript = null;
                Status = $"Compile Error: {e.Message}";
            }
        }

        private async Task UpdatePreview(Dispatcher dispatcher)
        {
            await ReloadScript();

            using (var imGui = await ImGuiWpf.BeginUi(m_previewPanel))
            {
                while (true)
                {
                    await imGui.BeginFrame();

                    if (m_activeScript != null)
                    {
                        try
                        {
                            await m_activeScript.OnGui(dispatcher, imGui);
                        }
                        catch (Exception e)
                        {
                            Status = $"Runtime Exception: {e.Message}";
                        }
                    }

                    await imGui.EndFrame();

                    await Task.Delay(17);
                }
            }
        }
    }
}
