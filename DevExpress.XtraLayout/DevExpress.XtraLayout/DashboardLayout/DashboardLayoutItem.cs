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
using System.Collections.Generic;
using System.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout.Helpers;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.ViewInfo;
using DevExpress.XtraLayout.Painting;
using DevExpress.XtraLayout.Utils;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Controls;
using System.Windows.Forms;
using DevExpress.XtraLayout.Handlers;
using DevExpress.XtraLayout.HitInfo;
using System.Collections.ObjectModel;
using DevExpress.XtraEditors.ButtonPanel;
using DevExpress.XtraBars.Docking2010;
using DevExpress.Utils.Serializing;
using DevExpress.XtraEditors.ButtonsPanelControl;
namespace DevExpress.XtraDashboardLayout {
	public class DashboardLayoutControlItemBase :LayoutControlItem, IGroupBoxButtonsPanelOwner, IButtonPanelControlAppearanceOwner {
		public override LayoutGroup Parent {
			get {
				return base.Parent;
			}
			set {
				if(base.Parent != value) {
					base.Parent = value;
				}
			}
		}
		public override Locations TextLocation {
			get {
				return Locations.Top;
			}
		}
		bool isLocked;
		protected internal bool IsLocked { get { return isLocked; } set { isLocked = value; } }
		public DashboardLayoutControlItemBase(DashboardLayoutControlGroupBase parent)
			: base(parent) {
				CustomHeaderButtons.CollectionChanged += OnButtonCollectionChanged;
		}
		public DashboardLayoutControlItemBase()
			: this(null) {
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
			return new DashboardLayoutControlItemViewInfo(this);
		}
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new DashboardLayoutControlItemViewInfo ViewInfo {
			get { return base.ViewInfo as DashboardLayoutControlItemViewInfo; }
		}
		private static readonly object buttonMouseClick = new object();
		public event DashboardLayoutControlItemButtonClickEventHandler ButtonMouseClick {
			add { base.Events.AddHandler(buttonMouseClick, value); }
			remove { base.Events.RemoveHandler(buttonMouseClick, value); }
		}
		protected internal void RaiseButtonMouseClick(int index) {
			DashboardLayoutControlItemButtonClickEventHandler handler = (DashboardLayoutControlItemButtonClickEventHandler)this.Events[buttonMouseClick];
			if(handler != null)
				handler(this, new DashboardLayoutControlItemButtonClickEventArgs() { ButtonIndex = index });
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
		protected internal override Size CalcDefaultMinMaxSize(bool getMin) {
			if(Parent != null) {
				if(getMin)
					return Parent.DefaultMinItemSize;
				else
					return Parent.DefaultMaxItemSize;
			} else
				return base.CalcDefaultMinMaxSize(getMin);
		}
		public void OnControlMouseDown(object sender, MouseEventArgs e) {
			((DashboardLayoutControl)Owner).handler.SelectedItem = this;
			((DashboardLayoutControl)Owner).RaiseMouseDown(sender, e);
		}
		public void OnControlMouseMove(object sender, MouseEventArgs e) {
			((DashboardLayoutControl)Owner).RaiseMouseMove(sender, e);
		}
		protected override void OnControlMouseLeave(object sender, EventArgs e) {
			base.OnControlMouseLeave(sender, e);
			((DashboardLayoutControl)Owner).RaiseMouseLeave(sender, e);
		}
		protected override void OnControlMouseEnter(object sender, EventArgs e) {
			base.OnControlMouseEnter(sender, e);
			((DashboardLayoutControl)Owner).RaiseMouseEnter(sender, e);
		}
		LayoutGroupAppearance paintAppearanceGroupCore = null;
		[Browsable(false)]
		public virtual LayoutGroupAppearance PaintAppearanceGroup {
			get {
				if(paintAppearanceGroupCore == null)
					paintAppearanceGroupCore = new LayoutGroupAppearance();
				return paintAppearanceGroupCore;
			}
		}
		internal DashboardViewMode ViewMode {
			get {
				if(Parent != null && !Parent.IsRoot && Parent.TextVisible) return DashboardViewMode.LayoutItem;
				return DashboardViewMode.LayoutGroup;
			}
		}
		protected internal override DevExpress.XtraLayout.HitInfo.BaseLayoutItemHitInfo CalcHitInfo(Point hitPoint, bool calcForHandler) {
			BaseLayoutItemHitInfo hitInfo = new DashboardHitInfo(base.CalcHitInfo(hitPoint, calcForHandler), LayoutGroupHitTypes.None);
			if(IsInCaptionArea(hitPoint)) {
				hitInfo = new DashboardHitInfo(hitInfo, LayoutGroupHitTypes.Caption);
			}
			int hotIndex = IsInExpandedButtonArea(hitPoint);
			if(hotIndex >= 0) hitInfo = new DashboardHitInfo(hitInfo, hotIndex);
			if(IsInCaptionImageArea(hitPoint)) hitInfo = new DashboardHitInfo(hitInfo, LayoutGroupHitTypes.CaptionImage);
			return hitInfo;
		}
		protected virtual int IsInExpandedButtonArea(Point point) {
			DashboardLayoutControlItemViewInfo vi = ViewInfo as DashboardLayoutControlItemViewInfo;
			BaseButtonInfo bbi = vi.BorderInfo.ButtonsPanel.ViewInfo.CalcHitInfo(point);
			if(bbi == null) return -1;
			return vi.Owner.CustomHeaderButtons.IndexOf(bbi.Button);
		}
		protected virtual bool IsInCaptionImageArea(Point point) {
			DashboardLayoutControlItemViewInfo vi = ViewInfo as DashboardLayoutControlItemViewInfo;
			if(vi == null)
				return false;
			return vi.BorderInfo.CaptionImageBounds.Contains(point);
		}
		protected virtual bool IsInCaptionArea(Point point) {
			if(ViewInfo == null) {
				LayoutControlItemViewInfo lcivi = ViewInfo as LayoutControlItemViewInfo;
				if(lcivi == null)
					return false;
				else
					return lcivi.TextAreaRelativeToControl.Contains(point);
			}
			if(ViewInfo is DashboardLayoutControlItemViewInfo) {
				return ((DashboardLayoutControlItemViewInfo)ViewInfo).BorderInfo.CaptionBounds.Contains(point);
			}
			return ViewInfo.TextAreaRelativeToControl.Contains(point);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(CustomHeaderButtons != null) CustomHeaderButtons.CollectionChanged -= OnButtonCollectionChanged;
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
		public virtual bool AllowDrawContentBorder { get { return true; } }
		#region IGroupBoxButtonsPanelOwner Members
		protected virtual BaseButtonsPanel ButtonsPanel {
			get { return ViewInfo != null ? ViewInfo.BorderInfo != null ? (ViewInfo.BorderInfo as GroupObjectInfoArgs).ButtonsPanel : null : null; }
		}
		void OnButtonCollectionChanged(object sender, CollectionChangeEventArgs e) {
			if(ButtonsPanel == null) return;
			ButtonsPanel.Buttons.Merge(CustomHeaderButtons);
			OnChanged();
		}
		protected void DoUpdateBorderInfo() {
			shouldResetBorderInfoCore = true;
			ShouldUpdateConstraintsDoUpdate = true;
			ComplexUpdate(true, true);
		}
		protected void OnChanged() {
			DoUpdateBorderInfo();
		}
		BaseButtonCollection customHeaderButtonsCore;
		public BaseButtonCollection CustomHeaderButtons {
			get {
				if(customHeaderButtonsCore == null) customHeaderButtonsCore = CreateButtonCollection();
				return customHeaderButtonsCore;
			}
		}
		protected virtual BaseButtonCollection CreateButtonCollection() {
			return new BaseButtonCollection((ViewInfo as DashboardLayoutControlItemViewInfo).BorderInfo.ButtonsPanel);
		}
		public bool IsRightToLeft {
			get { return this.IsRTL; }
		}
		public void LayoutChanged() {
		} 
		static readonly object customButtonClick = new object();
		[ Category("Behavior")]
		public event BaseButtonEventHandler CustomButtonClick {
			add { this.Events.AddHandler(customButtonClick, value); }
			remove { this.Events.RemoveHandler(customButtonClick, value); }
		}
		static readonly object customButtonUnchecked = new object();
		[ Category("Behavior")]
		public event BaseButtonEventHandler CustomButtonUnchecked {
			add { this.Events.AddHandler(customButtonUnchecked, value); }
			remove { this.Events.RemoveHandler(customButtonUnchecked, value); }
		}
		static readonly object customButtonChecked = new object();
		[ Category("Behavior")]
		public event BaseButtonEventHandler CustomButtonChecked {
			add { this.Events.AddHandler(customButtonChecked, value); }
			remove { this.Events.RemoveHandler(customButtonChecked, value); }
		}
		void IGroupBoxButtonsPanelOwner.RaiseButtonsPanelButtonClick(BaseButtonEventArgs ea) {
			BaseButtonEventHandler handler = (BaseButtonEventHandler)Events[customButtonClick];
			if(handler != null)
				handler(this, ea);
		}
		void IGroupBoxButtonsPanelOwner.RaiseButtonsPanelButtonChecked(BaseButtonEventArgs ea) {
			BaseButtonEventHandler handler = (BaseButtonEventHandler)Events[customButtonChecked];
			if(handler != null)
				handler(this, ea);
		}
		void IGroupBoxButtonsPanelOwner.RaiseButtonsPanelButtonUnchecked(BaseButtonEventArgs ea) {
			BaseButtonEventHandler handler = (BaseButtonEventHandler)Events[customButtonUnchecked];
			if(handler != null)
				handler(this, ea);
		}
		#endregion
		#region  IButtonsPanelOwner Members
		object IButtonsPanelOwner.ButtonBackgroundImages {
			get { return null; }
		}
		ButtonsPanelControlAppearance IButtonsPanelOwner.AppearanceButton {
			get { return new ButtonsPanelControlAppearance(this); }
		}
		bool IButtonsPanelOwner.Enabled {
			get { return Enabled; }
		}
		bool IButtonsPanelOwner.AllowHtmlDraw {
			get { return AllowHtmlStringInCaption; }
		}
		bool IButtonsPanelOwner.AllowGlyphSkinning {
			get { return AllowGlyphSkinning.ToBoolean(false); }
		}
		object IButtonsPanelOwner.Images {
			get { return null; }
		}
		bool IButtonsPanelOwner.IsSelected {
			get { return false; }
		}
		void IButtonsPanelOwner.Invalidate() {
		}
		ObjectPainter IButtonsPanelOwner.GetPainter() {
			if(Owner == null) return new GroupBoxButtonsPanelPainter();
			if(Owner.LookAndFeel.ActiveStyle == DevExpress.LookAndFeel.ActiveLookAndFeelStyle.Skin)
				return new DashboardGroupBoxButtonsSkinPainter(Owner.LookAndFeel);
			if(Owner.LookAndFeel.ActiveStyle == DevExpress.LookAndFeel.ActiveLookAndFeelStyle.WindowsXP)
				return new GroupBoxButtonsPanelWindowsXpPainter();
			if(Owner.LookAndFeel.ActiveStyle == DevExpress.LookAndFeel.ActiveLookAndFeelStyle.Office2003)
				return new BaseButtonsPanelOffice2003Painter();
			return new GroupBoxButtonsPanelPainter();
		}
		#endregion
		#region IButtonPanelControlAppearanceOwner Members
		public IButtonsPanelControlAppearanceProvider CreateAppearanceProvider() {
			var provider = new ButtonsPanelControlAppearanceProvider();
			return provider;
		}
		#endregion
		#region IAppearanceOwner Members
		public bool IsLoading {
			get { return false; }
		}
		#endregion
	}
	public class DashboardLayoutControlItemViewInfo :LayoutControlItemViewInfo {
		public DashboardLayoutControlItemViewInfo(DashboardLayoutControlItemBase owner)
			: base(owner) {
		}
		protected override Size AddLabel(Size size) {
			return size;
		}
		protected override void UpdateTextArea() {
		}
		public void Dispose() {
			base.Destroy();
		}
		protected internal override void Destroy() {
			base.Destroy();
			if((borderInfoCore as DashboardGroupObjectInfoArgs) != null)
				(borderInfoCore as DashboardGroupObjectInfoArgs).Dispose();
			if((LastInfo as DashboardGroupObjectInfoArgs) != null)
				(LastInfo as DashboardGroupObjectInfoArgs).Dispose();
			LastInfo = null;
		}
		public Rectangle BackgroundArea {
			get;
			set;
		}
		protected override Size AddIndentions(Size size) {
			Size result = base.AddIndentions(size);
			if(ShouldCorrectLockedItemHeightInDraggingState()) {
				result.Height -= DashboardLayoutSettings.DragDropIndicatorSize;
			}
			return result;
		}
		public virtual Rectangle BackgroundAreaRelativeToControl {
			get {
				return TranslateCoordinates(BackgroundArea);
			}
		}
		bool ShouldCorrectLockedItemHeightInDraggingState() {
			if(!Owner.IsLocked) return false;
			if(OwnerILayoutControl == null) return false;
			if(!(OwnerILayoutControl is DashboardLayoutControl)) return false;
			if((OwnerILayoutControl as DashboardLayoutControl).handler == null) return false;
			if((OwnerILayoutControl as DashboardLayoutControl).AdornerWindowHandlerState != AdornerWindowHandlerStates.Dragging) return false;
			if(Owner.Spacing.Height >= DashboardLayoutSettings.DragDropIndicatorSize && Owner.MaxSize.Height < Owner.Height) return true;
			return false;
		}
		protected override Rectangle CalcIndentionsRect() {
			Rectangle result = base.CalcIndentionsRect();
			if(ShouldCorrectLockedItemHeightInDraggingState())
				result.Height += DashboardLayoutSettings.DragDropIndicatorSize;
			return result;
		}
		protected override void CalculateRegionsCore() {
			ClientArea = CalcIndentionsRect();
			TextArea = Rectangle.Empty;
			var temp = GetPainterPadding();
			BackgroundArea = new Rectangle(ClientArea.Left - temp.Left, ClientArea.Top - temp.Top, ClientArea.Width + temp.Width, ClientArea.Height + temp.Height);
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
		public new DashboardLayoutControlItemBase Owner {
			get { return (DashboardLayoutControlItemBase)base.Owner; }
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
		public void UpdateAppearanceGroup(GroupObjectInfoArgs tempBorderInfo) {
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
			if(OwnerILayoutControl != null) tempBorderInfo.RightToLeft = Owner.IsRTL;
			if(!Owner.EnabledState)
				tempBorderInfo.State = ObjectState.Disabled;
		}
		protected override void CalculatePaddingCore() { if(Owner.BordersVisible) base.CalculatePaddingCore(); }
	}
	public class Crosshair {
		CrosshairGroupingTypes userCrosshairTypeCore;
		internal CrosshairGroupingTypes realCrosshairTypeCore;
		BaseLayoutItem lt, lb, rt, rb;
		public Crosshair() {
			userCrosshairTypeCore = CrosshairGroupingTypes.NotSet;
			realCrosshairTypeCore = CrosshairGroupingTypes.GroupVertical;
		}
		public override bool Equals(object obj) {
			if(lt == null || rt == null || lb == null || rb == null) return base.Equals(obj);
			Crosshair rObj = obj as Crosshair;
			if(rObj == null) return base.Equals(obj);
			return lt == rObj.lt &&
				   rt == rObj.rt &&
				   rb == rObj.rb &&
				   lb == rObj.lb;
		}
		public bool Contains(BaseLayoutItem item) {
			return item == lt || item == lb || item == rt || item == rb;
		}
		public override int GetHashCode() {
			if(lt == null || rt == null || lb == null || rb == null) return base.GetHashCode();
			return lt.GetHashCode() + rt.GetHashCode() + lb.GetHashCode() + rb.GetHashCode();
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		internal BaseLayoutItem LeftTopItem { get { return lt; } set { lt = value; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		internal BaseLayoutItem LeftBottomItem { get { return lb; } set { lb = value; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		internal BaseLayoutItem RightTopItem { get { return rt; } set { rt = value; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		internal BaseLayoutItem RightBottomItem { get { return rb; } set { rb = value; } }
		public List<BaseLayoutItem> GetTwoItems(bool bottomOrRight, LayoutType lType) {
			List<BaseLayoutItem> returnListBLI = new List<BaseLayoutItem>();
			switch(userCrosshairTypeCore) {
				case CrosshairGroupingTypes.GroupHorizontal:
					GetHorizontalItems(bottomOrRight, returnListBLI);
					break;
				case CrosshairGroupingTypes.GroupVertical:
					GetVerticalItems(bottomOrRight, returnListBLI);
					break;
				case CrosshairGroupingTypes.GroupBoth:
					if(lType == LayoutType.Vertical) GetHorizontalItems(bottomOrRight, returnListBLI);
					else GetVerticalItems(bottomOrRight, returnListBLI);
					break;
				default:
					break;
			}
			return returnListBLI;
		}
		private void GetVerticalItems(bool bottomOrRight, List<BaseLayoutItem> returnListBLI) {
			if(!bottomOrRight) GetLeftItems(returnListBLI);
			else GetRightItems(returnListBLI);
		}
		private void GetHorizontalItems(bool bottomOrRight, List<BaseLayoutItem> returnListBLI) {
			if(!bottomOrRight) GetTopItems(returnListBLI);
			else GetBottomItems(returnListBLI);
		}
		private void GetRightItems(List<BaseLayoutItem> returnListBLI) {
			returnListBLI.Add(rt);
			returnListBLI.Add(rb);
		}
		private void GetLeftItems(List<BaseLayoutItem> returnListBLI) {
			returnListBLI.Add(lt);
			returnListBLI.Add(lb);
		}
		private void GetBottomItems(List<BaseLayoutItem> returnListBLI) {
			returnListBLI.Add(lb);
			returnListBLI.Add(rb);
		}
		private void GetTopItems(List<BaseLayoutItem> returnListBLI) {
			returnListBLI.Add(lt);
			returnListBLI.Add(rt);
		}
		protected internal BaseLayoutItem[] GetItems() { return new BaseLayoutItem[] { lt, rt, lb, rb }; }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		public CrosshairGroupingTypes CrosshairGroupingType {
			get { return userCrosshairTypeCore; }
			set {
				if(userCrosshairTypeCore == value) return;
				userCrosshairTypeCore = value;
			}
		}
		public Rectangle Bounds {
			get {
				return Rectangle.Union(LeftTopItem.ViewInfo.BoundsRelativeToControl, RightBottomItem.ViewInfo.BoundsRelativeToControl);
			}
		}
		CrosshairLine verticalLine, horizontalLine;
		public CrosshairLine VerticalLine {
			get {
				if(verticalLine == null) {
					Rectangle tempRect = Rectangle.Union(lt.Bounds, lb.Bounds);
					verticalLine = new CrosshairLine() { Location = new Point(tempRect.X + tempRect.Width, tempRect.Y), Length = tempRect.Height };
				}
				return verticalLine;
			}
		}
		public CrosshairLine HorizontalLine {
			get {
				if(horizontalLine == null) {
					Rectangle tempRect = Rectangle.Union(lt.Bounds, rt.Bounds);
					horizontalLine = new CrosshairLine() { Location = new Point(tempRect.X, tempRect.Y + tempRect.Height), Length = tempRect.Width };
				}
				return horizontalLine;
			}
		}
		internal bool ContainsLockedItems() {
			if(LeftTopItem is DashboardLayoutControlItemBase && (LeftTopItem as DashboardLayoutControlItemBase).IsLocked) return true;
			if(LeftBottomItem is DashboardLayoutControlItemBase && (LeftBottomItem as DashboardLayoutControlItemBase).IsLocked) return true;
			if(RightTopItem is DashboardLayoutControlItemBase && (RightTopItem as DashboardLayoutControlItemBase).IsLocked) return true;
			if(RightBottomItem is DashboardLayoutControlItemBase && (RightBottomItem as DashboardLayoutControlItemBase).IsLocked) return true;
			return false;
		}
	}
	public class CrosshairLine {
		public Point Location;
		public int Length;
	}
	public enum CrosshairGroupingTypes {
		GroupHorizontal,
		GroupVertical,
		GroupBoth,
		NotSet
	}
	public enum DashboardViewMode {
		LayoutGroup,
		LayoutItem
	}
	public delegate void DashboardLayoutControlItemButtonClickEventHandler(object sender, DashboardLayoutControlItemButtonClickEventArgs e);
	public class DashboardLayoutControlItemButtonClickEventArgs :EventArgs {
		public int ButtonIndex { get; set; }
	}
}
