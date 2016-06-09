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
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using DevExpress.EasyTest.Framework;
namespace DevExpress.ExpressApp.EasyTest.WebAdapter.TestControls {
	public class JSTestControl : TestControlBase, IControlEnabled, IControlAct, IControlHint, IControlText, IControlActionItems,
		IControlExtendableAct {
		protected ControlReflectWrapper controlReflectWrapper;
		private string caption;
		private string lastTestControlsName;
		protected void CheckError() {
			string error = controlReflectWrapper.GetPropertyValue("error", WebCommandAdapter.CommonBindingFlags) as string;
			if(!string.IsNullOrEmpty(error)) {
				bool operationError = (bool)controlReflectWrapper.GetPropertyValue("operationError", WebCommandAdapter.CommonBindingFlags);
				if(operationError) {
					throw new AdapterOperationException(error);
				} else {
					throw new WarningException(error);
				}
			}
		}
		protected WebCommandAdapter adapter;
		public JSTestControl(WebCommandAdapter adapter, Object testControl) {
			this.adapter = adapter;
			if(!(testControl is IReflect)) {
				throw new ArgumentException("The control does not support IReflect interface");
			}
			this.controlReflectWrapper = new ControlReflectWrapper(testControl as IReflect);
			this.caption = Caption;
			this.lastTestControlsName = adapter.LastTestControlsName;
		}
		public delegate object InvokeDelegate(object[] args);
		private bool IsDialogWindowOpened {
			get {
				IWebBrowserCollection browsers = adapter.WebAdapter.WebBrowsers;
				for(int i = 0; i < browsers.Count; i++) {
					if(browsers[i].DialogWindowHandle != IntPtr.Zero) {
						return true;
					}
				}
				return false;
			}
		}
		private bool NeedWaitBrowserResponseForInvokeMethod(string methodName) {
			return !(
				methodName.ToLower().StartsWith("get") ||
				methodName.ToLower().StartsWith("is") ||
				methodName.ToLower().StartsWith("check")
				);
		}
		private class InvokeMethodThreadObject {
			private ControlReflectWrapper controlReflectWrapper;
			private string methodName;
			private object[] args;
			public InvokeMethodThreadObject(ControlReflectWrapper controlReflectWrapper, string methodName, object[] args) {
				this.controlReflectWrapper = controlReflectWrapper;
				this.methodName = methodName;
				this.args = args;
			}
			public void InvokeMethod() {
				try {
					Result = controlReflectWrapper.GetMethodValue(methodName, WebCommandAdapter.CommonBindingFlags, args);
				}
				catch(COMException) {
				}
				catch(TargetInvocationException) {
				}
				catch(Exception e) {
					this.InnerException = e;
				}
				finally {
					if(WaitHandle != null) {
						WaitHandle.Set();
					}
				}
			}
			public object Result { get; private set; }
			public Exception InnerException { get; private set; }
			public EventWaitHandle WaitHandle { get; set; }
		}
		private object InvokeImplementedMethod(string methodName, params object[] args) {
			return InvokeMethod(methodName, innerArgs => { throw new EasyTestException("The '" + methodName + "' method is not implemented"); }, args);
		}
		public object InvokeMethod(string methodName, InvokeDelegate baseControlInvoke, object[] args) {
			return InvokeMethod(methodName, baseControlInvoke, args, NeedWaitBrowserResponseForInvokeMethod(methodName));
		}
		public object InvokeMethod(string methodName, InvokeDelegate baseControlInvoke, object[] args, bool isWait) {
			if(adapter.NeedUpdateControlReflect) {
				adapter.NeedUpdateControlReflect = false;
				UpdateControlReflect();
			}
			controlReflectWrapper.SetPropertyValue("error", WebCommandAdapter.CommonBindingFlags, null);
			MethodInfo mi = controlReflectWrapper.GetMethodInfo(methodName);
			if(mi == null) {
				return baseControlInvoke(args);
			} else {
				string procedureEntry = string.Format("Control: {0}, Method: {1}.{2}", Name, ClassName, methodName);
				EasyTestTracer.Tracer.InProcedure(procedureEntry);
				try {
					bool waitPostBack = IsAutoPostbackMethod(methodName);
					object result;
					EventWaitHandle waitHandle = null;
					InvokeMethodThreadObject obj = null;
					try {
						waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
						obj = new InvokeMethodThreadObject(controlReflectWrapper, methodName, args);
						obj.WaitHandle = waitHandle;
						Thread worker = new Thread(new ThreadStart(obj.InvokeMethod));
						worker.IsBackground = true;
						waitHandle.Reset();
						worker.Start();
						const int timeout = 10000;
						const int sleepTime = 250;
						const int count = timeout / sleepTime;
						int counter = 0;
						while(counter < count) {
							counter++;
							if(waitHandle.WaitOne(sleepTime) || IsDialogWindowOpened) {
								break;
							}
						}
						if(counter == count) {
							throw new EasyTestException("Timeout expired: " + timeout);
						}
						if(obj.InnerException != null) {
							throw obj.InnerException;
						}
						result = obj.Result;
					}
					finally {
						obj.WaitHandle = null;
						waitHandle.Dispose();
					}
					if(result == DBNull.Value) {
						result = null;
					}
					CheckError();
					if(isWait) {
						adapter.WaitForBrowserResponse(waitPostBack);
					}
					return result;
				}
				catch(Exception e) {
					EasyTestTracer.Tracer.LogError(e);
					throw;
				}
				finally {
					string traceMessages = controlReflectWrapper.GetPropertyValue("traceMessages", WebCommandAdapter.CommonBindingFlags) as string;
					EasyTestTracer.Tracer.LogText("traceMessages: " + (string.IsNullOrEmpty(traceMessages) ? "<null>" : traceMessages));
					EasyTestTracer.Tracer.OutProcedure(procedureEntry);
				}
			}
		}
		public bool IsAutoPostbackMethod(string methodName) {
			bool result = false;
			object isAutoPostBackMethod = controlReflectWrapper.GetMethodValue("IsAutoPostBackMethod", BindingFlags.InvokeMethod, new object[] { });
			if(isAutoPostBackMethod != null) {
				result = (bool)isAutoPostBackMethod;
			}
			return result;
		}
		public string ClientID {
			get { return controlReflectWrapper.GetPropertyValue("id", WebCommandAdapter.CommonBindingFlags) as string; }
		}
		public string Caption {
			get { return controlReflectWrapper.GetPropertyValue("caption", WebCommandAdapter.CommonBindingFlags) as string; ;			}
		}
		public string ClassName {
			get { return controlReflectWrapper.GetPropertyValue("className", WebCommandAdapter.CommonBindingFlags) as string; }
		}
		#region IControlEnabled Members
		public bool Enabled {
			get {
				object result = InvokeMethod("IsEnabled", args => { throw new NotSupportedException(); }, new object[] { });
				return result is bool ? (bool)result : false;
			}
		}
		#endregion
		#region IControlAct Members
		public virtual void Act(string value) {
			InvokeImplementedMethod("Act", value);
		}
		#endregion
		#region IControlHint Members
		public string Hint {
			get { return InvokeImplementedMethod("GetHint", null) as string; }
		}
		#endregion
		#region IControlText Members
		public virtual string Text {
			get {
				object result = InvokeImplementedMethod("GetText");
				return result == null ? "" : result.ToString();
			}
			set { InvokeImplementedMethod("SetText", value); }
		}
		#endregion
		#region IControlExtendableAct Members
		public void ActEx(string actionName, string[] parameterValues) {
			InvokeMethod(actionName, args => { throw new EasyTestException(String.Format("The control action '{0}' is not supported", actionName)); }, parameterValues);
		}
		#endregion
		#region IControlActionItems Members
		public bool IsEnabled(string actionItemName) {
			object result = InvokeImplementedMethod("IsActionItemEnabled", actionItemName);
			return result is bool ? (bool)result : false;
		}
		public bool IsVisible(string actionItemName) {
			object result = InvokeImplementedMethod("IsActionItemVisible", actionItemName);
			return result is bool ? (bool)result : false;
		}
		#endregion
		private void UpdateControlReflect() {
			try {
				EasyTestTracer.Tracer.InProcedure("UpdateControlReflect");
				string localLastTestControlsName = adapter.LastTestControlsName;
				if(string.IsNullOrEmpty(localLastTestControlsName)) {
					Thread.Sleep(3000);
					adapter.WebAdapter.WaitForBrowserResponse();
					localLastTestControlsName = adapter.LastTestControlsName;
					if(string.IsNullOrEmpty(localLastTestControlsName)) {
						throw new TestControlsNotInitializedException("UpdateControlReflect");
					}
				}
				if(localLastTestControlsName != lastTestControlsName) {
					EasyTestTracer.Tracer.LogVerboseText("TestControl update started");
					EasyTestTracer.Tracer.LogVerboseText(string.Format("LastTestControlsName: {0}", localLastTestControlsName));
					lastTestControlsName = localLastTestControlsName;
					object control = adapter.FindControl(ControlType, Name);
					if(control != null) {
						EasyTestTracer.Tracer.LogVerboseText(string.Format("The control was found, name: {0}, controlType: {1}", Name, ControlType));
						IReflect controlReflect = control as IReflect;
						if(controlReflect == null) {
							throw new ArgumentException("The control does not support IReflect interface");
						}
						controlReflectWrapper = new ControlReflectWrapper(controlReflect);
					}
					else {
						throw new ArgumentException(string.Format("The control was not found, name: {0}, controlType: {1}", Name, ControlType));
					}
				}
			}
			finally {
				EasyTestTracer.Tracer.OutProcedure("UpdateControlReflect");
			}
		}
	}
}
