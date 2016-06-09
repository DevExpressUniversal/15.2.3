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
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DevExpress.Core;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Xpf.Utils;
using DevExpress.XtraGrid;
namespace DevExpress.Xpf.Grid {
	public partial class GridColumn : GridColumnBase, IDetailElement<BaseColumn> {
		protected internal static readonly DependencyPropertyKey IsGroupedPropertyKey;
		public static readonly DependencyProperty IsGroupedProperty;
		public static readonly DependencyProperty GroupIntervalProperty;
		public static readonly DependencyProperty GroupIndexProperty;
		public static readonly DependencyProperty GroupValueTemplateProperty;
		public static readonly DependencyProperty GroupValueTemplateSelectorProperty;
		static readonly DependencyPropertyKey ActualGroupValueTemplateSelectorPropertyKey;
		public static readonly DependencyProperty ActualGroupValueTemplateSelectorProperty;
		public static readonly DependencyProperty AllowGroupingProperty;
		static readonly DependencyPropertyKey ActualAllowGroupingPropertyKey;
		public static readonly DependencyProperty ActualAllowGroupingProperty;
		public static readonly DependencyProperty ShowGroupedColumnProperty;
		static GridColumn() {
			Type ownerType = typeof(GridColumn);
			IsGroupedPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsGrouped", typeof(bool), ownerType, new PropertyMetadata(false));
			IsGroupedProperty = IsGroupedPropertyKey.DependencyProperty;
			GroupIntervalProperty = DependencyPropertyManager.Register("GroupInterval", typeof(ColumnGroupInterval), ownerType, new FrameworkPropertyMetadata(ColumnGroupInterval.Default, new PropertyChangedCallback((d, e) => ((GridColumn)d).OnGroupIntervalChanged())));
			GroupIndexProperty = DependencyPropertyManager.Register("GroupIndex", typeof(int), ownerType, new PropertyMetadata(-1, OnGroupIndexChanged));
			GroupValueTemplateProperty = DependencyPropertyManager.Register("GroupValueTemplate", typeof(DataTemplate), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((GridColumn)d).UpdateActualGroupValueTemplateSelector()));
			GroupValueTemplateSelectorProperty = DependencyPropertyManager.Register("GroupValueTemplateSelector", typeof(DataTemplateSelector), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((GridColumn)d).UpdateActualGroupValueTemplateSelector()));
			ActualGroupValueTemplateSelectorPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualGroupValueTemplateSelector", typeof(DataTemplateSelector), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((GridColumn)d).OnActualGroupValueTemplateSelectorChanged()));
			ActualGroupValueTemplateSelectorProperty = ActualGroupValueTemplateSelectorPropertyKey.DependencyProperty;
			AllowGroupingProperty = DependencyPropertyManager.Register("AllowGrouping", typeof(DefaultBoolean), ownerType, new FrameworkPropertyMetadata(DefaultBoolean.Default, (d, e) => ((GridColumn)d).UpdateActualAllowGrouping()));
			ActualAllowGroupingPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualAllowGrouping", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			ActualAllowGroupingProperty = ActualAllowGroupingPropertyKey.DependencyProperty;
			ShowGroupedColumnProperty = DependencyPropertyManager.Register("ShowGroupedColumn", typeof(DefaultBoolean), ownerType, new PropertyMetadata(DefaultBoolean.Default, (d, e) => ((GridColumn)d).UpdateActualShowGroupedColumn()));
		}
		static void OnGroupIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			GridColumn column = d as GridColumn;
			if(column != null)
				column.OnGroupIndexChanged();
		}
		protected internal override bool ActualAllowGroupingCore {
			get {
				return ActualAllowGrouping; 
			}
		}
		internal override int GroupIndexCore { get { return GroupIndex; } set { GroupIndex = value; } }
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridColumnIsGrouped")]
#endif
		public bool IsGrouped { get { return (bool)GetValue(IsGroupedProperty); } }
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("GridColumnGroupInterval"),
#endif
 DefaultValue(ColumnGroupInterval.Default), Category(Categories.Data), XtraSerializableProperty, GridUIProperty]
		public ColumnGroupInterval GroupInterval {
			get { return (ColumnGroupInterval)GetValue(GroupIntervalProperty); }
			set { SetValue(GroupIntervalProperty, value); }
		}
		[Browsable(false)]
		public int GroupIndex {
			get { return (int)GetValue(GroupIndexProperty); }
			set { SetValue(GroupIndexProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("GridColumnGroupValueTemplate"),
#endif
 Category(Categories.Appearance)]
		public DataTemplate GroupValueTemplate {
			get { return (DataTemplate)GetValue(GroupValueTemplateProperty); }
			set { SetValue(GroupValueTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("GridColumnGroupValueTemplateSelector"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Category(Categories.Appearance)]
		public DataTemplateSelector GroupValueTemplateSelector {
			get { return (DataTemplateSelector)GetValue(GroupValueTemplateSelectorProperty); }
			set { SetValue(GroupValueTemplateSelectorProperty, value); }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridColumnActualGroupValueTemplateSelector")]
#endif
		public DataTemplateSelector ActualGroupValueTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ActualGroupValueTemplateSelectorProperty); }
			private set { this.SetValue(ActualGroupValueTemplateSelectorPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("GridColumnAllowGrouping"),
#endif
 Category(Categories.Layout), XtraSerializableProperty]
		public DefaultBoolean AllowGrouping {
			get { return (DefaultBoolean)GetValue(AllowGroupingProperty); }
			set { SetValue(AllowGroupingProperty, value); }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridColumnActualAllowGrouping")]
#endif
		public bool ActualAllowGrouping {
			get { return (bool)GetValue(ActualAllowGroupingProperty); }
			private set { this.SetValue(ActualAllowGroupingPropertyKey, value); }
		}
		[ Category(Categories.OptionsView), XtraSerializableProperty]
		public DefaultBoolean ShowGroupedColumn {
			get { return (DefaultBoolean)GetValue(ShowGroupedColumnProperty); }
			set { SetValue(ShowGroupedColumnProperty, value); }
		}
		public event GridCellValidationEventHandler Validate;
		GridControl Grid { get { return (GridControl)OwnerControl; } }
		protected override IColumnCollection ParentCollection {
			get {
				if(TableView.IsCheckBoxSelectorColumn(FieldName))
					return null;
				return base.ParentCollection;
			}
		}
		internal override void UpdateViewInfo(bool updateDataPropertiesOnly = false) {
			base.UpdateViewInfo(updateDataPropertiesOnly);
			if(Grid != null && !updateDataPropertiesOnly)
				UpdateAutoFilterValue();
		}
		protected internal override void SetSortInfo(ColumnSortOrder sortOrder, bool isGrouped) {
			base.SetSortInfo(sortOrder, isGrouped);
			this.SetValue(IsGroupedPropertyKey, isGrouped);
		}		
		void OnGroupIndexChanged() {
			if(Grid != null)
				Grid.ApplyColumnGroupIndex(this);
		}
		internal void UpdateActualGroupValueTemplateSelector() {
			UpdateActualTemplateSelector(ActualGroupValueTemplateSelectorPropertyKey, GroupValueTemplateSelector, GroupValueTemplate, (s, t) => ActualTemplateSelectorWrapper.Combine(Owner.ActualGroupValueTemplateSelector, s, t));
		}
		protected override void OnOwnerChanged() {
			base.OnOwnerChanged();
			UpdateActualGroupValueTemplateSelector();
		}
		protected void OnGroupIntervalChanged() {
			if(Grid != null)
				Grid.NeedSynchronize = true;
			OnDataPropertyChanged();
		}
		bool CalcActualAllowGrouping() {
			if(!ActualAllowSorting)
				return false;
			return GetActualAllowGroupingCore();
		}
		internal override bool GetActualAllowGroupingCore() {
			return AllowGrouping.GetValue(Owner.AllowGrouping) && base.GetActualAllowGroupingCore();
		}		
		void UpdateActualAllowGrouping() {
			ActualAllowGrouping = CalcActualAllowGrouping();
		}
		protected override void UpdateActualAllowSorting() {
			base.UpdateActualAllowSorting();
			UpdateActualAllowGrouping();
		}
		void UpdateActualShowGroupedColumn() {
			if (View!= null)
				View.RebuildVisibleColumns();
		}
		protected internal override void OnValidation(GridRowValidationEventArgs e) {
			 ((GridColumn)GetEventTargetColumn()).OnValidationCore(e);			
		}
		void OnValidationCore(GridRowValidationEventArgs e) {
			if(Validate != null)
				Validate(this, (GridCellValidationEventArgs)e);
		}
		protected internal override object GetWaitIndicator() {
			return new ColumnWaitIndicator();
		}
		protected override void UpdateGroupingCore(string oldFieldName) {
			if(IsGrouped && Grid != null) {
				int lastGroupIndex = GroupIndex;
				ColumnSortOrder lastSortOrder = SortOrder;
				Grid.BeginDataUpdate();
				Owner.UngroupColumn(oldFieldName);
				if((FieldName != null) || (FieldName != string.Empty))
					Owner.GroupColumn(FieldName, lastGroupIndex, lastSortOrder);
				Grid.EndDataUpdate();
			}
		}
		#region MasterDetail
		#region IDetailElement Members
		BaseColumn IDetailElement<BaseColumn>.CreateNewInstance(params object[] args) {
			Type realType = this.GetType();
			GridColumn column = Activator.CreateInstance(realType) as GridColumn;
			column.IsCloned = true;
			column.isAutoDetectedUnboundType = isAutoDetectedUnboundType;
			return column;
		}
		#endregion
		#endregion
		protected override ColumnFilterInfoBase CreateColumnFilterInfo() {
			if(!ShoulCreateDateFilter()) return base.CreateColumnFilterInfo();
			switch(FilterPopupMode) {
				case FilterPopupMode.DateAlt:
					return new DateAltColumnFilterInfo(this);
				case FilterPopupMode.Date:
					return new DateColumnFilterInfo(this);
				case FilterPopupMode.DateCompact:
					return new DateCompactColumnFilterInfo(this);
				case FilterPopupMode.Default:
				case FilterPopupMode.DateSmart:
				default:
					return new DateSmartColumnFilterInfo(this);
			}
		}
		bool ShoulCreateDateFilter() {
			bool isDateTime = FieldType == typeof(DateTime) || FieldType == typeof(DateTime?);
			return (FilterPopupMode == FilterPopupMode.Default && isDateTime)
				|| FilterPopupMode == FilterPopupMode.Date
				|| FilterPopupMode == FilterPopupMode.DateAlt
				|| FilterPopupMode == FilterPopupMode.DateSmart
				|| FilterPopupMode == FilterPopupMode.DateCompact;
		}
		internal override bool IsServiceColumn() {
			return TableView.IsCheckBoxSelectorColumn(FieldName);
		}
		void OnActualGroupValueTemplateSelectorChanged() {
			if(View != null)
				View.UpdateRowData(rowData => rowData.UpdateClientGroupValueTemplateSelector());
		}
	}
	public class GridColumnToGridColumnDataConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			ColumnBase column = value as ColumnBase;
			if(column != null && column.View != null && column.View.HeadersData != null)
				return column.View.HeadersData.GetCellDataByColumn(column);
			return null;
		}
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
}
