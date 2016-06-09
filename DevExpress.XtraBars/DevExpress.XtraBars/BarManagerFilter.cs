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

using DevExpress.XtraBars.Controls;
using System.Runtime.InteropServices;
using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.Utils.Win.Hook;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraBars.Forms;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Win;
using System.Collections.Generic;
using DevExpress.XtraBars.Ribbon.ViewInfo;
namespace DevExpress.XtraBars.MessageFilter {
	internal class CachedForm {
		static Form activeForm;
		public static Form ActiveForm {
			get { return activeForm; }
			set { activeForm = value; }
		}
	}
	public enum BarManagerHookResult { NotProcessed, Processed, ProcessedExit }
	public class BarHideException : Exception { }
	[System.Security.SecuritySafeCritical]
	public class BarManagerHook : IHookControllerWithResult {
		internal const int WM_ACTIVATEAPP = 0x1C, 
			WM_APP = 0x8000,
			XB_CheckMdi = WM_APP + 100,
			WM_CREATE = 0x0001,
			WM_NCACTIVATE = 0x0086,
			WM_SIZE = 5,
			WM_MOVE = 3,
			WM_ACTIVATE = 0x6,
			WM_KILLFOCUS = 0x8, 
			WM_SETFOCUS = 0x7, 
			WM_ENABLE = 0x000A,
			WM_PAINT = 0x000F,
			WM_ERASEBKGND = 0x0014,
			WM_WINDOWPOSCHANGED = 0x0047,
			WM_WINDOWPOSCHANGING = 0x0046,
			WM_NCLBUTTONDOWN = 0x00A1,
			WM_NCLBUTTONUP = 0x00A2,
			WM_NCLBUTTONDBLCLK = 0x00A3,
			WM_NCRBUTTONDOWN = 0x00A4,
			WM_NCRBUTTONUP = 0x00A5,
			WM_NCRBUTTONDBLCLK = 0x00A6,
			WM_NCMBUTTONDOWN = 0x00A7,
			WM_NCMBUTTONUP = 0x00A8,
			WM_NCMBUTTONDBLCLK = 0x00A9,
			WM_LBUTTONDOWN = 0x0201,
			WM_LBUTTONUP = 0x0202,
			WM_LBUTTONDBLCLK = 0x0203,
			WM_RBUTTONDOWN = 0x0204,
			WM_RBUTTONUP = 0x0205,
			WM_RBUTTONDBLCLK = 0x0206,
			WM_MBUTTONDOWN = 0x0207,
			WM_MBUTTONUP = 0x0208,
			WM_MBUTTONDBLCLK = 0x0209, WM_MINCLICK = 0x201, WM_MAXCLICK = 0x208, 
			WM_MINNCCLICK = 0xA1, WM_MAXNCCLICK = 0x0a9,
			WM_MOUSEWHEEL = 0x020A,
			WM_DESTROY = 0x02,
			WM_CHILDACTIVATE = 0x0022,
			WM_MDICREATE = 0x0220,
			WM_MDIDESTROY = 0x0221,
			WM_MDIACTIVATE = 0x0222,
			WM_MDIRESTORE = 0x0223,
			WM_MDINEXT = 0x0224,
			WM_MDIMAXIMIZE = 0x0225,
			WM_MDISETMENU = 0x0230,
			WM_CONTEXTMENU = 0x007B,
			WM_SYSCOLORCHANGE = 0x15,
			WA_INACTIVE = 0,
			WM_EXITMENULOOP = 530,
			WM_MENUCHAR = 288,
			WM_SYSCHAR = 0x0106,
			WM_COMMAND = 273,
			WM_MENUSELECT = 0x011F;
		BarManager manager;
		public BarManagerHook(BarManager manager) {
			this.manager = manager;
			HookManager.DefaultManager.AddController(this);
		}
		BarManagerHookResult resultCore;
		internal BarManagerHookResult ResultCore { get { return resultCore; } set { resultCore = value; } }
		public void CheckController() {
			HookManager.DefaultManager.CheckController(this);
		}
		public BarManager Manager { get { return manager; } }
		public RibbonBarManager RibbonManager { get { return manager as RibbonBarManager; } }
		public RibbonControl Ribbon { get { return RibbonManager != null? RibbonManager.Ribbon: null; } }
		public virtual void Dispose() {
			HookManager.DefaultManager.RemoveController(this);
		}
		HookResult hookResult;
		HookResult IHookControllerWithResult.Result { get { return hookResult; } set { hookResult = value; } }
		void UpdateMdiClient(MdiClient mdiClient) {
			if(!BarManager.UpdateMdiClientOnChildActivate)
				return;
			DevExpress.Skins.XtraForm.FormPainter.UpdateMdiClient(mdiClient);
		}
		bool IHookController.InternalPostFilterMessage(int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam) {
			((IHookControllerWithResult)this).Result = HookResult.Unknown;
			Form form = wnd as Form;
			Form activeChild = null;
			if(form == null) return false;
			switch(Msg) {
				case WM_CHILDACTIVATE :
					if(form.IsMdiChild && form.MdiParent != null) {
						BarManager manager = BarManager.FindManager(form.MdiParent);
						if(manager == Manager) {
							manager.CreateCheckMdiTimer();
						}
					}
					break;
				case WM_MDIACTIVATE :
					activeChild = form;
					goto case WM_DESTROY;
				case WM_DESTROY :
					if(form.IsMdiChild && form.MdiParent != null) {
						BarManager manager = BarManager.FindManager(form.MdiParent);
						if(manager == Manager) { 
							manager.Helper.MdiHelper.DoCheckMdi(activeChild);
						}
					}
					break;
			}
			return false;
		}
		bool IHookController.InternalPreFilterMessage(int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam) {
			((IHookControllerWithResult)this).Result = HookResult.Unknown;
			bool result = false;
			try {
				if(Manager.FilterForm == null || Manager.ThreadId != HookManager.GetCurrentThreadId()) return result;
				result |= InternalPreFilterMessage(Manager, Msg, wnd, HWnd, WParam, LParam);
			}
			catch(Exception e) {
				Application.OnThreadException(e);
			}
			return result;
		}
		IntPtr IHookController.OwnerHandle { 
			get {
				if(Manager.FilterForm == null || !Manager.FilterForm.IsHandleCreated) return IntPtr.Zero;
				return Manager.FilterForm.Handle;
			}
		}
		bool CheckEditorClick(BarManager manager, Control control, int msg) { 
			if(manager.SelectionInfo.ActiveEditor == null) return false;
			BarEditItemLink link = manager.SelectionInfo.EditingLink;
			if(link == null || link.LinkViewInfo == null) return false;
			if(msg == WM_LBUTTONUP || msg == WM_RBUTTONUP) return true;
			Point p = link.ScreenToLinkPoint(Control.MousePosition);
			if(link.LinkViewInfo.Bounds.Contains(p)) return true;
			BaseEdit edt = manager.SelectionInfo.ActiveEditor;
			if(edt.AllowMouseClick(control, Control.MousePosition)) {
				if(!CheckPopupEditorClick(control, Control.MousePosition))
					return false;
				return true;
			}
			return false;
		}
		private bool CheckPopupEditorClick(Control control, Point point) {
			if(manager.ActiveEditItemLink.Item.CloseOnMouseOuterClick != DevExpress.Utils.DefaultBoolean.True)
				return true;
			PopupBaseEdit popupEdit = manager.SelectionInfo.ActiveEditor as PopupBaseEdit;
			if(popupEdit == null)
				return true;
			if(popupEdit.RectangleToScreen(popupEdit.ClientRectangle).Contains(point))
				return true;
			Form form = ((IPopupControl)popupEdit).PopupWindow as Form;
			if(form == null || !form.Visible)
				return true;
			if(!form.Bounds.Contains(point))
				return false;
			return true;
		}
		protected virtual bool IsButtonCaptionEditor(Control control) {
			if (control == null || !(control is TextBoxMaskBox)) return false;
			if (control.Parent is ModalTextBox) return true;
			return control.Parent != null && control.Parent.Parent is ModalTextBox;
		}
		protected virtual bool IsPropertyGridActive(Control control) {
			if(control == null) return false;
			return control.GetType().Name == "PropertyGridView" || (control.Parent != null && control.Parent.GetType().Name == "PropertyGridView");
		}
		protected virtual BarMenuCloseType ShouldCloseOnClick(BarManager manager, Control control, MouseInfoArgs e) {
			if(control is DevExpress.Utils.Win.Shadow) return BarMenuCloseType.None;
			IBarObject barObject = GetBarObject(manager, control);
			if(manager.IsDesignMode) {
				if(control != null) {
					if(IsButtonCaptionEditor(control)) return BarMenuCloseType.None;
					if(IsPropertyGridActive(control)) return BarMenuCloseType.None;
				}
			}
			if(barObject == null) {
				if(manager.SelectionInfo.ActiveBarControl != null) {
					barObject = manager.SelectionInfo.ActiveBarControl as IBarObject;
					return (barObject == null || barObject.ShouldCloseOnOuterClick(control, e)) ? BarMenuCloseType.All : BarMenuCloseType.None;
				}
				return BarMenuCloseType.All;
			}
			return barObject.ShouldCloseMenuOnClick(e, control);
		}
		protected internal IBarObject GetBarObject(BarManager manager, Control control) {
			int level = 0;
			IBarObjectContainer boc = control as IBarObjectContainer;
			if(boc != null && boc.ContainsObject)
				control = boc.BarObject as Control;
			while(control != null) {
				if(control is IBarObject && !(control is PopupControlContainer)) {
					IBarObject bo = control as IBarObject;
					if(bo.IsBarObject) {
						if(bo.Manager == manager) return bo;
						if(manager.internalMenuManager != null && manager.internalMenuManager == bo.Manager) return bo;
						if(manager.Helper.CustomizationManager.DesignTimeManager != null) {
							if(manager.Helper.CustomizationManager.DesignTimeManager.DesignManager == bo.Manager) return bo;
						}
						if(bo.Manager != null && bo.Manager.FilterForm != null) 
							control = bo.Manager.FilterForm;
						else 
						break;
					}
				}
				PopupBaseForm editorPopupForm = control as PopupBaseForm;
				if(editorPopupForm == null) editorPopupForm = control.Parent as PopupBaseForm;
				if(editorPopupForm != null && editorPopupForm.OwnerEdit != null) 
					control = editorPopupForm.OwnerEdit;
				else 
				control = control.Parent;
				if(level ++ == 8) break;
			}
			return null;
		}
		private bool CheckControl(BarManager manager, Control control) {
			return GetBarObject(manager, control) != null;
		}
		protected bool CanHideBars(BarManager manager) {
			IDesignerEventService ide = manager.InternalGetService(typeof(IDesignerEventService)) as IDesignerEventService;
			if(ide == null || ide.ActiveDesigner == null) return true;
			IComponent comp = ide.ActiveDesigner.RootComponent;
			if(comp == null) return true;
			ContainerControl ctrl = comp as ContainerControl;
			if(ctrl == null) return true;
			if(manager.FilterForm == ctrl || ctrl.Contains(manager.FilterForm) || manager.FilterForm.Contains(ctrl)) return false;
			return true;
		}
		MouseInfoArgs CreateMouseArgs(int Msg, IntPtr wparam, Point pt) {
			MouseButtons btn = MouseButtons.Left;
			bool isMouseUp = Msg == WM_LBUTTONUP || Msg == WM_RBUTTONUP || Msg == WM_MBUTTONUP;
			int delta = 0;
			switch(Msg) {
				case WM_MOUSEWHEEL:
					btn = MouseButtons.None;
					delta = (wparam.ToInt32() >> 16);
					break;
				case WM_MBUTTONDOWN:
				case WM_MBUTTONUP:
					btn = MouseButtons.Left;
					break;
				case WM_RBUTTONDOWN:
				case WM_RBUTTONUP:
					btn = MouseButtons.Right;
					break;
				default:
					btn = MouseButtons.Left;
					break;
			}
			return new MouseInfoArgs(new MouseEventArgs(btn, 1, pt.X, pt.Y, delta), isMouseUp);			
		}
		MouseInfoArgs CreateMouseArgs(int Msg, IntPtr wparam) {
			return CreateMouseArgs(Msg, wparam, new Point(Control.MousePosition.X, Control.MousePosition.Y));
		}
		MdiClient GetMdiClient(Form frm) {
			if(frm == null)
				return null;
			foreach(Control ctrl in frm.Controls) {
				if(ctrl is MdiClient)
					return (MdiClient)ctrl;
			}
			return null;
		}
		bool HasVisibleMdiChild(MdiClient client) {
			foreach(Control control in client.Controls) {
				if(control.Visible)
					return true;
			}
			return false;
		}
		void OnWmButtonDown(Control control) {
			if(control == null)
				return;
			CustomFloatingForm floatForm = control as CustomFloatingForm;
			if(floatForm == null) floatForm = control.FindForm() as CustomFloatingForm;
			if(floatForm != null && !floatForm.Focused)
				floatForm.FocusCore();
		}
		void UpdateRibbonObjectsOnMouseMove(BarManager manager, IntPtr WParam, IntPtr LParam, Control wnd) {
			RibbonControl ribbon = GetRibbonFromBarManager(manager);
			int lp = IntPtr2Value(LParam);
			ribbon.UpdateMiniToolbarsVisibility(Control.MousePosition, wnd);
			ribbon.UpdateInRibbonGalleryCommandButtons(Control.MousePosition);
			if(ribbon.Contains(wnd)) {
				Point ribbonPoint = ribbon.PointToClient(Control.MousePosition);
				if(ribbon.Manager.ActiveEditor != null && (ribbon.Manager.ActiveEditor.Bounds.Contains(ribbonPoint) || ribbon.Manager.ActiveEditor.Capture))
					return;
				MouseInfoArgs e = CreateMouseArgs(MSG.WM_MOUSEMOVE, WParam, ribbonPoint);
				ribbon.OnMouseMoveCore(e);
			}
		}
		void UpdateRibbonObjectsOnNcLButtonDown(BarManager manager, IntPtr WParam, IntPtr LParam, Control wnd) {
			RibbonControl ribbon = GetRibbonFromBarManager(manager);
			if(ribbon == null) return;
			Point pt = ribbon.PointToClient(Control.MousePosition);
			if(ShouldProcessNcLButtonUpDownMsgs(ribbon, pt)) {
				ribbon.OnMouseDownCore(CreateMouseArgs(MSG.WM_LBUTTONDOWN, WParam, pt));
			}
		}
		void UpdateRibbonObjectsOnNcLButtonUp(BarManager manager, IntPtr WParam, IntPtr LParam, Control wnd) {
			RibbonControl ribbon = GetRibbonFromBarManager(manager);
			if(ribbon == null) return;
			Point pt = ribbon.PointToClient(Control.MousePosition);
			if(ShouldProcessNcLButtonUpDownMsgs(ribbon, pt)) {
				ribbon.OnMouseUpCore(CreateMouseArgs(MSG.WM_LBUTTONUP, WParam, ribbon.PointToClient(Control.MousePosition)));
			}
		}
		bool ShouldProcessNcLButtonUpDownMsgs(RibbonControl ribbon, Point pt) {
			RibbonForm form = ribbon.RibbonForm;
			if(form == null || form != Form.ActiveForm) return false;
			RibbonHitInfo hitInfo = ribbon.CalcHitInfo(pt);
			return hitInfo.InPageCategory && !hitInfo.InPage;
		}
		RibbonControl GetRibbonFromBarManager(BarManager manager) {
			RibbonBarManager rmanager = manager as RibbonBarManager;
			return rmanager == null ? null : rmanager.Ribbon;
		}
		bool IsMdiChildMaximized(Form childForm) {
			PropertyInfo pi = typeof(Form).GetProperty("IsMaximized", BindingFlags.NonPublic | BindingFlags.Instance);
			return pi != null && ((bool)pi.GetValue(childForm, null)) == true;
		}
		bool IsMdiChildClosing(Form childForm) {
			PropertyInfo pi = typeof(Form).GetProperty("IsClosing", BindingFlags.NonPublic | BindingFlags.Instance);
			return pi != null && ((bool)pi.GetValue(childForm, null)) == true;
		}
		private void OnWmMdiActivate(BarManager manager, Control control) {
			MdiClient mdiClient;
			Form childForm = control as Form;
			if(childForm == null || !IsMdiChildMaximized(childForm))
				return;
			if(!childForm.Visible || IsMdiChildClosing(childForm))
				return;
			if(manager == null)
				return;
			mdiClient = GetMdiClient(manager.GetForm());
			if(mdiClient != null)
				UpdateMdiClient(mdiClient);
		}
		private void OnWmMdiSetMenu(BarManager manager, Control control, IntPtr WParam) {
			MdiClient mdiClient = control as MdiClient;
			if(mdiClient != null && mdiClient.Parent == (manager.FilterForm as Control)) {
				if(manager.MainMenu != null && WParam != IntPtr.Zero) {
					BarNativeMethods.DestroyMenu(WParam);
					if(HasVisibleMdiChild(mdiClient))
						UpdateMdiClient(mdiClient);
				}
			}
		}
		private void OnWmNcActivate(BarManager manager, Control control, IntPtr WParam) {
			if(control != null) {
				if(manager.GetTopMostControl() != null && (control == manager.GetTopMostControl() || manager.GetTopMostControl().Contains(control))) {
					if(manager.IsDesignMode && manager.Helper.CustomizationManager.CustomizationForm == null) {
						if(manager.Helper.CustomizationManager.DesignTimeManager != null && manager.Helper.CustomizationManager.DesignTimeManager.DesignManager.SelectionInfo.OpenedPopups.Count > 0)
							return;
						if(WParam == IntPtr.Zero) {
							if(CanHideBars(manager))
								manager.Deactivate();
						}
						else
							manager.Activate();
					}
					else {
						if(manager.FilterForm.Visible && manager.GetForm() == Form.ActiveForm && GetFormState(manager.GetForm()) != FormWindowState.Minimized)
							manager.Activate();
					}
				}
			}
		}
		private bool? OnWmMouseWheel(BarManager manager, Control control, IntPtr WParam, IntPtr LParam) {
			int wp = IntPtr2Value(WParam);
			int lp = IntPtr2Value(LParam);
			if(manager != null && manager.IsDesignMode && IsPropertyGridActive(control)) {
				return false;
			}
			RibbonBarManager rmanager = manager as RibbonBarManager;
			RibbonControl ribbon = rmanager != null? rmanager.Ribbon : null;
			if(ribbon != null && Ribbon.FindForm() == Form.ActiveForm) {
				if(ribbon.Handler.ProcessMouseWheel(new Point(lp), (int)((short)(wp >> 16)))) {
					return true;
				}
			}
			if(manager.SelectionInfo.OpenedPopups.Count == 0) return null;
			if(manager.ActiveEditor != null) return false;
			manager.SelectionInfo.OpenedPopups.LastPopup.OnMouseWheel(CreateMouseArgs(WM_MOUSEWHEEL, new IntPtr(wp)));
			IBarObject ib = GetBarObject(manager, control);
			return ib == null;
		}
		private void OnWmEnable(BarManager manager, Control control, IntPtr WParam) {
			if(control != null) {
				if(control == manager.FilterForm || control.Contains(manager.FilterForm)) {
					IBarObject barObject = manager.SelectionInfo.ActiveBarControl as IBarObject;
					if(barObject == null || barObject.ShouldCloseOnLostFocus(null)) {
						manager.SelectionInfo.ModalDialogActive = (WParam.ToInt32() == 0);
					}
				}
			}
		}
		private void OnWmActivate(BarManager manager, Control control, IntPtr WParam) {
			if(control != null) {
				if(manager.GetTopMostControl() != null && (control == manager.GetTopMostControl() || 
					manager.GetTopMostControl().Contains(control) || manager.IsMdiParentControl(control, manager.GetTopMostControl()))) {
					if((WParam.ToInt32() & 0xffff) != WA_INACTIVE)
						manager.Activate();
				}
			}
		}
		private void OnWmActivateApp(BarManager manager, IntPtr WParam) {
			manager.RadialMenuAppActiveStatusChanging(WParam.ToInt32() != 0);
			if(manager.Helper.CustomizationManager.DesignTimeManager != null) {
				manager.Helper.CustomizationManager.DesignTimeManager.CloseModalTextBox();
			}
			RibbonBarManager rmanager = manager as RibbonBarManager;
			if(manager.SelectionInfo.internalFocusLock != 0) return;
			if(WParam.ToInt32() == 0) {
				if(manager.FilterForm.IsHandleCreated && rmanager == null && IsFloatingBars)
					manager.FilterForm.BeginInvoke(new MethodInvoker(manager.Deactivate));
				else
					manager.Deactivate();
			}
			else {
				if((manager.GetTopMostControl() != null && manager.GetForm() == null) ||
					(manager.GetForm() != null && GetFormState(manager.GetForm()) != FormWindowState.Minimized)) {
					manager.Activate();
				}
			}
		}
		private void OnWmWindowPosChanged(BarManager manager, Control control, IntPtr LParam) {
			if(control != null && manager.FilterForm == control && LParam != IntPtr.Zero) {
				WINDOWPOS pos = (WINDOWPOS)BarNativeMethods.PtrToStructure(LParam, typeof(WINDOWPOS));
				if(pos.hwndInsertAfter == IntPtr.Zero && (pos.flags & 4) == 0) 
					manager.SelectionInfo.UpdateZOrder();
			}
		}
		private void OnWmSizeMove(BarManager manager, Control control, int Msg, IntPtr WParam) {
			RibbonBarManager rmanager = manager as RibbonBarManager;
			RibbonControl ribbon = rmanager == null ? null : rmanager.Ribbon;
			const int WindowMaximized = 2;
			if(control != null && !manager.LockMenu) {
				if(manager.FilterForm == control || manager.GetForm() == control)
					manager.HideMenus();
			}
			if(Msg == WM_SIZE && WParam.ToInt32() == WindowMaximized) {
				RibbonForm ribbonForm = ribbon == null ? null : ribbon.ViewInfo.Form;
				Form frm = control as Form;
				if(frm != null && frm.MdiParent != null && ribbonForm == frm.MdiParent) {
					ribbonForm.DestroyMenu();
				}
			}
		}
		private void OnWmSetFocus(BarManager manager, Control control) {
			if(manager.SelectionInfo.internalFocusLock != 0) return;
			if(control == null ||
				(!manager.IsCustomizing && !CheckControl(manager, control) && !ActiveEditorIsFocused(manager))) {
			}
		}
		private bool? OnWmContextMenu(BarManager manager, Control control) {
			if(control != null) {
				if(CheckShowPopupMenu(manager, control, false)) {
					((IHookControllerWithResult)this).Result = HookResult.ProcessedExit;
					return true;
				}
			}
			return null;
		}
		protected virtual bool? OnWmRButtonUp(BarManager manager, Control control) {
			if(control != null && !(control is TreeView) && !(control is ListView)) { 
				if(CheckShowPopupMenu(manager, control, false)) {
					((IHookControllerWithResult)this).Result = HookResult.ProcessedExit;
					return true;
				}
			}
			return null;
		}
		private void OnWmKillFocus(BarManager manager, Control control, IntPtr WParam) {
			if(manager.SelectionInfo.internalFocusLock != 0) return;
			Control newFocus = Control.FromHandle(WParam);
			if(!ActiveEditorIsFocused(manager) && CheckControl(manager, control) && (newFocus == null || !CheckControl(manager, newFocus))) {
			}
		}
		protected bool OnWmMenuChar(BarManager manager, IntPtr WParam) {
			if(!RibbonControl.AllowSystemShortcuts) return false;
			RibbonBarManager rmanager = manager as RibbonBarManager;
			RibbonControl ribbon = rmanager != null ? rmanager.Ribbon : null;
			if(ribbon == null || !ribbon.IsShowKeyTip) return false;
			rmanager.Ribbon.Handler.OnKeyPress(new KeyPressEventArgs((char)(WParam.ToInt32() & 0xffff)));
			if(ribbon.IsShowKeyTip) skipKeyboardDeactivation = true;
			return false;
		}
		private void OnWmMenuSelect(BarManager manager, IntPtr WParam, IntPtr LParam) {
			RibbonBarManager rmanager = manager as RibbonBarManager;
			RibbonControl ribbon = rmanager != null ? rmanager.Ribbon : null;
			if(ribbon == null || !RibbonControl.AllowSystemShortcuts) return;
			if(shouldDisableSystemMenu) {
				DisableSystemMenu();
				if(!skipKeyboardDeactivation && IsMenuClosed(WParam, LParam)) {
					ribbon.DeactivateKeyboardNavigation();
					manager.SelectionInfo.Clear();
				}
				skipKeyboardDeactivation = false;
			}
		}
		private void OnWmExitMenuLoop() {
			if(!RibbonControl.AllowSystemShortcuts) return;
			shouldDisableSystemMenu = false;
			EnableSystemMenu();
		}		
		internal bool CheckInternalPreFilterMessageListeners(BarManager manager, int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam) {
			if(Manager == null)
				return false;
			var listeners = Manager.Listeners;
			for(int i = 0; i < listeners.Count; i++) {
				if(listeners[i].InternalPreFilterMessage(manager, Msg, wnd, HWnd, WParam, LParam) == BarManagerHookResult.ProcessedExit)
					return true;
			}
			return false;
		}
		internal bool CheckPreFilterMessageListeners(BarManager manager, int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam) {
			if(Manager == null)
				return false;
			var listeners = Manager.Listeners;
			for(int i = 0; i < listeners.Count; i++) {
				if(listeners[i].PreFilterMessage(manager, Msg, wnd, HWnd, WParam, LParam) == BarManagerHookResult.ProcessedExit)
					return true;
			}
			return false;
		}
		internal bool InternalPreFilterMessage(BarManager manager, int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr LParam) {
			Control control = wnd;
			RibbonBarManager rmanager = Manager as RibbonBarManager;
			RibbonControl ribbon = rmanager == null ? null : rmanager.Ribbon;
			if(CheckInternalPreFilterMessageListeners(manager, Msg, wnd, HWnd, WParam, LParam))
				return true;
			if((Msg == MSG.WM_MOUSEMOVE || Msg == MSG.WM_MOUSELEAVE)) { 
				if(ribbon != null && ribbon.Manager != null && ribbon.Manager.IsBarsActive)
					UpdateRibbonObjectsOnMouseMove(manager, WParam, LParam, wnd);
				manager.UpdateRadialMenuHoverInfo(Control.MousePosition);
			}
			if(Msg == MSG.WM_NCLBUTTONDOWN) {
				if(ribbon != null && ribbon.IsHandleCreated)
					UpdateRibbonObjectsOnNcLButtonDown(manager, WParam, LParam, wnd);
			}
			if(Msg == MSG.WM_LBUTTONUP) {
				if(ribbon != null && ribbon.IsHandleCreated)
					UpdateRibbonObjectsOnNcLButtonUp(manager, WParam, LParam, wnd);
			}
			if((Msg >= WM_MINCLICK && Msg <= WM_MAXCLICK) || (Msg >= WM_MINNCCLICK && Msg <= WM_MAXNCCLICK)) {
				manager.SelectionInfo.SetCurrentShortcut(BarShortcut.Empty);
				manager.SelectionInfo.ProcessRadialMenuOuterClick(manager, control, CreateMouseArgs(Msg, WParam));
				if(Msg == WM_RBUTTONUP && manager.IgnoreMouseUp > 0) {
					manager.IgnoreMouseUp--;
					return false;
				}
				if(Msg == WM_LBUTTONUP && manager.IgnoreLeftMouseUp > 0) {
					manager.IgnoreLeftMouseUp--;
					return false;
				}
				if(manager.SelectionInfo.internalFocusLock == 0 && !manager.IsDragging) {
					if(!CheckEditorClick(manager, control, Msg)) { 
						MouseInfoArgs e = CreateMouseArgs(Msg, WParam);
						BarMenuCloseType closeType = ShouldCloseOnClick(manager, control, e);
						if(closeType != BarMenuCloseType.None) {
							manager.SelectionInfo.OnCloseAll(closeType);
						}
						else {
							ClosePopupContainerChildPopups(manager, control, e);
						}
					}
				}
				if(manager.IsCustomizing && manager.SelectionInfo.ShouldInterceptMouse(control)) {
					Form csForm = manager.Helper.CustomizationManager.CustomizationForm;
					if((Form.ActiveForm != csForm && Form.ActiveForm != manager.GetForm()) && csForm.Visible) {
						csForm.Activate();
					}
					return true;
				}
			}
			switch(Msg) {
				case WM_PAINT:
				case WM_ERASEBKGND:
					return false;
			}
			if(control is DevExpress.Utils.Win.Shadow) return false;
			switch(Msg) {
				case WM_LBUTTONDOWN: 
				case WM_RBUTTONDOWN:
				case WM_MBUTTONDOWN:
					OnWmButtonDown(control);
					break;
				case WM_MDIACTIVATE:
					OnWmMdiActivate(manager, control);
					break;
				case WM_MDISETMENU:
					OnWmMdiSetMenu(manager, control, WParam);
					break;
				case WM_MOUSEWHEEL:
					bool? res = OnWmMouseWheel(manager, control, WParam, LParam);
					if(res.HasValue) 
						return res.Value;
					break;
				case WM_NCACTIVATE: 
					OnWmNcActivate(manager, control, WParam);
					break;
				case WM_ENABLE:
					OnWmEnable(manager, control, WParam);
					break;
				case WM_ACTIVATE:
					OnWmActivate(manager, control, WParam);
					break;
				case WM_ACTIVATEAPP:
					OnWmActivateApp(manager, WParam);
					break;
				case WM_WINDOWPOSCHANGED:
					OnWmWindowPosChanged(manager, control, LParam);
					break;
				case WM_SIZE:
				case WM_MOVE:
					OnWmSizeMove(manager, control, Msg, WParam);
					break;
				case WM_SETFOCUS:
					OnWmSetFocus(manager, control);
					break;
				case WM_CONTEXTMENU:
					bool? resCm = OnWmContextMenu(manager, control);
					if(resCm.HasValue)
						return resCm.Value;
					break;
				case WM_RBUTTONUP:
					bool? resRb = OnWmRButtonUp(manager, control);
					if(resRb.HasValue)
						return resRb.Value;
					break;
				case WM_KILLFOCUS:
					OnWmKillFocus(manager, control, WParam);
					break;
				case WM_MENUCHAR:
					return OnWmMenuChar(manager, WParam);
				case WM_MENUSELECT:
					OnWmMenuSelect(manager, WParam, LParam);
					break;
				case WM_EXITMENULOOP:
					OnWmExitMenuLoop();
					break;
			}
			return false;
		}
		private void ClosePopupContainerChildPopups(BarManager manager, Control control, MouseInfoArgs e) {
			IBarObject barObject = GetBarObject(manager, control);
			IPopup popup = barObject as IPopup;
			if(popup == null)
				return;
			if(!(popup.PopupCreator is PopupControlContainer))
				return;
			foreach(IPopup child in manager.SelectionInfo.OpenedPopups) {
				if(popup.CanOpenAsChild(child)) {
					manager.SelectionInfo.ClosePopup(child);
					break;
				}
			}
		}
		protected virtual int IntPtr2Value(IntPtr ptr) {
			int val = 0;
			if(DevExpress.Utils.OSVersionHelper.Is64BitOS()) {
				if(ptr.ToInt64() > Int32.MaxValue) val = 0;
				else val = ptr.ToInt32();
			}
			else {
				val = ptr.ToInt32();
			}
			return val;
		}
		protected virtual bool IsFloatingBars {
			get {
				foreach(Bar bar in manager.Bars)
					if(bar.DockStyle == BarDockStyle.None) return true;
				return false;
			}
		}
		protected virtual bool ShouldUpdateMenu { 
			get {
				Form frm = manager.GetForm();
				return manager != null && frm != null && frm.IsHandleCreated && !manager.IsDesignMode; 
			} 
		}
		void EnableSystemMenu() {
			if(!ShouldUpdateMenu) return;
			BarNativeMethods.GetSystemMenu(manager.GetForm().Handle, true);
		}
		void DisableSystemMenu() {
			if(!ShouldUpdateMenu) return;
			IntPtr sysMenu = BarNativeMethods.GetSystemMenu(manager.GetForm().Handle, false);
			int itemsCount = BarNativeMethods.GetMenuItemCount(sysMenu);
			for(int i = 0; i < itemsCount; i++) {
				bool res = BarNativeMethods.DeleteMenu(sysMenu, itemsCount - 1 - i, 0x00000400);
			}
		}
		bool IsMenuClosed(IntPtr WParam, IntPtr LParam) {
			return (IntPtr2Value(WParam) & 0xffff0000) == 0xffff0000 && IntPtr2Value(LParam) == 0;
		}
		bool skipKeyboardDeactivation = false;
		[StructLayout(LayoutKind.Sequential)]
		struct WINDOWPOS {
			public IntPtr hwnd;
			public IntPtr hwndInsertAfter;
			public int x;
			public int y;
			public int cx;
			public int cy;
			public int flags;
		}
		protected bool ActiveEditorIsFocused(BarManager manager) {
			if(manager.SelectionInfo.ActiveEditor == null) return false;
			return manager.SelectionInfo.ActiveEditor.EditorContainsFocus;
		}
		protected bool CanAcceptKeys(BarManager manager) {
			RibbonBarManager rmanager = manager as RibbonBarManager;
			RibbonControl ribbon = rmanager == null? null: rmanager.Ribbon;
			if(!manager.Active) return false;
			if(ribbon != null && ribbon.IsShowKeyTip) return true;
			if(manager.FilterForm == null) return false;
			if(!manager.ProcessShortcutsWhenInvisible && !manager.FilterForm.Visible) return false;
			if(manager.FilterForm.Focused) return true;
			if(manager.SelectionInfo.ActiveBarControl is AppMenuBarControl) return true;
			if(manager.SelectionInfo.ActiveEditor != null && manager.SelectionInfo.ActiveEditor.EditorContainsFocus) return true;
			Form activeForm = CachedForm.ActiveForm, frm = null;
			frm = manager.GetForm();
			if(frm != null) {
				if(!frm.IsHandleCreated || !BarNativeMethods.IsWindowEnabled(frm.Handle) || Manager.SelectionInfo.ModalDialogActive) return false;
			}
			if(frm != null && frm.IsMdiChild && frm.MdiParent != null) { 
				if(activeForm != null && !activeForm.IsMdiChild) {
					if(activeForm.IsMdiContainer && activeForm.ActiveMdiChild == frm) return true;
					if(BarManager.FindManager(activeForm) != null) return false; 
				}
				return frm == frm.MdiParent.ActiveMdiChild && frm.MdiParent == activeForm;
			}
			if(activeForm == frm) return true;
			DevExpress.XtraBars.Docking.FloatForm floatForm = CachedForm.ActiveForm as DevExpress.XtraBars.Docking.FloatForm;
			if(floatForm != null && floatForm.DockManager != null && manager.DockManager == floatForm.DockManager) return true;
			DevExpress.XtraBars.Docking2010.FloatDocumentForm docForm = CachedForm.ActiveForm as DevExpress.XtraBars.Docking2010.FloatDocumentForm;
			if(docForm != null && docForm.Manager != null && docForm.Manager.GetBarManager() == manager) return true;
			IBarObject barObject = Form.ActiveForm as IBarObject;
			if(barObject != null && barObject.Manager == manager) return true;
			return false;
		}
		BarManagerHookResult GetResult(bool processed) { return processed ? BarManagerHookResult.Processed : BarManagerHookResult.NotProcessed; }
		bool shouldDisableSystemMenu = false;
		public BarManagerHookResult PreFilterMessage(ref Message m ) {
			Control control = Control.FromHandle(m.HWnd);
			if(CheckPreFilterMessageListeners(manager, m.Msg, null, m.HWnd, m.WParam, m.LParam))
				return BarManagerHookResult.NotProcessed;
			if(m.Msg == WM_CONTEXTMENU) {
				PopupMenuBase menu = Manager.GetPopupContextMenu(control);
				if(menu != null) return BarManagerHookResult.Processed;
			}
			if(m.Msg == WM_SYSCHAR) {
				RibbonBarManager rmanager = Manager as RibbonBarManager;
				if(rmanager != null && rmanager.Ribbon != null && rmanager.Ribbon.IsShowKeyTip && rmanager.Ribbon.ActiveKeyTipManager != null) {
					rmanager.Ribbon.ActiveKeyTipManager.AddChar((char)m.WParam.ToInt32());
					return BarManagerHookResult.ProcessedExit;
				}
			}
			if(m.Msg < 0x100 || m.Msg > 0x105) return BarManagerHookResult.NotProcessed;
			const int WM_KEYDOWN = 0x0100, WM_KEYUP = 0x101, WM_SYSKEYDOWN = 0x0104, WM_SYSKEYUP = 0x0105, WM_CHAR = 0x0102;
			BarManagerHookResult result = BarManagerHookResult.NotProcessed;
			Keys keyData = ((Keys)m.WParam.ToInt32());
			Keys modifiers = Control.ModifierKeys;
			if(keyData == Keys.Menu) modifiers &=(~Keys.Alt);
			KeyEventArgs e = new KeyEventArgs(keyData | modifiers);
			if(m.Msg == WM_KEYDOWN || m.Msg == WM_SYSKEYDOWN || m.Msg == WM_SYSKEYUP) {
				if(m.Msg == WM_SYSKEYDOWN && keyData == Keys.Menu) {
					shouldDisableSystemMenu = true;
				}
				if(CanAcceptKeys(Manager) && Manager.IsDocking) {
					if((Keys)m.WParam.ToInt32() == Keys.Escape) {
						Manager.SelectionInfo.DockManager.StopMoving(true);
						Manager.SelectionInfo.DockManager = null;
					}
					return BarManagerHookResult.Processed;
				}
			}
			if(m.Msg == WM_SYSKEYDOWN || m.Msg == WM_SYSKEYUP) {
				if(!CanAcceptKeys(Manager)) return result;
				if(CheckAltKey(Manager, m.Msg == WM_SYSKEYUP, m.WParam.ToInt32())) return BarManagerHookResult.Processed;
			}
			if(m.Msg == WM_KEYUP || m.Msg == WM_SYSKEYUP) {
				if(CanAcceptKeys(Manager) && Manager.SelectionInfo.ActiveBarControl != null) {
					result = GetResult(Manager.SelectionInfo.ActiveBarControl.IsInterceptKey(e));
					Manager.SelectionInfo.ProcessKeyUp(e);
					return result;
				}
			}
			if(m.Msg == WM_KEYDOWN || m.Msg == WM_SYSKEYDOWN) {
				if(keyData == Keys.Packet)
					return BarManagerHookResult.NotProcessed;
				if(!CanAcceptKeys(Manager)) {
					return result;
				}
				if(manager.ProcessCommandKey((Keys)m.WParam.ToInt32())) return BarManagerHookResult.Processed;
				if(m.Msg == WM_SYSKEYDOWN && (Control.ModifierKeys & Keys.Alt) != 0 && Manager.SelectionInfo.ActiveBarControl == null) {
					result = Manager.ProcessLinkAccelerator((Keys)m.WParam.ToInt32())? BarManagerHookResult.ProcessedExit: BarManagerHookResult.NotProcessed;
				}
				if(result == BarManagerHookResult.NotProcessed) {
					if(Manager.SelectionInfo.ActiveBarControl == null) {
						result = Manager.SelectionInfo.CheckShortcut(Manager, e);
						if(result == BarManagerHookResult.ProcessedExit) return result;
					}
					else {
						if(Manager.PrimaryShortcutProcessor == PrimaryShortcutProcessor.Editor) {
							if(Manager.ActiveEditor != null && Manager.ActiveEditor.IsNeededKey(e))
								return BarManagerHookResult.NotProcessed;	
							result = GetResult(Manager.SelectionInfo.ActiveBarControl.IsInterceptKey(e));
						}
						if(result == BarManagerHookResult.NotProcessed) {
							result = Manager.SelectionInfo.CheckShortcut(Manager, e);
							if(result != BarManagerHookResult.NotProcessed)
								return result;
						}
						Manager.SelectionInfo.SetCurrentShortcut(BarShortcut.Empty);
						Manager.SelectionInfo.ProcessKeyDown(e);
						return result;
					}
				}
				if(result == BarManagerHookResult.NotProcessed && Manager.IsCustomizing) {
					result = GetResult(Manager.SelectionInfo.ShouldInterceptKey(e));
				}
			}
			if(m.Msg == WM_CHAR) {
				if(CanAcceptKeys(Manager)) {
					if(Manager.SelectionInfo.ActiveBarControl != null) {
						return GetResult(Manager.SelectionInfo.ActiveBarControl.ProcessKeyPress(new KeyPressEventArgs((char)m.WParam.ToInt32())));
					}
				}
			}
			if(result == BarManagerHookResult.NotProcessed && !Manager.IsCustomizing) {
				if((m.Msg == WM_SYSKEYDOWN && e.KeyData == (Keys.Shift | Keys.F10)) 
					|| ((m.Msg == WM_SYSKEYUP || m.Msg == WM_KEYUP) && e.KeyData == Keys.Apps)) {
					result = GetResult(CheckShowPopupMenu(Manager, control, true));
					if(result != BarManagerHookResult.NotProcessed) {
						m.Result = IntPtr.Zero;
						return BarManagerHookResult.Processed;
					}
				}
			}
			return result;
		}
		protected virtual PopupMenuBase FindPopupMenu(BarManager manager, Control control) {
			PopupMenuBase menu = null;
			while(control != null) {
				menu = manager.GetPopupContextMenu(control);
				if(menu != null) 
					break;
				if(control.ContextMenu != null || control.ContextMenuStrip != null)
					return null;
				control = control.Parent;
			}
			return menu;
		}
		bool IsCursorInFormClientArea(Form frm, bool fromKeyboard) {
			if(fromKeyboard) return true;
			Point pt = Cursor.Position;
			pt = frm.PointToClient(pt);
			return frm.ClientRectangle.Contains(pt);
		}
		protected internal virtual bool CheckShowPopupMenu(BarManager manager, Control control, bool fromKeyboard) {
			if(manager.IsDesignMode || manager.IsCustomizing) return false;
			Form frm = control as Form;
			if(frm != null && !IsCursorInFormClientArea(frm, fromKeyboard))
				return false;
			if(Manager.ToolBarsPopup != null && Manager.ToolBarsPopup.Visible)
				return false;
			PopupMenuBase menu = FindPopupMenu(manager, control);
			if(menu != null) {
				BarNativeMethods.SetCapture(IntPtr.Zero);
				if(menu.CanShowPopup) {
					Point pos = Control.MousePosition;
					if(fromKeyboard) {
						NativeMethods.POINT pnt = new NativeMethods.POINT(-9999, 0);
						if(control.Focused && BarNativeMethods.GetCaretPos(ref pnt))
							pos = control.PointToScreen(new Point(pnt.X, pnt.Y));
						else {
							pos = control.PointToScreen(new Point(0, control.ClientRectangle.Bottom));
						}
					}
					QueryShowPopupMenuEventArgs e = new QueryShowPopupMenuEventArgs(menu, control, pos);
					manager.RaiseQueryShowPopupMenu(e);
					if(!e.Cancel) {
						menu.ShowPopup(e.Position, control);
						if(!fromKeyboard) manager.IgnoreMouseUp = 0;
					}
				}
				return true;
			}
			return false;
		}
		protected virtual bool CheckAltKey(BarManager manager, bool keyUp, int keyCode) {
			Keys key = (Keys)keyCode;
			if((key == Keys.Menu) || (manager.UseF10KeyForMenu && key == Keys.F10 && Control.ModifierKeys == Keys.None))
				return manager.SelectionInfo.AltKeyPressed(keyUp, key == Keys.Menu);
			return false;
		}
		static int GetWindowState(IntPtr hWnd) {
			BarNativeMethods.WINDOWPLACEMENT wp = new BarNativeMethods.WINDOWPLACEMENT();
			wp.length = BarNativeMethods.SizeOf(typeof(BarNativeMethods.WINDOWPLACEMENT));
			BarNativeMethods.GetWindowPlacement(hWnd, ref wp);
			return wp.showCmd;
		}
		const int SW_SHOWMINIMIZED = 2, SW_SHOWMAXIMIZED = 3, SW_MINIMIZE = 6, SW_SHOWMINNOACTIVE = 7;
		internal static FormWindowState GetFormState(Form form) {
			if(form == null) return FormWindowState.Normal;
			if(!form.IsHandleCreated) return form.WindowState;
			switch(GetWindowState(form.Handle)) {
				case SW_SHOWMAXIMIZED:
					return FormWindowState.Maximized;
				case SW_SHOWMINIMIZED:
				case SW_MINIMIZE:
				case SW_SHOWMINNOACTIVE:
					return FormWindowState.Minimized;
			}
			return FormWindowState.Normal;
		}
	}
	public class BarManagerMessageFilter : IMessageFilter {
		ArrayList Managers;
		Hashtable IFilterInfo;
		bool hookInstalled = false;
		ManagersComparer comparer;
		public BarManagerMessageFilter() {
			Managers = new ArrayList();
			IFilterInfo = new Hashtable();
			comparer = new ManagersComparer();
		}
		protected internal virtual BarManagerHook FindHook(BarManager manager) {
			lock(Managers.SyncRoot) {
				foreach(BarManagerHook hook in Managers) {
					if(hook.Manager == manager) return hook;
				}
			}
			return null;
		}
		public virtual void CheckHook(BarManager manager) {
			lock(Managers.SyncRoot) {
				for(int n = 0; n < Managers.Count; n++) {
					BarManagerHook hook = Managers[n] as BarManagerHook;
					if(hook.Manager == manager) hook.CheckController();
				}
			}
		}
		public virtual void AddHook(BarManager manager) {
			RemoveHook(manager);
			lock(Managers.SyncRoot) {
				Managers.Add(new BarManagerHook(manager));
			}
			int thId = HookManager.GetCurrentThreadId();
			int count = 1;
			if(!IFilterInfo.Contains(thId)) {
				Application.AddMessageFilter(this);
			} else {
				count = ((int)IFilterInfo[thId]) + 1;
			}
			IFilterInfo[thId] = count;
		}
		public virtual void RemoveHook(BarManager manager) {
			BarManagerHook hook = FindHook(manager);
			if(hook != null) {
				lock(Managers.SyncRoot) {
					Managers.Remove(hook);
				}
				hook.Dispose();
			}
			if(hook != null) {
				int thId = HookManager.GetCurrentThreadId();
				if(IFilterInfo.Contains(thId)) {
					int count = (int)IFilterInfo[thId];
					count --;
					if(count == 0) {
						Application.RemoveMessageFilter(this);
						IFilterInfo.Remove(thId);
					} else {
						IFilterInfo[thId] = count;
					}
				}
			}
		}
		protected internal void InstallHook() {
			hookInstalled = true;
			Application.ApplicationExit += new EventHandler(OnApplicationExit);
		}
		protected internal void RemoveHook(bool disposing) {
			if(hookInstalled) {
				hookInstalled = false;
				Application.ApplicationExit -= new EventHandler(OnApplicationExit);
				if(disposing) Application.RemoveMessageFilter(this);
			}
		}
		private void OnApplicationExit(object sender, EventArgs e) {
			RemoveHook(true);
		}
		class ManagersComparer : IComparer {
			bool compareFocus;
			public void SetCondition(bool requireCompareFocus) {
				this.compareFocus = requireCompareFocus;
			}
			public ArrayList Filter(int threadId, ArrayList mlist) {
				for(int n = mlist.Count - 1; n >= 0; n--) {
					if(((BarManagerHook)mlist[n]).Manager.ThreadId != threadId) return FilterCore(threadId, mlist);
				}
				return mlist;
			}
			ArrayList FilterCore(int threadId, ArrayList mlist) {
				ArrayList list = new ArrayList();
				for(int n = 0; n < mlist.Count; n++) {
					BarManagerHook m = (BarManagerHook)mlist[n];
					if(m.Manager.ThreadId == threadId) list.Add(m);
				}
				return list;
			}
			int NestedLevel(Control container, Control active) {
				int res = 0;
				if(container == null || active == null) return 9999;
				if(!container.Visible) return 10000;
				while(active != null) {
					if(active == container) return res;
					active = active.Parent;
					res++;
				}
				return 9999;
			}
			Control GetActiveControl(IContainerControl container) {
				if(container == null || container.ActiveControl == null) return container as Control;
				int l = 0;
				Control activeControl = container.ActiveControl;
				while(l < 15) {
					Control containerControl = container as Control;
					if(containerControl != null && containerControl.ContainsFocus) {
						if(container.ActiveControl == null || !(container.ActiveControl is IContainerControl)) {
							if(activeControl != null) break;
							return containerControl;
						}
						activeControl = container.ActiveControl;
						container = activeControl as IContainerControl;
						l++;
						continue;
					}
					break;
				}
				return activeControl;
			}
			int IComparer.Compare(object x, object y) {
				BarManagerHook h1 = (BarManagerHook)x;
				BarManagerHook h2 = (BarManagerHook)y;
				BarManager m1 = h1.Manager, m2 = h2.Manager;
				if(m1 == m2) return 0;
				if(m1.FilterForm == m2.FilterForm) {
					if(m1.FilterForm == null) return 0;
				}
				if(m1.FilterForm == null) return 1;
				if(m2.FilterForm == null) return -1;
				if(compareFocus && m1.OriginalForm == m2.OriginalForm && m1.OriginalForm != null && m1.FilterForm != m2.FilterForm) {
					Control ac = GetActiveControl(m1.OriginalForm);
					if(ac != null) {
						int i1 = NestedLevel(m1.FilterForm, ac), i2 = NestedLevel(m2.FilterForm, ac);
						if(i1 != i2) return i1.CompareTo(i2);
					}
				}
				Form f1 = m1.FilterForm as Form, f2 = m2.FilterForm as Form;
				if(m1.IsMdiContainerManager != m2.IsMdiContainerManager) {
					if(m1.IsMdiContainerManager) {
						if(f1 != null && f1.ActiveMdiChild != null) {
							return 1;
						}
						return -1;
					}
					if(m2.IsMdiContainerManager) {
						if(f2 != null && f2.ActiveMdiChild != null) {
							return -1;
						}
						return 1;
					}
				}
				return m1.GetHashCode().CompareTo(m2.GetHashCode());
			}
		}
		ArrayList GetSortedManagers(ref Message m, int threadId) {
			bool requireCompareFocus = IsKeyboardMessage(ref m);
			ArrayList managers = Managers;
			if(Managers.Count > 1) {
				lock(Managers.SyncRoot) {
					managers = comparer.Filter(threadId, Managers);
					comparer.SetCondition(requireCompareFocus);
					managers.Sort(comparer);
				}
			}
			return managers;
		}
		bool IMessageFilter.PreFilterMessage(ref Message m) {
			bool result = false;
			try {
				if(!ShouldProcessMessage(m))
					return false;
				CachedForm.ActiveForm = Form.ActiveForm;
				int threadId = HookManager.GetCurrentThreadId();
				ArrayList managers = GetSortedManagers(ref m, threadId);
				int mCount = managers.Count;
				for(int n = 0; n < mCount; n++) {
					if (n >= managers.Count) break; 
					BarManagerHook hook = managers[n] as BarManagerHook;
					if (hook.Manager == null || hook.Manager.FilterForm == null || hook.Manager.ThreadId != threadId) continue;
					BarManagerHookResult res = hook.PreFilterMessage(ref m);
					if (res == BarManagerHookResult.ProcessedExit) return true;
					result |= (res == BarManagerHookResult.Processed);
				}
			}
			catch(Exception e) {
				if(e is BarHideException) {
					result = true;
				}
				else {
					Application.OnThreadException(e);
				}
			}
			finally {
				CachedForm.ActiveForm = null;
			}
			return result;
		}
		static int[] AllowedMessages = new int[] { 
			MSG.WM_ACTIVATEAPP,
			MSG.WM_APP,
			MSG.WM_APP + 100, 
			MSG.WM_CREATE,
			MSG.WM_NCACTIVATE,
			MSG.WM_SIZE,
			MSG.WM_MOVE,
			MSG.WM_ACTIVATE,
			MSG.WM_KILLFOCUS,
			MSG.WM_SETFOCUS,
			MSG.WM_ENABLE,
			MSG.WM_PAINT,
			MSG.WM_ERASEBKGND,
			MSG.WM_WINDOWPOSCHANGED,
			MSG.WM_WINDOWPOSCHANGING,
			MSG.WM_NCLBUTTONDOWN,
			MSG.WM_NCLBUTTONUP,
			MSG.WM_NCLBUTTONDBLCLK,
			MSG.WM_NCRBUTTONDOWN,
			MSG.WM_NCRBUTTONUP,
			MSG.WM_NCRBUTTONDBLCLK,
			MSG.WM_NCMBUTTONDOWN,
			MSG.WM_NCMBUTTONUP,
			MSG.WM_NCMBUTTONDBLCLK,
			MSG.WM_LBUTTONDOWN,
			MSG.WM_LBUTTONUP,
			MSG.WM_LBUTTONDBLCLK,
			MSG.WM_RBUTTONDOWN,
			MSG.WM_RBUTTONUP,
			MSG.WM_RBUTTONDBLCLK,
			MSG.WM_MBUTTONDOWN,
			MSG.WM_MBUTTONUP,
			MSG.WM_MBUTTONDBLCLK, 
			MSG.WM_MOUSEWHEEL,
			MSG.WM_DESTROY,
			MSG.WM_CHILDACTIVATE,
			MSG.WM_MDICREATE,
			MSG.WM_MDIDESTROY,
			MSG.WM_MDIACTIVATE,
			MSG.WM_MDIRESTORE,
			MSG.WM_MDINEXT,
			MSG.WM_MDIMAXIMIZE,
			MSG.WM_MDISETMENU,
			MSG.WM_CONTEXTMENU,
			MSG.WM_SYSCOLORCHANGE,
			MSG.WM_EXITMENULOOP,
			MSG.WM_MENUCHAR,
			MSG.WM_SYSCHAR,
			MSG.WM_COMMAND,
			MSG.WM_MENUSELECT,
			MSG.WM_KEYDOWN, 
			MSG.WM_KEYUP,
			MSG.WM_SYSKEYDOWN, 
			MSG.WM_SYSKEYUP, 
			MSG.WM_CHAR,
			MSG.WM_MOUSEMOVE,
			MSG.WM_MOUSELEAVE
		};
		protected virtual bool ShouldProcessMessage(Message m) {
			return Array.IndexOf<int>(AllowedMessages, m.Msg) > -1;
		}
		bool IsKeyboardMessage(ref Message m) {
			return m.Msg >= 0x100 && m.Msg < 0x106;
		}
	}
}
