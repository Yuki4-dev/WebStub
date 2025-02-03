using System.Windows;
using WebStub;

namespace WebStub_WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            DI.Injection();
        }
    }

}
