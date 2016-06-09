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
using System.Collections.Generic;
using System.Collections;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using DevExpress.EasyTest.Framework;
using System.Drawing.Imaging;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.ExpressApp.EasyTest.WinAdapter.TestControls;
using DevExpress.EasyTest.Framework.Utils;
using DevExpress.ExpressApp.EasyTest.WinAdapter.Utils;
using DevExpress.XtraEditors.Popup;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Win.EasyTest;
namespace DevExpress.ExpressApp.EasyTest.WinAdapter {
	[Serializable]
	public class ExceptionHelper {
		public static Exception CreateOrdinalException(Exception e) {
			return new Exception(
				string.Format("The \"{0}\" exception occurs. Exception text: \"{1}\", stack trace: \"{2}\".",
				e.GetType().FullName, e.Message, e.StackTrace));
		}
	}
	public class WinEasyTestCommandAdapter : MarshalByRefObject, ICommandAdapter {
		public void Disconnect() {
			System.Runtime.Remoting.RemotingServices.Disconnect(this);
		}
		public void CheckConnection() { }
		[System.Security.SecurityCritical]
		public override object InitializeLifetimeService() {
			return null;
		}
		static WinEasyTestCommandAdapter() {
			DevExpress.ExpressApp.Win.Utils.TimeLatch.DefaultTimeout = 0;
			EasyTestOptions.CheckIsFormAcceptable += new EventHandler<System.ComponentModel.CancelEventArgs>(delegate(object sender, System.ComponentModel.CancelEventArgs e) {
				Form form = sender as Form;
				e.Cancel = e.Cancel ||
					form is DevExpress.Utils.Win.TopFormBase ||
					form is DevExpress.XtraBars.Forms.FloatingBarControlForm ||
					form is DevExpress.ExpressApp.EasyTest.WinAdapter.Utils.ActiveActions.WndForm ||
					form is PopupLookUpEditForm || form is DevExpress.Utils.Win.Shadow ||
					form is DevExpress.XtraSplashForm.SplashFormBase || 
					((sender != null) && sender.GetType().Name.Contains("Splash"));
			});
			EasyTestOptions.CheckIsFormModal += new EventHandler<System.ComponentModel.CancelEventArgs>(delegate(object sender, System.ComponentModel.CancelEventArgs e) {
				e.Cancel = e.Cancel || (sender is LookupEditPopupForm);
			});
		}
		private void TraceException(Exception exception) {
			EasyTestTracer.Tracer.LogText("Exception:" + exception.Message);
			if(exception.InnerException != null) {
				EasyTestTracer.Tracer.LogText("Inner Exception:");
				TraceException(exception.InnerException);
			}
		}
		private object StartTimedOperation() {
			return DateTime.Now;
		}
		private void CheckTimedOperation(object context, string operationName) {
			TimeSpan span = DateTime.Now.Subtract((DateTime)context);
			if(span.TotalMinutes > 5)
				throw new Exception(string.Format("The '{0}' operation took {1} minutes. Timeout expired.", operationName, span.TotalMinutes.ToString()));
		}
		protected internal virtual Form GetActiveForm() {
			Form form = null;
			object context = StartTimedOperation();
			while(true) {
				CheckTimedOperation(context, "Getting Active Form");
				if(ProcessFormsEnumerator.IsFormAcceptable(Form.ActiveForm)) {
					form = Form.ActiveForm;
				}
				if(form == null) {
					ArrayList openForms = ProcessFormsEnumerator.GetApplicationOpenForms();
					for(int i = openForms.Count - 1; i >= 0; i--) {
						if(ProcessFormsEnumerator.IsFormAcceptable(openForms[i] as Form)) {
							form = openForms[i] as Form;
							break;
						}
					}
				}
				if(form != null && form.Visible && form.IsHandleCreated) {
					break;
				}
				else {
					Thread.Sleep(10);
				}
			}
			return form;
		}
		private delegate void ThreadExceptionHandler(Exception e);
		private delegate object FormInvokeHandle();
		private Form GetLogonForm() {
			Form result = GetActiveForm();
			if((result != null) && ((string)@"TestForm=""Logon""").Equals(result.Tag)) {
				return result;
			}
			return null;
		}
		public void WaitMainForm() {
			SynchronousMethodExecutor.Instance.Execute("WaitMainForm", delegate() {
				object context = StartTimedOperation();
				while(true) {
					Application.DoEvents();
					CheckTimedOperation(context, "Waiting the Main Form to be shown");
					Form form = GetActiveForm();
					if(form != null && (form.Tag != null || form is DevExpress.XtraEditors.XtraMessageBoxForm) && form != GetLogonForm() && form.Created &&
						form.Visible && form.IsHandleCreated && !form.IsDisposed) {
						form.Invoke(new ThreadStart(delegate() {
							string str = form.Text; 
							Application.DoEvents();
						}));
						break;
					}
					else {
						Thread.Sleep(100);
					}
				}
			});
		}
		public bool WaitLogonForm() {
			DevExpress.Persistent.Base.Tracing.Tracer.LogText("WinEasyTestCommandAdapter WaitLogonForm");
			bool result = false;
			SynchronousMethodExecutor.Instance.Execute("WaitLogonForm", delegate() {
				Form logonForm = null;
				object context = StartTimedOperation();
				while(true) {
					CheckTimedOperation(context, "Waiting the Logon Form to be shown");
					logonForm = GetLogonForm();
					Form openedForm = Form.ActiveForm;
					if(openedForm == null) {
						ArrayList open = ProcessFormsEnumerator.GetApplicationOpenForms();
						if(open.Count > 0 && ((Form)open[0]).Visible)
							openedForm = (Form)open[0];
					}
					if(logonForm == null && openedForm != null) {
						result = false;
						return;
					}
					if(logonForm != null && logonForm.Visible) {
						result = true;
						return;
					}
					Thread.Sleep(100);
				}
			});
			return result;
		}
		public IntPtr GetActiveFormHandle() {
			Form form = GetActiveForm();
			if(form != null) {
				return form.Handle;
			}
			return IntPtr.Zero;
		}
		public void DisableFormResizing() {
			SynchronousMethodExecutor.FormResizing = false;
		}
		#region ICommandAdapter Members
		public virtual ITestControl CreateTestControl(string controlType, string name) {
			object control = FindControl(controlType, name);
			if(control == null) {
				throw new AdapterOperationException(string.Format("Cannot find the '{0}' control, OperationTag:'{1}'", name, controlType));
			}
			ITestControl createControl = DevExpress.ExpressApp.EasyTest.WinAdapter.TestControls.TestControlFactoryWin.Instance.CreateControl(control);
			createControl.Name = name;
			return createControl;
		}
		public virtual object FindControl(string controlType, string name) {
			ControlFinder cf = new ControlFinder();
			return cf.FindControl(controlType, name);
		}
		public bool IsControlExist(string controlType, string name) {
			return FindControl(controlType, name) != null;
		}
		#endregion
	}
}
