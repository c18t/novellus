namespace Novellus
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Newtonsoft.Json;
    using Novellus.Models;
    using Xamarin.Forms;

    public class HybridWebView : View, IDisposable
    {
        public static readonly BindableProperty UriProperty = BindableProperty.Create(
            propertyName: "Uri",
            returnType: typeof(string),
            declaringType: typeof(HybridWebView),
            defaultValue: default(string));

        private readonly Dictionary<string, Action<string>> registeredCallbacks = new Dictionary<string, Action<string>>();

        public delegate Task<string> JavascriptInjectionRequestDelegate(string js);

        public static event EventHandler<string> CallbackAdded;

        public event JavascriptInjectionRequestDelegate OnJavascriptInjectionRequest;

        public static string InjectedFunction
        {
            get
            {
                switch (Device.RuntimePlatform)
                {
                    case Device.Android:
                        return "function csharp(data){jsBridge.invokeAction(data);}";

                    case Device.iOS:
                    case "macOS":
                        return "function csharp(data){window.webkit.messageHandlers.invokeAction.postMessage(data);}";

                    default:
                        return "function csharp(data){window.external.notify(data);}";
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
            return $"function {name}(str){{csharp(\"{{'action':'{name}','data':'\"+encodeURIComponent(str)+\"'}}\");}}";
        }

        public async Task<string> InjectJavascriptAsync(string js)
        {
            if (string.IsNullOrWhiteSpace(js))
            { 
                return string.Empty;
            }

            if (!(this.OnJavascriptInjectionRequest is null))
            {
                return await this.OnJavascriptInjectionRequest.Invoke(js);
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

        public void Dispose()
        {
            this.registeredCallbacks.Clear();
        }
    }
}
