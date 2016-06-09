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
using System.Drawing;
using System.Runtime.InteropServices;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.Export.Xl;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	#region TableApplyFlagsIndexAccessor
	public class TableApplyFlagsIndexAccessor : IIndexAccessor<Table, TableCellStyleApplyFlagsInfo, DocumentModelChangeActions> {
		#region IIndexAccessor<Table, TableCellStyleApplyFlagsInfo> Members
		public int GetIndex(Table owner) {
			return owner.ApplyFlagsIndex;
		}
		public int GetDeferredInfoIndex(Table owner) {
			return GetInfoIndex(owner, GetDeferredInfo(owner.BatchUpdateHelper));
		}
		public void SetIndex(Table owner, int value) {
			owner.AssignApplyFlagsIndex(value);
		}
		public int GetInfoIndex(Table owner, TableCellStyleApplyFlagsInfo value) {
			return value.PackedValues;
		}
		public TableCellStyleApplyFlagsInfo GetInfo(Table owner) {
			TableCellStyleApplyFlagsInfo info = new TableCellStyleApplyFlagsInfo();
			info.PackedValues = owner.ApplyFlagsIndex;
			return info;
		}
		public bool IsIndexValid(Table owner, int index) {
			return true;
		}
		public IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(Table owner) {
			return new TableApplyFlagsIndexChangeHistoryItem(owner);
		}
		public TableCellStyleApplyFlagsInfo GetDeferredInfo(MultiIndexBatchUpdateHelper helper) {
			return ((TableBatchUpdateHelper)helper).ApplyFlagsInfo;
		}
		public void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, TableCellStyleApplyFlagsInfo info) {
			((TableBatchUpdateHelper)helper).ApplyFlagsInfo = info.Clone();
		}
		public void InitializeDeferredInfo(Table owner) {
			this.SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(owner));
		}
		public void CopyDeferredInfo(Table owner, Table from) {
			SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(from));
		}
		public bool ApplyDeferredChanges(Table owner) {
			return owner.ReplaceInfo(this, GetDeferredInfo(owner.BatchUpdateHelper), owner.GetBatchUpdateChangeActions());
		}
		#endregion
	}
	#endregion
	public partial class Table : MultiIndexObject<Table, DocumentModelChangeActions> {
		#region Static Members
		readonly static TableInfoIndexAccessor tableInfoIndexAccessor = new TableInfoIndexAccessor();
		readonly static TableApplyFlagsIndexAccessor tableApplyFlagsIndexAccessor = new TableApplyFlagsIndexAccessor();
		readonly static IIndexAccessorBase<Table, DocumentModelChangeActions>[] indexAccessors = GetIndexAccessors();
		static IIndexAccessorBase<Table, DocumentModelChangeActions>[] GetIndexAccessors() {
			IIndexAccessorBase<Table, DocumentModelChangeActions>[] result = new IIndexAccessorBase<Table, DocumentModelChangeActions>[ElementFormatCount + 2];
			result[TableInfoAccessorIndex] = tableInfoIndexAccessor;
			for (int i = 0; i < DifferentialFormatElementCount; i++)
				result[1 + i] = new TableDifferentialFormatIndexAccessor(i);
			for (int i = 0; i < CellFormatElementCount; i++)
				result[1 + DifferentialFormatElementCount + i] = new TableCellFormatIndexAccessor(i);
			for (int i = 0; i < BorderFormatElementCount; i++)
				result[1 + DifferentialFormatElementCount + CellFormatElementCount + i] = new TableBorderFormatIndexAccessor(i);
			result[ElementFormatCount + 1] = tableApplyFlagsIndexAccessor;
			return result;
		}
		public static TableInfoIndexAccessor TableInfoIndexAccessor { get { return tableInfoIndexAccessor; } }
		public static TableApplyFlagsIndexAccessor TableApplyFlagsIndexAccessor { get { return tableApplyFlagsIndexAccessor; } }
		public static TableDifferentialFormatIndexAccessor GetDifferentialFormatIndexAccessor(int elementIndex) {
			int index = 1 + elementIndex;
			return (TableDifferentialFormatIndexAccessor)indexAccessors[index];
		}
		public static TableCellFormatIndexAccessor GetTableCellFormatIndexAccessor(int elementIndex) {
			int index = 1 + DifferentialFormatElementCount + elementIndex;
			return (TableCellFormatIndexAccessor)indexAccessors[index];
		}
		public static TableBorderFormatIndexAccessor GetTableBorderFormatIndexAccessor(int elementIndex) {
			int index = 1 + DifferentialFormatElementCount + CellFormatElementCount + elementIndex;
			return (TableBorderFormatIndexAccessor)indexAccessors[index];
		}
		#endregion
		internal const int DifferentialFormatElementCount = 3;
		internal const int CellFormatElementCount = 3;
		internal const int BorderFormatElementCount = 3;
		internal const int ElementFormatCount = 9;
		const int TableInfoAccessorIndex = 0;
		internal const int HeaderRowIndex = 0;
		internal const int TotalsRowIndex = 1;
		internal const int DataIndex = 2;
		readonly int[] differentialFormatIndexes;
		readonly int[] cellFormatIndexes;
		readonly int[] borderFormatIndexes;
		int tableInfoIndex;
		int applyFlagsIndex;
		internal int TableInfoIndex { get { return tableInfoIndex; } }
		internal int ApplyFlagsIndex { get { return applyFlagsIndex; } }
		internal int[] DifferentialFormatIndexes { get { return differentialFormatIndexes; } }
		internal int[] CellFormatIndexes { get { return cellFormatIndexes; } }
		internal int[] BorderFormatIndexes { get { return borderFormatIndexes; } }
		internal TableInfo TableInfo { get { return IsUpdateLocked ? BatchUpdateHelper.TableInfo : TableInfoCore; } }
		TableInfo TableInfoCore { get { return tableInfoIndexAccessor.GetInfo(this); } }
		protected internal TableCellStyleApplyFlagsInfo ApplyFlagsInfo { get { return IsUpdateLocked ? BatchUpdateHelper.ApplyFlagsInfo : ApplyFlagsInfoCore; } }
		TableCellStyleApplyFlagsInfo ApplyFlagsInfoCore { get { return tableApplyFlagsIndexAccessor.GetInfo(this); } }
		internal new TableBatchUpdateHelper BatchUpdateHelper { get { return (TableBatchUpdateHelper)base.BatchUpdateHelper; } }
		protected override IIndexAccessorBase<Table, DocumentModelChangeActions>[] IndexAccessors { get { return indexAccessors; } }
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045")]
		delegate DocumentModelChangeActions SetApplyFlagsInfoDelegate(ref TableCellStyleApplyFlagsInfo info, int elementIndex, bool newValue);
		void SetApplyFlagsInfo(int elementIndex, SetApplyFlagsInfoDelegate setter, bool newValue) {
			IIndexAccessor<Table, TableCellStyleApplyFlagsInfo, DocumentModelChangeActions> indexHolder = TableApplyFlagsIndexAccessor;
			TableCellStyleApplyFlagsInfo info = GetInfoForModification(indexHolder);
			DocumentModelChangeActions changeActions = setter(ref info, elementIndex, newValue);
			ReplaceInfoForFlags(indexHolder, info, changeActions);
		}
	}
	#region TableInfo
	public class TableInfo : ICloneable<TableInfo>, ISupportsCopyFrom<TableInfo>, ISupportsSizeOf {
		#region Static Members
		public static TableInfo CreateDefault() {
			TableInfo item = new TableInfo();
			item.StyleName = TableStyleName.DefaultStyleName.Name;
			item.TableType = TableType.Worksheet;
			item.HasHeadersRow = true;
			item.ShowRowStripes = true;
			item.ConnectionId = -1;
			return item;
		}
		#endregion
		#region Fields
		const uint MaskTableType = 0x00000003;		  
		const uint MaskPublished = 0x00000004;		  
		const uint MaskInsertRowProperty = 0x00000008;  
		const uint MaskInsertRowShift = 0x00000010;	 
		const uint MaskHasTotalsRow = 0x00000020;	   
		const uint MaskHasHeadersRow = 0x00000040;	  
		const uint MaskShowFirstColumn = 0x00000080;	
		const uint MaskShowLastColumn = 0x000000100;	
		const uint MaskShowRowStripes = 0x000000200;	
		const uint MaskShowColumnStripes = 0x000000400; 
		const uint MaskApplyTableStyle = 0x000000800;   
		uint packedValues;
		string comment;
		string styleName;
		int connectionId;
		#endregion
		#region Properties
		#region TableType
		public TableType TableType {
			get { return (TableType)(packedValues & MaskTableType); }
			set {
				packedValues &= ~MaskTableType;
				packedValues |= (uint)value & MaskTableType;
			}
		}
		#endregion
		public bool Published { get { return GetBooleanVal(MaskPublished); } set { SetBooleanVal(MaskPublished, value); } }
		public bool InsertRowProperty { get { return GetBooleanVal(MaskInsertRowProperty); } set { SetBooleanVal(MaskInsertRowProperty, value); } }
		public bool InsertRowShift { get { return GetBooleanVal(MaskInsertRowShift); } set { SetBooleanVal(MaskInsertRowShift, value); } }
		public bool HasTotalsRow { get { return GetBooleanVal(MaskHasTotalsRow); } set { SetBooleanVal(MaskHasTotalsRow, value); } }
		public bool HasHeadersRow { get { return GetBooleanVal(MaskHasHeadersRow); } set { SetBooleanVal(MaskHasHeadersRow, value); } }
		public bool ShowFirstColumn { get { return GetBooleanVal(MaskShowFirstColumn); } set { SetBooleanVal(MaskShowFirstColumn, value); } }
		public bool ShowLastColumn { get { return GetBooleanVal(MaskShowLastColumn); } set { SetBooleanVal(MaskShowLastColumn, value); } }
		public bool ShowRowStripes { get { return GetBooleanVal(MaskShowRowStripes); } set { SetBooleanVal(MaskShowRowStripes, value); } }
		public bool ShowColumnStripes { get { return GetBooleanVal(MaskShowColumnStripes); } set { SetBooleanVal(MaskShowColumnStripes, value); } }
		public bool ApplyTableStyle { get { return GetBooleanVal(MaskApplyTableStyle); } set { SetBooleanVal(MaskApplyTableStyle, value); } }
		public string Comment { get { return comment; } set { comment = value; } }
		public string StyleName { get { return styleName; } set { styleName = value; } }
		public int ConnectionId { get { return connectionId; } set { connectionId = value; } }
		#endregion
		#region GetBooleanVal/SetBooleanVal helpers
		void SetBooleanVal(uint mask, bool bitVal) {
			if (bitVal)
				packedValues |= mask;
			else
				packedValues &= ~mask;
		}
		bool GetBooleanVal(uint mask) {
			return (packedValues & mask) != 0;
		}
		#endregion
		#region ICloneable<TableInfo> Members
		public TableInfo Clone() {
			TableInfo clone = new TableInfo();
			clone.CopyFrom(this);
			return clone;
		}
		#endregion
		#region ISupportsCopyFrom<TableInfo> Members
		public void CopyFrom(TableInfo value) {
			this.packedValues = value.packedValues;
			this.comment = value.comment;
			this.styleName = value.styleName;
			this.connectionId = value.connectionId;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		public override bool Equals(object obj) {
			TableInfo info = obj as TableInfo;
			if (info == null)
				return false;
			return this.packedValues == info.packedValues && this.comment == info.comment &&
				this.styleName == info.styleName && this.connectionId == info.connectionId;
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32((int)packedValues, connectionId, comment.GetHashCode(), styleName.GetHashCode());
		}
	}
	#endregion
	#region TableInfoCache
	public class TableInfoCache : UniqueItemsCache<TableInfo> {
		internal const int DefaultItemIndex = 0;
		public TableInfoCache(IDocumentModelUnitConverter unitConverter)
			: base(unitConverter) {
		}
		protected override TableInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return TableInfo.CreateDefault();
		}
	}
	#endregion
	#region TableInfoIndexAccessor
	public class TableInfoIndexAccessor : IIndexAccessor<Table, TableInfo, DocumentModelChangeActions> {
		#region IIndexAccessor Members
		public int GetIndex(Table owner) {
			return owner.TableInfoIndex;
		}
		public int GetDeferredInfoIndex(Table owner) {
			return GetInfoIndex(owner, GetDeferredInfo(owner.BatchUpdateHelper));
		}
		public void SetIndex(Table owner, int value) {
			owner.AssignTableInfoIndex(value);
		}
		public int GetInfoIndex(Table owner, TableInfo value) {
			return GetInfoCache(owner).GetItemIndex(value);
		}
		public TableInfo GetInfo(Table owner) {
			return GetInfoCache(owner)[GetIndex(owner)];
		}
		public bool IsIndexValid(Table owner, int index) {
			return index < GetInfoCache(owner).Count;
		}
		UniqueItemsCache<TableInfo> GetInfoCache(Table owner) {
			return owner.DocumentModel.Cache.TableInfoCache;
		}
		public IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(Table owner) {
			return new TableInfoIndexChangeHistoryItem(owner);
		}
		public TableInfo GetDeferredInfo(MultiIndexBatchUpdateHelper helper) {
			return ((TableBatchUpdateHelper)helper).TableInfo;
		}
		public void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, TableInfo info) {
			((TableBatchUpdateHelper)helper).TableInfo = info.Clone();
		}
		public void InitializeDeferredInfo(Table owner) {
			this.SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(owner));
		}
		public void CopyDeferredInfo(Table owner, Table from) {
			SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(from));
		}
		public bool ApplyDeferredChanges(Table owner) {
			return owner.ReplaceInfo(this, GetDeferredInfo(owner.BatchUpdateHelper), owner.GetBatchUpdateChangeActions());
		}
		#endregion
	}
	#endregion
	#region TableFormatIndexAccessorBase (abstract class)
	public abstract class TableFormatIndexAccessorBase<TInfo> : IIndexAccessor<Table, TInfo, DocumentModelChangeActions>
		where TInfo : ICloneable<TInfo>, ISupportsSizeOf {
		readonly int elementIndex;
		protected TableFormatIndexAccessorBase(int elementIndex) {
			this.elementIndex = elementIndex;
		}
		protected int ElementIndex { get { return elementIndex; } }
		#region IIndexAccessor Members
		public int GetDeferredInfoIndex(Table owner) {
			return GetInfoIndex(owner, GetDeferredInfo(owner.BatchUpdateHelper));
		}
		public int GetInfoIndex(Table owner, TInfo value) {
			return GetInfoCache(owner).GetItemIndex(value);
		}
		public TInfo GetInfo(Table owner) {
			return GetInfoCache(owner)[GetIndex(owner)];
		}
		public bool IsIndexValid(Table owner, int index) {
			return index < GetInfoCache(owner).Count;
		}
		public void InitializeDeferredInfo(Table owner) {
			this.SetDeferredInfo(owner.BatchUpdateHelper, GetInfo(owner));
		}
		public void CopyDeferredInfo(Table owner, Table from) {
			TInfo sourceClonned = GetInfo(from);
			SetDeferredInfo(owner.BatchUpdateHelper, sourceClonned);
		}
		public bool ApplyDeferredChanges(Table owner) {
			return owner.ReplaceInfo(this, GetDeferredInfo(owner.BatchUpdateHelper), owner.GetBatchUpdateChangeActions());
		}
		public abstract int GetIndex(Table owner);
		public abstract void SetIndex(Table owner, int value);
		public abstract TInfo GetDeferredInfo(MultiIndexBatchUpdateHelper helper);
		public abstract void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, TInfo info);
		public abstract IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(Table owner);
		protected abstract UniqueItemsCache<TInfo> GetInfoCache(Table owner);
		#endregion
	}
	#endregion
	#region TableDifferentialFormatIndexAccessor
	public class TableDifferentialFormatIndexAccessor : TableFormatIndexAccessorBase<FormatBase> {
		public TableDifferentialFormatIndexAccessor(int elementIndex)
			: base(elementIndex) {
		}
		#region IIndexAccessor Members
		public override int GetIndex(Table owner) {
			return owner.DifferentialFormatIndexes[ElementIndex];
		}
		public override void SetIndex(Table owner, int value) {
			owner.AssignDifferentialFormatIndex(ElementIndex, value);
		}
		public override FormatBase GetDeferredInfo(MultiIndexBatchUpdateHelper helper) {
			return ((TableBatchUpdateHelper)helper).GetDifferentialFormatInfo(ElementIndex);
		}
		public override void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, FormatBase info) {
			DifferentialFormat sourceDiffFormat = info as DifferentialFormat;
			TableBatchUpdateHelper tableBatchUpdateHelper = helper as TableBatchUpdateHelper;
			System.Diagnostics.Debug.Assert(sourceDiffFormat != null);
			System.Diagnostics.Debug.Assert(tableBatchUpdateHelper != null);
			Table owner = helper.BatchUpdateHandler as Table;
			if (!Object.ReferenceEquals(sourceDiffFormat.DocumentModel, owner.DocumentModel)) {
				FormatBase targetDiffFormat = tableBatchUpdateHelper.GetDifferentialFormatInfo(ElementIndex);
				targetDiffFormat.CopyFrom(sourceDiffFormat);
			}
			else
				tableBatchUpdateHelper.SetDifferentialFormatInfo(ElementIndex, info.Clone());
		}
		public override IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(Table owner) {
			return new TableDifferentialFormatIndexChangeHistoryItem(owner, ElementIndex);
		}
		protected override UniqueItemsCache<FormatBase> GetInfoCache(Table owner) {
			return owner.DocumentModel.Cache.CellFormatCache;
		}
		#endregion
	}
	#endregion
	#region TableCellFormatIndexAccessor
	public class TableCellFormatIndexAccessor : TableFormatIndexAccessorBase<FormatBase> {
		public TableCellFormatIndexAccessor(int elementIndex)
			: base(elementIndex) {
		}
		#region IIndexAccessor Members
		public override int GetIndex(Table owner) {
			return owner.CellFormatIndexes[ElementIndex];
		}
		public override void SetIndex(Table owner, int value) {
			owner.AssignCellFormatIndex(ElementIndex, value);
		}
		public override FormatBase GetDeferredInfo(MultiIndexBatchUpdateHelper helper) {
			return ((TableBatchUpdateHelper)helper).GetCellFormatInfo(ElementIndex);
		}
		public override void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, FormatBase info) {
			CellFormat sourceCellFormat = info as CellFormat;
			TableBatchUpdateHelper tableBatchUpdateHelper = helper as TableBatchUpdateHelper;
			System.Diagnostics.Debug.Assert(sourceCellFormat != null);
			System.Diagnostics.Debug.Assert(tableBatchUpdateHelper != null);
			Table owner = helper.BatchUpdateHandler as Table;
			if (!Object.ReferenceEquals(sourceCellFormat.DocumentModel, owner.DocumentModel)) {
				FormatBase targetCellFormat = tableBatchUpdateHelper.GetCellFormatInfo(ElementIndex);
				targetCellFormat.CopyFrom(sourceCellFormat);
			}
			else
				tableBatchUpdateHelper.SetCellFormatInfo(ElementIndex, sourceCellFormat.Clone());
		}
		public override IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(Table owner) {
			return new TableCellFormatIndexChangeHistoryItem(owner, ElementIndex);
		}
		protected override UniqueItemsCache<FormatBase> GetInfoCache(Table owner) {
			return owner.DocumentModel.Cache.CellFormatCache;
		}
		#endregion
	}
	#endregion
	#region TableBorderFormatIndexAccessor
	public class TableBorderFormatIndexAccessor : TableFormatIndexAccessorBase<FormatBase> {
		public TableBorderFormatIndexAccessor(int elementIndex)
			: base(elementIndex) {
		}
		#region IIndexAccessor Members
		public override int GetIndex(Table owner) {
			return owner.BorderFormatIndexes[ElementIndex];
		}
		public override void SetIndex(Table owner, int value) {
			owner.AssignBorderFormatIndex(ElementIndex, value);
		}
		public override FormatBase GetDeferredInfo(MultiIndexBatchUpdateHelper helper) {
			return ((TableBatchUpdateHelper)helper).GetBorderFormatInfo(ElementIndex);
		}
		public override void SetDeferredInfo(MultiIndexBatchUpdateHelper helper, FormatBase info) {
			DifferentialFormat sourceBorderFormat = info as DifferentialFormat;
			TableBatchUpdateHelper tableBatchUpdateHelper = helper as TableBatchUpdateHelper;
			System.Diagnostics.Debug.Assert(sourceBorderFormat != null);
			System.Diagnostics.Debug.Assert(tableBatchUpdateHelper != null);
			Table owner = helper.BatchUpdateHandler as Table;
			if (!Object.ReferenceEquals(sourceBorderFormat.DocumentModel, owner.DocumentModel)) {
				FormatBase targetBorderFormat = tableBatchUpdateHelper.GetBorderFormatInfo(ElementIndex);
				targetBorderFormat.CopyFrom(sourceBorderFormat);
			}
			else
				tableBatchUpdateHelper.SetBorderFormatInfo(ElementIndex, info.Clone());
		}
		public override IndexChangedHistoryItemCore<DocumentModelChangeActions> CreateHistoryItem(Table owner) {
			return new TableBorderFormatIndexChangeHistoryItem(owner, ElementIndex);
		}
		protected override UniqueItemsCache<FormatBase> GetInfoCache(Table owner) {
			return owner.DocumentModel.Cache.CellFormatCache;
		}
		#endregion
	}
	#endregion
	#region TableBatchUpdateHelper
	public class TableBatchUpdateHelper : MultiIndexBatchUpdateHelper {
		TableInfo tableInfo;
		FormatBase[] differentialFormatInfoes;
		CellFormat[] cellFormatInfoes;
		FormatBase[] borderFormatInfoes;
		TableCellStyleApplyFlagsInfo applyFlagsInfo;
		int suppressDirectNotificationsCount;
		public TableBatchUpdateHelper(IBatchUpdateHandler handler)
			: base(handler) {
			this.differentialFormatInfoes = new FormatBase[Table.DifferentialFormatElementCount];
			this.cellFormatInfoes = new CellFormat[Table.CellFormatElementCount];
			this.borderFormatInfoes = new FormatBase[Table.BorderFormatElementCount];
		}
		public TableInfo TableInfo { get { return tableInfo; } set { tableInfo = value; } }
		public TableCellStyleApplyFlagsInfo ApplyFlagsInfo { get { return applyFlagsInfo; } set { applyFlagsInfo = value; } }
		public FormatBase GetDifferentialFormatInfo(int elementIndex) {
			return differentialFormatInfoes[elementIndex];
		}
		public void SetDifferentialFormatInfo(int elementIndex, FormatBase value) {
			differentialFormatInfoes[elementIndex] = value;
		}
		public FormatBase GetCellFormatInfo(int elementIndex) {
			return cellFormatInfoes[elementIndex];
		}
		public void SetCellFormatInfo(int elementIndex, CellFormat value) {
			cellFormatInfoes[elementIndex] = value; 
		}
		public FormatBase GetBorderFormatInfo(int elementIndex) {
			return borderFormatInfoes[elementIndex];
		}
		public void SetBorderFormatInfo(int elementIndex, FormatBase value) {
			borderFormatInfoes[elementIndex] = value;
		}
		public bool IsDirectNotificationsEnabled { get { return suppressDirectNotificationsCount == 0; } }
		public void SuppressDirectNotifications() {
			suppressDirectNotificationsCount++;
		}
		public void ResumeDirectNotifications() {
			suppressDirectNotificationsCount--;
		}
	}
	#endregion
	#region TableBatchInitHelper
	public class TableBatchInitHelper : FormatBaseBatchUpdateHelper {
		public TableBatchInitHelper(IBatchInitHandler handler)
			: base(new BatchInitAdapter(handler)) {
		}
		public IBatchInitHandler BatchInitHandler { get { return ((BatchInitAdapter)BatchUpdateHandler).BatchInitHandler; } }
	}
	#endregion
	#region ITableFormatInfo
	public interface ITableFormatInfo : IDifferentialFormatPropertyChanger, ITableBorderFormatPropertyChanger, ICellStylePropertyChanger {
		ITableElementFormatInfo HeaderRow { get; }
		ITableElementFormatInfo TotalsRow { get; }
		ITableElementFormatInfo Data { get; }
	}
	#endregion
	#region ITableElementFormatInfo
	public interface ITableElementFormatInfo {
		IDifferentialFormat DifferentialFormat { get; }
		IBorderInfo Border { get; }
		CellStyleBase CellStyle { get; set; }
		bool ApplyCellStyle { get; }
		bool ApplyDifferentialFormat { get; }
		bool ApplyBorderFormat { get; }
	}
	#endregion 
	#region TableElementFormatManager
	public class TableElementFormatManager : ITableElementFormatInfo {
		int elementIndex;
		readonly Table info;
		readonly DifferentialFormatPropertyChangeManager differentialFormatManager;
		readonly TableBorderPropertyChangeManager borderManager;
		public TableElementFormatManager(Table info) {
			this.info = info;
			differentialFormatManager = new DifferentialFormatPropertyChangeManager(info);
			borderManager = new TableBorderPropertyChangeManager(info);
		}
		#region ITableRowFormatInfo Members
		public IDifferentialFormat DifferentialFormat { get { return differentialFormatManager.GetFormatInfo(elementIndex); } }
		public IBorderInfo Border { get { return borderManager.GetFormatInfo(elementIndex); } }
		public CellStyleBase CellStyle { get { return info.GetCellStyle(elementIndex); } set { info.SetCellStyle(elementIndex, value); } }
		public bool ApplyCellStyle { get { return info.GetApplyCellStyle(elementIndex); } }
		public bool ApplyDifferentialFormat { get { return info.GetApplyDifferentialFormat(elementIndex); } }
		public bool ApplyBorderFormat { get { return info.GetBorderFormat(elementIndex).BorderOptionsIndex != BorderOptionsInfo.DefaultIndex; } }
		#endregion
		protected internal ITableElementFormatInfo GetFormatInfo(int elementIndex) {
			this.elementIndex = elementIndex;
			return this;
		}
	}
	#endregion
	#region ITableBorderFormatPropertyChanger
	public interface ITableBorderFormatPropertyChanger {
		XlBorderLineStyle GetTableBorderLeftLineStyle(int elementIndex);
		void SetTableBorderLeftLineStyle(int elementIndex, XlBorderLineStyle value);
		XlBorderLineStyle GetTableBorderRightLineStyle(int elementIndex);
		void SetTableBorderRightLineStyle(int elementIndex, XlBorderLineStyle value);
		XlBorderLineStyle GetTableBorderTopLineStyle(int elementIndex);
		void SetTableBorderTopLineStyle(int elementIndex, XlBorderLineStyle value);
		XlBorderLineStyle GetTableBorderBottomLineStyle(int elementIndex);
		void SetTableBorderBottomLineStyle(int elementIndex, XlBorderLineStyle value);
		XlBorderLineStyle GetTableBorderDiagonalUpLineStyle(int elementIndex);
		void SetTableBorderDiagonalUpLineStyle(int elementIndex, XlBorderLineStyle value);
		XlBorderLineStyle GetTableBorderDiagonalDownLineStyle(int elementIndex);
		void SetTableBorderDiagonalDownLineStyle(int elementIndex, XlBorderLineStyle value);
		XlBorderLineStyle GetTableBorderHorizontalLineStyle(int elementIndex);
		void SetTableBorderHorizontalLineStyle(int elementIndex, XlBorderLineStyle value);
		XlBorderLineStyle GetTableBorderVerticalLineStyle(int elementIndex);
		void SetTableBorderVerticalLineStyle(int elementIndex, XlBorderLineStyle value);
		bool GetTableBorderOutline(int elementIndex);
		void SetTableBorderOutline(int elementIndex, bool value);
		Color GetTableBorderLeftColor(int elementIndex);
		void SetTableBorderLeftColor(int elementIndex, Color value);
		int GetTableBorderLeftColorIndex(int elementIndex);
		void SetTableBorderLeftColorIndex(int elementIndex, int value);
		Color GetTableBorderRightColor(int elementIndex);
		void SetTableBorderRightColor(int elementIndex, Color value);
		int GetTableBorderRightColorIndex(int elementIndex);
		void SetTableBorderRightColorIndex(int elementIndex, int value);
		Color GetTableBorderTopColor(int elementIndex);
		void SetTableBorderTopColor(int elementIndex, Color value);
		int GetTableBorderTopColorIndex(int elementIndex);
		void SetTableBorderTopColorIndex(int elementIndex, int value);
		Color GetTableBorderBottomColor(int elementIndex);
		void SetTableBorderBottomColor(int elementIndex, Color value);
		int GetTableBorderBottomColorIndex(int elementIndex);
		void SetTableBorderBottomColorIndex(int elementIndex, int value);
		Color GetTableBorderDiagonalColor(int elementIndex);
		void SetTableBorderDiagonalColor(int elementIndex, Color value);
		int GetTableBorderDiagonalColorIndex(int elementIndex);
		void SetTableBorderDiagonalColorIndex(int elementIndex, int value);
		Color GetTableBorderHorizontalColor(int elementIndex);
		void SetTableBorderHorizontalColor(int elementIndex, Color value);
		int GetTableBorderHorizontalColorIndex(int elementIndex);
		void SetTableBorderHorizontalColorIndex(int elementIndex, int value);
		Color GetTableBorderVerticalColor(int elementIndex);
		void SetTableBorderVerticalColor(int elementIndex, Color value);
		int GetTableBorderVerticalColorIndex(int elementIndex);
		void SetTableBorderVerticalColorIndex(int elementIndex, int value);
	}
	#endregion
	#region ICellStylePropertyChanger
	public interface ICellStylePropertyChanger {
		CellStyleBase GetCellStyle(int elementIndex);
		void SetCellStyle(int elementIndex, CellStyleBase value);
		bool GetApplyCellStyle(int elementIndex);
	}
	#endregion
	#region TableBorderPropertyChangeManager
	public class TableBorderPropertyChangeManager : IBorderInfo {
		int elementIndex;
		readonly ITableBorderFormatPropertyChanger info;
		public TableBorderPropertyChangeManager(ITableBorderFormatPropertyChanger info) {
			this.info = info;
		}
		#region IBorderInfo Members
		public XlBorderLineStyle LeftLineStyle {
			get { return info.GetTableBorderLeftLineStyle(elementIndex); }
			set { info.SetTableBorderLeftLineStyle(elementIndex, value); }
		}
		public XlBorderLineStyle RightLineStyle {
			get { return info.GetTableBorderRightLineStyle(elementIndex); }
			set { info.SetTableBorderRightLineStyle(elementIndex, value); }
		}
		public XlBorderLineStyle TopLineStyle {
			get { return info.GetTableBorderTopLineStyle(elementIndex); }
			set { info.SetTableBorderTopLineStyle(elementIndex, value); }
		}
		public XlBorderLineStyle BottomLineStyle {
			get { return info.GetTableBorderBottomLineStyle(elementIndex); }
			set { info.SetTableBorderBottomLineStyle(elementIndex, value); }
		}
		public XlBorderLineStyle DiagonalUpLineStyle {
			get { return info.GetTableBorderDiagonalUpLineStyle(elementIndex); }
			set { info.SetTableBorderDiagonalUpLineStyle(elementIndex, value); }
		}
		public XlBorderLineStyle DiagonalDownLineStyle {
			get { return info.GetTableBorderDiagonalDownLineStyle(elementIndex); }
			set { info.SetTableBorderDiagonalDownLineStyle(elementIndex, value); }
		}
		public XlBorderLineStyle HorizontalLineStyle {
			get { return info.GetTableBorderHorizontalLineStyle(elementIndex); }
			set { info.SetTableBorderHorizontalLineStyle(elementIndex, value); }
		}
		public XlBorderLineStyle VerticalLineStyle {
			get { return info.GetTableBorderVerticalLineStyle(elementIndex); }
			set { info.SetTableBorderVerticalLineStyle(elementIndex, value); }
		}
		public bool Outline {
			get { return info.GetTableBorderOutline(elementIndex); }
			set { info.SetTableBorderOutline(elementIndex, value); }
		}
		public Color LeftColor {
			get { return info.GetTableBorderLeftColor(elementIndex); }
			set { info.SetTableBorderLeftColor(elementIndex, value); }
		}
		public Color RightColor {
			get { return info.GetTableBorderRightColor(elementIndex); }
			set { info.SetTableBorderRightColor(elementIndex, value); }
		}
		public Color TopColor {
			get { return info.GetTableBorderTopColor(elementIndex); }
			set { info.SetTableBorderTopColor(elementIndex, value); }
		}
		public Color BottomColor {
			get { return info.GetTableBorderBottomColor(elementIndex); }
			set { info.SetTableBorderBottomColor(elementIndex, value); }
		}
		public Color DiagonalColor {
			get { return info.GetTableBorderDiagonalColor(elementIndex); }
			set { info.SetTableBorderDiagonalColor(elementIndex, value); }
		}
		public Color HorizontalColor {
			get { return info.GetTableBorderHorizontalColor(elementIndex); }
			set { info.SetTableBorderHorizontalColor(elementIndex, value); }
		}
		public Color VerticalColor {
			get { return info.GetTableBorderVerticalColor(elementIndex); }
			set { info.SetTableBorderVerticalColor(elementIndex, value); }
		}
		public int LeftColorIndex {
			get { return info.GetTableBorderLeftColorIndex(elementIndex); }
			set { info.SetTableBorderLeftColorIndex(elementIndex, value); }
		}
		public int RightColorIndex {
			get { return info.GetTableBorderRightColorIndex(elementIndex); }
			set { info.SetTableBorderRightColorIndex(elementIndex, value); }
		}
		public int TopColorIndex {
			get { return info.GetTableBorderTopColorIndex(elementIndex); }
			set { info.SetTableBorderTopColorIndex(elementIndex, value); }
		}
		public int BottomColorIndex {
			get { return info.GetTableBorderBottomColorIndex(elementIndex); }
			set { info.SetTableBorderBottomColorIndex(elementIndex, value); }
		}
		public int DiagonalColorIndex {
			get { return info.GetTableBorderDiagonalColorIndex(elementIndex); }
			set { info.SetTableBorderDiagonalColorIndex(elementIndex, value); }
		}
		public int HorizontalColorIndex {
			get { return info.GetTableBorderHorizontalColorIndex(elementIndex); }
			set { info.SetTableBorderHorizontalColorIndex(elementIndex, value); }
		}
		public int VerticalColorIndex {
			get { return info.GetTableBorderVerticalColorIndex(elementIndex); }
			set { info.SetTableBorderVerticalColorIndex(elementIndex, value); }
		}
		#endregion
		protected internal IBorderInfo GetFormatInfo(int elementIndex) {
			this.elementIndex = elementIndex;
			return this;
		}
	}
	#endregion
}
