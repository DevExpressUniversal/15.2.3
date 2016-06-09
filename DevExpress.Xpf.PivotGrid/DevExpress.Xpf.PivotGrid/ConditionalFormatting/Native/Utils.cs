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
using System.Collections.Specialized;
using System.Windows;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
using DevExpress.Xpf.Core.ConditionalFormattingManager;
using DevExpress.Xpf.Editors.Filtering;
using DevExpress.XtraPivotGrid.Localization;
namespace DevExpress.Xpf.PivotGrid.Internal {
	public interface IFormatConditionCollectionOwner {
		void OnFormatConditionCollectionChanged(FormatConditionChangeType changeType);
		void SyncFormatCondtitionPropertyWithDetails(FormatConditionBase item, DependencyPropertyChangedEventArgs e);
		void SyncFormatCondtitionCollectionWithDetails(NotifyCollectionChangedEventArgs e);
	}
	public class DataControlDialogContext : IDialogContext, IFilteredComponent {
		public const string AnyFieldNameFieldName = "PivotGridAnyFieldNameFieldName";
		public const string GrandTotalNameFieldName = "PivotGridGrandTotalNameFieldName";
		class DataColumnInfoWrapper : IDataColumnInfo {
			readonly IDataColumnInfo field;
			List<IDataColumnInfo> columns;
			public DataColumnInfoWrapper(PivotGridField field, List<IDataColumnInfo> columns) {
				this.field = field.InternalField;
				this.columns = new List<IDataColumnInfo>();
				foreach(IDataColumnInfo info in columns)
					this.columns.Add(new IDataColumnInfo2(info, columns));
			}
			string IDataColumnInfo.Caption { get { return field.Caption; } }
			List<IDataColumnInfo> IDataColumnInfo.Columns { get { return columns; } }
			DataControllerBase IDataColumnInfo.Controller { get { return field.Controller; } }
			string IDataColumnInfo.FieldName { get { return field.Name; } }
			Type IDataColumnInfo.FieldType { get { return field.FieldType; } }
			string IDataColumnInfo.Name { get { return field.Name; } }
			string IDataColumnInfo.UnboundExpression { get { return field.UnboundExpression; } }
		}
		class IDataColumnInfo2 : IDataColumnInfo {
			IDataColumnInfo info;
			List<IDataColumnInfo> columns;
			public IDataColumnInfo2(IDataColumnInfo info, List<IDataColumnInfo> columns) {
				this.info = info;
				this.columns = columns;
			}
			string IDataColumnInfo.Caption {
				get { return info.Caption; }
			}
			List<IDataColumnInfo> IDataColumnInfo.Columns {
				get { return columns; }
			}
			DataControllerBase IDataColumnInfo.Controller {
				get { return info.Controller; }
			}
			string IDataColumnInfo.FieldName {
				get { return info.Name; }
			}
			Type IDataColumnInfo.FieldType {
				get { return info.FieldType; }
			}
			string IDataColumnInfo.Name {
				get { return info.Name; }
			}
			string IDataColumnInfo.UnboundExpression {
				get { return info.UnboundExpression; }
			}
		}
		PivotGridField field;
		IDesignTimeAdorner adorner;
		FilterColumn filterColumn;
		DataColumnInfoWrapper columnInfoWrapper;
		IConditionModelItemsBuilder builder;
		CriteriaOperator criteria;
		FormatConditionCommandParameters settings;
		public DataControlDialogContext(PivotGridField field, FormatConditionCommandParameters settings) {
			this.field = field;
			this.adorner = field.PivotGrid.DesignTimeAdorner;
			this.settings = settings;
			this.filterColumn = field.CreateFilterColumn();
			this.columnInfoWrapper = new DataColumnInfoWrapper(field, GetColumns());
			builder = new ConditionModelItemsBuilder(EditingContext, settings);
		}
		IModelItem GetParentModel() {
			return field.PivotGrid.GetParentModel();
		}
		IModelProperty GetConditions() {
			return GetParentModel().Properties["FormatConditions"];
		}
		List<IDataColumnInfo> GetColumns() {
			var columns = new List<IDataColumnInfo>();
			foreach(PivotGridField field2 in field.Data.Fields)
				if(!string.IsNullOrEmpty(field2.Name))
					columns.Add(field2.InternalField);
			return columns;
		}
		#region IDialogContext Members
		public IDataColumnInfo ColumnInfo {
			get { return columnInfoWrapper; }
		}
		public IModelItem CreateModelItem(object obj) {
			EditingContextBase context = GetParentModel().Context as EditingContextBase;
			return context != null ? context.CreateModelItem(obj, GetParentModel()) : null;
		}
		public IEditingContext EditingContext {
			get { return GetParentModel().Context; }
		}
		public FilterColumn FilterColumn {
			get { return filterColumn; }
		}
		public IFilteredComponent FilteredComponent {
			get { return this; }
		}
		public IFormatsOwner PredefinedFormatsOwner {
			get { return field.PivotGrid; }
		}
		public bool IsDesignTime {
			get { return adorner.IsDesignTime; }
		}
		public IModelItem GetRootModelItem() {
			return GetParentModel();
		}
		public IDialogContext Find(string name) {
			PivotGridField targetColumn = field.PivotGrid.Fields[name];
			if(targetColumn != null)
				return new DataControlDialogContext(targetColumn, settings);
			return null;
		}
		public IConditionModelItemsBuilder Builder {
			get { return builder; }
		}
		public IModelProperty Conditions {
			get { return GetConditions(); }
		}
		#endregion
		IEnumerable<FilterColumn> IFilteredComponent.CreateFilterColumnCollection() {
			foreach(PivotGridField field2 in field.PivotGrid.Fields) {
				if(field2.Area == FieldArea.FilterArea || !field2.Visible)
					continue;
				yield return field2.CreateFilterColumn();
			}
		}
		event EventHandler Data.Filtering.IFilteredComponentBase.PropertiesChanged {
			add { }
			remove { }
		}
		event EventHandler Data.Filtering.IFilteredComponentBase.RowFilterChanged {
			add { }
			remove { }
		}
		CriteriaOperator IFilteredComponentBase.RowCriteria {
			get { return criteria; }
			set { criteria = value; }
		}
		bool IDialogContext.IsPivot { get { return true; } }
		string IDialogContext.ApplyToFieldNameCaption { get { return PivotGridLocalizer.GetString(PivotGridStringId.PopupMenuFormatRulesMeasure); } }
		string IDialogContext.ApplyToPivotRowCaption { get { return PivotGridLocalizer.GetString(PivotGridStringId.PopupMenuFormatRulesRow); } }
		string IDialogContext.ApplyToPivotColumnCaption { get { return PivotGridLocalizer.GetString(PivotGridStringId.PopupMenuFormatRulesColumn); } }
		IEnumerable<FieldNameWrapper> IDialogContext.PivotSpecialFieldNames {
			get {
				yield return new FieldNameWrapper(GrandTotalNameFieldName, PivotGridLocalizer.GetString(PivotGridStringId.PopupMenuFormatRulesGrandTotal));
				yield return new FieldNameWrapper(AnyFieldNameFieldName, PivotGridLocalizer.GetString(PivotGridStringId.PopupMenuFormatRulesAnyField));
			}
		}
	}
	public class ConditionModelItemsBuilder : IConditionModelItemsBuilder {
		IEditingContext context;
		FormatConditionCommandParameters settings;
		public ConditionModelItemsBuilder(IEditingContext context, FormatConditionCommandParameters settings) {
			this.context = context;
			this.settings = settings;
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
			ApplyToSpecificLevelIfNecessary(item);
			return item;
		}
		#endregion
		IModelItem GetEditableCondition(IModelItem source, Type type) {
			IModelItem item = source ?? context.CreateItem(type);
			if(settings != null) {
				if(settings.Row != null)
					item.Properties[FormatConditionBase.RowNameProperty.Name].SetValue(settings.Row.Name);
				if(settings.Column != null)
					item.Properties[FormatConditionBase.ColumnNameProperty.Name].SetValue(settings.Column.Name);
				item.Properties[FormatConditionBase.MeasureNameProperty.Name].SetValue(settings.Measure == null ? null : settings.Measure.Name);
			}
			return item;
		}
		void SetBaseProperties(IModelItem item, BaseEditUnit unit) {
			unit.SetModelItemProperty<BaseEditUnit, string>(item, FormatConditionBase.MeasureNameProperty, x => x.FieldName);
			unit.SetModelItemProperty<BaseEditUnit, string>(item, FormatConditionBase.ExpressionProperty, x => x.Expression);
			unit.SetModelItemProperty<BaseEditUnit, string>(item, FormatConditionBase.PredefinedFormatNameProperty, x => x.PredefinedFormatName);
			unit.SetModelItemProperty<BaseEditUnit, bool>(item, FormatConditionBase.ApplyToSpecificLevelProperty, x => x.ApplyToRow);
			if(settings.IsManagerRule) {
				if(unit.RowName == DataControlDialogContext.AnyFieldNameFieldName || unit.RowName == DataControlDialogContext.GrandTotalNameFieldName)
					item.Properties[FormatConditionBase.RowNameProperty.Name].ClearValue();
				else
					unit.SetModelItemProperty<BaseEditUnit, string>(item, FormatConditionBase.RowNameProperty, x => x.RowName);
				if(unit.ColumnName == DataControlDialogContext.AnyFieldNameFieldName || unit.ColumnName == DataControlDialogContext.GrandTotalNameFieldName)
					item.Properties[FormatConditionBase.ColumnNameProperty.Name].ClearValue();
				else
					unit.SetModelItemProperty<BaseEditUnit, string>(item, FormatConditionBase.ColumnNameProperty, x => x.ColumnName);
				bool? specificLevel = !(unit.RowName == DataControlDialogContext.AnyFieldNameFieldName && unit.ColumnName == DataControlDialogContext.AnyFieldNameFieldName);
				if((item.Properties["ApplyToSpecificLevel"].ComputedValue as bool?) != specificLevel)
					item.Properties[FormatConditionBase.ApplyToSpecificLevelProperty.Name].SetValue(specificLevel);
			}
			Freezable format = unit.GetFormat();
			if(format != null && !Object.Equals(format, item.Properties["Format"].ComputedValue))
				item.Properties[FormatConditionBase.PredefinedFormatNameProperty.Name].ClearValue();
		}
		void SetConditionProperties(IModelItem item, ConditionEditUnit unit) {
			SetBaseProperties(item, unit);
			unit.SetModelItemProperty<ConditionEditUnit, Format>(item, FormatCondition.FormatProperty, x => x.Format);
			unit.SetModelItemProperty<ConditionEditUnit, object>(item, FormatCondition.Value1Property, x => x.Value1);
			unit.SetModelItemProperty<ConditionEditUnit, object>(item, FormatCondition.Value2Property, x => x.Value2);
			unit.SetModelItemProperty<ConditionEditUnit, ConditionRule>(item, FormatCondition.ValueRuleProperty, x => x.ValueRule);
		}
		void SetIndicatorProperties(IModelItem item, IndicatorEditUnit unit) {
			SetBaseProperties(item, unit);
			unit.SetModelItemProperty<IndicatorEditUnit, object>(item, IndicatorFormatConditionBase.MinValueProperty, x => x.MinValue);
			unit.SetModelItemProperty<IndicatorEditUnit, object>(item, IndicatorFormatConditionBase.MaxValueProperty, x => x.MaxValue);
			ApplyToSpecificLevelIfNecessary(item);
		}
		void ApplyToSpecificLevelIfNecessary(IModelItem item) {
			if(!settings.IsManagerRule)
				item.Properties[FormatConditionBase.ApplyToSpecificLevelProperty.Name].SetValue(true);
		}
	}
}
