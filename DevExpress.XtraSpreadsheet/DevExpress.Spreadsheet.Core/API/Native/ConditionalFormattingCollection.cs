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
using DevExpress.Spreadsheet;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraSpreadsheet.Localization;
using System.Collections.Generic;
using System.Globalization;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
using DevExpress.Office.Model;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.API.Native.Implementation {
	partial class NativeConditionalFormattingCollection : NativeCollectionBase<ConditionalFormatting, NativeConditionalFormatting, Model.ConditionalFormatting>, ConditionalFormattingCollection {
		public NativeConditionalFormattingCollection(NativeWorksheet worksheet)
			: base(worksheet) {
			Initialize();
			SubscribeEvents();
		}
		#region Events
		void SubscribeEvents() {
			Model.ConditionalFormattingCollection modelCollection = ModelWorksheet.ConditionalFormattings;
			modelCollection.OnAdd += OnAdd;
			modelCollection.OnAddRange += OnAddRange;
			modelCollection.OnRemoveAt += OnRemoveAt;
			modelCollection.OnInsert += OnInsert;
			modelCollection.OnClear += OnClear;
			modelCollection.OnRemoveInsertNoApiInvalidation += OnRemoveInsertNoApiInvalidation;
		}
		void OnAdd(object sender, EventArgs args) {
			UndoableCollectionAddEventArgs<Model.ConditionalFormatting> modelArgs = args as UndoableCollectionAddEventArgs<Model.ConditionalFormatting>;
			OnAdd(modelArgs.Item);
		}
		void OnRemoveAt(object sender, UndoableCollectionRemoveAtEventArgs args) {
			OnRemoveAt(args.Index);
		}
		void OnInsert(object sender, EventArgs args) {
			UndoableCollectionInsertEventArgs<Model.ConditionalFormatting> modelArgs = args as UndoableCollectionInsertEventArgs<Model.ConditionalFormatting>;
			OnInsert(modelArgs.Index, modelArgs.Item);
		}
		void OnClear(object sender) {
			OnClear();
		}
		void OnAddRange(object sender, EventArgs args) {
			UndoableCollectionAddRangeEventArgs<Model.ConditionalFormatting> modelArgs = args as UndoableCollectionAddRangeEventArgs<Model.ConditionalFormatting>;
			foreach (Model.ConditionalFormatting item in modelArgs.Collection)
				OnAdd(item);
		}
		void OnRemoveInsertNoApiInvalidation(object sender, EventArgs args) {
			Model.ConditionalFormattingRemoveInsertNoApiInvalidationEventArgs modelArgs = args as Model.ConditionalFormattingRemoveInsertNoApiInvalidationEventArgs;
			NativeConditionalFormatting item = InnerList[modelArgs.OldIndex];
			InnerList.RemoveAt(modelArgs.OldIndex);
			InnerList.Insert(modelArgs.NewIndex, item);
		}
		#endregion
		#region NativeCollectionBase
		public override IEnumerable<Model.ConditionalFormatting> GetModelItemEnumerable() {
			return ModelWorksheet.ConditionalFormattings.InnerList;
		}
		public override int ModelCollectionCount { get { return ModelWorksheet.ConditionalFormattings.Count; } }
		protected override void RemoveModelObjectAt(int index) {
			if (index < 0)
				throw new ArgumentException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorUseDeletedConditionalFormatting));
			ModelWorksheet.RemoveConditionalFormattingAtWithHistoryAndNotification(index);
		}
		protected override void InvalidateItem(NativeConditionalFormatting item) {
			item.Invalidate();
		}
		protected override void ClearModelObjects() {
			ModelWorksheet.ClearConditionalFormattings();
		}
		protected override NativeConditionalFormatting CreateNativeObject(Model.ConditionalFormatting modelObject) {
			switch (modelObject.Type) {
				case Model.ConditionalFormattingType.ColorScale:
					Model.ColorScaleConditionalFormatting conditionalFormatting = modelObject as Model.ColorScaleConditionalFormatting;
					return conditionalFormatting.ScaleType == Model.ColorScaleType.Color2 ?
						new NativeColorScale2ConditionalFormatting(Worksheet, conditionalFormatting) :
						new NativeColorScale3ConditionalFormatting(Worksheet, conditionalFormatting);
				case Model.ConditionalFormattingType.DataBar:
					return new NativeDataBarConditionalFormatting(Worksheet, modelObject as Model.DataBarConditionalFormatting);
				case Model.ConditionalFormattingType.IconSet:
					return new NativeIconSetConditionalFormatting(Worksheet, modelObject as Model.IconSetConditionalFormatting);
				case Model.ConditionalFormattingType.Formula:
					return CreateNativeFormulaConditionalFormatting(modelObject);
				default:
					return null;
			}
		}
		NativeConditionalFormatting CreateNativeFormulaConditionalFormatting(Model.ConditionalFormatting condition) {
			Model.FormulaConditionalFormatting conditionalFormatting = condition as Model.FormulaConditionalFormatting;
			if (conditionalFormatting == null)
				return null;
			if (conditionalFormatting is Model.AverageFormulaConditionalFormatting)
				return new NativeAverageConditionalFormatting(Worksheet, conditionalFormatting);
			if (conditionalFormatting is Model.TextFormulaConditionalFormatting)
				return new NativeTextConditionalFormatting(Worksheet, conditionalFormatting);
			if (conditionalFormatting is Model.RangeFormulaConditionalFormatting)
				return new NativeRangeConditionalFormatting(Worksheet, conditionalFormatting);
			if (conditionalFormatting is Model.SpecialFormulaConditionalFormatting)
				return new NativeSpecialConditionalFormatting(Worksheet, conditionalFormatting);
			if (conditionalFormatting is Model.RankFormulaConditionalFormatting)
				return new NativeRankConditionalFormatting(Worksheet, conditionalFormatting);
			if (conditionalFormatting is Model.TimePeriodFormulaConditionalFormatting)
				return new NativeTimePeriodConditionalFormatting(Worksheet, conditionalFormatting);
			Model.ExpressionFormulaConditionalFormatting expressionComparer = conditionalFormatting as Model.ExpressionFormulaConditionalFormatting;
			if (expressionComparer.Condition == Model.ConditionalFormattingExpressionCondition.ExpressionIsTrue)
				return new NativeFormulaExpressionConditionalFormatting(Worksheet, conditionalFormatting);
			return new NativeExpressionConditionalFormatting(Worksheet, conditionalFormatting);
		}
		#endregion
		Model.CellRangeBase GetModelCellRange(Range range) {
			if (!object.ReferenceEquals(range.Worksheet, Worksheet))
				throw new ArgumentException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorUseRangeFromAnotherWorksheet));
			return ((NativeWorksheet)(range.Worksheet)).GetModelRange(range);
		}
		public void Remove(Range range) {
			if (!object.ReferenceEquals(range.Worksheet, Worksheet))
				throw new ArgumentException(XtraSpreadsheetLocalizer.GetString(XtraSpreadsheetStringId.Msg_ErrorUseRangeFromAnotherWorksheet));
			List<int> deletedRangeIndexes = new List<int>();
			foreach (NativeConditionalFormatting condition in InnerList) {
				int index = condition.ChangeConditionalFormattingRange(range);
				if (index >= 0)
					deletedRangeIndexes.Add(index);
			}
			foreach (int index in deletedRangeIndexes)
				RemoveAt(index);
		}
		#region AddConditionalFormatting
		public ColorScale3ConditionalFormatting AddColorScale3ConditionalFormatting(Range range, ConditionalFormattingValue minPoint, Color minPointColor, ConditionalFormattingValue midPoint,
			Color midPointColor, ConditionalFormattingValue maxPoint, Color maxPointColor) {
			CheckLowValue(minPoint, false);
			CheckMidPoint(midPoint);
			CheckHighValue(maxPoint, false);
			Model.CellRangeBase modelRange = GetModelCellRange(range);
			modelRange = modelRange.GetWithModifiedPositionType(Model.PositionType.Absolute);
			DevExpress.XtraSpreadsheet.Model.ConditionalFormattingValueObject lowPointValue = GetLowValue(minPoint);
			DevExpress.XtraSpreadsheet.Model.ConditionalFormattingValueObject midPointValue = GetMidValue(midPoint);
			DevExpress.XtraSpreadsheet.Model.ConditionalFormattingValueObject highPointValue = GetHighValue(maxPoint);
			Model.ColorScaleConditionalFormatting modelCondition = new Model.ColorScaleConditionalFormatting(ModelWorksheet);
			modelCondition.LowPointValue = lowPointValue;
			modelCondition.LowPointColor = minPointColor;
			modelCondition.MiddlePointValue = midPointValue;
			modelCondition.MiddlePointColor = midPointColor;
			modelCondition.HighPointValue = highPointValue;
			modelCondition.HighPointColor = maxPointColor;
			modelCondition.SetCellRange(modelRange);
			int index = ModelWorksheet.InsertConditionalFormattingWithHistoryAndNotification(modelCondition);
			ColorScale3ConditionalFormatting result = this[index] as ColorScale3ConditionalFormatting;
			return result;
		}
		public ColorScale2ConditionalFormatting AddColorScale2ConditionalFormatting(Range range, ConditionalFormattingValue minPoint, Color minPointColor, ConditionalFormattingValue maxPoint, Color maxPointColor) {
			CheckLowValue(minPoint, false);
			CheckHighValue(maxPoint, false);
			Model.CellRangeBase modelRange = GetModelCellRange(range);
			modelRange = modelRange.GetWithModifiedPositionType(Model.PositionType.Absolute);
			Model.ColorScaleConditionalFormatting modelCondition = new Model.ColorScaleConditionalFormatting(ModelWorksheet);
			modelCondition.LowPointValue = GetLowValue(minPoint);
			modelCondition.LowPointColor = minPointColor;
			modelCondition.HighPointValue = GetHighValue(maxPoint);
			modelCondition.HighPointColor = maxPointColor;
			modelCondition.SetCellRange(modelRange);
			int index = ModelWorksheet.InsertConditionalFormattingWithHistoryAndNotification(modelCondition);
			ColorScale2ConditionalFormatting result = this[index] as ColorScale2ConditionalFormatting;
			return result;
		}
		public DataBarConditionalFormatting AddDataBarConditionalFormatting(Range range, ConditionalFormattingValue lowBound, ConditionalFormattingValue highBound, Color color) {
			CheckLowValue(lowBound, true);
			CheckHighValue(highBound, true);
			Model.CellRangeBase modelRange = GetModelCellRange(range);
			modelRange = modelRange.GetWithModifiedPositionType(Model.PositionType.Absolute);
			Model.ConditionalFormattingValueObject lowValue = GetLowValue(lowBound);
			Model.ConditionalFormattingValueObject highValue = GetHighValue(highBound);
			Model.DataBarConditionalFormatting modelCondition = new Model.DataBarConditionalFormatting(ModelWorksheet);
			modelCondition.LowBound = lowValue;
			modelCondition.HighBound = highValue;
			modelCondition.Color = color;
			modelCondition.SetCellRange(modelRange);
			int index = ModelWorksheet.InsertConditionalFormattingWithHistoryAndNotification(modelCondition);
			DataBarConditionalFormatting result = this[index] as DataBarConditionalFormatting;
			return result;
		}
		public IconSetConditionalFormatting AddIconSetConditionalFormatting(Range range, IconSetType iconSet, ConditionalFormattingIconSetValue[] points) {
			Model.CellRangeBase modelRange = GetModelCellRange(range);
			modelRange = modelRange.GetWithModifiedPositionType(Model.PositionType.Absolute);
			Model.IconSetConditionalFormatting modelCondition = new Model.IconSetConditionalFormatting(ModelWorksheet);
			modelCondition.IconSet = (Model.IconSetType)iconSet;
			for (int i = 0; i < points.Length; i++) {
				ConditionalFormattingIconSetValue point = points[i];
				modelCondition.SetPointValue(i, new Model.ConditionalFormattingValueObject(ModelWorksheet, GetModelObjectType(point.ValueType), point.Value, point.ComparisonOperator != ConditionalFormattingValueOperator.Greater));
			}
			modelCondition.SetCellRange(modelRange);
			int index = ModelWorksheet.InsertConditionalFormattingWithHistoryAndNotification(modelCondition);
			IconSetConditionalFormatting result = this[index] as IconSetConditionalFormatting;
			return result;
		}
		public AverageConditionalFormatting AddAverageConditionalFormatting(Range range, ConditionalFormattingAverageCondition condition) {
			Model.CellRangeBase modelRange = GetModelCellRange(range);
			modelRange = modelRange.GetWithModifiedPositionType(Model.PositionType.Absolute);
			Model.AverageFormulaConditionalFormatting cf =
				new Model.AverageFormulaConditionalFormatting(ModelWorksheet, modelRange, (Model.ConditionalFormattingAverageCondition)condition, 0);
			int index = ModelWorksheet.InsertConditionalFormattingWithHistoryAndNotification(cf);
			AverageConditionalFormatting result = this[index] as AverageConditionalFormatting;
			return result;
		}
		public AverageConditionalFormatting AddAverageConditionalFormatting(Range range, ConditionalFormattingAverageCondition condition, int stdDev) {
			Model.CellRangeBase modelRange = GetModelCellRange(range);
			modelRange = modelRange.GetWithModifiedPositionType(Model.PositionType.Absolute);
			Model.AverageFormulaConditionalFormatting cf =
				new Model.AverageFormulaConditionalFormatting(ModelWorksheet, modelRange, (Model.ConditionalFormattingAverageCondition)condition, stdDev);
			int index = ModelWorksheet.InsertConditionalFormattingWithHistoryAndNotification(cf);
			AverageConditionalFormatting result = this[index] as AverageConditionalFormatting;
			return result;
		}
		public RankConditionalFormatting AddRankConditionalFormatting(Range range, ConditionalFormattingRankCondition condition, int rank) {
			Model.CellRangeBase modelRange = GetModelCellRange(range);
			modelRange = modelRange.GetWithModifiedPositionType(Model.PositionType.Absolute);
			Model.RankFormulaConditionalFormatting cf = new Model.RankFormulaConditionalFormatting(ModelWorksheet, modelRange, (Model.ConditionalFormattingRankCondition)condition, rank, ApiErrorHandler.Instance);
			int index = ModelWorksheet.InsertConditionalFormattingWithHistoryAndNotification(cf);
			RankConditionalFormatting result = this[index] as RankConditionalFormatting;
			return result;
		}
		public RangeConditionalFormatting AddRangeConditionalFormatting(Range range, ConditionalFormattingRangeCondition condition, string lowBound, string highBound) {
			Model.CellRangeBase modelRange = GetModelCellRange(range);
			modelRange = modelRange.GetWithModifiedPositionType(Model.PositionType.Absolute);
			Model.RangeFormulaConditionalFormatting cf;
			Model.WorkbookDataContext context = ModelWorksheet.DataContext;
			context.PushCulture(CultureInfo.InvariantCulture);
			try {
				cf = new Model.RangeFormulaConditionalFormatting(ModelWorksheet, modelRange, (Model.ConditionalFormattingRangeCondition)condition, lowBound, highBound, ApiErrorHandler.Instance);
			}
			finally {
				context.PopCulture();
			}
			int index = ModelWorksheet.InsertConditionalFormattingWithHistoryAndNotification(cf);
			RangeConditionalFormatting result = this[index] as RangeConditionalFormatting;
			return result;
		}
		public TextConditionalFormatting AddTextConditionalFormatting(Range range, ConditionalFormattingTextCondition condition, string text) {
			Model.CellRangeBase modelRange = GetModelCellRange(range);
			modelRange = modelRange.GetWithModifiedPositionType(Model.PositionType.Absolute);
			Model.TextFormulaConditionalFormatting cf;
			Model.WorkbookDataContext context = ModelWorksheet.DataContext;
			context.PushCulture(CultureInfo.InvariantCulture);
			try {
				cf = new Model.TextFormulaConditionalFormatting(ModelWorksheet, modelRange, (Model.ConditionalFormattingTextCondition)condition, text, ApiErrorHandler.Instance);
			}
			finally {
				context.PopCulture();
			}
			int index = ModelWorksheet.InsertConditionalFormattingWithHistoryAndNotification(cf);
			TextConditionalFormatting result = this[index] as TextConditionalFormatting;
			return result;
		}
		public SpecialConditionalFormatting AddSpecialConditionalFormatting(Range range, ConditionalFormattingSpecialCondition condition) {
			Model.CellRangeBase modelRange = GetModelCellRange(range);
			modelRange = modelRange.GetWithModifiedPositionType(Model.PositionType.Absolute);
			Model.SpecialFormulaConditionalFormatting cf = new Model.SpecialFormulaConditionalFormatting(ModelWorksheet, modelRange, (Model.ConditionalFormattingSpecialCondition)condition);
			int index = ModelWorksheet.InsertConditionalFormattingWithHistoryAndNotification(cf);
			SpecialConditionalFormatting result = this[index] as SpecialConditionalFormatting;
			return result;
		}
		public TimePeriodConditionalFormatting AddTimePeriodConditionalFormatting(Range range, ConditionalFormattingTimePeriod condition) {
			Model.CellRangeBase modelRange = GetModelCellRange(range);
			modelRange = modelRange.GetWithModifiedPositionType(Model.PositionType.Absolute);
			Model.TimePeriodFormulaConditionalFormatting cf =
				new Model.TimePeriodFormulaConditionalFormatting(ModelWorksheet, modelRange, (Model.ConditionalFormattingTimePeriod)condition);
			int index = ModelWorksheet.InsertConditionalFormattingWithHistoryAndNotification(cf);
			TimePeriodConditionalFormatting result = this[index] as TimePeriodConditionalFormatting;
			return result;
		}
		public ExpressionConditionalFormatting AddExpressionConditionalFormatting(Range range, ConditionalFormattingExpressionCondition condition, string value) {
			Model.CellRangeBase modelRange = GetModelCellRange(range);
			modelRange = modelRange.GetWithModifiedPositionType(Model.PositionType.Absolute);
			Model.ExpressionFormulaConditionalFormatting cf;
			Model.WorkbookDataContext context = ModelWorksheet.DataContext;
			context.PushCulture(CultureInfo.InvariantCulture);
			try {
				cf = new Model.ExpressionFormulaConditionalFormatting(ModelWorksheet, modelRange, (Model.ConditionalFormattingExpressionCondition)condition, value, ApiErrorHandler.Instance);
			}
			finally {
				context.PopCulture();
			}
			int index = ModelWorksheet.InsertConditionalFormattingWithHistoryAndNotification(cf);
			ExpressionConditionalFormatting result = this[index] as ExpressionConditionalFormatting;
			return result;
		}
		public FormulaExpressionConditionalFormatting AddFormulaExpressionConditionalFormatting(Range range, string expression) {
			if (!string.IsNullOrEmpty(expression) && expression[0] != '=') 
				expression = '=' + expression;
			Model.CellRangeBase modelRange = GetModelCellRange(range);
			modelRange = modelRange.GetWithModifiedPositionType(Model.PositionType.Absolute);
			Model.ExpressionFormulaConditionalFormatting cf;
			Model.WorkbookDataContext context = ModelWorksheet.DataContext;
			context.PushCulture(CultureInfo.InvariantCulture);
			try {
				cf = new Model.ExpressionFormulaConditionalFormatting(ModelWorksheet, modelRange, Model.ConditionalFormattingExpressionCondition.ExpressionIsTrue, expression, ApiErrorHandler.Instance);
			}
			finally {
				context.PopCulture();
			}
			int index = ModelWorksheet.InsertConditionalFormattingWithHistoryAndNotification(cf);
			FormulaExpressionConditionalFormatting result = this[index] as FormulaExpressionConditionalFormatting;
			return result;
		}
		void CheckLowValue(ConditionalFormattingValue lowBound, bool isAutoAllowed) {
			if (lowBound.ValueType == ConditionalFormattingValueType.Auto) {
				if (!isAutoAllowed)
					SpreadsheetExceptions.ThrowArgumentException(XtraSpreadsheetStringId.Msg_ErrorIncorectLowBound, lowBound.ValueType.ToString());
			}
			else if (lowBound.ValueType != ConditionalFormattingValueType.MinMax && string.IsNullOrEmpty(lowBound.Value))
				SpreadsheetExceptions.ThrowArgumentException(XtraSpreadsheetStringId.Msg_ErrorIncorectLowBound);
		}
		void CheckMidPoint(ConditionalFormattingValue midPoint) {
			if (midPoint.ValueType == ConditionalFormattingValueType.Auto || midPoint.ValueType == ConditionalFormattingValueType.MinMax)
				SpreadsheetExceptions.ThrowArgumentException(XtraSpreadsheetStringId.Msg_ErrorIncorectMidPoint, midPoint.ValueType.ToString());
			else {
				string value = midPoint.Value;
				if (String.IsNullOrEmpty(value))
					SpreadsheetExceptions.ThrowArgumentException(XtraSpreadsheetStringId.Msg_ErrorIncorectMidPoint);
			}
		}
		void CheckHighValue(ConditionalFormattingValue highBound, bool isAutoAllowed) {
			if (highBound.ValueType == ConditionalFormattingValueType.Auto) {
				if (!isAutoAllowed)
					SpreadsheetExceptions.ThrowArgumentException(XtraSpreadsheetStringId.Msg_ErrorIncorectHighBound, highBound.ValueType.ToString());
			}
			else if (highBound.ValueType != ConditionalFormattingValueType.MinMax && string.IsNullOrEmpty(highBound.Value))
				SpreadsheetExceptions.ThrowArgumentException(XtraSpreadsheetStringId.Msg_ErrorIncorectHighBound);
		}
		Model.ConditionalFormattingValueObject GetMidValue(ConditionalFormattingValue midPoint) {
			string value = midPoint.Value;
			Model.ConditionalFormattingValueObject result = new Model.ConditionalFormattingValueObject(ModelWorksheet);
			Model.IModelErrorInfo errorInfo = result.CanSetValue(value);
			if (ApiErrorHandler.Instance.HandleError(errorInfo) == Model.ErrorHandlingResult.Abort)
				return null;
			result.ValueType = GetModelObjectType(midPoint.ValueType);
			result.Value = value;
			return result;
		}
		Model.ConditionalFormattingValueObject GetLowValue(ConditionalFormattingValue lowBound) {
			if (lowBound.ValueType == ConditionalFormattingValueType.Auto)
				return new Model.ConditionalFormattingValueObject(ModelWorksheet, Model.ConditionalFormattingValueObjectType.AutoMin, 0);
			if (lowBound.ValueType == ConditionalFormattingValueType.MinMax)
				return new Model.ConditionalFormattingValueObject(ModelWorksheet, Model.ConditionalFormattingValueObjectType.Min, 0);
			string value = lowBound.Value;
			DevExpress.XtraSpreadsheet.Model.ConditionalFormattingValueObject valueObj = new Model.ConditionalFormattingValueObject(ModelWorksheet);
			Model.IModelErrorInfo errorInfo = valueObj.CanSetValue(value);
			if (ApiErrorHandler.Instance.HandleError(errorInfo) == Model.ErrorHandlingResult.Abort)
				return null;
			valueObj.ValueType = GetModelObjectType(lowBound.ValueType);
			valueObj.SetValue(value);
			return valueObj;
		}
		Model.ConditionalFormattingValueObject GetHighValue(ConditionalFormattingValue highBound) {
			if (highBound.ValueType == ConditionalFormattingValueType.Auto)
				return new Model.ConditionalFormattingValueObject(ModelWorksheet, Model.ConditionalFormattingValueObjectType.AutoMax, 0);
			if (highBound.ValueType == ConditionalFormattingValueType.MinMax)
				return new Model.ConditionalFormattingValueObject(ModelWorksheet, Model.ConditionalFormattingValueObjectType.Max, 0);
			string value = highBound.Value;
			DevExpress.XtraSpreadsheet.Model.ConditionalFormattingValueObject valueObj = new Model.ConditionalFormattingValueObject(ModelWorksheet);
			Model.IModelErrorInfo errorInfo = valueObj.CanSetValue(value);
			if (ApiErrorHandler.Instance.HandleError(errorInfo) == Model.ErrorHandlingResult.Abort)
				return null;
			valueObj.ValueType = GetModelObjectType(highBound.ValueType);
			valueObj.SetValue(value);
			return valueObj;
		}
		Model.ConditionalFormattingValueObjectType GetModelObjectType(ConditionalFormattingValueType valueType) {
			switch (valueType) {
				case ConditionalFormattingValueType.Formula: return Model.ConditionalFormattingValueObjectType.Formula;
				case ConditionalFormattingValueType.Number: return Model.ConditionalFormattingValueObjectType.Num;
				case ConditionalFormattingValueType.Percent: return Model.ConditionalFormattingValueObjectType.Percent;
				case ConditionalFormattingValueType.Percentile: return Model.ConditionalFormattingValueObjectType.Percentile;
			}
			return Model.ConditionalFormattingValueObjectType.Unknown;
		}
		#endregion
		public ConditionalFormattingIconSetValue CreateIconSetValue(ConditionalFormattingValueType valueType, string value, ConditionalFormattingValueOperator comparisonOperator) {
			if (valueType == ConditionalFormattingValueType.Auto || valueType == ConditionalFormattingValueType.MinMax)
				SpreadsheetExceptions.ThrowArgumentException(XtraSpreadsheetStringId.Msg_CondFmtIncorectValueType, valueType.ToString());
			return new NativeConditionalFormattingIconSetValue(valueType, value, comparisonOperator);
		}
		public ConditionalFormattingValue CreateValue(ConditionalFormattingValueType valueType, string value) {
			return new NativeConditionalFormattingValue(valueType, value);
		}
		public ConditionalFormattingValue CreateValue(ConditionalFormattingValueType valueType) {
			return new NativeConditionalFormattingValue(valueType, string.Empty);
		}
	}
}
