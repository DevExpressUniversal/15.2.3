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

#define EXPORT_CHARTS
using System;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Office;
using DevExpress.Office.Utils;
using System.IO;
using DevExpress.Utils;
using DevExpress.Utils.StructuredStorage.Writer;
using DevExpress.XtraSpreadsheet.Import.Xls;
using System.Text;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Layout.Engine;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.XtraExport.Xls;
using DevExpress.Export.Xl;
using DevExpress.Office.DrawingML;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Export.Xls {
#if !DOTNET
	using System.Security.AccessControl;
#endif
	using DevExpress.XtraSpreadsheet.Services.Implementation;
	using DevExpress.XtraExport.Xlsx;
	using Compatibility.System.Drawing;
	using DevExpress.Office.Drawing;
	using DevExpress.Office.Model;
	public class XlsWorksheetExporter : XlsWorksheetExporterBase {
		#region Fields
		const int maxSelectionRecordCount = 512;
		XlsCellsExporter cellsExporter;
		XlsCondFmtExporter condFmtExporter;
		IndexRecordStub indexRecordStub;
		long defColWidthRecordPosition;
		Dictionary<int, int> shapeIdsTable = new Dictionary<int, int>();
		XlsExporter mainExporter;
#endregion
		public XlsWorksheetExporter(BinaryWriter streamWriter, DocumentModel documentModel, ExportXlsStyleSheet exportStyleSheet, Worksheet sheet, XlsExporter mainExporter)
			: base(streamWriter, documentModel, exportStyleSheet, sheet) {
			this.cellsExporter = new XlsCellsExporter(streamWriter, documentModel, exportStyleSheet, sheet);
			this.condFmtExporter = new XlsCondFmtExporter(streamWriter, documentModel, exportStyleSheet, sheet);
			this.mainExporter = mainExporter;
		}
#region Properties
		protected XlsCellsExporter CellsExporter { get { return cellsExporter; } }
		protected internal IndexRecordStub IndexRecordStub { get { return indexRecordStub; } set { indexRecordStub = value; } }
#endregion
		public override void WriteContent() {
			WriteBOF(XlsSubstreamType.Sheet);
			WriteIndex();
			WriteCalculationMode();
			WriteCalculationCount();
			WriteReferenceMode();
			WriteIterationEnabled();
			WriteCalculationDelta();
			WriteRecalculateBeforeSaved();
			WritePrintRowColHeadings();
			WritePrintGridLines();
			WritePrintGridLinesSet();
			WriteGuts();
			WriteDefaultRowHeight();
			WriteAdditionalWorksheetInfo();
			WriteHorizontalPageBreaks();
			WriteVerticalPageBreaks();
			WritePageHeader();
			WritePageFooter();
			WritePageHCenter();
			WritePageVCenter();
			WritePageLeftMargin();
			WritePageRightMargin();
			WritePageTopMargin();
			WritePageBottomMargin();
			WritePageSetup();
			WriteHeaderFooter();
			WriteSheetProtection();
			WriteSheetScenarioProtection();
			WriteSheetObjectsProtection();
			WriteSheetPasswordVerifier();
			WriteDefaultColumnWidth();
			WriteColumns();
			WriteAutoFilter();
			WriteDimensions();
			WriteCellTable();
			WriteObjects();
			WriteNotes();
			WritePivotTables();
			WriteSheetViewInfo();
			WritePageLayoutView();
			WriteSheetViewScale();
			WritePane();
			WriteSelections();
			WriteMergeCells();
			WriteConditionalFormatting();
			WriteHyperLinks();
			WriteDataValidations();
			WriteCodeName();
			WriteSharedFeatureProtection();
			WriteProtectedRanges();
			WriteSharedFeatureErrors();
			WriteListTables();
			WriteStubRecords();
			WriteEOF();
		}
		protected void WriteIndex() {
			XlsCommandIndex command = new XlsCommandIndex();
			int rowBlocksCount = CellsExporter.GetNumberOfRowBlocks();
			this.indexRecordStub = new IndexRecordStub(rowBlocksCount);
			command.Index = this.indexRecordStub;
			command.Write(StreamWriter);
		}
#region GLOBALS
		protected void WriteCalculationMode() {
			XlsCommandCalculationMode command = new XlsCommandCalculationMode();
			command.CalculationMode = DocumentModel.Properties.CalculationOptions.CalculationMode;
			command.Write(StreamWriter);
		}
		protected void WriteCalculationCount() {
			XlsCommandIterationCount command = new XlsCommandIterationCount();
			command.Value = (short)DocumentModel.Properties.CalculationOptions.MaximumIterations;
			command.Write(StreamWriter);
		}
		protected void WriteReferenceMode() {
			XlsCommandReferenceMode command = new XlsCommandReferenceMode();
			command.Value = !DocumentModel.Properties.UseR1C1ReferenceStyle;
			command.Write(StreamWriter);
		}
		protected void WriteIterationEnabled() {
			XlsCommandIterationsEnabled command = new XlsCommandIterationsEnabled();
			command.Value = DocumentModel.Properties.CalculationOptions.IterationsEnabled;
			command.Write(StreamWriter);
		}
		protected void WriteCalculationDelta() {
			XlsCommandCalculationDelta command = new XlsCommandCalculationDelta();
			command.Value = (double)DocumentModel.Properties.CalculationOptions.IterativeCalculationDelta;
			command.Write(StreamWriter);
		}
		protected void WriteRecalculateBeforeSaved() {
			XlsCommandRecalculateBeforeSaved command = new XlsCommandRecalculateBeforeSaved();
			command.Value = DocumentModel.Properties.CalculationOptions.RecalculateBeforeSaving;
			command.Write(StreamWriter);
		}
		protected void WritePrintRowColHeadings() {
			XlsCommandPrintRowColHeadings command = new XlsCommandPrintRowColHeadings();
			command.Value = Sheet.PrintSetup.PrintHeadings;
			command.Write(StreamWriter);
		}
		protected void WritePrintGridLines() {
			XlsCommandPrintGridLines command = new XlsCommandPrintGridLines();
			command.Value = Sheet.PrintSetup.PrintGridLines;
			command.Write(StreamWriter);
		}
		protected void WritePrintGridLinesSet() {
			XlsCommandPrintGridLinesSet command = new XlsCommandPrintGridLinesSet();
			command.Value = true;
			command.Write(StreamWriter);
		}
		protected void WriteGuts() {
			XlsCommandGuts command = new XlsCommandGuts();
			command.RowGutterMaxOutlineLevel = CellsExporter.RowsMaxOutlineLevel;
			command.ColumnGutterMaxOutlineLevel = GetColumnsMaxOutlineLevel();
			command.Write(StreamWriter);
		}
		protected void WriteDefaultRowHeight() {
			XlsCommandDefaultRowHeight command = new XlsCommandDefaultRowHeight();
			SheetFormatProperties formatProperties = Sheet.Properties.FormatProperties;
			command.IsCustomHeight = formatProperties.IsCustomHeight;
			command.ZeroHeight = formatProperties.ZeroHeight;
			command.ThickBottomBorder = formatProperties.ThickBottomBorder;
			command.ThickTopBorder = formatProperties.ThickTopBorder;
			command.DefaultRowHeightInTwips = (int)DocumentModel.UnitConverter.ModelUnitsToTwipsF(formatProperties.DefaultRowHeight);
			if(command.DefaultRowHeightInTwips == 0)
				command.DefaultRowHeightInTwips = XlsDefs.DefaultRowHeightInTwips;
			command.Write(StreamWriter);
		}
		protected void WriteAdditionalWorksheetInfo() {
			XlsCommandAdditionalWorksheetInformation command = new XlsCommandAdditionalWorksheetInformation();
			WorksheetProperties sheetProperties = Sheet.Properties;
			ModelWorksheetView view = Sheet.ActiveView;
			command.IsDialog = sheetProperties.IsDialog;
			command.ApplyStyles = sheetProperties.GroupAndOutlineProperties.ApplyStyles;
			command.ShowColumnSumsRight = sheetProperties.GroupAndOutlineProperties.ShowColumnSumsRight;
			command.ShowRowSumsBelow = sheetProperties.GroupAndOutlineProperties.ShowRowSumsBelow;
			command.FitToPage = sheetProperties.PrintSetup.FitToPage;
			command.ShowPageBreaks = view.ShowPageBreaks;
			command.SynchronizeHorizontalScrolling = view.SynchronizeHorizontalScrolling;
			command.SynchronizeVerticalScrolling = view.SynchronizeVerticalScrolling;
			command.TransitionFormulaEntry = sheetProperties.TransitionOptions.TransitionFormulaEntry;
			command.TransitionFormulaEvaluation = sheetProperties.TransitionOptions.TransitionFormulaEvaluation;
			command.Write(StreamWriter);
		}
		protected void WriteHorizontalPageBreaks() {
			PageBreakCollection breaks = Sheet.RowBreaks;
			int count = breaks.Count;
			if(count == 0) return;
			XlsCommandHorizontalPageBreaks command = new XlsCommandHorizontalPageBreaks();
			for(int i = 0; i < count; i++) {
				if (breaks[i] < XlsDefs.MaxRowCount) {
					PageBreakRecord item = new PageBreakRecord();
					item.Position = breaks[i];
					item.Start = 0;
					item.End = 0xff;
					command.Items.Add(item);
				}
			}
			command.Write(StreamWriter);
		}
		protected void WriteVerticalPageBreaks() {
			PageBreakCollection breaks = Sheet.ColumnBreaks;
			int count = breaks.Count;
			if(count == 0) return;
			XlsCommandVerticalPageBreaks command = new XlsCommandVerticalPageBreaks();
			for(int i = 0; i < count; i++) {
				if (breaks[i] < XlsDefs.MaxColumnCount) {
					PageBreakRecord item = new PageBreakRecord();
					item.Position = breaks[i];
					item.Start = 0;
					item.End = 0xffff;
					command.Items.Add(item);
				}
			}
			command.Write(StreamWriter);
		}
#endregion
#region PAGESETUP
		protected void WritePageHeader() {
			XlsCommandPageHeader command = new XlsCommandPageHeader();
			command.Value = Sheet.Properties.HeaderFooter.OddHeader;
			command.Write(StreamWriter);
		}
		protected void WritePageFooter() {
			XlsCommandPageFooter command = new XlsCommandPageFooter();
			command.Value = Sheet.Properties.HeaderFooter.OddFooter;
			command.Write(StreamWriter);
		}
		protected void WritePageHCenter() {
			XlsCommandPageHCenter command = new XlsCommandPageHCenter();
			command.Value = Sheet.PrintSetup.CenterHorizontally;
			command.Write(StreamWriter);
		}
		protected void WritePageVCenter() {
			XlsCommandPageVCenter command = new XlsCommandPageVCenter();
			command.Value = Sheet.PrintSetup.CenterVertically;
			command.Write(StreamWriter);
		}
		protected void WritePageLeftMargin() {
			XlsCommandPageLeftMargin command = new XlsCommandPageLeftMargin();
			command.Value = DocumentModel.UnitConverter.ModelUnitsToInchesF(Sheet.Margins.Left);
			command.Write(StreamWriter);
		}
		protected void WritePageRightMargin() {
			XlsCommandPageRightMargin command = new XlsCommandPageRightMargin();
			command.Value = DocumentModel.UnitConverter.ModelUnitsToInchesF(Sheet.Margins.Right);
			command.Write(StreamWriter);
		}
		protected void WritePageTopMargin() {
			XlsCommandPageTopMargin command = new XlsCommandPageTopMargin();
			command.Value = DocumentModel.UnitConverter.ModelUnitsToInchesF(Sheet.Margins.Top);
			command.Write(StreamWriter);
		}
		protected void WritePageBottomMargin() {
			XlsCommandPageBottomMargin command = new XlsCommandPageBottomMargin();
			command.Value = DocumentModel.UnitConverter.ModelUnitsToInchesF(Sheet.Margins.Bottom);
			command.Write(StreamWriter);
		}
		protected void WritePageSetup() {
			XlsCommandPageSetup command = new XlsCommandPageSetup();
			WorksheetProperties sheetProperties = Sheet.Properties;
			command.Properties.CopyFrom(sheetProperties.PrintSetup.Info);
			command.HeaderMargin = DocumentModel.UnitConverter.ModelUnitsToInchesF(sheetProperties.Margins.Header);
			command.FooterMargin = DocumentModel.UnitConverter.ModelUnitsToInchesF(sheetProperties.Margins.Footer);
			command.Write(StreamWriter);
		}
		protected void WriteHeaderFooter() {
			XlsCommandHeaderFooter command = new XlsCommandHeaderFooter();
			HeaderFooterOptions options = Sheet.Properties.HeaderFooter;
			command.AlignWithMargins = options.AlignWithMargins;
			command.DifferentFirst = options.DifferentFirst;
			command.DifferentOddEven = options.DifferentOddEven;
			command.ScaleWithDoc = options.ScaleWithDoc;
			if(options.DifferentOddEven) {
				command.EvenHeader = options.EvenHeader;
				command.EvenFooter = options.EvenFooter;
			}
			if(options.DifferentFirst) {
				command.FirstHeader = options.FirstHeader;
				command.FirstFooter = options.FirstFooter;
			}
			command.Write(StreamWriter);
		}
#endregion
#region PROTECTION
		protected void WriteSheetProtection() {
			if(!Sheet.Properties.Protection.SheetLocked) return;
			XlsCommandProtected command = new XlsCommandProtected();
			command.Value = true;
			command.Write(StreamWriter);
		}
		protected void WriteSheetScenarioProtection() {
			if (!Sheet.Properties.Protection.SheetLocked) return;
			if (!Sheet.Properties.Protection.ScenariosLocked) return;
			XlsCommandScenarioProtected command = new XlsCommandScenarioProtected();
			command.Value = true;
			command.Write(StreamWriter);
		}
		protected void WriteSheetObjectsProtection() {
			if (!Sheet.Properties.Protection.SheetLocked) return;
			if (!Sheet.Properties.Protection.ObjectsLocked) return;
			XlsCommandObjectsProtected command = new XlsCommandObjectsProtected();
			command.Value = true;
			command.Write(StreamWriter);
		}
		protected void WriteSheetPasswordVerifier() {
			ProtectionCredentials credentials = Sheet.Properties.Protection.Credentials;
			if (credentials.IsEmpty)
				return;
			ProtectionByPasswordVerifier byPassword = credentials.PasswordVerifier;
			if (byPassword == null)
				return;
			XlsCommandPasswordVerifier command = new XlsCommandPasswordVerifier();
			command.Value = (short)byPassword.Value;
			command.Write(StreamWriter);
		}
#endregion
#region COLUMNS
		protected void WriteDefaultColumnWidth() {
			this.defColWidthRecordPosition = StreamWriter.BaseStream.Position;
			XlsCommandDefaultColumnWidth command = new XlsCommandDefaultColumnWidth();
			command.Value = (short)Sheet.Properties.FormatProperties.BaseColumnWidth;
			if(command.Value == 0)
				command.Value = XlsDefs.DefaultColumnWidth;
			command.Write(StreamWriter);
		}
		protected void WriteColumns() {
			ColumnCollection columns = Sheet.Columns;
			IList<Column> columnRanges = columns.InnerList;
			XlsCommandColumnInfo command = new XlsCommandColumnInfo();
			IColumnWidthCalculationService columnWidthCalculator = DocumentModel.GetService<IColumnWidthCalculationService>();
			float defaultColumnWidth = columnWidthCalculator.CalculateDefaultColumnWidthInChars(Sheet, DocumentModel.MaxDigitWidthInPixels);
			float defaultColumnWidthWithGaps = columnWidthCalculator.AddGaps(Sheet, defaultColumnWidth);
			int lastColumn = -1;
			foreach (Column info in columnRanges) {
				if (info.StartIndex >= XlsDefs.MaxColumnCount)
					break;
				if(info.StartIndex > (lastColumn + 1))
					WriteDefaultColumns(lastColumn + 1, info.StartIndex - 1, defaultColumnWidthWithGaps);
				command.FirstColumn = info.StartIndex;
				if (info.EndIndex >= (XlsDefs.MaxColumnCount - 1))
					command.LastColumn = XlsDefs.FullRangeColumnIndex;
				else
					command.LastColumn = info.EndIndex;
				lastColumn = command.LastColumn;
				float columnWidth = info.Width;
				columnWidth = (float)Math.Min(255, columnWidthCalculator.AddGaps(Sheet, columnWidth));
				command.ColumnWidth = (int)(columnWidth * 256);
				command.FormatIndex = ExportStyleSheet.GetXFIndex(info.FormatIndex);
				command.BestFit = info.BestFit;
				command.Hidden = info.IsHidden;
				command.CustomWidth = info.IsCustomWidth;
				command.ShowPhoneticInfo = false;
				command.Collapsed = info.IsCollapsed;
				command.OutlineLevel = info.OutlineLevel;
				command.Write(StreamWriter);
			}
			if(lastColumn < XlsDefs.FullRangeColumnIndex)
				WriteDefaultColumns(lastColumn + 1, XlsDefs.FullRangeColumnIndex, defaultColumnWidthWithGaps);
		}
		void WriteDefaultColumns(int firstColumn, int lastColumn, float columnWidthInCharacters) {
			XlsCommandColumnInfo command = new XlsCommandColumnInfo();
			command.FirstColumn = firstColumn;
			command.LastColumn = lastColumn;
			command.ColumnWidth = (int)(Math.Min(255, columnWidthInCharacters) * 256);
			command.FormatIndex = 15;
			command.BestFit = false;
			command.Hidden = false;
			command.CustomWidth = false;
			command.ShowPhoneticInfo = false;
			command.Collapsed = false;
			command.OutlineLevel = 0;
			command.Write(StreamWriter);
		}
#endregion
#region SORTANDFILTER
		protected void WriteAutoFilter() {
			SheetAutoFilter autoFilter = Sheet.AutoFilter;
			if (!autoFilter.Enabled)
				return;
			WriteFilterMode(autoFilter.FilterColumns);
			WriteAutoFilterInfo();
			WriteAutoFilters(autoFilter, WriteSheetAutoFilter);
			SortState sortState = autoFilter.SortState;
			if (sortState.SortRange != null)
				WriteSortData(XlsSortDataInfo.CreateForSheetExport(sortState));
		}
		void WriteFilterMode(AutoFilterColumnCollection columns) {
			if (AutoFilterIsDefault(columns))
				return;
			XlsCommandFilterMode command = new XlsCommandFilterMode();
			command.Write(StreamWriter);
		}
		bool AutoFilterIsDefault(AutoFilterColumnCollection columns) {
			int count = columns.Count;
			for (int i = 0; i < count; i++)
				if (columns[i].IsNonDefault)
					return false;
			return true;
		}
		void WriteAutoFilterInfo() {
			XlsCommandAutoFilterInfo command = new XlsCommandAutoFilterInfo();
			command.Value = (short)Sheet.AutoFilter.Range.Width;
			command.Write(StreamWriter);
		}
		void WriteAutoFilters(AutoFilterBase autoFilter, Action<AutoFilterColumn, CellRange> writeAutoFilter) {
			AutoFilterColumnCollection filters = autoFilter.FilterColumns;
			int count = filters.Count;
			for (int i = 0; i < count; i++)
				writeAutoFilter(filters[i], autoFilter.Range);
		}
		void WriteSheetAutoFilter(AutoFilterColumn filterColumn, CellRange range) {
			if (!filterColumn.IsNonDefault)
				return;
			if (filterColumn.ShouldWriteAutoFilter12)
				WriteSheetAutoFilter12(filterColumn);
			else
				WriteAutoFilterCommand(filterColumn);
		}
		void WriteSheetAutoFilter12(AutoFilterColumn filterColumn) {
			WriteBlankAutoFilterCommand(filterColumn.ColumnId);
			CellRange range = Sheet.AutoFilter.Range;
			CellRangeInfo cellRangeInfo = new CellRangeInfo(range.TopLeft, range.BottomRight);
			XlsAutoFilter12Info info = XlsAutoFilter12Info.CreateForSheetExport(cellRangeInfo, filterColumn);
			WriteAutoFilter12Command(info, cellRangeInfo);
		}
		void WriteBlankAutoFilterCommand(int columnId) {
			XlsCommandAutoFilter command = new XlsCommandAutoFilter();
			XlsAutoFilterInfo autoFilterInfo = new XlsAutoFilterInfo();
			autoFilterInfo.IdEntry = columnId;
			autoFilterInfo.WJoin = true;
			autoFilterInfo.FirstCriteria.DataOperation.FilterComparisonOperator = XlsFilterComparisonOperator.Equal;
			command.AutoFilterInfo = autoFilterInfo;
			command.Write(StreamWriter);
		}
		void WriteAutoFilter12Command(XlsAutoFilter12Info info, CellRangeInfo cellRangeInfo) {
			XlsCommandAutoFilter12 command = new XlsCommandAutoFilter12();
			XlsCommandContinueFrt12 continueCommand = new XlsCommandContinueFrt12();
			short typeCode = XlsCommandFactory.GetTypeCodeByType(continueCommand.GetType());
			continueCommand.FrtHeader = FutureRecordHeaderRef.Create(cellRangeInfo, typeCode);
			using (XlsChunkWriter chunkWriter = new XlsChunkWriter(StreamWriter, command, continueCommand))
				info.Write(chunkWriter);
		}
		void WriteAutoFilterCommand(AutoFilterColumn filterColumn) {
			XlsCommandAutoFilter command = new XlsCommandAutoFilter();
			command.AutoFilterInfo = XlsAutoFilterInfo.CreateForExport(filterColumn);
			command.Write(StreamWriter);
		}
		void WriteSortData(XlsSortDataInfo info) {
			XlsCommandSortData command = new XlsCommandSortData();
			XlsCommandContinueFrt12 continueCommand = new XlsCommandContinueFrt12();
			FutureRecordHeader header = new FutureRecordHeader();
			header.RecordTypeId = XlsCommandFactory.GetTypeCodeByType(continueCommand.GetType());
			continueCommand.FrtHeader = header;
			using (XlsChunkWriter chunkWriter = new XlsChunkWriter(StreamWriter, command, continueCommand))
				info.Write(chunkWriter);
		}
		protected void WriteDimensions() {
			XlsCommandDimensions command = new XlsCommandDimensions();
			CellRange range = CellsExporter.DimensionsRange;
			if (range != null) {
				command.Dimensions.FirstRowIndex = range.TopLeft.Row + 1;
				command.Dimensions.LastRowIndex = range.BottomRight.Row + 1;
				command.Dimensions.FirstColumnIndex = range.TopLeft.Column + 1;
				command.Dimensions.LastColumnIndex = range.BottomRight.Column + 1;
			}
			else {
				command.Dimensions.FirstRowIndex = 1;
				command.Dimensions.LastRowIndex = 0;
				command.Dimensions.FirstColumnIndex = 1;
				command.Dimensions.LastColumnIndex = 0;
			}
			command.Write(StreamWriter);
		}
#endregion
#region CELLTABLE
		void WriteCellTable() {
			CellsExporter.WriteContent();
		}
#endregion
#region OBJECTS
		protected void WriteObjects() {
			if (ShouldWriteObjects()) {
				OfficeArtDrawingObjectsContainer drawingObjects = new OfficeArtDrawingObjectsContainer(Sheet.SheetId);
				FillShapeIdTable();
				FillDrawingData(drawingObjects);
				FillShapeGroup(drawingObjects);
				XlsCommandMsoDrawing firstCommand = new XlsCommandMsoDrawing();
				XlsCommandContinue continueCommand = new XlsCommandContinue();
				using (XlsChunkWriter writer = new XlsChunkWriter(StreamWriter, firstCommand, continueCommand)) {
					drawingObjects.Write(writer);
				}
			}
		}
#if EXPORT_CHARTS
		bool ShouldWriteObjects() {
			return (Sheet.DrawingObjects.GetPictureCount() > 0 && Sheet.Workbook.DocumentCapabilities.PicturesAllowed) ||
				   (Sheet.DrawingObjects.GetChartCount() > 0 && Sheet.Workbook.DocumentCapabilities.ChartsAllowed) ||
				   (Sheet.DrawingObjects.GetShapesCount() > 0 && Sheet.Workbook.DocumentCapabilities.ShapesAllowed) ||
				   Sheet.Comments.CountInXlsCellRange() > 0;
		}
		void FillShapeIdTable() {
			shapeIdsTable.Clear();
			if (!Sheet.Workbook.DocumentCapabilities.PicturesAllowed && !Sheet.Workbook.DocumentCapabilities.ChartsAllowed && !Sheet.Workbook.DocumentCapabilities.ShapesAllowed)
				return;
			int topmostShapeId = Sheet.GetTopmostShapeId();
			foreach(IDrawingObject drawing in Sheet.DrawingObjects.GetDrawings()) {
				switch(drawing.DrawingType) {
					case DrawingObjectType.Picture:
						if(!Sheet.Workbook.DocumentCapabilities.PicturesAllowed)
							continue;
						break;
					case DrawingObjectType.Chart:
						if(!Sheet.Workbook.DocumentCapabilities.ChartsAllowed)
							continue;
						break;
					case DrawingObjectType.Shape:
						if(!Sheet.Workbook.DocumentCapabilities.ShapesAllowed)
							continue;
						break;
					case DrawingObjectType.ConnectionShape:
						continue;
					case DrawingObjectType.GroupShape:
						continue;
					default:
						continue;
				}
				shapeIdsTable.Add(drawing.DrawingObject.Properties.Id, topmostShapeId + shapeIdsTable.Count + 1);
			}
		}
		void FillDrawingData(OfficeArtDrawingObjectsContainer drawingObjects) {
			int pictureCount = Sheet.DrawingObjects.GetPictureCount();
			int chartCount = Sheet.DrawingObjects.GetChartCount();
			int shapesCount = Sheet.DrawingObjects.GetShapesCount();
			if (!Sheet.Workbook.DocumentCapabilities.PicturesAllowed)
				pictureCount = 0;
			if (!Sheet.Workbook.DocumentCapabilities.ChartsAllowed)
				chartCount = 0;
			if(!Sheet.Workbook.DocumentCapabilities.ShapesAllowed)
				shapesCount = 0;
			int objectsCount = pictureCount + chartCount + Sheet.Comments.CountInXlsCellRange() + shapesCount;
			drawingObjects.DrawingData.LastShapeIdentifier = Sheet.GetTopmostShapeId() + objectsCount;
			drawingObjects.DrawingData.NumberOfShapes = objectsCount + 1;
		}
		void FillShapeGroup(OfficeArtDrawingObjectsContainer drawingObjects) {
			OfficeArtShapeGroupContainer shapeGroup = drawingObjects.ShapeGroup;
			List<IDrawingObject> drawings = Sheet.DrawingObjects.GetDrawings();
			CommentCollection comments = Sheet.Comments;
			int drawingCount = 0;
			int commentCount = comments.Count;
			int topmostShapeId = Sheet.GetTopmostShapeId();
			shapeGroup.TopmostShape.ShapeRecord.ShapeIdentifier = topmostShapeId;
			foreach(IDrawingObject drawing in drawings) {
				OfficeArtShapeContainer shape = null;
				switch(drawing.DrawingType) {
					case DrawingObjectType.Picture:
						if(!Sheet.Workbook.DocumentCapabilities.PicturesAllowed)
							continue;
						shape = CreateShape(drawing as Picture);
						break;
					case DrawingObjectType.Chart:
						if(!Sheet.Workbook.DocumentCapabilities.ChartsAllowed)
							continue;
						shape = CreateShape(drawing as Chart);
						break;
					case DrawingObjectType.Shape:
						if(!Sheet.Workbook.DocumentCapabilities.ShapesAllowed)
							continue;
						shape = CreateShape(drawing as ModelShape);
						break;
					case DrawingObjectType.ConnectionShape:
						continue;
					case DrawingObjectType.GroupShape:
						continue;
					default:
						continue;
				}
				if(shape == null)
					continue;
				shapeGroup.Items.Add(shape);
				drawingCount++;
			}
			int id = topmostShapeId + drawingCount + 1;
			for(int i = 0; i < commentCount; i++) {
				if(comments[i].Reference.OutOfLimits())
					continue;
				OfficeArtShapeContainer shape = CreateShape(comments[i], id++);
				shapeGroup.Items.Add(shape);
			}
		}
#else
		bool ShouldWriteObjects() {
			return (Sheet.DrawingObjects.GetPictureCount() > 0 && Sheet.Workbook.DocumentCapabilities.IsPicturesAllowed) ||
				Sheet.Comments.CountInXlsCellRange() > 0;
		}
		void FillShapeIdTable() {
			shapeIdsTable.Clear();
			if (!Sheet.Workbook.DocumentCapabilities.IsPicturesAllowed)
				return;
			int topmostShapeId = Sheet.GetTopmostShapeId();
			foreach(Picture picture in Sheet.DrawingObjects.GetPictures()) {
				shapeIdsTable.Add(picture.DrawingObject.Properties.Id, topmostShapeId + shapeIdsTable.Count + 1);
			}
		}
		void FillDrawingData(OfficeArtDrawingObjectsContainer drawingObjects) {
			int pictureCount = Sheet.DrawingObjects.GetPictureCount();
			if (!Sheet.Workbook.DocumentCapabilities.IsPicturesAllowed)
				pictureCount = 0;
			int objectsCount = pictureCount + Sheet.Comments.CountInXlsCellRange();
			drawingObjects.DrawingData.LastShapeIdentifier = Sheet.GetTopmostShapeId() + objectsCount;
			drawingObjects.DrawingData.NumberOfShapes = objectsCount + 1;
		}
		void FillShapeGroup(OfficeArtDrawingObjectsContainer drawingObjects) {
			OfficeArtShapeGroupContainer shapeGroup = drawingObjects.ShapeGroup;
			List<Picture> pictures = Sheet.DrawingObjects.GetPictures();
			CommentCollection comments = Sheet.Comments;
			int pictureCount = pictures.Count;
			if (!Sheet.Workbook.DocumentCapabilities.IsPicturesAllowed)
				pictureCount = 0;
			int commentCount = comments.Count;
			int topmostShapeId = Sheet.GetTopmostShapeId();
			shapeGroup.TopmostShape.ShapeRecord.ShapeIdentifier = topmostShapeId;
			for(int i = 0; i < pictureCount; i++) {
				OfficeArtShapeContainer shape = CreateShape(pictures[i]);
				shapeGroup.Items.Add(shape);
			}
			int id = topmostShapeId + pictureCount + 1;
			for(int i = 0; i < commentCount; i++) {
				if(comments[i].Reference.OutOfLimits()) continue;
				OfficeArtShapeContainer shape = CreateShape(comments[i], id++);
				shapeGroup.Items.Add(shape);
			}
		}
#endif
#region Pictures
		OfficeArtShapeContainer CreateShape(Picture picture) {
			OfficeArtShapeContainer shape = new OfficeArtShapeContainer();
			OfficeArtShapeRecord shapeRecord = CreateShapeRecord(picture);
			shape.Items.Add(shapeRecord);
			OfficeArtProperties artProperties = CreateProperties(picture);
			shape.Items.Add(artProperties);
			OfficeArtTertiaryProperties artTertiaryProperties = CreateTertiaryProperties(picture.DrawingObject);
			if (artTertiaryProperties != null)
				shape.Items.Add(artTertiaryProperties);
			OfficeArtClientAnchorSheet clientAnchor = CreateClientAnchor(picture, picture.CheckRotationAndSwapBox);
			shape.Items.Add(clientAnchor);
			OfficeArtClientData clientData = CreateClientData(picture);
			shape.Items.Add(clientData);
			return shape;
		}
#region Shape record
		OfficeArtShapeRecord CreateShapeRecord(Picture picture) {
			OfficeArtShapeRecord shapeRecord = new OfficeArtShapeRecord(GetShapeType(picture));
			shapeRecord.HaveAnchor = true;
			shapeRecord.HaveShapeType = true;
			shapeRecord.ShapeIdentifier = shapeIdsTable[picture.DrawingObject.Properties.Id];
			shapeRecord.FlipH = picture.ShapeProperties.Transform2D.FlipH;
			shapeRecord.FlipV = picture.ShapeProperties.Transform2D.FlipV;
			return shapeRecord;
		}
		int GetShapeType(Picture picture) {
			ShapeType shapeType = picture.ShapeProperties.ShapeType;
			switch (shapeType) {
				case ShapeType.RoundRect:
					return ShapeTypeCode.RoundRect;
				case ShapeType.Ellipse:
					return ShapeTypeCode.Ellipse;
				case ShapeType.Line:
					return ShapeTypeCode.Line;
				case ShapeType.Bevel:
					return ShapeTypeCode.Bevel;
			}
			return ShapeTypeCode.PictureFrame; 
		}
#endregion
#region Picture properties
		OfficeArtProperties CreateProperties(Picture picture) {
			OfficeArtProperties artProperties = new OfficeArtProperties();
			CreateTransformProperties(picture.ShapeProperties.Transform2D, artProperties);
			CreateProtectionProperties(picture, artProperties);
			CreateBlipProperties(picture, artProperties);
			artProperties.Properties.Add(new DrawingFillStyleBooleanProperties());
			CreateOutlineProperties(picture.ShapeProperties, artProperties);
			CreateShapeProperties(picture, artProperties);
			CreateGroupShapeProperties(picture.DrawingObject, artProperties);
			return artProperties;
		}
		OfficeArtTertiaryProperties CreateTertiaryProperties(DrawingObject drawingObject) {
			if (string.IsNullOrEmpty(drawingObject.Properties.HyperlinkClickUrl) ||
				string.IsNullOrEmpty(drawingObject.Properties.HyperlinkClickTooltip))
				return null;
			OfficeArtTertiaryProperties artProperties = new OfficeArtTertiaryProperties();
			artProperties.Properties.Clear();
			DrawingShapeTooltip prop = new DrawingShapeTooltip();
			prop.Data = drawingObject.Properties.HyperlinkClickTooltip + "\0";
			artProperties.Properties.Add(prop);
			return artProperties;
		}
		void CreateTransformProperties(Transform2D transform2D, OfficeArtProperties artProperties) {
			if (transform2D.Rotation != 0) {
				DrawingRotation rotation = new DrawingRotation();
				rotation.Value = DocumentModel.UnitConverter.ModelUnitsToDegreeF(transform2D.Rotation);
				artProperties.Properties.Add(rotation);
			}
		}
		void CreateProtectionProperties(Picture picture, OfficeArtProperties artProperties) {
			DrawingBooleanProtectionProperties protection = new DrawingBooleanProtectionProperties();
			protection.LockCropping = picture.Locks.NoCrop;
			protection.UseLockCropping = true;
			CreateCommonDrawingProtectionProperties(picture.Locks, protection);
			artProperties.Properties.Add(protection);
		}
		void CreateBlipProperties(Picture picture, OfficeArtProperties artProperties) {
			if (picture.PictureFill.Blip.Embedded) {
				int blipIndex = 0;
				if (ExportStyleSheet.DrawingTable.ContainsKey(picture.Image.RootImage))
					blipIndex = ExportStyleSheet.DrawingTable[picture.Image.RootImage].Index + 1;
				if (blipIndex > 0) {
					DrawingBlipIdentifier drawingBlipIdentifier = new DrawingBlipIdentifier();
					drawingBlipIdentifier.Value = blipIndex;
					artProperties.Properties.Add(drawingBlipIdentifier);
				}
			}
			else {
				string uri = picture.PictureFill.Blip.Link;
				DrawingBlipName blipName = new DrawingBlipName();
				blipName.Data = string.Format("{0}\0", uri);
				artProperties.Properties.Add(blipName);
				DrawingBlipFlags blipFlags = new DrawingBlipFlags();
				if (uri.IndexOf("://") != -1)
					blipFlags.IsUrl = true;
				else
					blipFlags.IsFile = true;
				blipFlags.DontSave = true;
				blipFlags.LinkToFile = true;
				artProperties.Properties.Add(blipFlags);
			}
			artProperties.Properties.Add(new DrawingBlipBooleanProperties());
		}
		void CreateOutlineProperties(ShapeProperties shapeProperties, OfficeArtProperties artProperties) {
			DrawingLineStyleBooleanProperties drawingLineStyle = new DrawingLineStyleBooleanProperties();
			if (shapeProperties.OutlineType == OutlineType.Solid) {
				DrawingColor outlineColor = shapeProperties.OutlineColor;
				if (!outlineColor.Info.IsEmpty) {
					DrawingLineColor drawingLineColor = new DrawingLineColor();
					if (outlineColor.ColorType == DrawingColorType.System) {
						int systemColorIndex = (int)outlineColor.Info.SystemColor;
						if (systemColorIndex >= 0)
							drawingLineColor.ColorRecord.SystemColorIndex = systemColorIndex;
						else
							drawingLineColor.ColorRecord.Color = outlineColor.FinalColor;
					}
					else
						drawingLineColor.ColorRecord.Color = outlineColor.FinalColor;
					artProperties.Properties.Add(drawingLineColor);
				}
				DrawingLineWidth drawingLineWidth = new DrawingLineWidth();
				drawingLineWidth.Value = DocumentModel.UnitConverter.ModelUnitsToEmu(shapeProperties.Outline.Width);
				artProperties.Properties.Add(drawingLineWidth);
				if (shapeProperties.Outline.MiterLimit != OutlineInfo.DefaultInfo.MiterLimit) {
					DrawingLineMiterLimit miterLimit = new DrawingLineMiterLimit();
					miterLimit.Value = DrawingValueConverter.FromPercentage(shapeProperties.Outline.MiterLimit);
					artProperties.Properties.Add(miterLimit);
				}
				if (shapeProperties.Outline.CompoundType != OutlineCompoundType.Single) {
					DrawingLineCompoundType lineType = new DrawingLineCompoundType();
					lineType.CompoundType = shapeProperties.Outline.CompoundType;
					artProperties.Properties.Add(lineType);
				}
				if (shapeProperties.Outline.Dashing != OutlineDashing.Solid) {
					DrawingLineDashing lineDashing = new DrawingLineDashing();
					lineDashing.Dashing = shapeProperties.Outline.Dashing;
					artProperties.Properties.Add(lineDashing);
				}
				DrawingLineJoinStyle lineJoin = new DrawingLineJoinStyle();
				lineJoin.Style = shapeProperties.Outline.JoinStyle;
				artProperties.Properties.Add(lineJoin);
				if (shapeProperties.Outline.EndCapStyle != OutlineEndCapStyle.Flat) {
					DrawingLineCapStyle capStyle = new DrawingLineCapStyle();
					capStyle.Style = shapeProperties.Outline.EndCapStyle;
					artProperties.Properties.Add(capStyle);
				}
				drawingLineStyle.UseLine = true;
				drawingLineStyle.Line = true;
			}
			artProperties.Properties.Add(drawingLineStyle);
		}
		void CreateShapeProperties(Picture picture, OfficeArtProperties artProperties) {
			DrawingShapeBooleanProperties shapeBoolProps = new DrawingShapeBooleanProperties();
			shapeBoolProps.Value = 0;
			shapeBoolProps.UseLockShapeType = true;
			shapeBoolProps.UsePreferRelativeResize = true;
			shapeBoolProps.LockShapeType = picture.Locks.NoChangeShapeType;
			shapeBoolProps.PreferRelativeResize = picture.Properties.PreferRelativeResize;
			artProperties.Properties.Add(shapeBoolProps);
			if (picture.ShapeProperties.BlackAndWhiteMode != OpenXmlBlackWhiteMode.Auto) {
				DrawingBlackWhiteMode drawingBlackWhiteMode = new DrawingBlackWhiteMode();
				drawingBlackWhiteMode.Mode = ConvertBlackWhiteMode(picture.ShapeProperties.BlackAndWhiteMode);
				artProperties.Properties.Add(drawingBlackWhiteMode);
			}
		}
		void CreateGroupShapeProperties(DrawingObject drawingObject, OfficeArtProperties artProperties) {
			if (!string.IsNullOrEmpty(drawingObject.Properties.Name)) {
				DrawingShapeName property = new DrawingShapeName();
				property.Data = String.Format("{0}\0", drawingObject.Properties.Name);
				artProperties.Properties.Add(property);
			}
			if (!string.IsNullOrEmpty(drawingObject.Properties.Description)) {
				DrawingShapeDescription property = new DrawingShapeDescription();
				property.Data = String.Format("{0}\0", drawingObject.Properties.Description);
				artProperties.Properties.Add(property);
			}
			byte[] hyperlinkData = GetPictureHyperlinkData(drawingObject);
			if (hyperlinkData != null) {
				DrawingShapeHyperlink property = new DrawingShapeHyperlink();
				property.HyperlinkData = hyperlinkData;
				artProperties.Properties.Add(property);
			}
			DrawingGroupShapeBooleanProperties groupShapeProps = new DrawingGroupShapeBooleanProperties();
			groupShapeProps.Hidden = drawingObject.Properties.Hidden;
			groupShapeProps.UseBehindDocument = false;
			groupShapeProps.IsBehindDoc = false;
			artProperties.Properties.Add(groupShapeProps);
		}
		byte[] GetPictureHyperlinkData(DrawingObject drawingObject) {
			if (string.IsNullOrEmpty(drawingObject.Properties.HyperlinkClickUrl))
				return null;
			XlsHyperlinkObject hyperlink = new XlsHyperlinkObject();
			string targetUri = drawingObject.Properties.HyperlinkClickUrl;
			if (drawingObject.Properties.HyperlinkClickIsExternal) {
				string location = string.Empty;
				int pos = targetUri.IndexOf('#');
				if (pos != -1) {
					location = targetUri.Substring(pos + 1);
					targetUri = targetUri.Substring(0, pos);
				}
				Uri uri;
				if (!Uri.TryCreate(targetUri, UriKind.RelativeOrAbsolute, out uri))
					return null;
				hyperlink.HasMoniker = true;
				hyperlink.IsAbsolute = uri.IsAbsoluteUri;
				if (uri.IsAbsoluteUri && uri.Scheme != "file") {
					XlsHyperlinkURLMoniker urlMoniker = new XlsHyperlinkURLMoniker();
					urlMoniker.Url = targetUri;
					urlMoniker.HasOptionalData = true;
					urlMoniker.AllowImplicitFileScheme = true;
					urlMoniker.AllowRelative = true;
					urlMoniker.Canonicalize = true;
					urlMoniker.CrackUnknownSchemes = true;
					urlMoniker.DecodeExtraInfo = true;
					urlMoniker.IESettings = true;
					urlMoniker.NoEncodeForbiddenChars = true;
					urlMoniker.PreProcessHtmlUri = true;
					hyperlink.OleMoniker = urlMoniker;
				}
				else {
					XlsHyperlinkFileMoniker fileMoniker = new XlsHyperlinkFileMoniker();
					fileMoniker.Path = targetUri;
					hyperlink.OleMoniker = fileMoniker;
				}
				if (!string.IsNullOrEmpty(location)) {
					hyperlink.Location = location;
					hyperlink.HasLocationString = true;
				}
			}
			else {
				hyperlink.Location = targetUri.TrimStart(new char[] { '#' });
				hyperlink.HasLocationString = true;
			}
			if (!string.IsNullOrEmpty(drawingObject.Properties.HyperlinkClickTargetFrame)) {
				hyperlink.FrameName = drawingObject.Properties.HyperlinkClickTargetFrame;
				hyperlink.HasFrameName = true;
			}
			return hyperlink.GetHyperlinkData();
		}
		BlackWhiteMode ConvertBlackWhiteMode(OpenXmlBlackWhiteMode mode) {
			switch (mode) {
				case OpenXmlBlackWhiteMode.Clr:
					return BlackWhiteMode.Normal;
				case OpenXmlBlackWhiteMode.Gray:
					return BlackWhiteMode.GrayScale;
				case OpenXmlBlackWhiteMode.LtGray:
					return BlackWhiteMode.LightGrayScale;
				case OpenXmlBlackWhiteMode.InvGray:
					return BlackWhiteMode.InverseGray;
				case OpenXmlBlackWhiteMode.GrayWhite:
					return BlackWhiteMode.GrayOutline;
				case OpenXmlBlackWhiteMode.BlackGray:
					return BlackWhiteMode.BlackTextLine;
				case OpenXmlBlackWhiteMode.BlackWhite:
					return BlackWhiteMode.HighContrast;
				case OpenXmlBlackWhiteMode.Black:
					return BlackWhiteMode.Black;
				case OpenXmlBlackWhiteMode.White:
					return BlackWhiteMode.White;
				case OpenXmlBlackWhiteMode.Hidden:
					return BlackWhiteMode.DontShow;
			}
			return BlackWhiteMode.Automatic;
		}
#endregion
#region Client anchor
		OfficeArtClientAnchorSheet CreateClientAnchor(IDrawingObject drawingObject, Action checkRotationAndSwapBoxMethod) {
			OfficeArtClientAnchorSheet clientAnchor = new OfficeArtClientAnchorSheet();
			switch(drawingObject.AnchorType) {
				case AnchorType.TwoCell:
					clientAnchor.KeepOnMove = drawingObject.ResizingBehavior == AnchorType.Absolute;
					clientAnchor.KeepOnResize = drawingObject.ResizingBehavior != AnchorType.TwoCell;
					break;
				case AnchorType.OneCell:
					clientAnchor.KeepOnMove = false;
					clientAnchor.KeepOnResize = true;
					break;
				default:
					clientAnchor.KeepOnMove = true;
					clientAnchor.KeepOnResize = true;
					break;
			}
			PageGridCalculator calculator = new PageGridCalculator(Sheet, Rectangle.Empty);
			checkRotationAndSwapBoxMethod();
			AnchorPoint point = drawingObject.From;
			clientAnchor.TopLeft = new CellPosition(point.CellKey.ColumnIndex, point.CellKey.RowIndex);
			clientAnchor.DeltaXLeft = CalculateDeltaX(calculator, clientAnchor.TopLeft.Column, point.ColOffset);
			clientAnchor.DeltaYTop = CalculateDeltaY(calculator, clientAnchor.TopLeft.Row, point.RowOffset);
			point = drawingObject.To;
			clientAnchor.BottomRight = new CellPosition(point.CellKey.ColumnIndex, point.CellKey.RowIndex);
			clientAnchor.DeltaXRight = CalculateDeltaX(calculator, clientAnchor.BottomRight.Column, point.ColOffset);
			clientAnchor.DeltaYBottom = CalculateDeltaY(calculator, clientAnchor.BottomRight.Row, point.RowOffset);
			checkRotationAndSwapBoxMethod();
			return clientAnchor;
		}
		int CalculateDeltaX(PageGridCalculator calculator, int columnIndex, float columnOffset) {
			float layoutColumnWidth = calculator.InnerCalculator.CalculateColumnWidth(Sheet, columnIndex, calculator.MaxDigitWidth, calculator.MaxDigitWidthInPixels);
			float modelUnitsColumnWidth = Sheet.Workbook.ToDocumentLayoutUnitConverter.ToModelUnits(layoutColumnWidth);
			return Math.Min(1023, (int)(columnOffset * 1024 / modelUnitsColumnWidth));
		}
		int CalculateDeltaY(PageGridCalculator calculator, int rowIndex, float rowOffset) {
			float layoutRowHeight = calculator.InnerCalculator.CalculateRowHeight(Sheet, rowIndex);
			float modelUnitsRowHeight = Sheet.Workbook.ToDocumentLayoutUnitConverter.ToModelUnits(layoutRowHeight);
			return Math.Min(255, (int)(rowOffset * 256 / modelUnitsRowHeight));
		}
#endregion
#region ClientData
		OfficeArtClientData CreateClientData(Picture picture) {
			OfficeArtClientData clientData = new OfficeArtClientData();
			XlsObjCommon common = new XlsObjCommon();
			common.ObjType = XlsObjType.Picture;
			common.ObjId = shapeIdsTable[picture.DrawingObject.Properties.Id];
			common.Locked = picture.LocksWithSheet;
			common.Print = picture.PrintsWithSheet;
			clientData.Data.Add(common);
			XlsObjPictureFormat format = new XlsObjPictureFormat();
			if (picture.Image.RawFormat == OfficeImageFormat.Bmp)
				format.PictureType = XlsObjPictureType.Bitmap;
			else if (picture.Image.RawFormat == OfficeImageFormat.Emf)
				format.PictureType = XlsObjPictureType.EnhancedMetafile;
			else
				format.PictureType = XlsObjPictureType.Unspecified;
			clientData.Data.Add(format);
			XlsObjPictureFlags flags = new XlsObjPictureFlags();
			clientData.Data.Add(flags);
			clientData.Data.Add(new XlsObjEnd());
			return clientData;
		}
#endregion
#endregion
#region Charts
#if EXPORT_CHARTS
		OfficeArtShapeContainer CreateShape(Chart chart) {
			OfficeArtShapeContainer shape = new OfficeArtShapeContainer();
			OfficeArtShapeRecord shapeRecord = CreateShapeRecord(chart);
			shape.Items.Add(shapeRecord);
			OfficeArtProperties artProperties = CreateProperties(chart);
			shape.Items.Add(artProperties);
			OfficeArtClientAnchorSheet clientAnchor = CreateClientAnchor(chart);
			shape.Items.Add(clientAnchor);
			OfficeArtClientData clientData = CreateClientData(chart);
			shape.Items.Add(clientData);
			return shape;
		}
		OfficeArtShapeRecord CreateShapeRecord(Chart chart) {
			OfficeArtShapeRecord shapeRecord = new OfficeArtShapeRecord(ShapeTypeCode.HostControl);
			shapeRecord.HaveAnchor = true;
			shapeRecord.HaveShapeType = true;
			shapeRecord.ShapeIdentifier = shapeIdsTable[chart.DrawingObject.Properties.Id];
			shapeRecord.FlipH = chart.ShapeProperties.Transform2D.FlipH;
			shapeRecord.FlipV = chart.ShapeProperties.Transform2D.FlipV;
			return shapeRecord;
		}
		OfficeArtProperties CreateProperties(Chart chart) {
			OfficeArtProperties artProperties = new OfficeArtProperties();
			CreateProtectionProperties(chart, artProperties);
			artProperties.Properties.Add(new DrawingFillStyleBooleanProperties());
			CreateGroupShapeProperties(chart, artProperties);
			return artProperties;
		}
		void CreateProtectionProperties(Chart chart, OfficeArtProperties artProperties) {
			DrawingBooleanProtectionProperties protection = new DrawingBooleanProtectionProperties();
			protection.LockGroup = chart.Locks.NoGroup;
			protection.LockAdjustHandles = chart.Locks.InnerLocks.NoAdjustHandles;
			protection.LockVertices = chart.Locks.InnerLocks.NoEditPoints;
			protection.LockCropping = chart.Locks.InnerLocks.NoCrop;
			protection.LockSelect = chart.Locks.NoSelect;
			protection.LockPosition = chart.Locks.NoMove;
			protection.LockAspectRatio = chart.Locks.NoChangeAspect;
			protection.UseLockGroup = true;
			protection.UseLockAdjustHandles = true;
			protection.UseLockVertices = true;
			protection.UseLockCropping = true;
			protection.UseLockSelect = true;
			protection.UseLockPosition = true;
			protection.UseLockAspectRatio = true;
			artProperties.Properties.Add(protection);
		}
		void CreateGroupShapeProperties(Chart chart, OfficeArtProperties artProperties) {
			if (!string.IsNullOrEmpty(chart.DrawingObject.Properties.Name)) {
				DrawingShapeName property = new DrawingShapeName();
				property.Data = String.Format("{0}\0", chart.DrawingObject.Properties.Name);
				artProperties.Properties.Add(property);
			}
			if (!string.IsNullOrEmpty(chart.DrawingObject.Properties.Description)) {
				DrawingShapeDescription property = new DrawingShapeDescription();
				property.Data = String.Format("{0}\0", chart.DrawingObject.Properties.Description);
				artProperties.Properties.Add(property);
			}
			DrawingGroupShapeBooleanProperties groupShapeProps = new DrawingGroupShapeBooleanProperties();
			groupShapeProps.Hidden = chart.DrawingObject.Properties.Hidden;
			artProperties.Properties.Add(groupShapeProps);
		}
		OfficeArtClientAnchorSheet CreateClientAnchor(Chart chart) {
			OfficeArtClientAnchorSheet clientAnchor = new OfficeArtClientAnchorSheet();
			if (chart.AnchorType == AnchorType.TwoCell) {
				clientAnchor.KeepOnMove = chart.ResizingBehavior == AnchorType.Absolute;
				clientAnchor.KeepOnResize = chart.ResizingBehavior != AnchorType.TwoCell;
			}
			else if (chart.AnchorType == AnchorType.OneCell) {
				clientAnchor.KeepOnMove = false;
				clientAnchor.KeepOnResize = true;
			}
			else {
				clientAnchor.KeepOnMove = true;
				clientAnchor.KeepOnResize = true;
			}
			PageGridCalculator calculator = new PageGridCalculator(Sheet, Rectangle.Empty);
			AnchorPoint point = chart.From;
			clientAnchor.TopLeft = new CellPosition(point.CellKey.ColumnIndex, point.CellKey.RowIndex);
			clientAnchor.DeltaXLeft = CalculateDeltaX(calculator, clientAnchor.TopLeft.Column, point.ColOffset);
			clientAnchor.DeltaYTop = CalculateDeltaY(calculator, clientAnchor.TopLeft.Row, point.RowOffset);
			point = chart.To;
			clientAnchor.BottomRight = new CellPosition(point.CellKey.ColumnIndex, point.CellKey.RowIndex);
			clientAnchor.DeltaXRight = CalculateDeltaX(calculator, clientAnchor.BottomRight.Column, point.ColOffset);
			clientAnchor.DeltaYBottom = CalculateDeltaY(calculator, clientAnchor.BottomRight.Row, point.RowOffset);
			return clientAnchor;
		}
		OfficeArtClientData CreateClientData(Chart chart) {
			OfficeArtClientData clientData = new OfficeArtClientData();
			XlsObjCommon common = new XlsObjCommon();
			common.ObjType = XlsObjType.Chart;
			common.ObjId = shapeIdsTable[chart.DrawingObject.Properties.Id];
			common.Locked = chart.LocksWithSheet;
			common.Print = chart.PrintsWithSheet;
			clientData.Data.Add(common);
			clientData.Data.Add(new XlsObjEnd());
			clientData.Data.ChartExporter = new XlsChartExporter(StreamWriter, chart, ExportStyleSheet);
			return clientData;
		}
#endif
#endregion
#region Comments
		OfficeArtShapeContainer CreateShape(Comment comment, int id) {
			OfficeArtShapeContainer shape = new OfficeArtShapeContainer();
			OfficeArtShapeRecord shapeRecord = CreateShapeRecord(id);
			shape.Items.Add(shapeRecord);
			OfficeArtProperties artProperties = CreateProperties(comment, id);
			shape.Items.Add(artProperties);
			OfficeArtClientAnchorSheet clientAnchor = CreateClientAnchor(comment);
			shape.Items.Add(clientAnchor);
			OfficeArtClientData clientData = CreateClientData(comment, id);
			shape.Items.Add(clientData);
			OfficeArtClientTextbox clientTextbox = CreateClientTextbox(comment, id);
			shape.Items.Add(clientTextbox);
			return shape;
		}
		OfficeArtShapeRecord CreateShapeRecord(int id) {
			OfficeArtShapeRecord shapeRecord = new OfficeArtShapeRecord(ShapeTypeCode.Textbox);
			shapeRecord.HaveAnchor = true;
			shapeRecord.HaveShapeType = true;
			shapeRecord.ShapeIdentifier = id;
			return shapeRecord;
		}
		OfficeArtProperties CreateProperties(Comment comment, int id) {
			OfficeArtProperties artProperties = new OfficeArtProperties();
			CreateTextProperties(comment, artProperties);
			CreateConnectionPointsProperties(comment, artProperties);
			CreateFillProperties(comment, artProperties);
			CreateStrokeProperties(comment, artProperties);
			CreateShadowProperties(comment, artProperties);
			CreateGroupShapeProperties(comment, artProperties);
			return artProperties;
		}
		void CreateTextProperties(Comment comment, OfficeArtProperties artProperties) {
			if (comment.Shape.InsetMode == VmlInsetMode.Custom) {
				VmlInsetData inset = comment.Shape.Textbox.Inset;
				DocumentModelUnitConverter unitConverter = comment.Workbook.UnitConverter;
				artProperties.Properties.Add(new DrawingTextLeft(unitConverter.ModelUnitsToEmu(inset.LeftMargin)));
				artProperties.Properties.Add(new DrawingTextTop(unitConverter.ModelUnitsToEmu(inset.TopMargin)));
				artProperties.Properties.Add(new DrawingTextRight(unitConverter.ModelUnitsToEmu(inset.RightMargin)));
				artProperties.Properties.Add(new DrawingTextBottom(unitConverter.ModelUnitsToEmu(inset.BottomMargin)));
			}
			OfficeTextReadingOrder textDir = OfficeTextReadingOrder.Context;
			if (comment.Shape.Textbox.TextDirection == XlReadingOrder.LeftToRight)
				textDir = OfficeTextReadingOrder.LeftToRight;
			else if (comment.Shape.Textbox.TextDirection == XlReadingOrder.RightToLeft)
				textDir = OfficeTextReadingOrder.RightToLeft;
			artProperties.Properties.Add(new DrawingTextDirection(textDir));
			DrawingTextBooleanProperties bpText = new DrawingTextBooleanProperties();
			bpText.Value = 0;
			bpText.AutoTextMargins = comment.Shape.InsetMode == VmlInsetMode.Auto;
			bpText.FitShapeToText = comment.Shape.Textbox.FitShapeToText;
			bpText.UseAutoTextMargins = true;
			bpText.UseFitShapeToText = true;
			artProperties.Properties.Add(bpText);
		}
		void CreateConnectionPointsProperties(Comment comment, OfficeArtProperties artProperties) {
			ConnectionPointsType type = ConnectionPointsType.None;
			switch (comment.Shape.Path.Connecttype) {
				case VmlConnectType.Segments:
					type = ConnectionPointsType.Segments;
					break;
				case VmlConnectType.Custom:
					type = ConnectionPointsType.Custom;
					break;
				case VmlConnectType.Rect:
					type = ConnectionPointsType.Rect;
					break;
			}
			artProperties.Properties.Add(new DrawingConnectionPointsType(type));
		}
		void CreateFillProperties(Comment comment, OfficeArtProperties artProperties) {
			artProperties.Properties.Add(new DrawingFillColor(comment.Shape.Fillcolor));
			if (comment.Shape.Fill.Opacity != 1.0)
				artProperties.Properties.Add(new DrawingFillOpacity(comment.Shape.Fill.Opacity));
			artProperties.Properties.Add(new DrawingFillBackColor(comment.Shape.Fill.Color2));
			DrawingFillStyleBooleanProperties bpFillStyle = new DrawingFillStyleBooleanProperties();
			bpFillStyle.Value = 0;
			bpFillStyle.Filled = comment.Shape.Filled.HasValue && comment.Shape.Filled.Value;
			bpFillStyle.UseFilled = true;
			artProperties.Properties.Add(bpFillStyle);
		}
		void CreateStrokeProperties(Comment comment, OfficeArtProperties artProperties) {
			if (comment.Shape.Strokecolor != DXColor.Black)
				artProperties.Properties.Add(new DrawingLineColor(comment.Shape.Strokecolor));
			int lineWidthInEmu = comment.Workbook.UnitConverter.ModelUnitsToEmu(comment.Shape.Strokeweight);
			if (lineWidthInEmu != OfficeArtProperties.DefaultLineWidthInEmu)
				artProperties.Properties.Add(new DrawingLineWidth(lineWidthInEmu));
			DrawingLineStyleBooleanProperties bpLineStyle = new DrawingLineStyleBooleanProperties();
			bpLineStyle.Line = comment.Shape.Stroked;
			bpLineStyle.UseLine = true;
			artProperties.Properties.Add(bpLineStyle);
		}
		void CreateShadowProperties(Comment comment, OfficeArtProperties artProperties) {
			artProperties.Properties.Add(new DrawingShadowColor(comment.Shape.Shadow.Color));
			DrawingShadowStyleBooleanProperties bpShadowStyle = new DrawingShadowStyleBooleanProperties();
			bpShadowStyle.Value = 0;
			bpShadowStyle.Shadow = comment.Shape.Shadow.On;
			bpShadowStyle.ShadowObscured = comment.Shape.Shadow.Obscured;
			bpShadowStyle.UseShadow = true;
			bpShadowStyle.UseShadowObscured = true;
			artProperties.Properties.Add(bpShadowStyle);
		}
		void CreateGroupShapeProperties(Comment comment, OfficeArtProperties artProperties) {
			DrawingGroupShapeBooleanProperties bpGroupShape = new DrawingGroupShapeBooleanProperties();
			bpGroupShape.Value = 0;
			bpGroupShape.Hidden = comment.Shape.IsHidden;
			bpGroupShape.UseHidden = true;
			artProperties.Properties.Add(bpGroupShape);
		}
		OfficeArtClientAnchorSheet CreateClientAnchor(Comment comment) {
			OfficeArtClientAnchorSheet clientAnchor = new OfficeArtClientAnchorSheet();
			VmlAnchorData anchorData = comment.Shape.ClientData.Anchor;
			PageGridCalculator calculator = new PageGridCalculator(Sheet, Rectangle.Empty);
			clientAnchor.KeepOnMove = !comment.Shape.ClientData.MoveWithCells;
			clientAnchor.KeepOnResize = !comment.Shape.ClientData.SizeWithCells;
			clientAnchor.TopLeft = new CellPosition(anchorData.LeftColumn, anchorData.TopRow);
			clientAnchor.BottomRight = new CellPosition(anchorData.RightColumn, anchorData.BottomRow);
			int columnOffset = comment.Workbook.UnitConverter.PixelsToModelUnits(anchorData.LeftOffset, DocumentModel.DpiX);
			int rowOffset = comment.Workbook.UnitConverter.PixelsToModelUnits(anchorData.TopOffset, DocumentModel.DpiY);
			clientAnchor.DeltaXLeft = CalculateDeltaX(calculator, clientAnchor.TopLeft.Column, columnOffset);
			clientAnchor.DeltaYTop = CalculateDeltaY(calculator, clientAnchor.TopLeft.Row, rowOffset);
			columnOffset = comment.Workbook.UnitConverter.PixelsToModelUnits(anchorData.RightOffset, DocumentModel.DpiX);
			rowOffset = comment.Workbook.UnitConverter.PixelsToModelUnits(anchorData.BottomOffset, DocumentModel.DpiY);
			clientAnchor.DeltaXRight = CalculateDeltaX(calculator, clientAnchor.BottomRight.Column, columnOffset);
			clientAnchor.DeltaYBottom = CalculateDeltaY(calculator, clientAnchor.BottomRight.Row, rowOffset);
			return clientAnchor;
		}
		OfficeArtClientData CreateClientData(Comment comment, int id) {
			OfficeArtClientData clientData = new OfficeArtClientData();
			XlsObjCommon common = new XlsObjCommon();
			common.ObjType = XlsObjType.Note;
			common.ObjId = id;
			common.Locked = comment.Shape.ClientData.Locked;
			common.Print = true;
			clientData.Data.Add(common);
			XlsObjNote note = new XlsObjNote();
			note.NoteGuid = Guid.NewGuid();
			note.IsSharedNote = false;
			clientData.Data.Add(note);
			clientData.Data.Add(new XlsObjEnd());
			return clientData;
		}
		OfficeArtClientTextbox CreateClientTextbox(Comment comment, int id) {
			OfficeArtClientTextbox clientTextbox = new OfficeArtClientTextbox();
			XlsTextObjData textData = clientTextbox.Data;
			int charIndex = 0;
			StringBuilder sb = new StringBuilder();
			foreach (CommentRun part in comment.Runs) {
				XlsFormatRun formatRun = new XlsFormatRun();
				formatRun.CharIndex = charIndex;
				int fontIndex = DocumentModel.Cache.FontInfoCache.GetItemIndex(part.Info);
				int runFontIndex = ExportStyleSheet.GetFontIndex(fontIndex);
				if (runFontIndex >= XlsDefs.UnusedFontIndex)
					runFontIndex++;
				formatRun.FontIndex = runFontIndex;
				textData.FormatRuns.Add(formatRun);
				charIndex += part.Text.Length;
				sb.Append(part.Text);
			}
			textData.Text = sb.ToString();
			textData.IsLocked = comment.Shape.ClientData.LockText;
			textData.HorizontalAlignment = comment.Shape.ClientData.TextHAlign;
			textData.VerticalAlignment = comment.Shape.ClientData.TextVAlign;
			return clientTextbox;
		}
#endregion
#region ModelShapes
		OfficeArtShapeContainer CreateShape(ModelShape modelShape) {
			OfficeArtShapeContainer shape = new OfficeArtShapeContainer();
			OfficeArtShapeRecord shapeRecord = CreateShapeRecord(modelShape);
			shape.Items.Add(shapeRecord);
			OfficeArtProperties artProperties = CreateProperties(modelShape);
			if(shapeRecord.HeaderInstanceInfo == 0) {
				CreateCustomGeometryProperties(modelShape, artProperties);
			}
			shape.Items.Add(artProperties);
			OfficeArtTertiaryProperties artTertiaryProperties = CreateTertiaryProperties(modelShape);
			if(artTertiaryProperties != null)
				shape.Items.Add(artTertiaryProperties);
			OfficeArtClientAnchorSheet clientAnchor = CreateClientAnchor(modelShape, modelShape.CheckRotationAndSwapBox);
			shape.Items.Add(clientAnchor);
			OfficeArtClientData clientData = CreateClientData(modelShape);
			shape.Items.Add(clientData);
			return shape;
		}
		void CreateCustomGeometryProperties(ModelShape modelShape, OfficeArtProperties artProperties) {
			CreateGeometrySpaceProperties(modelShape, artProperties);
			CreateComplexPath(modelShape, artProperties);
		}
		static void CreateComplexPath(ModelShape modelShape, OfficeArtProperties artProperties) {
			artProperties.Properties.Add(new DrawingGeometryShapePath {ShapePath = ShapePathType.Complex});
		}
		static void CreateGeometrySpaceProperties(ModelShape modelShape, OfficeArtProperties artProperties) {
			int geometrySpaceLeft = 0;
			int geometrySpaceRight = modelShape.DocumentModel.UnitConverter.ModelUnitsToEmuF(Math.Max(0, modelShape.Width));
			int geometrySpaceTop = 0;
			int geometrySpaceBottom = modelShape.DocumentModel.UnitConverter.ModelUnitsToEmuF(Math.Max(0, modelShape.Height));
			artProperties.Properties.Add(new DrawingGeometryLeft {Value = geometrySpaceLeft});
			artProperties.Properties.Add(new DrawingGeometryRight {Value = geometrySpaceRight});
			artProperties.Properties.Add(new DrawingGeometryTop {Value = geometrySpaceTop});
			artProperties.Properties.Add(new DrawingGeometryBottom {Value = geometrySpaceBottom});
		}
		#region Shape record
		OfficeArtShapeRecord CreateShapeRecord(ModelShape modelShape) {
			OfficeArtShapeRecord shapeRecord = new OfficeArtShapeRecord(GetShapeType(modelShape));
			shapeRecord.HaveAnchor = true;
			shapeRecord.HaveShapeType = true;
			shapeRecord.ShapeIdentifier = shapeIdsTable[modelShape.DrawingObject.Properties.Id];
			shapeRecord.FlipH = modelShape.ShapeProperties.Transform2D.FlipH;
			shapeRecord.FlipV = modelShape.ShapeProperties.Transform2D.FlipV;
			return shapeRecord;
		}
		static int GetShapeType(ModelShape shape) {
			ShapeType shapeType = shape.ShapeProperties.ShapeType;
			switch(shapeType) {
				case ShapeType.Rect:
					return ShapeTypeCode.Rectangle;
				case ShapeType.RoundRect:
					return ShapeTypeCode.RoundRect;
				case ShapeType.Ellipse:
					return ShapeTypeCode.Ellipse;
				case ShapeType.Diamond:
					return ShapeTypeCode.Diamond;
				case ShapeType.Triangle:
					return ShapeTypeCode.IsocelesTriangle;
				case ShapeType.RtTriangle:
					return ShapeTypeCode.RightTriangle;
				case ShapeType.Parallelogram:
					return ShapeTypeCode.Parallelogram;
				case ShapeType.Trapezoid:
					return ShapeTypeCode.Trapezoid;
				case ShapeType.Hexagon:
					return ShapeTypeCode.Hexagon;
				case ShapeType.Octagon:
					return ShapeTypeCode.Octagon;
				case ShapeType.Plus:
					return ShapeTypeCode.Plus;
				case ShapeType.Star5:
					return ShapeTypeCode.Star;
				case ShapeType.RightArrow:
					return ShapeTypeCode.Arrow;
				case ShapeType.HomePlate:
					return ShapeTypeCode.HomePlate;
				case ShapeType.Cube:
					return ShapeTypeCode.Cube;
				case ShapeType.Star16:
					return ShapeTypeCode.Seal;
				case ShapeType.Arc:
					return ShapeTypeCode.Arc;
				case ShapeType.Line:
					return ShapeTypeCode.Line;
				case ShapeType.Plaque:
					return ShapeTypeCode.Plaque;
				case ShapeType.Can:
					return ShapeTypeCode.Can;
				case ShapeType.Donut:
					return ShapeTypeCode.Donut;
				case ShapeType.StraightConnector1:
					return ShapeTypeCode.StraightConnector1;
				case ShapeType.BentConnector2:
					return ShapeTypeCode.BentConnector2;
				case ShapeType.BentConnector3:
					return ShapeTypeCode.BentConnector3;
				case ShapeType.BentConnector4:
					return ShapeTypeCode.BentConnector4;
				case ShapeType.BentConnector5:
					return ShapeTypeCode.BentConnector5;
				case ShapeType.CurvedConnector2:
					return ShapeTypeCode.CurvedConnector2;
				case ShapeType.CurvedConnector3:
					return ShapeTypeCode.CurvedConnector3;
				case ShapeType.CurvedConnector4:
					return ShapeTypeCode.CurvedConnector4;
				case ShapeType.CurvedConnector5:
					return ShapeTypeCode.CurvedConnector5;
				case ShapeType.Callout1:
					return ShapeTypeCode.Callout1;
				case ShapeType.Callout2:
					return ShapeTypeCode.Callout2;
				case ShapeType.Callout3:
					return ShapeTypeCode.Callout3;
				case ShapeType.AccentCallout1:
					return ShapeTypeCode.AccentCallout1;
				case ShapeType.AccentCallout2:
					return ShapeTypeCode.AccentCallout2;
				case ShapeType.AccentCallout3:
					return ShapeTypeCode.AccentCallout3;
				case ShapeType.BorderCallout1:
					return ShapeTypeCode.BorderCallout1;
				case ShapeType.BorderCallout2:
					return ShapeTypeCode.BorderCallout2;
				case ShapeType.BorderCallout3:
					return ShapeTypeCode.BorderCallout3;
				case ShapeType.AccentBorderCallout1:
					return ShapeTypeCode.AccentBorderCallout1;
				case ShapeType.AccentBorderCallout2:
					return ShapeTypeCode.AccentBorderCallout2;
				case ShapeType.AccentBorderCallout3:
					return ShapeTypeCode.AccentBorderCallout3;
				case ShapeType.Ribbon:
					return ShapeTypeCode.Ribbon;
				case ShapeType.Ribbon2:
					return ShapeTypeCode.Ribbon2;
				case ShapeType.Chevron:
					return ShapeTypeCode.Chevron;
				case ShapeType.Pentagon:
					return ShapeTypeCode.Pentagon;
				case ShapeType.NoSmoking:
					return ShapeTypeCode.NoSmoking;
				case ShapeType.Star8:
					return ShapeTypeCode.Seal8;
				case ShapeType.Star32:
					return ShapeTypeCode.Seal32;
				case ShapeType.WedgeRectCallout:
					return ShapeTypeCode.WedgeRectCallout;
				case ShapeType.WedgeRoundRectCallout:
					return ShapeTypeCode.WedgeRRectCallout;
				case ShapeType.WedgeEllipseCallout:
					return ShapeTypeCode.WedgeEllipseCallout;
				case ShapeType.Wave:
					return ShapeTypeCode.Wave;
				case ShapeType.FoldedCorner:
					return ShapeTypeCode.FoldedCorner;
				case ShapeType.LeftArrow:
					return ShapeTypeCode.LeftArrow;
				case ShapeType.DownArrow:
					return ShapeTypeCode.DownArrow;
				case ShapeType.UpArrow:
					return ShapeTypeCode.UpArrow;
				case ShapeType.LeftRightArrow:
					return ShapeTypeCode.LeftRightArrow;
				case ShapeType.UpDownArrow:
					return ShapeTypeCode.UpDownArrow;
				case ShapeType.IrregularSeal1:
					return ShapeTypeCode.IrregularSeal1;
				case ShapeType.IrregularSeal2:
					return ShapeTypeCode.IrregularSeal2;
				case ShapeType.LightningBolt:
					return ShapeTypeCode.LightningBolt;
				case ShapeType.Heart:
					return ShapeTypeCode.Heart;
				case ShapeType.QuadArrow:
					return ShapeTypeCode.QuadArrow;
				case ShapeType.LeftArrowCallout:
					return ShapeTypeCode.LeftArrowCallout;
				case ShapeType.RightArrowCallout:
					return ShapeTypeCode.RightArrowCallout;
				case ShapeType.UpArrowCallout:
					return ShapeTypeCode.UpArrowCallout;
				case ShapeType.DownArrowCallout:
					return ShapeTypeCode.DownArrowCallout;
				case ShapeType.LeftRightArrowCallout:
					return ShapeTypeCode.LeftRightArrowCallout;
				case ShapeType.UpDownArrowCallout:
					return ShapeTypeCode.UpDownArrowCallout;
				case ShapeType.QuadArrowCallout:
					return ShapeTypeCode.QuadArrowCallout;
				case ShapeType.Bevel:
					return ShapeTypeCode.Bevel;
				case ShapeType.LeftBracket:
					return ShapeTypeCode.LeftBracket;
				case ShapeType.RightBracket:
					return ShapeTypeCode.RightBracket;
				case ShapeType.LeftBrace:
					return ShapeTypeCode.LeftBrace;
				case ShapeType.RightBrace:
					return ShapeTypeCode.RightBrace;
				case ShapeType.LeftUpArrow:
					return ShapeTypeCode.LeftUpArrow;
				case ShapeType.BentUpArrow:
					return ShapeTypeCode.BentUpArrow;
				case ShapeType.BentArrow:
					return ShapeTypeCode.BentArrow;
				case ShapeType.Star24:
					return ShapeTypeCode.Seal24;
				case ShapeType.StripedRightArrow:
					return ShapeTypeCode.StripedRightArrow;
				case ShapeType.NotchedRightArrow:
					return ShapeTypeCode.NotchedRightArrow;
				case ShapeType.BlockArc:
					return ShapeTypeCode.BlockArc;
				case ShapeType.SmileyFace:
					return ShapeTypeCode.SmileyFace;
				case ShapeType.VerticalScroll:
					return ShapeTypeCode.VerticalScroll;
				case ShapeType.HorizontalScroll:
					return ShapeTypeCode.HorizontalScroll;
				case ShapeType.CircularArrow:
					return ShapeTypeCode.CircularArrow;
				case ShapeType.UturnArrow:
					return ShapeTypeCode.UturnArrow;
				case ShapeType.CurvedRightArrow:
					return ShapeTypeCode.CurvedRightArrow;
				case ShapeType.CurvedLeftArrow:
					return ShapeTypeCode.CurvedLeftArrow;
				case ShapeType.CurvedUpArrow:
					return ShapeTypeCode.CurvedUpArrow;
				case ShapeType.CurvedDownArrow:
					return ShapeTypeCode.CurvedDownArrow;
				case ShapeType.CloudCallout:
					return ShapeTypeCode.CloudCallout;
				case ShapeType.EllipseRibbon:
					return ShapeTypeCode.EllipseRibbon;
				case ShapeType.EllipseRibbon2:
					return ShapeTypeCode.EllipseRibbon2;
				case ShapeType.FlowChartProcess:
					return ShapeTypeCode.FlowChartProcess;
				case ShapeType.FlowChartDecision:
					return ShapeTypeCode.FlowChartDecision;
				case ShapeType.FlowChartInputOutput:
					return ShapeTypeCode.FlowChartInputOutput;
				case ShapeType.FlowChartPredefinedProcess:
					return ShapeTypeCode.FlowChartPredefinedProcess;
				case ShapeType.FlowChartInternalStorage:
					return ShapeTypeCode.FlowChartInternalStorage;
				case ShapeType.FlowChartDocument:
					return ShapeTypeCode.FlowChartDocument;
				case ShapeType.FlowChartMultidocument:
					return ShapeTypeCode.FlowChartMultidocument;
				case ShapeType.FlowChartTerminator:
					return ShapeTypeCode.FlowChartTerminator;
				case ShapeType.FlowChartPreparation:
					return ShapeTypeCode.FlowChartPreparation;
				case ShapeType.FlowChartManualInput:
					return ShapeTypeCode.FlowChartManualInput;
				case ShapeType.FlowChartManualOperation:
					return ShapeTypeCode.FlowChartManualOperation;
				case ShapeType.FlowChartConnector:
					return ShapeTypeCode.FlowChartConnector;
				case ShapeType.FlowChartPunchedCard:
					return ShapeTypeCode.FlowChartPunchedCard;
				case ShapeType.FlowChartPunchedTape:
					return ShapeTypeCode.FlowChartPunchedTape;
				case ShapeType.FlowChartSummingJunction:
					return ShapeTypeCode.FlowChartSummingJunction;
				case ShapeType.FlowChartOr:
					return ShapeTypeCode.FlowChartOr;
				case ShapeType.FlowChartCollate:
					return ShapeTypeCode.FlowChartCollate;
				case ShapeType.FlowChartSort:
					return ShapeTypeCode.FlowChartSort;
				case ShapeType.FlowChartExtract:
					return ShapeTypeCode.FlowChartExtract;
				case ShapeType.FlowChartMerge:
					return ShapeTypeCode.FlowChartMerge;
				case ShapeType.FlowChartOfflineStorage:
					return ShapeTypeCode.FlowChartOfflineStorage;
				case ShapeType.FlowChartOnlineStorage:
					return ShapeTypeCode.FlowChartOnlineStorage;
				case ShapeType.FlowChartMagneticTape:
					return ShapeTypeCode.FlowChartMagneticTape;
				case ShapeType.FlowChartMagneticDisk:
					return ShapeTypeCode.FlowChartMagneticDisk;
				case ShapeType.FlowChartMagneticDrum:
					return ShapeTypeCode.FlowChartMagneticDrum;
				case ShapeType.FlowChartDisplay:
					return ShapeTypeCode.FlowChartDisplay;
				case ShapeType.FlowChartDelay:
					return ShapeTypeCode.FlowChartDelay;
				case ShapeType.FlowChartAlternateProcess:
					return ShapeTypeCode.FlowChartAlternateProcess;
				case ShapeType.FlowChartOffpageConnector:
					return ShapeTypeCode.FlowChartOffpageConnector;
				case ShapeType.LeftRightUpArrow:
					return ShapeTypeCode.LeftRightUpArrow;
				case ShapeType.Sun:
					return ShapeTypeCode.Sun;
				case ShapeType.Moon:
					return ShapeTypeCode.Moon;
				case ShapeType.BracketPair:
					return ShapeTypeCode.BracketPair;
				case ShapeType.BracePair:
					return ShapeTypeCode.BracePair;
				case ShapeType.Star4:
					return ShapeTypeCode.Seal4;
				case ShapeType.DoubleWave:
					return ShapeTypeCode.DoubleWave;
				case ShapeType.ActionButtonBlank:
					return ShapeTypeCode.ActionButtonBlank;
				case ShapeType.ActionButtonHome:
					return ShapeTypeCode.ActionButtonHome;
				case ShapeType.ActionButtonHelp:
					return ShapeTypeCode.ActionButtonHelp;
				case ShapeType.ActionButtonInformation:
					return ShapeTypeCode.ActionButtonInformation;
				case ShapeType.ActionButtonForwardNext:
					return ShapeTypeCode.ActionButtonForwardNext;
				case ShapeType.ActionButtonBackPrevious:
					return ShapeTypeCode.ActionButtonBackPrevious;
				case ShapeType.ActionButtonEnd:
					return ShapeTypeCode.ActionButtonEnd;
				case ShapeType.ActionButtonBeginning:
					return ShapeTypeCode.ActionButtonBeginning;
				case ShapeType.ActionButtonReturn:
					return ShapeTypeCode.ActionButtonReturn;
				case ShapeType.ActionButtonDocument:
					return ShapeTypeCode.ActionButtonDocument;
				case ShapeType.ActionButtonSound:
					return ShapeTypeCode.ActionButtonSound;
				case ShapeType.ActionButtonMovie:
					return ShapeTypeCode.ActionButtonMovie;
			}
			return ShapeTypeCode.Rectangle;
		}
		#endregion
		#region ModelShape properties
		OfficeArtProperties CreateProperties(ModelShape modelShape) {
			OfficeArtProperties artProperties = new OfficeArtProperties();
			CreateTextProperties(modelShape, artProperties);
			ModelShapeExportFillHelper.CreateFillProperties(modelShape.ShapeProperties.Fill, artProperties);
			CreateTransformProperties(modelShape.ShapeProperties.Transform2D, artProperties);
			CreateProtectionProperties(modelShape, artProperties);
			CreateFillStyleBooleanProperties(modelShape, artProperties);
			CreateOutlineProperties(modelShape.ShapeProperties, artProperties);
			CreateShapeProperties(modelShape, artProperties);
			CreateGroupShapeProperties(modelShape.DrawingObject, artProperties);
			return artProperties;
		}
		void CreateFillStyleBooleanProperties(ModelShape modelShape, OfficeArtProperties artProperties) {
			DrawingFillStyleBooleanProperties fillStyleBooleanProperties = new DrawingFillStyleBooleanProperties();
			fillStyleBooleanProperties.UseFilled = true;
			if(modelShape.ShapeProperties.Fill.FillType == DrawingFillType.Automatic || modelShape.ShapeProperties.Fill.FillType == DrawingFillType.None) {
				fillStyleBooleanProperties.Filled = false;
			}
			else {
				fillStyleBooleanProperties.Filled = true;
			}
			artProperties.Properties.Add(fillStyleBooleanProperties);
		}
		void CreateTextProperties(ModelShape modelShape, OfficeArtProperties artProperties) {
			DrawingTextInset inset = modelShape.TextProperties.BodyProperties.Inset;
			if (!inset.IsDefault) {
				DocumentModelUnitConverter unitConverter = modelShape.DocumentModel.UnitConverter;
				artProperties.Properties.Add(new DrawingTextLeft(unitConverter.ModelUnitsToEmu(inset.Left)));
				artProperties.Properties.Add(new DrawingTextTop(unitConverter.ModelUnitsToEmu(inset.Top)));
				artProperties.Properties.Add(new DrawingTextRight(unitConverter.ModelUnitsToEmu(inset.Right)));
				artProperties.Properties.Add(new DrawingTextBottom(unitConverter.ModelUnitsToEmu(inset.Bottom)));				
			}
			DrawingTextBooleanProperties bpText = new DrawingTextBooleanProperties();
			bpText.Value = 0;
			bpText.AutoTextMargins = true;
			bpText.SelectText = true;
			bpText.FitShapeToText = modelShape.TextProperties.BodyProperties.AutoFit.Type == DrawingTextAutoFitType.Shape;
			bpText.UseAutoTextMargins = true;
			bpText.UseFitShapeToText = true;
			bpText.UseSelectText = true;
			artProperties.Properties.Add(bpText);
		}
		void CreateProtectionProperties(ModelShape modelShape, OfficeArtProperties artProperties) {
			if(modelShape.Locks.IsEmpty)
				return;
			DrawingBooleanProtectionProperties protection = new DrawingBooleanProtectionProperties();
			protection.LockText = modelShape.Locks.NoTextEdit;
			protection.UseLockText = true;
			CreateCommonDrawingProtectionProperties(modelShape.Locks, protection);
			artProperties.Properties.Add(protection);
		}
		void CreateCommonDrawingProtectionProperties(IDrawingLocks locks, DrawingBooleanProtectionProperties protectionProperties) {
			protectionProperties.LockGroup = locks.NoGroup;
			protectionProperties.LockAdjustHandles = locks.NoAdjustHandles;
			protectionProperties.LockVertices = locks.NoEditPoints;
			protectionProperties.LockSelect = locks.NoSelect;
			protectionProperties.LockPosition = locks.NoMove;
			protectionProperties.LockAspectRatio = locks.NoChangeAspect;
			protectionProperties.LockRotation = locks.NoRotate;
			protectionProperties.UseLockGroup = true;
			protectionProperties.UseLockAdjustHandles = true;
			protectionProperties.UseLockVertices = true;
			protectionProperties.UseLockCropping = true;
			protectionProperties.UseLockSelect = true;
			protectionProperties.UseLockPosition = true;
			protectionProperties.UseLockAspectRatio = true;
			protectionProperties.UseLockRotation = true;
		}
		void CreateShapeProperties(ModelShape modelShape, OfficeArtProperties artProperties) {
			DrawingShapeBooleanProperties shapeBoolProps = new DrawingShapeBooleanProperties();
			shapeBoolProps.Value = 0;
			shapeBoolProps.UseLockShapeType = true;
			shapeBoolProps.UsePreferRelativeResize = true;
			shapeBoolProps.LockShapeType = modelShape.Locks.NoChangeShapeType;
			artProperties.Properties.Add(shapeBoolProps);
			if(modelShape.ShapeProperties.BlackAndWhiteMode != OpenXmlBlackWhiteMode.Auto) {
				DrawingBlackWhiteMode drawingBlackWhiteMode = new DrawingBlackWhiteMode();
				drawingBlackWhiteMode.Mode = ConvertBlackWhiteMode(modelShape.ShapeProperties.BlackAndWhiteMode);
				artProperties.Properties.Add(drawingBlackWhiteMode);
			}
		}
		OfficeArtTertiaryProperties CreateTertiaryProperties(ModelShape modelShape) {
			OfficeArtTertiaryProperties artProperties = CreateTertiaryProperties(modelShape.DrawingObject);
			if(artProperties == null)
				return null;
			CreateLineStyleBooleanProperties(modelShape, artProperties);
			return artProperties;
		}
		void CreateLineStyleBooleanProperties(ModelShape modelShape, OfficeArtTertiaryProperties artProperties) {
		}
		#endregion
		#region ClientData
		OfficeArtClientData CreateClientData(ModelShape modelShape) {
			OfficeArtClientData clientData = new OfficeArtClientData();
			XlsObjCommon common = new XlsObjCommon();
			common.ObjType = GetObjectType(modelShape.ShapeProperties.ShapeType);
			common.ObjId = shapeIdsTable[modelShape.DrawingObject.Properties.Id];
			common.Locked = modelShape.LocksWithSheet;
			common.Print = modelShape.PrintsWithSheet;
			clientData.Data.Add(common);
			clientData.Data.Add(new XlsObjEnd());
			return clientData;
		}
		static XlsObjType GetObjectType(ShapeType shapeType) {
			switch(shapeType) {
				case ShapeType.Rect:
					return XlsObjType.Rectangle;
				case ShapeType.Parallelogram:
				case ShapeType.Diamond:
				case ShapeType.RoundRect:
				case ShapeType.Octagon:
				case ShapeType.Triangle:
				case ShapeType.RtTriangle:
				case ShapeType.Hexagon:
				case ShapeType.Plus:
				case ShapeType.Pentagon:
				case ShapeType.Can:
				case ShapeType.Cube:
				case ShapeType.Bevel:
				case ShapeType.FoldedCorner:
				case ShapeType.SmileyFace:
				case ShapeType.LightningBolt:
				case ShapeType.Sun:
				case ShapeType.Moon:
				case ShapeType.BracketPair:
				case ShapeType.BracePair:
				case ShapeType.Plaque:
				case ShapeType.LeftBracket:
				case ShapeType.RightBracket:
				case ShapeType.LeftBrace:
				case ShapeType.RightBrace:
				case ShapeType.RightArrow:
				case ShapeType.LeftArrow:
				case ShapeType.UpArrow:
				case ShapeType.DownArrow:
				case ShapeType.LeftRightArrow:
				case ShapeType.UpDownArrow:
				case ShapeType.CurvedRightArrow:
				case ShapeType.CurvedLeftArrow:
				case ShapeType.CurvedUpArrow:
				case ShapeType.CurvedDownArrow:
				case ShapeType.NotchedRightArrow:
				case ShapeType.HomePlate:
				case ShapeType.Chevron:
				case ShapeType.RightArrowCallout:
				case ShapeType.LeftArrowCallout:
				case ShapeType.UpArrowCallout:
				case ShapeType.DownArrowCallout:
				case ShapeType.LeftRightArrowCallout:
				case ShapeType.UpDownArrowCallout:
				case ShapeType.FlowChartProcess:
				case ShapeType.FlowChartAlternateProcess:
				case ShapeType.FlowChartDecision:
				case ShapeType.FlowChartInputOutput:
				case ShapeType.FlowChartPredefinedProcess:
				case ShapeType.FlowChartInternalStorage:
				case ShapeType.FlowChartDocument:
				case ShapeType.FlowChartMultidocument:
				case ShapeType.FlowChartTerminator:
				case ShapeType.FlowChartPreparation:
				case ShapeType.FlowChartManualInput:
				case ShapeType.FlowChartManualOperation:
				case ShapeType.FlowChartConnector:
				case ShapeType.FlowChartOffpageConnector:
				case ShapeType.FlowChartPunchedCard:
				case ShapeType.FlowChartPunchedTape:
				case ShapeType.FlowChartSummingJunction:
				case ShapeType.FlowChartOr:
				case ShapeType.FlowChartCollate:
				case ShapeType.FlowChartSort:
				case ShapeType.FlowChartExtract:
				case ShapeType.FlowChartMerge:
				case ShapeType.FlowChartOnlineStorage:
				case ShapeType.FlowChartDelay:
				case ShapeType.FlowChartMagneticTape:
				case ShapeType.FlowChartMagneticDisk:
				case ShapeType.FlowChartMagneticDrum:
				case ShapeType.FlowChartDisplay:
				case ShapeType.IrregularSeal1:
				case ShapeType.IrregularSeal2:
				case ShapeType.Star4:
				case ShapeType.Star8:
				case ShapeType.Star16:
				case ShapeType.Star24:
				case ShapeType.Star32:
				case ShapeType.Ribbon2:
				case ShapeType.Ribbon:
				case ShapeType.EllipseRibbon2:
				case ShapeType.EllipseRibbon:
				case ShapeType.VerticalScroll:
				case ShapeType.HorizontalScroll:
				case ShapeType.Wave:
				case ShapeType.DoubleWave:
				case ShapeType.WedgeRectCallout:
				case ShapeType.WedgeEllipseCallout:
				case ShapeType.CloudCallout:
				case ShapeType.BorderCallout2:
				case ShapeType.BorderCallout3:
				case ShapeType.AccentCallout1:
				case ShapeType.AccentCallout2:
				case ShapeType.AccentCallout3:
				case ShapeType.Callout2:
				case ShapeType.Callout3:
				case ShapeType.BorderCallout1:
				case ShapeType.AccentBorderCallout1:
				case ShapeType.AccentBorderCallout2:
				case ShapeType.AccentBorderCallout3:
				case ShapeType.ActionButtonBlank:
				case ShapeType.ActionButtonHome:
				case ShapeType.ActionButtonHelp:
				case ShapeType.ActionButtonInformation:
				case ShapeType.ActionButtonBackPrevious:
				case ShapeType.ActionButtonForwardNext:
				case ShapeType.ActionButtonBeginning:
				case ShapeType.ActionButtonEnd:
				case ShapeType.ActionButtonReturn:
				case ShapeType.ActionButtonDocument:
				case ShapeType.ActionButtonSound:
				case ShapeType.ActionButtonMovie:
				case ShapeType.WedgeRoundRectCallout:
				case ShapeType.FlowChartOfflineStorage:
					return XlsObjType.OfficeArtObject;
				case ShapeType.Ellipse:
					return XlsObjType.Oval;
				default:
					return XlsObjType.Polygon;
			}
		}
		#endregion
		#endregion
#endregion
#region Notes
		protected void WriteNotes() {
			CommentCollection comments = Sheet.Comments;
			int pictureCount = Sheet.DrawingObjects.GetPictureCount();
			int chartCount = Sheet.DrawingObjects.GetChartCount();
			int shapesCount = Sheet.DrawingObjects.GetShapesCount();
			if (!Sheet.Workbook.DocumentCapabilities.PicturesAllowed)
				pictureCount = 0;
			if (!Sheet.Workbook.DocumentCapabilities.ChartsAllowed)
				chartCount = 0;
			if(!Sheet.Workbook.DocumentCapabilities.ShapesAllowed)
				shapesCount = 0;
			int commentCount = comments.Count;
			int topmostShapeId = Sheet.GetTopmostShapeId();
			int id = topmostShapeId + pictureCount + chartCount + shapesCount + 1;
			for (int i = 0; i < commentCount; i++) {
				if (comments[i].Reference.OutOfLimits())
					continue;
				WriteNote(comments[i], id++);
			}
		}
		void WriteNote(Comment comment, int id) {
			XlsCommandNote command = new XlsCommandNote();
			command.Position = comment.Reference;
			command.ObjId = id;
			command.Author = DocumentModel.CommentAuthors[comment.AuthorId];
			command.Write(StreamWriter);
		}
#endregion
#region PivotTables
		protected void WritePivotTables() {
			foreach (PivotTable pivotTable in Sheet.PivotTables) {
				if (!mainExporter.ExcludedPivotCaches.Contains(pivotTable.Cache)) {
					int indexCache = mainExporter.IncludedPivotCaches.LastIndexOf(pivotTable.Cache);
					XlsPivotTableExporter pivotTableExporter = new XlsPivotTableExporter(StreamWriter, pivotTable, ExportStyleSheet, Sheet, indexCache);
					pivotTableExporter.WriteContent();
				}
			}
		}
#endregion
#region WINDOW
		protected void WriteSheetViewInfo() {
			XlsCommandSheetViewInformation command = new XlsCommandSheetViewInformation();
			ModelWorksheetView view = Sheet.ActiveView;
			command.ShowFormulas = view.ShowFormulas;
			command.ShowGridlines = view.ShowGridlines;
			command.ShowRowColumnHeadings = view.ShowRowColumnHeaders;
			command.Frozen = view.SplitState != ViewSplitState.Split;
			command.ShowZeroValues = view.ShowZeroValues;
			command.RightToLeft = view.RightToLeft;
			command.ShowOutlineSymbols = view.ShowOutlineSymbols;
			command.FrozenWithoutPaneSplit = view.SplitState == ViewSplitState.Frozen;
			command.SheetTabIsSelected = view.TabSelected;
			command.InPageBreakPreview = view.ViewType == SheetViewType.PageBreakPreview;
			CellPosition topLeftCell = view.GetTopLeftCell();
			if (topLeftCell.IsValid) {
				if (topLeftCell.Row < XlsDefs.MaxRowCount)
					command.TopRowIndex = topLeftCell.Row;
				if (topLeftCell.Column < XlsDefs.MaxColumnCount)
					command.LeftColumnIndex = topLeftCell.Column;
			}
			command.GridlinesColorIndex = GetGridlinesColorIndex(view.GridlinesColor);
			command.ZoomScalePageBreakPreview = view.ZoomScalePageLayoutView;
			command.ZoomScaleNormalView = view.ZoomScaleNormal;
			command.Write(StreamWriter);
		}
		protected void WritePageLayoutView() {
			XlsCommandPageLayoutView command = new XlsCommandPageLayoutView();
			ModelWorksheetView view = Sheet.ActiveView;
			command.ZoomScale = view.ZoomScalePageLayoutView;
			command.InPageLayoutView = view.ViewType == SheetViewType.PageLayout;
			command.RullerVisible = view.ShowRuler;
			command.WhitespaceHidden = !view.ShowWhiteSpace;
			command.Write(StreamWriter);
		}
		protected void WriteSheetViewScale() {
			XlsCommandSheetViewScale command = new XlsCommandSheetViewScale();
			ModelWorksheetView view = Sheet.ActiveView;
			command.Numerator = view.ZoomScale;
			command.Denominator = 100;
			command.Write(StreamWriter);
		}
		protected void WritePane() {
			ModelWorksheetView view = Sheet.ActiveView;
			CellPosition splitTopLeftCell = view.SplitTopLeftCell;
			CellPosition splitPosition = view.GetSplitPosition(Sheet);
			bool hasRightPane = view.HorizontalSplitPosition > 0 && splitPosition.Column < XlsDefs.MaxColumnCount;
			bool hasBottomPane = view.VerticalSplitPosition > 0 && splitPosition.Row < XlsDefs.MaxRowCount;
			if (!hasRightPane && !hasBottomPane)
				return;
			XlsCommandPane command = new XlsCommandPane();
			command.ActivePane = view.ActivePaneType;
			command.TopRow = Math.Min(XlsDefs.MaxRowCount - 1, splitTopLeftCell.Row);
			command.LeftColumn = Math.Min(XlsDefs.MaxColumnCount - 1, splitTopLeftCell.Column);
			if (view.SplitState == ViewSplitState.Split) {
				command.XPos = Math.Min(Int16.MaxValue, DocumentModel.UnitConverter.ModelUnitsToTwips(view.HorizontalSplitPosition));
				command.YPos = Math.Min(Int16.MaxValue, DocumentModel.UnitConverter.ModelUnitsToTwips(view.VerticalSplitPosition));
			}
			else {
				command.XPos = view.HorizontalSplitPosition;
				command.YPos = view.VerticalSplitPosition;
				if (command.XPos >= XlsDefs.MaxColumnCount) {
					command.LeftColumn = 0;
					command.XPos = 0;
				}
				if (command.YPos >= XlsDefs.MaxRowCount) {
					command.TopRow = 0;
					command.YPos = 0;
				}
			}
			command.Write(StreamWriter);
		}
		protected void WriteSelections() {
			ModelWorksheetView view = Sheet.ActiveView;
			CellPosition splitPosition = view.GetSplitPosition(Sheet);
			bool hasRightPane = view.HorizontalSplitPosition > 0 && splitPosition.Column < XlsDefs.MaxColumnCount;
			bool hasBottomPane = view.VerticalSplitPosition > 0 && splitPosition.Row < XlsDefs.MaxRowCount;
			if (hasRightPane || hasBottomPane) {
				WriteSelection(ViewPaneType.TopLeft, splitPosition, view.ActivePaneType);
				if (hasRightPane)
					WriteSelection(ViewPaneType.TopRight, splitPosition, view.ActivePaneType);
				if (hasBottomPane)
					WriteSelection(ViewPaneType.BottomLeft, splitPosition, view.ActivePaneType);
				if (hasRightPane && hasBottomPane)
					WriteSelection(ViewPaneType.BottomRight, splitPosition, view.ActivePaneType);
			}
			else
				WriteSelection(ViewPaneType.TopLeft, splitPosition, ViewPaneType.TopLeft);
		}
		void WriteSelection(ViewPaneType pane, CellPosition splitPosition, ViewPaneType activePane) {
			int activeCellIndex = 0;
			CellPosition activeCell;
			List<CellRangeInfo> selectedRanges;
			if (pane == activePane) {
				SheetViewSelection selection = Sheet.Selection;
				selectedRanges = new List<CellRangeInfo>(selection.SelectedRanges.Count);
				foreach (CellRange item in selection.SelectedRanges) {
					if (!item.TopLeft.OutOfLimits())
						selectedRanges.Add(XlsCellRangeFactory.CreateTruncatedCellRangeInfo(item));
				}
				activeCell = selection.ActiveCell;
				if (selectedRanges.Count == 0) {
					activeCell = new CellPosition();
					selectedRanges.Add(new CellRangeInfo(activeCell, activeCell));
				}
				else if (activeCell.OutOfLimits()) {
					activeCell = selectedRanges[0].First;
				}
				else {
					for (int i = 0; i < selectedRanges.Count; i++) {
						if (selectedRanges[i].ContainsCell(activeCell)) {
							activeCellIndex = i;
							break;
						}
					}
				}
			}
			else {
				switch (pane) {
					case ViewPaneType.TopRight:
						activeCell = new CellPosition(splitPosition.Column, 0);
						break;
					case ViewPaneType.BottomLeft:
						activeCell = new CellPosition(0, splitPosition.Row);
						break;
					case ViewPaneType.BottomRight:
						activeCell = splitPosition;
						break;
					default:
						activeCell = Sheet.ActiveView.GetTopLeftCell();
						break;
				}
				selectedRanges = new List<CellRangeInfo>();
				selectedRanges.Add(new CellRangeInfo(activeCell, activeCell));
			}
			XlsCommandSelection command = new XlsCommandSelection();
			command.Pane = pane;
			command.ActiveCell = activeCell;
			command.ActiveCellIndex = activeCellIndex;
			foreach (CellRangeInfo item in selectedRanges) {
				command.SelectedCells.Add(item);
				if (command.SelectedCells.Count >= maxSelectionRecordCount) {
					command.Write(StreamWriter);
					command.SelectedCells.Clear();
				}
			}
			if (command.SelectedCells.Count > 0)
				command.Write(StreamWriter);
		}
#endregion
#region MergeCells
		protected void WriteMergeCells() {
			MergedCellCollection mergedCells = Sheet.MergedCells;
			if (mergedCells.Count == 0)
				return;
			int firstRow;
			int firstColumn;
			int lastRow;
			int lastColumn;
			XlsCommandMergeCells command = new XlsCommandMergeCells();
			foreach (CellRange range in mergedCells.GetEVERYMergedRangeSLOWEnumerable()) {
				CellIntervalRange intervalRange = range as CellIntervalRange;
				if (intervalRange != null) {
					if (intervalRange.IsColumnInterval) { 
						firstRow = 0;
						firstColumn = intervalRange.TopLeft.Column;
						lastRow = XlsDefs.MaxRowCount - 1;
						lastColumn = intervalRange.BottomRight.Column;
					}
					else if (intervalRange.IsRowInterval) { 
						firstRow = intervalRange.TopLeft.Row;
						firstColumn = 0;
						lastRow = intervalRange.BottomRight.Row;
						lastColumn = XlsDefs.MaxColumnCount - 1;
					}
					else
						continue;
				}
				else {
					firstRow = range.TopLeft.Row;
					firstColumn = range.TopLeft.Column;
					lastRow = range.BottomRight.Row;
					lastColumn = range.BottomRight.Column;
				}
				if (firstRow >= XlsDefs.MaxRowCount)
					continue;
				if (firstColumn >= XlsDefs.MaxColumnCount)
					continue;
				if (lastRow >= XlsDefs.MaxRowCount)
					lastRow = XlsDefs.MaxRowCount - 1;
				if (lastColumn >= XlsDefs.MaxColumnCount)
					lastColumn = XlsDefs.MaxColumnCount - 1;
				CellRangeInfo item = new CellRangeInfo(new CellPosition(firstColumn, firstRow), new CellPosition(lastColumn, lastRow));
				command.MergedCells.Add(item);
				if (command.MergedCells.Count >= XlsDefs.MaxMergeCellCount) {
					command.Write(StreamWriter);
					command.MergedCells.Clear();
				}
			}
			if (command.MergedCells.Count > 0)
				command.Write(StreamWriter);
		}
#endregion
		protected void WriteConditionalFormatting() {
			this.condFmtExporter.WriteContent();
		}
#region Hyperlinks
		protected void WriteHyperLinks() {
			int count = Sheet.Hyperlinks.Count;
			for (int i = 0; i < count; i++)
				WriteHyperLink(Sheet.Hyperlinks[i]);
		}
		void WriteHyperLink(ModelHyperlink hyperlink) {
			CellRange range = hyperlink.Range;
			CellRange maxRange = XlsLimits.GetBoundRange(range);
			if (!maxRange.ContainsRange(range)) {
				if (!maxRange.Intersects(range))
					return;
				range = range.IntersectionWith(maxRange).CellRangeValue.GetFirstInnerCellRange();
			}
			XlsCommandHyperlink command = new XlsCommandHyperlink();
			command.Range = new CellRangeInfo(range.TopLeft, range.BottomRight);
			string targetUri = hyperlink.TargetUri;
			if (hyperlink.IsExternal) {
				string location = string.Empty;
				int pos = targetUri.IndexOf('#');
				if (pos != -1) {
					location = targetUri.Substring(pos + 1);
					targetUri = targetUri.Substring(0, pos);
				}
				Uri uri;
				if (!Uri.TryCreate(targetUri, UriKind.RelativeOrAbsolute, out uri))
					return;
				command.HasMoniker = true;
				command.IsAbsolute = uri.IsAbsoluteUri;
				if (uri.IsAbsoluteUri && uri.Scheme != "file") {
					XlsHyperlinkURLMoniker urlMoniker = new XlsHyperlinkURLMoniker();
					urlMoniker.Url = targetUri;
					urlMoniker.HasOptionalData = true;
					urlMoniker.AllowImplicitFileScheme = true;
					urlMoniker.AllowRelative = true;
					urlMoniker.Canonicalize = true;
					urlMoniker.CrackUnknownSchemes = true;
					urlMoniker.DecodeExtraInfo = true;
					urlMoniker.IESettings = true;
					urlMoniker.NoEncodeForbiddenChars = true;
					urlMoniker.PreProcessHtmlUri = true;
					command.OleMoniker = urlMoniker;
				}
				else {
					XlsHyperlinkFileMoniker fileMoniker = new XlsHyperlinkFileMoniker();
					fileMoniker.Path = targetUri;
					command.OleMoniker = fileMoniker;
				}
				if (!string.IsNullOrEmpty(location)) {
					command.Location = location;
					command.HasLocationString = true;
				}
			}
			else {
				command.Location = targetUri;
				command.HasLocationString = true;
			}
			if (!string.IsNullOrEmpty(hyperlink.DisplayText)) {
				command.DisplayName = hyperlink.DisplayText;
				command.HasDisplayName = true;
				command.SiteGaveDisplayName = true;
			}
			command.Write(StreamWriter);
			WriteHyperlinkTooltip(hyperlink, range);
		}
		void WriteHyperlinkTooltip(ModelHyperlink hyperlink, CellRange range) {
			if (string.IsNullOrEmpty(hyperlink.TooltipText))
				return;
			XlsCommandHyperlinkTooltip command = new XlsCommandHyperlinkTooltip();
			command.Range = new CellRangeInfo(range.TopLeft, range.BottomRight);
			command.Tooltip = hyperlink.TooltipText;
			command.Write(StreamWriter);
		}
#endregion
#region DataValidations
		protected void WriteDataValidations() {
			DataValidationCollection dataValidations = Sheet.DataValidations;
			int recordCount = XlsDataValidationHelper.GetDataValidationsRecordCount(dataValidations);
			if (recordCount == 0)
				return;
			XlsCommandDataValidations command = new XlsCommandDataValidations();
			command.RecordCount = recordCount;
			command.Write(StreamWriter);
			int count = 0;
			foreach (DataValidation item in dataValidations) {
				if (WriteDataValidation(item)) {
					count++;
					if (count > XlsDefs.MaxDataValidationRecordCount)
						break;
				}
			}
		}
		bool WriteDataValidation(DataValidation dataValidation) {
			if (!XlsDataValidationHelper.IsXlsDVFormulaCompliant(dataValidation.Expression1) || 
				!XlsDataValidationHelper.IsXlsDVFormulaCompliant(dataValidation.Expression2))
				return false;
			XlsCommandDataValidation command = new XlsCommandDataValidation();
			PrepareDataValidationRanges(command, dataValidation.CellRange);
			if (command.Ranges.Count == 0)
				return false;
			command.ValidationType = dataValidation.Type;
			command.ErrorStyle = dataValidation.ErrorStyle;
			command.AllowBlank = dataValidation.AllowBlank;
			command.SuppressCombo = dataValidation.SuppressDropDown;
			command.ShowInputMessage = dataValidation.ShowInputMessage;
			command.ShowErrorMessage = dataValidation.ShowErrorMessage;
			command.ImeMode = dataValidation.ImeMode;
			command.ValidationOperator = dataValidation.ValidationOperator;
			command.PromptTitle = CheckLength(dataValidation.PromptTitle, XlsDefs.MaxDataValidationTitleLength);
			command.ErrorTitle = CheckLength(dataValidation.ErrorTitle, XlsDefs.MaxDataValidationTitleLength);
			command.Prompt = CheckLength(dataValidation.Prompt, XlsDefs.MaxDataValidationPromptLength);
			command.Error = CheckLength(dataValidation.Error, XlsDefs.MaxDataValidationErrorLength);
			XlsRPNContext context = ExportStyleSheet.RPNContext;
			context.WorkbookContext.PushCurrentWorksheet(Sheet);
			context.WorkbookContext.PushRelativeToCurrentCell(true);
			context.WorkbookContext.PushCurrentCell(dataValidation.CellRange.TopLeft);
			try {
				ParsedExpression expression = dataValidation.Expression1;
				if (expression != null && expression.Count > 0) {
					expression = XlsParsedThingConverter.ToXlsExpression(expression, context);
					if (dataValidation.Type == DataValidationType.List)
						command.StringLookup = ConvertListSourceFormula(expression, context.WorkbookContext);
					command.SetFormula1(expression, context);
				}
				expression = dataValidation.Expression2;
				if (expression != null && expression.Count > 0) {
					expression = XlsParsedThingConverter.ToXlsExpression(expression, context);
					command.SetFormula2(expression, context);
				}
			}
			finally {
				context.WorkbookContext.PopCurrentCell();
				context.WorkbookContext.PopRelativeToCurrentCell();
				context.WorkbookContext.PopCurrentWorksheet();
			}
			command.Write(StreamWriter);
			return true;
		}
		void PrepareDataValidationRanges(XlsCommandDataValidation command, CellRangeBase cellRange) {
			if (cellRange.RangeType == CellRangeType.UnionRange) {
				CellUnion union = cellRange as CellUnion;
				int count = 0;
				foreach (CellRangeBase range in union.InnerCellRanges) {
					if (range.TopLeft.OutOfLimits())
						continue;
					if (count >= XlsDefs.MaxDataValidationSqRefCount) {
						break;
					}
					command.Ranges.Add(XlsRangeHelper.GetCellRangeInfo(range));
					count++;
				}
			}
			else if (!cellRange.TopLeft.OutOfLimits())
				command.Ranges.Add(XlsRangeHelper.GetCellRangeInfo(cellRange));
		}
		bool ConvertListSourceFormula(ParsedExpression expression, WorkbookDataContext context) {
			if (expression.Count == 0)
				return false;
			ParsedThingStringValue ptg = expression[0] as ParsedThingStringValue;
			if (ptg == null || string.IsNullOrEmpty(ptg.Value))
				return false;
			string value = ptg.Value.Replace(context.GetListSeparator(), '\0');
			expression.RemoveAt(0);
			expression.Insert(0, new ParsedThingStringValue(value) { DataType = ptg.DataType });
			return true;
		}
		string CheckLength(string value, int maxLength) {
			if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
				return value;
			return value.Substring(0, maxLength);
		}
#endregion
#region CodeName
		void WriteCodeName() {
			if (DocumentModel.VbaProjectContent.IsEmpty || string.IsNullOrEmpty(Sheet.Properties.CodeName))
				return;
			XlsCommandCodeName command = new XlsCommandCodeName();
			command.Name = Sheet.Properties.CodeName;
			command.Write(StreamWriter);
		}
#endregion
#region SharedFeatures
		void WriteSharedFeatureProtection() {
			WorksheetProtectionOptions protection = Sheet.Properties.Protection;
			XlsCommandSharedFeatureHeader command = new XlsCommandSharedFeatureHeader();
			command.SharedFeatureType = XlsSharedFeatureType.Protection;
			command.HasHeaderData = true;
			command.Protection.AutoFilter = !protection.AutoFiltersLocked;
			command.Protection.DeleteColumns = !protection.DeleteColumnsLocked;
			command.Protection.DeleteRows = !protection.DeleteRowsLocked;
			command.Protection.FormatCells = !protection.FormatCellsLocked;
			command.Protection.FormatColumns = !protection.FormatColumnsLocked;
			command.Protection.FormatRows = !protection.FormatRowsLocked;
			command.Protection.InsertColumns = !protection.InsertColumnsLocked;
			command.Protection.InsertHyperlinks = !protection.InsertHyperlinksLocked;
			command.Protection.InsertRows = !protection.InsertRowsLocked;
			command.Protection.Objects = !protection.ObjectsLocked;
			command.Protection.PivotTables = !protection.PivotTablesLocked;
			command.Protection.Scenarios = !protection.ScenariosLocked;
			command.Protection.SetLockedCells = !protection.SelectLockedCellsLocked;
			command.Protection.SetUnlockedCells = !protection.SelectUnlockedCellsLocked;
			command.Protection.Sort = !protection.SortLocked;
			command.Write(StreamWriter);
		}
		void WriteProtectedRanges() {
			if (Sheet.ProtectedRanges.Count == 0)
				return;
			foreach (ModelProtectedRange range in Sheet.ProtectedRanges)
				WriteProtectedRange(range);
		}
		void WriteProtectedRange(ModelProtectedRange protectedRange) {
			XlsSharedFeatureProtection protectionFeature = new XlsSharedFeatureProtection();
			AddRefs(protectionFeature, protectedRange.CellRange);
			if (protectionFeature.Refs.Count == 0)
				return;
			protectionFeature.Title = protectedRange.Name;
			if (protectedRange.Credentials.PasswordVerifier != null)
				protectionFeature.PasswordVerifier = protectedRange.Credentials.PasswordVerifier.Value;
#if !SL && !DOTNET
			if (!string.IsNullOrEmpty(protectedRange.SecurityDescriptor)) {
				RangeSecurity rangeSecurity = new RangeSecurity();
				rangeSecurity.SetSecurityDescriptorSddlForm(protectedRange.SecurityDescriptor);
				protectionFeature.SecurityDescriptor = rangeSecurity.GetSecurityDescriptorBinaryForm();
			}
#endif
			WriteSharedFeature(protectionFeature);
		}
		void WriteSharedFeatureErrors() {
			if (Sheet.IgnoredErrors.Count == 0)
				return;
			WriteSharedFeatureFecHeader();
			foreach (IgnoredError ignoredError in Sheet.IgnoredErrors)
				WriteIgnoredError(ignoredError);
		}
		void WriteSharedFeatureFecHeader() {
			XlsCommandSharedFeatureHeader command = new XlsCommandSharedFeatureHeader();
			command.SharedFeatureType = XlsSharedFeatureType.Fec2;
			command.Write(StreamWriter);
		}
		void WriteIgnoredError(IgnoredError ignoredError) {
			XlsSharedFeatureErrorCheck errorFeature = new XlsSharedFeatureErrorCheck();
			AddRefs(errorFeature, ignoredError.Range);
			errorFeature.CalculationErrors = ignoredError.EvaluateToError;
			errorFeature.DataValidation = ignoredError.ListDataValidation;
			errorFeature.EmptyCellRef = ignoredError.EmptyCellReferences;
			errorFeature.InconsistColumnFormula = ignoredError.InconsistentColumnFormula;
			errorFeature.InconsistFormula = ignoredError.InconsistentFormula;
			errorFeature.InconsistRange = ignoredError.FormulaRangeError;
			errorFeature.NumberStoredAsText = ignoredError.NumberAsText;
			errorFeature.TextDateInsuff = ignoredError.TwoDidgitTextYear;
			errorFeature.UnprotectedFormula = ignoredError.UnlockedFormula;
			WriteSharedFeature(errorFeature);
		}
		void AddRefs(XlsSharedFeatureBase sharedFeature, CellRangeBase modelRange) {
			foreach (CellRange cellRange in modelRange.GetAreasEnumerable()) {
				CellRangeInfo info = XlsCellRangeFactory.CreateTruncatedCellRangeInfo(cellRange);
				if (info != null)
					sharedFeature.Refs.Add(new Ref8U() { CellRangeInfo = info });
			}
		}
		void WriteSharedFeature(XlsSharedFeatureBase sharedFeature) {
			XlsCommandSharedFeature command = new XlsCommandSharedFeature();
			XlsCommandContinueFrt continueCommand = new XlsCommandContinueFrt();
			using (XlsChunkWriter chunkWriter = new XlsChunkWriter(StreamWriter, command, continueCommand)) {
				FutureRecordHeader header = new FutureRecordHeader();
				header.RecordTypeId = XlsCommandFactory.GetTypeCodeByType(typeof(XlsCommandSharedFeature));
				header.Write(chunkWriter);
				sharedFeature.Write(chunkWriter);
			}
		}
		void WriteListTables() {
			IList<Table> tables = GetExportTables(Sheet.Tables);
			int count = tables.Count;
			if (count > 0) {
				WriteListTableHeader();
				for (int i = 0; i < count; i++)
					WriteListTable(tables[i]);
			}
		}
		void WriteListTableHeader() {
			XlsCommandSharedFeatureHeader11 commandHeader = new XlsCommandSharedFeatureHeader11();
			commandHeader.IdListNext = GetCountTables() + 1;
			commandHeader.Write(StreamWriter);
		}
		void WriteListTable(Table table) {
			TableFeatureType tableInfo = TableFeatureType.FromTable(table, ExportStyleSheet);
			XlsCommandSharedFeature11 command = CreateCommandSharedFeature(table, tableInfo);
			XlsCommandRecordBase continueCommand = CreateCommandToContinueSharedFeature(table, tableInfo);
			using (XlsChunkWriter chunkWriter = new XlsChunkWriter(StreamWriter, command, continueCommand))
				command.TableObject.Write(chunkWriter);
			WriteList12(command.TableObject.TableInfo.IdList, table);
			SortState sortState = table.AutoFilter.SortState;
			if (sortState.SortRange != null)
				WriteSortData(XlsSortDataInfo.CreateForTableExport(sortState, table.HasHeadersRow, ExportStyleSheet.TableCount));
		}
		IList<Table> GetExportTables(TableCollection tables) {
			IList<Table> result = new List<Table>();
			int count = tables.Count;
			for (int i = 0; i < count; i++) {
				Table table = tables[i];
				if (table.TableType != TableType.QueryTable)
					result.Add(table);
			}
			return result;
		}
		int GetCountTables() {
			int result = 0;
			WorksheetCollection sheets = DocumentModel.Sheets;
			int sheetCount = sheets.Count;
			for (int i = 0; i < sheetCount; i++)
				result += sheets[i].Tables.Count;
			return result;
		}
		XlsCommandSharedFeature11 CreateCommandSharedFeature(Table table, TableFeatureType tableInfo) {
			XlsCommandSharedFeature11 result;
			if (IsUseFeature11(tableInfo, table))
				result = new XlsCommandSharedFeature11();
			else
				result = new XlsCommandSharedFeature12();
			AssignSharedFeatureTable(result, tableInfo);
			return result;
		}
		XlsCommandRecordBase CreateCommandToContinueSharedFeature(Table table, TableFeatureType tableInfo) {
			XlsCommandRecordBase result;
			if (IsUseFeature11(tableInfo, table))
				result = new XlsCommandContinueFrt11();
			else
				result = new XlsCommandContinueFrt();
			return result;
		}
		void AssignSharedFeatureTable(XlsCommandSharedFeature11 command, TableFeatureType tableInfo) {
			short typeCode = XlsCommandFactory.GetTypeCodeByType(command.GetType());
			command.TableObject = SharedFeatureTable.FromTableFeatureType(tableInfo, typeCode);
		}
		bool IsUseFeature11(TableFeatureType tableInfo, Table table) {
			bool result = tableInfo.TableType != TableType.QueryTable && (tableInfo.HasHeadersRow || tableInfo.FlagsInfo.SingleCell);
			if (!result)
				return result;
			return !CheckNotUseFeat12ForAllColumns(tableInfo.Columns, table.Columns);
		}
		bool CheckNotUseFeat12ForAllColumns(List<XlsTableColumnInfo> xlsColumns, TableColumnInfoCollection modelColumns) {
			int xlsColumnCount = xlsColumns.Count;
			for (int i = 0; i < xlsColumnCount; i++) {
				XlsTableColumnInfo xlsColumn = xlsColumns[i];
				TableColumn modelColumn = modelColumns[i];
				if (xlsColumn.HasLoadTotalString(modelColumn) || xlsColumn.HasLoadTotalFormula(modelColumn))
					return true;
			}
			return false;
		}
		void WriteList12(int idList, Table table) {
			WriteList12(idList, table, List12DataType.List12BlockLevel);
			WriteAutoFilters(table.AutoFilter, WriteTableAutoFilter);
			WriteList12(idList, table, List12DataType.List12TableStyleClientInfo);
			WriteList12(idList, table, List12DataType.List12DisplayName);
		}
		void WriteTableAutoFilter(AutoFilterColumn filterColumn, CellRange range) {
			if (!filterColumn.ShouldWriteAutoFilter12)
				return;
			CellRangeInfo cellRangeInfo = new CellRangeInfo(range.TopLeft, range.BottomRight);
			XlsAutoFilter12Info info = XlsAutoFilter12Info.CreateForTableExport(cellRangeInfo, filterColumn, ExportStyleSheet.TableCount);
			WriteAutoFilter12Command(info, cellRangeInfo);
		}
		void WriteList12(int idList, Table table, List12DataType type) {
			XlsCommandList12 command = new XlsCommandList12();
			command.Data = List12DataBuilder.CreateForExport(type, table, ExportStyleSheet);
			command.IdList = idList;
			command.Write(StreamWriter);
		}
#endregion
#region IDisposable Members
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) {
				if(this.cellsExporter != null) {
					this.cellsExporter.Dispose();
					this.cellsExporter = null;
				}
				if(this.condFmtExporter != null) {
					this.condFmtExporter.Dispose();
					this.condFmtExporter = null;
				}
			}
		}
