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
using DevExpress.Compatibility.System.Drawing;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Mouse;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Forms;
using DevExpress.XtraSpreadsheet.Commands.Internal;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Layout {
	#region PivotTableLayout
	public class PivotTableLayout : DocumentItemLayout {
		#region Fields
		readonly HotZoneCollection hotZones;
		Page lastPage;
		#endregion
		public PivotTableLayout(SpreadsheetView view)
			: base(view) {
			this.hotZones = new HotZoneCollection();
		}
		#region Properties
		public HotZoneCollection HotZones { get { return hotZones; } }
		#endregion
		public override void Update(Page page) {
			Invalidate();
			foreach (SingleCellTextBox box in page.Boxes)
				UpdateItems(page, box);
			foreach (ComplexCellTextBox box in page.ComplexBoxes)
				UpdateItems(page, box);
			this.lastPage = page;
		}
		void UpdateItems(Page page, ICellTextBox box) {
			AddExpandCollapseHotZone(page, box);
			AddPivotFilterHotZone(page, box);
		}
		void AddExpandCollapseHotZone(Page page, ICellTextBox box) {
			if (!box.HasPivotExpandCollapseButton)
				return;
			ICell cell = box.GetCell(page.GridColumns, page.GridRows, page.Sheet);
			PivotTableExpandCollapseHotZone hotZone = new PivotTableExpandCollapseHotZone(View.Control.InnerControl, cell.Position, box.GetPivotTableFieldIndex(), box.GetPivotTableItemIndex(), box.IsPivotButtonCollapsed);
			hotZone.Bounds = box.CalculateExpandCollapseButtonBounds(page, cell);
			hotZones.Add(hotZone);
		}
		void AddPivotFilterHotZone(Page page, ICellTextBox box) {
			if (!box.HasPivotLabelFilterButton && !box.HasPivotPageFilterButton)
				return;
			CellPosition position = box.GetCell(page.GridColumns, page.GridRows, page.Sheet).Position;
			int columnGridIndex = page.GridColumns.LookupItem(position.Column);
			if (columnGridIndex < 0)
				return;
			int rowGridIndex = page.GridRows.LookupItem(position.Row);
			if (rowGridIndex < 0)
				return;
			PivotTableFilterHotZone hotZone = new PivotTableFilterHotZone(View.Control.InnerControl, position);
			hotZone.FieldIndex = box.GetPivotTableFieldIndex();
			if (box.HasPivotPageFilterButton)
				hotZone.IsPageFilter = true;
			if (box.HasPivotLabelFilterButton)
				if (box.IsPivotRowFieldsFilter)
					hotZone.IsRowFilter = true;
				else
					hotZone.IsRowFilter = false;
			PageGridItem column = page.GridColumns[columnGridIndex];
			PageGridItem row = page.GridRows[rowGridIndex];
			int size = Math.Min(AutoFilterLayout.ImageSize, Math.Min(column.Extent, row.Extent));
			hotZone.BoundsHotZone = Rectangle.FromLTRB(column.Far - size, row.Far - size, column.Far, row.Far);
			hotZones.Add(hotZone);
		}
		public override void Invalidate() {
			this.hotZones.Clear();
		}
		protected internal override HotZone CalculateHotZone(Point point, Page page) {
			if (!object.ReferenceEquals(lastPage, page) && page != null)
				Update(page);
			return HotZoneCalculator.CalculateHotZone(HotZones, point, View.ZoomFactor, LayoutUnitConverter);
		}
	}
	#endregion
	#region PivotTableExpandCollapseHotZone
	public class PivotTableExpandCollapseHotZone : HotZone {
		#region Fields
		readonly InnerSpreadsheetControl control;
		CellPosition position;
		int fieldIndex;
		int itemIndex;
		bool isCollapsed;
		#endregion
		public PivotTableExpandCollapseHotZone(InnerSpreadsheetControl control, CellPosition position, int fieldIndex, int itemIndex, bool isCollapsed)
			: base(control) {
			this.control = control;
			this.position = position;
			this.fieldIndex = fieldIndex;
			this.itemIndex = itemIndex;
			this.isCollapsed = isCollapsed;
		}
		#region Properties
		public override SpreadsheetCursor Cursor { get { return SpreadsheetCursors.Default; } }
		public bool IsCollapsed { get { return isCollapsed; } }
		#endregion
		public override void Activate(SpreadsheetMouseHandler handler, SpreadsheetHitTestResult result) {
			PivotTable pivot = control.DocumentModel.ActiveSheet.TryGetPivotTable(position);
			if (pivot == null)
				return;
			pivot.SetItemIsCollapsed(fieldIndex, itemIndex, !isCollapsed, control.ErrorHandler);
		}
		public override void Visit(IHotZoneVisitor visitor) {
			visitor.Visit(this);
		}
	}
	#endregion
	#region PivotTableFilterHotZone
	public class PivotTableFilterHotZone : HotZone, IFilterHotZone {
		#region Fields
		readonly InnerSpreadsheetControl control;
		CellPosition position;
		#endregion
		public PivotTableFilterHotZone(InnerSpreadsheetControl control, CellPosition position)
			: base(control) {
			this.control = control;
			this.position = position;
		}
		#region Properties
		public override SpreadsheetCursor Cursor { get { return SpreadsheetCursors.Default; } }
		public Rectangle BoundsHotZone {
			get { return Bounds; }
			set {
				if (BoundsHotZone == value)
					return;
				Bounds = value;
			}
		}
		public bool IsRowFilter { get; set; }
		public bool IsPageFilter { get; set; }
		public int FieldIndex { get; set; }
		public bool IsFilterApplied {
			get {
				PivotTable table = control.DocumentModel.ActiveSheet.TryGetPivotTable(position);
				if (table == null)
					return false;
				if (IsPageFilter)
					return table.Fields[FieldIndex].Items.HiddenItemsCount > 0;
				return GetFilterStateOfPivotField(table, FieldIndex, IsRowFilter ? table.RowFields : table.ColumnFields);
			}
		}
		public bool IsSortApplied {
			get {
				PivotTable table = control.DocumentModel.ActiveSheet.TryGetPivotTable(position);
				if (table == null)
					return false;
				if (!IsPageFilter)
					return GetSortStateOfPivotField(table, FieldIndex, IsRowFilter ? table.RowFields : table.ColumnFields);
				return false;
			}
		}
		public bool SortTypeDescending { get; set; }
		#endregion
		bool GetFilterStateOfPivotField(PivotTable table, int fieldIndex, PivotTableColumnRowFieldIndices collection) {
			if (fieldIndex == -1) {
				for (int i = 0; i < collection.Count; i++) {
					int currentFieldIndex = collection[i].FieldIndex;
					if (currentFieldIndex >= 0) {
						PivotField field = table.Fields[currentFieldIndex];
						if (field.IsFilterApplied(currentFieldIndex))
							return true;
					}
				}
				return false;
			}
			else
				return table.Fields[FieldIndex].IsFilterApplied(FieldIndex);
		}
		bool GetSortStateOfPivotField(PivotTable table, int fieldIndex, PivotTableColumnRowFieldIndices collection) {
			if (fieldIndex >= 0)
				return GetSortStateOfPivotFieldCore(table, fieldIndex);
			for (int i = 0; i < collection.Count; i++) {
				int currentFieldIndex = collection[i].FieldIndex;
				if (currentFieldIndex >= 0 && GetSortStateOfPivotFieldCore(table, currentFieldIndex))
					return true;
			}
			return false;
		}
		bool GetSortStateOfPivotFieldCore(PivotTable table, int fieldIndex) {
			PivotTableSortTypeField sortType = table.Fields[fieldIndex].SortType;
			if (sortType != PivotTableSortTypeField.Manual) {
				SortTypeDescending = sortType == PivotTableSortTypeField.Descending;
				return true;
			}
			return false;
		}
		int GetActualFieldIndexInRowColumnCollection(PivotTableColumnRowFieldIndices fields) {
			foreach (PivotFieldReference fieldReference in fields)
				if (fieldReference.FieldIndex >= 0)
					return fieldReference.FieldIndex;
			return -1;
		}
		public override void Activate(SpreadsheetMouseHandler handler, SpreadsheetHitTestResult result) {
			Worksheet ActiveSheet = control.DocumentModel.ActiveSheet;
			PivotTable table = ActiveSheet.TryGetPivotTable(position);
			if (table == null)
				return;
			int tableIndex = ActiveSheet.PivotTables.IndexOf(table);
			ActiveSheet.PivotTableStaticInfo = new PivotTableStaticInfo(tableIndex, GetAxis());
			PivotTableColumnRowFieldIndices collection = IsRowFilter ? table.RowFields : table.ColumnFields;
			ActiveSheet.PivotTableStaticInfo.IsContextMenuFieldGroupActive = FieldIndex == -1 && !(collection.Count == 2 && collection.HasValuesField);
			ActiveSheet.PivotTableStaticInfo.PointActivateFilter = result.LogicalPoint;
			if (IsPageFilter) {
				ActiveSheet.PivotTableStaticInfo.FieldIndex = FieldIndex;
				PivotTablePageFieldsFilterItemsCommand command = new PivotTablePageFieldsFilterItemsCommand(control.Owner);
				command.Execute();
			}
			else {
				PivotTableStaticInfo info = ActiveSheet.PivotTableStaticInfo;
				info.FieldIndex = (FieldIndex == -1) ? GetActualFieldIndexInRowColumnCollection(collection) : FieldIndex;
				table.Filters.SetupPivotInfo(info, table.MultipleFieldFilters);
				control.Owner.ShowPivotTableAutoFilterForm();
			}
		}
		PivotTableAxis GetAxis() {
			if (IsPageFilter)
				return PivotTableAxis.Page;
			return IsRowFilter ? PivotTableAxis.Row : PivotTableAxis.Column;
		}
		public override void Visit(IHotZoneVisitor visitor) {
			visitor.Visit(this);
		}
	}
	#endregion
	public interface IFilterHotZone {
		Rectangle BoundsHotZone { get; set; }
	}
}
