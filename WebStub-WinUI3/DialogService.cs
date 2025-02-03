using Microsoft.UI.Xaml.Controls;
using System;
using System.Threading.Tasks;
using WebStub.Models;

namespace WebStub
{
    public class DialogService : IDialogService
    {
        public async Task<bool> ShowConfirmDialogAsync(string message, string? title = null)
        {
            if (App.Window == null)
            {
                throw new InvalidOperationException(nameof(App.Window) + "is null.");
            }

            var d = new ContentDialog()
            {
                Title = title ?? "Message",
                Content = message,
                PrimaryButtonText = "OK",
                SecondaryButtonText = "Cancel",
                XamlRoot = App.Window.Content.XamlRoot,
            };

            var result = await d.ShowAsync();

            return result == ContentDialogResult.Primary;
        }

        public async Task ShowMessageDialogAsync(string message, string? title = null)
        {
            if (App.Window == null)
            {
                throw new InvalidOperationException(nameof(App.Window) + "is null.");
            }

            var d = new ContentDialog()
            {
                Title = title ?? "Message",
                Content = message,
                CloseButtonText = "Close",
                XamlRoot = App.Window.Content.XamlRoot,
            };

            _ = await d.ShowAsync();
        }
    }
}
