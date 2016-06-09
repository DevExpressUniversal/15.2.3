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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Xml;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Import.OpenXml;
using DevExpress.Utils.Zip;
using DevExpress.XtraExport.Xlsx;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		#region Translation tables
		internal static Dictionary<SheetViewType, string> SheetViewTypeTable = CreateSheetViewTypeTable();
		internal static Dictionary<ViewPaneType, string> ViewPaneTypeTable = CreateViewPaneTypeTable();
		internal static Dictionary<ViewSplitState, string> ViewSplitStateTable = CreateViewSplitStateTable();
		static Dictionary<SheetViewType, string> CreateSheetViewTypeTable() {
			Dictionary<SheetViewType, string> result = new Dictionary<SheetViewType, string>();
			result.Add(SheetViewType.Normal, "normal");
			result.Add(SheetViewType.PageLayout, "pageLayout");
			result.Add(SheetViewType.PageBreakPreview, "pageBreakPreview");
			return result;
		}
		static Dictionary<ViewPaneType, string> CreateViewPaneTypeTable() {
			Dictionary<ViewPaneType, string> result = new Dictionary<ViewPaneType, string>();
			result.Add(ViewPaneType.TopLeft, "topLeft");
			result.Add(ViewPaneType.TopRight, "topRight");
			result.Add(ViewPaneType.BottomLeft, "bottomLeft");
			result.Add(ViewPaneType.BottomRight, "bottomRight");
			return result;
		}
		static Dictionary<ViewSplitState, string> CreateViewSplitStateTable() {
			Dictionary<ViewSplitState, string> result = new Dictionary<ViewSplitState, string>();
			result.Add(ViewSplitState.Split, "split");
			result.Add(ViewSplitState.Frozen, "frozen");
			result.Add(ViewSplitState.FrozenSplit, "frozenSplit");
			return result;
		}
		#endregion
		protected internal virtual void AddWorksheetRelationsPackageContent() {
			foreach (KeyValuePair<string, string> valuePair in WorksheetRelationPathTable) {
				currentRelations = SheetRelationsTable[valuePair.Key];
				AddPackageContent(valuePair.Value, ExportWorksheetRelations());
			}
		}
		protected internal virtual CompressedStream ExportWorksheetRelations() {
			return CreateXmlContent(GenerateWorksheetRelationsContent);
		}
		protected internal virtual void GenerateWorksheetRelationsContent(XmlWriter writer) {
			Builder.GenerateRelationsContent(writer, currentRelations);
		}
		protected internal virtual void AddSheetsPackageContent() {
			WorksheetCollection sheets = Workbook.Sheets;
			int sheetCount = sheets.Count;
			for (int i = 0; i < sheetCount; i++) {
				Worksheet currentSheet = sheets[i];
				this.activeSheet = currentSheet;
				IColumnWidthCalculationService gridCalculationService = Workbook.GetService<IColumnWidthCalculationService>();
				Debug.Assert(gridCalculationService != null);
				this.activeSheetDefaultColumnWidthInChars = gridCalculationService.AddGaps(currentSheet, gridCalculationService.CalculateDefaultColumnWidthInChars(currentSheet, Workbook.MaxDigitWidthInPixels));
				string sheetPath = SheetNamesTable[currentSheet.Name];
				AddPackageContent(sheetPath, ExportSheetContent());
				AddCommentsContent();
			}
		}
		protected internal virtual CompressedStream ExportSheetContent() {
			return CreateXmlContent(GenerateSheetXmlContent);
		}
		protected internal virtual void GenerateSheetXmlContent(XmlWriter writer) {
			DocumentContentWriter = writer;
			GenerateSheetContent();
		}
		protected internal virtual void GenerateSheetContent() {
			WriteShStartElement("worksheet");
			try {
				WriteStringAttr("xmlns", RelsPrefix, null, RelsNamespace);
				GenerateSheetProperties();
				GeneratePivotTableRelations(); 
				GenerateSheetViews();
				GenerateSheetFormatProperties();
				GenerateColumnsContent();
				GenerateDataSheetContent();
				GenerateSheetProtection();
				GenerateSheetProtectedRanges(ActiveSheet.ProtectedRanges);
				GenerateSheetAutoFilterContent(ActiveSheet.AutoFilter);
				GenerateMergedCellsContent();
				GenerateConditionalFormattings(); 
				GenerateDataValidations();
				GenerateHyperlinksContent(); 
				GeneratePrintOptionsContent(ActiveSheet.PrintSetup);
				GeneratePageMarginsContent(ActiveSheet.Margins);
				GeneratePrintSetupContent(ActiveSheet.PrintSetup);
				GenerateHeaderFooterContent(ActiveSheet.Properties.HeaderFooter);
				GenerateRowBreaksContent(); 
				GenerateColumnBreaksContent(); 
				GenerateIgnoredErrorsContent();
				GenerateDrawingContent();
				GenerateLegacyDrawingContent();
				GenerateTablePartsContent();
				GenerateFutureFeatureStorage();
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void GenerateChartsheetContent() {
			Chartsheet sheet = ActiveSheetCore as Chartsheet;
			if (sheet == null)
				return;
			WriteShStartElement("chartsheet");
			try {
				WriteStringAttr("xmlns", RelsPrefix, null, RelsNamespace);
				GenerateChartsheetProperties();
				GenerateChartsheetViews();
				GenerateChartsheetProtection();
				GenerateChartsheetCustomViews();
				GeneratePageMarginsContent(sheet.Properties.Margins);
				GeneratePrintSetupContent(sheet.Properties.PrintSetup);
				GenerateHeaderFooterContent(sheet.Properties.HeaderFooter);
				GenerateDrawingContent();
				GenerateLegacyDrawingHfContent();
				GenerateChartsheetPicture();
				GenerateChartsheetWebPublishItems();
				GenerateChartsheetExtLst();
			}
			finally {
				WriteShEndElement();
			}
		}
		void GenerateChartsheetViews() {
			WriteShStartElement("sheetViews");
			try {
				GenerateSheetView(ActiveSheetCore as Chartsheet);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void GenerateSheetView(Chartsheet sheet) {
			ModelWorksheetView view = sheet.ActiveView;
			WriteShStartElement("sheetView");
			try {
				if (view.TabSelected)
					WriteBoolValue("tabSelected", view.TabSelected);
				if (view.ZoomScale != 100)
					WriteIntValue("zoomScale", view.ZoomScale);
				WriteIntValue("workbookViewId", view.WorkbookViewId);
			}
			finally {
				WriteShEndElement();
			}
		}
		void GenerateChartsheetProtection() {
			Chartsheet chartSheet = ActiveSheetCore as Chartsheet;
			WorksheetProtectionInfo defaultItemInfo = this.Workbook.Cache.WorksheetProtectionInfoCache.SchemaDefaultItem;
			WorksheetProtectionOptions protection = chartSheet.Properties.Protection;
			if (protection.Index == WorksheetProtectionInfoCache.DefaultItemIndex)
				return;
			WriteShStartElement("sheetProtection");
			try {
				if (protection.ContentLocked != defaultItemInfo.ContentLocked)
					WriteBoolValue("content", protection.ContentLocked);
				if (protection.ObjectsLocked != defaultItemInfo.ObjectsLocked)
					WriteBoolValue("objects", protection.ObjectsLocked);
				ExportProtectionPasswordHashes(protection.Credentials, false);
			}
			finally {
				WriteShEndElement();
			}
		}
		void GenerateChartsheetCustomViews() { }
		void GenerateLegacyDrawingHfContent() { }
		void GenerateChartsheetPicture() { }
		void GenerateChartsheetWebPublishItems() { }
		void GenerateChartsheetExtLst() { }
		void GenerateChartsheetProperties() {
			WriteShStartElement("sheetPr");
			try {
				WorksheetProperties properties = ActiveSheetCore.Properties as WorksheetProperties;
				if (properties != null) {
					if (!string.IsNullOrEmpty(properties.CodeName))
						WriteStringValue("codeName", properties.CodeName);
					GenerateSheetPropertiesSheetTabColor(properties.TabColorIndex);
				}
			}
			finally {
				WriteShEndElement();
			}
		}
		#region SheetViews
		protected internal virtual void GenerateSheetViews() {
			WriteShStartElement("sheetViews");
			try {
				GenerateSheetView(ActiveSheet);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void GenerateSheetView(Worksheet sheet) {
			ModelWorksheetView view = sheet.ActiveView;
			WriteShStartElement("sheetView");
			try {
				if(view.ViewType != SheetViewType.Normal)
					WriteStringValue("view", SheetViewTypeTable[view.ViewType]);
				if (view.TabSelected)
					WriteBoolValue("tabSelected", view.TabSelected);
				if (view.ShowFormulas)
					WriteBoolValue("showFormulas", view.ShowFormulas);
				if (!view.ShowGridlines)
					WriteBoolValue("showGridLines", view.ShowGridlines);
				if (!view.ShowRowColumnHeaders)
					WriteBoolValue("showRowColHeaders", view.ShowRowColumnHeaders);
				if (!view.ShowZeroValues)
					WriteBoolValue("showZeros", view.ShowZeroValues);
				if (!view.ShowWhiteSpace)
					WriteBoolValue("showWhiteSpace", view.ShowWhiteSpace);
				if (!view.ShowRuler)
					WriteBoolValue("showRuler", view.ShowRuler);
				if (!view.ShowOutlineSymbols)
					WriteBoolValue("showOutlineSymbols", view.ShowOutlineSymbols);
				if (view.WindowProtection)
					WriteBoolValue("windowProtection", view.WindowProtection);
				if (view.ZoomScale != 100)
					WriteIntValue("zoomScale", view.ZoomScale);
				if (view.ZoomScaleNormal != 0)
					WriteIntValue("zoomScaleNormal", view.ZoomScaleNormal);
				if (view.ZoomScaleSheetLayoutView != 0)
					WriteIntValue("zoomScaleSheetLayoutView", view.ZoomScaleSheetLayoutView);
				if (view.ZoomScalePageLayoutView != 0)
					WriteIntValue("zoomScalePageLayoutView", view.ZoomScalePageLayoutView);
				WriteIntValue("workbookViewId", view.WorkbookViewId);
				CellPosition topLeftCell = view.GetTopLeftCell();
				if(topLeftCell.IsValid && (topLeftCell.Column != 0 || topLeftCell.Row != 0))
					WriteCellPosition("topLeftCell", topLeftCell);
				if (ShouldGenerateSheetViewPane(view))
					GenerateSheetViewPane(view);
				GenerateSheetViewSelections(sheet);
				if (sheet.PivotSelection.IsExportNeed())
					GenerateSheetViewPivotSelections(sheet);
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		protected internal virtual void GenerateSheetViewPivotSelections(Worksheet sheet) {
			PivotSelection pSelection = sheet.PivotSelection;
			WriteShStartElement("pivotSelection");
			try {
				if (pSelection.Pane != ViewPaneType.TopLeft)
					WriteStringValue("pane", ViewPaneTypeTable[pSelection.Pane]);
				WriteBoolValue("showHeader", pSelection.IsShowHeader, false);
				WriteBoolValue("label", pSelection.IsLabel, false);
				WriteBoolValue("data", pSelection.IsDataSelection, false);
				WriteBoolValue("extendable", pSelection.IsExtendable, false);
				WriteIntValue("count", pSelection.CountSelection, pSelection.CountSelection > 0);
				WriteEnumValue("axis", pSelection.Axis, PivotTablePivotFieldDestination.pivotTableAxisTable, PivotTableAxis.None);
				WriteIntValue("dimension", pSelection.Dimension, pSelection.Dimension > 0);
				WriteIntValue("start", pSelection.Start, pSelection.Start > 0);
				WriteIntValue("min", pSelection.Minimum, pSelection.Minimum > 0);
				WriteIntValue("max", pSelection.Maximum, pSelection.Maximum > 0);
				WriteIntValue("activeRow", pSelection.ActiveRow, pSelection.ActiveRow > 0);
				WriteIntValue("activeCol", pSelection.ActiveColumn, pSelection.ActiveColumn > 0);
				WriteIntValue("previousRow", pSelection.PreviousRow, pSelection.PreviousRow > 0);
				WriteIntValue("previousCol", pSelection.PreviousColumn, pSelection.PreviousColumn > 0);
				WriteIntValue("click", pSelection.CountClick, pSelection.CountClick > 0);
				if (pSelection.PivotTable != null)
					WriteStringAttr(RelsPrefix, "id", null, PivotTablesRelationId[pSelection.PivotTable]);
				GeneratePivotAreaContent(pSelection.PivotArea);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void GenerateSheetViewSelections(Worksheet sheet) {
			ModelWorksheetView view = sheet.ActiveView;
			CellPosition splitPosition = view.GetSplitPosition(sheet);
			bool hasRightPane = view.HorizontalSplitPosition > 0;
			bool hasBottomPane = view.VerticalSplitPosition > 0;
			if(hasRightPane || hasBottomPane) {
				GenerateSheetViewSelection(sheet, ViewPaneType.TopLeft, splitPosition, view.ActivePaneType);
				if(hasRightPane)
					GenerateSheetViewSelection(sheet, ViewPaneType.TopRight, splitPosition, view.ActivePaneType);
				if(hasBottomPane)
					GenerateSheetViewSelection(sheet, ViewPaneType.BottomLeft, splitPosition, view.ActivePaneType);
				if(hasRightPane && hasBottomPane)
					GenerateSheetViewSelection(sheet, ViewPaneType.BottomRight, splitPosition, view.ActivePaneType);
			}
			else
				GenerateSheetViewSelection(sheet, ViewPaneType.TopLeft, splitPosition, ViewPaneType.TopLeft);
		}
		protected internal virtual void GenerateSheetViewSelection(Worksheet sheet, ViewPaneType pane, CellPosition splitPosition, ViewPaneType activePane) {
			CellPosition activeCell;
			int activeCellIndex = 0;
			CellRangeBase selectionRange;
			if(pane == activePane) {
				SheetViewSelection selection = sheet.Selection;
				IList<CellRange> ranges = selection.SelectedRanges;
				if(ranges.Count == 1 && ranges[0].CellCount == 1) {
					CellPosition topLeft = ranges[0].TopLeft;
					if(topLeft.Column == 0 && topLeft.Row == 0)
						return;
				}
				activeCell = selection.ActiveCell;
				activeCellIndex = selection.ActiveRangeIndex;
				selectionRange = selection.AsRange();
			}
			else {
				switch(pane) {
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
						activeCell = sheet.ActiveView.GetTopLeftCell();
						break;
				}
				if(activeCell.Column == 0 && activeCell.Row == 0)
					return;
				selectionRange = new CellRange(sheet, activeCell, activeCell);
			}
			WriteShStartElement("selection");
			try {
				if(pane != ViewPaneType.TopLeft)
					WriteStringValue("pane", ViewPaneTypeTable[pane]);
				WriteCellPosition("activeCell", activeCell);
				if(activeCellIndex != 0)
					WriteIntValue("activeCellId", activeCellIndex);
				WriteStSqref(selectionRange, "sqref");
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual bool ShouldGenerateSheetViewPane(ModelWorksheetView view) {
			ModelWorksheetViewInfo defaultItem = ModelWorksheetView.DefaultItem;
			if (view.ActivePaneType == defaultItem.ActivePaneType &&
				view.SplitState == defaultItem.SplitState &&
				view.HorizontalSplitPosition == defaultItem.HorizontalSplitPosition &&
				view.VerticalSplitPosition == defaultItem.VerticalSplitPosition)
				return false;
			return true;
		}
		protected internal virtual void GenerateSheetViewPane(ModelWorksheetView view) {
			WriteShStartElement("pane");
			try {
				ModelWorksheetViewInfo defaultItem = ModelWorksheetView.DefaultItem;
				if (view.ActivePaneType != defaultItem.ActivePaneType)
					WriteStringValue("activePane", ViewPaneTypeTable[view.ActivePaneType]);
				if (view.SplitState != defaultItem.SplitState)
					WriteStringValue("state", ViewSplitStateTable[view.SplitState]);
				if(!view.SplitTopLeftCell.EqualsPosition(defaultItem.SplitTopLeftCell))
					WriteCellPosition("topLeftCell", view.SplitTopLeftCell);
				if (view.SplitState == ViewSplitState.Split) {
					if (view.HorizontalSplitPosition != defaultItem.HorizontalSplitPosition)
						WriteIntValue("xSplit", ActiveSheet.Workbook.UnitConverter.ModelUnitsToTwips(view.HorizontalSplitPosition));
					if (view.VerticalSplitPosition != defaultItem.VerticalSplitPosition)
						WriteIntValue("ySplit", ActiveSheet.Workbook.UnitConverter.ModelUnitsToTwips(view.VerticalSplitPosition));
				}
				else {
					if (view.HorizontalSplitPosition != defaultItem.HorizontalSplitPosition)
						WriteIntValue("xSplit", view.HorizontalSplitPosition);
					if (view.VerticalSplitPosition != defaultItem.VerticalSplitPosition)
						WriteIntValue("ySplit", view.VerticalSplitPosition);
				}
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void GenerateSheetFormatProperties() {
			SheetFormatProperties formatProperties = ActiveSheet.Properties.FormatProperties;
			if (formatProperties.IsDefault())
				return;
			SheetFormatInfo defaultValue = Workbook.Cache.SheetFormatInfoCache.DefaultItem;
			WriteShStartElement("sheetFormatPr");
			try {
				if (formatProperties.BaseColumnWidth != defaultValue.BaseColumnWidth)
					WriteIntValue("baseColWidth", formatProperties.BaseColumnWidth);
				if(formatProperties.DefaultColumnWidth != defaultValue.DefaultColumnWidth) {
					float value = Workbook.GetService<IColumnWidthCalculationService>().AddGaps(ActiveSheet, formatProperties.DefaultColumnWidth);
					WriteStringValue("defaultColWidth", value.ToString(CultureInfo.InvariantCulture));
				}
				float defaultRowHeightInPoints;
				float defaultRowHeightInModels = formatProperties.DefaultRowHeight;
				if (defaultRowHeightInModels <= 0.0) {
					float twoPixelPadding = Workbook.LayoutUnitConverter.PixelsToLayoutUnitsF(2, DocumentModel.DpiX);
					float resultHeight = twoPixelPadding + Workbook.CalculateDefaultRowHeightInLayoutUnits();
					defaultRowHeightInModels = Workbook.ToDocumentLayoutUnitConverter.ToModelUnits(resultHeight);
					defaultRowHeightInPoints = (float)Math.Round(this.Workbook.UnitConverter.ModelUnitsToPointsF(defaultRowHeightInModels));
				}
				else {
					defaultRowHeightInPoints = this.Workbook.UnitConverter.ModelUnitsToPointsF(defaultRowHeightInModels);
				}
				string defaultRowHeight = defaultRowHeightInPoints.ToString(CultureInfo.InvariantCulture);
				WriteStringValue("defaultRowHeight", defaultRowHeight);
				if (formatProperties.IsCustomHeight != defaultValue.IsCustomHeight)
					WriteBoolValue("customHeight", formatProperties.IsCustomHeight);
				if (formatProperties.ZeroHeight != defaultValue.ZeroHeight)
					WriteBoolValue("zeroHeight", formatProperties.ZeroHeight);
				if (formatProperties.ThickTopBorder != defaultValue.ThickTopBorder)
					WriteBoolValue("thickTop", formatProperties.ThickTopBorder);
				if (formatProperties.ThickBottomBorder != defaultValue.ThickBottomBorder)
					WriteBoolValue("thickBottom", formatProperties.ThickBottomBorder);
				if (formatProperties.OutlineLevelCol != defaultValue.OutlineLevelCol)
					WriteIntValue("outlineLevelCol", formatProperties.OutlineLevelCol);
				if (formatProperties.OutlineLevelRow != defaultValue.OutlineLevelRow)
					WriteIntValue("outlineLevelRow", formatProperties.OutlineLevelRow);
			}
			finally {
				WriteShEndElement();
			}
		}
		bool ShouldExportSheetProtection(WorksheetProtectionOptions protection, WorksheetProtectionInfo defaultValue) {
			return (protection.SheetLocked != defaultValue.SheetLocked) ||
				(protection.AutoFiltersLocked != defaultValue.AutoFiltersLocked) ||
				(protection.DeleteColumnsLocked != defaultValue.DeleteColumnsLocked) ||
				(protection.DeleteRowsLocked != defaultValue.DeleteRowsLocked) ||
				(protection.FormatCellsLocked != defaultValue.FormatCellsLocked) ||
				(protection.FormatColumnsLocked != defaultValue.FormatColumnsLocked) ||
				(protection.FormatRowsLocked != defaultValue.FormatRowsLocked) ||
				(protection.InsertColumnsLocked != defaultValue.InsertColumnsLocked) ||
				(protection.InsertHyperlinksLocked != defaultValue.InsertHyperlinksLocked) ||
				(protection.InsertRowsLocked != defaultValue.InsertRowsLocked) ||
				(protection.ObjectsLocked != defaultValue.ObjectsLocked && protection.SheetLocked != defaultValue.SheetLocked) ||
				(protection.PivotTablesLocked != defaultValue.PivotTablesLocked) ||
				(protection.ScenariosLocked != defaultValue.ScenariosLocked && protection.SheetLocked != defaultValue.SheetLocked) ||
				(protection.SelectLockedCellsLocked != defaultValue.SelectLockedCellsLocked) ||
				(protection.SelectUnlockedCellsLocked != defaultValue.SelectUnlockedCellsLocked) ||
				(protection.SortLocked != defaultValue.SortLocked);
		}
		protected internal virtual void GenerateSheetProtection() {
			WorksheetProtectionOptions protection = ActiveSheet.Properties.Protection;
			WorksheetProtectionInfo defaultValue = Workbook.Cache.WorksheetProtectionInfoCache.SchemaDefaultItem;
			if (!ShouldExportSheetProtection(protection, defaultValue))
				return;
			WriteShStartElement("sheetProtection");
			try {
				if(protection.SheetLocked != defaultValue.SheetLocked)
					WriteBoolValue("sheet", protection.SheetLocked);
				if (protection.AutoFiltersLocked != defaultValue.AutoFiltersLocked)
					WriteBoolValue("autoFilter", protection.AutoFiltersLocked);
				if (protection.DeleteColumnsLocked != defaultValue.DeleteColumnsLocked)
					WriteBoolValue("deleteColumns", protection.DeleteColumnsLocked);
				if (protection.DeleteRowsLocked != defaultValue.DeleteRowsLocked)
					WriteBoolValue("deleteRows", protection.DeleteRowsLocked);
				if (protection.FormatCellsLocked != defaultValue.FormatCellsLocked)
					WriteBoolValue("formatCells", protection.FormatCellsLocked);
				if (protection.FormatColumnsLocked != defaultValue.FormatColumnsLocked)
					WriteBoolValue("formatColumns", protection.FormatColumnsLocked);
				if (protection.FormatRowsLocked != defaultValue.FormatRowsLocked)
					WriteBoolValue("formatRows", protection.FormatRowsLocked);
				if (protection.InsertColumnsLocked != defaultValue.InsertColumnsLocked)
					WriteBoolValue("insertColumns", protection.InsertColumnsLocked);
				if (protection.InsertHyperlinksLocked != defaultValue.InsertHyperlinksLocked)
					WriteBoolValue("insertHyperlinks", protection.InsertHyperlinksLocked);
				if (protection.InsertRowsLocked != defaultValue.InsertRowsLocked)
					WriteBoolValue("insertRows", protection.InsertRowsLocked);
				if (protection.ObjectsLocked != defaultValue.ObjectsLocked && protection.SheetLocked != defaultValue.SheetLocked)
					WriteBoolValue("objects", protection.ObjectsLocked);
				if (protection.PivotTablesLocked != defaultValue.PivotTablesLocked)
					WriteBoolValue("pivotTables", protection.PivotTablesLocked);
				if (protection.ScenariosLocked != defaultValue.ScenariosLocked && protection.SheetLocked != defaultValue.SheetLocked)
					WriteBoolValue("scenarios", protection.ScenariosLocked);
				if (protection.SelectLockedCellsLocked != defaultValue.SelectLockedCellsLocked)
					WriteBoolValue("selectLockedCells", protection.SelectLockedCellsLocked);
				if (protection.SelectUnlockedCellsLocked != defaultValue.SelectUnlockedCellsLocked)
					WriteBoolValue("selectUnlockedCells", protection.SelectUnlockedCellsLocked);
				if (protection.SortLocked != defaultValue.SortLocked)
					WriteBoolValue("sort", protection.SortLocked);
				ExportProtectionPasswordHashes(protection.Credentials, false);
			}
			finally {
				WriteShEndElement();
			}
		}
		void ExportProtectionPasswordHashes(ProtectionCredentials credentials, bool isWorkbookPrefixExistsInAttributes) {
			if (credentials.IsEmpty)
				return;
			CryptographicProtectionInfo byHash = credentials.CryptographicProtection;
			if (byHash != null) {
				ExportProtectionHash(byHash, isWorkbookPrefixExistsInAttributes);
			}
			ProtectionByPasswordVerifier byPassWordVerifier = credentials.PasswordVerifier;
			if(byPassWordVerifier != null) {
				ExportPasswordVerifier(byPassWordVerifier, isWorkbookPrefixExistsInAttributes);
				return;
			}
			ProtectionByWorkbookRevisions byRevisionPasswordVerifier = credentials.RevisionProtection;
			if (byRevisionPasswordVerifier != null)
				ExportRevisionPasswordVerifier(byRevisionPasswordVerifier);
		}
		void ExportProtectionHash(CryptographicProtectionInfo info, bool isWorkbookPrefixExistsInAttributes) {
			if (info.IsEmpty)
				return;
			string hash = Convert.ToBase64String(info.HashValue);
			string salt = Convert.ToBase64String(info.SaltValue);
			string algorithmName = DevExpress.XtraSpreadsheet.Import.OpenXml.OpenXmlImporter.HashAlgorithmTypeTranslationTable.GetStringValue(info.AlgorithmType, Utils.HashAlgorithmType.Sha512);
			string spinCountAttrName = (isWorkbookPrefixExistsInAttributes) ? "workbookSpinCount" : "spinCount";
			string saltAttrName = (isWorkbookPrefixExistsInAttributes) ? "workbookSaltValue" : "saltValue";
			string hashAttrName = (isWorkbookPrefixExistsInAttributes) ? "workbookHashValue" : "hashValue";
			string algorithmAttrName = (isWorkbookPrefixExistsInAttributes) ? "workbookAlgorithmName" : "algorithmName";
			WriteIntValue(spinCountAttrName, info.SpinCount);
			WriteStringValue(saltAttrName, salt);
			WriteStringValue(hashAttrName, hash);
			WriteStringValue(algorithmAttrName, algorithmName);
		}
		void ExportPasswordVerifier(ProtectionByPasswordVerifier info, bool isWorkbookPrefixExistsInAttrName) {
			if (info.IsEmpty)
				return;
			string attr = isWorkbookPrefixExistsInAttrName ? "workbookPassword" : "password";
			WriteSTUnsignedShortHexValue(attr, info.Value);
		}
		void ExportRevisionPasswordVerifier(ProtectionByWorkbookRevisions info) {
			if (info.IsEmpty)
				return;
			WriteSTUnsignedShortHexValue("revisionsPassword", info.Value);
		}
		protected internal virtual void GenerateSheetProtectedRanges(ModelProtectedRangeCollection ranges) {
			if (ranges.Count == 0)
				return;
			WriteShStartElement("protectedRanges");
			try {
				ranges.ForEach(GenerateSheetProtectedRange);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void GenerateSheetProtectedRange(ModelProtectedRange range) {
			WriteShStartElement("protectedRange");
			try {
				WriteStringValue("name", range.Name);
				WriteStSqref(range.CellRange, "sqref");
				ExportProtectionPasswordHashes(range.Credentials, false);
				if (!string.IsNullOrEmpty(range.SecurityDescriptor))
					WriteStringValue("securityDescriptor", range.SecurityDescriptor);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual void GenerateDataSheetContent() {
			WriteShStartElement("sheetData");
			try {
				ExportRows();
			}
			finally {
				WriteShEndElement();
			}
		}
		#region Sheets
		protected internal override void ExportSheet(Worksheet sheet) {
			ExportSheetCore(sheet as InternalSheetBase);
		}
		protected internal override void ExportSheet(Chartsheet sheet) {
			ExportSheetCore(sheet as InternalSheetBase);
		}
		void ExportSheetCore(InternalSheetBase sheet) {
			WriteShStartElement("sheet");
			try {
				WriteShStringValue("name", sheet.Name);
				WriteShIntValue("sheetId", sheet.SheetId);
				if (sheet.VisibleState != SheetVisibleState.Visible)
					WriteShStringValue("state", visibilityTypeTable[sheet.VisibleState]);
				string sheetRelationId = PopulateSheetTables(sheet);
				WriteStringAttr(RelsPrefix, "id", null, sheetRelationId);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal virtual string PopulateSheetTables(Worksheet sheet) {
			return PopulateSheetTables(sheet as InternalSheetBase);
		}
		string PopulateSheetTables(InternalSheetBase sheet) {
			int counterValue;
			string sheetKind;
			bool shouldExportSheetRelations;
			if (sheet.SheetType == SheetType.ChartSheet) {
				counterValue = ++this.chartsheetCounter;
				sheetKind = "chartsheet";
				shouldExportSheetRelations = ShouldExportWorksheetRelations(sheet as Chartsheet);
			}
			else {
				counterValue = ++this.sheetCounter;
				sheetKind = "worksheet";
				shouldExportSheetRelations = ShouldExportWorksheetRelations(sheet as Worksheet);
			}
			string sheetRelationId = GenerateIdByCollection(Builder.WorkbookRelations);
			string fileName = String.Format("sheet{0}.xml", counterValue);
			string sheetRelationTarget = sheetKind + "s/" + fileName;
			Builder.WorkbookRelations.Add(new OpenXmlRelation(sheetRelationId, sheetRelationTarget, XlsxPackageBuilder.RelsWorksheetNamespace));
			string sheetPath = string.Format("xl\\{0}s\\{1}", sheetKind, fileName); 
			string sheetName = sheet.Name;
			SheetNamesTable.Add(sheetName, sheetPath);
			if (shouldExportSheetRelations) {
				WorksheetRelationPathTable.Add(sheetName, String.Format(@"xl\{0}s\_rels\sheet{1}.xml.rels", sheetKind, counterValue));
				SheetRelationsTable.Add(sheetName, new OpenXmlRelationCollection());
			}
			Builder.OverriddenContentTypes.Add("/xl/" + sheetRelationTarget, "application/vnd.openxmlformats-officedocument.spreadsheetml." + sheetKind + "+xml");
			return sheetRelationId;
		}
		protected internal virtual string PopulateSheetTables(Chartsheet sheet) {
			return PopulateSheetTables(sheet as InternalSheetBase);
		}
		protected internal virtual bool ShouldExportWorksheetRelations(Worksheet sheet) {
			return sheet.Tables.Count > 0 || HasExternalHyperlink(sheet.Hyperlinks) ||
				sheet.Comments.Count > 0 || ShouldExportVmlDrawing(sheet)|| ShouldExportDrawing(sheet) ||
				sheet.PivotTables.Count > 0;
		}
		protected internal virtual bool ShouldExportWorksheetRelations(Chartsheet sheet) {
			return  ShouldExportDrawing(sheet);
		}
		protected internal bool HasExternalHyperlink(ModelHyperlinkCollection hyperlinks) {
			foreach (ModelHyperlink hyperlink in hyperlinks) {
				if (hyperlink.IsExternal)
					return true;
			}
			return false;
		}
		#endregion
	}
}
