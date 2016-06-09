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
using System.ComponentModel;
namespace DevExpress.Web.Mvc {
	using System.Linq.Expressions;
	using DevExpress.Data;
	using DevExpress.Web;
	using DevExpress.Web.Data;
	using DevExpress.Web.Mvc.Internal;
	public enum GridViewOperationType { Paging, Filtering, Sorting, Grouping };
	public class GridViewSettings<RowType>: GridViewSettings {
		public GridViewSettings() {
		}
		public new MVCxGridViewColumnCollection<RowType> Columns {
			get { return (MVCxGridViewColumnCollection<RowType>)base.Columns; }
		}
		public new MVCxGridViewFormLayoutProperties<RowType> EditFormLayoutProperties {
			get { return (MVCxGridViewFormLayoutProperties<RowType>)base.EditFormLayoutProperties; }
		}
		public void KeyFields<ValueType>(params Expression<Func<RowType, ValueType>>[] keyExpressions) {
			if(keyExpressions == null) return;
			KeyFieldName = string.Empty;
			List<string> keys = new List<string>();
			foreach (var expression in keyExpressions) {
				if (expression == null) continue;
				string key = ExtensionsHelper.GetFullHtmlFieldName(expression);
				keys.Add(key);
			}
			KeyFieldName = string.Join(WebDataProxy.MultipleKeyFieldSeparator.ToString(), keys);
		}
		public void PreviewField<ValueType>(Expression<Func<RowType, ValueType>> previewFieldExpression) {
			PreviewFieldName = ExtensionsHelper.GetFullHtmlFieldName(previewFieldExpression);
		}
		protected override MVCxGridViewColumnCollection CreateColumnCollection() {
			return new MVCxGridViewColumnCollection<RowType>();
		}
		protected override MVCxGridViewFormLayoutProperties CreateEditFormLayoutProperties() {
			return new MVCxGridViewFormLayoutProperties<RowType>(null);
		}
	}
	public class GridViewSettings : GridSettingsBase {
		MVCxGridViewColumnCollection columns;
		MVCxGridViewCommandColumn commandColumn;
		ASPxSummaryItemCollection groupSummary;
		ASPxGroupSummarySortInfoCollection groupSummarySortInfo;
		MVCxGridViewSettings settings;
		ASPxGridViewAdaptivitySettings settingsAdaptivity;
		ASPxGridViewContextMenuSettings settingsContextMenu;
		MVCxGridViewBehaviorSettings settingsBehavior;
		ASPxGridViewCookiesSettings settingsCookies;
		MVCxGridViewExportSettings settingsExport;
		ASPxGridViewLoadingPanelSettings settingsLoadingPanel;
		ASPxGridViewPagerSettings settingsPager;
		MVCxGridViewTextSettings settingsText;
		ASPxGridViewCustomizationWindowSettings settingsCustomizationWindowInternal;
		MVCxGridViewDetailSettings settingsDetail;
		MVCxGridViewEditingSettings settingsEditing;
		MVCxGridViewPopupControlSettings settingsPopup;
		ASPxGridViewCommandButtonSettings settingsCommandButton;
		ASPxGridViewDataSecuritySettings settingsDataSecurity;
		MVCxGridViewSearchPanelSettings settingsSearchPanel;
		ASPxGridViewFilterControlSettings settingsFilterControl;
		GridViewPopupControlStyles stylesPopup;
		GridViewEditorStyles stylesEditors;
		GridViewPagerStyles stylesPager;
		GridViewContextMenuStyles stylesContextMenu;
		ASPxSummaryItemCollection totalSummary;
		MVCxGridViewFormatConditionCollection formatConditions;
		MVCxGridViewFormLayoutProperties formLayoutProperties;
		public GridViewSettings() {
			this.columns = CreateColumnCollection();
			this.commandColumn = new MVCxGridViewCommandColumn();
			this.groupSummary = new ASPxSummaryItemCollection(null);
			this.groupSummarySortInfo = new ASPxGroupSummarySortInfoCollection(null);
			this.settings = new MVCxGridViewSettings();
			this.settingsAdaptivity = new ASPxGridViewAdaptivitySettings(null);
			this.settingsContextMenu = new ASPxGridViewContextMenuSettings(null);
			this.settingsBehavior = new MVCxGridViewBehaviorSettings();
			this.settingsCookies = new ASPxGridViewCookiesSettings(null);
			this.settingsExport = new MVCxGridViewExportSettings();
			this.settingsLoadingPanel = new ASPxGridViewLoadingPanelSettings(null);
			this.settingsPager = new ASPxGridViewPagerSettings(null);
			this.stylesPopup = new GridViewPopupControlStyles(null);
			this.stylesEditors = new GridViewEditorStyles(null);
			this.stylesPager = new GridViewPagerStyles(null);
			this.stylesContextMenu = new GridViewContextMenuStyles(null);
			this.settingsText = new MVCxGridViewTextSettings();
			this.totalSummary = new ASPxSummaryItemCollection(null);
			this.formatConditions = new MVCxGridViewFormatConditionCollection();
			this.settingsCustomizationWindowInternal = new ASPxGridViewCustomizationWindowSettings(null);
			this.settingsDetail = new MVCxGridViewDetailSettings();
			this.settingsEditing = new MVCxGridViewEditingSettings();
			this.settingsPopup = new MVCxGridViewPopupControlSettings();
			this.settingsCommandButton = new ASPxGridViewCommandButtonSettings(null);
			this.settingsDataSecurity = new ASPxGridViewDataSecuritySettings(null);
			this.settingsSearchPanel = new MVCxGridViewSearchPanelSettings(null);
			this.settingsFilterControl = new ASPxGridViewFilterControlSettings(null);
			this.formLayoutProperties = CreateEditFormLayoutProperties();
			CustomBindingRouteValuesCollection = new Dictionary<GridViewOperationType, object>();
			EnableRowsCache = false;
			ImagesEditors = new GridViewEditorImages(null);
		}
		public bool AccessibilityCompliant { get { return AccessibilityCompliantInternal; } set { AccessibilityCompliantInternal = value; } }
		public IDictionary<GridViewOperationType, object> CustomBindingRouteValuesCollection { get; private set; }
		public string Caption { get; set; }
		public GridViewClientSideEvents ClientSideEvents { get { return (GridViewClientSideEvents)ClientSideEventsInternal; } }
		public MVCxGridViewColumnCollection Columns { get { return columns; } }
		public MVCxGridViewCommandColumn CommandColumn { get { return commandColumn; } }
		public bool EnableRowsCache { get; set; }
		public ASPxSummaryItemCollection GroupSummary { get { return groupSummary; } }
		public ASPxGroupSummarySortInfoCollection GroupSummarySortInfo { get { return groupSummarySortInfo; } }
		public GridViewImages Images { get { return (GridViewImages)ImagesInternal; } }
		public GridViewEditorImages ImagesEditors { get; private set; }
		public bool KeyboardSupport { get; set; }
		public string PreviewFieldName { get; set; }
		public MVCxGridViewSettings Settings { get { return settings; } }
		public ASPxGridViewAdaptivitySettings SettingsAdaptivity { get { return settingsAdaptivity; } }
		public ASPxGridViewContextMenuSettings SettingsContextMenu { get { return settingsContextMenu; } }
		public MVCxGridViewBehaviorSettings SettingsBehavior { get { return settingsBehavior; } }
		public ASPxGridViewCookiesSettings SettingsCookies { get { return settingsCookies; } }
		public MVCxGridViewExportSettings SettingsExport { get { return settingsExport; } }
		public ASPxGridViewLoadingPanelSettings SettingsLoadingPanel { get { return settingsLoadingPanel; } }
		public ASPxGridViewPagerSettings SettingsPager { get { return settingsPager; } }
		public MVCxGridViewTextSettings SettingsText { get { return settingsText; } }
		[Obsolete("Use the SettingsPopup.CustomizationWindow property instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ASPxGridViewCustomizationWindowSettings SettingsCustomizationWindow { get { return SettingsCustomizationWindowInternal; } }
		protected internal ASPxGridViewCustomizationWindowSettings SettingsCustomizationWindowInternal { get { return settingsCustomizationWindowInternal; } }
		public MVCxGridViewEditingSettings SettingsEditing { get { return settingsEditing; } }
		public MVCxGridViewDetailSettings SettingsDetail { get { return settingsDetail; } }
		public MVCxGridViewPopupControlSettings SettingsPopup { get { return settingsPopup; } }
		public ASPxGridViewCommandButtonSettings SettingsCommandButton { get { return settingsCommandButton; } }
		public ASPxGridViewDataSecuritySettings SettingsDataSecurity { get { return settingsDataSecurity; } }
		public MVCxGridViewSearchPanelSettings SettingsSearchPanel { get { return settingsSearchPanel; } }
		public ASPxGridViewFilterControlSettings SettingsFilterControl { get { return settingsFilterControl; } }
		public GridViewStyles Styles { get { return (GridViewStyles)StylesInternal; } }
		public GridViewPopupControlStyles StylesPopup { get { return stylesPopup; } }
		public GridViewEditorStyles StylesEditors { get { return stylesEditors; } }
		public GridViewPagerStyles StylesPager { get { return stylesPager; } }
		public GridViewContextMenuStyles StylesContextMenu { get { return stylesContextMenu; } }
		public string SummaryText { get; set; }
		public ASPxSummaryItemCollection TotalSummary { get { return totalSummary; } }
		public MVCxGridViewFormatConditionCollection FormatConditions { get { return formatConditions; } }
		public MVCxGridViewFormLayoutProperties EditFormLayoutProperties { get { return formLayoutProperties; } }
		public ASPxGridViewAfterPerformCallbackEventHandler AfterPerformCallback { get; set; }
		public ASPxGridViewEditorCreateEventHandler AutoFilterCellEditorCreate { get; set; }
		public ASPxGridViewEditorEventHandler AutoFilterCellEditorInitialize { get; set; }
		public ASPxGridViewSearchPanelEditorCreateEventHandler SearchPanelEditorCreate { get; set; }
		public ASPxGridViewSearchPanelEditorEventHandler SearchPanelEditorInitialize { get; set; }
		public ASPxGridViewBeforeColumnGroupingSortingEventHandler BeforeColumnSortingGrouping { get; set; }
		public ASPxGridViewBeforeHeaderFilterFillItemsEventHandler BeforeHeaderFilterFillItems { get; set; }
		public ASPxGridViewEditorEventHandler CellEditorInitialize { get; set; }
		public ASPxGridViewCommandButtonEventHandler CommandButtonInitialize { get; set; }
		public ASPxGridViewContextMenuItemVisibilityEventHandler ContextMenuItemVisibility { get; set; }
		public ASPxGridViewCustomButtonEventHandler CustomButtonInitialize { get; set; }
		[Obsolete("Use the GridViewSettings.CustomActionRouteValues property instead."), 
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ASPxGridViewCustomCallbackEventHandler CustomCallback { 
			get { return CustomCallbackInternal; } 
			set { CustomCallbackInternal = value; } 
		}
		[Obsolete("Use the GridViewSettings.CustomDataActionRouteValues property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public ASPxGridViewCustomDataCallbackEventHandler CustomDataCallback {
			get { return CustomDataCallbackInternal; }
			set { CustomDataCallbackInternal = value; } 
		}
		public ASPxGridViewClientJSPropertiesEventHandler CustomJSProperties { get; set; }
		public ASPxGridViewColumnDataEventHandler CustomUnboundColumnData { get; set; }
		public CustomSummaryEventHandler CustomSummaryCalculate { get; set; }
		public ASPxGridViewColumnDisplayTextEventHandler CustomGroupDisplayText { get; set; }
		public ASPxGridViewColumnDisplayTextEventHandler CustomColumnDisplayText { get; set; }
		public ASPxGridViewCustomColumnSortEventHandler CustomColumnGroup { get; set; }
		public ASPxGridViewCustomColumnSortEventHandler CustomColumnSort { get; set; }
		public ASPxGridViewDetailRowEventHandler DetailRowExpandedChanged { get; set; }
		public ASPxGridViewFillContextMenuItemsEventHandler FillContextMenuItems { get; set; }
		public ASPxGridViewHeaderFilterEventHandler HeaderFilterFillItems { get; set; }
		public ASPxGridViewTableCommandCellEventHandler HtmlCommandCellPrepared { get; set; }
		public ASPxGridViewTableDataCellEventHandler HtmlDataCellPrepared { get; set; }
		public ASPxGridViewEditFormEventHandler HtmlEditFormCreated { get; set; }
		public ASPxGridViewTableFooterCellEventHandler HtmlFooterCellPrepared { get; set; }
		public ASPxGridViewTableRowEventHandler HtmlRowCreated { get; set; }
		public ASPxGridViewTableRowEventHandler HtmlRowPrepared { get; set; }
		public ASPxDataInitNewRowEventHandler InitNewRow { get; set; }
		[Obsolete("Use the GridViewOperationType.Paging Action declared in the GridViewSettings.CustomBindingRouteValuesCollection property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public EventHandler PageIndexChanged { get { return PageIndexChangedInternal; } set { PageIndexChangedInternal = value; } }
		public ASPxGridViewAutoFilterEventHandler ProcessColumnAutoFilter { get; set; }
		public ASPxGridViewSummaryDisplayTextEventHandler SummaryDisplayText { get; set; }
		public ASPxDataValidationEventHandler RowValidating { get; set; }
		protected internal ASPxGridViewCustomCallbackEventHandler CustomCallbackInternal { get; set; }
		protected internal ASPxGridViewCustomDataCallbackEventHandler CustomDataCallbackInternal { get; set; }
		protected internal EventHandler PageIndexChangedInternal { get; set; }
		protected internal string HeaderTemplateContent { get; set; }
		protected internal Action<GridViewHeaderTemplateContainer> HeaderTemplateContentMethod { get; set; }
		protected internal string HeaderCaptionTemplateContent { get; set; }
		protected internal Action<GridViewHeaderTemplateContainer> HeaderCaptionTemplateContentMethod { get; set; }
		protected internal string DataRowTemplateContent { get; set; }
		protected internal Action<GridViewDataRowTemplateContainer> DataRowTemplateContentMethod { get; set; }
		protected internal string DataItemTemplateContent { get; set; }
		protected internal Action<GridViewDataItemTemplateContainer> DataItemTemplateContentMethod { get; set; }
		protected internal string GroupRowTemplateContent { get; set; }
		protected internal Action<GridViewGroupRowTemplateContainer> GroupRowTemplateContentMethod { get; set; }
		protected internal string GroupRowContentTemplateContent { get; set; }
		protected internal Action<GridViewGroupRowTemplateContainer> GroupRowContentTemplateContentMethod { get; set; }
		protected internal string PreviewRowTemplateContent { get; set; }
		protected internal Action<GridViewPreviewRowTemplateContainer> PreviewRowTemplateContentMethod { get; set; }
		protected internal string EmptyDataRowTemplateContent { get; set; }
		protected internal Action<GridViewEmptyDataRowTemplateContainer> EmptyDataRowTemplateContentMethod { get; set; }
		protected internal string FilterRowTemplateContent { get; set; }
		protected internal Action<GridViewFilterRowTemplateContainer> FilterRowTemplateContentMethod { get; set; }
		protected internal string FilterCellTemplateContent { get; set; }
		protected internal Action<GridViewFilterCellTemplateContainer> FilterCellTemplateContentMethod { get; set; }
		protected internal string FooterRowTemplateContent { get; set; }
		protected internal Action<GridViewFooterRowTemplateContainer> FooterRowTemplateContentMethod { get; set; }
		protected internal string FooterCellTemplateContent { get; set; }
		protected internal Action<GridViewFooterCellTemplateContainer> FooterCellTemplateContentMethod { get; set; }
		protected internal string StatusBarTemplateContent { get; set; }
		protected internal Action<GridViewStatusBarTemplateContainer> StatusBarTemplateContentMethod { get; set; }
		protected internal string TitlePanelTemplateContent { get; set; }
		protected internal Action<GridViewTitleTemplateContainer> TitlePanelTemplateContentMethod { get; set; }
		protected internal string PagerBarTemplateContent { get; set; }
		protected internal Action<GridViewPagerBarTemplateContainer> PagerBarTemplateContentMethod { get; set; }
		protected internal string DetailRowTemplateContent { get; set; }
		protected internal Action<GridViewDetailRowTemplateContainer> DetailRowTemplateContentMethod { get; set; }
		protected internal string EditFormTemplateContent { get; set; }
		protected internal Action<GridViewEditFormTemplateContainer> EditFormTemplateContentMethod { get; set; }
		public void SetHeaderTemplateContent(Action<GridViewHeaderTemplateContainer> contentMethod) {
			HeaderTemplateContentMethod = contentMethod;
		}
		public void SetHeaderTemplateContent(string content) {
			HeaderTemplateContent = content;
		}
		public void SetHeaderCaptionTemplateContent(Action<GridViewHeaderTemplateContainer> contentMethod) {
			HeaderCaptionTemplateContentMethod = contentMethod;
		}
		public void SetHeaderCaptionTemplateContent(string content) {
			HeaderCaptionTemplateContent = content;
		}
		public void SetDataRowTemplateContent(Action<GridViewDataRowTemplateContainer> contentMethod) {
			DataRowTemplateContentMethod = contentMethod;
		}
		public void SetDataRowTemplateContent(string content) {
			DataRowTemplateContent = content;
		}
		public void SetDataItemTemplateContent(Action<GridViewDataItemTemplateContainer> contentMethod) {
			DataItemTemplateContentMethod = contentMethod;
		}
		public void SetDataItemTemplateContent(string content) {
			DataItemTemplateContent = content;
		}
		public void SetGroupRowTemplateContent(Action<GridViewGroupRowTemplateContainer> contentMethod) {
			GroupRowTemplateContentMethod = contentMethod;
		}
		public void SetGroupRowTemplateContent(string content) {
			GroupRowTemplateContent = content;
		}
		public void SetGroupRowContentTemplateContent(Action<GridViewGroupRowTemplateContainer> contentMethod) {
			GroupRowContentTemplateContentMethod = contentMethod;
		}
		public void SetGroupRowContentTemplateContent(string content) {
			GroupRowContentTemplateContent = content;
		}
		public void SetPreviewRowTemplateContent(Action<GridViewPreviewRowTemplateContainer> contentMethod) {
			PreviewRowTemplateContentMethod = contentMethod;
		}
		public void SetPreviewRowTemplateContent(string content) {
			PreviewRowTemplateContent = content;
		}
		public void SetEmptyDataRowTemplateContent(Action<GridViewEmptyDataRowTemplateContainer> contentMethod) {
			EmptyDataRowTemplateContentMethod = contentMethod;
		}
		public void SetEmptyDataRowTemplateContent(string content) {
			EmptyDataRowTemplateContent = content;
		}
		public void SetFilterRowTemplateContent(Action<GridViewFilterRowTemplateContainer> contentMethod) {
			FilterRowTemplateContentMethod = contentMethod;
		}
		public void SetFilterRowTemplateContent(string content) {
			FilterRowTemplateContent = content;
		}
		public void SetFilterCellTemplateContent(Action<GridViewFilterCellTemplateContainer> contentMethod) {
			FilterCellTemplateContentMethod = contentMethod;
		}
		public void SetFilterCellTemplateContent(string content) {
			FilterCellTemplateContent = content;
		}
		public void SetFooterRowTemplateContent(Action<GridViewFooterRowTemplateContainer> contentMethod) {
			FooterRowTemplateContentMethod = contentMethod;
		}
		public void SetFooterRowTemplateContent(string content) {
			FooterRowTemplateContent = content;
		}
		public void SetFooterCellTemplateContent(Action<GridViewFooterCellTemplateContainer> contentMethod) {
			FooterCellTemplateContentMethod = contentMethod;
		}
		public void SetFooterCellTemplateContent(string content) {
			FooterCellTemplateContent = content;
		}
		public void SetStatusBarTemplateContent(Action<GridViewStatusBarTemplateContainer> contentMethod) {
			StatusBarTemplateContentMethod = contentMethod;
		}
		public void SetStatusBarTemplateContent(string content) {
			StatusBarTemplateContent = content;
		}
		public void SetTitlePanelTemplateContent(Action<GridViewTitleTemplateContainer> contentMethod) {
			TitlePanelTemplateContentMethod = contentMethod;
		}
		public void SetTitlePanelTemplateContent(string content) {
			TitlePanelTemplateContent = content;
		}
		public void SetPagerBarTemplateContent(Action<GridViewPagerBarTemplateContainer> contentMethod) {
			PagerBarTemplateContentMethod = contentMethod;
		}
		public void SetPagerBarTemplateContent(string content) {
			PagerBarTemplateContent = content;
		}
		public void SetDetailRowTemplateContent(Action<GridViewDetailRowTemplateContainer> contentMethod) {
			DetailRowTemplateContentMethod = contentMethod;
		}
		public void SetDetailRowTemplateContent(string content) {
			DetailRowTemplateContent = content;
		}
		public void SetEditFormTemplateContent(Action<GridViewEditFormTemplateContainer> contentMethod) {
			EditFormTemplateContentMethod = contentMethod;
		}
		public void SetEditFormTemplateContent(string content) {
			EditFormTemplateContent = content;
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new GridViewClientSideEvents();
		}
		protected override ImagesBase CreateImages() {
			return new GridViewImages(null);
		}
		protected override StylesBase CreateStyles() {
			return new GridViewStyles(null);
		}
		protected virtual MVCxGridViewColumnCollection CreateColumnCollection() {
			return new MVCxGridViewColumnCollection();
		}
		protected virtual MVCxGridViewFormLayoutProperties CreateEditFormLayoutProperties() {
			return new MVCxGridViewFormLayoutProperties(null);
		}
	}
}
