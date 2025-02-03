using System.Windows;
using WebStub.Models;

namespace WebStub
{
    public class DialogService : IDialogService
    {
        public Task<bool> ShowConfirmDialogAsync(string message, string? title = null)
        {
            var result = MessageBox.Show(message, title ?? "Message", MessageBoxButton.OKCancel);
            return Task.FromResult(result == MessageBoxResult.OK);
        }

        public Task ShowMessageDialogAsync(string message, string? title = null)
        {
            _ = MessageBox.Show(message, title ?? "Message");
            return Task.CompletedTask;
        }
    }
}
