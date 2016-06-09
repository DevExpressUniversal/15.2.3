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
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Extensions;
using DevExpress.Utils.Win.Hook;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Customization;
using DevExpress.XtraLayout.Handlers;
using DevExpress.XtraLayout.HitInfo;
using DevExpress.XtraLayout.Utils;
namespace DevExpress.XtraDashboardLayout {
	[ToolboxItem(false)]
	public class DashboardLayoutControl : LayoutControl, IToolFrameOwner {
		internal Adorner adorner;
		protected internal AdornerWindowHandler handler;
		DashboardLayoutControlHookController hookController;
		bool applicationActive = true;
		bool allowSelection, allowResize, allowCrosshair, allowDragDrop, drawItemGroupBordersCore = true, allowImageColorizationCore = true;
		public event EventHandler<CustomContextMenuEventArgs> CustomContextMenu;
		public event EventHandler DashboardLayoutChanging;
		public event EventHandler DashboardLayoutChanged;
		public event EventHandler<DashboardCustomContextMenuEventArgs> DashboardLayoutGetCaptionImageToolTip;
#if !SL
	[DevExpressXtraLayoutLocalizedDescription("LayoutControlRoot")]
#endif
		public new DashboardLayoutControlGroupBase Root {
			get { return base.Root as DashboardLayoutControlGroupBase; }
			set { base.Root = value; }
		}
		public AdornerWindowHandlerStates AdornerWindowHandlerState { get { return handler.State; } }
		[DefaultValue(false)]
		public bool DrawItemGroupBorders {
			get { return drawItemGroupBordersCore; }
			set {
				if(drawItemGroupBordersCore != value) {
					drawItemGroupBordersCore = value;
					ResetAllViewInfo();
				}
			}
		}
		[DefaultValue(true)]
		public bool AllowCrosshair {
			get { return allowCrosshair; }
			set {
				if(allowCrosshair != value) {
					allowCrosshair = value;
					if(handler != null)
						handler.RefreshBehaviours();
				}
			}
		}
		[DefaultValue(true)]
		public bool AllowDragDrop {
			get { return allowDragDrop; }
			set {
				if(allowDragDrop != value) {
					allowDragDrop = value;
					if(handler != null)
						handler.RefreshBehaviours();
				}
			}
		}
		internal bool DashboardAdornerWindowVisible { get { return AllowCrosshair || AllowDragDrop; } } 
		[DefaultValue(true)]
		public bool AllowSelection {
			get { return allowSelection; }
			set {
				if(allowSelection != value) {
					allowSelection = value;
					if(handler != null) {
						if(!allowSelection)
							handler.SelectedItem = null;
						handler.RefreshBehaviours();
					}
				}
			}
		}
		[DefaultValue(true)]
		public bool AllowResize {
			get { return allowResize; }
			set {
				if(allowResize != value) {
					allowResize = value;
					if(handler != null) {
						handler.RefreshBehaviours();
					}
				}
			}
		}
		[DefaultValue(true)]
		public bool AllowImageColorization {
			get { return allowImageColorizationCore; }
			set {
				if(allowImageColorizationCore != value) {
					allowImageColorizationCore = value;
					Refresh();
				}
			}
		}
		internal bool ApplicationActive {
			get { return applicationActive; }
			set {
				if(applicationActive != value) {
					applicationActive = value;
				}
			}
		}
		protected override bool CalcIsActiveAndCanProcessEvent() {
			return Enabled && Visible && AllowDragDrop;
		}
		protected override bool CalcAllowProcessDragging() {
			bool fIsMainControl = ((this as ILayoutControl).ControlRole == LayoutControlRoles.MainControl);
			return fIsMainControl && (AllowDragDrop);
		}
		protected override LayoutControlDragDropHelper CreateDragDropHelper() {
			return new DashboardLayoutControlDragDropHelper(this);													  
		}
		protected override ToolTipControlInfo GetToolTipObjectInfo(Point pt) {
			DashboardHitInfo dihi = CalcHitInfo(pt) as DashboardHitInfo;
			ToolTipControlInfo ttcInfo = null;
			if(dihi != null && dihi.ButtonIndex >= 0) {
				DashboardLayoutControlItemBase item = dihi.Item as DashboardLayoutControlItemBase;
				if(item != null && item.CustomHeaderButtons != null && item.ViewInfo != null)
				   ttcInfo =  (item.ViewInfo.BorderInfo as GroupObjectInfoArgs).ButtonsPanel.GetObjectInfo(pt);
				DashboardLayoutControlGroupBase group = dihi.Item as DashboardLayoutControlGroupBase;
				if(group != null && group.CustomHeaderButtons != null && group.ViewInfo != null)
					ttcInfo = (group.ViewInfo.BorderInfo as GroupObjectInfoArgs).ButtonsPanel.GetObjectInfo(pt);
			}
			if(ttcInfo != null) return ttcInfo;
			return base.GetToolTipObjectInfo(pt);
		}
		[DefaultValue(null)]
		public BaseLayoutItem SelectedItem {
			get {
				CheckHandler();
				return handler.SelectedItem;
			}
			set {
				CheckHandler();
				handler.SelectedItem = value;
			}
		}
		protected void CheckHandler() {
			if(handler == null)
				ForceCreateHandler();
		}
		[DefaultValue(null)]
		public BaseLayoutItem HotTrackedItem {
			get {
				CheckHandler();
				return handler.HotTrackedItem;
			}
			set {
				CheckHandler();
				handler.HotTrackedItem = value;
			}
		}
		public DashboardLayoutControl() {
			SetDefaults();
		}
		protected virtual void SetDefaults() {
			OptionsView.DrawItemBorders = true;
			OptionsView.AllowHotTrack = true;
			OptionsView.AllowItemSkinning = true;
			AllowCrosshair = true;
			AllowDragDrop = true;
			AllowSelection = true;
			AllowResize = true;
		}
		void ForceCreateHandler() {
			if(handler == null) handler = CreateHandler();
			if(hookController == null)
				hookController = CreateHookController();
		}
		protected virtual DashboardLayoutControlHookController CreateHookController() {
			return new DashboardLayoutControlHookController(this);
		}
		internal void RaiseDashboardLayoutChanging() {
			if(DashboardLayoutChanging != null)
				DashboardLayoutChanging(this, EventArgs.Empty);
		}
		internal void RaiseDashboardLayoutChanged() {
			if(DashboardLayoutChanged != null)
				DashboardLayoutChanged(this, EventArgs.Empty);
		}
		internal void RaiseCustomContextMenu(Point point) {
			if(CustomContextMenu != null) {
				CustomContextMenu(this, new CustomContextMenuEventArgs() { Point = point });
			}
		}
		protected override bool ProcessDialogKey(Keys keyData) {
			if(handler == null) return base.ProcessDialogKey(keyData);
			if(!handler.ProcessEvent(EventType.KeyDown, this, null, new KeyEventArgs(keyData)))
				return base.ProcessDialogKey(keyData);
			else return true;
		}
		internal void RaiseMouseMove(object sender, MouseEventArgs e) {
			Point client = PointToClient(e.Location);
			OnMouseMove(new MouseEventArgs(e.Button, e.Clicks, client.X, client.Y, e.Delta));
		}
		internal void RaiseMouseDown(object sender, MouseEventArgs e) {
			Point client = PointToClient(e.Location);
			OnMouseDown(new MouseEventArgs(e.Button, e.Clicks, client.X, client.Y, e.Delta));
		}
		internal void RaiseMouseEnter(object sender, EventArgs e) {
			OnMouseEnter(e);
		}
		internal void RaiseMouseLeave(object sender, EventArgs e) {
			OnMouseLeave(e);
		}
		internal void RaiseGetCaptionImageToolTip(Point point, LayoutGroupHitTypes layoutGroupHitTypes, BaseLayoutItem item) {
			if(DashboardLayoutGetCaptionImageToolTip != null)
				DashboardLayoutGetCaptionImageToolTip(this, new DashboardCustomContextMenuEventArgs() { Point = point, LayoutGroupHitTypes = layoutGroupHitTypes, Item = item });
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(hookController != null)
				hookController.Dispose();
		}
		protected virtual AdornerWindowHandler CreateHandler() {
			return new AdornerWindowHandler(this);
		}
		protected override LayoutControlImplementor CreateILayoutControlImplementorCore() {
			return new DashboardLayoutControlImplementor(this);
		}
		protected override void OnVisibleChanged(EventArgs e) {
			base.OnVisibleChanged(e);
			if(adorner != null) {
				if(Visible) {
					if(ApplicationActive)
						adorner.Show();
				}
				else
					adorner.Hide();
			}
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			ForceCreateHandler();
			if(DashboardAdornerWindowVisible) {
				adorner = new Adorner(this);
				adorner.Show();
			}
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			if(adorner != null) adorner.Hide();
		}
		protected override LayoutControlCustomizeHandler CreateLayoutControlCustomizeHandler() {
			return new LayoutControlCustomizeHandler(this);
		}
		protected override LayoutControlHandler CreateLayoutControlRuntimeHandler() {
			LayoutControlHandler lch = new DashboardLayoutControlHandler(this);
			lch.AllowSetCursor = false;
			return lch;
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			if(handler != null && handler.ProcessEvent(DevExpress.Utils.Controls.EventType.MouseDown, this, e, null))
				InvokeBaseMouseDown(e);
			else
				base.OnMouseDown(e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			if(handler != null && handler.ProcessEvent(DevExpress.Utils.Controls.EventType.MouseUp, this, e, null))
				InvokeBaseMouseUp(e);
			else {
				if(handler == null && e.Button == MouseButtons.Right) RaiseCustomContextMenu(e.Location);
				base.OnMouseUp(e);
			}
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			if(handler != null && handler.ProcessEvent(DevExpress.Utils.Controls.EventType.MouseMove, this, e, null))
				InvokeBaseMouseMove(e);
			else
				base.OnMouseMove(e);
		}
		protected override void OnMouseEnter(EventArgs e) {
			if(handler != null && handler.ProcessEvent(DevExpress.Utils.Controls.EventType.MouseEnter,this, null, null))
				InvokeBaseMouseEnter(e);
			else
				base.OnMouseEnter(e);
		}
		protected override void OnMouseLeave(EventArgs e) {
			if(handler != null && handler.ProcessEvent(DevExpress.Utils.Controls.EventType.MouseLeave, this, null, null))
				InvokeBaseMouseLeave(e);
			else
				base.OnMouseLeave(e);
		}
		public void UngroupInvisibleGroups(XtraLayout.Adapters.XAFLayoutItemInfo xafRoot) {
			if(Root == null) return;
			if(Root.Items == null) return;
			UngroupGroup(Root);
			List<Crosshair> crossCollection = Root.GetFlatCrosshairs();
			if(crossCollection.Count == 0) return;
			if(!(xafRoot is DevExpress.XtraLayout.Adapters.XAFLayoutGroupInfo)) return;
			SetCrosshairsType(crossCollection, xafRoot as XtraLayout.Adapters.XAFLayoutGroupInfo);
			Root.Resizer.CreateNewResizing();
		}
		private void SetCrosshairsType(List<Crosshair> crossCollection, XtraLayout.Adapters.XAFLayoutGroupInfo xafRoot) {
			List<XtraLayout.Adapters.XAFLayoutGroupInfo> flatList = xafRoot.GetFlatGroup();
			foreach(Crosshair cross in crossCollection) {
				bool wasSetCrossType = false;
				foreach(XtraLayout.Adapters.XAFLayoutGroupInfo groupInfo in flatList) {
					switch(groupInfo.LayoutType) {
						case LayoutType.Horizontal:
							cross.GetTwoItems(false, LayoutType.Horizontal);
							if(groupInfo.ContainsCrossItems(cross)) {
								cross.CrosshairGroupingType = CrosshairGroupingTypes.GroupHorizontal;
								wasSetCrossType = true;
							}
							break;
						case LayoutType.Vertical:
							if(groupInfo.ContainsCrossItems(cross)) {
								cross.CrosshairGroupingType = CrosshairGroupingTypes.GroupVertical;
								wasSetCrossType = true;
							}
							break;
					}
					if(wasSetCrossType) break;
				}			
			}
		}
		protected override void CreateAdornerHandlerAndWindow(){
		}
		protected override void ShowAdorner() {
		}
		void UngroupGroup(LayoutGroup group) {
			List<BaseLayoutItem> copyList = new List<BaseLayoutItem>(group.Items);
			foreach(BaseLayoutItem item in copyList) {
				if(item.IsGroup) {
					UngroupGroup((LayoutGroup)item);
				}
			}
			DashboardLayoutControlGroupBase dashboardGroup = group as DashboardLayoutControlGroupBase;
			if(dashboardGroup != null && dashboardGroup.IsHiddenGroup)
				group.UngroupItems();
		}
	}
	public class DashboardLayoutControlHandler : LayoutControlHandler {
		public DashboardLayoutControlHandler(ILayoutControl owner) : base(owner) { }
		public override void OnTimer() {
			base.OnTimer();
			DashboardLayoutControl dlc = Owner as DashboardLayoutControl;
			if(dlc == null) return;
			if(dlc.handler == null) return;
			if(dlc.adorner == null) return;
			if(dlc.adorner.adornerWindow == null) return;
			if(dlc.adorner.adornerWindow.IsVisible && (!dlc.handler.CheckCursorOnLayout() || dlc.handler.InvalidateOnTimer)) {
				if(dlc.handler.InvalidateOnTimer) dlc.handler.InvalidateOnTimer = false;
				dlc.handler.Invalidate();
			}
		}
		protected override void InvalidateLayoutAdornerHandler() {
		}
	}
	public class DashboardLayoutControlHookController : IHookController, IHookControllerWithResult, IDisposable {
		readonly DashboardLayoutControl owner;
		public DashboardLayoutControlHookController(DashboardLayoutControl owner) {
			this.owner = owner;
			HookManager.DefaultManager.AddController(this);
		}
		#region IHookController Members
		bool IHookController.InternalPostFilterMessage(int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr lParam) {
			return false;
		}
		protected Point TransformPointToOwner(Control client, IntPtr lParam) {
			return owner.PointToClient(lParam.PointFromLParam());
		}
		bool AllowProcessMouseEvent(Point p, Control wnd) {
			Control current = wnd;
			while(current != owner && current != null) 
				current = current.Parent;
			return current == owner && owner.Visible && owner.DisplayRectangle.Contains(p);
		}
		protected virtual bool CheckIfHookingAllowedForControl(Control wnd) {
			if(wnd is VScrollBarBase || wnd is HScrollBarBase) return false;
			return true;
		}
		HookResult result = HookResult.NotProcessed;
		bool IHookController.InternalPreFilterMessage(int Msg, Control wnd, IntPtr HWnd, IntPtr WParam, IntPtr lParam) {
			bool res = false;
			try {
				if(owner != null) {
					if(Msg == MSG.WM_ACTIVATEAPP) 
						owner.ApplicationActive = WParam != IntPtr.Zero;
					if(wnd != owner && wnd != null && CheckIfHookingAllowedForControl(wnd)) {
						Form form = wnd.FindForm();
						Form ownerform = owner.FindForm();
						if(form == ownerform) {
							if(Msg == MSG.WM_LBUTTONDOWN || Msg == MSG.WM_RBUTTONDOWN || Msg == MSG.WM_LBUTTONUP || Msg == MSG.WM_RBUTTONUP || Msg == MSG.WM_MOUSEMOVE) {
								Point p = TransformPointToOwner(wnd, lParam);
								if(!AllowProcessMouseEvent(p, wnd)) return false;
								if(Msg == MSG.WM_LBUTTONDOWN || Msg == MSG.WM_RBUTTONDOWN) {
									bool processEventResult = owner.handler.ProcessEvent(EventType.MouseDown, "HookEvent", new MouseEventArgs(Msg == MSG.WM_LBUTTONDOWN ? MouseButtons.Left : MouseButtons.Right, 0, p.X, p.Y, 0), null);
									res = processEventResult;
								}
								if(Msg == MSG.WM_LBUTTONUP || Msg == MSG.WM_RBUTTONUP) {
									owner.handler.ProcessEvent(EventType.MouseUp, "HookEvent",
										new MouseEventArgs(Msg == MSG.WM_LBUTTONUP ? MouseButtons.Left : MouseButtons.Right, 0, p.X, p.Y, 0), null);
								}
								if(Msg == MSG.WM_MOUSEMOVE) {
									owner.handler.ProcessEvent(EventType.MouseMove, "HookEvent", new MouseEventArgs(Control.MouseButtons, 0, p.X, p.Y, 0), null);
								}
							}
						}
					}
				}
			} catch {
			}
			result = res ? HookResult.ProcessedExit : HookResult.NotProcessed;
			return res;
		}
		IntPtr IHookController.OwnerHandle {
			get { return IntPtr.Zero; }
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
			HookManager.DefaultManager.RemoveController(this);
		}
		#endregion
		public HookResult Result {
			get {
				return result;
			}
			set {
				result = value;
			}
		}
	}
	public class CustomContextMenuEventArgs : EventArgs {
		public Point Point { get; set; }
	}
	public class DashboardCustomContextMenuEventArgs : CustomContextMenuEventArgs {
		public LayoutGroupHitTypes LayoutGroupHitTypes { get; set; }
		public BaseLayoutItem Item { get; set; }
	}
	public static class DashboardLayoutSettings {
		public static int DragIndicatorAlphaLevel {
			get {
				return dragIndicatorAlphaLevel;
			}
			set {
				if(value < 0 || value > 255) return;
				dragIndicatorAlphaLevel = value;
			}
		}
		public static int ResizingAreaThickness {
			get {
				return resizingAreaThickness;
			}
			set {
				if(value <= 0) return;
				resizingAreaThickness = value;
			}
		}
		public static int DragDropIndicatorSize {
			get {
				return dragDropIndicatorSize;
			}
			set {
				if(value < 0) return;
				dragDropIndicatorSize = value;
			}
		}
		public static int DragIndicatorShowTimeout {
			get {
				return dragIndicatorShowTimeout;
			}
			set {
				if(value <= 0) return;
				dragIndicatorShowTimeout = value;
			}
		}
		public static int DropIndicatorUpdateTimeout {
			get {
				return dropIndicatorUpdateTimeout;
			}
			set {
				if(value <= 0) return;
				dropIndicatorUpdateTimeout = value;
			}
		}
		static int dropIndicatorUpdateTimeout = 800;
		static int dragIndicatorShowTimeout = 200;
		static int dragDropIndicatorSize = 15;
		static int resizingAreaThickness = 6;
		static int dragIndicatorAlphaLevel = 100;
		internal const int buttonToButtonDistance = 4;
	}
}
