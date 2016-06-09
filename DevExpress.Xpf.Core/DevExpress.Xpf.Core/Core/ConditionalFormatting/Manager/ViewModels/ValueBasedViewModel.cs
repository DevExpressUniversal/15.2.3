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
using System.Windows;
using System.Windows.Media;
using DevExpress.Data;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DevExpress.Xpf.Core;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
namespace DevExpress.Xpf.Core.ConditionalFormattingManager {
	public class ValueBasedViewModel : ConditionViewModelBase {
		public static Func<IDialogContext, ValueBasedViewModel> Factory { get { return ViewModelSource.Factory((IDialogContext x) => new ValueBasedViewModel(x)); } }
		protected ValueBasedViewModel(IDialogContext context) : base(context) { }
		protected override IEnumerable<IConditionEditor> CreateChildViewModels() {
			return new IConditionEditor[] { ColorScaleViewModel.Factory(Context, false),
				ColorScaleViewModel.Factory(Context, true),
				DataBarViewModel.Factory(Context),
				IconViewModel.Factory(Context) };
		}
		public override string Description { get { return GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_ValueBasedDescription); } }
	}
	public abstract class FieldEditorOwner : ManagerViewModelBase {
		IDataColumnInfo columnInfo;
		class EditorViewModelWrapper {
			public string Expression { get; set; }
			public IDataColumnInfo ColumnInfo { get; set; }
		}
		class EditorDataColumnInfoWrapper : IDataColumnInfo {
			readonly IDataColumnInfo column;
			readonly string expression;
			public EditorDataColumnInfoWrapper(IDataColumnInfo column, string expression) {
				this.column = column;
				this.expression = expression == null ? string.Empty : expression;
			}
			string IDataColumnInfo.Caption { get { return column.Caption; } }
			List<IDataColumnInfo> IDataColumnInfo.Columns { get { return column.Columns; } }
			DataControllerBase IDataColumnInfo.Controller { get { return column.Controller; } }
			string IDataColumnInfo.FieldName { get { return column.FieldName; } }
			Type IDataColumnInfo.FieldType { get { return column.FieldType; } }
			string IDataColumnInfo.Name { get { return column.Name; } }
			string IDataColumnInfo.UnboundExpression { get { return expression; } }
		}
		public IList<FieldNameWrapper> FieldNames { get; private set; }
		[BindableProperty(OnPropertyChangedMethodName = "UpdateConditionValueType")]
		public virtual string FieldName { get; set; }
		public virtual string Expression { get; set; }
		public virtual FieldEditorMode FieldMode { get; set; }
		public virtual IDialogService DialogService { get { return null; } }
		public virtual IMessageBoxService MessageBoxService { get { return null; } }
		public virtual object MinValue { get; set; }
		public virtual object MaxValue { get; set; }
		public virtual DataValueType ConditionValueType { get; set; }
		protected FieldEditorOwner(IDialogContext context)
			: base(context) {
			columnInfo = Context.ColumnInfo;
			FieldNames = FieldNameWrapper.Create(context.ColumnInfo);
			FieldName = context.ColumnInfo.FieldName;
			UpdateConditionValueType();
		}
		public void ShowCustomConditionEditor() {
			var viewModel = new EditorViewModelWrapper { ColumnInfo = new EditorDataColumnInfoWrapper(columnInfo, GetValueExpression(Expression)) };
			if(ManagerHelper.ShowDialog(viewModel, GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_CustomConditionEditor_Title), DialogService))
				Expression = GetDisplayExpression(viewModel.Expression);
		}
		protected bool ValidateExpression() {
			string expression = GetValueExpression(Expression);
			if(string.IsNullOrEmpty(expression))
				return true;
			return ConditionalFormattingDialogHelper.ValidateExpression(expression, MessageBoxService, columnInfo);
		}
		protected void InitIndicator(IndicatorEditUnit unit) {
			FieldName = unit.FieldName;
			Expression = GetDisplayExpression(unit.Expression);
			FieldMode = string.IsNullOrEmpty(Expression) ? FieldEditorMode.FieldName : FieldEditorMode.Expression;
			MinValue = unit.MinValue;
			MaxValue = unit.MaxValue;
			UpdateConditionValueType();
		}
		protected void EditIndicator(IndicatorEditUnit unit) {
			unit.FieldName = FieldName;
			unit.Expression = GetValueExpression(Expression);
			unit.MinValue = GetConvertedValue(MinValue);
			unit.MaxValue = GetConvertedValue(MaxValue);
		}
		object GetConvertedValue(object value) {
			if(value == null)
				return null;
			return ConditionValueType == DataValueType.DateTime ? value : value.ToString();
		}
		protected virtual void UpdateConditionValueType() {
			ConditionValueType = CalcConditionValueType();
		}
		protected virtual void OnConditionValueTypeChanged() {
			if(ConditionValueType != CalcConditionValueType()) {
				MinValue = null;
				MaxValue = null;
			}
		}
		DataValueType CalcConditionValueType() {
			if(MinValue == null && MaxValue == null) {
				IDialogContext context = Context.Find(FieldName) ?? Context;
				return ExpressionConditionalFormattingDialogViewModel.SelectValue(context.ColumnInfo.FieldType, () => DataValueType.DateTime, () => DataValueType.Numeric, () => DataValueType.Numeric);
			}
			else {
				object value = MinValue ?? MaxValue;
				TypeCode typeCode = Type.GetTypeCode(value.GetType());
				return typeCode == TypeCode.DateTime ? DataValueType.DateTime : DataValueType.Numeric;
			}
		}
		string GetDisplayExpression(string expression) {
			return ManagerHelper.ConvertExpression(expression, columnInfo, DevExpress.Data.ExpressionEditor.UnboundExpressionConvertHelper.ConvertToCaption);
		}
		string GetValueExpression(string expression) {
			return ManagerHelper.ConvertExpression(expression, columnInfo, DevExpress.Data.ExpressionEditor.UnboundExpressionConvertHelper.ConvertToFields);
		}
	}
	public class ColorScaleViewModel : FieldEditorOwner, IConditionEditor {
		public static Func<IDialogContext, bool, ColorScaleViewModel> Factory { get { return ViewModelSource.Factory((IDialogContext x, bool y) => new ColorScaleViewModel(x, y)); } }
		public static Color DefaultMin { get { return Color.FromArgb(255, 248, 105, 107); } }
		public static Color DefaultMiddle { get { return Color.FromArgb(255, 255, 235, 132); } }
		public static Color DefaultMax { get { return Color.FromArgb(255, 99, 190, 123); } }
		readonly bool allowColorMiddle;
		protected ColorScaleViewModel(IDialogContext context, bool allowColorMiddle)
			: base(context) {
			this.allowColorMiddle = allowColorMiddle;
			ColorMin = DefaultMin;
			ColorMiddle = DefaultMiddle;
			ColorMax = DefaultMax;
		}
		[BindableProperty(OnPropertyChangedMethodName = "UpdateFormat")]
		public virtual Color ColorMin { get; set; }
		[BindableProperty(OnPropertyChangedMethodName = "UpdateFormat")]
		public virtual Color ColorMax { get; set; }
		[BindableProperty(OnPropertyChangedMethodName = "UpdateFormat")]
		public virtual Color ColorMiddle { get; set; }
		public bool AllowColorMiddle { get { return allowColorMiddle; } }
		public virtual ColorScaleFormat PreviewFormat { get; protected set; }
		BaseEditUnit IConditionEditor.Edit() {
			var unit = new ColorScaleEditUnit();
			unit.Format = PreviewFormat;
			EditIndicator(unit);
			return unit;
		}
		void IConditionEditor.Init(BaseEditUnit unit) {
			var colorUnit = unit as ColorScaleEditUnit;
			if(colorUnit == null)
				return;
			InitIndicator(colorUnit);
			ColorScaleFormat format = colorUnit.Format;
			if(format == null)
				return;
			ColorMin = format.ColorMin;
			ColorMax = format.ColorMax;
			if(AllowColorMiddle && format.ColorMiddle.HasValue)
				ColorMiddle = format.ColorMiddle.Value;
		}
		bool IConditionEditor.CanInit(BaseEditUnit unit) {
			return (unit as ColorScaleEditUnit).If(x => x.Format == null || x.Format.ColorMiddle.HasValue == AllowColorMiddle) != null;
		}
		bool IConditionEditor.Validate() {
			return ValidateExpression();
		}
		protected void UpdateFormat() {
			var format = new ColorScaleFormat() { ColorMax = this.ColorMax, ColorMin = this.ColorMin };
			if(AllowColorMiddle)
				format.ColorMiddle = ColorMiddle;
			format.Freeze();
			PreviewFormat = format;
		}
		public override string Description { get { return AllowColorMiddle ? GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_3ColorScaleDescription) : GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_2ColorScaleDescription); } }
	}
	public class DataBarViewModel : FieldEditorOwner, IConditionEditor {
		public static Func<IDialogContext, DataBarViewModel> Factory { get { return ViewModelSource.Factory((IDialogContext x) => new DataBarViewModel(x)); } }
		protected DataBarViewModel(IDialogContext context)
			: base(context) {
			ColorFill = Colors.Blue;
			ColorBorder = Colors.Black;
			BorderMode = DataBarBorderMode.NoBorder;
			ColorFillNegative = Colors.Red;
			ColorBorderNegative = Colors.Black;
			BorderModeNegative = DataBarBorderMode.NoBorder;
			UseDefaultNegativeBar = false;
		}
		[BindableProperty(OnPropertyChangedMethodName = "UpdateFormat")]
		public virtual Color ColorFill { get; set; }
		[BindableProperty(OnPropertyChangedMethodName = "UpdateFormat")]
		public virtual Color ColorBorder { get; set; }
		[BindableProperty(OnPropertyChangedMethodName = "UpdateFormat")]
		public virtual DataBarFillMode FillMode { get; set; }
		[BindableProperty(OnPropertyChangedMethodName = "UpdateFormat")]
		public virtual DataBarBorderMode BorderMode { get; set; }
		[BindableProperty(OnPropertyChangedMethodName = "UpdateFormat")]
		public virtual bool UseDefaultNegativeBar { get; set; }
		[BindableProperty(OnPropertyChangedMethodName = "UpdateFormat")]
		public virtual Color ColorFillNegative { get; set; }
		[BindableProperty(OnPropertyChangedMethodName = "UpdateFormat")]
		public virtual Color ColorBorderNegative { get; set; }
		[BindableProperty(OnPropertyChangedMethodName = "UpdateFormat")]
		public virtual DataBarFillMode FillModeNegative { get; set; }
		[BindableProperty(OnPropertyChangedMethodName = "UpdateFormat")]
		public virtual DataBarBorderMode BorderModeNegative { get; set; }
		public virtual DataBarFormat PreviewFormat { get; protected set; }
		BaseEditUnit IConditionEditor.Edit() {
			var unit = new DataBarEditUnit();
			unit.Format = PreviewFormat;
			EditIndicator(unit);
			return unit;
		}
		void IConditionEditor.Init(BaseEditUnit unit) {
			var barUnit = unit as DataBarEditUnit;
			if(barUnit == null)
				return;
			InitIndicator(barUnit);
			DataBarFormat format = barUnit.Format;
			if(format == null)
				return;
			ColorFill = GetColor(format.Fill, ColorFill, true);
			ColorBorder = GetColor(format.BorderBrush, ColorBorder, true);
			ColorFillNegative = GetColor(format.FillNegative, ColorFill, false);
			ColorBorderNegative = GetColor(format.BorderBrushNegative, ColorBorder, false);
			UseDefaultNegativeBar = format.FillNegative == null && format.BorderBrushNegative == null;
			BorderMode = GetBorderMode(format.BorderBrush);
			BorderModeNegative = GetBorderMode(format.BorderBrushNegative);
			FillMode = GetFillMode(format.Fill);
			FillModeNegative = GetFillMode(format.FillNegative);
		}
		bool IConditionEditor.CanInit(BaseEditUnit unit) {
			return unit is DataBarEditUnit;
		}
		bool IConditionEditor.Validate() {
			return ValidateExpression();
		}
		protected void UpdateFormat() {
			var format = new DataBarFormat();
			if(BorderMode == DataBarBorderMode.Border)
				format.BorderBrush = new SolidColorBrush(ColorBorder);
			if(!UseDefaultNegativeBar && BorderModeNegative == DataBarBorderMode.Border)
				format.BorderBrushNegative = new SolidColorBrush(ColorBorderNegative);
			format.Fill = CreateFillBrush(true);
			format.FillNegative = CreateFillBrush(false);
			format.Freeze();
			PreviewFormat = format;
		}
		Brush CreateFillBrush(bool isPositive) {
			DataBarFillMode mode = FillMode;
			Color color = ColorFill;
			if(!(isPositive || UseDefaultNegativeBar)) {
				mode = FillModeNegative;
				color = ColorFillNegative;
			}
			switch(mode) {
				case DataBarFillMode.SolidFill:
					return new SolidColorBrush(color);
				case DataBarFillMode.GradientFill:
					Color startColor = color;
					Color endColor = Colors.White;
					if(!isPositive) {
						Color tmp = startColor;
						startColor = endColor;
						endColor = tmp;
					}
					return new LinearGradientBrush(startColor, endColor, 0d);
				default:
					throw new InvalidOperationException();
			}
		}
		static Color GetColor(Brush brush, Color defalutColor, bool isPositive) {
			if(brush is SolidColorBrush)
				return ((SolidColorBrush)brush).Color;
			else if(brush is GradientBrush) {
				var gradient = (GradientBrush)brush;
				var stop = gradient.GradientStops.FirstOrDefault(x => x.Offset == (isPositive ? 0d : 1d));
				if(stop != null)
					return stop.Color;
			}
			return defalutColor;
		}
		DataBarBorderMode GetBorderMode(Brush brush) {
			return brush == null ? DataBarBorderMode.NoBorder : DataBarBorderMode.Border;
		}
		DataBarFillMode GetFillMode(Brush brush) {
			return brush is GradientBrush ? DataBarFillMode.GradientFill : DataBarFillMode.SolidFill;
		}
		public override string Description { get { return GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_DataBar); } }
	}
	public class IconViewModel : FieldEditorOwner, IConditionEditor {
		public static Func<IDialogContext, IconViewModel> Factory { get { return ViewModelSource.Factory((IDialogContext x) => new IconViewModel(x)); } }
		protected IconViewModel(IDialogContext context)
			: base(context) {
			ElementsViewModels = new IconItemViewModelCollection();
			FormatInfoCollection predefined = context.PredefinedFormatsOwner.PredefinedIconSetFormats;
			if(predefined == null)
				return;
			FormatStyles = predefined.Select(x => new IconFormatStyle((IconSetFormat)x.Format, x.FormatName)).ToList();
			FormatStyle = FormatStyles.FirstOrDefault();
		}
		public IList<IconFormatStyle> FormatStyles { get; private set; }
		public virtual IconFormatStyle FormatStyle { get; set; }
		public virtual IconValueType ValueType { get; set; }
		public IconItemViewModelCollection ElementsViewModels { get; private set; }
		BaseEditUnit IConditionEditor.Edit() {
			var unit = new IconSetEditUnit();
			unit.Format = CreateFormat();
			EditIndicator(unit);
			unit.PredefinedFormatName = null;
			return unit;
		}
		void IConditionEditor.Init(BaseEditUnit unit) {
			var iconUnit = unit as IconSetEditUnit;
			if(iconUnit == null)
				return;
			InitIndicator(iconUnit);
			IconSetFormat format = iconUnit.Format;
			string name = iconUnit.PredefinedFormatName;
			if(string.IsNullOrEmpty(name) && format != null) {
				var formatStyle = new IconFormatStyle(format, string.Empty);
				FormatStyles = FormatStyles.Concat(new IconFormatStyle[] { formatStyle }).ToList();
				FormatStyle = formatStyle;
			}
			else FormatStyle = FormatStyles.FirstOrDefault(x => x.FormatName == name);
		}
		bool IConditionEditor.CanInit(BaseEditUnit unit) {
			return unit is IconSetEditUnit;
		}
		bool IConditionEditor.Validate() {
			return ValidateExpression();
		}
		IconSetFormat CreateFormat() {
			var format = new IconSetFormat { ElementThresholdType = (ConditionalFormattingValueType)ValueType };
			IconSetElementCollection elements = format.Elements;
			foreach(var vm in ElementsViewModels)
				elements.Add(vm.CreateElement());
			return format;
		}
		protected void OnFormatStyleChanged() {
			if(FormatStyle == null)
				return;
			ValueType = (IconValueType)FormatStyle.Format.ElementThresholdType;
			UpdateElementsViewModels();
		}
		void UpdateElementsViewModels() {
			ElementsViewModels.BeginUpdate();
			ElementsViewModels.Clear();
			IconSetElement[] elements = FormatStyle.Format.GetSortedElements();
			IList<ImageSource> availableIcons = FormatStyles.SelectMany(x => x.Icons).ToList();
			for(int i = 0; i < elements.Length; i++) {
				IconItemViewModel vm = CreateItemViewModel(elements[i], availableIcons);
				ElementsViewModels.Add(vm);
				UpdateItemDescription(i);
			}
			ElementsViewModels.EndUpdate();
		}
		IconItemViewModel CreateItemViewModel(IconSetElement element, IList<ImageSource> availableIcons) {
			var vm = IconItemViewModel.Factory(this);
			vm.Threshold = element.Threshold;
			vm.ComparisonType = (IconComparisonType)element.ThresholdComparisonType;
			vm.Icon = element.Icon;
			vm.Icons = availableIcons;
			return vm;
		}
		internal void UpdateNextItemDescription(IconItemViewModel sourceItem) {
			int next = ElementsViewModels.IndexOf(sourceItem) + 1;
			if(next != 0 && next != ElementsViewModels.Count)
				UpdateItemDescription(next);
		}
		void UpdateItemDescription(int index) {
			IconItemViewModel vm = ElementsViewModels[index];
			if(index > 0) {
				IconItemViewModel prev = ElementsViewModels[index - 1];
				string op = prev.ComparisonType == IconComparisonType.Greater ? "<=" : "<";
				string description = String.Format(GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_IconDescriptionCondition) + " {0} {1}", op, Math.Round(prev.Threshold));
				if(vm.HasBottomLimit)
					description += GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_IconDescriptionConnector);
				vm.Description = description;
			}
			else vm.Description = GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_IconDescriptionValueCondition);
		}
		public override string Description { get { return GetLocalizedString(ConditionalFormattingStringId.ConditionalFormatting_Manager_IconDescription); } }
		public void ReverseIcons() {
			var reversedIcons = ElementsViewModels.Select(x => x.Icon).Reverse().ToList();
			for(int i = 0; i < ElementsViewModels.Count; i++)
				ElementsViewModels[i].Icon = reversedIcons[i];
		}
	}
	public class IconFormatStyle : Freezable {
		public IconFormatStyle(IconSetFormat format, string formatName) {
			Format = format;
			FormatName = formatName;
			Icons = format.Elements.Select(x => x.Icon).ToArray();
		}
		internal string FormatName { get; private set; }
		internal IconSetFormat Format { get; private set; }
		public ImageSource[] Icons { get; private set; }
		protected override Freezable CreateInstanceCore() {
			return new IconFormatStyle(Format, FormatName);
		}
	}
	public class IconItemViewModel {
		public static Func<IconViewModel, IconItemViewModel> Factory { get { return ViewModelSource.Factory((IconViewModel x) => new IconItemViewModel(x)); } }
		IconViewModel owner;
		protected IconItemViewModel(IconViewModel owner) {
			this.owner = owner;
		}
		public virtual ImageSource Icon { get; set; }
		public virtual IEnumerable<ImageSource> Icons { get; set; }
		[BindableProperty(OnPropertyChangedMethodName = "UpdateNextItemDescription")]
		public virtual IconComparisonType ComparisonType { get; set; }
		[BindableProperty(OnPropertyChangedMethodName = "UpdateNextItemDescription")]
		public virtual double Threshold { get; set; }
		public virtual string Description { get; set; }
		public bool HasBottomLimit { get { return Threshold != double.NegativeInfinity; } }
		internal IconSetElement CreateElement() {
			return new IconSetElement { Threshold = Threshold, ThresholdComparisonType = GetThresholdComparisonType(), Icon = Icon };
		}
		ThresholdComparisonType GetThresholdComparisonType() {
			switch(ComparisonType) {
				case IconComparisonType.Greater:
					return ThresholdComparisonType.Greater;
				case IconComparisonType.GreaterOrEqual:
					return ThresholdComparisonType.GreaterOrEqual;
				default:
					throw new InvalidOperationException();
			}
		}
		protected void UpdateNextItemDescription() {
			owner.UpdateNextItemDescription(this);
		}
	}
	public class IconItemViewModelCollection : ObservableCollectionCore<IconItemViewModel> { }
}
