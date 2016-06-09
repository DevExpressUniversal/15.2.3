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
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;
using DevExpress.Data;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using DevExpress.Xpf.Core;
using System.Windows.Markup;
using System.Windows.Media.Imaging;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
using DevExpress.Data.Filtering;
using System.Linq.Expressions;
namespace DevExpress.Xpf.Core.ConditionalFormattingManager {
	public abstract class ManagerViewModelBase {
		protected IDialogContext Context { get; private set; }
		public abstract string Description { get; }
		public ManagerViewModelBase(IDialogContext context) {
			Context = context;
		}
		protected string GetLocalizedString(ConditionalFormattingStringId id) {
			return ConditionalFormattingLocalizer.GetString(id);
		}
	}
	public class FormatEditorViewModel : ManagerViewModelBase {
		public static Func<IDialogContext, FormatEditorViewModel> Factory { get { return ViewModelSource.Factory((IDialogContext x) => new FormatEditorViewModel(x)); } }
		protected FormatEditorViewModel(IDialogContext context)
			: base(context) {
				ViewModels = new FormatEditorItemViewModel[] { FormatEditorFontViewModel.Factory(context), FormatEditorFillViewModel.Factory(context), FormatEditorIconViewModel.Factory(context) };
		}
		public IEnumerable<FormatEditorItemViewModel> ViewModels { get; private set; }
		[Command(isCommand: false)]
		public Format CreateFormat() {
			var format = new Format();
			bool hasChanged = false;
			foreach(var vm in ViewModels) {
				if(vm.HasChanged) {
					hasChanged = true;
					vm.SetFormatProperties(format);
				}
			}
			return hasChanged ? format : null;
		}
		[Command(isCommand: false)]
		public void Init(Format format) {
			if(format == null)
				return;
			foreach(var vm in ViewModels)
				vm.InitFromFormat(format);
		}
		public override string Description { get { return GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_FormatCells); } }
	}
	public abstract class FormatEditorItemViewModel : ManagerViewModelBase {
		protected FormatEditorItemViewModel(IDialogContext column) : base(column) { }
		public bool HasChanged { get; set; }
		protected SolidColorBrush CreateBrush(Color? color) {
			return color.HasValue && color.Value != Colors.Transparent ? new SolidColorBrush(color.Value) : null;
		}
		protected Color? GetBrushColor(Brush brush) {
			var solidBrush = brush as SolidColorBrush;
			if(solidBrush == null)
				return null;
			return solidBrush.Color;
		}
		protected void OnChanged() {
			HasChanged = true;
		}
		public abstract void Clear();
		public abstract void InitFromFormat(Format format);
		public abstract void SetFormatProperties(Format format);
	}
	public class FormatEditorFontViewModel : FormatEditorItemViewModel {
		public static Func<IDialogContext, FormatEditorFontViewModel> Factory { get { return ViewModelSource.Factory((IDialogContext x) => new FormatEditorFontViewModel(x)); } }
		protected FormatEditorFontViewModel(IDialogContext column)
			: base(column) {
			FontSizes = CreateFontSizes();
			FontStyles = CreateFontStyles();
			FontWeights = CreateFontWeights();
		}
		[BindableProperty(OnPropertyChangedMethodName = "OnChanged")]
		public virtual string FontName { get; set; }
		public IEnumerable<FontStyle> FontStyles { get; private set; }
		[BindableProperty(OnPropertyChangedMethodName = "OnChanged")]
		public virtual FontStyle FontStyle { get; set; }
		public IEnumerable<FontWeight> FontWeights { get; private set; }
		[BindableProperty(OnPropertyChangedMethodName = "OnChanged")]
		public virtual FontWeight FontWeight { get; set; }
		public IEnumerable<double> FontSizes { get; private set; }
		[BindableProperty(OnPropertyChangedMethodName = "OnChanged")]
		public virtual double? FontSize { get; set; }
		[BindableProperty(OnPropertyChangedMethodName = "OnChanged")]
		public virtual Color? FontColor { get; set; }
		[BindableProperty(OnPropertyChangedMethodName = "OnChanged")]
		public virtual bool Strikethrough { get; set; }
		[BindableProperty(OnPropertyChangedMethodName = "OnChanged")]
		public virtual bool Underline { get; set; }
		public override void Clear() {
			FontName = string.Empty;
			FontStyle = System.Windows.FontStyles.Normal;
			FontWeight = System.Windows.FontWeights.Normal;
			FontSize = null;
			FontColor = null;
			Strikethrough = false;
			Underline = false;
			HasChanged = false;
		}
		public override void InitFromFormat(Format format) {
			FontFamily family = format.FontFamily;
			if(family != null && Fonts.SystemFontFamilies.Contains(family) && !string.IsNullOrEmpty(family.ToString()))
				FontName = format.FontFamily.ToString();
			FontStyle style = format.FontStyle;
			if(FontStyles.Contains(style))
				FontStyle = style;
			FontWeight weight = format.FontWeight;
			if(FontWeights.Contains(weight))
				FontWeight = weight;
			double fontSize = format.FontSize;
			if(FontSizes.Contains(fontSize))
				FontSize = fontSize;
			Color? color = GetBrushColor(format.Foreground);
			if(color.HasValue)
				FontColor = color.Value;
			TextDecorationCollection textDecorations = format.TextDecorations;
			if(textDecorations != null) {
				Strikethrough = textDecorations.Contains(TextDecorations.Strikethrough[0]);
				Underline = textDecorations.Contains(TextDecorations.Underline[0]);
			}
		}
		public override void SetFormatProperties(Format format) {
			ManagerHelperBase.SetProperty(format, Format.TextDecorationsProperty, CreateTextDecorations());
			ManagerHelperBase.SetProperty(format, Format.FontFamilyProperty, CreateFontFamily(FontName));
			ManagerHelperBase.SetProperty(format, Format.ForegroundProperty, CreateBrush(FontColor));
			ManagerHelperBase.SetProperty(format, Format.FontStyleProperty, FontStyle);
			ManagerHelperBase.SetProperty(format, Format.FontWeightProperty, FontWeight);
			ManagerHelperBase.SetProperty(format, Format.FontSizeProperty, CreateFontSize(FontSize));
		}
		TextDecorationCollection CreateTextDecorations() {
			if(!(Strikethrough || Underline))
				return null;
			TextDecorationCollection decorations = new TextDecorationCollection();
			if(Strikethrough) decorations.Add(TextDecorations.Strikethrough[0]);
			if(Underline) decorations.Add(TextDecorations.Underline[0]);
			return decorations;
		}
		FontFamily CreateFontFamily(string fontName) {
			return string.IsNullOrEmpty(fontName) ? null : new FontFamily(fontName);
		}
		double CreateFontSize(double? size) {
			return size.HasValue ? size.Value : 0d;
		}
		IEnumerable<FontWeight> CreateFontWeights() {
			yield return System.Windows.FontWeights.Normal;
			yield return System.Windows.FontWeights.Bold;
			yield return System.Windows.FontWeights.Thin;
		}
		IEnumerable<FontStyle> CreateFontStyles() {
			yield return System.Windows.FontStyles.Normal;
			yield return System.Windows.FontStyles.Italic;
			yield return System.Windows.FontStyles.Oblique;
		}
		IEnumerable<double> CreateFontSizes() {
			return new double[] { 8d, 9d, 10d, 11d, 12d, 14d, 16d, 18d, 20d, 22d, 24d, 26d, 28d, 36d, 48d, 72d };
		}
		public override string Description { get { return GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_Font); } }
	}
	public class FormatEditorFillViewModel : FormatEditorItemViewModel {
		public static Func<IDialogContext, FormatEditorFillViewModel> Factory { get { return ViewModelSource.Factory((IDialogContext x) => new FormatEditorFillViewModel(x)); } }
		protected FormatEditorFillViewModel(IDialogContext column) : base(column) { }
		[BindableProperty(OnPropertyChangedMethodName = "OnChanged")]
		public virtual Color? Background { get; set; }
		public override void Clear() {
			Background = null;
			HasChanged = false;
		}
		public override void InitFromFormat(Format format) {
			Color? color = GetBrushColor(format.Background);
			if(color.HasValue)
				Background = color.Value;
		}
		public override void SetFormatProperties(Format format) {
			ManagerHelperBase.SetProperty(format, Format.BackgroundProperty, CreateBrush(Background));
		}
		public override string Description { get { return GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_Fill); } }
	}
	public class FormatEditorIconViewModel : FormatEditorItemViewModel {
		public static Func<IDialogContext, FormatEditorIconViewModel> Factory { get { return ViewModelSource.Factory((IDialogContext x) => new FormatEditorIconViewModel(x)); } }
		Dictionary<string, FormatEditorIconItemViewModel> iconCache = new Dictionary<string, FormatEditorIconItemViewModel>();
		[BindableProperty(OnPropertyChangedMethodName = "OnChanged")]
		public virtual FormatEditorIconItemViewModel IconItem { get; set; }
		public ObservableCollection<FormatEditorIconGroup> Groups { get; private set; }
		protected FormatEditorIconViewModel(IDialogContext column)
			: base(column) {
			CreateGroups();
		}
		void CreateGroups() {
			iconCache.Clear();
			var groups = Context.PredefinedFormatsOwner.PredefinedIconSetFormats
				.GroupBy(x => x.GroupName)
				.Select(x => new FormatEditorIconGroup(x.Key, ExtractIcons(x)))
				.Where(x => x.Icons.Length > 0);
			Groups = new ObservableCollection<FormatEditorIconGroup>(groups);
		}
		FormatEditorIconItemViewModel[] ExtractIcons(IEnumerable<FormatInfo> formatInfo) {
			return ExtractIcons(formatInfo.Select(x => x.Format as IconSetFormat));
		}
		FormatEditorIconItemViewModel[] ExtractIcons(IEnumerable<IconSetFormat> format) {
			return format
				.Where(x => x != null)
				.SelectMany(x => x.Elements)
				.Select(e => FormatEditorIconItemViewModel.Factory(e.Icon))
				.Where(i => CheckIsNewAndCache(i))
				.ToArray();
		}
		bool CheckIsNewAndCache(FormatEditorIconItemViewModel item) {
			ImageSource source = item.Icon;
			if(source == null)
				return false;
			string path = GetPath(source);
			if(path == null)
				return true;
			if(iconCache.ContainsKey(path))
				return false;
			iconCache.Add(path, item);
			return true;
		}
		string GetPath(ImageSource source) {
			var image = source as System.Windows.Media.Imaging.BitmapImage;
			if(image == null || image.UriSource == null)
				return null;
			string path = image.UriSource.AbsolutePath;
			return string.IsNullOrEmpty(path) ? null : path;
		}
		public override void Clear() {
			IconItem = null;
		}
		public override void InitFromFormat(Format format) {
			if(format.Icon == null) {
				Clear();
				return;
			}
			string path = GetPath(format.Icon);
			FormatEditorIconItemViewModel item = null;
			if(path != null)
				iconCache.TryGetValue(path, out item);
			if(item == null) {
				item = FormatEditorIconItemViewModel.Factory(format.Icon);
				var group = new FormatEditorIconGroup(GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_CustomIconGroup), new FormatEditorIconItemViewModel[] { item });
				Groups.Insert(0, group);
			}
			if(IconItem != null)
				IconItem.IsChecked = false;
			IconItem = item;
			IconItem.IsChecked = true;
		}
		public override void SetFormatProperties(Format format) {
			ManagerHelperBase.SetProperty(format, Format.IconProperty, IconItem.With(x => x.Icon));
		}
		public override string Description { get { return GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_Icon); } }
	}
	public class FormatEditorIconGroup {
		public string Header { get; private set; }
		public FormatEditorIconItemViewModel[] Icons { get; private set; }
		public FormatEditorIconGroup(string header, FormatEditorIconItemViewModel[] icons) {
			Header = header;
			Icons = icons;
		}
	}
	public class FormatEditorIconItemViewModel {
		public static Func<ImageSource, FormatEditorIconItemViewModel> Factory { get { return ViewModelSource.Factory((ImageSource x) => new FormatEditorIconItemViewModel(x)); } }
		public ImageSource Icon { get; private set; }
		public virtual bool IsChecked { get; set; }
		protected FormatEditorIconItemViewModel(ImageSource icon) {
			Icon = icon;
		}
	}
	public static class ManagerHelperBase {
		public static void SetProperty(DependencyObject dependencyObject, DependencyProperty property, object value) {
			if(!Object.Equals(dependencyObject.GetValue(property), value))
				dependencyObject.SetValue(property, value);
		}
		public static bool ShowDialog(object viewModel, string description, IDialogService service, Action<CancelEventArgs> executeMethod = null) {
			List<UICommand> commands = UICommand.GenerateFromMessageBoxButton(MessageBoxButton.OKCancel, new DXDialogWindowMessageBoxButtonLocalizer());
			if(executeMethod != null)
				commands[0].Command = new DelegateCommand<CancelEventArgs>(executeMethod);
			return service.ShowDialog(commands, description, viewModel) == commands[0];
		}
		public static object ConvertRuleValue(object source, Type fieldType, bool isInDesignMode) {
			if(fieldType.IsEnum)
				return ConvertEnum(fieldType, source);
			if(isInDesignMode)
				return ConvertDesignValue(source);
			return source;
		}
		static object ConvertEnum(Type fieldType, object source) {
			object result = null;
			try {
				result = Enum.Parse(fieldType, source.ToString());
			}
			catch {
				result = source;
			}
			return result;
		}
		static object ConvertDesignValue(object source) {
			object result = source;
			System.ComponentModel.TypeConverter converter = System.ComponentModel.TypeDescriptor.GetConverter(source.GetType());
			if(converter != null && converter.CanConvertFrom(typeof(string)) && converter.CanConvertTo(typeof(string)))
				result = converter.ConvertToInvariantString(source);
			return result;
		}
	}
	public class IconProcessor {
		public ImageSource Icon { get; set; }
		public IconProcessor(ImageSource icon) {
			this.Icon = icon;
		}
		public void SetIconProperty(IModelProperty property, IEditingContext context) {
			string iconFileName = GetPredefinedIconName();
			if(iconFileName == null)
				property.SetValue(Icon);
			else {
				IModelItem extension = context.CreateItem(typeof(Xpf.Core.ConditionalFormatting.Themes.ConditionalFormattingIconSetIconExtension));
				extension.Properties["IconName"].SetValue(iconFileName);
				property.SetValue(extension);
			}
		}
		public string GetPredefinedIconName() {
			string fileName = null;
			var image = Icon as System.Windows.Media.Imaging.BitmapImage;
			if(image != null) {
				try {
					string path = image.UriSource.AbsolutePath;
					string directory = System.IO.Path.GetDirectoryName(path);
					string defaultDirectory = System.IO.Path.GetDirectoryName(new Uri(ConditionalFormatResourceHelper.DefaultPathCore, UriKind.Absolute).AbsolutePath);
					if(directory == defaultDirectory)
						fileName = System.IO.Path.GetFileName(path);
				}
				catch { }
			}
			return fileName;
		}
	}
	public class ContainOperatorBase<T> where T : struct, IComparable {
		#region Nested classes
		class UniversalValue : OperandValue {
			public override bool Equals(object obj) {
				return obj is OperandValue;
			}
			public override int GetHashCode() {
				return base.GetHashCode();
			}
		}
		class UniversalProperty : OperandProperty {
			public override bool Equals(object obj) {
				return obj is OperandProperty;
			}
			public override int GetHashCode() {
				return base.GetHashCode();
			}
		}
		#endregion
		readonly string name;
		CriteriaOperator prototype;
		public Func<OperandProperty, OperandValue[], CriteriaOperator> Factory { get; private set; }
		public Func<CriteriaOperator, object[]> Extractor { get; private set; }
		public T Rule { get; private set; }
		public ContainOperatorBase(string name, T rule, Func<OperandProperty, OperandValue[], CriteriaOperator> factory, Func<CriteriaOperator, object[]> extractor) {
			this.name = name;
			Factory = factory;
			Extractor = extractor;
			prototype = Factory(new UniversalProperty(), Enumerable.Range(0, 2).Select(x => new UniversalValue()).ToArray());
			Rule = rule;
		}
		public bool Match(CriteriaOperator op) {
			return prototype.Equals(op);
		}
		public override string ToString() {
			return name;
		}
	}
	public interface IConditionModelItemsBuilder {
		IModelItem BuildCondition(ConditionEditUnit unit, IModelItem source);
		IModelItem BuildTopBottomCondition(TopBottomEditUnit unit, IModelItem source);
		IModelItem BuildColorScaleCondition(ColorScaleEditUnit unit, IModelItem source);
		IModelItem BuildDataBarCondition(DataBarEditUnit unit, IModelItem source);
		IModelItem BuildIconSetCondition(IconSetEditUnit unit, IModelItem source);
	}
	public interface ISupportManager {
		BaseEditUnit CreateEditUnit();
	}
	public abstract class BaseEditUnit {
		HashSet<string> modifiedPropertyNames = new HashSet<string>();
		string fieldName;
		string expression;
		bool applyToRow;
		string predefinedFormatName;
		string rowName = "PivotGridAnyFieldNameFieldName";
		string columnName = "PivotGridAnyFieldNameFieldName";
		public string FieldName {
			get {
				return fieldName;
			}
			set {
				if(fieldName != value) {
					fieldName = value;
					RegisterPropertyModification("FieldName");
				}
			}
		}
		public string Expression {
			get {
				return expression;
			}
			set {
				if(expression != value) {
					expression = value;
					RegisterPropertyModification("Expression");
				}
			}
		}
		[ManagerRestorablePropertyAttribute]
		public bool ApplyToRow {
			get {
				return applyToRow;
			}
			set {
				if(applyToRow != value) {
					applyToRow = value;
					RegisterPropertyModification("ApplyToRow");
				}
			}
		}
		public abstract bool CanApplyToRow { get; }
		public string PredefinedFormatName {
			get {
				return predefinedFormatName;
			}
			set {
				if(predefinedFormatName != value)
					predefinedFormatName = value;
				RegisterPropertyModification("PredefinedFormatName");
			}
		}
		[ManagerRestorablePropertyAttribute]
		public string RowName {
			get {
				return rowName ;
			}
			set {
				if(rowName != value) {
					rowName = value;
					RegisterPropertyModification("RowName");
				}
			}
		}
		[ManagerRestorablePropertyAttribute]
		public string ColumnName {
			get {
				return columnName;
			}
			set {
				if(columnName != value) {
					columnName = value;
					RegisterPropertyModification("ColumnName");
				}
			}
		}
		public abstract IModelItem BuildCondition(IConditionModelItemsBuilder builder, IModelItem source);
		public IModelItem BuildCondition(IConditionModelItemsBuilder builder) {
			return BuildCondition(builder, null);
		}
		public abstract Freezable GetFormat();
		public abstract string GetDescription();
		public void Populate(BaseEditUnit unit) {
			PopulateCore(unit, (prop) => unit.IsPropertyModified(prop.Name));
		}
		public void Restore(BaseEditUnit unit) {
			PopulateCore(unit, (prop) => unit.IsPropertyModified(prop.Name) && !IsPropertyModified(prop.Name) && prop.Attributes.Contains(new ManagerRestorablePropertyAttribute()));
		}
		void PopulateCore(BaseEditUnit unit, Func<PropertyDescriptor, bool> action) {
			PropertyDescriptorCollection currentProperties = TypeDescriptor.GetProperties(this);
			PropertyDescriptorCollection newProperties = TypeDescriptor.GetProperties(unit);
			foreach(PropertyDescriptor prop in newProperties) {
				if(currentProperties.Contains(prop) && action(prop)) {
					object newValue = prop.GetValue(unit);
					prop.SetValue(this, newValue);
				}
			}
		}
		public void SetModelItemProperty<TOwner, TProperty>(IModelItem item, DependencyProperty property, Expression<Func<TOwner, TProperty>> expression) where TOwner : BaseEditUnit {
			TOwner owner = this as TOwner;
			if(owner != null && IsPropertyModified<TOwner, TProperty>(expression)) {
				Func<TOwner, TProperty> valueAccessor = expression.Compile();
				TProperty value = valueAccessor(owner);
				if(!Object.Equals(item.Properties[property.Name].ComputedValue, value))
					item.Properties[property.Name].SetValue(value);
			}
		}
		protected void RegisterPropertyModification(string propertyName) {
			modifiedPropertyNames.Add(propertyName);
		}
		bool IsPropertyModified<TOwner, TProperty>(Expression<Func<TOwner, TProperty>> expression) where TOwner : BaseEditUnit {
			return (expression.Body as MemberExpression).Return(x => IsPropertyModified(x.Member.Name), () => false);
		}
		bool IsPropertyModified(string propertyName) {
			return modifiedPropertyNames.Contains(propertyName);
		}
	}
	public class ManagerRestorablePropertyAttribute : Attribute {
	}
	public class ConditionEditUnit : BaseEditUnit {
		Format format;
		ConditionRule valueRule;
		object value1;
		object value2;
		public Format Format {
			get {
				return format;
			}
			set {
				if(format != value)
					format = value;
				RegisterPropertyModification("Format");
			}
		}
		public ConditionRule ValueRule {
			get {
				return valueRule;
			}
			set {
				if(valueRule != value)
					valueRule = value;
				RegisterPropertyModification("ValueRule");
			}
		}
		public object Value1 {
			get {
				return value1;
			}
			set {
				if(value1 != value)
					value1 = value;
				RegisterPropertyModification("Value1");
			}
		}
		public object Value2 {
			get {
				return value2;
			}
			set {
				if(value2 != value)
					value2 = value;
				RegisterPropertyModification("Value2");
			}
		}
		public override bool CanApplyToRow { get { return true; } }
		public override IModelItem BuildCondition(IConditionModelItemsBuilder builder, IModelItem source) {
			return builder.BuildCondition(this, source);
		}
		public override Freezable GetFormat() {
			return Format;
		}
		public override string GetDescription() {
			return GetActualExpressionString();
		}
		public string GetActualExpressionString() {
			if(ValueRule == ConditionRule.Expression)
				return Expression;
			CriteriaOperator op = GetExpressionFactory(ValueRule)(new OperandProperty(FieldName), new OperandValue[] { new OperandValue(Value1), new OperandValue(Value2) });
			return op.ToString();
		}
		public static Func<OperandProperty, OperandValue[], CriteriaOperator> GetExpressionFactory(ConditionRule rule) {
			switch(rule) {
				case ConditionRule.Between:
					return (op, ov) => new BetweenOperator(op, ov[0], ov[1]);
				case ConditionRule.NotBetween:
					return (op, ov) => new BetweenOperator(op, ov[0], ov[1]).Not();
				case ConditionRule.Equal:
					return (op, ov) => new BinaryOperator(op, ov[0], BinaryOperatorType.Equal);
				case ConditionRule.NotEqual:
					return (op, ov) => new BinaryOperator(op, ov[0], BinaryOperatorType.NotEqual);
				case ConditionRule.Greater:
					return (op, ov) => new BinaryOperator(op, ov[0], BinaryOperatorType.Greater);
				case ConditionRule.Less:
					return (op, ov) => new BinaryOperator(op, ov[0], BinaryOperatorType.Less);
				case ConditionRule.GreaterOrEqual:
					return (op, ov) => new BinaryOperator(op, ov[0], BinaryOperatorType.GreaterOrEqual);
				case ConditionRule.LessOrEqual:
					return (op, ov) => new BinaryOperator(op, ov[0], BinaryOperatorType.LessOrEqual);
				case ConditionRule.None:
					return (_, __) => new OperandValue(true);
				default:
					throw new InvalidOperationException();
			}
		}
	}
	public class TopBottomEditUnit : ConditionEditUnit {
		TopBottomRule rule;
		double threshold;
		public TopBottomRule Rule {
			get {
				return rule;
			}
			set {
				if(rule != value)
					rule = value;
				RegisterPropertyModification("Rule");
			}
		}
		public double Threshold {
			get {
				return threshold;
			}
			set {
				if(threshold != value)
					threshold = value;
				RegisterPropertyModification("Threshold");
			}
		}
		public override string GetDescription() {
			if(IsAboveBelowCondition())
				return ConditionalFormattingLocalizer.GetString(Rule == TopBottomRule.AboveAverage ? ConditionalFormattingStringId.ConditionalFormatting_Manager_Above : ConditionalFormattingStringId.ConditionalFormatting_Manager_Below);
			if(IsTopBottomCondition()) {
				var builder = new System.Text.StringBuilder();
				builder.Append(ConditionalFormattingLocalizer.GetString((Rule == TopBottomRule.TopItems || Rule == TopBottomRule.TopPercent) ? ConditionalFormattingStringId.ConditionalFormatting_Manager_Top : ConditionalFormattingStringId.ConditionalFormatting_Manager_Bottom));
				builder.Append(' ');
				builder.Append(Threshold);
				if(Rule == TopBottomRule.TopPercent || Rule == TopBottomRule.BottomPercent)
					builder.Append("%");
				return builder.ToString();
			}
			return string.Empty;
		}
		public bool IsTopBottomCondition() {
			return Rule == TopBottomRule.BottomItems || Rule == TopBottomRule.BottomPercent || Rule == TopBottomRule.TopItems || Rule == TopBottomRule.TopPercent;
		}
		public bool IsAboveBelowCondition() {
			return Rule == TopBottomRule.AboveAverage || Rule == TopBottomRule.BelowAverage;
		}
		public override IModelItem BuildCondition(IConditionModelItemsBuilder builder, IModelItem source) {
			return builder.BuildTopBottomCondition(this, source);
		}
	}
	public abstract class IndicatorEditUnit : BaseEditUnit {
		object minValue;
		object maxValue;
		public object MinValue {
			get {
				return minValue;
			}
			set {
				if(minValue != value)
					minValue = value;
				RegisterPropertyModification("MinValue");
			}
		}
		public object MaxValue {
			get {
				return maxValue;
			}
			set {
				if(maxValue != value)
					maxValue = value;
				RegisterPropertyModification("MaxValue");
			}
		}
		public override bool CanApplyToRow { get { return false; } }
	}
	public class ColorScaleEditUnit : IndicatorEditUnit {
		ColorScaleFormat format;
		public ColorScaleFormat Format {
			get {
				return format;
			}
			set {
				if(format != value)
					format = value;
				RegisterPropertyModification("Format");
			}
		}
		public override IModelItem BuildCondition(IConditionModelItemsBuilder builder, IModelItem source) {
			return builder.BuildColorScaleCondition(this, source);
		}
		public override Freezable GetFormat() {
			return Format;
		}
		public override string GetDescription() {
			return ConditionalFormattingLocalizer.GetString(ConditionalFormattingStringId.ConditionalFormatting_Manager_GradedColorScale);
		}
	}
	public class DataBarEditUnit : IndicatorEditUnit {
		DataBarFormat format;
		public DataBarFormat Format {
			get {
				return format;
			}
			set {
				if(format != value)
					format = value;
				RegisterPropertyModification("Format");
			}
		}
		public override IModelItem BuildCondition(IConditionModelItemsBuilder builder, IModelItem source) {
			return builder.BuildDataBarCondition(this, source);
		}
		public override Freezable GetFormat() {
			return Format;
		}
		public override string GetDescription() {
			return ConditionalFormattingLocalizer.GetString(ConditionalFormattingStringId.ConditionalFormatting_Manager_DataBar);
		}
	}
	public class IconSetEditUnit : IndicatorEditUnit {
		IconSetFormat format;
		public IconSetFormat Format {
			get {
				return format;
			}
			set {
				if(format != value)
					format = value;
				RegisterPropertyModification("Format");
			}
		}
		public override IModelItem BuildCondition(IConditionModelItemsBuilder builder, IModelItem source) {
			return builder.BuildIconSetCondition(this, source);
		}
		public override Freezable GetFormat() {
			return Format;
		}
		public override string GetDescription() {
			return ConditionalFormattingLocalizer.GetString(ConditionalFormattingStringId.ConditionalFormatting_Manager_IconSet);
		}
	}
}
