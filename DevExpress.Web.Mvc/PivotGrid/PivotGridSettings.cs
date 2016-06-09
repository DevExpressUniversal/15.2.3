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
namespace DevExpress.Web.Mvc {
	using DevExpress.Web.ASPxPivotGrid;
	using DevExpress.Web;
	using DevExpress.Utils;
	using DevExpress.Web.ASPxPivotGrid.Data;
	public class PivotGridSettings : SettingsBase {
		MVCxPivotGridFieldCollection fields;
		MVCxPivotGridWebGroupCollection groups;
		MVCxPivotGridWebOptionsView optionsView;
		PivotGridWebOptionsBehavior optionsBehavior;
		PivotGridWebOptionsCustomization optionsCustomization;
		PivotGridWebOptionsFilter optionsFilter;
		MVCxPivotGridWebOptionsLoadingPanel optionsLoadingPanel;
		XtraPivotGrid.PivotGridOptionsOLAP optionsOLAP;
		PivotGridWebOptionsPager optionsPager;
		MVCxPivotGridWebOptionsData optionsData;
		MVCxPivotGridWebOptionsDataField optionsDataField;
		PivotGridWebOptionsChartDataSource optionsChartDataSource;
		PivotGridStyles styles;
		PivotGridPagerStyles stylesPager;
		EditorStyles stylesEditors;
		MVCxPivotGridExportSettings settingsExport;
		WebPrefilter prefilter;
		PivotCustomizationExtensionSettings pivotCustomizationExtensionSettings;
		public PivotGridSettings() {
			this.fields = new MVCxPivotGridFieldCollection();
			this.groups = new MVCxPivotGridWebGroupCollection();
			this.optionsView = new MVCxPivotGridWebOptionsView();
			this.optionsView.ShowAllTotals();
			this.optionsBehavior = new PivotGridWebOptionsBehavior(null);
			this.optionsCustomization = new PivotGridWebOptionsCustomization(null, null, null, "OptionsCustomization");
			this.optionsFilter = new PivotGridWebOptionsFilter(null, null, "OptionsFilter");
			this.optionsLoadingPanel = new MVCxPivotGridWebOptionsLoadingPanel();
			this.optionsOLAP = new XtraPivotGrid.PivotGridOptionsOLAP(null);
			this.optionsPager = new PivotGridWebOptionsPager(null, null, "OptionsPager");
			this.optionsData = new MVCxPivotGridWebOptionsData();
			this.optionsDataField = new MVCxPivotGridWebOptionsDataField();
			this.optionsChartDataSource = new PivotGridWebOptionsChartDataSource(null);
			this.styles = new PivotGridStyles(null);
			this.stylesPager = new PivotGridPagerStyles(null);
			this.stylesEditors = new EditorStyles(null);
			this.settingsExport = new MVCxPivotGridExportSettings();
			this.prefilter = new WebPrefilter(null);
			this.pivotCustomizationExtensionSettings = new PivotCustomizationExtensionSettings();
			EnablePagingGestures = AutoBoolean.Auto;
			EnableRowsCache = true;
		}
		public object CallbackRouteValues { get; set; }
		public object CustomActionRouteValues { get; set; }
		public bool EnableCallbackAnimation { get; set; }
		public bool EnablePagingCallbackAnimation { get; set; }
		public AutoBoolean EnablePagingGestures { get; set; }
		public bool EnableRowsCache { get; set; }
		public MVCxPivotGridFieldCollection Fields { get { return fields; } }
		public MVCxPivotGridWebGroupCollection Groups { get { return groups; } }
		public PivotGridClientSideEvents ClientSideEvents { get { return (PivotGridClientSideEvents)ClientSideEventsInternal; } }
		public MVCxPivotGridWebOptionsView OptionsView { get { return optionsView; } }
		public PivotGridWebOptionsBehavior OptionsBehavior { get { return optionsBehavior; } }
		public PivotGridWebOptionsCustomization OptionsCustomization { get { return optionsCustomization; } }
		public PivotGridWebOptionsFilter OptionsFilter { get { return optionsFilter; } }
		public MVCxPivotGridWebOptionsLoadingPanel OptionsLoadingPanel { get { return optionsLoadingPanel; } }
		public XtraPivotGrid.PivotGridOptionsOLAP OptionsOLAP { get { return optionsOLAP; } }
		public PivotGridWebOptionsPager OptionsPager { get { return optionsPager; } }
		public MVCxPivotGridWebOptionsData OptionsData { get { return optionsData; } }
		public MVCxPivotGridWebOptionsDataField OptionsDataField { get { return optionsDataField; } }
		public PivotGridWebOptionsChartDataSource OptionsChartDataSource { get { return optionsChartDataSource; } }
		public PivotGridStyles Styles { get { return styles; } }
		public PivotGridPagerStyles StylesPager { get { return stylesPager; } }
		public EditorStyles StylesEditors { get { return stylesEditors; } }
		public bool CustomizationFieldsVisible { get; set; }
		public int CustomizationFieldsLeft { get; set; }
		public int CustomizationFieldsTop { get; set; }
		public string SummaryText { get; set; }
		public XtraPivotGrid.OLAPDataProvider OLAPDataProvider { get; set; }
		public MVCxPivotGridExportSettings SettingsExport { get { return settingsExport; } }
		public WebPrefilter Prefilter { get { return prefilter; } }
		public PivotCustomizationExtensionSettings PivotCustomizationExtensionSettings { get { return pivotCustomizationExtensionSettings; } }
		public EventHandler AfterPerformCallback { get; set; }
		public EventHandler BeforeGetCallbackResult { get; set; }
		[Obsolete("Use the PivotGridSettings.CustomActionRouteValues property instead."),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public PivotCustomCallbackEventHandler CustomCallback { 
			get { return CustomCallbackInternal; } 
			set { CustomCallbackInternal = value; } 
		}
		public CustomFieldDataEventHandler CustomUnboundFieldData { get; set; }
		public PivotGridCustomSummaryEventHandler CustomSummary { get; set; }
		public PivotGridCustomFieldSortEventHandler CustomFieldSort { get; set; }
		public EventHandler<CustomServerModeSortEventArgs> CustomServerModeSort { get; set; }
		public PivotGridCustomGroupIntervalEventHandler CustomGroupInterval { get; set; }
		public PivotHtmlCellPreparedEventHandler HtmlCellPrepared { get; set; }
		public PivotHtmlFieldValuePreparedEventHandler HtmlFieldValuePrepared { get; set; }
		public PivotFieldDisplayTextEventHandler FieldValueDisplayText { get; set; }
		public PivotCellDisplayTextEventHandler CustomCellDisplayText { get; set; }
		public EventHandler<PivotCellValueEventArgs> CustomCellValue { get; set; }
		public EventHandler<PivotCustomFilterPopupItemsEventArgs> CustomFilterPopupItems { get; set; }
		public EventHandler<PivotCustomFieldValueCellsEventArgs> CustomFieldValueCells { get; set; }
		public EventHandler BeginRefresh { get; set; }
		public EventHandler EndRefresh { get; set; }
		public PivotCustomCellStyleEventHandler CustomCellStyle { get; set; }
		public PivotAddPopupMenuItemEventHandler AddPopupMenuItem { get; set; }
		public PivotPopupMenuCreatedEventHandler PopupMenuCreated { get; set; }
		public EventHandler<PivotDataAreaPopupCreatedEventArgs> DataAreaPopupCreated { get; set; }
		public PivotGridCallbackStateEventHandler CustomSaveCallbackState { get; set; }
		public PivotGridCallbackStateEventHandler CustomLoadCallbackState { get; set; }
		public EventHandler GridLayout { get; set; }
		public EventHandler BeforePerformDataSelect { get; set; }
		public CustomJSPropertiesEventHandler CustomJsProperties { get; set; }
		public CustomFilterExpressionDisplayTextEventHandler CustomFilterExpressionDisplayText { get; set; }
		public FilterControlOperationVisibilityEventHandler FilterControlOperationVisibility { get; set; }
		public FilterControlParseValueEventHandler FilterControlParseValue { get; set; }
		public FilterControlCustomValueDisplayTextEventHandler FilterControlCustomValueDisplayText { get; set; }
		public PivotCustomChartDataSourceDataEventHandler CustomChartDataSourceData { get; set; }
		public PivotCustomChartDataSourceRowsEventHandler CustomChartDataSourceRows { get; set; }
		protected internal PivotCustomCallbackEventHandler CustomCallbackInternal { get; set; }
		protected internal string CellTemplateContent { get; set; }
		protected internal Action<PivotGridCellTemplateContainer> CellTemplateContentMethod { get; set; }
		protected internal string HeaderTemplateContent { get; set; }
		protected internal Action<PivotGridHeaderTemplateContainer> HeaderTemplateContentMethod { get; set; }
		protected internal string EmptyAreaTemplateContent { get; set; }
		protected internal Action<PivotGridEmptyAreaTemplateContainer> EmptyAreaTemplateContentMethod { get; set; }
		protected internal string FieldValueTemplateContent { get; set; }
		protected internal Action<PivotGridFieldValueTemplateContainer> FieldValueTemplateContentMethod { get; set; }
		public void SetCellTemplateContent(Action<PivotGridCellTemplateContainer> contentMethod) {
			CellTemplateContentMethod = contentMethod;
		}
		public void SetCellTemplateContent(string content) {
			CellTemplateContent = content;
		}
		public void SetHeaderTemplateContent(Action<PivotGridHeaderTemplateContainer> contentMethod) {
			HeaderTemplateContentMethod = contentMethod;
		}
		public void SetHeaderTemplateContent(string content) {
			HeaderTemplateContent = content;
		}
		public void SetEmptyAreaTemplateContent(Action<PivotGridEmptyAreaTemplateContainer> contentMethod) {
			EmptyAreaTemplateContentMethod = contentMethod;
		}
		public void SetEmptyAreaTemplateContent(string content) {
			EmptyAreaTemplateContent = content;
		}
		public void SetFieldValueTemplateContent(Action<PivotGridFieldValueTemplateContainer> contentMethod) {
			FieldValueTemplateContentMethod = contentMethod;
		}
		public void SetFieldValueTemplateContent(string content) {
			FieldValueTemplateContent = content;
		}
		protected override ImagesBase CreateImages() {
			return new PivotGridImages(null);
		}
		protected override StylesBase CreateStyles() {
			return new PivotGridStyles(null);
		}
		protected override ClientSideEventsBase CreateClientSideEvents() {
			return new PivotGridClientSideEvents();
		}
	}
}
