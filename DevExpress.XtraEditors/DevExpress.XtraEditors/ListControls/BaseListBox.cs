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
using System.Collections;
using System.Windows.Forms;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ListControls;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.Utils;
using DevExpress.Utils.Paint;
using DevExpress.LookAndFeel;
using DevExpress.Utils.Drawing;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Accessibility;
using DevExpress.Skins;
using System.Collections.Generic;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Gesture;
using DevExpress.Utils.Text;
using DevExpress.Utils.Design;
namespace DevExpress.XtraEditors.ViewInfo {
	public class BaseListBoxViewInfo : BaseStyleControlViewInfo, IServiceProvider, IStringImageProvider {
		BaseListBoxControl owner;
		int columnCount, itemsPerColumn, itemHeight, columnWidth;
		Hashtable itemsInfo;
		StringFormat strFormat;
		Hashtable itemSizesCache;
		int focusedItemIndex, hotItemIndex;
		bool isNeededHScroll, isNeededVScroll;
		int bestColumnWidth;
		TextOptions defaultListBoxTextOptions;
		ListBoxItemObjectInfoArgs listBoxItemInfoArgs;
		BaseListBoxItemPainter itemPainter;
		public BaseListBoxViewInfo(BaseListBoxControl owner)
			: base(owner) {
			this.owner = owner;
			this.itemsInfo = new Hashtable();
			this.strFormat = new StringFormat(PaintAppearance.GetStringFormat());
			this.defaultListBoxTextOptions = new TextOptions(PaintAppearance);
			this.listBoxItemInfoArgs = CreateListBoxItemInfoArgs();
		}
		protected virtual bool HasItemActions { get { return OwnerControl != null && OwnerControl.HasItemActions; } }
		protected override Font GetDefaultFont() {
			if(!IsSkinLookAndFeel) return base.GetDefaultFont();
			return GetDefaultSkinFont(CommonSkins.SkinDropDown);
		}
		public override void CalcViewInfo(Graphics g) {
			this.itemHeight = 0;
			base.CalcViewInfo(g);
		}
		public override AppearanceDefault DefaultAppearance {
			get {
				bool useDisabledStatePainter = !OwnerControl.Enabled && !OwnerControl.IsDesignMode && OwnerControl.UseDisabledStatePainter;
				if(!useDisabledStatePainter)
					return base.DefaultAppearance;
				if(LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
					Skin skin = CommonSkins.GetSkin(LookAndFeel);
					return new AppearanceDefault(skin.Colors.GetColor(CommonColors.DisabledText), skin.Colors.GetColor(CommonColors.DisabledControl), GetDefaultFont());
				}
				return new AppearanceDefault(SystemColors.GrayText, SystemColors.Control);
			}
		}
		protected int ItemCount { get { return OwnerControl.ItemCount; } }
		public override TextOptions DefaultTextOptions { get { return defaultListBoxTextOptions; } }
		public int VisibleColumnCount {
			get {
				if(!OwnerControl.MultiColumn || ColumnWidth == 0) return 1;
				return Math.Max(1, ContentRect.Width / ColumnWidth);
			}
		}
		internal int FocusedItemIndex { get { return focusedItemIndex; } set { focusedItemIndex = value; } }
		internal ItemInfo HotItem {
			get {
				return GetItemByIndex(HotItemIndex);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public int HotItemIndex {
			get {
				if(!OwnerControl.IsAdvHotTrackEnabled()) return FocusedItemIndex;
				return hotItemIndex;
			}
			set {
				if(!OwnerControl.IsAdvHotTrackEnabled()) {
					FocusedItemIndex = value;
					return;
				}
				hotItemIndex = value;
			}
		}
		internal int LeftCoord { get { return OwnerControl.LeftCoord; } }
		public int ItemsPerColumn { get { return itemsPerColumn; } }
		public ICollection VisibleItems { get { return itemsInfo.Values; } }
		public int ColumnCount { get { return columnCount; } }
		public override bool IsSupportFastViewInfo { get { return false; } }
		public bool IsNeededHScroll { get { return isNeededHScroll; } }
		public bool IsNeededVScroll { get { return isNeededVScroll; } }
		public virtual bool DrawFocusRect { get { return OwnerControl.ShowFocusRect; } }
		protected new BaseListBoxControl OwnerControl { get { return (base.OwnerControl as BaseListBoxControl); } }
		public int ItemHeight {
			get {
				if(OwnerControl != null && OwnerControl.ItemHeight > 0) return OwnerControl.ItemHeight;
				if(itemHeight == 0) itemHeight = CalcItemMinHeight();
				return itemHeight;
			}
			set {
				itemHeight = value;
				itemSizesCache.Clear();
			}
		}
		public int ColumnWidth {
			get {
				if(columnWidth == 0) columnWidth = BestColumnWidth;
				return columnWidth;
			}
			set {
				columnWidth = value;
			}
		}
		public int BestColumnWidth {
			get {
				if(bestColumnWidth == -1) bestColumnWidth = CalcBestColumnWidth();
				return bestColumnWidth;
			}
		}
		public Rectangle BaseTextBounds { get; set; }
		public virtual int GetTextAscentHeight() {
			GInfo.AddGraphics(null);
			try {
				return DevExpress.Utils.Text.TextUtils.GetFontAscentHeight(GInfo.Graphics, PaintAppearance.Font);
			}
			finally {
				GInfo.ReleaseGraphics();
			}
		}
		protected internal bool IsSkinnedHighlightingEnabled {
			get { return OwnerControl.IsSkinnedHightlighingEnabled(); }
		}
		protected internal ListBoxItemObjectInfoArgs ListBoxItemInfoArgs {
			get { return listBoxItemInfoArgs; }
		}
		protected internal BaseListBoxItemPainter ListBoxItemPainter {
			get { return this.itemPainter; }
		}
		protected override void UpdatePainters() {
			base.UpdatePainters();
			this.itemPainter = CreateItemPainter();
		}
		protected virtual BaseListBoxItemPainter CreateItemPainter() {
			if(IsSkinnedHighlightingEnabled)
				return new ListBoxSkinItemPainter();
			return new ListBoxItemPainter();
		}
		protected virtual ListBoxItemObjectInfoArgs CreateListBoxItemInfoArgs() {
			return new ListBoxItemObjectInfoArgs(this, null, Rectangle.Empty);
		}
		public StringFormat ItemStrFormat { get { return strFormat; } }
		protected internal virtual int CalcItemMinHeight() {
			int height = 0;
			GraphicsInfo ginfo = new GraphicsInfo();
			Graphics g = ginfo.AddGraphics(null);
			try {
				height = PaintAppearance.CalcDefaultTextSize(g).Height + ListBoxItemPainter.GetVertPadding(ListBoxItemInfoArgs);
			}
			finally {
				ginfo.ReleaseGraphics();
			}
			return height;
		}
		public virtual int CalcBestColumnWidth() {
			int width = 0;
			try {
				UpdatePaintAppearance();
				Graphics g = GInfo.AddGraphics(null);
				for(int i = 0; i < ItemCount; i++) {
					string itemText = OwnerControl.GetDrawItemText(i);
					int currentWidth = 0;
					if(GetHtmlDrawText()) {
						StringInfo si = StringPainter.Default.Calculate(g, PaintAppearance, DefaultTextOptions, itemText, 0, null, GetHtmlContext());
						currentWidth = si.Bounds.Width;
					}
					else {
						currentWidth = PaintAppearance.CalcTextSize(g, itemText, 0).ToSize().Width;
					}
					MeasureItemEventArgs args = new MeasureItemEventArgs(g, i, 0);
					args.ItemWidth = currentWidth;
					OwnerControl.RaiseMeasureItem(args);
					currentWidth = args.ItemWidth;
					width = Math.Max(width, currentWidth);
				}
				width += ListBoxItemPainter.GetHorzPadding(ListBoxItemInfoArgs) + CalcItemActionPadding(g);
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			return (width == 0 ? ContentRect.Width : width + 5);
		}
		protected internal virtual int DefaultItemActionPadding { get { return 5; } }
		protected internal virtual int CalcItemActionPadding(Graphics g) {
			if(!HasItemActions) return 0;
			return DefaultItemActionPadding;
		}
		protected override void CalcContentRect(Rectangle bounds) {
			base.CalcContentRect(bounds);
			AdjustContentRect();
			CalcColumnsInfo();
			CalcItemsInfo();
			CalcFocusRect();
		}
		void CalcFocusRect() {
			if(Focused && ((OwnerControl.ItemCount == 0) || ((OwnerControl.SelectedIndex == -1) && (OwnerControl.TopIndex == 0)) && FocusedItemIndex > -1)) {
				Rectangle rect = new Rectangle(ContentRect.Location, CalcItemSize(0));
				fFocusRect = rect;
			}
		}
		public ListBoxScrollInfo ScrollInfo { get { return this.OwnerControl.ScrollInfo; } }
		protected virtual void AdjustContentRect() {
			isNeededHScroll = isNeededVScroll = false;
			if(OwnerControl.MultiColumn) {
				isNeededHScroll = CalcHScrollVisibility(fContentRect);
				if(isNeededHScroll) {
					fContentRect.Height -= (OwnerControl.ScrollInfo.IsOverlapScrollbar ? ScrollInfo.HScrollHeight / 2 : ScrollInfo.HScrollHeight);
				}
			}
			else {
				isNeededHScroll = CalcSingleColumnHScrollVisibility();
				isNeededVScroll = CalcVScrollVisibility(fContentRect);
				if(!OwnerControl.ScrollInfo.IsOverlapScrollbar) {
					if(isNeededVScroll) {
						if(OwnerControl.IsRightToLeft) {
							fContentRect.X += ScrollInfo.VScrollWidth;
							fContentRect.Width -= ScrollInfo.VScrollWidth;
						}
						else {
							fContentRect.Width -= ScrollInfo.VScrollWidth;
						}
					}
				}
				if(!isNeededHScroll)
					isNeededHScroll = CalcSingleColumnHScrollVisibility();
			}
		}
		bool CalcSingleColumnHScrollVisibility() {
			bool isNeeded = CheckHorizontalScroll();
			if(isNeeded)
				fContentRect.Height -= ScrollInfo.HScrollHeight;
			return isNeeded;
		}
		bool CheckHorizontalScroll() {
			return (OwnerControl.HorizontalScrollbar && BestColumnWidth > fContentRect.Width);
		}
		protected virtual bool CalcVScrollVisibility(Rectangle bounds) {
			if(OwnerControl.TopIndex > ItemCount) {
				ResetItemSizesCache();
				OwnerControl.Model.CheckTopIndex();
			}
			if(OwnerControl.TopIndex > 0) return true;
			if(bounds.Width <= ScrollInfo.VScrollWidth) return false;
			int height = 0;
			for(int i = 0; i < ItemCount; i++) {
				height += CalcItemSize(i, false).Height;
				if(height > bounds.Height) return true;
			}
			return false;
		}
		protected virtual bool CalcHScrollVisibility(Rectangle bounds) {
			if(bounds.Height < ItemHeight) return false;
			int colCount = CalcColumnsCount(bounds);
			if(colCount < 2) return false;
			return (colCount * ColumnWidth) > bounds.Width;
		}
		protected virtual int CalcColumnsCount(Rectangle contentRect) {
			if(!OwnerControl.MultiColumn) return 1;
			int rowCount = contentRect.Height / ItemHeight;
			if(rowCount < 1) rowCount = 1;
			int colCount = ItemCount / rowCount;
			if(colCount * rowCount < ItemCount) colCount++;
			return colCount;
		}
		protected virtual int CalcItemsPerColumnCount() {
			if(OwnerControl.MultiColumn) return Math.Max(1, ContentRect.Height / ItemHeight);
			return ItemCount;
		}
		protected void CalcColumnsInfo() {
			columnCount = CalcColumnsCount(ContentRect);
			itemsPerColumn = CalcItemsPerColumnCount();
		}
		public int GetItemColumnIndex(int itemIndex) {
			int columnIndex = itemIndex / ItemsPerColumn;
			if(((columnIndex + 1) * ItemsPerColumn < itemIndex)) columnIndex++;
			return columnIndex;
		}
		protected virtual void CalcItemsInfo() {
			itemsInfo.Clear();
			Rectangle r = Rectangle.Empty;
			if(OwnerControl.IsDesignMode && ItemCount == 0) {
				r = new Rectangle(ContentRect.Location, new Size(ContentRect.Width, ItemHeight));
				itemsInfo.Add(0, CalcItemInfo(r, -1));
				return;
			}
			for(int i = OwnerControl.TopIndex; i < ItemCount; i++) {
				r = CalcItemBounds(i, r.Bottom);
				if(r.IsEmpty) break;
				itemsInfo.Add(i, CalcItemInfo(r, i));
			}
		}
		protected virtual ItemInfo CreateItemInfo(Rectangle bounds, object item, string text, int index) {
			return new ItemInfo(OwnerControl, bounds, item, text, index);
		}
		protected ItemInfo CalcItemInfo(Rectangle bounds, int index) {
			object itemValue = (index == -1) ? ControlName : OwnerControl.GetItemValue(index);
			string itemText = (index == -1) ? ControlName : OwnerControl.GetDrawItemText(index);
			ItemInfo itemInfo = CreateItemInfo(bounds, itemValue, itemText, index);
			UpdateItem(itemInfo);
			if(index == -1 || index == 0) BaseTextBounds = itemInfo.Bounds;
			itemInfo.CalcContextButtons();
			return itemInfo;
		}
		protected virtual Rectangle CalcItemBounds(int itemIndex, int yCoord) {
			if(itemsPerColumn == 0) return Rectangle.Empty;
			int columnIndex = (itemIndex - OwnerControl.TopIndex) / ItemsPerColumn;
			int rowIndex = (itemIndex - OwnerControl.TopIndex) - (columnIndex * ItemsPerColumn);
			if(rowIndex == 0) yCoord = ContentRect.Top;
			Size size = CalcItemSize(itemIndex);
			Rectangle itemBounds = Rectangle.Empty;
			if(OwnerControl.IsRightToLeft)
				itemBounds = new Rectangle(ContentRect.Left + ContentRect.Width - size.Width * (columnIndex + 1) + OwnerControl.LeftCoord, yCoord, size.Width, size.Height);
			else
				itemBounds = new Rectangle(ContentRect.Left + (columnIndex * size.Width) - OwnerControl.LeftCoord, yCoord, size.Width, size.Height);
			if(itemBounds.Left > ContentRect.Right || itemBounds.Top > ContentRect.Bottom) return Rectangle.Empty;
			if(OwnerControl.IsRightToLeft && itemBounds.Left < ContentRect.Left && !CheckHorizontalScroll()) return Rectangle.Empty;
			return itemBounds;
		}
		Size GetCachedItemSize(int itemIndex) {
			if(!itemSizesCache.ContainsKey(itemIndex)) return Size.Empty;
			return (Size)itemSizesCache[itemIndex];
		}
		protected virtual Size CalcItemSize(int itemIndex) {
			return CalcItemSize(itemIndex, true);
		}
		protected virtual int CalcItemHeight(Graphics g, int itemIndex) {
			string text = OwnerControl.GetDrawItemText(itemIndex);
			if(string.IsNullOrEmpty(text))
				return 0;
			if(GetHtmlDrawText()) {
				StringInfo si = StringPainter.Default.Calculate(g, PaintAppearance, DefaultTextOptions, text, ContentRect.Width, null, GetHtmlContext());
				return si.Bounds.Height;
			}
			return PaintAppearance.CalcTextSize(g, text, ContentRect.Width).ToSize().Height;
		}
		protected virtual Size CalcItemSize(int itemIndex, bool cacheItemSize) {
			if(OwnerControl.MultiColumn) return new Size(ColumnWidth, ItemHeight);
			if(itemSizesCache.ContainsKey(itemIndex)) return GetCachedItemSize(itemIndex);
			Size size = Size.Empty;
			Graphics g = GInfo.AddGraphics(null);
			try {
				int itemHeight = ItemHeight;
				if(OwnerControl.ItemAutoHeight)
					itemHeight = Math.Max(itemHeight, CalcItemHeight(g, itemIndex));
				MeasureItemEventArgs args = new MeasureItemEventArgs(g, itemIndex, itemHeight);
				OwnerControl.RaiseMeasureItem(args);
				int width = 0;
				if(CheckHorizontalScroll())
					width = BestColumnWidth;
				width = Math.Max(width, ContentRect.Width);
				size = new Size(width, args.ItemHeight);
				if(cacheItemSize) itemSizesCache[itemIndex] = size;
			}
			finally {
				GInfo.ReleaseGraphics();
			}
			return size;
		}
		internal int CalcVisibleItemCountFromBottom(int itemIndex) {
			if(OwnerControl.MultiColumn) return -1;
			GInfo.AddGraphics(null);
			try {
				int count = 0, height = 0;
				for(int i = itemIndex; i > -1; i--) {
					height += CalcItemSize(i, false).Height;
					if(height > ContentRect.Height) return Math.Max(1, count);
					count++;
				}
				return Math.Max(1, count);
			}
			finally {
				GInfo.ReleaseGraphics();
			}
		}
		protected virtual void UpdateItemAppearance(ItemInfo itemInfo) {
			itemInfo.PaintAppearance.Assign(PaintAppearance);
			itemInfo.PaintAppearance.TextOptions.RightToLeft = RightToLeft;
			itemInfo.PaintAppearance.ForeColor = GetItemForeColor(itemInfo);
			itemInfo.PaintAppearance.BackColor = GetItemBackColor(itemInfo);
		}
		protected virtual void UpdateItemHighlight(ItemInfo itemInfo) {
			itemInfo.HighlightCharsCount = 0;
			if(!OwnerControl.Enabled || string.IsNullOrEmpty(OwnerControl.CurrentSearch))
				return;
			if(Focused && OwnerControl.IncrementalSearch && FocusedItemIndex == itemInfo.Index)
				itemInfo.HighlightCharsCount = OwnerControl.CurrentSearch.Length;
		}
		public virtual void UpdateItem(ItemInfo item) {
			if(item == null) return;
			UpdateItemState(item);
			UpdateItemAppearance(item);
			UpdateItemHighlight(item);
		}
		public void CalcItemActionInfo(Graphics g, ItemInfo itemInfo) {
			if(!HasItemActions) {
				itemInfo.ActionInfo = null;
				return;
			}
			CalcItemActionInfoCore(g, itemInfo);
		}
		public ListItemActionInfo GetActiveActionInfo(Point point) {
			ItemInfo item = GetItemInfoByPoint(point);
			if(item == null || item.ActionInfo == null) return null;
			foreach(ListItemActionInfo aInfo in item.ActionInfo) {
				if(aInfo.Bounds.Contains(point)) return aInfo;
			}
			return null;
		}
		public bool UpdateItemActionState(MouseEventArgs e, ItemInfo itemInfo) {
			if(itemInfo == null || itemInfo.ActionInfo == null) return false;
			bool changed = false;
			foreach(ListItemActionInfo aInfo in itemInfo.ActionInfo) {
				ObjectState state = ObjectState.Normal;
				if(aInfo.Bounds.Contains(new Point(e.X, e.Y)))
					state = (e.Button & MouseButtons.Left) == MouseButtons.Left ? ObjectState.Pressed : ObjectState.Hot;
				else {
					state = ObjectState.Normal;
				}
				if(state != aInfo.State) {
					changed = true;
					aInfo.State = state;
				}
			}
			return changed;
		}
		protected virtual bool ContainsItemAction(ItemInfo itemInfo) {
			if(!HasItemActions || !OwnerControl.IsHandleCreated) return false;
			if(itemInfo.Index != HotItemIndex) return false;
			Point mp = Control.MousePosition;
			mp = OwnerControl.PointToClient(mp);
			if(!itemInfo.Bounds.Contains(mp)) return false;
			return true;
		}
		protected virtual void CalcItemActionInfoCore(Graphics g, ItemInfo itemInfo) {
			if(!ContainsItemAction(itemInfo)) {
				itemInfo.ActionInfo = null;
				return;
			}
			Rectangle bounds = itemInfo.Bounds;
			int padding = CalcItemActionPadding(g);
			Rectangle action = bounds;
			action.X = bounds.Right - padding;
			action.Width = 0;
			if(action.X < bounds.X) {
				itemInfo.ActionInfo = null;
				return;
			}
			if(itemInfo.ActionInfo == null) {
				OwnerControl.CreateItemActions(itemInfo);
			}
			if(itemInfo.ActionInfo == null) return;
			for(int n = itemInfo.ActionInfo.Count - 1; n >= 0; n--) {
				ListItemActionInfo aInfo = itemInfo.ActionInfo[n];
				Rectangle itemBounds = ObjectPainter.CalcObjectMinBounds(g, aInfo.Painter, aInfo);
				if(itemBounds.Height > bounds.Height) {
					itemInfo.ActionInfo.RemoveAt(n);
					continue;
				}
				itemBounds.Y = action.Y + (bounds.Height - itemBounds.Height) / 2;
				itemBounds.X = action.Right - itemBounds.Width;
				if(itemBounds.X <= bounds.X) {
					itemInfo.ActionInfo.RemoveAt(n);
					continue;
				}
				aInfo.Bounds = itemBounds;
				action.X -= itemBounds.Width;
			}
		}
		public virtual void UpdateItemState(ItemInfo itemInfo) {
			if(!OwnerControl.Enabled) {
				if(OwnerControl.UseDisabledStatePainter) {
					itemInfo.State = DrawItemState.Grayed;
					return;
				}
			}
			if(OwnerControl.IsDesignMode) return;
			if(OwnerControl.CanHotTrack) {
				DrawItemState hotState = OwnerControl.IsAdvHotTrackEnabled() ? DrawItemState.HotLight : DrawItemState.Selected;
				if(HotItemIndex == itemInfo.Index) {
					itemInfo.State = hotState;
				}
				else {
					itemInfo.State &= ~hotState;
				}
				if(hotState == DrawItemState.HotLight) {
					if(!OwnerControl.SelectedIndices.Contains(itemInfo.Index)) return;
				}
				else {
					if(Focused && HotItemIndex == itemInfo.Index)
						itemInfo.State |= DrawItemState.Focus;
					else
						itemInfo.State &= ~DrawItemState.Focus;
					return;
				}
			}
			if(Focused && FocusedItemIndex == itemInfo.Index)
				itemInfo.State |= DrawItemState.Focus;
			else
				itemInfo.State &= ~DrawItemState.Focus;
			if(OwnerControl.SelectedIndices.Contains(itemInfo.Index)) {
				itemInfo.State |= DrawItemState.Selected;
			}
			else {
				itemInfo.State &= ~DrawItemState.Selected;
			}
			if(!itemInfo.Enabled) itemInfo.State |= DrawItemState.Grayed;
		}
		public ItemInfo GetItemInfoByPoint(Point pt) {
			foreach(ItemInfo itemInfo in VisibleItems) {
				if(itemInfo.Bounds.Contains(pt)) return itemInfo;
			}
			return null;
		}
		public Rectangle GetItemRectangle(int index) {
			if(itemsInfo.ContainsKey(index)) return ((ItemInfo)itemsInfo[index]).Bounds;
			return Rectangle.Empty;
		}
		public ItemInfo GetItemByIndex(int index) {
			if(itemsInfo.ContainsKey(index)) return itemsInfo[index] as ItemInfo;
			return null;
		}
		public override void Reset() {
			this.focusedItemIndex = this.hotItemIndex = -1;
			this.itemsPerColumn = -1;
			this.columnCount = 1;
			this.itemSizesCache = new Hashtable();
			this.ResetColumnWidth();
			base.Reset();
		}
		public void ResetColumnWidth() {
			if(OwnerControl.ColumnWidth == 0) this.columnWidth = 0;
			this.bestColumnWidth = -1;
		}
		internal void ResetItemSizesCache() {
			itemSizesCache.Clear();
		}
		protected override void UpdateFromOwner() {
			base.UpdateFromOwner();
			strFormat = new StringFormat(PaintAppearance.GetStringFormat());
			if(OwnerControl.IsRightToLeft) {
				strFormat.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
			}
			if(OwnerControl.IsDesignMode) strFormat.LineAlignment = StringAlignment.Near;
		}
		protected override bool UpdateObjectState() {
			ObjectState prevState = State;
			ObjectState res = ObjectState.Normal;
			if(!OwnerControl.IsDesignMode) {
				if(Focused) res = ObjectState.Selected;
				if(OwnerControl.Enabled)
					State |= Bounds.Contains(MousePosition) ? ObjectState.Hot : ObjectState.Normal;
				else
					res = ObjectState.Disabled;
			}
			return res != prevState;
		}
		protected internal void CustomDrawItem(ListBoxDrawItemEventArgs e) {
			OwnerControl.RaiseDrawItem(e);
		}
		protected virtual Color GetItemForeColor(ItemInfo itemInfo) {
			Color color = GetItemForeColorCore(itemInfo);
			if(!color.IsEmpty) return color;
			return PaintAppearance.ForeColor;
		}
		Color GetItemForeColorCore(ItemInfo itemInfo) {
			if(!OwnerControl.Enabled && OwnerControl.UseDisabledStatePainter || !itemInfo.Enabled)
				return ListBoxItemPainter.GetItemForeColor(ListBoxItemInfoArgs, ObjectState.Disabled, GetDisabledTextColor());
			if((itemInfo.State & DrawItemState.HotLight) != 0)
				return ListBoxItemPainter.GetItemForeColor(ListBoxItemInfoArgs, ObjectState.Hot, GetHighlightTextColor());
			if((itemInfo.State & DrawItemState.Selected) != 0)
				return ListBoxItemPainter.GetItemForeColor(ListBoxItemInfoArgs, ObjectState.Selected, GetHighlightTextColor());
			return PaintAppearance.ForeColor;
		}
		protected virtual Color GetItemBackColor(ItemInfo itemInfo) {
			if((itemInfo.State & DrawItemState.Selected) != 0) return ListBoxItemPainter.GetItemBackColor(ListBoxItemInfoArgs, ObjectState.Selected, GetHighlightColor());
			return PaintAppearance.BackColor;
		}
		protected virtual Color GetHighlightTextColor() { return GetSystemColor(SystemColors.HighlightText); }
		protected virtual Color GetHighlightColor() { return GetSystemColor(SystemColors.Highlight); }
		protected virtual Color GetDisabledTextColor() { return GetSystemColor(SystemColors.GrayText); }
		protected string ControlName { get { return ((OwnerControl.Site == null) ? string.Empty : OwnerControl.Site.Name); } }
		public override Size CalcBestFit(Graphics g) {
			if(!OwnerControl.IsHandleCreated || ItemCount == 0)
				return base.CalcBestFit(g);
			int height = 0;
			if(!OwnerControl.MeasureItemAssigned) {
				height = ItemHeight * ItemCount;
			}
			else {
				for(int i = 0; i < ItemCount; i++)
					height += CalcItemSize(i, false).Height;
			}
			return BorderPainter.CalcBoundsByClientRectangle(new BorderObjectInfoArgs(null, new Rectangle(0, 0, BestColumnWidth, height), null)).Size;
		}
		#region ItemInfo
		public class ItemInfo : ISupportContextItems {
			const int contextGlyphToCaptionIndent = 3;
			Rectangle bounds, textRect;
			ListItemActionCollection actionInfo;
			object item;
			string text;
			AppearanceObject paintAppearance;
			DrawItemState state;
			int index, highlightCharsCount;
			ContextItemCollectionViewInfo contextButtonsViewInfo;
			BaseListBoxControl listBoxControl;
			public ItemInfo(BaseListBoxControl ownerControl, Rectangle bounds, object item, string text, int index) {
				this.paintAppearance = new AppearanceObject();
				this.text = text;
				this.index = index;
				this.item = item;
				this.bounds = this.textRect = bounds;
				this.state = DrawItemState.None;
				this.listBoxControl = ownerControl;
			}
			public BaseListBoxControl ListBoxControl { get { return listBoxControl; } set { listBoxControl = value; } }
			public ListItemActionCollection ActionInfo { get { return actionInfo; } set { actionInfo = value; } }
			public object Item { get { return item; } }
			public int Index { get { return index; } }
			public AppearanceObject PaintAppearance { get { return paintAppearance; } }
			public virtual Rectangle Bounds {
				get { return bounds; }
				set {
					bounds = value;
					CalcContextButtons();
				}
			}
			public virtual Rectangle TextRect {
				get { return textRect; }
				set { textRect = value; }
			}
			public DrawItemState State {
				get { return state; }
				set { state = value; }
			}
			public string Text {
				get { return text; }
				set { text = value; }
			}
			[Obsolete("Use HighlightCharsCount"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
			public int HightlightCharsCount {
				get { return HighlightCharsCount; }
				set { HighlightCharsCount = value; }
			}
			public int HighlightCharsCount {
				get { return highlightCharsCount; }
				set { highlightCharsCount = value; }
			}
			public virtual bool Enabled { get { return true; } }
			public virtual void CalcContextButtons() {
				ContextButtonsViewInfo.CalcItems();
			}
			protected internal ContextItemCollectionViewInfo ContextButtonsViewInfo {
				get {
					if(contextButtonsViewInfo == null)
						contextButtonsViewInfo = CreateContextButtonsViewInfo();
					return contextButtonsViewInfo;
				}
			}
			ContextItemCollectionViewInfo CreateContextButtonsViewInfo() {
				return new ContextItemCollectionViewInfo(((ISupportContextItems)this).ContextItems, ((ISupportContextItems)this).Options, this);
			}
			Rectangle ISupportContextItems.ActivationBounds {
				get {
					return Bounds;
				}
			}
			bool ISupportContextItems.CloneItems {
				get { return true; }
			}
			ContextItemCollection ISupportContextItems.ContextItems {
				get { return ListBoxControl.ContextButtons; }
			}
			Control ISupportContextItems.Control { get { return ListBoxControl; } }
			bool ISupportContextItems.DesignMode { get { return ListBoxControl.IsDesignMode; } }
			Rectangle ISupportContextItems.DisplayBounds { get { return Bounds; } }
			Rectangle ISupportContextItems.DrawBounds { get { return Bounds; } }
			ItemHorizontalAlignment ISupportContextItems.GetCaptionHorizontalAlignment(ContextButton btn) {
				return ItemHorizontalAlignment.Default;
			}
			ItemVerticalAlignment ISupportContextItems.GetCaptionVerticalAlignment(ContextButton btn) {
				return ItemVerticalAlignment.Default;
			}
			ItemHorizontalAlignment ISupportContextItems.GetGlyphHorizontalAlignment(ContextButton btn) {
				return ItemHorizontalAlignment.Default;
			}
			ItemLocation ISupportContextItems.GetGlyphLocation(ContextButton btn) {
				return ItemLocation.Default;
			}
			int ISupportContextItems.GetGlyphToCaptionIndent(ContextButton btn) {
				return contextGlyphToCaptionIndent;
			}
			ItemVerticalAlignment ISupportContextItems.GetGlyphVerticalAlignment(ContextButton btn) {
				return ItemVerticalAlignment.Default;
			}
			UserLookAndFeel ISupportContextItems.LookAndFeel {
				get { return ListBoxControl.LookAndFeel.ActiveLookAndFeel; }
			}
			ContextItemCollectionOptions ISupportContextItems.Options {
				get { return ListBoxControl.ContextButtonOptions; }
			}
			void ISupportContextItems.RaiseContextItemClick(ContextItemClickEventArgs e) {
				e.DataItem = Item;
				ListBoxControl.RaiseContextButtonClick(e);
			}
			void ISupportContextItems.RaiseCustomContextButtonToolTip(ContextButtonToolTipEventArgs e) {
				ListBoxControl.RaiseCustomContextButtonToolTip(e);
			}
			void ISupportContextItems.RaiseCustomizeContextItem(ContextItem item) {
				ListBoxControl.RaiseCustomizeContextItem(new ListBoxControlContextButtonCustomizeEventArgs(Item, item));
			}
			void ISupportContextItems.Redraw(Rectangle rect) {
				ListBoxControl.Invalidate(rect);
			}
			void ISupportContextItems.Redraw() {
				ListBoxControl.Invalidate();
			}
			bool ISupportContextItems.ShowOutsideDisplayBounds { get { return false; } }
			void ISupportContextItems.Update() {
				ListBoxControl.Update();
			}
			ContextItemCollectionHandler contextButtonsHandler;
			protected ContextItemCollectionHandler ContextButtonsHandler {
				get {
					if(contextButtonsHandler == null)
						contextButtonsHandler = CreateContextButtonsHandler();
					return contextButtonsHandler;
				}
			}
			protected virtual ContextItemCollectionHandler CreateContextButtonsHandler() {
				return new ContextItemCollectionHandler();
			}
			protected internal virtual bool OnMouseMove(MouseEventArgs e) {
				ContextButtonsHandler.ViewInfo = ContextButtonsViewInfo;
				return ContextButtonsHandler.OnMouseMove(e);
			}
			protected internal virtual bool OnMouseEnter(EventArgs e) {
				ContextButtonsHandler.ViewInfo = ContextButtonsViewInfo;
				return ContextButtonsHandler.OnMouseEnter(e);
			}
			protected internal virtual bool OnMouseLeave(EventArgs e) {
				ContextButtonsHandler.ViewInfo = ContextButtonsViewInfo;
				return ContextButtonsHandler.OnMouseLeave(e);
			}
			protected internal virtual bool OnMouseDown(MouseEventArgs e) {
				ContextButtonsHandler.ViewInfo = ContextButtonsViewInfo;
				return ContextButtonsHandler.OnMouseDown(e);
			}
			protected internal virtual bool OnMouseUp(MouseEventArgs e) {
				ContextButtonsHandler.ViewInfo = ContextButtonsViewInfo;
				return ContextButtonsHandler.OnMouseUp(e);
			}
		}
		#endregion
		protected internal virtual bool GetHtmlDrawText() {
			if(OwnerControl != null) return OwnerControl.GetAllowHtmlDraw();
			return false;
		}
		internal object GetHtmlContext() { return this; }
		#region IStringImageProvider Members
		protected virtual ImageCollection GetHtmlImages() {
			return OwnerControl == null ? null : OwnerControl.HtmlImages;
		}
		Image IStringImageProvider.GetImage(string id) {
			ImageCollection images = GetHtmlImages();
			return images == null ? null : images.Images[id];
		}
		#endregion
		#region IServiceProvider Members
		object IServiceProvider.GetService(Type serviceType) {
			if(serviceType == typeof(IStringImageProvider)) return this;
			if(serviceType == typeof(ISite)) return OwnerControl == null ? null : OwnerControl.Site;
			return null;
		}
		#endregion
		protected virtual internal bool IsItemTextFit(ItemInfo item) {
			GraphicsInfo gInfo = new GraphicsInfo();
			Graphics g = gInfo.AddGraphics(null);
			try {
				using(GraphicsCache cache = new GraphicsCache(g)) {
					ListBoxItemObjectInfoArgs args = new ListBoxItemObjectInfoArgs(this, null, Rectangle.Empty);
					args.AssignFromItemInfo(item);
					Rectangle textRect = ListBoxItemPainter.GetItemTextRectangle(args);
					return CalcItemTextWidth(cache, args, 0) <= CalcItemTextWidth(cache, args, textRect.Width);
				}
			}
			finally {
				gInfo.ReleaseGraphics();
			}
		}
		int CalcItemTextWidth(GraphicsCache cache, ListBoxItemObjectInfoArgs args, int width) {
			if(args.IsHtmlDrawText)
				return StringPainter.Default.Calculate(cache.Graphics, args.PaintAppearance, DefaultTextOptions, args.ItemText, width, null, args.GetHtmlContext()).Bounds.Width;
			return args.PaintAppearance.CalcTextSize(cache, args.ItemText, width).ToSize().Width;
		}
		internal void OnResize() {
			ScrollInfo.OnAction(ScrollNotifyAction.Resize);
		}
	}
	public delegate void ListBoxControlContextButtonCustomizeEventHandler(object sender, ListBoxControlContextButtonCustomizeEventArgs e);
	public class ListBoxControlContextButtonCustomizeEventArgs : EventArgs {
		public ListBoxControlContextButtonCustomizeEventArgs(object item, ContextItem contextItem) {
			this.item = item;
			ContextItem = contextItem;
		}
		object item;
		public ContextItem ContextItem { get; private set; }
		public object Item { get { return item; } }
	}
}
namespace DevExpress.XtraEditors.Drawing {
	public class BaseListBoxPainter : BaseControlPainter {
		protected override void DrawContent(ControlGraphicsInfoArgs info) {
			base.DrawContent(info);
			info.Graphics.FillRectangle(info.ViewInfo.PaintAppearance.GetBackBrush(info.Cache), info.ViewInfo.ContentRect);
			DrawVisibleItems(info);
		}
		protected virtual void DrawVisibleItems(ControlGraphicsInfoArgs info) {
			BaseListBoxViewInfo vi = info.ViewInfo as BaseListBoxViewInfo;
			GraphicsClipState state = info.Cache.ClipInfo.SaveClip();
			info.Cache.ClipInfo.SetClip(info.ViewInfo.ContentRect);
			ArrayList items = new ArrayList(vi.VisibleItems);
			foreach(DevExpress.XtraEditors.ViewInfo.BaseListBoxViewInfo.ItemInfo item in items) {
				DrawItem(info, item);
			}
			info.Cache.ClipInfo.RestoreClipRelease(state);
		}
		protected void DrawItem(ControlGraphicsInfoArgs info, BaseListBoxViewInfo.ItemInfo itemInfo) {
			ListBoxDrawItemEventArgs e = CreateDrawItemEventArgs(info, itemInfo);
			e.ViewInfo.CalcItemActionInfo(info.Graphics, itemInfo);
			e.SetDefaultDraw(() => {
				DrawItemCore(info, itemInfo, e);
				DrawItemAction(info, itemInfo, e);
			});
			((BaseListBoxViewInfo)info.ViewInfo).CustomDrawItem(e);
			e.DefaultDraw();
		}
		protected virtual void DrawItemAction(ControlGraphicsInfoArgs info, BaseListBoxViewInfo.ItemInfo itemInfo, ListBoxDrawItemEventArgs e) {
			if(itemInfo.ActionInfo == null || itemInfo.ActionInfo.Count == 0) return;
			for(int n = 0; n < itemInfo.ActionInfo.Count; n++) {
				ListItemActionInfo aInfo = itemInfo.ActionInfo[n];
				ObjectPainter.DrawObject(info.Cache, aInfo.Painter, aInfo);
			}
		}
		protected virtual void DrawItemCore(ControlGraphicsInfoArgs info, BaseListBoxViewInfo.ItemInfo itemInfo, ListBoxDrawItemEventArgs e) {
			BaseListBoxViewInfo viewInfo = info.ViewInfo as BaseListBoxViewInfo;
			viewInfo.ListBoxItemInfoArgs.Cache = info.Cache;
			viewInfo.ListBoxItemInfoArgs.AssignFromItemInfo(itemInfo);
			viewInfo.ListBoxItemInfoArgs.AllowDrawSkinBackground = e.AllowDrawSkinBackground;
			try {
				viewInfo.ListBoxItemPainter.DrawObject(viewInfo.ListBoxItemInfoArgs);
			}
			finally {
				viewInfo.ListBoxItemInfoArgs.AllowDrawSkinBackground = true;
				viewInfo.ListBoxItemInfoArgs.Cache = null;
				viewInfo.ListBoxItemInfoArgs.PaintAppearance = null;
			}
		}
		protected ListBoxDrawItemEventArgs CreateDrawItemEventArgs(ControlGraphicsInfoArgs info, BaseListBoxViewInfo.ItemInfo itemInfo) {
			ListBoxDrawItemEventArgs res = new ListBoxDrawItemEventArgs(info.Cache, itemInfo);
			res.ViewInfo = info.ViewInfo as BaseListBoxViewInfo;
			return res;
		}
	}
	public class ListBoxItemObjectInfoArgs : ObjectInfoArgs {
		string itemText;
		Rectangle textRect, actionBounds;
		AppearanceObject paintAppearance;
		BaseListBoxViewInfo viewInfo;
		DrawItemState itemState;
		int highlightCharsCount;
		bool allowDrawSkinBackground;
		public ListBoxItemObjectInfoArgs(BaseListBoxViewInfo viewInfo, GraphicsCache cache, Rectangle bounds)
			: base(cache, bounds, ObjectState.Normal) {
			this.itemText = string.Empty;
			this.actionBounds = this.textRect = Rectangle.Empty;
			this.viewInfo = viewInfo;
			this.paintAppearance = null;
			this.allowDrawSkinBackground = true;
		}
		internal bool IsHtmlDrawText {
			get {
				return ViewInfo.GetHtmlDrawText();
			}
		}
		internal object GetHtmlContext() { return ViewInfo.GetHtmlContext(); }
		public bool DrawFocusRect { get { return ViewInfo.DrawFocusRect; } }
		public string ItemText {
			get { return itemText; }
			set { itemText = value; }
		}
		public int HighlightCharsCount {
			get { return highlightCharsCount; }
			set { highlightCharsCount = value; }
		}
		public Rectangle TextRect {
			get { return textRect; }
			set { textRect = value; }
		}
		public Rectangle ActionBounds {
			get { return actionBounds; }
			set { actionBounds = value; }
		}
		public AppearanceObject PaintAppearance {
			get { return paintAppearance; }
			set { paintAppearance = value; }
		}
		public BaseListBoxViewInfo ViewInfo {
			get { return viewInfo; }
		}
		public DrawItemState ItemState {
			get { return itemState; }
			set { itemState = value; }
		}
		public bool AllowDrawSkinBackground {
			get { return allowDrawSkinBackground; }
			set { allowDrawSkinBackground = value; }
		}
		public virtual void AssignFromItemInfo(BaseListBoxViewInfo.ItemInfo item) {
			this.itemText = item.Text;
			this.textRect = item.TextRect;
			this.itemState = item.State;
			this.paintAppearance = item.PaintAppearance;
			this.Bounds = item.Bounds;
			this.HighlightCharsCount = item.HighlightCharsCount;
			this.actionBounds = item.ActionInfo == null ? Rectangle.Empty : item.ActionInfo.Bounds;
			this.allowDrawSkinBackground = true;
		}
	}
	public abstract class BaseListBoxItemPainter : ObjectPainter {
		public static Color GetContrastColor(Color color) { return Color.FromArgb(0xff - color.R, 0xff - color.G, 0xff - color.B); }
		public BaseListBoxItemPainter()
			: base() {
		}
		public override void DrawObject(ObjectInfoArgs e) {
			ListBoxItemObjectInfoArgs args = e as ListBoxItemObjectInfoArgs;
			DrawItemBar(args);
			DrawItemText(args);
			DrawItemContextButton(args);
		}
		protected virtual void DrawItemContextButton(ListBoxItemObjectInfoArgs args) {
			BaseListBoxViewInfo.ItemInfo itemInfo = args.ViewInfo.GetItemInfoByPoint(args.Bounds.Location);
			new ContextItemCollectionPainter().Draw(new ContextItemCollectionInfoArgs(itemInfo.ContextButtonsViewInfo, args.Cache, args.Bounds));
		}
		protected virtual void DrawItemText(ListBoxItemObjectInfoArgs e) {
			Rectangle textRect = CalcItemTextRectangle(e);
			textRect = UpdateTextRect(e, textRect);
			StringFormat format = e.PaintAppearance.GetStringFormat(e.ViewInfo.DefaultTextOptions);
			if(e.IsHtmlDrawText) {
				StringPainter.Default.DrawString(e.Cache, e.PaintAppearance, e.ItemText, textRect, e.ViewInfo.DefaultTextOptions, e.GetHtmlContext());
				return;
			}
				e.PaintAppearance.DrawString(e.Cache, e.ItemText, textRect, e.PaintAppearance.Font, format);
				if(e.HighlightCharsCount > 0) {
					DrawItemHighlight(e, textRect, format);
				}
		}
		protected virtual Rectangle UpdateTextRect(ListBoxItemObjectInfoArgs e, Rectangle textRect) {
			if(e.ActionBounds.IsEmpty) return textRect;
			if(!e.ActionBounds.IntersectsWith(textRect)) return textRect;
			if(textRect.Right > e.ActionBounds.X) {
				textRect.Width = Math.Max(0, e.ActionBounds.X - textRect.X - 3);
			}
			return textRect;
		}
		protected abstract void DrawItemHighlight(ListBoxItemObjectInfoArgs e, Rectangle textRect, StringFormat format);		   
		protected string GetHighlightText(ListBoxItemObjectInfoArgs e) {
			int chrCount = Math.Min(e.HighlightCharsCount, e.ItemText.Length);
			return e.ItemText.Substring(0, chrCount);
		}
		protected Rectangle CalcHighlightBounds(ListBoxItemObjectInfoArgs e, Rectangle textRect, StringFormat format, string highlightText) {
			SizeF hlSize = e.PaintAppearance.CalcTextSize(e.Cache, format, highlightText, textRect.Width);
			return new Rectangle(textRect.Left - 1, e.Bounds.Y + 1, (int)hlSize.Width + 1, e.Bounds.Height - 2);
		}
		public virtual int GetHorzPadding(ListBoxItemObjectInfoArgs e) { return 0; }
		public virtual int GetVertPadding(ListBoxItemObjectInfoArgs e) { return 0; }
		public virtual Color GetItemForeColor(ListBoxItemObjectInfoArgs e, ObjectState state, Color defaultColor) { return defaultColor; }
		public virtual Color GetItemBackColor(ListBoxItemObjectInfoArgs e, ObjectState state, Color defaultColor) { return defaultColor; }  
		protected abstract void DrawItemBar(ListBoxItemObjectInfoArgs e);
		protected abstract Rectangle CalcItemTextRectangle(ListBoxItemObjectInfoArgs e);
		internal Rectangle GetItemTextRectangle(ListBoxItemObjectInfoArgs e) {
			return CalcItemTextRectangle(e);
		}
	}
	public class ListBoxItemPainter : BaseListBoxItemPainter {
		protected const int fHorzTextIndent = 2;
		public ListBoxItemPainter()
			: base() {
		}
		protected override void DrawItemBar(ListBoxItemObjectInfoArgs e) {
			e.PaintAppearance.FillRectangle(e.Cache, e.Bounds);
			if(e.DrawFocusRect && (e.ItemState & DrawItemState.Focus) != 0)
				Utils.Paint.XPaint.Graphics.DrawFocusRectangle(e.Cache.Graphics, e.Bounds, e.PaintAppearance.BackColor, GetContrastColor(e.PaintAppearance.BackColor));
		}
		protected override void DrawItemHighlight(ListBoxItemObjectInfoArgs e, Rectangle textRect, StringFormat format) {
			string highlightText = GetHighlightText(e);
			Rectangle hlBounds = CalcHighlightBounds(e, textRect, format, highlightText);
			e.Graphics.FillRectangle(e.PaintAppearance.GetForeBrush(e.Cache), hlBounds);
			e.PaintAppearance.DrawString(e.Cache, highlightText, textRect,
				e.PaintAppearance.Font, e.PaintAppearance.GetBackBrush(e.Cache), format);			
		}
		protected override Rectangle CalcItemTextRectangle(ListBoxItemObjectInfoArgs e) {
			return Rectangle.Inflate(e.TextRect, -fHorzTextIndent, 0);
		}
		public override int GetVertPadding(ListBoxItemObjectInfoArgs e) { return 2; }
	}
	public class ListBoxSkinItemPainter : BaseListBoxItemPainter {
		public ListBoxSkinItemPainter() : base() { }
		protected virtual SkinElement GetSkinElement(BaseListBoxViewInfo viewInfo) {
			return CommonSkins.GetSkin(viewInfo.LookAndFeel)[CommonSkins.SkinHighlightedItem];
		}
		protected override void DrawItemBar(ListBoxItemObjectInfoArgs e) {
			DrawItemBarCore(e);
			if(e.DrawFocusRect && (e.ItemState & DrawItemState.Focus) != 0)
				Utils.Paint.XPaint.Graphics.DrawFocusRectangle(e.Cache.Graphics, e.Bounds, e.PaintAppearance.BackColor, GetContrastColor(e.PaintAppearance.BackColor));
		}
		protected virtual void DrawItemBarCore(ListBoxItemObjectInfoArgs e) {
			if(e.AllowDrawSkinBackground) {
				SkinElementInfo info = new SkinElementInfo(GetSkinElement(e.ViewInfo), e.Bounds);
				if(info == null) return;
				int imageIndex = -1;
				if((e.ItemState & DrawItemState.HotLight) != 0)
					imageIndex = 2;
				else if((e.ItemState & DrawItemState.Selected) != 0)
					imageIndex = 1;
				if(imageIndex == -1) return;
				info.ImageIndex = imageIndex;
				ObjectPainter.DrawObject(e.Cache, DevExpress.Skins.SkinElementPainter.Default, info);
			}
			else {
				e.PaintAppearance.FillRectangle(e.Cache, e.Bounds);
			}
		}
		protected override void DrawItemHighlight(ListBoxItemObjectInfoArgs e, Rectangle textRect, StringFormat format) {
			string highlightText = GetHighlightText(e);
			Rectangle hlBounds = CalcHighlightBounds(e, textRect, format, highlightText);
			Brush backBrush = e.Cache.GetSolidBrush(LookAndFeelHelper.GetSystemColor(e.ViewInfo.LookAndFeel, SystemColors.WindowText));
			Brush foreBrush = e.Cache.GetSolidBrush(LookAndFeelHelper.GetSystemColor(e.ViewInfo.LookAndFeel, SystemColors.Window));
			e.Graphics.FillRectangle(backBrush, hlBounds);
			e.PaintAppearance.DrawString(e.Cache, highlightText, textRect,
				e.PaintAppearance.Font, foreBrush, format);
		}
		protected override Rectangle CalcItemTextRectangle(ListBoxItemObjectInfoArgs e) {
			Rectangle textRect = e.TextRect;
			SkinElement element = GetSkinElement(e.ViewInfo);
			if(element != null)
				return new Rectangle(textRect.X + element.ContentMargins.Left, textRect.Y, textRect.Width - element.ContentMargins.Right * 2, textRect.Height);
			return textRect;
		}
		public override int GetHorzPadding(ListBoxItemObjectInfoArgs e) {
			SkinElement element = GetSkinElement(e.ViewInfo);
			if(element != null)
				return element.ContentMargins.Width; 
			return base.GetHorzPadding(e);
		}
		public override int GetVertPadding(ListBoxItemObjectInfoArgs e) {
			SkinElement element = GetSkinElement(e.ViewInfo);
			if(element != null)
				return element.ContentMargins.Height + 2; 
			return base.GetVertPadding(e);
		}
		public override Color GetItemBackColor(ListBoxItemObjectInfoArgs e, ObjectState state, Color defaultColor) {
			return base.GetItemForeColor(e, state, defaultColor);
		}
		public override Color GetItemForeColor(ListBoxItemObjectInfoArgs e, ObjectState state, Color defaultColor) {
			if(state == ObjectState.Disabled)
				return base.GetItemForeColor(e, state, defaultColor); 
			SkinElement element = GetSkinElement(e.ViewInfo);
			if(element != null) {
				Color color = Color.Empty;
				if(state == ObjectState.Selected) 
					color = GetElementColor(element, "SelectedTextColor");
				if(state == ObjectState.Hot) 
					color = GetElementColor(element, "HotTextColor");
				if(!color.IsEmpty)
					return color;
				else
					return element.GetForeColor(state);
			}
			return base.GetItemForeColor(e, state, defaultColor); 
		}
		internal static Color GetElementColor(SkinElement element, string propName) {
			object prop = element.Properties[propName];
			if(prop == null) return Color.Empty;
			return (Color)prop;
		}
	}
}
namespace DevExpress.XtraEditors {
	public enum HighlightStyle { Default, Standard, Skinned }
	public enum HotTrackSelectMode { SelectItemOnHotTrack, SelectItemOnClick, SelectItemOnHotTrackEx }
	public delegate void ListBoxFindItemDelegate(ListBoxFindItemArgs e);
	public class ListBoxFindItemArgs {
		int index;
		BaseListBoxControl listBox;
		bool isFound;
		public ListBoxFindItemArgs(BaseListBoxControl listBox) {
			this.index = -1;
			this.listBox = listBox;
			this.isFound = false;
		}
		public int Index { 
			get { return index; }
			internal set { index = value; }
		}
		public BaseListBoxControl ListBox { get { return listBox; } }
		public bool IsFound {
			get { return isFound; }
			set { isFound = value; }
		}
		public string DisplayText {
			get {
				if(index < 0) return null;
				return listBox.GetItemText(Index);
			}
		}
		public object DisplayItemValue {
			get {
				if(index < 0) return null;
				return listBox.GetDisplayItemValue(index);
			}
		}
		public object ItemValue {
			get {
				if(index < 0) return null;
				return listBox.GetItemValue(index);
			}
		}
	}
	[ToolboxItem(false), Designer("DevExpress.XtraEditors.Design.BaseListBoxDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign),
   DefaultEvent("SelectedIndexChanged"), Docking(DockingBehavior.Ask), SmartTagFilter(typeof(BaseListBoxControlFilter)), SmartTagAction(typeof(BaseListBoxControlActions), "Items", "Edit multi-line text", SmartTagActionType.CloseAfterExecute)]
	public abstract class BaseListBoxControl : BaseStyleControl, DevExpress.Utils.Controls.IDXFocusController, ISupportInitialize, IDataInfo, IGestureClient, IMouseWheelSupport, IContextItemCollectionOwner, IContextItemCollectionOptionsOwner, IContextItemProvider, ISearchControlClient {
		public static bool DefaultShowToolTipForTrimmedText = false;
		SelectionMode selectionMode;
		SortOrder sortOrder;
		int lockUpdate, initializing, lockScrollUpdate;
		bool multiColumn, horizontalScrollbar, incrementalSearch;
		int topIndex, columnWidth, itemHeight;
		int leftCoord, horzScrollStep;
		bool hotTrackItems, sorting;
		ListBoxScrollInfo scrollInfo;
		bool focusOnMouseDown;
		DefaultBoolean allowHtmlDraw;
		object dataSource;
		string displayMember;
		string valueMember;
		bool firstPaint;
		bool loadFired;
		bool itemAutoHeight;
		bool showFocusRect;
		ImageCollection htmlImages;
		DefaultBoolean showTooltipForTrimmedText;
		SelectedIndexCollection selectedIndices;
		SelectedItemCollection selectedItems;
		ListBoxControlHandler handler;
		ListDataAdapter dataAdapter;
		BindingManagerBase bindingManager;
		HighlightStyle highlightedItemStyle;
		HotTrackSelectMode hotTrackMode;
		ListBoxItemCollection items;
		#region ListBoxControl events
		static object selectedValueChanged = new object();
		static object selectedIndexChanged = new object();
		static object dataSourceChanged = new object();
		static object valueMemberChanged = new object();
		static object displayMemberChanged = new object();
		static object drawItem = new object();
		static object measureItem = new object();
		static readonly object contextButtonClick = new object();
		static readonly object customContextButtonToolTip = new object();
		static readonly object contextButtonValueChanged = new object();
		static readonly object customizeContextItem = new object();
		static readonly object customItemDisplayText = new object();
		[ DXCategory(CategoryName.PropertyChanged)]
		public event EventHandler DataSourceChanged {
			add { this.Events.AddHandler(dataSourceChanged, value); }
			remove { this.Events.RemoveHandler(dataSourceChanged, value); }
		}
		[ DXCategory(CategoryName.PropertyChanged)]
		public event EventHandler DisplayMemberChanged {
			add { this.Events.AddHandler(displayMemberChanged, value); }
			remove { this.Events.RemoveHandler(displayMemberChanged, value); }
		}
		[ DXCategory(CategoryName.PropertyChanged)]
		public event EventHandler ValueMemberChanged {
			add { this.Events.AddHandler(valueMemberChanged, value); }
			remove { this.Events.RemoveHandler(valueMemberChanged, value); }
		}
		[ DXCategory(CategoryName.PropertyChanged)]
		public event EventHandler SelectedIndexChanged {
			add { this.Events.AddHandler(selectedIndexChanged, value); }
			remove { this.Events.RemoveHandler(selectedIndexChanged, value); }
		}
		[ DXCategory(CategoryName.PropertyChanged)]
		public event EventHandler SelectedValueChanged {
			add { this.Events.AddHandler(selectedValueChanged, value); }
			remove { this.Events.RemoveHandler(selectedValueChanged, value); }
		}
		[DXCategory(CategoryName.Behavior), Description("")]
		public event ListBoxDrawItemEventHandler DrawItem {
			add { this.Events.AddHandler(drawItem, value); }
			remove { this.Events.RemoveHandler(drawItem, value); }
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseListBoxControlMeasureItem")
#else
	Description("")
#endif
]
		public event MeasureItemEventHandler MeasureItem {
			add { this.Events.AddHandler(measureItem, value); }
			remove { this.Events.RemoveHandler(measureItem, value); }
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseListBoxControlContextButtonClick")
#else
	Description("")
#endif
]
		public event ContextItemClickEventHandler ContextButtonClick {
			add { Events.AddHandler(contextButtonClick, value); }
			remove { Events.RemoveHandler(contextButtonClick, value); }
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseListBoxControlCustomContextButtonToolTip")
#else
	Description("")
#endif
]
		public event ContextButtonToolTipEventHandler CustomContextButtonToolTip {
			add { Events.AddHandler(customContextButtonToolTip, value); }
			remove { Events.RemoveHandler(customContextButtonToolTip, value); }
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseListBoxControlContextButtonValueChanged")
#else
	Description("")
#endif
]
		public event ContextButtonValueChangedEventHandler ContextButtonValueChanged {
			add { Events.AddHandler(contextButtonValueChanged, value); }
			remove { Events.RemoveHandler(contextButtonValueChanged, value); }
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseListBoxControlCustomizeContextItem")
#else
	Description("")
#endif
]
		public event ListBoxControlContextButtonCustomizeEventHandler CustomizeContextItem {
			add { Events.AddHandler(customizeContextItem, value); }
			remove { Events.RemoveHandler(customizeContextItem, value); }
		}
		[DXCategory(CategoryName.Behavior), Description("")]
		public event CustomItemDisplayTextEventHandler CustomItemDisplayText {
			add { 
				this.Events.AddHandler(customItemDisplayText, value);
				ViewInfo.ResetColumnWidth();
			}
			remove { 
				this.Events.RemoveHandler(customItemDisplayText, value);
				ViewInfo.ResetColumnWidth();
			}
		}
		#endregion
		public BaseListBoxControl() {
			this.allowHtmlDraw = DefaultBoolean.Default;
			this.hotTrackItems = false;
			this.showFocusRect = true;
			this.selectionMode = SelectionMode.One;
			this.dataSource = null;
			this.topIndex = columnWidth = itemHeight = 0;
			this.itemAutoHeight = false;
			this.displayMember = string.Empty;
			this.valueMember = string.Empty;
			this.bindingManager = null;
			this.multiColumn = false;
			this.sorting = false;
			this.horizontalScrollbar = false;
			this.focusOnMouseDown = true;
			this.showTooltipForTrimmedText = DefaultBoolean.Default;
			this.firstPaint = true;
			this.loadFired = false;
			this.leftCoord = 0;
			this.horzScrollStep = 5;
			this.highlightedItemStyle = HighlightStyle.Default;
			this.hotTrackMode = HotTrackSelectMode.SelectItemOnHotTrack;
			this.lockUpdate = this.initializing = this.lockScrollUpdate = 0;
			this.scrollInfo = new ListBoxScrollInfo(this);
			this.scrollInfo.VScroll.ValueChanged += new EventHandler(OnVScroll);
			this.scrollInfo.HScroll.ValueChanged += new EventHandler(OnHScroll);
			this.scrollInfo.AddControls(this);
			this.selectedIndices = CreateSelectedIndexCollection();
			this.selectedItems = new SelectedItemCollection(this);
			this.model = CreateListBoxModel();
			this.handler = CreateHandler();
			this.items = CreateItemsCollection();
		}
		protected abstract ListBoxItemCollection CreateItemsCollection();
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
		}
		protected override CreateParams CreateParams {
			get {
				CreateParams res = base.CreateParams;
				res.Style &= ~0x10000;
				return res;
			}
		}
		protected internal virtual bool EnableAccessibleOnSelectedIndexChanged { get { return true; } }
		protected override DevExpress.Accessibility.BaseAccessible CreateAccessibleInstance() {
			return new DevExpress.Accessibility.ListBoxAccessible(this);
		}
		protected internal void AccessibleNotifyClients(AccessibleEvents accEvent, int childId) {
			AccessibilityNotifyClients(accEvent, childId);
		}
#if DXWhidbey
		protected override AccessibleObject GetAccessibilityObjectById(int childId) {
			if(childId < 0 || childId >= DXAccessible.ChildCount) return base.GetAccessibilityObjectById(childId);
			AccessibleObject acc = DXAccessible.Children[childId].Accessible;
			return acc;
		}
#endif
		[Browsable(false)]
		public override bool IsLoading { get { return initializing != 0; } }
		[ AttributeProvider(typeof(IListSource)), DefaultValue(null), RefreshProperties(RefreshProperties.Repaint), DXCategory(CategoryName.Data)]
		public virtual object DataSource {
			get { return dataSource; }
			set {
				if(value == DataSource) return;
				if(value != null && DataSource != null && DataSource.Equals(value)) return;
				if(!IsValidDataSource(value)) return;
				dataSource = value;
				ActivateDataSource();
				RaiseDataSourceChanged();
			}
		}
		[DefaultValue(null), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseListBoxControlHtmlImages")
#else
	Description("")
#endif
]
		public ImageCollection HtmlImages {
			get { return htmlImages; }
			set {
				if(htmlImages == value) return;
				if(htmlImages != null) htmlImages.Changed -= new EventHandler(OnHtmlImagesChanged);
				htmlImages = value;
				if(htmlImages != null) htmlImages.Changed += new EventHandler(OnHtmlImagesChanged);
				LayoutChanged();
			}
		}
		void OnHtmlImagesChanged(object sender, EventArgs e) {
			LayoutChanged();
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseListBoxControlAllowHtmlDraw"),
#endif
 DefaultValue(DefaultBoolean.Default), DXCategory(CategoryName.Behavior)]
		public DefaultBoolean AllowHtmlDraw {
			get { return allowHtmlDraw; }
			set {
				if(AllowHtmlDraw == value) return;
				allowHtmlDraw = value;
				LayoutChanged();
			}
		}
		protected override Size DefaultSize { get { return new Size(120, 95); } }
		[ DefaultValue(""), DXCategory(CategoryName.Data),
		TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, System.Design"),
		Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor))]
		public virtual string DisplayMember {
			get { return displayMember; }
			set {
				if(value == null) value = string.Empty;
				if(value == DisplayMember) return;
				displayMember = value;
				UpdateDataSource();
				RaiseDisplayMemberChanged();
			}
		}
		void UpdateDataSource() {
			LockSelectionChanged();
			try {
				ActivateDataSource();
			}
			finally {
				UnLockSelectionChanged();
			}
		}
		[ DefaultValue(""), DXCategory(CategoryName.Data),
		TypeConverter("System.Windows.Forms.Design.DataMemberFieldConverter, System.Design"),
		Editor("System.Windows.Forms.Design.DataMemberFieldEditor, System.Design", typeof(System.Drawing.Design.UITypeEditor))]
		public virtual string ValueMember {
			get { return valueMember; }
			set {
				if(value == null) value = string.Empty;
				if(value == ValueMember) return;
				valueMember = value;
				UpdateDataSource();
				RaiseValueMemberChanged();
			}
		}
		[Browsable(false)]
		public SelectedIndexCollection SelectedIndices { get { return selectedIndices; } }
		[Browsable(false)]
		public SelectedItemCollection SelectedItems { get { return selectedItems; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public virtual int SelectedIndex {
			get {
				if(SelectedIndices.Count == 0) return -1;
				return SelectedIndices[0];
			}
			set {
				if(value < 0) value = -1;
				if(value > ItemCount - 1) value = ItemCount - 1;
				if(SelectedIndex == value) return;
				SetSelectedIndexCore(value);
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public virtual int HotItemIndex { get { return ViewInfo.HotItemIndex; } }
		bool useDisabledStatePainterCore = true;
		[DefaultValue(true),  DXCategory(CategoryName.Appearance)]
		public bool UseDisabledStatePainter {
			get { return useDisabledStatePainterCore; }
			set {
				if(useDisabledStatePainterCore != value) {
					useDisabledStatePainterCore = value;
					Invalidate();
				}
			}
		}
		[DefaultValue(false)]
		public bool ItemAutoHeight {
			get { return itemAutoHeight; }
			set {
				if(ItemAutoHeight == value) return;
				itemAutoHeight = value;
				ClearItemSizes();
				LayoutChanged();
			}
		}
		[DefaultValue(true)]
		public bool ShowFocusRect {
			get { return showFocusRect; }
			set {
				if(ShowFocusRect == value) return;
				showFocusRect = value;
				LayoutChanged();
			}
		}
		[DefaultValue(DefaultBoolean.Default),  DXCategory(CategoryName.Behavior)]
		public DefaultBoolean ShowToolTipForTrimmedText {
			get { return showTooltipForTrimmedText; }
			set { showTooltipForTrimmedText = value; }
		}
		protected internal bool CanHotTrack { get { return ((Enabled && HotTrackItems) && (SelectionMode == SelectionMode.One)); } }
		protected virtual void SetSelectedIndexCore(int index) {
			lockUpdate++;
			try {
				Handler.OnSetSelectedIndex(index);
				MakeItemVisible(SelectedIndex);
			}
			finally {
				lockUpdate--;
			}
			LayoutChanged();
		}
		int lockPosition = 0;
		bool lockSelectionChanged;
		internal void LockSelectionChanged() {
			lockSelectionChanged = true;
		}
		internal void UnLockSelectionChanged() {
			lockSelectionChanged = false;
		}
		protected internal void RecalcViewInfoState() {
			base.UpdateViewInfoState();
		}
		internal bool CanChangeSelection { get { return !lockSelectionChanged; } }
		protected internal virtual void OnSelectionChanged() {
			if(IsLoading || IsDisposing) return;
			if(CanChangeSelection) {
				if(EnableAccessibleOnSelectedIndexChanged && IsHandleCreated) {
					AccessibleNotifyClients(AccessibleEvents.Focus, SelectedIndex);
					AccessibleNotifyClients(AccessibleEvents.Selection, SelectedIndex);
				}
				if(BindingManager != null && IsBoundMode) {
					lockPosition++;
					try {
						if(BindingManager.Position != SelectedIndex)
							BindingManager.Position = SelectedIndex;
					}
					finally {
						lockPosition--;
					}
				}
				RaiseSelectedIndexChanged();
				RaiseSelectedValueChanged();
				if(IsHandleCreated)
					if(SelectedIndex > -1) AccessibilityNotifyClients(AccessibleEvents.StateChange, SelectedIndex);
			}
			LayoutChanged();
		}
		protected internal virtual void OnHotItemChanged() {
			if(ViewInfo == null || ViewInfo.HotItemIndex < 0) return;
			AccessibleNotifyClients(AccessibleEvents.Focus, HotItemIndex);
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual object SelectedValue {
			get { return GetItemValue(SelectedIndex); }
			set {
				if(SelectedValue == value) return;
				OnSetSelectedValue(value);
				RaiseSelectedValueChanged();
			}
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			if(ViewInfo.IsReady) {
				ViewInfo.OnResize();
				Model.CheckTopIndex();
				UpdateLeftCoord();
				ViewInfo.ResetItemSizesCache();
				LayoutChanged();
			}
		}
		protected void UpdateLeftCoord() {
			if(IsLoading || !loadFired) return;
			if(HorizontalScrollbar && !MultiColumn)
				SetLeftCoordCore(scrollInfo.HScroll.Value);
		}
		protected virtual void OnSetSelectedValue(object itemValue) {
			if(ItemCount == 0) return;
			int index = FindValueIndex(itemValue, SelectedIndex + 1);
			if(index == -1)
				index = FindValueIndex(itemValue, 0);
			if(index != -1)
				SelectedIndex = index;
		}
		protected virtual int FindValueIndex(object itemValue, int beginIndex) {
			if(IsBoundMode) return DataAdapter.FindValueIndex(ValueMember, itemValue, beginIndex);
			for(int i = beginIndex; i < ItemCount; i++) {
				object value = GetItemValueCore(i);
				if(ListDataAdapter.ObjectsEqual(value, itemValue))
					return i;
			}
			return -1;
		}
		public void ForceInitialize() {
			OnLoaded();
		}
		public object GetItemValue(int index) {
			if(index < 0 || index > ItemCount - 1) return null;
			if(IsBoundMode) return GetDataSourceValue(index);
			return GetItemValueCore(index);
		}
		void UpdateScrollLookAndFeel() {
			scrollInfo.HScroll.LookAndFeel.Assign(LookAndFeel);
			scrollInfo.VScroll.LookAndFeel.Assign(LookAndFeel);
		}
		protected override void OnLookAndFeelChanged(object sender, EventArgs e) {
			base.OnLookAndFeelChanged(sender, e);
			UpdateScrollLookAndFeel();
			ClearItemSizes();
			LayoutChanged();
		}
		protected override void OnStyleChanged(object sender, EventArgs e) {
			base.OnStyleChanged(sender, e);
			ClearItemSizes();
			LayoutChanged();
		}
		protected void ClearItemSizes() {
			if(ItemHeight == 0)
				ViewInfo.ItemHeight = 0;
			if(ColumnWidth == 0)
				ViewInfo.ResetColumnWidth();
			ViewInfo.ResetItemSizesCache();
		}
		protected override void OnStyleControllerChanged() {
			base.OnStyleControllerChanged();
			UpdateScrollLookAndFeel();
		}
		protected virtual object GetDataSourceValue(int index, string dataMember) {
			return DataAdapter.GetValueAtIndex(dataMember, index);
		}
		protected virtual object GetDataSourceValue(int index) {
			return GetDataSourceValue(index, ValueMember);
		}
		protected virtual object GetItemValueCore(int index) {
			if(CanFilterItems)
				index = SourceItems.GetSourceIndex(index);
			return ItemsCore[index];
		}
		protected ListBoxItemCollection ItemsCore { get { return items; } }
		protected virtual void OnListChanged(object sender, ListChangedEventArgs e) {
			if(IsDisposing) return;
			CheckStopAnimation();
			if(ItemCount == 0) SelectedIndex = -1;
			ViewInfo.ResetColumnWidth();
			ViewInfo.ResetItemSizesCache();
			lockUpdate++;
			try {
				switch(e.ListChangedType) {
					case ListChangedType.ItemDeleted:
						OnListItemDeleted(e);
						break;
					case ListChangedType.ItemAdded:
						OnListItemAdded(e);
						break;
					case ListChangedType.Reset:
						OnListReset(e);
						break;
					default:
						OnListChangedDefault(e);
						break;
				}
				Handler.OnListChanged();
				Model.CheckTopIndex();
				ApplySort();
			}
			finally {
				lockUpdate--;
			}
			UpdateLeftCoord();
			LayoutChanged();
		}
		protected virtual void OnListItemDeleted(ListChangedEventArgs e) {
			bool currentSelected = e.NewIndex == SelectedIndex;
			SelectedIndices.OnListChanged(e);
			if(currentSelected && SelectedIndices.Count == 0) SelectedIndex = e.NewIndex;
		}
		protected virtual void OnListItemAdded(ListChangedEventArgs e) {
			SelectedIndices.OnListChanged(e);
			if(SelectedIndices.Count == 0) SelectedIndex = e.NewIndex;
		}
		protected virtual void OnListReset(ListChangedEventArgs e) {
			int selIndex = SelectedIndex;
			selectedIndices.OnListChanged(e);
			if(SelectionMode == SelectionMode.One) {
				if(BindingManager != null && BindingManager.Position >= 0 && lockPosition == 0)
					selIndex = BindingManager.Position; 
			}
			if(selIndex >= 0) SelectedIndex = selIndex;
		}
		protected virtual void OnListChangedDefault(ListChangedEventArgs e) {
			int selIndex = SelectedIndex;
			SelectedIndices.OnListChanged(e);
			if(SelectedIndices.Count == 0) SelectedIndex = selIndex;
		}
		public override Color BackColor {
			get { return base.BackColor; }
			set {
				if(!value.Equals(Color.Empty) && value.A < 255) throw new ArgumentException(Localizer.Active.GetLocalizedString(StringId.TransparentBackColorNotSupported));
				base.BackColor = value;
			}
		}
		[DXCategory(CategoryName.Behavior), DefaultValue(SortOrder.None), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseListBoxControlSortOrder"),
#endif
 SmartTagProperty("Sort Order", "", SmartTagActionType.RefreshBoundsAfterExecute)]
		public SortOrder SortOrder {
			get { return sortOrder; }
			set {
				if(SortOrder == value) return;
				sortOrder = value;
				ApplySort();
				LayoutChanged();
			}
		}
		bool IsSorting { get { return sorting; } set { sorting = value; } }
		protected void ApplySort() {
			if(SortOrder == SortOrder.None || IsBoundMode || IsDisposing || IsSorting) return;
			sorting = true;
			try {
				DoSort();
			}
			finally {
				sorting = false;
			}
		}
		protected abstract void DoSort();
		[DXCategory(CategoryName.Behavior),  DefaultValue(SelectionMode.One), SmartTagProperty("Selection Mode", "")]
		public SelectionMode SelectionMode {
			get { return selectionMode; }
			set {
				if(SelectionMode == value) return;
				selectionMode = value;
				Handler.SetHandlerState(CalcHandlerState(SelectionMode));
			}
		}
		[DXCategory(CategoryName.Behavior), DefaultValue(false), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("BaseListBoxControlHotTrackItems")
#else
	Description("")
#endif
]
		public virtual bool HotTrackItems {
			get { return hotTrackItems; }
			set {
				if(HotTrackItems != value) {
					hotTrackItems = value;
					ViewInfo.ResetItemSizesCache();
				}
			}
		}
		[DXCategory(CategoryName.Appearance), DefaultValue(HighlightStyle.Default)]
		public virtual HighlightStyle HighlightedItemStyle {
			get { return highlightedItemStyle; }
			set {
				if(HighlightedItemStyle == value) return;
				highlightedItemStyle = value;
				ViewInfo.HotItemIndex = -1;
				ClearItemSizes();
				LayoutChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), DefaultValue(HotTrackSelectMode.SelectItemOnHotTrack)]
		public virtual HotTrackSelectMode HotTrackSelectMode {
			get { return hotTrackMode; }
			set {
				if(HotTrackSelectMode == value) return;
				hotTrackMode = value;
				LayoutChanged();
			}
		}
		[Obsolete("Use the HotTrackSelectMode property instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool SetSelectedIndexOnHotTrack {
			get { return false; }
			set { }
		}
		[Browsable(false)]
		public virtual int ItemCount {
			get {
				if(IsBoundMode) return DataAdapter.ItemCount;
				return ItemCountCore;
			}
		}
		[Browsable(false)]
		protected virtual int ItemCountCore {
			get {
				if(CanFilterItems)
					return SourceItems.VisibleCount;
				return ItemsCore.Count;
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public virtual object SelectedItem {
			get { return GetItem(SelectedIndex); }
			set {
				if(SelectedItem == value) return;
				OnSetItem(value);
			}
		}
		void OnSetItem(object item) {
			if(IsBoundMode) {
				int index = FindItem(item);
				if(index != -1) SelectedIndex = index;
			}
			else {
				SetItemCore(item);
			}
		}
		protected override void OnRightToLeftChanged(EventArgs e) {
			base.OnRightToLeftChanged(e);
			LayoutChanged();
		}
		internal new bool IsRightToLeft { get { return base.IsRightToLeft; } }
		protected virtual void SetItemCore(object item) {
			int index = ItemsCore.IndexOf(item);
			if(index != -1)
				SelectedIndex = index;
		}
		[DXCategory(CategoryName.Behavior), DefaultValue(false),  SmartTagProperty("MultiColumn", "")]
		public virtual bool MultiColumn {
			get { return multiColumn; }
			set {
				if(MultiColumn == value) return;
				multiColumn = value;
				topIndex = 0;
				model = CreateListBoxModel();
				LayoutChanged();
			}
		}
		[DXCategory(CategoryName.Behavior), DefaultValue(false), Description("")]
		public virtual bool IncrementalSearch {
			get { return incrementalSearch; }
			set {
				if(IncrementalSearch == value) return;
				incrementalSearch = value;
				Handler.OnIncrementalSearchChanged();
			}
		}
		protected virtual ListBoxModel CreateListBoxModel() {
			if(MultiColumn) return new MultiColumnModel(this);
			return new SingleColumnModel(this);
		}
		ListBoxModel model = null;
		protected internal ListBoxModel Model {
			get { return model; }
			set {
				if(value == null || Model == value) return;
				model = value;
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public virtual int TopIndex {
			get { return topIndex; }
			set {
				if(value < 0) value = 0;
				if(value > ItemCount) value = ItemCount;
				if(value == TopIndex) return;
				if(loadFired)
					SetTopIndexCore(value);
				LayoutChanged();
			}
		}
		protected internal virtual void SetTopIndexCore(int value) {
			topIndex = Math.Max(0, Math.Min(value, Model.GetMaxTopIndex()));
		}
		[DXCategory(CategoryName.Appearance),  DefaultValue(0)]
		public virtual int ColumnWidth {
			get { return columnWidth; }
			set {
				if(value < 0) value = 0;
				if(ColumnWidth == value) return;
				columnWidth = value;
				ViewInfo.ColumnWidth = columnWidth;
				LayoutChanged();
			}
		}
		[ DXCategory(CategoryName.Appearance), DefaultValue(0)]
		public virtual int ItemHeight {
			get { return itemHeight; }
			set {
				if(value < 0) value = 0;
				if(ItemHeight == value) return;
				itemHeight = value;
				ViewInfo.ItemHeight = itemHeight;
				LayoutChanged();
			}
		}
		[ DefaultValue(false), SmartTagProperty("HorizontalScrollbar", "", SmartTagActionType.RefreshBoundsAfterExecute)]
		public bool HorizontalScrollbar {
			get { return horizontalScrollbar; }
			set {
				if(value == horizontalScrollbar) return;
				horizontalScrollbar = value;
				ViewInfo.ResetItemSizesCache();
				LayoutChanged();
			}
		}
		[ DefaultValue(5)]
		public int HorzScrollStep {
			get { return horzScrollStep; }
			set {
				value = Math.Min(100, Math.Max(value, 1));
				if(HorzScrollStep == value) return;
				horzScrollStep = value;
				LayoutChanged();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int LeftCoord {
			get {
				if(!MultiColumn && HorizontalScrollbar) return leftCoord;
				return 0;
			}
			set {
				if(value < 0) value = 0;
				SetLeftCoordCore(value);
				LayoutChanged();
			}
		}
		protected internal virtual void SetLeftCoordCore(int value) {
			int maxLeftCoord = ViewInfo.BestColumnWidth - ViewInfo.ContentRect.Width;
			if(value > maxLeftCoord) value = maxLeftCoord;
			if(LeftCoord == value) return;
			leftCoord = Math.Max(0, value);
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public override string Text { get { return GetItemText(SelectedIndex); } set { } }
		protected virtual ListDataAdapter CreateDataAdapter() { return new ListDataAdapter(); }
		protected ListDataAdapter DataAdapter {
			get {
				if(dataAdapter == null) {
					dataAdapter = CreateDataAdapter();
					dataAdapter.VisibleRowCountChanged += new EventHandler(this.OnVisibleRowCountChanged);
					dataAdapter.DataSourceChanged += new EventHandler(OnDataSourceChanged);
					dataAdapter.AdapterListChanged += new ListChangedEventHandler(OnListChanged);
				}
				return dataAdapter;
			}
		}
		protected internal new BaseListBoxViewInfo ViewInfo { get { return base.ViewInfo as BaseListBoxViewInfo; } }
		protected internal ListBoxScrollInfo ScrollInfo { get { return scrollInfo; } }
		protected internal string CurrentSearch { get { return Handler.CurrentSearch; } }
		void OnVScroll(object sender, EventArgs e) {
			if(lockScrollUpdate != 0) return;
			Model.OnVScroll(scrollInfo.VScrollPosition);
		}
		void OnHScroll(object sender, EventArgs e) {
			if(lockScrollUpdate != 0) return;
			Model.OnHScroll(scrollInfo.HScrollPosition);
		}
		protected override void OnMouseDown(MouseEventArgs e) {
			base.OnMouseDown(e);
			if(!GetValidationCanceled()) Handler.OnMouseDown(e);
		}
		protected override void OnLostCapture() {
			base.OnLostCapture();
			Handler.OnLostCapture();
		}
		protected override void OnMouseEnter(EventArgs e) {
			Handler.OnMouseEnter();
			base.OnMouseEnter(e);
		}
		protected override void OnMouseLeave(EventArgs e) {
			Handler.OnMouseLeave();
			base.OnMouseLeave(e);
		}
		protected override void OnLostFocus(EventArgs e) {
			base.OnLostFocus(e);
			Handler.OnLostFocus();
			LayoutChanged();
		}
		protected virtual void OnDataSourceChanged(object sender, EventArgs e) {
			UpdateBindingManager();
			ApplySort();
			LayoutChanged();
		}
		public int FindItem(object item) {
			if(IsBoundMode) {
				for(int n = 0; n < DataAdapter.VisibleListSourceRowCount; n++) {
					object dataRow = DataAdapter.GetRow(n);
					if(dataRow != null && dataRow.Equals(item)) return n;
				}
			}
			return -1;
		}
		public int FindString(string s, int startIndex, bool updown) {
			s = s.ToLower();
			return FindItem(startIndex, updown, delegate(ListBoxFindItemArgs e) {
				string itemText = e.DisplayText.ToLower();
				e.IsFound = itemText.StartsWith(s) && ((s != string.Empty) || (itemText == string.Empty));
			});
		}
		public int FindString(string s, int startIndex) { return FindString(s, startIndex, true); }
		public int FindString(string s) { return FindString(s, -1); }
		public int FindStringExact(string s, int startIndex) {
			s = s.ToLower();
			return FindItem(startIndex, true, delegate(ListBoxFindItemArgs e) {
				string itemText = e.DisplayText.ToLower();
				e.IsFound = s.Equals(itemText);
			});
		}
		public int FindStringExact(string s) { return FindStringExact(s, -1); }
		public int FindItem(int startIndex, bool updown, ListBoxFindItemDelegate predicate) {
			if((startIndex >= ItemCount && updown) || (startIndex < 0 && !updown))
				return -1;
			startIndex = Math.Max(0, startIndex);
			int endIndex = updown ? ItemCount : -1,
				step = updown ? 1 : -1;
			ListBoxFindItemArgs e = new ListBoxFindItemArgs(this);
			for(int i = startIndex; i != endIndex; i += step) {
				e.Index = i;
				predicate(e);
				if(e.IsFound)
					return i;
			}
			return -1;
		}
		protected virtual void UpdateBindingManager() {
			if(IsDisposing || DataAdapter.ListSource == null) return;
			if(bindingManager != null) bindingManager.PositionChanged -= new EventHandler(OnPositionChanged);
			if(BindingContext != null) {
				try {
					string dataMember = (DataSource is DataSet ? ViewMember : string.Empty);
					bindingManager = BindingContext[DataSource, dataMember];
				}
				catch { }
				if(BindingManager != null) {
					BindingManager.PositionChanged += new EventHandler(OnPositionChanged);
					SelectedIndex = BindingManager.Position;
				}
				this.viewMemberConverter = null;
			}
		}
		protected override void OnBindingContextChanged(EventArgs e) {
			base.OnBindingContextChanged(e);
			UpdateBindingManager();
		}
		protected virtual void OnVisibleRowCountChanged(object sender, EventArgs e) {
			LayoutChanged();
		}
		protected virtual void OnPositionChanged(object sender, EventArgs e) {
			if(SelectionMode == SelectionMode.None || SelectionMode == SelectionMode.MultiSimple) return;
			if(lockPosition == 0) {
				if(BindingManager.Position > ItemCount - 1) return; 
				SelectedIndex = BindingManager.Position;
			}
		}
		protected internal BindingManagerBase BindingManager { get { return bindingManager; } }
		protected override void OnGotFocus(EventArgs e) {
			base.OnGotFocus(e);
			Handler.OnGotFocus();
		}
		protected override bool IsInputKey(Keys keyData) {
			if(keyData == Keys.Tab || keyData == (Keys.Tab | Keys.Shift)) return false;
			if(keyData == Keys.Left || keyData == Keys.Right || keyData == Keys.Up || keyData == Keys.Down) return (ItemCount > 0);
			return base.IsInputKey(keyData);
		}
		protected override bool IsInputChar(char charCode) {
			if((Control.ModifierKeys & Keys.Alt) != 0) return base.IsInputChar(charCode);
			return (ItemCount > 0);
		}
		protected override bool ProcessDialogKey(Keys keyData) {
			Keys key = keyData & (~Keys.Modifiers);
			if(key == Keys.Down || key == Keys.Up ||
				key == Keys.Left || key == Keys.Right) return false;
			return base.ProcessDialogKey(keyData);
		}
		protected sealed override void OnMouseWheel(MouseEventArgs ev) {
			if(XtraForm.ProcessSmartMouseWheel(this, ev)) return;
			OnMouseWheelCore(ev);
		}
		void IMouseWheelSupport.OnMouseWheel(MouseEventArgs e) {
			OnMouseWheelCore(e);
		}
		protected virtual void OnMouseWheelCore(MouseEventArgs ev) {
			DXMouseEventArgs ee = DXMouseEventArgs.GetMouseArgs(ev);
			try {
				base.OnMouseWheel(ee);
				if(!ee.Handled) {
					ee.Handled = true;
					Handler.OnMouseWheel(ee);
				}
			}
			finally {
				ee.Sync();
			}
		}
		protected override Size CalcSizeableMinSize() { return AutoSizeInLayoutControl ? CalcBestSize() : new Size(50, 0); }
		protected override Size CalcSizeableMaxSize() { return new Size(0, 0); }
		protected override void OnKeyDown(KeyEventArgs e) {
			base.OnKeyDown(e);
			if(e.Handled) return;
			Handler.OnKeyDown(e);
		}
		protected override void OnKeyUp(KeyEventArgs e) {
			base.OnKeyUp(e);
			Handler.OnKeyUp(e);
		}
		protected override void OnKeyPress(KeyPressEventArgs e) {
			base.OnKeyPress(e);
			if(e.Handled) return;
			Handler.OnKeyPress(e);
		}
		protected override void OnMouseUp(MouseEventArgs e) {
			base.OnMouseUp(e);
			if(!GetValidationCanceled()) Handler.OnMouseUp(e);
		}
		protected override void OnEnabledChanged(EventArgs e) {
			base.OnEnabledChanged(e);
			LayoutChanged();
		}
		protected override void OnVisibleChanged(EventArgs e) {
			base.OnVisibleChanged(e);
			if(Visible && IsHandleCreated)
				OnLoaded();
			LayoutChanged();
		}
		protected override void OnMouseMove(MouseEventArgs e) {
			base.OnMouseMove(e);
			ScrollInfo.OnAction(ScrollNotifyAction.MouseMove);
			Handler.OnMouseMove(e);
		}
		public object GetItem(int index) {
			if((index < 0) || (index > (ItemCount - 1))) return null;
			if(IsBoundMode) return DataAdapter.GetRow(index);
			return GetItemCore(index);
		}
		protected virtual object GetItemCore(int index) {
			if(CanFilterItems)
				index = SourceItems.GetSourceIndex(index);
			return ItemsCore[index];
		}
		public virtual Rectangle GetItemRectangle(int index) {
			if((index < 0) || (index >= ItemCount)) return Rectangle.Empty;
			return ((ViewInfo == null) ? Rectangle.Empty : ViewInfo.GetItemRectangle(index));
		}
		protected string ViewMember { get { return ((DisplayMember == string.Empty) ? ValueMember : DisplayMember); } }
		TypeConverter viewMemberConverter = null;
		TypeConverter ViewMemberConverter {
			get {
				if((viewMemberConverter == null) && (BindingManager != null)) {
					PropertyDescriptorCollection itemProperties = BindingManager.GetItemProperties();
					if(itemProperties != null) {
						PropertyDescriptor descriptor = itemProperties.Find(ViewMember, true);
						if(descriptor != null)
							viewMemberConverter = descriptor.Converter;
					}
				}
				return viewMemberConverter;
			}
		}
		public virtual string GetItemText(int index) {
			return GetItemTextCore(index);
		}
		protected virtual string GetItemTextCore(int index) {
			object item = GetDisplayItemValue(index);
			if((item == null) || (item == DBNull.Value))
				return string.Empty;
			if(IsBoundMode && ViewMemberConverter != null)
				return ViewMemberConverter.ConvertToString(item);
			return Convert.ToString(item, System.Globalization.CultureInfo.CurrentCulture);
		}
		public object GetDisplayItemValue(int index) {
			if(index < 0 || index > ItemCount - 1) return string.Empty;
			if(IsBoundMode) return GetDataSourceDisplayValue(index);
			return GetItemValueCore(index);
		}
		protected internal virtual string GetDrawItemText(int index) { return GetDrawItemTextCore(index); }
		protected virtual string GetDrawItemTextCore(int index) {
			CustomItemDisplayTextEventArgs e = new CustomItemDisplayTextEventArgs(this, index, GetItemTextCore(index));
			CustomItemDisplayTextEventHandler handler = (CustomItemDisplayTextEventHandler)this.Events[customItemDisplayText];
			if(handler != null)
				handler(this, e);
			return e.DisplayText;
		}
		protected virtual object GetDataSourceDisplayValue(int index) {
			if(DisplayMember == string.Empty && ValueMember != string.Empty)
				return string.Empty;
			return GetDataSourceValue(index, ViewMember);
		}
		public void SetItemValue(object itemValue, int index) {
			if(index < 0 || index > ItemCount - 1) return;
			if(IsBoundMode) SetDataSourceValue(itemValue, index);
			else SetItemValueCore(itemValue, index);
		}
		protected virtual void SetDataSourceValue(object itemValue, int index) {
			DataAdapter.SetValueAtIndex(ValueMember, index, itemValue);
		}
		protected virtual void SetItemValueCore(object itemValue, int index) {
			ItemsCore[index] = itemValue;
		}
		protected internal bool IsBoundMode { get { return dataAdapter != null && DataAdapter.ListSource != null; } }
		protected virtual ListBoxControlHandler CreateHandler() { return new ListBoxControlHandler(this); }
		protected virtual SelectedIndexCollection CreateSelectedIndexCollection() { return new SelectedIndexCollection(this); }
		protected internal ListBoxControlHandler Handler { get { return handler; } }
		protected virtual void UpdateScrollBars() {
			if(IsLockUpdate || lockScrollUpdate != 0) return;
			lockScrollUpdate++;
			try {
				scrollInfo.IsRightToLeft = IsRightToLeft;
				UpdateHScrollBar();
				UpdateVScrollBar();
				scrollInfo.ClientRect = ViewInfo.ClientRect;
			}
			finally {
				lockScrollUpdate--;
			}
		}
		void UpdateHScrollBar() {
			scrollInfo.HScrollVisible = ViewInfo.IsNeededHScroll;
			if(scrollInfo.HScrollVisible) scrollInfo.HScrollArgs = Model.CalcHScrollArgs();
		}
		void UpdateVScrollBar() {
			scrollInfo.VScrollVisible = ViewInfo.IsNeededVScroll;
			if(scrollInfo.VScrollVisible) scrollInfo.VScrollArgs = Model.CalcVScrollArgs();
		}
		public void SetSelected(int index, bool value) {
			if(index < 0 || index > ItemCount - 1) return;
			Handler.OnSetSelected(index, value);
		}
		public void SelectAll() {
			if(SelectionMode == SelectionMode.None || SelectionMode == SelectionMode.One) return;
			BeginUpdate();
			for(int i = 0; i < ItemCount; i++) SetSelected(i, true);
			EndUpdate();
		}
		public void UnSelectAll() {
			if(SelectionMode == SelectionMode.None || SelectionMode == SelectionMode.One) return;
			SelectedIndices.Clear();
		}
		internal bool MeasureItemAssigned { get { return (this.Events[measureItem] != null); } }
		protected override void OnCreateControl() {
			base.OnCreateControl();
			if(Parent != null)
				OnLoaded();
		}
		public void MakeItemVisible(int index) {
			if((index > ItemCount) || (index < 0) || !ViewInfo.IsReady) return;
			Model.MakeItemVisible(index);
		}
		protected virtual void OnLoaded() {
			if(IsLoading || loadFired) return;
			loadFired = true;
			Handler.SetHandlerState(CalcHandlerState(SelectionMode));
			ActivateDataSource();
			LayoutChanged();
			if(SelectedIndex != -1) MakeItemVisible(SelectedIndex);
			ViewInfo.FocusedItemIndex = Math.Max(0, SelectedIndex);
			Invalidate();
		}
		public int IndexFromPoint(Point p) {
			BaseListBoxViewInfo.ItemInfo item = ViewInfo.GetItemInfoByPoint(p);
			return (item == null ? -1 : item.Index);
		}
		protected internal virtual ListBoxControlHandler.HandlerState CalcHandlerState(SelectionMode mode) {
			switch(mode) {
				case SelectionMode.One:
					return ListBoxControlHandler.HandlerState.SingleSelect;
				case SelectionMode.MultiSimple:
					return ListBoxControlHandler.HandlerState.MultiSimpleSelect;
				case SelectionMode.MultiExtended:
					return ListBoxControlHandler.HandlerState.ExtendedMultiSimpleSelect;
			}
			return ListBoxControlHandler.HandlerState.Unselectable;
		}
		protected virtual void ActivateDataSource() {
			if(IsLoading) return;
			DataAdapter.SetDataSource(DataSource, DisplayMember, ValueMember);
		}
		protected virtual bool IsValidDataSource(object dataSource) { return true; }
		protected void RaiseDataSourceChanged() {
			if(IsLoading) return;
			EventHandler handler = (EventHandler)this.Events[dataSourceChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected void RaiseDisplayMemberChanged() {
			if(IsLoading) return;
			EventHandler handler = (EventHandler)this.Events[displayMemberChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected void RaiseValueMemberChanged() {
			if(IsLoading) return;
			EventHandler handler = (EventHandler)this.Events[valueMemberChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected internal virtual void RaiseSelectedIndexChanged() {
			EventHandler handler = (EventHandler)this.Events[selectedIndexChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected internal void RaiseSelectedValueChanged() {
			EventHandler handler = (EventHandler)this.Events[selectedValueChanged];
			if(handler != null) handler(this, EventArgs.Empty);
		}
		protected internal virtual void RaiseDrawItem(ListBoxDrawItemEventArgs e) {
			if(AsyncServerModeDataController.IsNoValue(e.Item)) {
				e.Handled = true;
				LoadingAnimator.Bounds = ClientRectangle;
				LoadingAnimator.DrawAnimatedItem(e.Cache, e.Bounds);
				return;
			}
			ListBoxDrawItemEventHandler handler = (ListBoxDrawItemEventHandler)this.Events[drawItem];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseMeasureItem(MeasureItemEventArgs e) {
			MeasureItemEventHandler handler = (MeasureItemEventHandler)this.Events[measureItem];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseContextButtonClick(ContextItemClickEventArgs e) {
			ContextItemClickEventHandler handler = Events[contextButtonClick] as ContextItemClickEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected internal virtual void RaiseCustomContextButtonToolTip(ContextButtonToolTipEventArgs e) {
			ContextButtonToolTipEventHandler handler = Events[customContextButtonToolTip] as ContextButtonToolTipEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected internal virtual void RaiseContextButtonValueChanged(ContextButtonValueEventArgs e) {
			ContextButtonValueChangedEventHandler handler = Events[contextButtonValueChanged] as ContextButtonValueChangedEventHandler;
			if(handler != null)
				handler(this, e);
		}
		protected internal virtual void RaiseCustomizeContextItem(ListBoxControlContextButtonCustomizeEventArgs e) {
			ListBoxControlContextButtonCustomizeEventHandler handler = Events[customizeContextItem] as ListBoxControlContextButtonCustomizeEventHandler;
			if(handler != null)
				handler(this, e);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsLockUpdate { get { return lockUpdate != 0; } }
		public virtual void BeginUpdate() { lockUpdate++; }
		public virtual void EndUpdate() {
			CancelUpdate();
			LayoutChanged();
		}
		public virtual void CancelUpdate() { lockUpdate--; }
		protected override void Dispose(bool disposing) {
			fDisposing = true;
			base.Dispose(disposing);
			if(disposing) {
				if(loadingAnimator != null) loadingAnimator.Dispose();
				scrollInfo.VScroll.ValueChanged -= new EventHandler(OnVScroll);
				scrollInfo.HScroll.ValueChanged -= new EventHandler(OnHScroll);
				if(dataAdapter != null) {
					dataAdapter.DataSourceChanged -= new EventHandler(OnDataSourceChanged);
					dataAdapter.AdapterListChanged -= new ListChangedEventHandler(OnListChanged);
					dataAdapter.Dispose();
				}
				scrollInfo.RemoveControls(this);
				scrollInfo.Dispose();
				handler.Dispose();
			}
		}
		protected internal static HighlightStyle GetHighlightedItemStyle(UserLookAndFeel lookAndFeel, HighlightStyle highlightedItemStyle) {
			if(lookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
				if(highlightedItemStyle == HighlightStyle.Default || highlightedItemStyle == HighlightStyle.Skinned) return HighlightStyle.Skinned;
			}
			if(highlightedItemStyle == HighlightStyle.Default || highlightedItemStyle == HighlightStyle.Skinned) return HighlightStyle.Standard;
			return highlightedItemStyle;
		}
		protected internal virtual bool IsSkinnedHightlighingEnabled() {
			if(GetHighlightedItemStyle(LookAndFeel, HighlightedItemStyle) == HighlightStyle.Skinned) {
				return CommonSkins.GetSkin(LookAndFeel)[CommonSkins.SkinHighlightedItem] != null;
			}
			return false;
		}
		protected internal virtual bool IsAdvHotTrackEnabled() {
			return IsSkinnedHightlighingEnabled() && HotTrackSelectMode == HotTrackSelectMode.SelectItemOnClick;
		}
		protected internal virtual bool GetItemEnabledCore(int itemIndex) {
			return true;
		}
		protected internal override void LayoutChanged() {
			if(IsDisposing)
				return;
			if(lockUpdate != 0) return;
			base.LayoutChanged();
			UpdateScrollBars();
		}
		public Rectangle GetBaseTextBounds() {
			Rectangle textRect = ViewInfo.BaseTextBounds;
			textRect.Y = textRect.Y + (textRect.Height - ViewInfo.GetTextAscentHeight()) / 2;
			textRect.Height = ViewInfo.GetTextAscentHeight();
			return textRect;
		}
		void ISupportInitialize.BeginInit() { loadFired = false; initializing++; }
		void ISupportInitialize.EndInit() {
			initializing--;
		}
		bool DevExpress.Utils.Controls.IDXFocusController.FocusOnMouseDown { get { return focusOnMouseDown; } set { focusOnMouseDown = value; } }
		protected virtual bool CanFocusListBox { get { return ((DevExpress.Utils.Controls.IDXFocusController)this).FocusOnMouseDown; } }
		#region SelectedIndexCollection
		public class SelectedIndexCollection : ICollection, IEnumerable, IEnumerable<int> {
			List<int> indices;
			int lockChanged;
			BaseListBoxControl owner;
			public SelectedIndexCollection(BaseListBoxControl owner) {
				this.owner = owner;
				this.lockChanged = 0;
				this.indices = new List<int>();
			}
			public int Count { get { return indices.Count; } }
			public int this[int index] { get { return (int)indices[index]; } }
			public int IndexOf(int value) { return indices.IndexOf(value); }
			public virtual void CopyTo(Array array, int index) {
				((ICollection)indices).CopyTo(array, index);
			}
			public bool Contains(int value) { return (IndexOf(value) != -1); }
			bool ICollection.IsSynchronized { get { return ((ICollection)indices).IsSynchronized; } }
			object ICollection.SyncRoot { get { return ((ICollection)indices).SyncRoot; } }
			public bool IsReadOnly { get { return true; } }
			IEnumerator IEnumerable.GetEnumerator() { return indices.GetEnumerator(); }
			IEnumerator<int> IEnumerable<int>.GetEnumerator() { return indices.GetEnumerator(); }
			protected internal void Clear() {
				indices.Clear();
				Changed();
			}
			internal bool OnListChanged(ListChangedEventArgs e) {
				bool changed = false;
				BeginUpdate();
				if(e.ListChangedType == ListChangedType.ItemDeleted) {
					if(Contains(e.NewIndex)) {
						indices.Remove(e.NewIndex);
						changed = true;
					}
					changed |= ShiftSelection(e.NewIndex, true);
				}
				if(e.ListChangedType == ListChangedType.Reset) {
					indices.Clear();
				}
				if(e.ListChangedType == ListChangedType.ItemAdded) {
					changed |= ShiftSelection(e.NewIndex, false);
				}
				if(changed)
					EndUpdate();
				else
					CancelUpdate();
				return changed;
			}
			bool ShiftSelection(int position, bool delete) {
				bool changed = false;
				for(int n = indices.Count - 1; n >= 0; n--) {
					int index = (int)indices[n];
					if(delete && index > position) {
						indices[n] = --index;
						if(index < 0) indices.RemoveAt(n);
						changed = true;
					}
					if(!delete && index >= position) {
						indices[n] = ++index;
						changed = true;
					}
				}
				return changed;
			}
			protected internal void Set(int value) {
				if(indices.Count == 1 && (int)indices[0] == value) return;
				if(indices.Count == 0) {
					indices.Add(value);
				}
				else if(indices.Count == 1) {
					indices[0] = value;
				}
				else {
					indices.Clear();
					indices.Add(value);
				}
				Changed();
			}
			protected internal void Set(List<int> indexCollection) {
				indices = new List<int>(indexCollection);
				Changed();
			}
			protected internal void AddRemove(int value) {
				if(Contains(value))
					indices.Remove(value);
				else
					indices.Add(value);
				Changed();
			}
			protected internal void BeginUpdate() { lockChanged++; }
			protected internal void EndUpdate() {
				if(--lockChanged == 0) Changed();
			}
			protected internal void CancelUpdate() { lockChanged--; }
			protected virtual void Changed() {
				if(lockChanged != 0) return;
				owner.OnSelectionChanged();
			}
		}
		#endregion
		#region SelectedItemsCollection
		public class SelectedItemCollection : ICollection, IEnumerable {
			BaseListBoxControl listBox;
			internal SelectedItemCollection(BaseListBoxControl listBox) { this.listBox = listBox; }
			public int Count { get { return SelectedIndices.Count; } }
			protected SelectedIndexCollection SelectedIndices { get { return listBox.SelectedIndices; } }
			bool ICollection.IsSynchronized { get { return ((ICollection)SelectedIndices).IsSynchronized; } }
			object ICollection.SyncRoot { get { return ((ICollection)SelectedIndices).SyncRoot; } }
			void ICollection.CopyTo(Array array, int index) {
				ArrayList itemValues = new ArrayList();
				for(int i = 0; i < Count; i++) itemValues.Add(this[i]);
				itemValues.CopyTo(array, index);
			}
			IEnumerator IEnumerable.GetEnumerator() { return new SelectedItemCollectionIEnumerator(((IEnumerable)SelectedIndices).GetEnumerator(), listBox); }
			public bool IsReadOnly { get { return SelectedIndices.IsReadOnly; } }
			public int IndexOf(object itemValue) {
				for(int i = 0; i < Count; i++) {
					if(this[i] == itemValue) return i;
				}
				return -1;
			}
			public object this[int index] { get { return listBox.GetItem(SelectedIndices[index]); } }
			private class SelectedItemCollectionIEnumerator : IEnumerator {
				IEnumerator indicesEnumerator;
				BaseListBoxControl listBox;
				public SelectedItemCollectionIEnumerator(IEnumerator indicesEnumerator, BaseListBoxControl listBox) {
					this.indicesEnumerator = indicesEnumerator;
					this.listBox = listBox;
				}
				bool IEnumerator.MoveNext() { return indicesEnumerator.MoveNext(); }
				void IEnumerator.Reset() { indicesEnumerator.Reset(); }
				object IEnumerator.Current { get { return listBox.GetItem((int)indicesEnumerator.Current); } }
			}
		}
		#endregion
		#region ListBoxModels
		public abstract class ListBoxModel {
			BaseListBoxControl listBox;
			public ListBoxModel(BaseListBoxControl listBox) { this.listBox = listBox; }
			protected BaseListBoxControl ListBox { get { return listBox; } }
			protected BaseListBoxViewInfo ViewInfo { get { return listBox.ViewInfo; } }
			protected int TopIndex { get { return listBox.TopIndex; } set { listBox.TopIndex = value; } }
			protected int ItemCount { get { return listBox.ItemCount; } }
			protected int LeftCoord { get { return listBox.LeftCoord; } set { listBox.LeftCoord = value; } }
			public abstract int GetMaxTopIndex();
			public abstract void OnHScroll(int scrollPos);
			public abstract void OnVScroll(int scrollPos);
			public abstract ScrollArgs CalcHScrollArgs();
			public abstract ScrollArgs CalcVScrollArgs();
			public abstract void MakeItemVisible(int index);
			public abstract int GetWheelChange();
			public abstract int CalcKeyDownDelta(KeyEventArgs args);
			public abstract int CalcOnTimerDelta(Point ptMouse);
			public abstract Point GetHotTrackPoint(Point pt);
			public abstract int CalcScrollTimerInterval(Point hotPt);
			public virtual void CheckTopIndex() {
				if(ListBox.IsLoading || !ListBox.loadFired) return;
				if(TopIndex > GetMaxTopIndex()) TopIndex = GetMaxTopIndex();
			}
		}
		public class SingleColumnModel : ListBoxModel {
			public SingleColumnModel(BaseListBoxControl listBox) : base(listBox) { }
			public override int GetMaxTopIndex() {
				if(ItemCount == 0) return 0;
				return ItemCount - ViewInfo.CalcVisibleItemCountFromBottom(ItemCount - 1);
			}
			public override void OnVScroll(int scrollPos) {
				TopIndex = scrollPos;
			}
			public override void OnHScroll(int scrollPos) {
				LeftCoord = scrollPos;
			}
			public override ScrollArgs CalcHScrollArgs() {
				ScrollArgs args = new ScrollArgs();
				args.Maximum = ViewInfo.BestColumnWidth - 1;
				args.SmallChange = ListBox.HorzScrollStep;
				args.LargeChange = ViewInfo.ContentRect.Width;
				args.Value = LeftCoord;
				return args;
			}
			public override ScrollArgs CalcVScrollArgs() {
				ScrollArgs args = new ScrollArgs();
				int scrollRange = Math.Max(0, ItemCount - 1);
				args.Maximum = scrollRange;
				args.LargeChange = ViewInfo.CalcVisibleItemCountFromBottom(scrollRange);
				args.Value = TopIndex; 
				return args;
			}
			public override void MakeItemVisible(int index) {
				if(index < TopIndex) TopIndex = index;
				int visibleItemCount = ViewInfo.CalcVisibleItemCountFromBottom(index);
				if(index >= TopIndex + visibleItemCount)
					TopIndex = index - visibleItemCount + 1;
				else
					ListBox.LayoutChanged();
			}
			public override int GetWheelChange() {
				return SystemInformation.MouseWheelScrollLines;
			}
			public override int CalcKeyDownDelta(KeyEventArgs args) {
				int cx = 0;
				Keys keyCode = args.KeyCode;
				if((args.KeyData & Keys.Control) != 0) {
					if(keyCode == Keys.PageDown)
						keyCode = Keys.End;
					if(keyCode == Keys.PageUp)
						keyCode = Keys.Home;
				}
				switch(keyCode) {
					case Keys.Prior:
						if(ListBox.ScrollInfo.VScroll.ActualVisible) {
							cx = -ListBox.ScrollInfo.VScrollLargeChange;
						}
						else {
							cx = -ListBox.ItemCount; ;
						}
						break;
					case Keys.Next:
						if(ListBox.ScrollInfo.VScroll.ActualVisible) {
							cx = ListBox.ScrollInfo.VScrollLargeChange;
						}
						else {
							cx = ListBox.ItemCount;
						}
						break;
					case Keys.End:
						cx = ListBox.ItemCount - ListBox.SelectedIndex;
						break;
					case Keys.Home:
						cx = -ListBox.SelectedIndex;
						break;
					case Keys.Left:
						cx = -1;
						break;
					case Keys.Up:
						cx = -1;
						break;
					case Keys.Right:
						cx = 1;
						break;
					case Keys.Down:
						cx = 1;
						break;
				}
				return cx;
			}
			public override int CalcOnTimerDelta(Point ptMouse) {
				int dx = 0;
				if(ptMouse.Y < ViewInfo.ContentRect.Top) dx = -1;
				if(ptMouse.Y >= ViewInfo.ContentRect.Bottom) dx = 1;
				return dx;
			}
			public override Point GetHotTrackPoint(Point pt) {
				if(ListBox.ViewInfo.ContentRect.Contains(pt)) return pt;
				if(!ListBox.MultiColumn && (pt.Y >= ListBox.ViewInfo.ContentRect.Top) && (pt.Y < ListBox.ViewInfo.ContentRect.Bottom)) {
					pt.X = ListBox.ViewInfo.ContentRect.Left + 1;
					return pt;
				}
				return pt;
			}
			public override int CalcScrollTimerInterval(Point hotPt) {
				int distance = 0;
				if(hotPt.Y < ViewInfo.ContentRect.Top)
					distance = ViewInfo.ContentRect.Top - hotPt.Y;
				if(hotPt.Y >= ViewInfo.ContentRect.Bottom)
					distance = hotPt.Y - ViewInfo.ContentRect.Bottom;
				return (distance > ViewInfo.ItemHeight) ? ListBoxControlHandler.ListBoxControlState.HiScrollTimerInterval : ListBoxControlHandler.ListBoxControlState.LowScrollTimerInterval;
			}
		}
		public class MultiColumnModel : ListBoxModel {
			public MultiColumnModel(BaseListBoxControl listBox) : base(listBox) { }
			public override int GetMaxTopIndex() {
				if(ItemCount == 0) return 0;
				return Math.Max(0, ListBox.ViewInfo.ItemsPerColumn * (ListBox.ViewInfo.ColumnCount - ListBox.ViewInfo.VisibleColumnCount));
			}
			public override void OnHScroll(int scrollPos) {
				TopIndex = scrollPos * ViewInfo.ItemsPerColumn;
			}
			public override void OnVScroll(int scrollPos) {
				TopIndex = ViewInfo.GetItemColumnIndex(TopIndex) * ViewInfo.ItemsPerColumn + scrollPos;
			}
			public override ScrollArgs CalcHScrollArgs() {
				ScrollArgs args = new ScrollArgs();
				args.Maximum = Math.Max(0, ViewInfo.ColumnCount - 1);
				args.SmallChange = 1;
				args.LargeChange = ViewInfo.VisibleColumnCount;
				args.Value = ViewInfo.ItemsPerColumn != 0 ? TopIndex / ViewInfo.ItemsPerColumn : 0;
				ListBox.SetTopIndexCore(args.Value * ViewInfo.ItemsPerColumn);
				return args;
			}
			public override ScrollArgs CalcVScrollArgs() {
				ScrollArgs args = new ScrollArgs();
				int scrollRange = Math.Max(0, ViewInfo.ItemsPerColumn - 1);
				args.Maximum = scrollRange;
				args.LargeChange = ViewInfo.ContentRect.Height / ViewInfo.ItemHeight;
				args.Value = Math.Abs(ViewInfo.GetItemColumnIndex(TopIndex) * ViewInfo.ItemsPerColumn - TopIndex);
				return args;
			}
			public override void MakeItemVisible(int index) {
				if(index < TopIndex)
					TopIndex = ViewInfo.GetItemColumnIndex(index) * ViewInfo.ItemsPerColumn;
				if(index >= TopIndex + ViewInfo.VisibleColumnCount * ViewInfo.ItemsPerColumn) {
					int columnIndex = ViewInfo.GetItemColumnIndex(index) - ViewInfo.VisibleColumnCount + 1;
					TopIndex = columnIndex * ViewInfo.ItemsPerColumn;
				}
				else {
					ListBox.LayoutChanged();
				}
			}
			public override int GetWheelChange() {
				return ListBox.ViewInfo.ItemsPerColumn;
			}
			public override int CalcKeyDownDelta(KeyEventArgs args) {
				int cx = 0;
				switch(args.KeyCode) {
					case Keys.Prior:
						cx = -Math.Min(ListBox.ItemCount, ListBox.ViewInfo.VisibleColumnCount * ListBox.ViewInfo.ItemsPerColumn);
						break;
					case Keys.Next:
						cx = Math.Min(ListBox.ItemCount, ListBox.ViewInfo.VisibleColumnCount * ListBox.ViewInfo.ItemsPerColumn);
						break;
					case Keys.End:
						cx = ListBox.ItemCount - ListBox.SelectedIndex;
						break;
					case Keys.Home:
						cx = -ListBox.SelectedIndex;
						break;
					case Keys.Left:
						cx = ListBox.IsRightToLeft ? ListBox.ViewInfo.ItemsPerColumn : -ListBox.ViewInfo.ItemsPerColumn;
						break;
					case Keys.Up:
						cx = -1;
						break;
					case Keys.Right:
						cx = ListBox.IsRightToLeft ? -ListBox.ViewInfo.ItemsPerColumn : ListBox.ViewInfo.ItemsPerColumn;
						break;
					case Keys.Down:
						cx = 1;
						break;
				}
				return cx;
			}
			public override int CalcOnTimerDelta(Point ptMouse) {
				int dx = 0;
				if(ptMouse.X < ViewInfo.ContentRect.Left) {
					if(ListBox.IsRightToLeft && ListBox.TopIndex < GetMaxTopIndex()) dx = 1;
					if(!ListBox.IsRightToLeft && (ListBox.TopIndex != 0)) dx = -1;
				}
				if((ptMouse.X >= ViewInfo.ContentRect.Right)) {
					if(ListBox.IsRightToLeft && (ListBox.TopIndex != 0)) dx = -1;
					if(!ListBox.IsRightToLeft && ListBox.TopIndex < GetMaxTopIndex()) dx = 1;
				}
				dx *= ListBox.ViewInfo.ItemsPerColumn;
				return dx;
			}
			public override Point GetHotTrackPoint(Point pt) {
				if(ListBox.ViewInfo.ContentRect.Contains(pt)) return pt;
				if((pt.X >= ViewInfo.ContentRect.Left) && (pt.X < ViewInfo.ContentRect.Right)) {
					if(!ListBox.IsAdvHotTrackEnabled()) {
						if(ViewInfo.HotItemIndex != -1) {
							BaseListBoxViewInfo.ItemInfo item = ViewInfo.GetItemByIndex(ViewInfo.HotItemIndex);
							if(item != null) pt.Y = item.Bounds.Top + 1;
						}
						else {
							pt.Y = ViewInfo.ContentRect.Top + 1;
						}
					}
					return pt;
				}
				if(((pt.Y >= ViewInfo.ContentRect.Top) && (pt.Y < ViewInfo.ContentRect.Bottom))) {
					if(ViewInfo.FocusedItemIndex != -1 && !ListBox.IsAdvHotTrackEnabled()) {
						pt.X = ViewInfo.GetItemByIndex(ViewInfo.FocusedItemIndex).Bounds.Left + 1;
					}
					return pt;
				}
				return pt;
			}
			public override int CalcScrollTimerInterval(Point hotPt) {
				int distance = 0;
				if(hotPt.X < ViewInfo.ContentRect.Left) {
					distance = ViewInfo.ContentRect.Left - hotPt.X;
				}
				if(hotPt.X >= ViewInfo.ContentRect.Right) {
					distance = hotPt.X - ViewInfo.ContentRect.Right;
				}
				return (distance > ViewInfo.ColumnWidth) ? ListBoxControlHandler.ListBoxControlState.HiScrollTimerInterval : ListBoxControlHandler.ListBoxControlState.LowScrollTimerInterval;
			}
		}
		#endregion
		LoadingAnimator loadingAnimator;
		LoadingAnimator LoadingAnimator {
			get {
				if(loadingAnimator == null) {
					loadingAnimator = new LoadingAnimator(this, LoadingAnimator.LoadingImageLine);
				}
				return loadingAnimator;
			}
		}
		protected void CheckStopAnimation() {
			if(this.loadingAnimator == null || !LoadingAnimator.AnimationInProgress) return;
			foreach(DevExpress.XtraEditors.ViewInfo.BaseListBoxViewInfo.ItemInfo itemInfo in ViewInfo.VisibleItems) {
				if(AsyncServerModeDataController.IsNoValue(itemInfo.Item)) return;
			}
			LoadingAnimator.StopAnimation();
		}
		protected override void OnPaint(PaintEventArgs e) {
			if(firstPaint) {
				firstPaint = false;
				OnLoaded();
			}
			base.OnPaint(e);
			CheckStopAnimation();
		}
		protected internal virtual void OnActionItemClick(ListItemActionInfo action) {
		}
		protected internal virtual bool HasItemActions { get { return false; } } 
		protected internal virtual void CreateItemActions(BaseListBoxViewInfo.ItemInfo itemInfo) {
			itemInfo.ActionInfo = new ListItemActionCollection();
			itemInfo.ActionInfo.Add(new ListItemActionInfo(itemInfo));
		}
		#region IGestureClient Members
		GestureHelper gestureHelper;
		GestureHelper GestureHelper {
			get {
				if(gestureHelper == null) gestureHelper = new GestureHelper(this);
				return gestureHelper;
			}
		}
		protected override void WndProc(ref Message m) {
			if(GestureHelper.WndProc(ref m)) return;
			base.WndProc(ref m);
		}
		GestureAllowArgs[] IGestureClient.CheckAllowGestures(Point point) {
			if(this.ScrollInfo == null || !ScrollInfo.VScrollVisible) return GestureAllowArgs.None;
			if(!ScrollInfo.VScroll.Enabled) return GestureAllowArgs.None;
			return new GestureAllowArgs[] { GestureAllowArgs.PanVertical };
		}
		IntPtr IGestureClient.Handle { get { return IsHandleCreated ? Handle : IntPtr.Zero; } }
		void IGestureClient.OnEnd(GestureArgs info) { }
		void IGestureClient.OnBegin(GestureArgs info) { }
		int overPanY = 0;
		int startY = 0;
		void IGestureClient.OnPan(GestureArgs info, Point delta, ref Point overPan) {
			if(info.IsBegin) {
				overPanY = 0;
				startY = ScrollInfo.VScrollPosition;
				return;
			}
			if(delta.Y == 0) return;
			int prevPos = ScrollInfo.VScrollPosition;
			int newPos = startY - (info.Current.Y - info.Start.Y) / ViewInfo.ItemHeight;
			ScrollInfo.VScroll.Value = newPos;
			if(newPos != prevPos && ScrollInfo.VScrollPosition == prevPos) {
				overPanY += delta.Y;
			}
			else { overPanY = 0; }
			overPan.Y = overPanY;
		}
		void IGestureClient.OnTwoFingerTap(GestureArgs info) { }
		void IGestureClient.OnPressAndTap(GestureArgs info) {
		}
		void IGestureClient.OnRotate(GestureArgs info, Point center, double degreeDelta) { }
		void IGestureClient.OnZoom(GestureArgs info, Point center, double zoomDelta) { }
		IntPtr IGestureClient.OverPanWindowHandle { get { return GestureHelper.FindOverpanWindow(this); } }
		Point IGestureClient.PointToClient(Point p) { return PointToClient(p); }
		#endregion
		protected internal virtual bool GetAllowHtmlDraw() {
			if(AllowHtmlDraw == DefaultBoolean.Default) return XPaint.DefaultAllowHtmlDraw;
			return AllowHtmlDraw == DefaultBoolean.True ? true : false;
		}
		protected virtual bool CanShowToolTipForTrimmedText {
			get {
				if(!string.IsNullOrEmpty(ToolTip) || SuperTip != null) return false;
				if(ShowToolTipForTrimmedText == DefaultBoolean.Default) return BaseListBoxControl.DefaultShowToolTipForTrimmedText;
				return ShowToolTipForTrimmedText == DefaultBoolean.True ? true : false;
			}
		}
		protected override ToolTipControlInfo GetToolTipInfo(Point point) {
			 BaseListBoxViewInfo.ItemInfo item = ViewInfo.GetItemInfoByPoint(point);
			 if(item != null) {
				 ToolTipControlInfo contextBtnInfo = item.ContextButtonsViewInfo.GetToolTipInfo(point);
				 if(contextBtnInfo != null) return contextBtnInfo;
				 if(CanShowToolTipForTrimmedText) {
					 if(!ViewInfo.IsItemTextFit(item)) {
						 ToolTipControlInfo res = new ToolTipControlInfo(item, (ViewInfo.GetHtmlDrawText() && AllowHtmlTextInToolTip == DefaultBoolean.True) ? item.Text : StringPainter.Default.RemoveFormat(item.Text));
						 res.AllowHtmlText = AllowHtmlTextInToolTip;
						 return res;
					 }
				 }
			 }
			return base.GetToolTipInfo(point);
		}
		protected class ListControlCustomBindingPropertiesAttribute : DevExpress.Utils.Design.DataAccess.CustomBindingPropertiesAttribute {
			protected string ControlName;
			public ListControlCustomBindingPropertiesAttribute(string controlName) {
				this.ControlName = controlName;
			}
			public override IEnumerable<DevExpress.Utils.Design.DataAccess.ICustomBindingProperty> GetCustomBindingProperties() {
				return new DevExpress.Utils.Design.DataAccess.ICustomBindingProperty[] {
						new CustomBindingPropertyAttribute("DisplayMember", "Display Member", GetDisplayMemberDescription()),
						new CustomBindingPropertyAttribute("ValueMember", "Value Member", GetValueMemberDescription())
					};
			}
			protected virtual string GetValueMemberDescription() {
				return string.Format("Gets or sets the field name in the bound data source whose contents are assigned to item values of the {0}.", ControlName);
			}
			protected virtual string GetDisplayMemberDescription() {
				return string.Format("Gets or sets a field name in the bound data source whose contents are to be displayed by the {0}.", ControlName);
			}
		}
		ContextItemCollection contextButtons;
		[Editor("DevExpress.XtraEditors.Design.SimpleContextItemCollectionUITypeEditor, " + AssemblyInfo.SRAssemblyEditorsDesign, typeof(System.Drawing.Design.UITypeEditor)), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ContextItemCollection ContextButtons {
			get {
				if(contextButtons == null) {
					contextButtons = CreateContextButtonsCollection();
					contextButtons.Options = ContextButtonOptions;
				}
				return contextButtons;
			}
		}
		SimpleContextItemCollectionOptions contextButtonOptions;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), TypeConverter(typeof(ExpandableObjectConverter))]
		public virtual SimpleContextItemCollectionOptions ContextButtonOptions {
			get {
				if(contextButtonOptions == null) {
					contextButtonOptions = CreateContextButtonOptions();
				}
				return contextButtonOptions;
			}
		}
		protected virtual SimpleContextItemCollectionOptions CreateContextButtonOptions() {
			return new SimpleContextItemCollectionOptions(this);
		}
		ContextItem IContextItemProvider.CreateContextItem(Type type) {
			ContextItem item = CreateContextItemCore(type);
			if(item == null) return null;
			item.Name = item.GetType().Name;
			return item;
		}
		ContextItem CreateContextItemCore(Type type) {
			if(type == typeof(ContextButton)) return new SimpleContextButton();
			if(type == typeof(RatingContextButton)) return new SimpleRatingContextButton();
			if(type == typeof(CheckContextButton)) return new SimpleCheckContextButton();
			if(type == typeof(TrackBarContextButton)) return new SimpleTrackBarContextButton();
			return null;
		}
		protected virtual ContextItemCollection CreateContextButtonsCollection() {
			return new ContextItemCollection(this);
		}
		bool IContextItemCollectionOwner.IsDesignMode {
			get { return IsDesignMode; }
		}
		bool IContextItemCollectionOwner.IsRightToLeft {
			get { return IsRightToLeft; }
		}
		void IContextItemCollectionOwner.OnCollectionChanged() {
			OnPropertiesChanged();
		}
		void IContextItemCollectionOwner.OnItemChanged(ContextItem item, string propertyName, object oldValue, object newValue) {
			OnPropertiesChanged();
		}
		ContextAnimationType IContextItemCollectionOptionsOwner.AnimationType {
			get { return ContextAnimationType.OpacityAnimation; }
		}
		void IContextItemCollectionOptionsOwner.OnOptionsChanged(string propertyName, object oldValue, object newValue) {
			OnPropertiesChanged();
		}
		#region ISearchControlClient Members
		ISearchControl searchControl;
		protected virtual bool IsAttachedToSearchControl { get { return searchControl != null; } }
		void ISearchControlClient.ApplyFindFilter(SearchInfoBase searchInfo) {
			BeginUpdate();
			SearchCriteriaInfo infoArgs = searchInfo as SearchCriteriaInfo;
			DevExpress.Data.Filtering.CriteriaOperator criteriaOperator = infoArgs != null ? infoArgs.CriteriaOperator : null;
			isSearchingCore++;
			ApplyItemsFilter(criteriaOperator);
			if(!IsBoundMode)
				ViewInfo.ResetItemSizesCache();
			isSearchingCore--;
			EndUpdate();
		}
		protected virtual void ApplyItemsFilter(DevExpress.Data.Filtering.CriteriaOperator criteriaOperator) {
			using(SelectionContext context = new SelectionContext(this)) {
				if(IsBoundMode) {
					if(DataAdapter.IsDisposed) return;
					DataAdapter.FilterCriteria = criteriaOperator;
				}
				else {
					if(!CanFilterItems) return;
					SourceItems.FilterCriteria = criteriaOperator;
				}
			}
		}
		SearchControlProviderBase ISearchControlClient.CreateSearchProvider() {
			return CreateSearchControlProviderBase();
		}
		protected virtual SearchControlProviderBase CreateSearchControlProviderBase() {
			return new ListBoxCriteriaProvider(this);
		}
		void ISearchControlClient.SetSearchControl(ISearchControl searchControl) {
			if(this.searchControl == searchControl) return;
			this.searchControl = searchControl;
			ApplyItemsFilter(null);
			if(CanFilterItems)
				SourceItems.AdapterEnabled = IsAttachedToSearchControl;
		}
		bool ISearchControlClient.IsAttachedToSearchControl { get { return searchControl != null; } }
		protected virtual bool CanFilterItems { get { return SourceItems != null; } }
		ISupportSearchDataAdapter SourceItems { get { return ItemsCore as ISupportSearchDataAdapter; } }
		int isSearchingCore = 0;
		protected virtual bool IsSearching { get { return isSearchingCore > 0; } }
		#endregion
		class SelectionContext : IDisposable {
			BaseListBoxControl listBoxCore;
			bool isDisposing;
			int saveSelectedCount;
			public SelectionContext(BaseListBoxControl listBox) {
				this.listBoxCore = listBox;
				SaveSelectedIndices();
			}
			void BeginUpdate() {
				this.listBoxCore.SelectedIndices.BeginUpdate();
			}
			void EndUpdate() {
				this.listBoxCore.SelectedIndices.EndUpdate();
			}
			void CancelUpdate() {
				this.listBoxCore.SelectedIndices.CancelUpdate();
			}
			ISupportSelectionDataAdapter Selection { get { return listBoxCore.SourceItems as ISupportSelectionDataAdapter; } }
			void BeginSelection(int[] indices) {
				if(listBoxCore.IsBoundMode) {
					listBoxCore.DataAdapter.Selection.BeginSelection();
					foreach(int index in indices)
						listBoxCore.DataAdapter.Selection.SetSelected(index, true);
					return;
				}
				if(Selection != null) {
					Selection.BeginSelection();
					Selection.SetSelected(indices);
				}
			}
			void SaveSelectedIndices() {
				BeginUpdate();
				int[] indices = new int[listBoxCore.SelectedIndices.Count];
				listBoxCore.SelectedIndices.CopyTo(indices, 0);
				listBoxCore.SelectedIndices.Clear();
				BeginSelection(indices);
				saveSelectedCount = indices.Length;
			}
			#region IDisposable Members
			void IDisposable.Dispose() {
				if(!isDisposing) {
					isDisposing = true;
					OnDispose();
				}
			}
			int[] CancelSelection() {
				int[] indices = null;
				if(listBoxCore.IsBoundMode) {
					indices = listBoxCore.DataAdapter.Selection.GetSelectedRows();
					listBoxCore.DataAdapter.Selection.Clear();
					listBoxCore.DataAdapter.Selection.CancelSelection();
				}
				else {
					if(Selection != null) {
						indices = Selection.GetSelectedIndices();
						Selection.CancelSelection();
					}
				}
				return indices;
			}
			void RestoreSelectedIndex() {
				listBoxCore.SelectedIndices.Clear();
				int[] indices = CancelSelection();
				if(indices != null) {
					foreach(int index in indices)
						listBoxCore.SelectedIndices.AddRemove(index);
				}
			}
			void OnDispose() {
				RestoreSelectedIndex();
				if(listBoxCore.SelectedIndices.Count == saveSelectedCount)
					CancelUpdate();
				else
					EndUpdate();
			}
			#endregion
		}
		public void AddEnum(Type enumType, bool addEnumeratorIntegerValues) {
			BeginUpdate();
			try {
				Array values = EnumDisplayTextHelper.GetEnumValues(enumType);
				foreach(object obj in values) {
					object value = EnumDisplayTextHelper.GetEnumValue(addEnumeratorIntegerValues, obj, enumType);
					items.AddItem(value, EnumDisplayTextHelper.GetDisplayText(obj));
				}
			}
			finally { EndUpdate(); }
		}
		public void AddEnum(Type enumType) {
			AddEnum(enumType, false);
		}
		public void AddEnum<TEnum>() {
			AddEnum<TEnum>(null);
		}
		public void AddEnum<TEnum>(Converter<TEnum, string> displayTextConverter) {
			if(displayTextConverter == null)
				displayTextConverter = (v) => EnumDisplayTextHelper.GetDisplayText(v);
			BeginUpdate();
			try {
				var values = EnumDisplayTextHelper.GetEnumValues<TEnum>();
				foreach(TEnum value in values)
					items.AddItem(value, displayTextConverter(value));
			}
			finally { EndUpdate(); }
		}
	}
	#region ListBoxScrollInfo
	public class ListBoxScrollInfo : IDisposable {
		BaseListBoxControl container;
		HScrollBar hScroll;
		VScrollBar vScroll;
		Rectangle clientRect;
		Rectangle hscrollRect, vscrollRect;
		bool hScrollVisible, vScrollVisible;
		int lockLayout;
		bool isRightToLeft;
		public ListBoxScrollInfo(BaseListBoxControl container) {
			this.container = container;
			this.lockLayout = 0;
			this.clientRect = this.hscrollRect = this.vscrollRect = Rectangle.Empty;
			this.hScroll = CreateHScroll();
			this.vScroll = CreateVScroll();
			this.HScroll.Visible = false;
			this.VScroll.Visible = false;
			this.VScroll.SmallChange = this.HScroll.SmallChange = 1;
			this.HScroll.LargeChange = this.VScroll.LargeChange = 1;
			this.HScroll.Anchor = this.VScroll.Anchor = AnchorStyles.None;
			this.HScroll.Value = this.HScroll.Maximum = this.VScroll.Value = this.VScroll.Maximum = 0;
			this.hScrollVisible = this.vScrollVisible = false;
			this.HScroll.LookAndFeel.ParentLookAndFeel = container.LookAndFeel;
			this.VScroll.LookAndFeel.ParentLookAndFeel = container.LookAndFeel;
			ScrollBarBase.ApplyUIMode(VScroll);
			ScrollBarBase.ApplyUIMode(HScroll);
			this.isRightToLeft = false;
		}
		public virtual int HScrollHeight {
			get { return HScroll.GetDefaultHorizontalScrollBarHeight(); }
		}
		public virtual int VScrollWidth {
			get { return VScroll.GetDefaultVerticalScrollBarWidth(); }
		}
		protected internal bool IsOverlapScrollbar { get { return VScroll.IsOverlapScrollBar || HScroll.IsOverlapScrollBar; } }
		public BaseListBoxControl Container { get { return container; } }
		public BaseListBoxViewInfo ViewInfo { get { return container.ViewInfo; } }
		public virtual HScrollBar HScroll { get { return hScroll; } }
		public virtual VScrollBar VScroll { get { return vScroll; } }
		protected virtual HScrollBar CreateHScroll() { return new HScrollBar(); }
		protected virtual VScrollBar CreateVScroll() { return new VScrollBar(); }
		public virtual void AddControls(Control container) {
			if(container == null) return;
			container.Controls.Add(HScroll);
			container.Controls.Add(VScroll);
		}
		public virtual void RemoveControls(Control container) {
			if(container == null) return;
			container.Controls.Remove(HScroll);
			container.Controls.Remove(VScroll);
		}
		public virtual void Dispose() {
			if(HScroll != null) HScroll.Dispose();
			if(VScroll != null) VScroll.Dispose();
		}
		public ScrollArgs HScrollArgs {
			get { return new ScrollArgs(HScroll); }
			set { value.AssignTo(HScroll); }
		}
		public ScrollArgs VScrollArgs {
			get { return new ScrollArgs(VScroll); }
			set { value.AssignTo(VScroll); }
		}
		public int HScrollPosition { get { return HScroll.Value; } }
		public int VScrollPosition { get { return VScroll.Value; } }
		public Rectangle HScrollRect { get { return hscrollRect; } }
		public Rectangle VScrollRect { get { return vscrollRect; } }
		public int HScrollMaximum { get { return HScroll.Maximum; } }
		public int VScrollMaximum { get { return VScroll.Maximum; } }
		public int HScrollLargeChange { get { return HScroll.LargeChange; } }
		public int VScrollLargeChange { get { return VScroll.LargeChange; } }
		public bool HScrollVisible {
			get { return hScrollVisible; }
			set {
				if(HScrollVisible == value) UpdateVisibility();
				else {
					hScrollVisible = value;
					LayoutChanged();
				}
			}
		}
		public bool VScrollVisible {
			get { return vScrollVisible; }
			set {
				if(VScrollVisible == value) UpdateVisibility();
				else {
					vScrollVisible = value;
					LayoutChanged();
				}
			}
		}
		public Rectangle ClientRect {
			get { return clientRect; }
			set {
				if(ClientRect == value) return;
				clientRect = value;
				LayoutChanged();
			}
		}
		public bool IsRightToLeft {
			get {
				return isRightToLeft;
			}
			set {
				if(isRightToLeft == value) return;
				isRightToLeft = value;
				LayoutChanged();
			}
		}
		protected virtual void CalcRects() {
			hscrollRect = vscrollRect = Rectangle.Empty;
			Rectangle r = Rectangle.Empty;
			if(HScrollVisible) {
				int x = ClientRect.X;
				if(IsRightToLeft && VScrollVisible) x += VScrollWidth;
				r.Location = new Point(x, ClientRect.Bottom - HScrollHeight);
				r.Size = new Size((ClientRect.Width - (VScrollVisible ? VScrollWidth : 0)), HScrollHeight);
				hscrollRect = r;
			}
			if(VScrollVisible) {
				int x = ClientRect.X;
				if(!IsRightToLeft) x = ClientRect.Right - VScrollWidth;
				r.Location = new Point(x, ClientRect.Y);
				r.Size = new Size(VScrollWidth, ClientRect.Height - (HScrollVisible ? HScrollHeight : 0));
				vscrollRect = r;
			}
		}
		[Obsolete("Use UpdateVisibility"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void UpdateVisiblity() {
			UpdateVisibility();
		}
		public void UpdateVisibility() {
			HScroll.SetVisibility(hScrollVisible && !ClientRect.IsEmpty);
			VScroll.SetVisibility(vScrollVisible && !ClientRect.IsEmpty);
			HScroll.Bounds = HScrollRect;
			VScroll.Bounds = VScrollRect;
		}
		public virtual void LayoutChanged() {
			if(lockLayout != 0) return;
			lockLayout++;
			try {
				CalcRects();
				UpdateVisibility();
				if(ClientRect.IsEmpty) {
					VScroll.SetVisibility(false);
					HScroll.SetVisibility(false);
				}
			}
			finally {
				lockLayout--;
			}
		}
		public event ScrollEventHandler HScroll_Scroll {
			add { HScroll.Scroll += value; }
			remove { HScroll.Scroll -= value; }
		}
		public event EventHandler HScroll_ValueChanged {
			add { HScroll.ValueChanged += value; }
			remove { HScroll.ValueChanged -= value; }
		}
		public event ScrollEventHandler VScroll_Scroll {
			add { VScroll.Scroll += value; }
			remove { VScroll.Scroll -= value; }
		}
		public event EventHandler VScroll_ValueChanged {
			add { VScroll.ValueChanged += value; }
			remove { VScroll.ValueChanged -= value; }
		}
		internal void OnAction(ScrollNotifyAction action) {
			VScroll.OnAction(action);
			HScroll.OnAction(action);
		}
	}
	#endregion
	#region ListBoxControlHandler
	public class ListBoxControlHandler : IDisposable {
		public enum HandlerState { Default, Unselectable, SingleSelect, MultiSimpleSelect, ExtendedMultiSimpleSelect };
		BaseListBoxControl owner;
		ListBoxControlState controlState;
		public ListBoxControlHandler(BaseListBoxControl owner) {
			this.owner = owner;
			this.controlState = CreateState(HandlerState.Default);
			ControlState.Init();
			LastHotItem = null;
		}
		public BaseListBoxControl OwnerControl { get { return owner; } }
		protected internal ListBoxControlState ControlState { 
			get { return controlState; }
		}
		protected virtual BaseListBoxViewInfo.ItemInfo LastHotItem { get; set; }
		public HandlerState State { get { return ControlState.State; } }
		public string CurrentSearch { get { return ControlState.CurrentSearch; } }
		public virtual bool IsControlPressed { get { return ((Control.ModifierKeys & Keys.Control) != Keys.None); } }
		public virtual bool IsShiftPressed { get { return ((Control.ModifierKeys & Keys.Shift) != Keys.None); } }
		protected virtual ListBoxControlState CreateState(HandlerState state) {
			ListBoxControlState result = null;
			switch(state) {
				case HandlerState.Default:
				case HandlerState.SingleSelect:
					result = new SingleSelectState(this);
					break;
				case HandlerState.MultiSimpleSelect:
					result = new MultiSimpleSelectState(this);
					break;
				case HandlerState.ExtendedMultiSimpleSelect:
					result = new ExtendedMultiSelectState(this);
					break;
				case HandlerState.Unselectable:
					result = new UnselectableState(this);
					break;
			}
			return result;
		}
		protected internal void SetHandlerState(HandlerState state) {
			if(state == State) return;
			ListBoxControlState newState = CreateState(state);
			ControlState.Dispose();
			this.controlState = newState;
			ControlState.Init();
		}
		public void Dispose() { ControlState.Dispose(); }
		public virtual void OnMouseDown(MouseEventArgs e) {
			BaseListBoxViewInfo.ItemInfo itemInfo = OwnerControl.ViewInfo.GetItemInfoByPoint(e.Location);
			if(itemInfo != null)
				if(itemInfo.OnMouseDown(e)) return;
			ControlState.MouseDown(e); 
		}
		public virtual void OnMouseMove(MouseEventArgs e) {
			ContextButtonsMouseMove(e);
				LastHotItem = OwnerControl.ViewInfo.GetItemInfoByPoint(e.Location);
			ControlState.MouseMove(e);
		}
		void ContextButtonsMouseMove(MouseEventArgs e) {
			BaseListBoxViewInfo.ItemInfo prevInfo = LastHotItem;
			BaseListBoxViewInfo.ItemInfo itemInfo = OwnerControl.ViewInfo.GetItemInfoByPoint(e.Location);
			if(prevInfo != null && prevInfo != itemInfo) {
				prevInfo.OnMouseLeave(new EventArgs());
			}
			if(itemInfo != null) {
				if(prevInfo != itemInfo)
					itemInfo.OnMouseEnter(new EventArgs());
				itemInfo.OnMouseMove(e);
			}
		}
		public virtual void OnMouseUp(MouseEventArgs e) {
			BaseListBoxViewInfo.ItemInfo itemInfo = OwnerControl.ViewInfo.GetItemInfoByPoint(e.Location);
			if(itemInfo != null) 
				if(itemInfo.OnMouseUp(e)) return;
			ControlState.MouseUp(e); 
		}
		public virtual void OnMouseWheel(DXMouseEventArgs e) { ControlState.MouseWheel(e); }
		public virtual void OnKeyDown(KeyEventArgs e) { ControlState.KeyDown(e); }
		public virtual void OnKeyUp(KeyEventArgs e) { ControlState.KeyUp(e); }
		public virtual void OnKeyPress(KeyPressEventArgs e) { ControlState.KeyPress(e); }
		public virtual void OnMouseEnter() { ControlState.MouseEnter(); }
		public virtual void OnMouseLeave() { ControlState.MouseLeave(); }
		public virtual void OnSetSelectedIndex(int newIndex) {
			ControlState.SetSelectedIndex(newIndex);
		}
		public virtual void OnSetSelected(int index, bool value) { ControlState.SetSelected(index, value); }
		public virtual void OnLostCapture() { ControlState.LostCapture(); }
		public virtual void OnLostFocus() { ControlState.LostFocus(); }
		public virtual void OnGotFocus() { ControlState.GotFocus(); }
		public virtual void OnListChanged() { ControlState.ListChanged(); }
		public virtual void OnIncrementalSearchChanged() { ControlState.IncrementalSearchChanged(); }
		#region ListBoxControl states
		public abstract class ListBoxControlState : IDisposable {
			public static int HiScrollTimerInterval = 30;
			public static int LowScrollTimerInterval = 120;
			ListBoxControlHandler handler;
			protected Timer scrollTimer;
			protected internal int pressedItemIndex;
			protected int focusedIndexCore;
			protected Point lastHotPoint = Point.Empty; 
			protected internal ListBoxSearch search;
			public ListBoxControlState(ListBoxControlHandler handler) {
				this.handler = handler;
				this.scrollTimer = new Timer();
				this.scrollTimer.Interval = LowScrollTimerInterval;
				this.scrollTimer.Tick += new EventHandler(OnTimer);
				this.search = new ListBoxSearch(ListBox, this);
			}
			protected BaseListBoxControl ListBox { get { return handler.OwnerControl; } }
			protected ListBoxControlHandler Handler { get { return handler; } }
			protected bool IncrementalSearch { get { return ListBox.IncrementalSearch; } }
			public string CurrentSearch { get { return search.CurrentSearch; } }
			internal void SetCurrentSearch(string value) { search.CurrentSearch = value; }
			protected void StartScrollTimer() { if(ListBox.Capture) scrollTimer.Enabled = true; }
			protected void StopScrollTimer() { scrollTimer.Enabled = false; }
			public virtual void MouseDown(MouseEventArgs e) {
				UpdatePressedItemIndex(e);
				UpdateLastHotPoint(e);
				search.Reset();
				UpdateItemActionState(e);
			}
			bool UpdateItemActionState(MouseEventArgs e) {
				if(!ListBox.ViewInfo.UpdateItemActionState(e, ListBox.ViewInfo.HotItem)) return false; 
				ListBox.Invalidate();
				return true;
			}
			protected virtual void UpdatePressedItemIndex(MouseEventArgs e) {
				pressedItemIndex = -1;
				if(!ListBox.ViewInfo.ContentRect.Contains(new Point(e.X, e.Y))) return;
				if(((e.Button & MouseButtons.Left) != MouseButtons.None) && ListBox.Capture) {
					BaseListBoxViewInfo.ItemInfo pressedItem = ListBox.ViewInfo.GetItemInfoByPoint(new Point(e.X, e.Y));
					if(pressedItem != null) pressedItemIndex = pressedItem.Index;
				}
			}
			protected void UpdateLastHotPoint(MouseEventArgs e) {
				if(!ListBox.ViewInfo.ContentRect.Contains(new Point(e.X, e.Y))) return;
				lastHotPoint = new Point(e.X, e.Y);
			}
			public virtual void MouseEnter() {
				if(FocusedIndex > -1)
					ListBox.ViewInfo.FocusedItemIndex = FocusedIndex; 
				if(ListBox.IsAdvHotTrackEnabled()) ListBox.ViewInfo.HotItemIndex = -1;
				ListBox.RecalcViewInfoState();
			}
			public virtual void MouseLeave() {
				if(ListBox.IsAdvHotTrackEnabled()) ListBox.ViewInfo.HotItemIndex = -1;
				ListBox.RecalcViewInfoState();
			}
			public virtual void LostCapture() {
				StopScrollTimer();
			}
			public virtual void LostFocus() {
				StopScrollTimer();
			}
			public virtual void MouseUp(MouseEventArgs e) {
				DXMouseEventArgs de = DXMouseEventArgs.GetMouseArgs(e);
				StopScrollTimer();
				if(UpdateItemActionState(new MouseEventArgs(MouseButtons.None, e.Clicks, e.X, e.Y, e.Delta))) {
					ListItemActionInfo aInfo = ListBox.ViewInfo.GetActiveActionInfo(de.Location);
					if(aInfo != null) {
						de.Handled = true;
						OnActionItemClick(aInfo);
					}
				}
			}
			protected virtual void OnActionItemClick(ListItemActionInfo action) {
				ListBox.OnActionItemClick(action);
			}
			public virtual void GotFocus() {
				focusedIndexCore = ListBox.SelectedIndex;
				if(focusedIndexCore > -1)
					ListBox.ViewInfo.FocusedItemIndex = focusedIndexCore;
				ListBox.LayoutChanged();
			}
			public virtual void MouseMove(MouseEventArgs e) {
				UpdateItemActionState(e);
			}
			public virtual void MouseWheel(DXMouseEventArgs e) {
				int wheelChange = ListBox.Model.GetWheelChange();
				ListBox.TopIndex += (e.Delta > 0) ? -wheelChange : wheelChange;
			}
			public virtual void KeyUp(KeyEventArgs e) { }
			protected virtual void UpdateScrollTimerInterval(Point hotPt) {
				if(!scrollTimer.Enabled) return;
				scrollTimer.Interval = ListBox.Model.CalcScrollTimerInterval(hotPt);
			}
			protected virtual void SpaceKeyDown() { }
			protected virtual void EnterKeyDown() { }
			public virtual void KeyDown(KeyEventArgs e) {
				if(search.KeyDown(e.KeyCode, Handler.IsControlPressed))
					return;
				if(e.KeyCode == Keys.Space) {
					SpaceKeyDown();
					return;
				}
				if(e.KeyCode == Keys.Enter) {
					EnterKeyDown();
					return;
				}
				if(!ListBox.MultiColumn && ListBox.ScrollInfo.HScrollVisible) {
					switch(e.KeyCode) {
						case Keys.Left:
							ListBox.LeftCoord += ListBox.IsRightToLeft ? 1 : -1;
							return;
						case Keys.Right:
							ListBox.LeftCoord += ListBox.IsRightToLeft ? -1 : 1;
							return;
					}
				}
				Navigate(ListBox.Model.CalcKeyDownDelta(e));
			}
			public virtual void KeyPress(KeyPressEventArgs e) {
				if(e.Handled) return;
				search.KeyPress(e.KeyChar);				
			}
			public virtual void SetSelectedIndex(int newIndex) { }
			public virtual void ListChanged() {
				search.Reset();
			}
			public virtual void IncrementalSearchChanged() {
				search.Reset();
			}
			public virtual void Init() {
				pressedItemIndex = -1;
			}
			public virtual void Dispose() {
				scrollTimer.Tick -= new EventHandler(OnTimer);
				scrollTimer.Dispose();
			}
			protected virtual void OnTimer(object sender, EventArgs e) {
				Point ptMouse = ListBox.PointToClient(Cursor.Position);
				Navigate(ListBox.Model.CalcOnTimerDelta(ptMouse));
			}
			protected virtual void Navigate(int delta) {
				FocusedIndex = Math.Max(0, FocusedIndex + delta);
			}
			protected internal virtual int FocusedIndex { get { return focusedIndexCore; } set { focusedIndexCore = value; } }
			public abstract HandlerState State { get; }
			public virtual void SetSelected(int index, bool value) { }
		}		
		public class UnselectableState : ListBoxControlState {
			public UnselectableState(ListBoxControlHandler handler) : base(handler) { }
			public override HandlerState State { get { return HandlerState.Unselectable; } }
			public override void Init() {
				base.Init();
				focusedIndexCore = 0;
				ListBox.SelectedIndices.Clear();
			}
			protected internal override int FocusedIndex {
				get { return base.FocusedIndex; }
				set {
					if(value < 0) value = 0;
					if(value > ListBox.ItemCount - 1) value = ListBox.ItemCount - 1;
					if(FocusedIndex == value) return;
					focusedIndexCore = value;
					ListBox.ViewInfo.FocusedItemIndex = value;
					ListBox.MakeItemVisible(value);
				}
			}
			public override void MouseDown(MouseEventArgs e) {
				base.MouseDown(e);
				if(pressedItemIndex == -1) return;
				FocusedIndex = pressedItemIndex;
				StartScrollTimer();
			}
			public override void GotFocus() {
				ListBox.LayoutChanged();
			}
			public override void MouseMove(MouseEventArgs e) {
				if(!scrollTimer.Enabled) return;
				Point pt = ListBox.Model.GetHotTrackPoint(new Point(e.X, e.Y));
				UpdateScrollTimerInterval(pt);
				BaseListBoxViewInfo.ItemInfo hotItem = ListBox.ViewInfo.GetItemInfoByPoint(pt);
				if(hotItem == null || lastHotPoint.Equals(pt)) return;
				FocusedIndex = hotItem.Index;
				lastHotPoint = pt;
				base.MouseMove(e);
			}
			public override void SetSelectedIndex(int newIndex) { }
		}
		public class SingleSelectState : ListBoxControlState {
			int oldFocusedIndex;
			int oldHotItemIndex = -1;
			public SingleSelectState(ListBoxControlHandler handler) : base(handler) { }
			public override HandlerState State { get { return HandlerState.SingleSelect; } }
			protected virtual bool RaiseSelectedIndexChangedOnHotTrack { get { return ListBox.HotTrackItems == true && ListBox.HotTrackSelectMode == HotTrackSelectMode.SelectItemOnHotTrackEx; } }
			protected internal override int FocusedIndex {
				get { return ListBox.SelectedIndex; }
				set {
					if(value < 0) value = 0;
					if(value > ListBox.ItemCount - 1) value = ListBox.ItemCount - 1;
					if(FocusedIndex == value) return;
					ListBox.MakeItemVisible(value);
					SetFocusedIndexCore(value);
				}
			}
			protected virtual void SetFocusedIndexCore(int index) {
				if(isMouseDown) ListBox.LockSelectionChanged();
				try {
					ListBox.ViewInfo.FocusedItemIndex = index;
					SetSelectedIndex(index);
				}
				finally {
					if(isMouseDown) ListBox.UnLockSelectionChanged();
				}
			}
			public override void Init() {
				base.Init();
				int selIndex = ListBox.SelectedIndex;
				if(selIndex > 0 && ListBox.SelectedIndices.Count > 0) {
					ListBox.SelectedIndices.Clear();
					FocusedIndex = selIndex;
				}
			}
			bool isMouseDown = false;
			public override void MouseDown(MouseEventArgs e) {
				base.MouseDown(e);
				if(pressedItemIndex == -1) return;
				isMouseDown = true;
				ListBox.LockSelectionChanged();
				oldFocusedIndex = FocusedIndex;
				FocusedIndex = pressedItemIndex;
				StartScrollTimer();
			}
			public override void MouseUp(MouseEventArgs e) {
				DXMouseEventArgs de = DXMouseEventArgs.GetMouseArgs(e);
				base.MouseUp(de);
				if(pressedItemIndex == -1) return;
				isMouseDown = false;
				ListBox.UnLockSelectionChanged();
				if(!de.Handled) {
					if(oldFocusedIndex != FocusedIndex || (ListBox.HotTrackItems && oldHotItemIndex != FocusedIndex && !RaiseSelectedIndexChangedOnHotTrack)) {
						ListBox.OnSelectionChanged();
					}
				}
				if(ListBox.HotTrackItems) oldHotItemIndex = FocusedIndex;
			}
			protected override void Navigate(int delta) {
				if(!scrollTimer.Enabled && isMouseDown) isMouseDown = false;
				base.Navigate(delta);
			}
			public override void MouseMove(MouseEventArgs e) {
				if(!scrollTimer.Enabled && !ListBox.CanHotTrack) return;
				Point pt = ListBox.Model.GetHotTrackPoint(new Point(e.X, e.Y));
				UpdateScrollTimerInterval(pt);
				BaseListBoxViewInfo.ItemInfo hotItem = ListBox.ViewInfo.GetItemInfoByPoint(pt);
				if(hotItem == null || lastHotPoint.Equals(pt)) return;
				if(scrollTimer.Enabled) {
					FocusedIndex = hotItem.Index;
					if(ListBox.IsAdvHotTrackEnabled()) ListBox.ViewInfo.HotItemIndex = -1;
				}
				else {
					if(!ListBox.IsAdvHotTrackEnabled()) {
						if(!RaiseSelectedIndexChangedOnHotTrack)
							ListBox.SelectedIndices.BeginUpdate();
						ListBox.SelectedIndices.Set(hotItem.Index);
						if(!RaiseSelectedIndexChangedOnHotTrack)
							ListBox.SelectedIndices.CancelUpdate();
					}
					UpdateHotItem(hotItem);
				}
				lastHotPoint = pt;
				base.MouseMove(e);
			}
			internal void ResetFocusedIndex() {
				SetFocusedIndexCore(-1);
			}
			protected void UpdateHotItem(BaseListBoxViewInfo.ItemInfo hotItem) {
				if(hotItem == null) return;
				int oldFocusedIndex = ListBox.ViewInfo.HotItemIndex;
				ListBox.ViewInfo.HotItemIndex = -1;
				BaseListBoxViewInfo.ItemInfo oldHotItem = (oldFocusedIndex == -1) ? null : ListBox.ViewInfo.GetItemByIndex(oldFocusedIndex);
				if(oldHotItem != null) {
					ListBox.ViewInfo.UpdateItem(oldHotItem);
					ListBox.Invalidate(oldHotItem.Bounds);
				}
				ListBox.ViewInfo.HotItemIndex = hotItem.Index;
				BaseListBoxViewInfo.ItemInfo newHotItem = ListBox.ViewInfo.GetItemByIndex(ListBox.ViewInfo.HotItemIndex);
				if(newHotItem != null) {
					ListBox.ViewInfo.UpdateItem(newHotItem);
					ListBox.Invalidate(newHotItem.Bounds);
				}
				if(oldHotItem != newHotItem)
					ListBox.OnHotItemChanged();
			}
			public override void SetSelectedIndex(int newIndex) {
				if(newIndex == -1)
					ListBox.SelectedIndices.Clear();
				else {
					ListBox.ViewInfo.FocusedItemIndex = newIndex;
					ListBox.SelectedIndices.Set(ListBox.ViewInfo.FocusedItemIndex);
				}
				base.SetSelectedIndex(newIndex);
			}
			public override void SetSelected(int index, bool value) {
				if(ListBox.SelectedIndices.Contains(index) != value) {
					if(ListBox.SelectedIndex == index)
						ListBox.SelectedIndices.AddRemove(index);
					else if(value)
						SetSelectedIndex(index);
					base.SetSelected(index, value);
				}
			}
		}
		public class MultiSimpleSelectState : ListBoxControlState {
			public MultiSimpleSelectState(ListBoxControlHandler handler) : base(handler) { }
			public override HandlerState State { get { return HandlerState.MultiSimpleSelect; } }
			protected internal override int FocusedIndex {
				get { return base.FocusedIndex; }
				set {
					if(value < 0) value = 0;
					if(value > ListBox.ItemCount - 1) value = ListBox.ItemCount - 1;
					if(FocusedIndex == value) return;
					focusedIndexCore = value;
					ListBox.ViewInfo.FocusedItemIndex = value;
					ListBox.MakeItemVisible(focusedIndexCore);
				}
			}
			public override void Init() {
				base.Init();
				if(ListBox.SelectedIndex != -1)
					focusedIndexCore = ListBox.SelectedIndex;
				else
					focusedIndexCore = 0;
			}
			public override void GotFocus() {
				ListBox.LayoutChanged();
			}
			protected override void SpaceKeyDown() {
				SetSelectedIndexCore(FocusedIndex, true);
				base.SpaceKeyDown();
			}
			public override void MouseDown(MouseEventArgs e) {
				base.MouseDown(e);
				if(pressedItemIndex == -1) return;
				FocusedIndex = pressedItemIndex;
				SetSelectedIndexCore(pressedItemIndex, true);
				StartScrollTimer();
			}
			public override void MouseMove(MouseEventArgs e) {
				if(!scrollTimer.Enabled) return;
				Point pt = ListBox.Model.GetHotTrackPoint(new Point(e.X, e.Y));
				UpdateScrollTimerInterval(pt);
				BaseListBoxViewInfo.ItemInfo hotItem = ListBox.ViewInfo.GetItemInfoByPoint(pt);
				if(hotItem == null || lastHotPoint.Equals(pt)) return;
				FocusedIndex = hotItem.Index;
				lastHotPoint = pt;
				base.MouseMove(e);
			}
			protected virtual void SetSelectedIndexCore(int index, bool remove) {
				List<int> indices = new List<int>(ListBox.SelectedIndices);
				if(indices.Contains(index)) {
					if(remove) indices.Remove(index);
				}
				else
					indices.Add(index);
				indices.Sort();
				ListBox.SelectedIndices.Set(indices);
			}
			public override void SetSelectedIndex(int newIndex) {
				if(newIndex == -1) {
					ListBox.SelectedIndices.Clear();
					return;
				}
				ListBox.ViewInfo.FocusedItemIndex = newIndex;
				ListBox.SelectedIndices.Set(newIndex);
				focusedIndexCore = newIndex;
				base.SetSelectedIndex(newIndex);
			}
			public override void SetSelected(int index, bool value) {
				base.SetSelected(index, value);
				if(ListBox.SelectedIndices.Contains(index) != value) {
					ListBox.SelectedIndices.AddRemove(index);
				}
			}
		}
		public class ExtendedMultiSelectState : ListBoxControlState {
			int anchor;
			bool mousePressed;
			public ExtendedMultiSelectState(ListBoxControlHandler handler) : base(handler) { }
			int oldFocusedIndex;
			bool pressInSelection;
			public override void MouseDown(MouseEventArgs e) {
				base.MouseDown(e);
				if(pressedItemIndex == -1) return;
				if(!Handler.IsShiftPressed)
					anchor = pressedItemIndex;
				if(Handler.IsControlPressed)
					pressInSelection = ListBox.SelectedIndices.Contains(pressedItemIndex);
				else
					oldSelection.Clear();
				oldFocusedIndex = FocusedIndex;
				FocusedIndex = pressedItemIndex;
				SetSelectedIndexCore(pressedItemIndex, true);
				mousePressed = true;
				StartScrollTimer();
			}
			protected internal override int FocusedIndex {
				get { return base.FocusedIndex; }
				set {
					if(value < 0) value = 0;
					if(value > ListBox.ItemCount - 1) value = ListBox.ItemCount - 1;
					if(FocusedIndex == value) return;
					focusedIndexCore = value;
					ListBox.ViewInfo.FocusedItemIndex = value;
					ListBox.MakeItemVisible(focusedIndexCore);
				}
			}
			public override void Init() {
				base.Init();
				anchor = ListBox.SelectedIndex;
				focusedIndexCore = ListBox.SelectedIndex;
				mousePressed = false;
				UpdateOldSelection();
			}
			protected virtual void SetSelectedIndexCore(int index, bool remove) {
				if(anchor == -1)
					anchor = 0;
				if(Handler.IsControlPressed && !Handler.IsShiftPressed) {
					List<int> indices = new List<int>(ListBox.SelectedIndices);
					if(indices.Contains(index)) {
						if(remove) indices.Remove(index);
					}
					else
						indices.Add(index);
					indices.Sort();
					SetSelectedIndexCore(indices);
					return;
				}
				if(!Handler.IsControlPressed && !Handler.IsShiftPressed) {
					SetSelectedIndexCore(index);
					return;
				}
				if(Handler.IsControlPressed && Handler.IsShiftPressed) {
					List<int> indices = new List<int>(ListBox.SelectedIndices);
					int startIndex = Math.Min(anchor, FocusedIndex),
						endIndex = Math.Max(anchor, FocusedIndex);
					List<int> selection = new List<int>(endIndex - startIndex + 1);
					for(int i = startIndex; i <= endIndex; i++) {
						if(!indices.Contains(i))
							indices.Add(i);
					}
					indices.Sort();
					SetSelectedIndexCore(indices);
					return;
				}
				if(Handler.IsShiftPressed) {
					int startIndex = Math.Min(anchor, FocusedIndex),
						endIndex = Math.Max(anchor, FocusedIndex);
					List<int> selection = new List<int>(endIndex - startIndex + 1);
					for(int i = startIndex; i <= endIndex; i++) {
						selection.Add(i);
					}
					selection.Sort();
					SetSelectedIndexCore(selection);
				}
			}
			protected virtual void SetSelectedIndexCore(List<int> indexCollection) {
				ListBox.SelectedIndices.Set(indexCollection);
			}
			protected virtual void SetSelectedIndexCore(int index) {
				ListBox.SelectedIndices.Set(index);
			}
			protected virtual void ClearSelectedIndexesCore() {
				ListBox.SelectedIndices.Clear();
			}
			public override void MouseMove(MouseEventArgs e) {
				if(!mousePressed || pressedItemIndex == -1) return;
				if(!scrollTimer.Enabled && !ListBox.CanHotTrack) return;
				Point pt = ListBox.Model.GetHotTrackPoint(new Point(e.X, e.Y));
				UpdateScrollTimerInterval(pt);
				BaseListBoxViewInfo.ItemInfo hotItem = ListBox.ViewInfo.GetItemInfoByPoint(pt);
				if(hotItem == null || lastHotPoint.Equals(pt)) return;
				CreateSelection(hotItem.Index);
				lastHotPoint = pt;
				base.MouseMove(e);
			}
			List<int> oldSelection = new List<int>();
			protected virtual void CreateSelection(int index) {
				if(FocusedIndex == index) return;
				FocusedIndex = index;
				List<int> sel = new List<int>();
				if(pressInSelection) {
					sel = new List<int>(ListBox.SelectedIndices);
					for(int i = Math.Min(index, anchor); i <= Math.Max(index, anchor); i++) {
						if(i > -1 && sel.Contains(i)) sel.Remove(i);
					}
				}
				else {
					for(int i = Math.Min(anchor, index); i <= Math.Max(anchor, index); i++) {
						if(i > -1) 
							sel.Add(i);
					}
					for(int k = 0; k < oldSelection.Count; k++) {
						if(!sel.Contains(oldSelection[k])) sel.Add(oldSelection[k]);
					}
				}
				sel.Sort();
				SetSelectedIndexCore(sel);
			}
			public override void MouseUp(MouseEventArgs e) {
				base.MouseUp(e);
				UpdateOldSelection();
				if(pressedItemIndex == -1) return;
				mousePressed = pressInSelection = false;
			}
			protected bool IsSelecting { get { return mousePressed || Handler.IsShiftPressed; } }
			protected override void Navigate(int delta) {
				if(delta == 0) return;
				int hotIndex = Math.Min(Math.Max(0, FocusedIndex + delta), ListBox.ItemCount - 1);
				if(IsSelecting) {
					if(Handler.IsShiftPressed) oldSelection.Clear();
					CreateSelection(hotIndex);
				}
				else {
					if(FocusedIndex == hotIndex) return;
					FocusedIndex = anchor = hotIndex;
					SetSelectedIndexCore(hotIndex);
				}
			}
			public override void KeyUp(KeyEventArgs e) {
				base.KeyUp(e);
				UpdateOldSelection();
			}
			void UpdateOldSelection() {
				oldSelection.Clear();
				oldSelection.AddRange(ListBox.SelectedIndices);
			}
			public override void SetSelectedIndex(int newIndex) {
				if(ListBox.SelectedIndex == newIndex) return;
				if(newIndex == -1) {
					focusedIndexCore = newIndex;
					ClearSelectedIndexesCore();
					UpdateOldSelection();
					return;
				}
				ListBox.ViewInfo.FocusedItemIndex = focusedIndexCore = anchor = newIndex;
				SetSelectedIndexCore(newIndex, false);
				UpdateOldSelection();
				base.SetSelectedIndex(newIndex);
			}			
			public override HandlerState State { get { return HandlerState.ExtendedMultiSimpleSelect; } }
			public override void SetSelected(int index, bool value) {
				if(ListBox.SelectedIndices.Contains(index) != value) {
					ListBox.SelectedIndices.AddRemove(index);
				}
				UpdateOldSelection();
			}
			public override void KeyPress(KeyPressEventArgs e) {
				base.KeyPress(e);
				SetSelectedIndex(FocusedIndex);
			}
		}
		#endregion
	}
	#endregion
	public class ListBoxSearch {
		BaseListBoxControl listBox;
		DevExpress.XtraEditors.ListBoxControlHandler.ListBoxControlState controlState;
		string currentSearch;
		public ListBoxSearch(BaseListBoxControl listBox, DevExpress.XtraEditors.ListBoxControlHandler.ListBoxControlState controlState) {
			this.listBox = listBox;
			this.controlState = controlState;
		}
		protected BaseListBoxControl ListBox { get { return listBox; } }
		protected bool IncrementalSearch { get { return ListBox.IncrementalSearch; } }
		protected DevExpress.XtraEditors.ListBoxControlHandler.ListBoxControlState ControlState { get { return controlState; } }
		protected int FocusedIndex {
			get { return ControlState.FocusedIndex; }
			set { ControlState.FocusedIndex = value; }
		}
		protected int SelectedIndex {
			get { return ListBox.SelectedIndex; }
			set {
				ListBox.SelectedIndices.Clear();
				ListBox.SelectedIndex = value;
			}
		}
		public string CurrentSearch {
			get { return currentSearch; }
			protected internal set {
				if(currentSearch == value) return;
				currentSearch = value;
				DevExpress.XtraEditors.ViewInfo.BaseListBoxViewInfo.ItemInfo focusedItem = ListBox.ViewInfo.GetItemByIndex(FocusedIndex);
				if(focusedItem != null) {
					ListBox.ViewInfo.UpdateItem(focusedItem);
					ListBox.Invalidate(focusedItem.Bounds);
				}
			}
		}
		public bool IsEmpty { get { return string.IsNullOrEmpty(CurrentSearch); } }
		public bool KeyPress(char chr) {
			if((chr == ' ' && IsEmpty) || chr == '\r')
				return false;
			string newSearch = GetNewSearch(chr);
			return FindNext(newSearch, !IncrementalSearch, true);
		}		
		public bool KeyDown(Keys key, bool isCtrl) {
			if(!IncrementalSearch || IsEmpty) 
				return false;
			if(key == Keys.Space) {
				int index = FindString(CurrentSearch + " ", true, true);
				return index != -1;	
			}
			if(key == Keys.F3 || (key == Keys.Down && isCtrl) || (key == Keys.Up && isCtrl)) {
				FindNext(CurrentSearch, true, key != Keys.Up);
				return true;
			}
			if(IsResetKey(key)) {
				Reset();
				return true;
			}
			if(IsSoftResetKey(key)) {
				Reset();
				return false;
			}
			return false;
		}
		public void Reset() {
			CurrentSearch = null;
		}
		protected string GetNewSearch(char chr) {
			if(IncrementalSearch && chr == '\b') {
				if(string.IsNullOrEmpty(CurrentSearch) || CurrentSearch.Length == 1)
					return null;
				else 
					return CurrentSearch.Substring(0, CurrentSearch.Length - 1);
			}
			return IncrementalSearch ? CurrentSearch + chr.ToString() : chr.ToString();
		}
		protected bool FindNext(string s, bool startFromNext, bool updown) {
			if(string.IsNullOrEmpty(s)) {
				Reset();
				return true;
			}
			int newIndex = FindString(s, startFromNext, updown);
			if(newIndex != -1) {
				CurrentSearch = s;
				ListBox.LockSelectionChanged();
				int oldSelectedIndex = SelectedIndex;
				try {
					FocusedIndex = newIndex;
					SelectedIndex = newIndex;
				}
				finally {
					ListBox.UnLockSelectionChanged();
				}
				if(oldSelectedIndex != newIndex)
					ListBox.OnSelectionChanged();
			}
			return newIndex != -1;
		}
		protected int FindString(string s, bool startFromNext, bool updown) {
			int startIndex = FocusedIndex;
			if(startFromNext)
				startIndex += updown ? 1 : -1;
			int res = ListBox.FindString(s, startIndex, updown);
			if(res == -1)
				res = ListBox.FindString(s, updown ? -1 : ListBox.ItemCount - 1, updown);
			return res;
		}
		protected bool IsResetKey(Keys key) {
			return key == Keys.Escape || key == Keys.Enter;
		}
		protected bool IsSoftResetKey(Keys key) {
			return key == Keys.Left || key == Keys.Right || key == Keys.Up || key == Keys.Down;
		}
	}
	public class ListBoxDrawItemEventArgs : EventArgs {
		bool handled;
		object item;
		AppearanceObject appearance;
		GraphicsCache cache;
		int index;
		Rectangle bounds;
		DrawItemState state;
		BaseListBoxViewInfo viewInfo;
		BaseListBoxViewInfo.ItemInfo itemInfo;
		bool allowDrawSkinBackground;
		MethodInvoker defaultDraw;
		internal ListBoxDrawItemEventArgs(GraphicsCache cache, BaseListBoxViewInfo.ItemInfo itemInfo)
			: this(cache, itemInfo.PaintAppearance, itemInfo.Bounds, itemInfo.Item, itemInfo.Index, itemInfo.State) {
			this.itemInfo = itemInfo;
		}
		public ListBoxDrawItemEventArgs(GraphicsCache cache, AppearanceObject appearance, Rectangle bounds, object item, int index, DrawItemState state, bool allowDrawSkinBackground = true) {
			this.handled = false;
			this.item = item;
			this.appearance = appearance;
			this.cache = cache;
			this.index = index;
			this.bounds = bounds;
			this.state = state;
			this.viewInfo = null;
			this.allowDrawSkinBackground = allowDrawSkinBackground;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public object GetItemInfo() { return itemInfo; } 
		internal BaseListBoxViewInfo ViewInfo { get { return viewInfo; } set { viewInfo = value; } }
		public GraphicsCache Cache { get { return cache; } }
		public Graphics Graphics { get { return Cache == null ? null : Cache.Graphics; } }
		public bool Handled { get { return handled; } set { handled = value; } }
		public object Item { get { return item; } }
		public int Index { get { return index; } }
		public Rectangle Bounds { get { return bounds; } }
		public DrawItemState State { get { return state; } }
		public bool AllowDrawSkinBackground { get { return allowDrawSkinBackground; } set { allowDrawSkinBackground = value; } }
		public AppearanceObject Appearance {
			get { return appearance; }
		}
		public void DefaultDraw() {
			if(defaultDraw != null && !Handled) {
				Handled = true;
				defaultDraw();
			}
		}
		internal void SetDefaultDraw(MethodInvoker defaultDraw) {
			this.defaultDraw = defaultDraw;
		}
	}
	public delegate void ListBoxDrawItemEventHandler(object sender, ListBoxDrawItemEventArgs e);
	public class CustomItemDisplayTextEventArgs : EventArgs {
		public CustomItemDisplayTextEventArgs(BaseListBoxControl listBox, int index, string displayText) {
			this.ListBox = listBox;
			this.Index = index;
			this.DisplayText = displayText;
		}
		protected BaseListBoxControl ListBox { get; private set; }
		public object Item { get { return ListBox.GetItem(Index); } }
		public object Value { get { return ListBox.GetItemValue(Index); } }
		public int Index { get; private set; }
		public string DisplayText { get; set; }
	}
	public delegate void CustomItemDisplayTextEventHandler(object sender, CustomItemDisplayTextEventArgs e);
}
namespace DevExpress.XtraEditors.Controls {	
	public class ListBoxItem {
		protected internal event EventHandler ItemChanged;
		protected object fValue;
		protected object fTag;
		protected ListBoxItem(object value) : this(value, null) { }
		protected ListBoxItem(object value, object tag) {
			this.fValue = value;
			this.fTag = tag;
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ListBoxItemValue"),
#endif
 DefaultValue(null),
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))]
		public virtual object Value {
			get { return fValue; }
			set {
				if(Value != value) {
					fValue = value;
					Changed();
				}
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("ListBoxItemTag"),
#endif
 DefaultValue(null), Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public virtual object Tag {
			get { return fTag; }
			set { fTag = value; }
		}
		public override string ToString() {
			if(Value == null || Value == DBNull.Value) return base.ToString();
			return Value.ToString();
		}
		protected internal virtual void Changed() {
			Changed(EventArgs.Empty);
		}
		protected virtual void Changed(EventArgs e) {
			if(ItemChanged != null) ItemChanged(this, e);
		}
	}
	[ListBindable(false)]
	public class ListBoxItemCollection : CollectionBase, ISupportSearchDataAdapter, ISupportSelectionDataAdapter  {
		int lockUpdate;
		protected SearchDataAdapter adapter;
		public event ListChangedEventHandler ListChanged;
		public ListBoxItemCollection() {
			Init();
		}
		public ListBoxItemCollection(int capacity)
			: base(capacity) {
			Init();
		}
		public virtual void BeginUpdate() { lockUpdate++; }
		public virtual void EndUpdate() {
			if(--lockUpdate == 0) {
				OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
			}
		}
		public virtual int Add(object item) {
			return List.Add(item);
		}
		internal int AddItem(object value, string description) {
			var item = CreateItem(value, description);
			if(item != null)
				return List.Add(item);
			return -1;
		}
		protected virtual ListBoxItem CreateItem(object value, string description) {
			return null;
		}
		public void AddRange(object[] items) {
			if(items == null) return;
			BeginUpdate();
			try {
				for(int i = 0; i < items.Length; i++) Add(items[i]);
			}
			finally {
				EndUpdate();
			}
		}
#if !SL
	[DevExpressXtraEditorsLocalizedDescription("ListBoxItemCollectionItem")]
#endif
		public object this[int index] {
			get { return List[index]; }
			set { List[index] = value; }
		}
		public virtual bool Contains(object item) { return List.Contains(item); }
		public virtual int IndexOf(object item) { return List.IndexOf(item); }
		public void Insert(int index, object item) { List.Insert(index, item); }
		public virtual void Remove(object item) {
			int index = List.IndexOf(item);
			if(index != -1)
				List.RemoveAt(index);
		}
		public new int Count { get { return base.Count; } }
		protected virtual void Init() {
			this.lockUpdate = 0;
		}
		protected override void OnClearComplete() {
			OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}
		protected override void OnInsertComplete(int index, object value) {
			Attach(value);
			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemAdded, index));
		}
		protected override void OnRemoveComplete(int index, object value) {
			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemDeleted, index));
		}
		protected override void OnSetComplete(int index, object oldValue, object newValue) {
			Detach(oldValue);
			Attach(newValue);
			OnListChanged(new ListChangedEventArgs(ListChangedType.ItemChanged, index));
		}
		protected virtual void Attach(object item) { }
		protected virtual void Detach(object item) { }
		protected override void OnClear() {
			foreach(object item in this)
				Detach(item);
			base.OnClear();
		}
		protected override void OnRemove(int index, object value) {
			if(index >= 0) Detach(this[index]);
			base.OnRemove(index, value);
		}
		protected internal virtual void DoSort(SortOrder sortOrder) {
			InnerList.Sort(new ListItemComparer(sortOrder, this));
		}
		protected virtual void OnListChanged(ListChangedEventArgs e) {
			if(lockUpdate != 0) return;
			if(adapter != null)
				adapter.RefreshData();
			if(ListChanged != null) ListChanged(this, e);
		}
		protected virtual object ExtractValue(object item) {
			if(item is ListBoxItem) return (item as ListBoxItem).Value;
			return item;
		}
		protected int IndexOfValue(object value) {
			for(int i = 0; i < Count; i++) {
				ListBoxItem item = this[i] as ListBoxItem;
				if(item == null) continue;
				if(object.Equals(item.Value, value)) return i;
			}
			return -1;
		}
		void ISupportSelectionDataAdapter.SetSelected(int[] indices) {
			if(!AdapterEnabled) return;
			foreach(int index in indices)
				adapter.Selection.SetSelected(index, true);
		}
		int[] ISupportSelectionDataAdapter.GetSelectedIndices() {
			if(!AdapterEnabled) return null;
			int[] indices = adapter.Selection.GetSelectedRows();
			return indices;
		}
		void ISupportSelectionDataAdapter.BeginSelection() {
			if(!AdapterEnabled) return;
			adapter.Selection.BeginSelection();
		}
		void ISupportSelectionDataAdapter.CancelSelection() {
			if(!AdapterEnabled) return;
			adapter.Selection.Clear();
			adapter.Selection.CancelSelection();
		}
		class ListItemComparer : IComparer {
			ListBoxItemCollection collection;
			SortOrder sortOrder;
			public ListItemComparer(SortOrder sortOrder, ListBoxItemCollection collection) {
				this.sortOrder = sortOrder;
				this.collection = collection;
			}
			int IComparer.Compare(object x, object y) {
				if(sortOrder == SortOrder.None) return 0;
				x = collection.ExtractValue(x);
				y = collection.ExtractValue(y);
				return ((sortOrder == SortOrder.Ascending) ? InternalCompare(x, y) : -InternalCompare(x, y));
			}
			int InternalCompare(object x, object y) {
				if(x == y) return 0;
				if((x == null) || (x == DBNull.Value)) return -1;
				if((y == null) || (y == DBNull.Value)) return 1;
				IComparable xComp = (x is IComparable) ? (IComparable)x : (IComparable)x.ToString();
				IComparable yComp = (y is IComparable) ? (IComparable)y : (IComparable)y.ToString();
				int res = 0;
				try {
					res = xComp.CompareTo(yComp);
				}
				catch { }
				return res;
			}
		}
		#region ISupportSearchDataAdapter Members
		protected virtual bool AdapterEnabled { get { return adapter != null; } }
		protected void OnAdapterDisable() {
			if(adapter == null) return;
			adapter.Dispose();
			adapter = null;
		}
		protected virtual SearchDataAdapter CreateDataAdapter() { return new SearchDataAdapter(); }
		protected void OnAdapterEnable() {
			if(adapter != null) return;
			adapter = CreateDataAdapter();
			adapter.SetDataSource(this);
		}
		CriteriaOperator ISupportSearchDataAdapter.FilterCriteria {
			get { return AdapterEnabled ? adapter.FilterCriteria : null; }
			set {
				if(!AdapterEnabled) return;
				adapter.FilterCriteria = value;
			}
		}
		int ISupportSearchDataAdapter.GetSourceIndex(int filteredIndex) {
			return GetSourceIndexCore(filteredIndex);
		}
		protected virtual int GetSourceIndexCore(int filteredIndex) {
			if(AdapterEnabled) {
				object val = adapter.GetValueAtIndex("Column", filteredIndex);
				return List.IndexOf(val);
			}
			return filteredIndex;
		}
		int ISupportSearchDataAdapter.GetVisibleIndex(int index) { return index; }
		bool ISupportSearchDataAdapter.AdapterEnabled {
			get { return AdapterEnabled; }
			set {
				if(value) OnAdapterEnable();
				else OnAdapterDisable();
			}
		}
		int ISupportSearchDataAdapter.VisibleCount { get { return AdapterEnabled ? adapter.VisibleCount : base.Count; } }
		#endregion
	}
}
namespace DevExpress.XtraEditors.ListControls {
	public interface IDataInfo {
		string ValueMember { get; }
		string DisplayMember { get; }
	}
	public class ListDataAdapter : ListSourceDataController, IDataControllerVisualClient, IDataControllerData, IDataControllerData2 {
		public event ListChangedEventHandler AdapterListChanged;
		public event EventHandler DataSourceChanged;
		object dataSource;
		string displayMember, valueMember;
		bool sorted;
		public ListDataAdapter() {
			this.valueMember = this.displayMember = "";
			this.sorted = false;
			VisualClient = this;
			DataClient = this;
			this.dataSource = null;
		}
		public bool Sorted {
			get { return sorted; }
			set {
				if(Sorted == value) return;
				sorted = value;
				OnSortedChanged();
			}
		}
		protected virtual bool SupportSortedProperty { get { return true; } }
		public virtual void SetDataSource(object dataSource, string displayMember, string valueMember) {
			if(DataSource == dataSource && DisplayMember == displayMember && ValueMember == valueMember) return;
			if(DataSource == null && dataSource == null) return;
			this.dataSource = dataSource;
			this.displayMember = displayMember;
			this.valueMember = valueMember;
			OnDataSourceChanged();
		}
		protected override void DoSortRows() {
			if(SupportSortedProperty) {
				SortInfo.BeginUpdate();
				try {
					if(!Sorted)
						SortInfo.Clear();
					else {
						if(IsReady) {
							DataColumnInfo column = Columns[DisplayMember];
							if(column == null && Columns.Count > 0) column = Columns[0];
							if(column != null) SortInfo.Add(column, ColumnSortOrder.Ascending);
						}
					}
				}
				finally {
					SortInfo.CancelUpdate();
				}
			}
			base.DoSortRows();
		}
		protected virtual void OnSortedChanged() {
			DoRefresh();
		}
		void IDataControllerVisualClient.RequireSynchronization(IDataSync dataSync) { }
		void IDataControllerVisualClient.RequestSynchronization() {
			OnRequestSynchronization();
		}
		protected virtual void OnRequestSynchronization() { }
		void IDataControllerVisualClient.ColumnsRenewed() { }
		int IDataControllerVisualClient.VisibleRowCount { get { return -1; } }
		int IDataControllerVisualClient.TopRowIndex { get { return 0; } }
		int IDataControllerVisualClient.PageRowCount { get { return this.VisibleCount; } }
		void IDataControllerVisualClient.UpdateLayout() {
			OnListChanged(this, new ListChangedEventArgs(ListChangedType.Reset, -1));
		}
		void IDataControllerVisualClient.UpdateRowIndexes(int newTopRowIndex) {
			OnListChanged(this, new ListChangedEventArgs(ListChangedType.Reset, -1));
		}
		void IDataControllerVisualClient.UpdateRows(int topRowIndexDelta) {
			OnListChanged(this, new ListChangedEventArgs(ListChangedType.Reset, -1));
		}
		void IDataControllerVisualClient.UpdateColumns() { }
		void IDataControllerVisualClient.UpdateScrollBar() {
			OnListChanged(this, new ListChangedEventArgs(ListChangedType.Reset, -1));
		}
		void IDataControllerVisualClient.UpdateRow(int visibleRow) {
			OnListChanged(this, new ListChangedEventArgs(ListChangedType.ItemChanged, visibleRow));
		}
		UnboundColumnInfoCollection IDataControllerData.GetUnboundColumns() { return GetNotInListColumns(); }
		protected virtual UnboundColumnInfoCollection GetNotInListColumns() {
			UnboundColumnInfoCollection result = new UnboundColumnInfoCollection();
			return result;
		}
		object IDataControllerData.GetUnboundData(int row, DataColumnInfo col, object value) {
			return GetUnboundData(col.Name, row, value);
		}
		void IDataControllerData.SetUnboundData(int row, DataColumnInfo col, object value) { }
		public int FindValueIndex(string propName, object obj) {
			return FindValueIndex(propName, obj, 0);
		}
		public virtual int FindValueIndex(string propName, object obj, int indexFrom) {
			if(ItemCount == 0) return -1;
			if(indexFrom < 0) indexFrom = 0;
			if(propName != string.Empty)
				obj = ConvertValue(obj, propName);
			for(int i = indexFrom; i < ItemCount; i++) {
				object record = GetListSourceRow(i);
				if(ObjectsEqual(obj, GetValueAtIndex(propName, i)))
					return i;
			}
			return -1;
		}
		protected object ConvertValue(object obj, string fieldName) {
			DataColumnInfo info = Columns[fieldName];
			if(info == null) return obj;
			try {
				if(obj != null && info.Type.IsAssignableFrom(obj.GetType())) return obj;
				obj = Convert.ChangeType(obj, info.Type);
			}
			catch {
				return obj;
			}
			return obj;
		}
		public virtual object GetValueAtIndex(string fieldName, int index) {
			if(fieldName == string.Empty) return GetListSourceRow(index);
			return this.GetRowValue(index, fieldName);
		}
		public virtual void SetValueAtIndex(string fieldName, int index, object value) {
			if(fieldName == string.Empty) return;
			this.SetRowValue(index, fieldName, value);
		}
		public virtual object GetDataSourceRowAtIndex(int index) {
			if(ListSource == null || index < 0 || index > ListSource.Count - 1) return null;
			return GetListSourceRow(index);
		}
		public static IList GetList(object dataSource) {
			if(dataSource == null) return null;
			DataView dv = null;
			if(dataSource is DataView) dv = dataSource as DataView;
			else if(dataSource is DataTable) dv = ((DataTable)dataSource).DefaultView;
			if(dv != null) dataSource = (dv as ITypedList);
			if(dataSource is IListSource) return ((IListSource)dataSource).GetList();
			return (dataSource as IList);
		}
		public static bool ObjectsEqual(object x, object y) {
			if(x == null) return (y == null);
			return x.Equals(y);
		}
		public object Null {
			get {
				if(ListSource is DataView) return DBNull.Value;
				return null;
			}
		}
		public object DataSource {
			get { return dataSource; }
		}
		public string DisplayMember { get { return displayMember; } }
		public string ValueMember { get { return valueMember; } }
		public int ItemCount { get { return this.VisibleCount; } }
		protected virtual bool UnboundDataExists { get { return false; } }
		protected virtual object GetUnboundData(string propName, int index, object value) { return value; }
		protected virtual void SetUnboundData(string propName, int index, object value) { }
		protected virtual void OnDataSourceChanged() {
			SetListSource(null, null, string.Empty);
			SetListSource(null, DataSource, string.Empty);
			if(DataSourceChanged != null) DataSourceChanged(this, EventArgs.Empty);
		}
		protected virtual void OnListChanged(object sender, ListChangedEventArgs e) {
			if(AdapterListChanged != null) AdapterListChanged(sender, e);
		}
		#region IDataControllerData2 Members
		ComplexColumnInfoCollection IDataControllerData2.GetComplexColumns() {
			ComplexColumnInfoCollection collection = new ComplexColumnInfoCollection();
			if(ValueMember.Contains(".") && Columns[ValueMember] == null) collection.Add(ValueMember);
			if(!string.Equals(ValueMember, DisplayMember) && DisplayMember.Contains(".") && Columns[DisplayMember] == null) collection.Add(DisplayMember);
			return collection;
		}
		bool IDataControllerData2.CanUseFastProperties { get { return true; } }
		void IDataControllerData2.SubstituteFilter(SubstituteFilterEventArgs args) { }
		bool IDataControllerData2.HasUserFilter { get { return false; } }
		bool? IDataControllerData2.IsRowFit(int listSourceRow, bool fit) { return null; }
		PropertyDescriptorCollection IDataControllerData2.PatchPropertyDescriptorCollection(PropertyDescriptorCollection collection) { return collection; }
		#endregion
	}
	#region ListBoxItemSorter
	public class ListBoxItemSorter {
		ListBoxItemCollection items;
		BaseListBoxControl listBox;
		protected ListBoxItemCollection Items { get { return items; } }
		protected BaseListBoxControl ListBox { get { return listBox; } }
		public ListBoxItemSorter(ListBoxItemCollection items, BaseListBoxControl listBox) {
			this.items = items;
			this.listBox = listBox;
		}
		public void DoSort() {
			if(Items.Count < 2) return;
			object selItem = ListBox.SelectedItem;
			ArrayList selItems = new ArrayList();
			for(int i = 0; i < ListBox.SelectedIndices.Count; i++) {
				selItems.Add(Items[ListBox.SelectedIndices[i]]);
			}
			Items.DoSort(ListBox.SortOrder);
			List<int> selIndices = new List<int>();
			for(int i = 0; i < selItems.Count; i++) {
				selIndices.Add(Items.IndexOf(selItems[i]));
			}
			selIndices.Sort();
			ListBox.SelectedIndices.BeginUpdate();
			try {
				ListBox.SelectedIndices.Set(selIndices);
			}
			finally {
				ListBox.SelectedIndices.CancelUpdate();
			}
		}
	}
	#endregion
}
