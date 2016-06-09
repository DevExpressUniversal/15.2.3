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
using System.Drawing;
using System.Linq;
using DevExpress.Web.Internal;
using DevExpress.Web.Data;
using DevExpress.XtraPrinting;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.XtraExport;
namespace DevExpress.Web.Export {
	public class GridViewPrintInfo {
		int gridWidth = 0;
		int footerHeight = 0;
		Dictionary<GridViewColumn, Rectangle> headerRects;
		Dictionary<GridViewColumn, int> columnWidths;
		Dictionary<int, int> dataRowHeights;
		Dictionary<int, int> groupRowHeights;
		Dictionary<int, int> groupFooterRowHeights;
		public GridViewPrintInfo() {
			this.headerRects = new Dictionary<GridViewColumn, Rectangle>();
			this.columnWidths = new Dictionary<GridViewColumn, int>();
			this.dataRowHeights = new Dictionary<int, int>();
			this.groupRowHeights = new Dictionary<int, int>();
			this.groupFooterRowHeights = new Dictionary<int, int>();
		}
		public int GridWidth { get { return gridWidth; } }
		public int FooterHeight { get { return this.footerHeight; } }
		public Rectangle GetHeaderRect(GridViewColumn column) {
			return this.headerRects[column];
		}
		public int GetColumnWidth(GridViewColumn column) {
			return this.columnWidths[column];
		}
		public int GetDataRowHeight(int index) {
			return GetRowHeight(this.dataRowHeights, index);
		}
		public int GetGroupRowHeight(int index) {
			return GetRowHeight(this.groupRowHeights, index);
		}
		public int GetGroupFooterRowHeight(int index) {
			return GetRowHeight(this.groupFooterRowHeights, index);
		}
		public void UpdateGridWidth(int width) {
			this.gridWidth = width;
		}
		public void UpdateFooterHeight(int height) {
			this.footerHeight = Math.Max(this.footerHeight, height);
		}
		public void UpdateHeaderRect(GridViewColumn column, Rectangle rect) {
			this.headerRects[column] = rect;
		}
		public void UpdateColumnWidth(GridViewColumn column, int width) {
			this.columnWidths[column] = width;
		}
		public void UpdateDataRowHeight(int index, int height) {
			UpdateRowHeight(this.dataRowHeights, index, height);
		}
		public void UpdateGroupRowHeight(int index, int height) {
			UpdateRowHeight(this.groupRowHeights, index, height);
		}
		public void UpdateGroupFooterRowHeight(int index, int height) {
			UpdateRowHeight(this.groupFooterRowHeights, index, height);
		}
		int GetRowHeight(Dictionary<int, int> heights, int index) {
			if(!heights.ContainsKey(index))
				return 0;
			return heights[index];
		}
		void UpdateRowHeight(Dictionary<int, int> heights, int index, int height) {
			if(!heights.ContainsKey(index))
				heights[index] = 0;
			heights[index] = Math.Max(heights[index], height);
		}
	}
	public class GridViewPrintInfoCalculator {
		const string EmptyHeaderCellText = " ";
		GridViewPrinter printer;
		int[,] matrix;
		public GridViewPrintInfoCalculator() { 
		}
		protected GridViewPrinter Printer { get { return printer; } }
		protected ASPxGridViewExporter Exporter { get { return Printer.Exporter; } }
		protected GridExportStyleHelper StyleHelper { get { return Printer.StyleHelper; } }
		protected ASPxGridView Grid { get { return Printer.Grid; } }		
		protected WebDataProxy DataProxy { get { return Grid.DataBoundProxy; } }
		protected GridViewColumnHelper ColumnHelper { get { return Printer.ColumnHelper; } }
		protected GridViewPrintInfo PrintInfo { get { return Printer.PrintInfo; } }
		protected int[,] Matrix { get { return matrix; } set { matrix = value; } }
		public void Calculate(GridViewPrinter printer) {
			this.printer = printer;
			try {
				CalculateCore();
			} finally {
				this.printer = null;
			}
		}
		void CalculateCore() {
			foreach(GridViewColumnVisualTreeNode node in ColumnHelper.Leafs) {
				PrintInfo.UpdateColumnWidth(node.Column, CalcColumnWidth(node.Column));
			}
			PrintInfo.UpdateGridWidth(CalculateGridWidth());
			for(int i = 0; i < DataProxy.VisibleRowCountOnPage; i++) {
				if(Printer.GetRowType(i) == WebRowType.Group)
					PrintInfo.UpdateGroupRowHeight(i, GetGroupHeight(i));
			}
			CalculateHeaderSizes();
			PrintInfo.UpdateGridWidth(CalculateGridWidth());
		}
		int CalculateGridWidth() {
			int result = Printer.GetGroupLevelOffSet(Grid.GroupCount);
			foreach(GridViewColumnVisualTreeNode node in ColumnHelper.Leafs)
				result += PrintInfo.GetColumnWidth(node.Column);
			if(ColumnHelper.Leafs.Count == 0)
				result += Exporter.MaxColumnWidth;
			return result;
		}
		int CalcColumnWidth(GridViewColumn column) {
			int maxWidth = GetMinColumnWidth(column);
			foreach(int i in Printer.EnumerateExportedVisibleRows()) {
				maxWidth = Math.Max(maxWidth, CalcGroupFootersWidth(column, i));
				if(Printer.GetRowType(i) != WebRowType.Data)
					continue;
				maxWidth = Math.Max(maxWidth, CalcCellWidth(column, i));
			}
			return Math.Max(maxWidth, CalcFooterWidth(column));
		}
		int CalcCellWidth(GridViewColumn column, int rowIndex) {
			GridViewDataColumn dataColumn = column as GridViewDataColumn;
			if(dataColumn == null)
				return 12; 
			var style = StyleHelper.GetCellStyle(Printer.Graph, (IWebGridDataColumn)column, rowIndex, false, HorizontalAlign.Left, false);
			var size = GetColumnSize(column, Printer.GetDataRowText(dataColumn, rowIndex), style);
			PrintInfo.UpdateDataRowHeight(rowIndex, size.Height);
			return size.Width;
		}
		int CalcGroupFootersWidth(GridViewColumn column, int rowIndex) {
			List<int> indexes = Printer.GetGroupFooterVisibleIndexes(rowIndex);
			if(indexes == null)
				return 0;
			int width = 0;
			for(int i = 0; i < indexes.Count; i++) {
				width = Math.Max(width, CalcGroupFooterWidth(column, indexes[i]));
			}
			return width;
		}
		int CalcGroupFooterWidth(GridViewColumn column, int parentGroupVisibleIndex) {
			Size size = GetColumnSize(column, Printer.GetGroupFooterText(column, parentGroupVisibleIndex), StyleHelper.GetGroupFooterStyle(Printer.Graph, HorizontalAlign.Left));
			PrintInfo.UpdateGroupFooterRowHeight(parentGroupVisibleIndex, size.Height);
			return size.Width;
		}
		int CalcFooterWidth(GridViewColumn column) {
			int minWidth = GetMinColumnWidth(column);
			if(!Printer.ShowFooter)
				return minWidth;
			string text = Printer.GetFooterText(column);
			if(string.IsNullOrEmpty(text))
				return minWidth;
			Size size = GetColumnSize(column, text, StyleHelper.GetHeaderStyle(Printer.Graph, HorizontalAlign.Left)); 
			PrintInfo.UpdateFooterHeight(size.Height);
			return Math.Max(minWidth, size.Width);
		}
		void CalculateHeaderSizes() {
			Dictionary<GridViewColumn, int> headerWidths = CalculateHeaderWidths();
			foreach(GridViewColumnVisualTreeNode leaf in ColumnHelper.Leafs)
				PrintInfo.UpdateColumnWidth(leaf.Column, headerWidths[leaf.Column]);
			Matrix = CreateMatrix();
			Dictionary<GridViewColumn, int> headerLeftOffsets = new Dictionary<GridViewColumn, int>();
			int headerTableHeight = 0;
			int levelTop = 0;
			int[] levelHeights = new int[ColumnHelper.Layout.Count];
			for(int i = 0; i < ColumnHelper.Layout.Count; i++) {
				int lastFreeCellIndex = 0;
				int maxLevelHeight = 0;
				foreach(GridViewColumnVisualTreeNode node in ColumnHelper.Layout[i]) {
					int left = 0;
					var column = node.Column;
					int cellIndex = FindFreeCellIndex(i, lastFreeCellIndex);
					AddColumnToMatrix(node, i, cellIndex);
					if(cellIndex == 0) {
						headerWidths[column] += Printer.GetGroupLevelOffSet(Grid.GroupCount);
					} else {
						int[] leftColumnIndices = FindLeftColumnIndices(i, cellIndex);
						for(int j = 0; j < leftColumnIndices.Length; j++)
							left += headerWidths[ColumnHelper.AllColumns[leftColumnIndices[j]]];
					}
					headerLeftOffsets[column] = left;
					lastFreeCellIndex = cellIndex + node.ColSpan;
					Size size = GetHeaderCellSize(headerWidths[column], column);
					if(node.RowSpan > 1)
						headerTableHeight = Math.Max(headerTableHeight, size.Height + levelTop);
					else
						maxLevelHeight = Math.Max(maxLevelHeight, size.Height);
				}
				headerTableHeight = Math.Max(headerTableHeight, maxLevelHeight + levelTop);
				levelHeights[i] = maxLevelHeight;
				levelTop += levelHeights[i];
			}
			Matrix = null;
			levelTop = 0;
			for(int i = 0; i < ColumnHelper.Layout.Count; i++) {
				foreach(GridViewColumnVisualTreeNode node in ColumnHelper.Layout[i]) {
					int height;
					if(i == ColumnHelper.Layout.Count - 1) {
						height = headerTableHeight - levelTop;
					} else {
						if(node.RowSpan > 1)
							height = headerTableHeight - levelTop;
						else
							height = levelHeights[i];
					}
					Rectangle rect = new Rectangle(headerLeftOffsets[node.Column], levelTop, headerWidths[node.Column], height);
					PrintInfo.UpdateHeaderRect(node.Column, rect);
				}
				levelTop += levelHeights[i];
			}
		}
		Size GetHeaderCellSize(int cellWidth, GridViewColumn column) {
			var text = Printer.GetHeaderText(column);
			if(string.IsNullOrEmpty(text))
				text = EmptyHeaderCellText;
			return GetTextSize(text, StyleHelper.GetHeaderStyle(Printer.Graph, HorizontalAlign.Left), cellWidth);
		}
		Dictionary<GridViewColumn, int> CalculateHeaderWidths() {
			var widths = new Dictionary<GridViewColumn, int>();
			PopulateMaxColumnWidth(widths);
			CompensateColumnWidths(widths);
			return widths;
		}
		void PopulateMaxColumnWidth(Dictionary<GridViewColumn, int> widths) {
			for(int i = ColumnHelper.Layout.Count - 1; i >= 0; i--)
				foreach(GridViewColumnVisualTreeNode node in ColumnHelper.Layout[i])
					widths[node.Column] = CalculateMaxColumnWidth(node, widths);
		}
		int CalculateMaxColumnWidth(GridViewColumnVisualTreeNode node, Dictionary<GridViewColumn, int> widths) {
			var column = node.Column;
			var textWidth = GetTextWidth(Printer.GetHeaderText(column), StyleHelper.GetHeaderStyle(Printer.Graph, HorizontalAlign.Left), GetMaxColumnWidth(column));
			var result = GetChildLevelWidth(node, widths);
			if(ColumnHelper.IsLeaf(node.Column) || CanCompensateColumnWidth(node))
				result = Math.Max(result, textWidth);
			if(!ColumnHelper.IsLeaf(column))
				return result;
			return Math.Max(result, PrintInfo.GetColumnWidth(column));
		}
		void CompensateColumnWidths(Dictionary<GridViewColumn, int> widths) {
			foreach(var level in ColumnHelper.Layout) {
				foreach(var node in level) {
					if(node.Children.Count == 0) continue;
					var delta = widths[node.Column] - GetChildLevelWidth(node, widths);
					if(delta > 0)
						CompensateColumnWidthsCore(node.ChildrenEx, widths, delta);
				}
			}
		}
		void CompensateColumnWidthsCore(List<GridViewColumnVisualTreeNode> nodes, Dictionary<GridViewColumn, int> widths, int delta) {
			var columns = nodes.Where(n => CanCompensateColumnWidth(n)).Select(n => n.Column).ToList();
			if(columns.Count == 0)
				return;
			var excess = delta;
			var totalWidth = columns.Sum(c => widths[c]);
			foreach(var column in columns) {
				if(excess <= 0) break;
				int k = Convert.ToInt32(Math.Ceiling((float)delta * widths[column] / totalWidth));
				excess -= k;
				if(excess < 0)
					k += excess;
				widths[column] += k;
			}
		}
		int GetChildLevelWidth(GridViewColumnVisualTreeNode node, Dictionary<GridViewColumn, int> columnsWidth) {
			if(ColumnHelper.IsLeaf(node.Column)) return 0;
			return node.ChildrenEx.Sum(n => columnsWidth[n.Column]);
		}
		bool CanCompensateColumnWidth(GridViewColumnVisualTreeNode node) {
			var column = node.Column;
			if(column.ExportWidth > 0) return false;
			var result = true;
			if(!ColumnHelper.IsLeaf(column))
				result = node.ChildrenEx.Exists(n => CanCompensateColumnWidth(n));
			return result;
		}
		int[,] CreateMatrix() {
			int[,] result = new int[ColumnHelper.Layout.Count, ColumnHelper.Leafs.Count];
			for(int y = 0; y < ColumnHelper.Layout.Count; y++) {
				for(int x = 0; x < ColumnHelper.Leafs.Count; x++)
					result[y, x] = -1;
			}
			return result;
		}
		void AddColumnToMatrix(GridViewColumnVisualTreeNode node, int rowIndex, int cellIndex) {
			int columnIndex = ColumnHelper.GetColumnGlobalIndex(node.Column);
			for(int y = 0; y < rowIndex + node.RowSpan; y++) {
				for(int x = cellIndex; x < cellIndex + node.ColSpan; x++) {
					Matrix[y, x] = x == cellIndex ? columnIndex : -2;
				}
			}
		}
		int FindFreeCellIndex(int rowIndex, int lastFreeCellIndex) {
			for(int x = lastFreeCellIndex; x < ColumnHelper.Leafs.Count; x++) {
				if(Matrix[rowIndex, x] == -1)
					return x;
			}
			return 0;
		}
		int[] FindLeftColumnIndices(int rowIndex, int cellIndex) {
			List<int> list = new List<int>();
			for(int x = 0; x < cellIndex; x++) {
				int columnIndex = Matrix[rowIndex, x];
				if(columnIndex >= 0)
					list.Add(columnIndex);
			}
			return list.ToArray();
		}
		int GetGroupHeight(int rowIndex) {
			BrickStyle brickStyle = StyleHelper.GetGroupRowStyle(Printer.Graph);  
			int width = Printer.GetGroupRowWidth(rowIndex) - brickStyle.Padding.Left - brickStyle.Padding.Right;
			return GetTextSize(Printer.GetGroupRowText(rowIndex), brickStyle, width).Height;
		}
		Size GetColumnSize(GridViewColumn column, string text, BrickStyle style) {
			GridViewDataColumn dataColumn = column as GridViewDataColumn;
			IImageExportSettings imageExportSettings = GridViewPrinter.GetImageExportSettings(dataColumn);
			if(imageExportSettings != null) {
				var useColumnWidth = dataColumn.ExportWidth != 0;
				var width = useColumnWidth ? dataColumn.ExportWidth : imageExportSettings.Width;
				return StyleHelper.CalcImageSize(width, imageExportSettings.Height, Printer.Graph, style, useColumnWidth);
			}
			return GetTextSize(text, style, GetMaxColumnWidth(column));
		}
		int GetTextWidth(string text, BrickStyle style, int maxWidth) {
			return GetTextSize(text, style, maxWidth).Width;
		}
		Size GetTextSize(string text, BrickStyle style, int maxWidth) {
			return StyleHelper.CalcTextSize(Printer.Graph, text, style, maxWidth);
		}
		int GetMaxColumnWidth(GridViewColumn column) {
			return IsExportWidthAssigned(column) ? column.ExportWidth : Exporter.MaxColumnWidth;
		}
		int GetMinColumnWidth(GridViewColumn column) {
			return IsExportWidthAssigned(column) ? column.ExportWidth : ASPxGridViewExporter.MinColumnWidth;
		}
		bool IsExportWidthAssigned(GridViewColumn column) {
			return column.ExportWidth > 0;
		}
	}
}
