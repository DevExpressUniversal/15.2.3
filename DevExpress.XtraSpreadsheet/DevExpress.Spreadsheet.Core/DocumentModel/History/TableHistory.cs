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
using DevExpress.Office.History;
using DevExpress.Office;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model.History {
	#region TableInsertColumnHistoryItem
	public class TableInsertColumnHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		readonly TableColumn column;
		readonly int index;
		readonly Table table;
		#endregion
		public TableInsertColumnHistoryItem(Table table, TableColumn column, int index)
			: base(table.Worksheet) {
			this.index = index;
			this.table = table;
			this.column = column;
		}
		protected override void UndoCore() {
			table.Columns.RemoveAt(index);
		}
		protected override void RedoCore() {
			table.Columns.Insert(index, column);
		}
	}
	#endregion
	#region TableColumnRemoveHistoryItem
	public class TableColumnRemoveAtHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		readonly int index;
		readonly Table table;
		TableColumn column;
		#endregion
		public TableColumnRemoveAtHistoryItem(Table table, int index)
			: base(table.Worksheet) {
			this.index = index;
			this.table = table;
		}
		protected override void UndoCore() {
			if (column != null)
				table.Columns.Insert(index, column);
		}
		protected override void RedoCore() {
			if (index >= 0 && index < table.Columns.Count) {
				column = table.Columns[index];
				table.Columns.RemoveAt(index);
			}
			else
				column = null;
		}
	}
	#endregion
	#region ChangeTableRangeHistoryItem
	public class ChangeTableRangeHistoryItem : SpreadsheetCellRangeHistoryItem {
		#region Fields
		readonly Table table;
		#endregion
		public ChangeTableRangeHistoryItem(Table table, CellRange oldRange, CellRange newRange)
			: base(table.Worksheet, oldRange, newRange) {
			this.table = table;
		}
		protected override void UndoCore() {
			table.SetRangeCore(OldRange);
		}
		protected override void RedoCore() {
			table.SetRangeCore(NewRange);
		}
	}
	#endregion
	#region ChangeTableDisplayNameHistoryItem
	public class ChangeTableDisplayNameHistoryItem : SpreadsheetStringHistoryItem {
		#region Fields
		readonly Table table;
		#endregion
		public ChangeTableDisplayNameHistoryItem(Table table, string oldName, string newName)
			: base(table.Worksheet, oldName, newName) {
			this.table = table;
		}
		protected override void UndoCore() {
			table.SetNameCore(OldValue);
		}
		protected override void RedoCore() {
			table.SetNameCore(NewValue);
		}
	}
	#endregion
	#region ChangeTableColumnNameHistoryItem
	public class ChangeTableColumnNameHistoryItem : SpreadsheetStringHistoryItem {
		#region Fields
		readonly TableColumn column;
		#endregion
		public ChangeTableColumnNameHistoryItem(TableColumn column, string oldName, string newName)
			: base(column.Worksheet, oldName, newName) {
			this.column = column;
		}
		protected override void UndoCore() {
			column.SetNameCore(OldValue);
		}
		protected override void RedoCore() {
			column.SetNameCore(NewValue);
		}
	}
	#endregion
	#region ChangeTableColumnNamesHistoryItem
	public class ChangeTableColumnNamesHistoryItem : SpreadsheetHistoryItem {
		readonly TableColumnNamesCorrectedOperationBase operation;
		public ChangeTableColumnNamesHistoryItem(TableColumnNamesCorrectedOperationBase operation)
			: base(operation.Table.Worksheet) {
			this.operation = operation;
		}
		protected override void UndoCore() {
			operation.SetOldNames();
		}
		protected override void RedoCore() {
			operation.SetNewNames();
		}
	}
	#endregion
	#region ChangeTableHasHeadersPropertyHistoryItem
	public class ChangeTableHasHeadersPropertyHistoryItem : SpreadsheetBooleanHistoryItem {
		#region Fields
		readonly Table table;
		#endregion
		public ChangeTableHasHeadersPropertyHistoryItem(Table table, bool oldValue, bool newValue)
			: base(table.Worksheet, oldValue, newValue) {
			this.table = table;
		}
		protected override void UndoCore() {
			table.SetHasHeadersRowCore(OldValue);
		}
		protected override void RedoCore() {
			table.SetHasHeadersRowCore(NewValue);
		}
	}
	#endregion
	#region ChangeTableHasTotalsPropertyHistoryItem
	public class ChangeTableHasTotalsPropertyHistoryItem : SpreadsheetBooleanHistoryItem {
		#region Fields
		readonly Table table;
		#endregion
		public ChangeTableHasTotalsPropertyHistoryItem(Table table, bool oldValue, bool newValue)
			: base(table.Worksheet, oldValue, newValue) {
			this.table = table;
		}
		protected override void UndoCore() {
			table.SetHasTotalsRowCore(OldValue);
		}
		protected override void RedoCore() {
			table.SetHasTotalsRowCore(NewValue);
		}
	}
	#endregion
	#region TableColumnTotalsRowLabelHistoryItem
	public class TableColumnTotalsRowLabelHistoryItem : SpreadsheetStringHistoryItem {
		#region Fields
		readonly TableColumn column;
		#endregion
		public TableColumnTotalsRowLabelHistoryItem(TableColumn column, string oldValue, string newValue)
			: base(column.Worksheet, oldValue, newValue) {
			this.column = column;
		}
		protected override void UndoCore() {
			column.SetTotalsRowLabelCore(OldValue);
		}
		protected override void RedoCore() {
			column.SetTotalsRowLabelCore(NewValue);
		}
	}
	#endregion
	#region TableColumnTotalsRowFunctionHistoryItem
	public class TableColumnTotalsRowFunctionHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		readonly TableColumn column;
		readonly TotalsRowFunctionType oldValue;
		readonly TotalsRowFunctionType newValue;
		#endregion
		public TableColumnTotalsRowFunctionHistoryItem(TableColumn column, TotalsRowFunctionType oldValue, TotalsRowFunctionType newValue)
			: base(column.Worksheet) {
			this.column = column;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		protected override void UndoCore() {
			column.SetTotalsRowFunctionCore(oldValue);
		}
		protected override void RedoCore() {
			column.SetTotalsRowFunctionCore(newValue);
		}
	}
	#endregion
	#region TableColumnFormulaHistoryItem
	public class TableColumnFormulaHistoryItem : SpreadsheetHistoryItem {
		#region Fields
		readonly TableFormulaProvider provider;
		readonly byte[] oldValue;
		readonly byte[] newValue;
		#endregion
		public TableColumnFormulaHistoryItem(TableFormulaProvider provider, byte[] oldValue, byte[] newValue)
			: base(provider.Worksheet) {
				this.provider = provider;
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		protected override void UndoCore() {
			provider.SetFormulaCore(oldValue);
		}
		protected override void RedoCore() {
			provider.SetFormulaCore(newValue);
		}
	}
	#endregion
	#region TableStyleElementFormatHistoryItem
	public abstract class TableStyleElementFormatHistoryItem : IndexChangedHistoryItemCore<DocumentModelChangeActions> {
		readonly TableStyleElementFormat obj;
		static IDocumentModelPart GetModelPart(TableStyleElementFormat obj) {
			Guard.ArgumentNotNull(obj, "obj");
			return obj.DocumentModel;
		}
		protected TableStyleElementFormatHistoryItem(TableStyleElementFormat obj)
			: base(GetModelPart(obj)) {
			this.obj = obj;
		}
		protected TableStyleElementFormat Object { get { return obj; } }
		public override IIndexBasedObject<DocumentModelChangeActions> GetObject() {
			return null;
		}
	}
	#endregion
	#region TableStyleElementFormatStripeSizeInfoIndexChangeHistoryItem
	public class TableStyleElementFormatStripeSizeInfoIndexChangeHistoryItem : TableStyleElementFormatHistoryItem {
		public TableStyleElementFormatStripeSizeInfoIndexChangeHistoryItem(TableStyleElementFormat obj)
			: base(obj) {
		}
		protected override void UndoCore() {
			Object.SetIndexCore(TableStyleElementFormat.StripeSizeInfoIndexAccessor, OldIndex, ChangeActions);
		}
		protected override void RedoCore() {
			Object.SetIndexCore(TableStyleElementFormat.StripeSizeInfoIndexAccessor, NewIndex, ChangeActions);
		}
	}
	#endregion
	#region TableStyleElementFormatDifferentialFormatIndexChangeHistoryItem
	public class TableStyleElementFormatDifferentialFormatIndexChangeHistoryItem : TableStyleElementFormatHistoryItem {
		public TableStyleElementFormatDifferentialFormatIndexChangeHistoryItem(TableStyleElementFormat obj)
			: base(obj) {
		}
		protected override void UndoCore() {
			Object.SetIndexCore(TableStyleElementFormat.DifferentialFormatIndexAccessor, OldIndex, ChangeActions);
		}
		protected override void RedoCore() {
			Object.SetIndexCore(TableStyleElementFormat.DifferentialFormatIndexAccessor, NewIndex, ChangeActions);
		}
	}
	#endregion
	#region TableColumnInfoHistoryItem
	public abstract class TableColumnInfoHistoryItem : IndexChangedHistoryItemCore<DocumentModelChangeActions> {
		readonly TableColumn obj;
		static IDocumentModelPart GetModelPart(TableColumn obj) {
			Guard.ArgumentNotNull(obj, "obj");
			return obj.DocumentModel;
		}
		protected TableColumnInfoHistoryItem(TableColumn obj)
			: base(GetModelPart(obj)) {
			this.obj = obj;
		}
		protected TableColumn Object { get { return obj; } }
		public override IIndexBasedObject<DocumentModelChangeActions> GetObject() {
			return null;
		}
	}
	#endregion
	#region TableColumnCellFormatIndexChangeHistoryItem
	public class TableColumnCellFormatIndexChangeHistoryItem : TableColumnInfoHistoryItem {
		readonly int elementIndex;
		public TableColumnCellFormatIndexChangeHistoryItem(TableColumn obj, int elementIndex)
			: base(obj) {
			this.elementIndex = elementIndex;
		}
		protected override void UndoCore() {
			Object.SetIndexCore(TableColumn.GetCellFormatIndexAccessor(elementIndex), OldIndex, ChangeActions);
		}
		protected override void RedoCore() {
			Object.SetIndexCore(TableColumn.GetCellFormatIndexAccessor(elementIndex), NewIndex, ChangeActions);
		}
	}
	#endregion
	#region TableColumnDifferentialFormatIndexChangeHistoryItem
	public class TableColumnDifferentialFormatIndexChangeHistoryItem : TableColumnInfoHistoryItem {
		readonly int elementIndex;
		public TableColumnDifferentialFormatIndexChangeHistoryItem(TableColumn obj, int elementIndex)
			: base(obj) {
			this.elementIndex = elementIndex;
		}
		protected override void UndoCore() {
			Object.SetIndexCore(TableColumn.GetDifferentialFormatIndexAccessor(elementIndex), OldIndex, ChangeActions);
		}
		protected override void RedoCore() {
			Object.SetIndexCore(TableColumn.GetDifferentialFormatIndexAccessor(elementIndex), NewIndex, ChangeActions);
		}
	}
	#endregion
	#region TableColumnInfoApplyFlagsIndexChangeHistoryItem
	public class TableColumnInfoApplyFlagsIndexChangeHistoryItem : TableColumnInfoHistoryItem {
		public TableColumnInfoApplyFlagsIndexChangeHistoryItem(TableColumn obj)
			: base(obj) {
		}
		protected override void UndoCore() {
			Object.SetIndexCore(TableColumn.TableColumnApplyFlagsIndexAccessor, OldIndex, ChangeActions);
		}
		protected override void RedoCore() {
			Object.SetIndexCore(TableColumn.TableColumnApplyFlagsIndexAccessor, NewIndex, ChangeActions);
		}
	}
	#endregion
	#region TableHistoryItem
	public abstract class TableHistoryItem : IndexChangedHistoryItemCore<DocumentModelChangeActions> {
		readonly Table obj;
		static IDocumentModelPart GetModelPart(Table obj) {
			Guard.ArgumentNotNull(obj, "obj");
			return obj.DocumentModel;
		}
		protected TableHistoryItem(Table obj)
			: base(GetModelPart(obj)) {
			this.obj = obj;
		}
		protected Table Object { get { return obj; } }
		public override IIndexBasedObject<DocumentModelChangeActions> GetObject() {
			return null;
		}
	}
	#endregion
	#region TableInfoIndexChangeHistoryItem
	public class TableInfoIndexChangeHistoryItem : TableHistoryItem {
		public TableInfoIndexChangeHistoryItem(Table obj)
			: base(obj) {
		}
		protected override void UndoCore() {
			Object.ChangeSubscribeStyleCache(NewIndex, OldIndex);
			Object.SetIndexCore(Table.TableInfoIndexAccessor, OldIndex, ChangeActions);
		}
		protected override void RedoCore() {
			Object.ChangeSubscribeStyleCache(OldIndex, NewIndex);
			Object.SetIndexCore(Table.TableInfoIndexAccessor, NewIndex, ChangeActions);
		}
	}
	#endregion
	#region TableFormatIndexChangeHistoryItemBase (absract class)
	public abstract class TableFormatIndexChangeHistoryItemBase : TableHistoryItem {
		readonly int elementIndex;
		protected TableFormatIndexChangeHistoryItemBase(Table obj, int elementIndex)
			: base(obj) {
			this.elementIndex = elementIndex;
		}
		protected int ElementIndex { get { return elementIndex; } }
	}
	#endregion
	#region TableDifferentialFormatIndexChangeHistoryItem
	public class TableDifferentialFormatIndexChangeHistoryItem : TableFormatIndexChangeHistoryItemBase {
		public TableDifferentialFormatIndexChangeHistoryItem(Table obj, int elementIndex)
			: base(obj, elementIndex) {
		}
		protected override void UndoCore() {
			Object.SetIndexCore(Table.GetDifferentialFormatIndexAccessor(ElementIndex), OldIndex, ChangeActions);
		}
		protected override void RedoCore() {
			Object.SetIndexCore(Table.GetDifferentialFormatIndexAccessor(ElementIndex), NewIndex, ChangeActions);
		}
	}
	#endregion
	#region TableCellFormatIndexChangeHistoryItem
	public class TableCellFormatIndexChangeHistoryItem : TableFormatIndexChangeHistoryItemBase {
		public TableCellFormatIndexChangeHistoryItem(Table obj, int elementIndex)
			: base(obj, elementIndex) {
		}
		protected override void UndoCore() {
			Object.SetIndexCore(Table.GetTableCellFormatIndexAccessor(ElementIndex), OldIndex, ChangeActions);
		}
		protected override void RedoCore() {
			Object.SetIndexCore(Table.GetTableCellFormatIndexAccessor(ElementIndex), NewIndex, ChangeActions);
		}
	}
	#endregion
	#region TableBorderFormatIndexChangeHistoryItem
	public class TableBorderFormatIndexChangeHistoryItem : TableFormatIndexChangeHistoryItemBase {
		public TableBorderFormatIndexChangeHistoryItem(Table obj, int elementIndex)
			: base(obj, elementIndex) {
		}
		protected override void UndoCore() {
			Object.SetIndexCore(Table.GetTableBorderFormatIndexAccessor(ElementIndex), OldIndex, ChangeActions);
		}
		protected override void RedoCore() {
			Object.SetIndexCore(Table.GetTableBorderFormatIndexAccessor(ElementIndex), NewIndex, ChangeActions);
		}
	}
	#endregion
	#region TableApplyFlagsIndexChangeHistoryItem
	public class TableApplyFlagsIndexChangeHistoryItem : TableHistoryItem {
		public TableApplyFlagsIndexChangeHistoryItem(Table obj)
			: base(obj) {
		}
		protected override void UndoCore() {
			Object.SetIndexCore(Table.TableApplyFlagsIndexAccessor, OldIndex, ChangeActions);
		}
		protected override void RedoCore() {
			Object.SetIndexCore(Table.TableApplyFlagsIndexAccessor, NewIndex, ChangeActions);
		}
	}
	#endregion
	#region TableStyleHistoryItem
	public class TableStyleHistoryItem : IndexChangedHistoryItemCore<DocumentModelChangeActions> {
		readonly TableStyle style;
		readonly int elementIndex;
		static IDocumentModelPart GetModelElement(TableStyle style) {
			Guard.ArgumentNotNull(style, "style");
			return style.DocumentModel;
		}
		public TableStyleHistoryItem(TableStyle style, int elementIndex)
			: base(GetModelElement(style)) {
			this.style = style;
			this.elementIndex = elementIndex;
		}
		public override IIndexBasedObject<DocumentModelChangeActions> GetObject() {
			return null;
		}
		protected override void UndoCore() {
			style.SetIndexCore(style.GetIndexAccessor(elementIndex), OldIndex, ChangeActions);
		}
		protected override void RedoCore() {
			style.SetIndexCore(style.GetIndexAccessor(elementIndex), NewIndex, ChangeActions);
		}
	}
	#endregion
	#region TableStyleChangeNameHistoryItem
	public class TableStyleChangeNameHistoryItem : HistoryItem {
		readonly TableStyle style;
		readonly TableStyleName oldName;
		readonly TableStyleName newName;
		public TableStyleChangeNameHistoryItem(TableStyle style, TableStyleName newName)
			: base(style.DocumentModel.MainPart) {
			this.style = style;
			this.oldName = style.Name;
			this.newName = newName;
		}
		protected override void UndoCore() {
			style.SetNameCore(oldName);
		}
		protected override void RedoCore() {
			style.SetNameCore(newName);
		}
	}
	#endregion
	#region TableStyleChangeTableTypeHistoryItem
	public class TableStyleChangeTableTypeHistoryItem : HistoryItem {
		readonly TableStyle style;
		readonly TableStyleElementIndexTableType oldType;
		readonly TableStyleElementIndexTableType newType;
		public TableStyleChangeTableTypeHistoryItem(TableStyle style, TableStyleElementIndexTableType newType)
			: base(style.DocumentModel.MainPart) {
			this.style = style;
			this.oldType = style.TableType;
			this.newType = newType;
		}
		protected override void UndoCore() {
			style.SetTableTypeCore(oldType);
		}
		protected override void RedoCore() {
			style.SetTableTypeCore(newType);
		}
	}
	#endregion
	#region TableStyleCollectionHistoryItems
	#region TableStyleCollectionHistoryItemBase (abstract class)
	public abstract class TableStyleCollectionHistoryItemBase : HistoryItem {
		readonly TableStyleCollection styles;
		protected TableStyleCollectionHistoryItemBase(TableStyleCollection styles)
			: base(styles.DocumentModel.MainPart) {
			this.styles = styles;
		}
		protected TableStyleCollection Styles { get { return styles; } }
	}
	#endregion
	#region TableStyleCollectionAddHistoryItem
	public class TableStyleCollectionAddHistoryItem : TableStyleCollectionHistoryItemBase {
		TableStyle style;
		public TableStyleCollectionAddHistoryItem(TableStyleCollection styles, TableStyle style)
			: base(styles) {
			this.style = style;
		}
		protected override void UndoCore() {
			Styles.RemoveCore(style);
		}
		protected override void RedoCore() {
			Styles.AddCore(style);
		}
	}
	#endregion
	#region TableStyleCollectionRemoveHistoryItem
	public class TableStyleCollectionRemoveHistoryItem : TableStyleCollectionHistoryItemBase {
		TableStyle style;
		public TableStyleCollectionRemoveHistoryItem(TableStyleCollection styles, TableStyle style)
			: base(styles) {
			this.style = style;
		}
		protected override void UndoCore() {
			Styles.AddCore(style);
		}
		protected override void RedoCore() {
			Styles.RemoveCore(style);
		}
	}
	#endregion
	#region TableStyleCollectionClearHistoryItem
	public abstract class TableStyleCollectionClearHistoryItemBase : TableStyleCollectionHistoryItemBase {
		readonly TableStyleCollection copyStyles;
		protected TableStyleCollectionClearHistoryItemBase(TableStyleCollection styles)
			: base(styles) {
			copyStyles = styles.Clone();
		}
		protected override void UndoCore() {
			Styles.CopyFrom(copyStyles);
		}
	}
	public class TableStyleCollectionClearAllHistoryItem : TableStyleCollectionClearHistoryItemBase {
		public TableStyleCollectionClearAllHistoryItem(TableStyleCollection styles)
			: base(styles) {
		}
		protected override void RedoCore() {
			Styles.ClearAllCore();
		}
	}
	public class TableStyleCollectionClearPartHistoryItem : TableStyleCollectionClearHistoryItemBase {
		readonly TableStyleElementIndexTableType tableType;
		public TableStyleCollectionClearPartHistoryItem(TableStyleCollection styles, TableStyleElementIndexTableType tableType)
			: base(styles) {
			this.tableType = tableType; 
		}
		protected override void RedoCore() {
			Styles.ClearPartStylesCore(tableType);
		}
	}
	#endregion
	#region TableStyleCollectionSetDefaultTableStyleHistoryItem
	public class TableStyleCollectionSetDefaultTableStyleHistoryItem : TableStyleCollectionHistoryItemBase {
		#region Fields
		readonly string oldName;
		readonly string newName;
		#endregion
		public TableStyleCollectionSetDefaultTableStyleHistoryItem(TableStyleCollection styles, string oldName, string newName)
			: base(styles) {
			this.oldName = oldName;
			this.newName = newName;
		}
		protected override void UndoCore() {
			Styles.SetDefaultTableStyleNameCore(oldName);
		}
		protected override void RedoCore() {
			Styles.SetDefaultTableStyleNameCore(newName);
		}
	}
	#endregion
	#region TableStyleCollectionSetDefaultPivotStyleHistoryItem
	public class TableStyleCollectionSetDefaultPivotStyleHistoryItem : TableStyleCollectionHistoryItemBase {
		#region Fields
		readonly string oldName;
		readonly string newName;
		#endregion
		public TableStyleCollectionSetDefaultPivotStyleHistoryItem(TableStyleCollection styles, string oldName, string newName)
			: base(styles) {
			this.oldName = oldName;
			this.newName = newName;
		}
		protected override void UndoCore() {
			Styles.SetDefaultPivotStyleNameCore(oldName);
		}
		protected override void RedoCore() {
			Styles.SetDefaultPivotStyleNameCore(newName);
		}
	}
	#endregion
	#region TableStyleCollectionHasDefaultTableStyleNameHistoryItem
	public class TableStyleCollectionHasDefaultTableStyleNameHistoryItem : TableStyleCollectionHistoryItemBase {
		#region Fields
		readonly bool oldValue;
		readonly bool newValue;
		#endregion
		public TableStyleCollectionHasDefaultTableStyleNameHistoryItem(TableStyleCollection styles, bool oldValue, bool newValue)
			: base(styles) {
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		protected override void UndoCore() {
			Styles.SetHasDefaultTableStyleNameCore(oldValue);
		}
		protected override void RedoCore() {
			Styles.SetHasDefaultTableStyleNameCore(newValue);
		}
	}
	#endregion
	#region TableStyleCollectionHasDefaultPivotStyleNameHistoryItem
	public class TableStyleCollectionHasDefaultPivotStyleNameHistoryItem : TableStyleCollectionHistoryItemBase {
		#region Fields
		readonly bool oldValue;
		readonly bool newValue;
		#endregion
		public TableStyleCollectionHasDefaultPivotStyleNameHistoryItem(TableStyleCollection styles, bool oldValue, bool newValue)
			: base(styles) {
			this.oldValue = oldValue;
			this.newValue = newValue;
		}
		protected override void UndoCore() {
			Styles.SetHasDefaultPivotStyleNameCore(oldValue);
		}
		protected override void RedoCore() {
			Styles.SetHasDefaultPivotStyleNameCore(newValue);
		}
	}
	#endregion
	#endregion
	#region MarkupTablesForInvalidateFormatCacheHistoryItem
	public class MarkupTablesForInvalidateFormatCacheHistoryItem : SpreadsheetHistoryItem {
		readonly TableCollection tables;
		readonly CellRange range;
		public MarkupTablesForInvalidateFormatCacheHistoryItem(TableCollection tables, CellRange range)
			: base(tables.DocumentModelPart) {
			this.tables = tables;
			this.range = range;
		}
		protected override void RedoCore() {
			tables.InvalidateFormatCache(range);
		}
		protected override void UndoCore() {
			tables.InvalidateFormatCache(range);
		}
	}
	#endregion
}
