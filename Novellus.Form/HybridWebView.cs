using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Novellus.Models;
using Xamarin.Forms;

namespace Novellus
{
    public class HybridWebView : View, IDisposable
    {
        public readonly Dictionary<string, Action<string>> RegisteredCallbacks = new Dictionary<string, Action<string>>();

        public delegate Task<string> JavascriptInjectionRequestDelegate(string js);
        public event JavascriptInjectionRequestDelegate OnJavascriptInjectionRequest;

        public static event EventHandler<string> CallbackAdded;

        public static readonly BindableProperty UriProperty = BindableProperty.Create(
          propertyName: "Uri",
          returnType: typeof(string),
          declaringType: typeof(HybridWebView),
          defaultValue: default(string));

        public async Task<string> InjectJavascriptAsync(string js)
        {
            if (string.IsNullOrWhiteSpace(js)) return string.Empty;

            if (OnJavascriptInjectionRequest != null)
            {
                return await OnJavascriptInjectionRequest.Invoke(js);
            }

            return string.Empty;
        }

        public string Uri
        {
            get { return (string)GetValue(UriProperty); }
            set { SetValue(UriProperty, value); }
        }

        public void RegisterAction(string functionName, Action<string> action)
        {
            if (string.IsNullOrWhiteSpace(functionName)) return;

            if (RegisteredCallbacks.ContainsKey(functionName))
            {
                RegisteredCallbacks.Remove(functionName);
            }

            RegisteredCallbacks.Add(functionName, action);
            CallbackAdded?.Invoke(this, functionName);
        }

        public void RemoveAction(string functionName)
        {
            if (RegisteredCallbacks.ContainsKey(functionName))
            {
                RegisteredCallbacks.Remove(functionName);
            }
        }

        public void RemoveAllActions()
        {
            RegisteredCallbacks.Clear();
        }

        public void HandleScriptReceived(string data)
        {
            if (string.IsNullOrWhiteSpace(data)) return;

            var action = JsonConvert.DeserializeObject<ActionEvent>(data);

            // Decode
            //byte[] dBytes = Convert.FromBase64String(action.Data);
            //action.Data = Encoding.UTF8.GetString(dBytes, 0, dBytes.Length);
            action.Data = System.Uri.UnescapeDataString(action.Data);

            if (RegisteredCallbacks.ContainsKey(action.Action)) {
                RegisteredCallbacks[action.Action]?.Invoke(action.Data);
            }
        }

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

        public static string GenerateFunctionScript(string name)
        {
            return $"function {name}(str){{csharp(\"{{'action':'{name}','data':'\"+encodeURIComponent(str)+\"'}}\");}}";
        }

        public void Dispose()
        {
            RegisteredCallbacks.Clear();
        }
    }
}

