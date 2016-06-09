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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars.CodedUISupport;
using DevExpress.XtraBars.Docking.Paint;
namespace DevExpress.XtraBars.Docking.Helpers {
	public class DockPanelCaptionButton {
		ObjectState buttonState;
		Rectangle bounds;
		public DockPanelCaptionButton() {
			this.buttonState = ObjectState.Normal;
			this.bounds = LayoutConsts.InvalidRectangle;
		}
		public ObjectState ButtonState { get { return buttonState; } set { buttonState = value; } }
		public bool Disabled {
			get { return GetState(ObjectState.Disabled); }
			set { SetState(ObjectState.Disabled, value); }
		}
		public bool Hot {
			get { return GetState(ObjectState.Hot); }
			set { SetState(ObjectState.Hot, value); }
		}
		public bool Pressed {
			get { return GetState(ObjectState.Pressed); }
			set { SetState(ObjectState.Pressed, value); }
		}
		public bool Selected {
			get { return GetState(ObjectState.Selected); }
			set { SetState(ObjectState.Selected, value); }
		}
		bool GetState(ObjectState state) { return (ButtonState & state) != 0; }
		void SetState(ObjectState state, bool check) {
			if(check)
				ButtonState |= state;
			else
				ButtonState &= ~state;
		}
		public Rectangle Bounds { get { return bounds; } set { bounds = value; } }
	}
	public class DockPanelCaptionToggleButton : DockPanelCaptionButton {
		bool switched;
		public DockPanelCaptionToggleButton() {
			this.switched = true;
		}
		public bool Switched { get { return switched; } set { switched = value; } }
	}
	public interface IDockZonesOwner {
		DockZoneCollection DockZones { get; }
		Rectangle ScreenRectangle { get; }
		void CalcDockZones(DockingStyle dock);
		bool CanCreateDockZoneReference(DockZone dockZone);
		int GetSourceDockSize(DockLayout source, DockingStyle dock);
	}
	public interface IDockTarget {
		Control Target { get; }
	}
	public enum DockEmulatorState { Nothing, Added, Removed }
	public interface IDockEmulator {
		DockEmulatorState State { get; }
		void EmulateDocking(DockLayout source, int index);
		void Undock(DockLayout source, DockLayout prevActiveChild, int prevActiveChildIndex, Rectangle[] prevTabsBounds);
		void BeginUpdate();
		void EndUpdate();
	}
	public class DockLayout : LayoutInfo, IDockZonesOwner, IDockEmulator, IDockTarget {
		DockPanel panel;
		TabsPosition tabsPosition;
		bool tabsScroll;
		DockEmulatorState state;
		int adjustCounter, calcDockZonesCounter, calcResizeZonesCounter, lockRefreshCounter;
		int firstVisibleTabIndex, imageIndex;
		AppearanceObject captionAppearance, tabsAppearance, tabsAppearanceHot, activeTabAppearance;
		DockPanelCaptionButton prevTabButton, nextTabButton;
		ArrayList tabButtons;
		Rectangle[] tabsBounds;
		Rectangle borderBounds, clientBounds, captionBounds, captionTextBounds, tabPanelBounds, captionImageBounds;
		GraphicsCache cache;
		ResizeZoneCollection resizeZones;
		DockZoneCollection dockZones;
		Size floatSize;
		public DockLayout(DockingStyle dock, DockPanel panel) : this(dock, DockConsts.DefaultDockPanelSize, panel) { }
		public DockLayout(DockingStyle dock, Size size, DockPanel panel)
			: base(dock, size) {
			this.panel = panel;
			this.tabsPosition = DockConsts.DefaultTabsPosition;
			this.tabsScroll = DockConsts.DefaultTabsScroll;
			this.adjustCounter = this.calcDockZonesCounter = this.calcResizeZonesCounter = this.lockRefreshCounter = 0;
			this.firstVisibleTabIndex = 0;
			this.imageIndex = LayoutConsts.InvalidIndex;
			this.captionAppearance = new FrozenAppearance();
			this.tabsAppearance = new FrozenAppearance();
			this.tabsAppearanceHot = new FrozenAppearance();
			this.activeTabAppearance = new FrozenAppearance();
			this.cache = null;
			this.floatSize = DockConsts.DefaultFloatFormSize;
			this.resizeZones = new ResizeZoneCollection();
			this.dockZones = new DockZoneCollection();
			this.ClearViewInfo();
		}
		protected override void OnDisposing() {
			base.OnDisposing();
			CaptionAppearance.Dispose();
			TabsAppearance.Dispose();
			ActiveTabAppearance.Dispose();
		}
		public virtual void ClearViewInfo() {
			isRightToLeftCore = null;
			this.borderBounds = this.clientBounds = this.captionBounds =
				this.captionTextBounds = this.tabPanelBounds = this.captionImageBounds = LayoutConsts.InvalidRectangle;
			this.tabsBounds = new Rectangle[] { };
			this.prevTabButton = new DockPanelCaptionButton();
			this.nextTabButton = new DockPanelCaptionButton();
			this.tabButtons = new ArrayList();
			if(!IsCalcResizeZonesLocked)
				ResizeZones.Clear();
			if(!IsCalcDockZonesLocked)
				DockZones.Clear();
		}
		protected override void UpdateRecursive() {
			if(DockManager != null)
				this.SavedInfo.SavedParent = DockManager.LayoutManager;
		}
		public new DockLayout this[int index] { get { return base[index] as DockLayout; } }
		internal Size SavedSize {get; set;}
		public DockLayout AddLayout() {
			return AddLayoutCore(GetTailNewItemIndex());
		}
		public DockLayout AddLayout(int index) {
			index = Math.Max(0, Math.Min(Count, index));
			return AddLayoutCore(index);
		}
		protected virtual DockLayout AddLayoutCore(int index) {
			DockLayout result = base.InsertLayout(DockingStyle.Fill, index) as DockLayout;
			if(result != null)
				result.Panel.ZIndex = index;
			LayoutChanged();
			return result;
		}
		protected internal override void AddLayoutCore(LayoutInfo info) {
			Size oldSize = FloatSize;
			if(IsDeserializing) {
				if(!List.Contains(info))
					List.Add(info);
			}
			else base.AddLayoutCore(info);
			FloatSize = (Panel != null && Panel.FloatForm != null) ? Panel.FloatForm.ClientSize : oldSize;
		}
		protected override LayoutInfo CreateLayout(DockingStyle dock) {
			return CreateLayoutCore(dock, true);
		}
		protected override LayoutInfo CreateParentLayout(DockingStyle dock) {
			return CreateLayoutCore(dock, false);
		}
		protected override void DestroyLayout(LayoutInfo info) {
			DockLayout dockLayout = (DockLayout)info;
			Panel.RemovePanel(dockLayout.Panel);
		}
		protected virtual DockLayout CreateLayoutCore(DockingStyle dock, bool createControlContainer) {
			DockPanel resultPanel = LayoutManager.CreateDockPanel(createControlContainer, dock);
			return resultPanel.DockLayout;
		}
		protected override void DockToCore(BaseLayoutInfo destInfo, DockingStyle dock, int index) {
			DockManagerCodedUIHelper.AddDockInfoBeforeStartDock(this.Panel, (destInfo as IDockTarget).Target as DockPanel, dock, index);
			Guid sourcePanelId = Panel.ID;
			DockManager dm = DockManager;
			dm.OnStartDock(Panel, (destInfo as IDockTarget).Target, dock);
			BeginUpdate();
			try {
				base.DockToCore(destInfo, dock, index);
				Panel.ControlDock = DockLayoutUtils.ConvertToDockStyle(dock, destInfo);
				if(Panel.Dock == DockingStyle.Float)
					Panel.MakeFloatCore();
			}
			finally {
				if(!Panel.IsMdiDocument)
					Panel.dockedAsTabbedDocumentCore = false;
				EndUpdate();
				dm.OnEndDock();
			}
			DockManagerCodedUIHelper.AddDockInfoAfterEndDock(sourcePanelId, this.Panel, (destInfo as IDockTarget).Target as DockPanel);
		}
		protected override void SetParent(object info, bool assignThis) {
			base.SetParent(info, assignThis);
			DockLayout dockLayout = (DockLayout)info;
			if(assignThis) dockLayout.Panel.AssignDockManager(this);
			dockLayout.Panel.SetParent(assignThis ? Panel : null);
		}
		int lockUpdateCounter = 0;
		public void BeginParentUpdate() {
			lockUpdateCounter++;
		}
		public void EndParentUpdate() {
			lockUpdateCounter--;
		}
		public bool IsParentUpdateLocked {
			get { return lockUpdateCounter > 0; }
		}
		protected override void OnRemoveComplete(int index, object value) {
			bool lockUpdateForm = (CanDestroyOnRemoveComplete && DockManager != null && Panel.Parent == DockManager.Form);
			if(lockUpdateForm) DockManager.BeginFormLayoutUpdate();
			try {
				LockRefresh(); 
				DockLayout layout = (DockLayout)value;
				layout.BeginParentUpdate();
				layout.Panel.SetParent(null);
				layout.EndParentUpdate();
				try {
					base.OnRemoveComplete(index, value);
				}
				finally {
					UnlockRefresh();
				}
			}
			finally {
				if(lockUpdateForm) DockManager.EndFormLayoutUpdate();
				if(DockManager != null) DockManager.UpdateControlsZOrder();
			}
		}
		protected override void OnMadeContainer(LayoutInfo addedInfo) {
			base.OnMadeContainer(addedInfo);
			DockManager.RaiseRegisterDockPanel(Panel);
			if(Parent != null && Parent == LayoutManager) {
				LayoutManager.BeginUpdateZOrder();
				LayoutManager.EndUpdateZOrder();
			}
		}
		void LockRefresh() { this.lockRefreshCounter++; }
		void UnlockRefresh() { this.lockRefreshCounter--; }
		internal void BeginRefresh() {
			BeginUpdate();
			LockRefresh();
		}
		internal void EndRefresh() {
			UnlockRefresh();
			EndUpdate();
		}
		bool IsRefreshLocked { get { return (this.lockRefreshCounter != 0 || Disposing || IsDeserializing); } }
		public void UndockOnRestoreLayout() {
			SetVisibilityCore(DockVisibility.Visible);
			SetActiveChildCore(null);
			if(Parent != null)
				Parent.InternalRemoveLayout(this);
		}
		protected override void AssignContentToCore(LayoutInfo assignTo) {
			DockLayout dockLayout = (DockLayout)assignTo;
			LayoutManager.AutoHideContainers.BeginUpdate();
			dockLayout.LockRefresh();
			try {
				SwapPanels(dockLayout);
			}
			finally {
				dockLayout.UnlockRefresh();
				LayoutManager.AutoHideContainers.CancelUpdate();
			}
			dockLayout.imageIndex = ImageIndex;
			dockLayout.FloatSize = FloatSize;
			dockLayout.SizeFactor = SizeFactor;
			dockLayout.FloatVertical = FloatVertical;
			dockLayout.DockVertical = DockVertical;
			dockLayout.TabsPosition = TabsPosition;
			dockLayout.Image = imageCore;
			dockLayout.ImageUri = imageUriCore;
			dockLayout.AllowCustomHeaderButtonsGlyphSkinning = AllowCustomHeaderButtonsGlyphSkinning;
			this.imageIndex = -1;
			this.floatSize = DockConsts.DefaultFloatFormSize;
			base.AssignContentToCore(dockLayout);
		}
		protected override void AssignChildContent(LayoutInfo child) {
			base.AssignChildContent(child);
			DockLayout childLayout = (DockLayout)child;
			childLayout.Panel.SetParent(Panel);
		}
		void SwapPanels(DockLayout layoutToSwap) {
			DockPanel tmpPanel = Panel;
			this.panel = layoutToSwap.Panel;
			layoutToSwap.panel = tmpPanel;
			layoutToSwap.Panel.SwapContent(Panel);
			if(Manager != null)
				Manager.DockManager.UpdateRootPanels();
		}
		protected override bool IsLayoutLocked { get { return (base.IsLayoutLocked || IsDeserializing); } }
		protected override void LayoutChangedCore() {
			base.LayoutChangedCore();
			CalcViewInfo();
			Panel.SuspendLayout();
			try {
				AdjustControlBounds();
			}
			finally {
				Panel.ResumeLayout(false);
			}
			if(AutoHide) LayoutManager.AutoHideContainers.LayoutChanged();
			if(Panel.ControlContainer != null) {
				Panel.ControlContainer.LayoutChanged();
				UpdateDockControls(); 
			}
			Panel.Invalidate();
		}
		void UpdateDockControls() {
			foreach(Control control in Panel.ControlContainer.Controls) {
				BarDockControl dockControl = control as BarDockControl;
				if(dockControl != null)
					dockControl.UpdateDockSize();
			}
		}
		protected internal void AdjustControlBounds() {
			if((IsLoading || Parent == null || IsAdjustBoundsLocked) && !(AutoHide || IsParentAutoHide)) return;
			LockAdjustBounds();
			try {
				Panel.Bounds = Bounds;
				if(Panel.ControlContainer != null)
					Panel.ControlContainer.Bounds = ClientBounds;
				foreach(DockLayout dockLayout in this) {
					dockLayout.LayoutChanged();
				}
			}
			finally {
				UnlockAdjustBounds();
			}
		}
		protected override Rectangle GetAutoHideBounds() {
			Rectangle result = base.GetAutoHideBounds();
			if(Panel.ParentAutoHideControl != null) {
				DirectionRectangle dRect = new DirectionRectangle(Panel.Parent.ClientRectangle, AutoHideDock);
				result = dRect.GetSideRectangle(result.Size);
			}
			return result;
		}
		protected internal void LockAdjustBounds() { this.adjustCounter++; }
		protected internal void UnlockAdjustBounds() { this.adjustCounter--; }
		protected internal bool IsAdjustBoundsLocked { get { return (adjustCounter != 0); } }
		internal void SetInvisiblePanelLocation() {
			LockAdjustBounds();
			try {
				Panel.Location = LayoutConsts.InvalidPoint;
			}
			finally {
				UnlockAdjustBounds();
			}
		}
		protected internal Rectangle GetCorrectBounds(Rectangle bounds) {
			if(IsAdjustBoundsLocked) return bounds;
			Size = bounds.Size;
			return Bounds;
		}
		protected override void OnChildIndexChanged(LayoutInfo info) {
			DockLayoutUtils.UpdateChildrenDockPanelsZOrder(this);
		}
		protected override void OnTabbedChanged() {
			base.OnTabbedChanged();
			Panel.UpdateChildPanelsVisibiblity();
			Panel.RaiseTabbedChanged();
		}
		protected override void OnChangeVisibility(DockVisibility oldVisibility, DockVisibility newVisibility) {
			base.OnChangeVisibility(oldVisibility, newVisibility);
			Panel.RaiseVisibilityChanged(new VisibilityChangedEventArgs(Panel, oldVisibility));
		}
		internal override void OnRestoreVisibility() {
			DockVisibility oldVisibility = Panel.Visibility;
			if(!CanSetVisibity(DockVisibility.Visible)) return;
			base.OnRestoreVisibility();
			Panel.RaiseVisibilityChanged(new VisibilityChangedEventArgs(Panel, oldVisibility));
		}
		protected override void OnSetAutoHide() {
			if(IsDeserializing) return;
			try {
				SavedSize = Size;
				foreach(DockLayout layout in this)
					layout.SavedSize = layout.Size;
				FloatForm fForm = null;
				Docking2010.DocumentContainer dContainer = null;
				if(IsMdiDocument){
					fForm = Panel.FloatForm;
					dContainer = Panel.Parent as Docking2010.DocumentContainer ?? Panel.parentContainer;
				}
				base.OnSetAutoHide();
				Docking2010.Ref.Dispose(ref fForm);
				Docking2010.Ref.Dispose(ref dContainer);
				Panel.dockedAsTabbedDocumentCore = false;
				if(Panel == DockManager.ActivePanel && AutoHide)
					DockManager.ActivePanel = null;
				if(Panel.ParentAutoHideControl != null && Panel.ParentAutoHideControl.ContainsFocus) {
					DockManager.FocusDockFillControl();
				}
			}
			finally {
				if(DockManager.IsOwnerFormMDIContainer && LayoutManager != null) DockManager.UpdateControlsZOrder();
			}
		}
		protected override void OnActiveChildChanged() {
			Panel.UpdateChildPanelsVisibiblity();
			if(Panel.ButtonsPanel != null && Panel.ButtonsPanel.ViewInfo != null)
				Panel.ButtonsPanel.ViewInfo.SetDirty();
			base.OnActiveChildChanged();
			Panel.RaiseActiveChildChanged();
		}
		protected internal virtual void OnActivate() {
			RefreshCaption();
			if(RootLayout.AutoHide && !IsDeserializing)
				LayoutManager.ShowImmediately(RootLayout);
		}
		protected override Size DefaultSize { get { return DockConsts.DefaultDockPanelSize; } }
		public override Size MinSize {
			get {
				Size size = Painter.WindowPainter.GetSizeByClientSize(Painter.GetMinCaptionSize(CaptionAppearance, Float));
				if(size.Height < GetMinButtonPanelHeight())
					size.Height = GetMinButtonPanelHeight();
				return size;
			}
		}
		protected Size GetMinButtonPanelSize() {
			if(Panel != null && Panel.ButtonsPanel != null && Panel.ButtonsPanel.ViewInfo != null) {
				MergeCustomHeaderButtons();
				Graphics g = Painter.AddGraphics(null);
				try {
					return Panel.ButtonsPanel.ViewInfo.CalcMinSize(g);
				}
				finally { Painter.ReleaseGraphics(); }
			}
			return Size.Empty;
		}
		protected void MergeCustomHeaderButtons() {
			Panel.lockButtonsChanged++;
			try {
				if(Panel.ActiveChild != null && Panel.ActiveChild.IsTab && !Panel.IsDisposed)
					MergeCustomHeaderButtonsCore();
				else {
					Panel.ButtonsPanel.Buttons.ClearMergedButtons();
					Panel.ButtonsPanel.Buttons.Merge(Panel.CustomHeaderButtons);
				}
			}
			finally { Panel.lockButtonsChanged--; }
		}
		protected void MergeCustomHeaderButtonsCore() {
			int buttonsCount = Panel.CustomHeaderButtons.Count;
			int childButtonsCount = Panel.ActiveChild.CustomHeaderButtons.Count;
			var buttons = new DevExpress.XtraEditors.ButtonPanel.IBaseButton[buttonsCount + childButtonsCount];
			Panel.ActiveChild.CustomHeaderButtons.CopyTo(buttons, 0);
			Panel.CustomHeaderButtons.CopyTo(buttons, childButtonsCount);
			Panel.ButtonsPanel.Buttons.Merge(buttons);
		}
		protected virtual Rectangle ClipBorderBounds(Rectangle bounds) {
			if(HasBorder)
				return Painter.WindowPainter.GetClientRect(bounds);
			return bounds;
		}
		public override Rectangle GetBoundingResizeRectangle() {
			Rectangle result = base.GetBoundingResizeRectangle();
			if(Panel.ParentAutoHideControl != null)
				return DockManager.Form.RectangleToScreen(result);
			return Panel.Parent.RectangleToScreen(result);
		}
		public new DockLayoutManager Manager { get { return (base.Manager as DockLayoutManager); } }
		public DockManager DockManager { get { return Panel.DockManager; } }
		protected internal override bool IsLoading { get { return IsDeserializing || DockManager.IsInitializing; } }
		protected virtual bool IsDeserializing {
			get {
				if(DockManager == null) return true;
				return DockManager.IsDeserializing;
			}
		}
		public override void Restore() {
			if(IsDeserializing) return;
			base.Restore();
		}
		protected virtual bool CanCalcViewInfo {
			get {
				if(Disposing || DockManager == null) return false;
				if(Parent == null || Manager == null) {
					if(AutoHide) return true;
					return IsParentAutoHide;
				}
				return true;
			}
		}
		protected internal virtual void CalcViewInfo() {
			if(!CanCalcViewInfo) return;
			CalcCaptionAppearance();
			this.cachedSizeCore = Size.Empty;
			ClearViewInfo();
			CalcViewRects();
			CalcCaptionInfo();
			CalcTabPanelInfo();
			CalcResizeZones();
			CalcDockZones();
		}
		protected virtual void CalcViewRects() { CalcViewRects(Size); }
		protected virtual void CalcViewRects(Size clSize) {
			this.borderBounds = new Rectangle(Point.Empty, clSize);
			Rectangle currentBounds = ClipBorderBounds(borderBounds);
			this.clientBounds = LayoutRectangle.RemoveSize(currentBounds, CaptionHeight, DockingStyle.Top);
			this.captionBounds = LayoutRectangle.RemoveSize(currentBounds, ClientBounds.Height, DockingStyle.Bottom);
			if(HasBorder)
				this.borderBounds = Painter.WindowPainter.UpdateBorderAndClientBounds(borderBounds, ref this.clientBounds, Float);
			Rectangle clRect = this.clientBounds;
			this.clientBounds = LayoutRectangle.RemoveSize(clRect, TabPanelHeight, DockLayoutUtils.ConvertToDockingStyle(TabsPosition));
			if(this.clientBounds.Width < 0) this.clientBounds.Width = 0;
			if(this.clientBounds.Height < 0) this.clientBounds.Height = 0;
			this.tabPanelBounds = LayoutRectangle.RemoveSize(clRect, new DirectionSize(clientBounds.Size, LayoutRectangle.GetIsHorizontal(DockLayoutUtils.ConvertToOppositeDockingStyle(TabsPosition))).Width, DockLayoutUtils.ConvertToOppositeDockingStyle(TabsPosition));
		}
		internal Rectangle GetClientBoundsOnFloating() {
			CalcViewRects(FloatSize);
			return ClientBounds;
		}
		bool? isRightToLeftCore;
		protected internal bool IsRightToLeft {
			get {
				if(!isRightToLeftCore.HasValue)
					isRightToLeftCore = DevExpress.XtraEditors.WindowsFormsSettings.GetIsRightToLeft(Panel);
				return isRightToLeftCore.Value;
			}
		}
		protected virtual void CalcCaptionInfo() {
			if(!HasCaption) return;
			CalcCaptionAppearance();
			Rectangle captionClientBounds = Painter.WindowPainter.GetCaptionClientBounds(CaptionBounds, Float);
			int pos = captionClientBounds.Right - Painter.WindowPainter.CaptionButtonInterval;
			if(CanShowCaptionImage) {
				captionImageBounds = CalcCaptionImageBounds(captionClientBounds);
				captionClientBounds = new Rectangle(captionClientBounds.X + captionImageBounds.Width, captionClientBounds.Y,
					captionClientBounds.Width - captionImageBounds.Width, captionClientBounds.Height);
			}
			EnsureDefaultButtons();
			if(Panel != null && Panel.ButtonsPanel != null) {
				Graphics g = Painter.AddGraphics(null);
				try {
					int buttonsPanelOffset = (captionClientBounds.Height - GetMinButtonPanelSize().Height) / 2;
					Rectangle buttons = new Rectangle(
						captionClientBounds.X,
						captionClientBounds.Top + buttonsPanelOffset,
						pos - captionClientBounds.X,
						captionClientBounds.Height - buttonsPanelOffset * 2);
					Panel.ButtonsPanel.BeginUpdate();
					Panel.ButtonsPanel.ContentAlignment = IsRightToLeft ? 
						ContentAlignment.MiddleLeft : ContentAlignment.MiddleRight;
					Panel.ButtonsPanel.RightToLeft = IsRightToLeft;
					Panel.ButtonsPanel.ViewInfo.SetDirty();
					Panel.ButtonsPanel.CancelUpdate();
					if(IsRightToLeft)
						SwapRectangle(ref buttons, CaptionBounds);
					Panel.ButtonsPanel.ViewInfo.Calc(g, buttons);
					pos = GetNextPos(GetRTLBounds(Panel.ButtonsPanel.ViewInfo.Bounds, CaptionBounds));
				}
				finally { Painter.ReleaseGraphics(); }
			}
			captionTextBounds = CalcCaptionTextBounds(captionClientBounds, pos);
			if(IsRightToLeft) {
				SwapRectangle(ref captionTextBounds, CaptionBounds);
				SwapRectangle(ref captionImageBounds, CaptionBounds);
			}
		}
		internal Rectangle GetRTLBounds(Rectangle r, Rectangle bounds) {
			return IsRightToLeft ? SwapRectangle(ref r, bounds) : r;
		}
		internal static int SwapOffset(ref int offset, Rectangle bounds) {
			offset = bounds.Left + (bounds.Right - offset);
			return offset;
		}
		internal static Rectangle SwapRectangle(ref Rectangle r, Rectangle bounds) {
			if(r.Width > 0 && r.Height > 0)
				r = new Rectangle(bounds.Left + (bounds.Right - r.Right), r.Top, r.Width, r.Height);
			return r;
		}
		void EnsureDefaultButtons() {
			if(Panel == null || Panel.ButtonsPanel == null) return;
			Panel.lockButtonsChanged++;
			try {
				var closeButton = Panel.ButtonsPanel.Buttons[0].Properties;
				closeButton.Visible = HasCloseButton;
				var autoHideButton = Panel.ButtonsPanel.Buttons[1].Properties;
				autoHideButton.Visible = HasAutoHideButton;
				autoHideButton.LockCheckEvent();
				autoHideButton.Checked = (AutoHide || (IsTab && LayoutParent.AutoHide));
				autoHideButton.UnlockCheckEvent();
				var maximizeHideButton = Panel.ButtonsPanel.Buttons[2].Properties;
				maximizeHideButton.LockCheckEvent();
				maximizeHideButton.Visible = HasMaxButton;
				if(HasMaxButton) {
					if(Panel != null && Panel.Dock == DockingStyle.Float) {
						if(Panel.FloatForm != null)
							maximizeHideButton.Checked = Panel.FloatForm.WindowState != FormWindowState.Normal;
					}
					else maximizeHideButton.Checked = (LayoutParent.ActiveChild == this);
				}
				maximizeHideButton.UnlockCheckEvent();
				MergeCustomHeaderButtons();
			}
			finally { Panel.lockButtonsChanged--; }
		}
		void CalcCaptionAppearance() {
			if(Float) {
				bool IsActiveForm = IsFloatFormActive;
				if(Panel.FloatForm != null)
					IsActiveForm &= Panel.FloatForm.IsActive;
				Parameters.InitApplicationCaptionAppearance(CaptionAppearance, IsActiveForm ? AppearancesSettings.FloatFormCaptionActive : AppearancesSettings.FloatFormCaption, IsActiveForm);
			}
			else {
				Parameters.InitWindowCaptionAppearance(CaptionAppearance, Panel.CanDrawCaptionActive ? AppearancesSettings.PanelCaptionActive : AppearancesSettings.PanelCaption, Panel.CanDrawCaptionActive);
			}
			CaptionAppearance.TextOptions.RightToLeft = IsRightToLeft;
		}
		bool IsFloatFormActive { get { return (Panel.FloatForm == Form.ActiveForm && DockManager.CanActivateFloatForm); } }
		internal void InitControlContainerAppearance(AppearanceObject appearance, AppearanceObject controlContainerAppearance) {
			Parameters.InitPanelAppearance(appearance, controlContainerAppearance, AppearancesSettings.Panel, Float);
		}
		int GetNextPos(Rectangle bounds) {
			return bounds.Left - Painter.WindowPainter.CaptionButtonInterval;
		}
		Rectangle CalcCaptionTextBounds(Rectangle bounds, int pos) {
			Rectangle result = bounds;
			result.X += 2 * Painter.WindowPainter.CaptionButtonInterval;
			result.Width = pos - bounds.Left - Painter.WindowPainter.CaptionButtonInterval;
			return result;
		}
		Rectangle CalcCaptionImageBounds(Rectangle bounds) {
			if(CaptionImageSize.Width > bounds.Width)
				return Rectangle.Empty;
			Rectangle result = bounds;
			result.X += Painter.WindowPainter.CaptionButtonInterval;
			result.Y += (bounds.Height - CaptionImageSize.Height) / 2;
			result.Size = CaptionImageSize;
			return result;
		}
		protected virtual void CalcTabPanelInfo() {
			CalcTabPanelAppearances();
			if(!HasTabPanel) return;
			DirectionRectangle tabsBounds = new DirectionRectangle(TabPanelBounds, (DockingStyle)TabsPosition);
			tabsBounds.RemoveSize(TabVertForeIndent, true);
			tabsBounds.RemoveSize(TabVertBackIndent, false);
			tabsBounds = new DirectionRectangle(tabsBounds.Bounds, !tabsBounds.IsHorizontal);
			tabsBounds.RemoveSize(TabHorzBackIndent, true);
			tabsBounds.RemoveSize(TabHorzBackIndent, false);
			CheckDecreaseFirstVisibleTabIndex();
			CalcTabsScrollButtonsBounds(tabsBounds);
			CalcTabs(tabsBounds);
			UpdateTabScrollButtonsState();
		}
		void CalcTabPanelAppearances() {
			Parameters.InitTabsAppearance(TabsAppearance, AppearancesSettings.Tabs, TabsPosition);
			Parameters.InitActiveTabAppearance(ActiveTabAppearance, AppearancesSettings.ActiveTab, TabsPosition);
			Parameters.InitTabsAppearanceHot(TabsAppearanceHot, AppearancesSettings.Tabs, TabsPosition);
			TabsAppearance.TextOptions.RightToLeft = IsRightToLeft;
			ActiveTabAppearance.TextOptions.RightToLeft = IsRightToLeft;
			TabsAppearanceHot.TextOptions.RightToLeft = IsRightToLeft;
		}
		void CalcTabsScrollButtonsBounds(DirectionRectangle tabsBounds) {
			if(!TabsScroll) return;
			NextTabButton.Bounds = tabsBounds.GetPrevRectangle(Painter.TabPanelPainter.TabsButtonSize, 0);
			DirectionSize dSize = new DirectionSize(NextTabButton.Bounds.Size, tabsBounds.IsHorizontal);
			PrevTabButton.Bounds = tabsBounds.GetPrevRectangle(Painter.TabPanelPainter.TabsButtonSize, dSize.Width);
		}
		void CalcTabs(DirectionRectangle tabsArea) {
			List<TabWidthInfo> tabsWidthes = new List<TabWidthInfo>();
			int sum = 0;
			for(int i = 0; i < Count; i++) {
				tabsWidthes.Add(new TabWidthInfo(i, GetChildTabWidth(i)));
				sum += tabsWidthes[i].Width;
			}
			tabsBounds = new Rectangle[Count];
			CalcTabs(tabsArea, tabsWidthes, 0, FirstVisibleTabIndex, sum, -1);
			CalcTabs(tabsArea, tabsWidthes, FirstVisibleTabIndex, Count, sum, 0);
		}
		void CalcTabs(DirectionRectangle tabsArea, List<TabWidthInfo> tabsWidthes, int firstIndex, int boundIndex, int sumWidth, int tabWidthCoeff) {
			int tabWidth = 0;
			int pos = 0;
			List<TabWidthInfo> visibleTabsWidthes = new List<TabWidthInfo>();
			for(int i = firstIndex; i < boundIndex; i++) {
				visibleTabsWidthes.Add(tabsWidthes[i]);
			}
			visibleTabsWidthes.Sort(PositiveCompare);
			if(!(TabsScroll || sumWidth < tabsArea.Width)) {
				CalcTabsWidth(tabsArea.Width, sumWidth, visibleTabsWidthes);
			}
			for(int i = firstIndex; i < boundIndex; i++) {
				if(!this[i].IsValid) continue;
				tabWidth = visibleTabsWidthes.Find((TabWidthInfo info) => (info.Index == i)).Width;
				DirectionSize dSize = new DirectionSize(new Size(tabWidth, tabsArea.Height), tabsArea.IsHorizontal);
				Rectangle tabBounds = tabsArea.GetNextRectangle(dSize.DirectSize, pos);
				tabBounds.X += tabWidth * tabWidthCoeff;
				if(i == ActiveChildIndex)
					tabBounds = Painter.TabPanelPainter.UpdateActiveTabBounds(tabBounds, TabsPosition);
				if(tabsArea.IsHorizontal && IsRightToLeft)
					SwapRectangle(ref tabBounds, tabsArea.Bounds);
				TabsBounds[i] = tabBounds;
				pos += tabWidth;
			}
		}
		void CalcTabsWidth(int tabsAreaWidth, int sumWidth, List<TabWidthInfo> tabsWidthes) {
			int widthDifference = sumWidth - tabsAreaWidth;
			while(widthDifference > 0) {
				int equalsWidthTabCount = tabsWidthes.Select(d => d).Where(d => d.Width == tabsWidthes[0].Width).Count();
				if(equalsWidthTabCount < tabsWidthes.Count) {
					if((tabsWidthes[0].Width - tabsWidthes[equalsWidthTabCount].Width) * equalsWidthTabCount < widthDifference) {
						for(int i = 0; i < equalsWidthTabCount; i++) {
							widthDifference -= tabsWidthes[i].Width - tabsWidthes[equalsWidthTabCount].Width;
							tabsWidthes[i].Width = tabsWidthes[equalsWidthTabCount].Width;
						}
					}
					else {
						for(int i = 0; i < equalsWidthTabCount; i++) {
							int width = Round((float)widthDifference / (float)(equalsWidthTabCount - i));
							tabsWidthes[i].Width -= width;
							widthDifference -= width;
						}
						break;
					}
				}
				else {
					for(int i = 0; i < tabsWidthes.Count; i++) {
						int width = Round((float)tabsAreaWidth / (float)(tabsWidthes.Count - i));
						tabsWidthes[i].Width = width;
						tabsAreaWidth -= width;
					}
					break;
				}
			}
		}
		class TabWidthInfo {
			public TabWidthInfo(int index, int width) {
				Index = index;
				Width = width;
			}
			public int Index { get; set; }
			public int Width { get; set; }
		}
		int Round(float value) {
			return (int)(value + 0.5);
		}
		static int PositiveCompare(TabWidthInfo x, TabWidthInfo y) {
			if(x == y) return 0;
			return y.Width.CompareTo(x.Width);
		}
		int GetChildTabWidth(int index) {
			if(!this[index].IsValid) return 0;
			return Painter.TabPanelPainter.GetTabWidth(this[index], this);
		}
		void UpdateTabScrollButtonsState() {
			bool inLayout = TabPanelBounds.Contains(PrevTabButton.Bounds) && TabPanelBounds.Contains(NextTabButton.Bounds);
			PrevTabButton.Disabled = GetPrevValidChild(FirstVisibleTabIndex) == null || !inLayout;
			NextTabButton.Disabled = GetNextValidChild(LastVisibleTabIndex) == null || !inLayout;
		}
		internal void LockCalcResizeZones() { calcResizeZonesCounter++; }
		internal void UnlockCalcResizeZones() { calcResizeZonesCounter--; }
		bool IsCalcResizeZonesLocked { get { return (calcResizeZonesCounter != 0); } }
		protected internal virtual bool CheckResizeDirection(ResizeDirection direction) {
			ResizeDirection panelDirection = Panel.Options.ResizeDirection;
			if(panelDirection == ResizeDirection.All) return true;
			if(panelDirection == ResizeDirection.None) return false;
			return panelDirection.HasFlag(direction);
		}
		ResizeDirection ConvertToResizeDirection(DockingStyle style) {
			switch(style) {
				case DockingStyle.Left: return ResizeDirection.Left;
				case DockingStyle.Right: return ResizeDirection.Right;
				case DockingStyle.Bottom: return ResizeDirection.Bottom;
				case DockingStyle.Top: return ResizeDirection.Top;
				default: return ResizeDirection.None;
			}
		}
		ResizeDirection ConvertToResizeDirection(FloatResizeZonePosition position) {
			switch(position) {
				case FloatResizeZonePosition.Left: return ResizeDirection.Left;
				case FloatResizeZonePosition.Right: return ResizeDirection.Right;
				case FloatResizeZonePosition.Bottom: return ResizeDirection.Bottom;
				case FloatResizeZonePosition.Top: return ResizeDirection.Top;
				case FloatResizeZonePosition.LeftBottom: return ResizeDirection.Left | ResizeDirection.Bottom;
				case FloatResizeZonePosition.LeftTop: return ResizeDirection.Left | ResizeDirection.Top;
				case FloatResizeZonePosition.RightBottom: return ResizeDirection.Right | ResizeDirection.Bottom;
				case FloatResizeZonePosition.RightTop: return ResizeDirection.Right | ResizeDirection.Top;
				default: return ResizeDirection.None;
			}
		}
		protected virtual void CalcResizeZones() {
			if(IsCalcResizeZonesLocked) return;
			if(AutoHide) {
				CreateResizeZone(LayoutRectangle.GetOppositeDockingStyle(AutoHideDock));
				return;
			}
			if(Float) {
				CalcFloatResizeZones();
				return;
			}
			if(LayoutParent != null) {
				for(int i = 0; i < LayoutParent.ResizeZones.Count; i++)
					CreateResizeZoneReference(LayoutParent.ResizeZones[i]);
				if(CanAddSplitResizeZone)
					CreateResizeZone(IsHorizontal ? DockingStyle.Right : DockingStyle.Bottom);
			}
			if(IsSide && LayoutParent == null)
				CreateResizeZone(LayoutRectangle.GetOppositeDockingStyle(Dock));
		}
		void CalcFloatResizeZones() {
			FloatResizeZonePosition[] positions = Enum.GetValues(typeof(FloatResizeZonePosition)) as FloatResizeZonePosition[];
			for(int i = 0; i < positions.Length; i++) {
				DockLayout dockLayout = Tabbed ? ActiveChild : this;
				ResizeDirection direction = ConvertToResizeDirection(positions[i]);
				CreateResizeZoneEventArgs args = new CreateResizeZoneEventArgs(Panel, direction);
				if(dockLayout != null)
					args.Cancel = !dockLayout.CheckResizeDirection(direction);
				if(Panel != null && Panel.RaiseCreateResizeZone(args))
					ResizeZones.Add(new FloatResizeZone(this, positions[i]));
			}
		}
		protected internal override bool CanAddLayout(DockingStyle dock) {
			if(IsSide && dock != DockingStyle.Fill && Dock != DockingStyle.Float) {
				return (LayoutRectangle.GetIsHorizontal(dock) != IsHorizontal);
			}
			return base.CanAddLayout(dock);
		}
		protected override bool CanSetVisibity(DockVisibility value) {
			if(IsDeserializing) return true;
			return base.CanSetVisibity(value);
		}
		protected virtual void CalcDockZones() {
			if(IsCalcDockZonesLocked) return;
			DockLayoutUtils.CalcLayoutDockZones(this, Parent as IDockZonesOwner);
		}
		protected override DockMode DragVisualizationStyle {
			get {
				if(DockManager != null) {
					return DockManager.DockMode;
				}
				return base.DragVisualizationStyle;
			}
		}
		#region IDockZonesOwner
		void IDockZonesOwner.CalcDockZones(DockingStyle dock) {
			CalcDockZonesStandard(dock);
		}
		void CalcDockZonesStandard(DockingStyle dock) {
			if(!CanAddLayout(dock)) return;
			if(IsTab) return;
			if(dock == DockingStyle.Fill) {
				if(Count == 0) {
					DockZones.Add(new TabCaptionDockZone(this, dock));
				}
				else if(Tabbed) {
					DockZones.Add(new TabVisualTabPanelDockZone(this, dock));
				}
				if(CanDockPanelInCaptionRegion())
					DockZones.Add(new TabVisualCaptionDockZone(this, dock));
			}
			else {
				int increaseTailZoneValue = LayoutRectangle.GetIsHead(dock) ? 0 : 1;
				if(Count == 0) DockZones.Add(new NestedDockZone(this, dock, increaseTailZoneValue));
				else {
					if(Tabbed)
						DockZones.Add(new NestedDockZone(this, dock, increaseTailZoneValue));
					else
						for(int i = 0; i < Count; i++)
							DockZones.Add(new NestedDockZone(this, dock, i + increaseTailZoneValue));
				}
			}
		}
		bool IDockZonesOwner.CanCreateDockZoneReference(DockZone dockZone) {
			if(Float) return false;
			return ((IDockZonesOwner)this).ScreenRectangle.IntersectsWith(dockZone.Bounds);
		}
		int IDockZonesOwner.GetSourceDockSize(DockLayout source, DockingStyle dock) {
			int sourceSize = new DirectionSize(source.OriginalSize, LayoutRectangle.GetIsHorizontal(dock)).Width;
			int ownerSize = new DirectionSize((this as IDockZonesOwner).ScreenRectangle.Size, LayoutRectangle.GetIsHorizontal(dock)).Width;
			return Math.Min(sourceSize, ownerSize / 2);
		}
		DockZoneCollection IDockZonesOwner.DockZones { get { return this.DockZones; } }
		Rectangle IDockZonesOwner.ScreenRectangle {
			get {
				if(Panel.IsDisposing) return LayoutConsts.InvalidRectangle;
				return Panel.RectangleToScreen(Panel.ClientRectangle);
			}
		}
		#endregion IDockZonesOwner
		#region IDockEmulator
		int dockEmulator_LockUpdate = 0;
		void IDockEmulator.EmulateDocking(DockLayout source, int index) {
			IDockEmulator_UndockCore(source);
			IDockEmulator_DockCore(source, index);
			IDockEmulator_Update(true);
		}
		void IDockEmulator.Undock(DockLayout source, DockLayout prevActiveChild, int prevActiveChildIndex, Rectangle[] prevTabsBounds) {
			IDockEmulator_UndockCore(source);
			SetActiveChildCore(prevActiveChild);
			ActiveChild.Index = prevActiveChildIndex;
			tabsBounds = prevTabsBounds;
			IDockEmulator_Update(false);
		}
		void IDockEmulator_DockCore(DockLayout source, int index) {
			if(IDockEmulator_IsChangingChildIndex(source)) {
				IDockEmulator_LockCalcDockZones();
				try {
					source.Index = index;
				}
				finally {
					IDockEmulator_UnlockCalcDockZones();
				}
				return;
			}
			DockLayout activeChild = source;
			if(source.HasChildren) {
				for(int i = 0; i < source.Count; i++)
					InnerList.Insert(index + i, source[i]);
				activeChild = source.ActiveChild == null ? source[0] : source.ActiveChild;
			}
			else
				InnerList.Insert(index, source);
			SetActiveChildCore(activeChild);
			state = DockEmulatorState.Added;
		}
		void IDockEmulator_UndockCore(DockLayout source) {
			if(IDockEmulator_IsChangingChildIndex(source)) return;
			if(source.HasChildren) {
				for(int i = 0; i < source.Count; i++)
					IDockEmulator_Remove(source[i]);
			}
			else IDockEmulator_Remove(source);
			state = DockEmulatorState.Removed;
		}
		bool IDockEmulator_IsChangingChildIndex(DockLayout source) {
			return (source.Parent == this);
		}
		void IDockEmulator_Remove(DockLayout source) {
			if(InnerList.Contains(source))
				InnerList.Remove(source);
		}
		void IDockEmulator_LockCalcDockZones() { calcDockZonesCounter++; }
		void IDockEmulator_UnlockCalcDockZones() { calcDockZonesCounter--; }
		bool IsCalcDockZonesLocked { get { return (calcDockZonesCounter != 0) || !Panel.IsHandleCreated; } }
		void IDockEmulator_Update(bool lockCalcDockZones) {
			if(dockEmulator_LockUpdate != 0) return;
			if(lockCalcDockZones) IDockEmulator_LockCalcDockZones();
			try {
				CalcViewInfo();
			}
			finally {
				if(lockCalcDockZones) IDockEmulator_UnlockCalcDockZones();
			}
			if(DockManager != null && Panel != null) {
				Panel.Invalidate();
				if(DockManager.canImmediateRepaint)
					Panel.Update();
			}
		}
		void IDockEmulator.BeginUpdate() { dockEmulator_LockUpdate++; }
		void IDockEmulator.EndUpdate() { dockEmulator_LockUpdate--; }
		DockEmulatorState IDockEmulator.State { get { return state; } }
		#endregion IDockEmulator
		Control IDockTarget.Target { get { return Panel; } }
		bool CanAddSplitResizeZone {
			get {
				if(LayoutParent == null || IsTab || Index == LayoutParent.Count - 1) return false;
				if(AdjacentSplit == null) return false;
				DirectionSize dSize = new DirectionSize(Size, IsHorizontal),
						 dMinSize = new DirectionSize(MinSize, IsHorizontal),
						 dSplitSize = new DirectionSize(AdjacentSplit.Size, IsHorizontal),
						 dSplitMinSize = new DirectionSize(AdjacentSplit.MinSize, IsHorizontal);
				return (dSize.Width + dSplitSize.Width > dMinSize.Width + dSplitMinSize.Width);
			}
		}
		protected virtual void CreateResizeZoneReference(ResizeZone parentZone) {
			if(CheckResizeDirection(ConvertToResizeDirection(parentZone.Side)))
				ResizeZones.Add(new ResizeZoneReference(this, parentZone));
		}
		protected virtual void CreateResizeZone(DockingStyle side) {
			DockLayout dockLayout = Tabbed ? ActiveChild : this;
			ResizeDirection direction = ConvertToResizeDirection(side);
			CreateResizeZoneEventArgs args = new CreateResizeZoneEventArgs(Panel, direction);
			if(dockLayout != null)
				args.Cancel = !dockLayout.CheckResizeDirection(direction);
			if(Panel != null && Panel.RaiseCreateResizeZone(args))
				ResizeZones.Add(new ResizeZone(this, side));
		}
		public virtual HitInfo GetHitInfo(Point pt) {
			HitInfo result = new HitInfo(Panel, pt);
			if(!Panel.ClientRectangle.Contains(pt)) return result;
			result.HitTest = HitTest.Border;
			result.Bounds = Panel.ClientRectangle;
			for(int i = 0; i < ResizeZones.Count; i++) {
				if(ResizeZones[i].Contains(pt)) {
					result.ResizeZone = ResizeZones[i];
					break;
				}
			}
			if(Panel.FloatForm != null && Panel.FloatForm.WindowState == FormWindowState.Maximized)
				result.ResizeZone = null;
			if(CaptionBounds.Contains(pt)) {
				result.HitTest = HitTest.Caption;
				result.Bounds = CaptionBounds;
				if(Panel != null && Panel.ButtonsPanel != null && Panel.ButtonsPanel.ViewInfo.Bounds.Contains(pt)) {
					result.HitTest = HitTest.EmbeddedButtonPanel;
					result.Bounds = Panel.ButtonsPanel.ViewInfo.Bounds;
				}
				return result;
			}
			if(ClientBounds.Contains(pt)) {
				result.HitTest = HitTest.Client;
				result.Bounds = ClientBounds;
				return result;
			}
			if(TabPanelBounds.Contains(pt)) {
				result.HitTest = HitTest.TabPanel;
				result.Bounds = TabPanelBounds;
				if(PrevTabButton.Bounds.Contains(pt)) {
					result.SetButton(PrevTabButton, HitTest.PrevTabButton);
					return result;
				}
				if(NextTabButton.Bounds.Contains(pt)) {
					result.SetButton(NextTabButton, HitTest.NextTabButton);
					return result;
				}
				tabButtons = new ArrayList();
				for(int i = FirstVisibleTabIndex; i < TabsBounds.Length; i++) {
					if(TabsBounds[i].Contains(pt)) {
						result.HitTest = HitTest.Tab;
						result.Bounds = TabsBounds[i];
						result.Tab = this[i].Panel;
						DockPanelCaptionButton tabButton = new DockPanelCaptionButton();
						tabButton.Bounds = result.Bounds;
						result.SetButton(tabButton, result.HitTest);
						tabButtons.Add(tabButton);
						break;
					}
					else {
						tabButtons.Add(new DockPanelCaptionButton());
					}
				}
				return result;
			}
			return result;
		}
		protected override void UpdateLayoutOnResize() {
			if(Panel.ParentAutoHideControl != null) {
				AdjustControlBounds();
				LayoutManager.SetAutoHideControlBounds(Size);
			}
			else {
				Panel.BeforeUpdateLayoutOnResize();
				try {
					base.UpdateLayoutOnResize();
				}
				finally {
					Panel.AfterUpdateLayoutOnResize();
				}
			}
		}
		protected override bool IsDesignMode {
			get { return panel != null && panel.Site != null && panel.Site.DesignMode; }
		}
		protected internal virtual void UpdateOnChangeAutoHide() {
			BeginRefresh();
			try {
				if(!AutoHide)
					DockManager.ActivePanel = Panel;
			}
			finally {
				EndRefresh();
			}
		}
		protected internal void Panel_OnResize() {
			if(LayoutParent != null)
				LayoutParent.UpdateFloatSize(this);
			LayoutChanged();
		}
		protected virtual void UpdateFloatSize(DockLayout dockLayout) {
			if(RootLayout.Dock != DockingStyle.Float) return;
			dockLayout.FloatSize = (Tabbed ? Size : dockLayout.Size);
		}
		protected internal void UpdateView() {
			if(LayoutParent != null) {
				LayoutParent.LayoutChanged();
			}
			else if(AutoHide && !IsDeserializing) LayoutManager.AutoHideContainers.LayoutChanged();
			else LayoutChanged();
		}
		protected internal virtual void RefreshCaption() {
			if(IsTab)
				LayoutParent.RefreshCaption();
			if(!DockLayoutUtils.CanDraw(null, CaptionBounds) || IsRefreshLocked) return;
			int captionHeight = CaptionHeight;
			CalcCaptionAppearance();
			Panel.Invalidate();
			UpdateCaptionHeight(captionHeight);
		}
		void UpdateCaptionHeight(int captionHeight) {
			if(captionHeight != CaptionHeight) {
				if(!CanCalcViewInfo) return;
				this.cachedSizeCore = Size.Empty;
				CalcViewRects();
				CalcCaptionInfo();
				CalcTabPanelInfo();
				CalcResizeZones();
				CalcDockZones();
				Panel.SuspendLayout();
				try {
					AdjustControlBounds();
				}
				finally {
					Panel.ResumeLayout(false);
				}
			}
		}
		protected internal virtual void Draw(PaintEventArgs e, DockElementsPainter painter) {
			if(IsRefreshLocked) return;
			using(cache = new GraphicsCache(e)) {
				DrawCaption(painter);
				DrawBorder(painter);
				DrawTabPanel(painter);
			}
			cache = null;
		}
		protected internal virtual bool GetAllowGlyphSkinning() {
			return Panel != null && Panel.GetAllowGlyphSkinning();
		}
		protected internal virtual VerticalTextOrientation GetAutoHidePanelVerticalTextOrientation() {
			if(DockManager == null || DockManager.DockingOptions == null) return VerticalTextOrientation.Default;
			return DockManager.DockingOptions.AutoHidePanelVerticalTextOrientation;
		}
		protected internal virtual Image GetActualCaptionImage() {
			if(HasImageUri)
				return ImageUri.GetImage();
			if(IsImagePropertyChanged)
				return Image;
			return null;
		}
		protected virtual void DrawCaption(DockElementsPainter painter) {
			if(!DockLayoutUtils.CanDraw(cache.PaintArgs.PaintArgs, CaptionBounds)) return;
			if(Float) {
				DrawApplicationCaptionArgs args = new DrawApplicationCaptionArgs(cache, CaptionBounds, CaptionAppearance, CaptionTextBounds, CaptionImageBounds,
					 Caption, IsFloatFormActive, GetActualCaptionImage(),Images, ImageIndex);
				args.AllowGlyphSkinning = GetAllowGlyphSkinning();
				Painter.DrawApplicationCaption(args);
			}
			else {
				DrawWindowCaptionArgs args = new DrawWindowCaptionArgs(cache, CaptionBounds, CaptionAppearance, Caption,
					CaptionTextBounds, CaptionImageBounds, Panel.CanDrawCaptionActive, GetActualCaptionImage(), Images, ImageIndex);
				args.AllowGlyphSkinning = GetAllowGlyphSkinning();
				painter.DrawWindowCaption(args);
			}
			ObjectPainter.DrawObject(cache, ((DevExpress.XtraEditors.ButtonPanel.IButtonsPanelOwner)Panel).GetPainter(),
				(ObjectInfoArgs)Panel.ButtonsPanel.ViewInfo);
		}
		protected internal ObjectState GetObjectStateByCaptionButton(DockPanelCaptionButton button) {
			ObjectState result = ObjectState.Normal;
			if(button.Hot) result = ObjectState.Hot;
			if(button.Pressed) result = ObjectState.Pressed;
			return result;
		}
		protected virtual void DrawTabPanel(DockElementsPainter painter) {
			if(!DockLayoutUtils.CanDraw(cache.PaintArgs.PaintArgs, TabPanelBounds) || !Tabbed) return;
			painter.DrawTabPanel(new DrawTabPanelArgs(cache, this, IsTabsButtonsVisible, ObjectState.Normal));
		}
		protected virtual void DrawBorder(DockElementsPainter painter) {
			if(!HasBorder) return;
			DrawBorderArgs args = new DrawBorderArgs(cache, BorderBounds, Parameters.GetBorderPen(), CaptionAppearance.BackColor, Float ? IsFloatFormActive : Panel.CanDrawCaptionActive, Float);
			painter.WindowPainter.DrawBorder(args);
		}
		protected internal void DrawTabDockPointer(DockLayout insertLayout) {
			Painter.TabPanelPainter.DrawTabDockPointer(insertLayout, this);
		}
		public SavedDockPanelInfo GetSavedInfo() {
			SavedDockPanelInfo result = new SavedDockPanelInfo();
			if(!Saved && !IsDeserializing) return result;
			result.Assign(SavedInfo);
			return result;
		}
		public void SetSavedInfo(SavedDockPanelInfo savedInfo) {
			savedInfo.AssignTo(SavedInfo);
			if(SavedInfo.SavedParent == null) {
				SavedInfo.SavedParent = InnerManager;
			}
		}
		protected internal virtual void OnDeserialized() {
			if(Visible && Dock != DockingStyle.Float) SavedInfo.Clear();
			if(!HasChildren || Tabbed || ActiveChild != null) return;
			LockDeserialize();
			try {
				for(int i = 0; i < this.Count; i++) {
					LayoutInfo layout = this[i] as LayoutInfo;
					if(layout != null && panel != null) {
						Size clientSize = panel.AllowChangeBounds && panel.IsHandleCreated ? panel.DockLayout.ClientSize : panel.ClientSize;
						SetChildFactorSize(layout, layout.Size, clientSize);
					}
				}
			}
			finally {
				UnlockDeserialize();
				UpdateSavedSizeFactors();
			}
		}
		protected override bool CanUpdateChildrenSizeFactor { get { return base.CanUpdateChildrenSizeFactor && !IsDeserializing; ; } }
		public override Rectangle Bounds {
			get {
				if((Index < 0 && Panel != null && Panel.Visibility != DockVisibility.AutoHide) || Index < 0 && Panel == null) return Rectangle.Empty;
				if(panel.FloatForm != null)
					return new Rectangle(Point.Empty, panel.FloatForm.ClientSize);
				Docking2010.DocumentContainer container = panel.Parent as Docking2010.DocumentContainer;
				if(container != null)
					return new Rectangle(Point.Empty, container.ClientSize);
				return base.Bounds;
			}
		}
		protected internal virtual DockZone GetDockZoneAtPos(Point ptScreen) {
			return DockZones[ptScreen];
		}
		protected internal DockZone GetDockZoneByDockStyle(DockingStyle ds) {
			return DockZones[ds];
		}
		protected internal DockZone GetDockZoneByType(Type type) {
			return DockZones[type];
		}
		protected ResizeZoneCollection ResizeZones { get { return resizeZones; } }
		protected internal DockZoneCollection DockZones { get { return dockZones; } }
		protected virtual string Caption {
			get {
				if(ActiveChild != null) {
					if(ActiveChild.Panel.Text != string.Empty)
						return ActiveChild.Panel.Text;
					return ActiveChild.TabText;
				}
				return Panel.Text;
			}
		}
		public bool Float { get { return Dock == DockingStyle.Float; } }
		protected virtual int CaptionHeight { get { return (HasCaption ? Math.Max(GetMinButtonPanelHeight(), Painter.GetCaptionHeight(CaptionAppearance, Float)) : 0); } }
		protected virtual int TabPanelHeight { get { return (HasTabPanel ? Painter.TabPanelPainter.GetTabsHeight(this, TabsAppearance, ActiveTabAppearance) : 0); } }
		int GetMinButtonPanelHeight() {
			WindowSkinPainter skinPainter = Painter.WindowPainter as WindowSkinPainter;
			int minCaptionHeight = Math.Max(GetMinButtonPanelSize().Height, CaptionImageSize.Height);
			if(skinPainter == null) return minCaptionHeight;
			return skinPainter.GetCaptionBoundsByClientRectangle(new Rectangle(0, 0, 0, minCaptionHeight), Float).Height;
		}
		protected virtual bool HasBorder {
			get {
				if(IsMdiDocument)
					return false;
				if(LayoutParent == null && Dock != DockingStyle.Float) {
					if(HasChildren) return Tabbed;
					return true;
				}
				return HasCaptionAndBorderCore;
			}
		}
		protected virtual bool HasCaption {
			get {
				if(IsMdiDocument)
					return false;
				if(!HasCaptionAndBorderCore) return false;
				if(DockManager != null && DockManager.DockingOptions.ShowCaptionOnMouseHover)
					return LayoutManager.CaptionMouseHoverHelper.CanShowCaption(Panel);
				return true;
			}
		}
		protected bool HasCaptionAndBorderCore {
			get {
				if(IsMdiDocument) return false;
				if(Float) return true;
				if(IsTab) return (Panel.ParentAutoHideControl != null);
				return (!HasChildren || Tabbed);
			}
		}
		public override bool IsMdiDocument {
			get { return Panel.IsMdiDocument; }
		}
		public override bool CanDockPanelInCaptionRegion() {
			return (DockManager != null) && DockManager.CanDockPanelInCaptionRegion();
		}
		public override bool DockAsMdiDocument() {
			return Panel.DockAsMdiDocument();
		}
		new protected DockLayout ParentAutoHide { get { return (base.ParentAutoHide as DockLayout); } }
		protected internal virtual bool CanActivate {
			get {
				if(RootLayout.AutoHide) return true;
				return (Manager != null);
			}
		}
		protected DockElementsParameters Parameters { get { return DockManager.CurrentController.PaintStyle.ElementsParameters; } }
		protected DockManagerAppearances AppearancesSettings { get { return DockManager.CurrentController.AppearancesDocking; } }
		DockElementsPainter Painter { get { return Panel.Painter; } }
		int TabVertBackIndent { get { return Painter.TabPanelPainter.TabVertBackIndent; } }
		int TabVertForeIndent { get { return Painter.TabPanelPainter.TabVertForeIndent; } }
		int TabHorzTextIndent { get { return Painter.TabPanelPainter.TabHorzTextIndent; } }
		int TabHorzBackIndent { get { return Painter.TabPanelPainter.TabHorzBackIndent; } }
		protected virtual bool HasCloseButton {
			get {
				if(Tabbed && ActiveChild != null && !ActiveChild.IsLoading)
					return ActiveChild.HasCloseButton;
				return (DockManager.DockingOptions.ShowCloseButton && ShowCloseButton);
			}
		}
		protected virtual bool HasMaxButton {
			get {
				if(!DockManager.DockingOptions.ShowMaximizeButton || !ShowMaximizeButton) return false;
				if(Panel != null && Panel.Dock == DockingStyle.Float) return true;
				return (!AutoHide && LayoutParent != null && !IsTab);
			}
		}
		protected virtual bool HasAutoHideButton {
			get {
				if(!DockManager.DockingOptions.ShowAutoHideButton || !ShowAutoHideButton) return false;
				if(RootLayout.Float) return false;
				if(AutoHide) return true;
				return !IsParentAutoHide;
			}
		}
		protected virtual bool CanShowCaptionImage {
			get {
				if(ActiveChild != null &&
					ActiveChild != this 
					)
					return ActiveChild.CanShowCaptionImage;
				return DockManager != null && DockManager.DockingOptions.ShowCaptionImage && HasImage;
			}
		}
		protected virtual bool HasTabPanel { get { return (Tabbed && Count > 0 && !AutoHide); } }
		protected Size CaptionButtonSize { get { return Painter.ButtonSize; } }
		protected Size CaptionImageSize {
			get {
				if(CanShowCaptionImage)
					return HasImage ? ImageSize : Size.Empty;
				return Size.Empty;
			}
		}
		public new DockLayout LayoutParent { get { return Parent as DockLayout; } }
		public new DockLayout RootLayout { get { return base.RootLayout as DockLayout; } }
		public AppearanceObject CaptionAppearance { get { return captionAppearance; } }
		public AppearanceObject TabsAppearance { get { return tabsAppearance; } }
		public AppearanceObject TabsAppearanceHot { get { return tabsAppearanceHot; } }
		public AppearanceObject ActiveTabAppearance { get { return activeTabAppearance; } }
		AppearanceObject paintAppearanceCore = new FrozenAppearance();
		protected internal AppearanceObject HideBarPaintAppearance { get { return paintAppearanceCore; } }
		public override Rectangle ClientBounds { get { return clientBounds; } }
		public Rectangle CaptionBounds { get { return captionBounds; } }
		public Rectangle BorderBounds { get { return borderBounds; } }
		public Rectangle CaptionTextBounds { get { return captionTextBounds; } }
		public Rectangle CaptionImageBounds { get { return captionImageBounds; } }
		public Rectangle[] TabsBounds { get { return tabsBounds; } }
		public Rectangle TabPanelBounds { get { return tabPanelBounds; } }
		public DockPanelCaptionButton PrevTabButton { get { return prevTabButton; } }
		public DockPanelCaptionButton NextTabButton { get { return nextTabButton; } }
		public ArrayList TabButtons { get { return tabButtons; } }
		public TabsPosition TabsPosition {
			get { return tabsPosition; }
			set {
				if(TabsPosition == value) return;
				TabsPosition oldValue = TabsPosition;
				tabsPosition = value;
				LayoutChanged();
				Panel.RaiseTabsPositionChanged(oldValue);
			}
		}
		public bool TabsScroll {
			get { return tabsScroll; }
			set {
				if(TabsScroll == value) return;
				tabsScroll = value;
				LayoutChanged();
				Panel.RaiseTabsScrollChanged();
			}
		}
		public virtual string TabText {
			get {
				if(Panel.TabText != string.Empty) return Panel.TabText;
				return Panel.Text;
			}
		}
		public override bool FloatVertical {
			get { return base.FloatVertical; }
			set {
				if(FloatVertical == value) return;
				base.FloatVertical = value;
				LayoutChanged();
			}
		}
		public override DefaultBoolean DockVertical {
			get { return base.DockVertical; }
			set {
				if(DockVertical == value) return;
				base.DockVertical = value;
				LayoutChanged();
			}
		}
		protected bool ShowAutoHideButton { get { return Panel.Options.ShowAutoHideButton; } }
		protected bool ShowMaximizeButton { get { return Panel.Options.ShowMaximizeButton; } }
		protected bool ShowCloseButton { get { return Panel.Options.ShowCloseButton; } }
		public int FirstVisibleTabIndex {
			get {
				if(firstVisibleTabIndex > 0 && !TabsScroll)
					firstVisibleTabIndex = 0;
				return firstVisibleTabIndex;
			}
		}
		protected internal virtual void IncFirstVisibleTabIndex() {
			this.firstVisibleTabIndex++;
			LayoutChanged();
		}
		protected internal virtual void DecFirstVisibleTabIndex() {
			this.firstVisibleTabIndex--;
			LayoutChanged();
		}
		void CheckDecreaseFirstVisibleTabIndex() {
			if(FirstVisibleTabIndex == 0) return;
			if(FirstVisibleTabIndex > Count - 1)
				this.firstVisibleTabIndex = Math.Max(0, Count - 1);
			bool isHorz = !IsVerticalTabsPosition;
			DirectionSize dSize = new DirectionSize(TabPanelBounds.Size, isHorz);
			dSize.Width -= 2 * TabHorzBackIndent;
			int i = Count - 1;
			for(; i > -1; i--) {
				if(!this[i].IsValid) continue;
				dSize.Width -= GetChildTabWidth(i);
				if(dSize.Width < 0) {
					i++;
					break;
				}
			}
			this.firstVisibleTabIndex = Math.Min(FirstVisibleTabIndex, Math.Max(0, i));
		}
		Image imageCore;
		protected internal bool IsImagePropertyChanged {
			get {
				if(ActiveChild != null && imageCore == null)
					return ActiveChild.IsImagePropertyChanged;
				return imageCore != null;
			}
		}
		protected internal Size ImageSize {
			get {
				if(HasImageUri)
					return ImageUri.GetImage().Size;
				if(IsImagePropertyChanged)
					return Image.Size;
				if(ImageIndex != LayoutConsts.InvalidIndex)
					return ImageCollection.GetImageListSize(Images);
				return Size.Empty;
			}
		}
		protected internal bool HasImage {
			get { return IsImagePropertyChanged || HasImageUri || ImageCollection.IsImageListImageExists(Images, ImageIndex); }
		}
		public Image Image {
			get {
				Image result = GetActualImage();
				if(result == null && ActiveChild != null && ActiveChild != this)
					return ActiveChild.Image;
				return result;
			}
			set {
				if(imageCore == value) return;
				imageCore = value;
				UpdateView();
			}
		}
		DxImageUri imageUriCore = new DxImageUri();
		public DxImageUri ImageUri {
			get {
				DxImageUri result = GetActualImageUri();
				if(result == null && ActiveChild != null && ActiveChild != this)
					return ActiveChild.ImageUri;
				return result;
			}
			set {
				if(imageUriCore == value) return;
				imageUriCore = value;
				UpdateView();
			}
		}
		public bool HasImageUri { get { return ImageUri != null ? ImageUri.HasImage : false; } }
		DefaultBoolean allowCustomHeaderButtonsGlyphSkinning = DefaultBoolean.Default;
		public DefaultBoolean AllowCustomHeaderButtonsGlyphSkinning {
			get { return allowCustomHeaderButtonsGlyphSkinning; }
			set {
				if(AllowCustomHeaderButtonsGlyphSkinning == value)
					return;
				allowCustomHeaderButtonsGlyphSkinning = value;
				if(Panel != null && Panel.IsHandleCreated)
					Panel.Invalidate();
			}
		}
		internal bool ShouldSerializeImage() {
			return imageCore != null && GetActualImage() == imageCore;
		}
		internal void ResetImage() {
			Image = null;
		}
		Image GetActualImage() {
			if(ActiveChild != null) return null;
			if(imageCore != null) return imageCore;
			return ImageCollection.GetImageListImage(Images, ImageIndex);
		}
		internal bool ShouldSerializeImageUri() {
			return imageUriCore != null && GetActualImageUri() == imageUriCore;
		}
		internal void ResetImageUri() {
			ImageUri = null;
		}
		DxImageUri GetActualImageUri() {
			if(ActiveChild != null) return null;
			return imageUriCore;
		}
		public int ImageIndex {
			get {
				if(ActiveChild != null && ActiveChild != this)
					return ActiveChild.ImageIndex;
				return imageIndex;
			}
			set {
				if(ImageIndex == value) return;
				imageIndex = value;
				UpdateView();
			}
		}
		internal bool ShouldSerializeImageIndex() {
			return imageIndex != LayoutConsts.InvalidIndex;
		}
		internal void ResetImageIndex() {
			ImageIndex = LayoutConsts.InvalidIndex;
		}
		bool IsVerticalTabsPosition { get { return DockLayoutUtils.IsVerticalPosition(TabsPosition); } }
		public Size FloatSize {
			get { return floatSize; }
			set {
				if(FloatSize == value) return;
				floatSize = value;
			}
		}
		public int LastVisibleTabIndex {
			get {
				if(ValidChildCount <= 1)
					return FirstVisibleTabIndex;
				bool isHorz = !IsVerticalTabsPosition;
				DirectionRectangle dTabPanel = new DirectionRectangle(TabPanelBounds, isHorz);
				DirectionRectangle dPrevButton = new DirectionRectangle(PrevTabButton.Bounds, isHorz);
				int i = FirstVisibleTabIndex;
				for(; i < Count; i++) {
					if(!this[i].IsValid) continue;
					DirectionRectangle dRect = new DirectionRectangle(TabsBounds[i], isHorz);
					if(
						dRect.Right > dTabPanel.Right - TabHorzBackIndent) break;
				}
				if(i > FirstVisibleTabIndex) i--;
				return i;
			}
		}
		protected internal override BaseLayoutInfoManager InnerManager { get { return DockManager == null ? null : DockManager.LayoutManager; } }
		protected DockLayoutManager LayoutManager { get { return InnerManager as DockLayoutManager; } }
		public object Images { get { return DockManager == null ? null : DockManager.Images; } }
		internal protected bool IsTabsButtonsVisible { get { return TabsScroll && (!PrevTabButton.Disabled || !NextTabButton.Disabled); } }
		public DockPanel Panel { get { return panel; } }
		new public DockLayout ActiveChild { get { return base.ActiveChild as DockLayout; } set { base.ActiveChild = value; } }
	}
	public class DockLayoutManager : BaseLayoutInfoManager, IDockZonesOwner, IDockTarget, IDisposable {
		DockManager dockManager;
		DockZoneCollection dockZones;
		AutoHideContainerCollection autoHideContainers;
		int updateZOrderCounter, updateAutoHideContainersCollectionCounter;
		AutoHideMoveHelper autoHideMoveHelper;
		CaptionMouseHoverHelper captionMouseHoverHelper;
		int cachedAutoHidePanelSize;
		public DockLayoutManager(DockManager dockManager) {
			this.dockManager = dockManager;
			this.autoHideMoveHelper = CreateAutoHideMoveHelper();
			this.captionMouseHoverHelper = CreateCaptionMouseHoverHelper();
			this.dockZones = new DockZoneCollection();
			this.autoHideContainers = new AutoHideContainerCollection(this);
		}
		public virtual void Dispose() {
			this.autoHideMoveHelper.Dispose();
			this.captionMouseHoverHelper.Dispose();
			DockZones.Clear();
		}
		internal new DockLayout this[int index] { get { return base[index] as DockLayout; } }
		protected override LayoutInfo CreateLayout(DockingStyle dock) {
			DockPanel resultPanel = CreateDockPanel(true, dock);
			return resultPanel.DockLayout;
		}
		protected virtual AutoHideMoveHelper CreateAutoHideMoveHelper() {
			return new AutoHideMoveHelper(this);
		}
		protected virtual CaptionMouseHoverHelper CreateCaptionMouseHoverHelper() {
			return new CaptionMouseHoverHelper(this);
		}
		protected internal DockZone GetDockZoneByDockStyle(DockingStyle ds) {
			return DockZones[ds];
		}
		protected internal DockZone GetDockZoneByDockStyle(DockingStyle ds, int level) {
			int counter = 0;
			for(int i = 0; i < DockZones.Count; i++) {
				DockZone zone = DockZones[i];
				if(zone.DockStyle == ds) {
					LayoutManagerDockZone mzone = zone as LayoutManagerDockZone;
					if(mzone != null) {
						if(counter == level)
							return mzone;
						else
							counter++;
					}
				}
			}
			return null;
		}
		protected internal DockZone GetDockZoneByDockStyle(DockingStyle ds, Rectangle rect) {
			for(int i = 0; i < DockZones.Count; i++) {
				DockZone zone = DockZones[i];
				if(zone.DockStyle == ds) {
					if(rect.Contains(zone.Bounds))
						return zone;
				}
			}
			return null;
		}
		protected internal DockZone GetDockZoneByType(Type type) {
			return DockZones[type];
		}
		internal DockPanel CreateDockPanel(bool createControlContainer, DockingStyle dock) { return DockManager.CreateDockPanel(createControlContainer, dock); }
		protected internal void Deserialize() {
			Clear();
			BeginUpdateZOrder();
			LockUpdateAutoHideContainersCollection();
			try {
				DeserializeCollection(DockManager.RootPanels);
				DeserializeCollection(DockManager.HiddenPanels);
				DeserializeAutoHideContainers();
			}
			finally {
				UnlockUpdateAutoHideContainersCollection();
				CancelUpdateZOrder();
			}
		}
		void DeserializeCollection(DockPanelCollection panels) {
			for(int i = 0; i < panels.Count; i++) {
				if(panels[i].IsDisposed) continue;
				if(panels[i].Visibility == DockVisibility.Hidden)
					InvisibleCollection.OnLayoutInfoVisibleChanged(panels[i].DockLayout);
				else
					AddLayoutCore(panels[i].DockLayout);
			}
			for(int i = 0; i < panels.Count; i++) {
				panels[i].OnDeserialize(this);
			}
		}
		void DeserializeAutoHideContainers() {
			AutoHideInfoCollection.Clear();
			AutoHideContainers.Deserialize(this);
			CheckAutoHidePanelSize(true);
		}
		protected virtual void CalcDockZones() {
			DockZones.Clear();
			DockLayoutUtils.CalcLayoutDockZones(this, Parent as IDockZonesOwner);
		}
		void IDockZonesOwner.CalcDockZones(DockingStyle dock) {
			if(dock == DockingStyle.Fill || !CanAddLayout(dock)) return;
			DockZones.Add(new LayoutManagerDockZone(this, dock, 0, null));
			for(int i = 0; i < Count; i++) {
				DockLayout dockLayout = this[i];
				if(dockLayout.Dock != dock) continue;
				if(DockManager.DockMode == DockMode.Standard) DockZones.Add(new LayoutManagerDockZone(this, dock, (i == Count - 1 ? Math.Max(0, i - 1) : i), dockLayout));
				else DockZones.Add(new LayoutManagerDockZone(this, dock, i, dockLayout));
				DockZones.Add(new LayoutManagerDockZone(this, dock, i + 1, dockLayout));
			}
			DockZones.Add(new LayoutManagerDockZone(this, dock, Count, null));
		}
		bool IDockZonesOwner.CanCreateDockZoneReference(DockZone dockZone) {
			return ((IDockZonesOwner)this).ScreenRectangle.IntersectsWith(dockZone.Bounds);
		}
		int IDockZonesOwner.GetSourceDockSize(DockLayout source, DockingStyle dock) {
			DirectionSize dSize = new DirectionSize(source.OriginalSize, LayoutRectangle.GetIsHorizontal(dock));
			DirectionSize dRestSize = new DirectionSize(GetRestBounds(null).Size, LayoutRectangle.GetIsHorizontal(dock));
			if(source.Parent == this) {
				if(dRestSize.Width < 0)
					dSize.Width += dRestSize.Width;
				else if(dSize.Width > dRestSize.Width && LayoutRectangle.GetIsHorizontal(source.Dock) != LayoutRectangle.GetIsHorizontal(dock))
					dSize.Width = dRestSize.Width;
			}
			else if(dSize.Width > dRestSize.Width)
				dSize.Width = dRestSize.Width;
			return dSize.Width;
		}
		DockZoneCollection IDockZonesOwner.DockZones { get { return DockZones; } }
		Rectangle IDockZonesOwner.ScreenRectangle {
			get {
				if(DockManager.Form == null || DockManager.Form.Disposing) return LayoutConsts.InvalidRectangle;
				return DockManager.Form.RectangleToScreen(ClientBounds);
			}
		}
		Control IDockTarget.Target { get { return DockManager.Form; } }
		protected internal override bool CanAddLayout(DockingStyle dock) {
			return (dock != DockingStyle.Fill);
		}
		protected override void OnInsertComplete(int index, object value) {
			base.OnInsertComplete(index, value);
			CheckMaxSize((DockLayout)value);
			CheckDecreaseSize();
		}
		void CheckMaxSize(DockLayout dockLayout) {
			if(!DockManager.IsInitialized || dockLayout.Dock == DockingStyle.Fill || dockLayout.Float) return;
			int size = (this as IDockZonesOwner).GetSourceDockSize(dockLayout, dockLayout.Dock);
			DirectionSize dSize = new DirectionSize(dockLayout.Size, dockLayout.IsHorizontal);
			if(dSize.Width > size) {
				dSize.Width = size;
				dockLayout.Size = dSize.Size;
			}
		}
		protected internal override void SetChildIndex(LayoutInfo info, int index) {
			base.SetChildIndex(info, index);
			CheckMaxSize((DockLayout)info);
		}
		protected internal virtual void BeforeSerialize(System.ComponentModel.Design.Serialization.IDesignerSerializationManager manager) {
			AutoHideMoveHelper.StopAutoHide();
		}
		protected internal virtual void OnDockingOptionsChanged(BaseOptionChangedEventArgs e) {
			CaptionMouseHoverHelper.EnableMouseHoving(DockManager.DockingOptions.ShowCaptionOnMouseHover);
			LayoutChanged();
		}
		protected internal virtual void FormSizeChanged() {
			try {
				FormResizing = true;
				AutoHideMoveHelper.FormSizeChanged();
				CheckDecreaseSize();
			}
			finally {
				FormResizing = false;
			}
		}
		protected internal void CheckDecreaseSize() {
			if(IsFormIconic) return;
			BeginUpdate();
			try {
				Size freePlaceSize = GetRestBounds(null).Size;
				if(freePlaceSize.Width < 0)
					DecreaseSize(-freePlaceSize.Width, DockingStyle.Left, DockingStyle.Right);
				if(freePlaceSize.Height < 0)
					DecreaseSize(-freePlaceSize.Height, DockingStyle.Top, DockingStyle.Bottom);
			}
			finally {
				EndUpdate();
			}
		}
		protected internal override void InsertLayoutCore(int index, LayoutInfo info) {
			try {
				base.InsertLayoutCore(index, info);
			}
			finally {
			}
		}
		protected override bool IsLayoutLocked { get { return (base.IsLayoutLocked || Disposing || !DockManager.IsInitialized); } }
		protected override void LayoutChangedCore() {
			base.LayoutChangedCore();
			AutoHideContainers.LayoutChanged();
			AutoHideMoveHelper.OnLayoutChanged();
			CalcDockZones();
			DockManager.Form.SuspendLayout();
			foreach(BaseLayoutInfo info in this)
				info.LayoutChanged();
			DockManager.Form.ResumeLayout();
		}
		protected override void SetParent(object info, bool assignThis) {
			base.SetParent(info, assignThis);
			DockLayout dockLayout = info as DockLayout;
			if(dockLayout.Panel.IsDisposing) return;
			if(assignThis) DockManager.BeginFormLayoutUpdate();
			try {
				if(dockLayout.Dock != DockingStyle.Float && !dockLayout.IsMdiDocument) {
					dockLayout.Panel.SetParent(assignThis ? DockManager.Form : null);
					dockLayout.Panel.ControlDock = (DockStyle)dockLayout.Dock;
				}
				else {
					if(assignThis) {
						if(DockManager.IsDeserializing) dockLayout.Panel.AssignDockManager(this);
						dockLayout.Panel.MakeFloatCore();
					}
					else {
						FloatForm floatForm = dockLayout.Panel.FloatForm;
						dockLayout.Panel.SetParent(null);
						DestroyFloatForm(floatForm);
						dockLayout.SetInvisiblePanelLocation();
					}
				}
				if(assignThis) {
					OnChildIndexChanged(dockLayout);
					dockLayout.Panel.AssignDockManager(this);
				}
				DockManager.UpdateRootPanels();
				dockLayout.AdjustControlBounds();
			}
			finally {
				if(assignThis) DockManager.EndFormLayoutUpdate();
			}
		}
		void DestroyFloatForm(FloatForm floatForm) {
			if(floatForm == null) return;
			Form prevActiveForm = Form.ActiveForm;
			if(prevActiveForm == floatForm)
				prevActiveForm = DockManager.OwnerForm;
			floatForm.Owner = null;
			floatForm.Dispose();
			if(prevActiveForm != null && !prevActiveForm.InvokeRequired)
				prevActiveForm.Activate();
		}
		protected internal void FireChanged() { DockManager.FireChanged(); }
		protected override void OnChildIndexChanged(LayoutInfo info) {
			if(info.Dock != DockingStyle.Float)
				UpdateControlsZOrder();
			DockManager.UpdateRootPanels();
		}
		protected internal void BeginUpdateZOrder() { this.updateZOrderCounter++; }
		protected void CancelUpdateZOrder() { this.updateZOrderCounter--; }
		protected internal void EndUpdateZOrder() {
			CancelUpdateZOrder();
			UpdateControlsZOrder();
		}
		protected virtual void UpdateControlsZOrder() {
			if(IsZOrderLocked) return;
			DockManager.UpdateControlsZOrder();
		}
		protected bool IsZOrderLocked { get { return updateZOrderCounter != 0; } }
		public DockPanel GetDockPanelAtPos(Point ptScreen) {
			DockLayout result = null;
			for(int i = 0; i < GetCount(DockingStyle.Float); i++) {
				DockLayout dockLayout = (DockLayout)this[DockingStyle.Float, i];
				if(!dockLayout.IsValid) continue;
				if(dockLayout.Panel.FloatForm != null) {
					if(dockLayout.Panel.FloatForm.Bounds.Contains(ptScreen)) {
						FloatForm floatForm = DockLayoutUtils.GetFloatFormAtPos(ptScreen);
						if(floatForm != null && floatForm.DockManager == DockManager)
							result = floatForm.FloatLayout;
						break;
					}
				}
			}
			if(result == null)
				result = AutoHideMoveHelper.GetDockLayoutAtPos(ptScreen);
			if(result == null) {
				foreach(DockLayout dockLayout in this) {
					if(!(dockLayout.Visible && dockLayout.Panel.IsPointInside(ptScreen))) continue;
					result = dockLayout;
				}
			}
			return (result == null ? null : result.Panel.GetPanelAtPos(ptScreen));
		}
		protected internal DockPanel GetDockPanelAtPosBut(Point ptScreen, DockPanel dpanel) {
			DockLayout result = null;
			for(int i = 0; i < GetCount(DockingStyle.Float); i++) {
				DockLayout dockLayout = (DockLayout)this[DockingStyle.Float, i];
				if(!dockLayout.IsValid) continue;
				if(dockLayout.Panel.FloatForm != null && dockLayout.Panel.FloatForm.Bounds.Contains(ptScreen)) {
					FloatForm floatForm = DockLayoutUtils.GetFloatFormAtPos(ptScreen);
					if(floatForm != null && floatForm != dpanel.Parent && floatForm.DockManager == DockManager)
						result = floatForm.FloatLayout;
					break;
				}
			}
			if(result == null) {
				foreach(DockLayout dockLayout in this) {
					if(dockLayout.Panel.DockedAsTabbedDocument) continue;
					if(dockLayout.Visible && dockLayout.Panel.IsPointInside(ptScreen)) {
						if(dockLayout.Panel != dpanel) result = dockLayout;
						else {
							if(dockLayout.TabsBounds == null) continue;
							Point ptClient = dockLayout.Panel.PointToClient(ptScreen);
							for(int i = 0; i < dockLayout.TabsBounds.Length; i++) {
								if(dockLayout.TabsBounds[i].Contains(ptClient))
									result = dockLayout.ActiveChild;
							}
						}
					}
				}
			}
			if(result == null)
				result = AutoHideMoveHelper.GetDockLayoutAtPos(ptScreen);
			return result == null ? null : result.Panel;
		}
		internal void SetAutoHideControlBounds(Size newSize) {
			AutoHideMoveHelper.SetAutoHideControlBounds(newSize);
		}
		internal DockPanel[] GetRootPanelsArray() {
			DockPanel[] result = new DockPanel[Count];
			for(int i = 0; i < Count; i++)
				result[i] = this[i].Panel;
			return result;
		}
		internal DockPanel[] GetHiddenPanelsArray() {
			DockPanel[] result = new DockPanel[InvisibleCount];
			for(int i = 0; i < InvisibleCount; i++)
				result[i] = (GetInvisibleLayout(i) as DockLayout).Panel;
			return result;
		}
		protected internal virtual void ShowImmediately(DockLayout dockLayout) {
			AutoHideMoveHelper.ShowImmediately(dockLayout);
		}
		protected internal virtual void HideImmediately() {
			AutoHideMoveHelper.HideImmediately();
		}
		void LockUpdateAutoHideContainersCollection() { this.updateAutoHideContainersCollectionCounter++; }
		void UnlockUpdateAutoHideContainersCollection() { this.updateAutoHideContainersCollectionCounter--; }
		protected bool IsUpdateAutoHideContainersCollectionLocked { get { return (this.updateAutoHideContainersCollectionCounter != 0); } }
		protected virtual void UpdateAutoHideContainersCollection() {
			if(IsUpdateAutoHideContainersCollectionLocked) return;
			BeginUpdateZOrder();
			try {
				DockManager.Form.SuspendLayout();
				DockManager.IsCreatingAutohideControl = false;
				for(int i = 0; i < AutoHideInfoCollection.Count; i++) {
					AutoHideInfo info = AutoHideInfoCollection[i];
					if(AutoHideContainers[info.Dock] == null)
						CreateAutoHideContainer(info);
				}
				for(int i = 0; i < AutoHideContainers.Count; i++) {
					AutoHideContainer container = AutoHideContainers[i];
					if(AutoHideInfoCollection[(DockingStyle)container.Dock] == null)
						DestroyAutoHideContainer(container);
				}
			}
			finally {
				EndUpdateZOrder();
				DockManager.Form.ResumeLayout();
				DockManager.IsCreatingAutohideControl = true;
			}
			AutoHideContainers.LayoutChanged();
		}
		void CreateAutoHideContainer(AutoHideInfo info) {
			AutoHideContainer container = DockManager.InternalCreateAutoHideContainer((DockStyle)info.Dock);
			AutoHideContainers.Add(container);
			DockManager.Form.Controls.Add(container);
			foreach(DockLayout dockLayout in info) {
				dockLayout.Panel.SetParent(container);
			}
			container.ContainerSize = CalcAutoHidePanelSize(container);
			DockManager.RaiseCreateAutoHideContainer(new AutoHideContainerEventArgs(container));
		}
		internal void DestroyAutoHideContainer(AutoHideContainer container) {
			DockManager.RaiseDestroyAutoHideContainer(new AutoHideContainerEventArgs(container));
			container.Dispose();
		}
		protected internal override void OnLayoutInfoAutoHideChanged(LayoutInfo info) {
			AutoHideMoveHelper.StopAutoHide();
			BeginUpdate();
			DockManager.SuspendRedrawForm();
			try {
				base.OnLayoutInfoAutoHideChanged(info);
				UpdateAutoHideContainersCollection();
				AutoHideMoveHelper.OnLayoutInfoAutoHideChanged(info);
				DockLayout dockLayout = (DockLayout)info;
				dockLayout.UpdateOnChangeAutoHide();
			}
			finally {
				EndUpdate();
				DockManager.ResumeRedrawForm();
			}
		}
		protected internal override void OnLayoutInfoVisibleChanged(LayoutInfo info) {
			Form mdiContainerActiveChild = null;
			if(this.DockManager.Form != null) {
				Form form = this.DockManager.Form as Form;
				if(form != null && form.IsMdiContainer) {
					mdiContainerActiveChild = form.ActiveMdiChild;
				}
			}
			(info as DockLayout).Panel.UpdateControlVisible(info.Visible);
			base.OnLayoutInfoVisibleChanged(info);
			DockManager.UpdateHiddenPanels();
			if(mdiContainerActiveChild != null) {
				mdiContainerActiveChild.Focus();
			}
		}
		protected internal virtual void AutoHideContainerDisposing(AutoHideContainer container) {
			AutoHideContainers.Remove(container);
			if(AutoHideInfoCollection.Contains(container.AutoHideInfo))
				AutoHideInfoCollection.Remove(container.AutoHideInfo);
			AutoHideMoveHelper.AutoHideContainerDisposing(container);
		}
		protected internal virtual void AutoHideHotLayoutChanged(AutoHideContainer container) {
			AutoHideMoveHelper.AutoHideHotLayoutChanged(container);
		}
		protected internal virtual void HideSliding() {
			AutoHideMoveHelper.HideSliding();
		}
		protected internal virtual DockLayout GetHotLayout(AutoHideContainer container) {
			return AutoHideMoveHelper.GetHotLayout(container);
		}
		Form GetTopForm(Form frm) {
			if(frm == null)
				return null;
			while(frm.Parent != null || frm.MdiParent != null) {
				if(frm.Parent != null) {
					Form frm2 = frm.Parent.FindForm();
					if(frm2 == null) return frm;
					frm = frm2;
				}
				else
					if(frm.MdiParent != null)
						frm = frm.MdiParent.FindForm();
			}
			return frm;
		}
		bool IsFormIconic {
			get {
				if(DockManager.OwnerForm == null || !DockManager.Form.IsHandleCreated) return false;
				if(DockManager.IsOwnerFormMDIChild) {
					if(IsIconic(DockManager.OwnerForm.MdiParent)) return true;
					MdiClient client = GetMdiClient(DockManager.OwnerForm.MdiParent);
					if(client != null && (client.Width <= 0 || client.Height <= 0))
						return true;
				}
				Form topForm = GetTopForm(DockManager.OwnerForm);
				return IsIconic(topForm) || IsIconic(DockManager.OwnerForm);
			}
		}
		MdiClient GetMdiClient(Form form) {
			foreach(Control control in form.Controls) {
				if(control is MdiClient)
					return (MdiClient)control;
			}
			return null;
		}
		static bool IsIconic(Form form) {
			if(!form.IsHandleCreated) return false;
			if(BarNativeMethods.IsIconic(form.Handle)) return true;
			return (form.Bounds == Rectangle.Empty);
		}
		bool Disposing { get { return DockManager.Disposing || DockManager.Form == null || DockManager.Form.Disposing; } }
		protected internal AutoHideMoveHelper AutoHideMoveHelper { get { return autoHideMoveHelper; } }
		protected internal CaptionMouseHoverHelper CaptionMouseHoverHelper { get { return captionMouseHoverHelper; } }
		public override Dictionary<DockingStyle, int> AutoHidePanelsSize {
			get {
				Dictionary<DockingStyle, int> result = new Dictionary<DockingStyle, int>();
				foreach(AutoHideContainer container in DockManager.AutoHideContainers) {
					int leght = container.SavedContainerSize.HasValue ? container.SavedContainerSize.Value : CalcAutoHidePanelSize(container);
					result.Add((DockingStyle)((int)container.Dock), leght);
				}
				return result;
			}
		}
		public override int AutoHidePanelSize {
			get {
				CheckAutoHidePanelSize();
				return cachedAutoHidePanelSize;
			}
		}
		public int CalcAutoHidePanelSize(AutoHideContainer container) {
			int result = 0;
			Size maxImageSize = GetMaximumSizeImageCore(GetDockPanels(container));
			using(AppearanceObject app = DockManager.GetMaxHeightHideBarAppearance(TabsPosition.Left)) {
				result = DockManager.Painter.HideBarPainter.GetHideBarSize(maxImageSize, app);
			}
			return result;
		}
		IEnumerable<DockPanel> GetDockPanels(AutoHideContainer container) {
			foreach(Control control in container.Controls) {
				if(control is DockPanel) {
					DockPanel panel = control as DockPanel;
					if(panel.HasChildren)
						for(int i = 0; i < panel.Count; i++) {
							yield return panel[i];
						}
					yield return panel;
				}
			}
		}
		void CheckAutoHidePanelSize() {
			CheckAutoHidePanelSize(false);
		}
		void CheckAutoHidePanelSize(bool reset) {
			if(cachedAutoHidePanelSize != 0 && !reset) return;
			using(AppearanceObject app = DockManager.GetMaxHeightHideBarAppearance(TabsPosition.Left)) {
				cachedAutoHidePanelSize = DockManager.Painter.HideBarPainter.GetHideBarSize(GetMaximumSizeImage(), app);
			}
		}
		Size GetMaximumSizeImage() {
			if(DockManager == null) return Size.Empty;
			Size result = Size.Empty;
			ICollection collection;
			if(DockManager.Panels == null)
				collection = DockManager.RootPanels;
			else
				collection = DockManager.Panels;
			if(collection != null)
				result = GetMaximumSizeImageCore(collection);
			return result;
		}
		Size GetMaximumSizeImageCore(IEnumerable collection) {
			Size result = Size.Empty;
			foreach(DockPanel panel in collection) {
				if(panel.DockLayout != null && panel.DockLayout.HasImage && result.Height < panel.DockLayout.ImageSize.Height) {
					result = panel.DockLayout.ImageSize;
				}
			}
			return result;
		}
		protected override AutoHideInfoCollection GetAutoHideCollectionOnGetClientBounds() {
			if(!DockManager.IsDeserializing) return base.GetAutoHideCollectionOnGetClientBounds();
			AutoHideInfoCollection result = new AutoHideInfoCollection();
			for(int i = 0; i < DockManager.AutoHideContainers.Count; i++)
				result.Add(new AutoHideInfo((DockingStyle)DockManager.AutoHideContainers[i].Dock));
			return result;
		}
		internal void ViewChanged() {
			cachedAutoHidePanelSize = 0;
		}
		protected DockZoneCollection DockZones { get { return dockZones; } }
		public AutoHideContainerCollection AutoHideContainers { get { return autoHideContainers; } }
		public override Rectangle Bounds {
			get {
				return (DockManager.Form == null ? LayoutConsts.InvalidRectangle : DockManager.Form.DisplayRectangle);
			}
		}
		public override Rectangle ClientBounds { get { return DockManager.GetClientBounds(base.ClientBounds); } }
		protected internal DockManager DockManager { get { return dockManager; } }
	}
}
