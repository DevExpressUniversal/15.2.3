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

using System.Collections.Generic;
using System.Globalization;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Export;
using DevExpress.XtraSpreadsheet.Import;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
using DevExpress.Office.Drawing;
using System;
using DevExpress.XtraSpreadsheet.Layout;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Layout.Engine;
using DevExpress.XtraSpreadsheet.Model.CopyOperation;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Model {
	#region DocumentModelWrapper
	public abstract class DocumentModelWrapper : DocumentModel {
		readonly DocumentModel innerDocumentModel;
		protected DocumentModelWrapper(DocumentModel innerDocumentModel)
			: base() {
			this.innerDocumentModel = innerDocumentModel;
				CreateObjectsDependingInnerModel();
		}
		protected internal override void AddServices() {
		}
		protected override void CreateOptions() {
		}
		void CreateObjectsDependingInnerModel() {
			CopyCache(this.Cache, innerDocumentModel.Cache);
			StyleSheet.CopyFrom(innerDocumentModel.StyleSheet);
			innerDocumentModel.CopyDefinedNames(this);
			try {
				BeginUpdate();
				InitializeSheetCollection();
			}
			finally {
				EndUpdate();
			}
			this.DataContext = new WorkbookDataContext(this, innerDocumentModel.DataContext);
			this.OfficeTheme.CopyFrom(innerDocumentModel.OfficeTheme);
			this.LayoutUnit = DocumentLayoutUnit.Document;
		}
		void CopyCache(DocumentCache documentCache, DocumentCache source) {
			documentCache.AutoFilterColumnInfoCache.CopyFrom(source.AutoFilterColumnInfoCache);
			documentCache.AxisInfoCache.CopyFrom(source.AxisInfoCache);
			documentCache.BorderInfoCache.CopyFrom(source.BorderInfoCache);
			documentCache.CalculationOptionsInfoCache.CopyFrom(source.CalculationOptionsInfoCache);
			documentCache.CellAlignmentInfoCache.CopyFrom(source.CellAlignmentInfoCache);
			documentCache.ChartInfoCache.CopyFrom(source.ChartInfoCache);
			documentCache.ChartViewInfoCache.CopyFrom(source.ChartViewInfoCache);
			documentCache.ColorModelInfoCache.CopyFrom(source.ColorModelInfoCache);
			documentCache.ConditionalFormattingInfoCache.CopyFrom(source.ConditionalFormattingInfoCache);
			documentCache.ConditionalFormattingValueCache.CopyFrom(source.ConditionalFormattingValueCache);
			documentCache.DataLabelInfoCache.CopyFrom(source.DataLabelInfoCache);
			documentCache.DataTableInfoCache.CopyFrom(source.DataTableInfoCache);
			documentCache.DataValidationInfoCache.CopyFrom(source.DataValidationInfoCache);
			documentCache.DateGroupingInfoCache.CopyFrom(source.DateGroupingInfoCache);
			documentCache.DisplayUnitInfoCache.CopyFrom(source.DisplayUnitInfoCache);
			documentCache.DrawingObjectInfoCache.CopyFrom(source.DrawingObjectInfoCache);
			documentCache.ErrorBarsInfoCache.CopyFrom(source.ErrorBarsInfoCache);
			documentCache.FillInfoCache.CopyFrom(source.FillInfoCache);
			documentCache.FontInfoCache.CopyFrom(source.FontInfoCache);
			documentCache.GradientFillInfoCache.CopyFrom(source.GradientFillInfoCache);
			documentCache.GradientStopInfoCache.CopyFrom(source.GradientStopInfoCache);
			documentCache.GroupAndOutlinePropertiesInfoCache.CopyFrom(source.GroupAndOutlinePropertiesInfoCache);
			documentCache.HeaderFooterInfoCache.CopyFrom(source.HeaderFooterInfoCache);
			documentCache.MarginInfoCache.CopyFrom(source.MarginInfoCache);
			documentCache.NumberFormatCache.CopyFrom(source.NumberFormatCache);
			documentCache.PictureInfoCache.CopyFrom(source.PictureInfoCache);
			documentCache.PictureOptionsInfoCache.CopyFrom(source.PictureOptionsInfoCache);
			documentCache.PrintSetupInfoCache.CopyFrom(source.PrintSetupInfoCache);
			documentCache.ScalingInfoCache.CopyFrom(source.ScalingInfoCache);
			documentCache.ShapePropertiesInfoCache.CopyFrom(source.ShapePropertiesInfoCache);
			documentCache.ShapeStyleInfoCache.CopyFrom(source.ShapeStyleInfoCache);
			documentCache.SheetFormatInfoCache.CopyFrom(source.SheetFormatInfoCache);
			documentCache.SortConditionInfoCache.CopyFrom(source.SortConditionInfoCache);
			documentCache.SortStateInfoCache.CopyFrom(source.SortStateInfoCache);
			documentCache.TableInfoCache.CopyFrom(source.TableInfoCache);
			documentCache.Transform2DInfoCache.CopyFrom(source.Transform2DInfoCache);
			documentCache.TrendlineInfoCache.CopyFrom(source.TrendlineInfoCache);
			documentCache.View3DInfoCache.CopyFrom(source.View3DInfoCache);
			documentCache.VmlShapeInfoCache.CopyFrom(source.VmlShapeInfoCache);
			documentCache.WindowInfoCache.CopyFrom(source.WindowInfoCache);
			documentCache.WorksheetProtectionInfoCache.CopyFrom(source.WorksheetProtectionInfoCache);
			documentCache.CellFormatCache.CopyFrom(source.CellFormatCache);
			documentCache.TableStyleElementFormatCache.CopyFrom(source.TableStyleElementFormatCache);
		}
		void InitializeSheetCollection() {
			foreach (Worksheet sheet in innerDocumentModel.Sheets) {
				WorksheetWrapper wrapper = new WorksheetWrapper(sheet, this);
				this.Sheets.InnerList.Add(wrapper);
			}
			foreach (WorksheetWrapper wrapper in Sheets) {
				CopyWorksheet(wrapper.InnerSheet, wrapper);
			}
		}
		void CopyWorksheet(Worksheet sourceWorksheet, Worksheet targetWorksheet) {
			var ranges = new SourceTargetRangesForCopy(sourceWorksheet, targetWorksheet);
			CopyWorksheetOperation copyOperation = new CopyWorksheetOperation(ranges);
			copyOperation.GenerateNewTableName = false;
			copyOperation.CopyCellStyles = false;
			copyOperation.PasteSpecialOptions.InnerFlags = ModelPasteSpecialFlags.OtherFormats | ModelPasteSpecialFlags.Comments | ModelPasteSpecialFlags.Tables;
			copyOperation.DisabledHistory = true;
			copyOperation.SuppressChecks = true;
			copyOperation.Execute();
		}
		protected DocumentModel InnerDocumentModel { get { return innerDocumentModel; } }
		public override UIBorderInfoRepository UiBorderInfoRepository { get { return innerDocumentModel.UiBorderInfoRepository; } }
		public override SpreadsheetBehaviorOptions BehaviorOptions { get { return innerDocumentModel.BehaviorOptions; } }
		public override CalculationChain CalculationChain { get { return innerDocumentModel.CalculationChain; } }
		public override CultureInfo Culture { get { return innerDocumentModel.Culture; } set { innerDocumentModel.Culture = value; } }
		public override ModelDocumentApplicationProperties DocumentApplicationProperties { get { return innerDocumentModel.DocumentApplicationProperties; } }
		public override WorkbookCapabilitiesOptions DocumentCapabilities { get { return innerDocumentModel.DocumentCapabilities; } }
		public override ModelDocumentCoreProperties DocumentCoreProperties { get { return innerDocumentModel.DocumentCoreProperties; } }
		public override ModelDocumentCustomProperties DocumentCustomProperties { get { return innerDocumentModel.DocumentCustomProperties; } }
		public override WorkbookExportOptions DocumentExportOptions { get { return innerDocumentModel.DocumentExportOptions; } }
		public override WorkbookImportOptions DocumentImportOptions { get { return innerDocumentModel.DocumentImportOptions; } }
		public override WorkbookSaveOptions DocumentSaveOptions { get { return innerDocumentModel.DocumentSaveOptions; } }
		public override bool EnableFieldNames { get { return innerDocumentModel.EnableFieldNames; } }
		public override WorkbookEventOptions EventOptions { get { return innerDocumentModel.EventOptions; } }
		public override CultureInfo InnerCulture { get { return innerDocumentModel.InnerCulture; } set { innerDocumentModel.InnerCulture = value; } }
		public override bool IsNormalHistory { get { return innerDocumentModel.IsNormalHistory; } }
		public override int MaxFieldSwitchLength { get { return innerDocumentModel.MaxFieldSwitchLength; } }
		public override SpreadsheetPrintOptions PrintOptions { get { return innerDocumentModel.PrintOptions; } }
		public override WorkbookProperties Properties { get { return innerDocumentModel.Properties; } set { innerDocumentModel.Properties = value; } }
		public override SharedStringTable SharedStringTable { get { return innerDocumentModel.SharedStringTable; } }
		public override SheetDefinitionCollection SheetDefinitions { get { return innerDocumentModel.SheetDefinitions; } }
		public override SpreadsheetViewOptions ViewOptions { get { return innerDocumentModel.ViewOptions; } }
		public override Color SkinBackColor { get { return innerDocumentModel.SkinBackColor; } set { innerDocumentModel.SkinBackColor = value; } }
		public override Color SkinForeColor { get { return innerDocumentModel.SkinForeColor; } set { innerDocumentModel.SkinForeColor = value; } }
		public override Color SkinGridlineColor { get { return innerDocumentModel.SkinGridlineColor; } set { innerDocumentModel.SkinGridlineColor = value; } }
		public override CommentAuthorCollection CommentAuthors { get { return innerDocumentModel.CommentAuthors; } }
		public override int ContentVersion { get { return innerDocumentModel.ContentVersion; } set { innerDocumentModel.ContentVersion = value; } }
		public override T GetService<T>() {
			return innerDocumentModel.GetService<T>();
		}
		public override T ReplaceService<T>(T newService) {
			return innerDocumentModel.ReplaceService<T>(newService);
		}
		protected override Office.History.DocumentHistory CreateDocumentHistory() {
			return new DevExpress.Office.History.EmptyHistory(this);
		}
	}
	#endregion
	#region DocumentModelWithOverridenLayoutUnit
	public class DocumentModelWithOverridenLayoutUnit : DocumentModelWrapper {
		public DocumentModelWithOverridenLayoutUnit(DocumentModel innerDocumentModel, DocumentLayoutUnit layoutUnit)
			: base(innerDocumentModel) {
			LayoutUnit = layoutUnit;
		}
	}
	#endregion
	#region WorksheetWrapper
	public class WorksheetWrapper : Worksheet {
		readonly Worksheet innerSheet;
		readonly RowCollectionWrapper rows;
		public WorksheetWrapper(Worksheet innerSheet, DocumentModel documentModel)
			: base(documentModel, innerSheet.Name) {
			this.innerSheet = innerSheet;
			this.rows = new RowCollectionWrapper(this, innerSheet.Rows);
		}
		public Worksheet InnerSheet { get { return innerSheet; } }
		public override IRowCollection Rows { get { return rows; } }
		public override SharedFormulaCollection SharedFormulas { get { return innerSheet.SharedFormulas; } }
		public override ArrayFormulaRangesCollection ArrayFormulaRanges { get { return innerSheet.ArrayFormulaRanges; } }
		public override Margins Margins { get { return innerSheet.Margins; } }
		public override PrintSetup PrintSetup { get { return innerSheet.PrintSetup; } }
		public override HeaderFooterOptions HeaderFooter { get { return innerSheet.HeaderFooter; } }
		public override SheetViewSelection Selection { get { return innerSheet.Selection; } }
		public override SheetViewSelection ReferenceEditSelection { get { return innerSheet.ReferenceEditSelection; } }
		public override CellTagsCache CellTags { get { return innerSheet.CellTags; } }
		public override SharedStringTable SharedStringTable { get { return innerSheet.SharedStringTable; } }
		public override CellValueCache CellValueCache { get { return innerSheet.CellValueCache; } }
		public override int MaxRowCount { get { return innerSheet.MaxRowCount; } set { innerSheet.MaxRowCount = value; } }
		public override int MaxColumnCount { get { return innerSheet.MaxColumnCount; } set { innerSheet.MaxColumnCount = value; } }
		public override bool IsDataSheet { get { return innerSheet.IsDataSheet; } }
		public override IEnumerable<ICellBase> GetExistingCells() {
			return new Enumerable<ICellBase>(new EnumeratorConverter<ICellBase, ICellBase>(innerSheet.GetExistingCells().GetEnumerator(), ConvertCellToCellWrapper));
		}
		ICell ConvertCellToCellWrapper(ICellBase cell) {
			if (cell == null)
				return null;
			return new CellWrapperWithOverridenSheet(cell as ICell, this);
		}
		CellRange ConvertCellRangeToThisSheet(CellRange innerRange) {
			if (!Object.ReferenceEquals(innerRange.Worksheet, innerSheet))
				return innerRange;
			return (CellRange)ConvertCellRangeToSheet(innerRange, this);
		}
		CellRangeBase ConvertCellRangeBaseToInnerSheet(CellRangeBase thisSheetRange) {
			if (!Object.ReferenceEquals(thisSheetRange.Worksheet, this))
				return thisSheetRange;
			return ConvertCellRangeToSheet(thisSheetRange, innerSheet);
		}
		CellRangeBase ConvertCellRangeToSheet(CellRangeBase sourceRange, ICellTable destinationWorksheet) {
			if (sourceRange == null)
				return null;
			return sourceRange.Clone(destinationWorksheet);
		}
		public override ICell GetRegisteredCell(int columnIndex, int rowIndex) {
			return ConvertCellToCellWrapper(base.GetRegisteredCell(columnIndex, rowIndex));
		}
		public override ICell GetCellOrCreate(int columnIndex, int rowIndex) {
			return ConvertCellToCellWrapper(base.GetCellOrCreate(columnIndex, rowIndex));
		}
		public override CellRange GetPrintRange() {
			return ConvertCellRangeToThisSheet(innerSheet.GetPrintRange());
		}
		public override CellRange GetDataRange() {
			return ConvertCellRangeToThisSheet(innerSheet.GetDataRange());
		}
		public override CellRange GetUsedRange() {
			return ConvertCellRangeToThisSheet(innerSheet.GetUsedRange());
		}
		protected internal override Row CreateRowCore(int index) {
			throw new InvalidOperationException("This operation is prohibited for worksheet wrapper.");
		}
		protected internal override ICell CreateCellCore(CellPosition position) {
			throw new InvalidOperationException("This operation is prohibited for worksheet wrapper.");
		}
		internal override void PrepareFormulas() {
			innerSheet.PrepareFormulas();
		}
		public override ArrayFormula CreateArray(string formula, CellRange range) {
			throw new InvalidOperationException("This operation is prohibited for worksheet wrapper.");
		}
		public override SharedFormula LocateFormulaToMultipleCells(CellRangeBase range, CellPosition hostCellPosition, string formula, bool tryCreateShared) {
			throw new InvalidOperationException("This operation is prohibited for worksheet wrapper.");
		}
		public override void RemoveRange(CellRangeBase cellRange, RemoveCellMode mode, bool suppressTableChecks, bool clearFormat, IErrorHandler handler) {
			throw new InvalidOperationException("This operation is prohibited for worksheet wrapper.");
		}
		protected internal override void InsertRange(CellRangeBase cellRange, InsertCellMode mode, InsertCellsFormatMode formatMode, bool suppressTableChecks, IErrorHandler handler) {
			throw new InvalidOperationException("This operation is prohibited for worksheet wrapper.");
		}
		public override void Clear() {
			throw new InvalidOperationException("This operation is prohibited for worksheet wrapper.");
		}
		protected internal override void ClearFormatsCore(CellRangeBase range, bool checkFilteredRanges) {
			throw new InvalidOperationException("This operation is prohibited for worksheet wrapper.");
		}
		public override Color GetTabColor() {
			return innerSheet.GetTabColor();
		}
		public override void SetTabColor(Color color) {
			throw new InvalidOperationException("This operation is prohibited for worksheet wrapper.");
		}
		public override void TryBestFitColumn(int columnIndex, ColumnBestFitMode mode) {
			innerSheet.TryBestFitColumn(columnIndex, mode);
		}
		public override void TryBestFitColumn(ICell cell, ColumnBestFitMode mode) {
			innerSheet.TryBestFitColumn(cell, mode);
		}
		public override void TryBestFitColumn(CellRangeBase range, ColumnBestFitMode mode) {
			innerSheet.TryBestFitColumn(range, mode);
		}
		public override void TryBestFitRow(int rowIndex) {
			innerSheet.TryBestFitRow(rowIndex);
		}
		public override void TryBestFitRow(CellRangeBase range) {
			innerSheet.TryBestFitRow(range);
		}
		public override CellRange GetCurrentRegion(CellRangeBase rangeBase) {
			rangeBase = ConvertCellRangeBaseToInnerSheet(rangeBase);
			return ConvertCellRangeToThisSheet(innerSheet.GetCurrentRegion(rangeBase));
		}
		public override bool CanEditSelection() {
			return innerSheet.CanEditSelection();
		}
		public override bool CheckSelectionAccess() {
			return innerSheet.CheckSelectionAccess();
		}
	}
	#endregion
	#region RowCollectionWrapperBase
	public abstract class RowCollectionWrapperBase : IRowCollection {
		readonly IRowCollection innerCollection;
		readonly DynamicListWrapper<Row, Row> innerRows;
		readonly Worksheet sheet;
		protected RowCollectionWrapperBase(Worksheet sheet, IRowCollection innerCollection) {
			this.sheet = sheet;
			this.innerCollection = innerCollection;
			innerRows = new DynamicListWrapper<Row, Row>(innerCollection.InnerList, ConvertRowToRowWrapper);
		}
		protected abstract Row ConvertRowToRowWrapper(Row row);
		List<Row> ConvertRowList(List<Row> innerList) {
			if (innerList == null)
				return null;
			List<Row> result = new List<Row>();
			foreach (Row innerRow in innerList) {
				result.Add(ConvertRowToRowWrapper(innerRow));
			}
			return result;
		}
		IEnumerator<Row> ConvertEnumerator(IEnumerator<Row> innerEnumerator) {
			return new EnumeratorConverter<Row, Row>(innerEnumerator, ConvertRowToRowWrapper);
		}
		IOrderedEnumerator<Row> ConvertOrderedEnumerator(IOrderedEnumerator<Row> innerEnumerator) {
			return new OrderedEnumeratorConverter<Row, Row>(innerEnumerator, ConvertRowToRowWrapper);
		}
		IEnumerable<Row> ConvertEnumerable(IEnumerable<Row> innerEnumerable) {
			IEnumerator<Row> innerEnumerator = innerEnumerable.GetEnumerator();
			return new Enumerable<Row>(ConvertEnumerator(innerEnumerator));
		}
		public IList<Row> InnerList { get { return innerRows; } }
		public Worksheet Sheet { get { return sheet; } }
		public void Insert(int index, Row item) {
			throw new InvalidOperationException("This operation is prohibited for Row collection wrapper.");
		}
		public void Clear() {
			innerCollection.Clear();
		}
		public IOrderedEnumerator<Row> GetAllRowsEnumerator(int topRow, int bottomRow, bool reverseOrder) {
			return ConvertOrderedEnumerator(innerCollection.GetAllRowsEnumerator(topRow, bottomRow, reverseOrder));
		}
		public IEnumerable<Row> GetExistingRows() {
			return ConvertEnumerable(innerCollection.GetExistingRows());
		}
		public IEnumerable<Row> GetExistingRows(int topRow, int bottomRow, bool reverseOrder) {
			return ConvertEnumerable(innerCollection.GetExistingRows(topRow, bottomRow, reverseOrder));
		}
		public IEnumerator<Row> GetExistingRowsEnumerator(int topRow, int bottomRow) {
			return ConvertEnumerator(innerCollection.GetExistingRowsEnumerator(topRow, bottomRow));
		}
		public IOrderedItemsRangeEnumerator<Row> GetExistingRowsEnumerator(int topRow, int bottomRow, bool reverseOrder) {
			return new OrderedItemsRangeEnumeratorConverter<Row, Row>(innerCollection.GetExistingRowsEnumerator(topRow, bottomRow, reverseOrder), ConvertRowToRowWrapper);
		}
		public IOrderedItemsRangeEnumerator<Row> GetExistingRowsEnumerator(int topRow, int bottomRow, bool reverseOrder, Predicate<Row> filter) {
			return new OrderedItemsRangeEnumeratorConverter<Row, Row>(innerCollection.GetExistingRowsEnumerator(topRow, bottomRow, reverseOrder, filter), ConvertRowToRowWrapper);
		}
		public IEnumerable<Row> GetExistingVisibleRows(int topRow, int bottomRow, bool reverseOrder) {
			return ConvertEnumerable(innerCollection.GetExistingVisibleRows(topRow, bottomRow, reverseOrder));
		}
		public IOrderedEnumerator<Row> GetExistingVisibleRowsEnumerator(int topRow, int bottomRow, bool reverseOrder) {
			return ConvertOrderedEnumerator(innerCollection.GetExistingVisibleRowsEnumerator(topRow, bottomRow, reverseOrder));
		}
		public IOrderedEnumerator<Row> GetExistingNotVisibleRowsEnumerator(int topRow, int bottomRow, bool reverseOrder) {
			return ConvertOrderedEnumerator(innerCollection.GetExistingNotVisibleRowsEnumerator(topRow, bottomRow, reverseOrder));
		}
		public IEnumerator<Row> GetEnumerator() {
			return ConvertEnumerator(innerCollection.GetEnumerator());
		}
		#region IRowCollectionBase members
		System.Collections.IEnumerable IRowCollectionBase.GetExistingRows(int topRow, int bottomRow, bool reverseOrder) {
			return ((IRowCollectionGeneric<Row>)this).GetExistingRows(topRow, bottomRow, reverseOrder);
		}
		System.Collections.IEnumerator IRowCollectionBase.GetExistingRowsEnumerator(int topRow, int bottomRow) {
			return ((IRowCollectionGeneric<Row>)this).GetExistingRowsEnumerator(topRow, bottomRow);
		}
		System.Collections.IEnumerator IRowCollectionBase.GetExistingRowsEnumerator(int topRow, int bottomRow, bool reverseOrder) {
			return ((IRowCollectionGeneric<Row>)this).GetExistingRowsEnumerator(topRow, bottomRow, reverseOrder);
		}
		System.Collections.IEnumerable IRowCollectionBase.GetExistingVisibleRows(int topRow, int bottomRow, bool reverseOrder) {
			return ((IRowCollectionGeneric<Row>)this).GetExistingVisibleRows(topRow, bottomRow, reverseOrder);
		}
		IRowBase IRowCollectionBase.TryGetRow(int index) {
			return ((IRowCollectionGeneric<Row>)this).TryGetRow(index);
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return ((IRowCollectionGeneric<Row>)this).GetEnumerator();
		}
		#endregion
		public Row GetRowForReading(int rowIndex) {
			return ConvertRowToRowWrapper(innerCollection.GetRowForReading(rowIndex));
		}
		public List<Row> GetRowsForReading(int topIndex, int bottomIndex) {
			List<Row> innerResult = innerCollection.GetRowsForReading(topIndex, bottomIndex);
			return ConvertRowList(innerResult);
		}
		public List<Row> GetInnerRange(int position, int innerCount) {
			List<Row> innerResult = innerCollection.GetInnerRange(position, innerCount);
			return ConvertRowList(innerResult);
		}
		public void InsertRowsShiftDownCore(int position, int count) {
			innerCollection.InsertRowsShiftDownCore(position, count);
		}
		public void InsertRowsShiftDown(int rowIndex, int count) {
			innerCollection.InsertRowsShiftDown(rowIndex, count);
		}
		public bool Contains(int index) {
			return innerCollection.Contains(index);
		}
		public bool Contains(Row item) {
			RowWrapper wrapper = item as RowWrapper;
			if (wrapper != null)
				return innerCollection.Contains(wrapper.InnerRow);
			return innerCollection.Contains(item);
		}
		public Row this[int index] { get { return ConvertRowToRowWrapper(innerCollection[index]); } }
		public Row First { get { return ConvertRowToRowWrapper(innerCollection.First); } }
		public Row Last { get { return ConvertRowToRowWrapper(innerCollection.Last); } }
		public int LastRowIndex { get { return innerCollection.LastRowIndex; } }
		public int Count {
			get { return innerCollection.Count; }
		}
		public void ForEach(Action<Row> action) {
			foreach (Row innerRow in innerCollection)
				action(ConvertRowToRowWrapper(innerRow));
		}
		public Row GetRow(int index) {
			return ConvertRowToRowWrapper(innerCollection.GetRow(index));
		}
		public Row TryGetRow(int index) {
			return ConvertRowToRowWrapper(innerCollection.TryGetRow(index));
		}
		public int TryGetRowIndex(int modelIndex) {
			return innerCollection.TryGetRowIndex(modelIndex);
		}
		public Row CreateRow(int index) {
			return ConvertRowToRowWrapper(innerCollection.CreateRow(index));
		}
		public void SetCachedData(int cachedRowIndex, int cachedRowCollectionIndex) {
			innerCollection.SetCachedData(cachedRowIndex, cachedRowCollectionIndex);
		}
		public void RemoveRange(int from, int count) {
			innerCollection.RemoveRange(from, count);
		}
		public void RemoveRangeCore(int positionFrom, int innerCount, int count, bool removeRange) {
			innerCollection.RemoveRangeCore(positionFrom, innerCount, count, removeRange);
		}
		public void RemoveRangeInner(int index, int innerCount, int count, bool removeRange) {
			innerCollection.RemoveRangeInner(index, innerCount, count, removeRange);
		}
		public void RemoveCore(int index) {
			innerCollection.RemoveCore(index);
		}
		public void RemoveRangeCore(int index, int count) {
			innerCollection.RemoveRangeCore(index, count);
		}
		public void CheckForEmptyRows(int startIndex, int endIndex) {
			innerCollection.CheckForEmptyRows(startIndex, endIndex);
		}
		public void InsertCore(int index, Row item) {
			throw new InvalidOperationException("This operation is prohibited for Row collection wrapper.");
		}
		public void InsertRangeFromHistory(int index, IList<Row> items, int offsetCount) {
			throw new InvalidOperationException("This operation is prohibited for Row collection wrapper.");
		}
	}
	#endregion
	#region RowCollectionWrapper
	public class RowCollectionWrapper : RowCollectionWrapperBase {
		public RowCollectionWrapper(Worksheet sheet, IRowCollection innerCollection)
			: base(sheet, innerCollection) {
		}
		protected override Row ConvertRowToRowWrapper(Row innerItem) {
			if (innerItem == null)
				return null;
			return new RowWrapper(innerItem.Index, Sheet, innerItem);
		}
	}
	#endregion
	#region RowWrapper
	public class RowWrapper : Row {
		readonly Row innerRow;
		readonly CellCollectionWrapper cells;
		public RowWrapper(int index, Worksheet sheet, Row innerRow)
			: base(index, sheet) {
			this.innerRow = innerRow;
			cells = new CellCollectionWrapper(this, innerRow.Cells);
		}
		public Row InnerRow { get { return innerRow; } }
		public override ICellCollection Cells { get { return cells; } }
		public override int FormatIndex { get { return innerRow.FormatIndex; } }
		public override int HeightIndex { get { return innerRow.HeightIndex; } }
		public override int InfoIndex { get { return innerRow.InfoIndex; } }
	}
	#endregion
	#region CellCollectionWrapperBase<TObject>(abstract class)
	public abstract class CellCollectionWrapperBase : ICellCollection {
		readonly ICellContainer owner;
		readonly ICellCollection innerCellCollection;
		readonly DynamicListWrapper<ICell, ICell> innerCells;
		protected CellCollectionWrapperBase(ICellContainer owner, ICellCollection innerCellCollection) {
			this.owner = owner;
			this.innerCellCollection = innerCellCollection;
			innerCells = new DynamicListWrapper<ICell, ICell>(innerCellCollection.InnerList, ConvertCellToCellWrapper);
		}
		protected Worksheet Sheet { get { return owner.Sheet; } }
		public IList<ICell> InnerList { get { return innerCells; } }
		protected abstract ICell ConvertCellToCellWrapper(ICell cell);
		public IEnumerable<ICell> GetAllCellsWithFakeCellsAsNullInRow(Row modelRow, int near, int far) {
			return ConvertEnumerable(innerCellCollection.GetAllCellsWithFakeCellsAsNullInRow(modelRow, near, far));
		}
		public IOrderedEnumerator<ICell> GetAllCellsProvideFakeActualBorderEnumerator(int leftColumn, int rightColumn, bool reverseOrder, int rowIndex, IActualBorderInfo baseActualBorder) {
			return ConvertOrderedEnumerator(innerCellCollection.GetAllCellsProvideFakeActualBorderEnumerator(leftColumn, rightColumn, reverseOrder, rowIndex, baseActualBorder));
		}
		public ICell TryGetFirstCellWith(Func<ICell, bool> condition) {
			return ConvertCellToCellWrapper(innerCellCollection.TryGetFirstCellWith(condition));
		}
		public ICell TryGetLastCellWith(Func<ICell, bool> condition, int leftColumnIndexStopSearch, int rightColumnIndexIgnoreUnitil) {
			return ConvertCellToCellWrapper(innerCellCollection.TryGetLastCellWith(condition, leftColumnIndexStopSearch, rightColumnIndexIgnoreUnitil));
		}
		public ICell GetCellAssumeSequential(int index) {
			return ConvertCellToCellWrapper(innerCellCollection.GetCellAssumeSequential(index));
		}
		public ICell this[int index] {
			get { return ConvertCellToCellWrapper(innerCellCollection[index]); }
		}
		public int Count {
			get { return innerCellCollection.Count; }
		}
		public ICell First {
			get { return ConvertCellToCellWrapper(innerCellCollection.First); }
		}
		public ICell Last {
			get { return ConvertCellToCellWrapper(innerCellCollection.Last); }
		}
		public int Add(ICell item) {
			throw new InvalidOperationException("This operation is prohibited for cell collection wrapper");
		}
		public int TryGetCellIndex(int cachedColumnIndex) {
			return innerCellCollection.TryGetCellIndex(cachedColumnIndex);
		}
		public ICell GetCell(int index) {
			return ConvertCellToCellWrapper(innerCellCollection.GetCell(index));
		}
		public ICell TryGetCell(int columnIndex) {
			return ConvertCellToCellWrapper(innerCellCollection.TryGetCell(columnIndex));
		}
		public void InsertCore(int position, ICell cell) {
			throw new InvalidOperationException("This operation is prohibited for cell collection wrapper");
		}
		public void RemoveAtCore(int position) {
			throw new InvalidOperationException("This operation is prohibited for cell collection wrapper");
		}
		public void Insert(int index, ICell item) {
			throw new InvalidOperationException("This operation is prohibited for cell collection wrapper");
		}
		public void InsertInternal(int index, ICell item) {
			throw new InvalidOperationException("This operation is prohibited for cell collection wrapper");
		}
		public ICell CreateNewCell(int columnIndex) {
			return ConvertCellToCellWrapper(innerCellCollection.CreateNewCell(columnIndex));
		}
		public ICell CreateNewCellCore(CellPosition position) {
			return ConvertCellToCellWrapper(innerCellCollection.CreateNewCellCore(position));
		}
		public void ForEach(Action<ICell> action) {
			foreach (ICell innerCell in innerCellCollection)
				action(ConvertCellToCellWrapper(innerCell));
		}
		public bool Contains(ICell cell) {
			CellWrapper wrapper = cell as CellWrapper;
			if (wrapper != null)
				return innerCellCollection.Contains(wrapper.InnerCell);
			return innerCellCollection.Contains(cell);
		}
		public bool Contains(int index) {
			return innerCellCollection.Contains(index);
		}
		public void OffsetRowIndex(int offset) {
			innerCellCollection.OffsetRowIndex(offset);
		}
		public void RemoveAtColumnIndex(int index) {
			innerCellCollection.RemoveAtColumnIndex(index);
		}
		public void RemoveAtColumnIndexInternal(int index) {
			innerCellCollection.RemoveAtColumnIndexInternal(index);
		}
		public bool TryToInsertCell(ICell cell) {
			throw new InvalidOperationException("This operation is prohibited for cell collection wrapper");
		}
		public bool TryToInsertCellInternal(ICell cell) {
			throw new InvalidOperationException("This operation is prohibited for cell collection wrapper");
		}
		IEnumerator<ICell> ConvertEnumerator(IEnumerator<ICell> innerEnumerator) {
			return new EnumeratorConverter<ICell, ICell>(innerEnumerator, ConvertCellToCellWrapper);
		}
		IOrderedEnumerator<ICell> ConvertOrderedEnumerator(IOrderedEnumerator<ICell> innerEnumerator) {
			return new OrderedEnumeratorConverter<ICell, ICell>(innerEnumerator, ConvertCellToCellWrapper);
		}
		IEnumerable<ICell> ConvertEnumerable(IEnumerable<ICell> innerEnumerable) {
			IEnumerator<ICell> innerEnumerator = innerEnumerable.GetEnumerator();
			return new Enumerable<ICell>(ConvertEnumerator(innerEnumerator));
		}
		public IEnumerable<ICell> GetExistingCells(int leftColumn, int rightColumn, bool reverseOrder) {
			return ConvertEnumerable(innerCellCollection.GetExistingCells(leftColumn, rightColumn, reverseOrder));
		}
		public IEnumerable<ICell> GetExistingVisibleCells(int leftColumn, int rightColumn, bool reverseOrder) {
			return ConvertEnumerable(innerCellCollection.GetExistingVisibleCells(leftColumn, rightColumn, reverseOrder));
		}
		public IEnumerable<ICell> GetExistingNonEmptyVisibleCells(int leftColumn, int rightColumn, bool reverseOrder) {
			return ConvertEnumerable(innerCellCollection.GetExistingNonEmptyVisibleCells(leftColumn, rightColumn, reverseOrder));
		}
		public IOrderedEnumerator<ICell> GetExistingCellsEnumerator(int leftColumn, int rightColumn, bool reverseOrder) {
			return ConvertOrderedEnumerator(innerCellCollection.GetExistingCellsEnumerator(leftColumn, rightColumn, reverseOrder));
		}
		public IEnumerator<ICell> GetExistingCellsEnumerator(int leftColumn, int rightColumn) {
			return ConvertEnumerator(innerCellCollection.GetExistingCellsEnumerator(leftColumn, rightColumn));
		}
		public IEnumerable<ICell> GetExistingNonEmptyCells(int leftColumn, int rightColumn, bool reverseOrder) {
			return ConvertEnumerable(innerCellCollection.GetExistingNonEmptyCells(leftColumn, rightColumn, reverseOrder));
		}
		public IOrderedEnumerator<ICell> GetExistingNonEmptyCellsEnumerator(int leftColumn, int rightColumn, bool reverseOrder) {
			return ConvertOrderedEnumerator(innerCellCollection.GetExistingNonEmptyCellsEnumerator(leftColumn, rightColumn, reverseOrder));
		}
		public IOrderedEnumerator<ICell> GetExistingVisibleCellsEnumerator(int leftColumn, int rightColumn, bool reverseOrder) {
			return ConvertOrderedEnumerator(innerCellCollection.GetExistingVisibleCellsEnumerator(leftColumn, rightColumn, reverseOrder));
		}
		public IEnumerator<ICell> GetExistingNonEmptyVisibleCellsEnumerator(int leftColumn, int rightColumn, bool reverseOrder) {
			return ConvertEnumerator(innerCellCollection.GetExistingNonEmptyVisibleCellsEnumerator(leftColumn, rightColumn, reverseOrder));
		}
		System.Collections.IEnumerable ICellCollectionBase.GetExistingCells(int leftColumn, int rightColumn, bool reverseOrder) {
			return ((ICellCollectionGeneric<ICell>)this).GetExistingCells(leftColumn, rightColumn, reverseOrder);
		}
		System.Collections.IEnumerable ICellCollectionBase.GetExistingVisibleCells(int leftColumn, int rightColumn, bool reverseOrder) {
			return ((ICellCollectionGeneric<ICell>)this).GetExistingVisibleCells(leftColumn, rightColumn, reverseOrder);
		}
		System.Collections.IEnumerable ICellCollectionBase.GetExistingNonEmptyVisibleCells(int leftColumn, int rightColumn, bool reverseOrder) {
			return ((ICellCollectionGeneric<ICell>)this).GetExistingNonEmptyVisibleCells(leftColumn, rightColumn, reverseOrder);
		}
		System.Collections.IEnumerator ICellCollectionBase.GetExistingCellsEnumerator(int leftColumn, int rightColumn, bool reverseOrder) {
			return ((ICellCollectionGeneric<ICell>)this).GetExistingCellsEnumerator(leftColumn, rightColumn, reverseOrder);
		}
		System.Collections.IEnumerator ICellCollectionBase.GetExistingCellsEnumerator(int leftColumn, int rightColumn) {
			return ((ICellCollectionGeneric<ICell>)this).GetExistingCellsEnumerator(leftColumn, rightColumn);
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return ((IEnumerable<ICell>)this).GetEnumerator();
		}
		public IEnumerator<ICell> GetEnumerator() {
			return ConvertEnumerator(innerCellCollection.GetEnumerator());
		}
	}
	#endregion
	#region CellCollectionWrapper
	public class CellCollectionWrapper : CellCollectionWrapperBase {
		public CellCollectionWrapper(ICellContainer owner, ICellCollection innerCollection)
			: base(owner, innerCollection) {
		}
		#region Properties
		#endregion
		protected override ICell ConvertCellToCellWrapper(ICell cell) {
			if (cell == null)
				return null;
			return new CellWrapperWithOverridenSheet(cell, Sheet);
		}
	}
	#endregion
}
