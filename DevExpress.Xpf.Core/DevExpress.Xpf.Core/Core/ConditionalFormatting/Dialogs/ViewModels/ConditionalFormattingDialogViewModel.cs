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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Data;
using DevExpress.Data.ExpressionEditor;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Exceptions;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.UI;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Mvvm.UI.Native.ViewGenerator;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Filtering;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
using DevExpress.Xpf.Core.ConditionalFormatting.Printing;
using DevExpress.Xpf.Core.ConditionalFormattingManager;
namespace DevExpress.Xpf.Core.ConditionalFormatting {
	public abstract class ConditionalFormattingDialogViewModel : IConditionalFormattingDialogViewModel {
		public const string FormatServiceKey = "customFormatService";
		protected readonly IFormatsOwner owner;
		IList<FormatInfo> formats;
		Locker formatInfoLocker = new Locker();
		protected ConditionalFormattingDialogViewModel(IFormatsOwner owner, ConditionalFormattingStringId titleId, ConditionalFormattingStringId descriptionId, ConditionalFormattingStringId connectorId) {
			this.owner = owner;
			this.Title = ConditionalFormattingLocalizer.GetString(titleId);
			this.Description = ConditionalFormattingLocalizer.GetString(descriptionId);
			this.ConnectorText = ConditionalFormattingLocalizer.GetString(connectorId);
			this.formats = CreateFormats();
			this.SelectedFormatInfo = Formats.FirstOrDefault();
			this.ApplyFormatToWholeRowText = ConditionalFormattingLocalizer.GetString(ConditionalFormattingStringId.ConditionalFormatting_Dialog_ApplyFormatToWholeRowText);
			this.ApplyFormatToWholeRowEnabled = true;
		}
		public virtual IMessageBoxService MessageBox { get { return null; } }
		[ServiceProperty(Key = FormatServiceKey)]
		protected virtual IDialogService FormatService { get { return null; } }
		public string Title { get; private set; }
		public string Description { get; private set; }
		public string ConnectorText { get; set; }
		public string ApplyFormatToWholeRowText { get; set; }
		public bool ApplyFormatToWholeRowEnabled { get; set; }
		public IEnumerable<FormatInfo> Formats { get { return formats; } }
		public virtual FormatInfo SelectedFormatInfo { get; set; }
		public virtual object Value { get; set; }
		public bool ApplyFormatToWholeRow { get; set; }
		public abstract ConditionValueType ConditionValueType { get; }
		public virtual IModelItem CreateCondition(IEditingContext context, string fieldName) {
			BaseEditUnit unit = CreateEditUnit(fieldName);
			return unit.BuildCondition(Context.Builder);
		}
		protected abstract BaseEditUnit CreateEditUnit(string fieldName);
		protected IDialogContext Context { get; private set; }
		public virtual void Initialize(IDialogContext context) {
			Context = context;
			Value = GetInitialValue();
		}
		internal abstract object GetInitialValue();
		public virtual bool TryClose() { return true; }
		protected void OnSelectedFormatInfoChanged(FormatInfo oldValue) {
			if(SelectedFormatInfo != null && SelectedFormatInfo.IsCustom && !formatInfoLocker)
				SetCustomFormatInfo(oldValue);
		}
		public void SetFormatProperty(IModelItem condition) {
			var unit = new ConditionEditUnit();
			if(SelectedFormatInfo.IsCustom)
				unit.Format = SelectedFormatInfo.Format as Format;
			else unit.PredefinedFormatName = SelectedFormatInfo.FormatName;
			unit.BuildCondition(Context.Builder, condition);
		}
		void SetCustomFormatInfo(FormatInfo oldValue) {
			var formatVM = DevExpress.Xpf.Core.ConditionalFormattingManager.FormatEditorViewModel.Factory(Context);
			FormatInfo customInfo = oldValue;
			if(DevExpress.Xpf.Core.ConditionalFormattingManager.ManagerHelperBase.ShowDialog(formatVM, ConditionalFormattingLocalizer.GetString(ConditionalFormattingStringId.ConditionalFormatting_Manager_CustomFormat), FormatService)) {
				customInfo = (FormatInfo)SelectedFormatInfo.Clone();
				customInfo.Format = formatVM.CreateFormat();
				customInfo.Freeze();
				formats.Remove(SelectedFormatInfo);
				formats.Add(customInfo);
			}
			formatInfoLocker.DoLockedAction(() => SelectedFormatInfo = customInfo);
		}
		IList<FormatInfo> CreateFormats() {
			var formats = new List<FormatInfo>(owner.PredefinedFormats);
			formats.Add(new FormatInfo { DisplayName = ConditionalFormattingLocalizer.GetString(ConditionalFormattingStringId.ConditionalFormatting_Manager_CustomFormat) });
			return formats;
		}
	}
	public class DesignTimeConditionalFormattingDialogViewModel : IConditionalFormattingDialogViewModel {
		public string Description { get; set; }
		public string ConnectorText { get; set; }
		public IEnumerable<FormatInfo> Formats { get { return null; } }
		public FormatInfo SelectedFormatInfo { get; set; }
		public object Value { get; set; }
		public ConditionValueType ConditionValueType { get; set; }
	}
	public enum ConditionValueType {
		SingleText,
		SingleNumeric,
		SingleDateTime,
		SingleWithElipsisButton,
		RangeNumeric,
		RangeDateTime,
		RangeText,
		ItemCount,
		Percent,
		Selector,
		None,
	}
	public class FormatConditionValueEditorTemplateSelector : DataTemplateSelector {
		public override DataTemplate SelectTemplate(object item, DependencyObject container) {
			var viewModel = (IConditionalFormattingDialogViewModel)item;
			return viewModel.With(x => (DataTemplate)((FrameworkElement)container).FindResource(x.ConditionValueType + "ValueEditor"));
		}
	}
	public class InplaceBaseEditHelper {
		public static BaseEditSettings GetEditSettings(InplaceBaseEdit obj) {
			return (BaseEditSettings)obj.GetValue(EditSettingsProperty);
		}
		public static void SetEditSettings(InplaceBaseEdit obj, BaseEditSettings value) {
			obj.SetValue(EditSettingsProperty, value);
		}
		public static readonly DependencyProperty EditSettingsProperty =
			DependencyProperty.RegisterAttached("EditSettings", typeof(BaseEditSettings), typeof(InplaceBaseEditHelper), new PropertyMetadata(null, OnEditSettingsChanged));
		static void OnEditSettingsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((InplaceBaseEdit)d).SetSettings((BaseEditSettings)e.NewValue);
		}
	}
	public static class BaseEditFocusHelper {
		public static bool GetFocusOnLoad(BaseEdit obj) {
			return (bool)obj.GetValue(FocusOnLoadProperty);
		}
		public static void SetFocusOnLoad(BaseEdit obj, bool value) {
			obj.SetValue(FocusOnLoadProperty, value);
		}
		public static readonly DependencyProperty FocusOnLoadProperty =
			DependencyProperty.RegisterAttached("FocusOnLoad", typeof(bool), typeof(BaseEditFocusHelper), new PropertyMetadata(false, OnFocusOnLoadChanged));
		static void OnFocusOnLoadChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if((bool)e.NewValue)
				Interaction.GetBehaviors(d).Add(new FocusBehavior());
		}
	}
	public static class ConditionalFormattingMenuHelper {
		public static bool ShowColorScaleMenu(Type type) {
			return ShowIndicatorMenu(type, IndicatorFormatBase.IsNumericOrDateTimeTypeCode);
		}
		public static bool ShowDatBarMenu(Type type) {
			return ShowIndicatorMenu(type, IndicatorFormatBase.IsNumericTypeCode);
		}
		public static bool ShowIconSetMenu(Type type) {
			return ShowIndicatorMenu(type, IndicatorFormatBase.IsNumericOrDateTimeTypeCode);
		}
		static bool ShowIndicatorMenu(Type type, Func<TypeCode, bool> typeCodeCheck) {
			type = GetDataType(type);
			TypeCode code = Type.GetTypeCode(type);
			return typeCodeCheck(code);
		}
		public static IEnumerable<FormatConditionDialogType> GetAvailableTopBottomRuleItems(Type type, bool isServerMode) {
			type = GetDataType(type);
			if(!typeof(IComparable).IsAssignableFrom(type))
				yield break;
			TypeCode code = Type.GetTypeCode(type);
			if(!isServerMode) {
				yield return FormatConditionDialogType.Top10Items;
				yield return FormatConditionDialogType.Top10Percent;
				yield return FormatConditionDialogType.Bottom10Items;
				yield return FormatConditionDialogType.Bottom10Percent;
			}
			if(IndicatorFormatBase.IsNumericTypeCode(code) || (code == TypeCode.DateTime && !isServerMode)) {
				yield return FormatConditionDialogType.AboveAverage;
				yield return FormatConditionDialogType.BelowAverage;
			}
		}
		public static IEnumerable<FormatConditionDialogType> GetAvailableHighlightItems(Type type) {
			type = GetDataType(type);
			yield return FormatConditionDialogType.GreaterThan;
			yield return FormatConditionDialogType.LessThan;
			yield return FormatConditionDialogType.Between;
			yield return FormatConditionDialogType.EqualTo;
			yield return FormatConditionDialogType.TextThatContains;
			if(type == typeof(DateTime))
				yield return FormatConditionDialogType.ADateOccurring;
			yield return FormatConditionDialogType.CustomCondition;
		}
		static Type GetDataType(Type type) {
			return Nullable.GetUnderlyingType(type) ?? type;
		}
	}
	public static class ConditionalFormattingDialogHelper {
		public static bool ValidateExpression(string expression, IMessageBoxService service, IDataColumnInfo columnInfo) {
			try {
				CriteriaOperator.Parse(expression, null);
			}
			catch(CriteriaParserException e) {
				service.Show(
					string.Format(ConditionalFormattingLocalizer.GetString(ConditionalFormattingStringId.ConditionalFormatting_CustomConditionDialog_InvalidExpressionMessage), e.Column),
					ConditionalFormattingLocalizer.GetString(ConditionalFormattingStringId.ConditionalFormatting_CustomConditionDialog_InvalidExpressionMessageTitle));
				return false;
			}
			try {
				UnboundExpressionConvertHelper.ValidateExpressionFields(columnInfo, expression);
			}
			catch(Exception) {
				return ShowInvalidExpressionError(service);
			}
			return true;
		}
		public static bool ShowInvalidExpressionError(IMessageBoxService service) {
			service.Show(
				ConditionalFormattingLocalizer.GetString(ConditionalFormattingStringId.ConditionalFormatting_CustomConditionDialog_InvalidExpressionMessageEx),
				ConditionalFormattingLocalizer.GetString(ConditionalFormattingStringId.ConditionalFormatting_CustomConditionDialog_InvalidExpressionMessageTitle));
			return false;
		}
	}
	public enum FormatConditionDialogType {
		GreaterThan,
		LessThan,
		Between,
		EqualTo,
		TextThatContains,
		ADateOccurring,
		CustomCondition,
		Top10Items,
		Bottom10Items,
		Top10Percent,
		Bottom10Percent,
		AboveAverage,
		BelowAverage,
	}
}
namespace DevExpress.Xpf.Core.ConditionalFormatting {
	public interface IConditionalFormattingDialogViewModel {
		ConditionValueType ConditionValueType { get; }
	}
	#region criteria
	public abstract class ExpressionConditionalFormattingDialogViewModel : ConditionalFormattingDialogViewModel {
		protected ExpressionConditionalFormattingDialogViewModel(IFormatsOwner owner, ConditionalFormattingStringId titleId, ConditionalFormattingStringId descriptionId, ConditionalFormattingStringId connectorId)
			: base(owner, titleId, descriptionId, connectorId) {
		}
		public static string CriteriaOperatorToString(CriteriaOperator criteria) {
			return CriteriaOperator.ToString(criteria);
		}
		public static T SelectValue<T>(Type type, Func<T> dateTime, Func<T> numeric, Func<T> text) {
			type = Nullable.GetUnderlyingType(type) ?? type;
			if(type == typeof(DateTime))
				return dateTime();
			if(EditorsSource.NumericIntegerTypes.Concat(EditorsSource.NumericFloatTypes).Contains(type))
				return numeric();
			return text();
		}
		public override ConditionValueType ConditionValueType {
			get { return SelectValue(() => ConditionValueType.SingleDateTime, () => ConditionValueType.SingleNumeric, () => ConditionValueType.SingleText); }
		}
		protected override BaseEditUnit CreateEditUnit(string fieldName) {
			var unit = new ConditionEditUnit();
			if(ApplyFormatToWholeRow)
				unit.ApplyToRow = true;
			ConditionRule rule = GetValueRule();
			if(rule != ConditionRule.Expression)
				unit.ValueRule = rule;
			else
				unit.Expression = GetExpression(fieldName);
			if(Value != null)
				unit.Value1 = ConvertRuleValue(Value);
			return unit;
		}
		protected object ConvertRuleValue(object value) {
			return ManagerHelperBase.ConvertRuleValue(value, Context.ColumnInfo.FieldType, this.IsInDesignMode());
		}
		public string GetExpression(string fieldName) {
			return GetCriteriaString(fieldName) ?? CriteriaOperatorToString(GetCriteria(fieldName));
		}
		protected abstract CriteriaOperator GetCriteria(string fieldName);
		protected virtual string GetCriteriaString(string fieldName) {
			return null;
		}
		internal override object GetInitialValue() {
			return SelectValue<object>(() => DateTime.Today, () => (decimal)0, () => null);
		}
		protected T SelectValue<T>(Func<T> dateTime, Func<T> numeric, Func<T> text) {
			return SelectValue(Context.ColumnInfo.FieldType, dateTime, numeric, text);
		}
		internal virtual ConditionRule GetValueRule() {
			return ConditionRule.Expression;
		}
	}
	public class GreaterThanConditionalFormattingDialogViewModel : ExpressionConditionalFormattingDialogViewModel {
		public static Func<IFormatsOwner, ConditionalFormattingDialogViewModel> Factory { get { return ViewModelSource.Factory((IFormatsOwner x) => new GreaterThanConditionalFormattingDialogViewModel(x)); } }
		protected GreaterThanConditionalFormattingDialogViewModel(IFormatsOwner owner)
			: base(owner, ConditionalFormattingStringId.ConditionalFormatting_GreaterThanDialog_Title, ConditionalFormattingStringId.ConditionalFormatting_GreaterThanDialog_Description, ConditionalFormattingStringId.ConditionalFormatting_GreaterThanDialog_Connector) {
		}
		protected override CriteriaOperator GetCriteria(string fieldName) {
			return GetCriteria(fieldName, Value);
		}
		public static CriteriaOperator GetCriteria(string fieldName, object value) {
			return new BinaryOperator(fieldName, value, BinaryOperatorType.Greater);
		}
		internal override ConditionRule GetValueRule() {
			return ConditionRule.Greater;
		}
	}
	public class LessThanConditionalFormattingDialogViewModel : ExpressionConditionalFormattingDialogViewModel {
		public static Func<IFormatsOwner, ConditionalFormattingDialogViewModel> Factory { get { return ViewModelSource.Factory((IFormatsOwner x) => new LessThanConditionalFormattingDialogViewModel(x)); } }
		protected LessThanConditionalFormattingDialogViewModel(IFormatsOwner owner)
			: base(owner, ConditionalFormattingStringId.ConditionalFormatting_LessThanDialog_Title, ConditionalFormattingStringId.ConditionalFormatting_LessThanDialog_Description, ConditionalFormattingStringId.ConditionalFormatting_LessThanDialog_Connector) {
		}
		protected override CriteriaOperator GetCriteria(string fieldName) {
			return new BinaryOperator(fieldName, Value, BinaryOperatorType.Less);
		}
		internal override ConditionRule GetValueRule() {
			return ConditionRule.Less;
		}
	}
	public class BetweenConditionalFormattingDialogViewModel : ExpressionConditionalFormattingDialogViewModel {
		public static Func<IFormatsOwner, ConditionalFormattingDialogViewModel> Factory { get { return ViewModelSource.Factory((IFormatsOwner x) => new BetweenConditionalFormattingDialogViewModel(x)); } }
		protected BetweenConditionalFormattingDialogViewModel(IFormatsOwner owner)
			: base(owner, ConditionalFormattingStringId.ConditionalFormatting_BetweenDialog_Title, ConditionalFormattingStringId.ConditionalFormatting_BetweenDialog_Description, ConditionalFormattingStringId.ConditionalFormatting_BetweenDialog_Connector) {
		}
		public object Value2 { get; set; }
		public override ConditionValueType ConditionValueType {
			get { return SelectValue(() => ConditionValueType.RangeDateTime, () => ConditionValueType.RangeNumeric, () => ConditionValueType.RangeText); }
		}
		protected override CriteriaOperator GetCriteria(string fieldName) {
			return new BetweenOperator(fieldName, Value, Value2);
		}
		public override void Initialize(IDialogContext context) {
			base.Initialize(context);
			Value2 = GetInitialValue();
		}
		protected override BaseEditUnit CreateEditUnit(string fieldName) {
			ConditionEditUnit unit = base.CreateEditUnit(fieldName) as ConditionEditUnit;
			if(unit != null && Value2 != null)
				unit.Value2 = ConvertRuleValue(Value2);
			return unit;
		}
		internal override ConditionRule GetValueRule() {
			return ConditionRule.Between;
		}
	}
	public class EqualToConditionalFormattingDialogViewModel : ExpressionConditionalFormattingDialogViewModel {
		public static Func<IFormatsOwner, ConditionalFormattingDialogViewModel> Factory { get { return ViewModelSource.Factory((IFormatsOwner x) => new EqualToConditionalFormattingDialogViewModel(x)); } }
		protected EqualToConditionalFormattingDialogViewModel(IFormatsOwner owner)
			: base(owner, ConditionalFormattingStringId.ConditionalFormatting_EqualToDialog_Title, ConditionalFormattingStringId.ConditionalFormatting_EqualToDialog_Description, ConditionalFormattingStringId.ConditionalFormatting_EqualToDialog_Connector) {
		}
		protected override CriteriaOperator GetCriteria(string fieldName) {
			return GetCriteria(fieldName, Value);
		}
		public static CriteriaOperator GetCriteria(string fieldName, object value) {
			return new BinaryOperator(fieldName, value, BinaryOperatorType.Equal);
		}
		internal override ConditionRule GetValueRule() {
			return ConditionRule.Equal;
		}
	}
	public class TextThatContainsConditionalFormattingDialogViewModel : ExpressionConditionalFormattingDialogViewModel {
		public static Func<IFormatsOwner, ConditionalFormattingDialogViewModel> Factory { get { return ViewModelSource.Factory((IFormatsOwner x) => new TextThatContainsConditionalFormattingDialogViewModel(x)); } }
		protected TextThatContainsConditionalFormattingDialogViewModel(IFormatsOwner owner)
			: base(owner, ConditionalFormattingStringId.ConditionalFormatting_TextThatContainsDialog_Title, ConditionalFormattingStringId.ConditionalFormatting_TextThatContainsDialog_Description, ConditionalFormattingStringId.ConditionalFormatting_TextThatContainsDialog_Connector) {
		}
		public override ConditionValueType ConditionValueType { get { return ConditionValueType.SingleText; } }
		protected override CriteriaOperator GetCriteria(string fieldName) {
			return new FunctionOperator(FunctionOperatorType.Contains, new OperandProperty(fieldName), new OperandValue(Value));
		}
		internal override object GetInitialValue() {
			return null;
		}
	}
	public class DateOccurringConditionalFormattingDialogViewModel : ExpressionConditionalFormattingDialogViewModel {
		public class OperatorFactory {
			readonly ConditionalFormattingStringId stringId;
			public Func<OperandProperty, CriteriaOperator> Factory { get; private set; }
			internal DateOccurringConditionRule Rule { get; private set; }
			public OperatorFactory(ConditionalFormattingStringId stringId, Func<OperandProperty, CriteriaOperator> factory, DateOccurringConditionRule rule = DateOccurringConditionRule.None) {
				this.stringId = stringId;
				this.Factory = factory;
				this.Rule = rule;
			}
			public override string ToString() {
				return ConditionalFormattingLocalizer.GetString(stringId);
			}
		}
		static readonly FunctionOperator TodayOperator = new FunctionOperator(FunctionOperatorType.LocalDateTimeToday);
		public static IEnumerable<OperatorFactory> GetFactories() {
			yield return new OperatorFactory(ConditionalFormattingStringId.ConditionalFormatting_DateOccurringDialog_IntervalYesterday, x => new FunctionOperator(FunctionOperatorType.IsOutlookIntervalYesterday, x), DateOccurringConditionRule.Yesterday);
			yield return new OperatorFactory(ConditionalFormattingStringId.ConditionalFormatting_DateOccurringDialog_IntervalToday, x => new FunctionOperator(FunctionOperatorType.IsOutlookIntervalToday, x), DateOccurringConditionRule.Today);
			yield return new OperatorFactory(ConditionalFormattingStringId.ConditionalFormatting_DateOccurringDialog_IntervalTomorrow, x => new FunctionOperator(FunctionOperatorType.IsOutlookIntervalTomorrow, x), DateOccurringConditionRule.Tomorrow);
			yield return new OperatorFactory(ConditionalFormattingStringId.ConditionalFormatting_DateOccurringDialog_IntervalInTheLast7Days, x => new BetweenOperator(new FunctionOperator(FunctionOperatorType.DateDiffDay, x, TodayOperator), new ConstantValue(0), new ConstantValue(6)), DateOccurringConditionRule.InTheLast7Days);
			yield return new OperatorFactory(ConditionalFormattingStringId.ConditionalFormatting_DateOccurringDialog_IntervalLastWeek, x => new FunctionOperator(FunctionOperatorType.IsOutlookIntervalLastWeek, x), DateOccurringConditionRule.LastWeek);
			yield return new OperatorFactory(ConditionalFormattingStringId.ConditionalFormatting_DateOccurringDialog_IntervalThisWeek, x => new FunctionOperator(FunctionOperatorType.IsThisWeek, x), DateOccurringConditionRule.ThisWeek);
			yield return new OperatorFactory(ConditionalFormattingStringId.ConditionalFormatting_DateOccurringDialog_IntervalNextWeek, x => new FunctionOperator(FunctionOperatorType.IsOutlookIntervalNextWeek, x), DateOccurringConditionRule.NextWeek);
			yield return new OperatorFactory(ConditionalFormattingStringId.ConditionalFormatting_DateOccurringDialog_IntervalLastMonth, x => new BinaryOperator(new FunctionOperator(FunctionOperatorType.DateDiffMonth, x, TodayOperator), new ConstantValue(1), BinaryOperatorType.Equal), DateOccurringConditionRule.LastMonth);
			yield return new OperatorFactory(ConditionalFormattingStringId.ConditionalFormatting_DateOccurringDialog_IntervalThisMonth, x => new FunctionOperator(FunctionOperatorType.IsThisMonth, x), DateOccurringConditionRule.ThisMonth);
			yield return new OperatorFactory(ConditionalFormattingStringId.ConditionalFormatting_DateOccurringDialog_IntervalNextMonth, x => new BinaryOperator(new FunctionOperator(FunctionOperatorType.DateDiffMonth, TodayOperator, x), new ConstantValue(1), BinaryOperatorType.Equal), DateOccurringConditionRule.NextMonth);
		}
		public static CriteriaOperator GetCriteria(OperatorFactory factory, string fieldName) {
			return factory.Factory(new OperandProperty(fieldName));
		}
		public OperatorFactory[] SelectorItems { get; private set; }
		public static Func<IFormatsOwner, ConditionalFormattingDialogViewModel> Factory { get { return ViewModelSource.Factory((IFormatsOwner x) => new DateOccurringConditionalFormattingDialogViewModel(x)); } }
		protected DateOccurringConditionalFormattingDialogViewModel(IFormatsOwner owner)
			: base(owner, ConditionalFormattingStringId.ConditionalFormatting_DateOccurringDialog_Title, ConditionalFormattingStringId.ConditionalFormatting_DateOccurringDialog_Description, ConditionalFormattingStringId.ConditionalFormatting_DateOccurringDialog_Connector) {
			SelectorItems = GetFactories().ToArray();
		}
		public override ConditionValueType ConditionValueType { get { return ConditionValueType.Selector; } }
		protected override CriteriaOperator GetCriteria(string fieldName) {
			return GetCriteria((OperatorFactory)Value, fieldName);
		}
		internal override object GetInitialValue() {
			return SelectorItems[0];
		}
	}
	public class CustomConditionConditionalFormattingDialogViewModel : ExpressionConditionalFormattingDialogViewModel {
		public const string ConditionServiceKey = "customConditionService";
		public static Func<IFormatsOwner, CustomConditionConditionalFormattingDialogViewModel> Factory { get { return ViewModelSource.Factory((IFormatsOwner x) => new CustomConditionConditionalFormattingDialogViewModel(x)); } }
		protected CustomConditionConditionalFormattingDialogViewModel(IFormatsOwner owner)
			: base(owner, ConditionalFormattingStringId.ConditionalFormatting_CustomConditionDialog_Title, ConditionalFormattingStringId.ConditionalFormatting_CustomConditionDialog_Description, ConditionalFormattingStringId.ConditionalFormatting_CustomConditionDialog_Connector) {
		}
		public override ConditionValueType ConditionValueType { get { return ConditionValueType.SingleWithElipsisButton; } }
		protected override string GetCriteriaString(string fieldName) {
			return GetCriteriaStringCore();
		}
		protected override CriteriaOperator GetCriteria(string fieldName) {
			throw new InvalidOperationException();
		}
		[ServiceProperty(Key = ConditionServiceKey)]
		protected virtual IDialogService DialogService { get { return null; } }
		IDataColumnInfo DataColumnInfo { get { return Context.ColumnInfo; } }
		public class CustomConditionEditorViewModel {
			public CustomConditionEditorViewModel(FilterColumn defaultFilterColumn, IFilteredComponent filteredComponent) {
				this.DefaultFilterColumn = defaultFilterColumn;
				this.FilteredComponent = filteredComponent;
			}
			public CriteriaOperator Criteria { get; set; }
			public FilterColumn DefaultFilterColumn { get; private set; }
			public IFilteredComponent FilteredComponent { get; private set; }
		}
		public void ShowConditionEditor() {
			var viewModel = new CustomConditionEditorViewModel(Context.FilterColumn, Context.FilteredComponent) {
				Criteria = GetCiteriaForFilterEditor()
			};
			var dialogResult = DialogService.ShowDialog(MessageButton.OKCancel, ConditionalFormattingLocalizer.GetString(ConditionalFormattingStringId.ConditionalFormatting_CustomConditionEditor_Title), viewModel);
			if(dialogResult == MessageResult.OK) {
				Value = GetDisplayExpression(viewModel.Criteria);
			}
		}
		CriteriaOperator GetCiteriaForFilterEditor() {
			try {
				return CriteriaOperator.Parse(GetCriteriaStringCore());
			} catch (CriteriaParserException) {
				return null;
			}
		}
		internal override object GetInitialValue() {
			return GetDisplayExpression(new BinaryOperator(Context.ColumnInfo.FieldName, (object)null, BinaryOperatorType.Equal));
		}
		string GetDisplayExpression(CriteriaOperator criteria) {
			return criteria.Return(x => UnboundExpressionConvertHelper.ConvertToCaption(DataColumnInfo, x.ToString()), () => string.Empty);
		}
		string GetCriteriaStringCore() {
			return UnboundExpressionConvertHelper.ConvertToFields(DataColumnInfo, (string)Value ?? string.Empty);
		}
		public override bool TryClose() {
			string expression = GetCriteriaStringCore();
			if(string.IsNullOrEmpty(expression))
				return ConditionalFormattingDialogHelper.ShowInvalidExpressionError(MessageBox);
			return ConditionalFormattingDialogHelper.ValidateExpression(expression, MessageBox, DataColumnInfo) && base.TryClose();
		}
	}
	#endregion
	#region top/bottom
	public abstract class TopBottomConditionalFormattingDialogViewModel : ConditionalFormattingDialogViewModel {
		protected TopBottomConditionalFormattingDialogViewModel(IFormatsOwner owner, ConditionalFormattingStringId titleId, ConditionalFormattingStringId descriptionId, ConditionalFormattingStringId connectorId)
			: base(owner, titleId, descriptionId, connectorId) {
		}
		protected override BaseEditUnit CreateEditUnit(string fieldName) {
			var unit = new TopBottomEditUnit();
			unit.Rule = RuleKind;
			unit.Threshold = Convert.ToDouble(Value);
			if(ApplyFormatToWholeRow)
				unit.ApplyToRow = true;
			return unit;
		}
		protected abstract DevExpress.Xpf.Core.ConditionalFormatting.TopBottomRule RuleKind { get; }
		internal override object GetInitialValue() {
			return (decimal)10;
		}
	}
	public class AboveAverageConditionalFormattingDialogViewModel : TopBottomConditionalFormattingDialogViewModel {
		public static Func<IFormatsOwner, ConditionalFormattingDialogViewModel> Factory { get { return ViewModelSource.Factory((IFormatsOwner x) => new AboveAverageConditionalFormattingDialogViewModel(x)); } }
		protected AboveAverageConditionalFormattingDialogViewModel(IFormatsOwner owner)
			: base(owner, ConditionalFormattingStringId.ConditionalFormatting_AboveAverageDialog_Title, ConditionalFormattingStringId.ConditionalFormatting_AboveAverageDialog_Description, ConditionalFormattingStringId.ConditionalFormatting_AboveAverageDialog_Connector) {
		}
		public override ConditionValueType ConditionValueType { get { return ConditionValueType.None; } }
		protected override DevExpress.Xpf.Core.ConditionalFormatting.TopBottomRule RuleKind { get { return DevExpress.Xpf.Core.ConditionalFormatting.TopBottomRule.AboveAverage; } }
	}
	public class BelowAverageConditionalFormattingDialogViewModel : TopBottomConditionalFormattingDialogViewModel {
		public static Func<IFormatsOwner, ConditionalFormattingDialogViewModel> Factory { get { return ViewModelSource.Factory((IFormatsOwner x) => new BelowAverageConditionalFormattingDialogViewModel(x)); } }
		protected BelowAverageConditionalFormattingDialogViewModel(IFormatsOwner owner)
			: base(owner, ConditionalFormattingStringId.ConditionalFormatting_BelowAverageDialog_Title, ConditionalFormattingStringId.ConditionalFormatting_BelowAverageDialog_Description, ConditionalFormattingStringId.ConditionalFormatting_BelowAverageDialog_Connector) {
		}
		public override ConditionValueType ConditionValueType { get { return ConditionValueType.None; } }
		protected override DevExpress.Xpf.Core.ConditionalFormatting.TopBottomRule RuleKind { get { return DevExpress.Xpf.Core.ConditionalFormatting.TopBottomRule.BelowAverage; } }
	}
	public class Top10ItemsConditionalFormattingDialogViewModel : TopBottomConditionalFormattingDialogViewModel {
		public static Func<IFormatsOwner, ConditionalFormattingDialogViewModel> Factory { get { return ViewModelSource.Factory((IFormatsOwner x) => new Top10ItemsConditionalFormattingDialogViewModel(x)); } }
		protected Top10ItemsConditionalFormattingDialogViewModel(IFormatsOwner owner)
			: base(owner, ConditionalFormattingStringId.ConditionalFormatting_Top10ItemsDialog_Title, ConditionalFormattingStringId.ConditionalFormatting_Top10ItemsDialog_Description, ConditionalFormattingStringId.ConditionalFormatting_Top10ItemsDialog_Connector) {
		}
		public override ConditionValueType ConditionValueType { get { return ConditionValueType.ItemCount; } }
		protected override DevExpress.Xpf.Core.ConditionalFormatting.TopBottomRule RuleKind { get { return DevExpress.Xpf.Core.ConditionalFormatting.TopBottomRule.TopItems; } }
	}
	public class Bottom10ItemsConditionalFormattingDialogViewModel : TopBottomConditionalFormattingDialogViewModel {
		public static Func<IFormatsOwner, ConditionalFormattingDialogViewModel> Factory { get { return ViewModelSource.Factory((IFormatsOwner x) => new Bottom10ItemsConditionalFormattingDialogViewModel(x)); } }
		protected Bottom10ItemsConditionalFormattingDialogViewModel(IFormatsOwner owner)
			: base(owner, ConditionalFormattingStringId.ConditionalFormatting_Bottom10ItemsDialog_Title, ConditionalFormattingStringId.ConditionalFormatting_Bottom10ItemsDialog_Description, ConditionalFormattingStringId.ConditionalFormatting_Bottom10ItemsDialog_Connector) {
		}
		public override ConditionValueType ConditionValueType { get { return ConditionValueType.ItemCount; } }
		protected override DevExpress.Xpf.Core.ConditionalFormatting.TopBottomRule RuleKind { get { return DevExpress.Xpf.Core.ConditionalFormatting.TopBottomRule.BottomItems; } }
	}
	public class Top10PercentConditionalFormattingDialogViewModel : TopBottomConditionalFormattingDialogViewModel {
		public static Func<IFormatsOwner, ConditionalFormattingDialogViewModel> Factory { get { return ViewModelSource.Factory((IFormatsOwner x) => new Top10PercentConditionalFormattingDialogViewModel(x)); } }
		protected Top10PercentConditionalFormattingDialogViewModel(IFormatsOwner owner)
			: base(owner, ConditionalFormattingStringId.ConditionalFormatting_Top10PercentDialog_Title, ConditionalFormattingStringId.ConditionalFormatting_Top10PercentDialog_Description, ConditionalFormattingStringId.ConditionalFormatting_Top10PercentDialog_Connector) {
		}
		public override ConditionValueType ConditionValueType { get { return ConditionValueType.Percent; } }
		protected override DevExpress.Xpf.Core.ConditionalFormatting.TopBottomRule RuleKind { get { return DevExpress.Xpf.Core.ConditionalFormatting.TopBottomRule.TopPercent; } }
	}
	public class Bottom10PercentConditionalFormattingDialogViewModel : TopBottomConditionalFormattingDialogViewModel {
		public static Func<IFormatsOwner, ConditionalFormattingDialogViewModel> Factory { get { return ViewModelSource.Factory((IFormatsOwner x) => new Bottom10PercentConditionalFormattingDialogViewModel(x)); } }
		protected Bottom10PercentConditionalFormattingDialogViewModel(IFormatsOwner owner)
			: base(owner, ConditionalFormattingStringId.ConditionalFormatting_Bottom10PercentDialog_Title, ConditionalFormattingStringId.ConditionalFormatting_Bottom10PercentDialog_Description, ConditionalFormattingStringId.ConditionalFormatting_Bottom10PercentDialog_Connector) {
		}
		public override ConditionValueType ConditionValueType { get { return ConditionValueType.Percent; } }
		protected override DevExpress.Xpf.Core.ConditionalFormatting.TopBottomRule RuleKind { get { return DevExpress.Xpf.Core.ConditionalFormatting.TopBottomRule.BottomPercent; } }
	}
	#endregion
}
