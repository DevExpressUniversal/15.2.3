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
using System.Text;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using System.Drawing;
using LayoutUnit = System.Int32;
using ModelUnit = System.Int32;
using DevExpress.Office;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit.Layout.TableLayout {
	#region TableRowViewInfoBase (abstract class)
	public abstract class TableRowViewInfoBase {
		readonly TableCellViewInfoCollection cells;
		readonly TableViewInfo tableViewInfo;
		readonly List<TableCellVerticalBorderViewInfo> verticalBorders;
		readonly int rowIndex;
		protected TableRowViewInfoBase(TableViewInfo tableViewInfo, int rowIndex) {
			Guard.ArgumentNotNull(tableViewInfo, "tableViewInfo");
			this.cells = new TableCellViewInfoCollection();
			this.tableViewInfo = tableViewInfo;
			this.rowIndex = rowIndex;
			this.verticalBorders = CalculateVerticalBorders();
		}
		int TopAnchorIndex { get { return rowIndex - TableViewInfo.TopRowIndex; } }
		int BottomAnchorIndex { get { return TopAnchorIndex + 1; } }
		public TableViewInfo TableViewInfo { get { return tableViewInfo; } }
		public TableCellViewInfoCollection Cells { get { return cells; } }
		public ITableCellVerticalAnchor TopAnchor { get { return TableViewInfo.Anchors[TopAnchorIndex]; } }
		public ITableCellVerticalAnchor BottomAnchor { get { return TableViewInfo.Anchors[BottomAnchorIndex]; } }
		public List<TableCellVerticalBorderViewInfo> VerticalBorders { get { return verticalBorders; } }
		public TableRow Row { get { return tableViewInfo.Table.Rows[rowIndex]; } }
		public TableRowViewInfoBase Previous {
			get {
				int viewInfoRowIndex = rowIndex - TableViewInfo.TopRowIndex;
				if (viewInfoRowIndex <= 0)
					return null;
				return TableViewInfo.Rows[viewInfoRowIndex - 1];
			}
		}
		public TableRowViewInfoBase Next {
			get {
				int viewInfoRowIndex = rowIndex - TableViewInfo.TopRowIndex;
				if (viewInfoRowIndex + 1 < TableViewInfo.Rows.Count)
					return TableViewInfo.Rows[viewInfoRowIndex + 1];
				else
					return null;
			}
		}
		public abstract Rectangle GetVerticalBorderBounds(int layoutBorderIndex);
		protected abstract List<TableCellVerticalBorderViewInfo> CalculateVerticalBorders();
		public abstract void ExportTo(IDocumentLayoutExporter exporter);
		public abstract Rectangle GetBounds();
		protected virtual bool IsLastRow() {
			return this == TableViewInfo.Rows.Last;
		}
		protected virtual bool IsFirstRow() {
			return this == TableViewInfo.Rows.First;
		}
		public virtual bool ContainsEmptyCell() {
			for (int i = 0; i < this.Cells.Count; i++) {
				if (Cells[i].EmptyCell)
					return true;
			}
			return false;
		}
	}
	#endregion
	public class VerticalBorderAndColumnIndexComparer : IComparable<TableCellVerticalBorderViewInfo> {
		int layoutBorderIndex;
		public VerticalBorderAndColumnIndexComparer(int layoutBorderIndex) {
			this.layoutBorderIndex = layoutBorderIndex;
		}
		#region IComparable<TableCellVerticalBorderViewInfo> Members
		public int CompareTo(TableCellVerticalBorderViewInfo other) {
			return other.LayoutBorderIndex - layoutBorderIndex;
		}
		#endregion
	}
	#region TableRowViewInfoNoCellSpacing
	public class TableRowViewInfoNoCellSpacing : TableRowViewInfoBase {
		public TableRowViewInfoNoCellSpacing(TableViewInfo tableViewInfo, int rowIndex)
			: base(tableViewInfo, rowIndex) {
		}
		protected override List<TableCellVerticalBorderViewInfo> CalculateVerticalBorders() {
			TableRow row = Row;
			TableCellCollection cells = row.Cells;
			int count = cells.Count;
			List<TableCellVerticalBorderViewInfo> result = new List<TableCellVerticalBorderViewInfo>();
			TableBorderCalculator calculator = new TableBorderCalculator();
			int layoutBorderIndex = row.LayoutProperties.GridBefore;
			int modelBorderIndex = row.GridBefore;
			Table table = row.Table;
			for (int i = 0; i <= count; i++) {
				BorderInfo leftCellBorder = i > 0 ? cells[i - 1].GetActualRightCellBorder().Info : null;
				BorderInfo rightCellBorder = i < count ? cells[i].GetActualLeftCellBorder().Info : null;
				BorderInfo resultBorder = calculator.GetVerticalBorderSource(table, leftCellBorder, rightCellBorder);
				result.Add(new TableCellVerticalBorderViewInfo(this, resultBorder, layoutBorderIndex, modelBorderIndex, TableViewInfo.Table.DocumentModel.ToDocumentLayoutUnitConverter));
				if (i < count) {
					layoutBorderIndex += cells[i].LayoutProperties.ColumnSpan;
					modelBorderIndex += cells[i].ColumnSpan;
				}
			}
			return result;
		}
		protected virtual int GetBorderIndexByColumnIndex(int layoutBorderIndex) {
			return Algorithms.BinarySearch(VerticalBorders, new VerticalBorderAndColumnIndexComparer(layoutBorderIndex));
		}
		public override Rectangle GetVerticalBorderBounds(int layoutBorderIndex) {
			int borderIndex = GetBorderIndexByColumnIndex(layoutBorderIndex);
			TableCellVerticalBorderViewInfo border = VerticalBorders[borderIndex];
			Rectangle result = border.GetBounds(TableViewInfo);
			TableBorderCalculator calculator = new TableBorderCalculator();
			ModelUnit actualWidth = calculator.GetActualWidth(border.Border);
			LayoutUnit layoutActualWidth = Row.DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(actualWidth);
			return new Rectangle(result.X - layoutActualWidth / 2, result.Y, layoutActualWidth, result.Height);
		}
		public override void ExportTo(IDocumentLayoutExporter exporter) {
			if (VerticalBorders == null)
				return;
			int count = VerticalBorders.Count;
			for (int i = 0; i < count; i++) {
				VerticalBorders[i].ExportTo(TableViewInfo, exporter);
			}
		}
		public override Rectangle GetBounds() {
			int top = TopAnchor.VerticalPosition;
			int bottom = BottomAnchor.VerticalPosition;
			if (IsLastRow())
				bottom += BottomAnchor.BottomTextIndent;
			int left = TableViewInfo.GetVerticalBorderPosition(Row.LayoutProperties.GridBefore);
			int right = TableViewInfo.GetVerticalBorderPosition(Row.Table.GetLastCellColumnIndexConsiderRowGrid(Row, true));
			return new Rectangle(left, top, right - left, bottom - top);
		}
	}
	#endregion
	#region TableRowViewInfoWithCellSpacing
	public class TableRowViewInfoWithCellSpacing : TableRowViewInfoBase {
		ModelUnit cellSpacing;
		public TableRowViewInfoWithCellSpacing(TableViewInfo tableViewInfo, int rowIndex, ModelUnit cellSpacing)
			: base(tableViewInfo, rowIndex) {
			this.cellSpacing = cellSpacing;
		}
		protected DocumentModelUnitToLayoutUnitConverter Converter { get { return TableViewInfo.Table.DocumentModel.ToDocumentLayoutUnitConverter; } }
		public ModelUnit CellSpacing { get { return cellSpacing; } set { cellSpacing = value; } }
		protected override List<TableCellVerticalBorderViewInfo> CalculateVerticalBorders() {
			return new List<TableCellVerticalBorderViewInfo>();
		}
		public override Rectangle GetVerticalBorderBounds(int layoutBorderIndex) {
			LayoutUnit position = TableViewInfo.VerticalBorderPositions.AlignmentedPosition[layoutBorderIndex] + TableViewInfo.Column.Bounds.Left;
			LayoutUnit top = TopAnchor.VerticalPosition;
			LayoutUnit bottom = BottomAnchor.VerticalPosition;
			int cellIndex = Row.Table.GetAbsoluteCellIndexInRow(Row, layoutBorderIndex, true);
			TableBorderCalculator calculator = new TableBorderCalculator();
			TableCellCollection cells = Row.Cells;
			ModelUnit borderAtLeftWidth = cellIndex > 0 ? calculator.GetActualWidth(cells[cellIndex - 1].GetActualRightCellBorder()) : 0;
			ModelUnit borderAtRightWidth = cellIndex < cells.Count ? calculator.GetActualWidth(cells[cellIndex].GetActualLeftCellBorder()) : 0;
			LayoutUnit width = Converter.ToLayoutUnits(borderAtLeftWidth + borderAtRightWidth + cellSpacing);
			return new Rectangle(position - width / 2, top, width, bottom - top);
		}
		public override void ExportTo(IDocumentLayoutExporter exporter) {
			if (VerticalBorders == null)
				return;
			int cellCount = Cells.Count;
			ExportTableBorders(exporter);
			for (int i = 0; i < cellCount; i++) {
				TableCellViewInfo cellViewInfo = Cells[i];
				LeftTableCellBorderViewInfo leftBorderViewInfo = new LeftTableCellBorderViewInfo(cellViewInfo);
				RightTableCellBorderViewInfo rightBorderViewInfo = new RightTableCellBorderViewInfo(cellViewInfo);
				TopTableCellBorderViewInfo topBorderViewInfo = new TopTableCellBorderViewInfo(cellViewInfo);
				BottomTableCellBorderViewInfo bottomBorderViewInfo = new BottomTableCellBorderViewInfo(cellViewInfo);
				Rectangle topBounds = topBorderViewInfo.GetBoundsCore(TableViewInfo);
				CornerViewInfoBase topLeftCorner = CornerViewInfoBase.CreateCorner(Converter, null, null, topBorderViewInfo.Border, leftBorderViewInfo.Border, cellSpacing);				
				exporter.ExportTableBorderCorner(topLeftCorner, topBounds.Left, topBounds.Top);
				CornerViewInfoBase topRightCorner = CornerViewInfoBase.CreateCorner(Converter, topBorderViewInfo.Border, null, null, rightBorderViewInfo.Border, cellSpacing);
				exporter.ExportTableBorderCorner(topRightCorner, topBounds.Right, topBounds.Top);
				Rectangle bottomBounds = bottomBorderViewInfo.GetBoundsCore(TableViewInfo);
				CornerViewInfoBase bottomLeftCorner = CornerViewInfoBase.CreateCorner(Converter, null, leftBorderViewInfo.Border, bottomBorderViewInfo.Border, null, cellSpacing);
				exporter.ExportTableBorderCorner(bottomLeftCorner, bottomBounds.Left, bottomBounds.Bottom);
				CornerViewInfoBase bottomRightCorner = CornerViewInfoBase.CreateCorner(Converter, bottomBorderViewInfo.Border, rightBorderViewInfo.Border, null, null, cellSpacing);
				exporter.ExportTableBorderCorner(bottomRightCorner, bottomBounds.Right, bottomBounds.Bottom);
				leftBorderViewInfo.TopCorner = topLeftCorner;
				leftBorderViewInfo.BottomCorner = bottomLeftCorner;
				rightBorderViewInfo.TopCorner = topRightCorner;
				rightBorderViewInfo.BottomCorner = bottomRightCorner;
				topBorderViewInfo.LeftCorner = topLeftCorner;
				topBorderViewInfo.RightCorner = topRightCorner;
				bottomBorderViewInfo.LeftCorner = bottomLeftCorner;
				bottomBorderViewInfo.RightCorner = bottomRightCorner;
				leftBorderViewInfo.ExportTo(TableViewInfo, exporter);
				rightBorderViewInfo.ExportTo(TableViewInfo, exporter);
				topBorderViewInfo.ExportTo(TableViewInfo, exporter);
				bottomBorderViewInfo.ExportTo(TableViewInfo, exporter);
			}
		}
		protected internal virtual void ExportTableBorders(IDocumentLayoutExporter exporter) {
			LeftTableBorderWithSpacingViewInfo leftTableBorderViewInfo = new LeftTableBorderWithSpacingViewInfo(this);
			RightTableBorderWithSpacingViewInfo rightTableBorderViewInfo = new RightTableBorderWithSpacingViewInfo(this);
			TopTableBorderWithSpacingViewInfo topTableBorderViewInfo = IsFirstRow() ? new TopTableBorderWithSpacingViewInfo(this) : null;
			BottomTableBorderWithSpacingViewInfo bottomTableBorderViewInfo = IsLastRow() ? new BottomTableBorderWithSpacingViewInfo(this) : null;
			Rectangle leftBounds = leftTableBorderViewInfo.GetBounds(TableViewInfo);
			BorderInfo topBorder = topTableBorderViewInfo != null ? topTableBorderViewInfo.Border : null;
			CornerViewInfoBase topLeftCorner = CornerViewInfoBase.CreateCorner(Converter, null, null, topBorder, leftTableBorderViewInfo.Border, 0);
			exporter.ExportTableBorderCorner(topLeftCorner, leftBounds.Left, leftBounds.Top);
			CornerViewInfoBase topRightCorner = CornerViewInfoBase.CreateCorner(Converter, topBorder, null, null, rightTableBorderViewInfo.Border, 0);
			exporter.ExportTableBorderCorner(topRightCorner, leftBounds.Right, leftBounds.Top);
			Rectangle rightBounds = rightTableBorderViewInfo.GetBounds(TableViewInfo);
			BorderInfo bottomBorder = bottomTableBorderViewInfo != null ? bottomTableBorderViewInfo.Border : null;
			CornerViewInfoBase bottomLeftCorner = CornerViewInfoBase.CreateCorner(Converter, null, leftTableBorderViewInfo.Border, bottomBorder, null, 0);
			exporter.ExportTableBorderCorner(bottomLeftCorner, rightBounds.Left, rightBounds.Bottom);
			CornerViewInfoBase bottomRightCorner = CornerViewInfoBase.CreateCorner(Converter, bottomBorder, rightTableBorderViewInfo.Border, null, null, 0);
			exporter.ExportTableBorderCorner(bottomRightCorner, rightBounds.Right, rightBounds.Bottom);
			leftTableBorderViewInfo.LeftCorner = topLeftCorner;
			leftTableBorderViewInfo.RightCorner = bottomLeftCorner;
			rightTableBorderViewInfo.LeftCorner = topRightCorner;
			rightTableBorderViewInfo.RightCorner= bottomRightCorner;
			if (topTableBorderViewInfo != null) {
				topTableBorderViewInfo.LeftCorner = topLeftCorner;
				topTableBorderViewInfo.RightCorner = topRightCorner;
			}
			if (bottomTableBorderViewInfo != null) {
				bottomTableBorderViewInfo.LeftCorner = bottomLeftCorner;
				bottomTableBorderViewInfo.RightCorner = bottomRightCorner;
			}
			leftTableBorderViewInfo.ExportTo(TableViewInfo, exporter);
			rightTableBorderViewInfo.ExportTo(TableViewInfo, exporter);
			if (topTableBorderViewInfo != null)
				topTableBorderViewInfo.ExportTo(TableViewInfo, exporter);
			if (bottomTableBorderViewInfo != null)
				bottomTableBorderViewInfo.ExportTo(TableViewInfo, exporter);
		}
		public override Rectangle GetBounds() {
			int top = TopAnchor.VerticalPosition;
			if (!IsFirstRow() && (Previous is TableRowViewInfoWithCellSpacing))
				top += Converter.ToLayoutUnits(((TableRowViewInfoWithCellSpacing)Previous).cellSpacing) / 2;
			int bottom = BottomAnchor.VerticalPosition;
			int cellSpacing = Converter.ToLayoutUnits(CellSpacing);
			if (IsLastRow())
				bottom += cellSpacing;
			else
				bottom += cellSpacing / 2;
			TableCellViewInfo firstCellViewInfo = Cells[0];
			int left = firstCellViewInfo.Left - cellSpacing;
			TableCellViewInfo lastCellViewInfo = Cells.Last;
			int right = lastCellViewInfo.Left + lastCellViewInfo.Width + cellSpacing;
			return new Rectangle(left, top, right - left, bottom - top);
		}
	}
	#endregion
	#region TableRowViewInfoCollection
	public class TableRowViewInfoCollection : List<TableRowViewInfoBase> {
		readonly TableViewInfo tableViewInfo;
		public TableRowViewInfoCollection(TableViewInfo tableViewInfo) {
			Guard.ArgumentNotNull(tableViewInfo, "tableViewInfo");
			this.tableViewInfo = tableViewInfo;
		}
		public TableViewInfo TableViewInfo { get { return tableViewInfo; } }
		public TableRowViewInfoBase Last { get { return Count > 0 ? this[Count - 1] : null; } }
		public TableRowViewInfoBase First { get { return Count > 0 ? this[0] : null; } }
		public void RemoveRows(int startRowIndex, int rowCount) {
			for (int i = startRowIndex + rowCount - 1; i >= startRowIndex; i--)
				RemoveAt(i);
		}
		public void ShiftForward(int delta) {
			Guard.ArgumentNonNegative(delta, "delta");
			int initialRowCount = this.Count;
			for (int i = 0; i < delta; i++)
				Add(null);
			for (int i = initialRowCount - 1; i >= 0; i--) {
				this[i + delta] = this[i];
			}
			for (int i = 0; i < delta; i++)
				this[i] = null;
		}
	}
	#endregion
}
