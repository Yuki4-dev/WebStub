using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using System.Reflection;
using WebStub.Core;

namespace WebStub
{
    internal class EditorWebView : WebView2
    {
        private bool useEditorTextChanged = false;
        private bool isNavigateStaticHtml = false;

        private readonly Uri url = new(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Assets\Html\index.html", UriKind.Absolute);

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
            CoreWebView2Initialized += EditorWebView_CoreWebView2Initialized;
            ActualThemeChanged += (s, e) =>
            {
                SetTheme();
            };
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

        private void EditorWebView_NavigationStarting(WebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationStartingEventArgs args)
        {
            if (isNavigateStaticHtml)
            {
                _ = ExecuteScriptAsync($"alert('{args.Uri} is not safe.')");
                args.Cancel = true;
            }

            isNavigateStaticHtml = true;
        }

        private void EditorWebView_CoreWebView2Initialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
        {
            CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
        }

        private void CoreWebView2_WebMessageReceived(Microsoft.Web.WebView2.Core.CoreWebView2 sender, Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs args)
        {
            var json = Json.FromJson<MessageJson>(args.WebMessageAsJson);
            if (json != null)
            {
                if (json.messageType == "system")
                {
                    if (json.message == "loaded")
                    {
                        SetTheme();
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

        private void SetTheme()
        {
            var message = new MessageJson
            {
                messageType = "theme",
                message = ActualTheme == ElementTheme.Dark ? "dark" : "light"
            };
            CoreWebView2?.PostWebMessageAsJson(Json.ToJson(message));
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
