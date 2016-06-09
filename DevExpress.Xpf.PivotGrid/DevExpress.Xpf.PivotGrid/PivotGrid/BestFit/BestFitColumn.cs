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
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.XtraPivotGrid.Data;
using System.Collections;
using System.Collections.Generic;
using DevExpress.Data.PivotGrid;
namespace DevExpress.Xpf.PivotGrid.Internal {
	public enum BestFitColumnDecorator { ColumnDecorator, RowDecorator };
	public class BestFitColumn : IBestFitColumn {
		PivotGridWpfData data;
		int index;
		PivotFieldValueItem valueItem;
		List<PivotFieldValueItem> valueItems;
		public BestFitColumn(PivotGridWpfData data, int index) {
			if(data == null)
				throw new ArgumentNullException("data");
			if(!CheckIndex(data, index))
				throw new ArgumentException("incorrect index");
			this.data = data;
			this.index = index;
		}
		public BestFitMode AppliedBestFitMode { get; set; }
		protected PivotGridWpfData Data { get { return data; } }
		protected PivotVisualItems VisualItems { get { return Data.VisualItems; } }
		protected void SetIndex(int index) { this.index = index; }
		public int Index { get { return index; } }
		public virtual bool IsColumn { get { return true; } }
		public bool IsRowTree { get { return IsRowTreeCore(Data); } }
		protected virtual bool IsRowTreeCore(PivotGridWpfData data) {
			return false;
		}
		public virtual BestFitColumnDecorator Decorator { get { return BestFitColumnDecorator.ColumnDecorator; } }
		protected virtual PivotFieldValueItem ValueItem {
			get {
				if(valueItem == null)
					valueItem = VisualItems.GetLastLevelItem(IsColumn, Index);
				return valueItem;
			}
		}
		public virtual List<PivotFieldValueItem> ValueItems {
			get {
				if(valueItems == null) 
					valueItems = GetValueItems();
				return valueItems;
			}
		}		
		public virtual PivotGridField ResizingField {
			get { return ValueItem.ResizingField != null ? ((PivotFieldItem)ValueItem.ResizingField).Wrapper : null; }
		}
		public virtual FieldBestFitArea BestFitArea {
			get { return BestFitAreaCore & ~DenyBestFitArea; }
		}
		public virtual FieldBestFitArea DenyBestFitArea {
			get { return FieldBestFitArea.FieldHeader; }
		}
		protected FieldBestFitArea BestFitAreaCore {
			get { return ResizingField != null ? ResizingField.BestFitArea : FieldBestFitArea.All; }
		}
		protected virtual bool CheckIndex(PivotGridWpfData data, int index) {
			return index >= 0 && index < data.VisualItems.ColumnCount;
		}
		#region IBestFitColumn Members
		int IBestFitColumn.BestFitMaxRowCount {
			get { return ResizingField != null ? ResizingField.BestFitMaxRowCount : BestWidthCalculator.DefaultBestFitMaxRowCount; }
		}
		BestFitMode IBestFitColumn.BestFitMode {
			get { return ResizingField != null ? ResizingField.BestFitMode : BestFitMode.Default; }
		}
		#endregion
		public virtual int RowCount { get { return VisualItems.RowCount; } }
		public virtual object GetCellValue(int rowIndex) {
			return VisualItems.GetCellValue(Index, rowIndex);
		}
		public virtual ScrollableAreaItemBase GetCellsAreaItem(int rowIndex) {
			return new CellsAreaItem(VisualItems, Index, rowIndex);
		}
		public virtual bool SetSize(int newSize) {
			return SetSizeCore(ValueItem, null, newSize);
		}
		protected bool SetSizeCore(PivotFieldValueItem item, ResizingFieldsCache resizingFieldsCache, int newSize) {
			System.Drawing.Size size = VisualItems.GetLastLevelItemSize(item);
			bool changed = SetColumnSizeCore(ref size, newSize);
			if(changed)
				VisualItems.SetItemSize(item, size, resizingFieldsCache);
			return changed;
		}
		protected virtual bool SetColumnSizeCore(ref System.Drawing.Size size, int newSize) {
			if(size.Width == newSize) return false;
			size.Width = newSize;
			return true;
		}
		public virtual object[] GetUniqueValues() {
			int rowCount = RowCount;
#if !SL
			NullableHashtable hash = new NullableHashtable(rowCount);
#else
			NullableHashtable hash = new NullableHashtable(rowCount, EqualityComparer<object>.Default);
#endif
			for(int i = 0; i < rowCount; i++) {
				object value = GetCellValue(i);
				hash[value] = i;
			}
			object[] res = new object[hash.Count];
			hash.CopyValuesTo(res, 0);
			return res;
		}
		protected virtual List<PivotFieldValueItem> GetValueItems() {
			List<PivotFieldValueItem> res = new List<PivotFieldValueItem>();
			res.Add(ValueItem);
			PivotFieldValueItem parentItem = ValueItem;
			while((parentItem = VisualItems.GetParentItem(IsColumn, parentItem)) != null) {
				if(parentItem.MinLastLevelIndex != parentItem.MaxLastLevelIndex) break;
				res.Add(parentItem);
			}
			return res;
		}
		public virtual void GetCellIndex(int rowHandle, out int columnIndex, out int rowIndex) {
			columnIndex = Index;
			rowIndex = rowHandle;
		}
		public bool SetSizes(BestFitResult result, bool withChildren) {
			return SetItemsSize(result, withChildren);
		}
		bool SetItemsSize(BestFitResult result, bool withChildren) {
			if(result == null) return false;
			List<PivotFieldValueItem> items = result.ValueItems;
			List<int> newSizes = result.NewSizes;
			CheckItemsCount(items, newSizes);
			ResizingFieldsCache resFieldsCache = new ResizingFieldsCache();
			bool changed = false;
			for(int i = 0; i < items.Count; i++) {
				PivotFieldValueItem item = items[i];
				int newSize = newSizes[i];
				if(newSize <= 0) continue;
				changed |= SetSizeCore(item, resFieldsCache, newSize);
				int cellCount = item.CellCount;
				if(withChildren && cellCount > 0) {
					List<PivotFieldValueItem> childItems = new List<PivotFieldValueItem>(cellCount);
					List<int> newChildSizes = new List<int>(cellCount);
					double childrenSumSize = 0.0;
					for(int j = 0; j < cellCount; j++) {
						System.Drawing.Size size = VisualItems.GetLastLevelItemSize(item.GetCell(j));
						childrenSumSize += size.Width;
						newChildSizes.Add(size.Width);
					}
					for(int j = 0; j < cellCount; j++) {
						childItems.Add(item.GetCell(j));
						newChildSizes[j] = GetCorrectedChildSize(newChildSizes[j], newSize, childrenSumSize);
					}
					changed |= SetItemsSize(new BestFitResult(){ ValueItems = childItems, NewSizes = newChildSizes}, withChildren);
				}
			}
			resFieldsCache.RaiseFieldSizeChangedEvents(Data);
			return changed;
		}
		protected static void CheckItemsCount(List<PivotFieldValueItem> items, List<int> newSizes) {
			if(newSizes == null || newSizes.Count != items.Count)
				throw new ArgumentOutOfRangeException("Incorrect newSizes count (" + newSizes.Count + ")");
		}
		protected static int GetCorrectedChildSize(int childSize, int newSize, double childrenSumSize) {
			return (int)Math.Ceiling(childSize * newSize / childrenSumSize);
		}
	}
	public class BestFitRow : BestFitColumn {
		public BestFitRow(PivotGridWpfData data, int index)
			: base(data, index) {
		}
		public override bool IsColumn { get { return false; } }
		public override BestFitColumnDecorator Decorator { get { return BestFitColumnDecorator.RowDecorator; } }
		protected override bool IsRowTreeCore(PivotGridWpfData data) {
			return data.IsRowTree;
		}
		protected override bool CheckIndex(PivotGridWpfData data, int index) {
			return index >= 0 && index < data.VisualItems.RowCount;
		}
		public override int RowCount { get { return VisualItems.ColumnCount; } }
		public override object GetCellValue(int rowIndex) {
			return VisualItems.GetCellValue(rowIndex, Index);
		}
		public override ScrollableAreaItemBase GetCellsAreaItem(int rowIndex) {
			return new CellsAreaItem(VisualItems, rowIndex, Index);
		}
		protected override bool SetColumnSizeCore(ref System.Drawing.Size size, int newSize) {
			if(size.Height == newSize) return false;
			size.Height = newSize;
			return true;
		}
		public override void GetCellIndex(int rowHandle, out int columnIndex, out int rowIndex) {
			columnIndex = rowHandle;
			rowIndex = Index;
		}
		public override FieldBestFitArea BestFitArea {
			get {
				return FieldBestFitArea.All & ~FieldBestFitArea.FieldHeader;
			}
		}
		public override FieldBestFitArea DenyBestFitArea {
			get { return FieldBestFitArea.FieldHeader; }
		}
	}
	public class RowFieldBestFitColumn : BestFitColumn {
		const int SkippedIndex = -2;
		PivotGridField resizingField;
		public RowFieldBestFitColumn(PivotGridWpfData data, int index) 
			: base(data, index) {
			this.resizingField = data.GetFieldByArea(Area, index);
		}
		public RowFieldBestFitColumn(PivotGridWpfData data, PivotGridField resizingField)
			: base(data, (resizingField.Area == FieldArea.DataArea || (resizingField.InternalField.IsDataField && resizingField.AreaIndex < 0)) ? SkippedIndex : resizingField.AreaIndex) {
			FieldArea area = resizingField.Area;
			if(area != FieldArea.DataArea && area.ToPivotArea() != Area)
				throw new ArgumentException("resizingField.Area");
			this.resizingField = resizingField;
		}
		public override bool IsColumn { get { return false; } }
		public override int RowCount { get { return ValueItems.Count; } }
		public override PivotGridField ResizingField { get { return resizingField; } }
		public override BestFitColumnDecorator Decorator { get { return BestFitColumnDecorator.RowDecorator; } }
		public override FieldBestFitArea BestFitArea {
			get { return BestFitAreaCore & ~FieldBestFitArea.Cell; }
		}
		public override FieldBestFitArea DenyBestFitArea {
			get { return FieldBestFitArea.Cell; }
		}
		protected override bool IsRowTreeCore(PivotGridWpfData data) {
			return data.IsRowTree && (resizingField == null || resizingField.Area == FieldArea.RowArea || 
				resizingField != null && resizingField.Area == FieldArea.DataArea && data.DataFieldArea == XtraPivotGrid.PivotDataArea.RowArea);
		}
		protected virtual DevExpress.XtraPivotGrid.PivotArea Area {
			get { return DevExpress.XtraPivotGrid.PivotArea.RowArea; }
		}
		protected override PivotFieldValueItem ValueItem { get { return null; } }
		protected override bool CheckIndex(PivotGridWpfData data, int index) {
			return index == SkippedIndex || (index >= 0 && index < data.GetFieldsByArea(Area, true).Count) ||
				(IsRowTreeCore(data) && index == data.VisualItems.RowTreeField.AreaIndex);
		}
		protected override List<PivotFieldValueItem> GetValueItems() {
			List<PivotFieldValueItem> res = new List<PivotFieldValueItem>();
			int count = VisualItems.GetItemCount(IsColumn);
			for(int i = 0; i < count; i++) {
				PivotFieldValueItem item = VisualItems.GetItem(IsColumn, i);
				if(ResizingField.FieldItem.Equals(item.ResizingField) ||
					ResizingField.FieldItem.IsDataField && item.ResizingField != null && item.ResizingField.Area == XtraPivotGrid.PivotArea.DataArea)
					res.Add(item);
			}
			return res;
		}
		public override object[] GetUniqueValues() {
			int count = ValueItems.Count;
#if !SL
			NullableHashtable hash = new NullableHashtable(count);
#else
			NullableHashtable hash = new NullableHashtable(count, EqualityComparer<object>.Default);
#endif
			for(int i = 0; i < count; i++) {
				hash[ValueItems[i].Value] = true;
			}
			object[] res = new object[hash.Count];
			hash.CopyKeysTo(res, 0);
			return res;
		}
		public override bool SetSize(int newSize) {
			ResizingField.Width = newSize;
			return true;
		}
		public override void GetCellIndex(int rowHandle, out int columnIndex, out int rowIndex) {
			throw new Exception("Can't best fit cells for this column");
		}
		protected override bool SetColumnSizeCore(ref System.Drawing.Size size, int newSize) {
			if(size.Height == newSize) return false;
			size.Height = newSize;
			return true;
		}
	}
	public class ColumnFieldBestFitColumn : RowFieldBestFitColumn {
		public ColumnFieldBestFitColumn(PivotGridWpfData data, int index) 
			: base(data, index) {
		}
		public ColumnFieldBestFitColumn(PivotGridWpfData data, PivotGridField resizingField)
			: base(data, resizingField) {
		}
		public override bool IsColumn { get { return true; } }
		public override FieldBestFitArea BestFitArea {
			get { return BestFitAreaCore & ~FieldBestFitArea.Cell & ~FieldBestFitArea.FieldHeader; }
		}
		public override FieldBestFitArea DenyBestFitArea {
			get { return FieldBestFitArea.Cell | FieldBestFitArea.FieldHeader; }
		}
		public override bool SetSize(int newSize) {
			ResizingField.Height = newSize;
			return true;
		}
		protected override bool IsRowTreeCore(PivotGridWpfData data) {
			return false;
		}
		protected override DevExpress.XtraPivotGrid.PivotArea Area {
			get { return DevExpress.XtraPivotGrid.PivotArea.ColumnArea; }
		}
		protected override bool SetColumnSizeCore(ref System.Drawing.Size size, int newSize) {
			if(size.Width == newSize) return false;
			size.Width = newSize;
			return true;
		}
		public override BestFitColumnDecorator Decorator { get { return BestFitColumnDecorator.ColumnDecorator; } }
	}
	public class BestFitLevelFieldValues : BestFitColumn {
		int level;
		public BestFitLevelFieldValues(PivotGridWpfData data, int level)
			: base(data, 0) {
			this.level = level;
		}
		public override FieldBestFitArea BestFitArea { get { return FieldBestFitArea.FieldValue; } }
		public override FieldBestFitArea DenyBestFitArea {
			get { return FieldBestFitArea.Cell | FieldBestFitArea.FieldHeader; }
		}
		public void UpdateSizes(BestFitResult currentLevelSizes, BestFitResult downLevelSizes) {
			UpdateSizesCore(ValueItems, currentLevelSizes, downLevelSizes);
		}
		protected override List<PivotFieldValueItem> GetValueItems() {
			List<PivotFieldValueItem> res = new List<PivotFieldValueItem>();
			int count = VisualItems.GetItemCount(IsColumn);
			for(int i = 0; i < count; i++) {
				PivotFieldValueItem item = VisualItems.GetItem(IsColumn, i);
				if(item.ContainsLevel(level))
					res.Add(item);
			}
			return res;
		}
		void UpdateSizesCore(List<PivotFieldValueItem> items, BestFitResult currentLevel, BestFitResult downLevel) {
			items = currentLevel.ValueItems;
			List<int> currentLevelSizes = currentLevel.NewSizes;
			List<int> downLevelSizes = downLevel.NewSizes;
			CheckItemsCount(items, currentLevelSizes);
			for(int i = 0, downLevelIndex = 0; i < items.Count; i++) {
				PivotFieldValueItem item = items[i];
				if(item.CellCount > 0 && downLevelIndex + item.CellCount <= downLevelSizes.Count) {
					int childsSumSize = 0, levelCellCount = 0;
					for(int k = 0; k < item.CellCount; k++) {
						if(level + 1 == item.GetCell(k).Level) {
							childsSumSize += downLevelSizes[downLevelIndex + levelCellCount];
							levelCellCount++;
						}
					}
					if(currentLevelSizes[i] > childsSumSize) {
						for(int k = 0; k < levelCellCount; k++) {
							downLevelSizes[downLevelIndex + k] = GetCorrectedChildSize(downLevelSizes[downLevelIndex + k], currentLevelSizes[i], childsSumSize);
						}
					}
					downLevelIndex += levelCellCount;
				} else {
					downLevelIndex++;
				}
			}
		}
		public void UpdateMaxSizes(List<int> dest, List<int> source) {
			if(dest.Count != source.Count)
				throw new ArgumentOutOfRangeException("Incorrect element count (" + dest.Count + " != "+ source.Count + ")");
			for(int i = 0; i < dest.Count; i++) {
				dest[i] = Math.Max(dest[i], source[i]);
			}			
		}
		public override bool SetSize(int newSize) {
			PivotGridField field = Data.GetFieldByLevel(true, level);
			if(field == null) return false;
			field.Height = newSize;
			return true;
		}
	}
	public class BestFitData : BestFitColumn {
		BestFitResult bestFitResult;
		int index = 0;
		public BestFitData(PivotGridWpfData data, BestFitResult bestFitResult)
			: base(data, bestFitResult.ValueItems.Count > 0 ? bestFitResult.ValueItems[0].MinLastLevelIndex : 0) {
				this.bestFitResult = bestFitResult;
		}
		public override FieldBestFitArea BestFitArea { get { return FieldBestFitArea.Cell; } }
		public override FieldBestFitArea DenyBestFitArea {
			get { return FieldBestFitArea.FieldValue | FieldBestFitArea.FieldHeader; }
		}
		protected override List<PivotFieldValueItem> GetValueItems() { return null; }
		public bool MoveNext() {
			bool hasNext = index + 1 < bestFitResult.ValueItems.Count;
			if(hasNext) {
				index++;
				SetIndex(bestFitResult.ValueItems[index].MinLastLevelIndex);
			}
			return hasNext;
		}
	}
}
