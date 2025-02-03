using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Web.WebView2.Wpf;
using System.Windows;
using WebStub.Models;
using WebStub.Services;

namespace WebStub
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DispatcherService.SetDispatcher(new WindowDispatcherWrapper(this));
            JavaScriptExecutor.SetExecutor(new JavaScriptExecutorWrapper(JavaScriptWebView));

            DataContext = DI.Get<MainWindowViewModel>();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _ = WeakReferenceMessenger.Default.Send(new CloseApplicationMessage());
        }

        private void GridSplitter_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            LeftContentGridColumn.Width = new GridLength(400);
        }

        private class WindowDispatcherWrapper(Window window) : IDispatcherWrapper
        {
            public void Run(Action callback)
            {
                window.Dispatcher.Invoke(callback, System.Windows.Threading.DispatcherPriority.Normal);
            }
        }

        private class JavaScriptExecutorWrapper(WebView2 webView2) : IJavaScriptExecutorWrapper
        {
            public Task<string> ExecuteJavaScriptAsync(string script)
            {
                return webView2.ExecuteScriptAsync(script);
            }
        }
    }
}