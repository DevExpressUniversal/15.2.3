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
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.XtraBars.Docking.Helpers;
using DevExpress.XtraBars.Docking.Paint;
using DevExpress.XtraEditors;
namespace DevExpress.XtraBars.Docking {
	public enum AutoHiddenPanelCaptionShowMode {
		ShowForAllPanels, ShowForActivePanel
	}
	[ToolboxItem(false)]
	public abstract class ZIndexControl : ContainerControl {
		bool isDisposing;
		protected ZIndexControl() {
			this.isDisposing = false;
#if DXWhidbey
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
#else
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlConstants.DoubleBuffer, true);
#endif
		}
		protected override void Dispose(bool disposing) {
			if(disposing && !IsDisposing) {
				this.isDisposing = true;
				DisposeCore();
				if(Parent is IContainerControl && ((IContainerControl)Parent).ActiveControl == this) ((IContainerControl)Parent).ActiveControl = null;
			}
			base.Dispose(disposing);
		}
		protected virtual void DisposeCore() { }
		protected override bool ProcessTabKey(bool forward) {
			if(this.SelectNextControl(this.ActiveControl, forward, true, true, true))
				return true;
			else
				return false;
		}
		protected override void WndProc(ref System.Windows.Forms.Message m) {
			const int WM_CAPTURECHANGED = 0x215;
			if(m.Msg == WM_CAPTURECHANGED) {
				if(m.LParam == this.Handle)
					OnGotCapture();
				else
					OnLostCapture();
			}
			base.WndProc(ref m);
			CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref m, this);
		}
		protected virtual void OnLostCapture() { }
		protected virtual void OnGotCapture() { }
		internal bool IsMouseInside { get { return IsPointInside(MousePosition); } }
		internal bool IsPointInside(Point ptScreen) { return ClientRectangle.Contains(PointToClient(ptScreen)); }
		public abstract HitInfo GetHitInfo(Point ptClient);
		protected internal int ZIndex {
			get {
				if(Parent == null) return LayoutConsts.InvalidIndex;
				return GetZIndex();
			}
			set {
				if(ZIndex == value || Parent == null) return;
				Parent.Controls.SetChildIndex(this, value);
			}
		}
		protected virtual int GetZIndex() {
			return Parent.Controls.GetChildIndex(this);
		}
		protected internal bool IsDisposing { get { return isDisposing; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), XtraSerializableProperty()]
		public int XtraZIndex { get { return ZIndex; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		new public int TabIndex { get { return base.TabIndex; } set { base.TabIndex = value; } }
	}
	[Designer("DevExpress.XtraBars.Docking.Design.AutoHideContainerDesigner, " + AssemblyInfo.SRAssemblyBarsDesign, typeof(System.ComponentModel.Design.IDesigner))]
	[DesignerSerializer("DevExpress.XtraBars.Docking.Design.AutoHideContainerSerializer, " + AssemblyInfo.SRAssemblyBarsDesign, "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design")]
	public class AutoHideContainer : ZIndexControl {
		DockLayoutManager manager;
		AutoHideInfo autoHideInfo;
		AppearanceObject viewAppearance, activeHidePanelButtonAppearance, inactiveHidePanelButtonAppearance;
		int containerSize;
		int updateCounter;
		DockLayout hotLayout;
		AutoHideContainerViewInfo viewInfoCore;
		protected internal int? SavedContainerSize { get; set; }
		public AutoHideContainer() {
			this.manager = null;
			this.autoHideInfo = null;
			this.containerSize = 0;
			this.updateCounter = 0;
			this.hotLayout = null;
			this.viewInfoCore = CreateViewInfo();
			this.viewAppearance = new AppearanceObject();
			this.activeHidePanelButtonAppearance = new AppearanceObject();
			this.inactiveHidePanelButtonAppearance = new AppearanceObject();
			SetStyle(ControlStyles.Selectable, false);
		}
		protected override void OnSizeChanged(EventArgs e) {
			base.OnSizeChanged(e);
			if(Manager != null) {
				if(Manager.DockManager.needUpdateControlsZOrder) {
					Manager.DockManager.UpdateControlsZOrder();
					Manager.DockManager.needUpdateControlsZOrder = false;
				}
				Manager.CheckDecreaseSize();
			}
		}
		protected internal AutoHideContainerViewInfo ViewInfo {
			get { return viewInfoCore; }
		}
		protected virtual AutoHideContainerViewInfo CreateViewInfo() {
			return new AutoHideContainerViewInfo(this);
		}
		internal void BeforeSerialize(IDesignerSerializationManager manager) {
#if DXWhidbey
			if(Disposing || !IsHandleCreated) return;
			if(Manager != null) Manager.BeforeSerialize(manager); 
#endif
		}
		protected override void DisposeCore() {
			if(Manager != null) {
				Manager.AutoHideContainerDisposing(this);
				Manager = null;
			}
			ViewAppearance.Dispose();
			ActiveHidePanelButtonAppearance.Dispose();
			InactiveHidePanelButtonAppearance.Dispose();
		}
		protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
			if(IsInitialized && specified != BoundsSpecified.None) {
				if(IsHorizontal) height = ContainerSize;
				else width = ContainerSize;
			}
			base.SetBoundsCore(x, y, width, height, specified);
		}
		protected override void OnControlAdded(ControlEventArgs e) {
			DockPanel panel = e.Control as DockPanel;
			if(panel == null)
				throw new WarningException(string.Format(DockConsts.InvalidAutoHideContainerChildFormatString, e.Control.GetType().FullName));
			panel.OnAddIntoAutoHideContainer();
		}
		protected override void OnPaint(PaintEventArgs e) {
			if(Manager == null || Manager.DockManager == null || Manager.DockManager.PaintingSuspended) return;
			using(GraphicsCache cache = new GraphicsCache(e)) {
				ViewInfo.UpdateBeforePaint(cache, ClientRectangle);
				ObjectPainter.DrawObject(cache, HideBarPainter, ViewInfo);
			}
		}
		protected internal AppearanceObject GetButtonHeaderAppearance(bool hot) {
			return hot ? ActiveHidePanelButtonAppearance : InactiveHidePanelButtonAppearance;
		}
		protected internal bool IsHotButton(DockLayout dockLayout) {
			DockLayout realHotLayout = Manager.GetHotLayout(this);
			if(realHotLayout == null) return false;
			return (dockLayout == realHotLayout || dockLayout.ActiveChild == realHotLayout);
		}
		protected bool IsHotButton(LayoutInfo info) {
			return IsHotButton((DockLayout)info);
		}
		protected bool IsHotButton(int index) {
			return IsHotButton((DockLayout)AutoHideInfo[index]);
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			LayoutChanged();
		}
		protected override void OnParentChanged(EventArgs e) {
			base.OnParentChanged(e);
			OnParentChanged();
		}
		protected override void OnParentBackColorChanged(EventArgs e) {
			base.OnParentBackColorChanged(e);
			OnParentChanged();
		}
		void OnParentChanged() {
			UpdateBackColorSafe(this, GetStyle(ControlStyles.SupportsTransparentBackColor));
		}
		internal static void UpdateBackColorSafe(Control target, bool supportTBC) {
			if(target.Parent == null) return;
			if(target.Parent.BackColor == Color.Transparent) {
				if(supportTBC) target.BackColor = target.Parent.BackColor;
			}
			else {
				target.BackColor = target.Parent.BackColor;
			}
		}
		protected override void OnMouseLeave(EventArgs e) {
			UpdateHotLayout(LayoutConsts.InvalidPoint);
			base.OnMouseLeave(e);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			if(e.Button == MouseButtons.Right) {
				Manager.DockManager.DockController.ShowContextMenu(this, e.Location);
				return;
			}
			if((e.Button & MouseButtons.Left) == 0) return;
			HotLayout = GetHotLayout(e.Location);
			if(HotLayout != null) {
				if(Manager.DockManager.ActivePanel == HotLayout.Panel) {
					if(Manager.DockManager.CanAutoHideByClick)
						Manager.DockManager.LayoutManager.HideImmediately();
					return;
				}
				Manager.DockManager.ActivePanel = HotLayout.Panel;
			}
		}
		protected override void OnDragOver(DragEventArgs drgevent) {
			base.OnDragOver(drgevent);
			Point p = PointToClient(new Point(drgevent.X, drgevent.Y));
			UpdateHotLayout(p);
		}
		protected override void OnDragLeave(EventArgs e) {
			UpdateHotLayout(LayoutConsts.InvalidPoint);
			base.OnDragLeave(e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			UpdateHotLayout(e.Location);
		}
		void UpdateHotLayout(Point pt) {
			if(!IsInitialized) return;
			HotLayout = GetHotLayout(pt);
		}
		int showSlidingCore = 0;
		protected internal bool IsShowSliding {
			get { return showSlidingCore > 0; }
		}
		protected internal void BeginShowSliding(DockLayout layout) {
			if(layout == null || Manager == null) return;
			showSlidingCore++;
			hotLayout = layout;
			Manager.AutoHideHotLayoutChanged(this);
			ViewInfo.SetDirty();
		}
		protected internal void EndShowSliding() {
			showSlidingCore = 0;
			if(HotLayout != null && Manager != null) {
				Manager.DockManager.ActivePanel = HotLayout.Panel;
				hotLayout = null;
			}
		}
		protected internal DockLayout GetHotLayout() {
			if(!IsHandleCreated) return null;
			if(IsShowSliding) return hotLayout;
			return GetHotLayout(PointToClient(Control.MousePosition));
		}
		protected DockLayout GetHotLayout(Point pt) {
			HitInfo hi = GetHitInfo(pt);
			if(hi.Tab != null)
				return hi.Tab.DockLayout;
			return (hi.AutoHidePanel == null ? null : hi.AutoHidePanel.DockLayout);
		}
		protected internal void BeginUpdate() {
			updateCounter++;
		}
		protected internal void CancelUpdate() {
			updateCounter--;
		}
		protected internal void EndUpdate() {
			if(--updateCounter == 0)
				LayoutChanged();
		}
		protected bool IsLayoutLocked {
			get { return (this.updateCounter != 0 || !IsInitialized || IsDisposing); }
		}
		protected internal virtual void OnAutoHide() {
			LayoutChanged();
		}
		protected internal void OnAutoShow() {
			if(HotLayout == null) return;
			if(HotLayout.IsTab && HotLayout.LayoutParent.ActiveChild != HotLayout) {
				HotLayout.LayoutParent.ActiveChild = HotLayout;
				HotLayout.LayoutParent.Panel.FireChanged();
			}
			LayoutChanged();
		}
		protected internal void LayoutChanged() {
			if(IsLayoutLocked) return;
			ViewInfo.SetDirty();
			Invalidate();
		}
		protected internal void UpdateAppearances() {
			Parameters.InitHideContainerAppearance(ViewAppearance, AppearancesSettings.HideContainer, Position);
			Parameters.InitHidePanelButtonActiveAppearance(ActiveHidePanelButtonAppearance, AppearancesSettings.HidePanelButtonActive, Position);
			Parameters.InitHidePanelButtonAppearance(InactiveHidePanelButtonAppearance, AppearancesSettings.HidePanelButton, Position);
			ViewInfo.PaintAppearance.AssignInternal(ViewAppearance);
			ViewInfo.PaintAppearanceActiveHeader.AssignInternal(ActiveHidePanelButtonAppearance);
			ViewInfo.PaintAppearanceInactiveHeader.AssignInternal(InactiveHidePanelButtonAppearance);
			ViewInfo.PaintAppearance.TextOptions.RightToLeft = ViewInfo.IsRightToLeft;
			ViewInfo.PaintAppearanceActiveHeader.TextOptions.RightToLeft = ViewInfo.IsRightToLeft;
			ViewInfo.PaintAppearanceInactiveHeader.TextOptions.RightToLeft = ViewInfo.IsRightToLeft;
		}
		void AutoHideInfo_Changed(object sender, CollectionChangeEventArgs e) {
			DockLayout dockLayout = (DockLayout)e.Element;
			if(e.Action == CollectionChangeAction.Add) {
				Form mdiForm = dockLayout.Panel.Parent as Form;
				Form activeMdiChild = null;
				if(mdiForm != null) {
					activeMdiChild = mdiForm.ActiveMdiChild;
				}
				if(activeMdiChild != null) LayoutChanged(); 
				dockLayout.Panel.Parent = this;
				LayoutChanged();
				if(activeMdiChild != null) {
					activeMdiChild.BringToFront();
				}
			}
			if(e.Action == CollectionChangeAction.Remove) {
				dockLayout.Panel.UpdateControlVisible(dockLayout.Panel.Visible);
				dockLayout.Panel.UpdateChildPanelsVisibiblity();
				dockLayout.Panel.SetParent(null);
			}
		}
		protected internal void OnDeserialize(DockLayoutManager layoutManager) {
			BeginUpdate();
			try {
				if(Controls.Count == 0) {
					layoutManager.AutoHideContainers.Remove(this);
					return;
				}
				foreach(DockPanel panel in Controls) {
					if(panel.DockManager != null && panel.DockLayout.SavedInfo != null) {
						if(Dock != (DockStyle)panel.DockLayout.SavedInfo.AutoHideDock) {
							panel.DockLayout.SavedInfo.SavedIndex = -1;
							panel.DockLayout.SavedInfo.SavedParent = null;
							panel.DockLayout.SavedInfo.SavedDock = panel.Dock;
						}
					}
					layoutManager.OnLayoutInfoAutoHideChanged(panel.DockLayout);
				}
				Manager = layoutManager;
				for(int i = 0; i < Count; i++)
					this[i].OnDeserialize(layoutManager);
				ContainerSize = layoutManager.CalcAutoHidePanelSize(this);
			}
			finally {
				EndUpdate();
			}
		}
		protected virtual void UpdateSize() {
			if(IsHorizontal) Width = ContainerSize;
			else Height = ContainerSize;
		}
		public override HitInfo GetHitInfo(Point ptClient) {
			return ViewInfo.CalcHitInfo(ptClient);
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("AutoHideContainerDock"),
#endif
 XtraSerializableProperty()]
		public override DockStyle Dock {
			get { return base.Dock; }
			set {
				if(IsInitialized) return;
				base.Dock = value;
			}
		}
		[Browsable(false)]
		public int Count { get { return (AutoHideInfo == null ? 0 : AutoHideInfo.Count); } }
#if !SL
	[DevExpressXtraBarsLocalizedDescription("AutoHideContainerItem")]
#endif
		public DockPanel this[int index] { get { return ((DockLayout)AutoHideInfo[index]).Panel; } }
		public bool Contains(DockPanel panel) { return (IndexOf(panel) != LayoutConsts.InvalidIndex); }
		public int IndexOf(DockPanel panel) {
			if(AutoHideInfo == null || panel == null) return LayoutConsts.InvalidIndex;
			return AutoHideInfo.IndexOf(panel.DockLayout);
		}
		bool IsInitialized { get { return (ContainerSize != 0); } }
		[Browsable(false)]
		public TabsPosition Position {
			get {
				if(AutoHideInfo == null) return (TabsPosition)(DockingStyle)Dock;
				return (TabsPosition)AutoHideInfo.Dock;
			}
		}
		protected internal bool IsHorizontal { get { return LayoutRectangle.GetIsVertical(AutoHideInfo.Dock); } }
		protected Rectangle[] ButtonsBounds { get { return ViewInfo.Buttons; } }
		protected AppearanceObject ViewAppearance { get { return viewAppearance; } }
		protected AppearanceObject ActiveHidePanelButtonAppearance { get { return activeHidePanelButtonAppearance; } }
		protected AppearanceObject InactiveHidePanelButtonAppearance { get { return inactiveHidePanelButtonAppearance; } }
		protected internal DockLayoutManager Manager {
			get { return manager; }
			set {
				if(Manager != null) {
					this.autoHideInfo.Changed -= new CollectionChangeEventHandler(AutoHideInfo_Changed);
					this.autoHideInfo = null;
				}
				manager = value;
				if(Manager != null) {
					this.autoHideInfo = Manager.AutoHideInfoCollection[(DockingStyle)Dock];
					this.autoHideInfo.Changed += new CollectionChangeEventHandler(AutoHideInfo_Changed);
				}
			}
		}
		protected DockElementsParameters Parameters { get { return Manager.DockManager.CurrentController.PaintStyle.ElementsParameters; } }
		protected DockManagerAppearances AppearancesSettings { get { return Manager.DockManager.CurrentController.AppearancesDocking; } }
		protected DockElementsPainter Painter { get { return Manager.DockManager.Painter; } }
		protected internal HideBarPainter HideBarPainter { get { return Painter.HideBarPainter; } }
		protected internal AutoHideInfo AutoHideInfo { get { return autoHideInfo; } }
		protected internal int ContainerSize {
			get { return containerSize; }
			set {
				if(ContainerSize == value) return;
				if(SavedContainerSize.HasValue) return;
				this.containerSize = value;
				UpdateSize();
			}
		}
		protected internal DockLayout HotLayout {
			get { return hotLayout; }
			set {
				if(HotLayout == value) return;
				hotLayout = value;
				Manager.AutoHideHotLayoutChanged(this);
				ViewInfo.SetDirty();
			}
		}
	}
	#region AutoHideContainerViewInfo
	public class AutoHideContainerViewInfo : ObjectInfoArgs {
		public AutoHideContainerViewInfo(AutoHideContainer owner) {
			PaintAppearance = new FrozenAppearance();
			PaintAppearanceActiveHeader = new FrozenAppearance();
			PaintAppearanceInactiveHeader = new FrozenAppearance();
			Owner = owner;
		}
		public AutoHideContainer Owner { get; private set; }
		public bool IsHorizontal { get; private set; }
		Rectangle[] buttonsCore;
		public Rectangle[] Buttons {
			get { return buttonsCore; }
		}
		AutoHideButtonInfo[] buttonInfosCore;
		public AutoHideButtonInfo[] ButtonInfos {
			get { return buttonInfosCore; }
		}
		public AppearanceObject PaintAppearance { get; private set; }
		public AppearanceObject PaintAppearanceActiveHeader { get; private set; }
		public AppearanceObject PaintAppearanceInactiveHeader { get; private set; }
		bool? isRightToLeftCore;
		protected internal bool IsRightToLeft {
			get {
				if(!isRightToLeftCore.HasValue)
					isRightToLeftCore = DevExpress.XtraEditors.WindowsFormsSettings.GetIsRightToLeft(Owner.Parent);
				return isRightToLeftCore.Value;
			}
		}
		protected void Clear() {
			isRightToLeftCore = null;
			buttonsCore = new Rectangle[Owner.Count];
			buttonInfosCore = new AutoHideButtonInfo[Owner.Count];
		}
		bool isReadyCore;
		public bool IsReady {
			get { return isReadyCore; }
		}
		public void SetDirty() {
			isReadyCore = false;
		}
		public virtual HitInfo CalcHitInfo(Point point) {
			HitInfo result = new HitInfo(Owner, point);
			if(!Ensure() || !Bounds.Contains(point))
				return result;
			result.Bounds = Bounds;
			result.HitTest = HitTest.Client;
			int index = -1;
			for(index = 0; index < Buttons.Length; index++) {
				if(Buttons[index].Contains(point)) {
					result.Bounds = Buttons[index];
					result.HitTest = HitTest.AutoHidePanel;
					result.AutoHidePanel = Owner[index];
					break;
				}
			}
			if(result.AutoHidePanel != null && result.AutoHidePanel.Tabbed) {
				int headerIndex;
				Rectangle headerBounds = ButtonInfos[index].GetHeaderBoundsAtPoint(point, out headerIndex);
				if(headerIndex >= 0) {
					result.HitTest = HitTest.Tab;
					result.Bounds = headerBounds;
					result.Tab = result.AutoHidePanel[headerIndex];
				}
			}
			return result;
		}
		internal bool Ensure() {
			if(!IsReady) {
				using(Graphics g = Graphics.FromHwndInternal(Owner.Handle)) {
					Calc(g, Owner.ClientRectangle);
				}
			}
			return IsReady;
		}
		int lockCalculation = 0;
		public void Calc(Graphics g, Rectangle bounds) {
			if(isReadyCore || lockCalculation > 0) return;
			lockCalculation++;
			Clear();
			Bounds = bounds;
			IsHorizontal = Owner.IsHorizontal;
			CalcButtons(g, bounds);
			isReadyCore = true;
			lockCalculation--;
		}
		protected virtual void CalcButtons(Graphics g, Rectangle bounds) {
			int interval = Owner.HideBarPainter.HideBarHorzInterval;
			bool isRTLAware = IsHorizontal && IsRightToLeft;
			int offset = CalcContainerOffset();
			int top = (Owner.Position == TabsPosition.Bottom || Owner.Position == TabsPosition.Right) ?
				Owner.HideBarPainter.HideBarVertInterval : 0;
			for(int i = 0; i < Buttons.Length; i++) {
				DockLayout layout = (DockLayout)Owner.AutoHideInfo[i];
				AutoHideButtonInfo buttonInfo = CreateButtonInfo(layout);
				Size size = CalcButtonHeaders(g, layout, buttonInfo, offset, IsHorizontal ? bounds.Height : bounds.Width);
				Rectangle buttonInfoRect = new Rectangle(offset, top, size.Width, size.Height);
				Rectangle buttons = new Rectangle(
					bounds.Left + (IsHorizontal ? offset : top),
					bounds.Top + (IsHorizontal ? top : offset),
					IsHorizontal ? size.Width : size.Height,
					IsHorizontal ? size.Height : size.Width);
				if(isRTLAware) {
					DockLayout.SwapRectangle(ref buttonInfoRect, bounds);
					DockLayout.SwapRectangle(ref buttons, bounds);
					for(int h = 0; h < buttonInfo.Headers.Length; h++) {
						int relativeOffset = buttonInfo.Headers[h].RelativeOffset;
						buttonInfo.Headers[h].RelativeOffset = DockLayout.SwapOffset(ref relativeOffset, bounds) - size.Width;
					}
				}
				buttonInfo.Bounds = buttonInfoRect;
				ButtonInfos[i] = buttonInfo;
				Buttons[i] = buttons;
				offset += (size.Width + interval);
			}
		}
		int CalcContainerOffset() {
			if(!Owner.IsHorizontal && Owner.Parent != null) {
				for(int i = 0; i < Owner.Parent.Controls.Count; i++) {
					var container = Owner.Parent.Controls[i] as AutoHideContainer;
					if(container != null && container.Position == TabsPosition.Top) {
						return container.Height + Owner.HideBarPainter.HideBarHorzInterval;
					}
				}
			}
			return Owner.HideBarPainter.HideBarHorzInterval;
		}
		protected virtual AutoHideButtonInfo CreateButtonInfo(DockLayout layout) {
			return new AutoHideButtonInfo(layout, IsHorizontal, Owner.Position);
		}
		Size CalcButtonHeaders(Graphics g, DockLayout layout, AutoHideButtonInfo buttonInfo, int relativeoffset, int minSize) {
			int offset = 0; int h = 0;
			int maxWidth = 0;
			for(int i = 0; i < buttonInfo.Headers.Length; i++) {
				AutoHideButtonHeaderInfo headerInfo = buttonInfo.Headers[i];
				headerInfo.RelativeOffset = relativeoffset;
				headerInfo.IsHot = (Owner.HotLayout == headerInfo.Header);
				AppearanceObject appearance = Owner.GetButtonHeaderAppearance(headerInfo.IsHot);
				int collapsedWidth = headerInfo.Calc(g, appearance, Owner.HideBarPainter, offset, minSize);
				maxWidth = Math.Max(headerInfo.Bounds.Width + collapsedWidth, maxWidth);
				offset += (headerInfo.Bounds.Width - Owner.HideBarPainter.NextTabHeaderGrow);
				h = Math.Max(h, headerInfo.Bounds.Height);
			}
			int grow = CheckActiveItemGrow(buttonInfo, maxWidth);
			return new Size(offset + grow, h);
		}
		int CheckActiveItemGrow(AutoHideButtonInfo buttonInfo, int maxWidth) {
			bool canGrow = (buttonInfo.Headers.Length > 1 && Owner.Manager.DockManager.CanCollapseAutoHideCaptions);
			if(!canGrow)
				return 0;
			int activeItemGrow = 0;
			for(int i = 0; i < buttonInfo.Headers.Length; i++) {
				AutoHideButtonHeaderInfo headerInfo = buttonInfo.Headers[i];
				headerInfo.RelativeOffset += activeItemGrow;
				if(headerInfo.IsActive) {
					activeItemGrow = maxWidth - headerInfo.Bounds.Width;
					headerInfo.Grow(activeItemGrow);
				}
			}
			return activeItemGrow;
		}
		public void UpdateBeforePaint(GraphicsCache cache, Rectangle bounds) {
			Owner.UpdateAppearances();
			Calc(cache.Graphics, bounds);
		}
	}
	public class AutoHideButtonInfo {
		AutoHideButtonHeaderInfo[] headersCore;
		public AutoHideButtonInfo(DockLayout layout, bool isHorizontal, TabsPosition tabPosition) {
			IsHorizontal = isHorizontal;
			Position = tabPosition;
			int count = layout.Count;
			if(count == 0) {
				headersCore = new AutoHideButtonHeaderInfo[1];
				Headers[0] = CreateHeaderInfo(layout);
			}
			else {
				headersCore = new AutoHideButtonHeaderInfo[count];
				for(int i = 0; i < Headers.Length; i++)
					Headers[i] = CreateHeaderInfo(layout[i]);
			}
		}
		public AutoHideButtonHeaderInfo[] Headers {
			get { return headersCore; }
		}
		public TabsPosition Position { get; private set; }
		public bool IsHorizontal { get; private set; }
		public Rectangle Bounds { get; set; }
		protected virtual AutoHideButtonHeaderInfo CreateHeaderInfo(DockLayout header) {
			return new AutoHideButtonHeaderInfo(header, IsHorizontal, Position);
		}
		public Rectangle GetHeaderBoundsAtPoint(Point point, out int headerIndex) {
			Point relativePoint = GetRelativePoint(point);
			for(headerIndex = 0; headerIndex < Headers.Length; headerIndex++) {
				Rectangle r = Headers[headerIndex].Bounds;
				r.X += Headers[headerIndex].RelativeOffset;
				if(r.Contains(relativePoint))
					return GetRelativeRect(r);
			}
			headerIndex = -1;
			return Rectangle.Empty;
		}
		Point GetRelativePoint(Point point) {
			return IsHorizontal ? point : new Point(point.Y, point.X);
		}
		public Rectangle GetRelativeRect(Rectangle rect) {
			return IsHorizontal ? rect : new Rectangle(rect.Y, rect.X, rect.Height, rect.Width);
		}
	}
	public class AutoHideButtonHeaderInfo {
		FrozenAppearance paintAppearanceCore;
		public FrozenAppearance PaintAppearance {
			get { return paintAppearanceCore; }
		}
		public DockLayout Header { get; private set; }
		public TabsPosition Position { get; private set; }
		public bool IsHot { get; set; }
		public bool IsActive { get; private set; }
		protected bool CanCollapse { get; private set; }
		public AutoHideButtonHeaderInfo(DockLayout header, bool isHorizontal, TabsPosition tabPosition) {
			Position = tabPosition;
			Header = header;
			paintAppearanceCore = new FrozenAppearance();
			IsHorizontal = isHorizontal;
			CanCollapse = header.DockManager.CanCollapseAutoHideCaptions &&
				((header.Parent != null) && header.Parent.Count > 1);
			IsActive = (header.Parent == null) || (header.Parent is DockLayout && ((DockLayout)header.Parent).ActiveChild == header);
			Text = header.TabText;
			Image = header.Image;
			ImageSize = header.ImageSize;
		}
		public int RelativeOffset { get; set; }
		public bool IsHorizontal { get; private set; }
		public string Text { get; protected set; }
		public Image Image { get; private set; }
		public Size ImageSize { get; private set; }
		public Rectangle Bounds { get; private set; }
		public Rectangle ImageBounds { get; private set; }
		public Rectangle TextBounds { get; private set; }
		public bool AllowGlyphSkinning { get; set; }
		protected Size GetDefaultContentSize() {
			return Header.HasImage ? SystemInformation.SmallIconSize : Header.ImageSize;
		}
		public int Calc(Graphics g, AppearanceObject appearance, HideBarPainter painter, int offset, int minSize) {
			PaintAppearance.AssignInternal(appearance);
			PaintAppearance.TextOptions.RightToLeft = Header.IsRightToLeft;
			int collapsedWidth = 0;
			bool hasText = !string.IsNullOrEmpty(Text);
			bool hasImage = (Image != null);
			Size textSize = hasText ? Size.Ceiling(MeasureText(g)) : Size.Empty;
			Size imageSize = hasImage ? ImageSize : Size.Empty;
			int interval = hasImage && hasText ? painter.ImageToTextInterval : 0;
			if(CanCollapse) {
				if(!IsActive) {
					hasText &= (!hasImage);
					if(!hasText) {
						collapsedWidth = textSize.Width + interval;
						textSize = Size.Empty;
						interval = 0;
					}
				}
			}
			Padding padding = painter.GetTabButtonContentMargin();
			padding.Left += painter.TabHeaderContentIndent;
			padding.Right += painter.TabHeaderContentIndent;
			minSize -= padding.Vertical;
			Size content = new Size(textSize.Width + interval + imageSize.Width, Math.Max(minSize, Math.Max(textSize.Height, imageSize.Height)));
			if(content.IsEmpty)
				content = GetDefaultContentSize();
			int top = (Position == TabsPosition.Bottom || Position == TabsPosition.Right) ? painter.HideBarVertInterval : 0;
			Bounds = new Rectangle(new Point(offset, top), Size.Add(content, new Size(padding.Horizontal, padding.Vertical)));
			var imgBounds = new Rectangle(new Point(padding.Left + offset, top + padding.Top + (content.Height - imageSize.Height) / 2), imageSize);
			var txtBounds = new Rectangle(new Point(padding.Left + offset + imageSize.Width + interval, top + padding.Top + (content.Height - textSize.Height) / 2), textSize);
			ImageBounds = Header.GetRTLBounds(imgBounds, Bounds);
			TextBounds = Header.GetRTLBounds(txtBounds, Bounds);
			return collapsedWidth;
		}
		SizeF MeasureText(Graphics g) {
			return IsHorizontal ? PaintAppearance.CalcTextSize(g, Text, 0) :
				g.MeasureString(Text, PaintAppearance.GetFont(), 0, PaintAppearance.GetStringFormat());
		}
		public Rectangle GetRelativeRect(Rectangle rect) {
			return IsHorizontal ? new Rectangle(RelativeOffset + rect.Left, rect.Top, rect.Width, rect.Height) :
			  new Rectangle(rect.Y, rect.X + RelativeOffset, rect.Height, rect.Width);
		}
		public void DrawImage(GraphicsCache cache) {
			if(ImageBounds.Width == 0 || ImageBounds.Height == 0) return;
			Rectangle imageRect = GetRelativeRect(ImageBounds);
			if(IsHorizontal) {
				if(Header.GetAllowGlyphSkinning()) {
					var attributes = ImageColorizer.GetColoredAttributes(PaintAppearance.GetForeColor());
					cache.Graphics.DrawImage(Image, imageRect, 0, 0, ImageSize.Width, ImageSize.Height, GraphicsUnit.Pixel, attributes);
				}
				else cache.Graphics.DrawImage(Image, imageRect);
			}
			else {
				using(Bitmap buffer = new Bitmap(ImageBounds.Width, ImageBounds.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb)) {
					using(Graphics g = Graphics.FromImage(buffer)) {
						Rectangle r = new Rectangle(0, 0, ImageBounds.Width, ImageBounds.Height);
						if(Header.GetAllowGlyphSkinning()) {
							var attributes = ImageColorizer.GetColoredAttributes(PaintAppearance.GetForeColor());
							g.DrawImage(Image, r, 0, 0, ImageSize.Width, ImageSize.Height, GraphicsUnit.Pixel, attributes);
						}
						else g.DrawImage(Image, r);
					}
					buffer.RotateFlip(RotateFlipType.Rotate90FlipNone);
					cache.Graphics.DrawImageUnscaled(buffer, imageRect);
				}
			}
		}
		public void DrawText(GraphicsCache cache) {
			if(TextBounds.Width == 0 || TextBounds.Height == 0) return;
			Rectangle textRect = GetRelativeRect(TextBounds);
			if(IsHorizontal) cache.DrawString(Text, PaintAppearance.GetFont(),
				PaintAppearance.GetForeBrush(cache), textRect, PaintAppearance.GetStringFormat());
			else {
				int angle = Header.GetAutoHidePanelVerticalTextOrientation() == Docking.VerticalTextOrientation.BottomToTop ? -90 : 90;
				cache.DrawVString(Text, PaintAppearance.GetFont(),
				PaintAppearance.GetForeBrush(cache), textRect, PaintAppearance.GetStringFormat(), angle);
			}
		}
		protected internal void Grow(int grow) {
			Bounds = new Rectangle(Bounds.Location, new Size(Bounds.Width + grow, Bounds.Height));
		}
	}
	#endregion AutoHideContainerViewInfo
	[ToolboxItem(false)]
	public class AutoHideControl : XtraScrollableControl {
		TabsPosition position;
		Control savedParentCore;
		public AutoHideControl(TabsPosition position) {
			this.position = position;
			this.savedParentCore = null;
			this.AutoScroll = false;
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			UpdateClientControlLocation();
		}
		void UpdateClientControlLocation() {
			if(ClientControl == null) return;
			Rectangle clientControlBounds = LayoutConsts.InvalidRectangle;
			switch(Position) {
				case TabsPosition.Left: clientControlBounds = new Rectangle(Width - ClientControl.Width, 0, ClientControl.Width, Height); break;
				case TabsPosition.Right: clientControlBounds = new Rectangle(0, 0, ClientControl.Width, Height); break;
				case TabsPosition.Top: clientControlBounds = new Rectangle(0, Height - ClientControl.Height, Width, ClientControl.Height); break;
				case TabsPosition.Bottom: clientControlBounds = new Rectangle(0, 0, Width, ClientControl.Height); break;
			}
			ClientControl.Bounds = clientControlBounds;
		}
		protected override bool ProcessKeyPreview(ref Message m) {
			BaseEdit edit = null;
			Control control = Control.FromHandle(m.HWnd);
			if(control != null) edit = control as BaseEdit ?? control.Parent as BaseEdit;
			if((edit == null || edit.Properties.ReadOnly) && DockLayoutUtils.IsEscapeDownMessage(ref m)) {
				RaiseClose();
				return true;
			}
			return base.ProcessKeyPreview(ref m);
		}
		protected override void OnParentChanged(EventArgs e) {
			base.OnParentChanged(e);
			AutoHideContainer.UpdateBackColorSafe(this, GetStyle(ControlStyles.SupportsTransparentBackColor));
		}
		protected override void OnParentBackColorChanged(EventArgs e) {
			base.OnParentBackColorChanged(e);
			AutoHideContainer.UpdateBackColorSafe(this, GetStyle(ControlStyles.SupportsTransparentBackColor));
		}
		protected virtual void RaiseClose() {
			EventHandler handler = (EventHandler)this.Events[close];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		static readonly object close = new object();
		public event EventHandler Close {
			add { this.Events.AddHandler(close, value); }
			remove { this.Events.RemoveHandler(close, value); }
		}
		public TabsPosition Position { get { return position; } }
		public bool IsHorizontal { get { return !DockLayoutUtils.IsVerticalPosition(Position); } }
		public Control ClientControl {
			get { return GetClientControlCore(); }
			set {
				Control clientControl = GetClientControlCore();
				if(clientControl == value) return;
				if(clientControl != null) {
					clientControl.Visible = false;
					Utils.FocusHelper.SaveFocus();
					AutoHideContainer ahContainer = savedParentCore as AutoHideContainer;
					if(ahContainer == null || !ahContainer.IsDisposing) {
						clientControl.Parent = savedParentCore;
					}
					if(ahContainer != null && !ahContainer.IsDisposing)
						ahContainer.SavedContainerSize = null;
					Utils.FocusHelper.RestoreFocus();
				}
				if(value != null) {
					var dockingStyle = DockLayoutUtils.ConvertToOppositeDockingStyle(Position);
					if(Enum.IsDefined(typeof(DockingStyle), dockingStyle))
						value.Dock = (DockStyle)dockingStyle;
					savedParentCore = value.Parent;
					AutoHideContainer ahContainer = savedParentCore as AutoHideContainer;
					if(ahContainer != null && !ahContainer.IsDisposing)
						ahContainer.SavedContainerSize = ahContainer.ContainerSize;
					value.Parent = this;
					value.Visible = true;
				}
			}
		}
		Control GetClientControlCore() {
			return (Controls.Count == 0) ? null : Controls[0];
		}
		protected internal Control SavedParent { get { return savedParentCore; } }
		protected override void WndProc(ref System.Windows.Forms.Message m) {
			base.WndProc(ref m);
			CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref m, this);
		}
		public bool IsHot { get { return ClientRectangle.Contains(PointToClient(Control.MousePosition)); } }
	}
}
