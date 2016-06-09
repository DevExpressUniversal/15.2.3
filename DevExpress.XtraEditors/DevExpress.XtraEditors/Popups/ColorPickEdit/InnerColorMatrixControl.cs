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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.ViewInfo;
namespace DevExpress.XtraEditors {
	[ToolboxItem(false)]
	public class InnerColorMatrixControl : InnerColorPickControlBase {
		static readonly Size DefaultItemSize = new Size(19, 19);
		const int DefaultItemGap = 1;
		Size itemSize;
		int itemGap;
		ColorItemCollection colors;
		int topRow;
		ColorTooltipFormat colorTooltipFormat;
		InnerColorMatrixScrollController scrollController;
		public InnerColorMatrixControl() {
			this.itemSize = DefaultItemSize;
			this.itemGap = DefaultItemGap;
			this.colors = new ColorItemCollection();
			this.colors.ListChanged += OnColorListChanged;
			this.topRow = 0;
			this.colorTooltipFormat = ColorTooltipFormat.Argb;
			this.scrollController = CreateScrollController();
			this.scrollController.AddControls(this);
			this.scrollController.VScrollValueChanged += OnVScrollValueChanged;
			SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer, true);
		}
		protected override Padding DefaultPadding {
			get { return new Padding(2, 2, 0, 2); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color BackColor {
			get { return Color.Transparent; }
			set { base.BackColor = value; }
		}
		protected override BaseControlPainter CreatePainter() {
			return new InnerColorMatrixControlPainter();
		}
		protected override BaseStyleControlViewInfo CreateViewInfo() {
			return new InnerColorMatrixControlViewInfo(this);
		}
		protected virtual InnerColorMatrixScrollController CreateScrollController() {
			return new InnerColorMatrixScrollController(this);
		}
		public Size ItemSize {
			get { return itemSize; }
			set {
				if(ItemSize == value)
					return;
				itemSize = value;
				OnPropertiesChanged();
			}
		}
		bool ShouldSerializeItemSize() { return ItemSize != DefaultItemSize; }
		void ResetItemSize() { ItemSize = DefaultItemSize; }
		[DefaultValue(DefaultItemGap)]
		public int ItemGap {
			get { return itemGap; }
			set {
				if(ItemGap == value)
					return;
				itemGap = value;
				OnPropertiesChanged();
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ColorItemCollection Colors {
			get { return colors; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int TopRow {
			get { return topRow; }
			set {
				if(TopRow == value)
					return;
				topRow = value;
				OnPropertiesChanged();
			}
		}
		[DefaultValue(ColorTooltipFormat.Argb)]
		public ColorTooltipFormat ColorTooltipFormat {
			get { return colorTooltipFormat; }
			set {
				if(ColorTooltipFormat == value)
					return;
				colorTooltipFormat = value;
				OnPropertiesChanged();
			}
		}
		InnerColorMatrixControlHandler handler;
		public InnerColorMatrixControlHandler Handler {
			get {
				if(this.handler == null) {
					this.handler = CreateHandler();
				}
				return this.handler;
			}
		}
		protected virtual InnerColorMatrixControlHandler CreateHandler() {
			return new InnerColorMatrixControlHandler(this);
		}
		public void SetTopRow(int topRow) {
			int maxTopItem = Math.Max(0, ViewInfo.CalcRowCount() - ViewInfo.CalcVisibleRowCount() + 1);
			TopRow = Math.Min(Math.Max(0, topRow), maxTopItem);
		}
		protected override void OnMouseEnter(EventArgs e) {
			Handler.OnMouseEnter(e);
			base.OnMouseEnter(e);
		}
		protected override void OnMouseLeave(EventArgs e) {
			Handler.OnMouseLeave(e);
			base.OnMouseLeave(e);
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			Handler.OnMouseMove(e);
			base.OnMouseMove(e);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			Handler.OnMouseDown(e);
			base.OnMouseDown(e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			Handler.OnMouseUp(e);
			base.OnMouseUp(e);
		}
		protected override void OnMouseWheel(MouseEventArgs e) {
			Handler.OnMouseWheel(e);
			base.OnMouseWheel(e);
		}
		protected override void OnPaddingChanged(EventArgs e) {
			base.OnPaddingChanged(e);
			OnPropertiesChanged();
		}
		void OnColorListChanged(object sender, ListChangedEventArgs e) {
			OnColorListChanged(e);
		}
		protected virtual void OnColorListChanged(ListChangedEventArgs e) {
			OnPropertiesChanged();
		}
		protected virtual void OnVScrollValueChanged(object sender, EventArgs e) {
			TopRow = ScrollController.VScrollPosition;
		}
		int lockScrollUpdate = 0;
		protected virtual void UpdateScrollBars() {
			if(this.lockScrollUpdate != 0) return;
			this.lockScrollUpdate++;
			try {
				ScrollController.IsRightToLeft = IsRightToLeft;
				ScrollController.VScrollVisible = ViewInfo.IsVScrollVisible;
				ScrollController.ClientRect = ViewInfo.ClientRect;
				if(ScrollController.VScrollVisible) ScrollController.VScrollArgs = ViewInfo.CalcVScrollArgs();
			}
			finally {
				this.lockScrollUpdate--;
			}
		}
		protected internal override void LayoutChanged() {
			base.LayoutChanged();
			if(!ViewInfo.IsVScrollVisible) {
				TopRow = 0;
			}
			UpdateScrollBars();
		}
		public override bool ContainsColor(Color color) {
			return Colors.ContainsColor(color, true);
		}
		public override ColorItem GetColorItemByColor(Color color) {
			return Colors.GetItem(color, true);
		}
		protected internal InnerColorMatrixScrollController ScrollController { get { return scrollController; } }
		protected override ToolTipControlInfo GetToolTipInfo(Point pt) {
			InnerColorMatrixControlHitInfo hitInfo = ViewInfo.CalcHitInfo(pt);
			if(hitInfo.HitTest == InnerColorMatrixControlHitTest.Item) {
				return ColorItemUtils.GetTooltipInfo(hitInfo.ColorItem, ColorTooltipFormat);
			}
			return base.GetToolTipInfo(pt);
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(this.handler != null) {
					this.handler.Dispose();
				}
				this.handler = null;
				if(ScrollController != null) {
					ScrollController.VScrollValueChanged -= OnVScrollValueChanged;
					ScrollController.RemoveControls(this);
					ScrollController.Dispose();
				}
				this.scrollController = null;
			}
			base.Dispose(disposing);
		}
		protected internal new InnerColorMatrixControlViewInfo ViewInfo { get { return base.ViewInfo as InnerColorMatrixControlViewInfo; } }
	}
	public class InnerColorMatrixControlHandler : IDisposable {
		InnerColorMatrixControl owner;
		public InnerColorMatrixControlHandler(InnerColorMatrixControl owner) {
			this.owner = owner;
		}
		public virtual void OnMouseDown(MouseEventArgs e) {
			Owner.ViewInfo.UpdateState(e.Location);
		}
		public virtual void OnMouseUp(MouseEventArgs e) {
			Owner.ViewInfo.UpdateState(e.Location);
			DoCheckClick(e.Location);
		}
		public virtual void OnMouseEnter(EventArgs e) {
			Owner.ViewInfo.CheckHotObject();
		}
		public virtual void OnMouseLeave(EventArgs e) {
			Owner.ViewInfo.CheckHotObject();
		}
		public virtual void OnMouseMove(MouseEventArgs e) {
			Owner.ViewInfo.CheckHotObject(e.Location);
		}
		public virtual void OnMouseWheel(MouseEventArgs e) {
			if(!Owner.ViewInfo.IsVScrollVisible) return;
			DoMouseWheel(e.Delta);
		}
		protected virtual void DoMouseWheel(int delta) {
			int wheelChange =  1;
			Owner.SetTopRow(Owner.TopRow + ((delta > 0) ? -wheelChange : wheelChange));
		}
		protected virtual void DoCheckClick(Point pt) {
			InnerColorMatrixControlHitInfo hitInfo = Owner.ViewInfo.CalcHitInfo(pt);
			if(hitInfo.HitTest == InnerColorMatrixControlHitTest.Item) {
				Owner.SelectedColorItem = hitInfo.ColorItem.ColorItem;
			}
		}
		public InnerColorMatrixControl Owner { get { return owner; } }
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
		}
	}
	public enum InnerColorMatrixControlHitTest {
		None, Item
	}
	public class InnerColorMatrixControlHitInfo {
		Point hitPoint;
		object hitObject;
		InnerColorMatrixControlHitTest hitTest;
		public InnerColorMatrixControlHitInfo() : this(Point.Empty) { }
		public InnerColorMatrixControlHitInfo(Point hitPoint) {
			this.hitPoint = hitPoint;
			this.hitObject = null;
			this.hitTest = InnerColorMatrixControlHitTest.None;
		}
		public void SetHitTest(InnerColorMatrixControlHitTest hitTest) {
			this.hitTest = hitTest;
		}
		public void SetHitObject(object hitObject) {
			this.hitObject = hitObject;
		}
		public void Reset() {
			this.hitPoint = Point.Empty;
			this.hitObject = null;
		}
		public static readonly InnerColorMatrixControlHitInfo Empty = new InnerColorMatrixControlHitInfo();
		public bool IsEmpty {
			get { return Empty.Equals(this); }
		}
		public override bool Equals(object obj) {
			InnerColorMatrixControlHitInfo other = obj as InnerColorMatrixControlHitInfo;
			if(other == null) return false;
			return other.HitTest == HitTest && other.hitObject == hitObject;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		public ColorItemInfo ColorItem { get { return (ColorItemInfo)hitObject; } }
		public Point HitPoint { get { return hitPoint; } }
		public InnerColorMatrixControlHitTest HitTest { get { return hitTest; } }
	}
	public class InnerColorMatrixControlHotObject {
		InnerColorMatrixControlHitInfo hotObject;
		public InnerColorMatrixControlHotObject() {
			this.hotObject = InnerColorMatrixControlHitInfo.Empty;
		}
		public void SetHotObject(InnerColorMatrixControlHitInfo newObj) {
			if(newObj.Equals(this.hotObject)) return;
			InnerColorMatrixControlHitInfo prev = this.hotObject;
			this.hotObject = newObj;
			OnHotObjectChanged(new InnerColorMatrixControlHotObjectChangedEventArgs(prev, newObj));
		}
		public void Reset() {
			if(this.hotObject.IsEmpty) return;
			InnerColorMatrixControlHitInfo prev = this.hotObject;
			this.hotObject = InnerColorMatrixControlHitInfo.Empty;
			OnHotObjectChanged(new InnerColorMatrixControlHotObjectChangedEventArgs(prev, this.hotObject));
		}
		protected virtual void OnHotObjectChanged(InnerColorMatrixControlHotObjectChangedEventArgs e) {
			if(HotObjectChanged != null) HotObjectChanged(this, e);
		}
		public event InnerColorMatrixControlHotObjectChangedHandler HotObjectChanged;
	}
	public delegate void InnerColorMatrixControlHotObjectChangedHandler(object sender, InnerColorMatrixControlHotObjectChangedEventArgs e);
	public class InnerColorMatrixControlHotObjectChangedEventArgs : EventArgs {
		InnerColorMatrixControlHitInfo prev;
		InnerColorMatrixControlHitInfo next;
		public InnerColorMatrixControlHotObjectChangedEventArgs(InnerColorMatrixControlHitInfo prev, InnerColorMatrixControlHitInfo next) {
			this.prev = prev;
			this.next = next;
		}
		public InnerColorMatrixControlHitInfo Prev { get { return prev; } }
		public InnerColorMatrixControlHitInfo Next { get { return next; } }
	}
	public class InnerColorMatrixControlViewInfo : BaseStyleControlViewInfo {
		Hashtable items;
		bool isVScrollVisible;
		InnerColorMatrixControlHotObject hotObject;
		public InnerColorMatrixControlViewInfo(InnerColorMatrixControl owner) : base(owner) {
			this.items = new Hashtable();
			this.isVScrollVisible = false;
			this.hotObject = new InnerColorMatrixControlHotObject();
			this.hotObject.HotObjectChanged += OnHotObjectChanged;
		}
		public InnerColorMatrixControlHitInfo CalcHitInfo(Point pt) {
			InnerColorMatrixControlHitInfo hitInfo = new InnerColorMatrixControlHitInfo(pt);
			foreach(ColorItemInfo item in VisibleItems) {
				if(item.Bounds.Contains(pt)) {
					hitInfo.SetHitTest(InnerColorMatrixControlHitTest.Item);
					hitInfo.SetHitObject(item);
					break;
				}
			}
			return hitInfo;
		}
		void OnHotObjectChanged(object sender, InnerColorMatrixControlHotObjectChangedEventArgs e) {
			OnHotObjectChanged(e);
		}
		protected virtual void OnHotObjectChanged(InnerColorMatrixControlHotObjectChangedEventArgs e) {
			if(e.Prev.HitTest == InnerColorMatrixControlHitTest.Item) {
				UpdateState(e.Prev.ColorItem);
			}
			if(e.Next.HitTest == InnerColorMatrixControlHitTest.Item) {
				UpdateState(e.Next.ColorItem);
			}
			OwnerControl.Invalidate();
		}
		protected virtual void UpdateState(ColorItemInfo colorItem) {
			UpdateColorItemState(colorItem);
			UpdateColorItemAppearance(colorItem);
		}
		protected virtual void UpdateColorItemState(ColorItemInfo itemInfo) {
			if(!OwnerControl.Enabled) {
				itemInfo.SetState(ObjectState.Disabled);
				return;
			}
			if(OwnerControl.IsDesignMode) return;
			ObjectState state = ObjectState.Normal;
			if(Owner.SelectedColorItem != null && Owner.SelectedColorItem.Equals(itemInfo.ColorItem)) {
				state = ObjectState.Selected;
			}
			Point pt = OwnerControl.PointToClient(Control.MousePosition);
			if(itemInfo.Bounds.Contains(pt)) {
				state |= ObjectState.Hot;
				if((Control.MouseButtons & MouseButtons.Left) != 0) state |= ObjectState.Pressed;
			}
			itemInfo.SetState(state);
		}
		protected virtual void UpdateColorItemAppearance(ColorItemInfo itemInfo) {
			itemInfo.PaintAppearance.Assign(PaintAppearance);
		}
		protected internal virtual void UpdateState(Point pt) {
			InnerColorMatrixControlHitInfo hi = CalcHitInfo(pt);
			if(hi.HitTest == InnerColorMatrixControlHitTest.Item) {
				UpdateState(hi.ColorItem);
				OwnerControl.Invalidate();
			}
		}
		protected internal void CheckHotObject() { CheckHotObject(GetMousePoint()); }
		protected internal void CheckHotObject(Point pt) {
			InnerColorMatrixControlHitInfo hi = CalcHitInfo(pt);
			this.hotObject.SetHotObject(hi);
		}
		protected internal void ResetHotObject() {
			this.hotObject.Reset();
		}
		protected Point GetMousePoint() { return OwnerControl.PointToClient(Control.MousePosition); }
		protected override void CalcContentRect(Rectangle bounds) {
			base.CalcContentRect(bounds);
			AdjustContent(bounds);
			CalcItems();
		}
		protected virtual void AdjustContent(Rectangle bounds) {
			ApplyPadding();
			bool vScrollVisible = CalcVScrollBarVisibility();
			if(vScrollVisible) {
				if(!RightToLeft) {
					this.fContentRect.Width -= Owner.ScrollController.VScrollWidth;
				}
				else {
					this.fContentRect.X += Owner.ScrollController.VScrollWidth;
					this.fContentRect.Width -= Owner.ScrollController.VScrollWidth;
				}
			}
			this.isVScrollVisible = vScrollVisible;
		}
		protected virtual bool CalcVScrollBarVisibility() {
			return CalcRowCount() > CalcVisibleRowCount();
		}
		public bool IsVScrollVisible { get { return isVScrollVisible; } }
		public virtual ScrollArgs CalcVScrollArgs() {
			ScrollArgs args = new ScrollArgs();
			args.Maximum = CalcRowCount();
			args.LargeChange = CalcVisibleRowCount();
			args.Value = Owner.TopRow;
			return args;
		}
		protected new InnerColorMatrixControl Owner { get { return base.Owner as InnerColorMatrixControl; } }
		protected void ApplyPadding() {
			Padding padding = OwnerControl.Padding;
			this.fContentRect.X += padding.Left;
			this.fContentRect.Width -= (padding.Left + padding.Right);
			this.fContentRect.Y += padding.Top;
			this.fContentRect.Height -= (padding.Top + padding.Bottom);
		}
		protected virtual void CalcItems() {
			this.items.Clear();
			ColorItemCollection colors = Owner.Colors;
			Point hotPt = ContentRect.Location;
			for(int i = GetRowStartPos(Owner.TopRow), n = 0; i < colors.Count; i++, n++) {
				ColorItem item = colors[i];
				ColorItemInfo itemInfo = CreateItem(item, n, hotPt);
				this.items.Add(item, itemInfo);
				UpdateState(itemInfo);
				hotPt.X = itemInfo.Bounds.Right;
				hotPt.Y = itemInfo.Bounds.Top;
			}
		}
		protected virtual ColorItemInfo CreateItem(ColorItem colorItem, int pos, Point hotPoint) {
			ColorItemInfo item = new ColorItemInfo(colorItem);
			item.SetBounds(CalcItemBounds(pos, hotPoint));
			item.SetRow(GetRowIndex(pos));
			return item;
		}
		protected virtual Rectangle CalcItemBounds(int pos, Point hotPoint) {
			Rectangle rect = new Rectangle(hotPoint, Owner.ItemSize);
			if(Owner.ItemGap > 0 && GetIndexInRow(pos) != 0) {
				rect.X += Owner.ItemGap;
			}
			if(rect.Right > ContentRect.Right) {
				rect.X = ContentRect.X;
				rect.Y = rect.Bottom + Owner.ItemGap;
			}
			return rect;
		}
		protected internal int CalcRowCount() {
			int itemsInRow = CalcItemsInRow();
			if(itemsInRow == 0) return 0;
			int rowCount = Owner.Colors.Count / itemsInRow;
			if(Owner.Colors.Count % itemsInRow > 0) {
				rowCount++;
			}
			return rowCount;
		}
		protected int GetIndexInRow(int itemIndex) {
			int itemsInRow = CalcItemsInRow();
			return itemsInRow != 0 ? (itemIndex % itemsInRow) : 0;
		}
		protected int GetRowIndex(int itemIndex) {
			int itemsInRow = CalcItemsInRow();
			return itemsInRow != 0 ? (itemIndex / itemsInRow) : 0;
		}
		protected int GetRowStartPos(int row) {
			return CalcItemsInRow() * row;
		}
		protected int CalcItemsInRow() {
			int itemTotalWidth = Owner.ItemSize.Width + Owner.ItemGap;
			int itemCount = (int)(ContentRect.Width / itemTotalWidth);
			if(ContentRect.Width % itemTotalWidth >= Owner.ItemSize.Width) {
				return itemCount + 1;
			}
			return itemCount;
		}
		protected internal int CalcVisibleRowCount() {
			int itemTotalWidth = Owner.ItemSize.Height + Owner.ItemGap;
			if(itemTotalWidth == 0) return 0;
			int rowCount = ContentRect.Height / itemTotalWidth;
			if(ContentRect.Height % itemTotalWidth > 0) {
				rowCount++;
			}
			return rowCount;
		}
		public virtual Color HotItemBorderColor { get { return Color.FromArgb(242, 149, 54); } }
		public virtual Color HotItemInnerBorderColor { get { return Color.FromArgb(255, 227, 149); } }
		public virtual Color SelectedItemBorderColor { get { return Color.FromArgb(239, 72, 16); } }
		public virtual Color SelectedItemInnerBorderColor { get { return Color.FromArgb(255, 226, 148); } }
		public ICollection VisibleItems { get { return items.Values; } }
	}
	public class InnerColorMatrixControlPainter : BaseControlPainter {
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			base.DrawContent(info);
			DrawItems(info);
		}
		protected virtual void DrawItems(ControlGraphicsInfoArgs info) {
			InnerColorMatrixControlViewInfo viewInfo = (InnerColorMatrixControlViewInfo)info.ViewInfo;
			foreach(ColorItemInfo item in viewInfo.VisibleItems) {
				DoDrawItem(item, info);
			}
		}
		protected virtual void DoDrawItem(ColorItemInfo item, ControlGraphicsInfoArgs info) {
			InnerColorMatrixControlViewInfo viewInfo = (InnerColorMatrixControlViewInfo)info.ViewInfo;
			info.Cache.FillRectangle(info.Cache.GetSolidBrush(item.Color), item.Bounds);
			if((item.State & ObjectState.Hot) != 0) {
				info.Cache.DrawRectangle(info.Cache.GetPen(viewInfo.HotItemBorderColor), item.Bounds);
				info.Cache.DrawRectangle(info.Cache.GetPen(viewInfo.HotItemInnerBorderColor), Rectangle.Inflate(item.Bounds, -1, -1));
			}
			else if((item.State & ObjectState.Selected) != 0) {
				info.Cache.DrawRectangle(info.Cache.GetPen(viewInfo.SelectedItemBorderColor), item.Bounds);
				info.Cache.DrawRectangle(info.Cache.GetPen(viewInfo.SelectedItemInnerBorderColor), Rectangle.Inflate(item.Bounds, -1, -1));
			}
		}
	}
	public class InnerColorMatrixScrollController : ScrollControllerBase {
		public InnerColorMatrixScrollController(InnerColorMatrixControl owner)
			: base(owner) {
		}
	}
}
