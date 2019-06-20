using System.Threading.Tasks;
using System.Windows;
using FirstFloor.ModernUI.Windows.Controls;

namespace ImGui.Wpf.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new WindowViewModel(PreviewPanel);
        }
    }
}
