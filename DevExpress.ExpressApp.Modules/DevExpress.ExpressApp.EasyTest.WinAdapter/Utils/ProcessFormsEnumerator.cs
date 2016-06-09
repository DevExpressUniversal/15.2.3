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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Windows.Forms;
using DevExpress.EasyTest.Framework;
using DevExpress.ExpressApp.Win.EasyTest;
namespace DevExpress.ExpressApp.EasyTest.WinAdapter {
	public class ProcessFormsEnumerator {
		public delegate bool WndEnumProc(IntPtr hwnd, IntPtr lParam);
		private List<IntPtr> topLevelWndHandle;
		private static object lockObject;
		static ProcessFormsEnumerator() {
			lockObject = new object();
		}
		[SecuritySafeCritical]
		public static bool EnumWindows(WndEnumProc pfnEnum, IntPtr lParam) {
			return EnumWindowsNative(pfnEnum, lParam);
		}
		[DllImport("user32.dll", EntryPoint = "EnumWindows", CharSet = CharSet.Auto)]
		private static extern bool EnumWindowsNative(WndEnumProc pfnEnum, IntPtr lParam);
		[SecuritySafeCritical]
		public static IntPtr GetWindowThreadProcessId(IntPtr hWnd, ref IntPtr lpdwProcessId) {
			return GetWindowThreadProcessIdNative(hWnd, ref lpdwProcessId);
		}
		[DllImport("user32.dll", EntryPoint = "GetWindowThreadProcessId", CharSet = CharSet.Auto)]
		private static extern IntPtr GetWindowThreadProcessIdNative(IntPtr hWnd, ref IntPtr lpdwProcessId);
		[SecuritySafeCritical]
		public static bool IsWindowVisible(IntPtr hWnd) {
			return IsWindowVisibleNative(hWnd);
		}
		[DllImport("user32.dll", EntryPoint = "IsWindowVisible", CharSet = CharSet.Auto)]
		private static extern bool IsWindowVisibleNative(IntPtr hWnd);
		[SecuritySafeCritical]
		public static int GetClass(IntPtr hWnd, StringBuilder className, int nMaxCount) {
			return GetClassNameNative(hWnd, className, nMaxCount);
		}
		[DllImport("user32.dll", EntryPoint = "GetClassName")]
		public static extern int GetClassNameNative(IntPtr hWnd, StringBuilder className, int nMaxCount);
		public void FindOwnedFormsInternal(Process process) {
			EasyTestTracer.Tracer.LogVerboseText("Call Process at ProcessFormsEnumerator.FindOwnedFormsInternal(Process process)");
			EnumWindows(new WndEnumProc(MyWndEnumProc), new IntPtr(process.Id));
			EasyTestTracer.Tracer.LogVerboseText("Call Process success");
		}
		public Form[] GetActiveForms() { return GetActiveForms(false); }
		public Form[] GetActiveForms(bool activateForm) {
			Process process = Process.GetCurrentProcess();
			List<Form> result = new List<Form>();
			ArrayList applicationOpenForms = ProcessFormsEnumerator.GetApplicationOpenForms();
			EasyTestTracer.Tracer.LogVerboseText("ApplicationOpenForms count: " + applicationOpenForms.Count);
			for(int j = applicationOpenForms.Count - 1; j >= 0; j--) {
				Form form = (Form)applicationOpenForms[j];
				if(form != null && !form.IsDisposed && form.Visible) {
					result.Add(form);
					CollectOwnedForms(result, form);
				}
			}
			Form activeForm = Form.ActiveForm;
			for(int l = 0; l < result.Count; l++) {
				if(result[l] == activeForm) {
					result[l] = result[0];
					result[0] = activeForm;
					break;
				}
			}
			SortMDIChildren(result, GetMdiParent(result.AsReadOnly()));
			int i = 0;
			while(i < result.Count) {
				if(!IsFormAcceptable(result[i])) {
					result.RemoveAt(i);
				}
				else
					i++;
			}
			for(int k = 0; k < result.Count; k++) {
				if(IsFormModal(result[k])) {
					Form modalForm = result[k];
					result.RemoveRange(k + 1, result.Count - k - 1);
					break;
				}
			}
			return result.ToArray();
		}
		public static bool IsFormAcceptable(Form form) {
			return EasyTestOptions.OnCheckIsFormAcceptable(form);
		}
		public IntPtr[] GetActiveUnmanagedForms() {
			try {
				return GetActiveUnmanagedFormsCore();
			}
			catch(ArgumentOutOfRangeException) { 
				return GetActiveUnmanagedFormsCore();
			}
		}
		public static ArrayList GetApplicationOpenForms() {
			ArrayList result;
			if(!TryGetApplicationOpenFormsCore(out result)) {
				System.Threading.Thread.Sleep(200);
				TryGetApplicationOpenFormsCore(out result);
			}
			return result;
		}
		private static bool TryGetApplicationOpenFormsCore(out ArrayList forms) {
			forms = new ArrayList();
			FormCollection _formCollection = Application.OpenForms;
			int formsCount = _formCollection.Count;
			bool needRepit = false;
			for(int i = 0; i < formsCount; i++) {
				if(formsCount == _formCollection.Count) {
					try {
						Form form = _formCollection[i];
						if(!(form is DevExpress.XtraSplashForm.SplashFormBase)) {
							forms.Add(form);
						}
					}
					catch(ArgumentOutOfRangeException) {
						needRepit = true;
						break;
					}
				}
				else {
					needRepit = true;
					break;
				}
			}
			return !needRepit;
		}
		private IntPtr[] GetActiveUnmanagedFormsCore() {
			lock(lockObject) {
				Process process = Process.GetCurrentProcess();
				List<IntPtr> result = new List<IntPtr>();
				topLevelWndHandle = new List<IntPtr>();
				EnumWindows(new WndEnumProc(MyWndEnumProc), new IntPtr(process.Id));
				IntPtr[] list = topLevelWndHandle.ToArray();
				int bufferLength = 100;
				foreach(IntPtr wndHandle in list) {
					NativeWindow nativeWindow = NativeWindow.FromHandle(wndHandle);
					if(nativeWindow is DevExpress.XtraLayout.LayoutAdornerLayeredWindow) {
						continue;
					}
					Control form = Form.FromHandle(wndHandle);
					if(form == null && IsWindowVisible(wndHandle) && !result.Contains(wndHandle)) {
						StringBuilder classNameBuffer = new StringBuilder(bufferLength);
						GetClass(wndHandle, classNameBuffer, bufferLength);
						if("SysShadow" != classNameBuffer.ToString() &&
						   classNameBuffer.ToString() != "Internet Explorer_Hidden") {
							result.Add(wndHandle);
						}
					}
				}
				return result.ToArray();
			}
		}
		private bool MyWndEnumProc(IntPtr hwnd, IntPtr lParam) {
			IntPtr processId = new IntPtr();
			GetWindowThreadProcessId(hwnd, ref processId);
			if(lParam == processId) {
				topLevelWndHandle.Add(hwnd);
			}
			return true;
		}
		private static bool IsFormCompatible(List<Form> forms, Form form) {
			return form != null && !form.IsDisposed && !forms.Contains(form) && form.Visible;
		}
		private static void CollectOwnedForms(List<Form> forms, Form form) {
			if(IsFormCompatible(forms, form)) {
				forms.Add(form);
				List<Form> ownedForms = new List<Form>(form.OwnedForms);
				foreach(Form ownedForm in ownedForms) {
					CollectOwnedForms(forms, ownedForm);
				}
			}
		}
		private Form GetMdiParent(ReadOnlyCollection<Form> openedForms) {
			foreach(Form openedForm in openedForms) {
				if(openedForm.IsMdiContainer) {
					return openedForm;
				}
			}
			return null;
		}
		private void SortMDIChildren(List<Form> result, Form activeForm) {
			if(activeForm != null && activeForm.MdiChildren != null && activeForm.MdiChildren.Length != 0) {
				int firstChildIndex = -1;
				List<Form> mdiChildren = new List<Form>(activeForm.MdiChildren);
				for(int i = 0; i < result.Count; i++) {
					if(mdiChildren.Contains(result[i]) || activeForm == result[i]) {
						firstChildIndex = i;
						break;
					}
				}
				if(firstChildIndex >= 0) {
					for(int k = activeForm.MdiChildren.Length - 1; k >= 0; k--) {
						Form form = activeForm.MdiChildren[k];
						for(int l = 0; l < result.Count; l++) {
							if(result[l] == form) {
								result[l] = result[firstChildIndex];
								result[firstChildIndex] = form;
								break;
							}
						}
					}
					for(int l = 0; l < result.Count; l++) {
						if(result[l] == activeForm.ActiveMdiChild) {
							result[l] = result[firstChildIndex];
							result[firstChildIndex] = activeForm.ActiveMdiChild;
							break;
						}
					}
				}
			}
		}
		private bool IsFormModal(Form form) {
			return EasyTestOptions.OnCheckIsFormModal(form);
		}
	}
}
