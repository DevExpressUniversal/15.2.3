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
using System.Windows.Forms;
using System.Drawing;
using System.Threading;
using System.Collections;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using System.ComponentModel;
using System.Windows.Forms.Design;
using System.ComponentModel.Design;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.Utils;
using DevExpress.Skins;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Drawing;
using System.Diagnostics;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraBars.Ribbon.Internal;
namespace DevExpress.XtraBars.Ribbon.ViewInfo {
	public class ButtonGroupLayoutCalculator {
		RibbonPageGroupLayoutCalculator.GroupColumnLayoutInfo info;
		RibbonPageGroupComplexLayoutCalculator.Sequence sequence;
		IRibbonGroupInfo groupInfo;
		int rowCount, originX;
		int[] itemWidth;
		int[] rowsConfig = { 0, 0, 0 };
		int[] bestRowsConfig = { 0, 0, 0 };
		public ButtonGroupLayoutCalculator(IRibbonGroupInfo groupInfo, RibbonPageGroupLayoutCalculator.GroupColumnLayoutInfo info, int rowCount) {
			this.groupInfo = groupInfo;
			this.info = info;
			this.rowCount = rowCount;
			this.originX = Info.Location.X;
			sequence = GetButonGroupSequence();
		}
		protected internal virtual int ItemWidth(int index) { return itemWidth[index]; }
		protected internal virtual RibbonPageGroupComplexLayoutCalculator.Sequence GetButonGroupSequence() {
			int Count;
			for(Count = 0; Count < GroupInfo.Items.Count - Info.Position; Count++) {
				if(!groupInfo.IsButtonGroup(Info.Position + Count))
					break;
			}
			return new RibbonPageGroupComplexLayoutCalculator.Sequence(Info.Position, Count);
		}
		public IRibbonGroupInfo GroupInfo { get { return groupInfo; } }
		public int RowCount { get { return rowCount; } }
		public int OriginX { get { return originX; } }
		public RibbonPageGroupComplexLayoutCalculator.Sequence Sequence { get { return sequence; } }
		public RibbonPageGroupLayoutCalculator.GroupColumnLayoutInfo Info { get { return info; } }
		protected internal void CalcItemsWidth() { 
			itemWidth = new int[Sequence.Count];	
			for(int i=0; i<Sequence.Count; i ++) {
				itemWidth[i] = GroupInfo.Items[i + Sequence.BeginPos].CalcBestSize().Width;
			}
		}
		protected internal int[] RowsConfig { get { return rowsConfig; } }
		protected internal int[] BestRowsConfig { get { return bestRowsConfig; } }
		protected internal void SaveRowsConfig() {
			for(int rowIndex = 0; rowIndex < 3; rowIndex++) {
				bestRowsConfig[rowIndex] = rowsConfig[rowIndex];
			}
		}
		protected internal int GetBeginIndex(int[] rowCfg, int row) {
			int resIndex = 0;
			for(int rowIndex = 0; rowIndex < row; rowIndex++)
				resIndex += rowCfg[rowIndex];
			return resIndex;
		}
		protected internal int GetRowWidth(int rowIndex) { 
			int beginIndex = GetBeginIndex(rowsConfig, rowIndex);
			int resWidth = 0;
			for(int itemIndex = beginIndex; itemIndex < beginIndex + rowsConfig[rowIndex]; itemIndex++) {
				resWidth += itemWidth[itemIndex];
			}
			resWidth += (rowsConfig[rowIndex] - 1) * GroupInfo.DefaultIndentBetweenButtonGroups;
			return resWidth;
		}
		protected internal int GetRowMaxWidth() {
			int maxWidth = 0;
			for(int rowIndex = 0; rowIndex < RowCount; rowIndex++) {
				maxWidth = Math.Max(maxWidth, GetRowWidth(rowIndex));
			}
			return maxWidth;
		}
		protected internal int CheckCombination(int prevMaxWidth) {
			int maxWidth = GetRowMaxWidth();
			if(prevMaxWidth > maxWidth) {
				prevMaxWidth = maxWidth;
				SaveRowsConfig();
			}
			return prevMaxWidth;
		}
		protected internal int FindOptimal3RowsConfiguration() {
			int prevMaxWidth = int.MaxValue;
			for(rowsConfig[0] = 1; rowsConfig[0] <= Sequence.Count - 2; rowsConfig[0]++) {
				for(rowsConfig[1] = 1; rowsConfig[1] <= Sequence.Count - 1 - rowsConfig[0]; rowsConfig[1]++) {
					rowsConfig[2] = Sequence.Count - rowsConfig[0] - rowsConfig[1];
					prevMaxWidth = CheckCombination(prevMaxWidth);
				}
			}
			return prevMaxWidth;
		}
		protected internal int FindOptimal2RowsConfiguration() {
			int prevMaxWidth = int.MaxValue;
			for(rowsConfig[0] = 1; rowsConfig[0] <= Sequence.Count - 1; rowsConfig[0]++) {
				rowsConfig[1] = Sequence.Count - rowsConfig[0];
				prevMaxWidth = CheckCombination(prevMaxWidth);
			}
			return prevMaxWidth;
		}
		protected virtual void SetRowCount(int rowCount) {
			for(int i = Sequence.BeginPos; i <= Sequence.EndPos; i++) {
				GroupInfo.Items[i].RowCount = rowCount;
			}	
		}
		protected int GroupIndent {
			get {
				SkinElement elem = RibbonSkins.GetSkin(GroupInfo.ViewInfo.Provider)[RibbonSkins.SkinButton];
				return elem.ContentMargins.Left;
			}
		}
		public int CalcGroupsLayout() {
			CalcItemsWidth();
			int maxWidth = 0;
			if(Sequence.Count == 1 || RowCount == 1) maxWidth = CalcSingleRowBestWidth();
			else if(RowCount == 2) maxWidth = FindOptimal2RowsConfiguration();
			else if(RowCount == 3) maxWidth = FindOptimal3RowsConfiguration();
			maxWidth += GroupIndent * 2;
			StartNewColumnForButtonGroup();
			int rowIndex = 0;
			Info.Location.X += GroupIndent;
			this.originX = Info.Location.X;
			for(Info.Position = Sequence.BeginPos; Info.Position <= Sequence.EndPos; Info.Position++) {
				Info.ItemInfo = GroupInfo.Items[Info.Position];
				if(ShouldStartNewRow(Info.Position, rowIndex)) {
					rowIndex++;
					StartNewRow();
				}
				AppendItem();
			}
			SetRowCount(RowCount);
			Info.Location.X = OriginX - GroupIndent;
			Info.MaxColumnWidth = maxWidth;
			return Info.MaxColumnWidth;
		}
		protected virtual int CalcSingleRowBestWidth() {
			int width = 0;
			for(Info.Position = Sequence.BeginPos; Info.Position <= Sequence.EndPos; Info.Position++) {
				Info.ItemInfo = GroupInfo.Items[Info.Position];
				width += Info.ItemInfo.CalcBestSize().Width;
				width += GroupIndent;
			}
			width -= GroupIndent;
			return width;
		}
		protected internal virtual void StartNewRow() {
			Info.Location.Y += GroupInfo.ViewInfo.ButtonGroupHeight + GetIndentBetweenButtonGroupItems();
			Info.Location.X = OriginX;
			Info.MaxColumnWidth = 0;
			return;
		}
		protected internal virtual bool ShouldStartNewRow(int itemIndex, int rowIndex) {
			if(itemIndex == Sequence.BeginPos + GetBeginIndex(bestRowsConfig, rowIndex+1)) return true;
			return false;
		}
		protected internal virtual void AppendItem() {
			Info.ItemInfo.Bounds = new Rectangle(Info.Location, Info.ItemInfo.CalcBestSize());
			int width = Info.ItemInfo.Bounds.Width + GroupInfo.DefaultIndentBetweenButtonGroups;
			Info.MaxColumnWidth += width;
			Info.Location.X += width;
		}
		protected internal virtual int GetTopIndent() {
			if(GroupInfo.ButtonGroupsVertAlign == VertAlignment.Top) return 0;
			int deltaHeight = (GroupInfo.GroupContentHeight - GroupInfo.ViewInfo.ButtonGroupHeight * RowCount - Math.Max(0, GetIndentBetweenButtonGroupItems() * (RowCount - 1)));
			if(GroupInfo.ButtonGroupsVertAlign == VertAlignment.Bottom) return deltaHeight;
			return deltaHeight / 2;
		}
		protected internal virtual int GetIndentBetweenButtonGroupItems() {
			if(RowCount > 2 || GroupInfo.ButtonGroupsVertAlign == VertAlignment.Bottom || GroupInfo.ButtonGroupsVertAlign == VertAlignment.Top) return GroupInfo.ViewInfo.DefaultIndentBetweenButtonGroupRows;
			if(RowCount < 2) return 0;
			return (GroupInfo.GroupContentHeight - GroupInfo.ViewInfo.ButtonGroupHeight * RowCount) / (RowCount + 1);
		}
		protected internal virtual void StartNewColumnForButtonGroup() {
			if(Sequence.BeginPos != 0 && Info.MaxColumnWidth != 0) Info.Location.X += Info.MaxColumnWidth + GroupInfo.DefaultIndentBetweenColumns;
			Info.Location.Y = Info.ContentBounds.Top + GetTopIndent();
			Info.MaxColumnWidth = 0;
			Info.ColumnItemCount = 0;
		}
	}
	public class RibbonMiniToolbarButtonGroupLayoutCalculator : ButtonGroupLayoutCalculator {
		public RibbonMiniToolbarButtonGroupLayoutCalculator(IRibbonGroupInfo groupInfo, RibbonPageGroupLayoutCalculator.GroupColumnLayoutInfo info, int rowCount) : base(groupInfo, info, rowCount) { 
		}
		protected internal override int GetTopIndent() {
			if(GroupInfo.GroupContentHeight == 0)
				return 1;
			return base.GetTopIndent();
		}
		protected internal override int GetIndentBetweenButtonGroupItems() {
			if(GroupInfo.GroupContentHeight == 0)
				return 1;
			return base.GetIndentBetweenButtonGroupItems();
		}
	}
	public class RibbonPageGroupLayoutCalculator {
		IRibbonGroupInfo groupInfo;
		public RibbonPageGroupLayoutCalculator(IRibbonGroupInfo groupInfo) {
			this.groupInfo = groupInfo;
		}
		public virtual int CalcLargeButtonTextLinesCount() { return 2; }
		public virtual int CalcGroupContentHeight(RibbonViewInfo viewInfo) {
			return Math.Max(viewInfo.LargeButtonHeight, Math.Max(viewInfo.ButtonGroupHeight * MaxItemCountInColumn, viewInfo.ButtonHeight * MaxItemCountInColumn) + viewInfo.DefaultIndentBetweenRows * (MaxItemCountInColumn - 1));
		}
		public virtual IRibbonGroupInfo GroupInfo { get { return groupInfo; } }
		protected internal virtual void CalcSimpleLayout() {
			for(int i = 0; i < GroupInfo.Items.Count; i++) {
				Size size = GroupInfo.Items[i].CalcBestSize();
				GroupInfo.Items[i].Bounds = new Rectangle(0, 0, size.Width, Math.Min(size.Height, GroupInfo.ContentBounds.Height));
			}
		}
		protected internal virtual int Reduce() {
			return Reduce(null);
		}
		public class GroupColumnLayoutInfo {
			public Point Location;
			public int Position, MaxColumnWidth, ColumnItemCount;
			public RibbonItemViewInfo ItemInfo;
			public Rectangle ContentBounds;
		}
		protected virtual int GetButtonGroupsRowCount(int index) {
			if(GroupInfo.ViewInfo.IsOfficeTablet)
				return 1;
			return GroupInfo.Items[index].RowCount;
		}
		protected internal virtual void OffsetItemsHorizontally(int delta) {
			for(int i = 0; i < GroupInfo.Items.Count; i++) {
				GroupInfo.Items[i].Bounds = new Rectangle(new Point(GroupInfo.Items[i].Bounds.X + delta, GroupInfo.Items[i].Bounds.Y), GroupInfo.Items[i].Bounds.Size);
			}
		}
		protected virtual ButtonGroupLayoutCalculator CreateButtonGroupCalculator(IRibbonGroupInfo groupInfo, GroupColumnLayoutInfo info, int rowCount) {
			return new ButtonGroupLayoutCalculator(groupInfo, info, rowCount);
		}
		protected internal virtual int UpdateGroupLayout(Rectangle contentBounds) { 
			GroupColumnLayoutInfo info = new GroupColumnLayoutInfo();
			info.Location = contentBounds.Location;
			info.ContentBounds = contentBounds;
			GroupInfo.ContentBounds = contentBounds;
			info.MaxColumnWidth = 0;
			info.ColumnItemCount = 0;
			for(info.Position = 0; info.Position < GroupInfo.Items.Count; ) {
				info.ItemInfo = GroupInfo.Items[info.Position];
				if(GroupInfo.IsButtonGroup(info.Position)) {
					if(ShouldStartNewColumn(info))
						StartNewColumn(info);
					ButtonGroupLayoutCalculator buttonGroupCalc = CreateButtonGroupCalculator(GroupInfo, info, GetButtonGroupsRowCount(info.Position));
					buttonGroupCalc.CalcGroupsLayout();
					if(info.Position < GroupInfo.Items.Count)
						StartNewColumn(info);
					continue;
				}
				if(ShouldStartNewColumn(info))
					StartNewColumn(info);
				InsertItem(info);
				info.Position++;
			}
			if(GroupInfo.ViewInfo.Ribbon.AutoSizeItems) AutoSizeItems(info);
			RelayoutItemsInColumn();
			int width = info.Location.X + info.MaxColumnWidth - info.ContentBounds.Left;
			if(contentBounds.Width > width && groupInfo.AllowHorizontalCenterdItems) 
				OffsetItemsHorizontally((contentBounds.Width - width) / 2);
			return width;
		}
		protected virtual void CalcItemsViewInfo() {
			foreach(RibbonItemViewInfo itemInfo in GroupInfo.Items) {
				itemInfo.GetInfo().CalcViewInfo(GroupInfo.ViewInfo.GInfo.Graphics, itemInfo);
			}
		}
		protected internal virtual RibbonPageGroupComplexLayoutCalculator.Sequence FindItemsInColumn(RibbonPageGroupComplexLayoutCalculator.Sequence seq) {
			int itemIndex;
			for(itemIndex = seq.BeginPos; itemIndex < GroupInfo.Items.Count; itemIndex++) {
				if(GroupInfo.Items[seq.BeginPos].Bounds.X != GroupInfo.Items[itemIndex].Bounds.X) break;
			}
			seq.Count = itemIndex - seq.BeginPos;
			return seq;
		}
		protected internal virtual void RelayoutItemsInColumn() {
			RibbonPageGroupComplexLayoutCalculator.Sequence seq = new RibbonPageGroupComplexLayoutCalculator.Sequence(0, 0);
			for(; seq.BeginPos < GroupInfo.Items.Count; ) {
				seq = FindItemsInColumn(seq);
				if(seq.Count == 0) break;
				if(seq.Count == 2) RelayoutTwoItemsInColumn(seq);
				else if(seq.Count == 1) RelayoutOneItemInColumn(seq);
				seq.BeginPos += seq.Count;
			}
		}
		protected internal virtual bool IsSingleButtonGroup(RibbonPageGroupComplexLayoutCalculator.Sequence seq) { 
			if(GroupInfo.IsButtonGroup(seq.BeginPos - 1)) return false;
			if(GroupInfo.IsButtonGroup(seq.EndPos + 1)) return false;
			return true;
		}
		protected virtual void CenterItem(RibbonItemViewInfo itemInfo) {
			itemInfo.Bounds = new Rectangle(new Point(itemInfo.Bounds.X, GroupInfo.ContentBounds.Y + ( GroupInfo.ContentBounds.Height - itemInfo.Bounds.Height) / 2), itemInfo.Bounds.Size);			
		}
		protected internal virtual void RelayoutOneItemInColumn(RibbonPageGroupComplexLayoutCalculator.Sequence seq) {
			InRibbonGalleryRibbonItemViewInfo galleryInfo = GroupInfo.Items[seq.BeginPos] as InRibbonGalleryRibbonItemViewInfo;
			if(galleryInfo != null) {
				galleryInfo.Bounds = new Rectangle(new Point(galleryInfo.Bounds.X, CalcInRibbonGalleryY(galleryInfo)), galleryInfo.Bounds.Size);
				return;
			}
			RibbonItemViewInfo itemInfo = GroupInfo.Items[seq.BeginPos];			
			if(GroupInfo.IsButtonGroup(seq.BeginPos)){
				if(IsSingleButtonGroup(seq) || GroupInfo.ViewInfo.IsOfficeTablet) CenterItem(itemInfo);
				return;
			}
			if(GroupInfo.ItemsVertAlign == VertAlignment.Top) return;
			else if(GroupInfo.ItemsVertAlign == VertAlignment.Center)
				CenterItem(itemInfo);
			else
				itemInfo.Bounds = new Rectangle(new Point(itemInfo.Bounds.X, GroupInfo.ContentBounds.Bottom - itemInfo.Bounds.Height), itemInfo.Bounds.Size);
		}
		protected internal virtual void RelayoutTwoItemsInColumn(RibbonPageGroupComplexLayoutCalculator.Sequence seq) {
			if(GroupInfo.IsButtonGroup(seq.BeginPos)) return;
			if(GroupInfo.ItemsVertAlign == VertAlignment.Top) return;
			RibbonItemViewInfo itemInfo = GroupInfo.Items[seq.BeginPos], itemInfo2 = GroupInfo.Items[seq.BeginPos + 1];
			int summHeight = itemInfo.Bounds.Height + itemInfo2.Bounds.Height;
			int indent = (GroupInfo.ContentBounds.Height - summHeight) / 3;
			if(GroupInfo.ItemsVertAlign == VertAlignment.Center) {
				itemInfo.Bounds = new Rectangle(new Point(itemInfo.Bounds.X, GroupInfo.ContentBounds.Y + indent), itemInfo.Bounds.Size);
				itemInfo2.Bounds = new Rectangle(new Point(itemInfo2.Bounds.X, GroupInfo.ContentBounds.Y + indent * 2 + itemInfo.Bounds.Height), itemInfo2.Bounds.Size);
			}
			else {
				itemInfo2.Bounds = new Rectangle(new Point(itemInfo2.Bounds.X, GroupInfo.ContentBounds.Bottom - itemInfo2.Bounds.Height), itemInfo2.Bounds.Size);
				itemInfo.Bounds = new Rectangle(new Point(itemInfo.Bounds.X, itemInfo2.Bounds.Y - itemInfo.Bounds.Height - GroupInfo.ViewInfo.DefaultIndentBetweenRows), itemInfo.Bounds.Size);
			}
		}
		protected internal int IndentWithOneItem {
			get { return (GroupInfo.GroupContentHeight - GroupInfo.ViewInfo.ButtonHeight) / 2; }
		}
		protected internal int IndentBetweenTwoItems {
			get { return (GroupInfo.GroupContentHeight - GroupInfo.ViewInfo.ButtonHeight * 2) / 3; }
		}
		protected internal virtual bool ShouldStartNewColumn(GroupColumnLayoutInfo info) {
			int itemIndex = GroupInfo.Items.IndexOf(info.ItemInfo);
			if(itemIndex > 0 && GroupInfo.Items[itemIndex - 1].IsLargeButton) return true;
			if(info.ColumnItemCount == 0) return false;
			if(info.Location.Y + info.ItemInfo.CalcBestSize().Height > info.ContentBounds.Bottom) return true;
			if(info.ItemInfo.IsSeparator) return true;
			if(info.ItemInfo.IsLargeButton && info.ColumnItemCount > 0) return true;
			if(info.ColumnItemCount >= MaxItemCountInColumn) return true;
			return false;
		}
		protected virtual int MaxItemCountInColumn { 
			get {
				if(GroupInfo == null)
					return 3;
				return GroupInfo.ItemsLayout == RibbonPageGroupItemsLayout.TwoRows ? 2 : 3;
			}
		}
		protected virtual void AutoSizeItems(GroupColumnLayoutInfo info) {
			if(info.ColumnItemCount == 1) return;
			for(int pos = info.Position - 1; pos >= 0 && pos >= info.Position - info.ColumnItemCount; pos--) {
				BarEditItemLink link = GroupInfo.Items[pos].Item as BarEditItemLink;
				IHorzAlignmentProvider prov = link != null ? link.Item.Edit as IHorzAlignmentProvider : null;
				if(prov != null && prov.Alignment != HorzAlignment.Far) continue;
				GroupInfo.Items[pos].Bounds = new Rectangle(GroupInfo.Items[pos].Bounds.Location, new Size(info.MaxColumnWidth, GroupInfo.Items[pos].Bounds.Height));
			}
		}
		protected internal virtual void StartNewColumn(GroupColumnLayoutInfo info) {
			if(GroupInfo.ViewInfo.Ribbon.AutoSizeItems) AutoSizeItems(info);
			info.Location.X += info.MaxColumnWidth + GroupInfo.DefaultIndentBetweenColumns;
			info.Location.Y = info.ContentBounds.Top;
			info.MaxColumnWidth = 0;
			info.ColumnItemCount = 0;
		}
		protected internal virtual int CalcInRibbonGalleryY(InRibbonGalleryRibbonItemViewInfo ItemInfo) {
			return GroupInfo.ContentBounds.Y + (GroupInfo.ContentBounds.Height - ItemInfo.Bounds.Height) / 2;
		}
		protected internal virtual void InsertItemCore(GroupColumnLayoutInfo info) {
			Size itemSize = info.ItemInfo.CalcBestSize();
			info.MaxColumnWidth = Math.Max(info.MaxColumnWidth, itemSize.Width);
			info.ItemInfo.Bounds = new Rectangle(info.Location.X, info.Location.Y, itemSize.Width, Math.Min(GroupInfo.GroupContentHeight, itemSize.Height));
			info.Location.Y += info.ItemInfo.Bounds.Height + GroupInfo.ViewInfo.DefaultIndentBetweenRows;
			info.ColumnItemCount++;
		}
		protected internal virtual void InsertItem(GroupColumnLayoutInfo info) {
			if(info.ItemInfo.IsSeparator) InsertSeparator(info);
			else InsertItemCore(info);
		}
		protected internal virtual void InsertSeparator(GroupColumnLayoutInfo info) {
			InsertItemCore(info);
			StartNewColumn(info);
		}
		protected internal virtual int Reduce(ReduceOperation op) {
			CalcSimpleLayout();
			return 0;
		}
	}
	public class RibbonPageGroupTabletStyleLayoutCalculator : RibbonPageGroupComplexLayoutCalculator {
		public RibbonPageGroupTabletStyleLayoutCalculator(IRibbonGroupInfo groupInfo) : base(groupInfo) { 
		}
		protected override int MaxItemCountInColumn { get { return 1; } }
		protected internal override int LayoutButtonGroupsInto3Rows() {
			return 0;
		}
		protected internal override Sequence FindGroupOfLargeButtons(int startPos) {
			return new Sequence(-1, 0);
		}
		protected internal override Sequence FindGroupOfSmallButtonsWithText(int startPos) {
			return new Sequence(-1, 0);
		}
		public override int CalcLargeButtonTextLinesCount() { return 1; }
		public override int CalcGroupContentHeight(RibbonViewInfo viewInfo) {
			return Math.Max(viewInfo.ButtonGroupHeight, viewInfo.ButtonHeight);
		}
		protected override int PriorityReduce(ReduceOperation op) {
			BarItemLink link = null;
			RibbonItemViewInfo itemInfo = null;
			switch(op.Operation) { 
				case ReduceOperationType.SmallButtonsWithText:
					link = GroupInfo.ItemLinks[op.ItemLinkIndex];
					itemInfo = GetItemInfoByItem(link);
					if(itemInfo == null)
						return 0;
					return ReduceGroupOfSmallButtonsWithText(GroupInfo.Items.IndexOf(itemInfo), true);
				case ReduceOperationType.CollapseItem:
					link = GroupInfo.ItemLinks[op.ItemLinkIndex];
					itemInfo = GetItemInfoByItem(link);
					if(itemInfo == null)
						return 0;
					return TryCollapseItem(itemInfo);
			}
			return 0;
		}
		protected internal override int ReduceGroupOfSmallButtonsWithText(int startPos, bool matchIndex) {
			Sequence seq = new Sequence(startPos, 1);
			if(GroupInfo.Items.Count <= startPos || startPos < 0)
				return 0;
			return CutSmallButtonsText(seq);
		}
		protected internal override int Reduce(ReduceOperation op) {
			int deltaWidth = 0;
			if(op != null) {
				deltaWidth = PriorityReduce(op);
				return deltaWidth;
			}
			if(deltaWidth != 0) return deltaWidth;
			if(GroupInfo.Items.Count == 0) return 0;
			deltaWidth = ReduceItemWithText();
			if(deltaWidth != 0) return deltaWidth;
			deltaWidth = TryCollapseItems();
			return deltaWidth;
		}
		protected virtual int TryCollapseItem(RibbonItemViewInfo item) {
			if(GroupInfo.Items.Count <= 1)
				return 0;
			int deltaWidth = item.CalcBestSize().Width;
			GroupInfo.Items.RemoveAt(GroupInfo.Items.IndexOf(item));
			if(!HasContentButtonLink()) {
				RibbonItemViewInfo contentButtonViewInfo = GroupInfo.ViewInfo.CreateItemViewInfo(GroupInfo.ContentButtonLink);
				contentButtonViewInfo.Owner = GroupInfo;
				int addedWidth = contentButtonViewInfo.CalcBestSize().Width;
				int removedWidth = 0;
				while(addedWidth > removedWidth) {
					int index = GroupInfo.Items.Count - 1;
					if(index < 0) break;
					RibbonItemViewInfo lastItem = GroupInfo.Items[index];
					removedWidth += lastItem.CalcBestSize().Width;
					GroupInfo.Items.RemoveAt(index);
				}
				GroupInfo.Items.Add(contentButtonViewInfo);
				deltaWidth += removedWidth - addedWidth;
			}
			GroupInfo.PrecalculatedWidth -= deltaWidth;
			return deltaWidth;
		}
		protected virtual bool HasContentButtonLink() {
			foreach(RibbonItemViewInfo itemInfo in GroupInfo.Items) {
				if(itemInfo.Item == GroupInfo.ContentButtonLink)
					return true;
			}
			return false;
		}
		protected virtual int TryCollapseItems() {
			if(GroupInfo.Items.Count <= 1)
				return 0;
			RibbonItemViewInfo itemInfo = GroupInfo.Items[GroupInfo.Items.Count - 1];
			if(itemInfo.Item != GroupInfo.ContentButtonLink)
				return TryCollapseItem(itemInfo);
			return TryCollapseItem(GroupInfo.Items[GroupInfo.Items.Count - 2]);
		}
		protected virtual int ReduceItemWithText() {
			for(int i = GroupInfo.Items.Count - 1; i >= 0; i--) { 
				RibbonButtonItemViewInfo itemInfo = GroupInfo.Items[i] as RibbonButtonItemViewInfo;
				if(IsModifiableSmallButtonWithText(itemInfo)) {
					int prevWidth = itemInfo.CalcBestSize().Width;
					itemInfo.NextLevel();
					int nextWidth = itemInfo.CalcBestSize().Width;
					int deltaWidth = prevWidth - nextWidth;
					GroupInfo.PrecalculatedWidth -= deltaWidth;
					return deltaWidth;
				}
			}
			return 0;
		}
	} 
	public class RibbonPageGroupMacStyleLayoutCalculator : RibbonPageGroupComplexLayoutCalculator {
		public RibbonPageGroupMacStyleLayoutCalculator(IRibbonGroupInfo groupInfo) : base(groupInfo) { 
		}
		protected internal override int LayoutButtonGroupsInto3Rows() {
			return 0;
		}
		protected internal override Sequence FindGroupOfLargeButtons(int startPos) {
			return FindGroupOfLargeButtons(startPos, 2);
		}
		protected internal override Sequence FindGroupOfSmallButtonsWithText(int startPos) {
			return FindGroupOfSmallButtonsWithText(startPos, 2);
		}
		protected override int MaxItemCountInColumn { get { return 2; } }
		public override int CalcLargeButtonTextLinesCount() { return 1; }
		protected internal override int CalcInRibbonGalleryY(InRibbonGalleryRibbonItemViewInfo itemInfo) {
			if(itemInfo.Bounds.Height < GroupInfo.ContentBounds.Height)
				return base.CalcInRibbonGalleryY(itemInfo);
			return GroupInfo.ContentBounds.Y;
		}
	}
	public class RibbonMitiToolbarLayoutCalculator : RibbonPageGroupMacStyleLayoutCalculator {
		public RibbonMitiToolbarLayoutCalculator(IRibbonGroupInfo groupInfo) : base(groupInfo) { }
		public override int CalcGroupContentHeight(RibbonViewInfo viewInfo) {
			RibbonMiniToolbarButtonGroupLayoutCalculator calc = new RibbonMiniToolbarButtonGroupLayoutCalculator(GroupInfo, new GroupColumnLayoutInfo(), 1);
			int largeButtonsHeight = GroupInfo.HasLargeItems? viewInfo.SingleLineLargeButtonHeight: 0;
			int buttonGroupsHeight = viewInfo.ButtonGroupHeight * MaxItemCountInColumn + calc.GetTopIndent() * 2 + (MaxItemCountInColumn - 1) * calc.GetIndentBetweenButtonGroupItems();
			int buttonsHeight = viewInfo.ButtonHeight * MaxItemCountInColumn + viewInfo.DefaultIndentBetweenRows * (MaxItemCountInColumn - 1);
			int galleryHeight = GroupInfo.HasGallery ? CalcGalleryHeight() : 0;
			return Math.Max(galleryHeight, Math.Max(largeButtonsHeight, Math.Max(buttonGroupsHeight, buttonsHeight)));
		}
		protected virtual int CalcGalleryHeight() {
			foreach(RibbonItemViewInfo item in GroupInfo.Items) {
				InRibbonGalleryRibbonItemViewInfo galleryItem = item as InRibbonGalleryRibbonItemViewInfo;
				if(galleryItem != null)
					return galleryItem.CalcBestSize().Height;
			}
			return 0;
		}
		protected override ButtonGroupLayoutCalculator CreateButtonGroupCalculator(IRibbonGroupInfo groupInfo, RibbonPageGroupLayoutCalculator.GroupColumnLayoutInfo info, int rowCount) {
			return new RibbonMiniToolbarButtonGroupLayoutCalculator(groupInfo, info, rowCount);
		}
	}
	public class RibbonPageGroupComplexLayoutCalculator : RibbonPageGroupLayoutCalculator {
		public class Sequence {
			public int BeginPos;
			public int Count;
			public Sequence(int beginPos, int Count) {
				this.BeginPos = beginPos;
				this.Count = Count;
			}
			public int EndPos { get { return BeginPos + Count - 1; } }
			public int LeftPos { get { return BeginPos - 1; } }
			public int RightPos { get { return EndPos + 1; } }
		}
		public RibbonPageGroupComplexLayoutCalculator(IRibbonGroupInfo groupInfo) : base(groupInfo) { }
		protected internal override int Reduce(ReduceOperation op) {
			int deltaWidth = 0;
			if(op != null) {
				deltaWidth = PriorityReduce(op);
				return deltaWidth;
			}
			if(deltaWidth != 0) return deltaWidth;
			if(GroupInfo.Items.Count == 0) return 0;
			deltaWidth = ReduceGroupOfLargeButtons();
			if(deltaWidth != 0) return deltaWidth;
			deltaWidth = ReduceImageGallery();
			if(deltaWidth != 0) return deltaWidth;
			deltaWidth = ReduceGroupOfSmallButtonsWithText();
			if(deltaWidth != 0) return deltaWidth;
			deltaWidth = LayoutButtonGroupsInto3Rows();
			if(deltaWidth != 0) return deltaWidth;
			if(GroupInfo.AllowMinimize) deltaWidth = TryToMinimizePageGroup();
			return deltaWidth;
		}
		protected RibbonItemViewInfo GetItemInfoByItem(BarItemLink link) {
			if(link == null)
				return null;
			foreach(RibbonItemViewInfo itemInfo in GroupInfo.Items) {
				if(itemInfo.Item == link)
					return itemInfo;
			}
			return null;
		}
		protected virtual int PriorityReduce(ReduceOperation op) {
			BarItemLink link = null;
			RibbonItemViewInfo itemInfo;
			int endItemIndex = 0;
			switch(op.Operation) { 
				case ReduceOperationType.Gallery:
					link = GroupInfo.ItemLinks.Count > op.ItemLinkIndex? GroupInfo.ItemLinks[op.ItemLinkIndex] : null;
					if(link == null)
						return 0;
					InRibbonGalleryRibbonItemViewInfo galleryInfo = GetItemInfoByItem(link) as InRibbonGalleryRibbonItemViewInfo;
					if(galleryInfo == null)
						return 0;
					return ReduceImageGallery(galleryInfo);
				case ReduceOperationType.CollapseGroup:
					return TryToMinimizePageGroup();
				case ReduceOperationType.LargeButtons:
					endItemIndex = op.ItemLinkIndex + op.ItemLinksCount - 1;
					if(op.ItemLinksCount == 0 || endItemIndex >= GroupInfo.ItemLinks.Count)
						return 0;
					link = GroupInfo.ItemLinks[endItemIndex];
					itemInfo = GetItemInfoByItem(link);
					if(itemInfo == null)
						return 0;
					return ReduceGroupOfLargeButtons(GroupInfo.Items.IndexOf(itemInfo), true);
				case ReduceOperationType.SmallButtonsWithText:
					endItemIndex = op.ItemLinkIndex + op.ItemLinksCount - 1;
					if(op.ItemLinksCount == 0 || endItemIndex >= GroupInfo.ItemLinks.Count)
						return 0;
					link = GroupInfo.ItemLinks[endItemIndex];
					itemInfo = GetItemInfoByItem(link);
					if(itemInfo == null)
						return 0;
					return ReduceGroupOfSmallButtonsWithText(GroupInfo.Items.IndexOf(itemInfo), true);
				case ReduceOperationType.ButtonGroups:
					link = GroupInfo.ItemLinks[op.ItemLinkIndex];
					RibbonButtonGroupItemViewInfo groupItem = GetItemInfoByItem(link) as RibbonButtonGroupItemViewInfo;
					if(groupItem == null)
						return 0;
					return LayoutButtonGroupsInto3Rows(GroupInfo.Items.IndexOf(groupItem), true);
			}
			return 0;
		}
		protected internal virtual InRibbonGalleryRibbonItemViewInfo FindImageGalleryWithMaxWidth() {
			int maxIndex = -1;
			InRibbonGalleryRibbonItemViewInfo galleryInfo, maxGalleryInfo = null;
			for(int index = 0; index < GroupInfo.Items.Count; index++) {
				galleryInfo = GroupInfo.Items[index] as InRibbonGalleryRibbonItemViewInfo;
				if(galleryInfo == null || galleryInfo.GalleryInfo.ColCount == galleryInfo.GalleryItem.Gallery.MinimumColumnCount) continue;
				if(maxIndex < 0) {
					maxIndex = index;
					continue;
				}
				maxGalleryInfo = GroupInfo.Items[maxIndex] as InRibbonGalleryRibbonItemViewInfo;
				if(galleryInfo.CalcBestSize().Width < maxGalleryInfo.CalcBestSize().Width) continue;
				maxIndex = index;
			}
			if(maxIndex < 0) return null;
			return GroupInfo.Items[maxIndex] as InRibbonGalleryRibbonItemViewInfo;
		}
		protected internal virtual int ReduceImageGallery(InRibbonGalleryRibbonItemViewInfo galleryInfo) {
			if(galleryInfo == null) return 0;
			int deltaWidth = galleryInfo.GalleryInfo.Reduce();
			GroupInfo.PrecalculatedWidth -= deltaWidth;
			return deltaWidth;
		}
		protected internal virtual int ReduceImageGallery() {
			return ReduceImageGallery(FindImageGalleryWithMaxWidth());
		}
		protected internal virtual int LayoutButtonGroupsInto3Rows() {
			int startIndex = 0;
			return LayoutButtonGroupsInto3Rows(startIndex, false);
		}
		protected internal virtual int LayoutButtonGroupsInto3Rows(int startIndex, bool matchIndex) {
			if(GroupInfo.ViewInfo.Ribbon.GetButtonGroupsLayout() != ButtonGroupsLayout.Auto)
				return 0;
			Sequence seq = FindSequenceOfButtonGroupsIn2Rows();
			if(seq.Count == 0 || (seq.BeginPos != startIndex && matchIndex)) return 0;
			GroupColumnLayoutInfo info = new GroupColumnLayoutInfo();
			info.ItemInfo = GroupInfo.Items[seq.BeginPos];
			info.MaxColumnWidth = 0;
			info.Location = GroupInfo.Items[seq.BeginPos].Bounds.Location;
			info.Position = seq.BeginPos;
			ButtonGroupLayoutCalculator calc = new ButtonGroupLayoutCalculator(GroupInfo, info, 3);
			int deltaWidth = Get2RowButtonGroupsWidth(seq) - calc.CalcGroupsLayout();
			GroupInfo.PrecalculatedWidth -= deltaWidth;
			return deltaWidth;
		}
		protected internal virtual int GetMaxRightCoord(Sequence seq) {  
			int maxRight = 0;
			for(int itemIndex = seq.EndPos; itemIndex >= seq.BeginPos; itemIndex--) {
				maxRight = Math.Max(maxRight, GroupInfo.Items[itemIndex].Bounds.Right);
			}
			return maxRight;
		}
		protected internal virtual int Get2RowButtonGroupsWidth(Sequence seq) {
			return GetMaxRightCoord(seq) - GroupInfo.Items[seq.BeginPos].Bounds.Left;
		}
		protected internal virtual Sequence FindSequenceOfButtonGroupsIn2Rows() {
			int startPos = 0;
			Sequence seq;
			for(; ; ) {
				seq = FindSequenceOfButtonGroupsIn2Rows(startPos);
				if(seq.Count == 0) return seq;
				if(IsButtonGroupLayoutCanGiveEffect(seq)) return seq;
				startPos = seq.EndPos + 1;
			}
		}
		protected internal virtual bool IsButtonGroupLayoutCanGiveEffect(Sequence seq) {
			if(seq.Count < 3) return false;
			RibbonButtonGroupItemViewInfo vi = GroupInfo.Items[seq.BeginPos] as RibbonButtonGroupItemViewInfo;
			if(vi != null && vi.GetGroupsLayout() != ButtonGroupsLayout.Auto) return false;
			if(GetButtonGroupsRowCount(seq.BeginPos) == 3) return false;
			return true;
		}
		protected internal virtual Sequence FindSequenceOfButtonGroupsIn2Rows(int startPos) {
			for(int i = startPos; i < GroupInfo.Items.Count; i++) {
				if(!GroupInfo.IsButtonGroup(i)) continue;
				return new Sequence(i, GetButtonGroupCount(i));
			}
			return new Sequence(0, 0);
		}
		protected internal virtual int GetButtonGroupCount(int startPos) {
			int index;
			for(index = startPos; index < GroupInfo.Items.Count; index++) {
				if(!GroupInfo.IsButtonGroup(index)) break;
			}
			return index - startPos;
		}
		protected internal virtual int TryToMinimizePageGroup() {
			return TryToMinimizePageGroup(GroupInfo);
		}
		protected internal virtual int TryToMinimizePageGroup(IRibbonGroupInfo gi) {
			if(gi.Minimized) return 0;
			int deltaWidth = gi.PrecalculatedWidth - gi.CalcMinimizedWidth();
			if(deltaWidth > 0) {
				gi.Minimize();
				gi.PrecalculatedWidth -= deltaWidth;
			}
			return Math.Max(0, deltaWidth);
		}
		protected internal virtual int ReplaceLargeButtonsWithSmall(Sequence seq) {
			int prevWidth = GroupInfo.PrecalculatedWidth;
			for(int i = 0; i < seq.Count; i++) {
				RibbonButtonItemViewInfo button = (RibbonButtonItemViewInfo)GroupInfo.Items[i + seq.BeginPos];
				button.NextLevel();
			}
			GroupInfo.PrecalculatedWidth = GroupInfo.CalcBestWidth(GroupInfo.GroupContentHeight);
			return prevWidth - GroupInfo.PrecalculatedWidth;
		}
		protected internal virtual int ReduceGroupOfLargeButtons() {
			int startPos = GroupInfo.Items.Count - 1;
			return ReduceGroupOfLargeButtons(startPos, false);
		}
		protected internal virtual int ReduceGroupOfLargeButtons(int startPos, bool matchIndex) {
			Sequence seq = FindGroupOfLargeButtons(startPos);
			if(seq.Count == 0 || (matchIndex && seq.EndPos != startPos)) return 0;
			return ReplaceLargeButtonsWithSmall(seq);
		}
		protected internal bool IsModifiableLargeButton(RibbonButtonItemViewInfo buttonInfo) { 
			if(buttonInfo == null || buttonInfo.CurrentLevel != RibbonItemStyles.Large) return false;
			return buttonInfo.MaxLevel > 0;
		}
		protected internal bool IsModifiableSmallButtonWithText(RibbonButtonItemViewInfo buttonInfo) {
			if(buttonInfo == null || buttonInfo.CurrentLevel != RibbonItemStyles.SmallWithText) return false;
			return buttonInfo.ContainStyle(RibbonItemStyles.SmallWithoutText);
		}
		protected internal virtual Sequence FindGroupOfLargeButtons(int startPos) {
			return FindGroupOfLargeButtons(startPos, 3);
		}
		protected internal Sequence FindGroupOfLargeButtons(int startPos, int maxButttonCount) {
			for(int i = startPos; i >= 1; i--) {
				RibbonButtonItemViewInfo button = GroupInfo.Items[i] as RibbonButtonItemViewInfo;
				if(IsModifiableLargeButton(button)) {
					RibbonButtonItemViewInfo button2 = GroupInfo.Items[i - 1] as RibbonButtonItemViewInfo;
					if(IsModifiableLargeButton(button2) && maxButttonCount > 1) {
						if(i < 2) return new Sequence(i - 1, 2);
						RibbonButtonItemViewInfo button3 = GroupInfo.Items[i - 2] as RibbonButtonItemViewInfo;
						if(IsModifiableLargeButton(button3) && maxButttonCount > 2)
							return new Sequence(i - 2, 3);
						else
							return new Sequence(i - 1, 2);
					}
				}
			}
			return new Sequence(0, 0);
		}
		protected internal virtual int ReduceGroupOfSmallButtonsWithText() {
			int startPos = GroupInfo.Items.Count - 1;
			return ReduceGroupOfSmallButtonsWithText(startPos, false);
		}
		protected internal virtual int ReduceGroupOfSmallButtonsWithText(int startPos, bool matchIndex) {
			Sequence seq = FindGroupOfSmallButtonsWithText();
			if(seq.Count == 0 || (seq.EndPos != startPos && matchIndex)) return 0;
			return CutSmallButtonsText(seq);
		}
		protected internal virtual int CutSmallButtonsText(Sequence seq) {
			int prevWidth = 0, nextWidth = 0;
			for(int i = seq.BeginPos; i <= seq.EndPos; i++) {
				RibbonButtonItemViewInfo button = (RibbonButtonItemViewInfo)GroupInfo.Items[i];
				prevWidth = Math.Max(prevWidth, button.CalcBestSize().Width);
				button.NextLevel();
				nextWidth = Math.Max(nextWidth, button.CalcBestSize().Width);
			}
			nextWidth = Math.Max(nextWidth, GetThirdItemIndexWidth(seq));
			GroupInfo.PrecalculatedWidth -= prevWidth - nextWidth;
			return prevWidth - nextWidth;
		}
		protected internal int GetThirdItemIndexWidth(Sequence seq) { 
			int thirdItemIndex = GetThirdItemIndex(seq);
			if(thirdItemIndex != -1)return GroupInfo.Items[thirdItemIndex].CalcBestSize().Width;
			return 0;
		}
		protected internal bool IsSmallButtonTextCutGivesEffect(Sequence seq) {
			if(seq.Count == 3) return true;
			int smallButtonsMaxWidth = GetSmallButtonsMaxWidth(seq);
			int smallButtonsWithTextMaxWidth = GetSmallButtonsWithTextMaxWidth(seq);
			int thirdItemIndex = GetThirdItemIndex(seq), thirdItemIndexWidth = 0;
			if(thirdItemIndex != -1)thirdItemIndexWidth = GroupInfo.Items[thirdItemIndex].CalcBestSize().Width;
			if(thirdItemIndex == -1 || 
				(smallButtonsMaxWidth < thirdItemIndexWidth && smallButtonsWithTextMaxWidth > thirdItemIndexWidth)) return true;
			return false;
		}
		protected internal int GetThirdItemIndex(Sequence seq) {
			if(seq.LeftPos >= 0 && GroupInfo.Items[seq.LeftPos].Bounds.X == GroupInfo.Items[seq.BeginPos].Bounds.X) return seq.LeftPos;
			if(seq.RightPos <= GroupInfo.Items.Count - 1 && GroupInfo.Items[seq.EndPos].Bounds.X == GroupInfo.Items[seq.RightPos].Bounds.X) return seq.RightPos;
			return -1;
		}
		protected internal int GetSmallButtonsWithTextMaxWidth(Sequence seq) {
			int maxWidth = 0;
			for(int i = seq.BeginPos; i <= seq.EndPos; i++) {
				RibbonButtonItemViewInfo buttonInfo = (RibbonButtonItemViewInfo)GroupInfo.Items[i];
				maxWidth = Math.Max(maxWidth, buttonInfo.CalcBestSize().Width);
			}
			return maxWidth;
		}
		protected internal int GetSmallButtonsMaxWidth(Sequence seq) {
			int maxWidth = 0;
			for(int i = seq.BeginPos; i <= seq.EndPos; i++) {
				RibbonButtonItemViewInfo buttonInfo = (RibbonButtonItemViewInfo)GroupInfo.Items[i];
				buttonInfo.NextLevel();
				maxWidth = Math.Max(maxWidth, buttonInfo.CalcBestSize().Width);
				buttonInfo.PrevLevel();
			}
			return maxWidth;
		}
		protected internal Sequence FindGroupOfSmallButtonsWithText() {
			int startPos = GroupInfo.Items.Count - 1;
			Sequence seq;
			for(; ; ) {
				seq = FindGroupOfSmallButtonsWithText(startPos);
				if(seq.Count == 0) return seq;
				if(IsSmallButtonTextCutGivesEffect(seq)) return seq;
				startPos = seq.EndPos - 1;
			}
		}
		protected internal virtual Sequence FindGroupOfSmallButtonsWithText(int startPos) {
			return FindGroupOfSmallButtonsWithText(startPos, 3);
		}
		protected internal virtual Sequence FindGroupOfSmallButtonsWithText(int startPos, int maxButtonCount) {
			for(int i = startPos; i >= 1; i--) {
				RibbonButtonItemViewInfo button = GroupInfo.Items[i] as RibbonButtonItemViewInfo;
				if(IsModifiableSmallButtonWithText(button)) {
					RibbonButtonItemViewInfo button2 = GroupInfo.Items[i - 1] as RibbonButtonItemViewInfo;
					if(IsModifiableSmallButtonWithText(button2) && button2.Bounds.X == button.Bounds.X && maxButtonCount > 1) {
						if(i < 2) return new Sequence(i - 1, 2);
						RibbonButtonItemViewInfo button3 = GroupInfo.Items[i - 2] as RibbonButtonItemViewInfo;
						if(IsModifiableSmallButtonWithText(button3) && button3.Bounds.X == button2.Bounds.X && maxButtonCount > 2)
							return new Sequence(i - 2, 3);
						else
							return new Sequence(i - 1, 2);
					}
				}
			}
			return new Sequence(0, 0);
		}
	}
	public class RibbonPanelLayoutCalculator {
		RibbonPanelViewInfo panelInfo;
		public RibbonPanelLayoutCalculator(RibbonPanelViewInfo panelInfo) {
			this.panelInfo = panelInfo;
		}
		protected internal virtual RibbonPanelViewInfo PanelInfo { get { return panelInfo; } }
		protected virtual Rectangle ContentBounds { get { return PanelInfo.ContentBounds; } }
		protected internal virtual bool ShouldSimpleLayout(int bestWidth) { return bestWidth <= ContentBounds.Width; }
		public virtual RibbonPageGroupLayoutCalculator CreateGroupLayoutCalculator(RibbonPageGroupViewInfo groupInfo) {
			return new RibbonPageGroupLayoutCalculator(groupInfo);
		}
		protected internal virtual void CalcScrollInfo() {
			PanelInfo.SetShowScrollButtons(PanelInfo.MinContentWidth > PanelInfo.ContentWidth);
			if(!PanelInfo.ShowScrollButtons) {
				PanelInfo.SetLeftScrollButtonBounds(Rectangle.Empty);
				PanelInfo.SetRigthScrollButtonBounds(Rectangle.Empty);
				PanelInfo.SetPanelScrollOffset(0);
				return;
			}
			if(PanelInfo.PanelScrollOffset == 0) PanelInfo.SetLeftScrollButtonBounds(Rectangle.Empty);
			else PanelInfo.SetLeftScrollButtonBounds(new Rectangle(new Point(PanelInfo.ContentBounds.Left, PanelInfo.ContentBounds.Top), PanelInfo.GetLeftScrollButtonSize()));
			Size sz = PanelInfo.GetRightScrollButtonSize();
			if(PanelInfo.PanelScrollOffset >= PanelInfo.MinContentWidth - PanelInfo.ContentWidth) PanelInfo.SetRigthScrollButtonBounds(Rectangle.Empty);
			else PanelInfo.SetRigthScrollButtonBounds(new Rectangle(new Point(PanelInfo.ContentBounds.Left + PanelInfo.ContentWidth - sz.Width, PanelInfo.ContentBounds.Top), sz));
		}
		public virtual void CalcPanelLayout() {
			CalcSimplePanelLayout();
			int bestWidth = CalcBestWidth();
			if(!ShouldSimpleLayout(bestWidth) || RibbonReduceOperationHelper.Ribbon == PanelInfo.ViewInfo.Ribbon)
				CalcComplexPanelLayout(bestWidth);
			PanelInfo.MinContentWidth = CalcBestWidth();
			CalcScrollInfo();
			UpdatePanelLayout();
		}
		public virtual void CalcSimplePanelLayout() {
			for(int i = 0; i < PanelInfo.Groups.Count; i++) {
				PanelInfo.Groups[i].PrecalculatedWidth = PanelInfo.Groups[i].CalcBestWidth(PanelInfo.ViewInfo.GroupContentHeight);
				PanelInfo.Groups[i].ContentBounds = new Rectangle(0, 0, PanelInfo.Groups[i].PrecalculatedWidth, PanelInfo.ViewInfo.GroupContentHeight);
			}
		}
		protected virtual bool ForceStopReducing {
			get { return false; }
		}
		public virtual void CalcComplexPanelLayout(int bestWidth) {
			int deltaWidth;
			int width = bestWidth;
			do {
				if(ForceStopReducing)
					break;
				deltaWidth = Reduce();
				width -= deltaWidth;
			}
			while(width > ContentBounds.Width && deltaWidth != 0);
		}
		protected internal virtual int Reduce(RibbonPageGroupViewInfo groupInfo) {
			int prevWidth = groupInfo.PrecalculatedWidth;
			groupInfo.PrecalculatedWidth = groupInfo.CalcMinWidth();
			return prevWidth - groupInfo.PrecalculatedWidth;
		}
		protected virtual int Reduce() {
			for(int i = PanelInfo.Groups.Count - 1; i >= 0; i--) {
				int deltaWidth = Reduce(PanelInfo.Groups[i]);
				if(deltaWidth != 0) {
					return deltaWidth;
				}
			}
			return 0;
		}
		public virtual void UpdatePanelLayout() {
			int xPos = ContentBounds.Left - PanelInfo.PanelScrollOffset;
			for(int i = 0; i < PanelInfo.Groups.Count; i++) {
				PanelInfo.Groups[i].CalcViewInfo(new Rectangle(xPos, ContentBounds.Top, PanelInfo.Groups[i].PrecalculatedWidth, ContentBounds.Height));
				xPos += PanelInfo.Groups[i].PrecalculatedWidth + PanelInfo.DefaultIndentBetweenGroups;
			}
		}
		public virtual int CalcBestWidth() {
			int resWidth = 0;
			for(int i = 0; i < PanelInfo.Groups.Count; i++) {
				resWidth += PanelInfo.Groups[i].PrecalculatedWidth;
			}
			resWidth += (PanelInfo.Groups.Count - 1) * PanelInfo.DefaultIndentBetweenGroups;
			return resWidth;
		}
	}
	public class RibbonPanelComplexLayoutCalculator : RibbonPanelLayoutCalculator {
		int lastReducedGroupIndex;
		public RibbonPanelComplexLayoutCalculator(RibbonPanelViewInfo panelInfo)
			: base(panelInfo) {
			lastReducedGroupIndex = PanelInfo.Groups.Count;
			PriorityReduceIndex = 0;
		}
		protected internal override int Reduce(RibbonPageGroupViewInfo groupInfo) {
			int deltaWidth = groupInfo.Reduce();
			return deltaWidth;
		}
		protected override bool ForceStopReducing {
			get {
				return RibbonReduceOperationHelper.Ribbon == PanelInfo.ViewInfo.Ribbon && 
					PanelInfo.ViewInfo.Ribbon.ReduceOperations.Count > PriorityReduceIndex && 
					RibbonReduceOperationHelper.SelectedOperation == PanelInfo.ViewInfo.Ribbon.ReduceOperations[PriorityReduceIndex];
			}
		}
		protected override int Reduce() {
			int priorityDeltaWidth = PriorityReduce();
			if(priorityDeltaWidth != 0 || ForceStopReducing)
				return priorityDeltaWidth;
			int groupIndex = lastReducedGroupIndex - 1;
			int firstReducedGroup = groupIndex;
			bool firstProcessed = false;
			do {
				if(groupIndex < 0) groupIndex = PanelInfo.Groups.Count - 1;
				if(groupIndex < 0 || (groupIndex == firstReducedGroup && firstProcessed)) return 0;
				int deltaWidth = Reduce(PanelInfo.Groups[groupIndex]);
				if(deltaWidth != 0) {
					lastReducedGroupIndex = groupIndex;
					return deltaWidth;
				}
				groupIndex--;
				firstProcessed = true;
			}
			while(groupIndex != lastReducedGroupIndex - 1);
			return 0;
		}
		int PriorityReduceIndex { get; set; }
		protected virtual int PriorityReduce() { 
			int lastPriorityReduceIndex = PriorityReduceIndex;
			if(PanelInfo.ViewInfo.Ribbon.ReduceOperations.Count == 0)
				return 0;
			do { 
				ReduceOperation op = PanelInfo.ViewInfo.Ribbon.ReduceOperations[PriorityReduceIndex];
				if(RibbonReduceOperationHelper.Ribbon == PanelInfo.ViewInfo.Ribbon && op == RibbonReduceOperationHelper.SelectedOperation)
					return 0;
				int deltaWidth = 0;
				if(op.Group != null && op.Group.Page == PanelInfo.ViewInfo.Ribbon.SelectedPage) {
					deltaWidth = op.Group.GroupInfo != null? op.Group.GroupInfo.Reduce(op): 0;
				}
				if(deltaWidth == 0 || op.Behavior == ReduceOperationBehavior.Single)
					PriorityReduceIndex++;
				if(PriorityReduceIndex >= PanelInfo.ViewInfo.Ribbon.ReduceOperations.Count)
					PriorityReduceIndex = 0;
				if(deltaWidth != 0 && RibbonReduceOperationHelper.Ribbon != PanelInfo.ViewInfo.Ribbon)
					return deltaWidth;
			}
			while(PriorityReduceIndex != lastPriorityReduceIndex);
			return 0;
		}
	}
}
