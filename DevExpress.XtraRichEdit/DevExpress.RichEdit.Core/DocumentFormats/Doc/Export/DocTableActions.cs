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
using System.IO;
using System.Reflection;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Import.Doc;
using DevExpress.XtraRichEdit.Tables.Native;
using DevExpress.Utils;
using DevExpress.Office;
using LayoutUnit = System.Int32;
using ModelUnit = System.Int32;
namespace DevExpress.XtraRichEdit.Export.Doc {
	#region DocTableActions
	public class DocTableActions : IDisposable {
		#region Fields
		BinaryWriter writer;
		MergedTableProperties properties;
		TableLayoutType tableLayout;
		WidthUnit cellSpacing;
		DocumentModelUnitConverter unitConverter;
		#endregion
		DocTableActions(MemoryStream output) {
			Guard.ArgumentNotNull(output, "output");
			this.writer = new BinaryWriter(output);
			this.tableLayout = TableLayoutType.Autofit;
		}
		public DocTableActions(MemoryStream output, TableStyle tableStyle)
			: this(output) {
			this.properties = tableStyle.GetMergedTableProperties();
			this.unitConverter = tableStyle.DocumentModel.UnitConverter;
		}
		public DocTableActions(MemoryStream output, Table table)
			: this(output) {
			this.properties = table.GetMergedWithStyleTableProperties();
			this.tableLayout = table.TableLayout;
			this.cellSpacing = table.CellSpacing;
			this.unitConverter = table.DocumentModel.UnitConverter;
		}
		#region Properties
		protected MergedTableProperties TableProperties { get { return properties; } }
		TableFloatingPositionInfo FloatingPosition { get { return properties.Info.FloatingPosition; } }
		protected TableLayoutType TableLayout { get { return tableLayout; } }
		protected WidthUnit CellSpacing { get { return cellSpacing; } }
		protected DocumentModelUnitConverter UnitConverter { get { return unitConverter; } }
		#endregion
		public void CreateTablePropertyModifiers() {
			PreferredTableWidthAction();
			AutofitAction();
			BordersAction();
			MarginsAction();
			TableAlignmentAction();
			TableIndentAction();
			CellSpacingAction();
			BackgroundColorAction();
			AllowOverlapAction();
			FloatingPositionAction();
		}
		void PreferredTableWidthAction() {
			if (!TableProperties.Options.UsePreferredWidth)
				return;
			DocCommandPreferredTableWidth command = new DocCommandPreferredTableWidth();
			command.WidthUnit.ConvertFromWidthUnitInfo(TableProperties.Info.PreferredWidth, UnitConverter);
			command.Write(this.writer);
		}
		void AutofitAction() {
			DocCommandTableAutoFit command = new DocCommandTableAutoFit();
			command.Value = (TableLayout == TableLayoutType.Autofit);
			command.Write(this.writer);
		}
		void BordersAction() {
			if (!TableProperties.Options.UseBorders)
				return;
			CombinedTableBordersInfo borders = TableProperties.Info.Borders;
			DocCommandTableBorders command = new DocCommandTableBorders();
			command.TableBorders.BottomBorder.ConvertFromBorderInfo(borders.BottomBorder, unitConverter);
			command.TableBorders.InsideHorizontalBorder.ConvertFromBorderInfo(borders.InsideHorizontalBorder, unitConverter);
			command.TableBorders.InsideVerticalBorder.ConvertFromBorderInfo(borders.InsideVerticalBorder, unitConverter);
			command.TableBorders.LeftBorder.ConvertFromBorderInfo(borders.LeftBorder, unitConverter);
			command.TableBorders.RightBorder.ConvertFromBorderInfo(borders.RightBorder, unitConverter);
			command.TableBorders.TopBorder.ConvertFromBorderInfo(borders.TopBorder, unitConverter);
			command.Write(this.writer);
		}
		void MarginsAction() {
			BottomMarginAction();
			LeftMarginAction();
			RightMarginAction();
			TopMarginAction();
		}
		void BottomMarginAction() {
			if (!TableProperties.Options.UseBottomMargin)
				return;
			DocCommandCellMarginDefault command = new DocCommandCellMarginDefault();
			command.CellSpacing.CellBorders = DocTableCellBorders.Bottom;
			command.CellSpacing.WidthUnit.ConvertFromWidthUnitInfo(TableProperties.Info.CellMargins.Bottom, UnitConverter);
			command.Write(this.writer);
		}
		void LeftMarginAction() {
			if (!TableProperties.Options.UseLeftMargin)
				return;
			DocCommandCellMarginDefault command = new DocCommandCellMarginDefault();
			command.CellSpacing.CellBorders = DocTableCellBorders.Left;
			command.CellSpacing.WidthUnit.ConvertFromWidthUnitInfo(TableProperties.Info.CellMargins.Left, UnitConverter);
			command.Write(this.writer);
		}
		void RightMarginAction() {
			if (!TableProperties.Options.UseRightMargin)
				return;
			DocCommandCellMarginDefault command = new DocCommandCellMarginDefault();
			command.CellSpacing.CellBorders = DocTableCellBorders.Right;
			command.CellSpacing.WidthUnit.ConvertFromWidthUnitInfo(TableProperties.Info.CellMargins.Right, UnitConverter);
			command.Write(this.writer);
		}
		void TopMarginAction() {
			if (!TableProperties.Options.UseTopMargin)
				return;
			DocCommandCellMarginDefault command = new DocCommandCellMarginDefault();
			command.CellSpacing.CellBorders = DocTableCellBorders.Top;
			command.CellSpacing.WidthUnit.ConvertFromWidthUnitInfo(TableProperties.Info.CellMargins.Top, UnitConverter);
			command.Write(this.writer);
		}
		void TableIndentAction() {
			if (!TableProperties.Options.UseTableIndent)
				return;
			DocCommandWidthIndent command = new DocCommandWidthIndent();
			command.WidthUnit.ConvertFromWidthUnitInfo(TableProperties.Info.TableIndent, UnitConverter);
			command.Write(this.writer);
		}
		void TableAlignmentAction() {
			if (!TableProperties.Options.UseTableAlignment)
				return;
			DocCommandTableAlignment command = new DocCommandTableAlignment();
			command.TableAlignment = TableProperties.Info.GeneralSettings.TableAlignment;
			command.Write(this.writer);
		}
		void CellSpacingAction() {
			if (!TableProperties.Options.UseCellSpacing || CellSpacing == null)
				return;
			DocCommandCellSpacing command = new DocCommandCellSpacing();
			command.CellSpacing.CellBorders = DocTableCellBorders.All;
			command.CellSpacing.WidthUnit.ConvertFromWidthUnitInfo(CellSpacing.Info, UnitConverter);
			command.Write(this.writer);
		}
		void BackgroundColorAction() {
			if (!TableProperties.Options.UseBackgroundColor)
				return;
			DocCommandTableBackgroundColor command = new DocCommandTableBackgroundColor();
			command.ShadingDescriptor.BackgroundColor = TableProperties.Info.GeneralSettings.BackgroundColor;
			command.Write(this.writer);
		}
		void AllowOverlapAction() {
			if (!TableProperties.Options.UseIsTableOverlap)
				return;
			DocCommandTableOverlap command = new DocCommandTableOverlap();
			command.Value = !TableProperties.Info.GeneralSettings.IsTableOverlap;
			command.Write(this.writer);
		}
		void FloatingPositionAction() {
			if (!TableProperties.Options.UseFloatingPosition)
				return;
			if (FloatingPosition.IsHorizontalAbsolutePositionUse && FloatingPosition.IsVerticalAbsolutePositionUse)
				return;
			BottomFromTextAction();
			LeftFromTextAction();
			RightFromTextAction();
			TopFromTextAction();
			HorizontalAlignAction();
			VerticalAlignAction();
			TableStyleRowBandSizeAction();
			TableStyleColBandSizeAction();
			TableAnchorsAction();
		}
		void BottomFromTextAction() {
			DocCommandBottomFromText command = new DocCommandBottomFromText();
			command.Value = UnitConverter.ModelUnitsToTwips(FloatingPosition.BottomFromText);
			command.Write(this.writer);
		}
		void LeftFromTextAction() {
			DocCommandLeftFromText command = new DocCommandLeftFromText();
			command.Value = UnitConverter.ModelUnitsToTwips(FloatingPosition.LeftFromText);
			command.Write(this.writer);
		}
		void RightFromTextAction() {
			DocCommandRightFromText command = new DocCommandRightFromText();
			command.Value = UnitConverter.ModelUnitsToTwips(FloatingPosition.RightFromText);
			command.Write(this.writer);
		}
		void TopFromTextAction() {
			DocCommandTopFromText command = new DocCommandTopFromText();
			command.Value = UnitConverter.ModelUnitsToTwips(FloatingPosition.TopFromText);
			command.Write(this.writer);
		}
		void HorizontalAlignAction() {
			DocCommandTableHorizontalPosition command = new DocCommandTableHorizontalPosition();
			command.Value = CalcTableHorizontalPositionTypeCode();
			command.Write(this.writer);
		}
		int CalcTableHorizontalPositionTypeCode() {
			switch (FloatingPosition.HorizontalAlign) {
				case HorizontalAlignMode.Center: return DocCommandTableHorizontalPosition.Centered;
				case HorizontalAlignMode.Inside: return DocCommandTableHorizontalPosition.Inside;
				case HorizontalAlignMode.Left: return DocCommandTableHorizontalPosition.LeftAligned;
				case HorizontalAlignMode.Outside: return DocCommandTableHorizontalPosition.Outside;
				case HorizontalAlignMode.Right: return DocCommandTableHorizontalPosition.RightAligned;
				default: return UnitConverter.ModelUnitsToTwips(FloatingPosition.TableHorizontalPosition);
			}
		}
		void VerticalAlignAction() {
			DocCommandTableVerticalPosition command = new DocCommandTableVerticalPosition();
			command.Value = CalcTableVerticalPositionTypeCode();
			command.Write(this.writer);
		}
		int CalcTableVerticalPositionTypeCode() {
			switch (FloatingPosition.VerticalAlign) {
				case VerticalAlignMode.Bottom: return DocCommandTableVerticalPosition.Bottom;
				case VerticalAlignMode.Center: return DocCommandTableVerticalPosition.Center;
				case VerticalAlignMode.Inline: return DocCommandTableVerticalPosition.Inline;
				case VerticalAlignMode.Inside: return DocCommandTableVerticalPosition.Inside;
				case VerticalAlignMode.Outside: return DocCommandTableVerticalPosition.Outside;
				case VerticalAlignMode.Top: return DocCommandTableVerticalPosition.Top;
				default: return UnitConverter.ModelUnitsToTwips(FloatingPosition.TableVerticalPosition);
			}
		}
		void TableStyleRowBandSizeAction() {
			if (!TableProperties.Options.UseTableStyleRowBandSize)
				return;
			DocCommandTableStyleRowBandSize command = new DocCommandTableStyleRowBandSize();
			command.Value = (byte)TableProperties.Info.GeneralSettings.TableStyleRowBandSize;
			command.Write(this.writer);
		}
		void TableStyleColBandSizeAction() {
			if (!TableProperties.Options.UseTableStyleColBandSize)
				return;
			DocCommandTableStyleColBandSize command = new DocCommandTableStyleColBandSize();
			command.Value = (byte)TableProperties.Info.GeneralSettings.TableStyleColBandSize;
			command.Write(this.writer);
		}
		void TableAnchorsAction() {
			DocCommandTablePosition command = new DocCommandTablePosition(TableProperties.Info.FloatingPosition);
			command.Write(this.writer);
		}
		#region IDisposable Members
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				IDisposable resource = this.writer as IDisposable;
				if (resource != null) {
					resource.Dispose();
					resource = null;
				}
			}
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
	}
	#endregion
	#region DocTableRowActions
	public class DocTableRowActions : IDisposable {
		#region Fields
		const int firstCellsGroupCount = 22;
		const int secondCellsGroupCount = 44;
		const int thirdCellsGroupCount = 64;
		BinaryWriter writer;
		TableRow row;
		DocumentModelUnitConverter unitConverter;
		DocumentModelUnitToLayoutUnitConverter modelUnitToTwipsUnitConverter;
		TableGrid grid;
		#endregion
		public DocTableRowActions(MemoryStream output, TableRow row, TableGrid grid) {
			this.writer = new BinaryWriter(output);
			this.row = row;
			unitConverter = row.DocumentModel.UnitConverter;
			modelUnitToTwipsUnitConverter = unitConverter.CreateConverterToLayoutUnits(DevExpress.Office.DocumentLayoutUnit.Twip);
			this.grid = grid;
		}
		#region Properties
		protected TableRow Row { get { return this.row; } }
		protected DocumentModelUnitConverter UnitConverter { get { return unitConverter; } }
		protected DocumentModelUnitToLayoutUnitConverter ToLayoutUnitConverter { get { return modelUnitToTwipsUnitConverter; } }
		protected TableGrid Grid { get { return grid; } }
		#endregion
		public void CreateTableRowPropertyModifiers(int tableDepth, int tableStyleIndex) {
			TableDepthAction(tableDepth);
			DxaLeftAction();
			InsertAction();
			TableStyleAction(tableStyleIndex);
			PreferredCellWidthsAction();
			CellBordersAction();
			CellMarginsAction();
			CellsBackgroundColorAction();
			CellsVerticalAlignmentAction();
			VerticalMergingAction();
			HideCellMarkAction();
			HeightAction();
			WidthBeforeAction();
			WidthAfterAction();
			DxaGapHalfAction();
		}
		void TableDepthAction(int tableDepth) {
			DocCommandInTable inTableCommand = new DocCommandInTable();
			inTableCommand.Value = true;
			inTableCommand.Write(this.writer);
			if (tableDepth > 1)
				InnerTableRowAction();
			else
				TableTrailerAction();
			DocCommandTableDepth tableDepthCommand = new DocCommandTableDepth();
			tableDepthCommand.Value = tableDepth;
			tableDepthCommand.Write(writer);
		}
		void InnerTableRowAction() {
			DocCommandInnerTableCell cellCommand = new DocCommandInnerTableCell();
			cellCommand.Value = true;
			cellCommand.Write(this.writer);
			DocCommandInnerTableTrailer rowCommand = new DocCommandInnerTableTrailer();
			rowCommand.Value = true;
			rowCommand.Write(this.writer);
		}
		void TableTrailerAction() {
			DocCommandTableTrailer tableTrailerCommand = new DocCommandTableTrailer();
			tableTrailerCommand.Value = true;
			tableTrailerCommand.Write(this.writer);
		}
		void TableStyleAction(int tableStyleIndex) {
			DocCommandChangeTableStyle command = new DocCommandChangeTableStyle();
			command.Value = tableStyleIndex;
			command.Write(this.writer);
		}
		void InsertAction() {
			int count = Row.Cells.Count;
			int currentColumnIndex = Row.GridBefore;
			for (int cellIndex = 0; cellIndex < count; cellIndex++) {
				TableCell cell = Row.Cells[cellIndex];
				short cellWidth = GetCellWidth(currentColumnIndex, cell);
				CreateInsertCommand(cellIndex, cellWidth);
				currentColumnIndex += cell.ColumnSpan;
			}
		}
		void CreateInsertCommand(int cellIndex, short totalWidth) {
			DocCommandInsertTableCell insertCommand = new DocCommandInsertTableCell();
			insertCommand.Insert.StartIndex = (byte)cellIndex;
			insertCommand.Insert.Count = 1;
			insertCommand.Insert.WidthInTwips = totalWidth;
			insertCommand.Write(this.writer);
		}
		void PreferredCellWidthsAction() {
			int count = Row.Cells.Count;
			for (int i = 0; i < count; i++)
				CreatePreferredCellWidthCommand(i, Row.Cells[i]);
		}
		void CreatePreferredCellWidthCommand(int i, TableCell cell) {
			DocCommandPreferredTableCellWidth command = new DocCommandPreferredTableCellWidth();
			command.TableCellWidth.StartIndex = (byte)i;
			command.TableCellWidth.EndIndex = (byte)i;
			command.TableCellWidth.WidthUnit.ConvertFromWidthUnitInfo(cell.PreferredWidth.Info, UnitConverter);
			command.Write(writer);
		}
		void CellBordersAction() {
			int count = Row.Cells.Count;
			for (int i = 0; i < count; i++)
				CreateCellBordersCommand(Row.Cells[i]);
		}
		void CreateCellBordersCommand(TableCell cell) {
			TableCellBorders borders = cell.Properties.Borders;
			if (borders.UseBottomBorder)
				CreateCellBorderCommandCore(borders.BottomBorder.Info, DocTableCellBorders.Bottom, cell.IndexInRow);
			if (borders.UseLeftBorder)
				CreateCellBorderCommandCore(borders.LeftBorder.Info, DocTableCellBorders.Left, cell.IndexInRow);
			if (borders.UseRightBorder)
				CreateCellBorderCommandCore(borders.RightBorder.Info, DocTableCellBorders.Right, cell.IndexInRow);
			if (borders.UseTopBorder)
				CreateCellBorderCommandCore(borders.TopBorder.Info, DocTableCellBorders.Top, cell.IndexInRow);
		}
		void CreateCellBorderCommandCore(BorderInfo info, DocTableCellBorders cellBorderType, int index) {
			DocCommandOverrideCellBorders command = new DocCommandOverrideCellBorders();
			command.OverriddenBorders.StartIndex = (byte)index;
			command.OverriddenBorders.EndIndex = (byte)index;
			command.OverriddenBorders.Border.ConvertFromBorderInfo(info, UnitConverter);
			command.OverriddenBorders.CellBorders = cellBorderType;
			command.Write(this.writer);
		}
		void CellMarginsAction() {
			int count = Row.Cells.Count;
			for (int i = 0; i < count; i++)
				CreateCellMarginsCommands(Row.Cells[i]);
		}
		void CreateCellMarginsCommands(TableCell cell) {
			if (cell.Properties.UseBottomMargin)
				CreateCellMarginCommandCore(cell.Properties.CellMargins.Bottom.Info, DocTableCellBorders.Bottom, cell.IndexInRow);
			if (cell.Properties.UseLeftMargin)
				CreateCellMarginCommandCore(cell.Properties.CellMargins.Left.Info, DocTableCellBorders.Left, cell.IndexInRow);
			if (cell.Properties.UseRightMargin)
				CreateCellMarginCommandCore(cell.Properties.CellMargins.Right.Info, DocTableCellBorders.Right, cell.IndexInRow);
			if (cell.Properties.UseTopMargin)
				CreateCellMarginCommandCore(cell.Properties.CellMargins.Top.Info, DocTableCellBorders.Top, cell.IndexInRow);
		}
		void CreateCellMarginCommandCore(WidthUnitInfo info, DocTableCellBorders cellBorderType, int index) {
			DocCommandCellMargin command = new DocCommandCellMargin();
			command.CellSpacing.CellBorders = cellBorderType;
			command.CellSpacing.StartIndex = index;
			command.CellSpacing.EndIndex = index;
			command.CellSpacing.WidthUnit.ConvertFromWidthUnitInfo(info, UnitConverter);
			command.Write(this.writer);
		}
		void CellsBackgroundColorAction() {
			DefineTableShadingsActionCore(0, firstCellsGroupCount, typeof(DocCommandDefineTableShadings));
			DefineTableShadingsActionCore(firstCellsGroupCount, secondCellsGroupCount, typeof(DocCommandDefineTableShadings2nd));
			DefineTableShadingsActionCore(secondCellsGroupCount, thirdCellsGroupCount, typeof(DocCommandDefineTableShadings3rd));
		}
		void DefineTableShadingsActionCore(int startGroupIndex, int endGroupIndex, Type commandType) {
			if (Row.Cells.Count < startGroupIndex)
				return;
			ConstructorInfo commandConstructor = commandType.GetConstructor(new Type[] { });
			DocCommandShadingListBase command = commandConstructor.Invoke(new object[] { })
					as DocCommandShadingListBase;
			int count = Math.Min(Row.Cells.Count, endGroupIndex);
			for (int i = startGroupIndex; i < count; i++) {
				if (Row.Cells[i].Properties.UseBackgroundColor)
					command.CellColors.Add(Row.Cells[i].Properties.BackgroundColor);
				else
					command.CellColors.Add(DXColor.Empty);
			}
			command.Write(this.writer);
		}
		void CellsVerticalAlignmentAction() {
			int count = Row.Cells.Count;
			for (int i = 0; i < count; i++) {
				CreateCellVertacalAlignmentCommand(Row.Cells[i]);
			}
		}
		void CreateCellVertacalAlignmentCommand(TableCell cell) {
			if (!cell.Properties.UseVerticalAlignment)
				return;
			DocCommandCellRangeVerticalAlignment command = new DocCommandCellRangeVerticalAlignment();
			command.CellRangeVerticalAlignment.StartIndex = (byte)cell.IndexInRow;
			command.CellRangeVerticalAlignment.EndIndex = (byte)cell.IndexInRow;
			command.CellRangeVerticalAlignment.VerticalAlignment = cell.VerticalAlignment;
			command.Write(this.writer);
		}
		void VerticalMergingAction() {
			int count = Row.Cells.Count;
			for (int i = 0; i < count; i++) {
				TableCell currentCell = Row.Cells[i];
				CreateVerticalMergeCommand(currentCell);
			}
		}
		void CreateVerticalMergeCommand(TableCell cell) {
			if (cell.VerticalMerging == MergingState.None)
				return;
			DocCommandVerticalMergeTableCells command = new DocCommandVerticalMergeTableCells();
			command.CellIndex = (byte)cell.IndexInRow;
			command.VerticalMerging = cell.VerticalMerging;
			command.Write(this.writer);
		}
		void HideCellMarkAction() {
			int count = Row.Cells.Count;
			for (int i = 0; i < count; i++) {
				TableCell currentCell = Row.Cells[i];
				CreateHideCellMarkCommand(currentCell);
			}
		}
		void CreateHideCellMarkCommand(TableCell cell) {
			if (!cell.Properties.HideCellMark)
				return;
			DocCommandHideCellMark command = new DocCommandHideCellMark();
			command.CellHideMark.StartIndex = (byte)cell.IndexInRow;
			command.CellHideMark.EndIndex = (byte)cell.IndexInRow;
			command.CellHideMark.HideCellMark = true;
			command.Write(this.writer);
		}
		void HeightAction() {
			if (!row.Properties.UseHeight)
				return;
			DocCommandTableRowHeight command = new DocCommandTableRowHeight();
			command.Type = Row.Properties.Height.Type;
			command.Value = Row.Properties.Height.Value;
			command.Write(this.writer);
		}
		void WidthBeforeAction() {
			if (!Row.Properties.UseWidthBefore)
				return;
			DocCommandWidthBefore command = new DocCommandWidthBefore();
			command.WidthUnit.Type = WidthUnitType.ModelUnits;
			command.WidthUnit.Value = GetWidthBefore();
			command.Write(this.writer);
		}
		void WidthAfterAction() {
			if (!Row.Properties.UseWidthAfter)
				return;
			DocCommandWidthAfter command = new DocCommandWidthAfter();
			command.WidthUnit.Type = WidthUnitType.ModelUnits;
			command.WidthUnit.Value = GetWidthAfter();
			command.Write(this.writer);
		}
		void DxaLeftAction() {
			if (!Row.Properties.UseWidthBefore && !Row.Table.TableProperties.UseTableIndent && !Row.Table.TableProperties.UseCellSpacing)
				return;
			int dxaLeft = GetTableIndent() + GetWidthBefore() - GetCellSpacing();
			DocCommandTableDxaLeft command = new DocCommandTableDxaLeft();
			command.Value = dxaLeft;
			command.Write(this.writer);
		}
		void DxaGapHalfAction() {
			if (!Row.Table.TableProperties.UseLeftMargin && !Row.Table.TableProperties.UseRightMargin)
				return;
			DocCommandTableDxaGapHalf command = new DocCommandTableDxaGapHalf();
			command.Value = GetGapHalf();
			command.Write(this.writer);
		}
		int GetWidthBefore() {
			int totalWidth = 0;
			int count = Row.GridBefore;
			for (int i = 0; i < count; i++) {
				totalWidth += Grid.Columns[i].Width;
			}
			return ConvertLayoutUnitsToTwips(totalWidth);
		}
		short GetCellWidth(int firstColumnIndex, TableCell cell) {
			int totalWidth = 0;
			int lastColumnIndex = firstColumnIndex + cell.ColumnSpan;
			for (int columnIndex = firstColumnIndex; columnIndex < lastColumnIndex; columnIndex++)
				totalWidth += Grid.Columns[columnIndex].Width;
			return (short)Math.Min(ConvertLayoutUnitsToTwips(totalWidth), DocConstants.MaxXASValue);
		}
		int GetWidthAfter() {
			int totalWidth = 0;
			int count = Row.GridAfter;
			for (int i = 0; i < count; i++) {
				totalWidth += Grid.Columns[Grid.Columns.Count - i - 1].Width;
			}
			return ConvertLayoutUnitsToTwips(totalWidth);
		}
		int GetTableIndent() {
			if (Row.Table.TableProperties.TableIndent.Type == WidthUnitType.ModelUnits)
				return UnitConverter.ModelUnitsToTwips(Row.Table.TableProperties.TableIndent.Value);
			else
				return 0;
		}
		int GetCellSpacing() {
			if (Row.Table.TableProperties.CellSpacing.Type == WidthUnitType.ModelUnits)
				return UnitConverter.ModelUnitsToTwips(Row.Table.TableProperties.CellSpacing.Value);
			else
				return 0;
		}
		int GetGapHalf() {
			int result = 0;
			Table table = Row.Table;
			TableProperties properties = table.TableProperties;
			if (properties.UseLeftMargin && table.LeftMargin.Type == WidthUnitType.ModelUnits)
				result += UnitConverter.ModelUnitsToTwips(table.LeftMargin.Value);
			if (properties.UseRightMargin && table.RightMargin.Type == WidthUnitType.ModelUnits)
				result += UnitConverter.ModelUnitsToTwips(table.RightMargin.Value);
			return result / 2;
		}
		int ConvertLayoutUnitsToTwips(int totalWidth) {
			totalWidth = ToLayoutUnitConverter.ToModelUnits(totalWidth);
			totalWidth = UnitConverter.ModelUnitsToTwips(totalWidth);
			return totalWidth;
		}
		#region IDisposable Members
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				IDisposable resource = this.writer as IDisposable;
				if (resource != null) {
					resource.Dispose();
					resource = null;
				}
			}
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
	}
	#endregion
	#region DocTableCellActions
	public class DocTableCellActions : IDisposable {
		#region Fields
		BinaryWriter writer;
		TableCell cell;
		MergedTableProperties tableProperties;
		#endregion
		public DocTableCellActions(MemoryStream output, TableCell cell) {
			Guard.ArgumentNotNull(cell, "cell");
			this.writer = new BinaryWriter(output);
			this.cell = cell;
		}
		#region Properties
		protected TableCell Cell { get { return this.cell; } }
		protected MergedTableProperties TableProperties {
			get {
				if (tableProperties == null)
					tableProperties = cell.Table.GetMergedWithStyleTableProperties();
				return tableProperties;
			}
		}
		#endregion
		public void CreateTableCellPropertyModifiers(int tableDepth, bool finalParagraphInCell) {
			InTableAction();
			TableDepthAction(tableDepth, finalParagraphInCell);
			TableAnchorsAction();
		}
		void InTableAction() {
			DocCommandInTable inTableCommand = new DocCommandInTable();
			inTableCommand.Value = true;
			inTableCommand.Write(this.writer);
		}
		void TableDepthAction(int tableDepth, bool finalParagraphInCell) {
			DocCommandTableDepth tableDepthCommand = new DocCommandTableDepth();
			tableDepthCommand.Value = tableDepth;
			tableDepthCommand.Write(writer);
			if (tableDepth > 1 && finalParagraphInCell)
				InnerTableCellAction();
		}
		void InnerTableCellAction() {
			DocCommandInnerTableCell command = new DocCommandInnerTableCell();
			command.Value = true;
			command.Write(this.writer);
		}
		void TableAnchorsAction() {
			if (!TableProperties.Options.UseFloatingPosition)
				return;
			TableFloatingPositionInfo info = TableProperties.Info.FloatingPosition;
			if (info.IsHorizontalAbsolutePositionUse && info.IsVerticalAbsolutePositionUse)
				return;
			DocCommandTablePosition command = new DocCommandTablePosition(info);
			command.Write(this.writer);
		}
		#region IDisposable Members
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				IDisposable resource = this.writer as IDisposable;
				if (resource != null) {
					resource.Dispose();
					resource = null;
				}
			}
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
	}
	#endregion
	#region DocTableWidthsCalculator
	public class DocTableWidthsCalculator : TableWidthsCalculatorBase {
		const int defaultPreferredWidthInTwips = 168;
		internal const int DefaultPercentBaseWidthInTwips = 5 * 1440;
		public DocTableWidthsCalculator(DocumentModelUnitToLayoutUnitConverter converter)
			: base(converter, converter.ToLayoutUnits(DefaultPercentBaseWidthInTwips)) {
		}
		protected override WidthsContentInfo CalculateCellContentWidthsCore(TableCell cell, int percentBaseWidth, bool smpleView) {
			LayoutUnit preferredWidth = 0;
			if (cell.PreferredWidth.Type == WidthUnitType.ModelUnits) {
				preferredWidth = Converter.ToLayoutUnits(cell.PreferredWidth.Value);
			}
			if (preferredWidth == 0) {
				preferredWidth = Converter.ToLayoutUnits(defaultPreferredWidthInTwips) * cell.ColumnSpan;
			}
			return new WidthsContentInfo(preferredWidth, preferredWidth);
		}
		public override bool CanUseCachedTableLayoutInfo(TableLayoutInfo tableLayoutInfo) {
			return false;
		}
		public override TableLayoutInfo CreateTableLayoutInfo(TableGrid tableGrid, ModelUnit maxTableWidth, bool allowTablesToExtendIntoMargins, bool simpleView, ModelUnit percentBaseWidth) {
			return null;
		}
	}
	#endregion
}
