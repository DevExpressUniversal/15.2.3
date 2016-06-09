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
using System.IO;
using System.Web.Mvc;
using System.Web.UI;
namespace DevExpress.Web.Mvc {
	using DevExpress.Data;
	using DevExpress.Data.Linq;
	using DevExpress.Web.ASPxPivotGrid;
	using DevExpress.Web.ASPxPivotGrid.Export;
	using DevExpress.Web.Internal;
	using DevExpress.Web.Mvc.Internal;
	using DevExpress.Web.Mvc.UI;
	using DevExpress.XtraPrinting;
	using PivotGridFieldBase = DevExpress.XtraPivotGrid.PivotGridFieldBase;
	public class PivotGridExtension : ExtensionBase {
		public PivotGridExtension(PivotGridSettings settings)
			: base(settings) {
		}
		public PivotGridExtension(PivotGridSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal new MVCxPivotGrid Control {
			get { return (MVCxPivotGrid)base.Control; }
		}
		protected internal new PivotGridSettings Settings {
			get { return (PivotGridSettings)base.Settings; }
		}
		PivotCustomizationExtension customizationExtension;
		PivotCustomizationExtension CustomizationExtension {
			get {
				if(!IsNeedRenderPivotCustomizationExtension)
					return null;
				if(customizationExtension == null) {
					MVCxPivotGrid pivotGrid = Control;
					customizationExtension = new PivotCustomizationExtension(Settings, (pivotCustomization) => {
						((PivotCustomizationExtension)pivotCustomization).PivotGrid = pivotGrid;
					});
				}
				return customizationExtension;
			}
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			AssignFields();
			AssignGroups();
			Control.CallbackRouteValues = Settings.CallbackRouteValues;
			Control.CustomActionRouteValues = Settings.CustomActionRouteValues;
			Control.EnableCallbackAnimation = Settings.EnableCallbackAnimation;
			Control.EnablePagingCallbackAnimation = Settings.EnablePagingCallbackAnimation;
			Control.EnablePagingGestures = Settings.EnablePagingGestures;
			Control.EnableRowsCache = Settings.EnableRowsCache;
			Control.ClientSideEvents.Assign(Settings.ClientSideEvents);
			Control.OptionsView.Assign(Settings.OptionsView);
			Control.OptionsBehavior.Assign(Settings.OptionsBehavior);
			Control.OptionsCustomization.Assign(Settings.OptionsCustomization);
			Control.OptionsFilter.Assign(Settings.OptionsFilter);
			Control.OptionsLoadingPanel.Assign(Settings.OptionsLoadingPanel);
			Control.OptionsOLAP.Assign(Settings.OptionsOLAP);
			Control.OptionsPager.Assign(Settings.OptionsPager);
			Control.OptionsData.Assign(Settings.OptionsData);
			Control.OptionsDataField.Assign(Settings.OptionsDataField);
			Control.OptionsChartDataSource.Assign(Settings.OptionsChartDataSource);
			Control.Styles.CopyFrom(Settings.Styles);
			Control.StylesPager.CopyFrom(Settings.StylesPager);
			Control.StylesEditors.CopyFrom(Settings.StylesEditors);
			Control.StylesPrint.CopyFrom(Settings.SettingsExport.StylesPrint);
			Control.Prefilter.Assign(Settings.Prefilter);
			Control.CustomizationFieldsVisible = Settings.CustomizationFieldsVisible;
			Control.CustomizationFieldsLeft = Settings.CustomizationFieldsLeft;
			Control.CustomizationFieldsTop = Settings.CustomizationFieldsTop;
			Control.SummaryText = Settings.SummaryText;
			Control.OLAPDataProvider = Settings.OLAPDataProvider;
			Control.PivotCustomizationExtensionName = Settings.PivotCustomizationExtensionSettings.Name;
			Control.InitialPageSize = Settings.OptionsPager.RowsPerPage;
			Control.AfterPerformCallback += Settings.AfterPerformCallback;
			Control.BeforeGetCallbackResult += Settings.BeforeGetCallbackResult;
			Control.CustomCallback += Settings.CustomCallbackInternal;
			Control.CustomUnboundFieldData += Settings.CustomUnboundFieldData;
			Control.CustomSummary += Settings.CustomSummary;
			Control.CustomFieldSort += Settings.CustomFieldSort;
			Control.CustomServerModeSort += Settings.CustomServerModeSort;
			Control.CustomGroupInterval += Settings.CustomGroupInterval;
			Control.HtmlCellPrepared += Settings.HtmlCellPrepared;
			Control.HtmlFieldValuePrepared += Settings.HtmlFieldValuePrepared;
			Control.FieldValueDisplayText += Settings.FieldValueDisplayText;
			Control.CustomCellDisplayText += Settings.CustomCellDisplayText;
			Control.CustomCellValue += Settings.CustomCellValue;
			Control.CustomFilterPopupItems += Settings.CustomFilterPopupItems;
			Control.CustomFieldValueCells += Settings.CustomFieldValueCells;
			Control.BeginRefresh += Settings.BeginRefresh;
			Control.EndRefresh += Settings.EndRefresh;
			Control.CustomCellStyle += Settings.CustomCellStyle;
			Control.AddPopupMenuItem += Settings.AddPopupMenuItem;
			Control.PopupMenuCreated += Settings.PopupMenuCreated;
			Control.DataAreaPopupCreated += Settings.DataAreaPopupCreated;
			Control.CustomSaveCallbackState += Settings.CustomSaveCallbackState;
			Control.CustomLoadCallbackState += Settings.CustomLoadCallbackState;
			Control.GridLayout += Settings.GridLayout;
			Control.BeforePerformDataSelect += Settings.BeforePerformDataSelect;
			Control.CustomJsProperties += Settings.CustomJsProperties;
			Control.CustomFilterExpressionDisplayText += Settings.CustomFilterExpressionDisplayText;
			Control.FilterControlOperationVisibility += Settings.FilterControlOperationVisibility;
			Control.FilterControlParseValue += Settings.FilterControlParseValue;
			Control.FilterControlCustomValueDisplayText += Settings.FilterControlCustomValueDisplayText;
			Control.CustomChartDataSourceData += Settings.CustomChartDataSourceData;
			Control.CustomChartDataSourceRows += Settings.CustomChartDataSourceRows;
		}
		void AssignFields() {
			Dictionary<PivotGridFieldBase, PivotGridFieldBase> fieldsMap = new Dictionary<PivotGridFieldBase, PivotGridFieldBase>();
			Control.Fields.Capacity = Settings.Fields.Capacity;
			for(int index = 0; index < Settings.Fields.Count; index++) {
				PivotGridFieldBase field = (PivotGridFieldBase)Activator.CreateInstance(Settings.Fields[index].GetType());
				Control.Fields.Add(field);
				field.Assign(Settings.Fields[index]);
				fieldsMap.Add(Settings.Fields[index], field);
			}
			foreach(KeyValuePair<PivotGridFieldBase, PivotGridFieldBase> keyValuePair in fieldsMap) {
				DevExpress.XtraPivotGrid.PivotGridFieldSortBySummaryInfo info = keyValuePair.Value.SortBySummaryInfo;
				if(info.Field != null && fieldsMap.ContainsKey(info.Field))
					info.Field = fieldsMap[info.Field];
				for(int i = 0; i < info.Conditions.Count; i++)
					if(info.Conditions[i].Field != null && fieldsMap.ContainsKey(info.Conditions[i].Field))
					info.Conditions[i].Field = fieldsMap[info.Conditions[i].Field];
			}
		}
		void AssignGroups() {
			Control.Groups.Capacity = Settings.Groups.Capacity;
			foreach(PivotGridWebGroup group in Settings.Groups) {
				Control.Groups.Add(group);
			}
			Control.Data.RestoreFieldsInGroups();
		}
		protected override void AssignRenderProperties() {
			base.AssignRenderProperties();
			Control.CellTemplate = ContentControlTemplate<PivotGridCellTemplateContainer>.Create(
				Settings.CellTemplateContent, Settings.CellTemplateContentMethod,
				typeof(PivotGridCellTemplateContainer), true);
			Control.HeaderTemplate = ContentControlTemplate<PivotGridHeaderTemplateContainer>.Create(
				Settings.HeaderTemplateContent, Settings.HeaderTemplateContentMethod,
				typeof(PivotGridHeaderTemplateContainer), true);
			Control.EmptyAreaTemplate = ContentControlTemplate<PivotGridEmptyAreaTemplateContainer>.Create(
				Settings.EmptyAreaTemplateContent, Settings.EmptyAreaTemplateContentMethod,
				typeof(PivotGridEmptyAreaTemplateContainer), true);
			Control.FieldValueTemplate = ContentControlTemplate<PivotGridFieldValueTemplateContainer>.Create(
				Settings.FieldValueTemplateContent, Settings.FieldValueTemplateContentMethod,
				typeof(PivotGridFieldValueTemplateContainer), true);
			for(int i = 0; i < Control.Fields.Count; i++) {
				if (Settings.Fields[i] is MVCxPivotGridField) {
					var field = (MVCxPivotGridField)Settings.Fields[i];
					Control.Fields[i].HeaderTemplate = ContentControlTemplate<PivotGridHeaderTemplateContainer>.Create(
						field.HeaderTemplateContent, field.HeaderTemplateContentMethod,
						typeof(PivotGridHeaderTemplateContainer), true);
					Control.Fields[i].ValueTemplate = ContentControlTemplate<PivotGridFieldValueTemplateContainer>.Create(
						field.ValueTemplateContent, field.ValueTemplateContentMethod,
						typeof(PivotGridFieldValueTemplateContainer), true);
				}
			}
		}
		public PivotGridExtension Bind(object dataObject) {
			Control.DataSource = dataObject;
			return this;
		}
		public PivotGridExtension BindToOLAP(string connectionString) {
			Control.OLAPConnectionString = connectionString;
			return this;
		}
		public PivotGridExtension BindToLINQ(Type contextType, string tableName) {
			return BindToLINQ(contextType.FullName, tableName);
		}
		public PivotGridExtension BindToLINQ(string contextTypeName, string tableName) {
			return BindToLINQ(contextTypeName, tableName, null);
		}
		public PivotGridExtension BindToLINQ(Type contextType, string tableName, EventHandler<LinqServerModeDataSourceSelectEventArgs> selectingMethod) {
			return BindToLINQ(contextType.FullName, tableName, selectingMethod);
		}
		public PivotGridExtension BindToLINQ(string contextTypeName, string tableName, EventHandler<LinqServerModeDataSourceSelectEventArgs> selectingMethod) {
			return BindToLINQ(contextTypeName, tableName, selectingMethod, null);
		}
		public PivotGridExtension BindToLINQ(string contextTypeName, string tableName, EventHandler<LinqServerModeDataSourceSelectEventArgs> selectingMethod,
			EventHandler<DevExpress.Data.ServerModeExceptionThrownEventArgs> exceptionThrownMethod) {
			BindToLINQDataSourceInternal(contextTypeName, tableName, selectingMethod, exceptionThrownMethod);
			return this;
		}
		public PivotGridExtension BindToEF(Type contextType, string tableName) {
			return BindToEF(contextType.FullName, tableName);
		}
		public PivotGridExtension BindToEF(string contextTypeName, string tableName) {
			return BindToEF(contextTypeName, tableName, null);
		}
		public PivotGridExtension BindToEF(Type contextType, string tableName, EventHandler<LinqServerModeDataSourceSelectEventArgs> selectingMethod) {
			return BindToEF(contextType.FullName, tableName, selectingMethod);
		}
		public PivotGridExtension BindToEF(string contextTypeName, string tableName, EventHandler<LinqServerModeDataSourceSelectEventArgs> selectingMethod) {
			return BindToEF(contextTypeName, tableName, selectingMethod, null);
		}
		public PivotGridExtension BindToEF(string contextTypeName, string tableName, EventHandler<LinqServerModeDataSourceSelectEventArgs> selectingMethod,
			EventHandler<DevExpress.Data.ServerModeExceptionThrownEventArgs> exceptionThrownMethod) {
			BindToEFDataSourceInternal(contextTypeName, tableName, selectingMethod, exceptionThrownMethod);
			return this;
		}
		protected internal override bool IsBindInternalRequired() {
			return false;
		}
		public static object GetDataObject(PivotGridSettings pivotGridSettings, object dataSource) {
			PivotGridExtension pivotGridExtension = new PivotGridExtension(pivotGridSettings);
			pivotGridExtension.Control.DataSource = dataSource;
			return GetDataObjectInternal(pivotGridExtension, null);
		}
		public static object GetDataObject(PivotGridSettings pivotGridSettings, object dataSource, bool applyClientState) {
			PivotGridExtension pivotGridExtension = new PivotGridExtension(pivotGridSettings);
			pivotGridExtension.Control.DataSource = dataSource;
			return GetDataObjectInternal(pivotGridExtension, applyClientState);
		}
		public static object GetDataObject(PivotGridSettings pivotGridSettings, string olapConnectionString) {
			PivotGridExtension pivotGridExtension = new PivotGridExtension(pivotGridSettings);
			pivotGridExtension.Control.OLAPConnectionString = olapConnectionString;
			return GetDataObjectInternal(pivotGridExtension, null);
		}
		public static object GetDataObject(PivotGridSettings pivotGridSettings, string olapConnectionString, bool applyClientState) {
			PivotGridExtension pivotGridExtension = new PivotGridExtension(pivotGridSettings);
			pivotGridExtension.Control.OLAPConnectionString = olapConnectionString;
			return GetDataObjectInternal(pivotGridExtension, applyClientState);
		}
		public static object GetEFDataObject(PivotGridSettings pivotGridSettings, EventHandler<LinqServerModeDataSourceSelectEventArgs> selectingMethod) {
			return GetEFDataObject(pivotGridSettings, selectingMethod, null);
		}
		public static object GetEFDataObject(PivotGridSettings pivotGridSettings, EventHandler<LinqServerModeDataSourceSelectEventArgs> selectingMethod, EventHandler<ServerModeExceptionThrownEventArgs> exceptionThrownMethod) {
			PivotGridExtension pivotGridExtension = new PivotGridExtension(pivotGridSettings);
			pivotGridExtension.BindToEF(string.Empty, string.Empty, selectingMethod, exceptionThrownMethod);
			return GetDataObjectInternal(pivotGridExtension, null);
		}
		public static object GetEFDataObject(PivotGridSettings pivotGridSettings, EventHandler<LinqServerModeDataSourceSelectEventArgs> selectingMethod, EventHandler<ServerModeExceptionThrownEventArgs> exceptionThrownMethod, bool applyClientState) {
			PivotGridExtension pivotGridExtension = new PivotGridExtension(pivotGridSettings);
			pivotGridExtension.BindToEF(string.Empty, string.Empty, selectingMethod, exceptionThrownMethod);
			return GetDataObjectInternal(pivotGridExtension, applyClientState);
		}
		public static object GetLINQDataObject(PivotGridSettings pivotGridSettings, EventHandler<LinqServerModeDataSourceSelectEventArgs> selectingMethod) {
			return GetLINQDataObject(pivotGridSettings, selectingMethod, null);
		}
		public static object GetLINQDataObject(PivotGridSettings pivotGridSettings, EventHandler<LinqServerModeDataSourceSelectEventArgs> selectingMethod, EventHandler<ServerModeExceptionThrownEventArgs> exceptionThrownMethod) {
			PivotGridExtension pivotGridExtension = new PivotGridExtension(pivotGridSettings);
			pivotGridExtension.BindToLINQ(string.Empty, string.Empty, selectingMethod, exceptionThrownMethod);
			return GetDataObjectInternal(pivotGridExtension, null);
		}
		public static object GetLINQDataObject(PivotGridSettings pivotGridSettings, EventHandler<LinqServerModeDataSourceSelectEventArgs> selectingMethod, EventHandler<ServerModeExceptionThrownEventArgs> exceptionThrownMethod, bool applyClientState) {
			PivotGridExtension pivotGridExtension = new PivotGridExtension(pivotGridSettings);
			pivotGridExtension.BindToLINQ(string.Empty, string.Empty, selectingMethod, exceptionThrownMethod);
			return GetDataObjectInternal(pivotGridExtension, applyClientState);
		}
		static object GetDataObjectInternal(PivotGridExtension pivotGridExtension, bool? applyClientState) {
			try {
				pivotGridExtension.Control.BeginUpdate();
				pivotGridExtension.PrepareControl();
			} finally{
				pivotGridExtension.Control.EndUpdate();
			}
			bool requireLoadPostData = applyClientState.HasValue ? applyClientState.Value : !string.IsNullOrEmpty(MvcUtils.CallbackName);
			if(!requireLoadPostData)
				pivotGridExtension.PreRender();
			else
				pivotGridExtension.LoadPostData();
			pivotGridExtension.Control.EnsureChildControls();
			return pivotGridExtension.Control.ChartDataSource;
		}
		public static XtraPivotGrid.PivotDrillDownDataSource CreateDrillDownDataSource(PivotGridSettings pivotGridSettings, object dataSource) {
			return CreateExtension(pivotGridSettings, dataSource).Control.CreateDrillDownDataSource();
		}
		public static XtraPivotGrid.PivotDrillDownDataSource CreateDrillDownDataSource(PivotGridSettings pivotGridSettings, object dataSource, int columnIndex, int rowIndex) {
			return CreateExtension(pivotGridSettings, dataSource).Control.CreateDrillDownDataSource(columnIndex, rowIndex);
		}
		public static XtraPivotGrid.PivotDrillDownDataSource CreateDrillDownDataSource(PivotGridSettings pivotGridSettings, object dataSource, int columnIndex, int rowIndex, int dataIndex) {
			return CreateExtension(pivotGridSettings, dataSource).Control.CreateDrillDownDataSource(columnIndex, rowIndex, dataIndex);
		}
		public static XtraPivotGrid.PivotDrillDownDataSource CreateOLAPDrillDownDataSource(PivotGridSettings pivotGridSettings, string connectionString, int columnIndex, int rowIndex, List<string> customColumns) {
			return CreateOLAPExtension(pivotGridSettings, connectionString).Control.CreateDrillDownDataSource(columnIndex, rowIndex, customColumns);
		}
		public static XtraPivotGrid.PivotDrillDownDataSource CreateOLAPDrillDownDataSource(PivotGridSettings pivotGridSettings, string connectionString, int columnIndex, int rowIndex, int maxRowCount, List<string> customColumns) {
			return CreateOLAPExtension(pivotGridSettings, connectionString).Control.CreateDrillDownDataSource(columnIndex, rowIndex, maxRowCount, customColumns);
		}
		public static XtraPivotGrid.PivotSummaryDataSource CreateSummaryDataSource(PivotGridSettings pivotGridSettings, object dataSource) {
			return CreateExtension(pivotGridSettings, dataSource).Control.CreateSummaryDataSource();
		}
		public static XtraPivotGrid.PivotSummaryDataSource CreateSummaryDataSource(PivotGridSettings pivotGridSettings, object dataSource, int columnIndex, int rowIndex) {
			return CreateExtension(pivotGridSettings, dataSource).Control.CreateSummaryDataSource(columnIndex, rowIndex);
		}
		public static XtraPivotGrid.PivotSummaryDataSource CreateOLAPSummaryDataSource(PivotGridSettings pivotGridSettings, string connectionString) {
			return CreateOLAPExtension(pivotGridSettings, connectionString).Control.CreateSummaryDataSource();
		}
		public static XtraPivotGrid.PivotSummaryDataSource CreateOLAPSummaryDataSource(PivotGridSettings pivotGridSettings, string connectionString, int columnIndex, int rowIndex) {
			return CreateOLAPExtension(pivotGridSettings, connectionString).Control.CreateSummaryDataSource(columnIndex, rowIndex);
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			RegisterCustomizationControl();
			Control.ResetRequireDataUpdate();
		}
		void RegisterCustomizationControl() {
			if (CustomizationExtension == null)
				return;
			MVCxPivotCustomizationControl customizationControl = CustomizationExtension.Control;
			customizationControl.RegisterInPivotGrid();
			customizationControl.CustomizationFields.ReadPostData();
		}
		protected override void LoadPostDataInternal() {
			base.LoadPostDataInternal();
			if (IsFilterControlCallback())
				Control.EnsureChildControls();
		}
		bool IsNeedRenderPivotCustomizationExtension {
			get { 
				var renderedExtensions = ExtensionsFactory.RenderedExtensions;
				return !string.IsNullOrEmpty(Settings.PivotCustomizationExtensionSettings.Name) && renderedExtensions.ContainsKey(Settings.Name)
					&& !Control.Data.IsLockUpdate;
			}
		}
		protected internal override ICallbackEventHandler CallbackEventHandler {
			get { return IsFilterControlCallback() && Control.FilterControl != null ? Control.FilterControl : base.CallbackEventHandler; }
		}
		protected internal override bool IsCallback() {
			return base.IsCallback() || IsFilterControlCallback();
		}
		#region Export
		public static IPrintable CreatePrintableObject(PivotGridSettings settings, object dataObject) {
			return (IPrintable)CreateExporter(settings, dataObject);
		}
		public static IPrintable CreateOLAPPrintableObject(PivotGridSettings settings, string connectionString) {
			return (IPrintable)CreateOLAPExporter(settings, connectionString);
		}
		static ActionResult Export(PivotGridSettings settings, object dataObject, Action<MVCxPivotGridExporter, Stream> write, string fileExtension) {
			return Export(settings, dataObject, write, fileExtension, null);
		}
		static ActionResult Export(PivotGridSettings settings, object dataObject, Action<MVCxPivotGridExporter, Stream> write, string fileExtension, string fileName) {
			return Export(settings, dataObject, write, fileExtension, fileName, true);
		}
		static ActionResult Export(PivotGridSettings settings, object dataObject, Action<MVCxPivotGridExporter, Stream> write, string fileExtension, bool saveAsFile) {
			return Export(settings, dataObject, write, fileExtension, null, saveAsFile);
		}
		static ActionResult Export(PivotGridSettings settings, object dataObject, Action<MVCxPivotGridExporter, Stream> write, string fileExtension, string fileName, bool saveAsFile) {
			MVCxPivotGridExporter exporter = CreateExporter(settings, dataObject);
			return ExportCore(settings, exporter, write, fileName, saveAsFile, fileExtension);
		}
		static ActionResult OLAPExport(PivotGridSettings settings, string connectionString, Action<MVCxPivotGridExporter, Stream> write, string fileExtension) {
			return OLAPExport(settings, connectionString, write, fileExtension, null);
		}
		static ActionResult OLAPExport(PivotGridSettings settings, string connectionString, Action<MVCxPivotGridExporter, Stream> write, string fileExtension, string fileName) {
			return OLAPExport(settings, connectionString, write, fileExtension, fileName, true);
		}
		static ActionResult OLAPExport(PivotGridSettings settings, string connectionString, Action<MVCxPivotGridExporter, Stream> write, string fileExtension, bool saveAsFile) {
			return OLAPExport(settings, connectionString, write, fileExtension, null, saveAsFile);
		}
		static ActionResult OLAPExport(PivotGridSettings settings, string connectionString, Action<MVCxPivotGridExporter, Stream> write, string fileExtension, string fileName, bool saveAsFile) {
			MVCxPivotGridExporter exporter = CreateExporter(settings, connectionString);
			ActionResult fileStreamResult = ExportCore(settings, exporter, write, fileName, saveAsFile, fileExtension);
			exporter.Dispose();
			return fileStreamResult;
		}
		static ActionResult ExportCore(PivotGridSettings settings, MVCxPivotGridExporter exporter, Action<MVCxPivotGridExporter, Stream> write, string fileName, bool saveAsFile, string fileFormat) {
			RaiseBeforeExportEvent(settings, exporter);
			return ExportUtils.Export(exporter.Extension, s => write(exporter, s), fileName, saveAsFile, fileFormat);
		}
		static void RaiseBeforeExportEvent(PivotGridSettings settings, MVCxPivotGridExporter exporter) {
			if(settings.SettingsExport.BeforeExport == null)
				return;
			settings.SettingsExport.BeforeExport(exporter.PivotGrid, EventArgs.Empty);
		}
		#endregion
		#region Pdf Export
		static void ExportToPdf(MVCxPivotGridExporter exporter, Stream stream) {
			exporter.ExportToPdf(stream);
		}
		static Action<MVCxPivotGridExporter, Stream> ExportToPdf(DevExpress.XtraPrinting.PdfExportOptions options) {
			return (e, s) => e.ExportToPdf(s, options);
		}
		#region DataObject
		public static void ExportToPdf(PivotGridSettings settings, object dataObject, Stream stream) {
			CreateExporter(settings, dataObject).ExportToPdf(stream);
		}
		public static ActionResult ExportToPdf(PivotGridSettings settings, object dataObject) {
			return Export(settings, dataObject, ExportToPdf, "pdf");
		}
		public static ActionResult ExportToPdf(PivotGridSettings settings, object dataObject, string fileName) {
			return Export(settings, dataObject, ExportToPdf, "pdf", fileName);
		}
		public static ActionResult ExportToPdf(PivotGridSettings settings, object dataObject, bool saveAsFile) {
			return Export(settings, dataObject, ExportToPdf, "pdf", saveAsFile);
		}
		public static ActionResult ExportToPdf(PivotGridSettings settings, object dataObject, string fileName, bool saveAsFile) {
			return Export(settings, dataObject, ExportToPdf, "pdf", fileName, saveAsFile);
		}
		public static void ExportToPdf(PivotGridSettings settings, object dataObject, Stream stream, DevExpress.XtraPrinting.PdfExportOptions exportOptions) {
			CreateExporter(settings, dataObject).ExportToPdf(stream, exportOptions);
		}
		public static ActionResult ExportToPdf(PivotGridSettings settings, object dataObject, DevExpress.XtraPrinting.PdfExportOptions exportOptions) {
			return Export(settings, dataObject, ExportToPdf(exportOptions), "pdf");
		}
		public static ActionResult ExportToPdf(PivotGridSettings settings, object dataObject, string fileName, DevExpress.XtraPrinting.PdfExportOptions exportOptions) {
			return Export(settings, dataObject, ExportToPdf(exportOptions), "pdf", fileName);
		}
		public static ActionResult ExportToPdf(PivotGridSettings settings, object dataObject, bool saveAsFile, DevExpress.XtraPrinting.PdfExportOptions exportOptions) {
			return Export(settings, dataObject, ExportToPdf(exportOptions), "pdf", saveAsFile);
		}
		public static ActionResult ExportToPdf(PivotGridSettings settings, object dataObject, string fileName, bool saveAsFile, DevExpress.XtraPrinting.PdfExportOptions exportOptions) {
			return Export(settings, dataObject, ExportToPdf(exportOptions), "pdf", fileName, saveAsFile);
		}
		#endregion
		#region OLAP
		public static void OLAPExportToPdf(PivotGridSettings settings, string connectionString, Stream stream) {
			CreateOLAPExporter(settings, connectionString).ExportToPdf(stream);
		}
		public static ActionResult OLAPExportToPdf(PivotGridSettings settings, string connectionString) {
			return OLAPExport(settings, connectionString, ExportToPdf, "pdf");
		}
		public static ActionResult OLAPExportToPdf(PivotGridSettings settings, string connectionString, string fileName) {
			return OLAPExport(settings, connectionString, ExportToPdf, "pdf", fileName);
		}
		public static ActionResult OLAPExportToPdf(PivotGridSettings settings, string connectionString, bool saveAsFile) {
			return OLAPExport(settings, connectionString, ExportToPdf, "pdf", saveAsFile);
		}
		public static ActionResult OLAPExportToPdf(PivotGridSettings settings, string connectionString, string fileName, bool saveAsFile) {
			return OLAPExport(settings, connectionString, ExportToPdf, "pdf", fileName, saveAsFile);
		}
		public static void OLAPExportToPdf(PivotGridSettings settings, string connectionString, Stream stream, DevExpress.XtraPrinting.PdfExportOptions exportOptions) {
			CreateOLAPExporter(settings, connectionString).ExportToPdf(stream, exportOptions);
		}
		public static ActionResult OLAPExportToPdf(PivotGridSettings settings, string connectionString, DevExpress.XtraPrinting.PdfExportOptions exportOptions) {
			return OLAPExport(settings, connectionString, ExportToPdf(exportOptions), "pdf");
		}
		public static ActionResult OLAPExportToPdf(PivotGridSettings settings, string connectionString, string fileName, DevExpress.XtraPrinting.PdfExportOptions exportOptions) {
			return OLAPExport(settings, connectionString, ExportToPdf(exportOptions), "pdf", fileName);
		}
		public static ActionResult OLAPExportToPdf(PivotGridSettings settings, string connectionString, bool saveAsFile, DevExpress.XtraPrinting.PdfExportOptions exportOptions) {
			return OLAPExport(settings, connectionString, ExportToPdf(exportOptions), "pdf", saveAsFile);
		}
		public static ActionResult OLAPExportToPdf(PivotGridSettings settings, string connectionString, string fileName, bool saveAsFile, DevExpress.XtraPrinting.PdfExportOptions exportOptions) {
			return OLAPExport(settings, connectionString, ExportToPdf(exportOptions), "pdf", fileName, saveAsFile);
		}
		#endregion
		#endregion
		#region Xls Export
		static void ExportToXls(MVCxPivotGridExporter exporter, Stream stream) {
			exporter.ExportToXls(stream);
		}
		static Action<MVCxPivotGridExporter, Stream> ExportToXls(DevExpress.XtraPrinting.XlsExportOptions options) {
			return (e, s) => e.ExportToXls(s, options);
		}
		#region DataObject
		public static void ExportToXls(PivotGridSettings settings, object dataObject, Stream stream) {
			CreateExporter(settings, dataObject).ExportToXls(stream);
		}
		public static ActionResult ExportToXls(PivotGridSettings settings, object dataObject) {
			return Export(settings, dataObject, ExportToXls, "xls");
		}
		public static ActionResult ExportToXls(PivotGridSettings settings, object dataObject, string fileName) {
			return Export(settings, dataObject, ExportToXls, "xls", fileName);
		}
		public static ActionResult ExportToXls(PivotGridSettings settings, object dataObject, bool saveAsFile) {
			return Export(settings, dataObject, ExportToXls, "xls", saveAsFile);
		}
		public static ActionResult ExportToXls(PivotGridSettings settings, object dataObject, string fileName, bool saveAsFile) {
			return Export(settings, dataObject, ExportToXls, "xls", fileName, saveAsFile);
		}
		public static void ExportToXls(PivotGridSettings settings, object dataObject, Stream stream, DevExpress.XtraPrinting.XlsExportOptions exportOptions) {
			CreateExporter(settings, dataObject).ExportToXls(stream, exportOptions);
		}
		public static ActionResult ExportToXls(PivotGridSettings settings, object dataObject, DevExpress.XtraPrinting.XlsExportOptions exportOptions) {
			return Export(settings, dataObject, ExportToXls(exportOptions), "xls");
		}
		public static ActionResult ExportToXls(PivotGridSettings settings, object dataObject, string fileName, DevExpress.XtraPrinting.XlsExportOptions exportOptions) {
			return Export(settings, dataObject, ExportToXls(exportOptions), "xls", fileName);
		}
		public static ActionResult ExportToXls(PivotGridSettings settings, object dataObject, bool saveAsFile, DevExpress.XtraPrinting.XlsExportOptions exportOptions) {
			return Export(settings, dataObject, ExportToXls(exportOptions), "xls", saveAsFile);
		}
		public static ActionResult ExportToXls(PivotGridSettings settings, object dataObject, string fileName, bool saveAsFile, DevExpress.XtraPrinting.XlsExportOptions exportOptions) {
			return Export(settings, dataObject, ExportToXls(exportOptions), "xls", fileName, saveAsFile);
		}
		#endregion DataObject
		#region OLAP
		public static void OLAPExportToXls(PivotGridSettings settings, string connectionString, Stream stream) {
			CreateOLAPExporter(settings, connectionString).ExportToXls(stream);
		}
		public static ActionResult OLAPExportToXls(PivotGridSettings settings, string connectionString) {
			return OLAPExport(settings, connectionString, ExportToXls, "xls");
		}
		public static ActionResult OLAPExportToXls(PivotGridSettings settings, string connectionString, string fileName) {
			return OLAPExport(settings, connectionString, ExportToXls, "xls", fileName);
		}
		public static ActionResult OLAPExportToXls(PivotGridSettings settings, string connectionString, bool saveAsFile) {
			return OLAPExport(settings, connectionString, ExportToXls, "xls", saveAsFile);
		}
		public static ActionResult OLAPExportToXls(PivotGridSettings settings, string connectionString, string fileName, bool saveAsFile) {
			return OLAPExport(settings, connectionString, ExportToXls, "xls", fileName, saveAsFile);
		}
		public static void OLAPExportToXls(PivotGridSettings settings, string connectionString, Stream stream, DevExpress.XtraPrinting.XlsExportOptions exportOptions) {
			CreateOLAPExporter(settings, connectionString).ExportToXls(stream, exportOptions);
		}
		public static ActionResult OLAPExportToXls(PivotGridSettings settings, string connectionString, DevExpress.XtraPrinting.XlsExportOptions exportOptions) {
			return OLAPExport(settings, connectionString, ExportToXls(exportOptions), "xls");
		}
		public static ActionResult OLAPExportToXls(PivotGridSettings settings, string connectionString, string fileName, DevExpress.XtraPrinting.XlsExportOptions exportOptions) {
			return OLAPExport(settings, connectionString, ExportToXls(exportOptions), "xls", fileName);
		}
		public static ActionResult OLAPExportToXls(PivotGridSettings settings, string connectionString, bool saveAsFile, DevExpress.XtraPrinting.XlsExportOptions exportOptions) {
			return OLAPExport(settings, connectionString, ExportToXls(exportOptions), "xls", saveAsFile);
		}
		public static ActionResult OLAPExportToXls(PivotGridSettings settings, string connectionString, string fileName, bool saveAsFile, DevExpress.XtraPrinting.XlsExportOptions exportOptions) {
			return OLAPExport(settings, connectionString, ExportToXls(exportOptions), "xls", fileName, saveAsFile);
		}
		#endregion
		#endregion
		#region Xlsx Export
		static void ExportToXlsx(MVCxPivotGridExporter exporter, Stream stream) {
			exporter.ExportToXlsx(stream);
		}
		static Action<MVCxPivotGridExporter, Stream> ExportToXlsx(DevExpress.XtraPrinting.XlsxExportOptions options) {
			return (e, s) => e.ExportToXlsx(s, options);
		}
		#region DataObject
		public static void ExportToXlsx(PivotGridSettings settings, object dataObject, Stream stream) {
			CreateExporter(settings, dataObject).ExportToXlsx(stream);
		}
		public static ActionResult ExportToXlsx(PivotGridSettings settings, object dataObject) {
			return Export(settings, dataObject, ExportToXlsx, "xlsx");
		}
		public static ActionResult ExportToXlsx(PivotGridSettings settings, object dataObject, string fileName) {
			return Export(settings, dataObject, ExportToXlsx, "xlsx", fileName);
		}
		public static ActionResult ExportToXlsx(PivotGridSettings settings, object dataObject, bool saveAsFile) {
			return Export(settings, dataObject, ExportToXlsx, "xlsx", saveAsFile);
		}
		public static ActionResult ExportToXlsx(PivotGridSettings settings, object dataObject, string fileName, bool saveAsFile) {
			return Export(settings, dataObject, ExportToXlsx, "xlsx", fileName, saveAsFile);
		}
		public static void ExportToXlsx(PivotGridSettings settings, object dataObject, Stream stream, DevExpress.XtraPrinting.XlsxExportOptions exportOptions) {
			CreateExporter(settings, dataObject).ExportToXlsx(stream, exportOptions);
		}
		public static ActionResult ExportToXlsx(PivotGridSettings settings, object dataObject, DevExpress.XtraPrinting.XlsxExportOptions exportOptions) {
			return Export(settings, dataObject, ExportToXlsx(exportOptions), "xlsx");
		}
		public static ActionResult ExportToXlsx(PivotGridSettings settings, object dataObject, string fileName, DevExpress.XtraPrinting.XlsxExportOptions exportOptions) {
			return Export(settings, dataObject, ExportToXlsx(exportOptions), "xlsx", fileName);
		}
		public static ActionResult ExportToXlsx(PivotGridSettings settings, object dataObject, bool saveAsFile, DevExpress.XtraPrinting.XlsxExportOptions exportOptions) {
			return Export(settings, dataObject, ExportToXlsx(exportOptions), "xlsx", saveAsFile);
		}
		public static ActionResult ExportToXlsx(PivotGridSettings settings, object dataObject, string fileName, bool saveAsFile, DevExpress.XtraPrinting.XlsxExportOptions exportOptions) {
			return Export(settings, dataObject, ExportToXlsx(exportOptions), "xlsx", fileName, saveAsFile);
		}
		#endregion
		#region OLAP
		public static void OLAPExportToXlsx(PivotGridSettings settings, string connectionString, Stream stream) {
			CreateOLAPExporter(settings, connectionString).ExportToXlsx(stream);
		}
		public static ActionResult OLAPExportToXlsx(PivotGridSettings settings, string connectionString) {
			return OLAPExport(settings, connectionString, ExportToXlsx, "xlsx");
		}
		public static ActionResult OLAPExportToXlsx(PivotGridSettings settings, string connectionString, string fileName) {
			return OLAPExport(settings, connectionString, ExportToXlsx, "xlsx", fileName);
		}
		public static ActionResult OLAPExportToXlsx(PivotGridSettings settings, string connectionString, bool saveAsFile) {
			return OLAPExport(settings, connectionString, ExportToXlsx, "xlsx", saveAsFile);
		}
		public static ActionResult OLAPExportToXlsx(PivotGridSettings settings, string connectionString, string fileName, bool saveAsFile) {
			return OLAPExport(settings, connectionString, ExportToXlsx, "xlsx", fileName, saveAsFile);
		}
		public static void OLAPExportToXlsx(PivotGridSettings settings, string connectionString, Stream stream, DevExpress.XtraPrinting.XlsxExportOptions exportOptions) {
			CreateOLAPExporter(settings, connectionString).ExportToXlsx(stream, exportOptions);
		}
		public static ActionResult OLAPExportToXlsx(PivotGridSettings settings, string connectionString, DevExpress.XtraPrinting.XlsxExportOptions exportOptions) {
			return OLAPExport(settings, connectionString, ExportToXlsx(exportOptions), "xlsx");
		}
		public static ActionResult OLAPExportToXlsx(PivotGridSettings settings, string connectionString, string fileName, DevExpress.XtraPrinting.XlsxExportOptions exportOptions) {
			return OLAPExport(settings, connectionString, ExportToXlsx(exportOptions), "xlsx", fileName);
		}
		public static ActionResult OLAPExportToXlsx(PivotGridSettings settings, string connectionString, bool saveAsFile, DevExpress.XtraPrinting.XlsxExportOptions exportOptions) {
			return OLAPExport(settings, connectionString, ExportToXlsx(exportOptions), "xlsx", saveAsFile);
		}
		public static ActionResult OLAPExportToXlsx(PivotGridSettings settings, string connectionString, string fileName, bool saveAsFile, DevExpress.XtraPrinting.XlsxExportOptions exportOptions) {
			return OLAPExport(settings, connectionString, ExportToXlsx(exportOptions), "xlsx", fileName, saveAsFile);
		}
		#endregion
		#endregion
		#region Mht Export
		static void ExportToMht(MVCxPivotGridExporter exporter, Stream stream) {
			exporter.ExportToMht(stream);
		}
		static Action<MVCxPivotGridExporter, Stream> ExportToMht(DevExpress.XtraPrinting.MhtExportOptions options) {
			return (e, s) => e.ExportToMht(s, options);
		}
		#region DataObject
		public static void ExportToMht(PivotGridSettings settings, object dataObject, Stream stream) {
			CreateExporter(settings, dataObject).ExportToMht(stream, null);
		}
		public static ActionResult ExportToMht(PivotGridSettings settings, object dataObject) {
			return Export(settings, dataObject, ExportToMht, "mht");
		}
		public static ActionResult ExportToMht(PivotGridSettings settings, object dataObject, string fileName) {
			return Export(settings, dataObject, ExportToMht, "mht", fileName);
		}
		public static ActionResult ExportToMht(PivotGridSettings settings, object dataObject, bool saveAsFile) {
			return Export(settings, dataObject, ExportToMht, "mht", saveAsFile);
		}
		public static ActionResult ExportToMht(PivotGridSettings settings, object dataObject, string fileName, bool saveAsFile) {
			return Export(settings, dataObject, ExportToMht, "mht", fileName, saveAsFile);
		}
		public static void ExportToMht(PivotGridSettings settings, object dataObject, Stream stream, DevExpress.XtraPrinting.MhtExportOptions exportOptions) {
			CreateExporter(settings, dataObject).ExportToMht(stream, exportOptions);
		}
		public static ActionResult ExportToMht(PivotGridSettings settings, object dataObject, DevExpress.XtraPrinting.MhtExportOptions exportOptions) {
			return Export(settings, dataObject, ExportToMht(exportOptions), "mht");
		}
		public static ActionResult ExportToMht(PivotGridSettings settings, object dataObject, string fileName, DevExpress.XtraPrinting.MhtExportOptions exportOptions) {
			return Export(settings, dataObject, ExportToMht(exportOptions), "mht", fileName);
		}
		public static ActionResult ExportToMht(PivotGridSettings settings, object dataObject, bool saveAsFile, DevExpress.XtraPrinting.MhtExportOptions exportOptions) {
			return Export(settings, dataObject, ExportToMht(exportOptions), "mht", saveAsFile);
		}
		public static ActionResult ExportToMht(PivotGridSettings settings, object dataObject, string fileName, bool saveAsFile, DevExpress.XtraPrinting.MhtExportOptions exportOptions) {
			return Export(settings, dataObject, ExportToMht(exportOptions), "mht", fileName, saveAsFile);
		}
		#endregion
		#region OLAP
		public static void OLAPExportToMht(PivotGridSettings settings, string connectionString, Stream stream) {
			CreateOLAPExporter(settings, connectionString).ExportToMht(stream, null);
		}
		public static ActionResult OLAPExportToMht(PivotGridSettings settings, string connectionString) {
			return OLAPExport(settings, connectionString, ExportToMht, "mht");
		}
		public static ActionResult OLAPExportToMht(PivotGridSettings settings, string connectionString, string fileName) {
			return OLAPExport(settings, connectionString, ExportToMht, "mht", fileName);
		}
		public static ActionResult OLAPExportToMht(PivotGridSettings settings, string connectionString, bool saveAsFile) {
			return OLAPExport(settings, connectionString, ExportToMht, "mht", saveAsFile);
		}
		public static ActionResult OLAPExportToMht(PivotGridSettings settings, string connectionString, string fileName, bool saveAsFile) {
			return OLAPExport(settings, connectionString, ExportToMht, "mht", fileName, saveAsFile);
		}
		public static void OLAPExportToMht(PivotGridSettings settings, string connectionString, Stream stream, DevExpress.XtraPrinting.MhtExportOptions exportOptions) {
			CreateOLAPExporter(settings, connectionString).ExportToMht(stream, exportOptions);
		}
		public static ActionResult OLAPExportToMht(PivotGridSettings settings, string connectionString, DevExpress.XtraPrinting.MhtExportOptions exportOptions) {
			return OLAPExport(settings, connectionString, ExportToMht(exportOptions), "mht");
		}
		public static ActionResult OLAPExportToMht(PivotGridSettings settings, string connectionString, string fileName, DevExpress.XtraPrinting.MhtExportOptions exportOptions) {
			return OLAPExport(settings, connectionString, ExportToMht(exportOptions), "mht", fileName);
		}
		public static ActionResult OLAPExportToMht(PivotGridSettings settings, string connectionString, bool saveAsFile, DevExpress.XtraPrinting.MhtExportOptions exportOptions) {
			return OLAPExport(settings, connectionString, ExportToMht(exportOptions), "mht", saveAsFile);
		}
		public static ActionResult OLAPExportToMht(PivotGridSettings settings, string connectionString, string fileName, bool saveAsFile, DevExpress.XtraPrinting.MhtExportOptions exportOptions) {
			return OLAPExport(settings, connectionString, ExportToMht(exportOptions), "mht", fileName, saveAsFile);
		}
		#endregion
		#endregion
		#region Rtf Export
		static void ExportToRtf(MVCxPivotGridExporter exporter, Stream stream) {
			exporter.ExportToRtf(stream);
		}
		static Action<MVCxPivotGridExporter, Stream> ExportToRtf(DevExpress.XtraPrinting.RtfExportOptions options) {
			return (e, s) => e.ExportToRtf(s, options);
		}
		#region DataObject
		public static void ExportToRtf(PivotGridSettings settings, object dataObject, Stream stream) {
			CreateExporter(settings, dataObject).ExportToRtf(stream);
		}
		public static ActionResult ExportToRtf(PivotGridSettings settings, object dataObject) {
			return Export(settings, dataObject, ExportToRtf, "rtf");
		}
		public static ActionResult ExportToRtf(PivotGridSettings settings, object dataObject, string fileName) {
			return Export(settings, dataObject, ExportToRtf, "rtf", fileName);
		}
		public static ActionResult ExportToRtf(PivotGridSettings settings, object dataObject, bool saveAsFile) {
			return Export(settings, dataObject, ExportToRtf, "rtf", saveAsFile);
		}
		public static ActionResult ExportToRtf(PivotGridSettings settings, object dataObject, string fileName, bool saveAsFile) {
			return Export(settings, dataObject, ExportToRtf, "rtf", fileName, saveAsFile);
		}
		public static void ExportToRtf(PivotGridSettings settings, object dataObject, Stream stream, DevExpress.XtraPrinting.RtfExportOptions exportOptions) {
			CreateExporter(settings, dataObject).ExportToRtf(stream, exportOptions);
		}
		public static ActionResult ExportToRtf(PivotGridSettings settings, object dataObject, DevExpress.XtraPrinting.RtfExportOptions exportOptions) {
			return Export(settings, dataObject, ExportToRtf(exportOptions), "rtf");
		}
		public static ActionResult ExportToRtf(PivotGridSettings settings, object dataObject, string fileName, DevExpress.XtraPrinting.RtfExportOptions exportOptions) {
			return Export(settings, dataObject, ExportToRtf(exportOptions), "rtf", fileName);
		}
		public static ActionResult ExportToRtf(PivotGridSettings settings, object dataObject, bool saveAsFile, DevExpress.XtraPrinting.RtfExportOptions exportOptions) {
			return Export(settings, dataObject, ExportToRtf(exportOptions), "rtf", saveAsFile);
		}
		public static ActionResult ExportToRtf(PivotGridSettings settings, object dataObject, string fileName, bool saveAsFile, DevExpress.XtraPrinting.RtfExportOptions exportOptions) {
			return Export(settings, dataObject, ExportToRtf(exportOptions), "rtf", fileName, saveAsFile);
		}
		#endregion
		#region OLAP
		public static void OLAPExportToRtf(PivotGridSettings settings, string connectionString, Stream stream) {
			CreateExporter(settings, connectionString).ExportToRtf(stream);
		}
		public static ActionResult OLAPExportToRtf(PivotGridSettings settings, string connectionString) {
			return OLAPExport(settings, connectionString, ExportToRtf, "rtf");
		}
		public static ActionResult OLAPExportToRtf(PivotGridSettings settings, string connectionString, string fileName) {
			return OLAPExport(settings, connectionString, ExportToRtf, "rtf", fileName);
		}
		public static ActionResult OLAPExportToRtf(PivotGridSettings settings, string connectionString, bool saveAsFile) {
			return OLAPExport(settings, connectionString, ExportToRtf, "rtf", saveAsFile);
		}
		public static ActionResult OLAPExportToRtf(PivotGridSettings settings, string connectionString, string fileName, bool saveAsFile) {
			return OLAPExport(settings, connectionString, ExportToRtf, "rtf", fileName, saveAsFile);
		}
		public static void OLAPExportToRtf(PivotGridSettings settings, string connectionString, Stream stream, DevExpress.XtraPrinting.RtfExportOptions exportOptions) {
			CreateOLAPExporter(settings, connectionString).ExportToRtf(stream, exportOptions);
		}
		public static ActionResult OLAPExportToRtf(PivotGridSettings settings, string connectionString, DevExpress.XtraPrinting.RtfExportOptions exportOptions) {
			return OLAPExport(settings, connectionString, ExportToRtf(exportOptions), "rtf");
		}
		public static ActionResult OLAPExportToRtf(PivotGridSettings settings, string connectionString, string fileName, DevExpress.XtraPrinting.RtfExportOptions exportOptions) {
			return OLAPExport(settings, connectionString, ExportToRtf(exportOptions), "rtf", fileName);
		}
		public static ActionResult OLAPExportToRtf(PivotGridSettings settings, string connectionString, bool saveAsFile, DevExpress.XtraPrinting.RtfExportOptions exportOptions) {
			return OLAPExport(settings, connectionString, ExportToRtf(exportOptions), "rtf", saveAsFile);
		}
		public static ActionResult OLAPExportToRtf(PivotGridSettings settings, string connectionString, string fileName, bool saveAsFile, DevExpress.XtraPrinting.RtfExportOptions exportOptions) {
			return OLAPExport(settings, connectionString, ExportToRtf(exportOptions), "rtf", fileName, saveAsFile);
		}
		#endregion
		#endregion
		#region Text Export
		static void ExportToText(MVCxPivotGridExporter exporter, Stream stream) {
			exporter.ExportToText(stream);
		}
		static Action<MVCxPivotGridExporter, Stream> ExportToText(DevExpress.XtraPrinting.TextExportOptions options) {
			return (e, s) => e.ExportToText(s, options);
		}
		#region DataObject
		public static void ExportToText(PivotGridSettings settings, object dataObject, Stream stream) {
			CreateExporter(settings, dataObject).ExportToText(stream);
		}
		public static ActionResult ExportToText(PivotGridSettings settings, object dataObject) {
			return Export(settings, dataObject, ExportToText, "txt");
		}
		public static ActionResult ExportToText(PivotGridSettings settings, object dataObject, string fileName) {
			return Export(settings, dataObject, ExportToText, "txt", fileName);
		}
		public static ActionResult ExportToText(PivotGridSettings settings, object dataObject, bool saveAsFile) {
			return Export(settings, dataObject, ExportToText, "txt", saveAsFile);
		}
		public static ActionResult ExportToText(PivotGridSettings settings, object dataObject, string fileName, bool saveAsFile) {
			return Export(settings, dataObject, ExportToText, "txt", fileName, saveAsFile);
		}
		public static void ExportToText(PivotGridSettings settings, object dataObject, Stream stream, DevExpress.XtraPrinting.TextExportOptions exportOptions) {
			CreateExporter(settings, dataObject).ExportToText(stream, exportOptions);
		}
		public static ActionResult ExportToText(PivotGridSettings settings, object dataObject, DevExpress.XtraPrinting.TextExportOptions exportOptions) {
			return Export(settings, dataObject, ExportToText(exportOptions), "txt");
		}
		public static ActionResult ExportToText(PivotGridSettings settings, object dataObject, string fileName, DevExpress.XtraPrinting.TextExportOptions exportOptions) {
			return Export(settings, dataObject, ExportToText(exportOptions), "txt", fileName);
		}
		public static ActionResult ExportToText(PivotGridSettings settings, object dataObject, bool saveAsFile, DevExpress.XtraPrinting.TextExportOptions exportOptions) {
			return Export(settings, dataObject, ExportToText(exportOptions), "txt", saveAsFile);
		}
		public static ActionResult ExportToText(PivotGridSettings settings, object dataObject, string fileName, bool saveAsFile, DevExpress.XtraPrinting.TextExportOptions exportOptions) {
			return Export(settings, dataObject, ExportToText(exportOptions), "txt", fileName, saveAsFile);
		}
		#endregion
		#region OLAP
		public static void OLAPExportToText(PivotGridSettings settings, string connectionString, Stream stream) {
			CreateOLAPExporter(settings, connectionString).ExportToText(stream);
		}
		public static ActionResult OLAPExportToText(PivotGridSettings settings, string connectionString) {
			return OLAPExport(settings, connectionString, ExportToText, "txt");
		}
		public static ActionResult OLAPExportToText(PivotGridSettings settings, string connectionString, string fileName) {
			return OLAPExport(settings, connectionString, ExportToText, "txt", fileName);
		}
		public static ActionResult OLAPExportToText(PivotGridSettings settings, string connectionString, bool saveAsFile) {
			return OLAPExport(settings, connectionString, ExportToText, "txt", saveAsFile);
		}
		public static ActionResult OLAPExportToText(PivotGridSettings settings, string connectionString, string fileName, bool saveAsFile) {
			return OLAPExport(settings, connectionString, ExportToText, "txt", fileName, saveAsFile);
		}
		public static void OLAPExportToText(PivotGridSettings settings, string connectionString, Stream stream, DevExpress.XtraPrinting.TextExportOptions exportOptions) {
			CreateOLAPExporter(settings, connectionString).ExportToText(stream, exportOptions);
		}
		public static ActionResult OLAPExportToText(PivotGridSettings settings, string connectionString, DevExpress.XtraPrinting.TextExportOptions exportOptions) {
			return OLAPExport(settings, connectionString, ExportToText(exportOptions), "txt");
		}
		public static ActionResult OLAPExportToText(PivotGridSettings settings, string connectionString, string fileName, DevExpress.XtraPrinting.TextExportOptions exportOptions) {
			return OLAPExport(settings, connectionString, ExportToText(exportOptions), "txt", fileName);
		}
		public static ActionResult OLAPExportToText(PivotGridSettings settings, string connectionString, bool saveAsFile, DevExpress.XtraPrinting.TextExportOptions exportOptions) {
			return OLAPExport(settings, connectionString, ExportToText(exportOptions), "txt", saveAsFile);
		}
		public static ActionResult OLAPExportToText(PivotGridSettings settings, string connectionString, string fileName, bool saveAsFile, DevExpress.XtraPrinting.TextExportOptions exportOptions) {
			return OLAPExport(settings, connectionString, ExportToText(exportOptions), "txt", fileName, saveAsFile);
		}
		#endregion
		#endregion
		#region Html Export
		static void ExportToHtml(MVCxPivotGridExporter exporter, Stream stream) {
			exporter.ExportToHtml(stream);
		}
		static Action<MVCxPivotGridExporter, Stream> ExportToHtml(DevExpress.XtraPrinting.HtmlExportOptions options) {
			return (e, s) => e.ExportToHtml(s, options);
		}
		#region DataObject
		public static void ExportToHtml(PivotGridSettings settings, object dataObject, Stream stream) {
			CreateExporter(settings, dataObject).ExportToHtml(stream);
		}
		public static ActionResult ExportToHtml(PivotGridSettings settings, object dataObject) {
			return Export(settings, dataObject, ExportToHtml, "html");
		}
		public static ActionResult ExportToHtml(PivotGridSettings settings, object dataObject, string fileName) {
			return Export(settings, dataObject, ExportToHtml, "html", fileName);
		}
		public static ActionResult ExportToHtml(PivotGridSettings settings, object dataObject, bool saveAsFile) {
			return Export(settings, dataObject, ExportToHtml, "html", saveAsFile);
		}
		public static ActionResult ExportToHtml(PivotGridSettings settings, object dataObject, string fileName, bool saveAsFile) {
			return Export(settings, dataObject, ExportToHtml, "html", fileName, saveAsFile);
		}
		public static void ExportToHtml(PivotGridSettings settings, object dataObject, Stream stream, DevExpress.XtraPrinting.HtmlExportOptions exportOptions) {
			CreateExporter(settings, dataObject).ExportToHtml(stream, exportOptions);
		}
		public static ActionResult ExportToHtml(PivotGridSettings settings, object dataObject, DevExpress.XtraPrinting.HtmlExportOptions exportOptions) {
			return Export(settings, dataObject, ExportToHtml(exportOptions), "html");
		}
		public static ActionResult ExportToHtml(PivotGridSettings settings, object dataObject, string fileName, DevExpress.XtraPrinting.HtmlExportOptions exportOptions) {
			return Export(settings, dataObject, ExportToHtml(exportOptions), "html", fileName);
		}
		public static ActionResult ExportToHtml(PivotGridSettings settings, object dataObject, bool saveAsFile, DevExpress.XtraPrinting.HtmlExportOptions exportOptions) {
			return Export(settings, dataObject, ExportToHtml(exportOptions), "html", saveAsFile);
		}
		public static ActionResult ExportToHtml(PivotGridSettings settings, object dataObject, string fileName, bool saveAsFile, DevExpress.XtraPrinting.HtmlExportOptions exportOptions) {
			return Export(settings, dataObject, ExportToHtml(exportOptions), "html", fileName, saveAsFile);
		}
		#endregion
		#region OLAP
		public static void OLAPExportToHtml(PivotGridSettings settings, string connectionString, Stream stream) {
			CreateOLAPExporter(settings, connectionString).ExportToHtml(stream);
		}
		public static ActionResult OLAPExportToHtml(PivotGridSettings settings, string connectionString) {
			return OLAPExport(settings, connectionString, ExportToHtml, "html");
		}
		public static ActionResult OLAPExportToHtml(PivotGridSettings settings, string connectionString, string fileName) {
			return OLAPExport(settings, connectionString, ExportToHtml, "html", fileName);
		}
		public static ActionResult OLAPExportToHtml(PivotGridSettings settings, string connectionString, bool saveAsFile) {
			return OLAPExport(settings, connectionString, ExportToHtml, "html", saveAsFile);
		}
		public static ActionResult OLAPExportToHtml(PivotGridSettings settings, string connectionString, string fileName, bool saveAsFile) {
			return OLAPExport(settings, connectionString, ExportToHtml, "html", fileName, saveAsFile);
		}
		public static void OLAPExportToHtml(PivotGridSettings settings, string connectionString, Stream stream, DevExpress.XtraPrinting.HtmlExportOptions exportOptions) {
			CreateOLAPExporter(settings, connectionString).ExportToHtml(stream, exportOptions);
		}
		public static ActionResult OLAPExportToHtml(PivotGridSettings settings, string connectionString, DevExpress.XtraPrinting.HtmlExportOptions exportOptions) {
			return OLAPExport(settings, connectionString, ExportToHtml(exportOptions), "html");
		}
		public static ActionResult OLAPExportToHtml(PivotGridSettings settings, string connectionString, string fileName, DevExpress.XtraPrinting.HtmlExportOptions exportOptions) {
			return OLAPExport(settings, connectionString, ExportToHtml(exportOptions), "html", fileName);
		}
		public static ActionResult OLAPExportToHtml(PivotGridSettings settings, string connectionString, bool saveAsFile, DevExpress.XtraPrinting.HtmlExportOptions exportOptions) {
			return OLAPExport(settings, connectionString, ExportToHtml(exportOptions), "html", saveAsFile);
		}
		public static ActionResult OLAPExportToHtml(PivotGridSettings settings, string connectionString, string fileName, bool saveAsFile, DevExpress.XtraPrinting.HtmlExportOptions exportOptions) {
			return OLAPExport(settings, connectionString, ExportToHtml(exportOptions), "html", fileName, saveAsFile);
		}
		#endregion
		#endregion
		#region Csv Export
		static void ExportToCsv(MVCxPivotGridExporter exporter, Stream stream) {
			exporter.ExportToCsv(stream);
		}
		static Action<MVCxPivotGridExporter, Stream> ExportToCsv(DevExpress.XtraPrinting.CsvExportOptions options) {
			return (e, s) => e.ExportToCsv(s, options);
		}
		#region DataObject
		public static void ExportToCsv(PivotGridSettings settings, object dataObject, Stream stream) {
			CreateExporter(settings, dataObject).ExportToCsv(stream);
		}
		public static ActionResult ExportToCsv(PivotGridSettings settings, object dataObject) {
			return Export(settings, dataObject, ExportToCsv, "csv");
		}
		public static ActionResult ExportToCsv(PivotGridSettings settings, object dataObject, string fileName) {
			return Export(settings, dataObject, ExportToCsv, "csv", fileName);
		}
		public static ActionResult ExportToCsv(PivotGridSettings settings, object dataObject, bool saveAsFile) {
			return Export(settings, dataObject, ExportToCsv, "csv", saveAsFile);
		}
		public static ActionResult ExportToCsv(PivotGridSettings settings, object dataObject, string fileName, bool saveAsFile) {
			return Export(settings, dataObject, ExportToCsv, "csv", fileName, saveAsFile);
		}
		public static void ExportToCsv(PivotGridSettings settings, object dataObject, Stream stream, DevExpress.XtraPrinting.CsvExportOptions exportOptions) {
			CreateExporter(settings, dataObject).ExportToCsv(stream, exportOptions);
		}
		public static ActionResult ExportToCsv(PivotGridSettings settings, object dataObject, DevExpress.XtraPrinting.CsvExportOptions exportOptions) {
			return Export(settings, dataObject, ExportToCsv(exportOptions), "csv");
		}
		public static ActionResult ExportToCsv(PivotGridSettings settings, object dataObject, string fileName, DevExpress.XtraPrinting.CsvExportOptions exportOptions) {
			return Export(settings, dataObject, ExportToCsv(exportOptions), "csv", fileName);
		}
		public static ActionResult ExportToCsv(PivotGridSettings settings, object dataObject, bool saveAsFile, DevExpress.XtraPrinting.CsvExportOptions exportOptions) {
			return Export(settings, dataObject, ExportToCsv(exportOptions), "csv", saveAsFile);
		}
		public static ActionResult ExportToCsv(PivotGridSettings settings, object dataObject, string fileName, bool saveAsFile, DevExpress.XtraPrinting.CsvExportOptions exportOptions) {
			return Export(settings, dataObject, ExportToCsv(exportOptions), "csv", fileName, saveAsFile);
		}
		#endregion
		#region OLAP
		public static void OLAPExportToCsv(PivotGridSettings settings, string connectionString, Stream stream) {
			CreateOLAPExporter(settings, connectionString).ExportToCsv(stream);
		}
		public static ActionResult OLAPExportToCsv(PivotGridSettings settings, string connectionString) {
			return OLAPExport(settings, connectionString, ExportToCsv, "csv");
		}
		public static ActionResult OLAPExportToCsv(PivotGridSettings settings, string connectionString, string fileName) {
			return OLAPExport(settings, connectionString, ExportToCsv, "csv", fileName);
		}
		public static ActionResult OLAPExportToCsv(PivotGridSettings settings, string connectionString, bool saveAsFile) {
			return OLAPExport(settings, connectionString, ExportToCsv, "csv", saveAsFile);
		}
		public static ActionResult OLAPExportToCsv(PivotGridSettings settings, string connectionString, string fileName, bool saveAsFile) {
			return OLAPExport(settings, connectionString, ExportToCsv, "csv", fileName, saveAsFile);
		}
		public static void OLAPExportToCsv(PivotGridSettings settings, string connectionString, Stream stream, DevExpress.XtraPrinting.CsvExportOptions exportOptions) {
			CreateOLAPExporter(settings, connectionString).ExportToCsv(stream, exportOptions);
		}
		public static ActionResult OLAPExportToCsv(PivotGridSettings settings, string connectionString, DevExpress.XtraPrinting.CsvExportOptions exportOptions) {
			return OLAPExport(settings, connectionString, ExportToCsv(exportOptions), "csv");
		}
		public static ActionResult OLAPExportToCsv(PivotGridSettings settings, string connectionString, string fileName, DevExpress.XtraPrinting.CsvExportOptions exportOptions) {
			return OLAPExport(settings, connectionString, ExportToCsv(exportOptions), "csv", fileName);
		}
		public static ActionResult OLAPExportToCsv(PivotGridSettings settings, string connectionString, bool saveAsFile, DevExpress.XtraPrinting.CsvExportOptions exportOptions) {
			return OLAPExport(settings, connectionString, ExportToCsv(exportOptions), "csv", saveAsFile);
		}
		public static ActionResult OLAPExportToCsv(PivotGridSettings settings, string connectionString, string fileName, bool saveAsFile, DevExpress.XtraPrinting.CsvExportOptions exportOptions) {
			return OLAPExport(settings, connectionString, ExportToCsv(exportOptions), "csv", fileName, saveAsFile);
		}
		#endregion
		#endregion
		#region Image Export
		static void ExportToImage(MVCxPivotGridExporter exporter, Stream stream) {
			exporter.ExportToImage(stream);
		}
		static Action<MVCxPivotGridExporter, Stream> ExportToImage(DevExpress.XtraPrinting.ImageExportOptions options) {
			return (e, s) => e.ExportToImage(s, options);
		}
		#region DataObject
		public static void ExportToImage(PivotGridSettings settings, object dataObject, Stream stream) {
			CreateExporter(settings, dataObject).ExportToImage(stream);
		}
		public static ActionResult ExportToImage(PivotGridSettings settings, object dataObject) {
			return Export(settings, dataObject, ExportToImage, "png");
		}
		public static ActionResult ExportToImage(PivotGridSettings settings, object dataObject, string fileName) {
			return Export(settings, dataObject, ExportToImage, "png", fileName);
		}
		public static ActionResult ExportToImage(PivotGridSettings settings, object dataObject, bool saveAsFile) {
			return Export(settings, dataObject, ExportToImage, "png", saveAsFile);
		}
		public static ActionResult ExportToImage(PivotGridSettings settings, object dataObject, string fileName, bool saveAsFile) {
			return Export(settings, dataObject, ExportToImage, "png", fileName, saveAsFile);
		}
		public static void ExportToImage(PivotGridSettings settings, object dataObject, Stream stream, DevExpress.XtraPrinting.ImageExportOptions exportOptions) {
			CreateExporter(settings, dataObject).ExportToImage(stream, exportOptions);
		}
		public static ActionResult ExportToImage(PivotGridSettings settings, object dataObject, DevExpress.XtraPrinting.ImageExportOptions exportOptions) {
			return Export(settings, dataObject, ExportToImage(exportOptions), ImageFormatHelper.ToExtension(exportOptions.Format));
		}
		public static ActionResult ExportToImage(PivotGridSettings settings, object dataObject, string fileName, DevExpress.XtraPrinting.ImageExportOptions exportOptions) {
			return Export(settings, dataObject, ExportToImage(exportOptions), ImageFormatHelper.ToExtension(exportOptions.Format), fileName);
		}
		public static ActionResult ExportToImage(PivotGridSettings settings, object dataObject, bool saveAsFile, DevExpress.XtraPrinting.ImageExportOptions exportOptions) {
			return Export(settings, dataObject, ExportToImage(exportOptions), ImageFormatHelper.ToExtension(exportOptions.Format), saveAsFile);
		}
		public static ActionResult ExportToImage(PivotGridSettings settings, object dataObject, string fileName, bool saveAsFile, DevExpress.XtraPrinting.ImageExportOptions exportOptions) {
			return Export(settings, dataObject, ExportToImage(exportOptions), ImageFormatHelper.ToExtension(exportOptions.Format), fileName, saveAsFile);
		}
		#endregion
		#region OLAP
		public static void OLAPExportToImage(PivotGridSettings settings, string connectionString, Stream stream) {
			CreateOLAPExporter(settings, connectionString).ExportToImage(stream);
		}
		public static ActionResult OLAPExportToImage(PivotGridSettings settings, string connectionString) {
			return OLAPExport(settings, connectionString, ExportToImage, "png");
		}
		public static ActionResult OLAPExportToImage(PivotGridSettings settings, string connectionString, string fileName) {
			return OLAPExport(settings, connectionString, ExportToImage, "png", fileName);
		}
		public static ActionResult OLAPExportToImage(PivotGridSettings settings, string connectionString, bool saveAsFile) {
			return OLAPExport(settings, connectionString, ExportToImage, "png", saveAsFile);
		}
		public static ActionResult OLAPExportToImage(PivotGridSettings settings, string connectionString, string fileName, bool saveAsFile) {
			return OLAPExport(settings, connectionString, ExportToImage, "png", fileName, saveAsFile);
		}
		public static void OLAPExportToImage(PivotGridSettings settings, string connectionString, Stream stream, DevExpress.XtraPrinting.ImageExportOptions exportOptions) {
			CreateOLAPExporter(settings, connectionString).ExportToImage(stream, exportOptions);
		}
		public static ActionResult OLAPExportToImage(PivotGridSettings settings, string connectionString, DevExpress.XtraPrinting.ImageExportOptions exportOptions) {
			return OLAPExport(settings, connectionString, ExportToImage(exportOptions), ImageFormatHelper.ToExtension(exportOptions.Format));
		}
		public static ActionResult OLAPExportToImage(PivotGridSettings settings, string connectionString, string fileName, DevExpress.XtraPrinting.ImageExportOptions exportOptions) {
			return OLAPExport(settings, connectionString, ExportToImage(exportOptions), ImageFormatHelper.ToExtension(exportOptions.Format), fileName);
		}
		public static ActionResult OLAPExportToImage(PivotGridSettings settings, string connectionString, bool saveAsFile, DevExpress.XtraPrinting.ImageExportOptions exportOptions) {
			return OLAPExport(settings, connectionString, ExportToImage(exportOptions), ImageFormatHelper.ToExtension(exportOptions.Format), saveAsFile);
		}
		public static ActionResult OLAPExportToImage(PivotGridSettings settings, string connectionString, string fileName, bool saveAsFile, DevExpress.XtraPrinting.ImageExportOptions exportOptions) {
			return OLAPExport(settings, connectionString, ExportToImage(exportOptions), ImageFormatHelper.ToExtension(exportOptions.Format), fileName, saveAsFile);
		}
		#endregion
		#endregion
		protected internal static MVCxPivotGrid CreatePivotGridControl(PivotGridSettings settings) {
			return CreateExtensionInternal(settings, null, false, MvcUtils.CallbackName == settings.Name).Control;
		}
		internal static PivotGridExtension CreateExtension(PivotGridSettings settings, object dataObject) {
			return CreateExtensionInternal(settings, dataObject, false);
		}
		internal static PivotGridExtension CreateOLAPExtension(PivotGridSettings settings, string connectionString) {
			return CreateExtensionInternal(settings, connectionString, true);
		}
		static PivotGridExtension CreateExtensionInternal(PivotGridSettings settings, object dataObject, bool isOLAP, bool isLoadPostData = true) {
			PivotGridExtension extension = ExtensionsFactory.InstanceInternal.PivotGrid(settings);
			if (isOLAP)
				extension.BindToOLAP(dataObject.ToString());
			else
				extension.Bind(dataObject);
			extension.PrepareControl();
			if(isLoadPostData)
				extension.LoadPostData();
			return extension;
		}
		static MVCxPivotGridExporter CreateExporter(PivotGridSettings settings, object dataObject) {
			MVCxPivotGridExporter exporter = new MVCxPivotGridExporter(settings, dataObject);
			AssignExporterSettings(exporter, settings.SettingsExport);
			return exporter;
		}
		static MVCxPivotGridExporter CreateOLAPExporter(PivotGridSettings settings, string connectionString) {
			MVCxPivotGridExporter exporter = new MVCxPivotGridExporter(settings, connectionString);
			AssignExporterSettings(exporter, settings.SettingsExport);
			return exporter;
		}
		static void AssignExporterSettings(MVCxPivotGridExporter exporter, MVCxPivotGridExportSettings settings) {
			exporter.OptionsPrint.Assign(settings.OptionsPrint);
			exporter.CustomExportHeader += settings.CustomExportHeader;
			exporter.CustomExportFieldValue += settings.CustomExportFieldValue;
			exporter.CustomExportCell += settings.CustomExportCell;
		}
		protected override DevExpress.Web.ASPxWebControl CreateControl() {
			return new MVCxPivotGrid(ViewContext);
		}
		protected internal override void DisposeControl() {
			if(CustomizationExtension != null)
				CustomizationExtension.DisposeControl();
			base.DisposeControl();
		}
	}
}
