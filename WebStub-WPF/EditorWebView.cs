using Microsoft.Web.WebView2.Wpf;
using System.IO;
using System.Windows;
using WebStub.Core;

namespace WebStub
{
    internal class EditorWebView : WebView2
    {
        private bool useEditorTextChanged = false;
        private bool isNavigateStaticHtml = false;

        private readonly Uri url = new(Path.GetDirectoryName(AppContext.BaseDirectory) + @"\Assets\Html\index.html", UriKind.Absolute);

        public static readonly DependencyProperty TextProperty
            = DependencyProperty.Register(
                nameof(Text),
                typeof(string),
                typeof(EditorWebView),
                new PropertyMetadata(string.Empty, OnTextPropertyChanged));

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public EditorWebView()
        {
            Source = url;
            NavigationStarting += EditorWebView_NavigationStarting;
        }

        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is EditorWebView webView)
            {
                if (!webView.useEditorTextChanged)
                {
                    webView.SetEditorText((string)e.NewValue);
                }
            }
        }

        private void EditorWebView_NavigationStarting(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs e)
        {
            if (isNavigateStaticHtml)
            {
                _ = ExecuteScriptAsync($"alert('{e.Uri} is not safe.')");
                e.Cancel = true;
            }

            isNavigateStaticHtml = true;
            CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
        }

        private void CoreWebView2_WebMessageReceived(object? sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {
            MessageJson? json = Json.FromJson<MessageJson>(e.WebMessageAsJson);
            if (json != null)
            {
                if (json.messageType == "system")
                {
                    if (json.message == "loaded")
                    {
                        SetEditorText(Text);
                    }
                }
                else if (json.messageType == "data")
                {
                    useEditorTextChanged = true;
                    Text = json.message;
                    useEditorTextChanged = false;
                }
                else
                {
                    throw new Exception("Unkown messageType : " + json.messageType);
                }
            }

        }

        private void SetEditorText(string text)
        {
            var message = new MessageJson
            {
                messageType = "data",
                message = text
            };
            CoreWebView2?.PostWebMessageAsJson(Json.ToJson(message));
        }

        private class MessageJson
        {
            public string messageType { get; set; } = string.Empty;
            public string message { get; set; } = string.Empty;
        }
    }
}
