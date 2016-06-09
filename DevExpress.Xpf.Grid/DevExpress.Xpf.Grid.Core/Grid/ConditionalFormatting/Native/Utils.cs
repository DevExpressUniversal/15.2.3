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

using DevExpress.Xpf.Core.ConditionalFormatting.Native;
using System;
using DevExpress.Xpf.Grid.Native;
using System.Linq;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Data;
using DevExpress.Data;
using DevExpress.Xpf.Core.Native;
using System.Windows;
using DevExpress.Xpf.Core.ConditionalFormatting;
using System.Collections.Generic;
using DevExpress.Xpf.Core;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using DevExpress.Xpf.Editors.Filtering;
using System.Collections.Specialized;
using DevExpress.Xpf.Core.ConditionalFormattingManager;
using System.Linq.Expressions;
using DevExpress.Mvvm;
using System.Windows.Controls;
using System.Windows.Media;
namespace DevExpress.Xpf.Grid.Native {
	public class DataTreeBuilderFormatInfoProvider<T> : IFormatInfoProvider {
		readonly T owner;
		readonly Func<T, string, object> valueAccessor;
		readonly Func<T, DataTreeBuilder> treeBuilderAccessor;
		DataTreeBuilder TreeBuilder { get { return treeBuilderAccessor(owner); } }
		public DataTreeBuilderFormatInfoProvider(T owner, Func<T, DataTreeBuilder> treeBuilderAccessor, Func<T, string, object> valueAccessor) {
			this.owner = owner;
			this.treeBuilderAccessor = treeBuilderAccessor;
			this.valueAccessor = valueAccessor;
		}
		object IFormatInfoProvider.GetCellValue(string fieldName) {
			return valueAccessor(owner, fieldName);
		}
		object IFormatInfoProvider.GetCellValueByListIndex(int listIndex, string fieldName) {
			return TreeBuilder.View.DataControl.DataProviderBase.GetFormatInfoCellValueByListIndex(listIndex, fieldName);
		}
		object IFormatInfoProvider.GetTotalSummaryValue(string fieldName, ConditionalFormatSummaryType summaryType) {
			var summary = TreeBuilder.View.ViewBehavior.GetServiceSummaries().FirstOrDefault(x => ConditionalFormatSummaryInfoHelper.AreSameItems(x, summaryType, fieldName, TreeBuilder.View.DataControl.DataProviderBase));
			return summary.With(x => TreeBuilder.GetServiceTotalSummaryValue(x));
		}
		DevExpress.Data.ValueComparer IFormatInfoProvider.ValueComparer {
			get { return TreeBuilder.View.DataControl.DataProviderBase.ValueComparer; }
		}
	}
	public static class ConditionalFormatSummaryInfoHelper {
		public static ServiceSummaryItem ToSummaryItem(ConditionalFormatSummaryType summaryType, string fieldName, DataProviderBase dataProvider) {
			Type type = GetType(fieldName, dataProvider);
			return new ServiceSummaryItem()
			{
				SummaryType = ToSummaryItemType(summaryType, type),
				FieldName = fieldName,
				CustomServiceSummaryItemType = ToCustomServiceSummaryItemType(summaryType, type),
			};
		}
		public static bool AreSameItems(ServiceSummaryItem item, ConditionalFormatSummaryType summaryType, string fieldName, DataProviderBase dataProvider) {
			Type type = GetType(fieldName, dataProvider);
			return item.FieldName == fieldName &&
				item.SummaryType == ToSummaryItemType(summaryType, type) &&
				item.CustomServiceSummaryItemType == ToCustomServiceSummaryItemType(summaryType, type);
		}
		static Type GetType(string fieldName, DataProviderBase dataProvider) {
			return dataProvider.GetActualColumnInfo(fieldName).With(x => x.Type);
		}
		static CustomServiceSummaryItemType? ToCustomServiceSummaryItemType(ConditionalFormatSummaryType summaryType, Type type) {
			if(summaryType == ConditionalFormatSummaryType.SortedList)
				return CustomServiceSummaryItemType.SortedList;
			if(summaryType == ConditionalFormatSummaryType.Average && type == typeof(DateTime))
				return CustomServiceSummaryItemType.DateTimeAverage;
			return null;
		}
		static SummaryItemType ToSummaryItemType(ConditionalFormatSummaryType summaryType, Type type) {
			switch(summaryType) {
				case ConditionalFormatSummaryType.Min:
					return SummaryItemType.Min;
				case ConditionalFormatSummaryType.Max:
					return SummaryItemType.Max;
				case ConditionalFormatSummaryType.Average:
					return type != typeof(DateTime) ? SummaryItemType.Average : SummaryItemType.Custom;
				case ConditionalFormatSummaryType.SortedList:
					return SummaryItemType.Custom;
				default:
					throw new InvalidOperationException();
			}
		}
	}
	public interface IFormatConditionCollectionOwner {
		void OnFormatConditionCollectionChanged(FormatConditionChangeType changeType);
		void SyncFormatCondtitionPropertyWithDetails(FormatConditionBase item, DependencyPropertyChangedEventArgs e);
		void SyncFormatCondtitionCollectionWithDetails(NotifyCollectionChangedEventArgs e);
	}
	public class DataControlDialogContext : IDialogContext {
		class DataColumnInfoWrapper : IDataColumnInfo {
			readonly IDataColumnInfo column;
			List<IDataColumnInfo> columns;
			public DataColumnInfoWrapper(IDataColumnInfo column, List<IDataColumnInfo> columns) {
				this.column = column;
				this.columns = columns;
			}
			string IDataColumnInfo.Caption { get { return column.Caption; } }
			List<IDataColumnInfo> IDataColumnInfo.Columns { get { return columns; } }
			DataControllerBase IDataColumnInfo.Controller { get { return column.Controller; } }
			string IDataColumnInfo.FieldName { get { return column.FieldName; } }
			Type IDataColumnInfo.FieldType { get { return column.FieldType; } }
			string IDataColumnInfo.Name { get { return column.Name; } }
			string IDataColumnInfo.UnboundExpression { get { return column.UnboundExpression; } }
		}
		ColumnBase column;
		DataControlBase dataControl;
		ITableView view;
		IDesignTimeAdornerBase adorner;
		FilterColumn filterColumn;
		DataColumnInfoWrapper columnInfoWrapper;
		IConditionModelItemsBuilder builder;
		public DataControlDialogContext(ColumnBase column) {
			this.column = column;
			this.dataControl = column.OwnerControl;
			this.view = (ITableView)dataControl.viewCore;
			this.adorner = dataControl.DesignTimeAdorner;
			this.filterColumn = dataControl.GetFilterColumnFromGridColumn(column);
			this.columnInfoWrapper = new DataColumnInfoWrapper(column, GetColumns());
			builder = new ConditionModelItemsBuilder(EditingContext);
		}
		IModelItem GetParentModel() {
			return adorner.GetDataControlModelItem(dataControl);
		}
		IModelProperty GetConditions() {
			var parent = GetParentModel();
			IModelItem view = parent.Properties["View"].Value;
			return view.Properties["FormatConditions"];
		}
		List<IDataColumnInfo> GetColumns() {
			var columns = new List<IDataColumnInfo>();
			foreach(IDataColumnInfo column in dataControl.ColumnsCore)
				columns.Add(column);
			return columns;
		}
		#region IDialogContext Members
		public IDataColumnInfo ColumnInfo {
			get { return columnInfoWrapper; }
		}
		public IModelItem CreateModelItem(object obj) {
			return adorner.CreateModelItem(obj, GetParentModel());
		}
		public IEditingContext EditingContext {
			get { return GetParentModel().Context; }
		}
		public FilterColumn FilterColumn {
			get { return filterColumn; }
		}
		public IFilteredComponent FilteredComponent {
			get { return dataControl.FilteredComponent; }
		}
		public IFormatsOwner PredefinedFormatsOwner {
			get { return view; }
		}
		public bool IsDesignTime {
			get { return adorner.IsDesignTime; }
		}
		public IModelItem GetRootModelItem() {
			return adorner.GetDataControlModelItem(dataControl);
		}
		public IDialogContext Find(string name) {
			ColumnBase targetColumn = dataControl.ColumnsCore[name];
			if(targetColumn != null)
				return new DataControlDialogContext(targetColumn);
			return null;
		}
		public IConditionModelItemsBuilder Builder {
			get { return builder; }
		}
		public IModelProperty Conditions {
			get { return GetConditions(); }
		}
		bool IDialogContext.IsPivot { get { return false; } }
		string IDialogContext.ApplyToFieldNameCaption { get { return ConditionalFormattingLocalizer.GetString(ConditionalFormattingStringId.ConditionalFormatting_Manager_AppliesTo); } }
		string IDialogContext.ApplyToPivotColumnCaption { get { return null; } }
		string IDialogContext.ApplyToPivotRowCaption { get { return null; } }
		IEnumerable<FieldNameWrapper> IDialogContext.PivotSpecialFieldNames { get { return null; } }
		#endregion
	}
	public class ConditionModelItemsBuilder : IConditionModelItemsBuilder {
		IEditingContext context;
		public ConditionModelItemsBuilder(IEditingContext context) {
			this.context = context;
		}
		#region IConditionModelItemsBuilder Members
		public IModelItem BuildColorScaleCondition(ColorScaleEditUnit unit, IModelItem source) {
			IModelItem item = GetEditableCondition(source, typeof(ColorScaleFormatCondition));
			SetIndicatorProperties(item, unit);
			unit.SetModelItemProperty<ColorScaleEditUnit, ColorScaleFormat>(item, ColorScaleFormatCondition.FormatProperty, x => x.Format);
			return item;
		}
		public IModelItem BuildCondition(ConditionEditUnit unit, IModelItem source) {
			IModelItem item = GetEditableCondition(source, typeof(FormatCondition));
			SetConditionProperties(item, unit);
			return item;
		}
		public IModelItem BuildDataBarCondition(DataBarEditUnit unit, IModelItem source) {
			IModelItem item = GetEditableCondition(source, typeof(DataBarFormatCondition));
			SetIndicatorProperties(item, unit);
			unit.SetModelItemProperty<DataBarEditUnit, DataBarFormat>(item, DataBarFormatCondition.FormatProperty, x => x.Format);
			return item;
		}
		public IModelItem BuildIconSetCondition(IconSetEditUnit unit, IModelItem source) {
			IModelItem item = GetEditableCondition(source, typeof(IconSetFormatCondition));
			unit.SetModelItemProperty<IconSetEditUnit, IconSetFormat>(item, IconSetFormatCondition.FormatProperty, x => x.Format);
			SetIndicatorProperties(item, unit);
			return item;
		}
		public IModelItem BuildTopBottomCondition(TopBottomEditUnit unit, IModelItem source) {
			IModelItem item = GetEditableCondition(source, typeof(TopBottomRuleFormatCondition));
			SetConditionProperties(item, unit);
			unit.SetModelItemProperty<TopBottomEditUnit, TopBottomRule>(item, TopBottomRuleFormatCondition.RuleProperty, x => x.Rule);
			unit.SetModelItemProperty<TopBottomEditUnit, double>(item, TopBottomRuleFormatCondition.ThresholdProperty, x => x.Threshold);
			return item;
		}
		#endregion
		IModelItem GetEditableCondition(IModelItem source, Type type) {
			return source ?? context.CreateItem(type);
		}
		void SetBaseProperties(IModelItem item, BaseEditUnit unit) {
			unit.SetModelItemProperty<BaseEditUnit, string>(item, FormatConditionBase.FieldNameProperty, x => x.FieldName);
			unit.SetModelItemProperty<BaseEditUnit, string>(item, FormatConditionBase.ExpressionProperty, x => x.Expression);
			unit.SetModelItemProperty<BaseEditUnit, string>(item, FormatConditionBase.PredefinedFormatNameProperty, x => x.PredefinedFormatName);
			Freezable format = unit.GetFormat();
			if(format != null && !Object.Equals(format, item.Properties["Format"].ComputedValue))
				item.Properties[FormatConditionBase.PredefinedFormatNameProperty.Name].ClearValue();
		}
		void SetConditionProperties(IModelItem item, ConditionEditUnit unit) {
			SetBaseProperties(item, unit);
			unit.SetModelItemProperty<ConditionEditUnit, bool>(item, FormatCondition.ApplyToRowProperty, x => x.ApplyToRow);
			unit.SetModelItemProperty<ConditionEditUnit, Format>(item, FormatCondition.FormatProperty, x => x.Format);
			unit.SetModelItemProperty<ConditionEditUnit, object>(item, FormatCondition.Value1Property, x => x.Value1);
			unit.SetModelItemProperty<ConditionEditUnit, object>(item, FormatCondition.Value2Property, x => x.Value2);
			unit.SetModelItemProperty<ConditionEditUnit, ConditionRule>(item, FormatCondition.ValueRuleProperty, x => x.ValueRule);
		}
		void SetIndicatorProperties(IModelItem item, IndicatorEditUnit unit) {
			SetBaseProperties(item, unit);
			unit.SetModelItemProperty<IndicatorEditUnit, object>(item, IndicatorFormatConditionBase.MinValueProperty, x => x.MinValue);
			unit.SetModelItemProperty<IndicatorEditUnit, object>(item, IndicatorFormatConditionBase.MaxValueProperty, x => x.MaxValue);
		}
	}
	public class ColorScalePreviewConverter : System.Windows.Data.IValueConverter {
		static Point startPoint = new Point(0, 0.5);
		static Point endPoint = new Point(1, 0.5);
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			var format = value as ColorScaleFormat;
			if(format == null)
				return null;
			var gradientStops = new GradientStopCollection();
			gradientStops.Add(new GradientStop(format.ColorMin, 0));
			if(format.ColorMiddle.HasValue)
				gradientStops.Add(new GradientStop(format.ColorMiddle.Value, 0.5));
			gradientStops.Add(new GradientStop(format.ColorMax, 1));
			return new LinearGradientBrush() { StartPoint = startPoint, EndPoint = endPoint, GradientStops = gradientStops };
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return null;
		}
	}
	public class PreviewTemplateSelector : DataTemplateSelector {
		public DataTemplate ColorTemplate { get; set; }
		public DataTemplate DataBarTemplate { get; set; }
		public DataTemplate IconSetTemplate { get; set; }
		public DataTemplate FormatTemplate { get; set; }
		public DataTemplate EmptyTemplate { get; set; }
		public override DataTemplate SelectTemplate(object item, DependencyObject container) {
			if(item is Format)
				return FormatTemplate;
			if(item is ColorScaleFormat)
				return ColorTemplate;
			if(item is DataBarFormat)
				return DataBarTemplate;
			if(item is IconFormatStyle)
				return IconSetTemplate;
			return EmptyTemplate;
		}
	}
}
