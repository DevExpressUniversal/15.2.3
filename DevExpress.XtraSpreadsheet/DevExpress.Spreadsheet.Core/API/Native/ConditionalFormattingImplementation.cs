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
using DevExpress.Utils;
using DevExpress.Spreadsheet;
using DevExpress.XtraSpreadsheet.Localization;
using System.Collections.Generic;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	abstract partial class NativeConditionalFormatting : ConditionalFormatting {
		readonly Model.ConditionalFormatting modelCondition;
		readonly NativeWorksheet worksheet;
		bool isValid;
		protected NativeConditionalFormatting(NativeWorksheet worksheet, Model.ConditionalFormatting modelCondition) {
			Guard.ArgumentNotNull(worksheet, "worksheet");
			Guard.ArgumentNotNull(modelCondition, "modelCondition");
			this.worksheet = worksheet;
			this.modelCondition = modelCondition;
			this.isValid = true;
		}
		#region Properties
		protected NativeWorksheet Worksheet { get { return worksheet; } }
		protected internal Model.ConditionalFormatting ModelConditionBase {
			get {
				CheckValid();
				return modelCondition;
			}
		}
		protected internal bool IsValid { get { return isValid; } }
		public int Priority { get { return ModelConditionBase.Priority; } set { ModelConditionBase.Priority = value; } }
		public bool StopIfTrue { get { return ModelConditionBase.StopIfTrue; } set { ModelConditionBase.StopIfTrue = value; } }
		public Formatting Formatting {
			get {
				CheckValid();
				Model.FormulaConditionalFormatting formulaCF = modelCondition as Model.FormulaConditionalFormatting;
				if (formulaCF == null)
					DevExpress.Office.Utils.Exceptions.ThrowInternalException();
				return new NativeConditionalFormat(formulaCF);
			}
		}
		public Range Range {
			get {
				return new NativeRange(ModelConditionBase.CellRange, worksheet);
			}
			set {
				Guard.ArgumentNotNull(value, "Range");
				Model.ConditionalFormatting modelFormatting = ModelConditionBase;
				if (!object.ReferenceEquals(value.Worksheet, worksheet))
					throw new ArgumentException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorUseRangeFromAnotherWorksheet));
				modelFormatting.SetCellRange(worksheet.GetModelRange(value));
			}
		}
		public IList<Range> Ranges {
			get {
				List<Range> result = new List<Range>();
				if (ModelConditionBase.CellRange as Model.CellUnion != null) {
					List<Model.CellRangeBase> modelRanges = ((Model.CellUnion)ModelConditionBase.CellRange).InnerCellRanges;
					for (int i = 0; i < modelRanges.Count; i++)
						result.Add(new NativeRange((Model.CellRange)modelRanges[i], worksheet));
					return result;
				}
				result.Add(new NativeRange((Model.CellRange)ModelConditionBase.CellRange, worksheet));
				return result;
			}
			set {
				Guard.ArgumentNotNull(value, "Ranges");
				Guard.ArgumentPositive(value.Count, "Ranges.Count");
				Model.ConditionalFormatting modelFormatting = ModelConditionBase;
				if (!object.ReferenceEquals(value[0].Worksheet, worksheet))
					throw new ArgumentException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorUseRangeFromAnotherWorksheet));
				Model.CellRangeBase newRange = worksheet.GetModelRange(value[0]);
				for (int i = 1; i < value.Count; i++) {
					if (!object.ReferenceEquals(value[i].Worksheet, worksheet))
						throw new ArgumentException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorUseRangeFromAnotherWorksheet));
					Model.VariantValue rangeValue = newRange.ConcatinateWith(worksheet.GetModelRange(value[i]));
					if (rangeValue.IsCellRange)
						newRange = rangeValue.CellRangeValue;
					else
						throw new ArgumentException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorInvalidRange));
				}
				modelFormatting.SetCellRange(newRange);
			}
		}
		#endregion
		protected internal void CheckValid() {
			if (!IsValid)
				throw new InvalidOperationException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorUseDeletedConditionalFormatting));
		}
		protected internal void Invalidate() {
			CheckValid();
			isValid = false;
		}
		protected internal int ChangeConditionalFormattingRange(Range range) {
			if (!Range.IsIntersecting(range))
				return -1;
			int countIntersected = GetCountIntersect(range);
			switch (countIntersected) {
				case 0:
					CutConditionRangeWithoutIntersections(range);
					return -1;
				case 1:
					CutConditionRangeWithOneIntersection(range);
					return -1;
				case 2:
					CutConditionRangeWithTwoIntersections(range);
					return -1;
				case 3:
					CutConditionRangeWithThreeIntersections(range);
					return -1;
				default:
					return Worksheet.ConditionalFormattings.IndexOf(this);
			}
		}
		protected internal void CutConditionRangeWithThreeIntersections(Range range) {
			if (Range.RightColumnIndex > range.RightColumnIndex) {
				Model.CellRange strangeRange1 = GetRightRange(range.RightColumnIndex + 1, Range.TopRowIndex, Range.RightColumnIndex, Range.BottomRowIndex);
				modelCondition.SetCellRange(strangeRange1);
				return;
			}
			if (Range.LeftColumnIndex < range.LeftColumnIndex) {
				Model.CellRange strangeRange2 = GetLeftRange(Range.LeftColumnIndex, Range.TopRowIndex, range.LeftColumnIndex - 1, Range.BottomRowIndex);
				modelCondition.SetCellRange(strangeRange2);
				return;
			}
			if (Range.BottomRowIndex > range.BottomRowIndex) {
				modelCondition.SetCellRange(GetBottomRange(range));
				return;
			}
			if (Range.TopRowIndex < range.TopRowIndex) {
				modelCondition.SetCellRange(GetTopRange(range));
				return;
			}
		}
		protected internal void CutConditionRangeWithTwoIntersections(Range range) {
			List<Model.CellRangeBase> ranges = new List<Model.CellRangeBase>();
			if (Range.TopRowIndex >= range.TopRowIndex && Range.BottomRowIndex <= range.BottomRowIndex) {
				ranges.Add(GetLeftRange(Range.LeftColumnIndex, Range.TopRowIndex, range.LeftColumnIndex - 1, Range.BottomRowIndex));
				ranges.Add(GetRightRange(range.RightColumnIndex + 1, Range.TopRowIndex, Range.RightColumnIndex, Range.BottomRowIndex));
			}
			if (Range.LeftColumnIndex >= range.LeftColumnIndex && Range.RightColumnIndex <= range.RightColumnIndex) {
				ranges.Add(GetTopRange(range));
				ranges.Add(GetBottomRange(range));
			}
			if (Range.LeftColumnIndex >= range.LeftColumnIndex && Range.TopRowIndex >= range.TopRowIndex) {
				ranges.Add(GetBottomRange(range));
				ranges.Add(GetRightRange(range.RightColumnIndex + 1, Range.TopRowIndex, Range.RightColumnIndex, range.BottomRowIndex));
			}
			if (Range.TopRowIndex >= range.TopRowIndex && Range.RightColumnIndex <= range.LeftColumnIndex) {
				ranges.Add(GetBottomRange(range));
				ranges.Add(GetLeftRange(Range.LeftColumnIndex, Range.TopRowIndex, range.LeftColumnIndex - 1, range.BottomRowIndex));
			}
			if (Range.BottomRowIndex <= range.BottomRowIndex && Range.RightColumnIndex <= range.RightColumnIndex) {
				ranges.Add(GetTopRange(range));
				ranges.Add(GetLeftRange(Range.LeftColumnIndex, range.TopRowIndex, range.LeftColumnIndex - 1, Range.BottomRowIndex));
			}
			if (Range.BottomRowIndex <= range.BottomRowIndex && Range.LeftColumnIndex >= range.LeftColumnIndex) {
				ranges.Add(GetTopRange(range));
				ranges.Add(GetRightRange(range.RightColumnIndex + 1, range.TopRowIndex, Range.RightColumnIndex, Range.BottomRowIndex));
			}
			Model.CellUnion union = new Model.CellUnion(ranges);
			modelCondition.SetCellRange(union);
		}
		protected internal void CutConditionRangeWithOneIntersection(Range range) {
			List<Model.CellRangeBase> ranges = new List<Model.CellRangeBase>();
			if (Range.TopRowIndex >= range.TopRowIndex) {
				ranges.Add(GetBottomRange(range));
				ranges.Add(GetLeftRange(Range.LeftColumnIndex, Range.TopRowIndex, range.LeftColumnIndex - 1, range.BottomRowIndex));
				ranges.Add(GetRightRange(range.RightColumnIndex + 1, Range.TopRowIndex, Range.RightColumnIndex, range.BottomRowIndex));
			}
			if (Range.BottomRowIndex <= range.TopRowIndex) {
				ranges.Add(GetTopRange(range));
				ranges.Add(GetLeftRange(Range.LeftColumnIndex, range.TopRowIndex, range.LeftColumnIndex - 1, Range.BottomRowIndex));
				ranges.Add(GetRightRange(range.RightColumnIndex + 1, range.TopRowIndex, Range.RightColumnIndex, Range.BottomRowIndex));
			}
			if (Range.LeftColumnIndex >= range.LeftColumnIndex) {
				ranges.Add(GetTopRange(range));
				ranges.Add(GetBottomRange(range));
				ranges.Add(GetRightRange(range.RightColumnIndex + 1, range.TopRowIndex, Range.RightColumnIndex, range.BottomRowIndex));
			}
			if (Range.RightColumnIndex <= range.RightColumnIndex) {
				ranges.Add(GetTopRange(range));
				ranges.Add(GetBottomRange(range));
				ranges.Add(GetLeftRange(Range.LeftColumnIndex, range.TopRowIndex, range.LeftColumnIndex - 1, range.BottomRowIndex));
			}
			Model.CellUnion union = new Model.CellUnion(ranges);
			modelCondition.SetCellRange(union);
		}
		protected internal void CutConditionRangeWithoutIntersections(Range range) {
			List<Model.CellRangeBase> ranges = new List<Model.CellRangeBase>();
			ranges.Add(GetTopRange(range));
			ranges.Add(GetBottomRange(range));
			ranges.Add(GetLeftRange(Range.LeftColumnIndex, range.TopRowIndex, range.LeftColumnIndex - 1, range.BottomRowIndex));
			ranges.Add(GetRightRange(range.RightColumnIndex + 1, range.TopRowIndex, Range.RightColumnIndex, range.BottomRowIndex));
			Model.CellUnion union = new Model.CellUnion(ranges);
			modelCondition.SetCellRange(union);
		}
		Model.CellRange GetLeftRange(int topLeftColumn, int topLeftRow, int bottomRightColumn, int bottomRightRow) {
			return CreateCellRange(topLeftColumn, topLeftRow, bottomRightColumn, bottomRightRow);
		}
		Model.CellRange GetRightRange(int topLeftColumn, int topLeftRow, int bottomRightColumn, int bottomRightRow) {
			return CreateCellRange(topLeftColumn, topLeftRow, bottomRightColumn, bottomRightRow);
		}
		Model.CellRange GetBottomRange(Range range) {
			return CreateCellRange(Range.LeftColumnIndex, range.BottomRowIndex + 1, Range.RightColumnIndex, Range.BottomRowIndex);
		}
		Model.CellRange GetTopRange(Range range) {
			return CreateCellRange(Range.LeftColumnIndex, Range.TopRowIndex, Range.RightColumnIndex, range.TopRowIndex - 1);
		}
		Model.CellRange CreateCellRange(int topLeftColumn, int topLeftRow, int bottomRightColumn, int bottomRightRow) {
			Model.CellPosition topLeft = new Model.CellPosition(topLeftColumn, topLeftRow, Model.PositionType.Absolute, Model.PositionType.Absolute);
			Model.CellPosition bottomRight = new Model.CellPosition(bottomRightColumn, bottomRightRow, Model.PositionType.Absolute, Model.PositionType.Absolute);
			return new Model.CellRange(Worksheet.ModelWorksheet, topLeft, bottomRight);
		}
		int GetCountIntersect(Range range) {
			int countIntersected = 0;
			if (Range.LeftColumnIndex >= range.LeftColumnIndex)
				countIntersected++;
			if (Range.TopRowIndex >= range.TopRowIndex)
				countIntersected++;
			if (Range.RightColumnIndex <= range.RightColumnIndex)
				countIntersected++;
			if (Range.BottomRowIndex <= range.BottomRowIndex)
				countIntersected++;
			return countIntersected;
		}
	}
	abstract partial class ValueBasedNativeConditionalFormatting : NativeConditionalFormatting {
		protected ValueBasedNativeConditionalFormatting(NativeWorksheet worksheet, Model.ConditionalFormatting modelCondition)
			: base(worksheet, modelCondition) {
		}
		protected string ValueInvariant {
			get {
				Model.WorkbookDataContext context = ModelConditionBase.Sheet.DataContext;
				context.PushCulture(System.Globalization.CultureInfo.InvariantCulture);
				try {
					return ((Model.ValueBasedFormulaConditionalFormatting)ModelConditionBase).Value;
				}
				finally {
					context.PopCulture();
				}
			}
			set {
				Model.WorkbookDataContext context = ModelConditionBase.Sheet.DataContext;
				context.PushCulture(System.Globalization.CultureInfo.InvariantCulture);
				try {
					((Model.ValueBasedFormulaConditionalFormatting)ModelConditionBase).SetValue(value, ApiErrorHandler.Instance);
				}
				finally {
					context.PopCulture();
				}
			}
		}
	}
	abstract partial class RangeValueBasedNativeConditionalFormatting : ValueBasedNativeConditionalFormatting {
		protected RangeValueBasedNativeConditionalFormatting(NativeWorksheet worksheet, Model.ConditionalFormatting modelCondition)
			: base(worksheet, modelCondition) {
		}
		protected string Value2Invariant {
			get {
				Model.WorkbookDataContext context = ModelConditionBase.Sheet.DataContext;
				context.PushCulture(System.Globalization.CultureInfo.InvariantCulture);
				try {
					return ((Model.RangeValueBasedFormulaConditionalFormatting)ModelConditionBase).Value2;
				}
				finally {
					context.PopCulture();
				}
			}
			set {
				Model.WorkbookDataContext context = ModelConditionBase.Sheet.DataContext;
				context.PushCulture(System.Globalization.CultureInfo.InvariantCulture);
				try {
					((Model.RangeValueBasedFormulaConditionalFormatting)ModelConditionBase).SetValue2(value, ApiErrorHandler.Instance);
				}
				finally {
					context.PopCulture();
				}
			}
		}
	}
	partial class NativeRankConditionalFormatting : NativeConditionalFormatting, RankConditionalFormatting {
		public NativeRankConditionalFormatting(NativeWorksheet worksheet, Model.FormulaConditionalFormatting modelCondition)
			: base(worksheet, modelCondition) {
		}
		Model.RankFormulaConditionalFormatting ModelCondition { get { return (Model.RankFormulaConditionalFormatting)ModelConditionBase; } }
		public ConditionalFormattingRankCondition Condition {
			get { return (ConditionalFormattingRankCondition)ModelCondition.Condition; }
			set { ModelCondition.Condition = (Model.ConditionalFormattingRankCondition)value; }
		}
		public int Rank { get { return ModelCondition.Rank; } set { ModelCondition.Rank = value; } }
	}
	partial class NativeAverageConditionalFormatting : NativeConditionalFormatting, AverageConditionalFormatting {
		public NativeAverageConditionalFormatting(NativeWorksheet worksheet, Model.FormulaConditionalFormatting modelCondition)
			: base(worksheet, modelCondition) {
		}
		Model.AverageFormulaConditionalFormatting ModelCondition { get { return (Model.AverageFormulaConditionalFormatting)ModelConditionBase; } }
		public ConditionalFormattingAverageCondition Condition {
			get { return (ConditionalFormattingAverageCondition)ModelCondition.Condition; }
			set { ModelCondition.Condition = (Model.ConditionalFormattingAverageCondition)value; }
		}
		public int StdDev { get { return ModelCondition.StdDev; } set { ModelCondition.StdDev = value; } }
	}
	partial class NativeRangeConditionalFormatting : RangeValueBasedNativeConditionalFormatting, RangeConditionalFormatting {
		public NativeRangeConditionalFormatting(NativeWorksheet worksheet, Model.FormulaConditionalFormatting modelCondition)
			: base(worksheet, modelCondition) {
		}
		Model.RangeFormulaConditionalFormatting ModelCondition { get { return (Model.RangeFormulaConditionalFormatting)ModelConditionBase; } }
		public ConditionalFormattingRangeCondition Condition {
			get { return (ConditionalFormattingRangeCondition)ModelCondition.Condition; }
			set { ModelCondition.Condition = (Model.ConditionalFormattingRangeCondition)value; }
		}
		public string LowBound { get { return ValueInvariant; } set { ValueInvariant = value; } }
		public string HighBound { get { return Value2Invariant; } set { Value2Invariant = value; } }
	}
	partial class NativeTextConditionalFormatting : ValueBasedNativeConditionalFormatting, TextConditionalFormatting {
		public NativeTextConditionalFormatting(NativeWorksheet worksheet, Model.FormulaConditionalFormatting modelCondition)
			: base(worksheet, modelCondition) {
		}
		Model.TextFormulaConditionalFormatting ModelCondition { get { return (Model.TextFormulaConditionalFormatting)ModelConditionBase; } }
		public ConditionalFormattingTextCondition Condition {
			get { return (ConditionalFormattingTextCondition)ModelCondition.Condition; }
			set { ModelCondition.Condition = (Model.ConditionalFormattingTextCondition)value; }
		}
		public string Text { get { return ValueInvariant; } set { ValueInvariant = value; } }
	}
	partial class NativeSpecialConditionalFormatting : NativeConditionalFormatting, SpecialConditionalFormatting {
		public NativeSpecialConditionalFormatting(NativeWorksheet worksheet, Model.FormulaConditionalFormatting modelCondition)
			: base(worksheet, modelCondition) {
		}
		protected Model.SpecialFormulaConditionalFormatting ModelCondition { get { return (Model.SpecialFormulaConditionalFormatting)ModelConditionBase; } }
		public ConditionalFormattingSpecialCondition Condition {
			get { return (ConditionalFormattingSpecialCondition)ModelCondition.Condition; }
			set { ModelCondition.Condition = (Model.ConditionalFormattingSpecialCondition)value; }
		}
	}
	partial class NativeTimePeriodConditionalFormatting : NativeConditionalFormatting, TimePeriodConditionalFormatting {
		public NativeTimePeriodConditionalFormatting(NativeWorksheet worksheet, Model.FormulaConditionalFormatting modelCondition)
			: base(worksheet, modelCondition) {
		}
		Model.TimePeriodFormulaConditionalFormatting ModelCondition { get { return (Model.TimePeriodFormulaConditionalFormatting)ModelConditionBase; } }
		public ConditionalFormattingTimePeriod Condition {
			get { return (ConditionalFormattingTimePeriod)ModelCondition.TimePeriod; }
			set { ModelCondition.TimePeriod = (Model.ConditionalFormattingTimePeriod)value; }
		}
	}
	partial class NativeExpressionConditionalFormatting : ValueBasedNativeConditionalFormatting, ExpressionConditionalFormatting {
		public NativeExpressionConditionalFormatting(NativeWorksheet worksheet, Model.FormulaConditionalFormatting modelCondition)
			: base(worksheet, modelCondition) {
		}
		Model.ExpressionFormulaConditionalFormatting ModelCondition { get { return (Model.ExpressionFormulaConditionalFormatting)ModelConditionBase; } }
		public ConditionalFormattingExpressionCondition Condition {
			get { return (ConditionalFormattingExpressionCondition)ModelCondition.Condition; }
			set { ModelCondition.Condition = (Model.ConditionalFormattingExpressionCondition)value; }
		}
		public string Value { get { return ValueInvariant; } set { ValueInvariant = value; } }
	}
	partial class NativeFormulaExpressionConditionalFormatting : ValueBasedNativeConditionalFormatting, FormulaExpressionConditionalFormatting {
		public NativeFormulaExpressionConditionalFormatting(NativeWorksheet worksheet, Model.FormulaConditionalFormatting modelCondition)
			: base(worksheet, modelCondition) {
		}
		Model.ExpressionFormulaConditionalFormatting ModelCondition { get { return (Model.ExpressionFormulaConditionalFormatting)ModelConditionBase; } }
		public string Expression {
			get { return ValueInvariant; }
			set {
				if (!string.IsNullOrEmpty(value) && value[0] != '=') 
					value = '=' + value;
				ValueInvariant = value;
			}
		}
		public ConditionalFormattingExpressionCondition Condition { get { return (ConditionalFormattingExpressionCondition)ModelCondition.Condition; } }
	}
	partial class NativeIconSetConditionalFormatting : NativeConditionalFormatting, IconSetConditionalFormatting {
		public NativeIconSetConditionalFormatting(NativeWorksheet worksheet, Model.IconSetConditionalFormatting modelCondition)
			: base(worksheet, modelCondition) {
		}
		Model.IconSetConditionalFormatting ModelCondition { get { return (Model.IconSetConditionalFormatting)ModelConditionBase; } }
		public bool ShowValue { get { return ModelCondition.ShowValue; } set { ModelCondition.ShowValue = value; } }
		public IconSetType IconSet { get { return (IconSetType)ModelCondition.IconSet; } set { ModelCondition.IconSet = (Model.IconSetType)value; } }
		public bool Reversed { get { return ModelCondition.Reversed; } set { ModelCondition.Reversed = value; } }
		public ConditionalFormattingIconSetValue[] Values {
			get {
				NativeConditionalFormattingIconSetValue[] values = new NativeConditionalFormattingIconSetValue[ModelCondition.ExpectedPointsNumber];
				for (int i = 0; i < ModelCondition.ExpectedPointsNumber; i++) {
					Model.ConditionalFormattingValueObject pointValue = ModelCondition.GetPointValue(i);
					ConditionalFormattingValueOperator comparisonOperator = pointValue.IsGreaterOrEqual ? ConditionalFormattingValueOperator.GreaterOrEqual : ConditionalFormattingValueOperator.Greater;
					values[i] = new NativeConditionalFormattingIconSetValue(pointValue.ValueType, pointValue.Value, comparisonOperator);
				}
				return values;
			}
		}
		public bool IsCustom { get { return ModelCondition.IsCustom; } set { ModelCondition.IsCustom = value; } }
		public void SetCustomIcon(int index, ConditionalFormattingCustomIcon value) {
			ModelCondition.SetCustomIcon(index, new Model.ConditionalFormattingCustomIcon((Model.IconSetType)value.IconSet, value.IconIndex));
		}
		public ConditionalFormattingCustomIcon GetIcon(int index) {
			Model.ConditionalFormattingCustomIcon value = ModelCondition.GetIcon(index);
			return new ConditionalFormattingCustomIcon() { IconSet = (IconSetType)value.IconSet, IconIndex = value.IconIndex };
		}
	}
	partial class NativeDataBarConditionalFormatting : NativeConditionalFormatting, DataBarConditionalFormatting {
		public NativeDataBarConditionalFormatting(NativeWorksheet worksheet, Model.DataBarConditionalFormatting modelCondition)
			: base(worksheet, modelCondition) {
		}
		Model.DataBarConditionalFormatting ModelCondition { get { return (Model.DataBarConditionalFormatting)ModelConditionBase; } }
		public ConditionalFormattingValue LowBound { get { return new NativeConditionalFormattingValue(ModelCondition.LowBound.ValueType, ModelCondition.LowBound.ToString()); } }
		public ConditionalFormattingValue HighBound { get { return new NativeConditionalFormattingValue(ModelCondition.HighBound.ValueType, ModelCondition.HighBound.ToString()); } }
		public ConditionalFormattingDataBarAxisPosition AxisPosition {
			get { return (ConditionalFormattingDataBarAxisPosition)ModelCondition.AxisPosition; }
			set { ModelCondition.AxisPosition = (Model.ConditionalFormattingDataBarAxisPosition)value; }
		}
		public ConditionalFormattingDataBarDirection Direction {
			get { return (ConditionalFormattingDataBarDirection)ModelCondition.Direction; }
			set { ModelCondition.Direction = (Model.ConditionalFormattingDataBarDirection)value; }
		}
		public bool ShowValue { get { return ModelCondition.ShowValue; } set { ModelCondition.ShowValue = value; } }
		public bool GradientFill { get { return ModelCondition.GradientFill; } set { ModelCondition.GradientFill = value; } }
		public Color Color { get { return ModelCondition.Color; } set { ModelCondition.Color = value; } }
		public Color NegativeBarColor { get { return ModelCondition.NegativeValueColor; } set { ModelCondition.NegativeValueColor = value; } }
		public Color BorderColor { get { return ModelCondition.BorderColor; } set { ModelCondition.BorderColor = value; } }
		public Color NegativeBarBorderColor { get { return ModelCondition.NegativeValueBorderColor; } set { ModelCondition.NegativeValueBorderColor = value; } }
		public Color AxisColor { get { return ModelCondition.AxisColor; } set { ModelCondition.AxisColor = value; } }
		public int MinLength { get { return ModelCondition.MinLength; } set { ModelCondition.MinLength = value; } }
		public int MaxLength { get { return ModelCondition.MaxLength; } set { ModelCondition.MaxLength = value; } }
	}
	partial class NativeColorScale2ConditionalFormatting : NativeConditionalFormatting, ColorScale2ConditionalFormatting {
		public NativeColorScale2ConditionalFormatting(NativeWorksheet worksheet, Model.ColorScaleConditionalFormatting modelCondition)
			: base(worksheet, modelCondition) {
		}
		protected Model.ColorScaleConditionalFormatting ModelCondition { get { return (Model.ColorScaleConditionalFormatting)ModelConditionBase; } }
		public ConditionalFormattingValue MinPoint { get { return new NativeConditionalFormattingValue(ModelCondition.LowPointValue.ValueType, ModelCondition.LowPointValue.Value); } }
		public Color MinPointColor { get { return ModelCondition.LowPointColor; } }
		public ConditionalFormattingValue MaxPoint { get { return new NativeConditionalFormattingValue(ModelCondition.HighPointValue.ValueType, ModelCondition.HighPointValue.Value); } }
		public Color MaxPointColor { get { return ModelCondition.HighPointColor; } }
	}
	partial class NativeColorScale3ConditionalFormatting : NativeColorScale2ConditionalFormatting, ColorScale3ConditionalFormatting {
		public NativeColorScale3ConditionalFormatting(NativeWorksheet worksheet, Model.ColorScaleConditionalFormatting modelCondition)
			: base(worksheet, modelCondition) {
		}
		public ConditionalFormattingValue MidPoint {
			get { return new NativeConditionalFormattingValue(ModelCondition.MiddlePointValue.ValueType, ModelCondition.MiddlePointValue.Value); }
		}
		public Color MidPointColor { get { return ModelCondition.MiddlePointColor; } }
	}
	partial class NativeConditionalFormattingIconSetValue : NativeConditionalFormattingValue, ConditionalFormattingIconSetValue {
		bool greaterOrEqual;
		public NativeConditionalFormattingIconSetValue(ConditionalFormattingValueType valueType, string value, ConditionalFormattingValueOperator comparisonOperator)
			: base(valueType, value) {
			this.greaterOrEqual = comparisonOperator != ConditionalFormattingValueOperator.Greater;
		}
		public NativeConditionalFormattingIconSetValue(Model.ConditionalFormattingValueObjectType modelValueType, string value, ConditionalFormattingValueOperator comparisonOperator)
			: base(modelValueType, value) {
			this.greaterOrEqual = comparisonOperator != ConditionalFormattingValueOperator.Greater;
		}
		public ConditionalFormattingValueOperator ComparisonOperator {
			get { return greaterOrEqual ? ConditionalFormattingValueOperator.GreaterOrEqual : ConditionalFormattingValueOperator.Greater; }
		}
	}
	partial class NativeConditionalFormattingValue : ConditionalFormattingValue {
		ConditionalFormattingValueType valueType;
		string value;
		static Dictionary<Model.ConditionalFormattingValueObjectType, ConditionalFormattingValueType> valueTypeTable = CreateValueTypeTable();
		static Dictionary<Model.ConditionalFormattingValueObjectType, ConditionalFormattingValueType> CreateValueTypeTable() {
			Dictionary<Model.ConditionalFormattingValueObjectType, ConditionalFormattingValueType> table = new Dictionary<Model.ConditionalFormattingValueObjectType, ConditionalFormattingValueType>();
			table.Add(Model.ConditionalFormattingValueObjectType.Formula, ConditionalFormattingValueType.Formula);
			table.Add(Model.ConditionalFormattingValueObjectType.Num, ConditionalFormattingValueType.Number);
			table.Add(Model.ConditionalFormattingValueObjectType.Percent, ConditionalFormattingValueType.Percent);
			table.Add(Model.ConditionalFormattingValueObjectType.Percentile, ConditionalFormattingValueType.Percentile);
			table.Add(Model.ConditionalFormattingValueObjectType.Unknown, ConditionalFormattingValueType.Unknown);
			table.Add(Model.ConditionalFormattingValueObjectType.Min, ConditionalFormattingValueType.MinMax);
			table.Add(Model.ConditionalFormattingValueObjectType.Max, ConditionalFormattingValueType.MinMax);
			table.Add(Model.ConditionalFormattingValueObjectType.AutoMin, ConditionalFormattingValueType.Auto);
			table.Add(Model.ConditionalFormattingValueObjectType.AutoMax, ConditionalFormattingValueType.Auto);
			return table;
		}
		public NativeConditionalFormattingValue(ConditionalFormattingValueType valueType, string value) {
			this.valueType = valueType;
			this.value = value;
		}
		public NativeConditionalFormattingValue(Model.ConditionalFormattingValueObjectType modelValueType, string value) {
			this.value = value;
			this.valueType = valueTypeTable[modelValueType];
		}
		protected NativeConditionalFormattingValue() {
		}
		public ConditionalFormattingValueType ValueType { get { return valueType; } }
		public string Value {
			get {
				if (ValueType == ConditionalFormattingValueType.MinMax || ValueType == ConditionalFormattingValueType.Auto)
					return string.Empty;
				return value;
			}
		}
	}
}
