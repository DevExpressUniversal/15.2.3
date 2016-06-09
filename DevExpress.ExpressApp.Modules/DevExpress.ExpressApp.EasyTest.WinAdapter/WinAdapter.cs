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
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Diagnostics;
using System.Threading;
using DevExpress.EasyTest.Framework;
using DevExpress.ExpressApp.Win;
using DevExpress.ExpressApp.Win.Editors;
using System.ComponentModel;
using DevExpress.XtraEditors.Popup;
using DevExpress.EasyTest.Framework.Utils;
using DevExpress.ExpressApp.EasyTest.WinAdapter.TestControls;
using DevExpress.EasyTest.Framework.Commands;
using System.Collections.Generic;
using System.Net.Sockets;
using DevExpress.ExpressApp.Win.EasyTest;
using DevExpress.ExpressApp.EasyTest.WinAdapter.Utils;
using System.Collections;
using System.Reflection;
using DevExpress.EasyTest.Framework.Loggers;
namespace DevExpress.ExpressApp.EasyTest.WinAdapter {
	public class WinAdapter : WinApplicationAdapter, IApplicationAdapter, ICommandsRegistrator, IAdditionalPreconditionCommands {
		public const int WaitApplicationFormTimeoutMilliseconds = 40000;
		protected int applicationStartupTimeout = 60000;
		private static Size defaultFormSize = new Size(1024, 768);
		private bool disableFormResizing = false;
		public static Size DefaultFormSize {
			get { return defaultFormSize; }
			set { defaultFormSize = value; }
		}
		private WinEasyTestCommandAdapter commandAdapter;
		private WinEasyTestCommandAdapter WinEasyTestCommandAdapter {
			get {
				if(commandAdapter == null) {
					commandAdapter = InternalCreateCommandAdapter();
				}
				return commandAdapter;
			}
		}
		private int EasyTestCommunicationPort {
			get {
				int port = 0;
				if(TestApplication != null) {
					string communicationPort = TestApplication.GetParamValue("CommunicationPort");
					if(!int.TryParse(communicationPort, out port)) {
						port = 4100;
					}
				}
				return port;
			}
		}
		protected virtual WinEasyTestCommandAdapter InternalCreateCommandAdapter() {
			return InternalCreateCommandAdapter(EasyTestCommunicationPort, typeof(WinEasyTestCommandAdapter));
		}
		protected virtual WinEasyTestCommandAdapter InternalCreateCommandAdapter(int communicationPort, Type adapterType) {
			return CreateCommandAdapterCore(communicationPort, adapterType);
		}
		private bool ReceiveAdapterType(Type adapterType, int communicationPort) {
			EasyTestTracer.Tracer.LogText("ReceiveAdapterType");
			string url = string.Format("tcp://localhost:{0}/{1}", communicationPort.ToString(), typeof(XafEasyTestReceiver).Name);
			EasyTestTracer.Tracer.LogText("URL: " + url);
			string data = adapterType.Assembly.Location + Environment.NewLine + adapterType.FullName;
			Exception exception = null;
			bool result = CreateSenderThread(url, data, ref exception, 5000);
			if(!result && exception != null) {
				throw exception;
			}
			if(!result) {
				EasyTestTracer.Tracer.LogText("ReceiveAdapterType, not connected");
			}
			return result;
		}
		private bool CreateSenderThread(string url, string data, ref Exception exception, int wailtTime) {
			Exception _exception = exception;
			Thread worker = new Thread(new ThreadStart(delegate {
				try {
					XafEasyTestReceiver adapterReceiver = (XafEasyTestReceiver)Activator.GetObject(typeof(XafEasyTestReceiver), url);
					EasyTestTracer.Tracer.LogText("Try ReceiveAdapterType");
					adapterReceiver.ReceiveData(data);
				}
				catch(Exception e) {
					EasyTestTracer.Tracer.LogText(FileLogger.FormatExceptionReportDefault(e));
					_exception = e;
				}
			}));
			exception = _exception;
			worker.IsBackground = true;
			worker.Name = "SenderThread";
			worker.Start();
			if(!worker.Join(wailtTime)) {
				worker.Abort();
				return false;
			}
			return true;
		}
		private WinEasyTestCommandAdapter CreateCommandAdapterCore(int communicationPort, Type adapterType) {
			try {
				EasyTestTracer.Tracer.LogText("LocalIP:" + BaseLogger.GetLocalIP());
				EasyTestTracer.Tracer.LogText("WinAdapter.CreateCommandAdapterCore");
				WinEasyTestCommandAdapter result = null;
				if(TestApplication != null) {
					if(ReceiveAdapterType(adapterType, communicationPort)) {
						EasyTestTracer.Tracer.LogText("'CreateCommandAdapterCore' creating adapter instance");
						result = CreateAdapterInternal(communicationPort, adapterType);
						EasyTestTracer.Tracer.LogText("'CreateCommandAdapterCore' Created adapter instance");
					}
				}
				else {
					result = new WinEasyTestCommandAdapter(); 
				}
				return result;
			}
			catch(Exception e) {
				EasyTestTracer.Tracer.LogText(FileLogger.FormatExceptionReportDefault(e));
				throw;
			}
		}
		private IChannel GetRegisteredChannel(string name) {
			foreach(IChannel chanel in ChannelServices.RegisteredChannels) {
				if(chanel.ChannelName == name) {
					return chanel;
				}
			}
			return null;
		}
		private WinEasyTestCommandAdapter CreateAdapterInternal(int communicationPort, Type adapterType) {
			WinEasyTestCommandAdapter result = null;
			string url = string.Format("tcp://localhost:{0}/{1}", communicationPort.ToString(), adapterType.Name);
			SynchronousMethodExecutor.ExecuteTimeoutFunction(5000, delegate() {
				try {
					result = (WinEasyTestCommandAdapter)Activator.GetObject(adapterType, url);
					return result != null;
				}
				catch {
					return false;
				}
			});
			return result;
		}
		ICommandAdapter IApplicationAdapter.CreateCommandAdapter() {
			return WinEasyTestCommandAdapter;
		}
		private bool CheckConnectionCore() {
			WinEasyTestCommandAdapter adapter = WinEasyTestCommandAdapter;
			if(adapter != null) {
				EasyTestTracer.Tracer.LogText("WinAdapter CheckConnection");
				adapter.CheckConnection();
				return true;
			}
			else {
				return false;
			}
		}
		protected override void InternalRun(string appName, string arguments) {
			EasyTestTracer.Tracer.LogText(">InternalRun");
			base.InternalRun(appName, arguments);
			int sleepInterval = 8000;
			EasyTestTracer.Tracer.LogText("Application is started, waiting " + sleepInterval .ToString() + "ms ...");
			Thread.Sleep(sleepInterval);
			EasyTestTracer.Tracer.LogText("WaitForInputIdle: " + applicationStartupTimeout + "ms timeout");
			mainProcess.WaitForInputIdle(applicationStartupTimeout);
			EasyTestTracer.Tracer.LogText("WaitForInputIdle finished");
			if(CheckConnectionCore()) {
				if(disableFormResizing) {
					WinEasyTestCommandAdapter.DisableFormResizing();
				}
				SocketException socketException = null;
				bool waitLogonFormResult = false;
				EasyTestTracer.Tracer.LogText("Wait for logon form");
				bool result = SynchronousMethodExecutor.ExecuteTimeoutFunction(WaitApplicationFormTimeoutMilliseconds, delegate() {
					try {
						waitLogonFormResult = WinEasyTestCommandAdapter.WaitLogonForm();
						return true;
					}
					catch(SocketException e) {
						EasyTestTracer.Tracer.LogText("InternalRun.WaitLogonForm" + FileLogger.FormatExceptionReportDefault(e));
						socketException = e;
						return false;
					}
				});
				if(!result && socketException != null) {
					throw socketException;
				}
				if(!waitLogonFormResult) {
					EasyTestTracer.Tracer.LogText("Wait for main form");
					SynchronousMethodExecutor.ExecuteTimeoutFunction(WaitApplicationFormTimeoutMilliseconds, delegate() {
						try {
							WinEasyTestCommandAdapter.WaitMainForm();
							return true;
						}
						catch(SocketException e) {
							EasyTestTracer.Tracer.LogText("InternalRun.WaitMainForm" + FileLogger.FormatExceptionReportDefault(e));
							return false;
						}
					});
				}
			}
			else {
				if(!reopenApp && TestApplication != null) {
					EasyTestTracer.Tracer.LogText("Not connected, restart application");
					reopenApp = true;
					KillApplication(TestApplication, KillApplicationConext.TestAborted);
					InternalRun(appName, arguments);
					reopenApp = false;
				}
				else {
					throw new DevExpress.EasyTest.Framework.WarningException("Not connected");
				}
			}
			EasyTestTracer.Tracer.LogText("<InternalRun");
		}
		private static bool reopenApp = false;
		public override void KillApplication(TestApplication testApplication, KillApplicationConext context) {
			if(commandAdapter != null) {
				commandAdapter.Disconnect();
			}
			string fileName = GetProcessName(testApplication);
			if(fileName != "") {
				CloseApplication(fileName, true);
			}
		}
		public virtual ApplicationView[] GetApplicationViews() {
			List<ApplicationView> result = new List<ApplicationView>();
			try {
				result.Add(ImageHelper.CreateApplicationView(WinEasyTestCommandAdapter.GetActiveFormHandle()));
			}
			catch(SocketException ex) {
				result.Add(ImageHelper.CreateApplicationView(IntPtr.Zero));
				EasyTestTracer.Tracer.LogText(ex.Message + (ex.InnerException != null ? "\r\nInnerException: " + ex.InnerException.Message : string.Empty) + "\r\n" + ex.StackTrace);
			}
			return result.ToArray();
		}
		public virtual void Dispose() { }
		public virtual void CloseApplication() {
			ActionCommand actionCommand = new ActionCommand();
			actionCommand.ParseCommand(new CommandCreationParam(
				new ScriptStringList("*Action Exit"), 0));
			actionCommand.Execute(commandAdapter);
		}
		#region ICommandsRegistrator Members
		public virtual void RegisterCommands(IRegisterCommand registrator) {
			registrator.RegisterCommand("DragDrop", typeof(DragDropCommand));
			registrator.RegisterCommand("CheckValidationResult", typeof(WinCheckValidationResultCommand));
			registrator.RegisterCommand("SetActiveWindowPosition", typeof(SetActiveWindowPositionCommand));
			registrator.RegisterCommand("CheckActiveWindowPosition", typeof(CheckActiveWindowPositionCommand));
			registrator.RegisterCommand("CheckActiveWindowSize", typeof(CheckActiveWindowSizeCommand));
			registrator.RegisterCommand("AutoTest", typeof(AutoTestCommand));
			registrator.RegisterCommand("CloseActiveWindow", typeof(CloseActiveWindowCommand));
		}
		#endregion
		#region IAdditionalPreconditionCommands Members
		public virtual void SetPreconditions(CommandCollection preconditionCommands) {
			foreach(Command command in preconditionCommands) {
				if(command is PrecondiotionParameter) {
					string applicationParameter = command.Parameters.MainParameter.Value;
					if(applicationParameter == "DisableWindowResizing") {
						disableFormResizing = true;
					}
				}
			}
		}
		#endregion
	}
}
