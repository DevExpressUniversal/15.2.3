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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxSpreadsheet.Internal.JSONTypes;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.XtraSpreadsheet.Layout.Engine;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Commands;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.Web.ASPxSpreadsheet.Internal {
	public class TileLocation {
		public Dictionary<TilePosition, CellRange> VisibleTiles { get; private set; }
		public TileLocation(Dictionary<TilePosition, CellRange> visibleTiles) {
			VisibleTiles = visibleTiles;
		}
	}
	internal enum AutoFilterImageType {
		None,
		DropDown,
		Filtered,
		Ascending,
		Descending,
		FilteredAndAscending,
		FilteredAndDescending
	}
	[Flags]
	enum TablePropertiesMask : byte {
		None			 = 0,
		FilteredOrSorted = 1,
		HeaderRow		= 2,
		TotalRow		 = 4,
		BandedColumns	= 8,
		BandedRows	   = 16,
		FirstColumn	  = 32,
		LastColumn	   = 64,
		IsFilterEnabled  = 128
	}
	[Flags]
	enum ColumnFilterTypeMask : byte {
		None	= 0,
		Text	= 1,
		Number  = 2,
		Date	= 4
	}
	internal static class ResponseQueryStringKeys {
		public const string NewWorkSessionGuid = "newSessionGuid";
		public const string ExpiredTiles = "outdatedTiles";
		public const string NewTiles = "newTiles";
		public const string VisibleRange = "visibleRange";
		public const string HeaderOffsetHeight = "headerOffsetHeight";
		public const string HeaderOffsetWidths = "headerOffsetWidths";
		public const string ScrollAnchor = "scrollAnchor";
		public const string NewSheetLoading = "loadNewSheet";
		public const string LastFilledCell = "lastFilledCell";
		public const string DocumentFileName = "documentName"; 
		public const string ActiveSheetIndex = "sheetIndex";
		public const string History = "history"; 
		public const string SheetCalculation = "calculation";
		public const string Selection = "selection";
		public const string ChartInfo = "charts";
		public const string DocumentSheetTabInfo = "tabControlInfo";
		public const string RibbonControlUpdateRequired = "updateRibbonControl";
		public const string IsDocumentReadOnly = "readOnly";
		public const string CellSize = "defaultCellSize";
		public const string DocumentModifiedFlag = "modified";
		public const string PrintOptions = "printOptions";
		public const string SheetLocked = "sheetLocked";
		public const string DocumentLocked = "workbookLocked";
		public const string IsGridLinesHidden = "gridLinesHidden";
		public const string PageOrientation = "pageOrientation";
		public const string PageMargins = "pageMargins";
		public const string PaperKind = "paperKind";
		public const string ClearMatrixCacheOnResponseReceived = "clearGridCache";
		public const string FrozenCell = "frozenCell";
		public const string TableInfos = "tableInfos";
		public const string AutoFilters = "autoFilters";
		public const string sheetFilterState = "sheetFilterState";
		public const string ListValidations = "listValidations";
		public const string InputMessages = "inputMessages";
		public const string Validation = "validation";
		public const string InvalidDataCircles = "invalidDataCircles";
		public const string ValidationConfirm = "validationConfirm";
		public const string LastMessage = "lastMessage";
		public const string FrozenPaneSettings = "fp";
		public const string RemoveSheetConfirmation = "removeSheetConfirm";
	}
	public class SpreadsheetRenderHelper {
		protected internal const int
			DefaultCellWidth = 64,
			DefaultCellHeight = 20;
		public const int
			TileColCount = 30,
			TileRowCount = 30,
			WindowHorizontalPadding = TileRowCount / 2,
			WindowVerticalPadding = TileColCount / 2,
			columnHeaderBottomBorderSize = 1;
		public SpreadsheetRenderHelper(SpreadsheetWorkSession workSession, NameValueCollection commandContext) {
			WorkSession = workSession;
			Layout = new WebDocumentLayout(Model);
			CachedTiles = new List<CellRange>(); 
			ModifiedTiles = new Dictionary<TilePosition, CellRange>();
			VisibleTiles = new Dictionary<TilePosition, CellRange>();
			VisibleTilesByPanes = new Dictionary<PanesType, TileLocation>();
			CommandContext = commandContext;
			CommandHelper = new SpreadsheetCommandHelper(CommandContext);
		}
		public SpreadsheetWorkSession WorkSession { get; private set; }
		public WebSpreadsheetControl WebSpreadsheetControl { get { return WorkSession.WebSpreadsheetControl; } }
		public DocumentModel Model { get { return WorkSession.DocumentModel; } }
		public Worksheet ActiveSheet { get { return Model.ActiveSheet; } }
		public WebDocumentLayout Layout { get; private set; }
		internal SpreadsheetCommandHelper CommandHelper { get; private set; }
		ValidationHelper validationHelper;
		internal ValidationHelper ValidationHelper {
			get {
				if(validationHelper == null)
					validationHelper = new ValidationHelper(ActiveSheet);
				return validationHelper;
			}
		}
		TableHelper tableHelper;
		internal TableHelper TableHelper {
			get {
				if(tableHelper == null)
					tableHelper = new TableHelper(Layout, ActiveSheet);
				return tableHelper;
			}
		}
		internal NameValueCollection CommandContext { get; private set; }
		WebDocumentLayoutCalculator webCalculator;
		public WebDocumentLayoutCalculator WebCalculator {
			get {
				if(webCalculator == null)
					webCalculator = new WebDocumentLayoutCalculator(Layout, ActiveSheet);
				return webCalculator;
			}
			set { webCalculator = value; }
		}
		public int WindowClientWidth { get; private set; }
		public int WindowClientHeight { get; private set; }
		public int WindowClientScrollTop { get; private set; }
		public int WindowClientScrollLeft { get; private set; }
		public List<CellRange> CachedTiles { get; private set; }
		public Dictionary<TilePosition, CellRange> ModifiedTiles { get; private set; }
		public Dictionary<TilePosition, CellRange> VisibleTiles { get; private set; }
		public Dictionary<PanesType, TileLocation> VisibleTilesByPanes { get; private set; }
		public Dictionary<PanesType, CellRange> VisibleRange { get; private set; }
		public CellPosition ScrollAnchor { get; private set; }
		protected Rectangle GetClientWindowRect() {
			int x = WindowClientScrollLeft >= 0 ? WindowClientScrollLeft : 0;
			int y = WindowClientScrollTop >= 0 ? WindowClientScrollTop : 0;
			return new Rectangle(x, y, WindowClientWidth, WindowClientHeight);
		}
		public void LoadClientState(bool anotherDocumentOpened) {
			LoadClientWindowRect();
			if(!anotherDocumentOpened) {
				LoadScrollAnchor();
				ApplyScrollAnchor();
			}
			ApplyClientViewBounds(GetClientWindowRect());
			if(!anotherDocumentOpened) {
				SelectionHelper.LoadClientSelection(ActiveSheet, CommandContext);
				OnLoadAfterSelection();
			}
		}
		private void LoadClientWindowRect() {
			WindowClientWidth = ParseInt(CommandContext["width"]);
			WindowClientHeight = ParseInt(CommandContext["height"]);
			WindowClientScrollTop = ParseInt(CommandContext["scrollTop"]);
			WindowClientScrollLeft = ParseInt(CommandContext["scrollLeft"]);
		}
		private void LoadScrollAnchor() {
			var anchorCol = ParseInt(CommandContext["scrollAnchor.col"]);
			var anchorRow = ParseInt(CommandContext["scrollAnchor.row"]);
			bool clientScrollAnchorPassed = anchorCol >= 0 && anchorRow >= 0;
			if(clientScrollAnchorPassed)
				ScrollAnchor = new CellPosition(anchorCol, anchorRow);
			else
				ScrollAnchor = CellPosition.InvalidValue;
		}
		private void OnLoadAfterSelection() {
			LoadCachedTilesForCurrentActiveSheet();
		}
		private void ApplyClientViewBounds(Rectangle webClientViewBounds) {
			WebSpreadsheetControl.SetViewBounds(webClientViewBounds);
		}
		private void LoadCachedTilesForCurrentActiveSheet() {
			bool serverActiveSheetIndexChanged = false;
			bool clientActiveSheetIndexChanged = false;
			if(CommandContext["sheetIndex"] != null) {
				int sheetIndex = ParseInt(CommandContext["sheetIndex"]);
				if(sheetIndex >= 0) {
					if(SheetIndexExists(sheetIndex))
						Model.ActiveSheet = Model.GetVisibleSheets().Where(i => i.SheetId == sheetIndex).First();
					serverActiveSheetIndexChanged = true; 
				} else
					clientActiveSheetIndexChanged = true;
			}
			bool serverAndClientActiveSheetsAreTheSame = !serverActiveSheetIndexChanged && !clientActiveSheetIndexChanged;
			if(serverAndClientActiveSheetsAreTheSame)
				LoadCachedTiles(CommandContext["cachedTiles"]);
		}
		private bool SheetIndexExists(int sheetIndex) {
			return Model.GetSheetById(sheetIndex) != null;
		}
		public void ApplyScrollAnchor() {
			if(ScrollAnchor.IsValid)
				Model.ActiveSheet.ActiveView.ScrolledTopLeftCell = ScrollAnchor;
		}
		protected void UpdateScrollAnchorPosition() {
			if(!Model.ActiveSheet.IsColumnVisible(ScrollAnchor.Column)) {
				int visibleColumnIndex = GetNextVisibleIndex(ScrollAnchor.Column + 1, true);
				ScrollAnchor = new CellPosition(visibleColumnIndex, ScrollAnchor.Row);
			}
			if(!Model.ActiveSheet.IsRowVisible(ScrollAnchor.Row)) {
				int visibleRowIndex = GetNextVisibleIndex(ScrollAnchor.Row + 1, false);
				ScrollAnchor = new CellPosition(ScrollAnchor.Column, visibleRowIndex);
			}
		}
		public void OnAfterCommandProcessing() {
			if(CommandHelper.ReselectAfterCommand())
				SelectionHelper.LoadClientSelection(ActiveSheet, CommandContext);
		}
		protected void LoadCachedTiles(string cachedTilesString) {
			if(string.IsNullOrEmpty(cachedTilesString))
				return;
			List<TilePosition> tiles = new List<TilePosition>();
			foreach(var tileInfoString in cachedTilesString.Split(';')) {
				var indices = tileInfoString.Split('|');
				var row = ParseInt(indices[0]);
				var col = ParseInt(indices[1]);
				if(row < 0 || col < 0)
					continue;
				tiles.Add(new TilePosition(col, row));
			}
		}
		public string GetRenderResult() {
			var result = GetRenderHashtableResult(new Hashtable());
			return HtmlConvertor.ToJSON(result);
		}
		public string GetRenderResult(Hashtable json) {
			var result = GetRenderHashtableResult(json);
			return HtmlConvertor.ToJSON(result);
		}
		public Hashtable GetRenderHashtableResult(Hashtable json) {
			EnsureLayout();
			json[ResponseQueryStringKeys.ExpiredTiles] = ModifiedTiles.Select(t => new JSONTilePosition(t.Key.TileRow, t.Key.TileColumn)).ToArray();
			json[ResponseQueryStringKeys.VisibleRange] = GetVisibleRanges();
			json[ResponseQueryStringKeys.NewTiles] = GetTileRenderResult();
			json[ResponseQueryStringKeys.LastFilledCell] = GetLastFilledCell();
			json[ResponseQueryStringKeys.DocumentFileName] = Path.GetFileName(WorkSession.DocumentPathOrID);
			json[ResponseQueryStringKeys.ActiveSheetIndex] = Model.ActiveSheet.SheetId;
			json[ResponseQueryStringKeys.History] = new JSONHistory(Model.History.CanUndo, Model.History.CanRedo);
			json[ResponseQueryStringKeys.SheetCalculation] = new JSONCalculation(Model.Properties.CalculationOptions.CalculationMode.ToString(), CommandHelper.IsCalculationModeUpdateRequired());
			json[ResponseQueryStringKeys.Selection] = SelectionHelper.RenderSelectionToJSON(ActiveSheet);
			json[ResponseQueryStringKeys.ChartInfo] = HtmlConvertor.ToJSON(GetActiveSheetChartsInfo(Model.ActiveSheet.DrawingObjects));
			json[ResponseQueryStringKeys.DocumentModifiedFlag] = WorkSession.WebSpreadsheetControl.Modified;
			if(Model.ActiveSheet.IsFrozen())
				json[ResponseQueryStringKeys.FrozenPaneSettings] = GetFrozenPanesSettings();  
			if(CommandHelper.IsCommandCanChangeScrollPositionOrVisibleWindowSize()) {
				json[ResponseQueryStringKeys.HeaderOffsetHeight] = GetHeaderHeight();
				json[ResponseQueryStringKeys.HeaderOffsetWidths] = GetHeaderWidths();
			}
			bool scrollAnchorUpdateRequed = Model.ActiveSheet.IsFrozen() && !(Model.ActiveSheet.IsColumnVisible(ScrollAnchor.Column) && Model.ActiveSheet.IsRowVisible(ScrollAnchor.Row));
			if(scrollAnchorUpdateRequed)
				UpdateScrollAnchorPosition();
			if(CommandHelper.IsCommandCanChangeScrollPositionOrVisibleWindowSize() || CommandHelper.UpdateCellCommandsExecuted() || scrollAnchorUpdateRequed)
				json[ResponseQueryStringKeys.ScrollAnchor] = new JSONCellPosition(ScrollAnchor);
			if(CommandHelper.IsSheetLoading())
				json[ResponseQueryStringKeys.NewSheetLoading] = true;
			if(TileCellProtectionHelper.IsSheetLocked(Model.ActiveSheet))
				json[ResponseQueryStringKeys.SheetLocked] = true;
			if(TileCellProtectionHelper.IsWorkbookLocked(Model))
				json[ResponseQueryStringKeys.DocumentLocked] = true;
			if(!Model.ActiveSheet.ActiveView.ShowGridlines)
				json[ResponseQueryStringKeys.IsGridLinesHidden] = true;
			if(CommandHelper.IsTabControlUpdateRequired())
				json[ResponseQueryStringKeys.DocumentSheetTabInfo] = new JSONTabControlInfo(
					HtmlConvertor.ToJSON(GetSheetsInfo(Model.GetVisibleSheets())),
					HtmlConvertor.ToJSON(GetSheetsInfo(Model.GetHiddenSheets()))
				);
			if(CommandHelper.IsRibbonControlUpdateRequired())
				json[ResponseQueryStringKeys.RibbonControlUpdateRequired] = true;
			if(WorkSession.WebSpreadsheetControl.InnerControl != null && WorkSession.WebSpreadsheetControl.InnerControl.IsReadOnly)
				json[ResponseQueryStringKeys.IsDocumentReadOnly] = true;
			if(CommandHelper.IsDefaultCellSizeUpdateRequired())
				json[ResponseQueryStringKeys.CellSize] = new JSONCellSize(
					SpreadsheetRenderHelper.GetDefautlColumnWidthInPixels(Model),
					SpreadsheetRenderHelper.GetDefautlRowHeightInPixels(Model)
				);
			if(CommandHelper.IsPaperOrientationUpdateRequired())
				json[ResponseQueryStringKeys.PageOrientation] = Model.ActiveSheet.PrintSetup.Orientation.ToString();
			if(CommandHelper.IsPageMarginsUpdateRequired())
				json[ResponseQueryStringKeys.PageMargins] = GetCurrentPageMargins();
			if(CommandHelper.IsPaperKindUpdateRequired())
				json[ResponseQueryStringKeys.PaperKind] = Model.ActiveSheet.PrintSetup.PaperKind.ToString();
			if(CommandHelper.IsPrintOptionsUpdateRequired())
				json[ResponseQueryStringKeys.PrintOptions] = new JSONPrintOptions(Model.ActiveSheet.PrintSetup.PrintGridLines, Model.ActiveSheet.PrintSetup.PrintHeadings);
			if(CommandHelper.IsGridCacheUpdateRequired())
				json[ResponseQueryStringKeys.ClearMatrixCacheOnResponseReceived] = true;
			if(TableHelper.TableExistsOnSheet())
				json[ResponseQueryStringKeys.TableInfos] = TableHelper.GetTablesCallbackResult();
			if(TableHelper.AutoFiltersExistsOnSheet())
				json[ResponseQueryStringKeys.AutoFilters] = TableHelper.GetAutoFiltersCallbackResult();
			json[ResponseQueryStringKeys.sheetFilterState] = TableHelper.GetSheetFilterStateCallbackResult();
			ValidationHelper validationHelper = new ValidationHelper(ActiveSheet);
			Hashtable validationResult = validationHelper.GetValidationResult(VisibleTiles);
			if(validationResult != null && validationResult.Count > 0)
				json[ResponseQueryStringKeys.Validation] = validationResult;
			string lastMessage = WorkSession.WebSpreadsheetControl.LastMessage;
			if(!string.IsNullOrEmpty(lastMessage))
				json[ResponseQueryStringKeys.LastMessage] = lastMessage;
			return json;
		}
		protected List<object> GetActiveSheetChartsInfo(DrawingObjectsCollection drawings) {
			List<object> chartsInfo = new List<object>();
			foreach(IDrawingObject drawing in drawings) {
				if(drawing is Chart) {
					Chart chart = drawing as Chart;
					chartsInfo.Add(new JSONChartInfo(
						chart.IndexInCollection,
						GetChartRange(chart),
						GetChartViewType(chart),
						chart.Title.Text.PlainText,
						GetAxisTitleText(chart, true),
						GetAxisTitleText(chart, false)
					));
				}
			}
			return chartsInfo;
		}
		protected string GetChartRange(Chart chart) {
			ChartReferencedRanges dataReferencedRanges = chart.GetReferencedRanges();
			CellRangeBase dataRange = dataReferencedRanges.GetDataRange();
			if(dataRange == null)
				return string.Empty;
			dataRange = dataRange.GetWithModifiedPositionType(PositionType.Absolute);
			ParsedExpression rangeExpression = new ParsedExpression();
			WorkbookDataContext context = Model.DataContext;
			BasicExpressionCreator.CreateCellRangeExpression(rangeExpression, dataRange, BasicExpressionCreatorParameter.ShouldCreate3d, OperandDataType.Default, context);
			return rangeExpression.BuildExpressionString(context);
		}
		protected string GetChartViewType(Chart chart) {
			BarChartViewBase barView = chart.Views[0] as BarChartViewBase;
			if(barView != null) {
				switch(barView.Grouping) {
					case BarChartGrouping.Clustered:
					case BarChartGrouping.Standard:
						return barView.BarDirection.ToString() + BarChartGrouping.Clustered.ToString();
					default:
						return barView.BarDirection.ToString() + BarChartGrouping.Stacked.ToString();
				}
			}
			return chart.Views[0].ViewType.ToString().Replace("3D", "");
		}
		protected AxisBase GetAxis(Chart chart, bool isHorizontal) {
			foreach(AxisBase axis in chart.PrimaryAxes) {
				if(axis.AxisType != AxisDataType.Series) {
					if(isHorizontal) {
						if(axis.Position == AxisPosition.Top || axis.Position == AxisPosition.Bottom)
							return axis;
					} else {
						if(axis.Position == AxisPosition.Left || axis.Position == AxisPosition.Right)
							return axis;
					}
				}
			}
			return null;
		}
		protected string GetAxisTitleText(Chart chart, bool isHorizontal) {
			string axisTitle = string.Empty;
			AxisBase axis = GetAxis(chart, isHorizontal);
			if(axis != null)
				axisTitle = axis.Title.Text.PlainText;
			return axisTitle;
		}
		protected List<JSONSheetInfo> GetSheetsInfo(List<Worksheet> sheetCollection) {
			List<JSONSheetInfo> sheetInfo = new List<JSONSheetInfo>();
			for(int sheetIndex = 0; sheetIndex < sheetCollection.Count; sheetIndex++) {
				sheetInfo.Add(new JSONSheetInfo(
				   sheetCollection[sheetIndex].Name,
				   sheetCollection[sheetIndex].SheetId
				));
			}
			return sheetInfo;
		}
		#region DefaultSizes
		internal static int GetDefautlRowHeightInPixels(SpreadsheetWorkSession spreadhseetWorkSession) {
			if(spreadhseetWorkSession != null) {
				return GetDefautlRowHeightInPixels(spreadhseetWorkSession.DocumentModel);
			}
			return SpreadsheetRenderHelper.DefaultCellHeight;
		}
		internal static int GetDefautlRowHeightInPixels(DocumentModel documentModel) {
			float heightInLayoutUnits = documentModel.GetService<IColumnWidthCalculationService>().CalculateDefaultRowHeight(documentModel.ActiveSheet);
			float heightInPixels = documentModel.LayoutUnitConverter.LayoutUnitsToPixelsF(heightInLayoutUnits, DocumentModel.DpiY);
			return (int)heightInPixels;
		}
		internal static int GetDefautlColumnWidthInPixels(SpreadsheetWorkSession spreadhseetWorkSession) {
			if(spreadhseetWorkSession != null) {
				return GetDefautlColumnWidthInPixels(spreadhseetWorkSession.DocumentModel);
			}
			return SpreadsheetRenderHelper.DefaultCellWidth;
		}
		internal static int GetDefautlColumnWidthInPixels(DocumentModel documentModel) {
			float widthInLayoutUnits = documentModel.GetService<IColumnWidthCalculationService>().CalculateDefaultColumnWidth(documentModel.ActiveSheet, documentModel.MaxDigitWidth, documentModel.MaxDigitWidthInPixels);
			float widthInPixels = documentModel.LayoutUnitConverter.LayoutUnitsToPixelsF(widthInLayoutUnits, DocumentModel.DpiX);
			return (int)widthInPixels;
		}
		#endregion
		protected string GetCurrentPageMargins() {
			var cmdNormal = new XtraSpreadsheet.Commands.Internal.PageSetupMarginsNormalCommand(WorkSession.WebSpreadsheetControl);
			if(Model.ActiveSheet.Margins.Info.Equals(cmdNormal.PredefinedValue))
				return "normal";
			var cmdWide = new XtraSpreadsheet.Commands.Internal.PageSetupMarginsWideCommand(WorkSession.WebSpreadsheetControl);
			if(Model.ActiveSheet.Margins.Info.Equals(cmdWide.PredefinedValue))
				return "wide";
			var cmdNarrow = new XtraSpreadsheet.Commands.Internal.PageSetupMarginsNarrowCommand(WorkSession.WebSpreadsheetControl);
			if(Model.ActiveSheet.Margins.Info.Equals(cmdNarrow.PredefinedValue))
				return "narrow";
			return "custome";
		}
		protected object GetFrozenPanesSettings() {
			return new JSONFrozenPaneSettings(WebCalculator.FrozenPaneWidth, WebCalculator.FrozenPaneHeight, GetFrozenMode(), GetVisibleFrozenCell(), GetVisibleTopLeftCell());
		}
		protected CellPosition GetVisibleFrozenCell() {
			CellPosition frozenCell = Model.ActiveSheet.ActiveView.FrozenCell;
			if(!Model.ActiveSheet.IsRowVisible(frozenCell.Row)) {
				int visibleRowIndex = GetNextVisibleIndex(frozenCell.Row + 1, false);
				frozenCell = new CellPosition(frozenCell.Column, visibleRowIndex);
			}
			if(!Model.ActiveSheet.IsColumnVisible(frozenCell.Column)) {
				int visibleColumnIndex = GetNextVisibleIndex(frozenCell.Column + 1, true);
				frozenCell = new CellPosition(visibleColumnIndex, frozenCell.Row);
			}
			return frozenCell;
		}
		protected CellPosition GetVisibleTopLeftCell() {
			CellPosition topLeftCell = Model.ActiveSheet.ActiveView.TopLeftCell;
			if(!Model.ActiveSheet.IsRowVisible(topLeftCell.Row)) {
				int visibleRowIndex = GetNextVisibleIndex(topLeftCell.Row + 1, false);
				topLeftCell = new CellPosition(topLeftCell.Column, visibleRowIndex);
			}
			if(!Model.ActiveSheet.IsColumnVisible(topLeftCell.Column)) {
				int visibleColumnIndex = GetNextVisibleIndex(topLeftCell.Column + 1, true);
				topLeftCell = new CellPosition(visibleColumnIndex, topLeftCell.Row);
			}
			return topLeftCell;
		}
		protected int GetNextVisibleIndex(int startIndex, bool isCol) {
			int maxIndex = isCol ? IndicesChecker.MaxColumnIndex : IndicesChecker.MaxRowIndex,
				visibleIndex = startIndex;
			Worksheet activeSheet = Model.ActiveSheet;
			while((visibleIndex < maxIndex) && ((isCol && !activeSheet.IsColumnVisible(visibleIndex)) || (!isCol && !activeSheet.IsRowVisible(visibleIndex)))) {
				visibleIndex++;
			}
			return visibleIndex;
		}
		protected int GetFrozenMode() {
			int mode = 2;
			if(Model.ActiveSheet.ActiveView.IsOnlyColumnsFrozen())
				mode= 1;
			else if(Model.ActiveSheet.ActiveView.IsOnlyRowsFrozen())
				mode = 0;
			return mode;
		}
		protected JSONPanesRenderInfo GetTileRenderResult() {
			JSONPanesRenderInfo panesRenderInfo = new JSONPanesRenderInfo();
			ArrayList gridTiles,
			  columnHeaderTiles,
			  rowHeaderTiles;
			TableHelper.ProcessTables(VisibleTiles);
			foreach(KeyValuePair<PanesType, TileLocation> paneTiles in VisibleTilesByPanes) {
				gridTiles = new ArrayList();
				columnHeaderTiles = new ArrayList();
				rowHeaderTiles = new ArrayList();
				foreach(TilePosition tilePosition in paneTiles.Value.VisibleTiles.Keys)
					gridTiles.Add(GetGridTileCallbackResult(tilePosition, paneTiles.Key));
				List<TilePosition> columnHeaderIndices = null,
					rowHeaderIndices = null;
				columnHeaderIndices = GetHeaderIndices(paneTiles.Value.VisibleTiles, true);
				for(var i = 0; i < columnHeaderIndices.Count; i++)
					columnHeaderTiles.Add(GetHeaderTileCallbackResult(columnHeaderIndices[i], true));
				rowHeaderIndices = GetHeaderIndices(paneTiles.Value.VisibleTiles, false);
				for(var i = 0; i < rowHeaderIndices.Count; i++)
					rowHeaderTiles.Add(GetHeaderTileCallbackResult(rowHeaderIndices[i], false));
				JSONTileInfo tilesRenderInfo = new JSONTileInfo(gridTiles, columnHeaderTiles, rowHeaderTiles);
				panesRenderInfo.SetFieldByType(tilesRenderInfo, paneTiles.Key);
			}
			return panesRenderInfo;
		}
		protected JSONPanesVisibleRange GetVisibleRanges() {
			if(ActiveSheet.ActiveView.IsFrozen()) {
				if(ActiveSheet.ActiveView.IsOnlyColumnsFrozen())
					return new JSONPanesVisibleRange(GetVisibleRangeJSONTileIndicesRange(VisibleTilesByPanes[PanesType.MainPane].VisibleTiles),
						GetVisibleRangeJSONTileIndicesRange(VisibleTilesByPanes[PanesType.BottomLeftPane].VisibleTiles), true);
				if(ActiveSheet.ActiveView.IsOnlyRowsFrozen())
					return new JSONPanesVisibleRange(GetVisibleRangeJSONTileIndicesRange(VisibleTilesByPanes[PanesType.MainPane].VisibleTiles),
						GetVisibleRangeJSONTileIndicesRange(VisibleTilesByPanes[PanesType.TopRightPane].VisibleTiles), false);
				return new JSONPanesVisibleRange(GetVisibleRangeJSONTileIndicesRange(VisibleTilesByPanes[PanesType.MainPane].VisibleTiles),
						GetVisibleRangeJSONTileIndicesRange(VisibleTilesByPanes[PanesType.TopRightPane].VisibleTiles),
						GetVisibleRangeJSONTileIndicesRange(VisibleTilesByPanes[PanesType.BottomLeftPane].VisibleTiles),
						GetVisibleRangeJSONTileIndicesRange(VisibleTilesByPanes[PanesType.FrozenPane].VisibleTiles));
			}
			return new JSONPanesVisibleRange(GetVisibleRangeJSONTileIndicesRange(VisibleTilesByPanes[PanesType.MainPane].VisibleTiles));
		}
		protected JSONTileIndicesRange GetVisibleRangeJSONTileIndicesRange(Dictionary<TilePosition, CellRange> VisibleTiles) {
			int top = Int32.MaxValue,
				right = -1,
				bottom = -1,
				left = Int32.MaxValue;
			foreach(TilePosition tilePosition in VisibleTiles.Keys) {
				if(top > tilePosition.TileRow)
					top = tilePosition.TileRow;
				if(bottom < tilePosition.TileRow)
					bottom = tilePosition.TileRow;
				if(left > tilePosition.TileColumn)
					left = tilePosition.TileColumn;
				if(right < tilePosition.TileColumn)
					right = tilePosition.TileColumn;
			}
			return new JSONTileIndicesRange(
				top,
				right,
				bottom,
				left
			);
		}
		protected object GetHeaderWidths() {
			var list = new List<int>();
			var maxNumberCount = ActiveSheet.MaxRowCount.ToString().Length;
			var row = 1;
			for(var i = 0; i < maxNumberCount; i++, row *= 10)
				list.Add(WebCalculator.CalculateHeaderOffset(row).Width + 1); 
			return list;
		}
		protected int GetHeaderHeight() {
			return Layout.HeaderOffset.Height + columnHeaderBottomBorderSize;
		}
		protected JSONCellPosition GetLastFilledCell() {
			CellPosition lastVisibleCell = WebCalculator.GetLastVisibleCell();
			return new JSONCellPosition(lastVisibleCell);
		}
		protected virtual object GetGridTileCallbackResult(TilePosition tilePosition, PanesType paneType) {
			var result = new Hashtable();
			var tileRange = VisibleTiles[tilePosition];
			var page = Layout.TilePages[tilePosition];
			int colIndex = tilePosition.TileColumn;
			int rowIndex = tilePosition.TileRow;
			GridTileControl ctrl = new GridTileControl(this, page, colIndex, rowIndex, paneType);
			result["html"] = RenderProvider.GetRenderResult(ctrl);
			result["colIndex"] = colIndex;
			result["rowIndex"] = rowIndex;
			result["values"] = TileValuesHelper.GetValues(page);
			result["editTexts"] = TileValuesHelper.GetEditingTexts(page);
			result["complexes"] = TileComplexBoxHelper.GetTileLocalMergedCells(Layout, tilePosition, page);
			result["cellEditable"] = TileCellProtectionHelper.GetUnlockedCellPositionList(page, tilePosition);
			return result;
		}
		protected int[] ProcessPageGrid(PageGrid gridLines, Func<PageGridItem, int> getValue) {
			var list = new List<int>(gridLines.ActualCount);
			for(var i = gridLines.ActualFirstIndex; i <= gridLines.ActualLastIndex; i++)
				list.Add(getValue(gridLines[i]));
			return list.ToArray();
		}
		protected int[] GetGridSize(PageGrid gridLines) {
			return ProcessPageGrid(gridLines, delegate(PageGridItem gridLine) { return gridLine.Extent; });
		}
		protected int[] GetGridModelIndices(PageGrid gridLines) {
			return ProcessPageGrid(gridLines, delegate(PageGridItem gridLine) { return gridLine.ModelIndex; });
		}
		protected object GetHeaderTileCallbackResult(TilePosition tilePosition, bool isColumn) { 
			var result = new Hashtable();
			var tileRange = VisibleTiles[tilePosition];
			var page = Layout.TilePages[tilePosition];
			var index = isColumn ? tilePosition.TileColumn : tilePosition.TileRow;
			result["index"] = index;
			if(isColumn) {
				var widths = GetGridSize(page.GridColumns);
				result["widths"] = widths;
				result["totalWidth"] = widths.Sum();
			} else {
				var heights = GetGridSize(page.GridRows);
				result["heights"] = heights;
				result["totalHeight"] = heights.Sum();
			}
			result["modelIndices"] = isColumn ? GetGridModelIndices(page.GridColumns) : GetGridModelIndices(page.GridRows);
			return result;
		}
		protected List<TilePosition> GetHeaderIndices(Dictionary<TilePosition, CellRange> visibleTiles, bool isColumn) {
			var hash = new HashSet<int>();
			var indices = new List<TilePosition>();
			foreach(KeyValuePair<TilePosition, CellRange> visibleTile in visibleTiles) {
				TilePosition tilePosition = visibleTile.Key;
				CellRange tileRange = visibleTile.Value;
				CellPosition rangeTopLeftCell = tileRange.TopLeft;
				int tileIndex = isColumn ? rangeTopLeftCell.Column / TileColCount : rangeTopLeftCell.Row / TileRowCount;
				if(!hash.Contains(tileIndex)) {
					hash.Add(tileIndex);
					indices.Add(tilePosition);
				}
			}
			return indices;
		}
		protected virtual void EnsureLayout() {
			VisibleRange = CalculateVisibleRange();
			CalculateVisibleTiles();
			WebCalculator.CalculateLayout(VisibleTiles);
		}
		protected Dictionary<PanesType, CellRange> CalculateVisibleRange() {
			Rectangle clientBounds = GetClientWindowRect();
			Dictionary<PanesType, CellRange> visibleRange = null;
			if(CommandHelper.IsCommandCanChangeScrollPosition())
				ScrollAnchor = Model.ActiveSheet.ActiveView.ScrolledTopLeftCell;
			else if(ScrollAnchor.IsValid)
				ApplyScrollAnchor();
			else {
				visibleRange = WebCalculator.CalculateWebBoundingRanges(clientBounds);
				ScrollAnchor = WebCalculator.Anchor.CellPosition;
			}
			if(visibleRange == null) {
				visibleRange = WebCalculator.CalculateBoundingRanges(ScrollAnchor, clientBounds);
			}
			visibleRange = ExpandCellRangesSmart(visibleRange);
			return visibleRange;
		}
		protected Dictionary<PanesType, CellRange> ExpandCellRangesSmart(Dictionary<PanesType, CellRange> visibleRanges) {
			Dictionary<PanesType, CellRange> extendedVisibleRange = new Dictionary<PanesType, CellRange>();
			foreach(KeyValuePair<PanesType, CellRange> visibleRange in visibleRanges) {
				KeyValuePair<PanesType, CellRange> extendedRange = ExpandCellRange(visibleRange);
				extendedVisibleRange.Add(extendedRange.Key, extendedRange.Value);
			}
			return extendedVisibleRange;
		}
		protected KeyValuePair<PanesType, CellRange> ExpandCellRange(KeyValuePair<PanesType, CellRange> range) {
			PanesType paneType = range.Key;
			CellRange visibleRange = range.Value;
			switch(paneType) {
				case PanesType.MainPane:
					visibleRange.RightColumnIndex += WindowHorizontalPadding;
					visibleRange.BottomRowIndex += WindowVerticalPadding;
					visibleRange.LeftColumnIndex = Math.Max(visibleRange.LeftColumnIndex - WindowHorizontalPadding, ActiveSheet.ActiveView.FrozenCell.Column);
					visibleRange.TopRowIndex = Math.Max(visibleRange.TopRowIndex - WindowVerticalPadding, ActiveSheet.ActiveView.FrozenCell.Row);
					break;
				case PanesType.BottomLeftPane:
					visibleRange.BottomRowIndex += WindowVerticalPadding;
					visibleRange.TopRowIndex = Math.Max(visibleRange.TopRowIndex - WindowVerticalPadding, ActiveSheet.ActiveView.FrozenCell.Row);
					break;
				case PanesType.TopRightPane:
					visibleRange.RightColumnIndex += WindowHorizontalPadding;
					visibleRange.LeftColumnIndex = Math.Max(visibleRange.LeftColumnIndex - WindowHorizontalPadding, ActiveSheet.ActiveView.FrozenCell.Column);
					break;
			}
			return new KeyValuePair<PanesType, CellRange>(paneType, visibleRange);
		}
		protected void CalculateVisibleTiles() {
			var cachedNotModifiedTiles = CachedTiles.Except(ModifiedTiles.Values);
			Dictionary<TilePosition, CellRange> visibleTileDictionary = new Dictionary<TilePosition, CellRange>();
			Dictionary<PanesType, TileLocation> visibleTilesByPanes = new Dictionary<PanesType, TileLocation>();
			foreach(KeyValuePair<PanesType, CellRange> range in VisibleRange) {
				Dictionary<TilePosition, CellRange> visibleTiles = ActiveSheet.WebRanges.GetTilesByRange(range.Value);
				visibleTilesByPanes.Add(range.Key, new TileLocation(visibleTiles));
				foreach(KeyValuePair<TilePosition, CellRange> tile in visibleTiles) {
					if(!visibleTileDictionary.ContainsKey(tile.Key))
						visibleTileDictionary.Add(tile.Key, tile.Value);
				}
			}
			VisibleTiles = visibleTileDictionary.Where(x => !cachedNotModifiedTiles.Contains(x.Value))
				.ToDictionary(
					x => x.Key,
					x => x.Value);
			foreach(KeyValuePair<PanesType, TileLocation> panesVisibleTiles in visibleTilesByPanes) {
				TileLocation tileLocation = new TileLocation(panesVisibleTiles.Value.VisibleTiles.Where(x => !cachedNotModifiedTiles.Contains(x.Value))
				.ToDictionary(
					x => x.Key,
					x => x.Value));
				VisibleTilesByPanes.Add(panesVisibleTiles.Key, tileLocation);
			}
		}
		public void CalculateScrollAnchor(SpreadsheetCommandId commandId) {
			if(ScrollAnchor.IsValid) {
				CellPosition scrolledCell = CellPosition.InvalidValue;
				if(commandId == SpreadsheetCommandId.ViewFreezeTopRow)
					scrolledCell = new CellPosition(ScrollAnchor.Column, ScrollAnchor.Row + 1);
				else if(commandId == SpreadsheetCommandId.ViewFreezeFirstColumn)
					scrolledCell = new CellPosition(ScrollAnchor.Column + 1, ScrollAnchor.Row);
				if(scrolledCell.IsValid)
					Model.ActiveSheet.ActiveView.ScrolledTopLeftCell = scrolledCell;
			}
		}
		internal static int ParseInt(string str) {
			int val;
			if(int.TryParse(str, out val))
				return val;
			return -1;
		}
		internal static IList<CellRangeBase> GetRangesFromLTRB(ArrayList ltrbs, ICellTable worksheet) {
			IList<CellRangeBase> ranges = new List<CellRangeBase>();
			foreach(ArrayList ltrb in ltrbs)
				ranges.Add(ConvertLTRBToCellRange(ltrb, worksheet));
			return ranges;
		}
		internal static List<List<int>> SerializeCellRanges<T>(List<T> ranges) where T : CellRangeBase {
			var result = new List<List<int>>();
			foreach(var range in ranges)
				result.Add(ConvertCellRangeToLTRB(range));
			return result;
		}
		internal static CellRangeBase ConvertLTRBToCellRange(ArrayList ltrb, ICellTable worksheet) {
			return new CellRange(worksheet, new CellPosition((int)ltrb[0], (int)ltrb[1]), new CellPosition((int)ltrb[2], (int)ltrb[3]));
		}
		internal static List<int> ConvertCellRangeToLTRB(CellRangeBase range) {
			return new List<int>() { 
				range.TopLeft.Column,
				range.TopLeft.Row, 
				range.BottomRight.Column,
				range.BottomRight.Row
			};
		}
	}
	public static class RenderProvider {
		public static WebControl CreateDiv(Rectangle rect, string className) {
			var div = CreateDivCore();
			return SetControlRect(div, rect, className);
		}
		public static WebControl CreateDivWithSizesOnly(Rectangle rect, string className, int gridLineSizeCorrection) {
			var div = CreateDivCore();
			var correctedRect = new Rectangle(new Point(), rect.Size);
			correctedRect.Height -= gridLineSizeCorrection;
			return SetControlSizes(div, correctedRect, className);
		}
		public static WebControl CreateDivWithSizesOnly(Rectangle rect, string className) {
			var div = CreateDivCore();
			return SetControlSizes(div, rect, className);
		}
		static WebControl CreateDivCore() {
			return RenderUtils.CreateDiv();
		}
		public static WebControl SetControlRect(WebControl control, Rectangle rect, string className) {
			return SetControlStyles(control, rect, className, new HtmlTextWriterStyle[] {
				HtmlTextWriterStyle.Left, HtmlTextWriterStyle.Top,
				HtmlTextWriterStyle.Width, HtmlTextWriterStyle.Height
			});
		}
		public static WebControl SetControlSizes(WebControl control, Rectangle rect, string className) {
			return SetControlStyles(control, rect, className, new HtmlTextWriterStyle[] {
				HtmlTextWriterStyle.Width, HtmlTextWriterStyle.Height
			});
		}
		static WebControl SetControlStyles(WebControl control, Rectangle rect, string className, IEnumerable<HtmlTextWriterStyle> rectStyles) {
			control.CssClass += " " + className;
			foreach(var style in rectStyles)
				AssignRectStyle(control, rect, style);
			return control;
		}
		public static void AssignRectStyle(WebControl control, Rectangle rect, HtmlTextWriterStyle style) {
			int styleValue = 0;
			switch(style) {
				case HtmlTextWriterStyle.Left:
					styleValue = rect.Left;
					break;
				case HtmlTextWriterStyle.Top:
					styleValue = rect.Top;
					break;
				case HtmlTextWriterStyle.Width:
					styleValue = rect.Width;
					break;
				case HtmlTextWriterStyle.Height:
					styleValue = rect.Height;
					break;
			}
			control.Style.Add(style, string.Format("{0}px", styleValue));
		}
		public static WebControl CreateDiv(Rectangle rect, string className, Color color) {
			var div = CreateDiv(rect, className);
			div.BorderColor = color;
			return div;
		}
		public static string GetRenderResult(Control control) {
			using(var sw = new StringWriter(CultureInfo.InvariantCulture)) {
				using(var writer = new HtmlTextWriter(sw, null)) {
					writer.NewLine = string.Empty;
					control.RenderControl(writer);
					return sw.ToString();
				}
			}
		}
	}
	public class TableHelper {
		protected Worksheet ActiveSheet { get; private set; }
		protected Dictionary<TilePosition, CellRange> VisibleTiles { get; private set; }
		protected WebDocumentLayout Layout { get; private set; }
		internal List<AutoFilterJSON> AutoFilters { get; private set; }
		internal List<TableInfoJSON> TableInfos { get; private set; }
		internal bool IsSheetFilterEnabled { get; private set; }
		internal bool IsSheetFilterApplied { get; private set; }
		public TableHelper(WebDocumentLayout layout, Worksheet activeSheet) {
			Layout = layout;
			ActiveSheet = activeSheet;
			TableInfos = new List<TableInfoJSON>();
			AutoFilters = new List<AutoFilterJSON>();
		}
		public List<object> GetTablesCallbackResult() {
			var result = new List<object>();
			foreach(TableInfoJSON tableInfo in TableInfos)
				result.Add(tableInfo.Serialize());
			return result;
		}
		public List<object> GetAutoFiltersCallbackResult() {
			var result = new List<object>();
			foreach(AutoFilterJSON filter in AutoFilters)
				result.Add(filter.Serialize());
			return result;
		}
		public Hashtable GetSheetFilterStateCallbackResult() {
			var result = new Hashtable();
			result["applied"] = IsSheetFilterApplied;
			result["enabled"] = IsSheetFilterEnabled;
			return result;
		}
		public void ProcessTables(Dictionary<TilePosition, CellRange> visibleTiles) {
			VisibleTiles = visibleTiles;
			ClearTablesInfo();
			foreach(var table in GetVisibleTables())
				ProcessTable(table);
			ProcessSheetAutoFilter();
		}
		public bool TableExistsOnSheet() {
			return TableInfos.Count > 0;
		}
		public bool AutoFiltersExistsOnSheet() {
			return AutoFilters.Count > 0;
		}
		protected List<XtraSpreadsheet.Model.Table> GetVisibleTables() {
			List<XtraSpreadsheet.Model.Table> visibleTables = new List<XtraSpreadsheet.Model.Table>();
			foreach(CellRange tileRange in VisibleTiles.Values)
				visibleTables.AddRange(ActiveSheet.Tables.GetItems(tileRange, true));
			return visibleTables;
		}
		protected void ProcessSheetAutoFilter() {
			var sheetFilter = ActiveSheet.AutoFilter;
			ProcessAutoFilter(sheetFilter);
			if(sheetFilter != null) {
				IsSheetFilterEnabled = sheetFilter.Enabled;
				IsSheetFilterApplied = sheetFilter.IsNonDefault;
			}
		}
		protected void ClearTablesInfo() {
			TableInfos.Clear();
			AutoFilters.Clear();
			IsSheetFilterEnabled = IsSheetFilterApplied = false;
		}
		protected void ProcessTable(XtraSpreadsheet.Model.Table table) {
			var properties = PackTableProperties(table);
			var range = SpreadsheetRenderHelper.ConvertCellRangeToLTRB(table.Range);
			var name = table.Name;
			TableInfos.Add(new TableInfoJSON(name, properties, range));
			if(IsTableHeaderVisible(table))
				ProcessAutoFilter(table.AutoFilter);
		}
		protected bool ShouldRenderAutoFilterImage(XtraSpreadsheet.Model.Table table) {
			if(!(table.AutoFilter.Enabled && table.ShowAutoFilterButton))
				return false;
			return true;
		}
		protected bool IsTableHeaderVisible(XtraSpreadsheet.Model.Table table) {
			var range = table.AutoFilter.Range;
			if(range == null)
				return false;
			bool leftColumnVisible = IsAutoFilterColumnVisible(table.AutoFilter, range.TopLeft.Column);
			bool rightColumnVisible = IsAutoFilterColumnVisible(table.AutoFilter, range.BottomRight.Column);
			return leftColumnVisible || rightColumnVisible;
		}
		protected bool IsAutoFilterColumnVisible(AutoFilterBase filter, int filterColumnIndex) {
			bool isColumnInRange = false;
			bool isRowInRange = false;
			foreach(TilePosition tilePosition in Layout.TilePages.Keys) {
				CellRange range = filter.Range;
				var page = Layout.TilePages[tilePosition];
				int columnGridIndex = page.GridColumns.LookupItem(filterColumnIndex);
				if(columnGridIndex >= 0)
					isColumnInRange = true;
				int rowGridIndex = page.GridRows.LookupItem(range.TopLeft.Row);
				if(rowGridIndex >= 0)
					isRowInRange = true;
				if(isColumnInRange && isRowInRange)
					return true;
			}
			return false;
		}
		protected byte PackTableProperties(XtraSpreadsheet.Model.Table table) {
			TablePropertiesMask packedProperties = TablePropertiesMask.None;
			if(IsAutoFilterSortedOrFiltered(table.AutoFilter))
				packedProperties |= TablePropertiesMask.FilteredOrSorted;
			if(table.HasHeadersRow)
				packedProperties |= TablePropertiesMask.HeaderRow;
			if(table.HasTotalsRow)
				packedProperties |= TablePropertiesMask.TotalRow;
			if(table.ShowColumnStripes)
				packedProperties |= TablePropertiesMask.BandedColumns;
			if(table.ShowRowStripes)
				packedProperties |= TablePropertiesMask.BandedRows;
			if(table.ShowFirstColumn)
				packedProperties |= TablePropertiesMask.FirstColumn;
			if(table.ShowLastColumn)
				packedProperties |= TablePropertiesMask.LastColumn;
			if(table.AutoFilter.Enabled)
				packedProperties |= TablePropertiesMask.IsFilterEnabled;
			return (byte)packedProperties;
		}
		protected bool IsAutoFilterSortedOrFiltered(AutoFilterBase filter) {
			if(!HasAutoFilter(filter))
				return false;
			if(!filter.SortState.IsDefault)
				return true;
			foreach(AutoFilterColumn column in filter.FilterColumns)
				if(column.IsNonDefault)
					return true;
			return false;
		}
		protected bool HasAutoFilter(AutoFilterBase filter) {
			if(filter == null)
				return false;
			return filter.Enabled && filter.FilterColumns.Count > 0;
		}
		protected void ProcessAutoFilter(AutoFilterBase filter) {
			if(!HasAutoFilter(filter))
				return;
			CellRange range = filter.Range;
			int from = range.TopLeft.Column;
			int to = range.BottomRight.Column;
			int rowIndex = range.TopLeft.Row;
			bool isDefaultColumnFilter = true;
			for(int i = from; i <= to; i++) {
				var imageType = GetAutoFilterImageType(filter, i);
				var columnType = GetAutoFilterColumnType(filter, i);
				isDefaultColumnFilter = !filter.FilterColumns[i - from].IsNonDefault;
				AutoFilters.Add(new AutoFilterJSON(i, rowIndex, isDefaultColumnFilter, columnType, imageType.ToString()));
			}
		}
		internal AutoFilterImageType GetAutoFilterImageType(AutoFilterBase filter, int columnIndex) {
			var filterColumnIndex = columnIndex - filter.Range.TopLeft.Column;
			return CalculateAutoFilterImage(filter, filterColumnIndex);
		}
		protected byte GetAutoFilterColumnType(AutoFilterBase filter, int columnIndex) {
			int rowIndex = filter.Range.TopLeft.Row;
			ColumnFilterTypeMask columnType = ColumnFilterTypeMask.None;
			if(filter != null) {
				XtraSpreadsheet.Commands.Internal.SuggestedFilterTypeInfo filterTypeInfo = new XtraSpreadsheet.Commands.Internal.SuggestedFilterTypeInfo();
				filterTypeInfo.Calculate(filter, columnIndex);
				var cellPosition = new CellPosition(columnIndex, rowIndex);
				var filterColumnIndex = columnIndex - filter.Range.TopLeft.Column;
				var filterColumn = filter.FilterColumns[filterColumnIndex];
				if(filterTypeInfo.IsTextFilter)
					columnType |= ColumnFilterTypeMask.Text;
				if(filterTypeInfo.IsNumberFilter)
					columnType |= ColumnFilterTypeMask.Number;
				if(filterTypeInfo.IsDateTimeFilter)
					columnType |= ColumnFilterTypeMask.Date;
			}
			return (byte)columnType;
		}
		protected bool ColumnHasAutoFilterImage(AutoFilterColumn filterColumn) {
			return filterColumn.ShowFilterButton && !filterColumn.HiddenAutoFilterButton;
		}
		internal AutoFilterImageType CalculateAutoFilterImage(AutoFilterBase filter, int filterColumnIndex) {
			AutoFilterColumn filterColumn = filter.FilterColumns[filterColumnIndex];
			SortCondition sortCondition = GetFilterSortCondition(filter, filterColumnIndex);
			if(!ColumnHasAutoFilterImage(filterColumn))
				return AutoFilterImageType.None;
			if(filterColumn.IsNonDefault) {
				if(sortCondition == null)
					return AutoFilterImageType.Filtered;
				return sortCondition.Descending ? AutoFilterImageType.FilteredAndDescending : AutoFilterImageType.FilteredAndAscending;
			} else {
				if(sortCondition == null)
					return AutoFilterImageType.DropDown;
				return sortCondition.Descending ? AutoFilterImageType.Descending : AutoFilterImageType.Ascending;
			}
		}
		protected SortCondition GetFilterSortCondition(AutoFilterBase filter, int filterColumnIndex) {
			SortState sortState = filter.SortState;
			int count = sortState.SortConditions.Count;
			for(int i = 0; i < count; i++) {
				SortCondition condition = sortState.SortConditions[i];
				if(condition.SortReference.TopLeft.Column == filter.Range.TopLeft.Column + filterColumnIndex)
					return condition;
			}
			return null;
		}
	}
	public class ValidationHelper {
		protected Worksheet ActiveSheet { get; private set; }
		protected Dictionary<TilePosition, CellRange> VisibleTiles { get; private set; }
		public ValidationHelper(Worksheet activeSheet) {
			ActiveSheet = activeSheet;			
		}
		public Hashtable GetValidationResult(Dictionary<TilePosition, CellRange> visibleTiles) {
			VisibleTiles = visibleTiles;
			var result = new Hashtable();
			var listValidations = GetListValidations();
			if(listValidations.Count > 0)
				result[ResponseQueryStringKeys.ListValidations] = listValidations;
			var inputMessages = GetInputMessages();
			if(inputMessages.Count > 0)
				result[ResponseQueryStringKeys.InputMessages] = inputMessages;
			var invalidCircles = GetInvalidDataCircles();
			if(invalidCircles.Count > 0)
				result[ResponseQueryStringKeys.InvalidDataCircles] = invalidCircles;
			return result;
		}
		protected List<List<int>> GetInvalidDataCircles() {
			var circles = new List<List<int>>();
			foreach(var invalidDataCircle in ActiveSheet.InvalidDataCircles) {
				List<int> colAndRow = new List<int>();
				colAndRow.Add(invalidDataCircle.ColumnIndex);
				colAndRow.Add(invalidDataCircle.RowIndex);
				circles.Add(colAndRow);
			}
			return circles;
		}
		protected List<List<int>> GetListValidations() {
			return GetLTRBValidations((item) => item.Type == DataValidationType.List);
		}
		protected List<List<int>> GetInputMessages() {
			return GetLTRBValidations((item) => item.ShowInputMessage && !String.IsNullOrEmpty(item.Prompt));
		}
		protected List<List<int>> GetLTRBValidations(Predicate<DataValidation> match) {
			List<List<int>> validations = new List<List<int>>();
			foreach(DataValidation dataValidation in GetValidationsForTiles(match))
				validations.AddRange(GetLTRBListFromCellRange(dataValidation.CellRange));
			return validations;
		}
		protected List<DataValidation> GetValidationsForTiles(Predicate<DataValidation> match) {
			List<DataValidation> dataValidations = new List<DataValidation>();
			foreach(CellRange tileRange in VisibleTiles.Values) {
				List<DataValidation> validationsForTile = ActiveSheet.DataValidations.FindAll((item) => match(item) && item.CellRange.Intersects(tileRange));
				foreach(DataValidation validation in validationsForTile)
					if(!dataValidations.Contains(validation))
						dataValidations.Add(validation);
			}
			return dataValidations;
		}
		public static List<List<int>> GetLTRBListFromCellRange(CellRangeBase cellRange) {
			List<List<int>> list = new List<List<int>>();
			CellUnion union = cellRange as CellUnion;
			if(union == null)
				list.Add(SpreadsheetRenderHelper.ConvertCellRangeToLTRB(cellRange));
			else
				list.AddRange(SpreadsheetRenderHelper.SerializeCellRanges(union.InnerCellRanges));
			return list;
		}
	}
	public class SelectionUtils {
		SheetViewSelection selection;
		public SelectionUtils(SheetViewSelection selection) {
			this.selection = selection;
		}
		protected SheetViewSelection Selection {
			get { return selection; }
		}
		public bool HasEntireRowSelected {
			get {
				return Selection.ActiveRange.TopLeft.Column == 0 && Selection.ActiveRange.BottomRight.Column == Selection.Sheet.MaxColumnCount - 1;
			}
		}
		public bool HasEntireColumnSelected {
			get {
				return Selection.ActiveRange.TopLeft.Row == 0 && Selection.ActiveRange.BottomRight.Row == Selection.Sheet.MaxRowCount - 1;
			}
		}
		public int LeftEntireColSelected {
			get {
				if(HasEntireColumnSelected)
					return Selection.ActiveRange.TopLeft.Column;
				return -1;
			}
		}
		public int RightEntireColSelected {
			get {
				if(HasEntireColumnSelected)
					return Selection.ActiveRange.BottomRight.Column;
				return -1;
			}
		}
		public int TopEntireRowSelected {
			get {
				if(HasEntireRowSelected)
					return Selection.ActiveRange.TopLeft.Row;
				return -1;
			}
		}
		public int BottomEntireRowSelected {
			get {
				if(HasEntireRowSelected)
					return Selection.ActiveRange.BottomRight.Row;
				return -1;
			}
		}
	}
	public static class SelectionHelper {
		const string QueryStringKey_SelectLeftColIndex = "selectLeftColIndex";
		const string QueryStringKey_SelectTopRowIndex = "selectTopRowIndex";
		const string QueryStringKey_SelectRightColIndex = "selectRightColIndex";
		const string QueryStringKey_SelectBottomRowIndex = "selectBottomRowIndex";
		const string QueryStringKey_ActiveCellColIndex = "activeCellColIndex";
		const string QueryStringKey_ActiveCellRowIndex = "activeCellRowIndex";
		const string QueryStringKey_EntireColsSelected = "entireColsSelected";
		const string QueryStringKey_EntireRowsSelected = "entireRowsSelected";
		const string QueryStringKey_DrawingBoxIndex = "drawingBoxIndex";
		const string QueryStringKey_AllSelected = "allSelected";
		const string QueryStringKey_MultiSelection = "multiSelection";
		const string QueryStringKey_Ranges = "ranges";
		static int ParseInt(string str) {
			return SpreadsheetRenderHelper.ParseInt(str);
		}
		static bool HasClientSelection(NameValueCollection nameValueCollection, string selectionKeySample) {
			return !string.IsNullOrEmpty(nameValueCollection[selectionKeySample]) && ParseInt(nameValueCollection[selectionKeySample]) >= 0;
		}
		public static void LoadClientSelection(Worksheet ActiveSheet, NameValueCollection nameValueCollection) {
			if(HasClientSelection(nameValueCollection, QueryStringKey_SelectLeftColIndex)) {
				int selectLeftColIndex = ParseInt(nameValueCollection[QueryStringKey_SelectLeftColIndex]);
				int selectTopRowIndex = ParseInt(nameValueCollection[QueryStringKey_SelectTopRowIndex]);
				int selectRightColIndex = ParseInt(nameValueCollection[QueryStringKey_SelectRightColIndex]);
				int selectBottomRowIndex = ParseInt(nameValueCollection[QueryStringKey_SelectBottomRowIndex]);
				int activeCellColIndex = ParseInt(nameValueCollection[QueryStringKey_ActiveCellColIndex]);
				int activeCellRowIndex = ParseInt(nameValueCollection[QueryStringKey_ActiveCellRowIndex]);
				CellPosition topLeft = new CellPosition(selectLeftColIndex, selectTopRowIndex);
				CellPosition bottomRight = new CellPosition(selectRightColIndex, selectBottomRowIndex);
				CellPosition activeCell = new CellPosition(activeCellColIndex, activeCellRowIndex);
				CellRange cellRange = new CellRange(ActiveSheet, topLeft, bottomRight);
				bool entireColsSelected = !string.IsNullOrEmpty(nameValueCollection[QueryStringKey_EntireColsSelected]);
				bool entireRowsSelected = !string.IsNullOrEmpty(nameValueCollection[QueryStringKey_EntireRowsSelected]);
				bool multiSelection = !string.IsNullOrEmpty(nameValueCollection[QueryStringKey_MultiSelection]);
				if(entireColsSelected)
					cellRange = CellIntervalRange.CreateColumnInterval(ActiveSheet, selectLeftColIndex, PositionType.Absolute, selectRightColIndex, PositionType.Absolute);
				if(entireRowsSelected)
					cellRange = CellIntervalRange.CreateRowInterval(ActiveSheet, selectTopRowIndex, PositionType.Absolute, selectBottomRowIndex, PositionType.Absolute);
				if(multiSelection)
					ActiveSheet.Selection.SetSelectedRanges(SpreadsheetRenderHelper.GetRangesFromLTRB(HtmlConvertor.FromJSON<ArrayList>(nameValueCollection[QueryStringKey_Ranges]), ActiveSheet), false);
				else
					ActiveSheet.Selection.SetSelection(cellRange, activeCell, !entireColsSelected && !entireRowsSelected);
			} else if(!string.IsNullOrEmpty(nameValueCollection[QueryStringKey_DrawingBoxIndex])) {
				int selectedDrawingBoxIndex = ParseInt(nameValueCollection[QueryStringKey_DrawingBoxIndex]);
				ActiveSheet.Selection.SetSelectedDrawingIndex(selectedDrawingBoxIndex);
			} else if(!string.IsNullOrEmpty(nameValueCollection[QueryStringKey_AllSelected])) {
				int activeCellColIndex = ParseInt(nameValueCollection[QueryStringKey_ActiveCellColIndex]);
				int activeCellRowIndex = ParseInt(nameValueCollection[QueryStringKey_ActiveCellRowIndex]);
				CellPosition activeCell = new CellPosition(activeCellColIndex, activeCellRowIndex);
				ActiveSheet.Selection.SelectAll(activeCell, true);
			}
		}
		public static Hashtable RenderSelectionToJSON(Worksheet ActiveSheet) {
			return RenderSelectionToHash(ActiveSheet);
		}
		public static Hashtable RenderSelectionToHash(Worksheet ActiveSheet) {
			var hash = new Hashtable();
			if(ActiveSheet.Selection.IsAllSelected) {
				hash["allSelected"] = true;
			} else if(ActiveSheet.Selection.IsDrawingSelected) {
				hash["drawingBoxIndexSelected"] = true;
				hash["drawingBoxIndex"] = ActiveSheet.Selection.SelectedDrawingIndexes[0];
			} else {
				SelectionUtils selectionUtils = new SelectionUtils(ActiveSheet.Selection);
				if(selectionUtils.HasEntireColumnSelected) {
					hash["entireColsSelected"] = true;
					hash["leftEntireColSelected"] = selectionUtils.LeftEntireColSelected;
					hash["rightEntireColSelected"] = selectionUtils.RightEntireColSelected;
				} else if(selectionUtils.HasEntireRowSelected) {
					hash["entireRowsSelected"] = true;
					hash["topEntireRowSelected"] = selectionUtils.TopEntireRowSelected;
					hash["bottomEntireRowSelected"] = selectionUtils.BottomEntireRowSelected;
				} else {
					hash["left"] = ActiveSheet.Selection.ActiveRange.TopLeft.Column;
					hash["top"] = ActiveSheet.Selection.ActiveRange.TopLeft.Row;
					hash["right"] = ActiveSheet.Selection.ActiveRange.BottomRight.Column;
					hash["bottom"] = ActiveSheet.Selection.ActiveRange.BottomRight.Row;
				}
				hash["activeCol"] = ActiveSheet.Selection.ActiveCell.Column;
				hash["activeRow"] = ActiveSheet.Selection.ActiveCell.Row;
			}
			return hash;
		}
	}
	public static class TileComplexBoxHelper {
		public static object GetTileLocalMergedCells(WebDocumentLayout Layout, TilePosition tilePosition, XtraSpreadsheet.Layout.Page page) {
			List<object> data = new List<object>();
			int topRowTileOffeset = Layout.TileRanges[tilePosition].TopLeft.Row;
			int leftColumnTileOffeset = Layout.TileRanges[tilePosition].TopLeft.Column;
			List<CellRange> mergeCellRanges = Layout.GetMergedCells(page);
			foreach(var mergedCellRange in mergeCellRanges) {
				data.Add(new int[] { 
					mergedCellRange.TopLeft.Column	  , 
					mergedCellRange.TopLeft.Row		 , 
					mergedCellRange.BottomRight.Column  , 
					mergedCellRange.BottomRight.Row	 });
			}
			return data;
		}
	}
	public static class TileCellProtectionHelper {
		public static bool IsWorkbookLocked(DocumentModel Workbook) {
			return Workbook.Properties.Protection.IsLocked;
		}
		public static bool IsSheetLocked(XtraSpreadsheet.Layout.Page page) {
			return IsSheetLocked(page.DocumentLayout.DocumentModel.ActiveSheet);
		}
		public static bool IsSheetLocked(Worksheet sheet) {
			return sheet.Properties.Protection.SheetLocked;
		}
		public static object GetUnlockedCellPositionList(XtraSpreadsheet.Layout.Page page, TilePosition tilePosition) {
			if(!IsSheetLocked(page))
				return null;
			var cellUnlocked = new List<CellPositionJSON>();
			var firstColModelIndex = page.GridColumns.ActualFirst.ModelIndex;
			var lastColModelIndex = page.GridColumns.ActualLast.ModelIndex;
			var firstRowModelIndex = page.GridRows.ActualFirst.ModelIndex;
			var lastRowModelIndex = page.GridRows.ActualLast.ModelIndex;
			for(int colIndex = firstColModelIndex; colIndex <= lastColModelIndex; colIndex++) {
				for(int rowIndex = firstRowModelIndex; rowIndex <= lastRowModelIndex; rowIndex++) {
					if(ProtectionResolver.CanEditActiveSheetCellContent(page, colIndex, rowIndex))
						cellUnlocked.Add(new CellPositionJSON(colIndex, rowIndex));
				}
			}
			if(cellUnlocked.Count == 0)
				return null;
			string str = HtmlConvertor.ToJSON(cellUnlocked);
			return cellUnlocked;
		}
	}
	public static class ProtectionResolver {
		public static bool IsCheckAccessRightsSupported = false;
		public static bool SheetLocked(Worksheet worksheet) {
			return worksheet.Properties.Protection.SheetLocked;
		}
		public static bool CanEditCellContent(Worksheet worksheet, int columnIndex, int rowIndex) {
			return worksheet.CanEditCellContent(columnIndex, rowIndex, IsCheckAccessRightsSupported);
		}
		public static bool CanEditCellContent(Worksheet worksheet, CellPosition CellPosition) {
			return worksheet.CanEditCellContent(CellPosition, IsCheckAccessRightsSupported);
		}
		public static bool CanEditActiveSheetCellContent(XtraSpreadsheet.Layout.Page page, int colIndex, int rowIndex) {
			return CanEditCellContent(page.DocumentLayout.DocumentModel.ActiveSheet, colIndex, rowIndex);
		}
	}
	public static class TileValuesHelper {
		public static Hashtable GetValues(DevExpress.XtraSpreadsheet.Layout.Page page) {
			Hashtable hash = new Hashtable();
			ProcessBoxes(page.Boxes, page, hash, false);
			ProcessBoxes(page.ComplexBoxes, page, hash, false);
			return hash;
		}
		public static Hashtable GetEditingTexts(XtraSpreadsheet.Layout.Page page) {
			Hashtable hash = new Hashtable();
			ProcessBoxes(page.Boxes, page, hash, true);
			ProcessBoxes(page.ComplexBoxes, page, hash, true);
			return hash;
		}
		static void ProcessBoxes(IEnumerable<CellTextBoxBase> textBoxes, DevExpress.XtraSpreadsheet.Layout.Page page, Hashtable hash, bool isEditorText) {
			ICell cell;
			string clientFormatedVariantValue;
			foreach(var textBox in textBoxes) {
				clientFormatedVariantValue = null;
				cell = textBox.GetCell(page.GridColumns, page.GridRows, page.Sheet);
				bool needAllValues = !isEditorText;
				string cellValue = cell.Text;
				if(needAllValues)
					clientFormatedVariantValue = GetClientFormatedVariantValue(cell.Value.Type.ToString(), cellValue);
				else {
					var editingText = DevExpress.XtraSpreadsheet.Internal.CellInplaceEditorHelper.CalculateEditorText(null, cell);
					if(!string.Equals(editingText, cellValue))
						clientFormatedVariantValue = editingText;
				}
				if(clientFormatedVariantValue != null)
					hash[string.Format("{0}|{1}", cell.Position.Column, cell.Position.Row)] = clientFormatedVariantValue;
			}
		}
		static string GetClientFormatedVariantValue(string type, string value) {
			return value == null ? null : string.Format("{0}|{1}", type, value);
		}
	}
	public class SpreadsheetCallbackArgumentsReader : CallbackArgumentsReader {
		public const string CustomCallbackPrefix = "SSPC";
		public const string DocumentCallbackPrefix = "SSDC";
		public SpreadsheetCallbackArgumentsReader(string arguments)
			: base(arguments, new string[] { CustomCallbackPrefix, DocumentCallbackPrefix }) {
		}
		public bool IsCustomCallback { get { return CustomCallbackArg != null; } }
		public bool IsDocumentCallback { get { return DocumentCallbackArg != null; } }
		public bool IsInternalServiceCallback {
			get {
				return !IsCustomCallback && !IsDocumentCallback;
			}
		}
		public string CustomCallbackArg { get { return this[CustomCallbackPrefix]; } }
		public string DocumentCallbackArg { get { return this[DocumentCallbackPrefix]; } }
	}
	public class SpreadsheetDialogCallbackArgumentsReader {
		internal const string SaveImageToServerErrorCallbackPrefix = "ISE";
		internal const string SaveImageToServerNewUrlCallbackPrefix = "ISU";
		internal const string SaveImageToServerCallbackPrefix = "SSITS";
		internal const string ShowDialogCallbackPrefix = "SSCDCP";
		internal const string FileManagerCallbackPrefix = "SSFM";
		internal const string OpenFileCallbackPrefix = "SSOF";
		internal const string SaveFileCallbackPrefix = "SSSF";
		internal const string NewDocumentCallbackPrefix = "SSNF";
		protected Dictionary<string, object> callbackArgs = new Dictionary<string, object>();
		public SpreadsheetDialogCallbackArgumentsReader(string callbackArgs) {
			DictionarySerializer.Deserialize(callbackArgs, this.callbackArgs);
		}
		public bool IsLoadDialogFromRenderCallback { get { return !string.IsNullOrEmpty(DialogFormName); } }
		public bool IsUploadImageCallback { get { return !string.IsNullOrEmpty(ImageUrl); } }
		public string DialogFormName { get { return ClearArgsFromInternalFlag(GetCallbackArg(RenderUtils.DialogFormCallbackStatus)); } }
		public string ImageUrl { get { return ClearArgsFromInternalFlag(GetCallbackArg(SaveImageToServerCallbackPrefix)); } }
		public bool IsFileManagerCallback { get { return !string.IsNullOrEmpty(FileManagerCallbackData); } }
		public string FileManagerCallbackData { get { return ClearArgsFromInternalFlag(GetCallbackArg(FileManagerCallbackPrefix)); } }
		public bool IsNewDocumentCallback { get { return !string.IsNullOrEmpty(ClearArgsFromInternalFlag(GetCallbackArg(NewDocumentCallbackPrefix))); } }
		public bool IsOpenFileCallback { get { return !string.IsNullOrEmpty(OpenFilePath); } }
		public string OpenFilePath { get { return ClearArgsFromInternalFlag(GetCallbackArg(OpenFileCallbackPrefix)); } }
		public bool IsSaveFileCallback { get { return SaveFilePath != null; } }
		public string SaveFilePath { get { return ClearArgsFromInternalFlag(GetCallbackArg(SaveFileCallbackPrefix)); } }
		private string ClearArgsFromInternalFlag(string args) {
			if(!string.IsNullOrEmpty(args) && args.EndsWith(ASPxSpreadsheet.InternalCallbackPostfix))
				args = args.Substring(0, args.LastIndexOf(ASPxSpreadsheet.InternalCallbackPostfix) - 1);
			return args;
		}
		private string GetCallbackArg(string callbackPrefix) {
			return callbackArgs.ContainsKey(callbackPrefix) ? (string)callbackArgs[callbackPrefix] : null;
		}
	}
}
