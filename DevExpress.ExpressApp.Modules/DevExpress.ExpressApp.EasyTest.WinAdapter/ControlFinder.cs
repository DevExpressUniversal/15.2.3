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
using System.Threading;
using System.Windows.Forms;
using System.Collections;
using System.Diagnostics;
using DevExpress.XtraGrid;
using DevExpress.XtraBars;
using DevExpress.XtraBars.Controls;
using DevExpress.XtraBars.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Container;
using DevExpress.XtraLayout;
using DevExpress.XtraNavBar;
using DevExpress.XtraTab;
using DevExpress.XtraVerticalGrid;
using DevExpress.XtraVerticalGrid.Rows;
using DevExpress.EasyTest.Framework;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DevExpress.Utils.Menu;
using DevExpress.XtraScheduler;
using DevExpress.EasyTest.Framework.Utils;
using DevExpress.ExpressApp.EasyTest.WinAdapter.Utils;
using System.Security;
namespace DevExpress.ExpressApp.EasyTest.WinAdapter {
	public interface IControlFinder {
		object Find(Form activeForm, string contolType, string caption);
	}
	public class ControlFinderFactory : Singleton<ControlFinderFactory> {
		public IList<IControlFinder> controlFinders;
		public ControlFinderFactory() {
			controlFinders = new List<IControlFinder>();
			controlFinders.Add(new UnmanagedControlFinder());
			controlFinders.Add(new XafControlFinder());
			controlFinders.Add(new StandardControlFinder());
		}
		public virtual IList<IControlFinder> ControlFinders {
			get { return controlFinders; }
		}
	}
	public class ControlFinder {
		[System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto)]
		private static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);
		private const int WM_ACTIVATEAPP = 0x001C;
		Form[] activeForms;
		private object ThreadSafeFindControl(Form form, string controlType, string name) {
			object result = null;
			foreach(IControlFinder controlFinder in ControlFinderFactory.Instance.ControlFinders) {
				result = controlFinder.Find(form, controlType, name);
				if(result != null) {
					return result;
				}
			}
			return result;
		}
		public ControlFinder(params Form[] activeForms) {
			this.activeForms = activeForms;
		}
		public ControlFinder() {
			ProcessFormsEnumerator formEnumerator = new ProcessFormsEnumerator();
			activeForms = formEnumerator.GetActiveForms(false);
		}
		[SecuritySafeCritical]
		private void ActivateForm(Form form) {
			SendMessage(form.Handle, WM_ACTIVATEAPP, new IntPtr(1), new IntPtr(Thread.CurrentThread.GetHashCode()));
			form.Activate();
		}
		public object FindControl(string controlType, string name) {
			EasyTestTracer.Tracer.LogText("FindControl(" + controlType + ", " + name + ")");
			EasyTestTracer.Tracer.LogText("ActiveForms count: " + activeForms.Length.ToString());
			object control = null;
			foreach(Form form in activeForms) {
				EasyTestTracer.Tracer.LogText("FindControl: " + form.GetType().FullName);
				if (!form.IsDisposed && form.Visible) {
					ThreadStart threadStart = new ThreadStart(delegate() {
						control = ThreadSafeFindControl(form, controlType, name);
						if(control != null) {
							ActivateForm(form);
							Application.DoEvents();
							Application.DoEvents();
							Application.DoEvents();
						}
					});
					if(form.InvokeRequired) {
						try {
							form.Invoke(threadStart);
						}
						catch(Exception) {
							bool isClosing = (bool)typeof(Form).InvokeMember("IsClosing", System.Reflection.BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Instance, null, form, null);
							if(isClosing) {
								continue;
							}
							else if(form.IsHandleCreated) {
								throw;
							}
						}
					}
					else {
						threadStart.Invoke();
					}
					if(new ProcessFormsEnumerator().GetActiveForms().Length != activeForms.Length) {
						control = (new ControlFinder()).FindControl(controlType, name);
						break;
					}
					if(control != null)
						break;
				}
			}
			return control;
		}
	}
}
