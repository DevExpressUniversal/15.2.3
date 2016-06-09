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
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region ChartRangesCalculator
	public static class ChartRangesCalculator {
		public static ChartSeriesRangeModelBase CreateModel(CellRange dataRange, ChartViewType viewType) {
			CellRange seriesRange = GetSeriesRange(dataRange, viewType);
			return CreateModel(dataRange, seriesRange, viewType);
		}
		public static ChartSeriesRangeModelBase CreateModel(CellRange dataRange, CellRange seriesRange, ChartViewType viewType) {
			ChartViewSeriesDirection direction = GetSeriesDirection(dataRange, seriesRange, viewType);
			return CreateModel(dataRange, seriesRange, viewType, direction);
		}
		public static ChartSeriesRangeModelBase CreateModel(CellRange dataRange, CellRange seriesRange, ChartViewType viewType, ChartViewSeriesDirection direction) {
			ChartSeriesRangeModelBase result = direction == ChartViewSeriesDirection.Vertical ? GetInstanceVerticalModel(viewType) : GetInstanceHorizontalModel(viewType);
			result.CalculateProperties(dataRange, seriesRange);
			return result;
		}
		public static CellRange GetSeriesRange(CellRange dataRange, ChartViewType viewType) {
			CellRange seriesRange = GetSeriesRange(dataRange);
			if (viewType == ChartViewType.Bubble && seriesRange.TopLeft.EqualsPosition(dataRange.TopLeft))
				seriesRange = GetBubbleSeriesRange(dataRange);
			if (viewType == ChartViewType.Scatter) {
				ChartViewSeriesDirection direction = GetSeriesDirection(dataRange, seriesRange);
				if (direction == ChartViewSeriesDirection.Vertical && seriesRange.TopLeft.Column == dataRange.TopLeft.Column)
					seriesRange = GetSeriesRange(dataRange, seriesRange.TopLeft.Column + 1, seriesRange.TopLeft.Row);
				if (direction == ChartViewSeriesDirection.Horizontal && seriesRange.TopLeft.Row == dataRange.TopLeft.Row)
					seriesRange = GetSeriesRange(dataRange, seriesRange.TopLeft.Column, seriesRange.TopLeft.Row + 1);
			}
			return seriesRange;
		}
		public static CellRange GetSeriesRange(CellRange dataRange) {
			int firstColumn = Math.Max(CalculateColumnRight(dataRange), CalculateColumnLeft(dataRange));
			int firstRow = Math.Max(CalculateRowBottom(dataRange), CalculateRowTop(dataRange, firstColumn - 1));
			return GetSeriesRange(dataRange, firstColumn, firstRow);
		}
		#region Internals
		static CellRange GetSeriesRange(CellRange dataRange, int firstColumn, int firstRow) {
			if (firstColumn < dataRange.TopLeft.Column)
				firstColumn = dataRange.TopLeft.Column;
			if (firstColumn > dataRange.BottomRight.Column)
				firstColumn = dataRange.BottomRight.Column;
			if (firstRow < dataRange.TopLeft.Row)
				firstRow = dataRange.TopLeft.Row;
			if (firstRow > dataRange.BottomRight.Row)
				firstRow = dataRange.BottomRight.Row;
			CellPosition topLeft = new CellPosition(firstColumn, firstRow, PositionType.Absolute, PositionType.Absolute);
			CellPosition bottomRight = new CellPosition(dataRange.BottomRight.Column, dataRange.BottomRight.Row, PositionType.Absolute, PositionType.Absolute);
			return new CellRange(dataRange.Worksheet, topLeft, bottomRight);
		}
		static CellRange GetBubbleSeriesRange(CellRange dataRange) {
			int firstColumn = dataRange.TopLeft.Column;
			int firstRow = dataRange.TopLeft.Row;
			if (dataRange.Width % 2 == 1)
				firstColumn++;
			else if (dataRange.Height % 2 == 1)
				firstRow++;
			return GetSeriesRange(dataRange, firstColumn, firstRow);
		}
		static int CalculateColumnRight(CellRange range) {
			Worksheet sheet = range.Worksheet as Worksheet;
			Row row = sheet.Rows.TryGetRow(range.BottomRight.Row);
			int result = range.TopLeft.Column;
			if (row == null)
				return result;
			result = range.BottomRight.Column;
			bool isDate = false;
			for (int i = range.BottomRight.Column; i >= range.TopLeft.Column; i--) {
				ICell cell = row.Cells.TryGetCell(i);
				if (cell != null) {
					if (cell.Value.IsText || cell.Format.IsText)
						return result;
					if (i == range.BottomRight.Column)
						isDate = cell.Value.IsNumeric && cell.Format.IsDateTime;
					else if (!isDate && cell.Value.IsNumeric && cell.Format.IsDateTime)
						return result;
				}
				result = i;
			}
			return result;
		}
		static int CalculateRowBottom(CellRange range) {
			Worksheet sheet = range.Worksheet as Worksheet;
			int column = range.BottomRight.Column;
			int result = range.BottomRight.Row;
			bool isDate = false;
			for (int i = range.BottomRight.Row; i >= range.TopLeft.Row; i--) {
				ICell cell = sheet.TryGetCell(column, i);
				if (cell != null) {
					if (cell.Value.IsText || cell.Format.IsText)
						return result;
					if (i == range.BottomRight.Row)
						isDate = cell.Value.IsNumeric && cell.Format.IsDateTime;
					else if (!isDate && cell.Value.IsNumeric && cell.Format.IsDateTime)
						return result;
				}
				result = i;
			}
			return result;
		}
		static int CalculateColumnLeft(CellRange range) {
			if (range.Width == 1)
				return range.TopLeft.Column;
			Worksheet sheet = range.Worksheet as Worksheet;
			Row row = sheet.Rows.TryGetRow(range.TopLeft.Row);
			if (row == null)
				return range.TopLeft.Column + 1;
			for (int i = range.TopLeft.Column; i < range.BottomRight.Column; i++) {
				ICell cell = row.Cells.TryGetCell(i);
				if (cell != null && !cell.Value.IsEmpty)
					return i;
			}
			return range.BottomRight.Column;
		}
		static int CalculateRowTop(CellRange range, int column) {
			if (range.Height == 1)
				return range.TopLeft.Row;
			if (column < range.TopLeft.Column)
				column = range.TopLeft.Column;
			Worksheet sheet = range.Worksheet as Worksheet;
			for (int i = range.TopLeft.Row; i <= range.BottomRight.Row; i++) {
				ICell cell = sheet.TryGetCell(column, i);
				if (cell != null && !cell.Value.IsEmpty)
					return i;
			}
			return range.TopLeft.Row + 1;
		}
		static ChartSeriesRangeModelBase GetInstanceVerticalModel(ChartViewType viewType) {
			if (viewType == ChartViewType.Bubble)
				return new ChartBubbleSeriesRangeVerticalModel();
			return new ChartSeriesRangeVerticalModel();
		}
		static ChartSeriesRangeModelBase GetInstanceHorizontalModel(ChartViewType viewType) {
			if (viewType == ChartViewType.Bubble)
				return new ChartBubbleSeriesRangeHorizontalModel();
			return new ChartSeriesRangeHorizontalModel();
		}
		static ChartViewSeriesDirection GetSeriesDirection(CellRange dataRange, CellRange seriesRange) {
			bool useVertical = seriesRange.Width < seriesRange.Height;
			if (seriesRange.Width == seriesRange.Height && (seriesRange.Width == 1 || dataRange.TopLeft.Column < seriesRange.TopLeft.Column))
				useVertical = dataRange.Width < dataRange.Height;
			return useVertical ? ChartViewSeriesDirection.Vertical : ChartViewSeriesDirection.Horizontal;
		}
		static ChartViewSeriesDirection GetSeriesDirection(CellRange dataRange, CellRange seriesRange, ChartViewType viewType) {
			if (viewType == ChartViewType.Bubble && seriesRange.TopLeft.EqualsPosition(dataRange.TopLeft)) {
				if (dataRange.Width % 2 == 1)
					return ChartViewSeriesDirection.Vertical;
				else if (dataRange.Height % 2 == 1)
					return ChartViewSeriesDirection.Horizontal;
			}
			return GetSeriesDirection(dataRange, seriesRange);
		}
		#endregion
	}
	#endregion
	#region ChartSeriesDataRanges
	public class ChartSeriesDataRanges {
		#region Fields
		CellRange range;
		CellRange seriesNameRange;
		CellRange seriesValuesRange;
		IDataReference seriesArguments;
		#endregion
		#region Properties
		public CellRange Range { get { return range; } }
		public CellRange SeriesNameRange { get { return seriesNameRange; } }
		public CellRange SeriesValuesRange { get { return seriesValuesRange; } }
		public IDataReference SeriesArguments { get { return seriesArguments; } }
		#endregion
		protected internal virtual void CalculateProperties(CellRange dataRange, CellRange seriesRange, ChartSeriesRangeModelBase model) {
			range = dataRange;
			seriesValuesRange = seriesRange;
			seriesNameRange = model.GetSeriesNamesRange(dataRange, seriesRange);
			seriesArguments = model.GetArgumentData(dataRange, seriesRange);
		}
	}
	#endregion
	#region ChartSeriesRangeModelBase (abstract class)
	public abstract class ChartSeriesRangeModelBase {
		protected internal ChartSeriesDataRanges Data { get; set; }
		protected internal abstract ChartViewSeriesDirection Direction { get; }
		protected internal abstract int SeriesCount { get; }
		protected CellRange CreateSeriesRange(ICellTable sheet, int left, int top, int right, int bottom) {
			CellPosition topLeft = new CellPosition(left, top, PositionType.Absolute, PositionType.Absolute);
			CellPosition bottomRight = new CellPosition(right, bottom, PositionType.Absolute, PositionType.Absolute);
			return new CellRange(sheet, topLeft, bottomRight);
		}
		protected IDataReference CreateChartDataReference(DocumentModel documentModel, ChartViewSeriesDirection direction, CellRange range, bool isNumber) {
			ChartDataReference result = new ChartDataReference(documentModel, direction, isNumber);
			result.SetRange(range);
			return result;
		}
		protected void ApplyDataCore(SeriesBase series, int seriesValuesIndex) {
			CellRange seriesValuesRange = GetSeriesRange(seriesValuesIndex);
			series.Values = CreateChartDataReference(series.DocumentModel, Direction, seriesValuesRange, true);
			series.Arguments = Data.SeriesArguments;
			CellRange seriesNameRange = GetSeriesNameRange(seriesValuesIndex);
			series.Text = CreateChartTextReference(series.Parent, seriesNameRange);
		}
		protected internal virtual void ApplyData(ISeries series, int seriesValuesIndex) {
			SeriesBase seriesBase = series as SeriesBase;
			if (seriesBase == null)
				return;
			ApplyDataCore(seriesBase, seriesValuesIndex);
		}
		CellRange GetTopRange(CellRange dataRange, CellRange seriesRange) {
			if (seriesRange.TopLeft.Row == dataRange.TopLeft.Row)
				return null;
			CellPosition topLeft = new CellPosition(seriesRange.TopLeft.Column, dataRange.TopLeft.Row, PositionType.Absolute, PositionType.Absolute);
			CellPosition bottomRight = new CellPosition(seriesRange.BottomRight.Column, seriesRange.TopLeft.Row - 1, PositionType.Absolute, PositionType.Absolute);
			return new CellRange(dataRange.Worksheet, topLeft, bottomRight);
		}
		CellRange GetLeftRange(CellRange dataRange, CellRange seriesRange) {
			if (seriesRange.TopLeft.Column == dataRange.TopLeft.Column)
				return null;
			CellPosition topLeft = new CellPosition(dataRange.TopLeft.Column, seriesRange.TopLeft.Row, PositionType.Absolute, PositionType.Absolute);
			CellPosition bottomRight = new CellPosition(seriesRange.TopLeft.Column - 1, seriesRange.BottomRight.Row, PositionType.Absolute, PositionType.Absolute);
			return new CellRange(dataRange.Worksheet, topLeft, bottomRight);
		}
		protected internal IDataReference GetArgumentData(CellRange dataRange, CellRange seriesRange) {
			CellRange axisLabelRange = GetAxisLabelsRange(dataRange, seriesRange);
			DocumentModel documentModel = dataRange.Worksheet.Workbook as DocumentModel;
			if (documentModel == null || axisLabelRange == null)
				return DataReference.Empty;
			return CreateChartDataReference(documentModel, Direction, axisLabelRange, IsNumber(axisLabelRange));
		}
		bool IsNumber(CellRange range) {
			foreach (ICellBase cell in range.GetExistingCellsEnumerable()) {
				if (cell.Value.IsText)
					return false;
			}
			return true;
		}
		protected internal CellRange GetSeriesNamesRange(CellRange dataRange, CellRange seriesRange) {
			return Direction == ChartViewSeriesDirection.Horizontal ? GetLeftRange(dataRange, seriesRange) : GetTopRange(dataRange, seriesRange);
		}
		protected internal void CalculateProperties(CellRange dataRange, CellRange seriesRange) {
			if (dataRange == null || seriesRange == null)
				return;
			Data = new ChartSeriesDataRanges();
			Data.CalculateProperties(dataRange, seriesRange, this);
		}
		CellRange GetAxisLabelsRange(CellRange dataRange, CellRange seriesRange) {
			return Direction == ChartViewSeriesDirection.Horizontal ? GetTopRange(dataRange, seriesRange) : GetLeftRange(dataRange, seriesRange);
		}
		IChartText CreateChartTextReference(IChart chart, CellRange seriesNameRange) {
			if (seriesNameRange == null)
				return null;
			ChartTextRef result = new ChartTextRef(chart);
			result.SetRange(seriesNameRange);
			return result;
		}
		protected internal abstract CellRange GetSeriesRange(int seriesValuesIndex);
		protected internal abstract CellRange GetSeriesNameRange(int seriesValuesIndex);
		protected internal virtual IDataReference CreateBubbleChartSizeDataReference(DocumentModel documentModel, int seriesValuesIndex) {
			return null;
		}
		protected internal virtual int GetNewSeriesCount(int beginIndex, CellRange seriesRange) {
			return SeriesCount - beginIndex;
		}
	}
	#endregion
	#region ChartSeriesRangeHorizontalModel
	public class ChartSeriesRangeHorizontalModel : ChartSeriesRangeModelBase {
		#region Properties
		protected internal override ChartViewSeriesDirection Direction { get { return ChartViewSeriesDirection.Horizontal; } }
		protected internal override int SeriesCount { get { return Data.SeriesValuesRange.Height; } }
		#endregion
		protected internal override CellRange GetSeriesRange(int seriesValuesIndex) {
			CellRange range = Data.SeriesValuesRange;
			if (range == null)
				return null;
			int row = range.TopLeft.Row + seriesValuesIndex;
			return CreateSeriesRange(range.Worksheet, range.TopLeft.Column, row, range.BottomRight.Column, row);
		}
		protected internal override CellRange GetSeriesNameRange(int seriesValuesIndex) {
			CellRange nameRange = Data.SeriesNameRange;
			CellRange seriesRange = Data.SeriesValuesRange;
			if (nameRange == null || seriesRange == null)
				return null;
			int row = seriesRange.TopLeft.Row + seriesValuesIndex;
			CellPosition topLeft = new CellPosition(nameRange.TopLeft.Column, row, PositionType.Absolute, PositionType.Absolute);
			CellPosition bottomRight = new CellPosition(nameRange.BottomRight.Column, row, PositionType.Absolute, PositionType.Absolute);
			return new CellRange(nameRange.Worksheet, topLeft, bottomRight);
		}
	}
	#endregion
	#region ChartSeriesRangeVerticalModel
	public class ChartSeriesRangeVerticalModel : ChartSeriesRangeModelBase {
		#region Properties
		protected internal override ChartViewSeriesDirection Direction { get { return ChartViewSeriesDirection.Vertical; } }
		protected internal override int SeriesCount { get { return Data.SeriesValuesRange.Width; } }
		#endregion
		protected internal override CellRange GetSeriesRange(int seriesValuesIndex) {
			CellRange range = Data.SeriesValuesRange;
			if (range == null)
				return null;
			int column = range.TopLeft.Column + seriesValuesIndex;
			return CreateSeriesRange(range.Worksheet, column, range.TopLeft.Row, column, range.BottomRight.Row);
		}
		protected internal override CellRange GetSeriesNameRange(int seriesValuesIndex) {
			CellRange nameRange = Data.SeriesNameRange;
			CellRange seriesRange = Data.SeriesValuesRange;
			if (nameRange == null || seriesRange == null)
				return null;
			int column = seriesRange.TopLeft.Column + seriesValuesIndex;
			CellPosition topLeft = new CellPosition(column, nameRange.TopLeft.Row, PositionType.Absolute, PositionType.Absolute);
			CellPosition bottomRight = new CellPosition(column, nameRange.BottomRight.Row, PositionType.Absolute, PositionType.Absolute);
			return new CellRange(nameRange.Worksheet, topLeft, bottomRight);
		}
	}
	#endregion
	#region ChartBubbleSeriesRangeModel (absract class)
	public abstract class ChartBubbleSeriesRangeModel : ChartSeriesRangeModelBase {
		protected internal override int SeriesCount { get { return GetSeriesCount(SizeSeries); } }
		protected abstract int SizeSeries { get; }
		protected internal override void ApplyData(ISeries series, int seriesValuesIndex) {
			base.ApplyData(series, seriesValuesIndex);
			BubbleSeries bubbleSeries = series as BubbleSeries;
			if (bubbleSeries != null)
				bubbleSeries.BubbleSize = CreateBubbleChartSizeDataReference(bubbleSeries.DocumentModel, seriesValuesIndex);
		}
		protected IDataReference CreateOneLiteralsChartDataReference(DocumentModel documentModel, int count) {
			ChartDataReference result = new ChartDataReference(documentModel, Direction, true);
			VariantArray array = VariantArray.Create(1, count);
			for (int i = 0; i < count; i++)
				array.SetValue(i, 0, 1);
			VariantValue cachedValue = VariantValue.FromArray(array);
			ParsedExpression expression = BasicExpressionCreator.CreateExpressionForVariantValue(cachedValue, OperandDataType.Default, documentModel.DataContext);
			result.FormulaData.SetExpressionCore(expression);
			result.CachedValue = cachedValue;
			return result;
		}
		protected internal override int GetNewSeriesCount(int beginIndex, CellRange seriesRange) {
			return GetSeriesCount(SizeSeries - beginIndex);
		}
		int GetSeriesCount(int size) {
			return (size + 1) / 2;
		}
	}
	#endregion
	#region ChartBubbleSeriesRangeHorizontalModel
	public class ChartBubbleSeriesRangeHorizontalModel : ChartBubbleSeriesRangeModel {
		protected internal override ChartViewSeriesDirection Direction { get { return ChartViewSeriesDirection.Horizontal; } }
		protected override int SizeSeries { get { return Data.SeriesValuesRange.Height; } }
		protected internal override CellRange GetSeriesRange(int seriesValuesIndex) {
			CellRange range = Data.SeriesValuesRange;
			if (range == null)
				return null;
			int row = range.TopLeft.Row + 2 * seriesValuesIndex;
			return CreateSeriesRange(range.Worksheet, range.TopLeft.Column, row, range.BottomRight.Column, row);
		}
		protected internal override CellRange GetSeriesNameRange(int seriesValuesIndex) {
			CellRange nameRange = Data.SeriesNameRange;
			CellRange seriesRange = Data.SeriesValuesRange;
			if (nameRange == null || seriesRange == null)
				return null;
			int row = seriesRange.TopLeft.Row + 2 * seriesValuesIndex;
			CellPosition topLeft = new CellPosition(nameRange.TopLeft.Column, row, PositionType.Absolute, PositionType.Absolute);
			CellPosition bottomRight = new CellPosition(nameRange.BottomRight.Column, row, PositionType.Absolute, PositionType.Absolute);
			return new CellRange(nameRange.Worksheet, topLeft, bottomRight);
		}
		protected internal override IDataReference CreateBubbleChartSizeDataReference(DocumentModel documentModel, int seriesValuesIndex) {
			CellRange range = Data.SeriesValuesRange;
			if (range == null)
				return null;
			int row = range.TopLeft.Row + 2 * seriesValuesIndex + 1;
			if (row > range.BottomRight.Row)
				return CreateOneLiteralsChartDataReference(documentModel, range.BottomRight.Column - range.TopLeft.Column + 1);
			CellRange seriesRange = CreateSeriesRange(range.Worksheet, range.TopLeft.Column, row, range.BottomRight.Column, row);
			return CreateChartDataReference(documentModel, Direction, seriesRange, true);
		}
	}
	#endregion
	#region ChartBubbleSeriesRangeVerticalModel
	public class ChartBubbleSeriesRangeVerticalModel : ChartBubbleSeriesRangeModel {
		protected internal override ChartViewSeriesDirection Direction { get { return ChartViewSeriesDirection.Vertical; } }
		protected override int SizeSeries { get { return Data.SeriesValuesRange.Width; } }
		protected internal override CellRange GetSeriesRange(int seriesValuesIndex) {
			CellRange range = Data.SeriesValuesRange;
			if (range == null)
				return null;
			int column = range.TopLeft.Column + 2 * seriesValuesIndex;
			return CreateSeriesRange(range.Worksheet, column, range.TopLeft.Row, column, range.BottomRight.Row);
		}
		protected internal override CellRange GetSeriesNameRange(int seriesValuesIndex) {
			CellRange nameRange = Data.SeriesNameRange;
			CellRange seriesRange = Data.SeriesValuesRange;
			if (nameRange == null || seriesRange == null)
				return null;
			int column = seriesRange.TopLeft.Column + 2 * seriesValuesIndex;
			CellPosition topLeft = new CellPosition(column, nameRange.TopLeft.Row, PositionType.Absolute, PositionType.Absolute);
			CellPosition bottomRight = new CellPosition(column, nameRange.BottomRight.Row, PositionType.Absolute, PositionType.Absolute);
			return new CellRange(nameRange.Worksheet, topLeft, bottomRight);
		}
		protected internal override IDataReference CreateBubbleChartSizeDataReference(DocumentModel documentModel, int seriesValuesIndex) {
			CellRange range = Data.SeriesValuesRange;
			if (range == null)
				return null;
			int column = range.TopLeft.Column + 2 * seriesValuesIndex + 1;
			if (column > range.BottomRight.Column)
				return CreateOneLiteralsChartDataReference(documentModel, range.BottomRight.Row - range.TopLeft.Row + 1);
			CellRange seriesRange = CreateSeriesRange(range.Worksheet, column, range.TopLeft.Row, column, range.BottomRight.Row);
			return CreateChartDataReference(documentModel, Direction, seriesRange, true);
		}
	}
	#endregion
}
