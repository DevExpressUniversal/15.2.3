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
using System.Text;
using DevExpress.ExpressApp.EasyTest.WinAdapter.TestControls;
using System.Security;
namespace DevExpress.ExpressApp.EasyTest.WinAdapter {
	public class UnmanagedControlWrapperTestControl : TestControlBase, IControlAct, IControlText {
		private UnmanagedControlWrapper control;
		public UnmanagedControlWrapperTestControl(UnmanagedControlWrapper control)
			: base(control) {
			this.control = control;
		}
		#region IControlAct Members
		public void Act(string value) {
			control.PressSpace();
		}
		#endregion
		#region IControlText Members
		public string Text {
			get {
				return control.GetText();
			}
			set {
				control.SetText(value);
			}
		}
		#endregion
	}
	public class UnmanagedControlWrapper {
		IntPtr controlHandle;
		public UnmanagedControlWrapper(IntPtr controlHandle) {
			this.controlHandle = controlHandle;
		}
		public string GetText() {
			StringBuilder builder = new StringBuilder();
			int length = Win32APIHelper.SendMessage(controlHandle, Win32APIHelper.WM_GETTEXTLENGTH, 0, 0);
			builder.Capacity = length + 1;
			Win32APIHelper.SendMessage(controlHandle, Win32APIHelper.WM_GETTEXT, length + 1, builder);
			return builder.ToString().Replace("&", "").Replace(":", "");
		}
		public void SetText(string value) {
			Win32APIHelper.SendMessage(controlHandle, Win32APIHelper.WM_SETFOCUS, 0, 0);
			Win32APIHelper.SendMessage(controlHandle, Win32APIHelper.EM_SETSEL, 0, -1);
			Win32APIHelper.SendMessage(controlHandle, Win32APIHelper.WM_CLEAR, 0, IntPtr.Zero);
			Win32APIHelper.SendMessage(controlHandle, Win32APIHelper.WM_SETTEXT, 0, value);
		}
		public void PressSpace() {
			Win32APIHelper.SendMessage(controlHandle, Win32APIHelper.WM_KEYDOWN, Win32APIHelper.VK_SPACE, 1);
			Win32APIHelper.SendMessage(controlHandle, Win32APIHelper.WM_CHAR, Win32APIHelper.VK_SPACE, 1);
			Win32APIHelper.SendMessage(controlHandle, Win32APIHelper.WM_KEYUP, Win32APIHelper.VK_SPACE, 1);
		}
	}
	public static class Win32APIHelper {
		public const int WM_GETTEXT = 0x000D;
		public const int WM_GETTEXTLENGTH = 0x000E;
		public const int WM_SETTEXT = 0x000C;
		public const int WM_KEYDOWN = 0x0100;
		public const int WM_CHAR = 0x0102;
		public const int WM_KEYUP = 0x0101;
		public const int VK_SPACE = 0x20;
		public const uint WM_COMMAND = 0x0111;
		public const ushort BN_CLICKED = 0x0;
		public const ushort ButtonOpenID = 1;
		public const uint WM_CLEAR = 0x0303;
		public const uint EM_SETSEL = 0xB1;
		public const uint WM_SETFOCUS = 0x0007;
		[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
		public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
		[DllImport("user32.dll")]
		public static extern int SendMessage(IntPtr hwnd, int msg, int wParam, StringBuilder sb);
		[DllImport("USER32.DLL", EntryPoint = "SendMessage")]
		public static extern uint SendMessage(IntPtr hWnd, uint Msg, uint wParam, string lParam);
		[DllImport("USER32.DLL", EntryPoint = "SendMessage")]
		public static extern int SendMessage(IntPtr hWnd, uint Msg, uint wParam, IntPtr lParam);
		[DllImport("USER32.DLL", EntryPoint = "SendMessage")]
		public static extern int SendMessage(IntPtr hWnd, uint Msg, uint wParam, int lParam);
		[DllImport("USER32.DLL", EntryPoint = "FindWindowEx")]
		public static extern IntPtr FindWindowEx(IntPtr hwndParent, uint hwndChildAfter, string ClassName, string WindowName);
		[DllImport("USER32.DLL", EntryPoint = "EnumChildWindows")]
		public static extern bool EnumChildWindows(IntPtr hWndParent, DevExpress.ExpressApp.EasyTest.WinAdapter.ProcessFormsEnumerator.WndEnumProc lpEnumFunc, IntPtr lParam);
	}
	public class UnmanagedControlFinder : IControlFinder {
		string prevKey = "";
		private Dictionary<string, IntPtr> formControls = new Dictionary<string, IntPtr>();
		private void AddControl(IntPtr hwnd, string text) {
			StringBuilder builder = new StringBuilder();
			builder.Capacity = 100;
			Win32APIHelper.GetClassName(hwnd, builder, 100);
			if(text == "" && prevKey == "") {
				return;
			}
			if(builder.ToString() == "Button") {
				formControls.Add(text, hwnd);
			}
			else {
				if(builder.ToString() == "Static") {
					formControls.Add(text, IntPtr.Zero);
					prevKey = text;
					return;
				}
				if(prevKey!="") {
					formControls[prevKey] = hwnd;
					prevKey = "";
				}
			}
		}
		bool ChildEnumeration(IntPtr hwnd, IntPtr lParam) {
			StringBuilder builder = new StringBuilder();
			int length = Win32APIHelper.SendMessage(hwnd, Win32APIHelper.WM_GETTEXTLENGTH, 0, 0);
			builder.Capacity = length + 1;
			Win32APIHelper.SendMessage(hwnd, Win32APIHelper.WM_GETTEXT, length + 1, builder);
			AddControl(hwnd, builder.ToString().Replace("&", "").Replace(":", ""));
			return true;
		}
		private object FindFileNameXPStyle(IntPtr dialogHandle) {
			IntPtr ComboBoxHandle;
			IntPtr EditHandle;
			IntPtr ComboBoxEx32Handle = IntPtr.Zero;
			ComboBoxHandle = IntPtr.Zero;
			EditHandle = IntPtr.Zero;
			ComboBoxEx32Handle = Win32APIHelper.FindWindowEx(dialogHandle, 0, "ComboBoxEx32", null);
			if(ComboBoxEx32Handle != IntPtr.Zero) {
				ComboBoxHandle = Win32APIHelper.FindWindowEx(ComboBoxEx32Handle, 0, "ComboBox", null);
				if(ComboBoxHandle != IntPtr.Zero) {
					EditHandle = Win32APIHelper.FindWindowEx(ComboBoxHandle, 0, "Edit", null);
					if(EditHandle != IntPtr.Zero) {
						return new UnmanagedControlWrapper(EditHandle);
					}
				}
			}
			return null;
		}
		private object FindFileNameVistaStyle(IntPtr dialogHandle) {
			IntPtr ComboBoxHandle;
			IntPtr EditHandle;
			IntPtr DUIViewWndClassNameHandle = Win32APIHelper.FindWindowEx(dialogHandle, 0, "DUIViewWndClassName", null);
			if(DUIViewWndClassNameHandle != IntPtr.Zero) {
				IntPtr DirectUIHWNDHandle = Win32APIHelper.FindWindowEx(DUIViewWndClassNameHandle, 0, "DirectUIHWND", null);
				if(DirectUIHWNDHandle != IntPtr.Zero) {
					IntPtr FloatNotifySinkHandle = Win32APIHelper.FindWindowEx(DirectUIHWNDHandle, 0, "FloatNotifySink", null);
					if(FloatNotifySinkHandle != IntPtr.Zero) {
						ComboBoxHandle = Win32APIHelper.FindWindowEx(FloatNotifySinkHandle, 0, "ComboBox", null);
						if(ComboBoxHandle != IntPtr.Zero) {
							EditHandle = Win32APIHelper.FindWindowEx(ComboBoxHandle, 0, "Edit", null);
							if(EditHandle != IntPtr.Zero) {
								return new UnmanagedControlWrapper(EditHandle);
							}
						}
					}
				}
			}
			return null;
		}
		public object FindControl(IntPtr[] unmanagedHandles, string testTagType, string Name) {
			object result = FindInAllUnmanagedControls(unmanagedHandles[0], Name.TrimEnd(':'));
			if(result == null) {
				if(testTagType == TestControlType.Field && Name == "File name:") {
					foreach(IntPtr dialogHandle in unmanagedHandles) {
						result = FindFileNameXPStyle(dialogHandle);
						if(result != null) {
							return result;
						}
						result = FindFileNameVistaStyle(dialogHandle);
						if(result != null) {
							return result;
						}
					}
				}
				else {
					if(testTagType == TestControlType.Action.ToString() && (Name == "Open" || Name == "Save")) {
						foreach(IntPtr dialogHandle in unmanagedHandles) {
							IntPtr buttonHandle = Win32APIHelper.FindWindowEx(dialogHandle, 0, "Button", "&" + Name);
							if(buttonHandle != IntPtr.Zero) {
								return new UnmanagedControlWrapper(buttonHandle);
							}
						}
					}
				}
				result = null;
			}
			return result;
		}
		[SecuritySafeCritical]
		private object FindInAllUnmanagedControls(IntPtr dialogHandle, string name) {
			formControls.Clear();
			Win32APIHelper.EnumChildWindows(dialogHandle, new ProcessFormsEnumerator.WndEnumProc(ChildEnumeration), IntPtr.Zero);
			IntPtr controlHandle;
			if(formControls.TryGetValue(name, out controlHandle)) {
				return new UnmanagedControlWrapper(controlHandle);
			}
			return null;
		}
		#region IControlFinder Members
		public object Find(Form activeForm, string contolType, string caption) {
			ProcessFormsEnumerator formEnumerator = new ProcessFormsEnumerator();
			IntPtr[] unmanagedHandles = formEnumerator.GetActiveUnmanagedForms();
			if (unmanagedHandles.Length > 0) {
				return FindControl(unmanagedHandles, contolType, caption);
			}
			return null;
		}
		#endregion
	}
}
