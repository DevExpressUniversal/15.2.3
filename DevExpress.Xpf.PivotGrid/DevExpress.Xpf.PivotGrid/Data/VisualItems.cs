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
using DevExpress.Utils;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using CoreXtraPivotGrid = DevExpress.XtraPivotGrid;
using System.Drawing;
namespace DevExpress.Xpf.PivotGrid.Internal {
	public class PivotVisualItems : SelectionVisualItems {
		readonly CellSizeProvider cellSizeProvider;
		WeakEventHandler<EventArgs, ItemsEmptyEventHandler> areaItemsChanged;
		public new PivotGridWpfData Data { get { return (PivotGridWpfData)base.Data; } }
		public CellSizeProvider CellSizeProvider { get { return cellSizeProvider; } }
		public PivotVisualItems(PivotGridWpfData data)
			: base(data) {
			data.FieldSizeChanged += OnFieldSizeChanged;
			cellSizeProvider = new CellSizeProvider(data, this);
		}
		protected override int ViewportHeight {
			get {
				PivotGridScroller scroller = Data.PivotGrid != null ? Data.PivotGrid.PivotGridScroller : null;
				if(scroller == null || scroller.ScrollableHeight == 0.0)
					return 0;
				return scroller.ViewportHeight;
			}
		}
		protected override PivotFieldItemBase CreateFieldItem(PivotGridData data, PivotGroupItemCollection groupItems, CoreXtraPivotGrid.PivotGridFieldBase field) {
			return new PivotFieldItem(data, groupItems, (PivotGridInternalField)field);
		}
		public void StartAddSelection() {
			InnerSelection.StartSelection(true, false, true, true);
		}
		public event ItemsEmptyEventHandler AreaItemsChanged {
			add { areaItemsChanged += value; }
			remove { areaItemsChanged -= value; }
		}
		public new PivotGridField RowTreeField {
			get { return base.RowTreeField.GetWrapper(); }
		}
		protected virtual PivotOlapMember CreateOlapMember(CoreXtraPivotGrid.IOLAPMember member) {
			return new PivotOlapMember(member);
		}
		protected override CoreXtraPivotGrid.PivotGridFieldBase CreateRowTreeField() {
			return new PivotTreeRowInternalField(this);
		}
		protected override void PrepareRowTreeFieldAfterCreating() {
			new PivotTreeRowField((PivotTreeRowInternalField)base.RowTreeField, this);
		}
		public new PivotDrillDownDataSource CreateDrillDownDataSource(int columnIndex, int rowIndex) {
			return Data.CreateDrillDownDataSourceWrapper(base.CreateDrillDownDataSource(columnIndex, rowIndex));
		}
		public new PivotDrillDownDataSource CreateDrillDownDataSource(int columnIndex, int rowIndex, int maxRowCount) {
			return Data.CreateDrillDownDataSourceWrapper(base.CreateDrillDownDataSource(columnIndex, rowIndex, maxRowCount));
		}
		[Obsolete("This method is now obsolete. Use the CreateQueryModeDrillDownDataSource method instead.")]
		public new PivotDrillDownDataSource CreateOLAPDrillDownDataSource(int columnIndex, int rowIndex, int maxRowCount, List<string> customColumns) {
			return CreateQueryModeDrillDownDataSource(columnIndex, rowIndex, maxRowCount, customColumns);
		}
		public new PivotDrillDownDataSource CreateQueryModeDrillDownDataSource(int columnIndex, int rowIndex, int maxRowCount, List<string> customColumns) {
			return Data.CreateDrillDownDataSourceWrapper(base.CreateQueryModeDrillDownDataSource(columnIndex, rowIndex, maxRowCount, customColumns));
		}
		protected override void Calculate() {
			base.Calculate();
			RaiseAreaItemsChanged();
		}
		protected virtual void OnFieldSizeChanged(object sender, FieldSizeChangedEventArgs e) {
			ClearSizeCaches();
		}
		protected virtual void RaiseAreaItemsChanged() {
			if(areaItemsChanged != null)
				areaItemsChanged.Raise(this, EventArgs.Empty);
		}
		public bool IsObjectCollapsed(PivotGridField field, int lastLevelIndex) {
			return IsObjectCollapsed(field.InternalField, lastLevelIndex);
		}
		public object GetFieldValue(PivotGridField field, int lastLevelIndex) {
			return GetFieldValue(field.InternalField, lastLevelIndex);
		}
		public FieldValueType GetFieldValueType(PivotGridField field, int lastLevelIndex) {
			return GetFieldValueType(field.InternalField, lastLevelIndex).ToFieldValueType();
		}
		public PivotOlapMember GetOlapMember(PivotGridField field, int lastLevelIndex) {
			return CreateOlapMember(GetOLAPMember(field.InternalField, lastLevelIndex));
		}
		public object GetColumnTotalValue(PivotGridCellItem item, bool returnGrandTotal) {
			if(returnGrandTotal && item.ColumnFieldValueItem.StartLevel == 0)
				return Data.GetCellValue(-1, item.RowFieldIndex, item.DataIndex);
			PivotFieldValueItem parent = GetParentItem(true, item.ColumnFieldValueItem);
			if(parent != null)
				return Data.GetCellValue(parent.VisibleIndex, item.RowFieldIndex, item.DataIndex);
			return null;
		}
		public object GetRowTotalValue(PivotGridCellItem item, bool returnGrandTotal) {
			if(returnGrandTotal && item.RowFieldValueItem.StartLevel == 0)
				return Data.GetCellValue(item.ColumnFieldIndex, -1, item.DataIndex);
			PivotFieldValueItem parent = GetParentItem(false, item.RowFieldValueItem);
			if(parent != null)
				return Data.GetCellValue(item.ColumnFieldIndex, parent.VisibleIndex, item.DataIndex);
			return null;
		}
		public bool IsFieldSortedBySummary(bool isColumn, PivotGridField field, PivotGridField dataField, int itemIndex) {
			PivotFieldItem dataFieldItem = dataField != null ? dataField.FieldItem : null;
			return base.IsFieldSortedBySummary(isColumn, field.FieldItem, dataFieldItem, itemIndex);
		}
		protected override void ChangeExpandedCore(PivotFieldValueItem item, CoreXtraPivotGrid.AsyncCompletedHandler asyncCompleted) {
			Data.ChangeExpandedAsync(item, false, asyncCompleted);
		}
		public override void Clear() {
			base.Clear();
			if(IsReadOnly)
				return;
			ClearSizeCaches();
		}
		void ClearSizeCaches() {
			cellSizeProvider.Clear();
		}
		internal System.Drawing.Size GetLastLevelItemSize(PivotFieldValueItem item) {
			return cellSizeProvider.GetLastLevelItemSize(item);
		}
		internal System.Drawing.Size GetLastLevelItemSize(bool isColumn, int level) {
			return cellSizeProvider.GetLastLevelItemSize(isColumn, level);
		}
		internal int GetItemWidth(int level, bool isColumn) {
			return cellSizeProvider.GetWidthDifference(isColumn, level, level + 1);
		}
		internal int GetItemHeight(int level, bool isColumn) {
			return cellSizeProvider.GetHeightDifference(isColumn, level, level + 1);
		}
		internal int GetHeightDifference(int start, int end, bool isColumn) {
			return cellSizeProvider.GetHeightDifference(isColumn, start, end);
		}
		internal int GetWidthDifference(int start, int end, bool isColumn) {
			return cellSizeProvider.GetWidthDifference(isColumn, start, end);
		}
		internal void SetItemSize(PivotFieldValueItem item, System.Drawing.Size size, ResizingFieldsCache resizingFieldsCache) {
			cellSizeProvider.SetItemSize(item, size, resizingFieldsCache);
		}
		internal void SetItemSize(PivotFieldValueItem item, System.Drawing.Size size) {
			cellSizeProvider.SetItemSize(item, size);
		}
	}
	public class PivotTreeRowInternalField : PivotGridInternalField {
		PivotVisualItems visualItems;
		public PivotTreeRowInternalField(PivotVisualItems visualItems) {
			this.visualItems = visualItems;
			Area = CoreXtraPivotGrid.PivotArea.RowArea;
		}
		protected PivotVisualItems VisualItems { get { return visualItems; } }
		protected override PivotGridData Data { get { return VisualItems != null ? VisualItems.Data : null; } }
		public override int AreaIndex {
			get { return PivotTreeRowFieldBase.GetAreaIndex(VisualItems, false); }
			set {  }
		}
		public override int Width {
			get { return PivotTreeRowFieldBase.GetWidth(VisualItems); }
			set { PivotTreeRowFieldBase.SetWidth(VisualItems, value); }
		}
	}
	public class PivotTreeRowField : PivotGridField {
		PivotVisualItems visualItems;
		public PivotTreeRowField(PivotTreeRowInternalField rowTreeFieldInternal, PivotVisualItems visualItems)
			: base(rowTreeFieldInternal, true) {
				if(Area != FieldArea.RowArea || Width != InternalField.Width)
					throw new ArgumentException("Field wasn't successfully synchronized");
				this.visualItems = visualItems;
				BestFitArea = FieldBestFitArea.FieldValue;				
		}
		protected internal override PivotGridWpfData Data {
			get { return visualItems != null ? visualItems.Data : base.Data; }
		}
		protected override void SyncFieldHeight(bool read, bool write) {
			base.SyncFieldHeight(read, write);
			if(PivotGrid == null)
				return;
			if(PivotGrid.RowTreeHeight == Convert.ToInt32(Height))
				return;
			PivotGrid.RowTreeHeight = Convert.ToInt32(Height);
		}
		protected override void SyncFieldWidth(bool read, bool write) {
			base.SyncFieldWidth(read, write);
			if(PivotGrid == null)
				return;
			if(PivotGrid.RowTreeWidth == Convert.ToInt32(Width))
				return;
			PivotGrid.RowTreeWidth = Convert.ToInt32(Width);
		}
	}
	public class CellSizeProvider : CellSizeProviderBase {
		readonly FieldValueSizeContainer columnCellSizes, rowCellSizes;
		protected FieldValueSizeContainer ColumnCellSizes { get { return columnCellSizes; } }
		protected FieldValueSizeContainer RowCellSizes { get { return rowCellSizes; } }
		public CellSizeProvider(PivotGridData data, PivotVisualItemsBase visualItems)
			: base(data, visualItems) {
			this.columnCellSizes = new FieldValueSizeContainer(DefaultCellSizeCountLimit, DefaultCellSizeLifeTime, MinClearCheckPeriod);
			this.rowCellSizes = new FieldValueSizeContainer(DefaultCellSizeCountLimit, DefaultCellSizeLifeTime, MinClearCheckPeriod);
		}
		public void SetItemSize(PivotFieldValueItem item, System.Drawing.Size size) {
			SetItemSize(item, size, null);
		}
		public void SetItemSize(PivotFieldValueItem item, Size size, ResizingFieldsCache fieldsCache) {
			if(size.Width < 0 || size.Height < 0)
				throw new ArgumentException("size");
			Size oldSize = GetLastLevelItemSize(item);
			FieldValueSizeContainer container = item.IsColumn ? ColumnCellSizes : RowCellSizes;
			object[] values = GetItemKey(item);
			container.Add(values, size);
			OnFieldSizeChanged(item.ResizingField, fieldsCache, oldSize.Width != size.Width, oldSize.Height != size.Height);
		}
		void OnFieldSizeChanged(PivotFieldItemBase fieldItem, ResizingFieldsCache fieldsCache, bool widthChanged, bool heightChanged) {
			if(fieldItem == null || (!widthChanged && !heightChanged))
				return;
			PivotGridFieldBase internalField = Data.GetField(fieldItem);
			if(fieldsCache == null)
				Data.OnFieldSizeChanged(internalField, widthChanged, heightChanged);
			else {
				fieldsCache.Add(internalField, widthChanged, heightChanged);
			}
		}
		protected bool TryGetCustomItemSize(PivotFieldValueItem item, out Size size) {
			if(item == null) {
				size = default(Size);
				return false;
			}
			FieldValueSizeContainer container = item.IsColumn ? ColumnCellSizes : RowCellSizes;
			if(container.Count == 0) {
				size = default(Size);
				return false;
			}
			object[] values = GetItemKey(item);
			return container.TryGetValue(values, out size);
		}
		protected override int GetColumnFieldHeight(int columnAreaLevel) {
			PivotFieldItem field = (PivotFieldItem)FieldItems.GetFieldItemByLevel(true, columnAreaLevel);
			if(field == null)
				if(columnAreaLevel == VisualItems.GetLevelCount(true) - 1)
					field = (PivotFieldItem)DataFieldItem;
				else
					return PivotGridField.DefaultHeight;
			return field.Height;
		}
		protected override int GetRowFieldHeight(PivotFieldValueItem item) {
			int height = PivotGridField.DefaultHeight;
			if(item != null) {
				System.Drawing.Size size = default(System.Drawing.Size);
				if(TryGetCustomItemSize(item, out size))
					return size.Height;
				PivotFieldItem field = (PivotFieldItem)item.ResizingField;
				if(field != null) {
					height = field.Height;
				}
				if(!item.IsColumn && (item.IsRowTree ? item.ValueType == PivotGridValueType.GrandTotal : item.IsTotal)) {
					PivotGridControl pivot = ((PivotGridWpfData)Data).PivotGrid;
					if(pivot != null)
						height = Convert.ToInt32(height * pivot.RowTotalsHeightFactor);
					else
						height = Convert.ToInt32(height * PivotGridControl.RowTotalsHeightFactorPropertyDefaultValue);
				}
			}
			return height;
		}
		protected override System.Drawing.Size GetLastLevelItemSizeCore(PivotFieldValueItem item) {
			if(item != null) {
				System.Drawing.Size res;
				if(TryGetCustomItemSize(item, out res))
					return res;
			}
			return base.GetLastLevelItemSizeCore(item);
		}
		protected override int GetColumnFieldWidth(PivotFieldValueItem item) {
			if(item != null) {
				System.Drawing.Size res;
				if(TryGetCustomItemSize(item, out res))
					return res.Width;
			}
			return base.GetColumnFieldWidth(item);
		}
	}
	public class FieldValueSizeContainer : LifeTimeListContainer<object[], SizeRecord> {
		public FieldValueSizeContainer(int countLimit, TimeSpan lifeTime, TimeSpan minClearCheckPeriod)
			: base(countLimit, lifeTime, minClearCheckPeriod) {
			List = new SortedList<object[], SizeRecord>(new ObjectArrayComparer());
		}
		public bool TryGetValue(object[] values, out System.Drawing.Size size) {
			SizeRecord record;
			if(List.TryGetValue(values, out record)) {
				size = record.Size;
				return true;
			}
			size = System.Drawing.Size.Empty;
			return false;
		}
		public void Add(object[] values, System.Drawing.Size size) {
			OnAdd();
			SizeRecord record = new SizeRecord(size, Now());
			if(List.ContainsKey(values))
				List[values] = record;
			else
				List.Add(values, record);
		}
		protected class ObjectArrayComparer : IComparer<object[]>, IEqualityComparer<object[]> {
			#region IComparer<object[]> Members
			public int Compare(object[] x, object[] y) {
				int res = Comparer<int>.Default.Compare(x.Length, y.Length);
				if(res != 0)
					return res;
				for(int i = 0; i < x.Length; i++) {
					int hash1 = GetHash(x[i]),
						hash2 = GetHash(y[i]);
					res = Comparer<int>.Default.Compare(hash1, hash2);
					if(res != 0)
						return res;
				}
				return 0;
			}
			public int GetHash(object obj) {
				return obj != null ? obj.GetHashCode() : 0;
			}
			#endregion
			#region IEqualityComparer<object[]> Members
			bool IEqualityComparer<object[]>.Equals(object[] x, object[] y) {
				return Compare(x, y) == 0;
			}
			int IEqualityComparer<object[]>.GetHashCode(object[] obj) {
				int code = 0;
				for(int i = 0; i < obj.Length; i++)
					code += GetHash(obj[i]) + i;
				return code;
			}
			#endregion
		}
	}
}
