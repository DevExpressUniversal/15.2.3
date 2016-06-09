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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Serializing;
using DevExpress.XtraBars.Docking.Helpers;
using DevExpress.Utils.Drawing.Helpers;
namespace DevExpress.XtraBars.Docking.Helpers {
	[Flags]
	public enum ResizeDirection { None = 0, Left = 1, Right = 2, Top = 4, Bottom = 8, All = 15 }
	public enum FloatResizeZonePosition { LeftTop, RightTop, RightBottom, LeftBottom, Top, Bottom, Left, Right }
	public enum DockMode{  Standard, VS2005}
	public enum DockPanelInTabContainerTabRegion { DockImmediately, HighlightDockPosition}
	class AutoHideMoveEventsHelper : IDisposable {
		DockLayout layout;
		AutoHideMoveState state;
		public AutoHideMoveEventsHelper() { }
		public void SetLayout(DockLayout newLayout) {
			layout = newLayout;
			state = AutoHideMoveState.Collapsed;
		}
		DockPanel Panel { get { return layout == null ? null : layout.Panel; } }
		#region IDisposable Members
		public void Dispose() {
			layout = null;
			state = AutoHideMoveState.Collapsed;
		}
		#endregion
		public void OnCollapsing() {
			if(state != AutoHideMoveState.Expanded || Panel == null) return;
			state = AutoHideMoveState.Collapsing;
			Panel.RaiseAutoHideMoveEvents(state);			   
		}
		public void OnCollapsed() {
			if(state != AutoHideMoveState.Collapsing || Panel == null) return;
			state = AutoHideMoveState.Collapsed;
			Panel.RaiseAutoHideMoveEvents(state);			   
		}
		public bool OnExpanding() {
			if(state != AutoHideMoveState.Collapsed || Panel == null) return false;
			state = AutoHideMoveState.Expanding;
			return Panel.RaiseAutoHideMoveEvents(state);
		}
		public void OnExpanded() {
			if(state != AutoHideMoveState.Expanding || Panel == null) return;
			state = AutoHideMoveState.Expanded;
			Panel.RaiseAutoHideMoveEvents(state);
		}
	}
	public class AutoHideMoveHelper : IDisposable {
		Timer showTimer, hideTimer, moveTimer;
		AutoHideControl autoHideControl;
		DockLayoutManager layoutManager;
		DockLayout autoHideLayout;
		bool increaseSize;
		AutoHideMoveEventsHelper autoHideMoveEvents;
		public AutoHideMoveHelper(DockLayoutManager layoutManager) {
			this.layoutManager = layoutManager;
			this.showTimer = new Timer();
			this.showTimer.Interval = DockConsts.DefaultAutoShowInterval;
			this.hideTimer = new Timer();
			this.hideTimer.Interval = DockConsts.DefaultAutoHideInterval;
			this.moveTimer = new Timer();
			this.moveTimer.Interval = DockConsts.DefaultAutoMoveInterval;
			Subscribe();
			this.autoHideControl = null;
			this.autoHideLayout = null;
			this.increaseSize = false;
			this.autoHideMoveEvents = new AutoHideMoveEventsHelper();
		}
		public virtual void Dispose() {
			Unsubscribe();
			this.showTimer.Dispose();
			this.hideTimer.Dispose();
			this.moveTimer.Dispose();
			DestroyAutoHideControl();
			autoHideMoveEvents.Dispose();
		}
		void Subscribe(){
			this.moveTimer.Tick += new EventHandler(MoveTimer_Tick);
			this.showTimer.Tick += new EventHandler(ShowTimer_Tick);
			this.hideTimer.Tick += new EventHandler(HideTimer_Tick);
		}
		void Unsubscribe() {
			this.moveTimer.Tick -= new EventHandler(MoveTimer_Tick);
			this.showTimer.Tick -= new EventHandler(ShowTimer_Tick);
			this.hideTimer.Tick -= new EventHandler(HideTimer_Tick);
		}
		protected internal void AutoHiddenPanelShow(AutoHiddenPanelShowMode mouseClick){
			if(mouseClick == AutoHiddenPanelShowMode.MouseClick) Unsubscribe();
			else Subscribe();
		}
		protected internal virtual void AutoHideContainerDisposing(AutoHideContainer container) {
			if(AutoHideControl != null && AutoHideControl.Position == container.Position)
				DestroyAutoHideControl();
		}
		protected internal virtual void OnLayoutInfoAutoHideChanged(LayoutInfo info) {
			if(info.AutoHide && CanPerformAutoHideMoving && !DockManager.DockingOptions.HideImmediatelyOnAutoHide) {
				DockLayout dockLayout = (DockLayout)info;
				CheckActiveControl(dockLayout.Panel);
				ShowImmediately(dockLayout);
			}
		}
		void CheckActiveControl(ZIndexControl control) {
			ContainerControl ownerForm = DockManager.OwnerForm;
			if(ownerForm == null || ownerForm.ActiveControl == null) return;
			Control parent = ownerForm.ActiveControl.Parent;
			while(parent != null) {
				if(parent == control) {
					DockManager.ResetFormActiveControl(ownerForm.ActiveControl);
					return;
				}
				parent = parent.Parent;
			}
		}
		protected internal virtual void FormSizeChanged() {
			if(AutoHideControl == null || AutoHideControl.ClientControl == null) return;
			HideImmediately();
			StopAutoHide();
		}
		int creatingAutoHideControl = 0;
		protected internal virtual void ShowImmediately(DockLayout dockLayout) {
			DockLayout currentAutoHideControlDockLayout = GetAutoHideControlDockLayout();
			if(currentAutoHideControlDockLayout != null && (currentAutoHideControlDockLayout == dockLayout)) return;
			StopAutoHide();
			SetAutoHideLayoutCore(dockLayout);
			autoHideMoveEvents.SetLayout(dockLayout);
			if(autoHideMoveEvents.OnExpanding()) {
				creatingAutoHideControl++;
				if(AutoHideLayout == null)
					SetAutoHideLayoutCore(dockLayout);
				CreateAutoHideControl(dockLayout.Panel, AutoHideLayout);
				SetAutoHideControlBounds(dockLayout.Size);
				AutoHideContainers[dockLayout.AutoHideDock].LayoutChanged();
				creatingAutoHideControl--;
			}
		}
		bool IsCreatingAutoHideControl{ get { return creatingAutoHideControl > 0; } }
		protected internal virtual void HideImmediately() {
			if(AutoHideControl == null || IsCreatingAutoHideControl) return;   
			DockPanel autoHideControlPanel = GetAutoHideControlDockLayout().Panel;
			if(autoHideControlPanel == DockManager.ActivePanel || autoHideControlPanel.HasAsChild(DockManager.ActivePanel)) {
				if(autoHideControlPanel.ContainsFocus) {
					DockManager.ResetFocusedControl();
				}
				DockManager.ActivePanel = null;
			}
			StopAutoHide();
			AutoHideLayout = GetHotLayout();	
		}
		protected internal DockLayout AutoHideLayout { 
			get { return autoHideLayout; } 
			set {
				if(AutoHideLayout == value) return;
				if(MoveTimer.Enabled) {
					if(value == null) IncreaseSize = false;
					if(value == InnerAutoHideControlDockLayout) IncreaseSize = true;
					return;
				}
				AutoHideContainer ahc = GetAutoHideLayoutContainer(value);
				if(ahc == null || !ahc.IsShowSliding) {
					if(!CanPerformAutoHideMoving && !DockManager.CanAutoHideByClick) {
						StopAutoHide();
						return;
					}
				}			 
				InvertHideShowEnabled(AutoHideLayout != null && AutoHideControl != null);
				SetAutoHideLayoutCore(value);			 
			}
		}
		protected void SetAutoHideLayoutCore(DockLayout value) {
			this.autoHideLayout = value;
		}
		protected AutoHideContainer GetAutoHideLayoutContainer(DockLayout dockLayout) {
			if(dockLayout == null) return null;
			if(dockLayout.IsTab) dockLayout = dockLayout.Parent as DockLayout;
			if(dockLayout == null) return null;
			return LayoutManager.AutoHideContainers[dockLayout.AutoHideDock];
		}
		protected internal virtual void AutoHideHotLayoutChanged(AutoHideContainer container) {
			if(DockManager.AutoHiddenPanelShowMode != AutoHiddenPanelShowMode.MouseClick) {
				DockLayout hotLayout = GetHotLayout(container);
				AutoHideLayout = hotLayout;
			}
			if(AutoHideControl == null)
				container.LayoutChanged();
		}
		bool CheckValidHotLayout() {
			if(AutoHideControl.IsHot) return true;
			DockLayout dockLayout = GetAutoHideControlDockLayout();
			if(dockLayout.Tabbed) {
				if(AutoHideLayout != null && AutoHideLayout != dockLayout.ActiveChild)
					DockManager.ActivePanel = null;
				if(AutoHideLayout == null)
					return (dockLayout.Panel == DockManager.ActivePanel);
				return (dockLayout.ActiveChild == AutoHideLayout);
			}
			return (dockLayout == AutoHideLayout);
		}
		DockLayout GetHotLayout() {
			AutoHideContainer hot = null;
			foreach(AutoHideContainer container in DockManager.AutoHideContainers) {
				if(container.IsMouseInside || container.IsShowSliding) {
					hot = container;
					break;
				}
			}
			return GetHotLayout(hot);
		}
		bool IsAutoHideControlHot(DockPanel panel) {
			return AutoHideControl.IsHot;
		}
		protected internal virtual DockLayout GetHotLayout(AutoHideContainer container) {
			DockLayout result = null;
			if(container != null)
				result = container.GetHotLayout();
			if(result == null && AutoHideControl != null) {
				DockLayout hotLayout = InnerAutoHideControlDockLayout;
				if(AutoHideControl.IsHot)
					result = hotLayout;
				else if(hotLayout != null && hotLayout.Panel == DockManager.ActivePanel)
					result = hotLayout;
			}
			return result;
		}
		internal void DestroyAutoHideControlAndFireCollapseEvents() {
			autoHideMoveEvents.OnCollapsing();
			DestroyAutoHideControl();
			autoHideMoveEvents.OnCollapsed();
		}
		public virtual void StopAutoHide() {
			autoHideMoveEvents.OnCollapsing();
			Form mdiForm = this.DockManager.Form as Form;
			Form activeMdiChild = null;
			if(mdiForm != null) {
				activeMdiChild = mdiForm.ActiveMdiChild;
			}
			DisableTimers();
			DockLayout ahLayout = AutoHideLayout;
			DestroyAutoHideControl();
			AutoHideContainer container = GetAutoHideLayoutContainer(ahLayout);
			SetAutoHideLayoutCore(null);
			if(container != null)
				container.LayoutChanged();
			if(activeMdiChild != null) {
				activeMdiChild.BringToFront();
			}
			autoHideMoveEvents.OnCollapsed();
		}
		void DisableTimers() {
			EnableShowTimer(false);
			EnableMoveTimer(false);
			EnableHideTimer(false);
		}
		void InvertHideShowEnabled(bool enableHide) {
			EnableHideTimer(enableHide);
			EnableShowTimer(!enableHide);
		}
		void EnableShowTimer(bool enabled) {			
			ShowTimer.Enabled = enabled;			
		}
		void EnableHideTimer(bool enabled) {
			HideTimer.Enabled = enabled;			
		}
		void EnableMoveTimer(bool enabled) {			
			MoveTimer.Enabled = enabled;			
		}
		protected internal DockLayout GetDockLayoutAtPos(Point ptScreen) {
			DockLayout result = GetAutoHideControlDockLayout();
			if(result != null && !result.Panel.IsPointInside(ptScreen))
				result = null;
			return result;
		}
		DockLayout GetAutoHideControlDockLayout() {
			if(AutoHideControl == null || AutoHideControl.ClientControl == null) return null;
			DockPanel autoHideControlPanel = (DockPanel)AutoHideControl.ClientControl;
			return autoHideControlPanel.DockLayout;
		}
		internal void OnLayoutChanged() {
			DockLayout dockLayout = GetAutoHideControlDockLayout();
			if(dockLayout != null)
				dockLayout.LayoutChanged();
		}
		protected virtual void ShowTimer_Tick(object sender, EventArgs e) {
			EnableShowTimer(false);
			if(AutoHideControl != null && AreThereOpenDropDownControls(AutoHideControl, DockManager)) return;
			if(AutoHideLayout == null) return;
			if(AutoHideControl != null) {
				if(GetHotLayout() == null) {
					IncreaseSize = false;
					EnableMoveTimer(true);
					return;
				}
				if(GetHotLayout() == InnerAutoHideControlDockLayout) {
					if(InnerAutoHideControlDockLayout != null && !MoveTimer.Enabled) EnableHideTimer(true);
					return;
				}
			}
			if(AutoHideLayout != GetHotLayout()) return;
			DockLayout ahLayout = AutoHideLayout;
			DestroyAutoHideControlAndFireCollapseEvents();
			autoHideMoveEvents.SetLayout(ahLayout);
			if(autoHideMoveEvents.OnExpanding()) {
				IncreaseSize = true;
				CreateAutoHideControl(ahLayout.Panel, ahLayout);
				EnableMoveTimer(true);
				return;
			}
			EnableShowTimer(false);
		}
		protected virtual void HideTimer_Tick(object sender, EventArgs e) {
			EnableHideTimer(false);
			if(AutoHideControl == null) return;
			if(CanDecreaseAutoHideControlSize) {				
				IncreaseSize = false;
				DockLayout hotLayout = GetHotLayout();
				if(hotLayout == null) {
					autoHideMoveEvents.OnCollapsing();					
					EnableMoveTimer(AutoHideControl != null);
				}
				else {   
					Form activeForm = Form.ActiveForm;
					if(activeForm != null && activeForm.ShowInTaskbar == false &&
						activeForm.ControlBox == false &&
						activeForm.MinimizeBox == false &&
						activeForm.MaximizeBox == false &&
						activeForm.FormBorderStyle == FormBorderStyle.None
						) {				  
						EnableHideTimer(true);
						return;
					}					
					StopAutoHide();
					SetAutoHideLayoutCore(hotLayout);
					ShowTimer_Tick(ShowTimer, EventArgs.Empty);
				}
			}
			else {				
				EnableHideTimer(true);
			}
		}
		protected internal virtual void HideSliding() {
			if(AutoHideControl == null) return;
			EnableHideTimer(false);
			EnableShowTimer(false);
			IncreaseSize = false;
			autoHideMoveEvents.OnCollapsing();   
			EnableMoveTimer(true);
		}
		bool CanDecreaseAutoHideControlSize {
			get {
				if(AreThereOpenModalWindows())
					return false;
				if(AreThereOpenDropDownControls(AutoHideControl.ClientControl, DockManager))
					return false;
				Form parentForm = AutoHideControl.FindForm();
				Control activeControl = (parentForm == null ? null : parentForm.ActiveControl);
				if(AutoHideControl.ContainsFocus && activeControl != null) {
					return (GetHotLayout() != null && GetHotLayout() != InnerAutoHideControlDockLayout);
				} 
				if(activeControl != null && (AutoHideControl.Contains(activeControl) || AutoHideControl == activeControl)) {
					if(Form.ActiveForm != null && (!activeControl.ContainsFocus || DockManager.FindFocusedControl() != null)) { 
						return true;
					}
				}
				if(GetHotLayout() == InnerAutoHideControlDockLayout) return false;
				return (((DockPanel)AutoHideControl.ClientControl).State == DockPanelState.Regular);
			}
		}
		static internal bool AreThereOpenDropDownControls(Control control, DockManager dockManager) {
			foreach(Control tempControl in control.Controls) {
				DevExpress.XtraEditors.PopupBaseEdit pbe = tempControl as DevExpress.XtraEditors.PopupBaseEdit;
				if(pbe != null && pbe.IsPopupOpen) return true;
				if(tempControl.IsHandleCreated) {
					int result = (int)BarNativeMethods.SendMessage(tempControl.Handle, 0x157, 0, 0);
					if(result != 0) return true;
				}
				BarManager barManager = dockManager.GetMenuManager() as BarManager;
				if(barManager != null) {
					PopupMenuBase menu = barManager.GetPopupContextMenu(tempControl);
					if(menu != null && menu.Visible)
						return true;
				}
				BarDockControl bdc = tempControl as BarDockControl;
				if(bdc != null && bdc.Manager != null && bdc.Manager.SelectionInfo != null && bdc.Manager.SelectionInfo.OpenedPopups != null && bdc.Manager.SelectionInfo.OpenedPopups.Count != 0) return true;
				if(AreThereOpenDropDownControls(tempControl, dockManager)) return true;
			}
			return false;
		}
		Form GetParentForm() {
			return AutoHideControl.FindForm();
		}
		bool ChildContainsFocus() {
			Form parentForm = AutoHideControl.FindForm();
			Control activeControl = (parentForm == null ? null : parentForm.ActiveControl);
			if(AutoHideControl.ContainsFocus && activeControl != null) {
				return (GetHotLayout() != null && GetHotLayout() != InnerAutoHideControlDockLayout);
			} 
			if(activeControl != null && (AutoHideControl.Contains(activeControl) || AutoHideControl == activeControl)) {
				if(Form.ActiveForm != null && (!activeControl.ContainsFocus || DockManager.FindFocusedControl() != null)) { 
					return true;
				}
			}
			return false;
		}
		bool AreThereOpenModalWindows() {
			IntPtr hWnd = BarNativeMethods.GetForegroundWindow();
			Control control = Docking2010.WinAPIHelper.FindControl(hWnd);
			if(hWnd != IntPtr.Zero && control == null)
				return true;
			if(control is DevExpress.XtraEditors.XtraMessageBoxForm)
				return true;
			Form form = control as Form;
			if(form != null && form.Modal) {
				if(DockManager.Form != null)
					return (control != DockManager.Form && !Docking2010.Views.DocumentsHostContext.IsChild(DockManager.Form, form));
				return true;
			}
			if(DockManager.Form != null) {
				Form parentForm = GetParentForm();
				if(parentForm == null)
					return false;
				Form[] forms = parentForm.OwnedForms;
				for(int i = 0; i < forms.Length; i++) {
					if(forms[i].Modal) return true;
				}
			}
			return false;
		}
		bool CanDecreaseAutoHideControlSizeNew {
			get {
				if(DockManager == null && AutoHideControl == null) return false;
				DockPanel panel = AutoHideControl.ClientControl as DockPanel;
				if(panel != null) {
					if(panel.State == DockPanelState.Regular) { 
						if(IsAutoHideControlHot(panel)){
							return false;
						}
						else {
							if(ChildContainsFocus()) 
								return false;	 
							if(AreThereOpenDropDownControls(AutoHideControl.ClientControl, DockManager))
								return false;
							if(AreThereOpenModalWindows())
								return false;
							return true;
						}
					}
					else
						return false;
				}
				else
					return false;
			}
		}
		protected virtual void MoveTimer_Tick(object sender, EventArgs e) {
			if(AutoHideControl == null) return;
			int speedRatio = this.DockManager.AutoHideSpeed;
			DirectionSize dSize = new DirectionSize(AutoHideControl.Size, !AutoHideControl.IsHorizontal);
			dSize.Width += (IncreaseSize ? DockConsts.DefaultAutoMoveSize * speedRatio: -DockConsts.DefaultAutoMoveSize* speedRatio);
			SetAutoHideControlBounds(dSize.Size);
		}
		int suspendLayoutCount = 0;
		protected void SuspendLayout() {
			suspendLayoutCount++;
			DockManager.Form.SuspendLayout();
		}
		protected void ResumeLayout() {
			if(suspendLayoutCount == 0) return;
			if(suspendLayoutCount > 0)
				suspendLayoutCount--;
			if(suspendLayoutCount == 0)
				DockManager.Form.ResumeLayout();
		}
		protected internal void SetAutoHideControlBounds(Size newSize) {			
			DirectionRectangle dRect = new DirectionRectangle(LayoutManager.ClientBounds, DockLayoutUtils.ConvertToDockingStyle(AutoHideControl.Position));
			DockLayout hotLayout = GetAutoHideControlDockLayout();
			int maxSize = dRect.IsHorizontal ? hotLayout.Size.Width : hotLayout.Size.Height;
			int size = Math.Min(Math.Max(0, new DirectionSize(newSize, !AutoHideControl.IsHorizontal).Width), maxSize);
			bool enableMove = (size == 0 || size == maxSize);
			EnableMoveTimer(!enableMove);
			if(!enableMove) {
				AutoHideContainer container = GetAutoHideLayoutContainer(hotLayout);
				if(container != null && container.IsShowSliding) 
					container.EndShowSliding();
			}
			AutoHideControl.Bounds = dRect.GetSideRectangle(size);				
			if(size == 0) {		   
				DestroyAutoHideControl();
				autoHideMoveEvents.OnCollapsed();
				AutoHideLayout = GetHotLayout();				
				EnableHideTimer(true);
			}
			if(size == maxSize && !HideTimer.Enabled) {
				autoHideMoveEvents.OnExpanded();
				EnableHideTimer(true);
			}
		}
		Size GetCorrectSize(Size size, Size layoutSize) {
				Size resultSize = size;
				if(size.Width > layoutSize.Width)
					 resultSize.Width = layoutSize.Width;
				if(size.Height > layoutSize.Height)
					resultSize.Height = layoutSize.Height;
				return resultSize;
		}
		protected virtual void CreateAutoHideControl(DockPanel clientControl, DockLayout ahLayout) { 
			AutoHideContainer container = GetAutoHideLayoutContainer(ahLayout);
			if(container == null) return;
			this.autoHideControl = new AutoHideControl(container.Position);
			if(clientControl.IsTab && clientControl.ParentPanel != null && clientControl.ParentPanel.Visibility == DockVisibility.AutoHide)
				clientControl = clientControl.ParentPanel;
			clientControl.SuspendControlVisibleChanged();
			try {
				DirectionRectangle dRect = new DirectionRectangle(LayoutManager.ClientBounds, DockLayoutUtils.ConvertToDockingStyle(AutoHideControl.Position));
				int size = dRect.IsHorizontal ? ahLayout.Size.Width : ahLayout.Size.Height;
				this.autoHideControl.Bounds = dRect.GetSideRectangle(size);
				this.autoHideControl.Font = clientControl.Font; 
				this.autoHideControl.ClientControl = clientControl;
				this.autoHideControl.Close += new EventHandler(AutoHideControl_Close);
				Size newSize = Size.Empty;
				var topLayout = GetTopLayout(ahLayout);
				if(topLayout.SavedSize.IsEmpty)
					topLayout.SavedSize = topLayout.Size;
				topLayout.Size = GetCorrectSize(topLayout.SavedSize, LayoutManager.ClientSize);
				clientControl.DockLayout.LayoutChanged();
				container.OnAutoShow();
				SuspendLayout();
				DockManager.Form.Controls.Add(AutoHideControl);
				AutoHideControl.BringToFront();
				if(AutoHideControl.ContainsFocus && DockManager.Form.Controls.Count == 2) {
					DockManager.AutoHideContainers[0].Focus();
				}
				else {
					if(DockManager.ActivePanel == clientControl) DockManager.Form.ActiveControl = clientControl; 
				}
			}
			finally {
				clientControl.ResumeControlVisibleChanged();
			}
		}
		DockLayout GetTopLayout(DockLayout ahLayout) {
			if(ahLayout.Panel.ParentPanel != null) 
				return ahLayout.Panel.ParentPanel.DockLayout;
			else
				return ahLayout;
		}
		protected virtual void DestroyAutoHideControl() {
			if(AutoHideControl == null || IsCreatingAutoHideControl) return;			
			AutoHideContainer container = AutoHideContainers[DockLayoutUtils.ConvertToDockingStyle(AutoHideControl.Position)];
			((DockPanel)AutoHideControl.ClientControl).CheckMDIClient();			
			if(AutoHideControl.ContainsFocus) {
			}
			try {
				if(LayoutManager != null && DockManager != null)
					DockManager.IsCreatingAutohideControl = false;
				AutoHideControl.ClientControl = null;
			}
			finally {
				if(LayoutManager != null && DockManager != null)
					DockManager.IsCreatingAutohideControl = true;
			}
			AutoHideControl.Close -= new EventHandler(AutoHideControl_Close);
			AutoHideControl.Dispose();
			this.autoHideControl = null;
			SetAutoHideLayoutCore(null);
			if(container != null) {
				container.OnAutoHide();
				UpdatePanelsOrder(container);
			}
			ResumeLayout();
		}
		void UpdatePanelsOrder(AutoHideContainer autoHideContainer) {
			AutoHideInfo autoHideInfo = autoHideContainer.AutoHideInfo;
			int index = 0;
			for(int i = 0; i < autoHideInfo.Count; i++) {
				DockLayout dockLayout = autoHideInfo[i] as DockLayout;
				if(dockLayout == null) continue;
				DockPanel panel = dockLayout.Panel;
				if(panel != null && autoHideContainer.Controls.Contains(panel))
					autoHideContainer.Controls.SetChildIndex(panel, index++);
			}
		}
		protected virtual void AutoHideControl_Close(object sender, EventArgs e) {
			HideImmediately();
		}
		bool CanPerformAutoHideMoving { get { return DockManager.CanPerformAutoHideMoving; } }
		AutoHideContainerCollection AutoHideContainers { get { return LayoutManager.AutoHideContainers; } }
		internal DockLayout InnerAutoHideControlDockLayout {
			get {
				DockLayout dockLayout = GetAutoHideControlDockLayout();
				if(dockLayout == null) return null;
				return (dockLayout.Tabbed ? dockLayout.ActiveChild: dockLayout);
			}
		}
		protected bool IncreaseSize { get { return increaseSize; } set { increaseSize = value; } }
		protected DockLayoutManager LayoutManager { get { return layoutManager; } }
		protected DockManager DockManager { get { return LayoutManager.DockManager; } }
		protected Timer ShowTimer { get { return showTimer; } }
		protected Timer HideTimer { get { return hideTimer; } }
		protected Timer MoveTimer { get { return moveTimer; } }
		protected AutoHideControl AutoHideControl { get { return autoHideControl; } }
	}
	public class CaptionMouseHoverHelper : IDisposable {
		Timer hoverTimer, mouseMoveTimer;
		bool canMouseHover, hoverPanelHasCaption;
		DockLayoutManager manager;
		DockPanel hoverPanel;
		Point hotPos;
		public CaptionMouseHoverHelper(DockLayoutManager manager) {
			this.manager = manager;
			this.hoverTimer = new Timer();
			this.hoverTimer.Tick += new EventHandler(HoverTimer_Tick);
			this.hoverTimer.Interval = DockConsts.DefaultCaptionHoverInterval;
			this.mouseMoveTimer = new Timer();
			this.mouseMoveTimer.Tick += new EventHandler(MouseMoveTimer_Tick);
			this.mouseMoveTimer.Interval = DockConsts.DefaultMouseMoveInterval;
			this.canMouseHover = false;
			this.hoverPanelHasCaption = false;
			this.hoverPanel = null;
			this.hotPos = LayoutConsts.InvalidPoint;
		}
		public virtual void Dispose() {
			this.hoverTimer.Tick -= new EventHandler(HoverTimer_Tick);
			this.hoverTimer.Dispose();
			this.mouseMoveTimer.Tick -= new EventHandler(MouseMoveTimer_Tick);
			this.mouseMoveTimer.Dispose();
		}
		void HoverTimer_Tick(object sender, EventArgs e) {
			if(HoverPanelHasCaption) return;
			if(IsMouseOnTopEdge) {
				hoverPanelHasCaption = true;
				HoverPanel.DockLayout.LayoutChanged();
			}
			else {
				HotPos = Cursor.Position;
			}
		}
		void MouseMoveTimer_Tick(object sender, EventArgs e) {
			if(Control.MouseButtons != MouseButtons.None) return;
			DockPanel hotPanel = Manager.GetDockPanelAtPos(Cursor.Position);
			if(hotPanel != null && hotPanel.IsTab)
				hotPanel = hotPanel.ParentPanel;
			HoverPanel = hotPanel;
		}
		public void EnableMouseHoving(bool value) {
			if(CanMouseHover == value) return;
			this.canMouseHover = value;
			mouseMoveTimer.Enabled = value;
		}
		public virtual bool CanShowCaption(DockPanel panel) {
			if(panel == null) return false;
			if(panel.Dock == DockingStyle.Float) return true;
			if(HoverPanel != panel) return false;
			return HoverPanelHasCaption && (Manager.DockManager.FormContainsFocus || FloatFormContainsFocus(panel));
		}
		bool FloatFormContainsFocus(DockPanel panel) {
			if(panel.ParentPanel != null) {
				return FloatFormContainsFocus(panel.ParentPanel);
			}
			return panel.FloatForm != null && panel.FloatForm.ContainsFocus;
		}
		protected bool IsMouseOnTopEdge {
			get {
				if(HoverPanel == null || Cursor.Position != HotPos) return false;
				return HoverPanel.IsMouseOnTopEdge;
			}
		}
		protected DockPanel HoverPanel { 
			get { return hoverPanel; } 
			set {
				if(HoverPanel == value) return;
				if(value != null && value.IsTab) value = value.ParentPanel;
				if(value != null && value.Dock == DockingStyle.Float) value = null;
				if(HoverPanel != null) {
					HoverPanel.Disposed -= new EventHandler(OnHoverPanel_Disposed);
					this.hoverTimer.Enabled = false;
					this.hoverPanelHasCaption = false;
					HotPos = LayoutConsts.InvalidPoint;
					if(HoverPanel != null && HoverPanel.DockLayout != null) 
						HoverPanel.DockLayout.LayoutChanged();
				}
				this.hoverPanel = value;
				if(HoverPanel != null) {
					HoverPanel.Disposed += new EventHandler(OnHoverPanel_Disposed);
					HotPos = Cursor.Position;
					this.hoverTimer.Enabled = true;
				}
			}
		}
		void OnHoverPanel_Disposed(object sender, EventArgs e) {
			HoverPanel = null;
		}
		protected DockLayoutManager Manager { get { return manager; } }
		protected bool CanMouseHover { get { return canMouseHover; } }
		protected bool HoverPanelHasCaption { get { return hoverPanelHasCaption; } }
		protected Point HotPos { get { return hotPos; } set { hotPos = value; } }
	}
	public class DockConsts {
		public static readonly Size DefaultDockPanelSize = new Size(200, 200);
		public static readonly Size DefaultFloatFormMinSize = new Size(40, 30);
		public static readonly Size DefaultFloatFormSize = DefaultDockPanelSize;
		public const int DefaultAutoHideInterval = 850;
		public const int DefaultAutoShowInterval = 300;
		public const int DefaultAutoMoveInterval = 1;
		public const int DefaultCaptionHoverInterval = 250;
		public const int DefaultMouseMoveInterval = 10;
		public const int DefaultAutoMoveSize = 20;
		public const int DockZoneWidth = 6;
		public const int ResizeZoneWidth = 3;
		public const int DesignerResizeZoneWidth = 6;
		public const int ResizeSelectionFrameWidth = 4;
		public const int DockSelectionFrameWidth = 2;
		public const int RootParentID = -1;
		public const int AutoHideParentID = -2;
		public const int MdiContainerParentID = -10000;
		public const int MdiContainerParentIDMax = -2;
		public const int AutoHideContainerMaxCount = 4;
		public const TabsPosition DefaultTabsPosition = TabsPosition.Bottom;
		public const bool DefaultTabsScroll = false;
		public const bool DefaultOptionsHideImmediatelyOnAutoHide = false;
		public const bool DefaultOptionsCloseActiveTabOnly = true;
		public const bool DefaultOptionsCloseActiveFloatTabOnly = false;
		public const bool DefaultOptionsShowCaptionOnMouseHover = false;
		public const bool DefaultOptionsShowMaximizeButton = true;
		public const bool DefaultOptionsShowAutoHideButton = true;
		public const bool DefaultOptionsShowCloseButton = true;
		public const bool DefaultOptionsFloatOnDblClick = true;
		public const bool DefaultOptionsAllowFloating = true;
		public static readonly Color SelectionColor = Color.Gray;
		public static string ContainerControlFormatString { get { return "{0}_Container"; } }
		public static string AutoHideContainerNameFormatString { get { return "hideContainer{0}"; } }
		public static string InvalidAutoHideContainerChildFormatString { get { return "{0} is invalid AutoHideContainer child"; } }
		public static string InvalidControlContainerChildFormatString { get { return "{0} is invalid ControlContainer child"; } }
		public static string DockPanelContainerNameFormatString { get { return "panelContainer{0}"; } }
		public static string AddPanelManagerString { get { return "Add Panel"; } }
		public static string ClearLayoutManagerString { get { return "Clear Layout"; } }
		public static string DeleteContainerCaptionDialogString { get { return "Confirm Delete"; } }
		public static string DeleteContainerTextDialogFormatString { get { return "Are you sure you want to permanently delete {0} and all the panels it contains?"; } }
		public static string DockManagerFormIsNullMessageString { get { return "The 'Form' property must be assigned a value before panels can be added."; } }
		public static string DockManagerFormIsNullCaptionString { get { return "Error"; } }
		public static string DefaultDockPanelText { get { return "DockPanel"; } }
		public const string TempViewStyleName = "temp";
		public const string BehaviorCategory  = "Behavior";
		public const string PropertyChangedCategory  = "Property Changed";
		public const string DockingCategory = "Docking";
		public const string AppearanceCategory = "Appearance";
		public const string LayoutCategory = "Layout";
	}
	public class DockLayoutUtils {
		public static void UpdateAutoHideContainersZOrder(AutoHideContainerCollection autoHideContainers, int highestIndex) {
			DockingStyle[] docks = Enum.GetValues(typeof(DockingStyle)) as DockingStyle[];
			int counter = 0;
			for(int i = DockConsts.AutoHideContainerMaxCount - 1; i > -1; i --) {
				AutoHideContainer container = autoHideContainers[docks[1 + i]];
				if(container == null) continue;
				container.ZIndex = highestIndex - counter++;
			}
		}
		public static void UpdateRootDockPanelsZOrder(DockLayoutManager manager, int totalChildCount, int backControlsCount) {
			int layoutCount = GetLayoutCount(manager);
			int counter = 0;
			for(int i = manager.Count - 1; i > -1; i--) {
				DockLayout dockLayout = (DockLayout)manager[i];
				if(dockLayout.Panel == null || !IsValidLayout(dockLayout)) continue;
				dockLayout.Panel.ZIndex = counter++;
			}
		}
		public static void UpdateChildrenDockPanelsZOrder(DockLayout dockLayout) {
			dockLayout.Panel.SuspendLayout();
			try {
				for(int i = 0; i < dockLayout.Count; i++) {
					DockLayout child = dockLayout[i];
					child.Panel.ZIndex = i;
				}
			}
			finally {
				dockLayout.Panel.ResumeLayout();
			}
		}
		static int GetLayoutCount(BaseLayoutInfo info) {
			int result = 0;
			for(int i = 0; i < info.Count; i++) {
				if(IsValidLayout(info[i]))
					result++;
			}
			return result;
		}
		static bool IsValidLayout(LayoutInfo info) {
			return (info.Dock != DockingStyle.Float && !info.AutoHide);
		}
		public static DockStyle ConvertToDockStyle(DockingStyle dock, BaseLayoutInfo parent) {
			return (parent is DockLayout ? DockStyle.None : (DockStyle)dock);
		}
		public static DockStyle ConvertToDockStyle(TabsPosition position) {
			return ConvertToDockStyle(ConvertToDockingStyle(position), null);
		}
		public static DockStyle ConvertToDockStyle(string value) {
			return (DockStyle)Enum.Parse(typeof(DockStyle), value);
		}
		public static DockingStyle ConvertToDockingStyle(TabsPosition position) {
			return (DockingStyle)position;
		}
		public static SizingSide ConvertToSizingSide(ResizeZone zone) {
			FloatResizeZone frz = zone as FloatResizeZone;
			if(frz != null) {
				switch(frz.Position) {
					case FloatResizeZonePosition.Bottom:
						return SizingSide.Bottom;
					case FloatResizeZonePosition.Left:
						return SizingSide.Left;
					case FloatResizeZonePosition.LeftBottom:
						return SizingSide.LeftBottom;
					case FloatResizeZonePosition.LeftTop:
						return SizingSide.LeftTop;
					case FloatResizeZonePosition.Right:
						return SizingSide.Right;
					case FloatResizeZonePosition.RightBottom:
						return SizingSide.RightBottom;
					case FloatResizeZonePosition.RightTop:
						return SizingSide.RightTop;
					case FloatResizeZonePosition.Top:
						return SizingSide.Top;
					default:
						return SizingSide.Right;
				}
			}
			return (SizingSide)zone.Side;
		}
		public static DockingStyle ConvertToDockingStyle(string value) {
			return (DockingStyle)Enum.Parse(typeof(DockingStyle), value);
		}
		public static DockVisibility ConvertToDockVisibility(string value) {
			return (DockVisibility)Enum.Parse(typeof(DockVisibility), value);
		}
		public static Guid ConvertToGuid(string value) {
			Guid result = Guid.Empty;
			try{
				result = new Guid(value);
			}
			catch {}
			return result;
		}
		public static bool IsHead(TabsPosition position) {
			return LayoutRectangle.GetIsHead(ConvertToDockingStyle(position));
		}
		public static int ConvertToIndex(TabsPosition position) {
			return Convert.ToInt32(position) - 1;
		}
		public static DockingStyle ConvertToOppositeDockingStyle(TabsPosition position) {
			return LayoutRectangle.GetOppositeDockingStyle(ConvertToDockingStyle(position));
		}
		public static bool IsVerticalPosition(TabsPosition position) {
			return (position == TabsPosition.Left || position == TabsPosition.Right);
		}
		public static bool CanDraw(PaintEventArgs e, Rectangle bounds) {
			if(bounds.Width <= 0 || bounds.Height <= 0) return false;
			if(e == null) return true;
			return e.ClipRectangle.IntersectsWith(bounds);
		}
		public static Rectangle InflateRect(Rectangle bounds, TabsPosition position, int size) {
			bounds.Inflate(size, size);
			return LayoutRectangle.RemoveSize(bounds, size, ConvertToDockingStyle(position));
		}
		public static void CalcLayoutDockZones(IDockZonesOwner owner, IDockZonesOwner parent) {
			if(parent != null) {
				for(int i = 0; i < parent.DockZones.Count; i++)
					if(owner.CanCreateDockZoneReference(parent.DockZones[i])) {
						var dockZone = parent.DockZones[i] as LayoutManagerDockZone;
						if(dockZone != null && dockZone.RootLayout != owner)
							owner.DockZones.Insert(0, new DockZoneReference(owner, parent.DockZones[i]));
						else
							owner.DockZones.Add(new DockZoneReference(owner, parent.DockZones[i]));
					}
			}
			DockingStyle[] docks = Enum.GetValues(typeof(DockingStyle)) as DockingStyle[];
				for(int i = 1; i < docks.Length; i ++) {
					owner.CalcDockZones(docks[i]);
				}
		}
		public static bool IsEscapeDownMessage(ref Message m) {
			const int WM_KEYDOWN = 0x100;
			const int ESC = 27;
			return (m.Msg == WM_KEYDOWN && m.WParam.ToInt32() == ESC);
		}
		public static FloatForm GetFloatFormAtPos(Point ptScreen) {
			IntPtr result = BarNativeMethods.WindowFromPoint(new NativeMethods.POINT() { X = ptScreen.X, Y = ptScreen.Y });
			if(result.Equals(IntPtr.Zero)) return null;
			Control ctrl = Control.FromHandle(result);
			if(ctrl == null) return null;
			return ctrl.FindForm() as FloatForm;
		}
		public static Rectangle FlipRectangle(Rectangle bounds) {
			return new Rectangle(bounds.Top, bounds.Left, bounds.Height, bounds.Width);
		}
		#if DEBUG
		public static void OutDebugString(params object[] args) {
			string fmt = string.Empty;
			for(int i = 0; i < args.Length; i++) {
				fmt += "{" + i.ToString() + "}";
				if(i < args.Length - 1)
					fmt += " - ";
			}
		}
		#endif
	}
	public class SavedDockPanelInfo {
		DockPanel savedParent;
		DockingStyle savedDock;
		int savedIndex;
		bool savedTabbed;
		bool savedMdiDocument;
		public SavedDockPanelInfo() {
			this.savedDock = DockingStyle.Float;
			this.savedParent = null;
			this.savedIndex = LayoutConsts.InvalidIndex;
			this.savedTabbed = false;
			this.savedMdiDocument = false;
		}
		public virtual void Assign(SavedInfo savedInfo) {
			SavedDock = savedInfo.SavedDock;
			SavedIndex = savedInfo.SavedIndex;
			SavedTabbed = savedInfo.SavedTabbed;
			SavedParent = (savedInfo.SavedParent is DockLayout ? ((DockLayout)savedInfo.SavedParent).Panel : null);
			SavedMdiDocument = savedInfo.SavedMdiDocument;
		}
		public virtual void AssignTo(SavedInfo savedInfo) {
			savedInfo.SavedDock = SavedDock;
			savedInfo.SavedIndex = SavedIndex;
			savedInfo.SavedTabbed = SavedTabbed;
			savedInfo.SavedMdiDocument = SavedMdiDocument;
			if(SavedParent != null)
				savedInfo.SavedParent = SavedParent.DockLayout;
			else
				savedInfo.SavedParent = null;
		}
		public DockPanel SavedParent { get { return savedParent; } set { savedParent = value; }}
		public DockingStyle SavedDock { get { return savedDock; } set { savedDock = value; } }
		public int SavedIndex { get { return savedIndex; } set { savedIndex = value; } }
		public bool SavedTabbed{ get { return savedTabbed; } set { savedTabbed = value; } }
		public bool SavedMdiDocument { get { return savedMdiDocument; } set { savedMdiDocument = value; } }
	}
	public class StringUniqueCollection : CollectionBase {
		public string this[int index] { get { return (string)InnerList[index]; } }
		public int Add(string value) {
			if(Contains(value)) return -1;
			return List.Add(value);
		}
		public void Insert(int index, string value) {
			if(Contains(value)) return;
			List.Insert(index, value);
		}
		public void AddRange(string[] values) {
			if(values == null) return;
			for(int i = 0; i < values.Length; i++)
				Add(values[i]);
		}
		public void Remove(string value) {
			List.Remove(value);
		}
		public int IndexOf(string value) {
			return InnerList.IndexOf(value);
		}
		public bool Contains(string value) {
			return (IndexOf(value) != -1);
		}
	}
	public class XtraDeserializeInfo {
		DockPanel panel;
		int parentID, savedParentID;
		int activeChildID;
		int zIndex;
		DockVisibility visibility;
		DockingStyle autoHideDock;
		public XtraDeserializeInfo(DevExpress.Utils.Serializing.XtraSetItemIndexEventArgs e) {
			this.panel = e.Item.Value as DockPanel;
			this.parentID = Convert.ToInt32(e.Item.ChildProperties["XtraParentID"].Value, System.Globalization.CultureInfo.InvariantCulture);
			if(parentID < DockConsts.MdiContainerParentIDMax) {
				isMdiDocumentCore = true;
				parentID -= DockConsts.MdiContainerParentID;
			}
			this.savedParentID = Convert.ToInt32(e.Item.ChildProperties["XtraSavedParentID"].Value, System.Globalization.CultureInfo.InvariantCulture);
			this.activeChildID = Convert.ToInt32(e.Item.ChildProperties["XtraActiveChildID"].Value, System.Globalization.CultureInfo.InvariantCulture);
			this.zIndex = Convert.ToInt32(e.Item.ChildProperties["XtraZIndex"].Value, System.Globalization.CultureInfo.InvariantCulture);
			this.autoHideDock = DockLayoutUtils.ConvertToDockingStyle(e.Item.ChildProperties["XtraAutoHideContainerDock"].Value.ToString());
			if(!IsMdiDocument)
				this.visibility = DockLayoutUtils.ConvertToDockVisibility(e.Item.ChildProperties["Visibility"].Value.ToString());
		}
		bool isMdiDocumentCore;
		public bool IsMdiDocument {
			get { return isMdiDocumentCore; }
		}
		public DockPanel Panel { get { return panel; } }
		public int ParentID { get { return parentID; } }
		public int SavedParentID { get { return savedParentID; } }
		public int ActiveChildID { get { return activeChildID; } }
		public int ZIndex { get { return zIndex; } }
		public DockVisibility Visibility { get { return visibility; } }
		public DockingStyle AutoHideDock { get { return autoHideDock; } }
	}
}
namespace DevExpress.XtraBars.Docking {
	public enum SizingSide { Top = 1, Bottom, Left, Right, LeftTop, RightTop, RightBottom, LeftBottom, }
	public enum HitTest { None, Border, Client, Caption, TabPanel, Tab, PrevTabButton, NextTabButton, AutoHidePanel, EmbeddedButtonPanel }
	public enum VerticalTextOrientation { Default, BottomToTop, TopToBottom }
	public class HitInfo {
		HitTest hitTest;
		Point hitPoint;
		DockPanel tab, autoHidePanel;
		DockPanelCaptionButton button;
		Rectangle bounds;
		ResizeZone resizeZone;
		Control control;
		public static readonly HitInfo Empty = new HitInfo();
		HitInfo() : this(null, LayoutConsts.InvalidPoint) { }
		public HitInfo(Control control, Point hitPoint) {
			this.control = control;
			this.hitPoint = hitPoint;
			this.hitTest = HitTest.None;
			this.tab = null;
			this.autoHidePanel = null;
			this.button = null;
			this.resizeZone = null;
			this.bounds = LayoutConsts.InvalidRectangle;
		}
		internal void SetButton(DockPanelCaptionButton button, HitTest hitTest) {
			this.hitTest = hitTest;
			this.button = button;
			this.Bounds = (button == null ? LayoutConsts.InvalidRectangle : button.Bounds);
		}
		protected internal DockPanel GetDockingSourcePanel(DockPanel owner) {
			DockPanel result = null;
			if(HitTest == HitTest.Caption) result = owner;
			else result = Tab;
			if(result != null && !result.DockLayout.IsValid)
				result = null;
			return result;
		}
		protected internal Rectangle GetDockingSourceBounds(DockPanel owner) {
			DockPanel dockingSource = GetDockingSourcePanel(owner);
			if(dockingSource == null) return LayoutConsts.InvalidRectangle;
			if(dockingSource == Tab)
				return owner.DockLayout.TabsBounds[Tab.Index];
			return owner.DockLayout.CaptionBounds;
		}
		public bool IsEqual(object obj) {
			if(obj is HitInfo) {
				HitInfo hi = obj as HitInfo;
				return (hi.HitTest == this.HitTest && hi.Bounds == this.Bounds && hi.Button == this.Button && hi.Tab == this.Tab);
			}
			return false;
		}
		public HitTest HitTest { get { return hitTest; } set { hitTest = value; } }
		public Point HitPoint { get { return hitPoint; } }
		public DockPanel Tab { get { return tab; } set { tab = value; } }
		public DockPanel AutoHidePanel { get { return autoHidePanel; } set { autoHidePanel = value; } }
		public ResizeZone ResizeZone { get { return resizeZone; } set { resizeZone = value; } }
		public DockPanelCaptionButton Button { get { return button; } }
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		internal Control Control { get { return control; } }
	}
	public class BaseDockOptions : BaseOptions {
		bool floatOnDblClick, showMaximizeButton, showAutoHideButton, showCloseButton;
		protected BaseDockOptions() {
			this.showMaximizeButton = DockConsts.DefaultOptionsShowMaximizeButton;
			this.showAutoHideButton = DockConsts.DefaultOptionsShowAutoHideButton;
			this.showCloseButton = DockConsts.DefaultOptionsShowCloseButton;
			this.floatOnDblClick = DockConsts.DefaultOptionsFloatOnDblClick;
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				BaseDockOptions opt = options as BaseDockOptions;
				if(opt == null) return;
				this.showMaximizeButton = opt.ShowMaximizeButton;
				this.showAutoHideButton = opt.ShowAutoHideButton;
				this.showCloseButton = opt.ShowCloseButton;
				this.floatOnDblClick = opt.FloatOnDblClick;
			}
			finally {
				EndUpdate();
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseDockOptionsShowMaximizeButton"),
#endif
 DefaultValue(DockConsts.DefaultOptionsShowMaximizeButton), XtraSerializableProperty()]
		public virtual bool ShowMaximizeButton {
			get { return showMaximizeButton; }
			set {
				if(ShowMaximizeButton == value) return;
				this.showMaximizeButton = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowMaximizeButton", !ShowMaximizeButton, ShowMaximizeButton));
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseDockOptionsShowAutoHideButton"),
#endif
 DefaultValue(DockConsts.DefaultOptionsShowAutoHideButton), XtraSerializableProperty()]
		public virtual bool ShowAutoHideButton {
			get { return showAutoHideButton; }
			set {
				if(ShowAutoHideButton == value) return;
				this.showAutoHideButton = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowAutoHideButton", !ShowAutoHideButton, ShowAutoHideButton));
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseDockOptionsShowCloseButton"),
#endif
 DefaultValue(DockConsts.DefaultOptionsShowCloseButton), XtraSerializableProperty()]
		public virtual bool ShowCloseButton {
			get { return showCloseButton; }
			set {
				if(ShowCloseButton == value) return;
				this.showCloseButton = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowCloseButton", !ShowCloseButton, ShowCloseButton));
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("BaseDockOptionsFloatOnDblClick"),
#endif
 DefaultValue(DockConsts.DefaultOptionsFloatOnDblClick), XtraSerializableProperty()]
		public virtual bool FloatOnDblClick {
			get { return floatOnDblClick; }
			set {
				if(FloatOnDblClick == value) return;
				this.floatOnDblClick = value;
				OnChanged(new BaseOptionChangedEventArgs("FloatOnDblClick", !FloatOnDblClick, FloatOnDblClick));
			}
		}
		internal event BaseOptionChangedEventHandler Changed {
			add { base.ChangedCore += value; }
			remove { base.ChangedCore -= value; }
		}
	}
	public class LayoutSerializationOptions : BaseOptions {
		bool restoreDockPanelsTextCore;
		public LayoutSerializationOptions() {
			restoreDockPanelsTextCore = true;
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				LayoutSerializationOptions opt = options as LayoutSerializationOptions;
				if(opt == null) return;
				this.restoreDockPanelsTextCore = opt.restoreDockPanelsTextCore;
			}
			finally {
				EndUpdate();
			}
		}
		internal bool ShouldSerializeCore() { return ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("LayoutSerializationOptionsRestoreDockPanelsText"),
#endif
 DefaultValue(true)]
		public virtual bool RestoreDockPanelsText {
			get { return restoreDockPanelsTextCore; }
			set {
				if(restoreDockPanelsTextCore == value) return;
				this.restoreDockPanelsTextCore = value;
				OnChanged(new BaseOptionChangedEventArgs("RestoreDockPanelsText", !RestoreDockPanelsText, RestoreDockPanelsText));
			}
		}
	}
	public class DockingOptions : BaseDockOptions {
		bool hideImmediatelyOnAutoHide, closeActiveTabOnly, closeActiveFloatTabOnly, showCaptionOnMouseHover, showCaptionImage;
		DevExpress.Utils.DefaultBoolean dockPanelInCaptionRegion;
		Cursor cursorFloatCanceled;
		DockPanelInTabContainerTabRegion dockPanelInTabContainerTabRegion;
		VerticalTextOrientation autoHidePanelVerticalTextOrientation;
		public DockingOptions() {
			this.hideImmediatelyOnAutoHide = DockConsts.DefaultOptionsHideImmediatelyOnAutoHide;
			this.closeActiveTabOnly = DockConsts.DefaultOptionsCloseActiveTabOnly;
			this.closeActiveFloatTabOnly = DockConsts.DefaultOptionsCloseActiveFloatTabOnly;
			this.showCaptionOnMouseHover = DockConsts.DefaultOptionsShowCaptionOnMouseHover;
			this.dockPanelInTabContainerTabRegion = DockPanelInTabContainerTabRegion.DockImmediately;
			this.autoHidePanelVerticalTextOrientation = VerticalTextOrientation.Default;
			this.cursorFloatCanceled = null;
			this.showCaptionImage = false;
			this.dockPanelInCaptionRegion = DevExpress.Utils.DefaultBoolean.Default;
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				DockingOptions opt = options as DockingOptions;
				if(opt == null) return;
				this.hideImmediatelyOnAutoHide = opt.HideImmediatelyOnAutoHide;
				this.closeActiveTabOnly = opt.CloseActiveTabOnly;
				this.closeActiveFloatTabOnly = opt.CloseActiveFloatTabOnly;
				this.showCaptionOnMouseHover = opt.ShowCaptionOnMouseHover;
				this.dockPanelInTabContainerTabRegion = opt.DockPanelInTabContainerTabRegion;
				this.cursorFloatCanceled = opt.CursorFloatCanceled;
				this.showCaptionImage = opt.ShowCaptionImage;
				this.dockPanelInCaptionRegion = opt.DockPanelInCaptionRegion;
				this.autoHidePanelVerticalTextOrientation = opt.AutoHidePanelVerticalTextOrientation;
			}
			finally {
				EndUpdate();
			}
		}
		internal bool ShouldSerializeCore() { return ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockingOptionsAutoHidePanelVerticalTextOrientation"),
#endif
 DefaultValue(VerticalTextOrientation.Default), XtraSerializableProperty()]
		public VerticalTextOrientation AutoHidePanelVerticalTextOrientation {
			get { return autoHidePanelVerticalTextOrientation; }
			set {
				if(AutoHidePanelVerticalTextOrientation == value) return;
				VerticalTextOrientation prevValue = autoHidePanelVerticalTextOrientation;
				autoHidePanelVerticalTextOrientation = value;
				OnChanged(new BaseOptionChangedEventArgs("AutoHidePanelVerticalTextOrientation", prevValue, value));
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockingOptionsCursorFloatCanceled"),
#endif
 DefaultValue(null), XtraSerializableProperty()]
		public virtual Cursor CursorFloatCanceled {
			get { return cursorFloatCanceled; }
			set {
				if(CursorFloatCanceled == value) return;
				Cursor prevVal = this.cursorFloatCanceled;
				this.cursorFloatCanceled = value;
				OnChanged(new BaseOptionChangedEventArgs("CursorFloatCanceled", prevVal, CursorFloatCanceled));
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockingOptionsHideImmediatelyOnAutoHide"),
#endif
 DefaultValue(DockConsts.DefaultOptionsHideImmediatelyOnAutoHide), XtraSerializableProperty()]
		public virtual bool HideImmediatelyOnAutoHide {
			get { return hideImmediatelyOnAutoHide; }
			set {
				if(HideImmediatelyOnAutoHide == value) return;
				this.hideImmediatelyOnAutoHide = value;
				OnChanged(new BaseOptionChangedEventArgs("HideImmediatelyOnAutoHide", !HideImmediatelyOnAutoHide, HideImmediatelyOnAutoHide));
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockingOptionsCloseActiveTabOnly"),
#endif
 DefaultValue(DockConsts.DefaultOptionsCloseActiveTabOnly), XtraSerializableProperty()]
		public virtual bool CloseActiveTabOnly {
			get { return closeActiveTabOnly; }
			set {
				if(CloseActiveTabOnly == value) return;
				this.closeActiveTabOnly = value;
				OnChanged(new BaseOptionChangedEventArgs("CloseActiveTabOnly", !CloseActiveTabOnly, CloseActiveTabOnly));
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockingOptionsCloseActiveFloatTabOnly"),
#endif
 DefaultValue(DockConsts.DefaultOptionsCloseActiveFloatTabOnly), XtraSerializableProperty()]
		public virtual bool CloseActiveFloatTabOnly {
			get { return closeActiveFloatTabOnly; }
			set {
				if(CloseActiveFloatTabOnly == value) return;
				this.closeActiveFloatTabOnly = value;
				OnChanged(new BaseOptionChangedEventArgs("CloseActiveFloatTabOnly", !CloseActiveFloatTabOnly, CloseActiveFloatTabOnly));
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockingOptionsShowCaptionOnMouseHover"),
#endif
 DefaultValue(DockConsts.DefaultOptionsShowCaptionOnMouseHover), XtraSerializableProperty()]
		public virtual bool ShowCaptionOnMouseHover {
			get { return showCaptionOnMouseHover; }
			set {
				if(ShowCaptionOnMouseHover == value) return;
				this.showCaptionOnMouseHover = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowCaptionOnMouseHover", !ShowCaptionOnMouseHover, ShowCaptionOnMouseHover));
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockingOptionsShowCaptionImage"),
#endif
 DefaultValue(false), XtraSerializableProperty()]
		public virtual bool ShowCaptionImage {
			get { return showCaptionImage; }
			set {
				if(ShowCaptionImage == value) return;
				this.showCaptionImage = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowCaptionImage", !ShowCaptionImage, ShowCaptionImage));
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockingOptionsDockPanelInTabContainerTabRegion"),
#endif
 DefaultValue(DockPanelInTabContainerTabRegion.DockImmediately), XtraSerializableProperty()]
		public virtual DockPanelInTabContainerTabRegion DockPanelInTabContainerTabRegion {
			get { return dockPanelInTabContainerTabRegion; }
			set {
				if(DockPanelInTabContainerTabRegion == value) return;
				DockPanelInTabContainerTabRegion prevValue = this.dockPanelInTabContainerTabRegion;
				this.dockPanelInTabContainerTabRegion = value;
				OnChanged(new BaseOptionChangedEventArgs("DockPanelInTabContainerTabRegion", prevValue, DockPanelInTabContainerTabRegion));
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockingOptionsDockPanelInCaptionRegion"),
#endif
 DefaultValue(DevExpress.Utils.DefaultBoolean.Default), XtraSerializableProperty()]
		public virtual DevExpress.Utils.DefaultBoolean DockPanelInCaptionRegion {
			get { return dockPanelInCaptionRegion; }
			set {
				if(DockPanelInCaptionRegion == value) return;
				DevExpress.Utils.DefaultBoolean prevValue = this.dockPanelInCaptionRegion;
				this.dockPanelInCaptionRegion = value;
				OnChanged(new BaseOptionChangedEventArgs("DockPanelInCaptionRegion", prevValue, DockPanelInCaptionRegion));
			}
		}
	}
	public class DockPanelOptions : BaseDockOptions {
		DockPanel panel;
		bool allowFloating, allowDockLeft, allowDockRight, allowDockTop, allowDockBottom, allowDockFill, allowDockAsTabbedDocument;
		public DockPanelOptions(DockPanel panel) {
			this.panel = panel;
			resizeDirectionCore = ResizeDirection.Bottom | ResizeDirection.Right | ResizeDirection.Top | ResizeDirection.Left;
			this.allowFloating = DockConsts.DefaultOptionsAllowFloating;
			this.allowDockLeft = this.allowDockRight = this.allowDockTop = this.allowDockBottom =
				this.allowDockFill = this.allowDockAsTabbedDocument = true;
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				DockPanelOptions opt = options as DockPanelOptions;
				if(opt == null) return;
				this.allowFloating = opt.AllowFloating;
				this.allowDockLeft = opt.AllowDockLeft;
				this.allowDockRight = opt.AllowDockRight;
				this.allowDockTop = opt.AllowDockTop;
				this.allowDockBottom = opt.AllowDockBottom;
				this.allowDockFill = opt.AllowDockFill;
				this.allowDockAsTabbedDocument = opt.AllowDockAsTabbedDocument;
				this.resizeDirectionCore = opt.ResizeDirection;
			}
			finally {
				EndUpdate();
			}
		}
		protected internal bool GetOptionByDockingStyle(DockingStyle dock) {
			PropertyInfo pi = GetProperty(dock);
			if(pi == null) return false;
			return (bool)pi.GetValue(this, null);
		}
		protected internal void SetOptionByDockingStyle(DockingStyle dock) {
			PropertyInfo pi = GetProperty(dock);
			if(pi == null) return;
			bool oldValue = true;
			if(dock == DockingStyle.Fill)
				oldValue = (bool)pi.GetValue(this, null);
			pi.SetValue(this, oldValue & true, null);
		}
		PropertyInfo GetProperty(DockingStyle dock) {
			string optionName = "AllowDock" + dock.ToString();
			if(dock == DockingStyle.Float) {
				optionName = "AllowFloating";
				if(Panel != null && Panel.DockManager != null && Panel.DockManager.DockMode == DockMode.VS2005)
					optionName = "";
			}
			return GetType().GetProperty(optionName);
		}
		bool IsDockedAs(DockingStyle dock) {
			if(!(Panel.Parent is ContainerControl)) return false;
			return (Panel.Dock == dock);
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockPanelOptionsAllowFloating"),
#endif
 DefaultValue(DockConsts.DefaultOptionsAllowFloating), XtraSerializableProperty()]
		public virtual bool AllowFloating {
			get { return allowFloating; }
			set {
				if(AllowFloating == value) return;
				if(!value && Panel.Dock == DockingStyle.Float && !Panel.IsMdiDocument) return;
				this.allowFloating = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowFloating", !AllowFloating, AllowFloating));
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockPanelOptionsAllowDockLeft"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowDockLeft {
			get { return allowDockLeft; }
			set {
				if(AllowDockLeft == value) return;
				if(!value && IsDockedAs(DockingStyle.Left)) return;
				this.allowDockLeft = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowDockLeft", !AllowDockLeft, AllowDockLeft));
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockPanelOptionsAllowDockRight"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowDockRight {
			get { return allowDockRight; }
			set {
				if(AllowDockRight == value) return;
				if(!value && IsDockedAs(DockingStyle.Right)) return;
				this.allowDockRight = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowDockRight", !AllowDockRight, AllowDockRight));
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockPanelOptionsAllowDockTop"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowDockTop {
			get { return allowDockTop; }
			set {
				if(AllowDockTop == value) return;
				if(!value && IsDockedAs(DockingStyle.Top)) return;
				this.allowDockTop = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowDockTop", !AllowDockTop, AllowDockTop));
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockPanelOptionsAllowDockBottom"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowDockBottom {
			get { return allowDockBottom; }
			set {
				if(AllowDockBottom == value) return;
				if(!value && IsDockedAs(DockingStyle.Bottom)) return;
				this.allowDockBottom = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowDockBottom", !AllowDockBottom, AllowDockBottom));
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockPanelOptionsAllowDockFill"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowDockFill {
			get { return allowDockFill; }
			set {
				if(AllowDockFill == value) return;
				if(!value && Panel.ParentPanel != null) return;
				this.allowDockFill = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowDockFill", !AllowDockFill, AllowDockFill));
			}
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockPanelOptionsAllowDockAsTabbedDocument"),
#endif
 DefaultValue(true), XtraSerializableProperty()]
		public virtual bool AllowDockAsTabbedDocument {
			get { return allowDockAsTabbedDocument; }
			set {
				if(AllowDockAsTabbedDocument == value) return;
				if(!value && Panel.IsMdiDocument) return;
				this.allowDockAsTabbedDocument = value;
				OnChanged(new BaseOptionChangedEventArgs("AllowDockAsTabbedDocument", !value, value));
			}
		}
		ResizeDirection resizeDirectionCore;
		[XtraSerializableProperty, 
#if !SL
	DevExpressXtraBarsLocalizedDescription("DockPanelOptionsResizeDirection"),
#endif
 DefaultValue(ResizeDirection.All)]
		[System.ComponentModel.Editor(typeof(DevExpress.Utils.Editors.AttributesEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public ResizeDirection ResizeDirection { 
			get { return resizeDirectionCore; }
			set {
				if(resizeDirectionCore == value) return;
				ResizeDirection oldResizeDirection = resizeDirectionCore;
				resizeDirectionCore = value;
				OnChanged(new BaseOptionChangedEventArgs("ResizeDirection", oldResizeDirection, ResizeDirection));
			}
		}
		protected DockPanel Panel { get { return panel; } }
	}
}
