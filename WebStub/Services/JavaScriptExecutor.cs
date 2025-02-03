namespace WebStub.Services
{
    public class JavaScriptExecutor
    {
        private static IJavaScriptExecutorWrapper? wrapper;

        public static Task<string> ExecuteAsync(string script)
        {
            if (wrapper == null) throw new InvalidOperationException(nameof(wrapper) + "is null.");

            var tcs = new TaskCompletionSource<string>();
            DispatcherService.Run(async () =>
            {
                var result = await wrapper.ExecuteJavaScriptAsync(script);
                tcs.SetResult(result);
            });

            return tcs.Task;
        }

        public static void SetExecutor(IJavaScriptExecutorWrapper? executor)
        {
            wrapper = executor ?? throw new ArgumentNullException(nameof(executor));
        }
    }

    public interface IJavaScriptExecutorWrapper
    {
        Task<string> ExecuteJavaScriptAsync(string script);
    }
}