#endregion
#region Utils
		int GetColumnsMaxOutlineLevel() {
			int result = 0;
			foreach(Column column in Sheet.Columns.GetExistingColumns()) {
				if(column.OutlineLevel > result)
					result = column.OutlineLevel;
				if(result == XlsDefs.MaxOutlineLevel) break;
			}
			return result;
		}
		void WriteStubRecords() {
			XlsContentIndex indexRecord = new XlsContentIndex();
			CellRange rowsRange = CellsExporter.CustomRowsRange;
			if (rowsRange == null) {
				indexRecord.FirstRowIndex = 0;
				indexRecord.LastRowIndex = 0;
			}
			else {
				indexRecord.FirstRowIndex = rowsRange.TopLeft.Row;
				indexRecord.LastRowIndex = rowsRange.BottomRight.Row + 1;
			}
			indexRecord.DefaultColumnWidthOffset = this.defColWidthRecordPosition;
			indexRecord.DbCellsPositions.AddRange(CellsExporter.DbCellsPositions);
			IndexRecordStub.ReplaceStub(StreamWriter, indexRecord);
		}
		public int GetGridlinesColorIndex(Color color) {
			if(color == DXColor.Empty)
				return Palette.DefaultForegroundColorIndex;
			int index = DocumentModel.StyleSheet.Palette.GetColorIndex(color);
			if(index > Palette.DefaultForegroundColorIndex)
				index = Palette.DefaultForegroundColorIndex;
			return index;
		}
