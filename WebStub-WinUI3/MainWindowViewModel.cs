using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using WebStub.Core;
using WebStub.Models;
using WebStub.Services;
using WebStub.ViewModel;

namespace WebStub
{
    public partial class MainWindowViewModel : WebStubViewModel
    {
        [ObservableProperty]
        public override partial string FilePath { get; set; } = string.Empty;

        [ObservableProperty]
        public override partial string JavaScript { get; set; } = string.Empty;

        public bool NotBusy => !Running;

        [ObservableProperty]
        public partial int Port { get; set; } = 8080;

        [ObservableProperty]
        public partial string LogText { get; set; } = string.Empty;

        [ObservableProperty]
        public partial bool Running { get; set; } = false;

        private readonly IDialogService dialogService;

        public MainWindowViewModel(ILogger logger, IHttpService httpService, ILocalApplicationDataService localApplicationDataService, IDialogService dialogService) : base(logger, httpService, localApplicationDataService)
        {
            this.dialogService = dialogService;

            if (logger is BindableLogger bindable)
            {
                bindable.Bind((log) =>
                {
                    DispatcherService.Run(() =>
                    {
                        LogText = LogText + log + Environment.NewLine;
                    });
                });
            }
        }

        [RelayCommand]
        private void ClearLog()
        {
            LogText = string.Empty;
        }

        [RelayCommand]
        private async Task Refresh()
        {
            var result = await dialogService.ShowConfirmDialogAsync("Initialize Javascript?");
            if (result)
            {
                InitilyzeJavascript();
            }
        }

        partial void OnRunningChanging(bool oldValue, bool newValue)
        {
            if (newValue)
            {
                _ = OpenServerAsync(Port).ContinueWith(task =>
                {
                    if (task.IsCanceled || task.IsFaulted)
                    {
                        Running = false;
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            else
            {
                CloseServer();
            }
        }

        partial void OnRunningChanged(bool value)
        {
            OnPropertyChanged(nameof(NotBusy));
        }
    }
}
