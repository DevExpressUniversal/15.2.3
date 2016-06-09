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
using System.Text;
using System.Xml;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region DataReferenceTypeOpenXml
	public enum DataReferenceTypeOpenXml {
		MultiLevelString = 0x1,
		StringLiteral = 0x2,
		NumberLiteral = 0x4,
		NumberReference = 0x8,
		StringReference = 0x10,
		All = 0x1F,
	}
	#endregion
	#region ChartDataReferenceDestination
	public class ChartDataReferenceDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("multiLvlStrRef", OnMultiLevelStringReference);
			result.Add("strLit", OnStringLiteral);
			result.Add("numLit", OnNumberLiteral);
			result.Add("numRef", OnNumberReference);
			result.Add("strRef", OnStringReference);
			return result;
		}
		static ChartDataReferenceDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (ChartDataReferenceDestination)importer.PeekDestination();
		}
		#endregion
		#region Fields
		readonly Action<IOpenXmlChartDataReference> setterMethod;
		readonly DataReferenceTypeOpenXml allowedTypes;
		IOpenXmlChartDataReference openXmlReference;
		#endregion
		public ChartDataReferenceDestination(SpreadsheetMLBaseImporter importer, DataReferenceTypeOpenXml allowedTypes, Action<IOpenXmlChartDataReference> setterMethod)
			: base(importer) {
			this.setterMethod = setterMethod;
			this.allowedTypes = allowedTypes;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			if (openXmlReference == null)
				Importer.ThrowInvalidFile();
			setterMethod(openXmlReference);
		}
		#region Handlers
		static void CheckIntegrity(SpreadsheetMLBaseImporter importer, ChartDataReferenceDestination thisDestination, DataReferenceTypeOpenXml type) {
			if ((thisDestination.allowedTypes & type) == 0)
				importer.ThrowInvalidFile();
			if (thisDestination.openXmlReference != null)
				importer.ThrowInvalidFile();
		}
		static Destination OnNumberReference(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ChartDataReferenceDestination thisDestination = GetThis(importer);
			CheckIntegrity(importer, thisDestination, DataReferenceTypeOpenXml.NumberReference);
			OpenXmlNumberReference numberReference = new OpenXmlNumberReference();
			thisDestination.openXmlReference = numberReference;
			return new ChartNumberReferenceDestination(importer, numberReference);
		}
		static Destination OnStringReference(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ChartDataReferenceDestination thisDestination = GetThis(importer);
			CheckIntegrity(importer, thisDestination, DataReferenceTypeOpenXml.StringReference);
			OpenXmlStringReference stringReference = new OpenXmlStringReference();
			thisDestination.openXmlReference = stringReference;
			return new ChartStringReferenceDestination(importer, stringReference);
		}
		static Destination OnNumberLiteral(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ChartDataReferenceDestination thisDestination = GetThis(importer);
			CheckIntegrity(importer, thisDestination, DataReferenceTypeOpenXml.NumberLiteral);
			OpenXmlNumberLiteral numberLiteral = new OpenXmlNumberLiteral();
			thisDestination.openXmlReference = numberLiteral;
			return new ChartNumberLiteralDestination(importer, numberLiteral);
		}
		static Destination OnMultiLevelStringReference(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ChartDataReferenceDestination thisDestination = GetThis(importer);
			CheckIntegrity(importer, thisDestination, DataReferenceTypeOpenXml.MultiLevelString);
			OpenXmlChartMLDataReference stringMlReference = new OpenXmlChartMLDataReference();
			thisDestination.openXmlReference = stringMlReference;
			return new ChartMultilevelStringReferenceDestination(importer, stringMlReference);
		}
		static Destination OnStringLiteral(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			ChartDataReferenceDestination thisDestination = GetThis(importer);
			CheckIntegrity(importer, thisDestination, DataReferenceTypeOpenXml.StringLiteral);
			OpenXmlStringLiteral stringLiteral = new OpenXmlStringLiteral();
			thisDestination.openXmlReference = stringLiteral;
			return new ChartStringLiteralDestination(importer, stringLiteral);
		}
		#endregion
	}
	#endregion
	#region ChartDataCacheImportHelper
	public class ChartDataCacheImportHelper {
		readonly ChartDataCache cache;
		readonly WorkbookDataContext context;
		readonly CellRangeBase dataRange;
		public ChartDataCacheImportHelper(ChartDataCache cache, WorkbookDataContext context, CellRangeBase dataRange) {
			this.cache = cache;
			this.context = context;
			this.dataRange = dataRange;
		}
		public void SetPointValue(int index, string value, string formatString) {
			SetPointValueToCellRangeBase(dataRange, -1, index, value, formatString);
		}
		public void SetPointValue(int level, int index, string value, string formatString) {
			SetPointValueToCellRangeBase(dataRange, level, index, value, formatString);
		}
		void SetPointValueToCellRangeBase(CellRangeBase range, int level, int index, string value, string formatString) {
			if (range.RangeType == CellRangeType.UnionRange) {
				CellUnion union = (CellUnion)range;
				for (int i = 0; i < union.InnerCellRanges.Count; i++) {
					CellRangeBase currentInnerRange = union.InnerCellRanges[i];
					long currentInnerRangeCellCount = currentInnerRange.Height;
					if (currentInnerRangeCellCount > index) {
						SetPointValueToCellRangeBase(currentInnerRange, level, index, value, formatString);
						break;
					}
					index -= (int)currentInnerRangeCellCount;
				}
			}
			else
				SetValueToCellRange(range as CellRange, level, index, value, formatString);
		}
		void SetValueToCellRange(CellRange range, int level, int index, string value, string formatString) {
			int relativeColumnIndex = 0;
			int relativeRowIndex = 0;
			if (level >= 0) {
				relativeColumnIndex = range.Width - level - 1;
				relativeRowIndex = index;
			}
			else {
				if (range.Height > 1)
					relativeRowIndex = index;
				else
					relativeColumnIndex = index;
			}
			SetPointValueCore(range, relativeColumnIndex, relativeRowIndex, value, formatString);
		}
		void SetPointValueCore(CellRange range, int relativeColumnIndex, int relativeRowIndex, string value, string formatString) {
			ChartCacheCell cell = GetRelativeCell(range, relativeColumnIndex, relativeRowIndex);
			cell.Value = CellValueFormatter.GetValueCore(value, context, false).Value;
			if (!string.IsNullOrEmpty(formatString))
				cell.FormatString = formatString;
		}
		ChartCacheCell GetRelativeCell(CellRange range, int relativeColumnIndex, int relativeRowIndex) {
			int rowIndex = range.TopLeft.Row + relativeRowIndex;
			int columnIndex = range.TopLeft.Column + relativeColumnIndex;
			return cache.GetCell(columnIndex, rowIndex);
		}
	}
	#endregion
	#region StringPoint
	public class StringPoint {
		#region Fields
		int index;
		#endregion
		public StringPoint() {
			Value = string.Empty;
		}
		public StringPoint(int idx, string value) {
			Index = idx;
			Value = value;
		}
		#region Properties
		public int Index {
			get { return index; }
			set {
				if (value < 0)
					throw new ArgumentException("Index value must be equal or greater 0");
				index = value;
			}
		}
		public string Value { get; set; }
		#endregion
	}
	#endregion
	#region NumberPoint
	public class NumberPoint : StringPoint {
		public NumberPoint()
			: base() {
		}
		public NumberPoint(int idx, string value, string fmt_code)
			: base(idx, value) {
			FormatCode = fmt_code;
		}
		#region Properties
		public string FormatCode { get; set; }
		#endregion
	}
	#endregion
	#region IOpenXmlChartDataReference
	public interface IOpenXmlChartDataReference {
		IDataReference ToDataReference(DocumentModel documentModel, ChartViewSeriesDirection seriesDirection, bool isNumber);
		void FillFromDataReference(ChartDataReference dataReference);
		bool TryDetectDirectionByReference(WorkbookDataContext context, out ChartViewSeriesDirection result);
	}
	#endregion
	#region OpenXmlChartDataReference
	public abstract class OpenXmlChartDataReference<T> : IOpenXmlChartDataReference where T : StringPoint {
		#region Fields
		readonly List<T> points = new List<T>();
		string reference;
		#endregion
		protected OpenXmlChartDataReference() {
		}
		#region Properties
		public IList<T> Points { get { return points; } }
		public string FormulaBody { get { return reference; } set { reference = value; } }
		#endregion
		public bool TryDetectDirectionByReference(WorkbookDataContext context, out ChartViewSeriesDirection result) {
			result = ChartViewSeriesDirection.Horizontal;
			context.PushCurrentCell(0, 0);
			context.Workbook.SuppressCellValueAssignment = false;
			try {
				CellRangeBase cellRangeValue = CellRangeBase.TryParse(reference, context);
				if (cellRangeValue != null) {
					if (cellRangeValue.GetMaxWidth() == 1) {
						result = ChartViewSeriesDirection.Vertical;
						return true;
					}
					if (cellRangeValue.GetMaxHeight() == 1) {
						result = ChartViewSeriesDirection.Horizontal;
						return true;
					}
					return false;
				}
			}
			finally {
				context.PopCurrentCell();
				context.Workbook.SuppressCellValueAssignment = true;
			}
			return false;
		}
		#region FillFromDataReference
		public void FillFromDataReference(ChartDataReference dataReference) {
			dataReference.FormulaData.PrepareExpression();
			FillCommonData(dataReference);
			FillPoints(dataReference);
		}
		protected virtual void FillCommonData(ChartDataReference dataReference) {
			FormulaBody = dataReference.FormulaBody;
		}
		protected virtual void FillPoints(ChartDataReference dataReference) {
			VariantValue cachedValue = dataReference.CachedValue;
			switch (cachedValue.Type) {
				case VariantValueType.None:
				case VariantValueType.Missing:
					break;
				case VariantValueType.Array:
					FillPointsFromArray(cachedValue.ArrayValue, dataReference.DocumentModel.DataContext);
					break;
				case VariantValueType.CellRange:
					FillPointsFromCellRange(cachedValue.CellRangeValue, dataReference.DocumentModel.DataContext, 0);
					break;
				default:
					FillPointsFromValue(cachedValue, dataReference.DocumentModel.DataContext);
					break;
			}
		}
		void FillPointsFromArray(IVariantArray variantArray, WorkbookDataContext context) {
			int pointCount = (int)variantArray.Count;
			for (int i = 0; i < pointCount; i++) {
				VariantValue value = variantArray[i];
				if (value.IsEmpty || value.IsMissing)
					continue;
				points.Add(CreatePoint(value, i, context));
			}
		}
		long FillPointsFromCellRange(CellRangeBase cellRangeBase, WorkbookDataContext context, long startIndex) {
			if (cellRangeBase.RangeType == CellRangeType.UnionRange) {
				CellUnion union = (CellUnion)cellRangeBase;
				foreach (CellRangeBase rangeBase in union.InnerCellRanges)
					startIndex += FillPointsFromCellRange(rangeBase, context, startIndex);
			}
			else {
				startIndex += FillPointsFromCellRange((CellRange)cellRangeBase, context, startIndex);
			}
			return startIndex;
		}
		long FillPointsFromCellRange(CellRange cellRange, WorkbookDataContext context, long startIndex) {
			CellPosition topLeft = cellRange.TopLeft;
			foreach (CellBase cell in cellRange.GetExistingCellsEnumerable()) {
				VariantValue value = cell.Value;
				if (value.IsEmpty || value.IsMissing)
					continue;
				points.Add(CreatePoint(cell, (int)(startIndex + cell.ColumnIndex - topLeft.Column + cell.RowIndex - topLeft.Row), context));
			}
			return cellRange.CellCount;
		}
		void FillPointsFromValue(VariantValue value, WorkbookDataContext context) {
			points.Add(CreatePoint(value, 0, context));
		}
		protected abstract T CreatePoint(VariantValue value, int index, WorkbookDataContext context);
		protected abstract T CreatePoint(CellBase cell, int index, WorkbookDataContext context);
		#endregion
		#region ToDataReference
		public IDataReference ToDataReference(DocumentModel documentModel, ChartViewSeriesDirection seriesDirection, bool isNumber) {
			ChartDataReference result = new ChartDataReference(documentModel, seriesDirection, isNumber);
			WorkbookDataContext context = documentModel.DataContext;
			bool isValidFormula = true;
			if (!string.IsNullOrEmpty(FormulaBody)) {
				ParsedExpression expression = context.ParseExpression(FormulaBody, context.DefinedNameProcessing ? OperandDataType.Default : OperandDataType.Value, false);
				if (expression == null)
					isValidFormula = false;
			}
			if (isValidFormula)
				PreparedDataReference(result, documentModel, context, seriesDirection);
			else
				PrepareDataLiteral(result, context, isNumber);
			ApplyToDataReference(result);
			return result;
		}
		void PrepareDataLiteral(ChartDataReference result, WorkbookDataContext context, bool isNumber) {
			if (points.Count <= 0) {
				result.CachedValue = VariantValue.Empty;
				return;
			}
			List<VariantValue> values = new List<VariantValue>();
			int maxIndex = -1;
			foreach (T point in points)
				maxIndex = Math.Max(maxIndex, point.Index);
			VariantValue emptyValue;
			if (isNumber)
				emptyValue = 0;
			else
				emptyValue = string.Empty;
			for (int i = 0; i <= maxIndex; i++)
				values.Add(emptyValue);
			foreach (T point in points) {
				values[point.Index] = CellValueFormatter.GetValueCore(point.Value, context, false).Value;
			}
			VariantArray array = new VariantArray();
			array.SetValues(values, maxIndex + 1, 1);
			VariantValue cachedValue = VariantValue.FromArray(array);
			ParsedExpression expression = BasicExpressionCreator.CreateExpressionForVariantValue(cachedValue, OperandDataType.Default, context);
			result.FormulaData.SetExpressionCore(expression);
			result.CachedValue = cachedValue;
		}
		void PreparedDataReference(ChartDataReference result, DocumentModel documentModel, WorkbookDataContext context, ChartViewSeriesDirection seriesDirection) {
			result.FormulaData.SetFormulaBodyTemporarily(FormulaBody);
			ChartDataCache cache = new ChartDataCache(documentModel);
			int maxIndex = 0;
			int dx = 0;
			int dy = 0;
			if (seriesDirection == ChartViewSeriesDirection.Vertical)
				dy = 1;
			else
				dx = 1;
			foreach (T point in points) {
				ChartCacheCell cell = cache.GetCell(point.Index * dx, point.Index * dy);
				ApplyDataToCell(cell, point, context);
				maxIndex = Math.Max(maxIndex, point.Index);
			}
			CellPosition refRangeTopLeft = new CellPosition(0, 0, PositionType.Absolute, PositionType.Absolute);
			CellPosition refRangeBottomRight = new CellPosition(maxIndex * dx, maxIndex * dy, PositionType.Absolute, PositionType.Absolute);
			CellRange referencedRange = new CellRange(cache, refRangeTopLeft, refRangeBottomRight);
			result.CachedValue = VariantValue.Create(referencedRange);
		}
		protected virtual void ApplyToDataReference(ChartDataReference reference) {
		}
		protected virtual void ApplyDataToCell(ChartCacheCell cell, T point, WorkbookDataContext context) {
			cell.Value = CellValueFormatter.GetValueCore(point.Value, context, false).Value;
		}
		#endregion
	}
	#endregion
	#region OpenXmlStringReference
	public class OpenXmlStringReference : OpenXmlChartDataReference<StringPoint> {
		public static OpenXmlStringReference FromChartTextRef(ChartTextRef textRef) {
			OpenXmlStringReference strRef = new OpenXmlStringReference();
			strRef.FormulaBody = textRef.FormulaBody;
			string plainText = textRef.PlainText;
			if (!string.IsNullOrEmpty(plainText))
				strRef.Points.Add(new StringPoint(0, plainText));
			return strRef;
		}
		public ChartTextRef ToChartTextRef(IChart parent) {
			ChartTextRef result = new ChartTextRef(parent);
			result.FormulaData.SetFormulaBodyTemporarily(FormulaBody);
			if (Points.Count > 0) {
				StringBuilder stringBuilder = new StringBuilder();
				foreach (StringPoint point in Points) {
					stringBuilder.Append(point.Value);
					stringBuilder.Append(" ");
				}
				if (stringBuilder.Length > 0) {
					stringBuilder.Remove(stringBuilder.Length - 1, 1);
					string text = stringBuilder.ToString();
					result.CachedValue = text;
				}
			}
			return result;
		}
		#region FillFromDataReference
		protected override StringPoint CreatePoint(VariantValue value, int index, WorkbookDataContext context) {
			string stringValue = value.ToText(context).GetTextValue(context.Workbook.SharedStringTable);
			return new StringPoint(index, stringValue);
		}
		protected override StringPoint CreatePoint(CellBase cell, int index, WorkbookDataContext context) {
			return CreatePoint(cell.Value, index, context);
		}
		#endregion
	}
	#endregion
	#region OpenXmlNumberReference
	public class OpenXmlNumberReference : OpenXmlChartDataReference<NumberPoint> {
		string formatCode;
		public string FormatCode { get { return formatCode; } set { formatCode = value; } }
		protected override void ApplyToDataReference(ChartDataReference reference) {
			reference.SetFormatCodeCore(formatCode);
		}
		protected override void ApplyDataToCell(ChartCacheCell cell, NumberPoint point, WorkbookDataContext context) {
			base.ApplyDataToCell(cell, point, context);
			if (!string.IsNullOrEmpty(point.FormatCode))
				cell.FormatString = point.FormatCode;
		}
		#region FillFromDataReference
		protected override void FillCommonData(ChartDataReference dataReference) {
			base.FillCommonData(dataReference);
			FormatCode = dataReference.FormatCode;
		}
		protected override NumberPoint CreatePoint(VariantValue value, int index, WorkbookDataContext context) {
			string stringValue = ConvertValue(value, context);
			return new NumberPoint(index, stringValue, string.Empty);
		}
		protected override NumberPoint CreatePoint(CellBase cell, int index, WorkbookDataContext context) {
			string stringValue = ConvertValue(cell.Value, context);
			string formatCode = string.Empty;
			IFormatStringAccessor formatAccessor = cell as IFormatStringAccessor;
			if (formatAccessor != null)
				formatCode = formatAccessor.FormatString;
			return new NumberPoint(index, stringValue, formatCode);
		}
		string ConvertValue(VariantValue value, WorkbookDataContext context) {
			if (!value.IsText)
				return value.ToNumeric(context).ToText(context).GetTextValue(context.StringTable);
			else
				return "0";
		}
		#endregion
	}
	#endregion
	#region Literal
	#region OpenXmlChartDataLiteral
	public abstract class OpenXmlChartDataLiteral<T> : IOpenXmlChartDataReference where T : StringPoint {
		#region Fields
		readonly List<T> points = new List<T>();
		#endregion
		protected OpenXmlChartDataLiteral() {
		}
		#region Properties
		public IList<T> Points { get { return points; } }
		#endregion
		public bool TryDetectDirectionByReference(WorkbookDataContext context, out ChartViewSeriesDirection result) {
			result = ChartViewSeriesDirection.Horizontal;
			return true;
		}
		#region FillFromDataReference
		public void FillFromDataReference(ChartDataReference dataReference) {
			throw new NotSupportedException();
		}
		#endregion
		#region ToDataReference
		public IDataReference ToDataReference(DocumentModel documentModel, ChartViewSeriesDirection seriesDirection, bool isNumber) {
			WorkbookDataContext context = documentModel.DataContext;
			ChartDataReference result = new ChartDataReference(documentModel, seriesDirection, isNumber);
			ApplyToDataReference(result);
			if (points.Count <= 0) {
				result.CachedValue = VariantValue.Empty;
				return result;
			}
			List<VariantValue> values = new List<VariantValue>();
			int maxIndex = -1;
			foreach (T point in points)
				maxIndex = Math.Max(maxIndex, point.Index);
			VariantValue emptyValue;
			if (isNumber)
				emptyValue = 0;
			else
				emptyValue = string.Empty;
			for (int i = 0; i <= maxIndex; i++)
				values.Add(emptyValue);
			foreach (T point in points) {
				if(string.IsNullOrEmpty(point.Value))
					values[point.Index] = string.Empty;
				else
					values[point.Index] = CellValueFormatter.GetValueCore(point.Value, context, false).Value;
			}
			VariantArray array = new VariantArray();
			array.SetValues(values, maxIndex + 1, 1);
			VariantValue cachedValue = VariantValue.FromArray(array);
			ParsedExpression expression = BasicExpressionCreator.CreateExpressionForVariantValue(cachedValue, OperandDataType.Default, context);
			result.FormulaData.SetExpressionCore(expression);
			result.CachedValue = cachedValue;
			return result;
		}
		protected virtual void ApplyToDataReference(ChartDataReference reference) {
		}
		protected virtual void ApplyDataToCell(ChartCacheCell cell, T point, WorkbookDataContext context) {
			cell.Value = CellValueFormatter.GetValueCore(point.Value, context, false).Value;
		}
		#endregion
	}
	#endregion
	#region OpenXmlStringLiteral
	public class OpenXmlStringLiteral : OpenXmlChartDataLiteral<StringPoint> {
	}
	#endregion
	#region OpenXmlNumberLiteral
	public class OpenXmlNumberLiteral : OpenXmlChartDataLiteral<NumberPoint> {
		string formatCode;
		public string FormatCode { get { return formatCode; } set { formatCode = value; } }
		protected override void ApplyToDataReference(ChartDataReference reference) {
			reference.SetFormatCodeCore(formatCode);
		}
	}
	#endregion
	#endregion
	#region OpenXmlChartMLDataReference
	public class OpenXmlChartMLDataReference : IOpenXmlChartDataReference {
		#region Fields
		readonly List<OpenXmlChartMLReferenceLevel> levels = new List<OpenXmlChartMLReferenceLevel>();
		string reference;
		#endregion
		public OpenXmlChartMLDataReference() {
		}
		#region Properties
		public List<OpenXmlChartMLReferenceLevel> Levels { get { return levels; } }
		public string FormulaBody { get { return reference; } set { reference = value; } }
		#endregion
		#region IOpenXmlChartDataReference Members
		public bool TryDetectDirectionByReference(WorkbookDataContext context, out ChartViewSeriesDirection result) {
			result = ChartViewSeriesDirection.Horizontal;
			return false;
		}
		#region ToDataReference
		public IDataReference ToDataReference(DocumentModel documentModel, ChartViewSeriesDirection seriesDirection, bool isNumber) {
			WorkbookDataContext context = documentModel.DataContext;
			if (!string.IsNullOrEmpty(FormulaBody)) {
				ParsedExpression expression = context.ParseExpression(FormulaBody, context.DefinedNameProcessing ? OperandDataType.Default : OperandDataType.Value, false);
				if (expression == null)
					return DataReference.Empty;
			}
			ChartDataReference result = new ChartDataReference(documentModel, seriesDirection, isNumber);
			result.FormulaData.SetFormulaBodyTemporarily(FormulaBody);
			ChartDataCache cache = new ChartDataCache(documentModel);
			int maxIndex = 0;
			int levelCount = levels.Count;
			for (int i = levelCount - 1; i >= 0; i--) {
				List<StringPoint> points = levels[i].Points;
				foreach (StringPoint point in points) {
					ChartCacheCell cell = cache.GetCell(levelCount - i - 1, point.Index);
					ApplyDataToCell(cell, point, context);
					maxIndex = Math.Max(maxIndex, point.Index);
				}
			}
			int width = Math.Max(levels.Count - 1, 1);
			CellPosition refRangeTopLeft = new CellPosition(0, 0, PositionType.Absolute, PositionType.Absolute);
			CellPosition refRangeBottomRight = new CellPosition(width, maxIndex, PositionType.Absolute, PositionType.Absolute);
			CellRange referencedRange = new CellRange(cache, refRangeTopLeft, refRangeBottomRight);
			result.CachedValue = VariantValue.Create(referencedRange);
			return result;
		}
		protected virtual void ApplyDataToCell(ChartCacheCell cell, StringPoint point, WorkbookDataContext context) {
			cell.Value = CellValueFormatter.GetValueCore(point.Value, context, false).Value;
		}
		#endregion
		#region FillFromDataReference
		public void FillFromDataReference(ChartDataReference dataReference) {
			FillCommonData(dataReference);
			FillPoints(dataReference);
		}
		protected virtual void FillCommonData(ChartDataReference dataReference) {
			FormulaBody = dataReference.FormulaBody;
		}
		protected virtual void FillPoints(ChartDataReference dataReference) {
			VariantValue cachedValue = dataReference.CachedValue;
			System.Diagnostics.Debug.Assert(cachedValue.IsCellRange);
			FillPointsFromCellRange(dataReference.SeriesDirection, cachedValue.CellRangeValue, dataReference.DocumentModel.DataContext);
		}
		void FillPointsFromCellRange(ChartViewSeriesDirection seriesDirection, CellRangeBase cellRangeBase, WorkbookDataContext context) {
			int levelCount;
			if (seriesDirection == ChartViewSeriesDirection.Vertical) {
				levelCount = cellRangeBase.GetMaxWidth();
			}
			else {
				levelCount = cellRangeBase.GetMaxHeight();
			}
			List<CellRange> cellRanges = new List<CellRange>();
			cellRangeBase.AddRanges(cellRanges);
			for (int i = levelCount - 1; i >= 0; i--) {
				OpenXmlChartMLReferenceLevel level = new OpenXmlChartMLReferenceLevel();
				this.levels.Add(level);
				foreach (CellRange range in cellRanges) {
					int pointsInLevel = seriesDirection == ChartViewSeriesDirection.Vertical ? range.Width : range.Height;
					if (pointsInLevel <= i)
						continue;
					CellRange levelRange = seriesDirection == ChartViewSeriesDirection.Vertical ? range.GetSubColumnRange(i, i) : range.GetSubRowRange(i, i);
					CellPosition topLeft = levelRange.TopLeft;
					foreach (CellBase cell in levelRange.GetExistingCellsEnumerable()) {
						VariantValue value = cell.Value;
						if (value.IsEmpty || value.IsMissing)
							continue;
						level.Points.Add(CreatePoint(cell.Value, cell.ColumnIndex - topLeft.Column + cell.RowIndex - topLeft.Row, context));
					}
				}
			}
		}
		protected StringPoint CreatePoint(VariantValue value, int index, WorkbookDataContext context) {
			string stringValue;
			if (value.IsError)
				stringValue = CellErrorFactory.GetErrorName(value.ErrorValue, context);
			else
				stringValue = value.ToText(context).GetTextValue(context.Workbook.SharedStringTable);
			return new StringPoint(index, stringValue);
		}
		#endregion
		#endregion
	}
	#endregion
	#region OpenXmlChartMLReferenceLevel
	public class OpenXmlChartMLReferenceLevel {
		List<StringPoint> points;
		public OpenXmlChartMLReferenceLevel() {
			points = new List<StringPoint>();
		}
		public List<StringPoint> Points { get { return points; } }
	}
	#endregion
}