#endregion
	}
	public static class XlsTopmostShapeIdHelper {
		public static int GetTopmostShapeId(this Worksheet sheet) {
			return ((sheet.SheetId - 1) % 16 + 1) * 0x400;
		}
	}
	public static class XlsDataValidationHelper {
		public static int GetDataValidationsRecordCount(DataValidationCollection collection) {
			int result = 0;
			int count = collection.Count;
			for (int i = 0; i < count; i++) {
				DataValidation item = collection[i];
				if (!IsXlsDVFormulaCompliant(item.Expression1) ||
					!IsXlsDVFormulaCompliant(item.Expression2) ||
					!IsXlsCompatibleRange(item.CellRange))
					continue;
				result++;
				if (result == XlsDefs.MaxDataValidationRecordCount)
					break;
			}
			return result;
		}
		public static bool IsXlsCompliant(DataValidation dataValidation) {
			if (dataValidation == null)
				return false;
			return IsXlsDVFormulaCompliant(dataValidation.Expression1) &&
				IsXlsDVFormulaCompliant(dataValidation.Expression2) &&
				IsXlsCompatibleRange(dataValidation.CellRange);
		}
		public static bool IsXlsDVFormulaCompliant(ParsedExpression expression) {
			if (expression == null || expression.Count == 0)
				return true;
			return expression.IsXlsDVFormulaCompliant();
		}
		public static bool IsXlsCompatibleRange(CellRangeBase cellRange) {
			if (cellRange.RangeType == CellRangeType.UnionRange) {
				CellUnion union = cellRange as CellUnion;
				foreach (CellRangeBase range in union.InnerCellRanges) {
					if (!range.TopLeft.OutOfLimits())
						return true;
				}
				return false;
			}
			return !cellRange.TopLeft.OutOfLimits();
		}
	}
	#region ModelShapeExportFillHelper
	public static class ModelShapeExportFillHelper {
		public static void CreateFillProperties(IDrawingFill fill, OfficeArtProperties artProperties) {
			switch(fill.FillType) {
				case DrawingFillType.Automatic:
					return;
				case DrawingFillType.None:
					return;
				case DrawingFillType.Solid:
					CreateSolidFillProperties(fill as DrawingSolidFill, artProperties);
					break;
				case DrawingFillType.Gradient:
					break;
				case DrawingFillType.Group:
					break;
				case DrawingFillType.Pattern:
					break;
				case DrawingFillType.Picture:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
		static void CreateSolidFillProperties(DrawingSolidFill drawingSolidFill, OfficeArtProperties artProperties) {
			CreateSolidFillColor(drawingSolidFill.Color, artProperties);
		}
		static void CreateSolidFillColor(IDrawingColor color, OfficeArtProperties artProperties) {
			if(color == null || color.IsEmpty)
				artProperties.Properties.Add(new DrawingFillColor(DXColor.FromArgb(0xff, 0xff, 0xff)));
			else if(color.ColorType == DrawingColorType.System)
				CreateSystemColor(color, artProperties);
			else if(color.ColorType == DrawingColorType.Scheme)
				CreateSchemeColor(color, artProperties);
			else
				artProperties.Properties.Add(new DrawingFillColor(color.FinalColor));
		}
		static void CreateSchemeColor(IDrawingColor color, OfficeArtProperties artProperties) {
			OfficeColorRecord colorRecord = new OfficeColorRecord {ColorSchemeIndex = (byte) color.Scheme};
			DrawingFillColor drawingFillColor = new DrawingFillColor {ColorRecord = colorRecord};
			artProperties.Properties.Add(drawingFillColor);
		}
		static void CreateSystemColor(IDrawingColor color, OfficeArtProperties artProperties) {
			OfficeColorRecord colorRecord = new OfficeColorRecord {SystemColorIndex = (int) color.System};
			DrawingFillColor drawingFillColor = new DrawingFillColor {ColorRecord = colorRecord};
			artProperties.Properties.Add(drawingFillColor);
		}
	}
	#endregion
}
