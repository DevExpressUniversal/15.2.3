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
using DevExpress.Data;
using DevExpress.Web.Data;
using DevExpress.Web.Internal;
using DevExpress.Web.Mvc.Internal;
namespace DevExpress.Web.Mvc {
	public class MVCxGridViewProperties: GridViewProperties {
		public MVCxGridViewProperties(GridViewSettings gridViewSettings)
			: base(null) {
			GridViewSettings = gridViewSettings;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool EnableCallBacks { get; set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new ASPxGridViewCustomizationWindowSettings SettingsCustomizationWindow {
			get { return GridViewSettings.SettingsCustomizationWindowInternal; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool DataSourceForceStandardPaging { get; set; }
		public object CallbackRouteValues {
			get { return GridViewSettings.CallbackRouteValues; }
			set { GridViewSettings.CallbackRouteValues = value; }
		}
		public object CustomDataActionRouteValues {
			get { return GridViewSettings.CustomDataActionRouteValues; }
			set { GridViewSettings.CustomDataActionRouteValues = value; }
		}
		public IDictionary<GridViewOperationType, object> CustomBindingRouteValuesCollection {
			get { return GridViewSettings.CustomBindingRouteValuesCollection; }
		}
		[DefaultValue(false)]
		public new bool EnableRowsCache { 
			get { return GridViewSettings.EnableRowsCache; } 
			set { GridViewSettings.EnableRowsCache = value; }
		}
		public new bool EnableCallbackCompression {
			get { return GridViewSettings.EnableCallbackCompression; }
			set { GridViewSettings.EnableCallbackCompression = value; }
		}
		public new bool EnableCallbackAnimation {
			get { return GridViewSettings.EnableCallbackAnimation; }
			set { GridViewSettings.EnableCallbackAnimation = value; }
		}
		public new bool EnablePagingCallbackAnimation {
			get { return GridViewSettings.EnablePagingCallbackAnimation; }
			set { GridViewSettings.EnablePagingCallbackAnimation = value; }
		}
		public new AutoBoolean EnablePagingGestures {
			get { return GridViewSettings.EnablePagingGestures; }
			set { GridViewSettings.EnablePagingGestures = value; }
		}
		public new bool AccessibilityCompliant {
			get { return GridViewSettings.AccessibilityCompliant; }
			set { GridViewSettings.AccessibilityCompliant = value; }
		}
		public new string Caption {
			get { return GridViewSettings.Caption; }
			set { GridViewSettings.Caption = value; }
		}
		public new string SummaryText {
			get { return GridViewSettings.SummaryText; }
			set { GridViewSettings.SummaryText = value; }
		}
		public new string PreviewFieldName {
			get { return GridViewSettings.PreviewFieldName; }
			set { GridViewSettings.PreviewFieldName = value; }
		}
		public new ASPxSummaryItemCollection TotalSummary { get { return GridViewSettings.TotalSummary; } }
		public new ASPxSummaryItemCollection GroupSummary { get { return GridViewSettings.GroupSummary; } }
		public new MVCxGridViewBehaviorSettings SettingsBehavior { get { return GridViewSettings.SettingsBehavior; } }
		public new ASPxGridViewPagerSettings SettingsPager { get { return GridViewSettings.SettingsPager; } }
		public new MVCxGridViewEditingSettings SettingsEditing { get { return GridViewSettings.SettingsEditing; } }
		public new MVCxGridViewSettings Settings { get { return GridViewSettings.Settings; } }
		public new MVCxGridViewTextSettings SettingsText { get { return GridViewSettings.SettingsText; } }
		public new ASPxGridViewLoadingPanelSettings SettingsLoadingPanel { get { return GridViewSettings.SettingsLoadingPanel; } }
		public new ASPxGridViewCookiesSettings SettingsCookies { get { return GridViewSettings.SettingsCookies; } }
		public new MVCxGridViewDetailSettings SettingsDetail { get { return GridViewSettings.SettingsDetail; } }
		public new MVCxGridViewPopupControlSettings SettingsPopup { get { return GridViewSettings.SettingsPopup; } }
		public new ASPxGridViewCommandButtonSettings SettingsCommandButton { get { return GridViewSettings.SettingsCommandButton; } }
		public new ASPxGridViewDataSecuritySettings SettingsDataSecurity { get { return GridViewSettings.SettingsDataSecurity; } }
		public new MVCxGridViewSearchPanelSettings SettingsSearchPanel { get { return GridViewSettings.SettingsSearchPanel; } }
		protected GridViewSettings GridViewSettings { get; private set; }
		public ASPxGridViewEditorCreateEventHandler AutoFilterCellEditorCreate {
			get { return GridViewSettings.AutoFilterCellEditorCreate; }
			set { GridViewSettings.AutoFilterCellEditorCreate = value; }
		}
		public ASPxGridViewEditorEventHandler AutoFilterCellEditorInitialize {
			get { return GridViewSettings.AutoFilterCellEditorInitialize; }
			set { GridViewSettings.AutoFilterCellEditorInitialize = value; }
		}
		public ASPxGridViewSearchPanelEditorCreateEventHandler SearchPanelEditorCreate {
			get { return GridViewSettings.SearchPanelEditorCreate; }
			set { GridViewSettings.SearchPanelEditorCreate = value; }
		}
		public ASPxGridViewSearchPanelEditorEventHandler SearchPanelEditorInitialize {
			get { return GridViewSettings.SearchPanelEditorInitialize; }
			set { GridViewSettings.SearchPanelEditorInitialize = value; }
		}
		public ASPxGridViewBeforeColumnGroupingSortingEventHandler BeforeColumnSortingGrouping {
			get { return GridViewSettings.BeforeColumnSortingGrouping; }
			set { GridViewSettings.BeforeColumnSortingGrouping = value; }
		}
		public EventHandler BeforeGetCallbackResult { 
			get { return GridViewSettings.BeforeGetCallbackResult; }
			set { GridViewSettings.BeforeGetCallbackResult = value; } 
		}
		public ASPxGridViewBeforeHeaderFilterFillItemsEventHandler BeforeHeaderFilterFillItems {
			get { return GridViewSettings.BeforeHeaderFilterFillItems; }
			set { GridViewSettings.BeforeHeaderFilterFillItems = value; }
		}
		public ASPxGridViewEditorEventHandler CellEditorInitialize {
			get { return GridViewSettings.CellEditorInitialize; }
			set { GridViewSettings.CellEditorInitialize = value; }
		}
		public ASPxClientLayoutHandler ClientLayout {
			get { return GridViewSettings.ClientLayout; }
			set { GridViewSettings.ClientLayout = value; } 
		}
		public ASPxGridViewCommandButtonEventHandler CommandButtonInitialize {
			get { return GridViewSettings.CommandButtonInitialize; }
			set { GridViewSettings.CommandButtonInitialize = value; }
		}
		public ASPxGridViewCustomButtonEventHandler CustomButtonInitialize {
			get { return GridViewSettings.CustomButtonInitialize; }
			set { GridViewSettings.CustomButtonInitialize = value; }
		}
		public ASPxGridViewColumnDataEventHandler CustomUnboundColumnData {
			get { return GridViewSettings.CustomUnboundColumnData; }
			set { GridViewSettings.CustomUnboundColumnData = value; }
		}
		public CustomSummaryEventHandler CustomSummaryCalculate {
			get { return GridViewSettings.CustomSummaryCalculate; }
			set { GridViewSettings.CustomSummaryCalculate = value; }
		}
		public ASPxGridViewColumnDisplayTextEventHandler CustomGroupDisplayText {
			get { return GridViewSettings.CustomGroupDisplayText; }
			set { GridViewSettings.CustomGroupDisplayText = value; }
		}
		public ASPxGridViewColumnDisplayTextEventHandler CustomColumnDisplayText {
			get { return GridViewSettings.CustomColumnDisplayText; }
			set { GridViewSettings.CustomColumnDisplayText = value; }
		}
		public ASPxGridViewCustomColumnSortEventHandler CustomColumnGroup {
			get { return GridViewSettings.CustomColumnGroup; }
			set { GridViewSettings.CustomColumnGroup = value; }
		}
		public ASPxGridViewCustomColumnSortEventHandler CustomColumnSort {
			get { return GridViewSettings.CustomColumnSort; }
			set { GridViewSettings.CustomColumnSort = value; }
		}
		public ASPxGridViewHeaderFilterEventHandler HeaderFilterFillItems {
			get { return GridViewSettings.HeaderFilterFillItems; }
			set { GridViewSettings.HeaderFilterFillItems = value; }
		}
		public ASPxGridViewTableCommandCellEventHandler HtmlCommandCellPrepared {
			get { return GridViewSettings.HtmlCommandCellPrepared; }
			set { GridViewSettings.HtmlCommandCellPrepared = value; } 
		}
		public ASPxGridViewTableDataCellEventHandler HtmlDataCellPrepared {
			get { return GridViewSettings.HtmlDataCellPrepared; }
			set { GridViewSettings.HtmlDataCellPrepared = value; }
		}
		public ASPxGridViewEditFormEventHandler HtmlEditFormCreated {
			get { return GridViewSettings.HtmlEditFormCreated; }
			set { GridViewSettings.HtmlEditFormCreated = value; }
		}
		public ASPxGridViewTableFooterCellEventHandler HtmlFooterCellPrepared {
			get { return GridViewSettings.HtmlFooterCellPrepared; }
			set { GridViewSettings.HtmlFooterCellPrepared = value; }
		}
		public ASPxGridViewTableRowEventHandler HtmlRowCreated {
			get { return GridViewSettings.HtmlRowCreated; }
			set { GridViewSettings.HtmlRowCreated = value; }
		}
		public ASPxGridViewTableRowEventHandler HtmlRowPrepared {
			get { return GridViewSettings.HtmlRowPrepared; }
			set { GridViewSettings.HtmlRowPrepared = value; }
		}
		public ASPxDataInitNewRowEventHandler InitNewRow {
			get { return GridViewSettings.InitNewRow; }
			set { GridViewSettings.InitNewRow = value; }
		}
		public ASPxGridViewAutoFilterEventHandler ProcessColumnAutoFilter {
			get { return GridViewSettings.ProcessColumnAutoFilter; }
			set { GridViewSettings.ProcessColumnAutoFilter = value; }
		}
		public ASPxGridViewSummaryDisplayTextEventHandler SummaryDisplayText {
			get { return GridViewSettings.SummaryDisplayText; }
			set { GridViewSettings.SummaryDisplayText = value; }
		}
		public ASPxDataValidationEventHandler RowValidating {
			get { return GridViewSettings.RowValidating; }
			set { GridViewSettings.RowValidating = value;}
		}
		public FilterControlOperationVisibilityEventHandler FilterControlOperationVisibility {
			get { return GridViewSettings.FilterControlOperationVisibility; }
			set { GridViewSettings.FilterControlOperationVisibility = value; }
		}
		public FilterControlParseValueEventHandler FilterControlParseValue {
			get { return GridViewSettings.FilterControlParseValue; }
			set { GridViewSettings.FilterControlParseValue = value; }
		}
		public FilterControlCustomValueDisplayTextEventHandler FilterControlCustomValueDisplayText {
			get { return GridViewSettings.FilterControlCustomValueDisplayText; }
			set { GridViewSettings.FilterControlCustomValueDisplayText = value; }
		}
		public void SetHeaderTemplateContent(Action<GridViewHeaderTemplateContainer> contentMethod) {
			GridViewSettings.HeaderTemplateContentMethod = contentMethod;
		}
		public void SetHeaderTemplateContent(string content) {
			GridViewSettings.HeaderTemplateContent = content;
		}
		public void SetHeaderCaptionTemplateContent(Action<GridViewHeaderTemplateContainer> contentMethod) {
			GridViewSettings.HeaderCaptionTemplateContentMethod = contentMethod;
		}
		public void SetHeaderCaptionTemplateContent(string content) {
			GridViewSettings.HeaderCaptionTemplateContent = content;
		}
		public void SetDataRowTemplateContent(Action<GridViewDataRowTemplateContainer> contentMethod) {
			GridViewSettings.DataRowTemplateContentMethod = contentMethod;
		}
		public void SetDataRowTemplateContent(string content) {
			GridViewSettings.DataRowTemplateContent = content;
		}
		public void SetDataItemTemplateContent(Action<GridViewDataItemTemplateContainer> contentMethod) {
			GridViewSettings.DataItemTemplateContentMethod = contentMethod;
		}
		public void SetDataItemTemplateContent(string content) {
			GridViewSettings.DataItemTemplateContent = content;
		}
		public void SetGroupRowTemplateContent(Action<GridViewGroupRowTemplateContainer> contentMethod) {
			GridViewSettings.GroupRowTemplateContentMethod = contentMethod;
		}
		public void SetGroupRowTemplateContent(string content) {
			GridViewSettings.GroupRowTemplateContent = content;
		}
		public void SetGroupRowContentTemplateContent(Action<GridViewGroupRowTemplateContainer> contentMethod) {
			GridViewSettings.GroupRowContentTemplateContentMethod = contentMethod;
		}
		public void SetGroupRowContentTemplateContent(string content) {
			GridViewSettings.GroupRowContentTemplateContent = content;
		}
		public void SetPreviewRowTemplateContent(Action<GridViewPreviewRowTemplateContainer> contentMethod) {
			GridViewSettings.PreviewRowTemplateContentMethod = contentMethod;
		}
		public void SetPreviewRowTemplateContent(string content) {
			GridViewSettings.PreviewRowTemplateContent = content;
		}
		public void SetEmptyDataRowTemplateContent(Action<GridViewEmptyDataRowTemplateContainer> contentMethod) {
			GridViewSettings.EmptyDataRowTemplateContentMethod = contentMethod;
		}
		public void SetEmptyDataRowTemplateContent(string content) {
			GridViewSettings.EmptyDataRowTemplateContent = content;
		}
		public void SetFilterRowTemplateContent(Action<GridViewFilterRowTemplateContainer> contentMethod) {
			GridViewSettings.FilterRowTemplateContentMethod = contentMethod;
		}
		public void SetFilterRowTemplateContent(string content) {
			GridViewSettings.FilterRowTemplateContent = content;
		}
		public void SetFilterCellTemplateContent(Action<GridViewFilterCellTemplateContainer> contentMethod) {
			GridViewSettings.FilterCellTemplateContentMethod = contentMethod;
		}
		public void SetFilterCellTemplateContent(string content) {
			GridViewSettings.FilterCellTemplateContent = content;
		}
		public void SetFooterRowTemplateContent(Action<GridViewFooterRowTemplateContainer> contentMethod) {
			GridViewSettings.FooterRowTemplateContentMethod = contentMethod;
		}
		public void SetFooterRowTemplateContent(string content) {
			GridViewSettings.FooterRowTemplateContent = content;
		}
		public void SetFooterCellTemplateContent(Action<GridViewFooterCellTemplateContainer> contentMethod) {
			GridViewSettings.FooterCellTemplateContentMethod = contentMethod;
		}
		public void SetFooterCellTemplateContent(string content) {
			GridViewSettings.FooterCellTemplateContent = content;
		}
		public void SetStatusBarTemplateContent(Action<GridViewStatusBarTemplateContainer> contentMethod) {
			GridViewSettings.StatusBarTemplateContentMethod = contentMethod;
		}
		public void SetStatusBarTemplateContent(string content) {
			GridViewSettings.StatusBarTemplateContent = content;
		}
		public void SetTitlePanelTemplateContent(Action<GridViewTitleTemplateContainer> contentMethod) {
			GridViewSettings.TitlePanelTemplateContentMethod = contentMethod;
		}
		public void SetTitlePanelTemplateContent(string content) {
			GridViewSettings.TitlePanelTemplateContent = content;
		}
		public void SetPagerBarTemplateContent(Action<GridViewPagerBarTemplateContainer> contentMethod) {
			GridViewSettings.PagerBarTemplateContentMethod = contentMethod;
		}
		public void SetPagerBarTemplateContent(string content) {
			GridViewSettings.PagerBarTemplateContent = content;
		}
		public void SetDetailRowTemplateContent(Action<GridViewDetailRowTemplateContainer> contentMethod) {
			GridViewSettings.DetailRowTemplateContentMethod = contentMethod;
		}
		public void SetDetailRowTemplateContent(string content) {
			GridViewSettings.DetailRowTemplateContent = content;
		}
		public void SetEditFormTemplateContent(Action<GridViewEditFormTemplateContainer> contentMethod) {
			GridViewSettings.EditFormTemplateContentMethod = contentMethod;
		}
		public void SetEditFormTemplateContent(string content) {
			GridViewSettings.EditFormTemplateContent = content;
		}
	}
	public class MVCxGridLookupProperties: GridLookupProperties {
		public MVCxGridLookupProperties()
			: base(null) {
		}
		public MVCxGridLookupProperties(IPropertiesOwner owner)
			: base(owner) {
		}
		[Obsolete("Use the GridLookupSettings.EncodeHtml property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new bool EncodeHtml { get { return base.EncodeHtml; } set { base.EncodeHtml = value; } }
		GridLookupSelectionMode selectionMode;
		protected override GridLookupSelectionMode GetSelectionMode() {
			return Owner != null ? base.GetSelectionMode() : this.selectionMode;
		}
		protected override void SetSelectionMode(GridLookupSelectionMode selectionMode) {
			if(Owner != null)
				base.SetSelectionMode(selectionMode);
			else
				this.selectionMode = selectionMode;
		}
		public new string Caption {
			get { return base.Caption; }
			set { base.Caption = value; }
		}
		public new EditorCaptionSettings CaptionSettings {
			get { return base.CaptionSettings; }
		}
		public new EditorCaptionCellStyle CaptionCellStyle {
			get { return base.CaptionCellStyle; }
		}
		public new EditorCaptionStyle CaptionStyle {
			get { return base.CaptionStyle; }
		}
		public new EditorRootStyle RootStyle {
			get { return base.RootStyle; }
		}
		protected override SelectionStrategyOwner CreateSelectionStrategyOwner() {
			return new MVCxSelectionStrategyOwner(this);
		}
	}
}
