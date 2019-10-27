namespace Novellus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Novellus.Models;
    using Xamarin.Forms;

    public class HybridWebView : View, IDisposable
    {
        public static readonly string InvokerFunctionName = "csharp";

        public static readonly string EncoderFunctionName = "encodeURIComponent";

        public static readonly string AndroidJSBridgeName = "jsBridge";

        public const string JavaScriptMessageHandlerName = "invokeAction";

        public static readonly BindableProperty UriProperty = BindableProperty.Create(
            propertyName: nameof(Uri),
            returnType: typeof(string),
            declaringType: typeof(HybridWebView),
            defaultValue: default(string));

        private const string FunctionIdentifierRegex = @"^[A-Za-z0-9$_]+$";

        private static readonly string[] reservedFunctionNames = {
            InvokerFunctionName,
            EncoderFunctionName,
            AndroidJSBridgeName,
            "await", "break", "case", "catch", "class", "const", "continue", "debugger",
            "default", "delete", "do", "else", "enum", "export", "extends", "false",
            "finally", "for", "function", "if", "implements", "import", "in", "instanceof",
            "interface", "let", "new", "null", "package", "private", "protected", "public",
            "return", "static", "super", "switch", "this", "throw", "true", "try", "typeof",
            "var", "void", "while", "with", "yield",
        };

        private readonly Dictionary<string, Action<string>> registeredCallbacks = new Dictionary<string, Action<string>>();

        public delegate Task<string> JavaScriptInjectionRequestDelegate(string js);

        public static event EventHandler<string> CallbackAdded;

        public event JavaScriptInjectionRequestDelegate OnJavaScriptInjectionRequest;

        public static string InvokerFunctionScript
        {
            get
            {
                switch (Device.RuntimePlatform)
                {
                    case Device.Android:
                        return $@"function {InvokerFunctionName}(data){{{AndroidJSBridgeName}.{JavaScriptMessageHandlerName}(data);}}";

                    case Device.iOS:
                    case Device.macOS:
                        return $@"function {InvokerFunctionName}(data){{window.webkit.messageHandlers.{JavaScriptMessageHandlerName}.postMessage(data);}}";

                    default:
                        return $@"function {InvokerFunctionName}(data){{window.external.notify(data);}}";
                }
            }
        }

        public string Uri
        {
            get { return (string)this.GetValue(UriProperty); }
            set { this.SetValue(UriProperty, value); }
        }

        public static string GenerateFunctionScript(string name)
        {
            if (!Regex.IsMatch(name, FunctionIdentifierRegex))
            {
                throw new ArgumentException($"関数名は次のパターンに一致しなければなりません: /{FunctionIdentifierRegex}/", nameof(name));
            }

            if (reservedFunctionNames.Any(s => name == s))
            {
                throw new ArgumentException($"次の関数名は予約されています: {string.Join(", ", reservedFunctionNames.Select(s => $"'{s}'"))}", nameof(name));
            }

            return $@"function {name}(str){{{InvokerFunctionName}(""{{\""action\"":\""{name}\"",\""data\"":\""""+{EncoderFunctionName}(str)+""\""}}"");}}";
        }

        public async Task<string> InjectJavaScriptAsync(string js)
        {
            if (string.IsNullOrWhiteSpace(js))
            { 
                return string.Empty;
            }

            if (!(this.OnJavaScriptInjectionRequest is null))
            {
                return await this.OnJavaScriptInjectionRequest.Invoke(js);
            }

            return string.Empty;
        }

        public IEnumerable<string> GetRegisteredActionNames()
        {
            return this.registeredCallbacks.Keys;
        }

        public void RegisterAction(string functionName, Action<string> action)
        {
            if (string.IsNullOrWhiteSpace(functionName))
            {
                return;
            }

            if (this.registeredCallbacks.ContainsKey(functionName))
            {
                this.registeredCallbacks.Remove(functionName);
            }

            this.registeredCallbacks.Add(functionName, action);
            CallbackAdded?.Invoke(this, functionName);
        }

        public void RemoveAction(string functionName)
        {
            if (this.registeredCallbacks.ContainsKey(functionName))
            {
                this.registeredCallbacks.Remove(functionName);
            }
        }

        public void RemoveAllActions()
        {
            this.registeredCallbacks.Clear();
        }

        public void HandleScriptReceived(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
            {
                return;
            }

            var action = JsonConvert.DeserializeObject<ActionEvent>(data);

            if (!string.IsNullOrEmpty(action.Data))
            {
                action.Data = System.Uri.UnescapeDataString(action.Data);
            }

            if (this.registeredCallbacks.ContainsKey(action.Action))
            {
                this.registeredCallbacks[action.Action]?.Invoke(action.Data);
            }
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.registeredCallbacks.Clear();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
