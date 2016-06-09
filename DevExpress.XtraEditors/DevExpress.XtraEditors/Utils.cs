#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DevExpress.Utils.Win;
using DevExpress.Utils.Menu;
namespace DevExpress.XtraEditors.Native {
	[System.Security.SecuritySafeCritical]
	internal static class EditorsNativeMethods {
		#region Structs&Enums
		public enum PeekMessageOption {
			PM_NOREMOVE = 0,
			PM_REMOVE
		}
		public struct GetCaretPosPoint {
			public int x;
			public int y;
		}
		[StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
		public struct CURSORINFO {
			public int cbSize;
			public int flags;
			public IntPtr hCursor;
			public Point ptScreenPos;
		}
		#endregion Structs&Enums
		public static bool GetCursorInfo(out CURSORINFO info) {
			return UnsafeNativeMethods.GetCursorInfo(out info);
		}
		public static long GetWindowLong(HandleRef handle, int index) {
			return UnsafeNativeMethods.GetWindowLong(handle, index);
		}
		public static IntPtr SetWindowLongPtr32(HandleRef hWnd, int nIndex, HandleRef dwNewLong) {
			return UnsafeNativeMethods.SetWindowLongPtr32(hWnd, nIndex, dwNewLong);
		}
		public static IntPtr SetWindowLongPtr64(HandleRef hWnd, int nIndex, HandleRef dwNewLong) {
			return UnsafeNativeMethods.SetWindowLongPtr64(hWnd, nIndex, dwNewLong);
		}
		public static IntPtr SendMessage(HandleRef hWnd, int msg, IntPtr wParam, IntPtr lParam) {
			return UnsafeNativeMethods.SendMessage(hWnd, msg, wParam, lParam);
		}
		public static ushort GlobalAddAtom(string lpString) {
			return UnsafeNativeMethods.GlobalAddAtom(lpString);
		}
		public static ushort GlobalDeleteAtom(ushort nAtom) {
			return UnsafeNativeMethods.GlobalDeleteAtom(nAtom);
		}
		public static IntPtr RemoveProp(IntPtr hWnd, string lpString) {
			return UnsafeNativeMethods.RemoveProp(hWnd, lpString);
		}
		public static bool SetProp(IntPtr hWnd, string lpString, IntPtr hData) {
			return UnsafeNativeMethods.SetProp(hWnd, lpString, hData);
		}
		public static IntPtr GetMessageExtraInfo() {
			return UnsafeNativeMethods.GetMessageExtraInfo();
		}
		public static int GetSystemMetrics(int nIndex) {
			return UnsafeNativeMethods.GetSystemMetrics(nIndex);
		}
		public static bool ShowCaret(IntPtr hWnd) {
			return UnsafeNativeMethods.ShowCaret(hWnd);
		}
		public static bool HideCaret(IntPtr hWnd) {
			return UnsafeNativeMethods.HideCaret(hWnd);
		}
		public static short GetKeyState(int virtKey) {
			return UnsafeNativeMethods.GetKeyState(virtKey);
		}
		public static bool MessageBeep(uint uType) {
			return UnsafeNativeMethods.MessageBeep(uType);
		}
		public static bool GetCaretPos(out GetCaretPosPoint p) {
			return UnsafeNativeMethods.GetCaretPos(out p);
		}
		#region SecurityCritical
		static class UnsafeNativeMethods {
			[DllImport("user32.dll")]
			internal static extern bool GetCursorInfo(out CURSORINFO info);
			[DllImport("user32.dll", EntryPoint = "GetWindowLong", CharSet = CharSet.Auto)]
			internal static extern long GetWindowLong(HandleRef handle, int index);
			[DllImport("user32.dll", EntryPoint = "SetWindowLong", CharSet = CharSet.Auto)]
			internal static extern IntPtr SetWindowLongPtr32(HandleRef hWnd, int nIndex, HandleRef dwNewLong);
			[DllImport("user32.dll", EntryPoint = "SetWindowLongPtr", CharSet = CharSet.Auto)]
			internal static extern IntPtr SetWindowLongPtr64(HandleRef hWnd, int nIndex, HandleRef dwNewLong);
			[DllImport("user32.dll", CharSet = CharSet.Auto)]
			internal static extern IntPtr SendMessage(HandleRef hWnd, int msg, IntPtr wParam, IntPtr lParam);
			[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
			internal static extern ushort GlobalAddAtom(string lpString);
			[DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
			internal static extern ushort GlobalDeleteAtom(ushort nAtom);
			[DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
			internal static extern IntPtr RemoveProp(IntPtr hWnd, string lpString);
			[DllImport("user32.dll", SetLastError = true)]
			internal static extern bool SetProp(IntPtr hWnd, string lpString, IntPtr hData);
			[DllImport("user32.dll", SetLastError = false)]
			internal static extern IntPtr GetMessageExtraInfo();
			[DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
			internal static extern int GetSystemMetrics(int nIndex);
			[DllImport("user32.dll")]
			internal static extern bool HideCaret(IntPtr hWnd);
			[DllImport("user32.dll")]
			internal static extern bool ShowCaret(IntPtr hWnd);
			[DllImport("user32.dll")]
			internal static extern short GetKeyState(int virtKey);
			[DllImport("USER32.dll")]
			internal static extern bool MessageBeep(uint uType);
			[DllImport("USER32.dll")]
			internal static extern bool GetCaretPos(out GetCaretPosPoint p);
		}
		#endregion SecurityCritical
	}
	public static class MenuManagerUtils {
		public static void SetMenuManager(Control.ControlCollection controls, IDXMenuManager menuManager) {
			int count = controls.Count;
			for (int i = 0; i < count; i++) {
				Control control = controls[i];
				SetMenuManager(control.Controls, menuManager);
				BaseEdit baseEdit = control as BaseEdit;
				if (baseEdit == null)
					continue;
				baseEdit.MenuManager = menuManager;
			}
		}
	}
}
namespace DevExpress.XtraEditors.Controls {
	[ToolboxItem(false)]
	public class ModalTextBox : CustomTopForm {
		TextEdit te;
		public ModalTextBox() {
			this.FormBorderStyle = FormBorderStyle.FixedToolWindow;
			this.MinimumSize = Size.Empty;
			this.te = new TextEdit();
			this.te.BorderStyle = BorderStyles.NoBorder;
			this.te.Dock = DockStyle.Fill;
			this.te.Properties.AutoHeight = false;
			this.Controls.Add(te);
			this.KeyPreview = true;
		}
		public virtual TextEdit Editor { get { return te; } }
		protected override void OnClosing(CancelEventArgs e) {
			base.OnClosing(e);
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
		}
		protected override void OnVisibleChanged(EventArgs e) {
			base.OnVisibleChanged(e);
			if(Visible) {
				this.Activate();
				te.Focus();
			}
		}
		public Size CalcBestFit() {
			return te.ViewInfo.CalcBestFit(null);
		}
		[DXCategory(CategoryName.Appearance)]
		public Font EditFont { get { return te.Font; } set { te.Font = value; } }
		[DXCategory(CategoryName.Appearance)]
		public string EditText { get { return te.Text; } set { te.Text = value; } }
		protected override void OnKeyDown(KeyEventArgs e) {
			if(e.KeyData == Keys.Escape) {
				this.DialogResult = DialogResult.Cancel;
				Close();
				return;
			}
			if(e.KeyCode == Keys.Enter) {
				this.DialogResult = DialogResult.OK;
				Close();
				return;
			}
			base.OnKeyDown(e);
		}
	}
}
namespace DevExpress.XtraEditors.FeatureBrowser {
	[AttributeUsage(AttributeTargets.Class)]
	public class FeatureBrowserBaseType : Attribute {
		Type baseType;
		public FeatureBrowserBaseType(Type baseType) {
			this.baseType = baseType;
		}
		public Type BaseType { get { return baseType; } }
	}
}
