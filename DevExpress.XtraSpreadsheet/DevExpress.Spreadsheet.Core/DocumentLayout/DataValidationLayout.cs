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

using DevExpress.Office.Drawing;
using DevExpress.Office.Internal;
using DevExpress.Office.Layout;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Mouse;
using DevExpress.XtraSpreadsheet.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Layout {
	#region DataValidationLayout
	public class DataValidationLayout : DocumentItemLayout {
		#region Fields
		const int minInplaceEditorWidthInPixels = 60;
		internal const int horizontaCircleOffsetInPixels = 5;
		internal const int verticalCircleOffsetInPixels = 2;
		DataValidationHotZone hotZone;
		DataValidationMessageLayout messageLayout;
		List<Rectangle> invalidDataRectangles;
		int minInplaceEditorWidth;
		int onePixelInModelUnits;
		int horizontaCircleOffset;
		int verticalCircleOffset;
		int imageSize;
		Page lastPage;
		#endregion
		public DataValidationLayout(SpreadsheetView view)
			: base(view) {
		}
		#region Properties
		public DataValidationHotZone HotZone { get { return hotZone; } }
		public List<Rectangle> InvalidDataRectangles { get { return invalidDataRectangles; } }
		public DataValidationMessageLayout MessageLayout { get { return messageLayout; } }
		#endregion
		internal void Initialize() {
			this.minInplaceEditorWidth = LayoutUnitConverter.PixelsToLayoutUnits(minInplaceEditorWidthInPixels, DocumentModel.Dpi);
			this.onePixelInModelUnits = LayoutUnitConverter.PixelsToLayoutUnits(1, DocumentModel.Dpi);
			this.horizontaCircleOffset = LayoutUnitConverter.PixelsToLayoutUnits(horizontaCircleOffsetInPixels, DocumentModel.Dpi);
			this.verticalCircleOffset = LayoutUnitConverter.PixelsToLayoutUnits(verticalCircleOffsetInPixels, DocumentModel.Dpi);
			this.imageSize = LayoutUnitConverter.PixelsToLayoutUnits(AutoFilterLayout.ImageSize, DocumentModel.Dpi);
		}
		public override void Update(Page page) {
			Invalidate();
			Initialize();
			CellRange activeRange = ActiveSheet.Selection.GetActiveCellRange();
			this.hotZone = CreateHotZone(page, activeRange);
			this.invalidDataRectangles = CalculateInvalidDataRectangles(page);
			this.messageLayout = CreateMessageLayout(page, activeRange);
			this.lastPage = page;
		}
		DataValidationHotZone CreateHotZone(Page page, CellRange activeRange) {
			CellPosition activeTopRight = activeRange.TopRight;
			if (!IsContainsInVisibleRange(page, activeTopRight))
				return null;
			foreach (DataValidation dataValidation in ActiveSheet.DataValidations) {
				if (dataValidation.Type == DataValidationType.List && !dataValidation.SuppressDropDown) {
					if (dataValidation.CellRange.ContainsCell(activeRange.LeftColumnIndex, activeRange.TopRowIndex)) {
						DataValidationHotZone hotZone = new DataValidationHotZone(View.Control.InnerControl, dataValidation.Expression1, ActiveSheet);
						Rectangle bounds = CalculateHotZoneBounds(page, activeTopRight);
						hotZone.Bounds = bounds;
						Point bottomRight = new Point(bounds.Right, bounds.Bottom);
						hotZone.EditorLocation = CalculateHotZoneEditorLocation(page, activeRange.LeftColumnIndex, bottomRight);
						return hotZone;
					}
				}
			}
			return null;
		}
		Rectangle CalculateHotZoneBounds(Page page, CellPosition activeCellPosition) {
			int columnIndex = page.GridColumns.CalculateIndex(activeCellPosition.Column);
			int leftPosition = page.GridColumns[columnIndex].Far;
			int rowIndex = page.GridRows.CalculateIndex(activeCellPosition.Row);
			PageGridItem row = page.GridRows[rowIndex];
			int size = Math.Min(imageSize, row.Extent);
			Rectangle result = Rectangle.FromLTRB(leftPosition, row.Far - size, leftPosition + size, row.Far);
			result.Offset(onePixelInModelUnits, -onePixelInModelUnits);
			return result;
		}
		internal DataValidationMessageLayout CreateMessageLayout(Page page, CellRange activeRange) {
			foreach (DataValidation dataValidation in ActiveSheet.DataValidations) {
				if (dataValidation.ShowInputMessage && !String.IsNullOrEmpty(dataValidation.Prompt)) {
					CellPosition activePosition = activeRange.TopLeft;
					if (dataValidation.CellRange.ContainsCell(activePosition.Column, activePosition.Row)) {
						DataValidationMessageLayout messageLayout = new DataValidationMessageLayout(LayoutUnitConverter);
						messageLayout.Title = dataValidation.PromptTitle;
						messageLayout.Text = dataValidation.Prompt;
						messageLayout.CalculateMessageLayoutBounds(page, activeRange);
						return messageLayout;
					}
				}
			}
			return null;
		}
		Point CalculateHotZoneEditorLocation(Page page, int activeColumnIndex, Point bottomRightHotZone) {
			int columnIndex = page.GridColumns.CalculateExactOrFarItem(activeColumnIndex);
			int leftPosition = page.GridColumns[columnIndex].Near;
			int width = bottomRightHotZone.X - leftPosition;
			width = Math.Max(minInplaceEditorWidth, width);
			int x = LayoutUnitConverter.LayoutUnitsToPixels(bottomRightHotZone.X - width, DocumentModel.DpiX);
			int y = LayoutUnitConverter.LayoutUnitsToPixels(bottomRightHotZone.Y + onePixelInModelUnits, DocumentModel.DpiY);
			return new Point(x, y);
		}
		bool IsContainsDVInVisibleRange(Page page, CellRange range) {
			return IsContainsInVisibleRange(page, range.TopLeft, range.TopRight);
		}
		internal bool IsContainsInVisibleRange(Page page, CellPosition position) {
			bool isVisible = IsContainsInVisibleRange(page, position, position);
			return isVisible && IsNotHiddenPosition(page, position);
		}
		bool IsNotHiddenPosition(Page page, CellPosition position) {
			int columnIndex = page.GridColumns.TryCalculateIndex(position.Column);
			if (columnIndex == -1)
				return false;
			int rowIndex = page.GridRows.TryCalculateIndex(position.Row);
			return rowIndex > -1;
		}
		bool IsContainsInVisibleRange(Page page, CellPosition firstPosition, CellPosition lastPosition) {
			if (page.IsBoundsNotIntersectsWithVisibleBounds(page.GridColumns, firstPosition.Column, lastPosition.Column))
				return false;
			if (page.IsBoundsNotIntersectsWithVisibleBounds(page.GridRows, firstPosition.Row, lastPosition.Row))
				return false;
			return true;
		}
		List<Rectangle> CalculateInvalidDataRectangles(Page page) {
			List<Rectangle> result = new List<Rectangle>();
			foreach (CellKey key in ActiveSheet.InvalidDataCircles) {
				CellPosition position = new CellPosition(key.ColumnIndex, key.RowIndex);
				CellRange activeRange = ActiveSheet.Selection.GetActualCellRange(position);
				if (!IsContainsDVInVisibleRange(page, activeRange))
					continue;
				Rectangle invalidRect = CalculateInvalidRectangle(page, activeRange);
				if (!invalidRect.IsEmpty)
					result.Add(invalidRect);
			}
			return result;
		}
		internal Rectangle CalculateInvalidRectangle(Page page, CellRange selectionaRange) {
			CellPosition leftPosition = selectionaRange.TopLeft;
			PageGrid gridColumns = page.GridColumns;
			int leftColumnIndex = gridColumns.CalculateExactOrFarItem(leftPosition.Column);
			CellPosition rightPosition = selectionaRange.TopRight;
			int rightColumnIndex = gridColumns.CalculateExactOrNearItem(rightPosition.Column);
			if (rightColumnIndex < leftColumnIndex)
				return Rectangle.Empty;
			int left = gridColumns[leftColumnIndex].Near - horizontaCircleOffset;
			int right = gridColumns[rightColumnIndex].Far + horizontaCircleOffset;
			PageGrid gridRows = page.GridRows;
			int rowIndex = gridRows.TryCalculateIndex(leftPosition.Row);
			if (rowIndex == -1)
				return Rectangle.Empty;
			PageGridItem row = gridRows[rowIndex];
			int top = row.Near - verticalCircleOffset;
			int bottom = row.Far + verticalCircleOffset;
			return Rectangle.FromLTRB(left, top, right, bottom);
		}
		public override void Invalidate() {
			this.hotZone = null;
			this.invalidDataRectangles = null;
		}
		protected internal override HotZone CalculateHotZone(Point point, Page page) {
			if (!object.ReferenceEquals(lastPage, page) && page != null)
				Update(page);
			if (hotZone == null)
				return null;
			return HotZoneCalculator.CalculateHotZone(hotZone, point, View.ZoomFactor, LayoutUnitConverter);
		}
	}
	#endregion
	#region DataValidationMessageLayout
	public class DataValidationMessageLayout {
		#region Fields
#if DXPORTABLE
		FontInfoMeasurer fontMeasurer;
#else
		GdiFontInfoMeasurer fontMeasurer;
#endif
		DevExpress.Office.Layout.DocumentLayoutUnitConverter unitConverter;
		const int verticalOffsetLocationBoundsInPixels = 6;
		const int maxWidthInPixels = 240;
		const int maxHeightInPixels = 160;
		const int leftPaddingTitleInPixels = 4;
		const int rightPaddingTitleInPixels = 20;
		const int leftPaddingTextInPixels = 6;
		const int rightPaddingTextInPixels = 8;
		const int leftPaddingLocationTextInPixels = 7;
		const int rightPaddingLocationTextInPixels = 3;
		const int paddingLocationTitleInPixels = 5;
		const int topPaddingLocationTextWithTitleInPixels = 5;
		const int topPaddingLocationTextWithoutTitleInPixels = 20;
		const int correctHeightWithoutTitleInPixels = 10;
		const int correctHeightWithTitleInPixels = 24;
		#endregion
		public DataValidationMessageLayout(DocumentLayoutUnitConverter unitConverter) {
			this.unitConverter = unitConverter;
#if DXPORTABLE
			fontMeasurer = new PrecalculatedMetricsFontInfoMeasurer(unitConverter);
			TitleFontInfo = new PrecalculatedMetricsFontInfo(fontMeasurer, "Tahoma", 9 * 2, true, false, false, false);
			TextFontInfo = new PrecalculatedMetricsFontInfo(fontMeasurer, "Tahoma", 9 * 2, false, false, false, false);
#else
			fontMeasurer = new PureGdiFontInfoMeasurer(unitConverter);
			TitleFontInfo = new GdiFontInfo(fontMeasurer, "Tahoma", 9 * 2, true, false, false, false);
			TextFontInfo = new GdiFontInfo(fontMeasurer, "Tahoma", 9 * 2, false, false, false, false);
#endif
		}
		#region Properties
		public Rectangle BoundsMessage { get; set; }
		public Rectangle BoundsTitle { get; set; }
		public Rectangle BoundsText { get; set; }
		public string Title { get; set; }
		public string Text { get; set; }
		public FontInfo TitleFontInfo { get; set; }
		public FontInfo TextFontInfo { get; set; }
		#endregion
		internal void CalculateMessageLayoutBounds(Page page, CellRange activeRange) {
			PageGrid gridColumns = page.GridColumns;
			int leftPosition = activeRange.LeftColumnIndex;
			int rightPosition = activeRange.RightColumnIndex;
			if (page.IsBoundsNotIntersectsWithVisibleBounds(gridColumns, leftPosition, rightPosition))
				return;
			int leftColumnIndex;
			if (leftPosition <= gridColumns.ActualFirst.ModelIndex)
				leftColumnIndex = gridColumns.ActualFirstIndex;
			else
				leftColumnIndex = gridColumns.CalculateExactOrNearItem(leftPosition);
			int rightColumnIndex;
			if (rightPosition >= gridColumns.ActualLast.ModelIndex)
				rightColumnIndex = gridColumns.ActualLastIndex;
			else
				rightColumnIndex = gridColumns.CalculateExactOrFarItem(rightPosition);
			int gridLeftPosition = gridColumns[leftColumnIndex].Near;
			int left = gridLeftPosition + (gridColumns[rightColumnIndex].Far - gridLeftPosition) / 2;
			PageGrid gridRows = page.GridRows;
			int rowIndex = gridRows.CalculateExactOrNearItem(activeRange.BottomRowIndex);
			if (rowIndex == -1)
				return;
			PageGridItem row = gridRows[rowIndex];
			int top = row.Far + unitConverter.PixelsToLayoutUnits(verticalOffsetLocationBoundsInPixels, DocumentModel.DpiY);
			int width = CalculateWidthBounds();
			int height = CalculateHeightBounds(width);
			CheckAndRecalculateBounds(ref width, ref height);
			int right = left + unitConverter.PixelsToLayoutUnits(width, DocumentModel.DpiX);
			int bottom = top + unitConverter.PixelsToLayoutUnits(height, DocumentModel.DpiY);
			BoundsMessage = Rectangle.FromLTRB(left, top, right, bottom);
			BoundsTitle = Rectangle.FromLTRB(left + paddingLocationTitleInPixels, top + paddingLocationTitleInPixels, right, bottom);
			int titleBoundsTop = String.IsNullOrEmpty(Title) ? top + topPaddingLocationTextWithTitleInPixels : top + topPaddingLocationTextWithoutTitleInPixels;
			BoundsText = Rectangle.FromLTRB(left + leftPaddingLocationTextInPixels, titleBoundsTop, right - rightPaddingLocationTextInPixels, bottom);
		}
		int CalculateWidthBounds() {
			int width = 0;
			if (!String.IsNullOrEmpty(Title))
				width = leftPaddingTitleInPixels + fontMeasurer.MeasureString(Title, TitleFontInfo).Width + rightPaddingTitleInPixels;
			if (String.IsNullOrEmpty(Title)) {
				width = leftPaddingTextInPixels + fontMeasurer.MeasureString(Text, TextFontInfo).Width + rightPaddingTextInPixels;
				width = (width > maxWidthInPixels) ? maxWidthInPixels : width;
			}
			return width;
		}
		int CalculateHeightBounds(int width) {
#if DXPORTABLE
			return width;
#else
			int noMarginsWidth = width - (leftPaddingLocationTextInPixels + rightPaddingLocationTextInPixels);
			Size size = DevExpress.Utils.Text.TextUtils.GetStringSize(fontMeasurer.MeasureGraphics, Text, TextFontInfo.Font, StringFormat.GenericTypographic, noMarginsWidth, DocumentModel.WordBreakProvider);
			return String.IsNullOrEmpty(Title) ? (size.Height + correctHeightWithoutTitleInPixels) : (size.Height + correctHeightWithTitleInPixels);
#endif
		}
		void CheckAndRecalculateBounds(ref int width, ref int height) {
			double resizeRatio = 1.2;
			if ((double)width / (double)height < resizeRatio) {
				width = (int)(resizeRatio * height);
				if (width > maxWidthInPixels)
					width = maxWidthInPixels;
				height = CalculateHeightBounds(width);
				if (height > maxWidthInPixels)
					height = maxWidthInPixels;
			}
		}
	}
	#endregion
	#region DataValidationHotZone
	public class DataValidationHotZone : HotZone {
		#region Fields
		readonly InnerSpreadsheetControl control;
		readonly ParsedExpression expression;
		readonly Worksheet sheet;
		#endregion
		public DataValidationHotZone(InnerSpreadsheetControl control, ParsedExpression expression, Worksheet sheet)
			: base(control) {
			Guard.ArgumentNotNull(control, "control");
			Guard.ArgumentNotNull(expression, "expression");
			Guard.ArgumentNotNull(sheet, "sheet");
			this.control = control;
			this.expression = expression;
			this.sheet = sheet;
		}
		#region Properties
		public override SpreadsheetCursor Cursor { get { return SpreadsheetCursors.Default; } }
		public Point EditorLocation { get; set; }
		#endregion
		public override void Activate(SpreadsheetMouseHandler handler, SpreadsheetHitTestResult result) {
			if (control.IsDataValidationInplaceEditorActive) {
				control.DeactivateDataValidationInplaceEditor();
			}
			else {
				int widthInPixels = sheet.Workbook.LayoutUnitConverter.LayoutUnitsToPixels(Bounds.Right, DocumentModel.DpiX) - EditorLocation.X;
				Rectangle rect = new Rectangle(EditorLocation.X, EditorLocation.Y, widthInPixels, 0);
				DataValidationInplaceValueStorage allowedValuesStorage = DataValidationAllowedValueCalculator.CalculateAllowedValues(expression, sheet);
				if (!allowedValuesStorage.IsEmpty)
					control.ActivateDataValidationInplaceEditor(rect, allowedValuesStorage);
			}
		}
		public override void Visit(IHotZoneVisitor visitor) {
			visitor.Visit(this);
		}
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Model {
	#region DataValidationInplaceValueStorage
	public struct DataValidationInplaceValueStorage {
		public bool IsTextValue { get; set; }
		public List<string> TextAllowedValues { get; set; }
		public string TextActiveValue { get; set; }
		public List<DataValidationInplaceValue> DataValidationInplaceAllowedValues { get; set; }
		public DataValidationInplaceValue DataValidationInplaceActiveValue { get; set; }
		public bool IsEmpty {
			get {
				return !IsTextValue && TextAllowedValues == null && TextActiveValue == null &&
					DataValidationInplaceAllowedValues == null && DataValidationInplaceActiveValue.IsEmpty;
			}
		}
	}
	#endregion
	#region DataValidationInplaceValue
	public struct DataValidationInplaceValue {
		public static DataValidationInplaceValue Empty = new DataValidationInplaceValue() { Value = VariantValue.Empty, DisplayText = String.Empty };
		public string DisplayText { get; set; }
		public VariantValue Value { get; set; }
		public bool IsEmpty { get { return Value.IsEmpty && String.IsNullOrEmpty(String.Empty); } }
		public override string ToString() {
			return DisplayText;
		}
	}
	#endregion
	#region DataValidationAllowedValueCalculator
	public static class DataValidationAllowedValueCalculator {
		internal static DataValidationInplaceValue EmptyInplaceValue = DataValidationInplaceValue.Empty;
		public static DataValidationInplaceValueStorage CalculateAllowedValues(ParsedExpression expression, Worksheet activeSheet) {
			DataValidationInplaceValueStorage result;
			WorkbookDataContext dataContext = activeSheet.DataContext;
			CellPosition activeCellPosition = activeSheet.Selection.ActiveCell;
			activeCellPosition = activeSheet.Selection.GetActualCellRange(activeCellPosition).TopLeft;
			if (activeCellPosition.IsValid)
				dataContext.PushCurrentCell(activeCellPosition);
			else
				dataContext.PushCurrentCell(null);
			dataContext.PushCurrentWorksheet(activeSheet);
			try {
				dataContext.PushRelativeToCurrentCell(true);
				string currentCellDisplayText = activeSheet.GetCellForFormatting(activeCellPosition.Column, activeCellPosition.Row).Text;
				result = CalculateAllowedValuesCore(expression, dataContext, currentCellDisplayText);
			}
			finally {
				dataContext.PopCurrentCell();
				dataContext.PopRelativeToCurrentCell();
				dataContext.PopCurrentWorksheet();
			}
			return result;
		}
		static DataValidationInplaceValueStorage CalculateAllowedValuesCore(ParsedExpression expression, WorkbookDataContext dataContext, string currentCellDisplayText) {
			VariantValue allowedValuesExpression = expression.Evaluate(dataContext);
			if (allowedValuesExpression.IsError)
				return new DataValidationInplaceValueStorage();
			if (allowedValuesExpression.IsText)
				return CalculateTextValues(allowedValuesExpression, dataContext, currentCellDisplayText);
			System.Diagnostics.Debug.Assert(allowedValuesExpression.IsCellRange);
			return CalculateVariantValues(allowedValuesExpression.CellRangeValue, currentCellDisplayText);
		}
		static DataValidationInplaceValueStorage CalculateTextValues(VariantValue allowedValuesExpression, WorkbookDataContext dataContext, string currentCellDisplayText) {
			List<string> values = new List<string>();
			string textAllowedValues = allowedValuesExpression.GetTextValue(dataContext.Workbook.SharedStringTable);
			char separator = CultureInfo.InvariantCulture.TextInfo.ListSeparator[0];
			string[] textAllowedValuesArray = textAllowedValues.Split(new char[] { separator }, StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < textAllowedValuesArray.Length; i++) {
				string text = textAllowedValuesArray[i].Trim();
				if (!String.IsNullOrEmpty(text))
					values.Add(text);
			}
			DataValidationInplaceValueStorage result = new DataValidationInplaceValueStorage();
			result.IsTextValue = true;
			result.TextAllowedValues = values;
			result.TextActiveValue = currentCellDisplayText;
			return result;
		}
		internal static DataValidationInplaceValueStorage CalculateVariantValues(CellRangeBase cellRange, string currentCellDisplayText) {
			List<DataValidationInplaceValue> values = new List<DataValidationInplaceValue>();
			DataValidationInplaceValueStorage result = new DataValidationInplaceValueStorage();
			Worksheet sheet = (Worksheet)cellRange.Worksheet;
			if (cellRange.TopLeft.Column < cellRange.BottomRight.Column)
				result.DataValidationInplaceActiveValue = CalculateVariantValuesByColumns(cellRange, values, currentCellDisplayText);
			else
				result.DataValidationInplaceActiveValue = CalculateVariantValuesByRows(cellRange, sheet, values, currentCellDisplayText);
			result.IsTextValue = false;
			result.DataValidationInplaceAllowedValues = values;
			return result;
		}
		static DataValidationInplaceValue CalculateVariantValuesByColumns(CellRangeBase cellRange, List<DataValidationInplaceValue> values, string currentCellDisplayText) {
			DataValidationInplaceValue activeValue = EmptyInplaceValue;
			IRowBase row = cellRange.Worksheet.Rows.TryGetRow(cellRange.TopLeft.Row);
			if (row == null) {
				values.Add(EmptyInplaceValue);
				return activeValue;
			}
			bool isActiveValueFind = false;
			if (String.IsNullOrEmpty(currentCellDisplayText)) {
				activeValue = EmptyInplaceValue;
				isActiveValueFind = true;
			}
			int leftColumnIndex = cellRange.TopLeft.Column;
			int rightColumnIndex = cellRange.BottomRight.Column;
			int previousColumnIndex = leftColumnIndex - 1;
			bool isHasEmptyCell = false;
			IEnumerator<ICell> enumerator = (IEnumerator<ICell>)row.Cells.GetExistingCellsEnumerator(leftColumnIndex, rightColumnIndex);
			while (enumerator.MoveNext()) {
				ICell cell = enumerator.Current;
				if (cell.Value == VariantValue.Empty)
					continue;
				if (previousColumnIndex < cell.Position.Column - 1) {
					values.Add(EmptyInplaceValue);
					isHasEmptyCell = true;
				}
				string currentDisplayText = cell.Text;
				DataValidationInplaceValue inplaceValue = new DataValidationInplaceValue() { DisplayText = currentDisplayText, Value = cell.Value };
				values.Add(inplaceValue);
				if (!isActiveValueFind && currentCellDisplayText == currentDisplayText) {
					isActiveValueFind = true;
					activeValue = inplaceValue;
				}
				previousColumnIndex = cell.Position.Column;
			}
			if (values.Count == 0 || (previousColumnIndex != rightColumnIndex && !isHasEmptyCell)) {
				values.Add(EmptyInplaceValue);
			}
			return activeValue;
		}
		static DataValidationInplaceValue CalculateVariantValuesByRows(CellRangeBase cellRange, Worksheet sheet, List<DataValidationInplaceValue> values, string currentCellDisplayText) {
			int topRowIndex = cellRange.TopLeft.Row;
			int bottomRowIndex = cellRange.BottomRight.Row;
			int previousRowIndex = topRowIndex - 1;
			bool isHasEmptyCell = false;
			DataValidationInplaceValue activeValue = EmptyInplaceValue;
			bool isActiveValueFind = false;
			if (String.IsNullOrEmpty(currentCellDisplayText)) {
				activeValue = EmptyInplaceValue;
				isActiveValueFind = true;
			}
			int columnIndex = cellRange.TopLeft.Column;
			IEnumerator<Row> enumerator = sheet.Rows.GetExistingRowsEnumerator(topRowIndex, bottomRowIndex);
			while (enumerator.MoveNext()) {
				Row row = enumerator.Current;
				ICell cell = row.Cells.TryGetCell(columnIndex);
				if (cell == null || cell.Value == VariantValue.Empty)
					continue;
				if (previousRowIndex < row.Index - 1) {
					values.Add(EmptyInplaceValue);
					isHasEmptyCell = true;
				}
				string currentDisplayText = cell.Text;
				DataValidationInplaceValue inplaceValue = new DataValidationInplaceValue() { DisplayText = currentDisplayText, Value = cell.Value };
				values.Add(inplaceValue);
				if (!isActiveValueFind && currentCellDisplayText == currentDisplayText) {
					isActiveValueFind = true;
					activeValue = inplaceValue;
				}
				previousRowIndex = row.Index;
			}
			if (values.Count == 0 || (previousRowIndex != bottomRowIndex && !isHasEmptyCell)) {
				values.Add(EmptyInplaceValue);
			}
			return activeValue;
		}
	}
	#endregion
}
