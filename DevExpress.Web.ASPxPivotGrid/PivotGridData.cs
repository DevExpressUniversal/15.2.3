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

using DevExpress.Data.Filtering;
using DevExpress.Data.IO;
using DevExpress.Utils.Serializing;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.ASPxPivotGrid;
using DevExpress.Web.ASPxPivotGrid.Html;
using DevExpress.Web.ASPxPivotGrid.HtmlControls;
using DevExpress.WebUtils;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Customization;
using DevExpress.XtraPivotGrid.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Utils;
using DevExpress.PivotGrid.OLAP;
namespace DevExpress.Web.ASPxPivotGrid.Data {
	public interface IPivotGridEventsImplementor : IPivotGridEventsImplementorBase {
		void CustomCellStyle(PivotGridCellItem cellItem, PivotCellStyle cellStyle);
		void PageIndexChanged();
		void ElementTemplatesChanged();
		void EnsureRefreshData();
		bool IsPivotDataCanDoRefresh { get; }
		void ResetControlHierarchy();
		bool RequireUpdateData { get; set; }
		bool IsDataBindNecessary { get; }
		PivotKPIImages KPIImages { get; }
	}
	class PivotGridEventsRecorder : DevExpress.XtraPivotGrid.Events.PivotGridEventsRecorderBase, IPivotGridEventsImplementor {
		IPivotGridEventsImplementor implementor;
		protected IPivotGridEventsImplementor Implementor { get { return implementor; } }
		public PivotGridEventsRecorder(IPivotGridEventsImplementor implementor) : base(implementor) {
			this.implementor = implementor;
		}
		void IPivotGridEventsImplementor.CustomCellStyle(PivotGridCellItem cellItem, PivotCellStyle cellStyle) {
		   Implementor.CustomCellStyle(cellItem, cellStyle);
		}
		void IPivotGridEventsImplementor.PageIndexChanged() {
			Implementor.PageIndexChanged();
		}
		void IPivotGridEventsImplementor.ElementTemplatesChanged() {
			implementor.ElementTemplatesChanged();
		}
		void IPivotGridEventsImplementor.EnsureRefreshData() {
			implementor.EnsureRefreshData();
		}
		bool IPivotGridEventsImplementor.IsPivotDataCanDoRefresh {
			get { return Implementor.IsPivotDataCanDoRefresh; }
		}
		void IPivotGridEventsImplementor.ResetControlHierarchy() {
			Implementor.ResetControlHierarchy();
		}
		bool IPivotGridEventsImplementor.RequireUpdateData {
			get {
			   return Implementor.RequireUpdateData;
			}
			set {
				Implementor.RequireUpdateData = value;
			}
		}
		bool IPivotGridEventsImplementor.IsDataBindNecessary {
			get { return Implementor.IsDataBindNecessary; }
		}
		PivotKPIImages IPivotGridEventsImplementor.KPIImages {
			get { return Implementor.KPIImages; }
		}
		void IPivotGridEventsImplementorBase.BeginRefresh() {
			BaseImpl.BeginRefresh();
		}
		void IPivotGridEventsImplementorBase.EndRefresh() {
			BaseImpl.EndRefresh();
		}
	}
	internal enum HeaderType { Header, Group, Area };
	public class PivotGridWebData : PivotGridData, IViewBagOwner {
		public new const string WebResourcePath = "DevExpress.Web.ASPxPivotGrid.";
		public new const string PivotGridImagesResourcePath = WebResourcePath + "Images.";
		public const string PivotGridScriptsResourcePath = WebResourcePath + "Scripts.";
		public const string PivotGridScriptResourceName = PivotGridScriptsResourcePath + "PivotGrid.js";
		public const string PivotTableWrapperScriptResourceName = PivotGridScriptsResourcePath + "PivotTableWrapper.js";
		public const string AdjustingManagerScriptResourceName = PivotGridScriptsResourcePath + "AdjustingManager.js";
		public const string DragAndDropScriptResourceName = PivotGridScriptsResourcePath + "DragAndDrop.js";
		public const string GroupFilterScriptResourceName = PivotGridScriptsResourcePath + "GroupFilter.js";
		public const string CustomizationTreeScriptResourceName = PivotGridScriptsResourcePath + "CustomizationTree.js";
		public const string PivotGridSystemCssResourceName = WebResourcePath + "Css.System.css";
		public const string PivotGridDefaultCssResourceName = WebResourcePath + "Css.Default.css";
		public const string PivotGridSpriteCssResourceName = WebResourcePath + "Css.Sprite.css";
		internal const string CancelBubbleJs = "event.cancelBubble=true";
		WebControl owner;
		IDataSourceViewSchemaAccessor dataSourceViewSchemaAccessor = null;
		string webFieldValuesStateCache;
		internal bool fieldsWereGrouped;
		bool isInternalSerialization;
		CustomizationFormFields fieldListFields;
		ITemplate headerTemplate = null;
		ITemplate emptyAreaTemplate = null;
		ITemplate fieldValueTemplate = null;
		ITemplate cellTemplate = null;
		public PivotGridWebData(WebControl owner)  {
			this.owner = owner;
			OptionsView.ShowAllTotals();
			this.optionsPager = CreateOptionsPager();
			this.optionsLoadingPanel = new PivotGridWebOptionsLoadingPanel(PivotGrid);
			this.fieldListFields = null;
			this.EventsImplementor = owner as IPivotGridEventsImplementor;
			this.isInternalSerialization = true;
		}
		public WebControl Owner { get { return owner; } }
		internal ASPxPivotGrid PivotGrid { get { return Owner as ASPxPivotGrid; } }
		public new PivotWebVisualItems VisualItems { get { return (PivotWebVisualItems)base.VisualItems; } }
		protected new PivotWebVisualItems VisualItemsInternal { get { return (PivotWebVisualItems)base.VisualItemsInternal; } }
		internal PivotChartDataSourceBase ChartDataSource { get { return ChartDataSourceInternal; } }
		internal PivotGridStyles Styles { get { return PivotGrid.Styles; } }
		public PivotKPIImages KPIImages { get { return EventsImplementor != null ? EventsImplementor.KPIImages : null; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new IPivotGridEventsImplementor EventsImplementor {
			get { return (IPivotGridEventsImplementor)base.EventsImplementor; }
			set { base.EventsImplementor = value; }
		}
		public override bool IsDesignMode { get { return Owner as ASPxDataWebControlBase != null ? (Owner as ASPxDataWebControlBase).DesignMode : false; } }
		Nullable<bool> isLoadingInternal;
		public override bool IsLoading { 
			get {
				if(isLoadingInternal.HasValue)
					return isLoadingInternal.Value;
				return IsDeserializing || (PivotGrid != null && !PivotGrid.Initialized);
			}
		}
		public void SetIsLoading(bool? isLoading) {
			isLoadingInternal = isLoading;
		}
		public override void SetIsLoading(bool isLoading) {
			isLoadingInternal = isLoading;
		}
		public override void SetControlDataSource(IList ds) {
			if(PivotGrid != null) {
				PivotGrid.DataSource = ds;
			}
		}
		internal bool IsDataAreaCollapsed {
			get {
				return OptionsView.DataHeadersDisplayMode == PivotDataHeadersDisplayMode.Popup && GetFieldCountByArea(PivotArea.DataArea) >= OptionsView.DataHeadersPopupMinCount;
			}
		}
		internal bool LockDataRefreshOnCustomCallback {
			get {
				if(OptionsData.HasLockDataRefreshOnCustomCallbackValue())
					return OptionsData.LockDataRefreshOnCustomCallback == DefaultBoolean.True ? true : false;
				else
					return ASPxPivotGrid.LockDataRefreshOnCustomCallback;
			}
		}
		protected Page Page { get { return Owner.Page; } }
		public new PivotGridFieldCollection Fields { get { return base.Fields as PivotGridFieldCollection; } }
		public new PivotGridWebGroupCollection Groups { get { return (PivotGridWebGroupCollection)base.Groups; } }
		public new WebPrefilter Prefilter { get { return (WebPrefilter)base.Prefilter; } }
		public new PivotGridField DataField { get { return (PivotGridField)base.DataField; } }
		public override string OLAPConnectionString {
			get { return base.OLAPConnectionString; }
			set {
				fieldsWereGrouped = false;
				BeginUpdate();
				try {
					base.OLAPConnectionString = value;
				} finally {
					CancelUpdate();
				}
			}
		}
		internal void ResetFieldListFields(){
			fieldListFields = null;
		}
		public CustomizationFormFields FieldListFields {
			get {
				if(fieldListFields == null) {
					fieldListFields = new CustomizationFormFields(this);
					fieldListFields.DeferUpdates = true;
				}
				return fieldListFields;
			}
		}
		protected override PivotVisualItemsBase CreateVisualItems() {
			return new PivotWebVisualItems(this);
		}
		protected override PrefilterBase CreatePrefilter() {
			return new WebPrefilter(this);
		}
		protected override PivotChartDataSourceBase CreateChartDataSource() {
			return new PivotChartDataSource(this);
		}
		protected override PivotGridFieldBase CreateDataField() {
			return new PivotGridField(this);
		}
		protected override PivotGridFieldCollectionBase CreateFieldCollection() {
			return new PivotGridFieldCollection(this);
		}
		protected override PivotGridGroupCollection CreateGroupCollection() {
			return new PivotGridWebGroupCollection(this);
		}
		public string WebFieldValuesStateCache { get { return webFieldValuesStateCache; } set { webFieldValuesStateCache = value; } }
		public override void ChangeFieldExpanded(PivotGridFieldBase field, bool expanded) {
			if(EventsImplementor != null) EventsImplementor.EnsureRefreshData();
			WebFieldValuesStateCache = null;
			base.ChangeFieldExpanded(field, expanded);
		}
		public override void ChangeFieldExpanded(PivotGridFieldBase field, bool expanded, object value) {
			if(EventsImplementor != null) EventsImplementor.EnsureRefreshData();
			WebFieldValuesStateCache = null;
			base.ChangeFieldExpanded(field, expanded, value);
		}
		public override void ChangeExpandedAll(bool expanded) {
			bool forceAutoCollapse = !expanded && !IsOLAP && AutoExpandGroups &&
									(EventsImplementor != null && EventsImplementor.IsDataBindNecessary);
			DefaultBoolean autoExpandGroupsDefault = DefaultBoolean.Default;
			try {
				WebFieldValuesStateCache = null;
				if(forceAutoCollapse) {
					autoExpandGroupsDefault = OptionsData.AutoExpandGroups;
					OptionsData.AutoExpandGroups = DefaultBoolean.False;
					EventsImplementor.EnsureRefreshData();
					OnGroupRowCollapsed();
				} else {
					if(EventsImplementor != null)
						EventsImplementor.EnsureRefreshData();
					base.ChangeExpandedAll(expanded);
				}
			} finally {
				if(forceAutoCollapse) {
					SetAutoExpandGroups(true, false);
					OptionsData.SetAutoExpandGroups(autoExpandGroupsDefault);
				}
			}
		}
		public override void ChangeExpandedAll(bool isColumn, bool expanded) {
			if(EventsImplementor != null) EventsImplementor.EnsureRefreshData();
			WebFieldValuesStateCache = null;
			base.ChangeExpandedAll(isColumn, expanded);
		}
		public override object[] GetUniqueFieldValues(PivotGridFieldBase field) {
			if(EventsImplementor != null) EventsImplementor.EnsureRefreshData();
			return base.GetUniqueFieldValues(field);
		}
		public override List<object> GetSortedUniqueGroupValues(PivotGridGroup group, object[] parentValues) {
			if(EventsImplementor != null) EventsImplementor.EnsureRefreshData();
			return base.GetSortedUniqueGroupValues(group, parentValues);
		}
		public override bool ChangeExpanded(bool isColumn, object[] values, bool expanded) {
			if(EventsImplementor != null) EventsImplementor.EnsureRefreshData();
			WebFieldValuesStateCache = null;
			return base.ChangeExpanded(isColumn, values, expanded);
		}
		public new void ChangeExpanded(PivotFieldValueItem valueItem, bool raiseEvents) {
			if(EventsImplementor != null) EventsImplementor.EnsureRefreshData();
			WebFieldValuesStateCache = null;
			base.ChangeExpanded(valueItem, raiseEvents);
		}
		public new PivotGridWebOptionsView OptionsView { get { return (PivotGridWebOptionsView)base.OptionsView; } }
		public new PivotGridWebOptionsCustomization OptionsCustomization { get { return (PivotGridWebOptionsCustomization)base.OptionsCustomization; } }
		protected override PivotGridOptionsViewBase CreateOptionsView() {
			return new PivotGridWebOptionsView(OnOptionsViewChanged, this, "OptionsView"); 
		}
		protected override PivotGridOptionsCustomization CreateOptionsCustomization() {
			return new PivotGridWebOptionsCustomization(OnOptionsCustomizationChanged, OnOptionsChanged, this, "OptionsCustomization"); 
		}
		public new PivotGridWebOptionsData OptionsData { get { return (PivotGridWebOptionsData)base.OptionsData; } }
		protected override PivotGridOptionsData CreateOptionsData() {
			return new PivotGridWebOptionsData(this, new EventHandler(OnOptionsDataChanged));
		}
		protected override PivotGridOptionsDataField CreateOptionsDataField() {
			return new PivotGridWebOptionsDataField(this, OnOptionsChanged, this, "OptionsDataField"); 
		}
		PivotGridWebOptionsPager optionsPager;
		public PivotGridWebOptionsPager OptionsPager { get { return optionsPager; } }
		protected PivotGridWebOptionsPager CreateOptionsPager() {
			return new PivotGridWebOptionsPager(OnOptionsPagerChanged, this, "OptionsPager");
		}
		public new PivotGridWebOptionsChartDataSource OptionsChartDataSource { get { return (PivotGridWebOptionsChartDataSource)base.OptionsChartDataSource; } }
		protected override PivotGridOptionsChartDataSourceBase CreateOptionsChartDataSource() {
			return new PivotGridWebOptionsChartDataSource(this);
		}
		public new PivotGridWebOptionsFilter OptionsFilter { get { return (PivotGridWebOptionsFilter)base.OptionsFilter; } }
		protected override PivotGridOptionsFilterBase CreateOptionsFilter() {
			return new PivotGridWebOptionsFilter(OnOptionsFilterChanged, this, "OptionsFilter");
		}
		PivotGridWebOptionsLoadingPanel optionsLoadingPanel;
		public PivotGridWebOptionsLoadingPanel OptionsLoadingPanel { get { return optionsLoadingPanel; } }
		protected void OnOptionsCustomizationChanged(PivotGridWebOptionsCustomization sender, PivotGridWebOptionsCustomization.OptionsCustomizationChangedEventArgs e) {
			if(e.Reason == PivotGridWebOptionsCustomization.OptionsCustomizationChangedReason.AllowCustomizationWindowResizing) {
				if(PivotGrid.CustomizationFields != null)
					PivotGrid.CustomizationFields.AllowResize = sender.AllowCustomizationWindowResizing;
			}
		}
		protected void OnOptionsPagerChanged(PivotGridWebOptionsPager sender, PivotGridWebOptionsPager.OptionsPagerChangedEventArgs e) {
			if(e.Reason == PivotGridWebOptionsPager.OptionsPagerChangedReason.PageIndex)
				EventsImplementor.PageIndexChanged();
			LayoutChanged();
		}
		protected override PivotGridOptionsBehaviorBase CreateOptionsBehavior() {
			return new PivotGridWebOptionsBehavior(OnOptionsChanged);
		}
		protected internal bool GetIsGroupFilter(PivotGridField filterField) {
			return filterField != null && filterField.Group != null && filterField.Group.IsFilterAllowed;
		}
		public PivotFilterItemsBase CreatePivotGridFilterItems(PivotGridField field, bool deferUpdates) {
			PivotGridField filterField = Fields[field.Index];
			if(GetIsGroupFilter(filterField)) {
				if(deferUpdates && FieldListFields.GetGroupFilter(filterField) != null)
					return FieldListFields.GetGroupFilter(filterField);
				return new PivotGroupFilterItems(this, filterField, false, deferUpdates);
			} else {
				if(deferUpdates && FieldListFields.GetFieldFilter(filterField) != null)
					return FieldListFields.GetFieldFilter(filterField);
				return new PivotGridFilterItems(this, filterField, false, OptionsFilter.ShowOnlyAvailableItems, deferUpdates);
			}
		}
		protected internal PivotGridField GetFieldByIndex(int index) {
			return (index >= 0 && index < Fields.Count) ? Fields[index] : null;
		}
		public AppearanceStyle GetTableStyle() {
			AppearanceStyle style = Styles.GetMainTableStyle();
			MergeBorderWithControlBorder(style);
			return style;
		}
		void MergeBorderWithControlBorder(AppearanceStyleBase style) {
			AppearanceStyleBase controlStyle = PivotGrid.GetControlStyle();
			if(style.Border.BorderColor.IsEmpty) {
				style.Border.BorderColor = controlStyle.Border.BorderColor;
			}
			if(style.Border.BorderStyle == System.Web.UI.WebControls.BorderStyle.NotSet) {
				style.Border.BorderStyle = controlStyle.Border.BorderStyle;
			}
			if(style.Border.BorderWidth.IsEmpty) {
				style.Border.BorderWidth = controlStyle.Border.BorderWidth;
			}
		}
		public PivotHeaderStyle GetEmptyHeaderStyle(PivotFieldItem field) {
			PivotHeaderStyle style = new PivotHeaderStyle();
			Styles.ApplyDefaultHeaderStyle(style);
			return style;
		}
		public PivotHeaderStyle GetHeaderStyle(PivotFieldItem field) {
			PivotHeaderStyle style = new PivotHeaderStyle();
			Styles.ApplyDefaultHeaderStyle(style);
			style.CopyFrom(Styles.HeaderStyle);
			style.CopyFrom(field.HeaderStyle);		
			return style;
		}
		public PivotHeaderStyle GetGroupButtonStyle(PivotFieldItem field) {
			PivotHeaderStyle style = GetHeaderStyle(field);
			Styles.ApplyHeaderGroupButtonStyle(style);
			return style;
		}
		public PivotHeaderStyle GetHeaderTextStyle(PivotFieldItem field) {
			PivotHeaderStyle style = GetHeaderStyle(field);
			Styles.ApplyHeaderTextStyle(style);
			return style;
		}
		public PivotHeaderStyle GetHeaderTableStyle(PivotFieldItem field) {
			PivotHeaderStyle style = GetHeaderStyle(field);
			Styles.ApplyHeaderTableStyle(style);
			return style;
		}
		public PivotHeaderStyle GetHeaderSortStyle(PivotFieldItem field) {
			PivotHeaderStyle style = GetHeaderStyle(field);
			Styles.ApplyHeaderSortStyle(style);
			return style;
		}
		public PivotHeaderStyle GetHeaderFilterStyle(PivotFieldItem field) {
			PivotHeaderStyle style = GetHeaderStyle(field);
			Styles.ApplyHeaderFilterStyle(style);
			return style;
		}
		public AppearanceSelectedStyle GetHeaderHoverStyle(PivotFieldItem field) {
			AppearanceSelectedStyle style = new AppearanceSelectedStyle();
			style.CopyFrom(Styles.GetDefaultHeaderHoverStyle());
			style.CopyFrom(Styles.HeaderStyle.HoverStyle);
			style.CopyFrom(field.HeaderStyle.HoverStyle);
			return style;
		}
		public PivotAreaStyle GetAreaStyle(PivotArea area) {
			PivotAreaStyle style = Styles.GetAreaStyle(area);
			MergeBorderWithControlBorder(style);
			if(!OptionsView.ShowDataHeaders && !OptionsView.ShowColumnHeaders && area == PivotArea.RowArea)
				style.BorderTop.BorderStyle = BorderStyle.None;
			return style;
		}
		public PivotAreaStyle GetEmptyAreaStyle(PivotArea area) {
			PivotAreaStyle style = GetAreaStyle(area);
			Styles.ApplyEmptyAreaStyle(style);
			return style;
		}
		public PivotFieldValueStyle GetFieldValueStyle(PivotFieldValueItem item, PivotFieldItem field) {
			PivotFieldValueStyle style = new PivotFieldValueStyle();
			Styles.ApplyFieldValueStyle(style, item.Area == PivotArea.ColumnArea, field);
			if(item.IsTotal)
				Styles.ApplyTotalFieldValueStyle(style, item.Area == PivotArea.ColumnArea, field);
			if(!item.IsVisible) {
				Styles.ApplyRowTreeFieldValueStyle(style, field);
				if(item.Index != 0)
					style.BorderTop.BorderStyle = BorderStyle.None;
			}
			if(item.ValueType == PivotGridValueType.GrandTotal)
				Styles.ApplyGrandTotalFieldValueStyle(style, item.Area == PivotArea.ColumnArea);
			if(item.IsColumn && !item.ShowCollapsedButton && style.HorizontalAlign == HorizontalAlign.NotSet) 
				style.HorizontalAlign = HorizontalAlign.Center;
			MergeBorderWithControlBorder(style);
			return style;
		}
		public AppearanceStyle GetCollapsedButtonStyle() {
			AppearanceStyle style = new AppearanceStyle();
			Styles.ApplyCollapsedButtonStyle(style);
			style.Cursor = RenderUtils.GetPointerCursor();
			return style;
		}
		public AppearanceStyle GetSortByColumnImageStyle() {
			AppearanceStyle style = new AppearanceStyle();
			Styles.ApplySortByColumnImageStyle(style);
			return style;
		}
		public void ApplyCellStyle(PivotGridCellItem cellItem, PivotCellStyle style) {
			Styles.ApplyCellStyle(cellItem.ColumnIndex == 0, cellItem.RowIndex == 0, style, cellItem.ShowKPIGraphic);
			if(cellItem.IsGrandTotalAppearance) 
				Styles.ApplyGrandTotalCellStyle(style);	
			else {
				if(cellItem.IsTotalAppearance)
					Styles.ApplyTotalCellStyle(style);	
				else {
					if(cellItem.IsCustomTotalAppearance)
						Styles.ApplyCustomTotalCellStyle(style);
				}
			}
			PivotFieldItem dataField = GetFieldItemByBaseFieldItem(cellItem.DataField);
			if(dataField != null)
				style.CopyFrom(dataField.CellStyle);
			EventsImplementor.CustomCellStyle(cellItem, style);
		}
		public Paddings GetAreaPaddings(PivotArea area, bool isFirst, bool isLast) {
			PivotAreaStyle style = GetAreaStyle(area);
			Paddings result = new Paddings();
			result.CopyFrom(Styles.GetAreaPaddings(area, isFirst, isLast));
			result.CopyFrom(style.Paddings);			
			return result;
		}
		public Paddings GetFilterButtonPanelPaddings() {
			return GetFilterButtonPanelStyle().Paddings;
		}
		public Unit GetFilterButtonPanelSpacing() {
			return GetFilterButtonPanelStyle().Spacing;
		}
		public PivotFilterStyle GetFilterWindowStyle() {
			PivotFilterStyle style = new PivotFilterStyle();
			style.CopyFrom(Styles.FilterWindowStyle);			
			return style;
		}
		public PivotFilterStyle GetFilterItemsAreaStyle() {
			PivotFilterStyle style = new PivotFilterStyle();
			style.CopyFrom(Styles.FilterItemsAreaStyle);			
			return style;
		}
		public PivotFilterItemStyle GetFilterItemStyle() {
			PivotFilterItemStyle style = new PivotFilterItemStyle();
			style.CopyFrom(Styles.GetFilterItemStyle());
			style.CopyFrom(Styles.FilterItemStyle);
			return style;
		}
		public PivotFilterButtonStyle GetFilterButtonStyle() {
			PivotFilterButtonStyle style = new PivotFilterButtonStyle();
			style.CopyFrom(Styles.GetFilterButtonStyle());
			style.CopyFrom(Styles.FilterButtonStyle);
			return style;
		}
		public PivotFilterButtonPanelStyle GetFilterButtonPanelStyle() {
			PivotFilterButtonPanelStyle style = new PivotFilterButtonPanelStyle();
			style.CopyFrom(Styles.FilterButtonPanelStyle);
			return style;
		}
		public AppearanceStyle GetPagerStyle(bool isTopPager) {
			AppearanceStyle style = new AppearanceStyle();
			style.CopyFrom(Styles.GetPagerStyle(isTopPager));
			MergeBorderWithControlBorder(style);
			switch(OptionsPager.PagerAlign) {
				case PagerAlign.Left:
					style.HorizontalAlign = HorizontalAlign.Left;
					break;
				case PagerAlign.Right:
					style.HorizontalAlign = HorizontalAlign.Right;
					break;
				case PagerAlign.Center:
					style.HorizontalAlign = HorizontalAlign.Center;
					break;
				case PagerAlign.Justify:
					style.HorizontalAlign = HorizontalAlign.Justify;
					break;
			}
			return style;
		}
		public ITemplate HeaderTemplate {
			get { return headerTemplate; }
			set {headerTemplate = value; }
		}
		public ITemplate EmptyAreaTemplate {
			get { return emptyAreaTemplate; }
			set { emptyAreaTemplate = value; }
		}
		public ITemplate FieldValueTemplate {
			get { return fieldValueTemplate; }
			set { fieldValueTemplate = value; }
		}
		public ITemplate CellTemplate {
			get { return cellTemplate; }
			set { cellTemplate = value; }
		}
		public void TemplatesChanged() {
			if (EventsImplementor != null)
				EventsImplementor.ElementTemplatesChanged();
		}
		public void SetupTemplateContainer(Control templateContainer, ITemplate template) {
			template.InstantiateIn(templateContainer);
		}
		public string GetCollapsedImageEvent() {
			return "onclick";
		}
		public void RestoreFieldsInGroups() {
			if(Groups == null || Fields == null)
				return;
			SetIsDeserializing(true);
			BeginUpdate();
			try {
				foreach(PivotGridField field in Fields) {
					if(field.GroupIndexCore == -1)
						continue;
					Groups[field.GroupIndexCore].Add(field);
				}
				foreach(PivotGridWebGroup group in Groups)
					group.SortFields();
			} finally {
				CancelUpdate();
				SetIsDeserializing(false);
			}
		}
		public IDataSourceViewSchemaAccessor DataSourceViewSchemaAccessor {
			get { return dataSourceViewSchemaAccessor; }
			set { dataSourceViewSchemaAccessor = value; }
		}
		public bool IsCustomizationFields(string clientId) {
			return clientId.Contains(PivotGridHtmlCustomizationFields.ElementName_ID + HeaderPresenterType.FieldList.ToString());
		}
		public string SaveWebCollapsedState() {
			if(!string.IsNullOrEmpty(WebFieldValuesStateCache))
				return WebFieldValuesStateCache;
			using(MemoryStream stream = new MemoryStream()) {
				using(DeflateStream compressor = new DeflateStream(stream, CompressionMode.Compress, true)) {
					using(BufferedStream buffered = new BufferedStream(compressor)) {
						if(PivotGrid.CollapsedStateStoreMode == PivotCollapsedStateStoreMode.Indexes)
							WebSaveCollapsedStateToStream(buffered);
						else
							SaveCollapsedStateToStream(buffered);
					}
				}
				return Convert.ToBase64String(stream.GetBuffer(), 0, (int)stream.Length);
			}
		}
		public void RestoreWebCollapsedState(string value) {
			if(string.IsNullOrEmpty(value)) return;
			using(MemoryStream stream = new MemoryStream(Convert.FromBase64String(value))) {
				using(DeflateStream decompressor = new DeflateStream(stream, CompressionMode.Decompress)) {
					if(PivotGrid.CollapsedStateStoreMode == PivotCollapsedStateStoreMode.Indexes)
						WebLoadCollapsedStateFromStream(decompressor);
					else
						LoadCollapsedStateFromStream(decompressor);
				}
			}
		}
		public string SaveFieldDataTypes() {
			return DevExpress.Data.PivotGrid.PivotGridSerializeHelper.ToBase64String(writer => {
				writer.Write(Fields.Count);
				for(int i = 0; i < Fields.Count; i++) {
					writer.WriteType(GetFieldType(Fields[i], true));
				}
			}, null);
		}
		public void RestoreFieldDataTypes(string value) {
			if(string.IsNullOrEmpty(value)) return;
			if(storedFieldTypes == null)
				storedFieldTypes = new Dictionary<PivotGridFieldBase, Type>();
			else
				storedFieldTypes.Clear();
			using(MemoryStream stream = new MemoryStream(Convert.FromBase64String(value))) {
				TypedBinaryReader reader = new TypedBinaryReader(stream);
				int fieldsCount = reader.ReadInt32();
				if(fieldsCount != Fields.Count) 
					return;
				for(int i = 0; i < fieldsCount; i++) {
					storedFieldTypes.Add(Fields[i], reader.ReadType());
				}
			}
		}
		public string SaveFilterValues() {
			if(ListSource == null && string.IsNullOrEmpty(PivotGrid.OLAPConnectionString)) 
				return string.Empty;
			return DevExpress.Data.PivotGrid.PivotGridSerializeHelper.ToBase64String(SaveFilterValuesToStream, OptionsData.CustomObjectConverter);
		}		
		public bool RestoreFilterValues(string value) {
			if(string.IsNullOrEmpty(value)) return false;
			using(MemoryStream stream = new MemoryStream(Convert.FromBase64String(value))) {
				LoadFilterValuesFromStream(stream);
				return true;
			}
		}
		public string OLAPSessionID {
			get {
				if(!IsOLAP)
					return QueryDataSource != null ? QueryDataSource.GetHashCode().ToString() : string.Empty;
				return ((IOLAPHelpersOwner)OLAPDataSource).Metadata.SessionID ?? string.Empty;
			}
		}
		public string SaveDenyExpandState() {
			return DenyExpandValues.GetDenyExpandState();
		}
		public void RestoreDenyExpandState(string state) {
			DenyExpandValues.SetDenyExpandState(state);
		}
		public void LoadVisualItemsState(string fieldValueItems, string dataCells) {
			VisualItemsInternal.RestoreFieldValueItemsState(fieldValueItems);
			VisualItemsInternal.RestoreDataCellsState(dataCells);
		}
		bool isDataBinding = false;
		protected override bool LockRefresh {
			get {
				return EventsImplementor != null ? !EventsImplementor.IsPivotDataCanDoRefresh : base.LockRefresh;
			}
		}
		Dictionary<PivotGridFieldBase, Type> storedFieldTypes;
		public override Type GetFieldType(PivotGridFieldBase field, bool raw) {
			if(raw && storedFieldTypes != null && storedFieldTypes.ContainsKey(field))
				return storedFieldTypes[field];
			return base.GetFieldType(field, raw);
		}
		protected override void ClearCaches() {
			base.ClearCaches();
			storedFieldTypes = null;
		}
		protected override void DoRefreshCore() {
			if(isDataBinding) return;
			this.isDataBinding = true;
			if(IsOLAP && DelayFieldsGroupingByHierarchies && !fieldsWereGrouped) {
				Fields.GroupFieldsByHierarchies();
				fieldsWereGrouped = true;
			}
			if(EventsImplementor != null && !EventsImplementor.IsPivotDataCanDoRefresh)
				EventsImplementor.RequireUpdateData = true;
			else
				base.DoRefreshCore();
			this.isDataBinding = false;			
		}
		public bool IsRendering { get { return PivotGrid.IsRendering; } }
		public override bool DelayFieldsGroupingByHierarchies { get { return true; } }
		public PivotFieldItem GetFieldItemByBaseFieldItem(PivotFieldItemBase baseFieldItem) {
			return baseFieldItem as PivotFieldItem;
		}
		public new PivotGridField GetField(PivotFieldItemBase item) {
			return (PivotGridField)base.GetField(item);
		}
		public new PivotFieldItem GetFieldItem(PivotGridFieldBase field) {
			return (PivotFieldItem)base.GetFieldItem(field);
		}
		public PivotGridField GetFieldByBaseField(PivotGridFieldBase baseField) {
			return baseField as PivotGridField;
		}
		public List<PivotFieldItemBase> GetFieldItemsByArea(PivotArea area) {
			return this.FieldItems.GetFieldItemsByArea(area, true);
		}
		public PivotGridField[] GetFieldsByArea(PivotArea area) {
			List<PivotGridFieldBase> baseFields = GetFieldsByArea(area, true);
			PivotGridField[] fields = new PivotGridField[baseFields.Count];
			for(int i = 0; i < fields.Length; i++) {
				fields[i] = GetFieldByBaseField(baseFields[i]);
			}
			return fields;
		}
		#region Events
		public override void OnSortOrderChanged(PivotGridFieldBase field) {
			if(EventsImplementor != null && !EventsImplementor.IsPivotDataCanDoRefresh) {
				EventsImplementor.RequireUpdateData = true;
				return;
			}
			base.OnSortOrderChanged(field);
		}
		#endregion
		T IViewBagOwner.GetViewBagProperty<T>(string objectPath, string propertyName, T value) {
			IViewBagOwner viewBagOwner = Owner as IViewBagOwner;
			return viewBagOwner != null ? viewBagOwner.GetViewBagProperty(objectPath, propertyName, value): value;
		}
		void IViewBagOwner.SetViewBagProperty<T>(string objectPath, string propertyName, T defaultValue, T value) {
			IViewBagOwner viewBagOwner = Owner as IViewBagOwner;
			if(viewBagOwner != null) {
				viewBagOwner.SetViewBagProperty(objectPath, propertyName, defaultValue, value);
			}
		}
		public new void SetIsDeserializing(bool value) {
			base.SetIsDeserializing(value);
		}
		public void SetIsInternalSerialization(bool value) {
			this.isInternalSerialization = value;
		}
		internal bool IsInternalSerialization { get { return this.isInternalSerialization; } }
		protected override void ClearDenyExpandValues() {
			if(PivotGrid == null || !PivotGrid.IsDataBinding && PivotGrid.PostBackAction == null)
				base.ClearDenyExpandValues();
		}
		public override void LayoutChanged() {
			if(EventsImplementor != null && EventsImplementor.RequireUpdateData && !EventsImplementor.IsPivotDataCanDoRefresh)
				return;
			base.LayoutChanged();
		}
		protected override IPivotOLAPDataSource CreateOLAPDataSource() {
			IPivotOLAPDataSource olapDataSource = base.CreateOLAPDataSource();
			if(olapDataSource != null && PivotGrid != null && !string.IsNullOrEmpty(PivotGrid.OLAPSessionID))
				((IOLAPHelpersOwner)olapDataSource).Metadata.SessionID = PivotGrid.OLAPSessionID;
			return olapDataSource;
		}
		public override CustomizationFormFields GetCustomizationFormFields() {
			return FieldListFields;
		}
		internal void ElementTemplatesChanged() {
			InvalidateFieldItems();
		}
		protected override void OnGroupRowCollapsed() {
			VisualItems.Clear();
			if(EventsImplementor != null)
				EventsImplementor.ResetControlHierarchy();
		}
	}
	public class PivotWebVisualItems : PivotVisualItemsBase {
		Dictionary<Point, PivotGridCellItem> cellItems;
		Dictionary<PivotFieldValueItem, PivotFieldValueItem> invisibleVisibleItemsCache;
		public PivotWebVisualItems(PivotGridWebData data)
			: base(data) {
				cellItems = new Dictionary<Point, PivotGridCellItem>();
		}
		public bool IsVirtualScrollingMode(bool isColumn) {
			PivotGridWebOptionsView opts = Data.OptionsView;
			PivotGridWebOptionsPager pagerOpts = Data.OptionsPager;
			int itemsPerPage = isColumn ? pagerOpts.ColumnsPerPage : pagerOpts.RowsPerPage;
			ScrollBarMode scrollBarMode = isColumn ? opts.HorizontalScrollBarMode : opts.VerticalScrollBarMode;
			PivotScrollingMode scrollingMode = isColumn ? opts.HorizontalScrollingMode : opts.VerticalScrollingMode;
			return scrollBarMode != ScrollBarMode.Hidden && scrollingMode == PivotScrollingMode.Virtual && itemsPerPage > 0;
		}
		bool GrandTotalsOnEveryPage(bool isColumn) { return IsVirtualScrollingMode(isColumn); }
		bool IsMovingPaging(bool isColumn) { return IsVirtualScrollingMode(isColumn); }
		protected new PivotGridWebData Data { get { return (PivotGridWebData)base.Data; } }
		PivotGridWebOptionsPager OptionsPager { get { return Data.OptionsPager; } }
		public bool HasPaging(bool isColumn) {
			return OptionsPager.HasPager(isColumn) || IsVirtualScrollingMode(isColumn);
		}
		public int GetPageIndex(bool isColumn) {
			return isColumn ? OptionsPager.ColumnPageIndex : OptionsPager.PageIndex;
		}
		public int GetPageSize(bool isColumn) {
			return isColumn ? OptionsPager.ColumnsPerPage : OptionsPager.RowsPerPage;
		}
		int GetStartIndex(bool isColumn) {
			int pageIndex = GetPageIndex(isColumn);
			if(pageIndex <= 0)
				return 0;
			int pageSize = GetPageSize(isColumn);
			int shift = IsMovingPaging(isColumn) ? 1 : 0;
			return (pageIndex - shift) * pageSize;
		}
		public int GetPagedPageStartIndex(bool isColumn) {
			return GetPageStartIndex(isColumn) - GetStartIndex(isColumn);
		}
		public int GetPagedPageEndIndex(bool isColumn) {
			return GetPageEndIndex(isColumn) - GetStartIndex(isColumn);
		}
		public int GetPageStartIndex(bool isColumn) {
			var pageIndex = GetPageIndex(isColumn);
			pageIndex = pageIndex < 0 ? 0 : pageIndex;
			return pageIndex * GetPageSize(isColumn);
		}
		int GetPageEndIndex(bool isColumn) {
			int endIndex = GetPageStartIndex(isColumn) + GetPageSize(isColumn) - 1;
			int totalCount = GetLastLevelUnpagedItemCount(isColumn);
			endIndex = endIndex < totalCount ? endIndex : totalCount - 1;
			return endIndex;
		}
		public PivotFieldValueItem GetPageStartItem(bool isColumn) {
			return GetLastLevelItem(isColumn, GetPagedPageStartIndex(isColumn));
		}
		public PivotFieldValueItem GetPageEndItem(bool isColumn) {
			return GetLastLevelItem(isColumn, GetPagedPageEndIndex(isColumn));
		}
		public int GetPageItemsCount(bool isColumn) {
			int pageIndex = GetPageIndex(isColumn);
			if(pageIndex < 0)
				return 0;
			int ratio = 1;
			if(IsMovingPaging(isColumn))
				ratio = (pageIndex == 0) ? 2 : 3;
			return GetPageSize(isColumn) * ratio;
		}
		public int GetLastLevelUnpagedItemCount(bool isColumn) {
			return GetCreator(isColumn).LastLevelUnpagedItemCount;
		}
		public int GetLastLevelUnpagedItemCountWithoutGrandTotals(bool isColumn) {
			PivotFieldValueItemsCreator creator = GetCreator(isColumn);
			return creator.LastLevelUnpagedItemCount - creator.GrandTotalItemCount;
		}
		public int UnpagedRowCount { get { return GetLastLevelUnpagedItemCount(false); } }
		public int GetPageCount(bool isColumn) {
			PivotGridWebOptionsView optsView = Data.OptionsView;
			bool showGrandTotals = isColumn ? optsView.ShowColumnGrandTotals : optsView.ShowRowGrandTotals;
			int pageSize = GetPageSize(isColumn);
			int itemCount = GetLastLevelUnpagedItemCount(isColumn);
			int grandTotalItemCount = GetCreator(isColumn).GrandTotalItemCount;
			if(pageSize == 0 || itemCount <= pageSize ||
				(showGrandTotals && itemCount - grandTotalItemCount == pageSize))
				return 1;
			if(Data.OptionsPager.IsPageSizeVisible() && GetPageIndex(isColumn) == -1)
				return 1;
			return itemCount / pageSize + ((itemCount % pageSize > grandTotalItemCount) ? 1 : 0);
		}
		public override void Clear() {
			base.Clear();
			cellItems.Clear();
			if(invisibleVisibleItemsCache != null)
				invisibleVisibleItemsCache.Clear();
		}
		public PivotFieldValueItem GetItem(bool isColumn, int index, bool paged) {
			if(paged)
				return GetLastLevelItem(isColumn, index);
			PivotFieldValueItemsCreator creator = isColumn ? ColumnItemsCreator : RowItemsCreator;
			return creator.LastLevelUnpagedItemCount > index ? creator.GetLastLevelUnpagedItem(index) : null;
		}
		public PivotFieldValueItem GetRowItemByInvisibleItem(PivotFieldValueItem invisibleItem) {
			if(invisibleVisibleItemsCache == null)
				invisibleVisibleItemsCache = new Dictionary<PivotFieldValueItem, PivotFieldValueItem>();
			PivotFieldValueItem visibleItem;
			if(invisibleVisibleItemsCache.TryGetValue(invisibleItem, out visibleItem))
				return visibleItem;
			int lastLevelVisibleItemIndex = invisibleItem.MinLastLevelIndex - 1;
			visibleItem = GetItem(false, lastLevelVisibleItemIndex, false);
			invisibleVisibleItemsCache.Add(invisibleItem, visibleItem);
			return visibleItem;
		}
		protected override void Calculate() {
			CreateItems(CreatePagingOptions(true), CreatePagingOptions(false));
		}
		VisualItemsPagingOptions CreatePagingOptions(bool isColumn) {
			return new VisualItemsPagingOptions {
				Enabled = HasPaging(isColumn),
				ShowGrandTotalsOnEachPage = !IsVirtualScrollingMode(isColumn),
				Start = GetStartIndex(isColumn),
				Count = GetPageItemsCount(isColumn)
			};
		}
		public object[] GetItemValues(bool isColumn, int uniqueIndex) {
			PivotFieldValueItemsCreator itemsCreator = GetItemsCreator(isColumn);
			return itemsCreator.GetItemValues(uniqueIndex);
		}
		public object[] GetItemValues(PivotFieldValueItem item) {
			PivotFieldValueItemsCreator itemsCreator = GetItemsCreator(item.IsColumn);
			return itemsCreator.GetItemValues(item);
		}
		public PivotFieldItemBase GetItemField(bool isColumn, int uniqueIndex) {
			PivotFieldValueItemsCreator itemsCreator = GetItemsCreator(isColumn);
			return itemsCreator.GetUnpagedItem(uniqueIndex).Field;
		}
		PivotGridCellItem CreateCellItem(PivotFieldValueItem columnItem, PivotFieldValueItem rowItem, int columnIndex, int rowIndex, bool useItemsCache) {
			if(!useItemsCache)
				return base.CreateCellItem(columnItem, rowItem, columnIndex, rowIndex);
			else {
				Point key = new Point(columnIndex, rowIndex);
				if(cellItems.ContainsKey(key))
					return cellItems[key];
				PivotGridCellItem cellItem = base.CreateCellItem(columnItem, rowItem, columnIndex, rowIndex);
				cellItems[key] = cellItem;
				return cellItem;
			}
		}
		public override PivotGridCellItem CreateCellItem(PivotFieldValueItem columnItem, PivotFieldValueItem rowItem, int columnIndex, int rowIndex) {
			return CreateCellItem(columnItem, rowItem, columnIndex, rowIndex, true);
		}
		public PivotGridCellItem CreateCellItem(int columnIndex, int rowIndex, bool paged) {
			return CreateCellItem(columnIndex, rowIndex, paged, true);
		}
		public PivotGridCellItem CreateCellItem(int columnIndex, int rowIndex, bool paged, bool useItemsCache) {
			PivotFieldValueItem rowItem = GetItem(false, rowIndex, paged);
			PivotFieldValueItem columnItem = GetItem(true, columnIndex, true);
			if(rowItem == null || columnItem == null)
				return null;
			return CreateCellItem(columnItem, rowItem, columnIndex, rowIndex, useItemsCache);
		}
		protected override PivotFieldItemBase CreateFieldItem(PivotGridData data, PivotGroupItemCollection groupItems, PivotGridFieldBase field) {
			return new PivotFieldItem(data, groupItems, field);
		}
		public int GetVisibleIndex(bool isColumn, int lastLevelIndex, bool paged) {
			PivotFieldValueItem item = GetLastLevelItem(isColumn, lastLevelIndex, paged);
			return item != null ? item.VisibleIndex : -1;
		}
		public int GetUnpagedRowIndex(int pageRowIndex) {
			return GetItem(false, pageRowIndex, true).MinLastLevelIndex;
		}
		public PivotFieldValueItem GetUnpagedItem(bool isColumn, int uniqueIndex) {
			PivotFieldValueItemsCreator itemsCreator = GetItemsCreator(isColumn);
			return itemsCreator.GetUnpagedItem(uniqueIndex);
		}
		public override PivotFieldValueItem GetUnpagedItem(bool isColumn, int lastLevelIndex, int level) {
			PivotFieldValueItemsCreator itemsCreator = GetItemsCreator(isColumn);
			return GetUnpagedItemCore(lastLevelIndex, level, itemsCreator);
		}
		public PivotFieldValueItem GetUnpagedItem(PivotGridFieldBase field, int lastLevelIndex) {
			PivotFieldValueItemsCreator itemsCreator = GetItemsCreator(field.IsColumn);
			return GetUnpagedItemCore(field, lastLevelIndex, itemsCreator);
		}
		public PivotFieldValueItem GetLastLevelItem(bool isColumn, int lastLevelIndex, bool paged) {
			PivotFieldValueItemsCreator itemsCreator = GetItemsCreator(isColumn);
			return paged ? GetLastLevelItemCore(lastLevelIndex, itemsCreator) : GetLastLevelUnpagedItemCore(lastLevelIndex, itemsCreator);
		}
		public object GetUnpagedCellValue(int columnIndex, int rowIndex) {
			return GetCellValue(columnIndex, rowIndex, false);
		}
		public object GetCellValue(int columnIndex, int rowIndex, bool paged) {
			return PivotCellValue.GetValue(GetCellValueEx(columnIndex, rowIndex, paged));
		}
		public PivotCellValue GetCellValueEx(int columnIndex, int rowIndex, bool paged) {
			return CellDataProvider.GetCellValueEx(GetItem(true, columnIndex, paged), GetItem(false, rowIndex, paged));
		}
		public PivotFieldValueItem GetParentItem(bool isColumn, PivotFieldValueItem item, bool paged) {
			return paged ? GetParentItem(isColumn, item) : GetParentUnpagedItem(isColumn, item);
		}
		public object GetUnpagedFieldValue(PivotGridFieldBase field, int lastLevelIndex) {
			PivotFieldValueItem item = GetUnpagedItem(field, lastLevelIndex);
			return item != null ? item.Value : null;
		}
		public IOLAPMember GetUnpagedOLAPMember(PivotGridFieldBase field, int lastLevelIndex) {
			PivotFieldValueItem item = GetUnpagedItem(field, lastLevelIndex);
			return GetOLAPMemberCore(field, item);
		}
		public bool IsUnpagedObjectCollapsed(PivotGridFieldBase field, int lastLevelIndex) {
			PivotFieldValueItem item = GetUnpagedItem(field, lastLevelIndex);
			return item != null ? Data.IsObjectCollapsed(field.IsColumn, item.VisibleIndex) : false;
		}
		public PivotDrillDownDataSource CreateDrillDownDataSource(int columnIndex, int rowIndex, int dataIndex, bool paged) {
			PivotGridCellItem cellItem = CreateCellItem(columnIndex, rowIndex, paged, false);
			if(cellItem == null)
				return null;
			return Data.GetDrillDownDataSource(cellItem.ColumnFieldIndex, cellItem.RowFieldIndex, dataIndex);
		}
		public PivotDrillDownDataSource CreateDrillDownDataSource(int columnIndex, int rowIndex, bool paged) {
			PivotGridCellItem cellItem = CreateCellItem(columnIndex, rowIndex, paged, false);
			if(cellItem == null)
				return null;
			return Data.GetDrillDownDataSource(cellItem.ColumnFieldIndex, cellItem.RowFieldIndex, cellItem.DataIndex);
		}
		[Obsolete("This method is now obsolete. Use the CreateQueryModeDrillDownDataSource method instead.")]
		public PivotDrillDownDataSource CreateOLAPDrillDownDataSource(int columnIndex, int rowIndex, int maxRowCount, List<string> customColumns, bool paged) {
			return CreateQueryModeDrillDownDataSource(columnIndex, rowIndex, maxRowCount, customColumns, paged);
		}
		public PivotDrillDownDataSource CreateQueryModeDrillDownDataSource(int columnIndex, int rowIndex, int maxRowCount, List<string> customColumns, bool paged) {
			PivotGridCellItem cellItem = CreateCellItem(columnIndex, rowIndex, paged, false);
			if(cellItem == null)
				return null;
			return Data.GetQueryModeDrillDownDataSource(cellItem.ColumnFieldIndex, cellItem.RowFieldIndex, cellItem.DataIndex,
				maxRowCount, customColumns);
		}
		public PivotSummaryDataSource CreateSummaryDataSource(int columnIndex, int rowIndex, bool paged) {
			return Data.CreateSummaryDataSource(GetVisibleIndex(true, columnIndex, paged), GetVisibleIndex(false, rowIndex, paged));
		}
		protected void SaveFieldValueItemsStateCore(TypedBinaryWriter writer) {
			ColumnItemsCreator.SaveToStream(writer);
			RowItemsCreator.SaveToStream(writer);
			bool writeInvisibleItems = HasPaging(false);
			writer.Write(writeInvisibleItems);
			if(writeInvisibleItems) {
				CreateInvisibleItemsCache(RowItemsCreator);
				SaveInvisibleItemsCache(writer);
			}
		}		
		protected void RestoreFieldValueItemsStateCore(Stream stream) {
			ColumnItemsCreator.LoadFromStream(stream);
			RowItemsCreator.LoadFromStream(stream);
			EnsureFieldItems();
			EnsureIsReady();
			using(TypedBinaryReader reader = TypedBinaryReader.CreateReader(stream, Data.OptionsData.CustomObjectConverter)) {
				bool hasInvisibleItemsCache = reader.ReadBoolean();
				if(hasInvisibleItemsCache)
					LoadInvisibleItemsCache(reader);
			}
		}
		PivotFieldValueItemsCreator GetCreator(bool isColumn) {
			return isColumn ? ColumnItemsCreator : RowItemsCreator;
		}
		void CreateInvisibleItemsCache(PivotFieldValueItemsCreator creator) {
			for(int i = 0; i < creator.Count; i++) {
				PivotFieldValueItem item = creator[i];
				if(!item.IsVisible)
					GetRowItemByInvisibleItem(item);
			}
		}
		void SaveInvisibleItemsCache(TypedBinaryWriter writer) {
			if(invisibleVisibleItemsCache == null) {
				writer.Write((int)0);
				return;
			}
			writer.Write(invisibleVisibleItemsCache.Count);
			foreach(KeyValuePair<PivotFieldValueItem, PivotFieldValueItem> pair in invisibleVisibleItemsCache) {
				PivotFieldValueItem invisibleItem = pair.Key,
					visibleItem = pair.Value;
				int pageStartRow = GetStartIndex(false);
				PivotFieldValueItemsCreator creator = RowItemsCreator;
				writer.Write(invisibleItem.Index);
				bool isVisibleItemOnScreen =
					(visibleItem.MinLastLevelIndex >= pageStartRow && visibleItem.MinLastLevelIndex < pageStartRow + creator.LastLevelItemCount) ||
					(visibleItem.MaxLastLevelIndex >= pageStartRow && visibleItem.MaxLastLevelIndex < pageStartRow + creator.LastLevelItemCount);
				writer.Write(isVisibleItemOnScreen);
				if(!isVisibleItemOnScreen) {
					byte type = (byte)visibleItem.ItemType;
					writer.Write(type);
					visibleItem.SaveToStream(writer);
				} else
					writer.Write(visibleItem.Index);
			}
		}
		void LoadInvisibleItemsCache(TypedBinaryReader reader) {
			if(invisibleVisibleItemsCache != null)
				invisibleVisibleItemsCache.Clear();
			else
				invisibleVisibleItemsCache = new Dictionary<PivotFieldValueItem, PivotFieldValueItem>();
			int pairCount = reader.ReadInt32();
			for(int i = 0; i < pairCount; i++) {
				int invisibleIndex = reader.ReadInt32();
				PivotFieldValueItemsCreator creator = RowItemsCreator;
				PivotFieldValueItem invisibleItem = creator[invisibleIndex];
				bool isVisibleItemOnScreen = reader.ReadBoolean();
				PivotFieldValueItem visibleItem = null;
				if(isVisibleItemOnScreen) {
					int visibleIndex = reader.ReadInt32();
					visibleItem = creator[visibleIndex];
				} else {
					PivotFieldValueItemType type = (PivotFieldValueItemType)reader.ReadByte();
					visibleItem = PivotFieldValueItem.LoadItem(type, creator.DataProvider, reader);
				}
				invisibleVisibleItemsCache.Add(invisibleItem, visibleItem);
			}
		}
		public string SaveFieldValueItemsState() {
			return DevExpress.Data.PivotGrid.PivotGridSerializeHelper.ToBase64StringDeflateBuffered(SaveFieldValueItemsStateCore, Data.OptionsData.CustomObjectConverter);
		}
		public string SaveDataCellsState() {
			return DevExpress.Data.PivotGrid.PivotGridSerializeHelper.ToBase64StringDeflateBuffered(SaveDataCellsStateCore, Data.OptionsData.CustomObjectConverter);
		}
		protected virtual void SaveDataCellsStateCore(TypedBinaryWriter stream) {
			CellDataProvider.SaveToStream(stream, ColumnItemsCreator, RowItemsCreator);
		}
		internal void RestoreFieldValueItemsState(string state) {
			if(string.IsNullOrEmpty(state)) return;
			MemoryStream stream = new MemoryStream(Convert.FromBase64String(state));
			Stream dstream = new DeflateStream(stream, CompressionMode.Decompress);
			RestoreFieldValueItemsStateCore(dstream);
		}
		internal void RestoreDataCellsState(string state) {
			if(string.IsNullOrEmpty(state)) return;
			StreamDataProvider = new PivotGridCellStreamDataProvider(Data);
			MemoryStream stream = new MemoryStream(Convert.FromBase64String(state));
			Stream dstream = new DeflateStream(stream, CompressionMode.Decompress);
			RestoreDataCellsStateCore(dstream);
		}
		protected virtual void RestoreDataCellsStateCore(Stream stream) {
			StreamDataProvider.LoadFromStream(stream, ColumnItemsCreator, RowItemsCreator);
		}
	}
	public class PivotFieldItem : PivotFieldItemBase {
		ITemplate headerTemplate;
		ITemplate valueTemplate;
		PivotCellStyle cellStyle;
		PivotHeaderStyle headerStyle;
		PivotFieldValueStyle valueStyle;
		PivotFieldValueStyle valueTotalStyle;
		ImageProperties filterButtonImage;
		ImageProperties deferFilterButtonImage;
		ImageProperties sortImage;
		string id;
		string clientID;
		bool exportBestFit;
		bool canSortOLAP;
		public PivotFieldItem(PivotGridData data, PivotGroupItemCollection groupItems, PivotGridFieldBase fieldBase)
			: base(data, groupItems, fieldBase) {
			if(fieldBase.IsRowTreeField)
				return;
			PivotGridField field = (PivotGridField)fieldBase;
			this.cellStyle = field.CellStyle;
			this.headerStyle = field.HeaderStyle;
			this.valueStyle = field.ValueStyle;
			this.valueTotalStyle = field.ValueTotalStyle;
			this.headerTemplate = field.HeaderTemplate;
			this.valueTemplate = field.ValueTemplate;
			this.deferFilterButtonImage = field.DeferFilterButtonImage;
			this.filterButtonImage = field.FilterButtonImage;
			this.sortImage = field.SortImage;
			this.id = field.ID;
			this.clientID = field.ClientID;
			this.exportBestFit = field.ExportBestFit;
			this.canSortOLAP = field.CanSortOLAP;
		}
		public PivotCellStyle CellStyle { get { return cellStyle; } }
		public PivotHeaderStyle HeaderStyle { get { return headerStyle; } }
		public PivotFieldValueStyle ValueStyle { get { return valueStyle; } }
		public PivotFieldValueStyle ValueTotalStyle { get { return valueTotalStyle; } }
		public ITemplate HeaderTemplate { get { return headerTemplate; } }
		public ITemplate ValueTemplate { get { return valueTemplate; } }
		public ImageProperties DeferFilterButtonImage { get { return deferFilterButtonImage; } }
		public ImageProperties FilterButtonImage { get { return filterButtonImage; } }
		public ImageProperties SortImage { get { return sortImage; } }
		public string ID { get { return id; } }
		internal string ClientID { get { return clientID; } }
		public bool ExportBestFit { get { return exportBestFit; } }
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class WebPrefilter : PrefilterBase {
		static WebPrefilter() {
			EnumProcessingHelper.RegisterEnum(typeof(System.DayOfWeek));
		}
		public WebPrefilter(IPrefilterOwnerBase owner)
			: base(owner) { }
		[
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DefaultValue(null),
		NotifyParentProperty(true), Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)
		]
		public new CriteriaOperator Criteria {
			get { return base.Criteria; }
			set { base.Criteria = value; }
		}
		[
#if !SL
	DevExpressWebASPxPivotGridLocalizedDescription("WebPrefilterCriteriaString"),
#endif
		DefaultValue(""), NotifyParentProperty(true), XtraSerializableProperty(),
		Browsable(true), EditorBrowsable(EditorBrowsableState.Always)
		]
		public new string CriteriaString {
			get { return base.CriteriaString; }
			set { base.CriteriaString = value; }
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Reset() {
			Enabled = true;
			Clear();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShouldSerialize() {
			return Enabled != true || !object.ReferenceEquals(Criteria, null);
		}
		internal void SetUserCriteriaString(string criteriaString) {
			bool shouldRaiseChangedEvent = !EnabledCore;
			EnabledCore = true;
			if(CriteriaString != criteriaString)
				shouldRaiseChangedEvent = false;
			CriteriaString = criteriaString;
			if(shouldRaiseChangedEvent)
				RaiseCriteriaChanged();
		}
	}
}
