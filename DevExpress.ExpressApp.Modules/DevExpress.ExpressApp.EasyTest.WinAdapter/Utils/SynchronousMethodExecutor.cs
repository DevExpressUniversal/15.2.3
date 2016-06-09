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
using System.Text;
using System.Windows.Forms;
using System.Threading;
using DevExpress.EasyTest.Framework;
using System.Collections;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Drawing;
using System.Collections.ObjectModel;
namespace DevExpress.ExpressApp.EasyTest.WinAdapter.Utils {
	public class SynchronousMethodExecutor : Singleton<SynchronousMethodExecutor> {
		class InvokedThreadContext {
			private int invokedThreadCount;
			public int InvokedThreadCount {
				get { return invokedThreadCount; }
				set {
					invokedThreadCount = value;
				}
			}
		}
		private delegate void ThreadExceptionHandler(Exception e);
		private int callCount = 0;
		private static bool formResizing = true;
		InvokedThreadContext currentThreadContext = new InvokedThreadContext();
		private ProcessFormsEnumerator pfe = new ProcessFormsEnumerator();
		private EasyTestException threadException = null;
		ManualResetEvent loopSynchronizer = new ManualResetEvent(false);
		private bool isApplicationQuit = false;
		private void Application_ApplicationExit(object sender, EventArgs e) {
			isApplicationQuit = true;
		}
		public delegate bool Predicate();
		public static bool FormResizing {
			get {
				return formResizing;
			}
			set {
				formResizing = value;
			}
		}
		public static bool ExecuteTimeoutFunction(int timeoutMilliseconds, Predicate function) {
			DateTime endTime = DateTime.Now.AddMilliseconds(timeoutMilliseconds);
			bool result = false;
			while(!((DateTime.Now > endTime) || (result = function()))) {
				Thread.Sleep(20);
			}
			if(!result) {
				EasyTestTracer.Tracer.LogVerboseText("Timeout expired:" + timeoutMilliseconds);
			}
			return result;
		}
		protected Form GetActiveForm() {
			Form result = null;
			ExecuteTimeoutFunction(60000, delegate() {
				Form[] activeForms = pfe.GetActiveForms();
				if(activeForms.Length > 0) {
					result = activeForms[0];
				}
				return result != null;
			});
			return result;
		}
		private void SetFormSizeAndLocation(Form form) {
			bool isAlivaForm = !form.IsDisposed && form.Visible;
			if(isAlivaForm) {
				form.Invoke(new ThreadStart(delegate() {
					bool canResize = form.FormBorderStyle == FormBorderStyle.Sizable && form.Size != WinAdapter.DefaultFormSize;
					if(canResize) {
						form.Size = WinAdapter.DefaultFormSize;
						EasyTestTracer.Tracer.LogVerboseText("SetSize");
					}
					if(!form.Location.IsEmpty) {
						form.Location = Point.Empty;
					}
				}));
			}
		}
		private void TraceException(Exception exception) {
			EasyTestTracer.Tracer.LogVerboseText("Exception:" + exception.Message);
			if(exception.InnerException != null) {
				EasyTestTracer.Tracer.LogVerboseText("Inner Exception:");
				TraceException(exception.InnerException);
			}
		}
		private void SafeExecuteMethod(string context, ThreadStart threadStart) {
			EasyTestTracer.Tracer.LogVerboseText(">>SafeExcuteMethod: " + context);
			try {
				threadStart.Invoke();
			}
			catch(EasyTestException e) {
				threadException = e;
			}
			catch(Exception e) {
				EasyTestTracer.Tracer.LogVerboseText(">>Application.OnThreadException:" + context);
				Application.OnThreadException(e);
				EasyTestTracer.Tracer.LogVerboseText("<<Application.OnThreadException:" + context);
			}
			EasyTestTracer.Tracer.LogVerboseText("<<SafeExcuteMethod: " + context);
		}
		private Thread StartInvokeThread(string context, InvokedThreadContext threadContext, ThreadStart threadStart, Form beforeExecuteActiveForm) {
			ManualResetEvent invokeThreadStarted = new ManualResetEvent(false);
			invokeThreadStarted.Reset();
			Thread invokeThread = new Thread(delegate() {
				try {
					EasyTestTracer.Tracer.LogVerboseText(">>Invoking:" + context);
					threadContext.InvokedThreadCount++;
					invokeThreadStarted.Set();
					loopSynchronizer.WaitOne();
					if(beforeExecuteActiveForm != null && beforeExecuteActiveForm.InvokeRequired) {
						beforeExecuteActiveForm.Invoke(new ThreadStart(delegate() {
							SafeExecuteMethod(context, threadStart);
						}));
					}
					else {
						SafeExecuteMethod(context, threadStart);
					}
					if(beforeExecuteActiveForm == null || beforeExecuteActiveForm.DialogResult != DialogResult.None) {
						EasyTestTracer.Tracer.LogVerboseText("Begin wait new Window");
						while(true) {
							Form activeForm = GetActiveForm();
							if(beforeExecuteActiveForm != activeForm) {
								EasyTestTracer.Tracer.LogVerboseText("End wait modal count changing");
								break;
							}
						}
					}
				}
				catch(Exception ex) {
					EasyTestTracer.Tracer.LogVerboseText("InvokeThread Exception:" + ex.Message);
				}
				finally {
					loopSynchronizer.WaitOne();
					EasyTestTracer.Tracer.LogVerboseText("<<Invoked:" + context);
					threadContext.InvokedThreadCount--;
				}
			});
			invokeThread.Name = context;
			invokeThread.Start();
			invokeThreadStarted.WaitOne();
			return invokeThread;
		}
		private ReadOnlyCollection<Form> GetModalForms() {
			List<Form> result = new List<Form>();
			foreach(Form form in ProcessFormsEnumerator.GetApplicationOpenForms()) {
				if(form.Modal) {
					result.Add(form);
				}
			}
			return result.AsReadOnly();
		}
		public SynchronousMethodExecutor() {
#if (!DebugTest)
			Application.ApplicationExit += new EventHandler(Application_ApplicationExit);
#endif
		}
		private bool IsUnmanagedFormsCountChanged(int unmanagedFormsCount) {
			int activeUnmanagedFormCount = pfe.GetActiveUnmanagedForms().Length;
			if(unmanagedFormsCount != activeUnmanagedFormCount) {
				EasyTestTracer.Tracer.LogVerboseText("Unmanaged forms count changed: " + activeUnmanagedFormCount);
				return true;
			}
			return false;
		}
		bool IsActiveModalFormBeforeOldForm(Form oldForm, ReadOnlyCollection<Form> currentModalForms, ReadOnlyCollection<Form> initialModalForms) {
			Form newModalForm = null;
			ExecuteTimeoutFunction(2000, delegate() {
				foreach(Form form in currentModalForms) {
					if(form.ActiveControl != null && !initialModalForms.Contains(form)) {
						newModalForm = form;
						return true;
					}
				}
				return false;
			});
			return newModalForm != null;
		}
		private ReadOnlyCollection<Form> UpdateModalForms(ReadOnlyCollection<Form> initialModalForms, int unmanagedFormsCount, Form beforeExecuteActiveForm) {
			ReadOnlyCollection<Form> currentModalForms = GetModalForms();
			if(currentModalForms.Count > 0) {
				bool isNewModal = currentModalForms.Count > initialModalForms.Count;
				if(isNewModal) {
					while(true) {
						if(isNewModal && IsActiveModalFormBeforeOldForm(beforeExecuteActiveForm, currentModalForms, initialModalForms)) {
							return currentModalForms;
						}
						if(pfe.GetActiveUnmanagedForms().Length > unmanagedFormsCount) {
							return currentModalForms;
						}
						Application.DoEvents();
					}
				}
			}
			return currentModalForms;
		}
		public virtual void ExecuteSimple(string context, ThreadStart threadStart) {
			EasyTestTracer.Tracer.InProcedure("ExecuteSimple");
			Form beforeExecuteActiveForm = GetActiveForm();
			if(beforeExecuteActiveForm.InvokeRequired) {
				beforeExecuteActiveForm.Invoke(threadStart);
			}
			else {
				threadStart.Invoke();
			}
			EasyTestTracer.Tracer.OutProcedure("ExecuteSimple");
		}
		private static void IsTimeout(ref bool isDone, DateTime startTime) {
			if(((TimeSpan)(DateTime.Now - startTime)).TotalSeconds > 30) {
				EasyTestTracer.Tracer.LogVerboseText("Timeout");
				isDone = true;
			}
		}
		public virtual void Execute(string context, ThreadStart threadStart) {
			EasyTestTracer.Tracer.LogText(">" + context);
			Form beforeExecuteActiveForm = GetActiveForm();
			if(beforeExecuteActiveForm != null) {
				EasyTestTracer.Tracer.LogVerboseText("beforeExecuteActiveForm:" + beforeExecuteActiveForm + " hashcode:" + beforeExecuteActiveForm.GetHashCode());
				if(FormResizing) {
					SetFormSizeAndLocation(beforeExecuteActiveForm);
				}
			}
			threadException = null;
			int unmanagedFormsCount = pfe.GetActiveUnmanagedForms().Length;
			InvokedThreadContext threadContext = currentThreadContext;
			int initialThreadCount = threadContext.InvokedThreadCount + 1;
			ReadOnlyCollection<Form> initialModalForms = GetModalForms();
			EasyTestTracer.Tracer.LogVerboseText("initialThreadCount: " + initialThreadCount + " initialModalCount: " + initialModalForms.Count + " Reenterable call: " + callCount); bool isDone = false;
			if(callCount > 0) {
				EasyTestTracer.Tracer.LogVerboseText("Reenterable call: " + callCount);
				threadStart.Invoke();
				isDone = true;
			}
			else {
				loopSynchronizer.Reset();
				Thread currentThread = StartInvokeThread(context, threadContext, threadStart, beforeExecuteActiveForm);
				isDone = false;
			}
			bool isModalClosed = false;
#if (DebugTest)
			DateTime startTime = DateTime.Now;
#endif
			try {
				callCount++;
				while(!isDone) {
#if (DebugTest)
					IsTimeout(ref isDone, startTime);
#endif
					loopSynchronizer.Reset();
					if(IsUnmanagedFormsCountChanged(unmanagedFormsCount)) {
						isDone = true;
					}
					if(isApplicationQuit) {
						EasyTestTracer.Tracer.LogVerboseText("Application is quit");
						isDone = true;
					}
					else {
						ReadOnlyCollection<Form> currentModalForms = UpdateModalForms(initialModalForms, unmanagedFormsCount, beforeExecuteActiveForm);
						if(isModalClosed) {
							if(currentModalForms.Count > initialModalForms.Count) {
								EasyTestTracer.Tracer.LogVerboseText("New Modal After Closed");
								isDone = true;
							}
							else {
								initialModalForms = currentModalForms;
							}
							if(threadContext.InvokedThreadCount == currentModalForms.Count || threadContext.InvokedThreadCount == 0) {
								isDone = true;
								EasyTestTracer.Tracer.LogVerboseText("All modal thread executed. Current modal count: " + currentModalForms.Count + ", InvokedThreadCount: " + threadContext.InvokedThreadCount);
							}
						}
						else {
							if(currentModalForms.Count != initialModalForms.Count) {
								if(currentModalForms.Count > initialModalForms.Count) {
									EasyTestTracer.Tracer.LogVerboseText("New modal form. Current modal count: " + currentModalForms.Count);
									isDone = true;
								}
								else {
									isModalClosed = true;
									initialModalForms = currentModalForms;
									EasyTestTracer.Tracer.LogVerboseText("Modal form closed. Current modal count: " + currentModalForms.Count);
								}
							}
							else {
								if(initialThreadCount != threadContext.InvokedThreadCount) {
									EasyTestTracer.Tracer.LogVerboseText("Thread was exited: " + threadContext.InvokedThreadCount);
									isDone = true;
								}
							}
						}
					}
					loopSynchronizer.Set();
					Thread.Sleep(10);
				}
				if(threadException != null) {
					TraceException(threadException);
					throw threadException;
				}
			}
			catch(InvalidOperationException ex) {
				EasyTestTracer.Tracer.LogText("Exception:" + ex.Message + " StackTrace: " + ex.StackTrace);
				throw new InvalidOperationException(ex.Message + " StackTrace: " + ex.StackTrace, ex);
			}
			catch(IndexOutOfRangeException ex) {
				EasyTestTracer.Tracer.LogText("Exception:" + ex.Message + " StackTrace: " + ex.StackTrace);
				throw new IndexOutOfRangeException(ex.Message + " StackTrace: " + ex.StackTrace, ex);
			}
			catch(Exception ex) {
				EasyTestTracer.Tracer.LogText("Exception:" + ex.Message + " StackTrace: " + ex.StackTrace);
				throw;
			}
			finally {
				callCount--;
				ExecuteTimeoutFunction(10000, delegate() {
					ArrayList coll = ProcessFormsEnumerator.GetApplicationOpenForms();
					return coll != null && coll.Count > 0;
				});
				EasyTestTracer.Tracer.LogText("<" + context);
			}
		}
	}
}
