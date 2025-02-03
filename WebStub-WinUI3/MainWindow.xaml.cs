using CommunityToolkit.Mvvm.Messaging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Threading.Tasks;
using WebStub.Models;
using WebStub.Services;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WebStub
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindowViewModel ViewModel { get; private set; }

        public MainWindow()
        {
            this.InitializeComponent();
            DispatcherService.SetDispatcher(new WindowDispatcherWrapper(this));
            JavaScriptExecutor.SetExecutor(new JavaScriptExecutorWrapper(JavaScriptWebView));

            ViewModel = DI.Get<MainWindowViewModel>();
        }

        private void Window_Closed(object sender, WindowEventArgs args)
        {
            _ = WeakReferenceMessenger.Default.Send(new CloseApplicationMessage());
        }

        private void GridSplitter_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            LeftContextLength.Width = new GridLength(420);
        }

        private class WindowDispatcherWrapper(Window window) : IDispatcherWrapper
        {
            public void Run(Action callback)
            {
                _ = window.DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal, () => callback.Invoke());
            }
        }

        private class JavaScriptExecutorWrapper(WebView2 webView2) : IJavaScriptExecutorWrapper
        {
            public Task<string> ExecuteJavaScriptAsync(string script)
            {
                return webView2.ExecuteScriptAsync(script).AsTask();
            }
        }
    }
}
