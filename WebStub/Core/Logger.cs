using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace WebStub.Core
{
    public interface ILogger
    {
        void Info(string message = "", [CallerFilePath] string path = "", [CallerMemberName] string name = "", [CallerLineNumber] int line = 0);
        void Warn(string message = "", [CallerFilePath] string path = "", [CallerMemberName] string name = "", [CallerLineNumber] int line = 0);
        void Error(string message = "", [CallerFilePath] string path = "", [CallerMemberName] string name = "", [CallerLineNumber] int line = 0);
    }

    public class Logger
    {
        private static ILogger? _logger;
        private static readonly Object lockObj = new();

        public static void Error(string message = "", [CallerFilePath] string path = "", [CallerMemberName] string name = "", [CallerLineNumber] int line = 0)
        {
            Create();
            _logger!.Error(message, path, name, line);
        }

        public static void Info(string message = "", [CallerFilePath] string path = "", [CallerMemberName] string name = "", [CallerLineNumber] int line = 0)
        {
            Create();
            _logger!.Info(message, path, name, line);
        }

        public static void Warn(string message = "", [CallerFilePath] string path = "", [CallerMemberName] string name = "", [CallerLineNumber] int line = 0)
        {
            Create();
            _logger!.Warn(message, path, name, line);
        }

        private static void Create()
        {
            lock (lockObj)
            {
                _logger ??= new ConsolLogger();
            }
        }
    }

    public class ConsolLogger : ILogger
    {
        public void Error(string message = "", [CallerFilePath] string path = "", [CallerMemberName] string name = "", [CallerLineNumber] int line = 0)
        {
            Write(nameof(Error), message, path, name, line);
        }

        public void Info(string message = "", [CallerFilePath] string path = "", [CallerMemberName] string name = "", [CallerLineNumber] int line = 0)
        {
#if DEBUG
            Write(nameof(Info), message, path, name, line);
#endif
        }

        public void Warn(string message = "", [CallerFilePath] string path = "", [CallerMemberName] string name = "", [CallerLineNumber] int line = 0)
        {
            Write(nameof(Warn), message, path, name, line);
        }

        private void Write(string lebel, string message, string path, string name, int line)
        {
            Debug.WriteLine($"[{lebel}]{DateTime.Now} : {message} (Method : {name}, line : {line}, path : {path} )");
        }
    }

    public class BindableLogger : ILogger
    {
        private Action<string>? bindable;

        public BindableLogger()
        {
        }

        public void Bind(Action<string> action)
        {
            bindable = action;
        }

        public void Error(string message = "", [CallerFilePath] string path = "", [CallerMemberName] string name = "", [CallerLineNumber] int line = 0)
        {
            Write(nameof(Error), message, path, name, line);
        }

        public void Info(string message = "", [CallerFilePath] string path = "", [CallerMemberName] string name = "", [CallerLineNumber] int line = 0)
        {
            Write(nameof(Info), message, path, name, line);
        }

        public void Warn(string message = "", [CallerFilePath] string path = "", [CallerMemberName] string name = "", [CallerLineNumber] int line = 0)
        {
            Write(nameof(Warn), message, path, name, line);
        }

        private void Write(string lebel, string message, string path, string name, int line)
        {
#if DEBUG
            bindable?.Invoke($"[{lebel}]{DateTime.Now} : {message} (Method : {name}, line : {line}, path : {path} )");
#else
            bindable?.Invoke($"[{lebel}]{DateTime.Now} : {message} ");
#endif
        }
    }
}
