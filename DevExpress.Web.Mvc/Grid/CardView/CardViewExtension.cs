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

namespace DevExpress.Web.Mvc {
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Web.Mvc;
	using System.Web.UI;
	using DevExpress.Data.Linq;
	using DevExpress.Web.Mvc.Internal;
	using DevExpress.Web.Mvc.UI;
	using DevExpress.XtraPrinting;
	public class CardViewExtension<CardType> : CardViewExtension {
		public CardViewExtension(CardViewSettings<CardType> settings)
			: base(settings) {
		}
		public CardViewExtension(CardViewSettings<CardType> settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
	}
	public class CardViewExtension : GridExtensionBase {
		public CardViewExtension(CardViewSettings settings)
			: base(settings) {
		}
		public CardViewExtension(CardViewSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		public static CardViewModel GetViewModel(string name) {
			return CardViewModel.Load(name);
		}
		protected internal new MVCxCardView Control {
			get { return (MVCxCardView)base.Control; }
			protected set { base.Control = value; }
		}
		protected internal new CardViewSettings Settings {
			get { return (CardViewSettings)base.Settings; }
		}
		protected override DevExpress.Web.ASPxWebControl CreateControl() {
			return new MVCxCardView(ViewContext);
		}
		protected override void AssignInitialProperties() {
			AssignRouteValuesToControl();
			Control.Styles.CopyFrom(Settings.Styles);
			base.AssignInitialProperties();
			Control.EditFormLayoutProperties.Assign(Settings.EditFormLayoutProperties);
			Control.CardLayoutProperties.Assign(Settings.CardLayoutProperties);
			Control.Columns.Assign(Settings.Columns);
			Control.ClientSideEvents.Assign(Settings.ClientSideEvents);
			Control.EnableCardsCache = Settings.EnableCardsCache;
			Control.Images.CopyFrom(Settings.Images);
			Control.Settings.Assign(Settings.Settings);
			Control.SettingsBehavior.Assign(Settings.SettingsBehavior);
			Control.SettingsEditing.Assign(Settings.SettingsEditing);
			Control.SettingsPager.Assign(Settings.SettingsPager);
			Control.SettingsPopup.Assign(Settings.SettingsPopup);
			Control.SettingsSearchPanel.Assign(Settings.SettingsSearchPanel);
			Control.SettingsFilterControl.Assign(Settings.SettingsFilterControl);
			Control.TotalSummary.Assign(Settings.TotalSummary);
			Control.FormatConditions.Assign(Settings.FormatConditions);
			Control.AfterPerformCallback += Settings.AfterPerformCallback;
			Control.BeforeColumnSorting += Settings.BeforeColumnSorting;
			Control.BeforeGetCallbackResult += Settings.BeforeGetCallbackResult;
			Control.BeforeHeaderFilterFillItems += Settings.BeforeHeaderFilterFillItems;
			Control.CardValidating += Settings.CardValidating;
			Control.CellEditorInitialize += Settings.CellEditorInitialize;
			Control.CommandButtonInitialize += Settings.CommandButtonInitialize;
			Control.CustomButtonInitialize += Settings.CustomButtonInitialize;
			Control.CustomColumnDisplayText += Settings.CustomColumnDisplayText;
			Control.CustomColumnSort += Settings.CustomColumnSort;
			Control.CustomJSProperties += Settings.CustomJSProperties;
			Control.CustomSummaryCalculate += Settings.CustomSummaryCalculate;
			Control.CustomUnboundColumnData += Settings.CustomUnboundColumnData;
			Control.InitNewCard += Settings.InitNewCard;
			Control.HeaderFilterFillItems += Settings.HeaderFilterFillItems;
			Control.SearchPanelEditorCreate += Settings.SearchPanelEditorCreate;
			Control.SearchPanelEditorInitialize += Settings.SearchPanelEditorInitialize;
			Control.SummaryDisplayText += Settings.SummaryDisplayText;
		}
		void AssignRouteValuesToControl() {
			Control.CallbackRouteValues = Settings.CallbackRouteValues;
			Control.CallbackActionUrlCollection.Clear();
			if(Settings.CustomActionRouteValues != null)
				Control.CallbackActionUrlCollection.Add(GridViewCallbackCommand.CustomCallback, Utils.GetUrl(Settings.CustomActionRouteValues));
			if(Settings.CustomDataActionRouteValues != null)
				Control.CallbackActionUrlCollection.Add(GridViewCallbackCommand.CustomValues, Utils.GetUrl(Settings.CustomDataActionRouteValues));
			foreach(KeyValuePair<CardViewOperationType, object> keyValuePair in Settings.CustomBindingRouteValuesCollection) {
				string[] commands = GridAdapter.GetCommandsByActionType((int)keyValuePair.Key);
				foreach(string command in commands) {
					Control.CallbackActionUrlCollection.Add(command, Utils.GetUrl(keyValuePair.Value));
				}
			}
		}
		protected override void AssignRenderProperties() {
			base.AssignRenderProperties();
			Control.Templates.DataItem = ContentControlTemplate<CardViewDataItemTemplateContainer>.Create(
				Settings.DataItemTemplateContent, Settings.DataItemTemplateContentMethod,
				typeof(CardViewDataItemTemplateContainer));
			Control.Templates.Card = ContentControlTemplate<CardViewCardTemplateContainer>.Create(
				Settings.CardTemplateContent, Settings.CardTemplateContentMethod,
				typeof(CardViewCardTemplateContainer));
			Control.Templates.CardFooter = ContentControlTemplate<CardViewCardFooterTemplateContainer>.Create(
				Settings.CardFooterTemplateContent, Settings.CardFooterTemplateContentMethod,
				typeof(CardViewCardFooterTemplateContainer));
			Control.Templates.CardHeader = ContentControlTemplate<CardViewCardHeaderTemplateContainer>.Create(
				Settings.CardHeaderTemplateContent, Settings.CardHeaderTemplateContentMethod,
				typeof(CardViewCardHeaderTemplateContainer));
			Control.Templates.EditForm = ContentControlTemplate<CardViewEditFormTemplateContainer>.Create(
				Settings.EditFormTemplateContent, Settings.EditFormTemplateContentMethod,
				typeof(CardViewEditFormTemplateContainer));
			Control.Templates.TitlePanel = ContentControlTemplate<CardViewTitleTemplateContainer>.Create(
				Settings.TitlePanelTemplateContent, Settings.TitlePanelTemplateContentMethod,
				typeof(CardViewTitleTemplateContainer));
			Control.Templates.StatusBar = ContentControlTemplate<CardViewStatusBarTemplateContainer>.Create(
				Settings.StatusBarTemplateContent, Settings.StatusBarTemplateContentMethod,
				typeof(CardViewStatusBarTemplateContainer));
			Control.Templates.Header = ContentControlTemplate<CardViewHeaderTemplateContainer>.Create(
				Settings.HeaderTemplateContent, Settings.HeaderTemplateContentMethod,
				typeof(CardViewHeaderTemplateContainer));
			Control.Templates.EditItem = ContentControlTemplate<CardViewEditItemTemplateContainer>.Create(
				Settings.EditItemTemplateContent, Settings.EditItemTemplateContentMethod,
				typeof(CardViewEditItemTemplateContainer));
			Control.Templates.PagerBar = ContentControlTemplate<CardViewPagerBarTemplateContainer>.Create(
				Settings.PagerBarTemplateContent, Settings.PagerBarTemplateContentMethod,
				typeof(CardViewPagerBarTemplateContainer));
			Control.Templates.HeaderPanel= ContentControlTemplate<CardViewHeaderPanelTemplateContainer>.Create(
				Settings.HeaderPanelTemplateContent, Settings.HeaderPanelTemplateContentMethod,
				typeof(CardViewHeaderPanelTemplateContainer));
			for(int i = 0; i < Control.Columns.Count; i++) {
				AssignColumnTemplates(Settings.Columns[i], Control.Columns[i]);
			}
			Control.CardLayoutProperties.ForEach(item => AssignFormLayoutItemTemplate(item));
			Control.EditFormLayoutProperties.ForEach(item => AssignFormLayoutItemTemplate(item));
		}
		protected void AssignColumnTemplates(CardViewColumn sourceColumn, CardViewColumn destinationColumn) {
			MVCxCardViewColumn sourceDataColumn = sourceColumn as MVCxCardViewColumn;
			if(sourceDataColumn == null) return;
			destinationColumn.HeaderTemplate = ContentControlTemplate<CardViewHeaderTemplateContainer>.Create(
				sourceDataColumn.HeaderTemplateContent, sourceDataColumn.HeaderTemplateContentMethod,
				typeof(CardViewHeaderTemplateContainer));
			destinationColumn.DataItemTemplate = ContentControlTemplate<CardViewDataItemTemplateContainer>.Create(
			   sourceDataColumn.DataItemTemplateContent, sourceDataColumn.DataItemTemplateContentMethod,
			   typeof(CardViewDataItemTemplateContainer), Control.RenderHelper.UseEndlessPaging);
			destinationColumn.EditItemTemplate = ContentControlTemplate<CardViewEditItemTemplateContainer>.Create(
				sourceDataColumn.EditItemTemplateContent, sourceDataColumn.EditItemTemplateContentMethod,
				typeof(CardViewEditItemTemplateContainer), Control.RenderHelper.UseEndlessPaging);
		}
		protected void AssignFormLayoutItemTemplate(LayoutItemBase layoutItem) {
			MVCxCardViewColumnLayoutItem columnLayoutItem = layoutItem as MVCxCardViewColumnLayoutItem;
			if(columnLayoutItem == null) return;
			columnLayoutItem.Template = ContentControlTemplate<CardViewDataItemTemplateContainer>.Create(
				columnLayoutItem.TemplateContent, columnLayoutItem.TemplateContentMethod,
				typeof(CardViewDataItemTemplateContainer));
		}
		protected override void ProcessCallback(string callbackArgument) {
			Control.EnsureChildControls();
			base.ProcessCallback(callbackArgument);
		}
		protected override Control GetCallbackResultControl() {
			if(IsExistFilterControl) return null;
			return Control.GetCallbackResultControl();
		}
		protected override void RenderCallbackResultControl() {
			base.RenderCallbackResultControl();
			RenderString(Control.GetCallbackClientObjectScript());
			RenderString(Control.GetPostponeScript());
		}
		protected override MVCxPopupFilterControl FilterControl { get { return Control.FilterControl; } }
		public CardViewExtension Bind(object dataObject) {
			BindInternal(dataObject);
			return this;
		}
		public CardViewExtension BindToXML(string fileName) {
			return BindToXML(fileName, string.Empty, string.Empty);
		}
		public CardViewExtension BindToXML(string fileName, string xPath) {
			return BindToXML(fileName, xPath, string.Empty);
		}
		public CardViewExtension BindToXML(string fileName, string xPath, string transformFileName) {
			BindToXMLInternal(fileName, xPath, transformFileName);
			return this;
		}
		public CardViewExtension BindToLINQ(Type contextType, string tableName) {
			return BindToLINQ(contextType.FullName, tableName);
		}
		public CardViewExtension BindToLINQ(string contextTypeName, string tableName) {
			return BindToLINQ(contextTypeName, tableName, null);
		}
		public CardViewExtension BindToLINQ(Type contextType, string tableName, EventHandler<LinqServerModeDataSourceSelectEventArgs> selectingMethod) {
			return BindToLINQ(contextType.FullName, tableName, selectingMethod);
		}
		public CardViewExtension BindToLINQ(string contextTypeName, string tableName, EventHandler<LinqServerModeDataSourceSelectEventArgs> selectingMethod) {
			return BindToLINQ(contextTypeName, tableName, selectingMethod, null);
		}
		public CardViewExtension BindToLINQ(string contextTypeName, string tableName, EventHandler<LinqServerModeDataSourceSelectEventArgs> selectingMethod,
			EventHandler<DevExpress.Data.ServerModeExceptionThrownEventArgs> exceptionThrownMethod) {
			BindToLINQDataSourceInternal(contextTypeName, tableName, selectingMethod, exceptionThrownMethod);
			return this;
		}
		public CardViewExtension BindToEF(Type contextType, string tableName) {
			return BindToEF(contextType.FullName, tableName);
		}
		public CardViewExtension BindToEF(string contextTypeName, string tableName) {
			return BindToEF(contextTypeName, tableName, null);
		}
		public CardViewExtension BindToEF(Type contextType, string tableName, EventHandler<LinqServerModeDataSourceSelectEventArgs> selectingMethod) {
			return BindToEF(contextType.FullName, tableName, selectingMethod);
		}
		public CardViewExtension BindToEF(string contextTypeName, string tableName, EventHandler<LinqServerModeDataSourceSelectEventArgs> selectingMethod) {
			return BindToEF(contextTypeName, tableName, selectingMethod, null);
		}
		public CardViewExtension BindToEF(string contextTypeName, string tableName, EventHandler<LinqServerModeDataSourceSelectEventArgs> selectingMethod,
			EventHandler<DevExpress.Data.ServerModeExceptionThrownEventArgs> exceptionThrownMethod) {
			BindToEFDataSourceInternal(contextTypeName, tableName, selectingMethod, exceptionThrownMethod);
			return this;
		}
		public CardViewExtension BindToCustomData(CardViewModel viewModel) {
			Control.EnableCustomOperations = true;
			Control.CustomOperationViewModel.Assign(viewModel);
			Control.Settings.LayoutMode = viewModel.LayoutMode;
			Control.SettingsPager.SettingsTableLayout.Assign(viewModel.Pager.SettingsTableLayout);
			Control.SettingsPager.SettingsFlowLayout.Assign(viewModel.Pager.SettingsFlowLayout);
			Control.PageIndex = viewModel.Pager.PageIndex;
			Control.SettingsSearchPanel.GroupOperator = viewModel.SearchPanel.GroupOperator;
			Control.SettingsSearchPanel.ColumnNames = viewModel.SearchPanel.ColumnNames;
			Control.SearchPanelFilter = viewModel.SearchPanel.Filter;
			Control.FilterExpression = viewModel.FilterExpression;
			Control.FilterEnabled = viewModel.IsFilterApplied;
			PrepareColumnsViaViewModel(viewModel);
			if(!string.IsNullOrEmpty(viewModel.KeyFieldName))
				Control.KeyFieldName = viewModel.KeyFieldName;
			AddSummaryFromState(Control.TotalSummary, viewModel.TotalSummary);
			return this;
		}
		void PrepareColumnsViaViewModel(CardViewModel viewModel) {
			foreach(var columnState in viewModel.Columns) {
				MVCxCardViewColumn column = Control.Columns[columnState.FieldName] as MVCxCardViewColumn;
				if(Control.Columns.Count == 0 && column == null) {
					column = new MVCxCardViewColumn(columnState.FieldName);
					Control.Columns.Add(column);
				}
				if(column != null)
					column.AssignState(columnState);
			}
		}
		static void AddSummaryFromState(ASPxCardViewSummaryItemCollection summaryItems, IGridSummaryItemStateCollection summaryItemStates) {
			foreach(var itemState in summaryItemStates) {
				var item = summaryItems.Where(s => s.FieldName == itemState.FieldName && s.SummaryType == itemState.SummaryType && s.Tag == itemState.Tag).SingleOrDefault();
				if(item != null)
					return;
				item = new ASPxCardViewSummaryItem();
				summaryItems.Add(item);
				item.FieldName = itemState.FieldName;
				item.SummaryType = itemState.SummaryType;
				item.Tag = itemState.Tag;
			}
		}
		public CardViewExtension SetEditErrorText(string message) {
			Control.ErrorText = message;
			return this;
		}
		public static T GetEditValue<T>(string fieldName) {
			return GridValueProvider.GetValue<T>(fieldName);
		}
		public static Dictionary<S, T> GetBatchUpdateValues<S, T>(string fieldName) {
			return GridValueProvider.GetBatchUpdateValues<S, T>(fieldName);
		}
		public static List<T> GetBatchInsertValues<T>(string fieldName) {
			return GridValueProvider.GetBatchInsertValues<T>(fieldName);
		}
		public static List<T> GetBatchDeleteKeys<T>() {
			return GridValueProvider.GetBatchDeleteKeys<T>();
		}
		#region Export
		public static IPrintable CreatePrintableObject(CardViewSettings settings, object dataObject) {
			return (IPrintable)CreateExporterAndRaiseBeforeExport(settings, dataObject);
		}
		static ActionResult Export(CardViewSettings settings, object dataObject, Action<MVCxCardViewExporter, Stream> write, string fileExtension) {
			return Export(settings, dataObject, write, fileExtension, null);
		}
		static ActionResult Export(CardViewSettings settings, object dataObject, Action<MVCxCardViewExporter, Stream> write, string fileExtension, string fileName) {
			return Export(settings, dataObject, write, fileExtension, fileName, true);
		}
		static ActionResult Export(CardViewSettings settings, object dataObject, Action<MVCxCardViewExporter, Stream> write, string fileExtension, bool saveAsFile) {
			return Export(settings, dataObject, write, fileExtension, null, saveAsFile);
		}
		static ActionResult Export(CardViewSettings settings, object dataObject, Action<MVCxCardViewExporter, Stream> write, string fileExtension, string fileName, bool saveAsFile) {
			var extension = CreateExtension(settings, dataObject);
			var exporter = CreateExporterAndRaiseBeforeExport(extension);
			return ExportUtils.Export(extension, s => write(exporter, s), fileName ?? exporter.FileName, saveAsFile, fileExtension);
		}
		internal static void RaiseBeforeExportEvent(CardViewExtension extension) {
			if(extension == null || extension.Settings.SettingsExport.BeforeExport == null)
				return;
			extension.Settings.SettingsExport.BeforeExport(extension.Control, EventArgs.Empty);
		}
		#endregion
		#region Pdf Export
		static void WritePdf(MVCxCardViewExporter exporter, Stream stream) {
			exporter.WritePdf(stream);
		}
		static Action<MVCxCardViewExporter, Stream> WritePdf(DevExpress.XtraPrinting.PdfExportOptions options) {
			return (e, s) => e.WritePdf(s, options);
		}
		public static void WritePdf(CardViewSettings settings, object dataObject, Stream stream) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WritePdf(stream);
		}
		public static ActionResult ExportToPdf(CardViewSettings settings, object dataObject) {
			return Export(settings, dataObject, WritePdf, "pdf");
		}
		public static ActionResult ExportToPdf(CardViewSettings settings, object dataObject, string fileName) {
			return Export(settings, dataObject, WritePdf, "pdf", fileName);
		}
		public static ActionResult ExportToPdf(CardViewSettings settings, object dataObject, bool saveAsFile) {
			return Export(settings, dataObject, WritePdf, "pdf", saveAsFile);
		}
		public static ActionResult ExportToPdf(CardViewSettings settings, object dataObject, string fileName, bool saveAsFile) {
			return Export(settings, dataObject, WritePdf, "pdf", fileName, saveAsFile);
		}
		public static void WritePdf(CardViewSettings settings, object dataObject, Stream stream, DevExpress.XtraPrinting.PdfExportOptions exportOptions) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WritePdf(stream, exportOptions);
		}
		public static ActionResult ExportToPdf(CardViewSettings settings, object dataObject, DevExpress.XtraPrinting.PdfExportOptions exportOptions) {
			return Export(settings, dataObject, WritePdf(exportOptions), "pdf");
		}
		public static ActionResult ExportToPdf(CardViewSettings settings, object dataObject, string fileName, DevExpress.XtraPrinting.PdfExportOptions exportOptions) {
			return Export(settings, dataObject, WritePdf(exportOptions), "pdf", fileName);
		}
		public static ActionResult ExportToPdf(CardViewSettings settings, object dataObject, bool saveAsFile, DevExpress.XtraPrinting.PdfExportOptions exportOptions) {
			return Export(settings, dataObject, WritePdf(exportOptions), "pdf", saveAsFile);
		}
		public static ActionResult ExportToPdf(CardViewSettings settings, object dataObject, string fileName, bool saveAsFile, DevExpress.XtraPrinting.PdfExportOptions exportOptions) {
			return Export(settings, dataObject, WritePdf(exportOptions), "pdf", fileName, saveAsFile);
		}
		#endregion
		#region Xls Export
		static void WriteXls(MVCxCardViewExporter exporter, Stream stream) {
			exporter.WriteXls(stream);
		}
		static Action<MVCxCardViewExporter, Stream> WriteXls(DevExpress.XtraPrinting.XlsExportOptionsEx options) {
			return (e, s) => e.WriteXls(s, options);
		}
		public static void WriteXls(CardViewSettings settings, object dataObject, Stream stream) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteXls(stream);
		}
		public static ActionResult ExportToXls(CardViewSettings settings, object dataObject) {
			return Export(settings, dataObject, WriteXls, "xls");
		}
		public static ActionResult ExportToXls(CardViewSettings settings, object dataObject, string fileName) {
			return Export(settings, dataObject, WriteXls, "xls", fileName);
		}
		public static ActionResult ExportToXls(CardViewSettings settings, object dataObject, bool saveAsFile) {
			return Export(settings, dataObject, WriteXls, "xls", saveAsFile);
		}
		public static ActionResult ExportToXls(CardViewSettings settings, object dataObject, string fileName, bool saveAsFile) {
			return Export(settings, dataObject, WriteXls, "xls", fileName, saveAsFile);
		}
		public static void WriteXls(CardViewSettings settings, object dataObject, Stream stream, DevExpress.XtraPrinting.XlsExportOptionsEx exportOptions) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteXls(stream, exportOptions);
		}
		public static ActionResult ExportToXls(CardViewSettings settings, object dataObject, DevExpress.XtraPrinting.XlsExportOptionsEx exportOptions) {
			return Export(settings, dataObject, WriteXls(exportOptions), "xls");
		}
		public static ActionResult ExportToXls(CardViewSettings settings, object dataObject, string fileName, DevExpress.XtraPrinting.XlsExportOptionsEx exportOptions) {
			return Export(settings, dataObject, WriteXls(exportOptions), "xls", fileName);
		}
		public static ActionResult ExportToXls(CardViewSettings settings, object dataObject, bool saveAsFile, DevExpress.XtraPrinting.XlsExportOptionsEx exportOptions) {
			return Export(settings, dataObject, WriteXls(exportOptions), "xls", saveAsFile);
		}
		public static ActionResult ExportToXls(CardViewSettings settings, object dataObject, string fileName, bool saveAsFile, DevExpress.XtraPrinting.XlsExportOptionsEx exportOptions) {
			return Export(settings, dataObject, WriteXls(exportOptions), "xls", fileName, saveAsFile);
		}
		#endregion
		#region Xlsx Export
		static void WriteXlsx(MVCxCardViewExporter exporter, Stream stream) {
			exporter.WriteXlsx(stream);
		}
		static Action<MVCxCardViewExporter, Stream> WriteXlsx(DevExpress.XtraPrinting.XlsxExportOptionsEx options) {
			return (e, s) => e.WriteXlsx(s, options);
		}
		public static void WriteXlsx(CardViewSettings settings, object dataObject, Stream stream) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteXlsx(stream);
		}
		public static ActionResult ExportToXlsx(CardViewSettings settings, object dataObject) {
			return Export(settings, dataObject, WriteXlsx, "xlsx");
		}
		public static ActionResult ExportToXlsx(CardViewSettings settings, object dataObject, string fileName) {
			return Export(settings, dataObject, WriteXlsx, "xlsx", fileName);
		}
		public static ActionResult ExportToXlsx(CardViewSettings settings, object dataObject, bool saveAsFile) {
			return Export(settings, dataObject, WriteXlsx, "xlsx", saveAsFile);
		}
		public static ActionResult ExportToXlsx(CardViewSettings settings, object dataObject, string fileName, bool saveAsFile) {
			return Export(settings, dataObject, WriteXlsx, "xlsx", fileName, saveAsFile);
		}
		public static void WriteXlsx(CardViewSettings settings, object dataObject, Stream stream, DevExpress.XtraPrinting.XlsxExportOptionsEx exportOptions) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteXlsx(stream, exportOptions);
		}
		public static ActionResult ExportToXlsx(CardViewSettings settings, object dataObject, DevExpress.XtraPrinting.XlsxExportOptionsEx exportOptions) {
			return Export(settings, dataObject, WriteXlsx(exportOptions), "xlsx");
		}
		public static ActionResult ExportToXlsx(CardViewSettings settings, object dataObject, string fileName, DevExpress.XtraPrinting.XlsxExportOptionsEx exportOptions) {
			return Export(settings, dataObject, WriteXlsx(exportOptions), "xlsx", fileName);
		}
		public static ActionResult ExportToXlsx(CardViewSettings settings, object dataObject, bool saveAsFile, DevExpress.XtraPrinting.XlsxExportOptionsEx exportOptions) {
			return Export(settings, dataObject, WriteXlsx(exportOptions), "xlsx", saveAsFile);
		}
		public static ActionResult ExportToXlsx(CardViewSettings settings, object dataObject, string fileName, bool saveAsFile, DevExpress.XtraPrinting.XlsxExportOptionsEx exportOptions) {
			return Export(settings, dataObject, WriteXlsx(exportOptions), "xlsx", fileName, saveAsFile);
		}
		#endregion
		#region Rtf Export
		static void WriteRtf(MVCxCardViewExporter exporter, Stream stream) {
			exporter.WriteRtf(stream);
		}
		static Action<MVCxCardViewExporter, Stream> WriteRtf(DevExpress.XtraPrinting.RtfExportOptions options) {
			return (e, s) => e.WriteRtf(s, options);
		}
		public static void WriteRtf(CardViewSettings settings, object dataObject, Stream stream) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteRtf(stream);
		}
		public static ActionResult ExportToRtf(CardViewSettings settings, object dataObject) {
			return Export(settings, dataObject, WriteRtf, "rtf");
		}
		public static ActionResult ExportToRtf(CardViewSettings settings, object dataObject, string fileName) {
			return Export(settings, dataObject, WriteRtf, "rtf", fileName);
		}
		public static ActionResult ExportToRtf(CardViewSettings settings, object dataObject, bool saveAsFile) {
			return Export(settings, dataObject, WriteRtf, "rtf", saveAsFile);
		}
		public static ActionResult ExportToRtf(CardViewSettings settings, object dataObject, string fileName, bool saveAsFile) {
			return Export(settings, dataObject, WriteRtf, "rtf", fileName, saveAsFile);
		}
		public static void WriteRtf(CardViewSettings settings, object dataObject, Stream stream, DevExpress.XtraPrinting.RtfExportOptions exportOptions) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteRtf(stream, exportOptions);
		}
		public static ActionResult ExportToRtf(CardViewSettings settings, object dataObject, DevExpress.XtraPrinting.RtfExportOptions exportOptions) {
			return Export(settings, dataObject, WriteRtf(exportOptions), "rtf");
		}
		public static ActionResult ExportToRtf(CardViewSettings settings, object dataObject, string fileName, DevExpress.XtraPrinting.RtfExportOptions exportOptions) {
			return Export(settings, dataObject, WriteRtf(exportOptions), "rtf", fileName);
		}
		public static ActionResult ExportToRtf(CardViewSettings settings, object dataObject, bool saveAsFile, DevExpress.XtraPrinting.RtfExportOptions exportOptions) {
			return Export(settings, dataObject, WriteRtf(exportOptions), "rtf", saveAsFile);
		}
		public static ActionResult ExportToRtf(CardViewSettings settings, object dataObject, string fileName, bool saveAsFile, DevExpress.XtraPrinting.RtfExportOptions exportOptions) {
			return Export(settings, dataObject, WriteRtf(exportOptions), "rtf", fileName, saveAsFile);
		}
		#endregion
		#region Csv Export
		static void WriteCsv(MVCxCardViewExporter exporter, Stream stream) {
			exporter.WriteCsv(stream);
		}
		static Action<MVCxCardViewExporter, Stream> WriteCsv(DevExpress.XtraPrinting.CsvExportOptionsEx options) {
			return (e, s) => e.WriteCsv(s, options);
		}
		public static void WriteCsv(CardViewSettings settings, object dataObject, Stream stream) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteCsv(stream);
		}
		public static ActionResult ExportToCsv(CardViewSettings settings, object dataObject) {
			return Export(settings, dataObject, WriteCsv, "csv");
		}
		public static ActionResult ExportToCsv(CardViewSettings settings, object dataObject, string fileName) {
			return Export(settings, dataObject, WriteCsv, "csv", fileName);
		}
		public static ActionResult ExportToCsv(CardViewSettings settings, object dataObject, bool saveAsFile) {
			return Export(settings, dataObject, WriteCsv, "csv", saveAsFile);
		}
		public static ActionResult ExportToCsv(CardViewSettings settings, object dataObject, string fileName, bool saveAsFile) {
			return Export(settings, dataObject, WriteCsv, "csv", fileName, saveAsFile);
		}
		public static void WriteCsv(CardViewSettings settings, object dataObject, Stream stream, DevExpress.XtraPrinting.CsvExportOptionsEx exportOptions) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteCsv(stream, exportOptions);
		}
		public static ActionResult ExportToCsv(CardViewSettings settings, object dataObject, DevExpress.XtraPrinting.CsvExportOptionsEx exportOptions) {
			return Export(settings, dataObject, WriteCsv(exportOptions), "csv");
		}
		public static ActionResult ExportToCsv(CardViewSettings settings, object dataObject, string fileName, DevExpress.XtraPrinting.CsvExportOptionsEx exportOptions) {
			return Export(settings, dataObject, WriteCsv(exportOptions), "csv", fileName);
		}
		public static ActionResult ExportToCsv(CardViewSettings settings, object dataObject, bool saveAsFile, DevExpress.XtraPrinting.CsvExportOptionsEx exportOptions) {
			return Export(settings, dataObject, WriteCsv(exportOptions), "csv", saveAsFile);
		}
		public static ActionResult ExportToCsv(CardViewSettings settings, object dataObject, string fileName, bool saveAsFile, DevExpress.XtraPrinting.CsvExportOptionsEx exportOptions) {
			return Export(settings, dataObject, WriteCsv(exportOptions), "csv", fileName, saveAsFile);
		}
		#endregion
		static MVCxCardViewExporter CreateExporterAndRaiseBeforeExport(CardViewSettings settings, object dataObject) {
			return CreateExporterAndRaiseBeforeExport(CreateExtension(settings, dataObject));
		}
		static MVCxCardViewExporter CreateExporterAndRaiseBeforeExport(CardViewExtension extension) {
			var exporter = new MVCxCardViewExporter(extension.Control);
			AssignExporterSettings(exporter, extension.Settings.SettingsExport);
			RaiseBeforeExportEvent(extension);
			return exporter;
		}
		static void AssignExporterSettings(MVCxCardViewExporter exporter, MVCxCardViewExportSettings settings) {
			exporter.RenderBrick += settings.RenderBrick;
			exporter.LeftMargin = settings.LeftMargin;
			exporter.TopMargin = settings.TopMargin;
			exporter.RightMargin = settings.RightMargin;
			exporter.BottomMargin = settings.BottomMargin;
			exporter.FileName = settings.FileName;
			exporter.CardWidth = settings.CardWidth;
			exporter.ExportSelectedCardsOnly = settings.ExportSelectedCardsOnly;
			exporter.PrintSelectCheckBox = settings.PrintSelectCheckBox;
			exporter.Landscape = settings.Landscape;
			exporter.Styles.Assign(settings.Styles);
			exporter.PaperKind = settings.PaperKind;
			exporter.PaperName = settings.PaperName;
		}
		static CardViewExtension CreateExtension(CardViewSettings settings, object dataObject) {
			var extension = ExtensionsFactory.InstanceInternal.CardView(settings).Bind(dataObject);
			extension.PrepareControl();
			extension.LoadPostData();
			return extension;
		}
	}
}
