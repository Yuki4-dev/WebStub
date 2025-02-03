using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System;
using WebStub.Core;
using WebStub.Models;
using WebStub.Services;

namespace WebStub
{
    public class DI
    {
        public static T Get<T>() where T : class
        {
            T services = Ioc.Default.GetService<T>() ?? throw new InvalidOperationException($"UnRegister : {typeof(T)}.");
            return services;
        }

        public static void Injection()
        {
            var sc = new ServiceCollection();
            _ = sc.AddSingleton<ILogger, BindableLogger>();
            _ = sc.AddSingleton<IHttpService, HttpService>();
            _ = sc.AddSingleton<IDialogService, DialogService>();
            _ = sc.AddSingleton<ILocalApplicationDataService, LocalApplicationDataService>();
            _ = sc.AddSingleton<MainWindowViewModel>();

            Ioc.Default.ConfigureServices(sc.BuildServiceProvider());
        }
    }
}
