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

using System.Windows.Controls;
using System.Windows;
using System.Collections.Generic;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System;
using System.Windows.Media;
using System.Linq;
using System.Collections.ObjectModel;
using DevExpress.Mvvm;
using DevExpress.Data;
using DevExpress.Xpf.Core.Internal;
using System.Reflection;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Grid.Hierarchy {
	public class SeparatorInfo : BindableBase {
		public Thickness Margin {
			get { return GetProperty(() => Margin); }
			set { SetProperty(() => Margin, value); }
		}
		public Orientation Orientation {
			get { return GetProperty(() => Orientation); }
			set { SetProperty(() => Orientation, value); }
		}
		public double Length {
			get { return GetProperty(() => Length); }
			set { SetProperty(() => Length, value); }
		}
		public bool IsVisible {
			get { return GetProperty(() => IsVisible); }
			set { SetProperty(() => IsVisible, value); }
		}
		public int RowIndex {
			get { return GetProperty(() => RowIndex); }
			set { SetProperty(() => RowIndex, value); }
		}
	}
	public class CardRowInfo {
		public Size Size { get; set; }
		public Size RenderSize { get; set; }
		public IList<IItem> Elements { get; set; }
		public int Level { get; set; }
		public bool IsItemsContainer { get; set; }
		public bool HasSeparator { get; set; }
	}
	public class CardsHierarchyPanel : HierarchyPanel {
		#region DependencyProperties
		public static readonly DependencyProperty CardMarginProperty = DependencyProperty.Register("CardMargin", typeof(Thickness), typeof(CardsHierarchyPanel), new FrameworkPropertyMetadata(new Thickness(), (d, e) => ((CardsHierarchyPanel)d).OnCardMarginChanged()));
		public static readonly DependencyProperty ContainerIndentProperty = DependencyProperty.Register("ContainerIndent", typeof(double), typeof(CardsHierarchyPanel), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty FixedSizeProperty = DependencyProperty.Register("FixedSize", typeof(double), typeof(CardsHierarchyPanel), new FrameworkPropertyMetadata(double.NaN, FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty SeparatorThicknessProperty = DependencyProperty.Register("SeparatorThickness", typeof(double), typeof(CardsHierarchyPanel), new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty MaxCardCountInRowProperty = DependencyProperty.Register("MaxCardCountInRow", typeof(int), typeof(CardsHierarchyPanel), new FrameworkPropertyMetadata(int.MaxValue, FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty CardAlignmentProperty = DependencyProperty.Register("CardAlignment", typeof(Alignment), typeof(CardsHierarchyPanel), new FrameworkPropertyMetadata(Alignment.Near, FrameworkPropertyMetadataOptions.AffectsArrange));
		static readonly DependencyPropertyKey SeparatorInfoPropertyKey = DependencyProperty.RegisterReadOnly("SeparatorInfo", typeof(IList<SeparatorInfo>), typeof(CardsHierarchyPanel), new PropertyMetadata(null));
		public static readonly DependencyProperty SeparatorInfoProperty = SeparatorInfoPropertyKey.DependencyProperty;
		public Thickness CardMargin {
			get { return (Thickness)GetValue(CardMarginProperty); }
			set { SetValue(CardMarginProperty, value); }
		}
		public double ContainerIndent {
			get { return (double)GetValue(ContainerIndentProperty); }
			set { SetValue(ContainerIndentProperty, value); }
		}
		public double FixedSize {
			get { return (double)GetValue(FixedSizeProperty); }
			set { SetValue(FixedSizeProperty, value); }
		}
		public double SeparatorThickness {
			get { return (double)GetValue(SeparatorThicknessProperty); }
			set { SetValue(SeparatorThicknessProperty, value); }
		}
		public int MaxCardCountInRow {
			get { return (int)GetValue(MaxCardCountInRowProperty); }
			set { SetValue(MaxCardCountInRowProperty, value); }
		}
		public Alignment CardAlignment {
			get { return (Alignment)GetValue(CardAlignmentProperty); }
			set { SetValue(CardAlignmentProperty, value); }
		}
		public IList<SeparatorInfo> SeparatorInfo {
			get { return (IList<SeparatorInfo>)GetValue(SeparatorInfoProperty); }
			private set { SetValue(SeparatorInfoPropertyKey, value); }
		}
		#endregion
		Size CardMarginCached = new Size();
		void OnCardMarginChanged() {
			CardMarginCached = new Size(CardMargin.Left + CardMargin.Right, CardMargin.Top + CardMargin.Bottom); ;
			InvalidateMeasure();
		}
		internal double MaxSecondarySize { get; private set; }
		internal List<CardRowInfo> RowsInfo { get; private set; }
		List<FrameworkElement> hiddenItems;
		protected override Size MeasureItemsContainer(Size availableSize, IItemsContainer itemsContainer) {
			if(itemsContainer == null)
				return new Size();
			oldOffset = DataPresenter != null ? DataPresenter.ActualScrollOffset : 0;
			CorrectScrollParametersIfScrollOutOfBounds();
			hiddenItems = new List<FrameworkElement>();
			RowsInfo = MeasureItemsContainer(0, availableSize, itemsContainer, 0);
			UpdateSeparatorInfo(availableSize);
			MaxSecondarySize = SizeHelper.GetSecondarySize(itemsContainer.DesiredSize);
			return itemsContainer.DesiredSize;
		}
		internal void ClearRows() {
			RowsInfo.Clear();
		}
		int tryCount = 0;
		internal bool OnLayoutUpdated() {
			Size availableSize = DataPresenter.LastConstraint;
			if(CorrectGenerateItemsOffset(SizeHelper.GetSecondarySize(availableSize)) && GenerateItemsIfNeeded(SizeHelper.GetDefineSize(availableSize) - GetRowOffset())) {
				tryCount = 0;
				return true;
			}
			tryCount++;
			if(tryCount > 100) {
#if DEBUGTEST
				throw new InvalidOperationException("Cyclic items regeneration.");
#else
				tryCount = 0;
				return false;
#endif
			}
			InvalidateMeasure();
			return false;
		}
		bool CorrectScrollParametersIfScrollOutOfBounds() {
			if(DataPresenter == null)
				return true;
			if(ScrollOutOfBounds()) {
				oldOffset = Math.Max(0, Math.Min(Extent - Viewport, CalcExtentSimple() - 1));
				actualGenerateItemsOffset = Math.Max(0, DataPresenter.ItemCount - 1);
				DataPresenter.SetDefineScrollOffset(oldOffset);
				lastRowIndex = actualGenerateItemsOffset;
				NodeContainer.ReGenerateItems(GenerateItemsOffset, 1, false);
				return false;
			}
			return true;
		}
		bool ScrollOutOfBounds() {
			return DataPresenter != null && actualGenerateItemsOffset >= DataPresenter.ItemCount && actualGenerateItemsOffset != 0;
		}
		double CalcExtentSimple() {
			if(GroupInfo.Count == 0)
				return (int)Math.Ceiling((double)DataPresenter.ItemCount / rowCardCount);
			double extent = 0;
			foreach(GroupRowInfo groupInfo in GroupInfo) {
				if(groupInfo.ParentGroup != null && !groupInfo.ParentGroup.Expanded) continue;
				extent++;
				if(groupInfo.Expanded && groupInfo.Level == LastGroupLevel)
					extent += Math.Ceiling((double)groupInfo.ChildControllerRowCount / rowCardCount);
			}
			return extent;
		}
		DetailNodeContainer NodeContainer { get { return DataPresenter.View.RootNodeContainer; } }
		double? rest;
		bool GenerateItemsIfNeeded(double defineSize) {
			if(DataPresenter == null || !AllowPerPixelScrolling)
				return true;
			double scrollOffset = (double)((decimal)DataPresenter.ActualScrollOffset - (decimal)Math.Floor(DataPresenter.ActualScrollOffset));
			if((int)DataPresenter.ActualScrollOffset == 0 && GenerateItemsOffset > 0) {
				oldOffset = Math.Ceiling((double)GenerateItemsOffset / rowCardCount) + scrollOffset;
				DataPresenter.SetDefineScrollOffset(oldOffset);
				return false;
			}
			if(GenerateItemsOffset == 0 && DataPresenter.ActualScrollOffset > 1) {
				oldOffset = scrollOffset;
				DataPresenter.SetDefineScrollOffset(oldOffset);
				return false;
			}
			for(int i = 0; i < RowsInfo.Count; i++) {
				if(defineSize <= 0 && RowsInfo.Count - i > 1) {
					if(rest.HasValue) {
						double rowDefineSize = SizeHelper.GetDefineSize(GetRowInfo().RenderSize);
						AdjustScrollBar((rowDefineSize - rest.Value) / rowDefineSize);
						return false;
					}
					return true;
				}
				defineSize -= SizeHelper.GetDefineSize(RowsInfo[i].Size);
			}
			if(NodeContainer.IsFinished) {
				defineSize = Math.Round(defineSize, 4);
				if(DataPresenter.ActualScrollOffset == 0 || defineSize <= 0) {
					if(rest.HasValue && defineSize < 0) {
						AdjustScrollBar(-defineSize / SizeHelper.GetDefineSize(GetRowInfo().RenderSize));
						return false;
					}
					rest = null;
					return true;
				}
				if(-GetRowOffset() >= defineSize) {
					AdjustScrollBar(-defineSize / SizeHelper.GetDefineSize(GetRowInfo().RenderSize));
					return false;
				}
				rest = defineSize;
				DataPresenter.SetDefineScrollOffset(scrollOffset > 0 ? Math.Floor(DataPresenter.ActualScrollOffset) : DataPresenter.ActualScrollOffset - 1);
				NodeContainer.ReGenerateItems(GenerateItemsOffset, NodeContainer.ItemCount + 1, false);
				return false;
			}
			NodeContainer.GenerateItems(1);
			return false;
		}
		void AdjustScrollBar(double delta) {
			oldOffset = Math.Round(DataPresenter.ActualScrollOffset + delta, 4);
			DataPresenter.SetDefineScrollOffset(oldOffset);
			rest = null;
		}
		internal int GenerateItemsOffset { get { return Math.Max(0, Math.Min(actualGenerateItemsOffset, DataPresenter.ItemCount - 1)); } }
		int actualGenerateItemsOffset;
		double oldOffset;
		int scrollDiff;
		internal void OnDefineScrollInfoChanged() {
			if(DataPresenter == null || oldOffset == DataPresenter.ActualScrollOffset)
				return;
			actualGenerateItemsOffset = CalcGenerateItemsOffset(DataPresenter.ActualScrollOffset);
			if((int)DataPresenter.ActualScrollOffset - (int)oldOffset < 0 && RowsInfo.Count > 0 && AllowPerPixelScrolling)
				visibleIndex = GetControllerIndex(RowsInfo[0].Elements[0]);
			scrollDiff = (int)DataPresenter.ActualScrollOffset - (int)oldOffset;
			InvalidateMeasure();
		}
		bool CorrectGenerateItemsOffset(double defineSize) {
			if(DataPresenter == null)
				return true;
			if(!FillFirstRowIfNeeded()) {
				return false;
			}
			if(!CorrectGenerateItemsOffsetCore(defineSize)) {
				NodeContainer.ReGenerateItems(GenerateItemsOffset, NodeContainer.ItemCount, false);
				return false;
			}
			previousGenerateOffset = null;
			scrollDiff = 0;
			return true;
		}
		bool FillFirstRowIfNeeded() {
			if(!lastRowIndex.HasValue)
				return true;
			if(RowsInfo.Count == 0 || lastRowIndex == GetControllerIndex(RowsInfo[0].Elements.Last())) {
				if(actualGenerateItemsOffset <= 0 || RowsInfo[0].IsItemsContainer) {
					lastRowIndex = null;
					return true;
				}
				actualGenerateItemsOffset--;
				NodeContainer.ReGenerateItems(GenerateItemsOffset, NodeContainer.ItemCount + 1, false);
				return false;
			}
			actualGenerateItemsOffset++;
			NodeContainer.ReGenerateItems(GenerateItemsOffset, NodeContainer.ItemCount - 1, false);
			lastRowIndex = null;
			return false;
		}
		int? lastRowIndex;
		int? visibleIndex;
		bool CorrectGenerateItemsOffsetCore(double defineSize) {
			if(!visibleIndex.HasValue)
				return true;
			int rowIndex, itemIndex;
			if(FindRowItem(visibleIndex.Value, out rowIndex, out itemIndex)) {
				return EnsureOffset(rowIndex, itemIndex);
			}
			if(actualGenerateItemsOffset <= 0) {
				actualGenerateItemsOffset = 0;
				visibleIndex = null;
				return true;
			}
			lastRowIndex = GetControllerIndex(RowsInfo[0].Elements.Last());
			visibleIndex = null;
			return false;
		}
		int? previousGenerateOffset;
		bool EnsureOffset(int rowIndex, int itemIndex) {
			if(scrollDiff < 0 && rowIndex > -scrollDiff) {
				for(int i = 0; i < rowIndex + scrollDiff; i++) {
					actualGenerateItemsOffset += RowsInfo[i].Elements.Count;
				}
				return false;
			}
			if((itemIndex == 0 || rowIndex == 0) && actualGenerateItemsOffset <= 0) {
				actualGenerateItemsOffset = 0;
				visibleIndex = null;
				return false;
			}
			if(itemIndex == 0) {
				visibleIndex = null;
				if(RowsInfo[0].IsItemsContainer)
					return true;
				lastRowIndex = GetControllerIndex(RowsInfo[0].Elements.Last());
				return false;
			}
			if(rowIndex == 0) {
				previousGenerateOffset = actualGenerateItemsOffset;
				actualGenerateItemsOffset--;
				return false;
			}
			bool hasItemsContainer = false;
			for(int i = 0; i < rowIndex; i++)
				if(RowsInfo[i].IsItemsContainer) {
					hasItemsContainer = true;
					break;
				}
			if(!hasItemsContainer || previousGenerateOffset.HasValue)
					actualGenerateItemsOffset++;
			if(hasItemsContainer || (previousGenerateOffset.HasValue && previousGenerateOffset.Value == actualGenerateItemsOffset))
				visibleIndex = null;
			return false;
		}
		bool FindRowItem(int controllerIndex, out int rowIndex, out int itemIndex) {
			rowIndex = -1;
			itemIndex = -1;
			for(int i = 0; i < RowsInfo.Count; i++) {
				for(int j = 0; j < RowsInfo[i].Elements.Count; j++) {
					IItem item = RowsInfo[i].Elements[j];
					if(GetControllerIndex(item) == controllerIndex) {
						rowIndex = i;
						itemIndex = j;
						return true;
					}
				}
			}
			return false;
		}
		internal bool TryFindRowElement(int index, out int rowIndex, out int elementIndex) {
			rowIndex = 0;
			elementIndex = 0;
			for(int i = 0; i < RowsInfo.Count; i++) {
				for(int j = 0; j < RowsInfo[i].Elements.Count; j++)
					if(index == GetControllerIndex(RowsInfo[i].Elements[j])) {
						rowIndex = i;
						elementIndex = j;
						return true;
					}
			}
			return false;
		}
		int GetControllerIndex(IItem item) {
			return ((RowData)item).ControllerVisibleIndex;
		}		
		internal int CalcGenerateItemsOffset(double scrollOffset) {
			if(!AllowPerPixelScrolling)
				return (int)scrollOffset;
			int diff = (int)scrollOffset - (int)oldOffset;
			if(diff == 0)
				return actualGenerateItemsOffset;
			if(GroupInfo.Count == 0) {
				return CalcOffset(scrollOffset);
			}
			if(diff > 0) {
				return CalcGroupedOffsetForward(scrollOffset);
			}
			return CalcGroupedOffsetBack(scrollOffset);
		}
		int CalcOffset(double scrollOffset) {
			int diff = (int)scrollOffset - (int)oldOffset;
			int itemOffset = NodeContainer.StartScrollIndex;
			if(diff > 0 && diff < RowsInfo.Count) {
				for(int i = 0; i < diff; i++)
					itemOffset += RowsInfo[i].Elements.Count;
				return itemOffset;
			}
			if(diff < 0 && -diff < RowsInfo.Count) {
				for(int i = 0; i < -diff; i++)
					itemOffset -= RowsInfo[i].Elements.Count;
				return itemOffset;
			}
			return itemOffset + diff * rowCardCount;
		}
		int CalcGroupedOffsetForward(double scrollOffset) {
			int itemOffset = NodeContainer.StartScrollIndex;
			int rowOffset = (int)oldOffset;
			int groupIndex, rowIndex;
			FindGroupIndex(out groupIndex, out rowIndex);
			for(int i = 0; i < RowsInfo.Count - 1; i++) {
				CardRowInfo cardRowInfo = RowsInfo[i];
				if(rowOffset == (int)scrollOffset)
					return itemOffset;
				rowOffset++;
				itemOffset += cardRowInfo.Elements.Count;
				if(cardRowInfo.IsItemsContainer) {
					if(i != 0) groupIndex++;
					rowIndex = 0;
				}
				else {
					if(rowIndex == -1)
						rowIndex = 0;
					rowIndex += cardRowInfo.Elements.Count;
				}
			}
			for(int i = groupIndex; i < GroupInfo.Count; i++) {
				GroupRowInfo groupInfo = GroupInfo[i];
				if(rowOffset == (int)scrollOffset)
					break;
				if(i > groupIndex || rowIndex == -1) {
					itemOffset++;
					rowOffset++;
				}
				if(!groupInfo.Expanded || groupInfo.Level != LastGroupLevel)
					continue;
				int groupItemCount = groupInfo.ChildControllerRowCount;
				if(i == groupIndex && rowIndex > 0)
					groupItemCount -= rowIndex;
				int groupRowCount = (int)Math.Ceiling((double)groupItemCount / rowCardCount);
				if(rowOffset + groupRowCount > scrollOffset) {
					itemOffset += ((int)scrollOffset - rowOffset) * rowCardCount;
					break;
				}
				else {
					itemOffset += groupItemCount;
					rowOffset += groupRowCount;
				}
			}
			return itemOffset;
		}
		int CalcGroupedOffsetBack(double scrollOffset) {
			int itemOffset = NodeContainer.StartScrollIndex;
			int rowOffset = (int)oldOffset;
			int groupIndex, rowIndex;
			FindGroupIndex(out groupIndex, out rowIndex);
			for(int i = groupIndex; i >= 0; i--) {
				GroupRowInfo groupInfo = GroupInfo[i];
				if(rowOffset == (int)scrollOffset)
					break;
				if(groupInfo.Expanded && groupInfo.Level == LastGroupLevel) {
					int groupItemCount = groupInfo.ChildControllerRowCount;
					if(i == groupIndex) {
						groupItemCount = rowIndex >= 0 ? rowIndex : 0;
					}
					int groupRowCount = (int)Math.Ceiling((double)groupItemCount / rowCardCount);
					if(rowOffset - groupRowCount < scrollOffset) {
						itemOffset -= Math.Min(groupItemCount, (rowOffset - (int)scrollOffset) * rowCardCount);
						break;
					}
					else {
						itemOffset -= groupItemCount;
						rowOffset -= groupRowCount;
					}
				}
				if(rowOffset == (int)scrollOffset)
					break;
				if(i != groupIndex || rowIndex != -1) {
					itemOffset--;
					rowOffset--;
				}
			}
			return itemOffset;
		}
		void FindGroupIndex(out int groupIndex, out int rowIndex) {
			rowIndex = -1;
			groupIndex = 0;
			int itemOffset = 0;
			while(itemOffset < NodeContainer.StartScrollIndex) {
				itemOffset++;
				GroupRowInfo groupInfo = GroupInfo[groupIndex];
				if(!groupInfo.Expanded) {
					for(int j = groupIndex + 1; j < GroupInfo.Count; j++) {
						if(GroupInfo[j].Level - 1 != groupInfo.Level)
							break;
						groupIndex++;
					}
				}
				if(itemOffset == NodeContainer.StartScrollIndex) {
					if(groupInfo.Level == LastGroupLevel)
						rowIndex = 0;
					else
						groupIndex++;
					return;
				}
				if(!groupInfo.Expanded || groupInfo.Level != LastGroupLevel) {
					groupIndex++;
					continue;
				}
				if(itemOffset + groupInfo.ChildControllerRowCount > NodeContainer.StartScrollIndex) {
					rowIndex = NodeContainer.StartScrollIndex - itemOffset;
					return;
				}
				itemOffset += groupInfo.ChildControllerRowCount;
				groupIndex++;
			}
		}
		internal double CalcExtent(double defineSize, int? visibleIndex = null) {
			defineSize -= GetRowOffset();
			if(DataPresenter == null)
				return 0;
			if(!AllowPerPixelScrolling)
				return visibleIndex.HasValue ? visibleIndex.Value : DataPresenter.ItemCount;
			if(GroupInfo.Count == 0) {
				return CalcExtentUngrouped(defineSize, visibleIndex);
			}
			return CalcExtentGrouped(defineSize, visibleIndex);
		}
		double CalcExtentGrouped(double defineSize, int? visibleIndex) {
			int extent = (int)DataPresenter.ActualScrollOffset;
			int itemCount = NodeContainer.StartScrollIndex;
			int groupIndex, rowIndex;
			FindGroupIndex(out groupIndex, out rowIndex);
			if(visibleIndex.HasValue && visibleIndex < itemCount) {
				for(int i = groupIndex; i >= 0; i--) {
					GroupRowInfo groupInfo = GroupInfo[i];
					if(itemCount == visibleIndex)
						break;
					if(groupInfo.Expanded && groupInfo.Level == LastGroupLevel) {
						int groupItemCount = groupInfo.ChildControllerRowCount;
						if(i == groupIndex) {
							groupItemCount = rowIndex >= 0 ? rowIndex : 0;
						}
						if(itemCount - visibleIndex <= groupItemCount) {
							extent -= (int)Math.Ceiling((double)(itemCount - visibleIndex.Value) / rowCardCount);
							break;
						}
						else {
							itemCount -= groupItemCount;
							extent -= (int)Math.Ceiling((double)groupItemCount / rowCardCount);
						}
					}
					if(i != groupIndex || rowIndex != -1) {
						itemCount--;
						extent--;
					}
				}
				return extent;
			}
			for(int i = 0; i < RowsInfo.Count; i++) {
				CardRowInfo cardRowInfo = RowsInfo[i];
				if(defineSize <= 0) break;
				itemCount += cardRowInfo.Elements.Count;
				if(visibleIndex < itemCount)
					return extent;
				extent++;
				if(itemCount >= DataPresenter.ItemCount)
					return extent;
				if(cardRowInfo.IsItemsContainer) {
					if(rowIndex != -1) {
						groupIndex++;
						rowIndex = -1;
					}
					GroupRowInfo groupInfo = GroupInfo[groupIndex];
					if(!groupInfo.Expanded) {
						for(int j = groupIndex + 1; j < GroupInfo.Count; j++) {
							if(GroupInfo[j].Level - 1 != groupInfo.Level)
								break;
							groupIndex++;
						}
					}
					if(groupInfo.Level != LastGroupLevel || !groupInfo.Expanded)
						groupIndex++;
					else
						rowIndex = 0;
				}
				else {
					if(rowIndex == -1)
						rowIndex = 0;
					rowIndex += cardRowInfo.Elements.Count;
				}
				defineSize -= SizeHelper.GetDefineSize(cardRowInfo.RenderSize);
			}
			for(int i = groupIndex; i < GroupInfo.Count; i++) {
				GroupRowInfo groupInfo = GroupInfo[i];
				if(groupInfo.ParentGroup != null && !groupInfo.ParentGroup.Expanded) continue;
				if(i > groupIndex || rowIndex == -1) {
					extent++;
				}
				if(!groupInfo.Expanded || groupInfo.Level != LastGroupLevel)
					continue;
				int groupItemCount = groupInfo.ChildControllerRowCount;
				if(i == groupIndex && rowIndex > 0)
					groupItemCount -= rowIndex;
				int groupRowCount = (int)Math.Ceiling((double)groupItemCount / rowCardCount);
				extent += groupRowCount;
			}
			return extent;
		}
		double CalcExtentUngrouped(double defineSize, int? visibleIndex) {
			int extent = (int)DataPresenter.ActualScrollOffset;
			int itemCount = NodeContainer.StartScrollIndex;
			if(visibleIndex.HasValue && visibleIndex < itemCount) {
				return extent - Math.Ceiling((double)(itemCount - visibleIndex.Value) / rowCardCount);
			}
			foreach(CardRowInfo cardRowInfo in RowsInfo) {
				if(defineSize <= 0) break;
				itemCount += cardRowInfo.Elements.Count;
				if(visibleIndex.HasValue && itemCount - 1 >= visibleIndex.Value)
					return extent;
				extent++;
				defineSize -= SizeHelper.GetDefineSize(cardRowInfo.RenderSize);
			}
			int totalCount = visibleIndex.HasValue ? visibleIndex.Value : DataPresenter.ItemCount;
			return Math.Ceiling(extent + (double)(totalCount - itemCount) / rowCardCount);
		}
		int rowCardCount;
		internal double Extent { get; private set; }
		GroupRowInfoCollection GroupInfo { get { return DataPresenter.DataControl.DataProviderBase.DataController.GroupInfo; } }
		int LastGroupLevel { get { return DataPresenter.DataControl.DataProviderBase.DataController.GroupedColumnCount - 1; } }
		protected override void CalcViewportCore(Size availableSize) {
			if(ScrollOutOfBounds())
				return;
			Viewport = 0;
			double cardSize = 0;
			int cardCount = 0;
			double defineSize = SizeHelper.GetDefineSize(availableSize);
			foreach(CardRowInfo rowInfo in RowsInfo) {
				double actualDefineSize = SizeHelper.GetDefineSize(rowInfo.RenderSize);
				if(rowInfo == GetRowInfo())
					actualDefineSize += GetRowOffset();
				double desiredDefineSize = SizeHelper.GetDefineSize(rowInfo.Size);
				double visiblePart = Math.Max(0, Math.Min(actualDefineSize / desiredDefineSize, defineSize / desiredDefineSize));
				if(visiblePart == 1 || AllowPerPixelScrolling)
					Viewport += visiblePart * (AllowPerPixelScrolling ? 1 : rowInfo.Elements.Count);
				if(visiblePart == 1)
					FullyVisibleItemsCount += rowInfo.Elements.Count;
				if(!rowInfo.IsItemsContainer && defineSize >= 0)
					foreach(IItem item in rowInfo.Elements) {
						cardCount += 1;
						cardSize += SizeHelper.GetSecondarySize(item.Element.DesiredSize) + SizeHelper.GetSecondarySize(CardMarginCached);
					}
				defineSize -= actualDefineSize;
			}
			rowCardCount = Math.Min(Math.Max(1, (int)Math.Floor(SizeHelper.GetSecondarySize(availableSize) / cardSize * cardCount)), MaxCardCountInRow);
			Viewport = Math.Round(Viewport, 4);
			Extent = CalcExtent(SizeHelper.GetDefineSize(availableSize));
		}
		List<CardRowInfo> MeasureItemsContainer(double offset, Size availableSize, IItemsContainer itemsContainer, int level) {
			List<CardRowInfo> rows = new List<CardRowInfo>();
			double maxSecondarySize = 0, defineSize = 0, maxDefineSize = 0, secondarySize = 0;
			foreach(IItem item in GetSortedChildrenElements(itemsContainer)) {
				bool newRow = MeasureItem(item, availableSize, rows.Count > 0 ? rows.Last().Elements.Count : 0, ref defineSize, ref secondarySize, ref maxDefineSize, ref maxSecondarySize);
				if(newRow && !item.IsItemsContainer && rows.Count != 0)
					defineSize += SeparatorThickness;
				if(item.IsRowVisible) {
					CardRowInfo currentRowInfo = GetOrCreateCurrentRowInfo(newRow, level, item.IsItemsContainer, rows);
					currentRowInfo.Elements.Add(item);
					UpdateRowInfo(currentRowInfo, maxDefineSize, secondarySize);
				} else {
					hiddenItems.Add(item.Element);
				}
				rows.AddRange(MeasureItemsContainer(defineSize + offset, GetContainerSize(availableSize, defineSize), item.ItemsContainer, level + 1));
				defineSize += SizeHelper.GetDefineSize(item.ItemsContainer.DesiredSize);
				double containerSecondarySize = SizeHelper.GetSecondarySize(item.ItemsContainer.DesiredSize);
				if(containerSecondarySize != 0)
					maxSecondarySize = Math.Max(maxSecondarySize, containerSecondarySize + ContainerIndent);
			}
			defineSize += maxDefineSize;
			itemsContainer.RenderSize = SizeHelper.CreateSize(defineSize, maxSecondarySize);
			double containerSize = Math.Ceiling(defineSize * itemsContainer.AnimationProgress);
			itemsContainer.DesiredSize = SizeHelper.CreateSize(containerSize, maxSecondarySize);
			UpdateRowInfoSize(rows, containerSize);
			return rows;
		}
		bool MeasureItem(IItem item, Size availableSize, int currentRowCount, ref double defineSize, ref double secondarySize, ref double maxDefineSize, ref double maxSecondarySize) {
			item.Element.Measure(GetMeasureSize(item));
			if(!item.IsRowVisible) return false;
			double defineElementWidth = GetDefineSize(item);
			double secondaryElementSize = SizeHelper.GetSecondarySize(item.Element.DesiredSize);
			if(!item.IsItemsContainer) {
				defineElementWidth += SizeHelper.GetDefineSize(CardMarginCached);
				secondaryElementSize += SizeHelper.GetSecondarySize(CardMarginCached);
			}
			bool newRow = item.IsItemsContainer || secondaryElementSize + secondarySize > SizeHelper.GetSecondarySize(availableSize) || currentRowCount == MaxCardCountInRow;
			if(newRow) {
				secondarySize = secondaryElementSize;
				defineSize += maxDefineSize;
				maxDefineSize = defineElementWidth;
			} else {
				secondarySize += secondaryElementSize;
				maxDefineSize = Math.Max(maxDefineSize, defineElementWidth);
			}
			maxSecondarySize = Math.Max(maxSecondarySize, secondarySize);
			return newRow;
		}
		CardRowInfo GetOrCreateCurrentRowInfo(bool newRow, int level, bool isItemsContainer, List<CardRowInfo> rows) {
			if(newRow || rows.Count == 0) {
				if(rows.Count != 0) {
					CardRowInfo prevRowInfo = rows.Last();
					if(prevRowInfo.Level == level && !prevRowInfo.IsItemsContainer) {
						prevRowInfo.HasSeparator = true;
						UpdateRowInfo(prevRowInfo, SizeHelper.GetDefineSize(prevRowInfo.Size) + SeparatorThickness, SizeHelper.GetSecondarySize(prevRowInfo.Size));
					}
				}
				CardRowInfo rowInfo = new CardRowInfo() { Elements = new List<IItem>(), Level = level, IsItemsContainer = isItemsContainer, HasSeparator = false };
				rows.Add(rowInfo);
				return rowInfo;
			} else {
				return rows.Last();
			}
		}
		void UpdateRowInfo(CardRowInfo rowInfo, double maxDefineSize, double secondarySize) {
			rowInfo.Size = SizeHelper.CreateSize(maxDefineSize, secondarySize);
			rowInfo.RenderSize = rowInfo.Size;
		}
		void UpdateRowInfoSize(List<CardRowInfo> info, double availableSize) {
			foreach(CardRowInfo rowInfo in info) {
				rowInfo.RenderSize = SizeHelper.CreateSize(Math.Min(SizeHelper.GetDefineSize(rowInfo.RenderSize), availableSize), SizeHelper.GetSecondarySize(rowInfo.RenderSize));
				availableSize -= SizeHelper.GetDefineSize(rowInfo.RenderSize);
			}
		}
		Size GetMeasureSize(IItem item) {
			return SizeHelper.CreateSize(item.IsItemsContainer || double.IsNaN(FixedSize) ? double.PositiveInfinity : FixedSize, double.PositiveInfinity);
		}
		double GetDefineSize(IItem item) {
			if(item.IsItemsContainer || double.IsNaN(FixedSize))
				return SizeHelper.GetDefineSize(item.Element.DesiredSize);
			return FixedSize;
		}
		Size GetContainerSize(Size availableSize, double defineSize) {
			return SizeHelper.CreateSize(Math.Max(0, SizeHelper.GetDefineSize(availableSize) - defineSize), Math.Max(0, SizeHelper.GetSecondarySize(availableSize) - ContainerIndent));
		}
		internal int RowItemCount { get { return RowsInfo != null && RowsInfo.Count != 0 ? GetRowInfo().Elements.Count : 0; } }
		CardRowInfo GetRowInfo() {
			return RowsInfo.First();
		}
		internal double GetRowOffset() {
			if(RowsInfo == null || RowsInfo.Count == 0 || DataPresenter == null) return 0;
			return Math.Round((Math.Floor(DataPresenter.ActualScrollOffset) - DataPresenter.ActualScrollOffset) * SizeHelper.GetDefineSize(GetRowInfo().Size));
		}
		protected override Size ArrangeItemsContainer(double offset, Size availableSize, IItemsContainer itemsContainer, int level) {
			if(itemsContainer == null)
				return new Size();
			double correctedOffset = offset;
			foreach(CardRowInfo rowInfo in RowsInfo) {
				if(SizeHelper.GetDefineSize(rowInfo.RenderSize) > 0)
					correctedOffset = offset;
				double secondarySize = GetArrangeSecondaryOffset(rowInfo, availableSize);
				foreach(IItem item in rowInfo.Elements) {
					Point location = SizeHelper.CreatePoint(correctedOffset + GetRowOffset(), secondarySize);
					double defineSize = SizeHelper.GetDefineSize(rowInfo.RenderSize);
					if(!item.IsItemsContainer) {
						location.Offset(CardMargin.Left, CardMargin.Top);
						defineSize = Math.Max(0, defineSize - SizeHelper.GetDefineSize(new Size(CardMargin.Left, CardMargin.Top)));
						if(rowInfo.HasSeparator)
							defineSize = Math.Max(0, defineSize - Math.Min(SeparatorThickness, SeparatorThickness + SizeHelper.GetDefineSize(rowInfo.Size) - SizeHelper.GetDefineSize(rowInfo.RenderSize)));
						double margin = SizeHelper.GetDefineSize(new Size(CardMargin.Right, CardMargin.Bottom));
						double rest = SizeHelper.GetDefineSize(rowInfo.Size) - SizeHelper.GetDefineSize(rowInfo.RenderSize);
						defineSize = Math.Max(0, defineSize - Math.Max(0, margin - rest));
					}
					Size size = SizeHelper.CreateSize(defineSize, item.IsItemsContainer ? SizeHelper.GetSecondarySize(availableSize) - rowInfo.Level * ContainerIndent : SizeHelper.GetSecondarySize(item.Element.DesiredSize));
					ArrangeItem(item, new Rect(location, size), SizeHelper.GetDefineSize(size) > 0, rowInfo.Level);
					secondarySize += SizeHelper.GetSecondarySize(item.Element.DesiredSize);
					if(!item.IsItemsContainer)
						secondarySize += SizeHelper.GetSecondarySize(CardMarginCached);
				}
				offset += SizeHelper.GetDefineSize(rowInfo.RenderSize);
				correctedOffset += SizeHelper.GetDefineSize(rowInfo.Size);
			}
			foreach(UIElement element in hiddenItems) {
				element.Arrange(new Rect(new Point(double.MinValue, double.MinValue), element.DesiredSize));
			}
			return itemsContainer.DesiredSize;
		}
		double GetArrangeSecondaryOffset(CardRowInfo rowInfo, Size availableSize) {
			double size = SizeHelper.GetSecondarySize(availableSize);
			double offset = 0;
			if(Orientation == Orientation.Vertical)
				offset += ContainerIndent * rowInfo.Level;
			else
				size -= ContainerIndent * rowInfo.Level;
			if(rowInfo.IsItemsContainer) return offset;
			double rest = size - SizeHelper.GetSecondarySize(rowInfo.Size) - offset;
			if(CardAlignment == Alignment.Center)
				offset += Math.Floor(rest / 2);
			else if(CardAlignment == Alignment.Far) {
				offset += rest;
			}
			return offset;
		}
		void UpdateSeparatorInfo(Size availableSize) {
			if(SeparatorInfo == null)
				SeparatorInfo = new ObservableCollection<SeparatorInfo>();
			int index = 0;
			double defineSize = 0;
			foreach(CardRowInfo rowInfo in RowsInfo) {
				if(rowInfo.IsItemsContainer || !rowInfo.HasSeparator) {
					defineSize += SizeHelper.GetDefineSize(rowInfo.Size);
					continue;
				}
				if(SeparatorInfo.Count == index)
					SeparatorInfo.Add(new SeparatorInfo() { RowIndex = index + 1 });
				SeparatorInfo separatorInfo = SeparatorInfo[index];
				separatorInfo.Orientation = Orientation;
				separatorInfo.Length = SizeHelper.GetSecondarySize(availableSize) - rowInfo.Level * ContainerIndent;
				defineSize += SizeHelper.GetDefineSize(rowInfo.Size) - SeparatorThickness;
				Point point = SizeHelper.CreatePoint(defineSize + GetRowOffset(), 0);
				separatorInfo.Margin = new Thickness(point.X, point.Y, 0, 0);
				separatorInfo.IsVisible = SeparatorThickness - SizeHelper.GetDefineSize(rowInfo.Size) + SizeHelper.GetDefineSize(rowInfo.RenderSize) >= 0;
				defineSize += SeparatorThickness;
				index++;
			}
			for(int i = index; i < SeparatorInfo.Count; i++)
				SeparatorInfo[i].IsVisible = false;
		}
	}
}
