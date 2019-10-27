namespace Novellus.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Novellus.FormTests;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Xamarin.Forms;

    [TestClass]
    public class HybridWebViewTests
    {
        #region Const/Field
        private const string HarmonyId = @"org.chimata.Novellus.Form.HybridWebView";
        //private static HarmonyInstance harmonyInstance = null;

        private static string dummyRuntimePlatform = string.Empty;
        #endregion

        #region Stub
        public static bool Device_RuntimePlatform(dynamic __instance, ref string __result)
        {
            __result = dummyRuntimePlatform;
            return false;
        }
        #endregion Stub

        #region SetUp/TearDown
        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
        }

        [TestInitialize]
        public void SetUp()
        {
            // .Net Core 3 で動かない
            //harmonyInstance = HarmonyInstance.Create(HarmonyId);
            dummyRuntimePlatform = string.Empty;
        }

        [TestCleanup]
        public void TearDown()
        {
            //if (harmonyInstance.HasAnyPatches(HarmonyId)) {
            //    harmonyInstance.UnpatchAll(HarmonyId);
            //}
            //harmonyInstance = null;
            dummyRuntimePlatform = null;
        }
        #endregion SetUp/TearDown

        #region TestCase
        [Ignore("Lib.Harmonyが.Net Core 3で動いてくれないのでスタブが作れないため(現在テスト中とのこと)")]
        [TestMethod]
        [TestCategory(TestUtil.TestCategoryPositive)]
        public void StaticProperty_InvokerFunctionScriptTest001()
        {
            // Xamarin.Forms.Device.RuntimePlatform を差し替え
            //var originalRuntimePlatform = typeof(Device).GetProperty(nameof(Device.RuntimePlatform)).GetGetMethod();
            //var prefixRuntimePlatform = typeof(HybridWebViewTests).GetMethod(nameof(HybridWebViewTests.Device_RuntimePlatform));
            //harmonyInstance.Patch(originalRuntimePlatform, new HarmonyMethod(prefixRuntimePlatform));

            string functionDeclaration = $@"\bfunction\s+{HybridWebView.InvokerFunctionName}\s*(\s*\w+\s*)";

            // Android
            dummyRuntimePlatform = Device.Android;
            string androidScript = HybridWebView.InvokerFunctionScript;
            Assert.IsTrue(Regex.IsMatch(androidScript, functionDeclaration), $"関数宣言が不正です: Platform<{dummyRuntimePlatform}> Actual<{androidScript}>");

            // iOS
            dummyRuntimePlatform = Device.iOS;
            string iOSScript = HybridWebView.InvokerFunctionScript;
            Assert.IsTrue(Regex.IsMatch(iOSScript, functionDeclaration), $"関数宣言が不正です: Platform<{dummyRuntimePlatform}> Actual<{iOSScript}>");

            // macOS
            dummyRuntimePlatform = Device.macOS;
            string macOSScript = HybridWebView.InvokerFunctionScript;
            Assert.IsTrue(Regex.IsMatch(macOSScript, functionDeclaration), $"関数宣言が不正です: Platform<{dummyRuntimePlatform}> Actual<{macOSScript}>");

            // iOS と macOS は同じコードを使用する(Safari)
            Assert.AreEqual(iOSScript, macOSScript, $"{Device.iOS} と {Device.macOS} で結果が異なります");

            // UWP
            dummyRuntimePlatform = Device.UWP;
            string UWPScript = HybridWebView.InvokerFunctionScript;
            Assert.IsTrue(Regex.IsMatch(UWPScript, functionDeclaration), $"関数宣言が不正です: Platform<{dummyRuntimePlatform}> Actual<{UWPScript}>");

            // other
            dummyRuntimePlatform = Device.WPF;
            string otherScript = HybridWebView.InvokerFunctionScript;
            Assert.IsTrue(Regex.IsMatch(otherScript, functionDeclaration), $"関数宣言が不正です: Platform<{dummyRuntimePlatform}> Actual<{otherScript}>");

            // デフォルトは UWP と同じコードを使用する
            Assert.AreEqual(UWPScript, otherScript, $"{Device.UWP} と {Device.WPF} で結果が異なります");
        }

        [TestMethod]
        [TestCategory(TestUtil.TestCategoryPositive)]
        public void StaticMethod_GenerateFunctionScriptTest001_正常()
        {
            // Microsoft.JScript アセンブリの System.CodeDom.Compiler.JScriptCodeProvider 使いたい…

            string actionName = "hoge";
            string actual = HybridWebView.GenerateFunctionScript(actionName);

            Assert.IsTrue(actual.StartsWith("function"), "結果が'function'で始まっていません");

            string expectedFunctionDeclaration = @"^function (.+?)\((\w+)\)";
            Match matchFunction = Regex.Match(actual, expectedFunctionDeclaration);
            Assert.IsTrue(matchFunction.Success, $"関数定義が正しくありません: Actual<{actual}>");

            string actualFunctionName = matchFunction.Groups[1].Value;
            Assert.AreEqual(actionName, actualFunctionName, $"関数名が違います: Actual<{actualFunctionName}>");

            string actualArgName = matchFunction.Groups[2].Value;
            string expectedDataDefinition = $@"""{{\""action\"":\""{actionName}\"",\""data\"":\""""+{HybridWebView.EncoderFunctionName}({actualArgName})+""\""}}""";

            string expectedInvokerCode = $@"\b{HybridWebView.InvokerFunctionName}\((.*)\)";
            Match matchInvoker = Regex.Match(actual, expectedInvokerCode);
            Assert.IsTrue(matchInvoker.Success, $"ネイティブコード呼び出しが正しくありません: Actual<{actual}>");

            string actualDataDefinition = matchInvoker.Groups[1].Value;
            Assert.AreEqual(expectedDataDefinition, actualDataDefinition, $"リクエストデータの定義が正しくありません");
        }

        [TestMethod]
        [TestCategory(TestUtil.TestCategoryPositive)]
        public void StaticMethod_GenerateFunctionScriptTest002_ドル記号()
        {
            string actionName = "$";
            string actual = HybridWebView.GenerateFunctionScript(actionName);

            Assert.IsTrue(actual.StartsWith("function"), "結果が'function'で始まっていません");

            string expectedFunctionDeclaration = @"^function (.+?)\((\w+)\)";
            Match matchFunction = Regex.Match(actual, expectedFunctionDeclaration);
            Assert.IsTrue(matchFunction.Success, $"関数定義が正しくありません: Actual<{actual}>");

            string actualFunctionName = matchFunction.Groups[1].Value;
            Assert.AreEqual(actionName, actualFunctionName, $"関数名が違います: Actual<{actualFunctionName}>");

            string actualArgName = matchFunction.Groups[2].Value;
            string expectedDataDefinition = $@"""{{\""action\"":\""{actionName}\"",\""data\"":\""""+{HybridWebView.EncoderFunctionName}({actualArgName})+""\""}}""";

            string expectedInvokerCode = $@"\b{HybridWebView.InvokerFunctionName}\((.*)\)";
            Match matchInvoker = Regex.Match(actual, expectedInvokerCode);
            Assert.IsTrue(matchInvoker.Success, $"ネイティブコード呼び出しが正しくありません: Actual<{actual}>");

            string actualDataDefinition = matchInvoker.Groups[1].Value;
            Assert.AreEqual(expectedDataDefinition, actualDataDefinition, $"リクエストデータの定義が正しくありません");
        }

        [TestMethod]
        [TestCategory(TestUtil.TestCategoryPositive)]
        public void StaticMethod_GenerateFunctionScriptTest003_アンダースコア()
        {
            string actionName = "_";
            string actual = HybridWebView.GenerateFunctionScript(actionName);

            Assert.IsTrue(actual.StartsWith("function"), "結果が'function'で始まっていません");

            string expectedFunctionDeclaration = @"^function (.+?)\((\w+)\)";
            Match matchFunction = Regex.Match(actual, expectedFunctionDeclaration);
            Assert.IsTrue(matchFunction.Success, $"関数定義が正しくありません: Actual<{actual}>");

            string actualFunctionName = matchFunction.Groups[1].Value;
            Assert.AreEqual(actionName, actualFunctionName, $"関数名が違います: Actual<{actualFunctionName}>");

            string actualArgName = matchFunction.Groups[2].Value;
            string expectedDataDefinition = $@"""{{\""action\"":\""{actionName}\"",\""data\"":\""""+{HybridWebView.EncoderFunctionName}({actualArgName})+""\""}}""";

            string expectedInvokerCode = $@"\b{HybridWebView.InvokerFunctionName}\((.*)\)";
            Match matchInvoker = Regex.Match(actual, expectedInvokerCode);
            Assert.IsTrue(matchInvoker.Success, $"ネイティブコード呼び出しが正しくありません: Actual<{actual}>");

            string actualDataDefinition = matchInvoker.Groups[1].Value;
            Assert.AreEqual(expectedDataDefinition, actualDataDefinition, $"リクエストデータの定義が正しくありません");
        }

        [TestMethod]
        [TestCategory(TestUtil.TestCategoryNegative)]
        public void StaticMethod_GenerateFunctionScriptTest004_不正な関数名()
        {
            // https://www.ecma-international.org/ecma-262/6.0/
            // FunctionDeclaration :: function BindingIdentifier ( FormalParameters ) { FunctionBody }
            // BindingIdentifier :: Identifier
            // Identifier :: IdentifierName but not ReservedWord
            // IdentifierName :: IdentifierStart IdentifierName IdentifierPart
            // IdentifierStart :: UnicodeIDStart $ _ \ UnicodeEscapeSequence
            // IdentifierPart :: UnicodeIDContinue $ _ \ UnicodeEscapeSequence <ZWNJ> < ZWJ >
            // しんどいので半角英数字と $ _ だけ許可する

            string functionName = string.Empty;
            Assert.ThrowsException<ArgumentException>(() =>
            {
                HybridWebView.GenerateFunctionScript(functionName);
            }, $"空文字を使用しました: <{functionName}>");

            functionName = " ";
            Assert.ThrowsException<ArgumentException>(() =>
            {
                HybridWebView.GenerateFunctionScript(functionName);
            }, $"空白を使用しました: <{functionName}>");

            functionName = "　";
            Assert.ThrowsException<ArgumentException>(() =>
            {
                HybridWebView.GenerateFunctionScript(functionName);
            }, $"全角空白を使用しました: <{functionName}>");

            functionName = "０";
            Assert.ThrowsException<ArgumentException>(() =>
            {
                HybridWebView.GenerateFunctionScript(functionName);
            }, $"全角数字を使用しました: <{functionName}>");

            functionName = "０";
            Assert.ThrowsException<ArgumentException>(() =>
            {
                HybridWebView.GenerateFunctionScript(functionName);
            }, $"全角英字を使用しました: <{functionName}>");

            functionName = "＄";
            Assert.ThrowsException<ArgumentException>(() =>
            {
                HybridWebView.GenerateFunctionScript(functionName);
            }, $"全角記号を使用しました: <{functionName}>");

            functionName = "＿";
            Assert.ThrowsException<ArgumentException>(() =>
            {
                HybridWebView.GenerateFunctionScript(functionName);
            }, $"全角記号を使用しました: <{functionName}>");

            functionName = "a(b)";
            Assert.ThrowsException<ArgumentException>(() =>
            {
                HybridWebView.GenerateFunctionScript(functionName);
            }, $"不正な記号を使用しました: <{functionName}>");

            functionName = @""""; // アクション名にダブルクォートが入るとデータ構造が壊れるのではじく
            Assert.ThrowsException<ArgumentException>(() =>
            {
                HybridWebView.GenerateFunctionScript(functionName);
            }, $"不正な記号を使用しました: <{functionName}>");

            functionName = @"te st";
            Assert.ThrowsException<ArgumentException>(() =>
            {
                HybridWebView.GenerateFunctionScript(functionName);
            }, $"空白を含む名前を使用しました: <{functionName}>");

            functionName = @" test";
            Assert.ThrowsException<ArgumentException>(() =>
            {
                HybridWebView.GenerateFunctionScript(functionName);
            }, $"空白を含む名前を使用しました: <{functionName}>");

            functionName = @"test ";
            Assert.ThrowsException<ArgumentException>(() =>
            {
                HybridWebView.GenerateFunctionScript(functionName);
            }, $"空白を含む名前を使用しました: <{functionName}>");
        }

        [TestMethod]
        [TestCategory(TestUtil.TestCategoryNegative)]
        public void StaticMethod_GenerateFunctionScriptTest005_予約語()
        {
            // https://www.ecma-international.org/ecma-262/6.0/
            // ReservedWord :: Keyword FutureReservedWord NullLiteral BooleanLiteral
            // Keyword :: one of
            //      break   do  in	typeof
            //      case	else instanceof var
            //      catch   export  new void
            //      class extends return	while
            //      const   finally	super with
            //      continue	for	switch	yield
            //      debugger    function this
            //      default	if	throw	
            //      delete import	try
            //      let static
            // FutureReservedWord :: one of
            //      enum await
            //      implements package protected
            //      interface   private public
            // NullLiteral :: null
            // BooleanLiteral :: true false

            string keyword = Regex.Replace(@"
                  break   do  in	typeof
                  case	else instanceof var
                  catch   export  new void
                  class extends return	while
                  const   finally	super with
                  continue	for	switch	yield
                  debugger    function this
                  default	if	throw	
                  delete import	try
                  let static
            ".Trim(), @"\s+", ",");
            string futureReservedWord = Regex.Replace(@"
                  enum await
                  implements package protected
                  interface   private public
            ".Trim(), @"\s+", ",");
            string nullLiteral = "null";
            string booleanLiteral = "true,false";

            string[] reservedWord = string.Join(",", keyword, futureReservedWord, nullLiteral, booleanLiteral).Split(",");

            Assert.ThrowsException<ArgumentException>(() =>
            {
                HybridWebView.GenerateFunctionScript(HybridWebView.InvokerFunctionName);
            }, $"ネイティブコード呼び出し用の関数を上書きしてはいけません: '{HybridWebView.InvokerFunctionName}'");

            Assert.ThrowsException<ArgumentException>(() =>
            {
                HybridWebView.GenerateFunctionScript(HybridWebView.EncoderFunctionName);
            }, $"ネイティブコード呼び出しで使用している関数を上書きしてはいけません: '{HybridWebView.EncoderFunctionName}'");

            Assert.ThrowsException<ArgumentException>(() =>
            {
                HybridWebView.GenerateFunctionScript(HybridWebView.EncoderFunctionName);
            }, $"AndroidのJavascriptInterfaceオブジェクトを上書きしてはいけません: '{HybridWebView.AndroidJSBridgeName}'");

            foreach (var word in reservedWord)
            {
                Assert.ThrowsException<ArgumentException>(() =>
                {
                    HybridWebView.GenerateFunctionScript(word);
                }, $"JavaScriptの予約語を使用しました: '{word}'");
            }
        }

        [TestMethod]
        [TestCategory(TestUtil.TestCategoryPositive)]
        public void Property_UriTest001_正常()
        {
            using var webView = new HybridWebView();

            Assert.AreEqual(default(string), webView.Uri, "デフォルト値が不正です");

            string expectedUri = @"https://www.google.com/";
            webView.Uri = expectedUri;
            Assert.AreEqual(expectedUri, webView.Uri, "プロパティのセッターが動作しません");
        }

        [TestMethod]
        [TestCategory(TestUtil.TestCategoryPositive)]
        public void InjectJavaScriptAsyncTest001_正常()
        {
            static async Task<string> onJavaScriptInjectionRequestAsync(string js) => await Task.FromResult(js);

            string expectedInjectedJavaScript = string.Empty;
            string actualInjectedJavaScript = null;

            using var webView = new HybridWebView();
            try
            {
                webView.OnJavaScriptInjectionRequest += onJavaScriptInjectionRequestAsync;

                expectedInjectedJavaScript = "testcode";
                actualInjectedJavaScript = webView.InjectJavaScriptAsync(expectedInjectedJavaScript).ConfigureAwait(false).GetAwaiter().GetResult();
            }
            finally
            {
                webView.OnJavaScriptInjectionRequest -= onJavaScriptInjectionRequestAsync;
            }

            Assert.AreEqual(expectedInjectedJavaScript, actualInjectedJavaScript, $"イベントが発生しません: {nameof(HybridWebView.OnJavaScriptInjectionRequest)}");
        }

        [TestMethod]
        [TestCategory(TestUtil.TestCategoryNegative)]
        public void InjectJavaScriptAsyncTest002_コードが空()
        {
            static async Task<string> onJavaScriptInjectionRequestAsync(string js) => await Task.FromResult(js);

            using var webView = new HybridWebView();
            string actualResponse = null;
            try
            {
                webView.OnJavaScriptInjectionRequest += onJavaScriptInjectionRequestAsync;

                // コードが空文字のみの場合は空を返す
                actualResponse = webView.InjectJavaScriptAsync(" ").ConfigureAwait(false).GetAwaiter().GetResult();
            }
            finally
            {
                webView.OnJavaScriptInjectionRequest -= onJavaScriptInjectionRequestAsync;
            }
            Assert.AreEqual(string.Empty, actualResponse, $"予期しない値の返却: {nameof(HybridWebView.InjectJavaScriptAsync)}");
        }

        [TestMethod]
        [TestCategory(TestUtil.TestCategoryNegative)]
        public void InjectJavaScriptAsyncTest003_イベントが未登録()
        {
            using var webView = new HybridWebView();

            // イベントが未登録の場合は空を返す
            string actualResponse = webView.InjectJavaScriptAsync("testcode").ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.AreEqual(string.Empty, actualResponse, $"予期しない値の返却: {nameof(HybridWebView.InjectJavaScriptAsync)}");
        }

        [TestMethod]
        [TestCategory(TestUtil.TestCategoryPositive)]
        public void GetRegisteredActionNamesTest001_アクションが未登録()
        {
            using var webView = new HybridWebView();
            Exception actualException = null;
            string[] actualActionNames = null;
            try
            {
                actualActionNames = webView.GetRegisteredActionNames()?.ToArray();
            }
            catch (Exception ex)
            {
                actualException = ex;
            }
            Assert.IsNull(actualException, $"例外が発生しました: Message<{actualException?.Message}> StackTrace:\n{actualException?.StackTrace}");

            string[] expectedActionNames = Array.Empty<string>();
            Assert.IsNotNull(actualActionNames, $"戻り値がありません: Method<{nameof(HybridWebView.GetRegisteredActionNames)}>");
            Assert.AreEqual(expectedActionNames.Length, actualActionNames.Length, $"登録してないのに何かが返ってきました: Actual<{string.Join(", ", actualActionNames)}>");
        }

        [TestMethod]
        [TestCategory(TestUtil.TestCategoryPositive)]
        public void RegisterActionTest001_正常()
        {
            string expectedRegisterdActionName = string.Empty;
            string actualRegisterdActionName = null;
            void onCallbackAdded(object sender, string name) => actualRegisterdActionName = name;

            using var webView = new HybridWebView();
            try
            {
                HybridWebView.CallbackAdded += onCallbackAdded;

                expectedRegisterdActionName = "test";
                actualRegisterdActionName = null;
                webView.RegisterAction(expectedRegisterdActionName, _ => { });
                Assert.AreEqual(expectedRegisterdActionName, actualRegisterdActionName, $"イベントが発生しません: {nameof(HybridWebView.CallbackAdded)}");
            }
            finally
            {
                HybridWebView.CallbackAdded -= onCallbackAdded;
            }
        }

        [TestMethod]
        [TestCategory(TestUtil.TestCategoryNegative)]
        public void RegisterActionTest002_アクション名が空()
        {
            string actualRegisterdActionName = null;
            void onCallbackAdded(object sender, string name) => actualRegisterdActionName = "called";

            using var webView = new HybridWebView();
            try
            {
                HybridWebView.CallbackAdded += onCallbackAdded;

                actualRegisterdActionName = null;

                // コードが空文字のみの場合はイベントは発生しない
                webView.RegisterAction(" ", _ => { });
                Assert.IsNull(actualRegisterdActionName, $"予期しないイベントが発生しました: {nameof(HybridWebView.CallbackAdded)}");
            }
            finally
            {
                HybridWebView.CallbackAdded -= onCallbackAdded;
            }
        }

        [TestMethod]
        [TestCategory(TestUtil.TestCategoryNegative)]
        public void RegisterActionTest003_イベントが未登録()
        {
            using var webView = new HybridWebView();
            Exception actualException = null;
            try
            {
                webView.RegisterAction("test", _ => { });
            }
            catch (Exception ex)
            {
                actualException = ex;
            }
            Assert.IsNull(actualException, $"例外が発生しました: Message<{actualException?.Message}> StackTrace:\n{actualException?.StackTrace}");
        }

        [TestMethod]
        [TestCategory(TestUtil.TestCategoryPositive)]
        public void RemoveActionTest001_アクションが未登録()
        {
            using var webView = new HybridWebView();
            Exception actualException = null;
            try
            {
                webView.RemoveAction("test");
            }
            catch (Exception ex)
            {
                actualException = ex;
            }
            Assert.IsNull(actualException, $"例外が発生しました: Message<{actualException?.Message}> StackTrace:\n{actualException?.StackTrace}");
        }

        [TestMethod]
        [TestCategory(TestUtil.TestCategoryPositive)]
        public void RemoveAllActionsTest001_正常()
        {
            using var webView = new HybridWebView();
            webView.RegisterAction("test_a", _ => { });
            webView.RegisterAction("test_b", _ => { });
            webView.RegisterAction("test_c", _ => { });
            Assert.AreEqual(3, webView.GetRegisteredActionNames().Count(), "登録したアクションの数が一致しません");

            webView.RemoveAllActions();
            Assert.AreEqual(0, webView.GetRegisteredActionNames().Count(), "登録したアクションが消えていません");
        }

        [TestMethod]
        [TestCategory(TestUtil.TestCategoryPositive)]
        public void RemoveAllActionsTest002_アクションが未登録()
        {
            using var webView = new HybridWebView();
            Exception actualException = null;
            try
            {
                webView.RemoveAllActions();
            }
            catch (Exception ex)
            {
                actualException = ex;
            }
            Assert.IsNull(actualException, $"例外が発生しました: Message<{actualException?.Message}> StackTrace:\n{actualException?.StackTrace}");
        }

        [TestMethod]
        [TestCategory(TestUtil.TestCategoryPositive)]
        public void HandleScriptReceivedTest001_正常()
        {
            string unexpectedActionName = "unexpected_action_name";
            string expectedActionName = null;
            string expectedActionData = null;

            string eventData = null;
            IEnumerable<string> actualActionNames = null;
            string actualActionData = null;
            void action(string data) => actualActionData = $"my name is {data}";

            using var webView = new HybridWebView();

            expectedActionName = "expected_action_name";
            expectedActionData = $"my name is {expectedActionName}";
            actualActionData = null;
            webView.RegisterAction(unexpectedActionName, action);
            webView.RegisterAction(expectedActionName, action);
            actualActionNames = webView.GetRegisteredActionNames();
            Assert.AreEqual(expectedActionName, actualActionNames.FirstOrDefault(n => n == expectedActionName), $"アクションが登録されていません: Action<{expectedActionName}>");
            Assert.AreEqual(2, actualActionNames.Count(), $"アクションの登録数が一致しません");

            eventData = $@"{{""action"":""{expectedActionName}"",""data"":""{System.Uri.EscapeDataString(expectedActionName)}""}}";
            webView.HandleScriptReceived(eventData);
            Assert.AreEqual(expectedActionData, actualActionData, $"アクションが実行されません: Action<{expectedActionName}>");
        }

        [TestMethod]
        [TestCategory(TestUtil.TestCategoryNegative)]
        public void HandleScriptReceivedTest002_未登録のアクション呼び出し()
        {
            string unexpectedActionName = "unexpected_action_name";
            string expectedActionName = null;
            string expectedActionData = null;

            string eventData = null;
            IEnumerable<string> actualActionNames = null;
            string actualActionData = null;
            void action(string data) => actualActionData = $"my name is {data}";

            using var webView = new HybridWebView();

            expectedActionName = "expected_action_name";
            expectedActionData = $"my name is {expectedActionName}";
            actualActionData = null;
            webView.RegisterAction(unexpectedActionName, action);
            webView.RegisterAction(expectedActionName, action);
            webView.RemoveAction(unexpectedActionName);
            actualActionNames = webView.GetRegisteredActionNames();
            Assert.AreEqual(expectedActionName, actualActionNames.FirstOrDefault(n => n == expectedActionName), $"アクションが登録されていません: Action<{expectedActionName}>");
            Assert.AreEqual(1, actualActionNames.Count(), $"アクションの登録数が一致しません");

            eventData = $@"{{""action"":""{unexpectedActionName}"",""data"":""{System.Uri.EscapeDataString(unexpectedActionName)}""}}";
            webView.HandleScriptReceived(eventData);
            Assert.IsNull(actualActionData, $"予期しないアクションが実行されました: Action<{unexpectedActionName}> Actual Result<{actualActionData}>");
        }

        [TestMethod]
        [TestCategory(TestUtil.TestCategoryNegative)]
        public void HandleScriptReceivedTest003_引数が空()
        {
            string eventData = null;
            string unexpectedActionName = "unexpected_action_name";
            string actualActionData = null;
            void action(string data) => actualActionData = $"my name is {data}";

            using var webView = new HybridWebView();
            webView.RegisterAction(unexpectedActionName, action);

            Exception actualException = null;
            try
            {
                actualException = null;
                actualActionData = null;
                eventData = null;
                webView.HandleScriptReceived(eventData);
            }
            catch (Exception ex)
            {
                actualException = ex;
            }
            Assert.IsNull(actualException, $"例外が発生しました: Args<null> Message<{actualException?.Message}> StackTrace:\n{actualException?.StackTrace}");

            try
            {
                actualException = null;
                actualActionData = null;
                eventData = " ";
                webView.HandleScriptReceived(eventData);
            }
            catch (Exception ex)
            {
                actualException = ex;
            }
            Assert.IsNull(actualException, $"例外が発生しました: Args< > Message<{actualException?.Message}> StackTrace:\n{actualException?.StackTrace}");
        }

        [TestMethod]
        [TestCategory(TestUtil.TestCategoryNegative)]
        public void HandleScriptReceivedTest004_アクション名が空()
        {
            string eventData = null;
            string unexpectedActionName = "unexpected_action_name";
            string actualActionData = null;
            void action(string data) => actualActionData = $"my name is {data}";

            using var webView = new HybridWebView();
            webView.RegisterAction(unexpectedActionName, action);

            Exception actualException = null;
            try
            {
                actualException = null;
                actualActionData = null;
                eventData = $@"{{""action"":null,""data"":""{System.Uri.EscapeDataString(unexpectedActionName)}""}}";
                webView.HandleScriptReceived(eventData);
            }
            catch (Exception ex)
            {
                actualException = ex;
            }
            Assert.IsNull(actualException, $"例外が発生しました: Action<null> Message<{actualException?.Message}> StackTrace:\n{actualException?.StackTrace}");

            try
            {
                actualException = null;
                actualActionData = null;
                eventData = $@"{{""action"":"" "",""data"":""{System.Uri.EscapeDataString(unexpectedActionName)}""}}";
                webView.HandleScriptReceived(eventData);
            }
            catch (Exception ex)
            {
                actualException = ex;
            }
            Assert.IsNull(actualException, $"例外が発生しました: Action< > Message<{actualException?.Message}> StackTrace:\n{actualException?.StackTrace}");
        }

        [TestMethod]
        [TestCategory(TestUtil.TestCategoryNegative)]
        public void HandleScriptReceivedTest004_データが空()
        {
            string eventData = null;
            string unexpectedActionName = "unexpected_action_name";
            string actualActionData = null;
            void action(string data) => actualActionData = $"my name is {data}";

            using var webView = new HybridWebView();
            webView.RegisterAction(unexpectedActionName, action);

            Exception actualException = null;
            try
            {
                actualException = null;
                actualActionData = null;
                eventData = $@"{{""action"":""{unexpectedActionName}"",""data"":null}}";
                webView.HandleScriptReceived(eventData);
            }
            catch (Exception ex)
            {
                actualException = ex;
            }
            Assert.IsNull(actualException, $"例外が発生しました: Data<null> Message<{actualException?.Message}> StackTrace:\n{actualException?.StackTrace}");

            try
            {
                actualException = null;
                actualActionData = null;
                eventData = $@"{{""action"":""{unexpectedActionName}"",""data"":"" ""}}";
                webView.HandleScriptReceived(eventData);
            }
            catch (Exception ex)
            {
                actualException = ex;
            }
            Assert.IsNull(actualException, $"例外が発生しました: Data< > Message<{actualException?.Message}> StackTrace:\n{actualException?.StackTrace}");
        }

        [TestMethod]
        [TestCategory(TestUtil.TestCategoryPositive)]
        public void DisposeTest001_正常()
        {
            using var webView = new HybridWebView();
            webView.RegisterAction("test_a", _ => { });
            webView.RegisterAction("test_b", _ => { });

            webView.Dispose();

            var actionNames = webView.GetRegisteredActionNames();
            Assert.IsFalse(actionNames.Any(), $"アクションが削除されていません: Actual<{string.Join(", ", actionNames)}>");
        }
        #endregion
    }
}
