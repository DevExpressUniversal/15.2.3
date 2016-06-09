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
using System.Drawing.Design;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.XtraEditors.ButtonPanel;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Customization;
using DevExpress.XtraLayout.Handlers;
using DevExpress.XtraLayout.Helpers;
using DevExpress.XtraLayout.HitInfo;
using DevExpress.XtraLayout.Resizing;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraLayout.ViewInfo;
namespace DevExpress.XtraDashboardLayout {
	public class DashboardLayoutControlGroupBase : LayoutControlGroup, IButtonsPanelOwner {
		bool allowDropGroup = true;
		public DashboardLayoutControlGroupBase()
			: this(null) { }
		public DashboardLayoutControlGroupBase(DashboardLayoutControlGroupBase parent)
			: base(parent) {
			UpdatePaintAppearance();
			InitializeCrosshairCollection();
		}
		protected internal override void OnItems_Changed(object sender, CollectionChangeEventArgs e) {
			base.OnItems_Changed(sender, e);
			RebuildCrosshairCollection();
		}
		internal void RebuildCrosshairCollection() {
			if(LayoutMode == XtraLayout.Utils.LayoutMode.Flow) {
				InitializeCrosshairCollection();
				return;
			}
			List<Crosshair> result = new List<Crosshair>();
			for(int i = 0; i < Items.Count; i++) {
				BaseLayoutItem item1 = Items[i] as BaseLayoutItem;
				var group = Items[i] as DashboardLayoutControlGroupBase;
				if(group != null) {
					group.RebuildCrosshairCollection();
				}
				if(item1 == null) continue;
				if(!item1.ActualItemVisibility) continue;
				Rectangle item1Bounds = item1.Bounds;
				BaseLayoutItem rightBottomItem = null, rightItem = null, bottomItem = null;
				for(int j = 0; j < Items.Count; j++) {
					BaseLayoutItem item2 = Items[j] as BaseLayoutItem;
					if(item2 == null) continue;
					if(!item2.ActualItemVisibility) continue;
					Rectangle item2Bounds = item2.Bounds;
					if(RectangleHelper.RightBottomCorner(item1Bounds) == RectangleHelper.LeftBottomCorner(item2Bounds)) rightItem = item2;
					if(RectangleHelper.RightBottomCorner(item1Bounds) == RectangleHelper.LeftTopCorner(item2Bounds)) rightBottomItem = item2;
					if(RectangleHelper.RightBottomCorner(item1Bounds) == RectangleHelper.RightTopCorner(item2Bounds)) bottomItem = item2;
					if(rightBottomItem != null && rightItem != null && bottomItem != null &&
						 item1.Bounds.Height == rightItem.Bounds.Height && item1.Bounds.Width == bottomItem.Bounds.Width &&
						 rightItem.Bounds.Width == rightBottomItem.Bounds.Width && bottomItem.Bounds.Height == rightBottomItem.Bounds.Height) {
						Crosshair cros = new Crosshair();
						cros.LeftTopItem = item1;
						cros.LeftBottomItem = bottomItem;
						cros.RightTopItem = rightItem;
						cros.RightBottomItem = rightBottomItem;
						result.Add(cros);
						break;
					}
				}
			}
			crosshairCollectionCore = SaveOldResizeType(result);
			if(ResizeManager != null && ResizeManager.resizer != null && ResizeManager.resizer.resultH != null && crosshairCollectionCore.Count != 0)
				ResizerWithCrosshair.FillRealCrosshairGroupingType(LayoutType.Horizontal, this, crosshairCollectionCore, ResizeManager.resizer.resultH);
		}
		protected internal virtual bool IsHiddenGroup { get { return !GroupBordersVisible && !IsRoot; } }
		protected virtual bool IsInCaptionArea(Point point) {
			if(ViewInfo == null) {
				DashboardLayoutControlGroupViewInfo lcivi = ViewInfo as DashboardLayoutControlGroupViewInfo;
				if(lcivi == null)
					return false;
				else
					return lcivi.TextAreaRelativeToControl.Contains(point);
			}
			if(ViewInfo is DashboardLayoutControlGroupViewInfo) {
				return ((DashboardLayoutControlGroupViewInfo)ViewInfo).BorderInfo.CaptionBounds.Contains(point);
			}
			return ViewInfo.TextAreaRelativeToControl.Contains(point);
		}
		protected new virtual int IsInExpandedButtonArea(Point point) {
			BaseButtonInfo bbi = ViewInfo.BorderInfo.ButtonsPanel.ViewInfo.CalcHitInfo(point);
			if(bbi == null) return -1;
			return ViewInfo.Owner.CustomHeaderButtons.IndexOf(bbi.Button);
		}
		protected virtual bool IsInCaptionImageArea(Point point) {
			DashboardLayoutControlGroupViewInfo vi = ViewInfo as DashboardLayoutControlGroupViewInfo;
			if(vi == null)
				return false;
			return vi.BorderInfo.CaptionImageBounds.Contains(point);
		}
		protected internal override BaseLayoutItemHitInfo CalcHitInfo(Point hitPoint, bool calcForHandler) {
			BaseLayoutItemHitInfo hitInfoBase = base.CalcHitInfo(hitPoint, calcForHandler);
			BaseLayoutItemHitInfo hitInfo = new DashboardHitInfo(hitInfoBase, (hitInfoBase is DashboardHitInfo) ? (hitInfoBase as DashboardHitInfo).AdditionalHitType : LayoutGroupHitTypes.None);
			if(IsInCaptionArea(hitPoint)) {
				hitInfo = new DashboardHitInfo(hitInfo, LayoutGroupHitTypes.Caption);
			}
			int hotIndex = IsInExpandedButtonArea(hitPoint);
			if(hotIndex >= 0) hitInfo = new DashboardHitInfo(hitInfo, hotIndex);
			if(IsInCaptionImageArea(hitPoint)) hitInfo = new DashboardHitInfo(hitInfo, LayoutGroupHitTypes.CaptionImage);
			return hitInfo;
		}
		private static readonly object buttonMouseClick = new object();
		public event DashboardLayoutControlGroupButtonClickEventHandler ButtonMouseClick {
			add { base.Events.AddHandler(buttonMouseClick, value); }
			remove { base.Events.RemoveHandler(buttonMouseClick, value); }
		}
		protected internal void RaiseButtonMouseClick(int index) {
			DashboardLayoutControlGroupButtonClickEventHandler handler = (DashboardLayoutControlGroupButtonClickEventHandler)this.Events[buttonMouseClick];
			if(handler != null)
				handler(this, new DashboardLayoutControlGroupButtonClickEventArgs() { ButtonIndex = index });
		}
		private static readonly object paintCore = new object();
		public event PaintEventHandler Paint {
			add { base.Events.AddHandler(paintCore, value); }
			remove { base.Events.RemoveHandler(paintCore, value); }
		}
		protected internal void RaisePaintEvent(PaintEventArgs args) {
			PaintEventHandler handler = (PaintEventHandler)this.Events[paintCore];
			if(handler != null) handler(this, args);
		}
		private List<Crosshair> SaveOldResizeType(List<Crosshair> newResult) {
			foreach(Crosshair newCross in newResult)
				foreach(Crosshair oldCross in crosshairCollectionCore) {
					if(oldCross.LeftTopItem == newCross.LeftTopItem &&
						oldCross.RightTopItem == newCross.RightTopItem &&
						oldCross.LeftBottomItem == newCross.LeftBottomItem &&
						oldCross.RightBottomItem == newCross.RightBottomItem) {
						newCross.CrosshairGroupingType = oldCross.CrosshairGroupingType;
						break;
					}
				}
			return newResult;
		}
		public virtual bool AllowDropGroup { get { return allowDropGroup; } set { allowDropGroup = value; } } 
		protected internal override LayoutItem CreateLayoutItem() {
			return new DashboardLayoutControlItemBase();
		}
		protected internal override LayoutGroup CreateLayoutGroup() {
			var result = new DashboardLayoutControlGroupBase();
			return result;
		}
		protected internal override TabbedGroup CreateTabbedGroup() {
			throw new Exception("Not allowed");
		}
		protected override ResizeManager CreateResizeManager() {
			return new ResizeManager2(this);
		}
		protected internal override void AddItemToSelectedList(BaseLayoutItem item) {
			if(SelectedItems.Count > 1) throw new Exception("Multiselection is not allowed");
			base.AddItemToSelectedList(item);
		}
		protected override void OnAppearanceChanged(object sender, EventArgs e) {
			base.OnAppearanceChanged(sender, e);
			UpdatePaintAppearance();
		}
		LayoutGroupAppearance localPaintAppearance;
		protected void UpdatePaintAppearance() {
			localPaintAppearance = new LayoutGroupAppearance();
			localPaintAppearance.AppearanceGroup = AppearanceGroup;
			localPaintAppearance.AppearanceGroup.TextOptions.Trimming = Trimming.EllipsisCharacter;
		}
		public override LayoutGroupAppearance PaintAppearanceGroup {
			get {
				if(localPaintAppearance == null)
					localPaintAppearance = new LayoutGroupAppearance();
				return localPaintAppearance;
			}
		}
		public delegate void DashboardLayoutControlGroupButtonClickEventHandler(object sender, DashboardLayoutControlGroupButtonClickEventArgs e);
		public class DashboardLayoutControlGroupButtonClickEventArgs : EventArgs {
			public int ButtonIndex { get; set; }
		}
		protected enum GroupType{ Column, Row, Layout}
		protected GroupType ClassifyGroupType() {
			GroupType result = GroupType.Layout;
			bool isColumn = true, isRow = true;
			foreach(BaseLayoutItem item1 in Items)
				foreach(BaseLayoutItem item2 in Items) {
					if(item1 == item2) continue;
					if(item1.X != item2.X || item1.Width != item2.Width) { isColumn = false; }
					if(item1.X == item2.X && item1.Width == item2.Width) result = GroupType.Column;
					if(item1.Y != item2.Y || item1.Height != item2.Height) { isRow = false; }
					if(item1.Y == item2.Y && item1.Height == item2.Height) result = GroupType.Row;
				}
			if(!isColumn && !isRow) result = GroupType.Layout;
			return result;
		}
		public override LayoutMode LayoutMode {
			get {
				return XtraLayout.Utils.LayoutMode.Regular;
			}
		}
		public override bool CanChangeLayoutModeForChildSelectedGroup {
			get {
				return false;
			}
		}
		protected internal override bool EnabledState {
			get {
				return true;
			}
		}
		Image layoutGroupLabelImage = null;
#if !SL
	[DevExpressXtraLayoutLocalizedDescription("LayoutControlItemImage")]
#endif
		[XtraSerializableProperty(), DefaultValue(null), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		public virtual Image Image {
			get { return layoutGroupLabelImage; }
			set {
				if(layoutGroupLabelImage == value) return;
				using(new SafeBaseLayoutItemChanger(this)) {
					layoutGroupLabelImage = value;
					ShouldUpdateConstraints = true;
					ComplexUpdate(true, true, true);
				}
			}
		}
		bool isGroupBoundsVisible = true;
		[DefaultValue(true)]
		public bool BordersVisible {
			get { return isGroupBoundsVisible; }
			set {
				if(isGroupBoundsVisible == value) return;
				isGroupBoundsVisible = value;
				shouldResetBorderInfoCore = true;
				ShouldUpdateConstraintsDoUpdate = true;
				ComplexUpdate(true, true);
			}
		}
		protected override DevExpress.XtraLayout.ViewInfo.BaseLayoutItemViewInfo CreateViewInfo() {
			if(!IsRoot) return new DashboardLayoutControlGroupViewInfo(this);
			else return base.CreateViewInfo();
		}
		private void HardResize(Size bSize, Size resizeSize, LayoutType layoutType) {
			LayoutSize lbSize, lResizeSize;
			lbSize = new LayoutSize(bSize, layoutType);
			lResizeSize = new LayoutSize(resizeSize, layoutType);
			int pixelsToGive = lResizeSize.Height - lbSize.Height;
			int pixelsGiven = 0;
			int currentPos = 0;
			int watchdog = Math.Abs(pixelsToGive) * 10;
			while(pixelsGiven < Math.Abs(pixelsToGive) && watchdog > 0) {
				LayoutSize newSize = new LayoutSize(Items[currentPos].Size, layoutType);
				newSize.Height = newSize.Height + (pixelsToGive > 0 ? 1 : -1);
				if(newSize.Height > 0) {
					pixelsGiven++;
					Items[currentPos].Size = newSize.Size;
				}
				currentPos++;
				if(currentPos == Items.Count) currentPos = 0;
				watchdog--;
			}
			int pixelsToGive2 = lResizeSize.Width - lbSize.Width;
			currentPos = 0;
			foreach(BaseLayoutItem bli in Items) {
				LayoutRectangle newItemRect = new LayoutRectangle(bli.Bounds, layoutType);
				newItemRect.Y = currentPos;
				newItemRect.Width += pixelsToGive2;
				bli.SetBounds(newItemRect.Rectangle);
				currentPos += newItemRect.Height;
			}
			if(watchdog == 0 && pixelsToGive > 0) throw new Exception("Error in the group.UngroupItems method");
		}
		protected override int GetInsertOutsideSizeDelta(LayoutItemDragController controller, BaseItemCollection items) {
			if(controller.DragItem is DashboardLayoutControlItemBase && (controller.DragItem as DashboardLayoutControlItemBase).IsLocked && controller.LayoutType == LayoutType.Vertical) {
				return controller.DragItem.MinSize.Height;
			}
			int delta = int.MaxValue;
			foreach(BaseLayoutItem bItem in items) {
				LayoutRectangle lrect = new LayoutRectangle(bItem.Bounds, controller.LayoutType);
				LayoutSize lsize = new LayoutSize(bItem.MinSize, controller.LayoutType);
				int halfWidth = lrect.Width / 2;
				if(lrect.Width - halfWidth < lsize.Width) halfWidth = lrect.Width - lsize.Width;
				if(delta > halfWidth) delta = halfWidth;
			}
			if(delta < 1) return 1;
			return delta;
		}
		List<Crosshair> crosshairCollectionCore = new List<Crosshair>();
		protected virtual void InitializeCrosshairCollection() { crosshairCollectionCore = CreateCrosshairCollection(); }
		protected virtual List<Crosshair> CreateCrosshairCollection() { return new List<Crosshair>(); }
		public List<Crosshair> CrosshairCollection { get { return crosshairCollectionCore; } set { crosshairCollectionCore = value; } }
		protected internal override Size DefaultItemSize { get { return new Size(100, 100); } }
		protected internal override Size DefaultMinItemSize { get { return new Size(40, 40); } }
		protected internal override Size DefaultMaxItemSize { get { return new Size(0, 0); } }
		protected override bool IsStandartRemoveItemNeighborsInsertLocation { get { return false; } }
		internal List<Crosshair> GetFlatCrosshairs() {
			FlatItemsList list = new FlatItemsList();
			List<Crosshair> result = new List<Crosshair>();
			foreach(BaseLayoutItem item in list.GetItemsList(this)) {
				var childGroup = item as DashboardLayoutControlGroupBase;
				if(childGroup != null) {
					result.AddRange(childGroup.CrosshairCollection);
				}
			}
			return result;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(Owner != null) {
					DashboardLayoutControl dlc = Owner as DashboardLayoutControl;
					if(dlc != null && dlc.handler != null) {
						if(dlc.handler.SelectedItem == this)
							dlc.handler.SelectedItem = null;
						if(dlc.handler.HotTrackedItem == this)
							dlc.handler.HotTrackedItem = null;
					}
				}
			}
			base.Dispose(disposing);
		}
		public ObjectPainter GetPainter() {
			if(Owner == null) return new GroupBoxButtonsPanelPainter();
			if(Owner.LookAndFeel.ActiveStyle == DevExpress.LookAndFeel.ActiveLookAndFeelStyle.Skin)
				return new DashboardGroupBoxButtonsSkinPainter(Owner.LookAndFeel);
			if(Owner.LookAndFeel.ActiveStyle == DevExpress.LookAndFeel.ActiveLookAndFeelStyle.WindowsXP)
				return new GroupBoxButtonsPanelWindowsXpPainter();
			if(Owner.LookAndFeel.ActiveStyle == DevExpress.LookAndFeel.ActiveLookAndFeelStyle.Office2003)
				return new BaseButtonsPanelOffice2003Painter();
			return new GroupBoxButtonsPanelPainter();
		}
	}
	public class DashboardLayoutControlGroupViewInfo : LayoutControlGroupViewInfo {
		public DashboardLayoutControlGroupViewInfo(DashboardLayoutControlGroupBase owner) : base(owner) {
		}
		protected override Size AddLabel(Size size) {
			return size;
		}
		protected override void UpdateTextArea() {
		}
		public Rectangle BackgroundArea {
			get;
			set;
		}
		public virtual Rectangle BackgroundAreaRelativeToControl {
			get {
				return TranslateCoordinates(BackgroundArea);
			}
		}
		protected override void CalculateRegionsCore() {
			ClientArea = CalcIndentionsRect();
			TextArea = Rectangle.Empty;
			var temp = GetPainterPadding();
			BackgroundArea = new Rectangle(Bounds.Left - temp.Left, Bounds.Top - temp.Top, Bounds.Width + temp.Width, Bounds.Height + temp.Height);
		}
		public override void UpdateAppearance() {
		}
		Rectangle controlArea;
		public virtual Rectangle ControlArea {
			get {
				CalculateViewInfoIfNeeded();
				return controlArea;
			}
			set { controlArea = value; }
		}
		protected override ObjectInfoArgs CreateLastInfo() {
			var info = new DashboardGroupObjectInfoArgs();
			info.SetButtonsPanelOwner(this.Owner, false);
			return info;
		}
		protected override ObjectInfoArgs CreateBorderInfo() {
			var info = new DashboardGroupObjectInfoArgs();
			info.SetButtonsPanelOwner(this.Owner);
			return info;
		}
		public new DashboardGroupObjectInfoArgs BorderInfo {
			get {
				CalculateViewInfoIfNeeded();
				return base.BorderInfo as DashboardGroupObjectInfoArgs;
			}
		}
		protected override internal void UpdateState() {
			DashboardGroupObjectInfoArgs tempBorderInfo = BorderInfo as DashboardGroupObjectInfoArgs;
			if(tempBorderInfo != null) {
				tempBorderInfo.State = State;
			}
		}
		public new DashboardLayoutControlGroupBase Owner {
			get { return (DashboardLayoutControlGroupBase)base.Owner; }
		}
		private bool IsInCaptionArea(Point hitPoint) {
			throw new NotImplementedException();
		}
		public override ObjectState State {
			get {
				if(Owner != null) {
					if(Owner.Selected)
						return ObjectState.Selected;
					if(!Owner.EnabledState)
						return ObjectState.Disabled;
					DashboardLayoutControl control = Owner.Owner as DashboardLayoutControl;
					if(control != null && control.handler != null) {
						if(control.HotTrackedItem == Owner && control.handler.State != AdornerWindowHandlerStates.Dragging)
							return ObjectState.Hot;
					}
				}
				return ObjectState.Normal;
			}
		}
		public new void UpdateAppearanceGroup(GroupObjectInfoArgs tempBorderInfo) {
			tempBorderInfo.SetAppearance(Owner.PaintAppearanceGroup.AppearanceGroup);
			tempBorderInfo.AppearanceCaption = Owner.PaintAppearanceItemCaption;
		}
		protected override void UpdateBorderInfo(ObjectInfoArgs borderInfo) {
			DashboardGroupObjectInfoArgs tempBorderInfo = borderInfo as DashboardGroupObjectInfoArgs;
			UpdateAppearanceGroup(tempBorderInfo);
			tempBorderInfo.ShowCaption = Owner.BordersVisible && Owner.TextVisible;
			tempBorderInfo.ShowButton = false;
			tempBorderInfo.ButtonsPanel.ShowToolTips = true;
			tempBorderInfo.ButtonsPanel.ButtonInterval = DashboardLayoutSettings.buttonToButtonDistance;
			if(Owner != null && Owner.Owner != null && Owner.Owner is DashboardLayoutControl) tempBorderInfo.AllowImageColorization = ((DashboardLayoutControl)Owner.Owner).AllowImageColorization;
			tempBorderInfo.ButtonLocation = GroupElementLocation.AfterText;
			tempBorderInfo.AllowHtmlText = true;
			tempBorderInfo.Expanded = Owner.Expanded;
			tempBorderInfo.Caption = Owner.Text;
			tempBorderInfo.CaptionLocation = Locations.Top;
			tempBorderInfo.ShowCaptionImage = Owner.Image != null;
			tempBorderInfo.CaptionImagePadding = new System.Windows.Forms.Padding(0);
			tempBorderInfo.CaptionImage = Owner.Image;
			tempBorderInfo.BorderStyle = Owner.BordersVisible ? DevExpress.XtraEditors.Controls.BorderStyles.Default : DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			if(!Owner.EnabledState)
				tempBorderInfo.State = ObjectState.Disabled;
			tempBorderInfo.RightToLeft = Owner.IsRTL;
			if(tempBorderInfo.AppearanceCaption.TextOptions.HotkeyPrefix == HKeyPrefix.Default)
				tempBorderInfo.AppearanceCaption.TextOptions.HotkeyPrefix = HKeyPrefix.Show;
		}
		protected override void CalculatePaddingCore() { if(Owner.BordersVisible) base.CalculatePaddingCore(); }
	}
}
