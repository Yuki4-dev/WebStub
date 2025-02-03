namespace WebStub.Models
{
    public interface IDialogService
    {
        public Task ShowMessageDialogAsync(string message, string? title = null);

        public Task<bool> ShowConfirmDialogAsync(string message, string? title = null);
    }
}
