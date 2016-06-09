#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Web;
using System.Web.UI;
using DevExpress.Persistent.Base;
[assembly: WebResource("DevExpress.ExpressApp.Web.Resources.TestControlsFactory.js", "text/javascript")]
namespace DevExpress.ExpressApp.Web.TestScripts {
	public enum TestControlType {
		Action = 1,
		Field = 2,
		Table = 3,
		Message = 4, 
	}
	public class TestControlsImplemantationsList {
		private Dictionary<string, IJScriptTestControl> testControlsTypes = new Dictionary<string, IJScriptTestControl>();
		public TestControlsImplemantationsList() {
			AssemblyName jScriptTestControlAssembly = typeof(IJScriptTestControl).Assembly.GetName();
			foreach(Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
				try {
					bool isRefToTestControlAssembly = AssemblyName.ReferenceMatchesDefinition(assembly.GetName(), jScriptTestControlAssembly);
					if (!isRefToTestControlAssembly) {
						foreach (AssemblyName referencedAssembly in assembly.GetReferencedAssemblies()) {
							if (AssemblyName.ReferenceMatchesDefinition(jScriptTestControlAssembly, referencedAssembly)) {
								isRefToTestControlAssembly = true;
								break;
							}
						}
					}
					if (isRefToTestControlAssembly) {
						foreach (Type type in assembly.GetTypes()) {
							if (typeof(IJScriptTestControl).IsAssignableFrom(type) && !type.IsAbstract && type.IsClass) {
								IJScriptTestControl testControl = Activator.CreateInstance(type) as IJScriptTestControl;
								if (testControlsTypes.ContainsKey(testControl.JScriptClassName)) {
									throw new Exception("Test control implementation with name " + testControl.JScriptClassName + " is already exist");
								}
								testControlsTypes.Add(testControl.JScriptClassName, testControl);
							}
						}
					}
				}
				catch(ReflectionTypeLoadException e) {
					Tracing.Tracer.LogError(e);
					Tracing.Tracer.LogText("Assembly: " + assembly.FullName);
				}
			}
		}
		public IJScriptTestControl[] Items {
			get {
				IJScriptTestControl[] result = new IJScriptTestControl[testControlsTypes.Count];
				testControlsTypes.Values.CopyTo(result, 0);
				return result;
			}
		}
		public string GetScript() {
			StringBuilder sb = new StringBuilder();
			foreach(IJScriptTestControl testControl in testControlsTypes.Values) {
				if(sb.Length != 0) {
					sb.AppendLine();
				}
				sb.Append(testControl.ScriptsDeclaration.GetJScript(testControl.JScriptClassName));
			}
			return sb.ToString();
		}
	}
	[Serializable]
	public class ScriptFileCreatingException : Exception {
		private HttpContext httpContext;
		protected ScriptFileCreatingException(SerializationInfo info, StreamingContext context) : base(info, context) { }
		public ScriptFileCreatingException(HttpContext httpContext, string message)
			: base(message) {
			this.httpContext = httpContext;
		}
		public ScriptFileCreatingException(HttpContext httpContext, string message, Exception innerException)
			: base(message, innerException) {
			this.httpContext = httpContext;
		}
		public HttpContext HttpContext {
			get { return httpContext; }
		}
	}
	public class TestControlDescription {
		string getCode = null;
		string jsClassName;
		string clientID;
		TestControlType controlType;
		string caption;
		string fullCaption;
		string additionalParameters;
		public TestControlDescription(string jsClassName, string clientID, TestControlType controlType, string caption, string fullCaption, string additionalParameters) {
			this.jsClassName = jsClassName;
			this.clientID = clientID;
			this.controlType = controlType;
			this.caption = caption;
			this.fullCaption = fullCaption;
			this.additionalParameters = additionalParameters;
		}
		public string JsClassName { get { return jsClassName; } }
		public string ClientID {
			get { return clientID; }
			set {
				if(ClientID == value) return;
				clientID = value;
				ResetGetCode();
			}
		}
		public TestControlType ControlType { get { return controlType; } }
		public string Caption {
			get { return caption; }
			set {
				if(Caption == value) return;
				caption = value;
				ResetGetCode();
			}
		}
		public string FullCaption {
			get { return fullCaption; }
			set {
				if(FullCaption == value) return;
				fullCaption = value;
				ResetGetCode();
			}
		}
		public string AdditionalParameters {
			get { return additionalParameters; }
			set { additionalParameters = value; }
		}
		public string GetCode() {
			if(!string.IsNullOrEmpty(this.getCode)) return this.getCode;
			this.getCode = string.Format("TestControl('{0}', '{1}', '{2}', '{3}', '{4}'{5});",
				jsClassName, clientID, controlType.ToString(), caption, fullCaption, additionalParameters);
			return this.getCode;
		}
		public override bool Equals(object obj) {
			TestControlDescription other = obj as TestControlDescription;
			if(other != null) {
				return this.ControlType == other.ControlType && this.Caption == other.Caption && this.FullCaption == other.FullCaption;
			}
			else {
				return base.Equals(obj);
			}
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		void ResetGetCode() {
			this.getCode = null;
		}
		public override string ToString() {
			return base.ToString() + String.Format(" Caption: '{0}', FullCaption '{1}'", caption, fullCaption);
		}
	}
	public class TestScriptsManager : IXafHttpHandler {
		public const string StaticTestControlsScriptInclude = "DXX.axd?handlerName=TestControls";
		private static int count = 0;
		private static Dictionary<int, List<TestControlDescription>> listOfCodes = new Dictionary<int, List<TestControlDescription>>();
		private static bool easyTestEnabled = false;
		private static string testControlsFactory = null; 
		private string commentCode; 
		private object key;
		private Type webWindowType;
		private static object lockObject = new object();
		public List<TestControlDescription> TestControlDescriptions {
			get {
				List<TestControlDescription> result = null;
				lock(lockObject) {
					if(!listOfCodes.TryGetValue(KeyHashCode, out result)) {
						result = new List<TestControlDescription>();
						listOfCodes.Add(KeyHashCode, result);
					}
				}
				return result;
			}
		}
		private Page GetPage() {
			if(key is Page) {
				return (Page)key;
			}
			else if(key is WebWindow && ((WebWindow)key).Template != null && ((WebWindow)key).Template is Page) {
				return ((Page)((WebWindow)key).Template);
			}
			return null;
		}
		private int KeyHashCode {
			get {
				if(key != null) {
					return key.GetHashCode();
				}
				else {
					return -1;
				}
			}
		}
		[Browsable(false)]
		public static void Init() {
			Tracing.Tracer.LogText(typeof(TestScriptsManager).Name + ".Init");
			count = 0;
			listOfCodes = new Dictionary<int, List<TestControlDescription>>();
		}
		public static string GetTestControlsArrayScript(int keyHashCode) {
			string stringCount = count.ToString();
			count++;
			return 
@"
var _TestControls" + stringCount + @"_;
var TestControls;

function initTestControls() {
    TestControls = new TestControls(" + keyHashCode + @");
	_TestControls" + stringCount + @"_= TestControls; 
    TestControls.IsInited = true;
}
initTestControls();
";
		}
		public static void CreateTestControlsScriptsFile() {
			if(HttpContext.Current != null) {
				if(EasyTestEnabled) {
					try {
						string filePath = HttpContext.Current.Server.MapPath(StaticTestControlsScriptInclude);
						TestControlsImplemantationsList testControlsImplemantationsList = new TestControlsImplemantationsList();
						File.WriteAllText(filePath, testControlsImplemantationsList.GetScript());
					}
					catch(Exception e) {
						Tracing.Tracer.LogError("Failed when creating a script file.");
						Tracing.Tracer.LogError(e);
						throw new ScriptFileCreatingException(HttpContext.Current, "Failed when creating a script file.", e);
					}
				}
			}
			else {
				Tracing.Tracer.LogError("It is impossible to create a script file. HttpContext is required.");
				throw new ScriptFileCreatingException(HttpContext.Current, "It is impossible to create a script file. HttpContext is required.");
			}
		}
		static TestScriptsManager() {
			Stream stream = typeof(TestScriptsManager).Assembly.GetManifestResourceStream("DevExpress.ExpressApp.Web.Resources.TestControlsFactory.js");
			using(StreamReader reader = new StreamReader(stream, Encoding.UTF8)) {
				testControlsFactory = reader.ReadToEnd();
			}
		}
		public TestScriptsManager() {
		}
		public TestScriptsManager(Page page, Type webWindowType)
			: this() {
			this.key = page;
			this.webWindowType = webWindowType;
		}
		public TestScriptsManager(WebWindow webWindow)
			: this() {
			this.key = webWindow;
			this.webWindowType = typeof(WebWindow);
		}
		public TestScriptsManager(Page page) : this(page, typeof(WebWindow)) { }
		public static bool EasyTestEnabled {
			get { return easyTestEnabled; 
			}
			set {
				easyTestEnabled = value;
#if !DebugTest
				if (value) {
				}
#endif
			}
		}
		private string GetTestControlsScriptInclude() {
			return StaticTestControlsScriptInclude;
		}
		public void UpdateClientControl(int index, ITestable testable) {
			foreach(TestControlDescription item in TestControlDescriptions) {
				if(item.GetHashCode() == index) {
					item.ClientID = testable.ClientId;
					item.AdditionalParameters = GetAdditionalParametersString(testable);
				}
			}
		}
		public void RemoveTestControl(int index) {
			TestControlDescription foundedTestControlDescription = null;
			foreach(TestControlDescription item in TestControlDescriptions) {
				if(item.GetHashCode() == index) {
					foundedTestControlDescription = item;
					break;
				}
			}
			if(foundedTestControlDescription != null) {
				TestControlDescriptions.Remove(foundedTestControlDescription);
			}
		}
		public void AllControlRegistered(string addCode) {
			if(EasyTestEnabled) {
				Page page = key as Page;
				if(page != null) {
					page.ClientScript.RegisterClientScriptInclude(webWindowType, "TestControlsInclude", GetTestControlsScriptInclude());
					page.ClientScript.RegisterStartupScript(webWindowType, "TestControlsArray", GetTestControlsArrayScript(KeyHashCode), true);
					page.ClientScript.RegisterClientScriptBlock(webWindowType, "TestControlsComment",
						"/*" + Environment.NewLine + commentCode + "*/" + Environment.NewLine, true);
					commentCode = "";
				}
				else {
					WebWindow webWindow = (WebWindow)key;
					webWindow.RegisterClientScriptInclude("TestControlsInclude", GetTestControlsScriptInclude());
					webWindow.RegisterStartupScript("TestControlsArray", GetTestControlsArrayScript(KeyHashCode), true);
					webWindow.RegisterClientScript("TestControlsComment",
						"/*" + Environment.NewLine + commentCode + "*/" + Environment.NewLine, true);
					commentCode = "";
				}
			}
		}
		public int RegisterControl(string jsClassName, string clientID, TestControlType controlType, string caption, string fullCaption, string additionalParameters) {
			TestControlDescription testControlDescription = new TestControlDescription(jsClassName, clientID, controlType, caption, fullCaption, additionalParameters);
			foreach(TestControlDescription item in TestControlDescriptions) {
				if(item.Equals(testControlDescription)) {
					if(item.ClientID == testControlDescription.ClientID) {
						return item.GetHashCode();
					}
					else {
						if(testControlDescription.ControlType != TestControlType.Field) {
							testControlDescription.Caption += "." + testControlDescription.ClientID;
						}
						testControlDescription.FullCaption += "." + testControlDescription.ClientID;
					}
				}
			}
			TestControlDescriptions.Add(testControlDescription);
			AddComment(testControlDescription.GetCode());
			return testControlDescription.GetHashCode();
		}
		public void RegisterControl(string jsClassName, string clientID, TestControlType controlType, string caption, string fullCaption) {
			RegisterControl(jsClassName, clientID, controlType, caption, fullCaption, "");
		}
		public void RegisterControl(string jsClassName, string clientID, TestControlType controlType, string fullCaption) {
			this.RegisterControl(jsClassName, clientID, controlType, fullCaption, fullCaption, "");
		}
		private string GetAdditionalParametersString(ITestable testControl) {
			List<string> additionalParameters = new List<string>();
			if(testControl is ISupportAdditionalParametersTestControl) {
				additionalParameters.AddRange(((ISupportAdditionalParametersTestControl)testControl).GetAdditionalParameters(testControl));
			}
			else {
				if(testControl.TestControl is ISupportAdditionalParametersTestControl) {
					additionalParameters.AddRange(((ISupportAdditionalParametersTestControl)testControl.TestControl).GetAdditionalParameters(testControl));
				}
				if(testControl.TestControl != null && testControl.TestControl.ScriptsDeclaration is ISupportAdditionalParametersTestControl) {
					additionalParameters.AddRange(((ISupportAdditionalParametersTestControl)testControl.TestControl.ScriptsDeclaration).GetAdditionalParameters(testControl));
				}
			}
			string additionalParametersString = "";
			if(additionalParameters.Count > 0) {
				additionalParametersString = @", " + string.Join(", ", additionalParameters.ToArray()) + @"";
			}
			return additionalParametersString;
		}
		public int RegisterControl(ITestable testControl, TestControlType controlType, string caption, string fullCaption) {
			string className = null;
			if(testControl is ITestableEx) {
				className = GetTypePath(((ITestableEx)testControl).RegisterControlType);
			}
			else {
				className = testControl.TestControl.JScriptClassName;
			}
			string additionalParametersString = GetAdditionalParametersString(testControl);
			return RegisterControl(className, testControl.ClientId, controlType, caption, fullCaption, additionalParametersString);
		}
		public static string GetTypePath(Type type) {
			string result = "";
			Type baseType = type;
			while(baseType != typeof(object)) {
				result += baseType.BaseType != typeof(object) ? baseType.FullName + ";" : baseType.FullName;
				baseType = baseType.BaseType;
			}
			return result;
		}
		public int RegisterControl(ITestable testControl) {
			return RegisterControl(testControl, testControl.TestControlType, testControl.TestCaption, testControl.TestCaption);
		}
		public void AddComment(string comment) {
			commentCode += comment + "\r\n";
		}
		public void Clear() {
			commentCode = "";
			TestControlDescriptions.Clear();
		}
#if DebugTest 
		public string GetCommentCode() {
			return commentCode;
		}
#endif
		#region IHttpHandler Members
		public bool IsReusable {
			get { return true; }
		}
		private bool IsMatch(TestControlDescription controlDescription, string controlType, string caption, string fullCaption) {
			return controlDescription.ControlType.ToString() == controlType &&
				(string.IsNullOrEmpty(fullCaption) || controlDescription.FullCaption == fullCaption) &&
				(string.IsNullOrEmpty(caption) || controlDescription.Caption == caption);
		}
		private string GetTestControlsDescriptions(List<TestControlDescription> testControlsDescriptions) {
			string result = "";
			foreach(TestControlDescription testControlsDescription in testControlsDescriptions) {
				result += "ControlType: " + testControlsDescription.ControlType;
				result += ", Caption: " + testControlsDescription.Caption;
				result += ", FullCaption: " + testControlsDescription.FullCaption;
				result += ", AdditionalParameters: " + testControlsDescription.AdditionalParameters.Replace("\'", "");
				result += ";";
			}
			return string.Format("'{0}'", result);
		}
		private string FindControl(List<TestControlDescription> testControlsDescriptions, string controlType, string caption, string fullCaption) {
			string result = "''";
			foreach(TestControlDescription controlDescription in testControlsDescriptions) {
				if(IsMatch(controlDescription, controlType, caption, fullCaption)) {
					if(controlDescription.JsClassName.Contains(";")) {
						result = string.Format(@"""{0},'{1}','{2}'{3}""", controlDescription.JsClassName, controlDescription.ClientID, controlDescription.FullCaption, controlDescription.AdditionalParameters);
					}
					else {
						result = string.IsNullOrEmpty(controlDescription.AdditionalParameters) ?
						string.Format(@"new {0}(""{1}"",""{2}"")", controlDescription.JsClassName, controlDescription.ClientID, controlDescription.FullCaption)
						: string.Format(@"new {0}(""{1}"",""{2}""{3})", controlDescription.JsClassName, controlDescription.ClientID, controlDescription.FullCaption, controlDescription.AdditionalParameters);
					}
				}
			}
			return result;
		}
		private string FindControlsFullName(List<TestControlDescription> testControlsDescriptions, string controlType, string caption, string fullCaption) {
			Tracing.Tracer.LogText(">FindControlsFullName");
			Tracing.Tracer.LogValue("caption", caption);
			Tracing.Tracer.LogValue("fullCaption", fullCaption);
			List<string> foundControlsFullNameResult = new List<string>();
			foreach(TestControlDescription controlDescription in testControlsDescriptions) {
				if(IsMatch(controlDescription, controlType, caption, fullCaption)) {
					foundControlsFullNameResult.Add(controlDescription.FullCaption);
				}
			}
			if(foundControlsFullNameResult.Count == 0) {
				Tracing.Tracer.LogText("Entry was not found. " + testControlsDescriptions.Count + " entries:");
				foreach(TestControlDescription controlDescription in testControlsDescriptions) {
					Tracing.Tracer.LogText(controlDescription.FullCaption);
				}
			}
			Tracing.Tracer.LogText("<FindControlsFullName");
			return string.Format("'{0}'", string.Join(";", foundControlsFullNameResult.ToArray()));
		}
		public void ProcessRequest(HttpContext context) {
			string result = string.Empty;
			if(context.Request.RawUrl.Contains("&method=")) {
				string method = context.Request.QueryString["method"];
				int keyHashCode;
				if(Int32.TryParse(context.Request.QueryString["keyHashCode"], out keyHashCode) && listOfCodes.ContainsKey(keyHashCode)) {
					List<TestControlDescription> currentCodes = listOfCodes[keyHashCode];
					string controlType = context.Request.QueryString["controlType"];
					string caption = context.Request.QueryString["caption"];
					string fullCaption = context.Request.QueryString["fullCaption"];
					if(method == "FindControl") {
						result = FindControl(currentCodes, controlType, caption, fullCaption);
					}
					else if(method == "FindControlsFullName") {
						result = FindControlsFullName(currentCodes, controlType, caption, fullCaption);
					}
					else if(method == "GetTestControlsDescriptions") {
						result = GetTestControlsDescriptions(currentCodes);
					}
					else {
						Tracing.Tracer.LogText("TestScriptsManager.ProcessRequest: unknown '" + method + "' method was passed, url='" + context.Request.QueryString + "'");
						result = "null";
					}
				}
				else {
					Tracing.Tracer.LogText("TestScriptsManager.ProcessRequest: unknown '" + keyHashCode + "' entry key was passed, url='" + context.Request.QueryString + "'");
					result = "null";
				}
			}
			else {
				result = Script + testControlsFactory;
			}
			context.Response.Clear();
			context.Response.Cache.SetExpires(DateTime.Now);
			context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
			context.Response.Write(result);
			context.Response.End();
		}
		private static string script = null;
		private static string Script {
			get {
				if(string.IsNullOrEmpty(script)) {
					script = new TestControlsImplemantationsList().GetScript();
				}
				return script;
			}
		}
		#endregion
		public bool CanProcessRequest(HttpRequest request) {
			return request.QueryString["handlerName"] == "TestControls";
		}
		public System.Web.SessionState.SessionStateBehavior SessionClientMode { get { return System.Web.SessionState.SessionStateBehavior.Disabled; } }
	}
}
