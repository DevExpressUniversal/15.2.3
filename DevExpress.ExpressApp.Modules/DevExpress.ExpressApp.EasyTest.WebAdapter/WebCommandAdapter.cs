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
using System.Configuration;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using DevExpress.EasyTest.Framework;
using DevExpress.ExpressApp.EasyTest.WebAdapter.TestControls;
using DevExpress.ExpressApp.EasyTest.WebAdapter.Utils;
using mshtml;
namespace DevExpress.ExpressApp.EasyTest.WebAdapter {
	public interface IScriptExecutor {
		object ExecuteScript(string scriptText);
		void AddScript(string scriptText, string scriptName, string paramNames);
	}
	public class WebCommandAdapter : ICommandAdapter, IScriptExecutor {
		public const BindingFlags CommonBindingFlags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase |
				BindingFlags.Public | BindingFlags.Static;
		public const int MaxTimeout = 2000;
		public const string TestControlsSignature = "_TestControls";
		public const string ThrowExceptionOnWaitTimeoutExpiredAppConfigEntryName = "ThrowExceptionOnWaitTimeoutExpired";
		public const string ThrowExceptionOnWaitCallbackExpiredAppConfigEntryName = "ThrowExceptionOnWaitCallbackExpired";
		public int WaitCallbackTime = 25000;
		public bool? CanThrowExceptionOnWaitCallback = true;
		public int WaitTimeoutTime = 25000;
		public bool? CanThrowExceptionOnWaitTimeout = true;
		private WebAdapter adapter;
		private ControlFinder controlFinder;
		private UnmanagedControlFinder unmanagedControlFinder;
		private LastTestControls lastTestControls;
		private IReflect lastIReflectTestControl;
		private string documentUrl;
		private int lastWebBrowserCount;
		private bool needUpdateControlReflect = true;
		public WebCommandAdapter(WebAdapter adapter) {
			this.adapter = adapter;
			controlFinder = new ControlFinder(adapter);
			unmanagedControlFinder = new UnmanagedControlFinder(this);
		}
		public WebAdapter WebAdapter {
			get {
				return adapter;
			}
		}
		public static bool ExecuteTimeoutFunction(Predicate<object> predicate) {
			return ExecuteTimeoutFunction(MaxTimeout, predicate);
		}
		public static bool ExecuteTimeoutFunction(int timeoutMilliseconds, Predicate<object> predicate) {
			return ExecuteTimeoutFunction(timeoutMilliseconds, 200, predicate);
		}
		public static bool ExecuteTimeoutFunction(int timeoutMilliseconds, int sleepTime, Predicate<object> predicate, Func<object> onTimeoutPredicate) {
			DateTime endTime = DateTime.Now.AddMilliseconds(timeoutMilliseconds);
			bool result = false;
			while(true) {
				try {
					result = predicate(null);
					if((DateTime.Now > endTime) || result) {
						break;
					}
				}
				catch(Exception e) {
					EasyTestTracer.Tracer.LogText("Exception:" + e.Message);
					if(DateTime.Now > endTime) {
						EasyTestTracer.Tracer.LogText("Throw exception!!!");
						throw;
					}
				}
				Thread.Sleep(sleepTime);
			}
			if(!result) {
				if(onTimeoutPredicate != null) {
					onTimeoutPredicate();
				}
				EasyTestTracer.Tracer.LogText("Timeout expired:" + timeoutMilliseconds);
			}
			return result;
		}
		public static bool ExecuteTimeoutFunction(int timeoutMilliseconds, int sleepTime, Predicate<object> predicate) {
			return ExecuteTimeoutFunction(timeoutMilliseconds, sleepTime, predicate, null);
		}
		public bool NeedUpdateControlReflect {
			get {
				return needUpdateControlReflect;
			}
			set {
				needUpdateControlReflect = value;
			}
		}
		public void WaitForBrowserResponse(bool waitPostback) {
			WaitForBrowserResponse(waitPostback, false);
		}
		public object ExecuteScript(string scriptText) {
			UpdateTestControls(); 
			object result = CurrentTestControls.ExecuteScript(scriptText);
			WaitForBrowserResponse(false);
			return result;
		}
		public void AddScript(string scriptText, string scriptName, string paramNames) {
			CurrentTestControls.AddScript(scriptText, scriptName, paramNames);
		}
		public string LastTestControlsName {
			get {
				LastTestControls test = CreateTestControls();
				return test != null ? test.LastTestControlsName : null;
			}
		}
		#region ICommandAdapter Members
		public virtual ITestControl CreateTestControl(string controlType, string name) {
			try {
				EasyTestTracer.Tracer.InProcedure("CreateTestControl");
				return CreateTestControlCore(controlType, name);
			}
			catch(AdapterOperationException) {
				WaitForBrowserResponse(false);
				return CreateTestControlCore(controlType, name);
			}
			finally {
				EasyTestTracer.Tracer.OutProcedure("CreateTestControl");
			}
		}
		private ITestControl CreateTestControlCore(string controlType, string name) {
			try {
				EasyTestTracer.Tracer.InProcedure("CreateTestControlCore");
				if(FileDownloadDialogHelper.ProcessFileDownloadDialog(this)) {
					EasyTestTracer.Tracer.LogVerboseText("ProcessFileDownloadDialog completed");
				}
				ITestControl result = unmanagedControlFinder.FindControl(CurrentTestControls, controlType, name);
				if(result != null) {
					Thread.Sleep(200);
				}
				else {
					if(controlType == TestControlType.Dialog) {
						WaitForBrowserResponse(false);
						result = new BrowserWindowTestControl(adapter.WebBrowsers[adapter.WebBrowsers.Count - 1], this);
					}
					else {
						object testControl = controlFinder.FindControl(CurrentTestControls, controlType, name);
						if(testControl == null) {
							throw new AdapterOperationException(string.Format("Cannot find the '{0}' control, OperationTag:'{1}'", name, controlType));
						}
						if(testControl is string) {
							string testControlInfo = ((string)testControl);
							int separatorIndex = ((string)testControl).IndexOf(',');
							string controlTypePath = testControlInfo.Substring(0, separatorIndex);
							string controlParameters = testControlInfo.Substring(separatorIndex + 1);
							result = TestControlFactoryWeb.Instance.CreateControl(new WebEasyTestControlDescription(this, controlTypePath, controlParameters));
						}
						else {
							if(controlType == TestControlType.Table) {
								result = new JSTestTable(this, testControl);
							}
							else {
								result = new JSTestControl(this, testControl);
							}
							if(((JSTestControl)result).ClassName == "ASPxLookupPropertyEditor") {
								result = new JSASPxLookupProperytEditorTestControl(this, testControl);
							}
						}
					}
				}
				result.Name = name;
				result.ControlType = controlType;
				return result;
			}
			finally {
				EasyTestTracer.Tracer.OutProcedure("CreateTestControlCore");
			}
		}
		public object FindControl(string controlType, string name) {
			return controlFinder.FindControl(CurrentTestControls, controlType, name);
		}
		public bool IsControlExist(string controlType, string name) {
			return FindControl(controlType, name) != null;
		}
		#endregion
		private IReflect LastIReflectTestControl {
			get {
				return lastIReflectTestControl;
			}
		}
		private string GetTestControlsName(mshtml.IHTMLDocument2 document) {
			EasyTestTracer.Tracer.InProcedure("GetTestControlsNameCore");
			string result = null;
			if(!string.IsNullOrEmpty(document.url)) {
				foreach(IHTMLScriptElement scriptSrc in document.scripts) {
					string text = scriptSrc.text;
					if(string.IsNullOrEmpty(text)) continue;
					int testControlsTypeIdIndex = text.IndexOf(TestControlsSignature);
					if(testControlsTypeIdIndex != -1) {
						int endIndex = text.IndexOf("_", testControlsTypeIdIndex + 1);
						result = text.Substring(testControlsTypeIdIndex, endIndex - (testControlsTypeIdIndex)).Trim() + "_";
						break;
					}
				}
			}
			EasyTestTracer.Tracer.OutProcedure("GetTestControlsNameCore");
			return result;
		}
		private LastTestControls CreateTestControls() {
			EasyTestTracer.Tracer.InProcedure("CreateTestControls");
			mshtml.IHTMLDocument2 documentToTest = null;
			string testControlsName = null;
			mshtml.IHTMLDocument2 rootDocument = GetCurrentDocument();
			EasyTestTracer.Tracer.LogText("Found frames count: " + rootDocument.frames.length);
			ExecuteTimeoutFunction(delegate(object obj) {
				try {
					for(int frameIndex = rootDocument.frames.length - 1; frameIndex >= 0; frameIndex--) {
						object pvarIndex = frameIndex;
						mshtml.IHTMLWindow2 frameWindow = (mshtml.IHTMLWindow2)rootDocument.frames.item(ref pvarIndex);
						string frameTestControlsName = GetTestControlsName(frameWindow.document);
						if(!string.IsNullOrEmpty(frameTestControlsName)) {
							EasyTestTracer.Tracer.LogText("Found testControls variable at frame : " + frameIndex);
							documentToTest = frameWindow.document;
							testControlsName = frameTestControlsName;
							break;
						}
					}
					if(string.IsNullOrEmpty(testControlsName)) {
						string rootDocumentTestControlsName = GetTestControlsName(rootDocument);
						if(!string.IsNullOrEmpty(rootDocumentTestControlsName)) {
							EasyTestTracer.Tracer.LogText("Found testControls variable at root window");
							documentToTest = rootDocument;
							testControlsName = rootDocumentTestControlsName;
						}
					}
				}
				catch(Exception e) {
					EasyTestTracer.Tracer.LogError(e);
				}
				EasyTestTracer.Tracer.LogText("Found testControls variable name: " + testControlsName);
				return !string.IsNullOrEmpty(testControlsName);
			});
			LastTestControls result = null;
			if(documentToTest != null && !string.IsNullOrEmpty(testControlsName)) {
				result = new LastTestControls(testControlsName, documentToTest);
			}
			EasyTestTracer.Tracer.OutProcedure("CreateTestControls");
			return result;
		}
		private IReflect GetIReflectTestControl(string testControlsName, mshtml.IHTMLDocument2 doc) {
			try {
				EasyTestTracer.Tracer.InProcedure("GetIReflectTestControl");
				return GetIReflectTestControlCore(testControlsName, doc);
			}
			catch {
				LastTestControls _lastTestControls = null;
				ExecuteTimeoutFunction(60000, delegate(object obj) {
					if(doc.scripts.length != 0) {
						_lastTestControls = CreateTestControls();
					}
					return _lastTestControls != null;
				});
				if(_lastTestControls != null) {
					return GetIReflectTestControlCore(_lastTestControls.LastTestControlsName, _lastTestControls.Doc);
				}
				return null;
			}
			finally {
				EasyTestTracer.Tracer.OutProcedure("GetIReflectTestControl");
			}
		}
		private IReflect GetIReflectTestControlCore(string testControlsName, mshtml.IHTMLDocument2 doc) {
			EasyTestTracer.Tracer.InProcedure("GetIReflectTestControlCore");
			try {
				EasyTestTracer.Tracer.InProcedure("WaitScript");
				IReflect testControls = null;
				try {
					WebCommandAdapter.ExecuteTimeoutFunction(delegate(object obj) {
						try {
							foreach(IHTMLScriptElement scriptElement in doc.scripts) {
								if(!string.IsNullOrEmpty(scriptElement.src)) {
									if((scriptElement.src.StartsWith("DXX.axd?handlerName=TestControls") && (scriptElement.readyState != "complete"))
										|| !((scriptElement.readyState == "loaded") || (scriptElement.readyState == "complete")))
										return false;
								}
							}
							return true;
						}
						catch { }
						return false;
					});
				}
				finally {
					EasyTestTracer.Tracer.OutProcedure("WaitScript");
				}
				EasyTestTracer.Tracer.InProcedure("GetTestControlsValue");
				try {
					if(!WebCommandAdapter.ExecuteTimeoutFunction(5200, delegate(object obj) {
						IReflect script = doc.Script as IReflect;
						PropertyInfo p = script.GetProperty(testControlsName, WebCommandAdapter.CommonBindingFlags);
						if(p == null) {
							throw new WarningException("Cannot get property info for the '" + testControlsName + "' ");
						}
						testControls = p.GetValue(script, null) as IReflect;
						PropertyInfo isInitedPropertyInfo = testControls.GetProperty("IsInited", WebCommandAdapter.CommonBindingFlags);
						return testControls != null && ((bool)isInitedPropertyInfo.GetValue(testControls, null));
					})) {
						throw new Exception("Cannot retrieve the '" + testControlsName + "' variable's value.");
					}
				}
				finally {
					EasyTestTracer.Tracer.OutProcedure("GetTestControlsValue");
				}
				return testControls;
			}
			finally {
				EasyTestTracer.Tracer.OutProcedure("GetIReflectTestControlCore");
			}
		}
		private ITestControls CurrentTestControls {
			get {
				try {
					EasyTestTracer.Tracer.InProcedure("CurrentTestControls");
					IReflect testControls = LastIReflectTestControl;
					if(testControls == null) {
						LastTestControls testControl = CreateTestControls();
						if(testControl != null) {
							testControls = GetIReflectTestControl(testControl.LastTestControlsName, testControl.Doc);
						}
					}
					if(testControls == null) {
						return new FakeTestControlsImpl();
					}
					return new TestControlsImpl(testControls);
				}
				finally {
					EasyTestTracer.Tracer.OutProcedure("CurrentTestControls");
				}
			}
		}
		private IHTMLDocument2 GetCurrentDocument() {
			try {
				return adapter.GetDocument(adapter.WebBrowsers.Count - 1);
			}
			catch {
				ExecuteTimeoutFunction(delegate(object obj) {
					return adapter.WebBrowsers.Count > 0;
				});
				return adapter.GetDocument(adapter.WebBrowsers.Count - 1);
			}
		}
		private void WaitCallback(bool isTimoutHappened) {
			EasyTestTracer.Tracer.InProcedure("IsCallback");
			PropertyInfo isCallBackField = LastIReflectTestControl.GetProperty("IsCallBack", CommonBindingFlags);
			if(!isTimoutHappened && isCallBackField != null && ((bool)isCallBackField.GetValue(LastIReflectTestControl, null))) {
				EasyTestTracer.Tracer.LogVerboseText("IsCallBack");
				ExecuteTimeoutFunction(WaitCallbackTime, 200, delegate(object obj) {
					bool isCallBack = (bool)isCallBackField.GetValue(LastIReflectTestControl, null);
					return !isCallBack;
				}, delegate() {
					string throwExceptionOnTimeoutExpired = ConfigurationManager.AppSettings[ThrowExceptionOnWaitCallbackExpiredAppConfigEntryName];
					MethodInfo getCurrentTimeoutListMethod = LastIReflectTestControl.GetMethod("GetCurrentTimeoutList", CommonBindingFlags);
					string currentTimeoutList = (string)getCurrentTimeoutListMethod.Invoke(LastIReflectTestControl, null);
					string message = "WaitCallback - operation is not finished, pending 'setTimeout' handlers: \r\n" + currentTimeoutList;
					bool shouldThrowException = (CanThrowExceptionOnWaitTimeout.HasValue && CanThrowExceptionOnWaitTimeout.Value);
					bool shouldIgnoreException = (CanThrowExceptionOnWaitTimeout.HasValue && !CanThrowExceptionOnWaitTimeout.Value);
					if(!shouldIgnoreException &&
						(shouldThrowException || string.IsNullOrEmpty(throwExceptionOnTimeoutExpired) || throwExceptionOnTimeoutExpired.ToLower() == "true")) {
						throw new Exception(message); 
					}
					else {
						EasyTestTracer.Tracer.LogVerboseText(message);
						return null;
					}
				});
			}
			EasyTestTracer.Tracer.OutProcedure("IsCallback");
		}
		private void WaitFrameLoading() {
			EasyTestTracer.Tracer.InProcedure("IsFrameLoading");
			MethodInfo isFrameLoadingMethod = LastIReflectTestControl.GetMethod("CheckFrameLoading", CommonBindingFlags);
			if(isFrameLoadingMethod != null && ((bool)isFrameLoadingMethod.Invoke(LastIReflectTestControl, null))) {
				EasyTestTracer.Tracer.LogVerboseText("IsFrameLoading");
				ExecuteTimeoutFunction(10000, delegate(object obj) {
					bool isFrameLoading = (bool)isFrameLoadingMethod.Invoke(LastIReflectTestControl, null);
					return !isFrameLoading;
				});
			}
			EasyTestTracer.Tracer.OutProcedure("IsFrameLoading");
		}
		private void WaitPostback(bool waitPostback) {
			EasyTestTracer.Tracer.InProcedure("WaitPostback");
			PropertyInfo isSubmitField = LastIReflectTestControl.GetProperty("IsSubmitting", CommonBindingFlags);
			if(waitPostback || (isSubmitField != null) && ((bool)isSubmitField.GetValue(LastIReflectTestControl, null))) {
				EasyTestTracer.Tracer.LogText(string.Format("WaitPostback: {0}", waitPostback.ToString()));
				if(FileDownloadDialogHelper.ProcessFileDownloadDialog(this)) {
					EasyTestTracer.Tracer.LogVerboseText("ProcessFileDownloadDialog completed");
					isSubmitField.SetValue(LastIReflectTestControl, false, null);
				}
				else {
					LastTestControls testControls = null;
					int counter = 1;
					bool processingFileDownloadDialog = false;
					ExecuteTimeoutFunction(10000, 200, delegate(object obj) {
						counter++;
						if(counter % 5 == 0) {
							if(FileDownloadDialogHelper.ProcessFileDownloadDialog(this)) {
								EasyTestTracer.Tracer.LogVerboseText("ProcessFileDownloadDialog completed in WaitPostback");
								isSubmitField.SetValue(LastIReflectTestControl, false, null);
								processingFileDownloadDialog = true;
								return processingFileDownloadDialog;
							}
						}
						adapter.WaitForBrowserResponse();
						testControls = CreateTestControls();
						return testControls != null && lastTestControls.LastTestControlsName != testControls.LastTestControlsName;
					});
					if(!processingFileDownloadDialog) {
						EasyTestTracer.Tracer.LogVerboseText("IsSubmitting");
						lastTestControls = testControls;
						if(testControls != null) {
							lastIReflectTestControl = GetIReflectTestControl(testControls.LastTestControlsName, testControls.Doc);
						}
						lastWebBrowserCount = adapter.WebBrowsers.Count;
					}
				}
			}
			EasyTestTracer.Tracer.OutProcedure("WaitPostback");
		}
		private void WaitShowModalWindow() {
			EasyTestTracer.Tracer.InProcedure("WaitShowModalWindow");
			PropertyInfo testControlsGetField = LastIReflectTestControl.GetProperty("IsShowModalWindow", CommonBindingFlags);
			if(testControlsGetField != null && ((bool)testControlsGetField.GetValue(LastIReflectTestControl, null))) {
				EasyTestTracer.Tracer.LogVerboseText("IsShowModalWindow");
				ExecuteTimeoutFunction(5500, delegate(object obj) {
					adapter.WaitForBrowserResponse();
					return lastWebBrowserCount == adapter.WebBrowsers.Count;
				});
				ExecuteTimeoutFunction(60000, delegate(object obj) {
					adapter.WaitForBrowserResponse();
					lastTestControls = CreateTestControls();
					if(lastTestControls != null) {
						lastIReflectTestControl = GetIReflectTestControl(lastTestControls.LastTestControlsName, lastTestControls.Doc);
						lastWebBrowserCount = adapter.WebBrowsers.Count;
					}
					return lastTestControls != null;
				});
			}
			EasyTestTracer.Tracer.OutProcedure("WaitShowModalWindow");
		}
		private bool WaitTimeout() {
			try {
				EasyTestTracer.Tracer.InProcedure("isTimeOut");
				MethodInfo timeoutCountMethod = LastIReflectTestControl.GetMethod("TimeoutCountTest", CommonBindingFlags);
				bool isTimoutHappened = false;
				bool timeoutCountMethodResult = (bool)timeoutCountMethod.Invoke(LastIReflectTestControl, null);
				if((timeoutCountMethod != null) && !timeoutCountMethodResult) {
					EasyTestTracer.Tracer.LogVerboseText("Timeout");
					isTimoutHappened = !ExecuteTimeoutFunction(WaitTimeoutTime, delegate(object obj) {
						timeoutCountMethodResult = (bool)timeoutCountMethod.Invoke(LastIReflectTestControl, null);
						EasyTestTracer.Tracer.LogVerboseText("TICK");
						return timeoutCountMethodResult;
					});
				}
				if(isTimoutHappened) {
					MethodInfo getCurrentTimeoutListMethod = LastIReflectTestControl.GetMethod("GetCurrentTimeoutList", CommonBindingFlags);
					string currentTimeoutList = (string)getCurrentTimeoutListMethod.Invoke(LastIReflectTestControl, null);
					string message = "WaitTimeout - operation is not finished, pending 'setTimeout' handlers:\r\n" + currentTimeoutList;
					string throwExceptionOnTimeoutExpired = ConfigurationManager.AppSettings[ThrowExceptionOnWaitTimeoutExpiredAppConfigEntryName];
					bool shouldThrowException = (CanThrowExceptionOnWaitTimeout.HasValue && CanThrowExceptionOnWaitTimeout.Value);
					bool shouldIgnoreException = (CanThrowExceptionOnWaitTimeout.HasValue && !CanThrowExceptionOnWaitTimeout.Value);
					if(!shouldIgnoreException && (shouldThrowException ||
						(string.IsNullOrEmpty(throwExceptionOnTimeoutExpired) || throwExceptionOnTimeoutExpired.ToLower() == "true")
						)) {
						throw new Exception(message); 
					}
					else {
						EasyTestTracer.Tracer.LogVerboseText(message);
					}
				}
				return isTimoutHappened;
			}
			finally {
				EasyTestTracer.Tracer.OutProcedure("isTimeout");
			}
		}
		private void UpdateTestControls() {
			try {
				EasyTestTracer.Tracer.InProcedure("UpdateTestControls");
				lastTestControls = CreateTestControls();
				lastWebBrowserCount = adapter.WebBrowsers.Count;
				if(lastTestControls != null) {
					lastIReflectTestControl = GetIReflectTestControl(lastTestControls.LastTestControlsName, lastTestControls.Doc);
				}
			}
			finally {
				EasyTestTracer.Tracer.OutProcedure("UpdateTestControls");
			}
		}
		private bool WaitIntervalCallback(IHTMLDocument2 currentDocument) {
			EasyTestTracer.Tracer.LogText("WaitIntervalCallback: url = " + currentDocument.url);
			if(currentDocument != null && (currentDocument.url != documentUrl && !currentDocument.url.Contains("#"))) {
				EasyTestTracer.Tracer.LogText("The document URL has been changed to " + currentDocument.url + ", old URL " + documentUrl);
				ExecuteTimeoutFunction(300, delegate(object obj) {
					IReflect testControls = LastIReflectTestControl;
					PropertyInfo isCallBackField = testControls != null ? testControls.GetProperty("IsCallBack", CommonBindingFlags) : null;
					bool isCallback = isCallBackField != null ? (bool)isCallBackField.GetValue(testControls, null) : false;
					if(isCallback) {
						WaitCallback(false);
					}
					return isCallback;
				});
				documentUrl = currentDocument.url;
				return true;
			}
			return false;
		}
		private void WaitForBrowserResponse(bool waitPostback, bool isSecondCall) {
			EasyTestTracer.Tracer.InProcedure("WaitForBrowserResponse(" + (lastTestControls != null ? lastTestControls.LastTestControlsName : "") + ", " + lastWebBrowserCount + ")");
			try {
				adapter.WaitForBrowserResponse();
				if(LastIReflectTestControl == null || isSecondCall) {
					UpdateTestControls();
					if(LastIReflectTestControl == null) {
						adapter.WaitForBrowserResponse();
						UpdateTestControls();
					}
				}
				else {
					LastTestControls _test = CreateTestControls();
					string currentTestControlName = _test != null ? _test.LastTestControlsName : null;
					if(lastTestControls == null || currentTestControlName != lastTestControls.LastTestControlsName) {
						UpdateTestControls();
					}
				}
				if(lastTestControls != null && !string.IsNullOrEmpty(lastTestControls.LastTestControlsName)) {
					WaitPostback(waitPostback);
					WaitShowModalWindow();
					bool isTimoutHappened = WaitTimeout();
					WaitCallback(isTimoutHappened);
					WaitFrameLoading();
					adapter.WaitForBrowserResponse();
					ExecuteTimeoutFunction(60000, delegate(object obj) {
						adapter.WaitForBrowserResponse();
						UpdateTestControls();
						if(WaitIntervalCallback(GetCurrentDocument())) {
							UpdateTestControls();
						}
						return lastTestControls != null;
					});
				}
			}
			catch(COMException) {
				if(isSecondCall) {
					throw;
				}
				else {
					WaitForBrowserResponse(waitPostback, true);
				}
			}
			finally {
				needUpdateControlReflect = true;
				if(lastIReflectTestControl != null) {
					string traceMessages = lastIReflectTestControl.GetMethod("GetTraceMessages", WebCommandAdapter.CommonBindingFlags).Invoke(LastIReflectTestControl, null) as string;
					EasyTestTracer.Tracer.LogText("TestControls.traceMessages: " + (string.IsNullOrEmpty(traceMessages) ? "<null>" : traceMessages));
				}
				EasyTestTracer.Tracer.OutProcedure("WaitForBrowserResponse(" + (lastTestControls != null ? lastTestControls.LastTestControlsName : "") + ", " + lastWebBrowserCount + ")");
			}
		}
		internal class LastTestControls {
			string lastTestControlsName;
			mshtml.IHTMLDocument2 doc;
			public LastTestControls(string lastTestControlsName, mshtml.IHTMLDocument2 doc) {
				this.lastTestControlsName = lastTestControlsName;
				this.doc = doc;
			}
			public string LastTestControlsName {
				get { return lastTestControlsName; }
			}
			public mshtml.IHTMLDocument2 Doc {
				get { return doc; }
			}
		}
	}
}
