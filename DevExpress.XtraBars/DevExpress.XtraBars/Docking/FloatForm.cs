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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Mdi;
using DevExpress.XtraBars.Docking.Helpers;
using DevExpress.XtraEditors;
namespace DevExpress.XtraBars.Docking {
	[ToolboxItem(false)]
	public class FloatForm : XtraForm, IDockPanelInfo {
		DockLayout floatLayoutCore;
		public FloatForm() {
			this.ShowInTaskbar = false;
			this.StartPosition = FormStartPosition.Manual;
			this.FormBorderStyle = FormBorderStyle.None;
			this.ControlBox = false;
			this.MinimumSize = DockConsts.DefaultFloatFormMinSize;
			this.floatLayoutCore = null;
			SetStyle(ControlStyles.Selectable, false);
		}
		protected internal event EventHandler DisposingFloatForm;
		protected sealed override void Dispose(bool disposing) {
			if(disposing) {
				if(!isDisposingCore) {
					isDisposingCore = true;
					RaiseDisposingForms();
					OnDispose();
				}
			}
			base.Dispose(disposing);
		}
		void RaiseDisposingForms() {
			if(DisposingFloatForm != null)
				DisposingFloatForm(this, EventArgs.Empty);
		}
		string IDockPanelInfo.PanelName {
			get {
				DockPanel panel = FloatLayout.Panel as DockPanel;
				if(panel != null && panel.DockedAsTabbedDocument)
					return panel.Name;
				return Name;
			}
		}
		bool IDockPanelInfo.DockedAsMdiDocument {
			get {
				DockPanel panel = FloatLayout.Panel as DockPanel;
				if(panel != null)
					return panel.DockedAsTabbedDocument && IsMdiChild;
				return false;
			}
		}
		bool isDisposingCore;
		[Browsable(false)]
		public bool IsDisposing {
			get { return isDisposingCore; }
		}
		protected virtual void OnDispose() {
			Docking2010.Ref.Dispose(ref sizingAdornerCore);
			UnSubscribeOwnerEvents();
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			FloatLayout.FloatSize = ClientSize;
			if(this.Visible) FloatLayout.FloatLocation = this.PointToScreen(Point.Empty);
			FloatLayout.LayoutChanged();
			Refresh();
		}
		bool isActiveCore = false;
		protected internal bool IsActive { get { return isActiveCore; } }
		void SetIsActive(bool value) {
			isActiveCore = value;
		}
		protected override void OnActivated(EventArgs e) {
			base.OnActivated(e);
			if(IsDisposing) return;
			SetIsActive(true);
			RefreshNonClient();
			UpdateActiveDockPanel();
		}
		void UpdateActiveDockPanel() {
			if(DockManager.suspendRaiseActivePanelChanged)
				return;
			if(ActiveControl == null) return;
			DockPanel activePanel = null;
			Control parentCtrl = ActiveControl;
			while(parentCtrl != null && parentCtrl != this) {
				ControlContainer controlContainer = parentCtrl as ControlContainer;
				if(controlContainer != null) {
					controlContainer.Panel.ControlContainer_Activate();
					return;
				}
				activePanel = parentCtrl as DockPanel;
				if(activePanel != null) {
					if(activePanel.ActiveControl == null)
						break;
					DockPanel container = activePanel.ActiveControl as DockPanel;
					while(container != null) {
						activePanel = container;
						container = container.ActiveControl as DockPanel;
					}
					break;
				}
				parentCtrl = parentCtrl.Parent;
			}
			if(activePanel != null)
				DockManager.ActivePanel = activePanel;
		}
		protected override void OnDeactivate(EventArgs e) {
			base.OnDeactivate(e);
			if(Disposing) return;
			SetIsActive(false);
			try {
#if DXWhidbey
				if(!DockManager.ValidateFloatFormChildrenOnDeactivate)
					return;
				if(AutoHideMoveHelper.AreThereOpenDropDownControls(this, DockManager))
					return;
				ValidateChildren();
#endif
			}
			finally {
				RefreshNonClient();
			}
		}
		protected override void OnClosing(CancelEventArgs e) {
			base.OnClosing(e);
			if(!e.Cancel) {
				DockPanelCancelEventArgs args = new DockPanelCancelEventArgs(FloatLayout.Panel);
				args.Panel.RaiseClosingPanel(args);
				e.Cancel = args.Cancel;
			}
		}
		protected override void OnClosed(EventArgs e) {
			base.OnClosed(e);
			if(!CanChangeVisibilityState) return;
			UnSubscribeOwnerEvents();
			FloatLayout.Panel.Hide();
			FloatLayout.Panel.RaiseClosedPanel();
		}
		protected internal void CheckMaximizedBounds() {
			Screen currentScreen = Screen.FromPoint(Location);
			Point actualWorkingAreaLocation = Point.Empty;
			actualWorkingAreaLocation.X = Math.Max(currentScreen.WorkingArea.X - currentScreen.Bounds.X, 0);
			actualWorkingAreaLocation.Y = Math.Max(currentScreen.WorkingArea.Y - currentScreen.Bounds.Y, 0);
			MaximizedBounds = new Rectangle(actualWorkingAreaLocation, currentScreen.WorkingArea.Size);
		}
		protected internal void SetFloatBoundsCore(Rectangle bounds) {
			SetBoundsCore(bounds.X, bounds.Y, bounds.Width, bounds.Height, BoundsSpecified.All);
		}
		static PropertyInfo reasonInfo;
		bool CanChangeVisibilityState {
			get {
#if DXWhidbey
				if(DockManager == null || DockManager.OwnerForm == null)
					return true;
				if(reasonInfo == null)
					reasonInfo = typeof(Form).GetProperty("CloseReason", BindingFlags.Instance | BindingFlags.NonPublic);
				if(reasonInfo == null)
					return true;
				CloseReason reason = (CloseReason)reasonInfo.GetValue(DockManager.OwnerForm, null);
				return (reason != CloseReason.UserClosing);
#else
				return true;
#endif
			}
		}
		const int WM_CHANGEUISTATE = 0x127;
		const int WM_UPDATEUISTATE = 0x128;
		const int UIS_CLEAR = 0x020000;
		protected override void WndProc(ref Message msg) {
			switch(msg.Msg) {
				case WM_CHANGEUISTATE:
					if((Docking2010.WinAPIHelper.GetInt(msg.WParam) & UIS_CLEAR) == UIS_CLEAR)
						NativeMethods.PostMessage(Handle, WM_UPDATEUISTATE, msg.WParam, msg.LParam);
					break;
				case MSG.WM_NCLBUTTONDBLCLK:
					CheckMaximizedBounds();
					break;
				case MSG.WM_SYSCOMMAND:
					if((Docking2010.WinAPIHelper.GetInt(msg.WParam) & 0xFFF0) == NativeMethods.SC.SC_MAXIMIZE)
						CheckMaximizedBounds();
					break;
				case MSG.WM_ACTIVATE:
					if(DockManager == null || !DockManager.CanActivateFloatForm)
						return;
					break;
				case MSG.WM_GETMINMAXINFO:
					ProcessGetMinMaxInfo(ref msg);
					break;
				case MSG.WM_WINDOWPOSCHANGING:
					ProcessWindowsPosChanging(ref msg);
					break;
				case MSG.WM_WINDOWPOSCHANGED:
					if(parentOpening)
						return;
					break;
				case MSG.WM_SHOWWINDOW:
					if(msg.LParam == new IntPtr(1) && WindowState == FormWindowState.Maximized)
						parentOpening = true;
					break;
				case MSG.WM_MOUSEACTIVATE:
					if(DockManager != null && IsMdiChild)
						DockManager.ActivePanel = FloatLayout.Panel;
					break;
			}
			base.WndProc(ref msg);
			CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref msg, this);
		}
		void ProcessWindowsPosChanging(ref Message msg) {
			WinAPI.WINDOWPOS pos = (WinAPI.WINDOWPOS)BarNativeMethods.PtrToStructure(msg.LParam, typeof(WinAPI.WINDOWPOS));
			if(parentOpening) {
				pos.x = Bounds.X;
				pos.y = Bounds.Y;
				pos.cx = Bounds.Width;
				pos.cy = Bounds.Height;
				BarNativeMethods.StructureToPtr(pos, msg.LParam, false);
			}
		}
		void ProcessGetMinMaxInfo(ref Message msg) {
			if(msg.LParam == IntPtr.Zero) return;
			if(parentOpening) {
				BeginInvoke(new MethodInvoker(() =>
				{
					WindowState = FormWindowState.Maximized;
					parentOpening = false;
				}));
			}
		}
		bool parentOpening;
		protected virtual void RefreshNonClient() {
			FloatLayout.RefreshCaption();
			if(DockManager.ActivePanel != null && DockManager.ActivePanel.DockLayout != FloatLayout)
				DockManager.ActivePanel.DockLayout.RefreshCaption();
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("FloatFormDockManager")]
#endif
		public DockManager DockManager { get { return (FloatLayout.DockManager); } }
		internal DockLayout FloatLayout {
			get { return floatLayoutCore; }
			set { floatLayoutCore = value; }
		}
		protected override void OnShown(EventArgs e) {
			base.OnShown(e);
			SubscribeOwnerEvents();
		}
		protected void SubscribeOwnerEvents() {
			if(Owner != null) {
				Owner.HandleDestroyed += Owner_HandleDestroyed;
				Owner.HandleCreated += Owner_HandleCreated;
			}
		}
		protected void UnSubscribeOwnerEvents() {
			Form owner = Owner ?? (DockManager != null ? DockManager.OwnerForm : null);
			if(owner != null) {
				owner.HandleDestroyed -= Owner_HandleDestroyed;
				owner.HandleCreated -= Owner_HandleCreated;
			}
		}
		protected override void OnParentChanged(EventArgs e) {
			base.OnParentChanged(e);
			if(IsMdiChild)
				UnSubscribeOwnerEvents();
		}
		void Owner_HandleCreated(object sender, EventArgs e) {
			if(Owner == null && DockManager != null && DockManager.OwnerForm != null)
				DockManager.OwnerForm.AddOwnedForm(this);
		}
		void Owner_HandleDestroyed(object sender, EventArgs e) {
			Form owner = Owner ?? (sender as Form);
			if(owner != null) owner.RemoveOwnedForm(this);
		}
		SizingAdorer sizingAdornerCore;
		protected internal SizingAdorer SizingAdorner {
			get {
				if(IsDisposing) return null;
				if(sizingAdornerCore == null) {
					DockPanel panel = (FloatLayout != null) ? FloatLayout.Panel as DockPanel : null;
					if(!CanCreateSizingAdorner(panel))
						return null;
					sizingAdornerCore = new SizingAdorer(this);
				}
				return sizingAdornerCore;
			}
		}
		bool CanCreateSizingAdorner(DockPanel panel) {
			return (panel != null) && !panel.DockedAsTabbedDocument && !panel.IsMdiDocument && (DockManager != null) && DockManager.SupportVS2010DockingStyle();
		}
		protected internal void ResetSizingAdorner() {
			Docking2010.Ref.Dispose(ref sizingAdornerCore);
		}
	}
}
