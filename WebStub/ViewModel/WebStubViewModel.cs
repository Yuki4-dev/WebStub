using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using WebStub.Core;
using WebStub.Models;
using WebStub.Services;

namespace WebStub.ViewModel
{
    public abstract class WebStubViewModel : ObservableObject, IRecipient<CloseApplicationMessage>
    {
        private const int server_error_max_count = 5;

        private const string initilyze_javascript = $"/**\r\n * @param request {{ HttpRequest }}\r\n * @param meta {{ Meta }}\r\n * @returns {{ HttpResponse }}\r\n */\r\nfunction server(request, meta) {{\r\n    return {{ \"status\": 200, \"body\": \"OK\" }}\r\n\r\n    class Meta {{\r\n        /**\r\n         * @type {{ string }}\r\n         */\r\n        file\r\n    }}\r\n\r\n    class HttpRequest {{\r\n        /**\r\n         * @type {{ string }}\r\n         */\r\n        uri\r\n        /**\r\n         * @type {{ \"GET\"|\"POST\"|\"PUT\"|\"DELETE\" }}\r\n         */\r\n        method\r\n        /**\r\n         * @type {{ string }}\r\n         */\r\n        body\r\n        /**\r\n         * @type {{ Record<string, string[]> }}\r\n         */\r\n        header\r\n        /**\r\n         * @type {{ Record<string, string[]> }}\r\n         */\r\n        cookie\r\n        /**\r\n         * @type {{ Record<string, string[]> }}\r\n         */\r\n        parameter\r\n    }}\r\n\r\n    class HttpResponse {{\r\n        /**\r\n         * @type {{ number }}\r\n         */\r\n        status\r\n        /**\r\n         * @type {{ string }}\r\n         */\r\n        body\r\n        /**\r\n         * @type {{ Record<string, string[]> }}\r\n         */\r\n        header\r\n        /**\r\n         * @type {{ Record<string, string[]> }}\r\n         */\r\n        cookie\r\n    }}\r\n}}\r\n";

        private readonly ILogger logger;

        private readonly IHttpService httpService;

        private readonly ILocalApplicationDataService localApplicationDataService;

        public abstract string FilePath { get; set; }

        public abstract string JavaScript { get; set; }

        public WebStubViewModel(ILogger logger, IHttpService httpService, ILocalApplicationDataService localApplicationDataService)
        {
            this.logger = logger;
            this.httpService = httpService;
            this.localApplicationDataService = localApplicationDataService;

            this.httpService.ErrorHandle += (arg) =>
            {
                logger.Error(arg.Message);
                return server_error_max_count < arg.Count;
            };

            DispatcherService.Run(async () =>
            {
                var js = await this.localApplicationDataService.GetLocalDataAsync("javascript");
                if (string.IsNullOrEmpty(js))
                {
                    JavaScript = initilyze_javascript;
                }
                else
                {
                    JavaScript = js;
                }
            });

            WeakReferenceMessenger.Default.Register(this);
        }

        public void Receive(CloseApplicationMessage message)
        {
            _ = localApplicationDataService.SetLocalDataAsync("javascript", JavaScript);
        }

        protected async Task OpenServerAsync(int port)
        {
            if (httpService.IsListening)
            {
                logger.Warn("Server aleady open.");
                return;
            }

            try
            {
                logger.Info("Server start.");
                await httpService.ListenAsync("http://localhost:" + port + "/", Server);
            }
            catch (Exception ex)
            {
                logger.Error("Server faild.");
                logger.Error(ex.Message);
                throw;
            }
        }

        protected void CloseServer()
        {
            if (!httpService.IsListening)
            {
                logger.Warn("Server not open.");
                return;
            }

            logger.Info("Server close.");
            httpService.Stop();
        }

        protected void InitilyzeJavascript()
        {
            JavaScript = initilyze_javascript;
        }

        private async Task<HttpResponse> Server(HttpRequest request)
        {
            var fileValue = string.Empty;
            if (!string.IsNullOrEmpty(FilePath))
            {
                try
                {
                    fileValue = File.ReadAllText(FilePath);
                }
                catch (Exception ex)
                {
                    logger.Error(ex.Message);
                }
            }

            logger.Info("Run JavaScript.");
            var script = GetJavaScript(request.ToJson(), new MetaJson() { file = fileValue });
            var javaScriptResult = await JavaScriptExecutor.ExecuteAsync(script);

            logger.Info($"JavaScript result -> {javaScriptResult ?? "null"}");
            if (javaScriptResult == null)
            {
                return HttpResponse.Empty;
            }

            var response = Json.FromJson<HttpResponseJson>(javaScriptResult);
            if (response == null)
            {
                return HttpResponse.Empty;
            }

            return HttpResponse.FromJson(response);
        }

        private string GetJavaScript(HttpRequestJson requestJson, MetaJson metaJson)
        {
            return "(function(){" +
                "const window = {};" +
                "const document = {};" +
                "const navigator = {};" +
                $"const request = {Json.ToJson(requestJson)};" +
                $"const meta = {Json.ToJson(metaJson)};" +
                $"return ({JavaScript}(request,meta))" +
                "}());";
        }
    }
}
