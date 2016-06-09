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
using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Core.ConditionalFormattingManager;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
namespace DevExpress.Xpf.Core.ConditionalFormattingManager {
	public class ContainViewModel : FormatEditorOwnerViewModel {
		public static Func<IDialogContext, ContainViewModel> Factory { get { return ViewModelSource.Factory((IDialogContext x) => new ContainViewModel(x)); } }
		internal bool IsContainCondition(ConditionEditUnit unit) {
			Func<ContainOperator, bool> predicate;
			ConditionRule rule = unit.ValueRule;
			if(rule == ConditionRule.Expression) {
				var op = CriteriaOperator.TryParse(unit.Expression);
				if(ReferenceEquals(op, null))
					return false;
				predicate = x => x.Match(op);
			}
			else predicate = x => x.Match(rule);
			return ViewModels.SelectMany(x => x.Operators).Any(predicate);
		}
		protected ContainViewModel(IDialogContext context)
			: base(context) {
			ViewModels = GetViewModels();
			SelectedViewModel = ViewModels.First();
		}
		public IEnumerable<ContainItemViewModel> ViewModels { get; private set; }
		public virtual ContainItemViewModel SelectedViewModel { get; set; }
		public bool ForceExpressionMode { get; set; }
		protected override void AddChanges(ConditionEditUnit unit) {
			base.AddChanges(unit);
			unit.FieldName = Context.ColumnInfo.FieldName;
			if(ForceExpressionMode || SelectedViewModel.Operator.Rule == ConditionRule.Expression) {
				unit.Expression = SelectedViewModel.Expression;
				unit.ValueRule = ConditionRule.Expression;
			}
			else {
				unit.ValueRule = SelectedViewModel.Operator.Rule;
				object[] values = SelectedViewModel.GetValues();
				unit.Value1 = GetValue(0, values);
				unit.Value2 = GetValue(1, values);
			}
		}
		object GetValue(int index, object[] values) {
			object actualValue = ManagerHelper.SafeGetValue(index, values);
			if(actualValue != null)
				actualValue = ManagerHelperBase.ConvertRuleValue(actualValue, Context.ColumnInfo.FieldType, this.IsInDesignMode());
			return actualValue;
		}
		protected override void InitCore(ConditionEditUnit unit) {
			base.InitCore(unit);
			Func<ContainOperator, bool> predicate;
			Action<ContainItemViewModel> initAction;
			ForceExpressionMode = unit.ValueRule == ConditionRule.Expression;
			if(ForceExpressionMode) {
				var op = CriteriaOperator.TryParse(unit.Expression);
				if(ReferenceEquals(op, null))
					return;
				predicate = x => x.Match(op);
				initAction = y => y.InitFromCriteria(op);
			}
			else {
				predicate = x => x.Match(unit.ValueRule);
				initAction = y => y.InitFromRule(unit);
			}
			ContainItemViewModel vm = ViewModels.FirstOrDefault(x => x.Operators.Any(predicate));
			if(vm != null) {
				SelectedViewModel = vm;
				initAction(SelectedViewModel);
			}
		}
		protected override bool CanInitCore(ConditionEditUnit unit) {
			return IsContainCondition(unit);
		}
		IEnumerable<ContainItemViewModel> GetViewModels() {
			return new ContainItemViewModel[] {
				ContainCellValueViewModel.Factory(Context),
				ContainTextViewModel.Factory(Context),
				ContainDateViewModel.Factory(Context),
				ContainBlanksViewModel.Factory(Context, true),
				ContainBlanksViewModel.Factory(Context, false)
			};
		}
		public override string Description { get { return GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_CellsContainDescription); } }
	}
	public abstract class ContainItemViewModel : ManagerViewModelBase {
		string expressionField;
		protected ContainItemViewModel(IDialogContext context) : base(context) { }
		public string Expression { get { return GetCriteria().ToString(); } }
		public virtual ContainOperator Operator { get; set; }
		public IEnumerable<ContainOperator> Operators { get; protected set; }
		protected static object ExtractOperandValue(CriteriaOperator op) {
			var operand = op as OperandValue;
			return ReferenceEquals(operand, null) ? null : operand.Value;
		}
		protected static string ExtractProperty(CriteriaOperator op) {
			var operand = op as OperandProperty;
			return ReferenceEquals(operand, null) ? null : operand.PropertyName;
		}
		public virtual void InitFromRule(ConditionEditUnit unit) {
			Operator = Operators.First(x => x.Match(unit.ValueRule));
			ProcessExtractedValues(new object[] { unit.FieldName, unit.Value1, unit.Value2 });
		}
		CriteriaOperator GetCriteria() {
			return Operator.Factory(new OperandProperty(string.IsNullOrEmpty(expressionField) ? Context.ColumnInfo.FieldName : expressionField), GetCriteriaValues());
		}
		public virtual void InitFromCriteria(CriteriaOperator op) {
			Operator = Operators.First(x => x.Match(op));
			ProcessExtractedValues(Operator.Extractor(op));
		}
		protected virtual void ProcessExtractedValues(object[] values) {
			expressionField = values[0] as string;
		}
		OperandValue[] GetCriteriaValues() {
			return GetValues().Select(x => new OperandValue(x)).ToArray();
		}
		internal protected virtual object[] GetValues() {
			return new object[0];
		}
		protected IDataColumnInfo GetExpressionColumn() {
			IDialogContext context = Context;
			if(!string.IsNullOrEmpty(expressionField))
				context = Context.Find(expressionField) ?? Context;
			return context.ColumnInfo;
		}
	}
	public class ContainDateViewModel : ContainItemViewModel {
		public static Func<IDialogContext, ContainDateViewModel> Factory { get { return ViewModelSource.Factory((IDialogContext x) => new ContainDateViewModel(x)); } }
		IEnumerable<ContainOperator> GetOperators() {
			return DateOccurringConditionalFormattingDialogViewModel.GetFactories().Select(x => new ContainOperator(x.ToString(), ConditionRule.Expression, (op, _) => x.Factory(op), ExtractDate)).ToArray();
		}
		static object[] ExtractDate(CriteriaOperator op) {
			FunctionOperator fop = null;
			if(op is FunctionOperator)
				fop = op as FunctionOperator;
			else if(op is BetweenOperator)
				fop = ((BetweenOperator)op).TestExpression as FunctionOperator;
			else if(op is BinaryOperator)
				fop = ((BinaryOperator)op).LeftOperand as FunctionOperator;
			if(!ReferenceEquals(fop, null)) {
				CriteriaOperator property = fop.Operands.FirstOrDefault(x => x is OperandProperty);
				if(!ReferenceEquals(property, null))
					return new object[] { ExtractProperty(property) };
			}
			return new object[] { null };
		}
		protected ContainDateViewModel(IDialogContext context)
			: base(context) {
			Operators = GetOperators();
			Operator = Operators.First();
		}
		public override string Description { get { return GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_DatesOccurring); } }
	}
	public class ContainBlanksViewModel : ContainItemViewModel {
		public static Func<IDialogContext, bool, ContainBlanksViewModel> Factory { get { return ViewModelSource.Factory((IDialogContext x, bool y) => new ContainBlanksViewModel(x, y)); } }
		ContainOperator emptyOp;
		ContainOperator emptyStringOp;
		IEnumerable<ContainOperator> GetOperators() {
			if(isBlank) {
				emptyOp = new ContainOperator(GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_Blanks), ConditionRule.Expression, (op, _) => op.IsNull(), ExtractUnary);
				emptyStringOp = new ContainOperator(GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_Blanks), ConditionRule.Expression, (op, _) => new FunctionOperator(FunctionOperatorType.IsNullOrEmpty, op), ExtractNullOrEmpty);
			}
			else {
				emptyOp = new ContainOperator(GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_NoBlanks), ConditionRule.Expression, (op, _) => op.IsNotNull(), ExtractNotNull);
				emptyStringOp = new ContainOperator(GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_NoBlanks), ConditionRule.Expression, (op, _) => new FunctionOperator(FunctionOperatorType.IsNullOrEmpty, op).Not(), ExtractNotNullOrEmpty);
			}
			return new ContainOperator[] { emptyOp, emptyStringOp };
		}
		static object[] ExtractUnary(CriteriaOperator op) {
			return new object[] { ExtractProperty(((UnaryOperator)op).Operand) };
		}
		static object[] ExtractNotNull(CriteriaOperator op) {
			return ExtractUnary(((UnaryOperator)op).Operand);
		}
		static object[] ExtractNullOrEmpty(CriteriaOperator op) {
			return new object[] { ExtractProperty(((FunctionOperator)op).Operands[0]) };
		}
		static object[] ExtractNotNullOrEmpty(CriteriaOperator op) {
			return ExtractNullOrEmpty(((UnaryOperator)op).Operand);
		}
		readonly bool isBlank;
		protected ContainBlanksViewModel(IDialogContext context, bool isBlank)
			: base(context) {
			this.isBlank = isBlank;
			Operators = GetOperators();
			Operator = Context.ColumnInfo.FieldType.Equals(typeof(string)) ? emptyStringOp : emptyOp;
		}
		public override string Description { get { return isBlank ? GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_Blanks) : GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_NoBlanks); } }
	}
	public class ContainTextViewModel : ContainItemViewModel {
		public static Func<IDialogContext, ContainTextViewModel> Factory { get { return ViewModelSource.Factory((IDialogContext x) => new ContainTextViewModel(x)); } }
		IEnumerable<ContainOperator> GetOperators() {
			return new ContainOperator[] {
				new ContainOperator(GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_Containing), ConditionRule.Expression, (op, ov) => new FunctionOperator(FunctionOperatorType.Contains, op, ov[0]), ExtractFunction),
				new ContainOperator(GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_NotContaining), ConditionRule.Expression, (op, ov) => new UnaryOperator(UnaryOperatorType.Not, new FunctionOperator(FunctionOperatorType.Contains, op, ov[0])), ExtractUnary),
				new ContainOperator(GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_BeginningWith), ConditionRule.Expression, (op, ov) => new FunctionOperator(FunctionOperatorType.StartsWith, op, ov[0]), ExtractFunction),
				new ContainOperator(GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_EndingWith), ConditionRule.Expression, (op, ov) => new FunctionOperator(FunctionOperatorType.EndsWith, op, ov[0]), ExtractFunction),
			};
		}
		protected ContainTextViewModel(IDialogContext column)
			: base(column) {
			Operators = GetOperators();
			Operator = Operators.First();
		}
		static object[] ExtractFunction(CriteriaOperator op) {
			FunctionOperator fop = (FunctionOperator)op;
			return new object[] { ExtractProperty(fop.Operands[0]), ExtractOperandValue(fop.Operands[1]) };
		}
		static object[] ExtractUnary(CriteriaOperator op) {
			return ExtractFunction(((UnaryOperator)op).Operand);
		}
		public virtual object Value { get; set; }
		internal protected override object[] GetValues() {
			return new object[] { Value };
		}
		protected override void ProcessExtractedValues(object[] values) {
			base.ProcessExtractedValues(values);
			Value = values[1];
		}
		public override string Description { get { return GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_SpecificText); } }
	}
	public class ContainCellValueViewModel : ContainItemViewModel, IConditionalFormattingDialogViewModel {
		public static Func<IDialogContext, ContainCellValueViewModel> Factory { get { return ViewModelSource.Factory((IDialogContext x) => new ContainCellValueViewModel(x)); } }
		ContainOperator betweenOperator;
		ContainOperator notBetweenOperator;
		IEnumerable<ContainOperator> GetOperators() {
			betweenOperator = new ContainOperator(GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_Between), ConditionRule.Between, ConditionEditUnit.GetExpressionFactory(ConditionRule.Between), ExtractBetween);
			notBetweenOperator = new ContainOperator(GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_NotBetween), ConditionRule.NotBetween, ConditionEditUnit.GetExpressionFactory(ConditionRule.NotBetween), ExtractNotBetween);
			return new ContainOperator[] {
				betweenOperator,
				notBetweenOperator,
				new ContainOperator(GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_EqualTo), ConditionRule.Equal, ConditionEditUnit.GetExpressionFactory(ConditionRule.Equal), ExtractBinary),
				new ContainOperator(GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_NotEqualTo), ConditionRule.NotEqual, ConditionEditUnit.GetExpressionFactory(ConditionRule.NotEqual), ExtractBinary),
				new ContainOperator(GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_GreaterThan), ConditionRule.Greater, ConditionEditUnit.GetExpressionFactory(ConditionRule.Greater), ExtractBinary),
				new ContainOperator(GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_LessThan), ConditionRule.Less, ConditionEditUnit.GetExpressionFactory(ConditionRule.Less), ExtractBinary),
				new ContainOperator(GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_GreaterOrEqual), ConditionRule.GreaterOrEqual, ConditionEditUnit.GetExpressionFactory(ConditionRule.GreaterOrEqual), ExtractBinary),
				new ContainOperator(GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_LessOrEqual), ConditionRule.LessOrEqual, ConditionEditUnit.GetExpressionFactory(ConditionRule.LessOrEqual), ExtractBinary),
			};
		}
		protected ContainCellValueViewModel(IDialogContext context)
			: base(context) {
			Operators = GetOperators();
			Operator = Operators.First();
		}
		static object[] ExtractBetween(CriteriaOperator op) {
			var bop = (BetweenOperator)op;
			return new object[] { ExtractProperty(bop.TestExpression), ExtractOperandValue(bop.BeginExpression), ExtractOperandValue(bop.EndExpression) };
		}
		static object[] ExtractNotBetween(CriteriaOperator op) {
			return ExtractBetween(((UnaryOperator)op).Operand);
		}
		static object[] ExtractBinary(CriteriaOperator op) {
			var bop = (BinaryOperator)op;
			return new object[] { ExtractProperty(bop.LeftOperand), ExtractOperandValue(bop.RightOperand), null };
		}
		public virtual object Value { get; set; }
		public virtual object Value2 { get; set; }
		public virtual bool IsBetween { get; protected set; }
		internal protected override object[] GetValues() {
			var values = new List<object>();
			values.Add(Value);
			if(IsBetween)
				values.Add(Value2);
			return values.ToArray();
		}
		protected void OnOperatorChanged() {
			IsBetween = Operator == betweenOperator || Operator == notBetweenOperator;
		}
		protected override void ProcessExtractedValues(object[] values) {
			base.ProcessExtractedValues(values);
			Value = values[1];
			Value2 = values[2];
		}
		public ConditionValueType ConditionValueType {
			get { return IsBetween ? GetRangeType() : GetSingleType(); }
		}
		ConditionValueType GetSingleType() {
			return ExpressionConditionalFormattingDialogViewModel.SelectValue(GetExpressionColumn().FieldType, () => ConditionValueType.SingleDateTime, () => ConditionValueType.SingleNumeric, () => ConditionValueType.SingleText);
		}
		ConditionValueType GetRangeType() {
			return ExpressionConditionalFormattingDialogViewModel.SelectValue(GetExpressionColumn().FieldType, () => ConditionValueType.RangeDateTime, () => ConditionValueType.RangeNumeric, () => ConditionValueType.RangeText);
		}
		public override string Description { get { return GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_CellValue); } }
	}
	public class ContainOperator : ContainOperatorBase<ConditionRule> {
		public ContainOperator(string name, ConditionRule rule, Func<OperandProperty, OperandValue[], CriteriaOperator> factory, Func<CriteriaOperator, object[]> extractor)
			: base(name, rule, factory, extractor) { }
		public virtual bool Match(ConditionRule rule) {
			return Rule == rule && rule != ConditionRule.Expression;
		}
	}
}
