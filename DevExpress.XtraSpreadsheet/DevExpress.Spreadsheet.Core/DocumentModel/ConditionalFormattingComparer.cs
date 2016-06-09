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

using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using System;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Model {
	public abstract class ValueBasedFormulaConditionalFormatting : FormulaConditionalFormatting {
		ParsedExpression valueExpression;
		protected ValueBasedFormulaConditionalFormatting(Worksheet sheet, CellRangeBase range, ParsedExpression value)
			: base(sheet, range) {
			this.valueExpression = value.Clone();
		}
		protected ValueBasedFormulaConditionalFormatting(Worksheet sheet, CellRangeBase range, string value)
			: base(sheet, range) {
			this.valueExpression = ParseExpression(value);
		}
		protected ValueBasedFormulaConditionalFormatting(Worksheet sheet, CellRangeBase range, string value, IErrorHandler errorHandler)
			: base(sheet, range) {
			this.valueExpression = ParseExpressionWithChecks(value, errorHandler);
		}
		#region Value
		public string Value { get { return GetStringValue(ValueExpression); } set { ValueExpression = ParseExpression(value); } }
		protected internal ParsedExpression ValueExpression {
			get { return valueExpression; }
			set {
				if (valueExpression == value)
					return;
				DocumentHistory history = Sheet.Workbook.History;
				ConditionalFormattingValueBasedValueHistoryItem item = new ConditionalFormattingValueBasedValueHistoryItem(this, valueExpression, value);
				history.Add(item);
				item.Execute();
			}
		}
		protected internal void SetValueCore(ParsedExpression value) {
			this.valueExpression = value;
			RefreshCachedExpression();
		}
		protected internal void SetValue(string value, IErrorHandler errorHandler) {
			ValueExpression = ParseExpressionWithChecks(value, errorHandler);
		}
		#endregion
		#region Notification
		protected override void ProcessNotification(InsertRemoveRangeNotificationContextBase notificationContext, ShiftCellsNotificationMode mode) {
			ConditionalFormattingRelativeReferenceChecker checker = new ConditionalFormattingRelativeReferenceChecker();
			CellRange processedRange = notificationContext.Range;
			if (ShouldUseStrongLogic(checker)) {
				List<ValueBasedFormulaConditionalFormatting> list = new List<ValueBasedFormulaConditionalFormatting>();
				list.Add(this);
				ProcessNotificationForRange(processedRange, mode, list); 
				UseStrongLogic(list, processedRange, mode);
				list.RemoveAt(0);
				foreach (ValueBasedFormulaConditionalFormatting cf in list)
					Sheet.InsertConditionalFormattingWithHistoryAndNotification(cf);
			}
			else {
				RemovedShiftLeftDefinedNameRPNVisitor visitor = null;
				switch (mode) {
					case ShiftCellsNotificationMode.ShiftDown:
						visitor = new InsertedShiftDownDefinedNameRPNVisitor(notificationContext, Sheet.DataContext);
						break;
					case ShiftCellsNotificationMode.ShiftLeft:
						visitor = new RemovedShiftLeftDefinedNameRPNVisitor(notificationContext, Sheet.DataContext);
						break;
					case ShiftCellsNotificationMode.ShiftRight:
						visitor = new InsertedShiftRightDefinedNameRPNVisitor(notificationContext, Sheet.DataContext);
						break;
					case ShiftCellsNotificationMode.ShiftUp:
						visitor = new RemovedShiftUpDefinedNameRPNVisitor(notificationContext, Sheet.DataContext);
						break;
				}
				ProcessAllValues(visitor);
				base.ProcessNotification(notificationContext, mode);
			}
		}
		void ProcessNotificationForRange(CellRange removableRange, ShiftCellsNotificationMode mode, List<ValueBasedFormulaConditionalFormatting> list) {
			if (!object.ReferenceEquals(removableRange.Worksheet, Sheet))
				return;
			ConditionalFormattingRPNVisitor walker = null;
			switch (mode) {
				case ShiftCellsNotificationMode.ShiftRight:
					walker = new ShiftRightConditionalFormattingRPNVisitor(Sheet.DataContext);
					break;
				case ShiftCellsNotificationMode.ShiftDown:
					walker = new ShiftDownConditionalFormattingRPNVisitor(Sheet.DataContext);
					break;
				case ShiftCellsNotificationMode.ShiftLeft:
					walker = new ShiftLeftConditionalFormattingRPNVisitor(Sheet.DataContext);
					break;
				case ShiftCellsNotificationMode.ShiftUp:
					walker = new ShiftUpConditionalFormattingRPNVisitor(Sheet.DataContext);
					break;
			}
			RangeNotificationInfo info = ConditionalFormattingNotificator.GetRangeNotificationInfo(removableRange, CellRange, Sheet, mode, false);
			walker.DiffBetweenNewAndOldRanges = info.Offset;
			if (info.NonShiftedRange == null) {
				if (info.ShiftedRange == null) {
					SetCellRangeOrNull(null);
					return;
				}
				SetCellRange(info.ShiftedRange);
				ProcessAllValues(walker);
			}
			else {
				if (info.ShiftedRange != null) {
					ValueBasedFormulaConditionalFormatting newCF = FormulaConditionalFormatting.CreateCopy(this, Sheet) as ValueBasedFormulaConditionalFormatting;
					newCF.SetCellRange(info.ShiftedRange);
					list.Add(newCF);
					newCF.ProcessAllValues(walker);
				}
				SetCellRange(info.NonShiftedRange);
			}
		}
		protected virtual bool ShouldUseStrongLogic(ConditionalFormattingRelativeReferenceChecker checker) {
			return checker.HasRelativeReferences(ValueExpression);
		}
		protected virtual void UseStrongLogic(List<ValueBasedFormulaConditionalFormatting> list, CellRange removableRange, ShiftCellsNotificationMode mode) {
			new Value1ConditionalFormattingStrongLogic(list, removableRange, mode, Sheet.DataContext).Process();
		}
		protected internal virtual void ProcessAllValues(ParsedThingVisitor walker) {
			ValueExpression = walker.Process(ValueExpression.Clone());
		}
		#endregion
		protected override void CopyFormulaData(FormulaConditionalFormatting source) {
			ValueBasedFormulaConditionalFormatting cf = source as ValueBasedFormulaConditionalFormatting;
			if (cf == null)
				Exceptions.ThrowInvalidOperationException("Can't copy from another conditional formatting type.");
			ValueExpression = cf.ValueExpression.Clone();
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			ValueBasedFormulaConditionalFormatting cf = obj as ValueBasedFormulaConditionalFormatting;
			if (cf == null)
				return false;
			return valueExpression.Equals(cf.valueExpression);
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32(base.GetHashCode(), valueExpression.GetHashCode());
		}
		#region ParseExpression
		protected ParsedExpression ParseExpressionWithChecks(string value, IErrorHandler errorHandler) {
			ParsedExpression expression = ParseExpression(value);
			ConditionalFormattingFormulaInspector inspector = new ConditionalFormattingFormulaInspector();
			IModelErrorInfo errorInfo = inspector.Process(expression, true);
			errorHandler.HandleError(errorInfo);
			return expression;
		}
		protected ParsedExpression ParseExpression(string value) {
			if (string.IsNullOrEmpty(value))
				return null;
			ParsedExpression result = null;
			WorkbookDataContext context = Sheet.DataContext;
			if (value[0] != '=') {
				result = new ParsedExpression();
				VariantValue variantValue = CellValueFormatter.GetValueCore(value, context, false).Value; 
				result.Add(new ParsedThingVariantValue(variantValue));
				return result;
			}
			context.PushCurrentCell(GetCellRangeAnchor());
			context.PushSharedFormulaProcessing(true);
			try {
				return result = context.ParseExpression(value, OperandDataType.Value, false);
			}
			finally {
				context.PopSharedFormulaProcessing();
				context.PopCurrentCell();
			}
		}
		#endregion
	}
	public abstract class RangeValueBasedFormulaConditionalFormatting : ValueBasedFormulaConditionalFormatting {
		ParsedExpression value2Expression;
		protected RangeValueBasedFormulaConditionalFormatting(Worksheet sheet, CellRangeBase range, ParsedExpression value, ParsedExpression value2)
			: base(sheet, range, value) {
			this.value2Expression = value2.Clone();
		}
		protected RangeValueBasedFormulaConditionalFormatting(Worksheet sheet, CellRangeBase range, string value, string value2)
			: base(sheet, range, value) {
			this.value2Expression = ParseExpression(value2);
		}
		protected RangeValueBasedFormulaConditionalFormatting(Worksheet sheet, CellRangeBase range, string value, string value2, IErrorHandler errorHandler)
			: base(sheet, range, value, errorHandler) {
			this.value2Expression = ParseExpressionWithChecks(value2, errorHandler);
		}
		#region Value2
		public string Value2 { get { return GetStringValue(Value2Expression); } set { Value2Expression = ParseExpression(value); } }
		protected internal ParsedExpression Value2Expression {
			get { return value2Expression; }
			set {
				if (value2Expression == value)
					return;
				DocumentHistory history = Sheet.Workbook.History;
				ConditionalFormattingValueBasedValue2HistoryItem item = new ConditionalFormattingValueBasedValue2HistoryItem(this, value2Expression, value);
				history.Add(item);
				item.Execute();
			}
		}
		protected internal void SetValue2Core(ParsedExpression value) {
			this.value2Expression = value;
			RefreshCachedExpression();
		}
		protected internal void SetValue2(string value, IErrorHandler errorHandler) {
			Value2Expression = ParseExpressionWithChecks(value, errorHandler);
		}
		#endregion
		protected override bool ShouldUseStrongLogic(ConditionalFormattingRelativeReferenceChecker checker) {
			if (checker.HasRelativeReferences(ValueExpression))
				return true;
			return checker.HasRelativeReferences(Value2Expression);
		}
		protected override void UseStrongLogic(List<ValueBasedFormulaConditionalFormatting> list, CellRange removableRange, ShiftCellsNotificationMode mode) {
			new Value1ConditionalFormattingStrongLogic(list, removableRange, mode, Sheet.DataContext).Process();
			new Value2ConditionalFormattingStrongLogic(list, removableRange, mode, Sheet.DataContext).Process();
		}
		protected internal override void ProcessAllValues(ParsedThingVisitor walker) {
			ValueExpression = walker.Process(ValueExpression.Clone());
			Value2Expression = walker.Process(Value2Expression.Clone());
		}
		protected override void CopyFormulaData(FormulaConditionalFormatting source) {
			base.CopyFormulaData(source);
			RangeValueBasedFormulaConditionalFormatting cf = source as RangeValueBasedFormulaConditionalFormatting;
			if (cf == null)
				Exceptions.ThrowInvalidOperationException("Can't copy from another conditional formatting type.");
			Value2Expression = cf.Value2Expression.Clone();
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			RangeValueBasedFormulaConditionalFormatting cf = obj as RangeValueBasedFormulaConditionalFormatting;
			if (cf == null)
				return false;
			return value2Expression.Equals(cf.value2Expression);
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32(base.GetHashCode(), value2Expression.GetHashCode());
		}
	}
	#region AverageFormulaConditionalFormatting
	#region ConditionalFormattingAverageCondition
	public enum ConditionalFormattingAverageCondition {
		Above,
		AboveOrEqual,
		Below,
		BelowOrEqual
	}
	#endregion
	public class AverageFormulaConditionalFormatting : FormulaConditionalFormatting {
		ConditionalFormattingAverageCondition condition;
		int stdDev;
		public AverageFormulaConditionalFormatting(Worksheet sheet, CellRangeBase range, ConditionalFormattingAverageCondition condition, int stdDev)
			: base(sheet, range) {
			this.condition = condition;
			this.stdDev = stdDev;
			RefreshCachedExpression();
		}
		#region OpenXmlTypeId
		public override ConditionalFormattingRuleType RuleType { get { return ConditionalFormattingRuleType.AboveOrBelowAverage; } }
		#endregion
		#region Condition
		public ConditionalFormattingAverageCondition Condition {
			get { return condition; }
			set {
				if (condition == value)
					return;
				DocumentHistory history = Sheet.Workbook.History;
				ConditionalFormattingAverageConditionHistoryItem item = new ConditionalFormattingAverageConditionHistoryItem(this, condition, value);
				history.Add(item);
				item.Execute();
			}
		}
		protected internal void SetConditionCore(ConditionalFormattingAverageCondition value) {
			condition = value;
			RefreshCachedExpression();
		}
		#endregion
		#region StdDev
		public int StdDev {
			get { return stdDev; }
			set {
				if (StdDev == value)
					return;
				DocumentHistory history = Sheet.Workbook.History;
				ConditionalFormattingAverageStdDevHistoryItem item = new ConditionalFormattingAverageStdDevHistoryItem(this, stdDev, value);
				history.Add(item);
				item.Execute();
			}
		}
		protected internal void SetStdDevCore(int value) {
			this.stdDev = value;
			RefreshCachedExpression();
		}
		#endregion
		#region RefreshCachedExpression
		protected override void RefreshCachedExpression() {
			if (StdDev > 0)
				PrepareForStdDev();
			else
				PrepareForAverage();
		}
		protected void AppendAverageExpression(ParsedExpression target) {
			target.Add(new ParsedThingVariantValue(new VariantValue() { CellRangeValue = CellRange }));
			target.Add(new ParsedThingFuncVar(funcAverage, 1));
		}
		protected void AppendAverageStDevExpression(ParsedExpression target, bool above, int stdDev) {
			AppendAverageExpression(target);
			target.Add(new ParsedThingVariantValue(new VariantValue() { CellRangeValue = CellRange }));
			target.Add(new ParsedThingFuncVar(funcStDev, 1));
			if (stdDev > 1) {
				target.Add(new ParsedThingInteger() { Value = stdDev });
				target.Add(ParsedThingMultiply.Instance);
			}
			if (above)
				target.Add(ParsedThingAdd.Instance);
			else
				target.Add(ParsedThingSubtract.Instance);
		}
		protected void PrepareForAverage() {
			ParsedExpression newExpression = new ParsedExpression();
			newExpression.Add(CellValue);
			newExpression.Add(new ParsedThingFuncVar(funcIsNumber, 1));
			newExpression.Add(CellValue);
			AppendAverageExpression(newExpression);
			switch (Condition) {
				case ConditionalFormattingAverageCondition.Above:
					newExpression.Add(ParsedThingGreater.Instance);
					break;
				case ConditionalFormattingAverageCondition.AboveOrEqual:
					newExpression.Add(ParsedThingGreaterEqual.Instance);
					break;
				case ConditionalFormattingAverageCondition.Below:
					newExpression.Add(ParsedThingLess.Instance);
					break;
				case ConditionalFormattingAverageCondition.BelowOrEqual:
					newExpression.Add(ParsedThingLessEqual.Instance);
					break;
				default:
					Exceptions.ThrowArgumentException("Condition", Condition);
					break;
			}
			newExpression.Add(new ParsedThingFuncVar(funcAnd, 2));
			CachedExpression = newExpression;
		}
		protected void PrepareForStdDev() {
			CachedExpression = new ParsedExpression();
			CachedExpression.Add(CellValue);
			CachedExpression.Add(new ParsedThingFuncVar(funcIsNumber, 1));
			CachedExpression.Add(CellValue);
			switch (Condition) {
				case ConditionalFormattingAverageCondition.Above:
					AppendAverageStDevExpression(CachedExpression, true, StdDev);
					CachedExpression.Add(ParsedThingGreater.Instance);
					break;
				case ConditionalFormattingAverageCondition.AboveOrEqual:
					AppendAverageStDevExpression(CachedExpression, true, StdDev);
					CachedExpression.Add(ParsedThingGreaterEqual.Instance);
					break;
				case ConditionalFormattingAverageCondition.Below:
					AppendAverageStDevExpression(CachedExpression, false, StdDev);
					CachedExpression.Add(ParsedThingLess.Instance);
					break;
				case ConditionalFormattingAverageCondition.BelowOrEqual:
					AppendAverageStDevExpression(CachedExpression, false, StdDev);
					CachedExpression.Add(ParsedThingLessEqual.Instance);
					break;
				default:
					Exceptions.ThrowArgumentException("Condition", Condition);
					break;
			}
			CachedExpression.Add(new ParsedThingFuncVar(funcAnd, 2));
		}
		#endregion
		protected override ConditionalFormatting CreateInstanceCore(Worksheet targetWorksheet) {
			return new AverageFormulaConditionalFormatting(targetWorksheet, CellRange.Clone(), Condition, StdDev);
		}
		protected override void CopyFormulaData(FormulaConditionalFormatting source) {
			AverageFormulaConditionalFormatting cf = source as AverageFormulaConditionalFormatting;
			if (cf == null)
				Exceptions.ThrowInvalidOperationException("Can't copy from another conditional formatting type.");
			Condition = cf.Condition;
			StdDev = cf.StdDev;
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			AverageFormulaConditionalFormatting cf = obj as AverageFormulaConditionalFormatting;
			if (cf == null)
				return false;
			if (condition != cf.condition)
				return false;
			return stdDev == cf.stdDev;
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32(base.GetHashCode(), (int)condition, stdDev);
		}
	}
	#endregion
	#region ExpressionFormulaConditionalFormatting
	#region ConditionalFormattingExpressionCondition
	public enum ConditionalFormattingExpressionCondition {
		EqualTo,
		InequalTo,
		LessThan,
		LessThanOrEqual,
		GreaterThan,
		GreaterThanOrEqual,
		ExpressionIsTrue
	}
	#endregion
	public class ExpressionFormulaConditionalFormatting : ValueBasedFormulaConditionalFormatting {
		ConditionalFormattingExpressionCondition condition;
		ExpressionFormulaConditionalFormatting(Worksheet sheet, CellRangeBase range, ConditionalFormattingExpressionCondition condition, ParsedExpression value)
			: base(sheet, range, value) {
			this.condition = condition;
			RefreshCachedExpression();
		}
		public ExpressionFormulaConditionalFormatting(Worksheet sheet, CellRangeBase range, ConditionalFormattingExpressionCondition condition, string value)
			: base(sheet, range, value) {
			this.condition = condition;
			RefreshCachedExpression();
		}
		public ExpressionFormulaConditionalFormatting(Worksheet sheet, CellRangeBase range, ConditionalFormattingExpressionCondition condition, string value, IErrorHandler errorHandler)
			: base(sheet, range, value, errorHandler) {
			this.condition = condition;
			RefreshCachedExpression();
		}
		#region OpenXmlTypeId
		public override ConditionalFormattingRuleType RuleType { get { return GetXmlRuleType(); } }
		ConditionalFormattingRuleType GetXmlRuleType() {
			return Condition == ConditionalFormattingExpressionCondition.ExpressionIsTrue ?
							ConditionalFormattingRuleType.ExpressionIsTrue :
							ConditionalFormattingRuleType.CompareWithFormulaResult;
		}
		#endregion
		#region OpenXmlOperator
		public override ConditionalFormattingOperator Operator { get { return GetXmlOperator(); } }
		ConditionalFormattingOperator GetXmlOperator() {
			ConditionalFormattingOperator result;
			switch (Condition) {
				case ConditionalFormattingExpressionCondition.EqualTo:
					result = ConditionalFormattingOperator.Equal;
					break;
				case ConditionalFormattingExpressionCondition.GreaterThan:
					result = ConditionalFormattingOperator.GreaterThan;
					break;
				case ConditionalFormattingExpressionCondition.GreaterThanOrEqual:
					result = ConditionalFormattingOperator.GreaterThanOrEqual;
					break;
				case ConditionalFormattingExpressionCondition.InequalTo:
					result = ConditionalFormattingOperator.NotEqual;
					break;
				case ConditionalFormattingExpressionCondition.LessThan:
					result = ConditionalFormattingOperator.LessThan;
					break;
				case ConditionalFormattingExpressionCondition.LessThanOrEqual:
					result = ConditionalFormattingOperator.LessThanOrEqual;
					break;
				default:
					result = ConditionalFormattingOperator.Unknown;
					break;
			}
			return result;
		}
		#endregion
		#region Condition
		public ConditionalFormattingExpressionCondition Condition {
			get { return condition; }
			set {
				if (condition == value)
					return;
				DocumentHistory history = Sheet.Workbook.History;
				ConditionalFormattingExpressionConditionHistoryItem item = new ConditionalFormattingExpressionConditionHistoryItem(this, condition, value);
				history.Add(item);
				item.Execute();
			}
		}
		protected internal void SetConditionCore(ConditionalFormattingExpressionCondition value) {
			condition = value;
			RefreshCachedExpression();
		}
		#endregion
		#region RefreshCachedExpression
		protected override void RefreshCachedExpression() {
			if (ValueExpression == null)
				return;
			CachedExpression = new ParsedExpression();
			BinaryBooleanParsedThing conditionOperator;
			switch (Condition) {
				case ConditionalFormattingExpressionCondition.EqualTo:
					conditionOperator = ParsedThingEqual.Instance;
					break;
				case ConditionalFormattingExpressionCondition.InequalTo:
					conditionOperator = ParsedThingNotEqual.Instance;
					break;
				case ConditionalFormattingExpressionCondition.GreaterThan:
					conditionOperator = ParsedThingGreater.Instance;
					break;
				case ConditionalFormattingExpressionCondition.GreaterThanOrEqual:
					conditionOperator = ParsedThingGreaterEqual.Instance;
					break;
				case ConditionalFormattingExpressionCondition.LessThan:
					conditionOperator = ParsedThingLess.Instance;
					break;
				case ConditionalFormattingExpressionCondition.LessThanOrEqual:
					conditionOperator = ParsedThingLessEqual.Instance;
					break;
				case ConditionalFormattingExpressionCondition.ExpressionIsTrue:
					CachedExpression.AddRange(ValueExpression);
					return;
				default:
					Exceptions.ThrowArgumentException("Condition", Condition);
					return;
			}
			CachedExpression.Add(CellValue);
			CachedExpression.AddRange(ValueExpression);
			CachedExpression.Add(conditionOperator);
		}
		#endregion
		protected override ConditionalFormatting CreateInstanceCore(Worksheet targetWorksheet) {
			return new ExpressionFormulaConditionalFormatting(targetWorksheet, CellRange.Clone(), Condition, ValueExpression.Clone());
		}
		protected override void CopyFormulaData(FormulaConditionalFormatting source) {
			base.CopyFormulaData(source);
			ExpressionFormulaConditionalFormatting cf = source as ExpressionFormulaConditionalFormatting;
			if (cf == null)
				Exceptions.ThrowInvalidOperationException("Can't copy from another conditional formatting type.");
			Condition = cf.Condition;
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			ExpressionFormulaConditionalFormatting cf = obj as ExpressionFormulaConditionalFormatting;
			if (cf == null)
				return false;
			return condition == cf.condition;
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32(base.GetHashCode(), (int)condition);
		}
	}
	#endregion
	#region RangeFormulaConditionalFormatting
	#region ConditionalFormattingRangeCondition
	public enum ConditionalFormattingRangeCondition {
		Inside,
		Outside
	}
	#endregion
	public class RangeFormulaConditionalFormatting : RangeValueBasedFormulaConditionalFormatting {
		ConditionalFormattingRangeCondition condition;
		RangeFormulaConditionalFormatting(Worksheet sheet, CellRangeBase range, ConditionalFormattingRangeCondition condition, ParsedExpression value, ParsedExpression value2)
			: base(sheet, range, value, value2) {
			this.condition = condition;
			RefreshCachedExpression();
		}
		public RangeFormulaConditionalFormatting(Worksheet sheet, CellRangeBase range, ConditionalFormattingRangeCondition condition, string value, string value2)
			: base(sheet, range, value, value2) {
			this.condition = condition;
			RefreshCachedExpression();
		}
		public RangeFormulaConditionalFormatting(Worksheet sheet, CellRangeBase range, ConditionalFormattingRangeCondition condition, string value, string value2, IErrorHandler errorHandler)
			: base(sheet, range, value, value2, errorHandler) {
			this.condition = condition;
			RefreshCachedExpression();
		}
		#region OpenXmlTypeId
		public override ConditionalFormattingRuleType RuleType { get { return ConditionalFormattingRuleType.CompareWithFormulaResult; } }
		#endregion
		#region OpenXmlOperator
		public override ConditionalFormattingOperator Operator {
			get { return Condition == ConditionalFormattingRangeCondition.Inside ? ConditionalFormattingOperator.Between : ConditionalFormattingOperator.NotBetween; }
		}
		#endregion
		#region Condition
		public ConditionalFormattingRangeCondition Condition {
			get { return condition; }
			set {
				if (condition == value)
					return;
				DocumentHistory history = Sheet.Workbook.History;
				ConditionalFormattingRangeConditionHistoryItem item = new ConditionalFormattingRangeConditionHistoryItem(this, condition, value);
				history.Add(item);
				item.Execute();
			}
		}
		protected internal void SetConditionCore(ConditionalFormattingRangeCondition value) {
			condition = value;
			RefreshCachedExpression();
		}
		#endregion
		#region RefreshCachedExpression
		protected override void RefreshCachedExpression() {
			if (ValueExpression == null || Value2Expression == null)
				return;
			BinaryBooleanParsedThing conditionOperator1;
			BinaryBooleanParsedThing conditionOperator2;
			ParsedThingFuncVar conditionFunction;
			switch (Condition) {
				case ConditionalFormattingRangeCondition.Inside:
					conditionOperator1 = ParsedThingGreaterEqual.Instance;
					conditionOperator2 = ParsedThingLessEqual.Instance;
					conditionFunction = new ParsedThingFuncVar(funcAnd, 2);
					break;
				case ConditionalFormattingRangeCondition.Outside:
					conditionOperator1 = ParsedThingLess.Instance;
					conditionOperator2 = ParsedThingGreater.Instance;
					conditionFunction = new ParsedThingFuncVar(funcOr, 2);
					break;
				default:
					Exceptions.ThrowArgumentException("Condition", Condition);
					return;
			}
			CachedExpression = new ParsedExpression();
			CachedExpression.Add(CellValue);
			CachedExpression.AddRange(ValueExpression);
			CachedExpression.Add(conditionOperator1);
			CachedExpression.Add(CellValue);
			CachedExpression.AddRange(Value2Expression);
			CachedExpression.Add(conditionOperator2);
			CachedExpression.Add(conditionFunction);
		}
		#endregion
		protected override ConditionalFormatting CreateInstanceCore(Worksheet targetWorksheet) {
			return new RangeFormulaConditionalFormatting(targetWorksheet, CellRange.Clone(), Condition, ValueExpression.Clone(), Value2Expression.Clone());
		}
		protected override void CopyFormulaData(FormulaConditionalFormatting source) {
			base.CopyFormulaData(source);
			RangeFormulaConditionalFormatting cf = source as RangeFormulaConditionalFormatting;
			if (cf == null)
				Exceptions.ThrowInvalidOperationException("Can't copy from another conditional formatting type.");
			Condition = cf.Condition;
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			RangeFormulaConditionalFormatting cf = obj as RangeFormulaConditionalFormatting;
			if (cf == null)
				return false;
			return condition == cf.condition;
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32(base.GetHashCode(), (int)condition);
		}
	}
	#endregion
	#region RankFormulaConditionalFormatting
	#region ConditionalFormattingRankCondition
	public enum ConditionalFormattingRankCondition {
		TopByRank,
		TopByPercent,
		BottomByRank,
		BottomByPercent
	}
	#endregion
	public class RankFormulaConditionalFormatting : FormulaConditionalFormatting {
		public const int DefaultRankValue = 10;
		ConditionalFormattingRankCondition condition;
		int rank;
		public RankFormulaConditionalFormatting(Worksheet sheet, CellRangeBase range, ConditionalFormattingRankCondition condition, int rank)
			: base(sheet, range) {
			this.condition = condition;
			this.rank = rank;
			RefreshCachedExpression();
		}
		public RankFormulaConditionalFormatting(Worksheet sheet, CellRangeBase range, ConditionalFormattingRankCondition condition, int rank, IErrorHandler errorHandler)
			: base(sheet, range) {
			if (rank < 1 || rank > 1000)
				errorHandler.HandleError(new ModelErrorInfo(ModelErrorType.CondFmtRank));
			this.condition = condition;
			this.rank = rank;
			RefreshCachedExpression();
		}
		#region OpenXmlTypeId
		public override ConditionalFormattingRuleType RuleType { get { return ConditionalFormattingRuleType.TopOrBottomValue; } }
		#endregion
		#region Condition
		public ConditionalFormattingRankCondition Condition {
			get { return condition; }
			set {
				if (condition == value)
					return;
				DocumentHistory history = Sheet.Workbook.History;
				ConditionalFormattingRankConditionHistoryItem item = new ConditionalFormattingRankConditionHistoryItem(this, condition, value);
				history.Add(item);
				item.Execute();
			}
		}
		protected internal void SetConditionCore(ConditionalFormattingRankCondition value) {
			condition = value;
			RefreshCachedExpression();
		}
		#endregion
		#region Rank
		public int Rank {
			get { return rank; }
			set {
				if (Rank == value)
					return;
				DocumentHistory history = Sheet.Workbook.History;
				ConditionalFormattingRankRankHistoryItem item = new ConditionalFormattingRankRankHistoryItem(this, rank, value);
				history.Add(item);
				item.Execute();
			}
		}
		protected internal void SetRankCore(int value) {
			this.rank = value;
			RefreshCachedExpression();
		}
		#endregion
		#region RefreshCachedExpression
		protected override void RefreshCachedExpression() {
			ParsedExpression newExpression = new ParsedExpression();
			switch (Condition) {
				case ConditionalFormattingRankCondition.BottomByPercent:
					AppendPercentExpression(newExpression, false);
					break;
				case ConditionalFormattingRankCondition.BottomByRank:
					AppendRankExpression(newExpression, false);
					break;
				case ConditionalFormattingRankCondition.TopByPercent:
					AppendPercentExpression(newExpression, true);
					break;
				case ConditionalFormattingRankCondition.TopByRank:
					AppendRankExpression(newExpression, true);
					break;
				default:
					Exceptions.ThrowArgumentException("Condition", Condition);
					break;
			}
			CachedExpression = newExpression;
		}
		void AppendLargeOrSmall(ParsedExpression target, bool top) {
			if (top) {
				target.Add(new ParsedThingFuncVar(funcLarge, 2));
				target.Add(ParsedThingGreaterEqual.Instance);
			}
			else {
				target.Add(new ParsedThingFuncVar(funcSmall, 2));
				target.Add(ParsedThingLessEqual.Instance);
			}
		}
		protected void AppendRankExpression(ParsedExpression target, bool top) {
			target.Add(CellValue);
			target.Add(new ParsedThingFuncVar(funcIsNumber, 1));
			target.Add(new ParsedThingNumeric() { Value = Rank });
			target.Add(CellValue);
			target.Add(new ParsedThingVariantValue(new VariantValue() { CellRangeValue = CellRange }));
			target.Add(top ? PtgZero : PtgOne);
			target.Add(new ParsedThingFuncVar(funcRank, 3));
			target.Add(ParsedThingGreaterEqual.Instance);
			target.Add(new ParsedThingFuncVar(funcAnd, 2));
		}
		protected void AppendPercentExpression(ParsedExpression target, bool top) {
			target.Add(CellValue);
			target.Add(new ParsedThingFuncVar(funcIsNumber, 1));
			target.Add(CellValue);
			target.Add(new ParsedThingVariantValue(new VariantValue() { CellRangeValue = CellRange }));
			target.Add(new ParsedThingVariantValue(new VariantValue() { CellRangeValue = CellRange }));
			target.Add(new ParsedThingFuncVar(funcCount, 1));
			target.Add(new ParsedThingNumeric() { Value = Rank * 0.01 });
			target.Add(ParsedThingMultiply.Instance);
			target.Add(PtgZero);
			target.Add(new ParsedThingFuncVar(funcRoundDown, 2));
			target.Add(PtgOne);
			target.Add(new ParsedThingFuncVar(funcMax, 2));
			AppendLargeOrSmall(target, top);
			target.Add(new ParsedThingFuncVar(funcAnd, 2));
		}
		#endregion
		protected override ConditionalFormatting CreateInstanceCore(Worksheet targetWorksheet) {
			return new RankFormulaConditionalFormatting(targetWorksheet, CellRange.Clone(), Condition, Rank);
		}
		protected override void CopyFormulaData(FormulaConditionalFormatting source) {
			RankFormulaConditionalFormatting cf = source as RankFormulaConditionalFormatting;
			if (cf == null)
				Exceptions.ThrowInvalidOperationException("Can't copy from another conditional formatting type.");
			Condition = cf.Condition;
			Rank = cf.Rank;
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			RankFormulaConditionalFormatting cf = obj as RankFormulaConditionalFormatting;
			if (cf == null)
				return false;
			if (condition != cf.condition)
				return false;
			return rank == cf.rank;
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32(base.GetHashCode(), (int)condition, rank);
		}
	}
	#endregion
	#region SpecialFormulaConditionalFormatting
	#region ConditionalFormattingSpecialCondition
	public enum ConditionalFormattingSpecialCondition {
		ContainError,
		NotContainError,
		ContainBlanks,
		ContainNonBlanks,
		ContainUniqueValue,
		ContainDuplicateValue,
		Unknown = -1
	}
	#endregion
	public class SpecialFormulaConditionalFormatting : FormulaConditionalFormatting {
		ConditionalFormattingSpecialCondition condition;
		public SpecialFormulaConditionalFormatting(Worksheet sheet, CellRangeBase range, ConditionalFormattingSpecialCondition condition)
			: base(sheet, range) {
			this.condition = condition;
			RefreshCachedExpression();
		}
		#region OpenXmlTypeId
		public override ConditionalFormattingRuleType RuleType { get { return GetOpenXmlTypeId(); } }
		ConditionalFormattingRuleType GetOpenXmlTypeId() {
			ConditionalFormattingRuleType result;
			switch (Condition) {
				case ConditionalFormattingSpecialCondition.ContainBlanks:
					result = ConditionalFormattingRuleType.CellIsBlank;
					break;
				case ConditionalFormattingSpecialCondition.ContainDuplicateValue:
					result = ConditionalFormattingRuleType.DuplicateValues;
					break;
				case ConditionalFormattingSpecialCondition.ContainError:
					result = ConditionalFormattingRuleType.ContainsErrors;
					break;
				case ConditionalFormattingSpecialCondition.ContainUniqueValue:
					result = ConditionalFormattingRuleType.UniqueValue;
					break;
				case ConditionalFormattingSpecialCondition.ContainNonBlanks:
					result = ConditionalFormattingRuleType.CellIsNotBlank;
					break;
				case ConditionalFormattingSpecialCondition.NotContainError:
					result = ConditionalFormattingRuleType.NotContainsErrors;
					break;
				default:
					result = ConditionalFormattingRuleType.Unknown;
					break;
			}
			return result;
		}
		#endregion
		#region Condition
		public ConditionalFormattingSpecialCondition Condition {
			get { return condition; }
			set {
				if (condition == value)
					return;
				DocumentHistory history = Sheet.Workbook.History;
				ConditionalFormattingSpecialConditionHistoryItem item = new ConditionalFormattingSpecialConditionHistoryItem(this, condition, value);
				history.Add(item);
				item.Execute();
			}
		}
		protected internal void SetConditionCore(ConditionalFormattingSpecialCondition value) {
			condition = value;
			RefreshCachedExpression();
		}
		#endregion
		#region RefreshCachedExpression
		protected override void RefreshCachedExpression() {
			ParsedExpression newExpression = new ParsedExpression();
			switch (Condition) {
				case ConditionalFormattingSpecialCondition.ContainBlanks:
					newExpression.Add(CellValue);
					newExpression.Add(new ParsedThingFuncVar(funcTrim, 1));
					newExpression.Add(new ParsedThingFuncVar(funcLen, 1));
					newExpression.Add(PtgZero);
					newExpression.Add(ParsedThingLessEqual.Instance);
					break;
				case ConditionalFormattingSpecialCondition.ContainUniqueValue:
					CreateUniqueDuplicatesExpression(newExpression, true);
					break;
				case ConditionalFormattingSpecialCondition.ContainDuplicateValue:
					CreateUniqueDuplicatesExpression(newExpression, false);
					break;
				case ConditionalFormattingSpecialCondition.ContainError:
					newExpression.Add(CellValue);
					newExpression.Add(new ParsedThingFuncVar(funcIsError, 1));
					break;
				case ConditionalFormattingSpecialCondition.ContainNonBlanks:
					newExpression.Add(CellValue);
					newExpression.Add(new ParsedThingFuncVar(funcIsError, 1));
					newExpression.Add(PtgTrue);
					newExpression.Add(CellValue);
					newExpression.Add(new ParsedThingFuncVar(funcTrim, 1));
					newExpression.Add(new ParsedThingFuncVar(funcLen, 1));
					newExpression.Add(PtgZero);
					newExpression.Add(ParsedThingGreater.Instance);
					newExpression.Add(new ParsedThingFuncVar(funcIf, 3));
					break;
				case ConditionalFormattingSpecialCondition.NotContainError:
					newExpression.Add(CellValue);
					newExpression.Add(new ParsedThingFuncVar(funcIsError, 1));
					newExpression.Add(new ParsedThingFuncVar(funcNot, 1));
					break;
				default:
					Exceptions.ThrowArgumentException("Condition", Condition);
					break;
			}
			CachedExpression = newExpression;
		}
		void CreateUniqueDuplicatesExpression(ParsedExpression result, bool isUnique) {
			if (CellRange == null)
				return;
			ParsedThingAttrSpace ptgAttrSpace = new ParsedThingAttrSpace(ParsedThingAttrSpaceType.SpaceBeforeBaseExpression, 1);
			if (CellRange.RangeType == CellRangeType.UnionRange) {
				CellUnion union = CellRange as CellUnion;
				int count = union.InnerCellRanges.Count;
				for (int i = 0; i < count; i++) {
					CellRangeBase part = union.InnerCellRanges[i];
					result.Add(new ParsedThingArea(part.GetWithModifiedPositionType(PositionType.Absolute) as CellRange));
					result.Add(ptgAttrSpace);
					result.Add(CellValue);
					result.Add(new ParsedThingFunc(funcCountIf, OperandDataType.Value));
				}
				for (int i = 0; i < (count - 1); i++)
					result.Add(ParsedThingAdd.Instance);
			}
			else {
				result.Add(new ParsedThingArea(CellRange.GetWithModifiedPositionType(PositionType.Absolute) as CellRange));
				result.Add(ptgAttrSpace);
				result.Add(CellValue);
				result.Add(new ParsedThingFunc(funcCountIf, OperandDataType.Value));
			}
			result.Add(PtgOne);
			if (isUnique)
				result.Add(ParsedThingEqual.Instance);
			else
				result.Add(ParsedThingGreater.Instance);
			result.Add(CellValue);
			result.Add(new ParsedThingFunc(funcIsBlank, OperandDataType.Value));
			result.Add(new ParsedThingFunc(funcNot, OperandDataType.Value));
			result.Add(new ParsedThingFuncVar(funcAnd, 2, OperandDataType.Value));
		}
		#endregion
		protected override ConditionalFormatting CreateInstanceCore(Worksheet targetWorksheet) {
			return new SpecialFormulaConditionalFormatting(targetWorksheet, CellRange.Clone(), Condition);
		}
		protected override void CopyFormulaData(FormulaConditionalFormatting source) {
			SpecialFormulaConditionalFormatting cf = source as SpecialFormulaConditionalFormatting;
			if (cf == null)
				Exceptions.ThrowInvalidOperationException("Can't copy from another conditional formatting type.");
			Condition = cf.Condition;
		}
		internal string GetFormulaString() {
			if (Condition == ConditionalFormattingSpecialCondition.ContainDuplicateValue || Condition == ConditionalFormattingSpecialCondition.ContainUniqueValue)
				return null;
			ParsedExpression expression = DevExpress.XtraSpreadsheet.Export.Xls.XlsCFExportHelper.GetRuleFormula(this);
			string result = GetStringValue(expression);
			return result.Remove(0, 1);
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			SpecialFormulaConditionalFormatting cf = obj as SpecialFormulaConditionalFormatting;
			if (cf == null)
				return false;
			return condition == cf.condition;
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32(base.GetHashCode(), (int)condition);
		}
	}
	#endregion
	#region TextFormulaConditionalFormatting
	#region ConditionalFormattingTextCondition
	public enum ConditionalFormattingTextCondition {
		Contains,
		NotContains,
		BeginsWith,
		EndsWith
	}
	#endregion
	public class TextFormulaConditionalFormatting : ValueBasedFormulaConditionalFormatting {
		ConditionalFormattingTextCondition condition;
		TextFormulaConditionalFormatting(Worksheet sheet, CellRangeBase range, ConditionalFormattingTextCondition condition, ParsedExpression value)
			: base(sheet, range, value) {
			this.condition = condition;
			RefreshCachedExpression();
		}
		public TextFormulaConditionalFormatting(Worksheet sheet, CellRangeBase range, ConditionalFormattingTextCondition condition, string value)
			: base(sheet, range, value) {
			this.condition = condition;
			RefreshCachedExpression();
		}
		public TextFormulaConditionalFormatting(Worksheet sheet, CellRangeBase range, ConditionalFormattingTextCondition condition, string value, IErrorHandler errorHandler)
			: base(sheet, range, value, errorHandler) {
			this.condition = condition;
			RefreshCachedExpression();
		}
		#region OpenXmlTypeId
		public override ConditionalFormattingRuleType RuleType { get { return GetOpenXmlTypeId(); } }
		ConditionalFormattingRuleType GetOpenXmlTypeId() {
			switch (Condition) {
				case ConditionalFormattingTextCondition.BeginsWith:
					return ConditionalFormattingRuleType.BeginsWithText;
				case ConditionalFormattingTextCondition.Contains:
					return ConditionalFormattingRuleType.ContainsText;
				case ConditionalFormattingTextCondition.EndsWith:
					return ConditionalFormattingRuleType.EndsWithText;
				case ConditionalFormattingTextCondition.NotContains:
					return ConditionalFormattingRuleType.NotContainsText;
				default:
					Exceptions.ThrowInvalidOperationException("Invalid condition code");
					return ConditionalFormattingRuleType.Unknown;
			}
		}
		#endregion
		#region Condition
		public ConditionalFormattingTextCondition Condition {
			get { return condition; }
			set {
				if (condition == value)
					return;
				DocumentHistory history = Sheet.Workbook.History;
				ConditionalFormattingTextConditionHistoryItem item = new ConditionalFormattingTextConditionHistoryItem(this, condition, value);
				history.Add(item);
				item.Execute();
			}
		}
		protected internal void SetConditionCore(ConditionalFormattingTextCondition value) {
			condition = value;
			RefreshCachedExpression();
		}
		#endregion
		#region RefreshCachedExpression
		protected override void RefreshCachedExpression() {
			CachedExpression = new ParsedExpression();
			switch (Condition) {
				case ConditionalFormattingTextCondition.BeginsWith:
					AppendLeftRightExpression(ValueExpression, funcLeft);
					break;
				case ConditionalFormattingTextCondition.Contains:
					AppendFindExpression(ValueExpression, true);
					break;
				case ConditionalFormattingTextCondition.EndsWith:
					AppendLeftRightExpression(ValueExpression, funcRight);
					break;
				case ConditionalFormattingTextCondition.NotContains:
					AppendFindExpression(ValueExpression, false);
					break;
				default:
					Exceptions.ThrowArgumentException("Condition", Condition);
					return;
			}
		}
		void AppendFindExpression(ParsedExpression valueExpression, bool contains) {
			CachedExpression.AddRange(valueExpression);
			CachedExpression.Add(CellValue);
			CachedExpression.Add(new ParsedThingFuncVar(funcSearch, 2));
			CachedExpression.Add(new ParsedThingFuncVar(funcIsError, 1));
			if (contains)
				CachedExpression.Add(new ParsedThingFuncVar(funcNot, 1));
		}
		void AppendLeftRightExpression(ParsedExpression valueExpression, int funcCode) {
			CachedExpression.Add(CellValue);
			CachedExpression.AddRange(valueExpression);
			CachedExpression.Add(new ParsedThingFuncVar(funcLen, 1));
			CachedExpression.Add(new ParsedThingFuncVar(funcCode, 2));
			CachedExpression.AddRange(valueExpression);
			CachedExpression.Add(ParsedThingEqual.Instance);
		}
		#endregion
		protected override ConditionalFormatting CreateInstanceCore(Worksheet targetWorksheet) {
			return new TextFormulaConditionalFormatting(targetWorksheet, CellRange.Clone(), Condition, ValueExpression.Clone());
		}
		protected override void CopyFormulaData(FormulaConditionalFormatting source) {
			base.CopyFormulaData(source);
			TextFormulaConditionalFormatting cf = source as TextFormulaConditionalFormatting;
			if (cf == null)
				Exceptions.ThrowInvalidOperationException("Can't copy from another conditional formatting type.");
			Condition = cf.Condition;
		}
		internal string GetFormulaString() {
			int cellValueIndex = Condition == ConditionalFormattingTextCondition.Contains || Condition == ConditionalFormattingTextCondition.NotContains ?
				ValueExpression.Count : 0;
			CachedExpression[cellValueIndex] = PtgRefRel;
			string result = GetStringValue(CachedExpression);
			CachedExpression[cellValueIndex] = CellValue;
			return result.Remove(0, 1);
		}
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			TextFormulaConditionalFormatting cf = obj as TextFormulaConditionalFormatting;
			if (cf == null)
				return false;
			return condition == cf.condition;
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32(base.GetHashCode(), (int)condition);
		}
	}
	#endregion
	#region TimePeriodFormulaConditionalFormatting
	#region ConditionalFormattingTimePeriod
	public enum ConditionalFormattingTimePeriod {
		Last7Days,
		LastMonth,
		LastWeek,
		NextMonth,
		NextWeek,
		ThisMonth,
		ThisWeek,
		Today,
		Tomorrow,
		Yesterday,
		Unknown = -1
	}
	#endregion
	public class TimePeriodFormulaConditionalFormatting : FormulaConditionalFormatting {
		protected delegate bool CheckDateFunction(DateTime value);
		CheckDateFunction dateChecker;
		readonly long weekOffset;
		ConditionalFormattingTimePeriod condition;
		public TimePeriodFormulaConditionalFormatting(Worksheet sheet, CellRangeBase range, ConditionalFormattingTimePeriod condition)
			: base(sheet, range) {
			this.condition = condition;
			weekOffset = GetDayOfWeekBase(sheet.DataContext);
			dateChecker = GetDateChecker(condition);
			RefreshCachedExpression();
		}
		#region OpenXmlTypeId
		public override ConditionalFormattingRuleType RuleType { get { return ConditionalFormattingRuleType.InsideDatePeriod; } }
		#endregion
		#region TimePeriod
		public ConditionalFormattingTimePeriod TimePeriod {
			get { return condition; }
			set {
				if (condition == value)
					return;
				DocumentHistory history = Sheet.Workbook.History;
				ConditionalFormattingTimePeriodConditionHistoryItem item = new ConditionalFormattingTimePeriodConditionHistoryItem(this, condition, value);
				history.Add(item);
				item.Execute();
			}
		}
		protected internal void SetConditionCore(ConditionalFormattingTimePeriod value) {
			condition = value;
			RefreshCachedExpression();
		}
		#endregion
		protected override void RefreshCachedExpression() {
		}
		public override bool Evaluate(ICell cell) {
			Guard.ArgumentNotNull(cell, "cell");
			VariantValue cellValue = cell.Value;
			if (cellValue.IsNumeric) {
				DateTime value = cell.Value.ToDateTime(DocumentModel.DataContext);
				return dateChecker(value);
			}
			return false;
		}
		protected CheckDateFunction GetDateChecker(ConditionalFormattingTimePeriod timePeriod) {
			switch (timePeriod) {
				case ConditionalFormattingTimePeriod.Last7Days:
					return DateLast7Days;
				case ConditionalFormattingTimePeriod.LastMonth:
					return DateLastMonth;
				case ConditionalFormattingTimePeriod.LastWeek:
					return DateLastWeek;
				case ConditionalFormattingTimePeriod.NextMonth:
					return DateNextMonth;
				case ConditionalFormattingTimePeriod.NextWeek:
					return DateNextWeek;
				case ConditionalFormattingTimePeriod.ThisMonth:
					return DateThisMonth;
				case ConditionalFormattingTimePeriod.ThisWeek:
					return DateThisWeek;
				case ConditionalFormattingTimePeriod.Today:
					return DateToday;
				case ConditionalFormattingTimePeriod.Tomorrow:
					return DateTomorrow;
				case ConditionalFormattingTimePeriod.Yesterday:
					return DateYesterday;
				default:
					Exceptions.ThrowInternalException();
					break;
			}
			return null;
		}
		#region Date checkers
		protected int GetMonth(DateTime dateTime) {
			return dateTime.Month + dateTime.Year * 12;
		}
		protected int GetWeek(DateTime dateTime) {
			long week = (dateTime.Ticks - weekOffset) / (7 * TimeSpan.TicksPerDay);
			return Convert.ToInt32(week);
		}
		protected int GetDay(DateTime dateTime) {
			long result = dateTime.Ticks / TimeSpan.TicksPerDay;
			return Convert.ToInt32(result);
		}
		protected bool DateLast7Days(DateTime value) {
			int day = GetDay(value);
			int now = GetDay(DateTime.Now);
			return (day <= now) && (day > (now - 7));
		}
		protected bool DateLastMonth(DateTime value) {
			return (GetMonth(DateTime.Now) - GetMonth(value)) == 1;
		}
		protected bool DateLastWeek(DateTime value) {
			return (GetWeek(DateTime.Now) - GetWeek(value)) == 1;
		}
		protected bool DateNextMonth(DateTime value) {
			return (GetMonth(value) - GetMonth(DateTime.Now)) == 1;
		}
		protected bool DateNextWeek(DateTime value) {
			return (GetWeek(value) - GetWeek(DateTime.Now)) == 1;
		}
		protected bool DateThisMonth(DateTime value) {
			return GetMonth(DateTime.Now) == GetMonth(value);
		}
		protected bool DateThisWeek(DateTime value) {
			return GetWeek(DateTime.Now) == GetWeek(value);
		}
		protected bool DateToday(DateTime value) {
			return DateTime.Now.Date == value.Date;
		}
		protected bool DateTomorrow(DateTime value) {
			return DateTime.Now.AddDays(1).Date == value.Date;
		}
		protected bool DateYesterday(DateTime value) {
			return DateTime.Now.AddDays(-1).Date == value.Date;
		}
		#endregion
		long GetDayOfWeekBase(WorkbookDataContext dataContext) {
			DateTime firstDayOfFirstWeek = new DateTime(1904, 1, 3 + (int)dataContext.Culture.DateTimeFormat.FirstDayOfWeek);
			return firstDayOfFirstWeek.Ticks;
		}
		protected override ConditionalFormatting CreateInstanceCore(Worksheet targetWorksheet) {
			return new TimePeriodFormulaConditionalFormatting(targetWorksheet, CellRange.Clone(), TimePeriod);
		}
		protected override void CopyFormulaData(FormulaConditionalFormatting source) {
			TimePeriodFormulaConditionalFormatting cf = source as TimePeriodFormulaConditionalFormatting;
			if (cf == null)
				Exceptions.ThrowInvalidOperationException("Can't copy from another conditional formatting type.");
			TimePeriod = cf.TimePeriod;
		}
		#region GetFormulaString
		public string GetFormulaString() {
			ParsedExpression result = new ParsedExpression();
			switch (TimePeriod) {
				case ConditionalFormattingTimePeriod.Last7Days:
					result.Add(new ParsedThingFuncVar(funcToday, 0));
					PutFloorExpression(result, PtgRefRel);
					result.Add(ParsedThingSubtract.Instance);
					result.Add(new ParsedThingNumeric() { Value = 6 });
					result.Add(ParsedThingLessEqual.Instance);
					PutFloorExpression(result, PtgRefRel);
					result.Add(new ParsedThingFuncVar(funcToday, 0));
					result.Add(ParsedThingLessEqual.Instance);
					result.Add(new ParsedThingFuncVar(funcAnd, 2));
					break;
				case ConditionalFormattingTimePeriod.LastMonth:
					result.Add(PtgRefRel);
					result.Add(new ParsedThingFuncVar(funcMonth, 1));
					result.Add(new ParsedThingFuncVar(funcToday, 0));
					result.Add(PtgZero);
					result.Add(PtgOne);
					result.Add(ParsedThingSubtract.Instance);
					result.Add(new ParsedThingFuncVar(funcEDate, 2));
					result.Add(new ParsedThingFuncVar(funcMonth, 1));
					result.Add(ParsedThingEqual.Instance);
					result.Add(PtgRefRel);
					result.Add(new ParsedThingFuncVar(funcYear, 1));
					result.Add(new ParsedThingFuncVar(funcToday, 0));
					result.Add(PtgZero);
					result.Add(PtgOne);
					result.Add(ParsedThingSubtract.Instance);
					result.Add(new ParsedThingFuncVar(funcEDate, 2));
					result.Add(new ParsedThingFuncVar(funcYear, 1));
					result.Add(ParsedThingEqual.Instance);
					result.Add(new ParsedThingFuncVar(funcAnd, 2));
					break;
				case ConditionalFormattingTimePeriod.LastWeek:
					result.Add(new ParsedThingFuncVar(funcToday, 0));
					PutRoundDownExpression(result, PtgRefRel);
					result.Add(ParsedThingSubtract.Instance);
					PutPartOfToday(result, funcWeekday);
					result.Add(ParsedThingGreaterEqual.Instance);
					result.Add(new ParsedThingFuncVar(funcToday, 0));
					PutRoundDownExpression(result, PtgRefRel);
					result.Add(ParsedThingSubtract.Instance);
					PutPartOfToday(result, funcWeekday);
					result.Add(new ParsedThingNumeric() { Value = 7 });
					result.Add(ParsedThingAdd.Instance);
					result.Add(ParsedThingLess.Instance);
					result.Add(new ParsedThingFuncVar(funcAnd, 2));
					break;
				case ConditionalFormattingTimePeriod.NextMonth:
					result.Add(PtgRefRel);
					result.Add(new ParsedThingFuncVar(funcMonth, 1));
					result.Add(new ParsedThingFuncVar(funcToday, 0));
					result.Add(PtgZero);
					result.Add(PtgOne);
					result.Add(ParsedThingAdd.Instance);
					result.Add(new ParsedThingFuncVar(funcEDate, 2));
					result.Add(new ParsedThingFuncVar(funcMonth, 1));
					result.Add(ParsedThingEqual.Instance);
					result.Add(PtgRefRel);
					result.Add(new ParsedThingFuncVar(funcYear, 1));
					result.Add(new ParsedThingFuncVar(funcToday, 0));
					result.Add(PtgZero);
					result.Add(PtgOne);
					result.Add(ParsedThingAdd.Instance);
					result.Add(new ParsedThingFuncVar(funcEDate, 2));
					result.Add(new ParsedThingFuncVar(funcYear, 1));
					result.Add(ParsedThingEqual.Instance);
					result.Add(new ParsedThingFuncVar(funcAnd, 2));
					break;
				case ConditionalFormattingTimePeriod.NextWeek:
					PutRoundDownExpression(result, PtgRefRel);
					result.Add(new ParsedThingFuncVar(funcToday, 0));
					result.Add(ParsedThingSubtract.Instance);
					result.Add(new ParsedThingNumeric() { Value = 7 });
					PutPartOfToday(result, funcWeekday);
					result.Add(ParsedThingSubtract.Instance);
					result.Add(ParsedThingGreater.Instance);
					PutRoundDownExpression(result, PtgRefRel);
					result.Add(new ParsedThingFuncVar(funcToday, 0));
					result.Add(ParsedThingSubtract.Instance);
					result.Add(new ParsedThingNumeric() { Value = 15 });
					PutPartOfToday(result, funcWeekday);
					result.Add(ParsedThingSubtract.Instance);
					result.Add(ParsedThingLess.Instance);
					result.Add(new ParsedThingFuncVar(funcAnd, 2));
					break;
				case ConditionalFormattingTimePeriod.ThisMonth:
					result.Add(PtgRefRel);
					result.Add(new ParsedThingFuncVar(funcMonth, 1));
					PutPartOfToday(result, funcMonth);
					result.Add(ParsedThingEqual.Instance);
					result.Add(PtgRefRel);
					result.Add(new ParsedThingFuncVar(funcYear, 1));
					PutPartOfToday(result, funcYear);
					result.Add(ParsedThingEqual.Instance);
					result.Add(new ParsedThingFuncVar(funcAnd, 2));
					break;
				case ConditionalFormattingTimePeriod.ThisWeek:
					result.Add(new ParsedThingFuncVar(funcToday, 0));
					PutRoundDownExpression(result, PtgRefRel);
					result.Add(ParsedThingSubtract.Instance);
					PutPartOfToday(result, funcWeekday);
					result.Add(PtgOne);
					result.Add(ParsedThingSubtract.Instance);
					result.Add(ParsedThingLessEqual.Instance);
					PutRoundDownExpression(result, PtgRefRel);
					result.Add(new ParsedThingFuncVar(funcToday, 0));
					result.Add(ParsedThingSubtract.Instance);
					result.Add(new ParsedThingNumeric() { Value = 7 });
					PutPartOfToday(result, funcWeekday);
					result.Add(ParsedThingSubtract.Instance);
					result.Add(ParsedThingLessEqual.Instance);
					result.Add(new ParsedThingFuncVar(funcAnd, 2));
					break;
				case ConditionalFormattingTimePeriod.Today:
					PutFloorExpression(result, PtgRefRel);
					result.Add(new ParsedThingFuncVar(funcToday, 0));
					result.Add(ParsedThingEqual.Instance);
					break;
				case ConditionalFormattingTimePeriod.Tomorrow:
					PutFloorExpression(result, PtgRefRel);
					result.Add(new ParsedThingFuncVar(funcToday, 0));
					result.Add(PtgOne);
					result.Add(ParsedThingAdd.Instance);
					result.Add(ParsedThingEqual.Instance);
					break;
				case ConditionalFormattingTimePeriod.Yesterday:
					PutFloorExpression(result, PtgRefRel);
					result.Add(new ParsedThingFuncVar(funcToday, 0));
					result.Add(PtgOne);
					result.Add(ParsedThingSubtract.Instance);
					result.Add(ParsedThingEqual.Instance);
					break;
			}
			return GetStringValue(result).Remove(0, 1);
		}
		void PutRoundDownExpression(ParsedExpression result, IParsedThing value) {
			result.Add(value);
			result.Add(PtgZero);
			result.Add(new ParsedThingFuncVar(funcRoundDown, 2));
		}
		void PutPartOfToday(ParsedExpression result, int partFuncCode) {
			result.Add(new ParsedThingFuncVar(funcToday, 0));
			result.Add(new ParsedThingFuncVar(partFuncCode, 1));
		}
		void PutFloorExpression(ParsedExpression result, IParsedThing value) {
			result.Add(value);
			result.Add(PtgOne);
			result.Add(new ParsedThingFuncVar(funcFloor, 2));
		}
		#endregion
		public override bool Equals(object obj) {
			if (!base.Equals(obj))
				return false;
			TimePeriodFormulaConditionalFormatting cf = obj as TimePeriodFormulaConditionalFormatting;
			if (cf == null)
				return false;
			return condition == cf.condition;
		}
		public override int GetHashCode() {
			return HashCodeCalculator.CalcHashCode32(base.GetHashCode(), (int)condition);
		}
	}
	#endregion
	#region ConditionalFormattingRPNVisitor
	public abstract class ConditionalFormattingRPNVisitor : ParsedThingVisitor {
		int diffBetweenNewAndOldRanges;
		WorkbookDataContext context;
		protected ConditionalFormattingRPNVisitor(WorkbookDataContext context) {
			this.context = context;
		}
		public int DiffBetweenNewAndOldRanges { get { return diffBetweenNewAndOldRanges; } set { diffBetweenNewAndOldRanges = value; } }
		protected abstract void ProcessRefRel(ParsedThingRefRel thing);
		protected abstract void ProcessAreaRel(ParsedThingAreaN thing);
		public override void Visit(ParsedThingRefRel thing) {
			ProcessRefRel(thing);
		}
		public override void Visit(ParsedThingRef3dRel thing) {
			if (!context.GetSheetDefinition(thing.SheetDefinitionIndex).ValidReference)
				return;
			ProcessRefRel(thing);
		}
		public override void Visit(ParsedThingAreaN thing) {
			ProcessAreaRel(thing);
		}
		public override void Visit(ParsedThingArea3dRel thing) {
			if (!context.GetSheetDefinition(thing.SheetDefinitionIndex).ValidReference)
				return;
			ProcessAreaRel(thing);
		}
	}
	#endregion
	#region ShiftLeftConditionalFormattingRPNVisitor
	public class ShiftLeftConditionalFormattingRPNVisitor : ConditionalFormattingRPNVisitor {
		public ShiftLeftConditionalFormattingRPNVisitor(WorkbookDataContext context)
			: base(context) {
		}
		protected override void ProcessRefRel(ParsedThingRefRel thing) {
			CellOffset offset = thing.Location;
			if (offset.ColumnType == CellOffsetType.Offset) {
				offset.Column += DiffBetweenNewAndOldRanges;
				thing.Location = offset;
			}
		}
		protected override void ProcessAreaRel(ParsedThingAreaN thing) {
			if (thing.First.ColumnType != CellOffsetType.Offset && thing.Last.ColumnType == CellOffsetType.Offset)
				return;
			CellOffset first = thing.First;
			if (first.ColumnType == CellOffsetType.Offset) {
				first.Column += DiffBetweenNewAndOldRanges;
				thing.First = first;
			}
			CellOffset last = thing.Last;
			if (last.ColumnType == CellOffsetType.Offset) {
				last.Column += DiffBetweenNewAndOldRanges;
				thing.Last = last;
			}
		}
	}
	#endregion
	#region ShiftRightConditionalFormattingRPNVisitor
	public class ShiftRightConditionalFormattingRPNVisitor : ConditionalFormattingRPNVisitor {
		public ShiftRightConditionalFormattingRPNVisitor(WorkbookDataContext context)
			: base(context) {
		}
		protected override void ProcessRefRel(ParsedThingRefRel thing) {
			CellOffset offset = thing.Location;
			if (offset.ColumnType == CellOffsetType.Offset) {
				offset.Column -= DiffBetweenNewAndOldRanges;
				thing.Location = offset;
			}
		}
		protected override void ProcessAreaRel(ParsedThingAreaN thing) {
			if (thing.First.ColumnType != CellOffsetType.Offset && thing.Last.ColumnType == CellOffsetType.Offset)
				return;
			CellOffset first = thing.First;
			if (first.ColumnType == CellOffsetType.Offset) {
				first.Column -= DiffBetweenNewAndOldRanges;
				thing.First = first;
			}
			CellOffset last = thing.Last;
			if (last.ColumnType == CellOffsetType.Offset) {
				last.Column -= DiffBetweenNewAndOldRanges;
				thing.Last = last;
			}
		}
	}
	#endregion
	#region ShiftUpConditionalFormattingRPNVisitor
	public class ShiftUpConditionalFormattingRPNVisitor : ConditionalFormattingRPNVisitor {
		public ShiftUpConditionalFormattingRPNVisitor(WorkbookDataContext context)
			: base(context) {
		}
		protected override void ProcessRefRel(ParsedThingRefRel thing) {
			CellOffset offset = thing.Location;
			if (offset.RowType == CellOffsetType.Offset) {
				offset.Row += DiffBetweenNewAndOldRanges;
				thing.Location = offset;
			}
		}
		protected override void ProcessAreaRel(ParsedThingAreaN thing) {
			if (thing.First.RowType != CellOffsetType.Offset && thing.Last.RowType == CellOffsetType.Offset)
				return;
			CellOffset first = thing.First;
			if (first.RowType == CellOffsetType.Offset) {
				first.Row += DiffBetweenNewAndOldRanges;
				thing.First = first;
			}
			CellOffset last = thing.Last;
			if (last.RowType == CellOffsetType.Offset) {
				last.Row += DiffBetweenNewAndOldRanges;
				thing.Last = last;
			}
		}
	}
	#endregion
	#region ShiftDownConditionalFormattingRPNVisitor
	public class ShiftDownConditionalFormattingRPNVisitor : ConditionalFormattingRPNVisitor {
		public ShiftDownConditionalFormattingRPNVisitor(WorkbookDataContext context)
			: base(context) {
		}
		protected override void ProcessRefRel(ParsedThingRefRel thing) {
			CellOffset offset = thing.Location;
			if (offset.RowType == CellOffsetType.Offset) {
				offset.Row -= DiffBetweenNewAndOldRanges;
				thing.Location = offset;
			}
		}
		protected override void ProcessAreaRel(ParsedThingAreaN thing) {
			if (thing.First.RowType != CellOffsetType.Offset && thing.Last.RowType == CellOffsetType.Offset)
				return;
			CellOffset first = thing.First;
			if (first.RowType == CellOffsetType.Offset) {
				first.Row -= DiffBetweenNewAndOldRanges;
				thing.First = first;
			}
			CellOffset last = thing.Last;
			if (last.RowType == CellOffsetType.Offset) {
				last.Row -= DiffBetweenNewAndOldRanges;
				thing.Last = last;
			}
		}
	}
	#endregion
	#region ConditionalFormattingRelativeReferenceChecker
	public class ConditionalFormattingRelativeReferenceChecker : ParsedThingVisitor {
		bool hasRelativeReferences;
		public override ParsedExpression Process(ParsedExpression expression) {
			throw new InvalidOperationException();
		}
		public bool HasRelativeReferences(ParsedExpression expression) {
			foreach (IParsedThing thing in expression) {
				thing.Visit(this);
				if (hasRelativeReferences)
					return true;
			}
			return false;
		}
		void CheckRelative(CellOffset offset) {
			if (offset.ColumnType == CellOffsetType.Offset || offset.RowType == CellOffsetType.Offset)
				hasRelativeReferences = true;
		}
		public override void Visit(ParsedThingRefRel thing) {
			CheckRelative(thing.Location);
		}
		public override void Visit(ParsedThingRef3dRel thing) {
			CheckRelative(thing.Location);
		}
		public override void Visit(ParsedThingAreaN thing) {
			CheckRelative(thing.First);
			CheckRelative(thing.Last);
		}
		public override void Visit(ParsedThingArea3dRel thing) {
			CheckRelative(thing.First);
			CheckRelative(thing.Last);
		}
	}
	#endregion
	#region ValueBasedConditionalFormattingNotificator
	public enum ShiftCellsNotificationMode {
		ShiftLeft = 1,
		ShiftRight = 2,
		ShiftUp = 4,
		ShiftDown = 8
	}
	public abstract class ConditionalFormattingStrongLogic : ReplaceThingsPRNVisitor {
		#region CellOffsetProcessor
		delegate CellOffset CellOffsetProcessor(int offset, CellOffset cellOffset);
		CellOffset ShiftDownProcessor(int offset, CellOffset cellOffset) {
			cellOffset.Row -= offset;
			return cellOffset;
		}
		CellOffset ShiftLeftProcessor(int offset, CellOffset cellOffset) {
			cellOffset.Column += offset;
			return cellOffset;
		}
		CellOffset ShiftRightProcessor(int offset, CellOffset cellOffset) {
			cellOffset.Column -= offset;
			return cellOffset;
		}
		CellOffset ShiftUpProcessor(int offset, CellOffset cellOffset) {
			cellOffset.Row += offset;
			return cellOffset;
		}
		#endregion
		#region class ValueBasedFormulaConditionalFormattingQueueItem
		class ValueBasedFormulaConditionalFormattingQueueItem {
			public ValueBasedFormulaConditionalFormattingQueueItem(ValueBasedFormulaConditionalFormatting conditionalFormatting, int startIndex) {
				ConditionalFormatting = conditionalFormatting;
				StartIndex = startIndex;
			}
			public ValueBasedFormulaConditionalFormattingQueueItem(ValueBasedFormulaConditionalFormatting conditionalFormatting, int startIndex, int cachedOffset, ParsedExpression cachedExpression)
				: this(conditionalFormatting, startIndex) {
				CachedOffset = cachedOffset;
				CachedExpression = cachedExpression;
			}
			public int CachedOffset { get; set; }
			public int StartIndex { get; set; }
			public ParsedExpression CachedExpression { get; set; }
			public ValueBasedFormulaConditionalFormatting ConditionalFormatting { get; set; }
		}
		#endregion
		CellOffsetProcessor cellOffsetProcessor;
		CellRange removableRange;
		List<ValueBasedFormulaConditionalFormatting> list;
		Queue<ValueBasedFormulaConditionalFormattingQueueItem> queue;
		ShiftCellsNotificationMode mode;
		WorkbookDataContext context;
		int currentIndex;
		ParsedExpression currentExpression;
		ValueBasedFormulaConditionalFormatting currentCF;
		protected ConditionalFormattingStrongLogic(List<ValueBasedFormulaConditionalFormatting> list, CellRange removableRange, ShiftCellsNotificationMode mode, WorkbookDataContext context) {
			this.list = list;
			this.removableRange = removableRange;
			this.mode = mode;
			this.context = context;
			queue = new Queue<ValueBasedFormulaConditionalFormattingQueueItem>();
			switch (mode) {
				case ShiftCellsNotificationMode.ShiftDown:
					cellOffsetProcessor = ShiftDownProcessor;
					break;
				case ShiftCellsNotificationMode.ShiftLeft:
					cellOffsetProcessor = ShiftLeftProcessor;
					break;
				case ShiftCellsNotificationMode.ShiftRight:
					cellOffsetProcessor = ShiftRightProcessor;
					break;
				case ShiftCellsNotificationMode.ShiftUp:
					cellOffsetProcessor = ShiftUpProcessor;
					break;
			}
		}
		protected List<ValueBasedFormulaConditionalFormatting> List { get { return list; } }
		protected CellRange RemovableRange { get { return removableRange; } }
		protected ShiftCellsNotificationMode Mode { get { return mode; } }
		protected ValueBasedFormulaConditionalFormatting CurrentCF { get { return currentCF; } set { currentCF = value; } }
		protected abstract ParsedExpression GetClonedExpression(ValueBasedFormulaConditionalFormatting cf);
		protected abstract void SetExpression(ValueBasedFormulaConditionalFormatting cf, ParsedExpression expression);
		public override ParsedExpression Process(ParsedExpression expression) {
			throw new InvalidOperationException();
		}
		public virtual void Process() {
			foreach (ValueBasedFormulaConditionalFormatting cf in list) {
				ValueBasedFormulaConditionalFormattingQueueItem queueItem = new ValueBasedFormulaConditionalFormattingQueueItem(cf, 0);
				if (cf.CellRange != null)
					queue.Enqueue(queueItem);
			}
			while (queue.Count > 0) {
				ValueBasedFormulaConditionalFormattingQueueItem queueItem = queue.Dequeue();
				currentCF = queueItem.ConditionalFormatting;
				currentIndex = queueItem.StartIndex;
				currentExpression = queueItem.CachedExpression == null ? GetClonedExpression(currentCF) : queueItem.CachedExpression;
				if (queueItem.CachedOffset != 0) {
					ProcessCachedOffset(queueItem.CachedOffset, currentExpression[currentIndex]);
					++currentIndex;
				}
				while (currentIndex < currentExpression.Count) {
					currentExpression[currentIndex].Visit(this);
					++currentIndex;
				}
				SetExpression(currentCF, currentExpression);
			}
		}
		void ProcessCachedOffset(int cachedOffset, IParsedThing thing) {
			ParsedThingRefRel refRel = thing as ParsedThingRefRel;
			if (refRel != null)
				refRel.Location = cellOffsetProcessor(cachedOffset, refRel.Location);
			else {
				ParsedThingAreaN areaRel = thing as ParsedThingAreaN;
				areaRel.First = cellOffsetProcessor(cachedOffset, areaRel.First);
				areaRel.Last = cellOffsetProcessor(cachedOffset, areaRel.Last);
			}
		}
		#region ParsedThingVisitor
		bool ShouldProcessSheet(IWorksheet sheet) {
			return sheet.SheetId == removableRange.SheetId;
		}
		bool ShouldProcessSheetDefinition(int sheetDefinitionIndex) {
			SheetDefinition sheetDefinition = context.GetSheetDefinition(sheetDefinitionIndex);
			if (sheetDefinition.Is3DReference || !sheetDefinition.ValidReference)
				return false;
			IWorksheet sheet = sheetDefinition.GetSheetStart(context);
			return ShouldProcessSheet(sheet);
		}
		bool IsColumnRange(CellOffset first, CellOffset last) {
			return first.Row == 0 && last.Row == context.MaxRowCount - 1 && first.RowType == CellOffsetType.Position && last.RowType == CellOffsetType.Position;
		}
		bool IsRowRange(CellOffset first, CellOffset last) {
			return first.Column == 0 && last.Column == context.MaxColumnCount - 1 && first.ColumnType == CellOffsetType.Position && last.ColumnType == CellOffsetType.Position;
		}
		void VisitRef(ParsedThingRefRel thing) {
			int offset = ProcessRelativeRange(thing.Location);
			thing.Location = cellOffsetProcessor(offset, thing.Location);
		}
		void VisitArea(ParsedThingAreaN thing) {
			if (IsColumnRange(thing.First, thing.Last) && mode.HasAnyFlag(ShiftCellsNotificationMode.ShiftDown, ShiftCellsNotificationMode.ShiftUp) ||
				IsRowRange(thing.First, thing.Last) && mode.HasAnyFlag(ShiftCellsNotificationMode.ShiftLeft, ShiftCellsNotificationMode.ShiftRight))
				return;
			int firstOffset = ProcessRelativeRange(thing.First);
			int lastOffset = ProcessRelativeRange(thing.Last);
			if (firstOffset != lastOffset)
				return;
			thing.First = cellOffsetProcessor(firstOffset, thing.First);
			thing.Last = cellOffsetProcessor(lastOffset, thing.Last);
		}
		public override void Visit(ParsedThingRefRel thing) {
			if (!ShouldProcessSheet(context.CurrentWorksheet))
				return;
			VisitRef(thing);
		}
		public override void Visit(ParsedThingRef3dRel thing) {
			if (!ShouldProcessSheetDefinition(thing.SheetDefinitionIndex))
				return;
			VisitRef(thing);
		}
		public override void Visit(ParsedThingAreaN thing) {
			if (!ShouldProcessSheet(context.CurrentWorksheet))
				return;
			VisitArea(thing);
		}
		public override void Visit(ParsedThingArea3dRel thing) {
			if (!ShouldProcessSheetDefinition(thing.SheetDefinitionIndex))
				return;
			VisitArea(thing);
		}
		#endregion
		int ProcessRelativeRange(CellOffset offset) {
			CellRangeBase range = currentCF.CellRange;
			RangeNotificationInfo info;
			if (range.RangeType == CellRangeType.UnionRange) {
				CellUnion union = range as CellUnion;
				info = new RangeNotificationInfo();
				foreach (CellRange item in union.InnerCellRanges) {
					RangeNotificationInfo itemInfo = ProcessRelativeRangeCore(item, offset);
					info.AddNonShiftedRange(itemInfo.NonShiftedRange);
					info.AddShiftedRange(itemInfo.ShiftedRange, itemInfo.Offset);
				}
			}
			else
				info = ProcessRelativeRangeCore(range as CellRange, offset);
			return ProcessNotificationByInfo(info);
		}
		RangeNotificationInfo ProcessRelativeRangeCore(CellRange range, CellOffset offset) {
			CellPosition relativeTopLeft;
			CellRangeBase relativeRange = GetRelativeRangeCore(range, offset, out relativeTopLeft);
			CellOffset howToGetHostRange = new CellOffset(range.TopLeft.Column - relativeTopLeft.Column, range.TopLeft.Row - relativeTopLeft.Row, CellOffsetType.Offset, CellOffsetType.Offset);
			RangeNotificationInfo info = ConditionalFormattingNotificator.GetRangeNotificationInfo(removableRange, relativeRange, removableRange.Worksheet, mode, true);
			info.NonShiftedRange = GetHostRange(info.NonShiftedRange, howToGetHostRange);
			howToGetHostRange = cellOffsetProcessor(info.Offset, howToGetHostRange);
			info.ShiftedRange = GetHostRange(info.ShiftedRange, howToGetHostRange);
			info.Offset = -info.Offset;
			if (offset.ColumnType != CellOffsetType.Offset) {
				info.NonShiftedRange = EnlargeRange(range, info.NonShiftedRange, false);
				info.ShiftedRange = EnlargeRange(range, info.ShiftedRange, false);
			}
			if (offset.RowType != CellOffsetType.Offset) {
				info.NonShiftedRange = EnlargeRange(range, info.NonShiftedRange, true);
				info.ShiftedRange = EnlargeRange(range, info.ShiftedRange, true);
			}
			return info;
		}
		CellRangeBase GetHostRange(CellRangeBase range, CellOffset offset) {
			if (range == null)
				return null;
			if (range.RangeType == CellRangeType.UnionRange) {
				CellUnion union = range as CellUnion;
				List<CellRangeBase> result = new List<CellRangeBase>();
				foreach (CellRange item in union.InnerCellRanges) {
					CellRangeBase relativeToItem = GetRelativeRangeCore(item, offset);
					if (relativeToItem.RangeType == CellRangeType.UnionRange) {
						CellUnion unionRelativeToItem = relativeToItem as CellUnion;
						foreach (CellRangeBase itemRelativeToItem in unionRelativeToItem.InnerCellRanges)
							result.Add(itemRelativeToItem);
					}
					else
						result.Add(relativeToItem);
				}
				return new CellUnion(result);
			}
			else
				return GetRelativeRangeCore(range as CellRange, offset);
		}
		CellRangeBase GetRelativeRangeCore(CellRange range, CellOffset offset) {
			CellPosition pos;
			return GetRelativeRangeCore(range, offset, out pos);
		}
		CellRangeBase GetRelativeRangeCore(CellRange range, CellOffset offset, out CellPosition topLeft) {
			CellPosition botRight;
			context.PushCurrentCell(range.TopLeft);
			try {
				topLeft = offset.ToCellPosition(context);
			}
			finally {
				context.PopCurrentCell();
			}
			context.PushCurrentCell(range.BottomRight);
			try {
				botRight = offset.ToCellPosition(context);
			}
			finally {
				context.PopCurrentCell();
			}
			List<CellRangeBase> list;
			if (topLeft.Column > botRight.Column) {
				if (topLeft.Row > botRight.Row) {
					list = new List<CellRangeBase>();
					list.Add(new CellRange(range.Worksheet, new CellPosition(0, 0, PositionType.Absolute, PositionType.Absolute), topLeft));
					list.Add(new CellRange(range.Worksheet, botRight, new CellPosition(range.Worksheet.MaxColumnCount - 1, range.Worksheet.MaxRowCount - 1, PositionType.Absolute, PositionType.Absolute)));
					return new CellUnion(list);
				}
				list = new List<CellRangeBase>();
				list.Add(new CellRange(range.Worksheet, new CellPosition(0, topLeft.Row, PositionType.Absolute, topLeft.RowType), botRight));
				list.Add(new CellRange(range.Worksheet, topLeft, new CellPosition(range.Worksheet.MaxColumnCount - 1, botRight.Row, PositionType.Absolute, botRight.ColumnType)));
				return new CellUnion(list);
			}
			if (topLeft.Row > botRight.Row) {
				list = new List<CellRangeBase>();
				list.Add(new CellRange(range.Worksheet, new CellPosition(topLeft.Column, 0, topLeft.ColumnType, PositionType.Absolute), botRight));
				list.Add(new CellRange(range.Worksheet, topLeft, new CellPosition(botRight.Column, range.Worksheet.MaxRowCount - 1, botRight.ColumnType, PositionType.Absolute)));
				return new CellUnion(list);
			}
			return new CellRange(removableRange.Worksheet, topLeft, botRight);
		}
		CellRangeBase EnlargeRange(CellRange cfRange, CellRangeBase relativeRange, bool enlargeByRows) {
			if (relativeRange == null || cfRange == null)
				return null;
			Func<CellRange, CellRange> enlarge = enlargeByRows ?
				new Func<CellRange, CellRange>(delegate(CellRange range) { return range.GetResized(0, 0, 0, cfRange.Height - 1); }) :
				new Func<CellRange, CellRange>(delegate(CellRange range) { return range.GetResized(0, 0, cfRange.Width - 1, 0); });
			if (relativeRange.RangeType == CellRangeType.UnionRange) {
				CellUnion union = relativeRange as CellUnion;
				List<CellRangeBase> result = new List<CellRangeBase>();
				foreach (CellRange item in union.InnerCellRanges)
					result.Add(enlarge(item));
				return new CellUnion(result);
			}
			return enlarge(relativeRange as CellRange);
		}
		int ProcessNotificationByInfo(RangeNotificationInfo info) {
			if (info.NonShiftedRange == null) {
				if (info.ShiftedRange == null) {
					currentCF.SetCellRangeOrNull(null);
					return 0;
				}
				currentCF.SetCellRange(info.ShiftedRange);
				return info.Offset;
			}
			else {
				if (info.ShiftedRange != null) {
					ValueBasedFormulaConditionalFormatting newCF = FormulaConditionalFormatting.CreateCopy(currentCF, currentCF.Sheet) as ValueBasedFormulaConditionalFormatting;
					newCF.SetCellRange(info.ShiftedRange);
					list.Add(newCF);
					ValueBasedFormulaConditionalFormattingQueueItem item = new ValueBasedFormulaConditionalFormattingQueueItem(newCF, currentIndex, info.Offset, currentExpression.Clone());
					queue.Enqueue(item);
				}
				currentCF.SetCellRange(info.NonShiftedRange);
				return 0;
			}
		}
	}
	public class Value1ConditionalFormattingStrongLogic : ConditionalFormattingStrongLogic {
		public Value1ConditionalFormattingStrongLogic(List<ValueBasedFormulaConditionalFormatting> list, CellRange removableRange, ShiftCellsNotificationMode mode, WorkbookDataContext context)
			: base(list, removableRange, mode, context) {
		}
		protected override ParsedExpression GetClonedExpression(ValueBasedFormulaConditionalFormatting cf) {
			return cf.ValueExpression.Clone();
		}
		protected override void SetExpression(ValueBasedFormulaConditionalFormatting cf, ParsedExpression expression) {
			cf.ValueExpression = expression;
		}
	}
	public class Value2ConditionalFormattingStrongLogic : ConditionalFormattingStrongLogic {
		public Value2ConditionalFormattingStrongLogic(List<ValueBasedFormulaConditionalFormatting> list, CellRange removableRange, ShiftCellsNotificationMode mode, WorkbookDataContext context)
			: base(list, removableRange, mode, context) {
		}
		protected override ParsedExpression GetClonedExpression(ValueBasedFormulaConditionalFormatting cf) {
			return (cf as RangeValueBasedFormulaConditionalFormatting).Value2Expression.Clone();
		}
		protected override void SetExpression(ValueBasedFormulaConditionalFormatting cf, ParsedExpression expression) {
			(cf as RangeValueBasedFormulaConditionalFormatting).Value2Expression = expression;
		}
	}
	#endregion
	#region ConditionalFormattingNotificator
	public class ConditionalFormattingNotificator : ParsedThingVisitor {
		public static RangeNotificationInfo GetRangeNotificationInfo(CellRange removableRange, CellRangeBase cfRange, ICellTable cfSheet, ShiftCellsNotificationMode mode, bool splitIfInsertedInto) {
			RangeNotificationInfo info = new RangeNotificationInfo();
			if (removableRange == null || cfRange == null)
				return info;
			if (!object.ReferenceEquals(removableRange.Worksheet, cfSheet)) {
				info.NonShiftedRange = cfRange;
				return info;
			}
			switch (mode) {
				case ShiftCellsNotificationMode.ShiftDown:
					ProcessRangeShiftDown(removableRange, cfRange, info, splitIfInsertedInto);
					break;
				case ShiftCellsNotificationMode.ShiftLeft:
					ProcessRangeShiftLeft(removableRange, cfRange, info);
					break;
				case ShiftCellsNotificationMode.ShiftRight:
					ProcessRangeShiftRight(removableRange, cfRange, info, splitIfInsertedInto);
					break;
				case ShiftCellsNotificationMode.ShiftUp:
					ProcessRangeShiftUp(removableRange, cfRange, info);
					break;
				default:
					info.AddNonShiftedRange(cfRange);
					break;
			}
			return info;
		}
		#region Shift up
		static void ProcessRangeShiftUp(CellRange removableRange, CellRange processableRange, RangeNotificationInfo info) {
			if (removableRange.Includes(processableRange))
				return;
			if (processableRange.BottomRight.Row < removableRange.TopLeft.Row ||
				processableRange.BottomRight.Column < removableRange.TopLeft.Column ||
				processableRange.TopLeft.Column > removableRange.BottomRight.Column) {
				info.AddNonShiftedRange(processableRange);
				return;
			}
			int firstShiftedColumn = 0;
			if (processableRange.TopLeft.Column < removableRange.TopLeft.Column) {
				firstShiftedColumn = removableRange.TopLeft.Column - processableRange.TopLeft.Column;
				CellRange rangeBeforeActialProcessableRange = processableRange.GetSubColumnRange(0, firstShiftedColumn - 1);
				info.AddNonShiftedRange(rangeBeforeActialProcessableRange);
			}
			int lastShiftedColumn = processableRange.Width - 1;
			if (processableRange.BottomRight.Column > removableRange.BottomRight.Column) {
				lastShiftedColumn = removableRange.BottomRight.Column - processableRange.TopLeft.Column;
				CellRange rangeAfterActualProcessableRange = processableRange.GetSubColumnRange(lastShiftedColumn + 1, processableRange.Width - 1);
				info.AddNonShiftedRange(rangeAfterActualProcessableRange);
			}
			processableRange = processableRange.GetSubColumnRange(firstShiftedColumn, lastShiftedColumn);
			ProcessRangeShiftUpCore(removableRange, processableRange, info);
		}
		static void ProcessRangeShiftUpCore(CellRange removableRange, CellRange processableRange, RangeNotificationInfo info) {
			int topShift = 0;
			int bottomShift = 0;
			int firstRowShifted = 0;
			if (processableRange.TopLeft.Row > removableRange.TopLeft.Row)
				if (processableRange.TopLeft.Row >= removableRange.BottomRight.Row)
					topShift = -removableRange.Height;
				else
					topShift = removableRange.TopLeft.Row - processableRange.TopLeft.Row;
			else
				firstRowShifted = removableRange.TopLeft.Row - processableRange.TopLeft.Row;
			if (processableRange.BottomRight.Row >= removableRange.BottomRight.Row) {
				bottomShift = -removableRange.Height;
				processableRange = processableRange.GetResized(0, topShift, 0, bottomShift);
				if (firstRowShifted > 0) {
					CellRange rowsBeforeShifted = processableRange.GetSubRowRange(0, firstRowShifted - 1);
					info.AddNonShiftedRange(rowsBeforeShifted);
				}
				if (firstRowShifted <= processableRange.Height - 1) {
					CellRange shiftedRows = processableRange.GetSubRowRange(firstRowShifted, processableRange.Height - 1);
					info.AddShiftedRange(shiftedRows, removableRange.Height);
				}
			}
			else {
				bottomShift = removableRange.TopLeft.Row - processableRange.BottomRight.Row - 1;
				processableRange = processableRange.GetResized(0, topShift, 0, bottomShift);
				info.AddNonShiftedRange(processableRange);
			}
		}
		static void ProcessRangeShiftUp(CellRange removableRange, CellRangeBase cfRange, RangeNotificationInfo info) {
			if (cfRange.RangeType == CellRangeType.UnionRange) {
				CellUnion union = cfRange as CellUnion;
				foreach (CellRange item in union.InnerCellRanges)
					ProcessRangeShiftUp(removableRange, item, info);
			}
			else
				ProcessRangeShiftUp(removableRange, cfRange as CellRange, info);
		}
		#endregion
		#region Shift left
		static void ProcessRangeShiftLeft(CellRange removableRange, CellRange processableRange, RangeNotificationInfo info) {
			if (removableRange.Includes(processableRange))
				return;
			if (processableRange.BottomRight.Column < removableRange.TopLeft.Column ||
				processableRange.BottomRight.Row < removableRange.TopLeft.Row ||
				processableRange.TopLeft.Row > removableRange.BottomRight.Row) {
				info.AddNonShiftedRange(processableRange);
				return;
			}
			int firstShiftedRow = 0;
			if (processableRange.TopLeft.Row < removableRange.TopLeft.Row) {
				firstShiftedRow = removableRange.TopLeft.Row - processableRange.TopLeft.Row;
				CellRange rowsBeforeModifiedRows = processableRange.GetSubRowRange(0, firstShiftedRow - 1);
				info.AddNonShiftedRange(rowsBeforeModifiedRows);
			}
			int lastShiftedRow = processableRange.Height - 1;
			if (processableRange.BottomRight.Row > removableRange.BottomRight.Row) {
				lastShiftedRow = removableRange.BottomRight.Row - processableRange.TopLeft.Row;
				CellRange rowsAfterModifiedRows = processableRange.GetSubRowRange(lastShiftedRow + 1, processableRange.Height - 1);
				info.AddNonShiftedRange(rowsAfterModifiedRows);
			}
			processableRange = processableRange.GetSubRowRange(firstShiftedRow, lastShiftedRow);
			ProcessRangeShiftLeftCore(removableRange, processableRange, info);
		}
		static void ProcessRangeShiftLeftCore(CellRange removableRange, CellRange processableRange, RangeNotificationInfo info) {
			int leftShift = 0;
			int rightShift = 0;
			int firstColumnShifted = 0;
			if (processableRange.TopLeft.Column > removableRange.TopLeft.Column)
				if (processableRange.TopLeft.Column >= removableRange.BottomRight.Column)
					leftShift = -removableRange.Width;
				else
					leftShift = removableRange.TopLeft.Column - processableRange.TopLeft.Column;
			else
				firstColumnShifted = removableRange.TopLeft.Column - processableRange.TopLeft.Column;
			if (processableRange.BottomRight.Column >= removableRange.BottomRight.Column) {
				rightShift = -removableRange.Width;
				processableRange = processableRange.GetResized(leftShift, 0, rightShift, 0);
				if (firstColumnShifted > 0) {
					CellRange columnsBeforeShifted = processableRange.GetSubColumnRange(0, firstColumnShifted - 1);
					info.AddNonShiftedRange(columnsBeforeShifted);
				}
				if (firstColumnShifted <= processableRange.Width - 1) {
					CellRange shiftedColumns = processableRange.GetSubColumnRange(firstColumnShifted, processableRange.Width - 1);
					info.AddShiftedRange(shiftedColumns, removableRange.Width);
				}
			}
			else {
				rightShift = removableRange.TopLeft.Column - processableRange.BottomRight.Column - 1;
				processableRange = processableRange.GetResized(leftShift, 0, rightShift, 0);
				info.AddNonShiftedRange(processableRange);
			}
		}
		static void ProcessRangeShiftLeft(CellRange removableRange, CellRangeBase cfRange, RangeNotificationInfo info) {
			if (cfRange.RangeType == CellRangeType.UnionRange) {
				CellUnion union = cfRange as CellUnion;
				foreach (CellRange item in union.InnerCellRanges)
					ProcessRangeShiftLeft(removableRange, item, info);
			}
			else
				ProcessRangeShiftLeft(removableRange, cfRange as CellRange, info);
		}
		#endregion
		#region Shift down
		static void ProcessRangeShiftDown(CellRange insertableRange, CellRange processableRange, RangeNotificationInfo info, bool splitIfInsertedInto) {
			if (insertableRange.TopLeft.Column > processableRange.BottomRight.Column ||
				insertableRange.BottomRight.Column < processableRange.TopLeft.Column ||
				insertableRange.TopLeft.Row > processableRange.BottomRight.Row + 1) {
				info.AddNonShiftedRange(processableRange);
				return;
			}
			int firstModifiedColumn = 0;
			if (processableRange.TopLeft.Column < insertableRange.TopLeft.Column) {
				firstModifiedColumn = insertableRange.TopLeft.Column - processableRange.TopLeft.Column;
				CellRange columnsBeforeModifiedColumns = processableRange.GetSubColumnRange(0, firstModifiedColumn - 1);
				info.AddNonShiftedRange(columnsBeforeModifiedColumns);
			}
			int lastModifiedColumn = processableRange.Width - 1;
			if (processableRange.BottomRight.Column > insertableRange.BottomRight.Column) {
				lastModifiedColumn = insertableRange.BottomRight.Column - processableRange.TopLeft.Column;
				CellRange columnsAfterModyfiedColumns = processableRange.GetSubColumnRange(lastModifiedColumn + 1, processableRange.Width - 1);
				info.AddNonShiftedRange(columnsAfterModyfiedColumns);
			}
			CellRange modifiedColumns = processableRange.GetSubColumnRange(firstModifiedColumn, lastModifiedColumn);
			CellIntervalRange columnsInterval = modifiedColumns as CellIntervalRange;
			if (columnsInterval != null && columnsInterval.IsColumnInterval) {
				info.AddNonShiftedRange(processableRange);
				return;
			}
			ProcessRangeShiftDownCore(insertableRange, modifiedColumns, info, splitIfInsertedInto);
		}
		static void ProcessRangeShiftDownCore(CellRange insertableRange, CellRange modifiedColumns, RangeNotificationInfo info, bool splitIfInsertedInto) {
			int height = insertableRange.Height;
			if (modifiedColumns.TopLeft.Row >= insertableRange.TopLeft.Row) {
				modifiedColumns = modifiedColumns.GetResizedLimited(0, height, 0, height);
				info.AddShiftedRange(modifiedColumns, height);
			}
			else {
				modifiedColumns = modifiedColumns.GetResizedLimited(0, 0, 0, height);
				int firstShiftedRow = insertableRange.BottomRight.Row - modifiedColumns.TopLeft.Row + 1;
				int lastNonShiftedRow =
					splitIfInsertedInto ? insertableRange.TopLeft.Row - modifiedColumns.TopLeft.Row - 1 : firstShiftedRow - 1;
				CellRange nonShiftedRows = modifiedColumns.GetSubRowRange(0, lastNonShiftedRow);
				info.AddNonShiftedRange(nonShiftedRows);
				if (firstShiftedRow <= modifiedColumns.Height - 1) {
					CellRange shiftedRows = modifiedColumns.GetSubRowRange(firstShiftedRow, modifiedColumns.Height - 1);
					info.AddShiftedRange(shiftedRows, height);
				}
			}
		}
		static void ProcessRangeShiftDown(CellRange insertableRange, CellRangeBase cfRange, RangeNotificationInfo info, bool splitIfInsertedInto) {
			if (cfRange.RangeType == CellRangeType.UnionRange) {
				CellUnion union = cfRange as CellUnion;
				foreach (CellRange item in union.InnerCellRanges)
					ProcessRangeShiftDown(insertableRange, item, info, splitIfInsertedInto);
			}
			else
				ProcessRangeShiftDown(insertableRange, cfRange as CellRange, info, splitIfInsertedInto);
		}
		#endregion
		#region Shift right
		static void ProcessRangeShiftRight(CellRange insertableRange, CellRange processableRange, RangeNotificationInfo info, bool splitIfInsertedInto) {
			if (insertableRange.TopLeft.Row > processableRange.BottomRight.Row ||
				insertableRange.BottomRight.Row < processableRange.TopLeft.Row ||
				insertableRange.TopLeft.Column > processableRange.BottomRight.Column + 1) {
				info.AddNonShiftedRange(processableRange);
				return;
			}
			int firstModifiedRow = 0;
			if (processableRange.TopLeft.Row < insertableRange.TopLeft.Row) {
				firstModifiedRow = insertableRange.TopLeft.Row - processableRange.TopLeft.Row;
				CellRange rowsBeforeModifiedRows = processableRange.GetSubRowRange(0, firstModifiedRow - 1);
				info.AddNonShiftedRange(rowsBeforeModifiedRows);
			}
			int lastModifiedRow = processableRange.Height - 1;
			if (processableRange.BottomRight.Row > insertableRange.BottomRight.Row) {
				lastModifiedRow = insertableRange.BottomRight.Row - processableRange.TopLeft.Row;
				CellRange rowsAterModifiedRows = processableRange.GetSubRowRange(lastModifiedRow + 1, processableRange.Height - 1);
				info.AddNonShiftedRange(rowsAterModifiedRows);
			}
			CellRange modifiedRows = processableRange.GetSubRowRange(firstModifiedRow, lastModifiedRow);
			ProcessRangeShiftRightCore(insertableRange, modifiedRows, info, splitIfInsertedInto);
		}
		static void ProcessRangeShiftRightCore(CellRange insertableRange, CellRange modifiedRows, RangeNotificationInfo info, bool splitIfInsertedInto) {
			int width = insertableRange.Width;
			if (modifiedRows.TopLeft.Column >= insertableRange.TopLeft.Column) {
				modifiedRows = modifiedRows.GetResized(width, 0, width, 0);
				info.AddShiftedRange(modifiedRows, width);
			}
			else {
				modifiedRows = modifiedRows.GetResized(0, 0, width, 0);
				int firstShiftedColumn = insertableRange.BottomRight.Column - modifiedRows.TopLeft.Column + 1;
				int lastNonShiftedColumn =
					splitIfInsertedInto ? insertableRange.TopLeft.Column - modifiedRows.TopLeft.Column - 1 : firstShiftedColumn - 1;
				CellRange nonShiftedColumns = modifiedRows.GetSubColumnRange(0, lastNonShiftedColumn);
				info.AddNonShiftedRange(nonShiftedColumns);
				if (firstShiftedColumn <= modifiedRows.Width - 1) {
					CellRange shiftedColumns = modifiedRows.GetSubColumnRange(firstShiftedColumn, modifiedRows.Width - 1);
					info.AddShiftedRange(shiftedColumns, width);
				}
			}
		}
		static void ProcessRangeShiftRight(CellRange insertableRange, CellRangeBase cfRange, RangeNotificationInfo info, bool splitIfInsertedInto) {
			if (cfRange.RangeType == CellRangeType.UnionRange) {
				CellUnion union = cfRange as CellUnion;
				foreach (CellRange item in union.InnerCellRanges)
					ProcessRangeShiftRight(insertableRange, item, info, splitIfInsertedInto);
			}
			else
				ProcessRangeShiftRight(insertableRange, cfRange as CellRange, info, splitIfInsertedInto);
		}
		#endregion
	}
	#endregion
	#region RangeNotificationInfo
	public class RangeNotificationInfo {
		CellRangeBase shiftedRange;
		CellRangeBase nonShiftedRange;
		int offset;
		public CellRangeBase ShiftedRange { get { return shiftedRange; } set { shiftedRange = value; } }
		public CellRangeBase NonShiftedRange { get { return nonShiftedRange; } set { nonShiftedRange = value; } }
		public int Offset { get { return offset; } set { offset = value; } }
		public CellRangeBase GetMergedRange() {
			if (shiftedRange == null)
				return nonShiftedRange;
			if (nonShiftedRange == null)
				return shiftedRange;
			CellRangeBase result = shiftedRange.MergeWithRange(nonShiftedRange);
			result.Worksheet = shiftedRange.Worksheet;
			return result;
		}
		public void AddShiftedRange(CellRangeBase value, int offset) {
			if (value != null) {
				shiftedRange = shiftedRange == null ? value : shiftedRange.MergeWithRange(value);
				this.offset = offset;
			}
		}
		public void AddNonShiftedRange(CellRangeBase value) {
			if (value != null)
				nonShiftedRange = nonShiftedRange == null ? value : nonShiftedRange.MergeWithRange(value);
		}
	}
	#endregion
}
