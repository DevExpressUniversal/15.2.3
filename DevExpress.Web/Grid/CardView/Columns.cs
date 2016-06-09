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

using DevExpress.Data;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.Helpers;
using DevExpress.Utils;
using DevExpress.Web.Data;
using DevExpress.Web.Design;
using DevExpress.Web.FilterControl;
using DevExpress.Web.Internal;
using DevExpress.XtraGrid;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace DevExpress.Web {
	public class CardViewColumn : WebColumnBase, IWebGridDataColumn, IFilterColumn, IWebColumnInfo, IDataSourceViewSchemaAccessor, IWebGridDataColumnAdapterOwner {
		ITemplate dataItemTemplate, headerTemplate, editItemTemplate;
		public CardViewColumn() : this(string.Empty) { }
		public CardViewColumn(string fieldName) : this(fieldName, string.Empty) { }
		public CardViewColumn(string fieldName, string caption)
			: base() {
			ColumnAdapter = CreateColumnAdapter();
			FieldName = fieldName;
			Caption = caption;
			HeaderStyle = new CardViewHeaderStyle();
			ExportCellStyle = new CardViewExportAppearance();
		}
		protected internal GridDataColumnAdapter ColumnAdapter { get; private set; }
		[Browsable(false)]
		public virtual ASPxCardView CardView { get { return ColumnAdapter.Grid as ASPxCardView; } }
#if !SL
	[DevExpressWebLocalizedDescription("CardViewColumnShowInCustomizationForm")]
#endif
		public new bool ShowInCustomizationForm {
			get { return base.ShowInCustomizationForm; }
			set { base.ShowInCustomizationForm = value; }
		}
		[Category("Behavior"), Browsable(false), PersistenceMode(PersistenceMode.InnerProperty), RefreshProperties(RefreshProperties.All), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public virtual EditPropertiesBase PropertiesEdit { get { return ColumnAdapter.PropertiesEdit; } set { ColumnAdapter.PropertiesEdit = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewColumnSettings"),
#endif
 Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public CardViewDataColumnSettings Settings { get { return (CardViewDataColumnSettings)ColumnAdapter.Settings; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewColumnSettingsHeaderFilter"),
#endif
 Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public CardViewDataColumnHeaderFilterSettings SettingsHeaderFilter { get { return (CardViewDataColumnHeaderFilterSettings)ColumnAdapter.SettingsHeaderFilter; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Localizable(false)]
		public string FilterExpression { get { return ColumnAdapter.FilterExpression; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewColumnFieldName"),
#endif
 Category("Data"), DefaultValue(""), Localizable(false), RefreshProperties(RefreshProperties.Repaint), TypeConverter("DevExpress.Web.Design.GridViewFieldConverter, " + AssemblyInfo.SRAssemblyWebDesignFull), NotifyParentProperty(true)]
		public virtual string FieldName { get { return ColumnAdapter.FieldName; } set { ColumnAdapter.FieldName = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewColumnUnboundType"),
#endif
 Category("Data"), DefaultValue(UnboundColumnType.Bound), NotifyParentProperty(true)]
		public virtual UnboundColumnType UnboundType { get { return ColumnAdapter.UnboundType; } set { ColumnAdapter.UnboundType = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewColumnUnboundExpression"),
#endif
 Category("Data"), DefaultValue(""), NotifyParentProperty(true), Localizable(false), Editor("DevExpress.Web.Design.GridViewUnboundExpressionEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
		public virtual string UnboundExpression { get { return ColumnAdapter.UnboundExpression; } set { ColumnAdapter.UnboundExpression = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataColumnSortIndex"),
#endif
 Category("Data"), DefaultValue(-1), NotifyParentProperty(true)]
		public int SortIndex { get { return ColumnAdapter.SortIndex; } set { ColumnAdapter.SortIndex = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewColumnSortOrder"),
#endif
 Category("Data"), DefaultValue(ColumnSortOrder.None), NotifyParentProperty(true)]
		public virtual ColumnSortOrder SortOrder { get { return ColumnAdapter.SortOrder; } set { ColumnAdapter.SortOrder = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewColumnReadOnly"),
#endif
 Category("Behavior"), DefaultValue(false), NotifyParentProperty(true)]
		public virtual bool ReadOnly { get { return ColumnAdapter.ReadOnly; } set { ColumnAdapter.ReadOnly = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ShowInFilterControl { get { return ColumnAdapter.ShowInFilterControl; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewColumnExportCellStyle"),
#endif
 Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public CardViewExportAppearance ExportCellStyle { get; private set; }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewColumnHeaderStyle"),
#endif
 Category("Styles"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public virtual CardViewHeaderStyle HeaderStyle { get; private set; }
		[Category("Templates"), Browsable(false), DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(CardViewDataItemTemplateContainer)), NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate DataItemTemplate {
			get { return dataItemTemplate; }
			set {
				if(DataItemTemplate == value)
					return;
				dataItemTemplate = value;
				TemplatesChanged();
			}
		}
		[Category("Templates"), Browsable(false), DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(GridViewHeaderTemplateContainer)), NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate HeaderTemplate {
			get { return headerTemplate; }
			set {
				if(HeaderTemplate == value)
					return;
				headerTemplate = value;
				TemplatesChanged();
			}
		}
		[Category("Templates"), Browsable(false), DefaultValue(null), PersistenceMode(PersistenceMode.InnerProperty), TemplateContainer(typeof(CardViewEditItemTemplateContainer), BindingDirection.TwoWay), NotifyParentProperty(true), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual ITemplate EditItemTemplate {
			get { return editItemTemplate; }
			set {
				if(EditItemTemplate == value)
					return;
				editItemTemplate = value;
				TemplatesChanged();
			}
		}
		public void SortAscending() { ColumnAdapter.SortAscending(); }
		public void SortDescending() { ColumnAdapter.SortDescending(); }
		public void UnSort() { ColumnAdapter.UnSort(); }
		public override bool IsClickable() { return ColumnAdapter.AllowSort; }
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			var column = source as CardViewColumn;
			if(column != null) {
				ColumnAdapter.Assign(column.ColumnAdapter);
				HeaderStyle.Assign(column.HeaderStyle);
				ExportCellStyle.Assign(column.ExportCellStyle);
				DataItemTemplate = column.DataItemTemplate;
				EditItemTemplate = column.EditItemTemplate;
				HeaderTemplate = column.HeaderTemplate;
			}
		}
		protected virtual GridDataColumnAdapter CreateColumnAdapter() {
			return new GridDataColumnAdapter(this);
		}
		protected virtual EditPropertiesBase CreateEditProperties() {
			return IsDesignMode() ? new TextBoxProperties() : null;
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.MergedBaseAndNewStateManagedObjects(base.GetStateManagedObjects(), ColumnAdapter, HeaderStyle);
		}
		protected override string GetDesignTimeFieldName() { return FieldName; }
		protected override PropertiesBase GetDesignTimeItemEditProperties() { return PropertiesEdit; }
		protected override string GetDesignTimeCaption() { return ToString(); }
		protected override int GetDesignTimeVisibleIndex() { return VisibleIndex; }
		protected override void SetDesignTimeVisibleIndex(int index) { VisibleIndex = index; }
		protected override bool GetDesignTimeVisible() { return Visible; }
		protected override void SetDesignTimeVisible(bool visible) { Visible = visible; }
		public override string ToString() { return ColumnAdapter.GetDisplayName(); }
		#region IFilterColumn Members
		string IFilterablePropertyInfo.PropertyName { get { return (ColumnAdapter as IFilterColumn).PropertyName; } }
		string IFilterablePropertyInfo.DisplayName { get { return (ColumnAdapter as IFilterColumn).DisplayName; } }
		Type IFilterablePropertyInfo.PropertyType { get { return (ColumnAdapter as IFilterColumn).PropertyType; } }
		int IFilterColumn.Index { get { return (ColumnAdapter as IFilterColumn).Index; } }
		FilterColumnClauseClass IFilterColumn.ClauseClass { get { return (ColumnAdapter as IFilterColumn).ClauseClass; } }
		EditPropertiesBase IFilterColumn.PropertiesEdit { get { return (ColumnAdapter as IFilterColumn).PropertiesEdit; } }
		#endregion
		#region IWebColumnInfo Members
		ColumnSortOrder IWebColumnInfo.SortOrder { get { return SortOrder; } }
		string IWebColumnInfo.FieldName { get { return FieldName; } }
		UnboundColumnType IWebColumnInfo.UnboundType { get { return UnboundType; } }
		string IWebColumnInfo.UnboundExpression { get { return UnboundExpression; } }
		bool IWebColumnInfo.ReadOnly { get { return ReadOnly; } }
		ColumnGroupInterval IWebColumnInfo.GroupInterval { get { return ColumnGroupInterval.Default; } }
		EditPropertiesBase IWebColumnInfo.CreateEditProperties() { return CreateEditProperties(); }
		#endregion
		#region IDataSourceViewSchemaAccessor Members
		object IDataSourceViewSchemaAccessor.DataSourceViewSchema { get { return CardView is IDataSourceViewSchemaAccessor ? (CardView as IDataSourceViewSchemaAccessor).DataSourceViewSchema : null; } set { } }
		#endregion
		#region IWebGridDataColumn Members
		string IWebGridDataColumn.FieldName { get { return FieldName; } set { FieldName = value; } }
		UnboundColumnType IWebGridDataColumn.UnboundType { get { return UnboundType; } set { UnboundType = value; } }
		string IWebGridDataColumn.UnboundExpression { get { return UnboundExpression; } set { UnboundExpression = value; } }
		int IWebGridDataColumn.SortIndex { get { return SortIndex; } set { SortIndex = value; } }
		ColumnSortOrder IWebGridDataColumn.SortOrder { get { return SortOrder; } set { SortOrder = value; } }
		bool IWebGridDataColumn.ReadOnly { get { return ReadOnly; } set { ReadOnly = value; } }
		EditPropertiesBase IWebGridDataColumn.PropertiesEdit { get { return PropertiesEdit; } set { PropertiesEdit = value; } }
		GridDataColumnAdapter IWebGridDataColumn.Adapter { get { return ColumnAdapter; } }
		string IWebGridColumn.Name { get { return Name; } set { Name = value; } }
		string IWebGridColumn.Caption { get { return Caption; } set { Caption = value; } }
		int IWebGridColumn.VisibleIndex { get { return VisibleIndex; } set { VisibleIndex = value; } }
		bool IWebGridColumn.Visible { get { return Visible; } set { Visible = value; } }
		GridExportAppearanceBase IWebGridColumn.ExportCellStyle { get { return ExportCellStyle; } }
		#endregion
		#region IWebGridDataColumnAdapterOwner Members
		WebColumnBase IWebGridDataColumnAdapterOwner.Column { get { return this; } }
		bool IWebGridDataColumnAdapterOwner.HasGrouping { get { return false; } }
		bool IWebGridDataColumnAdapterOwner.HasAutoFilter { get { return false; } }
		Func<GridDataColumnAdapter, GridDataColumnSettings> IWebGridDataColumnAdapterOwner.CreateSettings {
			get { return a => new CardViewDataColumnSettings(a); }
		}
		Func<GridDataColumnAdapter, GridDataColumnHeaderFilterSettings> IWebGridDataColumnAdapterOwner.CreateSettingsHeaderFilter {
			get { return a => new CardViewDataColumnHeaderFilterSettings(a); }
		}
		#endregion
		#region IWebGridColumn Members
		#endregion
	}
	public class CardViewDataColumnSettings : GridDataColumnSettings {
		internal CardViewDataColumnSettings(GridDataColumnAdapter columnAdapter)
			: base(columnAdapter) {
		}
		public CardViewDataColumnSettings() : base() { }
		[DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true)]
		public new DefaultBoolean AllowEllipsisInText { get { return base.AllowEllipsisInText; } set { base.AllowEllipsisInText = value; } }
		[ DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true)]
		public new DefaultBoolean AllowSort { get { return base.AllowSort; } set { base.AllowSort = value; } }
		[ DefaultValue(ColumnSortMode.Default), NotifyParentProperty(true)]
		public new ColumnSortMode SortMode { get { return base.SortMode; } set { base.SortMode = value; } }
		[ DefaultValue(ColumnFilterMode.Value), NotifyParentProperty(true)]
		public new ColumnFilterMode FilterMode { get { return base.FilterMode; } set { base.FilterMode = value; } }
		[ DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true)]
		public new DefaultBoolean AllowFilterBySearchPanel { get { return base.AllowFilterBySearchPanel; } set { base.AllowFilterBySearchPanel = value; } }
		[ DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true)]
		public new DefaultBoolean AllowHeaderFilter { get { return base.AllowHeaderFilter; } set { base.AllowHeaderFilter = value; } }
		[ DefaultValue(HeaderFilterMode.List), NotifyParentProperty(true),
		 Obsolete("Use the SettingsHeaderFilter.Mode property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new HeaderFilterMode HeaderFilterMode { get { return base.HeaderFilterMode; } set { base.HeaderFilterMode = value; } }
		[ DefaultValue(DefaultBoolean.Default), NotifyParentProperty(true)]
		public new DefaultBoolean ShowInFilterControl { get { return base.ShowInFilterControl; } set { base.ShowInFilterControl = value; } }
	}
	public class CardViewDataColumnHeaderFilterSettings : GridDataColumnHeaderFilterSettings {
		internal CardViewDataColumnHeaderFilterSettings(GridDataColumnAdapter columnAdapter)
			: base(columnAdapter) {
		}
		public CardViewDataColumnHeaderFilterSettings() {
		}
		[ DefaultValue(GridHeaderFilterMode.Default), NotifyParentProperty(true)]
		public new GridHeaderFilterMode Mode { get { return base.Mode; } set { base.Mode = value; } }
		[ AutoFormatDisable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CardViewColumnDateRangeCalendarSettings DateRangeCalendarSettings { get { return (CardViewColumnDateRangeCalendarSettings)base.DateRangeCalendarSettings; } }
		[ AutoFormatDisable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CardViewColumnDateRangePickerSettings DateRangePickerSettings { get { return (CardViewColumnDateRangePickerSettings)base.DateRangePickerSettings; } }
		[ AutoFormatDisable, NotifyParentProperty(true),
		PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new CardViewColumnDateRangePeriodsSettings DateRangePeriodsSettings { get { return (CardViewColumnDateRangePeriodsSettings)base.DateRangePeriodsSettings; } }
		protected override GridColumnDateRangeCalendarSettings CreateCalendarSettings() {
			return new CardViewColumnDateRangeCalendarSettings();
		}
		protected override GridColumnDateRangePickerSettings CreateDateRangePickerSettings() {
			return new CardViewColumnDateRangePickerSettings();
		}
		protected override GridColumnDateRangePeriodsSettings CreateDateRangePeriodsSettings() {
			return new CardViewColumnDateRangePeriodsSettings();
		}
	}
	public class CardViewColumnDateRangeCalendarSettings : GridColumnDateRangeCalendarSettings {
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewColumnDateRangeCalendarSettingsClearButtonText"),
#endif
 DefaultValue(StringResources.Calendar_Clear), NotifyParentProperty(true)]
		public new string ClearButtonText { get { return base.ClearButtonText; } set { base.ClearButtonText = value; } }
		[ DefaultValue(StringResources.Calendar_Today), NotifyParentProperty(true)]
		public new string TodayButtonText { get { return base.TodayButtonText; } set { base.TodayButtonText = value; } }
		[
#if !SL
	DevExpressWebLocalizedDescription("CardViewColumnDateRangeCalendarSettingsShowClearButton"),
#endif
 DefaultValue(true), NotifyParentProperty(true)]
		public new bool ShowClearButton { get { return base.ShowClearButton; } set { base.ShowClearButton = value; } }
		[ DefaultValue(true), NotifyParentProperty(true)]
		public new bool ShowTodayButton { get { return base.ShowTodayButton; } set { base.ShowTodayButton = value; } }
		[ DefaultValue(true), NotifyParentProperty(true)]
		public new bool ShowHeader { get { return base.ShowHeader; } set { base.ShowHeader = value; } }
		[ DefaultValue(true), NotifyParentProperty(true)]
		public new bool ShowDayHeaders { get { return base.ShowDayHeaders; } set { base.ShowDayHeaders = value; } }
		[ DefaultValue(true), NotifyParentProperty(true)]
		public new bool ShowWeekNumbers { get { return base.ShowWeekNumbers; } set { base.ShowWeekNumbers = value; } }
		[ DefaultValue(true), NotifyParentProperty(true)]
		public new bool HighlightWeekends { get { return base.HighlightWeekends; } set { base.HighlightWeekends = value; } }
		[ DefaultValue(true), NotifyParentProperty(true)]
		public new bool HighlightToday { get { return base.HighlightToday; } set { base.HighlightToday = value; } }
		[ DefaultValue(true), NotifyParentProperty(true)]
		public new bool EnableMonthNavigation { get { return base.EnableMonthNavigation; } set { base.EnableMonthNavigation = value; } }
		[ DefaultValue(true), NotifyParentProperty(true)]
		public new bool EnableYearNavigation { get { return base.EnableYearNavigation; } set { base.EnableYearNavigation = value; } }
		[ DefaultValue(true), NotifyParentProperty(true)]
		public new bool EnableMultiSelect { get { return base.EnableMultiSelect; } set { base.EnableMultiSelect = value; } }
		[ DefaultValue(FirstDayOfWeek.Default), NotifyParentProperty(true)]
		public new FirstDayOfWeek FirstDayOfWeek { get { return base.FirstDayOfWeek; } set { base.FirstDayOfWeek = value; } }
		[ DefaultValue(typeof(DateTime), ""), NotifyParentProperty(true)]
		public new DateTime MinDate { get { return base.MinDate; } set { base.MinDate = value; } }
		[ DefaultValue(typeof(DateTime), ""), NotifyParentProperty(true)]
		public new DateTime MaxDate { get { return base.MaxDate; } set { base.MaxDate = value; } }
	}
	public class CardViewColumnDateRangePickerSettings : GridColumnDateRangePickerSettings {
		[ DefaultValue(DefaultDisplayFormatString), NotifyParentProperty(true)]
		public new string DisplayFormatString { get { return base.DisplayFormatString; } set { base.DisplayFormatString = value; } }
		[ DefaultValue(0), NotifyParentProperty(true)]
		public new int MinDayCount { get { return base.MinDayCount; } set { base.MinDayCount = value; } }
		[ DefaultValue(0), NotifyParentProperty(true)]
		public new int MaxDayCount { get { return base.MaxDayCount; } set { base.MaxDayCount = value; } }
	}
	public class CardViewColumnDateRangePeriodsSettings : GridColumnDateRangePeriodsSettings {
		[ DefaultValue(2), NotifyParentProperty(true)]
		public new int RepeatColumns { get { return base.RepeatColumns; } set { base.RepeatColumns = value; } }
		[ DefaultValue(true), NotifyParentProperty(true)]
		public new bool ShowDaysSection { get { return base.ShowDaysSection; } set { base.ShowDaysSection = value; } }
		[ DefaultValue(true), NotifyParentProperty(true)]
		public new bool ShowWeeksSection { get { return base.ShowWeeksSection; } set { base.ShowWeeksSection = value; } }
		[ DefaultValue(true), NotifyParentProperty(true)]
		public new bool ShowMonthsSection { get { return base.ShowMonthsSection; } set { base.ShowMonthsSection = value; } }
		[ DefaultValue(true), NotifyParentProperty(true)]
		public new bool ShowYearsSection { get { return base.ShowYearsSection; } set { base.ShowYearsSection = value; } }
		[ DefaultValue(true), NotifyParentProperty(true)]
		public new bool ShowPastPeriods { get { return base.ShowPastPeriods; } set { base.ShowPastPeriods = value; } }
		[ DefaultValue(true), NotifyParentProperty(true)]
		public new bool ShowFuturePeriods { get { return base.ShowFuturePeriods; } set { base.ShowFuturePeriods = value; } }
		[ DefaultValue(true), NotifyParentProperty(true)]
		public new bool ShowPresentPeriods { get { return base.ShowPresentPeriods; } set { base.ShowPresentPeriods = value; } }
	}
	[Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))]
	public class CardViewColumnCollection : GridColumnCollection {
		public CardViewColumnCollection(IWebControlObject webControlObject)
			: base(webControlObject) {
		}
		public new CardViewColumn this[int index] { get { return base[index] as CardViewColumn; } }
		public new CardViewColumn this[string ID_FieldName_Caption] { get { return base[ID_FieldName_Caption] as CardViewColumn; } }
		[Browsable(false)]
		public ASPxCardView CardView { get { return base.Grid as ASPxCardView; } }
		public void Add(CardViewColumn column) {
			base.Add(column);
		}
		public void AddRange(params CardViewColumn[] columns) {
			base.AddRange(columns);
		}
		public void Insert(int index, CardViewColumn column) {
			base.Insert(index, column);
		}
		public void Remove(CardViewColumn column) {
			base.Remove(column);
		}
		public int IndexOf(CardViewColumn column) {
			return base.IndexOf(column);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new void Add(IWebGridColumn column) {
			base.Add(column);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new void AddRange(params IWebGridColumn[] columns) {
			base.AddRange(columns);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new void Insert(int index, IWebGridColumn column) {
			base.Insert(index, column);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new void Remove(IWebGridColumn column) {
			base.Remove(column);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new int IndexOf(IWebGridColumn column) {
			return base.IndexOf(column);
		}
		protected override Type GetKnownType() {
			return typeof(CardViewColumn);
		}
	}
	[ControlBuilder(typeof(ControlBuilder))]
	public abstract class CardViewEditColumn : CardViewColumn {
		public CardViewEditColumn() : base(string.Empty) { }
		public CardViewEditColumn(string fieldName) : base(fieldName, string.Empty) { }
		public CardViewEditColumn(string fieldName, string caption) : base(fieldName, caption) { }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false), Localizable(false), PersistenceMode(PersistenceMode.Attribute)]
		public override EditPropertiesBase PropertiesEdit { get { return base.PropertiesEdit; } set { } }
		[ Category("Appearance"), DefaultValue(0), AutoFormatDisable, NotifyParentProperty(true)]
		public virtual int ExportWidth {
			get { return GetIntProperty("ExportWidth", 0); }
			set {
				CommonUtils.CheckNegativeValue(value, "ExportWidth");
				SetIntProperty("ExportWidth", 0, value);
			}
		}
		protected override EditPropertiesBase CreateEditProperties() { return null; }
		public static CardViewEditColumn CreateColumn(Type dataType) {
			if(dataType == null) return new CardViewTextColumn();
			dataType = ReflectionUtils.StripNullableType(dataType);
			if(dataType.Equals(typeof(DateTime))) return new CardViewDateColumn();
			if(dataType.Equals(typeof(bool))) return new CardViewCheckColumn();
			return new CardViewTextColumn();
		}
	}
	public class CardViewTextColumn : CardViewEditColumn {
		public CardViewTextColumn() : base(string.Empty) { }
		public CardViewTextColumn(string fieldName) : base(fieldName, string.Empty) { }
		public CardViewTextColumn(string fieldName, string caption) : base(fieldName, caption) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataTextColumnPropertiesTextEdit"),
#endif
 Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public TextBoxProperties PropertiesTextEdit { get { return (TextBoxProperties)PropertiesEdit; } }
		protected override EditPropertiesBase CreateEditProperties() { return new TextBoxProperties(this); }
		protected override string[] GetDesignTimeHiddenPropertyNames() { return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "PropertiesTextEdit" }); }
	}
	public class CardViewButtonEditColumn : CardViewEditColumn {
		public CardViewButtonEditColumn() : base(string.Empty) { }
		public CardViewButtonEditColumn(string fieldName) : base(fieldName, string.Empty) { }
		public CardViewButtonEditColumn(string fieldName, string caption) : base(fieldName, caption) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataButtonEditColumnPropertiesButtonEdit"),
#endif
 Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ButtonEditProperties PropertiesButtonEdit { get { return (ButtonEditProperties)PropertiesEdit; } }
		protected override EditPropertiesBase CreateEditProperties() { return new ButtonEditProperties(this); }
		protected override string[] GetDesignTimeHiddenPropertyNames() { return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "PropertiesButtonEdit" }); }
	}
	public class CardViewMemoColumn : CardViewEditColumn {
		public CardViewMemoColumn() : base(string.Empty) { }
		public CardViewMemoColumn(string fieldName) : base(fieldName, string.Empty) { }
		public CardViewMemoColumn(string fieldName, string caption) : base(fieldName, caption) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataMemoColumnPropertiesMemoEdit"),
#endif
 Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public MemoProperties PropertiesMemoEdit { get { return (MemoProperties)PropertiesEdit; } }
		protected override EditPropertiesBase CreateEditProperties() { return new MemoProperties(this); }
		protected override string[] GetDesignTimeHiddenPropertyNames() { return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "PropertiesMemoEdit" }); }
	}
	public class CardViewHyperLinkColumn : CardViewEditColumn {
		public CardViewHyperLinkColumn() : base(string.Empty) { }
		public CardViewHyperLinkColumn(string fieldName) : base(fieldName, string.Empty) { }
		public CardViewHyperLinkColumn(string fieldName, string caption) : base(fieldName, caption) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataHyperLinkColumnPropertiesHyperLinkEdit"),
#endif
 Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public HyperLinkProperties PropertiesHyperLinkEdit { get { return (HyperLinkProperties)PropertiesEdit; } }
		protected override EditPropertiesBase CreateEditProperties() { return new HyperLinkProperties(this); }
		protected override string[] GetDesignTimeHiddenPropertyNames() { return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "PropertiesHyperLinkEdit" }); }
	}
	public class CardViewCheckColumn : CardViewEditColumn {
		public CardViewCheckColumn() : base(string.Empty) { }
		public CardViewCheckColumn(string fieldName) : base(fieldName, string.Empty) { }
		public CardViewCheckColumn(string fieldName, string caption) : base(fieldName, caption) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataCheckColumnPropertiesCheckEdit"),
#endif
 Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public CheckBoxProperties PropertiesCheckEdit { get { return (CheckBoxProperties)PropertiesEdit; } }
		protected override EditPropertiesBase CreateEditProperties() { return new CheckBoxProperties(this); }
		protected override string[] GetDesignTimeHiddenPropertyNames() { return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "PropertiesCheckEdit" }); }
	}
	public class CardViewDateColumn : CardViewEditColumn, IDateEditIDResolver {
		public CardViewDateColumn() : base(string.Empty) { }
		public CardViewDateColumn(string fieldName) : base(fieldName, string.Empty) { }
		public CardViewDateColumn(string fieldName, string caption) : base(fieldName, caption) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataDateColumnPropertiesDateEdit"),
#endif
 Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public DateEditProperties PropertiesDateEdit { get { return (DateEditProperties)PropertiesEdit; } }
		protected override EditPropertiesBase CreateEditProperties() { return new DateEditProperties(this); }
		protected override string[] GetDesignTimeHiddenPropertyNames() { return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "PropertiesDateEdit" }); }
		string IDateEditIDResolver.GetDateEditIdByDataItemName(string dataItemName) { return (ColumnAdapter as IDateEditIDResolver).GetDateEditIdByDataItemName(dataItemName); }
		string[] IDateEditIDResolver.GetPossibleDataItemNames() { return (ColumnAdapter as IDateEditIDResolver).GetPossibleDataItemNames(); }
	}
	public class CardViewSpinEditColumn : CardViewEditColumn {
		public CardViewSpinEditColumn() : base(string.Empty) { }
		public CardViewSpinEditColumn(string fieldName) : base(fieldName, string.Empty) { }
		public CardViewSpinEditColumn(string fieldName, string caption) : base(fieldName, caption) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataSpinEditColumnPropertiesSpinEdit"),
#endif
 Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public SpinEditProperties PropertiesSpinEdit { get { return (SpinEditProperties)PropertiesEdit; } }
		protected override EditPropertiesBase CreateEditProperties() { return new SpinEditProperties(this); }
		protected override string[] GetDesignTimeHiddenPropertyNames() { return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "PropertiesSpinEdit" }); }
	}
	public class CardViewComboBoxColumn : CardViewEditColumn {
		public CardViewComboBoxColumn() : base(string.Empty) { }
		public CardViewComboBoxColumn(string fieldName) : base(fieldName, string.Empty) { }
		public CardViewComboBoxColumn(string fieldName, string caption) : base(fieldName, caption) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataComboBoxColumnPropertiesComboBox"),
#endif
 Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ComboBoxProperties PropertiesComboBox { get { return (ComboBoxProperties)PropertiesEdit; } }
		protected override EditPropertiesBase CreateEditProperties() { return new ComboBoxProperties(this); }
		protected override string[] GetDesignTimeHiddenPropertyNames() { return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "PropertiesComboBox" }); }
	}
	public class CardViewTokenBoxColumn : CardViewEditColumn { 
		public CardViewTokenBoxColumn() : base(string.Empty) { }
		public CardViewTokenBoxColumn(string fieldName) : base(fieldName, string.Empty) { }
		public CardViewTokenBoxColumn(string fieldName, string caption) : base(fieldName, caption) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataTokenBoxColumnPropertiesTokenBox"),
#endif
 Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public TokenBoxProperties PropertiesTokenBox { get { return (TokenBoxProperties)PropertiesEdit; } }
		protected override EditPropertiesBase CreateEditProperties() { return new TokenBoxProperties(this); }
		protected override string[] GetDesignTimeHiddenPropertyNames() { return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "PropertiesTokenBox" }); }
	}
	public class CardViewDropDownEditColumn : CardViewEditColumn {
		public CardViewDropDownEditColumn() : base(string.Empty) { }
		public CardViewDropDownEditColumn(string fieldName) : base(fieldName, string.Empty) { }
		public CardViewDropDownEditColumn(string fieldName, string caption) : base(fieldName, caption) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataDropDownEditColumnPropertiesDropDownEdit"),
#endif
 Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public DropDownEditProperties PropertiesDropDownEdit { get { return (DropDownEditProperties)PropertiesEdit; } }
		protected override EditPropertiesBase CreateEditProperties() { return new DropDownEditProperties(this); }
		protected override string[] GetDesignTimeHiddenPropertyNames() { return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "PropertiesDropDownEdit" }); }
	}
	public class CardViewImageColumn : CardViewEditColumn {
		public CardViewImageColumn() : base(string.Empty) { }
		public CardViewImageColumn(string fieldName) : base(fieldName, string.Empty) { }
		public CardViewImageColumn(string fieldName, string caption) : base(fieldName, caption) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataImageColumnPropertiesImage"),
#endif
 Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ImageEditProperties PropertiesImage { get { return (ImageEditProperties)PropertiesEdit; } }
		protected override EditPropertiesBase CreateEditProperties() { return new ImageEditProperties(this); }
		protected override string[] GetDesignTimeHiddenPropertyNames() { return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "PropertiesImage" }); }
	}
	public class CardViewBinaryImageColumn : CardViewEditColumn {
		public CardViewBinaryImageColumn() : base(string.Empty) { }
		public CardViewBinaryImageColumn(string fieldName) : base(fieldName, string.Empty) { }
		public CardViewBinaryImageColumn(string fieldName, string caption) : base(fieldName, caption) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataBinaryImageColumnPropertiesBinaryImage"),
#endif
 Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public BinaryImageEditProperties PropertiesBinaryImage { get { return (BinaryImageEditProperties)PropertiesEdit; } }
		protected override EditPropertiesBase CreateEditProperties() { return new BinaryImageEditProperties(this); }
		protected override string[] GetDesignTimeHiddenPropertyNames() { return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "PropertiesBinaryImage" }); }
	}
	public class CardViewProgressBarColumn : CardViewEditColumn {
		public CardViewProgressBarColumn() : base(string.Empty) { }
		public CardViewProgressBarColumn(string fieldName) : base(fieldName, string.Empty) { }
		public CardViewProgressBarColumn(string fieldName, string caption) : base(fieldName, caption) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataProgressBarColumnPropertiesProgressBar"),
#endif
 Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ProgressBarProperties PropertiesProgressBar { get { return (ProgressBarProperties)PropertiesEdit; } }
		protected override EditPropertiesBase CreateEditProperties() { return new ProgressBarProperties(this); }
		protected override string[] GetDesignTimeHiddenPropertyNames() { return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "PropertiesProgressBar" }); }
	}
	public class CardViewColorEditColumn : CardViewEditColumn {
		public CardViewColorEditColumn() : base(string.Empty) { }
		public CardViewColorEditColumn(string fieldName) : base(fieldName, string.Empty) { }
		public CardViewColorEditColumn(string fieldName, string caption) : base(fieldName, caption) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataColorEditColumnPropertiesColorEdit"),
#endif
 Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public ColorEditProperties PropertiesColorEdit { get { return (ColorEditProperties)PropertiesEdit; } }
		protected override EditPropertiesBase CreateEditProperties() { return new ColorEditProperties(this); }
		protected override string[] GetDesignTimeHiddenPropertyNames() { return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "PropertiesColorEdit" }); }
	}
	public class CardViewTimeEditColumn : CardViewEditColumn {
		public CardViewTimeEditColumn() : base(string.Empty) { }
		public CardViewTimeEditColumn(string fieldName) : base(fieldName, string.Empty) { }
		public CardViewTimeEditColumn(string fieldName, string caption) : base(fieldName, caption) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("GridViewDataTimeEditColumnPropertiesTimeEdit"),
#endif
 Category("Behavior"), PersistenceMode(PersistenceMode.InnerProperty), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true)]
		public TimeEditProperties PropertiesTimeEdit { get { return (TimeEditProperties)PropertiesEdit; } }
		protected override EditPropertiesBase CreateEditProperties() { return new TimeEditProperties(this); }
		protected override string[] GetDesignTimeHiddenPropertyNames() { return DataUtils.MergeStringArrays(base.GetDesignTimeHiddenPropertyNames(), new string[] { "PropertiesTimeEdit" }); }
	}
}
