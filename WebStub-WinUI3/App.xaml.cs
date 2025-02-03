using CommunityToolkit.WinUI.Helpers;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Windows.UI;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WebStub
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public partial class App : Application
    {
        public static Window? Window;

        public static ThemeListener? Listener;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            DI.Injection();

            UnhandledException += App_UnhandledException;
        }

        private async void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            if (Window != null)
            {
                ContentDialog d = new ContentDialog()
                {
                    Title = "Unhandled Exception",
                    Content = e.Message,
                    CloseButtonText = "Close",
                    XamlRoot = Window.Content.XamlRoot,
                };

                _ = await d.ShowAsync();
            }
        }

        /// <summary>
        /// Invoked when the application is launched.
        /// </summary>
        /// <param name="args">Details about the launch request and process.</param>
        protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
        {
            Window = new MainWindow();
            Window.ExtendsContentIntoTitleBar = true;

            Listener = new ThemeListener();
            Listener.ThemeChanged += ThemeListener_ThemeChanged;

            _ = SetTitleBarColors(RequestedTheme == ApplicationTheme.Dark);

            Window.Activate();
        }

        private void ThemeListener_ThemeChanged(ThemeListener sender)
        {
            _ = SetTitleBarColors(RequestedTheme == ApplicationTheme.Dark);
        }

        private bool SetTitleBarColors(bool dark)
        {
            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                nint hWnd = WindowNative.GetWindowHandle(Window);
                Microsoft.UI.WindowId wndId = Win32Interop.GetWindowIdFromWindow(hWnd);
                AppWindow appWindow = AppWindow.GetFromWindowId(wndId);

                AppWindowTitleBar titleBar = appWindow.TitleBar;

                if (dark)
                {
                    // Set active window colors
                    titleBar.ButtonForegroundColor = Colors.White;
                    titleBar.ButtonBackgroundColor = Colors.Transparent;
                    titleBar.ButtonHoverForegroundColor = Colors.White;
                    titleBar.ButtonHoverBackgroundColor = Color.FromArgb(255, 90, 90, 90);
                    titleBar.ButtonPressedForegroundColor = Colors.White;
                    titleBar.ButtonPressedBackgroundColor = Color.FromArgb(255, 120, 120, 120);

                    // Set inactive window colors
                    titleBar.InactiveForegroundColor = Colors.Gray;
                    titleBar.InactiveBackgroundColor = Colors.Transparent;
                    titleBar.ButtonInactiveForegroundColor = Colors.Gray;
                    titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

                    titleBar.BackgroundColor = Color.FromArgb(255, 45, 45, 45);
                }
                else
                {
                    // Set active window colors
                    titleBar.ButtonForegroundColor = Colors.Black;
                    titleBar.ButtonBackgroundColor = Colors.Transparent;
                    titleBar.ButtonHoverForegroundColor = Colors.Black;
                    titleBar.ButtonHoverBackgroundColor = Color.FromArgb(255, 180, 180, 180);
                    titleBar.ButtonPressedForegroundColor = Colors.Black;
                    titleBar.ButtonPressedBackgroundColor = Color.FromArgb(255, 150, 150, 150);

                    // Set inactive window colors
                    titleBar.InactiveForegroundColor = Colors.DimGray;
                    titleBar.InactiveBackgroundColor = Colors.Transparent;
                    titleBar.ButtonInactiveForegroundColor = Colors.DimGray;
                    titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

                    titleBar.BackgroundColor = Color.FromArgb(255, 210, 210, 210);
                }
                return true;
            }
            return false;
        }
    }
}
