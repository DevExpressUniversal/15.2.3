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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
namespace DevExpress.XtraBars.Docking.Helpers {
	public class DockPanelMouseHandler {
		DockPanel owner;
		HitInfo pressInfo;
		int lockFocus;
		BaseHandlerState mouseHandlerState;
		public DockPanelMouseHandler(DockPanel owner) {
			this.owner = owner;
			this.pressInfo = HitInfo.Empty;
			this.lockFocus = 0;
			this.mouseHandlerState = CreateHandlerState(DockPanelState.Regular);
		}
		protected internal bool allowReset = true;
		bool IsVS2005Style {
			get {
				return Owner.DockManager.DockMode == DockMode.VS2005;
			}
		}
		protected virtual BaseHandlerState CreateHandlerState(DockPanelState state) {
			switch(state) {
				case DockPanelState.Regular: return new RegularHandlerState(this);
				case DockPanelState.Sizing: return new SizingHandlerState(this);
				case DockPanelState.Docking:
					if(IsVS2005Style) {
						if(Owner.DockManager.SupportVS2010DockingStyle())
							return new DockingHandlerStateVS2010(this);
						return new DockingHandlerStateVS2005(this);
					}
					return new DockingHandlerState(this);
			}
			return null;
		}
		internal void SetState(DockPanelState state, HitInfo hitInfo) {
			if(State == state) return;
			this.mouseHandlerState = CreateHandlerState(state);
			MouseHandlerState.Init(hitInfo);
		}
		void Activate(DockPanel panel) {
			BeginLockFocus();
			try {
				Owner.DockManager.ActivePanel = panel;
			}
			catch { EndLockFocus(); }
		}
		public virtual void MouseDown(MouseEventArgs e) {
			if(e.Button == MouseButtons.Right) {
				HitInfo clickInfo = GetHitInfo(GetPoint(e));
				if(clickInfo.HitTest == HitTest.Caption)
					ShowContextMenu(Owner, e.Location);
				if(clickInfo.HitTest == HitTest.Tab)
					ShowContextMenu(clickInfo.Tab, e.Location);
				return;
			}
			if(e.Button == MouseButtons.Left) {
				pressInfo = GetHitInfo(GetPoint(e));
				MouseHandlerState.MouseDown(PressInfo);
			}
		}
		void ShowContextMenu(DockPanel panel, Point point) {
			if(panel == null || panel.DockManager == null) return;
			if(State == DockPanelState.Regular)
				Activate(panel);
			panel.DockManager.DockController.ShowContextMenu(panel, point);
		}
		public virtual void MouseMove(MouseEventArgs e) {
			MouseHandlerState.MouseMove(GetHitInfo(GetPoint(e)));
		}
		public virtual void MouseUp(MouseEventArgs e) {
			if(Owner.IsDisposed) return;
			MouseHandlerState.MouseUp(GetHitInfo(GetPoint(e)));
			ResetCore();
		}
		public virtual void DoubleClick() {
			if(Owner.IsDisposed) return;
			MouseHandlerState.DoubleClick();
		}
		public virtual void MouseEnter(Point pt) {
			MouseHandlerState.MouseEnter(GetHitInfo(pt));
		}
		public virtual void MouseLeave() {
			MouseHandlerState.MouseLeave();
		}
		public virtual void ModifierKey(Keys key) {
			MouseHandlerState.ModifierKey(key);
		}
		public virtual void Reset() {
			if(!allowReset) return;
			MouseHandlerState.Reset();
			ResetCore();
		}
		public virtual void ResetCore() {
			if(!allowReset) return;
			this.pressInfo = HitInfo.Empty;
			Cursor.Current = Owner.Cursor;
			SetState(DockPanelState.Regular, HitInfo.Empty);
		}
		HitInfo GetHitInfo(Point pt) { return Owner.GetHitInfo(pt); }
		Point GetPoint(MouseEventArgs e) { return new Point(e.X, e.Y); }
		protected internal void BeginLockFocus() { lockFocus++; }
		protected internal void EndLockFocus() { lockFocus--; }
		protected internal BaseHandlerState MouseHandlerState { get { return mouseHandlerState; } }
		public DockPanelState State { get { return MouseHandlerState.State; } }
		protected internal DockPanel Owner { get { return owner; } }
		protected internal void SetOwner(DockPanel newOwner) { owner = newOwner; }
		protected internal HitInfo PressInfo { get { return pressInfo; } }
		public bool IsFocusLocked { get { return (lockFocus != 0) || Owner.IsFocusLocked; } }
	}
	public abstract class BaseHandlerState {
		DockPanelMouseHandler handler;
		protected BaseHandlerState(DockPanelMouseHandler handler) {
			this.handler = handler;
		}
		public virtual void Init(HitInfo hitInfo) { }
		public virtual void MouseDown(HitInfo hitInfo) { }
		public virtual void MouseMove(HitInfo hitInfo) { }
		public virtual void MouseUp(HitInfo hitInfo) { }
		public virtual void MouseEnter(HitInfo hitInfo) { }
		public virtual void MouseLeave() { }
		public virtual void DoubleClick() { }
		public virtual void Reset() { }
		public virtual void ModifierKey(Keys key) { }
		protected void SetState(DockPanelState state, HitInfo hitInfo) { Handler.SetState(state, hitInfo); }
		protected DockPanelMouseHandler Handler { get { return handler; } }
		protected DockPanel Owner { get { return Handler.Owner; } }
		protected DockLayout DockLayout { get { return Owner.DockLayout; } }
		protected DockManager DockManager { get { return Owner.DockManager; } }
		protected HitInfo PressInfo { get { return Handler.PressInfo; } }
		public abstract DockPanelState State { get; }
	}
	public class RegularHandlerState : BaseHandlerState {
		DockPanelCaptionButton hotElement;
		int raiseStartDockingCounter;
		public RegularHandlerState(DockPanelMouseHandler handler)
			: base(handler) {
			this.hotElement = null;
			this.raiseStartDockingCounter = 0;
		}
		void SetActivePanel(DockPanel panel) {
			Handler.BeginLockFocus();
			try {
				DockManager.ActivePanel = panel;
			}
			catch {
				Handler.EndLockFocus();
			}
		}
		protected virtual void SetActiveTabChild(DockPanel panel) {
			bool canFireChanged = (Owner.ActiveChild != panel);
			SetActivePanel(panel);
			DockManager.SetDesignTimeActiveChild(Owner, panel, canFireChanged);
		}
		public override void MouseDown(HitInfo hitInfo) {
			CheckHotTrack(PressInfo);
			base.MouseDown(hitInfo);
			if(PressInfo.ResizeZone != null) {
				StartSizingEventArgs e = new StartSizingEventArgs(PressInfo.ResizeZone.SizingPanel, DockLayoutUtils.ConvertToSizingSide(PressInfo.ResizeZone));
				DockManager.RaiseStartSizing(e);
				if(!e.Cancel)
					SetState(DockPanelState.Sizing, hitInfo);
			}
			else {
				if(Owner.Tabbed) {
					if(PressInfo.HitTest == HitTest.Tab) {
						SetActiveTabChild(PressInfo.Tab);
						return;
					}
				}
				if(PressInfo.Button != null && !PressInfo.Button.Disabled) {
					PressInfo.Button.Pressed = true;
					InvalidateButton(PressInfo.Button);
					SetActivePanel(Owner);
				}
				if(PressInfo.HitTest == HitTest.Caption || PressInfo.HitTest == HitTest.EmbeddedButtonPanel)
					SetActivePanel(Owner);
			}
		}
		public override void MouseMove(HitInfo hitInfo) {
			if(CanBeginDocking(hitInfo)) {
				HotElement = null;
				SetState(DockPanelState.Docking, hitInfo);
			}
			else
				CheckHotTrack(hitInfo);
		}
		public override void MouseUp(HitInfo hitInfo) {
			this.raiseStartDockingCounter = 0;
			if(hitInfo.IsEqual(PressInfo)) {
				if(hitInfo.Button != null) {
					if(hitInfo.Button is DockPanelCaptionToggleButton) {
						DockPanelCaptionToggleButton sb = hitInfo.Button as DockPanelCaptionToggleButton;
						sb.Switched = !sb.Switched;
					}
					SetUnpressed(hitInfo.Button);
					OnButtonClick(hitInfo);
				}
			}
			else
				SetUnpressed(PressInfo.Button);
		}
		protected virtual bool CanBeginDocking(HitInfo hitInfo) {
			if(WasStartDockingRaised || DockLayout.AutoHide || DockLayout.IsParentAutoHide) return false;
			if(PressInfo.GetDockingSourcePanel(Owner) == null) return false;
			bool result = false;
			if(Owner.Dock == DockingStyle.Float) result = (Math.Abs(PressInfo.HitPoint.X - hitInfo.HitPoint.X) >= SystemInformation.DragSize.Width || Math.Abs(PressInfo.HitPoint.Y - hitInfo.HitPoint.Y) >= SystemInformation.DragSize.Height);
			else result = !PressInfo.GetDockingSourceBounds(Owner).Contains(hitInfo.HitPoint);
			if(result) {
				result = CanStartDocking();
				this.raiseStartDockingCounter++;
			}
			return result;
		}
		bool CanStartDocking() {
			DockPanelCancelEventArgs e = new DockPanelCancelEventArgs(PressInfo.GetDockingSourcePanel(Owner));
			DockManager.RaiseStartDocking(e);
			return !e.Cancel;
		}
		bool DoubleClickDocking(DockPanel source) {
			Control target = null;
			bool tabbed = false;
			DockingStyle dock = DockingStyle.Float;
			if(source.Dock == DockingStyle.Float) {
				if(source.SavedParent == null)
					target = DockManager.Form;
				else
					target = source.SavedParent;
				tabbed = source.SavedTabbed;
				dock = source.SavedDock;
			}
			DockingEventArgs e = new DockingEventArgs(source, target, Cursor.Position, dock, tabbed, source.SavedIndex);
			DockManager.RaiseDocking(e);
			return e.Cancel;
		}
		public override void MouseEnter(HitInfo hitInfo) {
			CheckHotTrack(hitInfo);
		}
		public override void MouseLeave() {
			CheckHotTrack(HitInfo.Empty);
		}
		public override void DoubleClick() {
			DockPanel sourcePanel = PressInfo.GetDockingSourcePanel(Owner);
			if(sourcePanel == null) return;
			if(sourcePanel.Dock == DockingStyle.Float && DockManager.documentManagerCore != null) {
				FloatForm floatForm = sourcePanel.FloatForm;
				if((Control.ModifierKeys & Keys.Control) == 0 && floatForm != null) {
					floatForm.CheckMaximizedBounds();
					floatForm.WindowState = (floatForm.WindowState == FormWindowState.Maximized) ?
						FormWindowState.Normal : FormWindowState.Maximized;
					return;
				}
			}
			if(!CanFloatOnDoubleClick(sourcePanel) || !CanStartDocking()) return;
			bool canceled = DoubleClickDocking(sourcePanel);
			DockManager manager = DockManager;
			if(!canceled) {
				DockingStyle dock = sourcePanel.Dock;
				if(sourcePanel.FloatForm != null)
					sourcePanel.Restore();
				else
					sourcePanel.MakeFloat();
				canceled = canceled || (dock == sourcePanel.Dock);
			}
			manager.RaiseEndDocking(new EndDockingEventArgs(sourcePanel, canceled));
		}
		bool CanFloatOnDoubleClick(DockPanel panel) {
			return DockManager.DockingOptions.FloatOnDblClick && panel.Options.AllowFloating && panel.Options.FloatOnDblClick;
		}
		void OnButtonClick(HitInfo hitInfo) {
			if(hitInfo.Button == null || hitInfo.Button.Disabled) return;
			switch(hitInfo.HitTest) {
				case HitTest.PrevTabButton: OnClickPrevTabButton(); break;
				case HitTest.NextTabButton: OnClickNextTabButton(); break;
			}
			FireChanged();
		}
		protected virtual void OnClickPrevTabButton() {
			DockLayout.DecFirstVisibleTabIndex();
		}
		protected virtual void OnClickNextTabButton() {
			DockLayout.IncFirstVisibleTabIndex();
		}
		protected virtual void CheckHotTrack(HitInfo hitInfo) {
			bool resetCursor = true;
			if(PressInfo.Button != null) {
				InvalidateButton(PressInfo.Button);
			}
			else if(hitInfo.ResizeZone != null) {
				Cursor.Current = hitInfo.ResizeZone.Cursor;
				resetCursor = false;
			}
			if(resetCursor)
				Cursor.Current = Owner.Cursor;
			HotElement = hitInfo.Button;
		}
		void InvalidateButton(DockPanelCaptionButton button) {
			InvalidateRect(button.Bounds);
		}
		protected void InvalidateRect(Rectangle bounds) {
			if(DockLayoutUtils.CanDraw(null, bounds))
				Owner.Invalidate(bounds);
		}
		public override void Reset() {
			SetUnpressed(PressInfo.Button);
			if(IsTabContainerProcessing)
				Owner.ParentPanel.InternalKeyDown(new KeyEventArgs(Keys.Escape));
		}
		public override void ModifierKey(Keys key) {
			if(IsTabContainerProcessing)
				Owner.ParentPanel.InternalKeyDown(new KeyEventArgs(key));
		}
		bool IsTabContainerProcessing { get { return (Owner.ParentPanel != null && Owner.ParentPanel.State == DockPanelState.Docking); } }
		void SetUnpressed(DockPanelCaptionButton button) {
			if(button == null) return;
			button.Pressed = false;
			InvalidateButton(button);
		}
		void FireChanged() { Owner.FireChanged(); }
		protected bool WasStartDockingRaised { get { return (raiseStartDockingCounter > 0); } }
		protected DockPanelCaptionButton HotElement {
			get { return hotElement; }
			set {
				if(HotElement == value) return;
				SetHot(false);
				hotElement = value;
				SetHot(true);
			}
		}
		void SetHot(bool hot) {
			if(HotElement != null && !HotElement.Disabled) {
				HotElement.Hot = hot;
				InvalidateRect(HotElement.Bounds);
			}
		}
		protected CaptionMouseHoverHelper HoverHelper { get { return DockManager.LayoutManager.CaptionMouseHoverHelper; } }
		public sealed override DockPanelState State { get { return DockPanelState.Regular; } }
	}
	public class SizingHandlerState : BaseHandlerState {
		public SizingHandlerState(DockPanelMouseHandler handler) : base(handler) { }
		public override void Init(HitInfo hitInfo) {
			CheckActiveAutoHidePanel();
			ResizeZone.StartSizing(PressInfo.HitPoint);
		}
		protected virtual void CheckActiveAutoHidePanel() {
			DockPanel panel = GetActiveAutoHidePanel(DockManager.ActivePanel);
			if(panel != null && ResizeZone.SizingPanel != panel)
				panel.HideImmediately();
		}
		DockPanel GetActiveAutoHidePanel(DockPanel dockPanel) {
			for(DockPanel parentPanel = dockPanel; parentPanel != null; parentPanel = parentPanel.ParentPanel) {
				if(parentPanel.Visibility == DockVisibility.AutoHide)
					return parentPanel;
			}
			return null;
		}
		protected Point GetSizingPoint(HitInfo hitInfo) {
			if(hitInfo.Control != null && hitInfo.Control.IsHandleCreated) {
				return hitInfo.Control.PointToClient(Control.MousePosition);
			}
			return hitInfo.HitPoint;
		}
		public override void MouseMove(HitInfo hitInfo) {
			SizingEventArgs e = new SizingEventArgs(ResizeZone.SizingPanel, DockLayoutUtils.ConvertToSizingSide(ResizeZone), GetSizingPoint(hitInfo), ResizeZone.GetNewSize(PressInfo.HitPoint, GetSizingPoint(hitInfo)));
			DockManager.RaiseSizing(e);
			if(e.Cancel) return;
			ResizeZone.DoSizing(GetSizingPoint(hitInfo));
		}
		public override void MouseUp(HitInfo hitInfo) {
			ResizeZone.EndSizing(PressInfo.HitPoint, GetSizingPoint(hitInfo));
			RaiseEndSizing(false);
		}
		public override void Reset() {
			ResizeZone.Release();
			RaiseEndSizing(true);
		}
		void RaiseEndSizing(bool canceled) {
			if(ResizeZone == null) return;
			DockManager.LayoutManager.CheckDecreaseSize();
			DockManager.RaiseEndSizing(new EndSizingEventArgs(ResizeZone.SizingPanel, canceled, DockLayoutUtils.ConvertToSizingSide(ResizeZone)));
		}
		ResizeZone ResizeZone { get { return PressInfo.ResizeZone; } }
		public sealed override DockPanelState State { get { return DockPanelState.Sizing; } }
	}
	public class DockingHandlerStateVS2010 : DockingHandlerStateVS2005 {
		Docking2010.DocumentManager manager;
		public DockingHandlerStateVS2010(DockPanelMouseHandler handler)
			: base(handler) {
			manager = Owner.DockManager.DocumentManager;
		}
		Docking2010.Views.BaseDocument document;
		Docking2010.Dragging.FloatPanelUIView GetUIView(Form floatForm) {
			return manager.UIViewAdapter.GetView(floatForm) as Docking2010.Dragging.FloatPanelUIView;
		}
		bool IsOverDocumentManager(Docking2010.DragEngine.IUIView view, Point screenPoint) {
			return manager.UIViewAdapter.GetBehindView(view, screenPoint) is Docking2010.Dragging.DocumentManagerUIView;
		}
		protected override void StartDocking(Point pt) {
			base.StartDocking(pt);
			if(Owner.Dock == DockingStyle.Float) {
				Form floatForm = Owner.FloatForm;
				if(manager.View == null) return;
				if(floatForm != null) {
					if(floatForm.WindowState == FormWindowState.Maximized) {
						floatForm.Location = pt;
						floatForm.WindowState = FormWindowState.Normal;
					}
					floatForm.Update();
					document = ((Docking2010.Views.IBaseViewControllerInternal)manager.View.Controller).RegisterDockPanel(floatForm);
					if(document != null) {
						Docking2010.Dragging.FloatPanelUIView view = GetUIView(floatForm);
						if(view != null && !view.InDragging) {
							Point screenPoint = PointToScreen(pt);
							view.Adapter.DragService.ReparentDragOperation();
							view.BeginExternalDragging(screenPoint);
							helper = new Docking2010.Dragging.SnapHelper(view);
						}
					}
				}
			}
		}
		int newDockingOperationStarted = 0;
		protected override void StartNewDockingOperation(Point pt, DockPanel tpanel) {
			if(dockingOperationInProgress > 0)
				newDockingOperationStarted++;
			base.StartNewDockingOperation(pt, tpanel);
		}
		int dockingOperationInProgress = 0;
		protected override void DoDocking(Point pt) {
			dockingOperationInProgress++;
			base.DoDocking(pt);
			dockingOperationInProgress--;
			Form floatForm = Owner.FloatForm;
			Docking2010.Dragging.FloatPanelUIView view = GetUIView(floatForm);
			if(view != null && !view.InDragging) {
				if(newDockingOperationStarted > 0) {
					Owner.Update();
					view.CancelExternalDragging();
					helper.ResetSnapping();
					newDockingOperationStarted = 0;
				}
				else {
					Point screenPoint = PointToScreen(pt);
					view.ExternalDragging(screenPoint);
					helper.UpdateSnapping(screenPoint, view.LayoutRoot);
				}
			}
		}
		protected override void EndDocking(Point pt) {
			Form floatForm = Owner.FloatForm;
			Docking2010.Dragging.FloatPanelUIView view = GetUIView(floatForm);
			Point screenPoint = Point.Empty;
			Docking2010.DragEngine.ILayoutElement element = null;
			if(view != null && !view.InDragging) {
				element = view.LayoutRoot;
				screenPoint = PointToScreen(pt);
				view.EndExternalDragging(screenPoint);
				CheckDocking(floatForm);
			}
			if(!Owner.IsDisposed && CheckCanShowGuidesOnPressingShiftChanged())
				base.EndDocking(pt);
			CheckDocking(floatForm);
			if(helper != null)
				helper.ResetSnapping();
		}
		public override void Reset() {
			base.Reset();
			if(manager != null && manager.UIViewAdapter != null) {
				Docking2010.Dragging.FloatPanelUIView view = GetUIView(Owner.FloatForm);
				if(view != null && !view.InDragging)
					view.Adapter.DragService.CancelDragOperation();
				if(helper != null)
					helper.ResetSnapping();
			}
		}
		void CheckDocking(Form floatForm) {
			if(Owner.Dock != DockingStyle.Float) {
				if(manager.View != null)
					((Docking2010.Views.IBaseViewControllerInternal)manager.View.Controller).UnregisterDockPanel(floatForm);
				document = null;
			}
		}
		protected override void OnEndDockingCanceled() {
			if(IsDocument(Owner)) return;
			base.OnEndDockingCanceled();
		}
		Docking2010.Dragging.SnapHelper helper;
		protected override void UpdateVisualizers(Point screenPoint) {
			Form floatForm = Owner.FloatForm;
			if(floatForm != null) {
				bool dockPanelPresent = (DockPanelCore != null);
				Docking2010.Dragging.FloatPanelUIView view = GetUIView(floatForm);
				if(view != null)
					view.SuspendBehindDragging(dockPanelPresent);
				if(!dockPanelPresent || IsDocument(DockPanelCore)) {
					if(view != null) {
						if(IsOverDocumentManager(view, screenPoint))
							manager.Adorner.Show();
						else
							manager.Adorner.Hide();
						UpdateFloatingCursor(CalcDocumentManagerAdornerIsHot);
					}
					generalVisualizer.Hide();
					dockZoneVisualizer.Hide();
					return;
				}
				if(dockPanelPresent)
					manager.Adorner.Hide();
				UpdateFloatingCursor(CalcDockManagerAdornerIsHot);
			}
			Docking2010.Customization.VS2010StyleDockZoneVisualizer.OwnerPanel = Owner;
			Docking2010.Customization.VS2010StyleDockZoneVisualizer.TargetPanel = DockPanelCore;
			try {
				base.UpdateVisualizers(screenPoint);
			}
			finally {
				Docking2010.Customization.VS2010StyleDockZoneVisualizer.OwnerPanel = null;
				Docking2010.Customization.VS2010StyleDockZoneVisualizer.TargetPanel = null;
			}
		}
		protected override Rectangle GetScreenVisualizerBounds() {
			return Owner.DockManager.GetScreenDockingBounds();
		}
		protected override VisualizerVisibilityArgs RequestVisibilityPanel() {
			return ((Docking2010.Customization.VS2010StyleDockZoneVisualizer)dockZoneVisualizer).RequestVisibilityPanel(base.RequestVisibilityPanel());
		}
		protected override VisualizerVisibilityArgs RequestVisibilityGlobal() {
			return ((Docking2010.Customization.VS2010StyleDockZoneVisualizer)dockZoneVisualizer).RequestVisibilityGlobal(base.RequestVisibilityGlobal());
		}
		bool CalcDockManagerAdornerIsHot() {
			bool isHot = ((Docking2010.Customization.VS2010StyleDockZoneVisualizer)dockZoneVisualizer).CalcIsHotTrack() ||
				((Docking2010.Customization.VS2010StyleDockZoneVisualizer)generalVisualizer).CalcIsHotTrack();
			return isHot;
		}
		bool CalcDocumentManagerAdornerIsHot() {
			bool isHot = false;
			foreach(var e in manager.Adorner.Elements) {
				var rInfoArgs = e.InfoArgs as Docking2010.Customization.ReorderingAdornerInfoArgs;
				if(rInfoArgs != null)
					isHot |= rInfoArgs.TabHintVisible;
				var dInfoArgs = e.InfoArgs as Docking2010.Customization.DockingAdornerInfoArgs;
				if(dInfoArgs != null)
					isHot |= (dInfoArgs.HotHint != Docking2010.Customization.DockHint.None);
			}
			return isHot;
		}
		void UpdateFloatingCursor(Func<bool> CalcIsHot) {
			if(Owner.Options.AllowFloating) return;
			Cursor.Current = CalcIsHot() ? Cursors.Default :
				(DockManager.DockingOptions.CursorFloatCanceled == null ?
				Cursors.No : DockManager.DockingOptions.CursorFloatCanceled);
		}
		bool canShowDockGuidesCore = true;
		protected override void MouseOver(Point point) {
			Point screenPoint = PointToScreen(point);
			UpdateDockPanelCore(screenPoint);
			if(ProcessInTabDragging(point, screenPoint)) return;
			var canShowDockGuides = CheckCanShowGuidesOnPressingShiftChanged();
			if(canShowDockGuides) {
				BaseMouseOver(screenPoint);
				UpdateVisualizers(screenPoint);
			}
			Form floatForm = Owner.FloatForm;
			Docking2010.Dragging.FloatPanelUIView view = GetUIView(floatForm);
			if(view != null && !view.InDragging)
				DockSource.Panel.FloatLocation = CalcDockPanelLocationWithOffset(Control.MousePosition);
		}
		bool CheckCanShowGuidesOnPressingShiftChanged() {
			var canShowDockGuides = CanShowDockGuidesOnPressingShift();
			if(canShowDockGuides != canShowDockGuidesCore) {
				canShowDockGuidesCore = canShowDockGuides;
				if(!canShowDockGuides) {
					generalVisualizer.Hide();
					dockZoneVisualizer.Hide();
				}
			}
			return canShowDockGuides;
		}
		bool CanShowDockGuidesOnPressingShift() {
			if(manager == null || manager.View == null) return true;
			return manager.View.CanShowDockGuidesOnPressingShift();
		}
		bool IsDocument(DockPanel panel) {
			if(manager == null || manager.View == null) return false;
			Control control = panel;
			if(manager.IsMdiStrategyInUse && panel.FloatForm != null)
				control = panel.FloatForm;
			Docking2010.Views.BaseDocument document;
			return (manager.View.Documents.TryGetValue(control, out document));
		}
		protected override void CheckCursor(DockZone dockZone) {
			if(dockZone is FloatDockZone) return;
			base.CheckCursor(dockZone);
		}
	}
	public class DockingHandlerStateVS2005 : DockingHandlerState {
		protected DockPanel dockPanel;
		protected internal static VS2005StyleDockingVisualizer dockZoneVisualizer, generalVisualizer;
		public DockingHandlerStateVS2005(DockPanelMouseHandler handler)
			: base(handler) {
			shouldDrawPointers = false;
			dockZoneVisualizer = GetPanelDockZoneVisualizer();
			generalVisualizer = GetGlobalDockZoneVisualizer();
		}
		protected virtual VS2005StyleDockingVisualizer GetPanelDockZoneVisualizer() {
			return DockManager.PanelDockZoneVisualizer;
		}
		protected virtual VS2005StyleDockingVisualizer GetGlobalDockZoneVisualizer() {
			return DockManager.GlobalDockZoneVisualizer;
		}
		protected override void StartDocking(Point pt) {
			DockPanel tpanel = null;
			if(DockSource.Panel != null) {
				tpanel = DockSource.Panel;
				if(!DockSource.Panel.IsTab) {
					wasTab = false;
					Handler.allowReset = false;
					if(tpanel.FloatForm != null && tpanel.FloatForm.WindowState == FormWindowState.Maximized) {
						tpanel.FloatForm.Location = pt;
						tpanel.FloatForm.WindowState = FormWindowState.Normal;
					}
					if(!DockSource.Panel.IsDockingInitialization)
						DockSource.Panel.FloatLocation = CalcDockPanelLocationWithOffset(Control.MousePosition);
					DockSource.Panel.Dock = DockingStyle.Float;
					if(!DockSource.Panel.IsDockingInitialization)
						DockSource.Panel.FloatLocation = CalcDockPanelLocationWithOffset(Control.MousePosition);
					Handler.allowReset = true;
					base.StartDocking(Owner.PointToClient(CalcDockPanelLocationWithOffset(Control.MousePosition)));
				}
				else {
					wasTab = true;
				}
			}
			endDockingRised = false;
		}
		protected TabsPosition GetTabsPosition() {
			if(DockPanelCore != null) {
				if(DockPanelCore.IsTab && DockPanelCore != null)
					return DockPanelCore.ParentPanel.TabsPosition;
				return DockPanelCore.TabsPosition;
			}
			else return TabsPosition.Bottom;
		}
		void MakeFloatDockPanel(DockPanel panel) {
			panel.FloatLocation = CalcDockPanelLocationWithOffset(Control.MousePosition);
			panel.Dock = DockingStyle.Float;
			panel.FloatLocation = CalcDockPanelLocationWithOffset(Control.MousePosition);
			panel.MouseHandler.MouseEnter(new Point(20, 20));
			panel.MouseHandler.MouseDown(new MouseEventArgs(MouseButtons.Left, 0, 8, 8, 0));
			panel.MouseHandler.MouseMove(new MouseEventArgs(MouseButtons.Left, 0, 20, 20, 0));
			panel.Capture = true;
		}
		protected void BaseMouseOverInTabDragging(Point screenPoint) {
			DockZone targetZone = null;
			bool stop = false;
			foreach(DockPanel panel in DockManager.Panels) {
				foreach(DockZone zone in panel.DockLayout.DockZones) {
					if((zone is TabVisualTabPanelDockZone || zone is TabVisualCaptionDockZone) && zone.Bounds.Contains(screenPoint) && zone.TargetControl != DockSource.Panel) {
						targetZone = zone;
						stop = true;
						break;
					}
				}
				if(stop) break;
			}
			if(targetZone != null && targetZone.TargetControl == DockSource.Panel)
				DockZoneCore = null;
			else
				DockZoneCore = targetZone;
			if(DockZoneCore == null) return;
			if(DockZoneCore.TargetControl == DockSource.Panel) return;
			if(!RequestVisibility(DockPanelCore, DockingStyle.Fill, true)) {
				if(DockZoneCore.DockStyle != DockingStyle.Float)
					DockZoneCore = CreateFloatZone(screenPoint);
				return;
			}
			DockZoneCore.MouseOver(DockSource, screenPoint);
		}
		protected bool IsInTabRegion(Point pt) {
			if(DockPanelCore != null) {
				if(DockZoneCore == null || DockZoneCore.TargetControl == DockSource.Panel) return false;
				if(Owner.IsTab) return true;
				if(IsInTabRegionCore(pt)) return true;
			}
			return false;
		}
		protected bool IsInTabRegionCore(Point pt) {
			if(!(DockZoneCore is TabVisualTabPanelDockZone)) return false;
			if(!DockZoneCore.Bounds.Contains(pt)) return false;
			if((DockZoneCore as TabVisualTabPanelDockZone).CanEmulateDocking) return true;
			else return DockSource != null && !DockSource.Float;
		}
		protected void MakeOwnerFloat(Point pt) {
			if(DockSource.Panel != null && DockSource.Panel.Parent != null) {
				DockPanel tpanel = DockSource.Panel;
				if(tpanel.Parent is DockPanel && ((DockPanel)tpanel.Parent).Count == 2) {
					MakeFloatDockPanel(tpanel);
				}
				else StartNewDockingOperation(pt, tpanel);
			}
		}
		protected virtual void StartNewDockingOperation(Point pt, DockPanel tpanel) {
			base.StartDocking(pt);
			DockZoneCore = CreateFloatZone(CalcDockPanelLocationWithOffset(Control.MousePosition));
			MakeFloatDockPanel(tpanel);
			Owner.MouseHandler.Reset();
		}
		bool wasTab = false;
		FloatForm hiddenForm = null;
		protected enum InTabDraggingState { TabDraggedInTabCaptionRegion, TabDraggedOutOfTabCaptionRegion, PanelDraggedInTabCaptionRegion, PanelDraggedOutOfTabCaptionRegion };
		protected InTabDraggingState inTabState = InTabDraggingState.PanelDraggedOutOfTabCaptionRegion;
		protected bool ProcessInTabDragging(Point pt, Point screenPoint) {
			if(DockPanelCore != null)
				BaseMouseOverInTabDragging(screenPoint);
			bool inTab = IsInTabRegion(screenPoint);
			InTabDraggingState prevInTabState = inTabState;
			if(inTab) {
				if(wasTab) inTabState = InTabDraggingState.TabDraggedInTabCaptionRegion;
				else inTabState = InTabDraggingState.PanelDraggedInTabCaptionRegion;
			}
			else {
				if(!wasTab) inTabState = InTabDraggingState.PanelDraggedOutOfTabCaptionRegion;
				else {
					inTabState = InTabDraggingState.TabDraggedOutOfTabCaptionRegion;
					inTab = IsInTabRegion(screenPoint);
				}
			}
			switch(inTabState) {
				case InTabDraggingState.PanelDraggedInTabCaptionRegion:
					if(prevInTabState != inTabState) {
						if(prevInTabState != inTabState) {
							if(Owner.Parent is FloatForm && DockZoneCore is TabVisualDockZone && ((TabVisualDockZone)DockZoneCore).DockEmulator.State == DockEmulatorState.Added) {
								Handler.allowReset = false;
								if(!IsFloatDockZone(DockZoneCore))
									DockManager.Form.Focus();
								Handler.allowReset = true;
								Owner.Parent.Visible = false;
								hiddenForm = (FloatForm)Owner.Parent;
							}
							generalVisualizer.Hide();
							dockZoneVisualizer.Hide();
						}
					}
					BaseMouseOverInTabDragging(screenPoint);
					return true;
				case InTabDraggingState.TabDraggedInTabCaptionRegion:
					BaseMouseOverInTabDragging(screenPoint);
					return true;
				case InTabDraggingState.PanelDraggedOutOfTabCaptionRegion:
					if(prevInTabState != inTabState)
						ResetTabCaptionRegionPanelDragging();
					return false;
				case InTabDraggingState.TabDraggedOutOfTabCaptionRegion:
					if(prevInTabState != inTabState) {
						wasTab = false;
						MakeOwnerFloat(pt);
					}
					return true;
			}
			return false;
		}
		void ResetTabCaptionRegionPanelDragging() {
			if(Owner.Parent is FloatForm) {
				Owner.Parent.Visible = true;
				Owner.ControlVisible = true;
				hiddenForm = null;
			}
		}
		protected void BaseMouseOver(Point screenPoint) {
			DockZone targetZone = GetDockZoneAtPos(screenPoint);
			if(!TargetFormVisible())
				targetZone = CreateFloatZone(screenPoint);
			if(DockZoneCore is TabVisualDockZone && DockZoneCore.Bounds.Contains(screenPoint)) {
			}
			else DockZoneCore = targetZone;
			if(DockZoneCore is TabVisualDockZone) return;
			DockZoneCore.MouseOver(DockSource, screenPoint);
		}
		protected Control FindControlToRefresh(Control control) {
			Control result = control.FindForm();
			if(result == null) {
				result = control;
				while(result.Parent != null) {
					result = result.Parent;
				}
			}
			return result;
		}
		protected override void MouseOver(Point pt) {
			Point screenPoint = PointToScreen(pt);
			UpdateDockPanelCore(screenPoint);
			if(ProcessInTabDragging(pt, screenPoint)) return;
			BaseMouseOver(screenPoint);
			UpdateVisualizers(screenPoint);
			UpdateCursor(screenPoint);
			DockSource.Panel.FloatLocation = CalcDockPanelLocationWithOffset(Control.MousePosition);
		}
		void ForceUpdate() {
			if(DockManager != null && DockManager.Form != null) {
				Control controlToRefresh = FindControlToRefresh(DockManager.Form);
				controlToRefresh.Invalidate();
				controlToRefresh.Update();
				foreach(DockPanel tPanel in DockManager.Panels) {
					if(tPanel.Dock == DockingStyle.Float) {
						tPanel.Invalidate();
						tPanel.Update();
					}
				}
			}
		}
		protected void UpdateDockPanelCore(Point screenPt) {
			DockPanel dockPanelCore = null;
			if(Owner.Dock == DockingStyle.Float) {
				dockPanelCore = DockManager.GetDockPanelAtPosBut(screenPt, Owner);
				if(dockPanelCore != null)
					dockPanelCore = dockPanelCore.GetPanelAtPos(screenPt);
			}
			else dockPanelCore = DockManager.GetDockPanelAtPos(screenPt);
			DockPanelCore = dockPanelCore;
		}
		void UpdateCursor(Point screenPt) {
			if(DockSource.Panel.Options.AllowFloating) return;
			VisualizerHitInfoType dockZoneVisualizerHitInfo = dockZoneVisualizer.CalcHitInfo(screenPt);
			VisualizerHitInfoType generalVisualizerHitInfo = generalVisualizer.CalcHitInfo(screenPt);
			if(dockZoneVisualizerHitInfo != VisualizerHitInfoType.Nothing || generalVisualizerHitInfo != VisualizerHitInfoType.Nothing) {
				Cursor.Current = Cursors.Default;
			}
		}
		protected virtual Point CalcDockPanelLocationWithOffset(Point point) {
			if(Owner.ClickPoint.X < Owner.Width && Owner.ClickPoint.Y < Owner.Height)
				return new Point(point.X - Owner.ClickPoint.X, point.Y - Owner.ClickPoint.Y);
			return new Point(point.X - Owner.Width / 2, point.Y);
		}
		protected override DockZone GetDockZoneAtPos(Point screenPoint) {
			if((Control.ModifierKeys & Keys.Control) != 0) return CreateFloatZone(screenPoint);
			DockZone result = null;
			DockPanel panel;
			panel = DockManager.GetDockPanelAtPos(screenPoint);
			if((panel != null && panel != DockSource.Panel && !DockSource.Panel.HasAsChild(panel)) || (panel != null && panel.IsTab))
				result = panel.DockLayout.GetDockZoneAtPos(screenPoint);
			else
				result = ((IDockZonesOwner)DockLayout.Manager).DockZones[screenPoint];
			return (result == null ? CreateFloatZone(screenPoint) : result);
		}
		protected virtual DockZone GetGeneralEndDockingZone(VisualizerHitInfoType generalHitInfo) {
			DockZone dockZone = null;
			switch(generalHitInfo) {
				case VisualizerHitInfoType.Top:
					dockZone = Owner.DockManager.LayoutManager.GetDockZoneByDockStyle(DockingStyle.Top, 0);
					break;
				case VisualizerHitInfoType.Bottom:
					dockZone = Owner.DockManager.LayoutManager.GetDockZoneByDockStyle(DockingStyle.Bottom, 0);
					break;
				case VisualizerHitInfoType.Left:
					dockZone = Owner.DockManager.LayoutManager.GetDockZoneByDockStyle(DockingStyle.Left, 0);
					break;
				case VisualizerHitInfoType.Right:
					dockZone = Owner.DockManager.LayoutManager.GetDockZoneByDockStyle(DockingStyle.Right, 0);
					break;
				case VisualizerHitInfoType.CenterBottom:
					dockZone = Owner.DockManager.LayoutManager.GetDockZoneByDockStyle(DockingStyle.Bottom, generalVisualizer.RestBounds);
					break;
				case VisualizerHitInfoType.CenterTop:
					dockZone = Owner.DockManager.LayoutManager.GetDockZoneByDockStyle(DockingStyle.Top, generalVisualizer.RestBounds);
					break;
				case VisualizerHitInfoType.CenterLeft:
					dockZone = Owner.DockManager.LayoutManager.GetDockZoneByDockStyle(DockingStyle.Left, generalVisualizer.RestBounds);
					break;
				case VisualizerHitInfoType.CenterRight:
					dockZone = Owner.DockManager.LayoutManager.GetDockZoneByDockStyle(DockingStyle.Right, generalVisualizer.RestBounds);
					break;
			}
			return dockZone;
		}
		protected DockPanel GetTabParent() {
			if(DockPanelCore != null) {
				if(DockPanelCore.IsTab) {
					if(DockPanelCore.Parent != null) {
						if(DockPanelCore.Parent is DockPanel) {
							return DockPanelCore.Parent as DockPanel;
						}
					}
				}
			}
			return null;
		}
		protected DockPanel GetNeightbourPanel(DockPanel source, DockingStyle ds) {
			DockPanel result = null;
			Rectangle sourceScreenBounds = new Rectangle(source.PointToScreen(Point.Empty), source.Size);
			foreach(DockPanel tpanel in DockManager.Panels) {
				if(tpanel == source) continue;
				Rectangle tempPanelScreenBounds = new Rectangle(tpanel.PointToScreen(Point.Empty), tpanel.Size);
				switch(ds) {
					case DockingStyle.Left:
						if(tempPanelScreenBounds.Y == sourceScreenBounds.Y && tempPanelScreenBounds.Right == sourceScreenBounds.Left)
							result = tpanel;
						break;
					case DockingStyle.Right:
						if(tempPanelScreenBounds.Y == sourceScreenBounds.Y && tempPanelScreenBounds.Left == sourceScreenBounds.Right)
							result = tpanel;
						break;
					case DockingStyle.Top:
						if(tempPanelScreenBounds.X == sourceScreenBounds.X && tempPanelScreenBounds.Bottom == sourceScreenBounds.Top)
							result = tpanel;
						break;
					case DockingStyle.Bottom:
						if(tempPanelScreenBounds.X == sourceScreenBounds.X && tempPanelScreenBounds.Top == sourceScreenBounds.Bottom)
							result = tpanel;
						break;
				}
			}
			return result;
		}
		protected DockZone CheckDockZone(DockZone dockZone, DockPanel dpanel, DockingStyle expected, DockingStyle corrected) {
			DockZone result = dockZone;
			if(dockZone == null) {
				DockPanel tempPanel = GetNeightbourPanel(dpanel, expected);
				if(tempPanel != null) {
					result = tempPanel.DockLayout.GetDockZoneByDockStyle(corrected);
				}
				else {
					if(expected == DockingStyle.Top && dpanel.ParentPanel != null)
						result = dpanel.ParentPanel.DockLayout.GetDockZoneByDockStyle(expected);
				}
			}
			return result;
		}
		protected virtual DockZone GetPanelEndDockingZone(VisualizerHitInfoType inPanelHitInfo) {
			DockZone dockZone = null;
			DockPanel panel = GetTabParent();
			if(panel == null) panel = DockPanelCore;
			switch(inPanelHitInfo) {
				case VisualizerHitInfoType.CenterLeft:
					dockZone = panel.DockLayout.GetDockZoneByDockStyle(DockingStyle.Left);
					dockZone = CheckDockZone(dockZone, panel, DockingStyle.Left, DockingStyle.Right);
					break;
				case VisualizerHitInfoType.CenterRight:
					dockZone = panel.DockLayout.GetDockZoneByDockStyle(DockingStyle.Right);
					dockZone = CheckDockZone(dockZone, panel, DockingStyle.Right, DockingStyle.Left);
					break;
				case VisualizerHitInfoType.CenterTop:
					dockZone = panel.DockLayout.GetDockZoneByDockStyle(DockingStyle.Top);
					dockZone = CheckDockZone(dockZone, panel, DockingStyle.Top, DockingStyle.Bottom);
					break;
				case VisualizerHitInfoType.CenterBottom:
					dockZone = panel.DockLayout.GetDockZoneByDockStyle(DockingStyle.Bottom);
					dockZone = CheckDockZone(dockZone, panel, DockingStyle.Bottom, DockingStyle.Top);
					break;
				case VisualizerHitInfoType.CenterCenter:
					dockZone = panel.DockLayout.GetDockZoneByType(typeof(BaseTabDockZone));
					break;
			}
			return dockZone;
		}
		protected virtual void EndDockingByDockZone(DockZone zone, Point newHitPoint, Point originalPoint) {
			Point newDockLocation = Owner.PointToClient(newHitPoint);
			DockZoneCore = zone;
			DockLayoutManager dlm = (DockManager != null && DockManager.LayoutManager != null) ? DockManager.LayoutManager : null;
			base.EndDocking(newDockLocation);
			if(dlm != null) dlm.LayoutChanged();
		}
		protected virtual void EndDockingVS2005(Point pt) {
			Point screenPoint = Owner.PointToScreen(pt);
			VisualizerHitInfoType generalHitInfo = generalVisualizer.CalcHitInfo(screenPoint);
			VisualizerHitInfoType inPanelHitInfo = dockZoneVisualizer.CalcHitInfo(screenPoint);
			generalVisualizer.Hide();
			dockZoneVisualizer.Hide();
			EndDocking2005Core(pt, DockManager, generalHitInfo, inPanelHitInfo);
		}
		protected void EndDocking2005Core(Point pt, DockManager manager, VisualizerHitInfoType generalHitInfo, VisualizerHitInfoType inPanelHitInfo) {
			DockZone zone = null;
			if(inPanelHitInfo != VisualizerHitInfoType.Nothing) {
				zone = GetPanelEndDockingZone(inPanelHitInfo);
			}
			else {
				if(generalHitInfo != VisualizerHitInfoType.Nothing) {
					zone = GetGeneralEndDockingZone(generalHitInfo);
				}
			}
			if(DockZoneCore is TabCaptionDockZone || DockZoneCore is TabVisualCaptionDockZone || DockZoneCore is TabVisualTabPanelDockZone) {
				base.EndDocking(pt);
				if(hiddenForm != null && manager != null && manager.RootPanels.IndexOf(Owner) >= 0) {
					manager.RootPanels.RemoveAt(manager.RootPanels.IndexOf(Owner));
				}
				if(manager != null) {
					manager.LayoutManager.LayoutChanged(); 
				}
			}
			else {
				if(zone != null) {
					EndDockingByDockZone(zone, new Point(zone.Bounds.X + zone.Bounds.Width / 2, zone.Bounds.Y + zone.Bounds.Height / 2), pt);
				}
			}
			if(zone == null || DockSource.Dock == DockingStyle.Float && !(zone is FloatDockZone)) {
				EndDockingEventArgs e = new EndDockingEventArgs(DockSource.Panel, false);
				if(manager != null) {
					manager.RaiseEndDocking(e);
					endDockingRised = true;
					manager.LayoutManager.LayoutChanged();
				}
				if(e.Canceled || !DockSource.Panel.Options.AllowFloating)
					OnEndDockingCanceled();
			}
			if(manager != null)
				manager.LayoutManager.FireChanged();
		}
		protected virtual void OnEndDockingCanceled() {
			DockSource.Panel.Restore();
		}
		protected override void EndDocking(Point pt) {
			EndDockingVS2005(pt);
			Owner.Capture = false;
		}
		public override void Reset() {
			base.Reset();
			if(hiddenForm != null && inTabState == InTabDraggingState.PanelDraggedInTabCaptionRegion) {
				inTabState = InTabDraggingState.PanelDraggedOutOfTabCaptionRegion;
				ResetTabCaptionRegionPanelDragging();
			}
			generalVisualizer.Hide();
			dockZoneVisualizer.Hide();
		}
		protected void UpdateVisualizerBounds() {
			if(DockPanelCore != null) {
				DockPanel panel = GetTabParent();
				if(panel != null)
					dockZoneVisualizer.Bounds = new Rectangle(panel.PointToScreen(Point.Empty), panel.Size);
				else
					dockZoneVisualizer.Bounds = new Rectangle(DockPanelCore.PointToScreen(Point.Empty), DockPanelCore.Size);
			}
			Rectangle restBoundsClient = DockManager.LayoutManager.GetRestBounds(null);
			Rectangle restBoundsScreen = new Rectangle(DockManager.Form.PointToScreen(restBoundsClient.Location), restBoundsClient.Size);
			generalVisualizer.RestBounds = restBoundsScreen;
			generalVisualizer.Bounds = GetScreenVisualizerBounds();
		}
		protected virtual Rectangle GetScreenVisualizerBounds() {
			return Owner.DockManager.GetScreenClientBounds();
		}
		protected bool CheckDockOptions(DockPanel panel, DockingStyle dockStyle) {
			if(DockSource == null) return true;
			DockPanel dockSourceCore = DockSource.Panel;
			if(!dockSourceCore.Options.AllowDockLeft && dockStyle == DockingStyle.Left) return false;
			if(!dockSourceCore.Options.AllowDockRight && dockStyle == DockingStyle.Right) return false;
			if(!dockSourceCore.Options.AllowDockTop && dockStyle == DockingStyle.Top) return false;
			if(!dockSourceCore.Options.AllowDockBottom && dockStyle == DockingStyle.Bottom) return false;
			return CheckDockFillOption(panel, dockStyle);
		}
		protected bool CheckDockFillOption(DockPanel panel, DockingStyle dockStyle) {
			DockPanel dockSourceCore = DockSource.Panel;
			if(dockSourceCore.Options.AllowDockFill) return true;
			if(dockStyle == DockingStyle.Fill) return false;
			if(dockPanel != null) {
				switch(dockPanel.Dock) {
					case DockingStyle.Top:
					case DockingStyle.Bottom:
						return dockStyle != DockingStyle.Left & dockStyle != DockingStyle.Right;
					case DockingStyle.Left:
					case DockingStyle.Right:
						return dockStyle != DockingStyle.Top & dockStyle != DockingStyle.Bottom;
					case DockingStyle.Float:
						return false;
				}
			}
			return true;
		}
		protected bool RequestVisibility(DockPanel panel, DockingStyle dockStyle, bool isTabbed) {
			if(!CheckDockOptions(panel, dockStyle)) return false;
			DockingEventArgs e = new DockingEventArgs(DockSource.Panel, panel, Point.Empty, dockStyle, isTabbed, 0);
			DockManager.RaiseDocking(e);
			if(e.Cancel)
				return false;
			return true;
		}
		protected virtual VisualizerVisibilityArgs RequestVisibilityPanel() {
			VisualizerVisibilityArgs args = new VisualizerVisibilityArgs();
			bool suppressAutoHidePanelHints = CheckVisibility(DockPanelCore);
			args.Top = suppressAutoHidePanelHints && RequestVisibility(DockPanelCore, DockingStyle.Top, false);
			args.Bottom = suppressAutoHidePanelHints && RequestVisibility(DockPanelCore, DockingStyle.Bottom, false);
			args.Left = suppressAutoHidePanelHints && RequestVisibility(DockPanelCore, DockingStyle.Left, false);
			args.Right = suppressAutoHidePanelHints && RequestVisibility(DockPanelCore, DockingStyle.Right, false);
			args.Tabbed = suppressAutoHidePanelHints && RequestVisibility(DockPanelCore, DockingStyle.Fill, true);
			return args;
		}
		protected virtual VisualizerVisibilityArgs RequestVisibilityGlobal() {
			VisualizerVisibilityArgs args = new VisualizerVisibilityArgs();
			args.Top = RequestVisibility(null, DockingStyle.Top, false);
			args.Bottom = RequestVisibility(null, DockingStyle.Bottom, false);
			args.Left = RequestVisibility(null, DockingStyle.Left, false);
			args.Right = RequestVisibility(null, DockingStyle.Right, false);
			args.Tabbed = false;
			return args;
		}
		bool CheckVisibility(DockPanel panel) {
			if(panel.Parent is DockPanel)
				return ((DockPanel)panel.Parent).Visibility == DockVisibility.Visible;
			return panel.Visibility == DockVisibility.Visible;
		}
		protected virtual void UpdateVisualizers(Point screenPt) {
			if(PressInfo.GetDockingSourcePanel(Owner) != DockPanelCore) {
				Form form = DockManager.Form as Form;
				if(form != null && form.IsMdiChild && form.MdiParent != null) {
					if(form != form.MdiParent.ActiveMdiChild)
						form.BringToFront();
				}
				UpdateVisualizerBounds();
				if(DockPanelCore != null) {
					generalVisualizer.ShowAllButCenter(RequestVisibilityGlobal());
					if(DockPanelCore.Count > 0) {
						if(DockPanelCore.Tabbed)
							dockZoneVisualizer.ShowCenter(RequestVisibilityPanel());
					}
					else
						dockZoneVisualizer.ShowCenter(RequestVisibilityPanel());
				}
				else {
					dockZoneVisualizer.Hide();
					if(generalVisualizer.RestBounds.Contains(Control.MousePosition)) {
						generalVisualizer.ShowAll(RequestVisibilityGlobal());
					}
					else
						generalVisualizer.ShowAllButCenter(RequestVisibilityGlobal());
				}
				UpdateVisualizerHotTrack(screenPt);
			}
		}
		protected virtual void UpdateVisualizerHotTrack(Point screenPoint) {
			if(!dockZoneVisualizer.UpdateHotTracked(screenPoint, true, GetTabsPosition(), GetHeaderRect())) {
				generalVisualizer.UpdateHotTracked(screenPoint, false, GetTabsPosition());
			}
		}
		Rectangle GetHeaderRect() {
			if((DockZoneCore is TabVisualDockZone)) {
				if(DockZoneCore is TabVisualTabPanelDockZone && (DockZoneCore as TabVisualTabPanelDockZone).CanEmulateDocking)
					return Rectangle.Empty;
				return (DockZoneCore as TabVisualDockZone).HeaderRect;
			}
			return Rectangle.Empty;
		}
		void OnDockPanelCoreChanged() {
			if(DockPanelCore != null)
				dockZoneVisualizer.Hide();
		}
		protected DockPanel DockPanelCore {
			get {
				return dockPanel;
			}
			set {
				if(DockPanelCore == value) return;
				dockPanel = value;
				OnDockPanelCoreChanged();
			}
		}
	}
	public class DockingHandlerState : BaseHandlerState {
		protected DockZone dockZone;
		protected bool shouldDrawPointers = true;
		public DockingHandlerState(DockPanelMouseHandler handler)
			: base(handler) {
			this.dockZone = null;
		}
		public override void Init(HitInfo hitInfo) {
			StartDocking(hitInfo.HitPoint);
		}
		public override void MouseMove(HitInfo hitInfo) {
			DoDocking(hitInfo.HitPoint);
		}
		public override void MouseUp(HitInfo hitInfo) {
			EndDocking(hitInfo.HitPoint);
		}
		protected bool endDockingRised = false;
		protected virtual void StartDocking(Point pt) {
			MouseOver(pt);
			endDockingRised = false;
		}
		protected virtual void DoDocking(Point pt) {
			MouseOver(pt);
		}
		protected bool TargetFormVisible() {
			return this.Handler.Owner.DockManager.Form.Visible;
		}
		protected virtual void MouseOver(Point pt) {
			Point screenPt = PointToScreen(pt);
			DockZone targetZone = GetDockZoneAtPos(screenPt);
			if(targetZone.DockStyle != DockingStyle.Float) {
				DockingEventArgs e = new DockingEventArgs(DockSource.Panel, targetZone.TargetControl, screenPt, targetZone.DockStyle, targetZone.TargetTabbed, targetZone.Index);
				DockManager.RaiseDocking(e);
				if(e.Cancel)
					targetZone = CreateFloatZone(screenPt);
			}
			if(!TargetFormVisible()) {
				targetZone = CreateFloatZone(screenPt);
			}
			DockZoneCore = targetZone;
			DockZoneCore.MouseOver(DockSource, screenPt);
		}
		DockLayout oldDockSource = null;
		protected virtual void EndDocking(Point pt) {
			DockZone dest = DockZoneCore;
			DockManager manager = DockManager;
			EndDockingEventArgs e = new EndDockingEventArgs(DockSource.Panel, false);
			if(DockSource != null) oldDockSource = DockSource; 
			dest.BeginDock();
			try {
				DockZoneCore = null;
				if(DockSource.Panel != null && DockSource.Float && !IsFloatDockZone(dest)) {
					Form frm = manager.Form as Form;
					if(frm != null) frm.Activate();
				}
				dest.Dock(DockSource);
				if(DockSource.Panel != null && DockSource.Panel.DockManager != null) {
					DockSource.Panel.DockManager.ActivePanel = DockSource.Panel;
				}
			}
			finally {
				dest.EndDock();
			}
			manager.RaiseEndDocking(e);
			endDockingRised = true;
		}
		protected bool IsFloatDockZone(DockZone dest) {
			if(dest.DockStyle == DockingStyle.Float)
				return true;
			if(dest.TargetControl != null)
				return dest.TargetControl.FindForm() is FloatForm;
			return false;
		}
		public override void Reset() {
			EndDockingEventArgs e = new EndDockingEventArgs(DockSource.Panel, true);
			DockZoneCore = null;
			if(!endDockingRised && DockManager != null)
				DockManager.RaiseEndDocking(e);
		}
		public override void ModifierKey(Keys key) {
			DoDocking(Owner.PointToClient(Cursor.Position));
		}
		protected virtual DockZone GetDockZoneAtPos(Point screenPoint) {
			if((Control.ModifierKeys & Keys.Control) != 0) return CreateFloatZone(screenPoint);
			DockZone result = null;
			DockPanel panel = DockManager.GetDockPanelAtPos(screenPoint);
			if(panel != null && panel != DockSource.Panel && !DockSource.Panel.HasAsChild(panel))
				result = panel.DockLayout.GetDockZoneAtPos(screenPoint);
			else
				result = ((IDockZonesOwner)DockLayout.Manager).DockZones[screenPoint];
			return (result == null ? CreateFloatZone(screenPoint) : result);
		}
		protected FloatDockZone CreateFloatZone(Point screenPoint) {
			double factor = (double)DockSource.FloatSize.Width / (double)DockSource.Size.Width;
			if(!double.IsNaN(factor) && !double.IsInfinity(factor))
				screenPoint.X -= (int)((double)PressInfo.HitPoint.X * factor);
			int cy = (DockSource == DockLayout) ? 0 : (DockSource.FloatSize.Height - DockLayout.Size.Height);
			screenPoint.Y -= PressInfo.HitPoint.Y + cy;
			Point cursorPosition = Cursor.Position;
			if(cursorPosition.Y < screenPoint.Y)
				screenPoint.Y = cursorPosition.Y - PressInfo.HitPoint.Y;
			if(cursorPosition.X > screenPoint.X + DockSource.FloatSize.Width)
				screenPoint.X = cursorPosition.X - DockSource.FloatSize.Width + (Owner.Width - PressInfo.HitPoint.X);
			return new FloatDockZone(DockSource, screenPoint);
		}
		protected Point PointToScreen(Point pt) { return Owner.IsDisposed ? Point.Empty : Owner.PointToScreen(pt); }
		protected virtual DockZone DockZoneCore {
			get { return dockZone; }
			set {
				if(DockZoneCore == value && DockZoneCore != null && DockZoneCore.Bounds == value.Bounds) return;
				if(DockZoneCore != null && DockSource != null && !DockSource.IsParentUpdateLocked) {
					if(shouldDrawPointers) DockZoneCore.DrawPointer(DockSource);
					DockZoneCore.MouseLeave(DockSource);
				}
				dockZone = value;
				CheckCursor(DockZoneCore);
			}
		}
		protected virtual void CheckCursor(DockZone dockZone) {
			if(dockZone != null) {
				if(shouldDrawPointers) dockZone.DrawPointer(DockSource);
				Cursor.Current = dockZone.Cursor;
			}
			else Cursor.Current = Cursors.Default;
		}
		protected DockLayout DockSource {
			get {
				DockLayout result = null;
				if(PressInfo.GetDockingSourcePanel(Owner) == null)
					result = oldDockSource;
				else
					result = PressInfo.GetDockingSourcePanel(Owner).DockLayout;
				if(result == null) result = Owner.DockLayout;
				return result;
			}
		}
		public sealed override DockPanelState State { get { return DockPanelState.Docking; } }
	}
}
