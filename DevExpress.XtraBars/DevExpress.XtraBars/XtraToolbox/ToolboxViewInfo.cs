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

using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Menu;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraBars.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.ViewInfo;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.XtraToolbox {
	public class ToolboxViewInfo : BaseStyleControlViewInfo, ISupportXtraAnimation {
		Hashtable items;
		Hashtable groups;
		ToolboxImageCache imageCache;
		AppearanceDefault defaultAppearancePressed, defaultAppearanceHovered, defaultAppearanceDisabled, defaultAppearanceNormal;
		ToolboxRectangles rects;
		ToolboxElementPainter elementPainter;
		ToolBoxElementInfoStateObject stateObject;
		int topGroupIndent, topItemIndent;
		ToolboxGroup searchResult;
		ToolboxItemCollection selectedItems;
		public ToolboxViewInfo(ToolboxControl owner) : base(owner) {
			UpdateDefaultAppearances();
			this.selectedItems = new ToolboxItemCollection(null);
			this.groups = new Hashtable();
			this.items = new Hashtable();
			this.imageCache = new ToolboxImageCache(this);
			this.elementPainter = new ToolboxElementPainter();
			this.rects = new ToolboxRectangles();
			this.topItemIndent = this.topGroupIndent = 0;
			this.stateObject = new ToolBoxElementInfoStateObject();
			this.stateObject.StateObjectChanged += OnStateObjectChanged;
			IsInAnimation = false;
			CurrentGroupsContentHeight = DefaultGroupsContentHeight;
			visibleItems = new Hashtable();
		}
		public ToolboxElementPainter ElementPainter {
			get { return elementPainter; }
		}
		void OnStateObjectChanged(object sender, ToolBoxElementInfoStateObjectChangedEventArgs e) {
			OnStateObjectChanged(e);
		}
		protected virtual void OnStateObjectChanged(ToolBoxElementInfoStateObjectChangedEventArgs e) {
			if(e.Prev.HitTest == ToolboxHitTest.Group || e.Prev.HitTest == ToolboxHitTest.Item) {
				UpdateContentState(e.Prev.ElementInfo);
			}
			if(e.Next.HitTest == ToolboxHitTest.Group || e.Prev.HitTest == ToolboxHitTest.Item) {
				UpdateContentState(e.Next.ElementInfo);
			}
			UpdateButtonsState();
			foreach(ToolboxItemInfo info in VisibleItems.Values)
				info.PaintAppearance = CreatePaintAppearance(info);
			foreach(ToolboxGroupInfo  info in Groups.Values)
				info.PaintAppearance = CreatePaintAppearance(info);
			Toolbox.Invalidate();
		}
		Cursor prev;
		protected virtual void SetCursor(Cursor cursor) {
			if(Cursor.Current == cursor) return;
			if(prev != null) ResetCursor();
			prev = Cursor.Current;
			Cursor.Current = cursor;
		}
		protected virtual void ResetCursor() {
			if(prev == null) return;
			Cursor.Current = prev;
			prev = null;
		}
		protected internal virtual void UpdateCursor() {
			if(InDragging && SelectedItems.Count > 0) SetCursor(Cursors.SizeAll);
			else ResetCursor();
		}
		protected internal virtual void UpdateCursor(DragDropEffects effects) {
			switch(effects) {
				case DragDropEffects.Copy: SetCursor(Cursors.Default); return;
				case DragDropEffects.Move: SetCursor(Cursors.Cross); return;
				case DragDropEffects.None: SetCursor(Cursors.No); return;
				case DragDropEffects.Link: 
				case DragDropEffects.Scroll:
				case DragDropEffects.All:
				default: break;
			}
			ResetCursor();
		}
		protected internal virtual void UpdateCursor(ToolboxHitTest hit) {
			if(hit == ToolboxHitTest.Splitter) {
				SetCursor(Cursors.SizeNS);
				return;
			}
			ResetCursor();
		}
		protected virtual void UpdateContentState(ToolboxElementInfoBase info) {
			if(info == null) return;
			foreach(ToolboxElementInfoBase item in GetContentItems()) {
				if(item.State == ObjectState.Hot) item.State = ObjectState.Normal;
				if(Equals(Toolbox.SelectedGroup, item.Element)) item.State = ObjectState.Pressed;
			}
			if(!Toolbox.Enabled) {
				info.State = ObjectState.Disabled;
				return;
			}
			if(Toolbox.IsDesignMode) return;
			if(info.State != ObjectState.Normal) return;
			ObjectState state = ObjectState.Normal;
			Point pt = Toolbox.PointToClient(Control.MousePosition);
			if(info.Bounds.Contains(pt)) {
				state |= ObjectState.Hot;
			}
			info.State = state;
		}
		public IEnumerable<ToolboxElementInfoBase> GetContentItems() {
			foreach(ToolboxGroupInfo group in Groups.Values) {
				yield return group;
			}
			foreach(ToolboxElementInfoBase item in Items.Values) {
				yield return item;
			}
		}
		protected virtual void UpdateButtonsState() {
			MenuButton.State = CalcElementStateCore(MenuButton.Bounds);
			MoreItemsButton.State = CalcElementStateCore(MoreItemsButton.Bounds);
			ScrollButtonUp.State = CalcElementStateCore(ScrollButtonUp.Bounds);
			ScrollButtonDown.State = CalcElementStateCore(ScrollButtonDown.Bounds);
			ExpandButton.State = CalcElementStateCore(ExpandButton.Bounds);
		}
		protected Point GetMousePoint() {
			return Toolbox.PointToClient(Control.MousePosition);
		}
		protected virtual ObjectState CalcElementStateCore(Rectangle bounds) {
			if(!Toolbox.Enabled) return ObjectState.Disabled;
			Point pt = GetMousePoint();
			ObjectState state = ObjectState.Normal;
			if(bounds.Contains(pt)) {
				state = ObjectState.Hot;
				if((Control.MouseButtons & MouseButtons.Left) != 0) state |= ObjectState.Pressed;
			}
			return state;
		}
		protected internal virtual void ResetStateObject() {
			stateObject.Reset();
		}
		protected internal void CheckStateObject() {
			CheckStateObject(GetMousePoint());
		}
		protected internal void CheckStateObject(Point pt, ToolboxHitInfo hi) {
			this.stateObject.SetStateObject(hi);
		}
		protected internal void CheckStateObject(Point pt) {
			ToolboxHitInfo hi = CalcHitInfo(pt);
			CheckStateObject(pt, hi);
		}
		internal bool InDragging { get; set; }
		internal ToolboxMenuButtonViewInfo MenuButton {
			get { return (ToolboxMenuButtonViewInfo)Toolbox.MenuButton.ViewInfo; }
		}
		internal ToolboxExpandButtonViewInfo ExpandButton {
			get { return (ToolboxExpandButtonViewInfo)Toolbox.ExpandButton.ViewInfo; }
		}
		internal ToolboxMoreItemsButtonViewInfo MoreItemsButton {
			get { return (ToolboxMoreItemsButtonViewInfo)Toolbox.MoreItemsButton.ViewInfo; }
		}
		internal ToolboxScrollButtonUpViewInfo ScrollButtonUp {
			get { return (ToolboxScrollButtonUpViewInfo)Toolbox.ScrollButtonUp.ViewInfo; }
		}
		internal ToolboxScrollButtonDownViewInfo ScrollButtonDown {
			get { return (ToolboxScrollButtonDownViewInfo)Toolbox.ScrollButtonDown.ViewInfo; }
		}
		protected internal void UpdateButtonsAppearance() {
			MenuButton.PaintAppearance = CreatePaintAppearance(MenuButton);
			MoreItemsButton.PaintAppearance = CreatePaintAppearance(MoreItemsButton);
		}
		protected internal void UpdateDefaultAppearances() {
			this.defaultAppearanceNormal = CreateDefaultAppearance(ObjectState.Normal);
			this.defaultAppearanceHovered = CreateDefaultAppearance(ObjectState.Hot);
			this.defaultAppearancePressed = CreateDefaultAppearance(ObjectState.Pressed);
			this.defaultAppearanceDisabled = CreateDefaultAppearance(ObjectState.Disabled);
		}
		protected override void CalcContentRect(Rectangle bounds) {
			if(IsInAnimation) return;
			Toolbox.BeginUpdate();
			try {
				Rects.HeaderRect = CalcHeaderInfo();
				CalcHeaderCaptionBounds();
				Rects.GroupsClientRect = CalcGroupsInfo(Rects.HeaderRect);
				Rects.ItemsClientRect = CalcItemsInfo(bounds, Rects.GroupsClientRect.Bottom);
				Toolbox.ScrollController.AdjustContent(RightToLeft);
				Toolbox.ScrollController.UpdateScrolls();
				CalcGroups();
				CalcItems(GetActiveGroup());
				MoreItemsButton.CalcBounds(Rects.ItemsContentRect);
			}
			finally {
				Toolbox.EndUpdate();
			}
		}
		protected virtual ToolboxGroup SearchResult {
			get { return searchResult ?? (searchResult = new ToolboxGroup()); }
		}
		protected internal virtual void SetSearchResult(IEnumerable<ToolboxItem> items) {
			ResetSearchResult();
			SearchResult.Items.AddRange(items);
		}
		protected internal virtual void ResetSearchResult() {
			SearchResult.Items.Clear();
			Toolbox.Refresh();
		}
		protected internal virtual IToolboxGroup GetActiveGroup() {
			return IsSearching ? SearchResult : Toolbox.SelectedGroup;
		}
		internal ToolboxRectangles Rects {
			get { return rects; }
		}
		protected virtual int CalcMinimizedBestWidth() {
			return CalcItemOnlyImageBestSize().Width * Toolbox.OptionsMinimizing.ColumnCount;
		}
		protected virtual Rectangle CalcItemsInfo(Rectangle rect, int y) {
			Rects.ItemsClientRect = new Rectangle(rect.X, y, rect.Width, rect.Height - y);
			MoreItemsButton.CalcViewInfo(Rects.ItemsClientRect);
			ScrollButtonUp.CalcViewInfo(Rects.ItemsClientRect);
			ScrollButtonDown.CalcViewInfo(Rects.ItemsClientRect);
			Rects.ItemsContentRect = CalcItemsContentBounds(Rects.ItemsClientRect, y);
			return Rects.ItemsClientRect;
		}
		protected virtual Rectangle CalcItemsContentBounds(Rectangle rect, int top) {
			return new Rectangle(
				Rects.ItemsClientRect.Left,
				Rects.ItemsClientRect.Top + ScrollButtonUp.Bounds.Height,
				Rects.ItemsClientRect.Width,
				Rects.ItemsClientRect.Height - MoreItemsButton.Bounds.Height - ScrollButtonUp.Bounds.Height - ScrollButtonDown.Bounds.Height);
		}
		protected internal virtual bool CanShowMenuButton() {
			if(!Toolbox.OptionsView.ShowMenuButton || IsSearching) return false;
			if(Toolbox.ShouldDrawOnlyItems) return false;
			return true;
		}
		protected internal virtual bool CanShowExpandButton() {
			return Toolbox.OptionsMinimizing.AllowMinimizing;
		}
		protected internal virtual bool CanDrawSplitter() {
			if(Toolbox.ShouldDrawOnlyItems) return false;
			return true;
		}
		protected internal virtual bool CanDrawSeparatorLine(ToolboxItem item) {
			return GetActiveGroup().Items.IndexOf(item) > 0;
		}
		protected internal virtual bool CanDrawSeparatorText() {
			return !IsMinimized;
		}
		protected internal virtual bool CanDrawGroups() {
			if(Toolbox.ShouldDrawOnlyItems || IsMinimized) return false;
			return Rects.GroupsContentRect.Height > 0;
		}
		protected internal virtual bool CanDrawItems() {
			return Rects.ItemsContentRect.Height > 0;
		}
		protected internal bool UseOffice2003Style {
			get {
				ActiveLookAndFeelStyle style = LookAndFeel.ActiveLookAndFeel.ActiveStyle;
				return style == ActiveLookAndFeelStyle.Office2003 || style == ActiveLookAndFeelStyle.WindowsXP;
			}
		}
		protected internal bool UseFlatStyle {
			get {
				ActiveLookAndFeelStyle style = LookAndFeel.ActiveLookAndFeel.ActiveStyle;
				return style == ActiveLookAndFeelStyle.Flat || style == ActiveLookAndFeelStyle.Style3D || style == ActiveLookAndFeelStyle.UltraFlat;
			}
		}
		protected internal virtual Color GetBackColor() {
			if(Toolbox.Appearance.Toolbox.Options.UseBackColor)
				return Toolbox.Appearance.Toolbox.BackColor;
			if(UseOffice2003Style) {
				return Office2003PaintHelper.BackColor;
			}
			if(UseFlatStyle) {
				return FlatPaintHelper.BackColor;
			}
			SkinElement elem = AccordionControlSkins.GetSkin(LookAndFeel.ActiveLookAndFeel)[AccordionControlSkins.SkinBackground];
			if(elem != null) return elem.Color.GetBackColor();
			elem = NavBarSkins.GetSkin(LookAndFeel.ActiveLookAndFeel)[NavBarSkins.SkinBackground];
			return elem.Color.GetBackColor();
		}
		protected virtual Rectangle CalcHeaderInfo() {
			if(Toolbox.ShouldDrawOnlyItems) return Rectangle.Empty;
			Padding padding = GetCaptionPadding();
			Rectangle header = new Rectangle(Bounds.X, Bounds.Y, Bounds.Width, padding.Vertical);
			ExpandButton.CalcViewInfo(Rectangle.Empty);
			CalcHeaderCaptionBounds();
			if(ExpandButton.Bounds.Height == 0 && Rects.HeaderCaptionRect.Height == 0)
				header.Height = 0;
			else
				header.Height += Math.Max(ExpandButton.Bounds.Height, Rects.HeaderCaptionRect.Height);
			ExpandButton.CalcViewInfo(header);
			return header;
		}
		protected virtual Size CalcHeaderCaptionSize() {
			if(!CanShowHeaderCaption()) return Size.Empty;
			return Toolbox.Appearance.Toolbox.CalcTextSize(GInfo.Graphics, Toolbox.Caption, 0).ToSize();
		}
		protected internal virtual Padding GetCaptionPadding() {
			return NavPaneSkins.GetSkin(LookAndFeel.ActiveLookAndFeel)[NavPaneSkins.SkinCaption].ContentMargins.ToPadding();
		}
		protected virtual void CalcHeaderCaptionBounds() {
			Size size = CalcHeaderCaptionSize();
			int indent = Math.Max((Rects.HeaderRect.Height - size.Height) / 2, 0);
			int x = IsRightToLeft ? Rects.HeaderRect.Right - GetCaptionPadding().Left - size.Width : Rects.HeaderRect.Left + GetCaptionPadding().Left;
			Rects.HeaderCaptionRect = new Rectangle(new Point(x, Rects.HeaderRect.Top + indent), size);
		}
		protected internal virtual bool CanShowHeaderCaption() {
			if(IsMinimized || string.IsNullOrEmpty(Toolbox.Caption)) return false;
			return Toolbox.OptionsView.ShowToolboxCaption;
		}
		protected virtual ToolboxItemCollection FindItems(string text) {
			ToolboxItemCollection items = new ToolboxItemCollection(null);
			string lastGroupCaption = string.Empty;
			foreach(ToolboxGroup g in Groups.Keys) {
				foreach(ToolboxItem i in g.Items) {
					if(!i.Caption.ToLower().Contains(text.ToLower())) continue;
					ToolboxItem item = new ToolboxItem(i.Caption);
					item.BeginGroup = lastGroupCaption != g.Caption;
					item.BeginGroupCaption = item.BeginGroup ? g.Caption : string.Empty;
					lastGroupCaption = g.Caption;
					items.Add(item);
				}
			}
			return items;
		}
		protected bool HasSelectedGroup {
			get { return Toolbox.ViewInfo.GetActiveGroup() != null; }
		}
		internal int TopGroupIndent {
			get { return topGroupIndent; }
			set {
				if(TopGroupIndent == value)
					return;
				topGroupIndent = value;
				Toolbox.Refresh();
				Toolbox.Invalidate();
			}
		}
		internal int TopItemIndent {
			get { return topItemIndent; }
			set {
				if(TopItemIndent == value)
					return;
				topItemIndent = value;
				Toolbox.Refresh();
				Toolbox.Invalidate();
			}
		}
		protected int CheckTopItemIndent(int value) {
			return Math.Max(0, Math.Min(value, Rects.ItemsContentRect.Height));
		}
		protected internal virtual void Search(string text) {
			ToolboxItemCollection items = FindItems(text);
			if(!CanRefreshFoundResult(items)) return;
			ShowFoundItems(items);
		}
		protected virtual bool CanRefreshFoundResult(ToolboxItemCollection items) {
			if(items.Count != SearchResult.Items.Count) return true;
			for(int i = 0; i < items.Count; i++) {
				if(SearchResult.Items[i] == items[i]) continue;
				return true;
			}
			return false;
		}
		protected internal virtual void ShowFoundItems(IEnumerable<ToolboxItem> items) {
			SetSearchResult(items);
			CalcViewInfo(GInfo.Graphics);
			Toolbox.Invalidate();
		}
		protected virtual void CalcGroups() {
			if(IsMinimized) return;
			Groups.Clear();
			int y = Rects.GroupsContentRect.Top - TopGroupIndent;
			foreach(ToolboxGroup group in Toolbox.Groups) {
				ToolboxGroupInfo info = GetGroupInfo(group);
				info.State = Equals(group, Toolbox.SelectedGroup) ? ObjectState.Pressed : ObjectState.Normal;
				info.PaintAppearance = CreatePaintAppearance(info);
				info.CalcViewInfo(new Rectangle(Rects.GroupsContentRect.X, y, Rects.GroupsContentRect.Width, GetGroupHeight()));
				y = info.Bounds.Bottom;
			}
		}
		protected virtual int SearchContentIndent {
			get { return 5; }
		}
		protected virtual Rectangle CalcSearchContentRect(Rectangle rect) {
			Toolbox.SearchControl.Visible = CanShowSearchPanel();
			if(!CanShowSearchPanel()) return Rectangle.Empty;
			Rects.SearchContentRect = new Rectangle(rect.Left, rect.Bottom, rect.Width, CalcSearchContentHeight());
			Toolbox.SearchControl.Bounds = Rectangle.Inflate(Rects.SearchContentRect, -SearchContentIndent, -SearchContentIndent);
			return Rects.SearchContentRect;
		}
		protected virtual int CalcSearchContentHeight() {
			if(!CanShowSearchPanel()) return 0;
			return Toolbox.SearchControl.Bounds.Height + SearchContentIndent * 2;
		}
		protected virtual bool CanShowSearchPanel() {
			if(Toolbox.ShouldDrawOnlyItems) return false;
			if(IsMinimized || !Toolbox.OptionsView.ShowSearchPanel) return false;
			return true;
		}
		protected virtual Rectangle CalcGroupsInfo(Rectangle rect) {
			int height = 0;
			Rectangle content = new Rectangle(rect.Left, rect.Bottom, rect.Width, height);
			content.Height += CalcSearchContentRect(content).Height;
			MenuButton.CalcViewInfo(content);
			content.Height += MenuButton.Bounds.Height;
			content.Height += CalcGroupsContentBounds(content).Height;
			content.Height += CalcSplitterBounds().Height;
			return content;
		}
		protected virtual Rectangle CalcGroupsContentBounds(Rectangle rect) {
			Rectangle bounds = new Rectangle(rect.X, rect.Bottom, rect.Width, CurrentGroupsContentHeight);
			if(Toolbox.ShouldDrawOnlyItems || IsSearching || IsMinimized)
				bounds = Rectangle.Empty;
			Rects.GroupsContentRect = new Rectangle(rect.X, rect.Bottom, rect.Width, bounds.Height);
			return Rects.GroupsContentRect;
		}
		protected virtual int DefaultGroupHeight {
			get { return 30; }
		}
		protected internal virtual int GetGroupHeight() {
			int height = DefaultGroupHeight;
			foreach(ToolboxGroup group in Toolbox.Groups) {
				ToolboxGroupInfo info = GetGroupInfo(group);
				if(info.CaptionBounds.Size.IsEmpty) info.CalcCaptionBounds();
				int max = Math.Max(info.HasImage ? group.Image.Size.Height + info.GetPadding().Vertical : 0, info.CaptionBounds.Size.Height);
				if(height > max) continue;
				height = max;
			}
			return height;
		}
		protected virtual int DefaultItemHeight {
			get { return 30; }
		}
		protected internal virtual int GetItemHeight() {
			Padding padding = GetPadding(GetSkinElement());
			int height = Math.Max(GetItemImageSize().Height, PaintAppearance.CalcDefaultTextSize().Height) + padding.Vertical;
			return Math.Max(DefaultItemHeight, height);
		}
		protected virtual int DefaultGroupsContentHeight {
			get { return 100; }
		}
		protected internal virtual int GetAllGroupsHeight() {
			if(Toolbox.Groups == null || IsSearching) return 0;
			return Toolbox.Groups.Count * GetGroupHeight();
		}
		protected virtual int GetBestPopupItemColumnCount() {
			return Rects.ItemsContentRect.Width / CalcItemOnlyImageBestSize().Width;
		}
		protected virtual int GetColumnCount() {
			if(Toolbox.ShouldDrawOnlyItems)
				return GetBestPopupItemColumnCount();
			return IsMinimized ? Toolbox.OptionsMinimizing.ColumnCount : Toolbox.OptionsView.ColumnCount;
		}
		int firstInvisibleMinimizedItemIndex = -1;
		internal int FirstInvisibleMinimizedItemIndex {
			get { return firstInvisibleMinimizedItemIndex; }
		}
		Hashtable visibleItems;
		public Hashtable VisibleItems {
			get { return visibleItems; }
		}
		protected internal virtual void CalcItems(IToolboxGroup group) {
			VisibleItems.Clear();
			if(!HasSelectedGroup || group.Items == null) return;
			Size bestSize = CalcItemBestSize();
			Size separator = CalcSeparatorSize(CanDrawSeparatorText());
			int y = Rects.ItemsContentRect.Y;
			int currentColumn = -1;
			int columns = GetColumnCount();
			firstInvisibleMinimizedItemIndex = -1;
			for(int i = 0; i < group.Items.Count; i++) {
				ToolboxItem item = group.Items[i];
				ToolboxItemInfo itemInfo = GetItemInfo(item);
				if(!itemInfo.GetItemVisibility()) continue;
				if(item.BeginGroup) {
					y += separator.Height + (i == 0 ? 0 : bestSize.Height);
					currentColumn = 0;
				}
				else {
					currentColumn++;
					if(currentColumn >= columns) {
						currentColumn = 0;
						y += bestSize.Height;
					}
				}
				int indent = currentColumn * bestSize.Width;
				int x = IsRightToLeft ? Rects.ItemsContentRect.Right - indent - bestSize.Width : Rects.ItemsContentRect.X + indent;
				itemInfo.Bounds = new Rectangle(new Point(x, y - TopItemIndent), bestSize);
				if(ShouldCalcItemViewInfo(itemInfo)) {
					VisibleItems.Add(item, itemInfo);
					CalcItemInfo(itemInfo);
				}
				if(itemInfo.IsReady)
					UpdateItemBounds(itemInfo);
				if(itemInfo.Bounds.Top > Rects.ItemsContentRect.Bottom && FirstInvisibleMinimizedItemIndex < 0)
					firstInvisibleMinimizedItemIndex = i;
			}
		}
		protected virtual bool ShouldCalcItemViewInfo(ToolboxItemInfo itemInfo) {
			Rectangle rect = itemInfo.Bounds;
			Rectangle area = Rects.ItemsContentRect;
			return rect.Bottom >= area.Top && rect.Top <= area.Bottom;
		}
		protected virtual void UpdateItemBounds(ToolboxItemInfo itemInfo) {
			itemInfo.CalcImageBounds();
			itemInfo.CalcCaptionBounds();
		}
		protected virtual void CalcItemInfo(ToolboxItemInfo itemInfo) {
			itemInfo.PaintAppearance = CreatePaintAppearance(itemInfo);
			itemInfo.Element.Image = GetItemImage(itemInfo.Item);
			itemInfo.WrappedText = CreateWrappedText(itemInfo.Item);
			itemInfo.IsReady = true;
		}
		protected internal virtual bool CanShowMoreItemsButton() {
			if(Toolbox.ShouldDrawOnlyItems || Toolbox.OptionsMinimizing.ScrollMode == ToolboxMinimizingScrollMode.Default) return false;
			return IsMinimized && FirstInvisibleMinimizedItemIndex >= 0 && GetAllItemsHeight() > Rects.ItemsContentRect.Height;
		}
		protected internal virtual bool CanShowMinimizedScrollButtons() {
			return IsMinimized && Toolbox.OptionsMinimizing.ScrollMode == ToolboxMinimizingScrollMode.Default;
		}
		protected internal virtual ToolboxItemInfo GetItemInfo(ToolboxItem item) {
			if(!Items.ContainsKey(item))
				Items[item] = new ToolboxItemInfo(item, this);
			return Items[item] as ToolboxItemInfo;
		}
		protected internal virtual ToolboxGroupInfo GetGroupInfo(ToolboxGroup group) {
			if(!Groups.ContainsKey(group))
				Groups[group] = new ToolboxGroupInfo(group, this);
			return Groups[group] as ToolboxGroupInfo;
		}
		protected virtual Size CalcItemOnlyImageBestSize() {
			Padding padding = GetPadding(GetSkinElement());
			Size sz = GetItemImageSize();
			return new Size(sz.Width + padding.Horizontal, sz.Height + padding.Vertical);
		}
		protected internal virtual Size CalcItemBestSize() {
			return new Size(Rects.ItemsContentRect.Width / GetColumnCount(), GetItemHeight());
		}
		protected internal virtual int GetAllItemsHeight() {
			if(!HasSelectedGroup || GetActiveGroup().Items == null || GetActiveGroup().Items.Count <= 0) return 0;
			Size itemSize = CalcItemBestSize();
			Rectangle first = CalcItemBounds(GetActiveGroup(), 0, itemSize);
			Rectangle last = CalcItemBounds(GetActiveGroup(), GetActiveGroup().Items.Count - 1, CalcItemBestSize());
			int fullHeight = last.Bottom - first.Top + (GetActiveGroup().Items[0].BeginGroup ? CalcSeparatorSize(CanDrawSeparatorText()).Height : 0);
			int invisibleItems = 0;
			int invisibleBeginGroups = 0;
			foreach(ToolboxItem item in Toolbox.SelectedGroup.Items) {
				ToolboxItemInfo info = GetItemInfo(item);
				if(info.GetItemVisibility()) continue;
				invisibleItems++;
				if(item.BeginGroup)
					invisibleBeginGroups++;
			}
			int invisibleRows = (int)Math.Ceiling((double)(invisibleItems - invisibleBeginGroups) / GetColumnCount()) + invisibleBeginGroups;
			int invisibleHeight = invisibleRows * itemSize.Height + invisibleBeginGroups * CalcSeparatorSize(true).Height;
			return fullHeight - invisibleHeight;
		}
		protected virtual Rectangle CalcItemBounds(IToolboxGroup group, int index, Size bestSize) {
			int x = Rects.ItemsContentRect.X;
			int y = Rects.ItemsContentRect.Y - GetItemHeight();
			int column = GetColumnCount();
			int separatorHeight = CalcSeparatorSize(CanDrawSeparatorText()).Height;
			for(int i = 0; i <= index; i++) {
				if(GetItemColumn(group, i) == 0)
					y += GetItemHeight();
				if(group.Items[i].BeginGroup)
					y += separatorHeight;
			}
			Point loc = new Point(x + GetItemColumn(group, index) * bestSize.Width, y - TopItemIndent);
			return new Rectangle(loc, bestSize);
		}
		protected internal virtual int GetItemColumn(IToolboxGroup group, int itemIndex) {
			int itemCount = -1;
			for(int i = 0; i <= itemIndex; i++) {
				if(group.Items[i].BeginGroup)
					itemCount += GetColumnCount() - itemCount % GetColumnCount();
				else
					itemCount++;
			}
			return itemCount % GetColumnCount();
		}
		protected bool HasWrappedText(AppearanceObject obj) {
			return obj.TextOptions.Trimming == Trimming.Default && obj.TextOptions.WordWrap == WordWrap.Default;
		}
		protected int CalcTextMaxWidth(int contentWidth, bool hasImage) {
			return hasImage ? contentWidth - GetItemImageSize().Width - Toolbox.OptionsView.ImageToTextDistance : contentWidth;
		}
		protected string[] CreateWrappedText(ToolboxItem item) {
			ToolboxItemInfo info = GetItemInfo(item);
			if(!HasWrappedText(info.PaintAppearance)) return null;
			int maxWidth = CalcTextMaxWidth(info.Bounds.Width - GetPadding(GetSkinElement()).Horizontal, info.HasImage);
			return ToolboxHelper.WrapText(GInfo.Cache, info.PaintAppearance, item.Caption, maxWidth);
		}
		internal ToolboxImageCache ImageCache {
			get { return imageCache; }
		}
		protected internal virtual Image GetItemImage(ToolboxItem item) {
			if(item.Image != null) return item.Image;
			return ImageCache.GetItemImage(item);
		}
		protected AppearanceDefault CreateDefaultAppearance(ObjectState state) {
			return new AppearanceDefault() { ForeColor = GetElementForeColor(state) };
		}
		protected internal virtual AppearanceObject CreatePaintAppearance(ToolboxElementInfoBase info) {
			return new AppearanceObject(GetPaintAppearance(info), GetElementDefaultAppearance(info.State));
		}
		protected virtual AppearanceDefault GetElementDefaultAppearance(ObjectState state) {
			if(state == ObjectState.Pressed) return this.defaultAppearancePressed;
			if(state == ObjectState.Hot) return this.defaultAppearanceHovered;
			if(state == ObjectState.Disabled) return this.defaultAppearanceDisabled;
			return this.defaultAppearanceNormal;
		}
		protected virtual AppearanceObject GetPaintAppearance(ToolboxElementInfoBase info) {
			AppearanceObject res = new AppearanceObject();
			AppearanceObject[] combine = new AppearanceObject[] {
				GetElementAppearance(info.State, info.Element.Appearance),
				GetBaseElementAppearance(info.State, info.Element)
			};
			AppearanceHelper.Combine(res, combine);
			return res;
		}
		protected internal virtual AppearanceObject GetBaseElementAppearance(ObjectState state, ToolboxElementBase element) {
			if(element is ToolboxItem)  return GetElementAppearance(state, Toolbox.Appearance.Item);
			if(element is ToolboxGroup) return GetElementAppearance(state, Toolbox.Appearance.Group);
			return GetElementAppearance(state, element.Appearance);
		}
		protected AppearanceObject GetElementAppearance(ObjectState state, ToolboxElementAppearance appearance) {
			if(state == ObjectState.Disabled) return appearance.Disabled;
			if(state == ObjectState.Hot) return appearance.Hovered;
			if(state == ObjectState.Pressed) return appearance.Pressed;
			return appearance.Normal;
		}
		public override void Reset() {
			ItemImageSizeCore = Size.Empty;
			BestItemImageSizeCore = Size.Empty;
			GroupImageSizeCore = Size.Empty;
		}
		Size ItemImageSizeCore { get; set; }
		Size GetItemImageSizeCore() {
			if(GetItemViewMode() == ToolboxItemViewMode.NameOnly) return Size.Empty;
			if(Toolbox.OptionsView.ItemImageSize.IsEmpty) return GetBestItemImageSize();
			return Toolbox.OptionsView.ItemImageSize;
		} 
		public virtual Size GetItemImageSize() {
			if(!ItemImageSizeCore.IsEmpty)
				return ItemImageSizeCore;
			ItemImageSizeCore = GetItemImageSizeCore();
			return ItemImageSizeCore;
		}
		Size BestItemImageSizeCore { get; set; }
		Size GetBestItemImageSizeCore() {
			int width = 0, height = 0;
			foreach(ToolboxGroup g in Groups.Keys) {
				foreach(ToolboxItem i in g.Items) {
					width = Math.Max(width, GetItemInfo(i).HasImage ? i.Image.Size.Width : 0);
					height = Math.Max(height, GetItemInfo(i).HasImage ? i.Image.Size.Height : 0);
				}
			}
			return new Size(width, height);
		}
		public Size GetBestItemImageSize() {
			if(!BestItemImageSizeCore.IsEmpty)
				return BestItemImageSizeCore;
			BestItemImageSizeCore = GetBestItemImageSizeCore();
			return BestItemImageSizeCore;
		}
		Size GroupImageSizeCore { get; set; }
		Size GetGroupImageSizeCore() {
			int width = 0, height = 0;
			foreach(ToolboxGroup g in Groups.Keys) {
				width = Math.Max(width, GetGroupInfo(g).HasImage ? g.Image.Size.Width : 0);
				height = Math.Max(height, GetGroupInfo(g).HasImage ? g.Image.Size.Height : 0);
			}
			return new Size(width, height);
		}
		protected internal virtual Size GetGroupImageSize() {
			if(!GroupImageSizeCore.IsEmpty)
				return GroupImageSizeCore;
			GroupImageSizeCore = GetGroupImageSizeCore();
			return GroupImageSizeCore;
		}
		int currentGroupsContentHeight;
		protected virtual int CurrentGroupsContentHeight {
			get {
				if(Toolbox.OptionsView.GroupPanelAutoHeight) return GetAllGroupsHeight();
				if(IsMinimized || Toolbox.ShouldDrawOnlyItems) return 0;
				return currentGroupsContentHeight;
			}
			set {
				if(CurrentGroupsContentHeight == value)
					return;
				currentGroupsContentHeight = value;
				OnGroupsContentHeightChanged();
			}
		}
		protected virtual void OnGroupsContentHeightChanged() { }
		protected internal void SetGroupsRegionHeight(int value) {
			CurrentGroupsContentHeight = Math.Max(0, value);
		}
		internal Hashtable Groups {
			get { return groups; }
		}
		internal Hashtable Items {
			get { return items; }
		}
		protected internal ToolboxControl Toolbox {
			get { return OwnerControl as ToolboxControl; }
		}
		protected Color GetElementForeColor(ObjectState state) {
			if(state == ObjectState.Disabled)
				return CommonSkins.GetSkin(LookAndFeel.ActiveLookAndFeel).GetSystemColor(SystemColors.GrayText);
			SkinElement elem = GetSkinElement();
			Color defColor = elem.Color.GetForeColor();
			if(state == ObjectState.Hot) return elem.Properties.GetColor("ForeColorHot", defColor);
			if(state == ObjectState.Pressed) return elem.Properties.GetColor("ForeColorPressed", defColor);
			return defColor;
		}
		protected SkinElement GetSkinElement() {
			return AccordionControlSkins.GetSkin(LookAndFeel.ActiveLookAndFeel)[AccordionControlSkins.SkinItem];
		}
		protected internal Padding GetPadding(SkinElement skin) {
			if(skin == null) return Padding.Empty;
			return skin.ContentMargins.ToPadding();
		}
		protected virtual Size CalcSeparatorSize(bool withText = false) {
			Size line = CommonSkins.GetSkin(LookAndFeel.ActiveLookAndFeel)[CommonSkins.SkinLabelLine].Size.MinSize;
			Size tsCore = CalcTextSizeCore(GInfo.Graphics, "Wg", 0);
			tsCore.Height += GetCaptionPadding().Vertical;
			Size text = withText ? tsCore : Size.Empty;
			return new Size(line.Width, line.Height + text.Height);
		}
		protected internal Rectangle CalcSeparatorLineBounds(ToolboxItemInfo itemInfo) {
			Rectangle area = GetSeparatorArea(itemInfo);
			return new Rectangle(area.X, area.Y, area.Width, 1);
		}
		protected internal Rectangle CalcSeparatorTextBounds(ToolboxItemInfo itemInfo) {
			if(!CanDrawSeparatorText()) return Rectangle.Empty;
			Rectangle area = GetSeparatorArea(itemInfo);
			Size textSize = CalcTextSizeCore(GInfo.Graphics, itemInfo.Item.BeginGroupCaption, 0);
			int x = IsRightToLeft ? area.Right - GetCaptionPadding().Left - textSize.Width: area.Left + GetCaptionPadding().Left;
			Point loc = new Point(x, area.Bottom - textSize.Height - GetCaptionPadding().Top);
			return new Rectangle(loc, textSize);
		}
		protected internal Rectangle GetSeparatorArea(ToolboxItemInfo itemInfo) {
			Size sz = CalcSeparatorSize(CanDrawSeparatorText());
			return new Rectangle(Rects.ItemsContentRect.X, itemInfo.Bounds.Top - sz.Height, Rects.ItemsContentRect.Width, sz.Height);
		}
		protected internal Rectangle CalcSplitterBounds() {
			if(!CanDrawSplitter()) return Rectangle.Empty;
			Size sz = CalcSeparatorSize();
			return new Rectangle(Rects.GroupsClientRect.X, Rects.GroupsClientRect.Bottom - sz.Height, Rects.GroupsClientRect.Width, sz.Height);
		}
		protected internal Rectangle GetSplitterRectArea() {
			if(IsMinimized || Toolbox.OptionsView.GroupPanelAutoHeight) return Rectangle.Empty;
			return Rectangle.Inflate(CalcSplitterBounds(), 0, 2);
		}
		public virtual ToolboxHitInfo CalcHitInfo(Point pt) {
			ToolboxHitInfo hitInfo = new ToolboxHitInfo(pt);
			if(Toolbox.ScrollController.GroupScroll.IsVisible) {
				if(hitInfo.CheckBounds(Toolbox.ScrollController.GroupScroll.VScroll.Bounds, ToolboxHitTest.ScrollBar, null))
					return hitInfo;
			}
			if(Toolbox.ScrollController.ItemScroll.IsVisible) {
				if(hitInfo.CheckBounds(Toolbox.ScrollController.ItemScroll.VScroll.Bounds, ToolboxHitTest.ScrollBar, null))
					return hitInfo;
			}
			if(CanShowMoreItemsButton())
				if(hitInfo.CheckBounds(MoreItemsButton.Bounds, ToolboxHitTest.MoreItemsButton, null)) return hitInfo;
			if(CanShowMinimizedScrollButtons()) {
				if(hitInfo.CheckBounds(ScrollButtonUp.Bounds, ToolboxHitTest.ScrollButtonUp, null)) return hitInfo;
				if(hitInfo.CheckBounds(ScrollButtonDown.Bounds, ToolboxHitTest.ScrollButtonDown, null)) return hitInfo;
			}
			if(hitInfo.CheckBounds(ExpandButton.Bounds, ToolboxHitTest.ExpandButton, null)) return hitInfo;
			if(hitInfo.CheckBounds(GetSplitterRectArea(), ToolboxHitTest.Splitter, null)) return hitInfo;
			if(hitInfo.CheckBounds(MenuButton.Bounds, ToolboxHitTest.MenuButton, null)) return hitInfo;
			if(Rects.GroupsContentRect.Contains(pt)) {
				foreach(ToolboxGroupInfo group in Groups.Values) {
					if(hitInfo.CheckBounds(group.Bounds, ToolboxHitTest.Group, group)) return hitInfo;
				}
			}
			if(Rects.ItemsContentRect.Contains(pt)) {
				foreach(ToolboxItemInfo item in VisibleItems.Values) {
					if(hitInfo.CheckBounds(item.Bounds, ToolboxHitTest.Item, item)) return hitInfo;
				}
			}
			return hitInfo;
		}
		public void RunMinimizingAnimation() {
			if(IsInAnimation) return;
			XtraAnimator.Current.AddAnimation(new ToolboxExpandCollapseAnimationInfo(this, this, IsMinimized, 100));
		}
		protected internal virtual void CreatePrevItemsBitmap() {
			if(Toolbox.Width == 0 || Toolbox.Height == 0) return;
			if(Toolbox.ViewInfo.Rects.ItemsClientRect.Width == 0 || Toolbox.ViewInfo.Rects.ItemsClientRect.Height == 0) return;
			Bitmap bmp = new Bitmap(Toolbox.Width, Toolbox.Height);
			Toolbox.DrawToBitmap(bmp, new Rectangle(Point.Empty, Toolbox.Size));
			PrevToolboxBitmap = bmp;
		}
		protected internal virtual void AnimationComplete() {
			TopItemIndent = 0;
			CreatePrevItemsBitmap();
			IsInAnimation = false;
			RunFadeAnimation();
			Toolbox.Refresh();
		}
		protected internal virtual Bitmap PrevToolboxBitmap { get; set; }
		protected internal virtual float PrevToolboxBitmapOpacity { get; set; }
		public void RunFadeAnimation() {
			if(IsInAnimation) return;
			PrevToolboxBitmapOpacity = 1;
			XtraAnimator.Current.AddAnimation(new ToolboxFadeAnimationInfo(this, this, 150));
		}
		int defaultExpandedWidth = 250;
		internal int GetExpandedWidth() {
			if(Toolbox.OptionsMinimizing.NormalWidth == -1)
				return defaultExpandedWidth;
			return Toolbox.OptionsMinimizing.NormalWidth;
		}
		protected internal int GetMinimizedWidth() {
			if(Toolbox.OptionsMinimizing.MinimizedWidth == -1)
				return CalcMinimizedBestWidth();
			return Toolbox.OptionsMinimizing.MinimizedWidth;
		}
		public void InvertMinimizedState() {
			Toolbox.OptionsMinimizing.State = IsMinimized ? ToolboxState.Normal : ToolboxState.Minimized;
		}
		internal bool ShouldInvertExpandButton() {
			return IsMinimized ^ Toolbox.OptionsMinimizing.MinimizeButtonMode == MinimizeButtonMode.Inverted;
		}
		public bool IsMinimized {
			get { return Toolbox.OptionsMinimizing.State == ToolboxState.Minimized; }
		}
		public bool CanAnimate {
			get { return true; }
		}
		Control ISupportXtraAnimation.OwnerControl {
			get { return Toolbox; }
		}
		public bool IsInAnimation { get; set; }
		public bool IsRightToLeft {
			get { return Toolbox.IsRightToLeft; }
		}
		protected internal virtual bool IsSmoothScrolling {
			get { return Toolbox.OptionsBehavior.AllowSmoothScrolling; }
		}
		public bool IsDesignTime {
			get { return Toolbox.IsDesignMode; }
		}
		SplitContainerControl scParentCore;
		protected virtual SplitContainerControl SCParent {
			get {
				if(!(Toolbox.Parent is SplitGroupPanel)) return null;
				return scParentCore ?? (scParentCore = GetParentSplitContainer());
			}
		}
		protected internal virtual void ResetParent() {
			scParentCore = null;
		}
		protected internal void OnSizeChanged(int delta) {
			if(SCParent == null) return;
			int c = SCParent.Panel1 == Toolbox.Parent ? 1 : -1;
			SCParent.SplitterPosition += c * delta;
		}
		protected internal SplitContainerControl GetParentSplitContainer() {
			if(Toolbox.Parent is SplitGroupPanel) {
				SplitContainerControl control = Toolbox.Parent.Parent as SplitContainerControl;
				if(control == null || !control.Horizontal)
					return null;
				return control;
			}
			return null;
		}
		protected internal virtual ToolboxItemViewMode GetItemViewMode() {
			if(IsMinimized) return ToolboxItemViewMode.IconOnly;
			return Toolbox.OptionsView.ItemViewMode;
		}
		protected internal virtual bool IsSearching {
			get {
				if(Toolbox.SearchControl == null) return false;
				return !string.IsNullOrEmpty(Toolbox.SearchControl.Text);
			}
		}
		protected internal virtual ToolboxItemCollection SelectedItems {
			get { return selectedItems; }
			set { selectedItems = value; }
		}
		protected internal virtual void StateChanged() {
			if(!IsReady) return;
			RunMinimizingAnimation();
		}
	}
	public class ToolboxItemInfo : ToolboxElementInfoBase {
		public ToolboxItemInfo(ToolboxItem item, ToolboxViewInfo info) : base(item, info) {
			IsReady = false;
		}
		public bool IsReady { get; set; }
		public ToolboxItem Item {
			get { return Element as ToolboxItem; }
		}
		protected virtual bool IsChecked {
			get { return ViewInfo.SelectedItems.Contains(Item) && ViewInfo.Toolbox.OptionsBehavior.ItemSelectMode != ToolboxItemSelectMode.None; }
		}
		public override ObjectState State {
			get { return IsChecked ? ObjectState.Pressed : base.State; }
			set { base.State = value; }
		}
		protected internal override Size GetBestImageSize() {
			return ViewInfo.GetItemImageSize();
		}
		protected internal bool GetItemVisibility() {
			if(ViewInfo.IsDesignTime) return true;
			return Item.Visible;
		}
	}
	public class ToolboxGroupInfo : ToolboxElementInfoBase {
		public ToolboxGroupInfo(ToolboxGroup group, ToolboxViewInfo info) : base(group, info) { }
		public ToolboxGroup Group {
			get { return Element as ToolboxGroup; }
		}
		protected internal override Rectangle CalcBounds(Rectangle content) {
			return content;
		}
		protected internal override Size GetBestImageSize() {
			return ViewInfo.GetGroupImageSize();
		}
		protected override bool IsIconOnly {
			get { return false; }
		}
	}
	public class ToolboxMenuButtonViewInfo : ToolboxElementInfoBase {
		public ToolboxMenuButtonViewInfo(ToolboxElementBase element, ToolboxViewInfo info) : base(element, info) { }
		protected internal override Rectangle CalcBounds(Rectangle content) {
			int height = Math.Max(ViewInfo.GetGroupHeight(), HasImage ? Element.Image.Size.Height + GetPadding().Vertical : 0);
			return new Rectangle(content.X, content.Bottom, content.Width, ViewInfo.CanShowMenuButton() ? height : 0);
		}
		protected internal override void CalcViewInfo(Rectangle content) {
			base.CalcViewInfo(content);
			CalcArrowImageBounds();
		}
		protected virtual void CalcArrowImageBounds() {
			Rectangle rect = Rectangle.Empty;
			int offset = 3;
			if(CanShowArrow()) {
				rect.Size = new Size(ArrowSkinElement.Size.MinSize.Width + offset, ArrowSkinElement.Size.MinSize.Height + offset);
				int indent = Bounds.Height / 2 - rect.Height / 2;
				Rectangle r = HasCaption ? CaptionBounds : GetImageArea();
				rect.X = ViewInfo.IsRightToLeft ? r.Left - rect.Width - indent : r.Right + indent;
				rect.Y = Bounds.Y + indent;
			}
			ArrowBounds = rect;
		}
		protected virtual bool CanShowArrow() {
			return ArrowSkinElement != null;
		}
		protected virtual SkinElement ArrowSkinElement {
			get { return AccordionControlSkins.GetSkin(ViewInfo.LookAndFeel.ActiveLookAndFeel)[AccordionControlSkins.SkinGroupOpenButton]; }
		}
		protected internal virtual SkinElementInfo GetArrowSkinElementInfo() {
			SkinElementInfo info = new SkinElementInfo(ArrowSkinElement, ArrowBounds);
			info.RightToLeft = ViewInfo.IsRightToLeft;
			return info;
		}
		protected virtual Rectangle ArrowBounds { get; set; }
		protected virtual Point GetMenuLocation() {
			return new Point(ViewInfo.IsRightToLeft ? Bounds.Left : Bounds.Right, Bounds.Top);
		}
		protected virtual DXPopupMenu GetMenu() {
			DXPopupMenu popup = new DXPopupMenu();
			foreach(ToolboxGroup group in ViewInfo.Toolbox.Groups) {
				DXMenuItem item = new DXMenuItem(group.Caption);
				item.CloseMenuOnClick = false;
				item.Click += delegate { ViewInfo.Toolbox.SelectedGroup = group; };
				item.Image = group.Image;
				popup.Items.Add(item);
			}
			return popup;
		}
		protected internal virtual void Show() {
			DXPopupMenu popup = GetMenu();
			ViewInfo.Toolbox.RaiseInitializeMenu(new ToolboxInitializeMenuEventArgs(popup, ViewInfo.IsMinimized));
			MenuManagerHelper.ShowMenu(popup, ViewInfo.LookAndFeel, ViewInfo.Toolbox.MenuManager, ViewInfo.Toolbox, GetMenuLocation());
		}
	}
	public class ToolboxExpandButtonViewInfo : ToolboxElementInfoBase {
		public ToolboxExpandButtonViewInfo(ToolboxElementBase element, ToolboxViewInfo info) : base(element, info) { }
		protected internal override Rectangle CalcBounds(Rectangle content) {
			if(!ViewInfo.CanShowExpandButton()) return Rectangle.Empty;
			Size size = SkinElement.Image.GetImageBounds(0).Size;
			int indent = (content.Height - size.Height) / 2;
			int x = ViewInfo.IsRightToLeft ? content.Left + indent : content.Right - size.Width - indent;
			return new Rectangle(new Point(x, content.Y + indent), size);
		}
		protected override SkinElement SkinElement {
			get {
				string skin = ViewInfo.ShouldInvertExpandButton() ? NavPaneSkins.SkinExpandButton : NavPaneSkins.SkinCollapseButton;
				return NavPaneSkins.GetSkin(ViewInfo.LookAndFeel.ActiveLookAndFeel)[skin];
			}
		}
	}
	public class ToolboxMoreItemsButtonViewInfo : ToolboxElementInfoBase {
		public ToolboxMoreItemsButtonViewInfo(ToolboxElementBase element, ToolboxViewInfo info) : base(element, info) { }
		protected internal override Rectangle CalcBounds(Rectangle content) {
			int height = ViewInfo.CanShowMoreItemsButton() ? ViewInfo.GetGroupHeight() : 0;
			return new Rectangle(content.X, content.Bottom - height, content.Width, height);
		}
		protected virtual ToolboxPopupForm CreatePopupForm() {
			return new ToolboxPopupForm(ViewInfo.Toolbox);
		}
		protected internal virtual void Show() {
			ToolboxPopupForm popup = CreatePopupForm();
			popup.DoShow();
		}
	}
	public class ToolboxScrollButtonUpViewInfo : ToolboxScrollButtonViewInfoBase {
		public ToolboxScrollButtonUpViewInfo(ToolboxButtonBase button, ToolboxViewInfo info) : base(button, info) { }
		protected internal override Rectangle CalcBounds(Rectangle content) {
			if(!CanVisible(content)) return Rectangle.Empty;
			return new Rectangle(content.X, content.Top, content.Width, DefaultHeight);
		}
		protected override bool AllowScroll {
			get { return ViewInfo.TopItemIndent > ViewInfo.Toolbox.ScrollController.ItemScroll.VScrollArgs.Minimum; }
		}
	}
	public class ToolboxScrollButtonDownViewInfo : ToolboxScrollButtonViewInfoBase {
		public ToolboxScrollButtonDownViewInfo(ToolboxButtonBase button, ToolboxViewInfo info) : base(button, info) { }
		protected internal override Rectangle CalcBounds(Rectangle content) {
			if(!CanVisible(content)) return Rectangle.Empty;
			return new Rectangle(content.X, content.Bottom - DefaultHeight, content.Width, DefaultHeight);
		}
		protected override bool AllowScroll {
			get { return ViewInfo.TopItemIndent < ViewInfo.Toolbox.ScrollController.ItemScroll.VScrollArgs.Maximum - ViewInfo.Rects.ItemsContentRect.Height; }
		}
		protected override int DeltaImageIndex {
			get { return 4; }
		}
	}
	public class ToolboxScrollButtonViewInfoBase : ToolboxElementInfoBase {
		public ToolboxScrollButtonViewInfoBase(ToolboxButtonBase button, ToolboxViewInfo info) : base(button, info) { }
		protected virtual bool AllowScroll {
			get { return true; }
		}
		public override ObjectState State {
			get { return AllowScroll ? base.State : ObjectState.Disabled; }
			set { base.State = value; }
		}
		protected virtual int DefaultHeight {
			get { return 15; }
		}
		protected virtual bool CanVisible(Rectangle content) {
			return ViewInfo.CanShowMinimizedScrollButtons() && content.Height < ViewInfo.GetAllItemsHeight();
		}
		protected virtual int DeltaImageIndex {
			get { return 0; }
		}
		protected internal override SkinElementInfo GetSkinElementInfo() {
			SkinElementInfo info = base.GetSkinElementInfo();
			info.ImageIndex += DeltaImageIndex;
			return info;
		}
		protected override SkinElement SkinElement {
			get { return CommonSkins.GetSkin(ViewInfo.LookAndFeel.ActiveLookAndFeel)[CommonSkins.SkinScrollButton]; }
		}
	}
}
