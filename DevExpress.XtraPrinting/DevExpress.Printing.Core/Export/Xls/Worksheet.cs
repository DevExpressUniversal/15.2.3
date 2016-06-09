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
using System.Globalization;
using System.IO;
using System.Text;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.Export.Xl;
using System.Drawing.Printing;
using DevExpress.Compatibility.System.Drawing.Printing;
namespace DevExpress.XtraExport.Xls {
	using DevExpress.XtraExport.Implementation;
	#region XlsDataAwareExporter
	public partial class XlsDataAwareExporter {
		readonly List<XlsTableBasedDocumentSheet> sheets = new List<XlsTableBasedDocumentSheet>();
		XlsTableBasedDocumentSheet currentSheet = null;
		int maxRowOutlineLevel = 1;
		bool firstVisibleSheet = true;
		long indexRecordPosition;
		long defColWidthRecordPosition;
		long cellTablePosition;
		bool rowContentStarted = false;
		public IXlSheet BeginSheet() {
			rowCount = 0;
			currentRowIndex = 0;
			currentColumnIndex = 0;
			dbCellCalculator.Reset();
			sharedFormulaHostCells.Clear();
			shapeId = 0;
			currentSheet = new XlsTableBasedDocumentSheet(this);
			currentSheet.Name = String.Format("Sheet{0}", sheets.Count + 1);
			this.rowContentStarted = false;
			return currentSheet;
		}
		public void EndSheet() {
			if(currentSheet == null)
				throw new InvalidOperationException("BeginSheet/EndSheet calls consistency.");
			if(!IsUniqueSheetName())
				throw new InvalidOperationException(string.Format("Worksheet name '{0}' is not unique.", currentSheet.Name));
			WritePendingRowContent();
			sheets.Add(currentSheet);
			currentSheet.SheetId = sheets.Count;
			RegisterSheetDefinitions();
			RegisterMergeCells();
			currentSheet = null;
			this.writer = null;
		}
		protected void WriteWorksheets() {
			for(int i = 0; i < sheets.Count; i++) {
				currentSheet = sheets[i];
				WriteBoundSheetStartPosition();
				WriteWorksheet();
			}
			currentSheet = null;
		}
		protected void WriteWorksheet() {
			WriteBOF(XlsSubstreamType.Sheet);
			WriteIndex();
			WriteWorksheetGlobals();
			WriteWorksheetPageSetup();
			WriteWorksheetColumns();
			WriteAutoFilter();
			WriteDimensions();
			WriteCellTable();
			WriteDrawingObjects();
			WriteWorksheetWindow();
			WriteMergeCells();
			WriteConditionalFormattings();
			WriteHyperlinks();
			WriteDataValidations();
			WriteFeatures();
			RewriteIndex();
			WriteEOF();
		}
		void ClearSheets() {
			foreach(XlsTableBasedDocumentSheet sheet in sheets)
				sheet.Dispose();
			sheets.Clear();
		}
		void WriteBoundSheetStartPosition() {
			long boundPos = boundPositions.Dequeue();
			long currentPos = writer.BaseStream.Position;
			writer.BaseStream.Seek(boundPos, SeekOrigin.Begin);
			writer.Write((uint)currentPos);
			writer.BaseStream.Seek(currentPos, SeekOrigin.Begin);
		}
		#region Worksheet index
		void WriteIndex() {
			indexRecordPosition = writer.BaseStream.Position;
			XlsContentIndex content = new XlsContentIndex();
			content.DbCellsPositions.AddRange(currentSheet.DbCellsPositions);
			WriteContent(XlsRecordType.Index, content);
		}
		void RewriteIndex() {
			long savedPosition = writer.BaseStream.Position;
			writer.BaseStream.Position = indexRecordPosition;
			XlsContentIndex content = new XlsContentIndex();
			XlDimensions dimensions = currentSheet.Dimensions;
			if(dimensions != null) {
				content.FirstRowIndex = dimensions.FirstRowIndex;
				content.LastRowIndex = dimensions.LastRowIndex + 1;
			}
			content.DefaultColumnWidthOffset = defColWidthRecordPosition;
			foreach(long position in currentSheet.DbCellsPositions)
				content.DbCellsPositions.Add(position + cellTablePosition);
			WriteContent(XlsRecordType.Index, content);
			writer.BaseStream.Position = savedPosition;
		}
		#endregion
		#region Worksheet Globals
		void WriteWorksheetGlobals() {
			WriteContent(XlsRecordType.CalcMode, (short)1); 
			WriteContent(XlsRecordType.CalcCount, (short)100);
			WriteContent(XlsRecordType.CalcRefMode, (short)1); 
			WriteContent(XlsRecordType.CalcIter, (short)0);
			WriteContent(XlsRecordType.CalcDelta, 0.001);
			WriteContent(XlsRecordType.CalcSaveRecalc, true);
			WriteContent(XlsRecordType.PrintRowCol, currentSheet.PrintOptions != null ? currentSheet.PrintOptions.Headings : false);
			WriteContent(XlsRecordType.PrintGrid, currentSheet.PrintOptions != null ? currentSheet.PrintOptions.GridLines : false);
			WriteContent(XlsRecordType.GridSet, (short)1); 
			WriteGuts();
			WriteDefaultRowHeight();
			WriteWsBool();
			WritePageBreaks();
		}
		void WriteGuts() {
			int columnOutlineLevel = 0;
			foreach(XlsTableColumn column in currentSheet.Columns)
				columnOutlineLevel = Math.Max(columnOutlineLevel, column.OutlineLevel);
			XlsContentGuts content = new XlsContentGuts();
			content.RowGutterMaxOutlineLevel = Math.Max(0, Math.Min(7, maxRowOutlineLevel - 1));
			content.ColumnGutterMaxOutlineLevel = columnOutlineLevel;
			WriteContent(XlsRecordType.Guts, content);
		}
		void WriteDefaultRowHeight() {
			XlsContentDefaultRowHeight content = new XlsContentDefaultRowHeight();
			content.DefaultRowHeightInTwips = 300;
			WriteContent(XlsRecordType.DefaultRowHeight, content);
		}
		void WriteWsBool() {
			XlsContentWsBool content = new XlsContentWsBool();
			content.ShowPageBreaks = true;
			content.ShowRowSumsBelow = currentSheet.OutlineProperties.SummaryBelow;
			content.ShowColumnSumsRight = currentSheet.OutlineProperties.SummaryRight;
			content.FitToPage = (currentSheet.PageSetup != null) ? currentSheet.PageSetup.FitToPage : false;
			WriteContent(XlsRecordType.WsBool, content);
		}
		void WritePageBreaks() {
			if(currentSheet.RowPageBreaks.Count > 0) {
				XlsContentRowPageBreaks content = new XlsContentRowPageBreaks(currentSheet.RowPageBreaks);
				WriteContent(XlsRecordType.HorizontalPageBreaks, content);
			}
			if(currentSheet.ColumnPageBreaks.Count > 0) {
				XlsContentColumnPageBreaks content = new XlsContentColumnPageBreaks(currentSheet.ColumnPageBreaks);
				WriteContent(XlsRecordType.VerticalPageBreaks, content);
			}
		}
		#endregion
		#region Worksheet page setup
		void WriteWorksheetPageSetup() {
			WriteContent(XlsRecordType.Header, currentSheet.HeaderFooter.OddHeader);
			WriteContent(XlsRecordType.Footer, currentSheet.HeaderFooter.OddFooter);
			WriteContent(XlsRecordType.HCenter, currentSheet.PrintOptions != null ? currentSheet.PrintOptions.HorizontalCentered : false);
			WriteContent(XlsRecordType.VCenter, currentSheet.PrintOptions != null ? currentSheet.PrintOptions.VerticalCentered : false);
			XlPageMargins pageMargins = currentSheet.PageMargins;
			if(pageMargins == null) {
				WriteContent(XlsRecordType.LeftMargin, 0.7);
				WriteContent(XlsRecordType.RightMargin, 0.7);
				WriteContent(XlsRecordType.TopMargin, 0.75);
				WriteContent(XlsRecordType.BottomMargin, 0.75);
			}
			else {
				WriteContent(XlsRecordType.LeftMargin, pageMargins.LeftInches);
				WriteContent(XlsRecordType.RightMargin, pageMargins.RightInches);
				WriteContent(XlsRecordType.TopMargin, pageMargins.TopInches);
				WriteContent(XlsRecordType.BottomMargin, pageMargins.BottomInches);
			}
			WritePageSetup();
			WriteHeaderFooter();
		}
		void WritePageSetup() {
			XlsContentPageSetup content = new XlsContentPageSetup();
			XlPageSetup pageSetup = currentSheet.PageSetup;
			if(pageSetup == null) {
				content.PaperKind = PaperKind.Letter;
				content.Scale = 100;
				content.HorizontalDpi = 600;
				content.VerticalDpi = 600;
				content.Copies = 1;
				content.CommentsPrintMode = XlCommentsPrintMode.None;
				content.ErrorsPrintMode = XlErrorsPrintMode.Displayed;
				content.PageOrientation = XlPageOrientation.Default;
				content.FirstPageNumber = 1;
				content.FitToWidth = 0;
				content.FitToHeight = 0;
				content.PagePrintOrder = XlPagePrintOrder.DownThenOver;
			}
			else {
				content.PaperKind = pageSetup.PaperKind;
				content.Scale = pageSetup.Scale;
				content.HorizontalDpi = pageSetup.HorizontalDpi;
				content.VerticalDpi = pageSetup.VerticalDpi;
				content.Copies = pageSetup.Copies;
				content.CommentsPrintMode = pageSetup.CommentsPrintMode;
				content.ErrorsPrintMode = pageSetup.ErrorsPrintMode;
				content.PageOrientation = pageSetup.PageOrientation;
				content.FirstPageNumber = pageSetup.FirstPageNumber;
				content.FitToWidth = pageSetup.FitToWidth;
				content.FitToHeight = pageSetup.FitToHeight;
				content.PagePrintOrder = pageSetup.PagePrintOrder;
				content.BlackAndWhite = pageSetup.BlackAndWhite;
				content.Draft = pageSetup.Draft;
				content.UseFirstPageNumber = !pageSetup.AutomaticFirstPageNumber;
			}
			XlPageMargins pageMargins = currentSheet.PageMargins;
			if(pageMargins == null) {
				content.HeaderMargin = 0.3;
				content.FooterMargin = 0.3;
			}
			else {
				content.HeaderMargin = pageMargins.HeaderInches;
				content.FooterMargin = pageMargins.FooterInches;
			}
			WriteContent(XlsRecordType.Setup, content);
		}
		void WriteHeaderFooter() {
			XlHeaderFooter headerFooter = currentSheet.HeaderFooter;
			XlsContentHeaderFooter content = new XlsContentHeaderFooter();
			content.AlignWithMargins = headerFooter.AlignWithMargins;
			content.DifferentFirst = headerFooter.DifferentFirst;
			content.DifferentOddEven = headerFooter.DifferentOddEven;
			content.ScaleWithDoc = headerFooter.ScaleWithDoc;
			content.EvenFooter = headerFooter.EvenFooter;
			content.EvenHeader = headerFooter.EvenHeader;
			content.FirstFooter = headerFooter.FirstFooter;
			content.FirstHeader = headerFooter.FirstHeader;
			WriteContent(XlsRecordType.HeaderFooter, content);
		}
		#endregion
		#region AutoFilter
		void WriteAutoFilter() {
			XlCellRange range = currentSheet.AutoFilterRange;
			if(range == null)
				return;
			int columnCount = Math.Min(256, range.BottomRight.Column - range.TopLeft.Column + 1);
			WriteContent(XlsRecordType.AutoFilterInfo, (short)columnCount);
		}
		#endregion
		#region Worksheet dimensions
		void WriteDimensions() {
			XlsContentDimensions content = new XlsContentDimensions();
			XlDimensions dimensions = currentSheet.Dimensions;
			if(dimensions != null) {
				content.FirstRowIndex = dimensions.FirstRowIndex + 1;
				content.LastRowIndex = dimensions.LastRowIndex + 1;
				content.FirstColumnIndex = dimensions.FirstColumnIndex + 1;
				content.LastColumnIndex = dimensions.LastColumnIndex + 1;
			}
			else {
				content.FirstRowIndex = 1;
				content.LastRowIndex = 0;
				content.FirstColumnIndex = 1;
				content.LastColumnIndex = 0;
			}
			WriteContent(XlsRecordType.Dimensions, content);
		}
		#endregion
		#region Worksheet cell table
		void WriteCellTable() {
			if(currentSheet.Dimensions == null)
				return;
			writer.Flush();
			if(currentSheet.CellTableWriter != null) {
				currentSheet.CellTableWriter.Flush();
				Stream destStream = writer.BaseStream;
				Stream sourceStream = currentSheet.CellTableWriter.BaseStream;
				cellTablePosition = destStream.Position;
				workbookStream.Attach(sourceStream);
				destStream.Seek(0, SeekOrigin.End);
			}
		}
		#endregion
		#region Worksheet window
		protected void WriteWorksheetWindow() {
			XlsContentWorksheetWindow content = new XlsContentWorksheetWindow();
			IXlSheetViewOptions view = currentSheet.ViewOptions;
			content.ShowFormulas = view.ShowFormulas;
			content.ShowGridlines = view.ShowGridLines;
			content.ShowRowColumnHeadings = view.ShowRowColumnHeaders;
			content.ShowOutlineSymbols = view.ShowOutlineSymbols;
			content.ShowZeroValues = view.ShowZeroValues;
			content.RightToLeft = view.RightToLeft;
			content.Frozen = !currentSheet.SplitPosition.EqualsPosition(XlCellPosition.TopLeft);
			content.FrozenWithoutPaneSplit = content.Frozen;
			if(currentSheet.VisibleState == XlSheetVisibleState.Visible) {
				content.SheetTabIsSelected = firstVisibleSheet;
				if(firstVisibleSheet)
					firstVisibleSheet = false;
			}
			content.GridlinesColorIndex = XlsPalette.DefaultForegroundColorIndex;
			WriteContent(XlsRecordType.Window2, content);
			if(content.Frozen)
				WritePane();
		}
		void WritePane() {
			XlsContentPane content = new XlsContentPane();
			content.XPos = currentSheet.SplitPosition.Column;
			content.YPos = currentSheet.SplitPosition.Row;
			content.LeftColumn = content.XPos;
			content.TopRow = content.YPos;
			byte activePane = 0x00;
			if (content.XPos == 0)
				activePane = 0x02;
			if (content.YPos == 0)
				activePane = 0x01;
			content.ActivePane = activePane;
			WriteContent(XlsRecordType.Pane, content);
		}
		#endregion
		#region Merge cells
		protected void WriteMergeCells() {
			IXlMergedCells mergedCells = currentSheet.MergedCells;
			if(mergedCells.Count == 0) 
				return;
			XlsContentMergeCells content = new XlsContentMergeCells();
			foreach(XlCellRange range in mergedCells) {
				XlsRef8 item = XlsRef8.FromRange(range);
				if(item == null)
					continue;
				content.MergeCells.Add(item);
				if(content.MergeCells.Count >= XlsDefs.MaxMergeCellCount) {
					WriteContent(XlsRecordType.MergeCells, content);
					content.MergeCells.Clear();
				}
			}
			if(content.MergeCells.Count > 0)
				WriteContent(XlsRecordType.MergeCells, content);
		}
		#endregion
		#region SharedFeatures
		void WriteFeatures() {
			if(currentSheet.IgnoreErrors == XlIgnoreErrors.None || currentSheet.DataRange == null)
				return;
			WriteIgnoredErrorsHeader();
			WriteIngoredErrors();
		}
		void WriteIgnoredErrorsHeader() {
			XlsContentFeatHdr content = new XlsContentFeatHdr();
			content.FeatureType = XlsFeatureType.IgnoredErrors;
			WriteContent(XlsRecordType.FeatHdr, content);
		}
		void WriteIngoredErrors() {
			XlsContentFeatIgnoredErrors content = new XlsContentFeatIgnoredErrors();
			content.Refs.Add(XlsRef8.FromRange(currentSheet.DataRange));
			content.CalculationErrors = (currentSheet.IgnoreErrors & XlIgnoreErrors.EvaluationError) != 0;
			content.DataValidation = (currentSheet.IgnoreErrors & XlIgnoreErrors.ListDataValidation) != 0;
			content.EmptyCellRef = (currentSheet.IgnoreErrors & XlIgnoreErrors.EmptyCellReference) != 0;
			content.InconsistFormula = (currentSheet.IgnoreErrors & XlIgnoreErrors.Formula) != 0;
			content.InconsistRange = (currentSheet.IgnoreErrors & XlIgnoreErrors.FormulaRange) != 0;
			content.NumberStoredAsText = (currentSheet.IgnoreErrors & XlIgnoreErrors.NumberStoredAsText) != 0;
			content.TextDateInsuff = (currentSheet.IgnoreErrors & XlIgnoreErrors.TwoDigitTextYear) != 0;
			content.UnprotectedFormula = (currentSheet.IgnoreErrors & XlIgnoreErrors.UnlockedFormula) != 0;
			WriteContent(XlsRecordType.Feat, content);
		}
		#endregion
		void RegisterMergeCells() {
			IXlMergedCells mergedCells = currentSheet.MergedCells;
			if(mergedCells.Count == 0) 
				return;
			foreach(XlCellRange range in mergedCells) {
				XlsRef8 item = XlsRef8.FromRange(range);
				if(item != null)
					RegisterDimensions(item);
			}
		}
		void RegisterDimensions(XlsRef8 mergeCells) {
			if(currentSheet == null)
				return;
			XlDimensions dimensions = currentSheet.Dimensions;
			if(dimensions == null) {
				dimensions = new XlDimensions();
				dimensions.FirstRowIndex = mergeCells.FirstRowIndex;
				dimensions.LastRowIndex = mergeCells.LastRowIndex;
				dimensions.FirstColumnIndex = mergeCells.FirstColumnIndex;
				dimensions.LastColumnIndex = mergeCells.LastColumnIndex;
				currentSheet.Dimensions = dimensions;
			}
			else {
				dimensions.FirstRowIndex = Math.Min(dimensions.FirstRowIndex, mergeCells.FirstRowIndex);
				dimensions.LastRowIndex = Math.Max(dimensions.LastRowIndex, mergeCells.LastRowIndex);
				dimensions.FirstColumnIndex = Math.Min(dimensions.FirstColumnIndex, mergeCells.FirstColumnIndex);
				dimensions.LastColumnIndex = Math.Max(dimensions.LastColumnIndex, mergeCells.LastColumnIndex);
			}
		}
		void RegisterSheetDefinitions() {
			IList<XlDataValidation> dataValidations = currentSheet.DataValidations;
			int count = 0;
			foreach(XlDataValidation item in dataValidations) {
				if(RegisterDataValidation(item)) {
					count++;
					if(count > XlsDefs.MaxDataValidationRecordCount)
						break;
				}
			}
		}
		bool IsUniqueSheetName() {
			foreach(IXlSheet sheet in sheets)
				if(string.Equals(currentSheet.Name, sheet.Name, StringComparison.OrdinalIgnoreCase))
					return false;
			return true;
		}
	}
	#endregion
}
namespace DevExpress.XtraExport.Implementation {
	#region XlDimensions
	public class XlDimensions {
		public int FirstRowIndex { get; set; }
		public int LastRowIndex { get; set; }
		public int FirstColumnIndex { get; set; }
		public int LastColumnIndex { get; set; }
	}
	#endregion
	#region XlsPictureObject
	public class XlsPictureObject : XlDrawingObjectBase {
		public int PictureId { get; set; }
		public int BlipId { get; set; }
		public XlPictureHyperlink HyperlinkClick { get; set; }
	}
	#endregion
	#region XlsTableBasedDocumentSheet
	public class XlsTableBasedDocumentSheet : XlSheet, IDisposable {
		List<long> dbCellsPositions = new List<long>();
		ChunkedMemoryStream cellTableStream = null;
		BinaryWriter cellTableWriter = null;
		int columnIndex = 0;
		List<XlsTableColumn> columns = new List<XlsTableColumn>();
		Dictionary<int, IXlColumn> columnsTable = new Dictionary<int, IXlColumn>();
		List<XlsPictureObject> drawingObjects = new List<XlsPictureObject>();
		readonly Dictionary<int, int> rowHeights = new Dictionary<int, int>();
		public XlsTableBasedDocumentSheet(IXlExport exporter)
			: base(exporter) {
		}
		public int SheetId { get; set; }
		public List<XlsTableColumn> Columns { get { return columns; } }
		public Dictionary<int, IXlColumn> ColumnsTable { get { return columnsTable; } }
		public List<XlsPictureObject> DrawingObjects { get { return drawingObjects; } }
		public XlDimensions Dimensions { get; set; }
		public List<long> DbCellsPositions { get { return dbCellsPositions; } }
		public BinaryWriter CellTableWriter { get { return cellTableWriter; } }
		protected internal Dictionary<int, int> RowHeights { get { return rowHeights; } }
		protected internal int ColumnIndex { get { return columnIndex; } set { columnIndex = value; } }
		public XlsTableColumn CreateXlsColumn() {
			XlsTableColumn column = new XlsTableColumn(this);
			column.ColumnIndex = columnIndex;
			return column;
		}
		public void RegisterColumn(XlsTableColumn column) {
			if(columnsTable.ContainsKey(column.ColumnIndex))
				throw new InvalidOperationException("Column with such index already exists.");
			RegisterColumnIndex(column);
			columns.Add(column);
			columnsTable[column.ColumnIndex] = column;
			columnIndex = column.ColumnIndex + 1;
		}
		public BinaryWriter GetCellTableWriter() {
			if(cellTableWriter == null) {
				cellTableStream = new ChunkedMemoryStream();
				cellTableWriter = new BinaryWriter(cellTableStream);
			}
			return cellTableWriter;
		}
		public void RegisterRow(XlsTableRow row) {
			int height = row.HeightInPixels;
			if(height < 0)
				height = row.AutomaticHeightInPixels;
			if(row.IsHidden)
				height = 0;
			if(height >= 0)
				rowHeights.Add(row.RowIndex, height);
		}
		#region IDisposable Members
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(cellTableWriter != null) {
					((IDisposable)cellTableWriter).Dispose();
					cellTableWriter = null;
				}
				if(cellTableStream != null) {
					cellTableStream.Dispose();
					cellTableStream = null;
				}
				Dimensions = null;
				dbCellsPositions = null;
				columns = null;
				columnsTable = null;
				drawingObjects = null;
			}
			base.Dispose(disposing);
		}
		#endregion
	}
	#endregion
}
