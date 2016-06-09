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
using System.Windows.Forms;
using System.Drawing;
using System.ComponentModel;
using System.Collections;
using DevExpress.Utils.Win.Hook;
using DevExpress.Utils.Win;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Properties;
using System.Security;
using System.Runtime.InteropServices;
using System.Text;
using DevExpress.XtraEditors.Internal;
namespace DevExpress.XtraEditors {
	[AttributeUsage(AttributeTargets.All)]
	public sealed class DXCategoryAttribute : CategoryAttribute {
		public DXCategoryAttribute(string val)
			: base(val) {
		}
		protected override string GetLocalizedString(string val) {
			switch(val) {
				case CategoryName.Accessibility: return Resources.CategoryNameAccessibility;
				case CategoryName.Appearance: return Resources.CategoryNameAppearance;
				case CategoryName.Behavior: return Resources.CategoryNameBehavior;
				case CategoryName.Properties: return Resources.CategoryNameProperties;
				case CategoryName.Data: return Resources.CategoryNameData;
				case CategoryName.Format: return Resources.CategoryNameFormat;
				case CategoryName.Focus: return Resources.CategoryNameFocus;
				case CategoryName.Events: return Resources.CategoryNameEvents;
				case CategoryName.Action: return Resources.CategoryNameAction;
				case CategoryName.ToolTip: return Resources.CategoryNameToolTip;
				case CategoryName.PropertyChanged: return Resources.CategoryNamePropertyChanged;
				case CategoryName.Options: return Resources.CategoryNameOptions;
				case CategoryName.CustomDraw: return Resources.CategoryNameCustomDraw;
				case CategoryName.MasterDetail: return Resources.CategoryNameMasterDetail;
				case CategoryName.Editor: return Resources.CategoryNameEditor;
				case CategoryName.Customization: return Resources.CategoryNameCustomization;
				case CategoryName.DataAsync: return Resources.CategoryNameDataAsync;
				case CategoryName.Merging: return Resources.CategoryNameMerging;
				case CategoryName.Printing: return Resources.CategoryNamePrinting;
				case CategoryName.Sorting: return Resources.CategoryNameSorting;
				case CategoryName.CardOptions: return Resources.CategoryNameCardOptions;
				case CategoryName.DragDrop: return Resources.CategoryNameDragDrop;
				case CategoryName.Layout: return Resources.CategoryNameLayout;
				case CategoryName.Grid: return Resources.CategoryNameGrid;
				case CategoryName.Split: return Resources.CategoryNameSplit;
				case CategoryName.Mouse: return Resources.CategoryNameMouse;
				case CategoryName.Key: return Resources.CategoryNameKey;
			}
			return val;
		}
	}
	public class CategoryName {
		public const string Accessibility = "Accessibility";
		public const string Appearance = "Appearance";
		public const string Behavior = "Behavior";
		public const string BarManager = "BarManager";
		public const string Properties = "Properties";
		public const string Data = "Data";
		public const string Format = "Format";
		public const string Focus = "Focus";
		public const string Events = "Events";
		public const string NativeEventsCategory = "Events";
		public const string Action = "Action";
		public const string ContainerEvents = "ContainerEvents";
		public const string PropertyChanged = "Property Changed";
		public const string ToolTip = "ToolTip";
		public const string Options = "Options";
		public const string CustomDraw = "CustomDraw";
		public const string MasterDetail = "MasterDetail";
		public const string Editor = "Editor";
		public const string Customization = "Customization";
		public const string DataAsync = "DataAsync";
		public const string Merging = "Merging";
		public const string Printing = "Printing";
		public const string Sorting = "Sorting";
		public const string CardOptions = "CardOptions";
		public const string DragDrop = "DragDrop";
		public const string Layout = "Layout";
		public const string Mouse = "Mouse";
		public const string Key = "Key";
		public const string Grid = "Grid";
		public const string Split = "Split";
		public const string ContextButtons = "Context Buttons";
	}
}
namespace DevExpress.XtraEditors.Controls {
	public class PopupController {
		public static IPopupServiceControl CreateController() {
			return new WinPopupController();
		}
	}
	public class WinPopupController : IPopupServiceControl {
		void IPopupServiceControl.UpdateTopMost(IntPtr handle) {
			NativeMethods.SetWindowPos(handle, (IntPtr)HWND_TOPMOST, 0, 0, 0, 0, SWP_NOACTIVATE | SWP_SHOWWINDOW | SWP_NOSIZE | SWP_NOMOVE);
		}
		bool IPopupServiceControl.SetVisibleCore(Control control, bool newVisible) {
#if DXWhidbey
			if(newVisible) {
				(this as IPopupServiceControl).UpdateTopMost(control.Handle);
				NativeMethods.ShowWindow(control.Handle, 8);
			}
			return true;
#else
			if(!newVisible)
				return false;
			else {
				(this as IPopupServiceControl).UpdateTopMost(control.Handle);
				ShowWindow(control.Handle, 8);
				return true;
			}
#endif
		}
		bool IPopupServiceControl.SetSimpleVisibleCore(Control control, IntPtr parentForm, bool newVisible) {
#if DXWhidbey
			if(newVisible) {
				NativeMethods.SetWindowPos(control.Handle, parentForm, 0, 0, 0, 0, SWP_NOACTIVATE | SWP_SHOWWINDOW | SWP_NOMOVE | SWP_NOSIZE);
				if(parentForm != IntPtr.Zero)
					ReparentWindow(control, parentForm);
				NativeMethods.ShowWindow(control.Handle, 8);
			}
			return true;
#else
			if(!newVisible)
				return false;
			else {
				SetWindowPos(control.Handle, parentForm, 0,0,100,100, SWP_NOACTIVATE | SWP_SHOWWINDOW | SWP_NOMOVE | SWP_NOSIZE);
				ShowWindow(control.Handle, 8);
				return true;
			}
#endif
		}
		public void ReparentWindow(Control control, IntPtr parentForm) {
			if(!control.IsHandleCreated) return;
			parentForm = NativeMethods.GetDesktopWindow(); 
			NativeMethods.SetWindowLong(control.Handle, -8, parentForm);
		}
		public void EmulateFormFocus(IntPtr formHandle) {
			DevExpress.Utils.Drawing.Helpers.NativeMethods.SendMessage(formHandle, 0x86, 1, (IntPtr) (-1)); 
			DevExpress.Utils.Drawing.Helpers.NativeMethods.RedrawWindow(formHandle, IntPtr.Zero, IntPtr.Zero, 0x401);
		}
		public virtual void PopupClosed(IPopupControl popup) {
		}
		public virtual void PopupShowing(IPopupControl popup) {
		}
		protected const int MA_NOACTIVATE = 3;
		protected const int WM_MOUSEACTIVATE = 0x0021,
			WM_LBUTTONDOWN   = 0x0201;
		bool IPopupServiceControl.WndProc(ref System.Windows.Forms.Message m) {
			Control control = Control.FromHandle(m.HWnd);
			switch(m.Msg) {
				case WM_MOUSEACTIVATE : 
					m.Result = (IntPtr)MA_NOACTIVATE;
					return true;
				case WM_LBUTTONDOWN : 
					if(control is ListBox) return true;
					break;
			}
			return false;
		}
		bool IPopupServiceControl.IsDummy { get { return false; } 
		}
		const int SWP_NOSIZE = 0x0001,
			SWP_NOMOVE = 0x0002,
			SWP_NOACTIVATE = 0x0010,
			SWP_SHOWWINDOW = 0x0040,
			HWND_TOPMOST = -1;
	}
	public class HookPopup : IHookController, IDisposable {
		IPopupControl popup;
		public HookPopup(IPopupControl popup) {
			this.popup = popup;
			HookManager.DefaultManager.AddController(this);
		}
		public virtual void Dispose() {
			HookManager.DefaultManager.RemoveController(this);
		}
		public IPopupControl Popup { get { return popup; } }
		public IPopupControlEx PopupEx { get { return Popup as IPopupControlEx; } }
		protected Control GetParent(Control control) {
			Control parent = control;
			while(parent.Parent != null) parent = parent.Parent;
			return parent;
		}
		protected bool CheckMouseDown(IntPtr hWnd, Control control, Point mousePosition) {
			Control popup = Popup as Control;
			if(popup != null && PopupHelper.IsBelowModalForm(popup, !PopupHelper.AllowClosePopupOnModalFormShow))
				return false;
			IPopupControl pc = Popup;
			if(pc == null || !popup.Created || !popup.Visible || pc.PopupWindow == null || !pc.PopupWindow.Visible) return false;
			Control parent = GetParent(pc.PopupWindow);
			if(parent.Contains(control) || parent == control || popup == control || popup.Contains(control)) return false;
			if(IsPopupMenu(control)) return false;
			if(!pc.AllowMouseClick(control, mousePosition)) {
				pc.ClosePopup();
				return pc.SuppressOutsideMouseClick;
			}
			return false;
		}
		protected bool IsPopupMenu(Control control) {
			if(control is Shadow) return true;
			if(control != null && (control.GetType().Name == "SubMenuBarControl" || control.GetType().Name == "PopupMenuBarControl")) return true; 
			return false;
		}
		protected void ClosePopups() {
			Popup.ClosePopup();
		}
		const int WM_ACTIVATEAPP = 0x1C,WM_MINCLICK = 0x201, WM_MAXCLICK = 0x208, WM_ACTIVATE = 0x6, 
			WM_MINNCCLICK = 0xA1, WM_MAXNCCLICK = 0x0a9, WM_LBUTTONUP = 0x0202, WM_RBUTTONUP = 0x0205, WM_MBUTTONUP = 0x0208, WA_INACTIVE = 0;
		static int[] upMessages = new int[] { WM_LBUTTONUP, WM_RBUTTONUP, WM_MBUTTONUP};
		public static Control GetControlFromHandle(IntPtr hwnd) { return GetControlFromHandle(hwnd, false); }
		public static Control GetControlFromHandle(IntPtr hwnd, bool allowChildhandle) {
			Control control = Control.FromHandle(hwnd);
			if(control == null && allowChildhandle) control = Control.FromChildHandle(hwnd);
			if(control == null) {
				FloatingScrollbar sb = NativeWindow.FromHandle(hwnd) as FloatingScrollbar;
				if(sb != null && sb.Owner != null) control = sb.Owner.Parent;
			}
			return control;
		}
		protected Control GetControl(IntPtr hwnd) {
			return GetControlFromHandle(hwnd, true);
		}
		bool IHookController.InternalPreFilterMessage(int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam) {
			return PreFilterMessageCore(Msg, HWnd, WParam);
		}
		protected virtual bool PreFilterMessageCore(int Msg, IntPtr HWnd, IntPtr WParam) {
			if((Msg >= WM_MINCLICK && Msg <= WM_MAXCLICK) || (Msg >= WM_MINNCCLICK && Msg <= WM_MAXNCCLICK)) {
				if(Array.IndexOf(upMessages, Msg) != -1) return false;
				return CheckMouseDown(HWnd, GetControl(HWnd), Control.MousePosition);
			}
			if(Msg == WM_ACTIVATEAPP) {
				if(WParam.ToInt32() == 0) ClosePopups();
			}
			if(Msg == 0x18 && WParam == IntPtr.Zero) { 
				if(PopupEx != null) PopupEx.WindowHidden(GetControl(HWnd));
			}
			if(Msg == WM_ACTIVATE) {
				if(PopupEx != null && (WParam.ToInt32() & 0xffff) != WA_INACTIVE) PopupEx.CheckClosePopup(GetControl(HWnd));
			}
			return false;
		}
		bool IHookController.InternalPostFilterMessage(int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam) {
			return false;
		}
		IntPtr IHookController.OwnerHandle {
			get {
				Control popup = Popup as Control;
				return popup == null || !popup.IsHandleCreated ? IntPtr.Zero : popup.Handle;
			}
		}
	}
	public class HookPopupController : WinPopupController {
		ArrayList popups;
		protected ArrayList Popups { get { return popups; } }
		public HookPopupController() {
			popups = new ArrayList();
		}
		protected virtual HookPopup FindPopup(IPopupControl popup) {
			for(int n = 0; n < Popups.Count; n++) {
				HookPopup ppp = Popups[n] as HookPopup;
				if(ppp.Popup == popup) return ppp;
			}
			return null;
		}
		public override void PopupClosed(IPopupControl popup) {
			HookPopup ppp = FindPopup(popup);
			if(ppp != null) {
				Popups.Remove(ppp);
				ppp.Dispose();
			}
		}
		public override void PopupShowing(IPopupControl popup) {
			Popups.Add(new HookPopup(popup));
		}
	}
	public static class PopupHelper {
		static PopupHelper() { 
			AllowClosePopupOnModalFormShow = true; 
		}
		public delegate bool CallBackPtr(int hwnd, int lParam);
		[DllImport("user32.dll")]
		private static extern int EnumWindows(CallBackPtr callPtr, int lPar);
		[DllImport("user32.dll")]
		static extern IntPtr GetTopWindow(IntPtr hWnd);
		enum GetWindow_Cmd : uint {
			GW_HWNDFIRST = 0,
			GW_HWNDLAST = 1,
			GW_HWNDNEXT = 2,
			GW_HWNDPREV = 3,
			GW_OWNER = 4,
			GW_CHILD = 5,
			GW_ENABLEDPOPUP = 6
		}
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
		[DllImport("user32.dll")]
		static extern uint GetWindowThreadProcessId(IntPtr hWnd, IntPtr ProcessId);
		[SecuritySafeCritical]
		public static bool IsBelowModalForm(Control control, bool processModalForms) {
			if(!processModalForms || control == null)
				return false;
			Form frm = control.FindForm();
			if(frm == null || !frm.Visible)
				return false;
			uint processId1 = GetWindowThreadProcessId(frm.Handle, IntPtr.Zero);
			IntPtr handle = GetTopWindow(IntPtr.Zero);
			while(handle != IntPtr.Zero) {
				if(handle == IntPtr.Zero || handle == frm.Handle)
					return false;
				uint processId2 = GetWindowThreadProcessId(handle, IntPtr.Zero);
				if(processId1 == processId2 && IsModalDialog(handle))
					return true;
				handle = NativeMethods.GetWindow(handle, (int)GetWindow_Cmd.GW_HWNDNEXT);
			}
			return false;
		}
		enum GetAncestorFlags {
			GetParent = 1,
			GetRoot = 2,
			GetRootOwner = 3
		}
		[DllImport("user32.dll", ExactSpelling = true)]
		static extern IntPtr GetAncestor(IntPtr hwnd, GetAncestorFlags flags);
		[SecuritySafeCritical]
		static bool IsModalDialog(IntPtr handle) {
			int GWL_STYLE = -16;
			int GWL_EXSTYLE = -20;
			const UInt32 WS_DISABLED = 0x8000000;
			const UInt32 WS_POPUP = 0x80000000;
			const UInt32 WS_VISIBLE = 0x10000000;
			const UInt32 WS_EX_TOPMOST = 0x00000008;
			int style = NativeMethods.GetWindowLong(handle, GWL_STYLE);
			int exstyle = NativeMethods.GetWindowLong(handle, GWL_EXSTYLE);
			if((style & WS_VISIBLE) != WS_VISIBLE)
				return false;
			if((style & WS_POPUP) != WS_POPUP && (exstyle & WS_EX_TOPMOST) != WS_EX_TOPMOST)
				return false;
			if(GetAncestor(handle, GetAncestorFlags.GetParent) != NativeMethods.GetDesktopWindow())
				return false;
			IntPtr owner = NativeMethods.GetWindow(handle, (int)GetWindow_Cmd.GW_OWNER);
			int ownerStyle = NativeMethods.GetWindowLong(owner, GWL_STYLE);
			if((ownerStyle & WS_DISABLED) != WS_DISABLED)
				return false;
			return true;
		}
		static bool EnumWindowProc(int hwnd, int lParam) {
			return true;
		}
		public static bool AllowClosePopupOnModalFormShow { get; set; }
	}
	public static class PopupHookHelper {
		public static void FocusFormControl(Control control, Form owner, IPopupServiceControl popupService) {
			if(control == null)
				return;
			XtraForm xform = owner as XtraForm;
			if(xform != null) xform.SuspendRedraw();
			try {
				XtraForm.SuppressDeactivation = true;
				control.Focus();
				XtraForm.SuppressDeactivation = false;
				if(owner == null || !owner.IsHandleCreated) return;
				popupService.EmulateFormFocus(owner.Handle);
				if(owner.MdiParent != null && owner.MdiParent.IsHandleCreated) {
					popupService.EmulateFormFocus(owner.MdiParent.Handle);
				}
			}
			finally {
				if(xform != null) xform.ResumeRedraw();
			}
		}
	}
}
