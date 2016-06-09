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
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
namespace DevExpress.Web.Mvc {
	using DevExpress.Data.Linq;
	using DevExpress.Web;
	using DevExpress.Web.Internal;
	using DevExpress.Web.Mvc.Internal;
	using DevExpress.Web.Mvc.UI;
	using DevExpress.XtraPrinting;
	public class GridViewExtension<RowType>: GridViewExtension {
		public GridViewExtension(GridViewSettings<RowType> settings)
			: base(settings) {
		}
		public GridViewExtension(GridViewSettings<RowType> settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
	}
	public class GridViewExtension : GridExtensionBase {
		public GridViewExtension(GridViewSettings settings)
			: base(settings) {
		}
		public GridViewExtension(GridViewSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal new MVCxGridView Control {
			get { return (MVCxGridView)base.Control; }
			protected set { base.Control = value; }
		}
		protected internal new GridViewSettings Settings {
			get { return (GridViewSettings)base.Settings; }
		}
		protected bool HasCommandColumn() {
			return Settings.CommandColumn.Visible && Settings.CommandColumn.VisibleIndex > -1;
		}
		protected override void AssignInitialProperties() {
			Control.Styles.CopyFrom(Settings.Styles);
			base.AssignInitialProperties();
			AssignRouteValuesToControl();
			Control.Caption = Settings.Caption;
			Control.ClientSideEvents.Assign(Settings.ClientSideEvents);
			Control.Columns.Assign(Settings.Columns);
			if(HasCommandColumn())
				Control.Columns.Insert(0, Settings.CommandColumn);
			Control.EnableRowsCache = Settings.EnableRowsCache;
			Control.GroupSummary.Assign(Settings.GroupSummary);
			Control.GroupSummarySortInfo.Assign(Settings.GroupSummarySortInfo);
			Control.Images.CopyFrom(Settings.Images);
			Control.KeyboardSupport = Settings.KeyboardSupport;
			Control.PreviewFieldName = Settings.PreviewFieldName;
			Control.Settings.Assign(Settings.Settings);
			Control.SettingsAdaptivity.Assign(Settings.SettingsAdaptivity);
			Control.SettingsContextMenu.Assign(Settings.SettingsContextMenu);
			Control.SettingsBehavior.Assign(Settings.SettingsBehavior);
			Control.SettingsCookies.Assign(Settings.SettingsCookies);
			Control.SettingsLoadingPanel.Assign(Settings.SettingsLoadingPanel);
			Control.SettingsPager.Assign(Settings.SettingsPager);
			Control.SettingsText.Assign(Settings.SettingsText);
			Control.SettingsCustomizationWindowInternal.Assign(Settings.SettingsCustomizationWindowInternal);
			Control.SettingsDetail.Assign(Settings.SettingsDetail);
			Control.SettingsEditing.Assign(Settings.SettingsEditing);
			Control.SettingsPopup.Assign(Settings.SettingsPopup);
			Control.SettingsCommandButton.Assign(Settings.SettingsCommandButton);
			Control.SettingsDataSecurity.Assign(Settings.SettingsDataSecurity);
			Control.SettingsSearchPanel.Assign(Settings.SettingsSearchPanel);
			Control.SettingsFilterControl.Assign(Settings.SettingsFilterControl);
			Control.StylesPopup.CopyFrom(Settings.StylesPopup);
			Control.StylesEditors.CopyFrom(Settings.StylesEditors);
			Control.StylesPager.CopyFrom(Settings.StylesPager);
			Control.StylesContextMenu.CopyFrom(Settings.StylesContextMenu);
			Control.SummaryText = Settings.SummaryText;
			Control.TotalSummary.Assign(Settings.TotalSummary);
			Control.FormatConditions.Assign(Settings.FormatConditions);
			Control.ImagesEditors.Assign(Settings.ImagesEditors);
			Control.EditFormLayoutProperties.Assign(Settings.EditFormLayoutProperties);
			Control.AccessibilityCompliant = Settings.AccessibilityCompliant;
			Control.InitialPageSize = Settings.SettingsPager.PageSize;
			Control.AfterPerformCallback += Settings.AfterPerformCallback;
			Control.AutoFilterCellEditorCreate += Settings.AutoFilterCellEditorCreate;
			Control.AutoFilterCellEditorInitialize += Settings.AutoFilterCellEditorInitialize;
			Control.SearchPanelEditorCreate += Settings.SearchPanelEditorCreate;
			Control.SearchPanelEditorInitialize += Settings.SearchPanelEditorInitialize;
			Control.BeforeColumnSortingGrouping += Settings.BeforeColumnSortingGrouping;
			Control.BeforeGetCallbackResult += Settings.BeforeGetCallbackResult;
			Control.BeforeHeaderFilterFillItems += Settings.BeforeHeaderFilterFillItems;
			Control.CellEditorInitialize += Settings.CellEditorInitialize;
			Control.CommandButtonInitialize += Settings.CommandButtonInitialize;
			Control.ContextMenuItemVisibility += Settings.ContextMenuItemVisibility;
			Control.CustomButtonInitialize += Settings.CustomButtonInitialize;
			Control.CustomCallback += Settings.CustomCallbackInternal;
			Control.CustomDataCallback += Settings.CustomDataCallbackInternal;
			Control.CustomJSProperties += Settings.CustomJSProperties;
			Control.CustomUnboundColumnData += Settings.CustomUnboundColumnData;
			Control.CustomSummaryCalculate += Settings.CustomSummaryCalculate;
			Control.CustomGroupDisplayText += Settings.CustomGroupDisplayText;
			Control.CustomColumnDisplayText += Settings.CustomColumnDisplayText;
			Control.CustomColumnGroup += Settings.CustomColumnGroup;
			Control.CustomColumnSort += Settings.CustomColumnSort;
			Control.DetailRowExpandedChanged += Settings.DetailRowExpandedChanged;
			Control.FillContextMenuItems += Settings.FillContextMenuItems;
			Control.HeaderFilterFillItems += Settings.HeaderFilterFillItems;
			Control.HtmlCommandCellPrepared += Settings.HtmlCommandCellPrepared;
			Control.HtmlDataCellPrepared += Settings.HtmlDataCellPrepared;
			Control.HtmlEditFormCreated += Settings.HtmlEditFormCreated;
			Control.HtmlFooterCellPrepared += Settings.HtmlFooterCellPrepared;
			Control.HtmlRowCreated += Settings.HtmlRowCreated;
			Control.HtmlRowPrepared += Settings.HtmlRowPrepared;
			Control.InitNewRow += Settings.InitNewRow;
			Control.PageIndexChanged += Settings.PageIndexChangedInternal;
			Control.ProcessColumnAutoFilter += Settings.ProcessColumnAutoFilter;
			Control.SummaryDisplayText += Settings.SummaryDisplayText;
			Control.RowValidating += Settings.RowValidating;
		}
		void AssignRouteValuesToControl() {
			Control.CallbackRouteValues = Settings.CallbackRouteValues;
			Control.CallbackActionUrlCollection.Clear();
			if(Settings.CustomActionRouteValues != null)
				Control.CallbackActionUrlCollection.Add(GridViewCallbackCommand.CustomCallback, Utils.GetUrl(Settings.CustomActionRouteValues));
			if(Settings.CustomDataActionRouteValues != null)
				Control.CallbackActionUrlCollection.Add(GridViewCallbackCommand.CustomValues, Utils.GetUrl(Settings.CustomDataActionRouteValues));
			foreach (KeyValuePair<GridViewOperationType, object> keyValuePair in Settings.CustomBindingRouteValuesCollection) {
				string[] commands = GridAdapter.GetCommandsByActionType((int)keyValuePair.Key);
				foreach (string command in commands) {
					Control.CallbackActionUrlCollection.Add(command, Utils.GetUrl(keyValuePair.Value));
				}
			}
		}
		protected override void AssignRenderProperties() {
			base.AssignRenderProperties();
			Control.Templates.Header = ContentControlTemplate<GridViewHeaderTemplateContainer>.Create(
				Settings.HeaderTemplateContent, Settings.HeaderTemplateContentMethod,
				typeof(GridViewHeaderTemplateContainer));
			Control.Templates.HeaderCaption = ContentControlTemplate<GridViewHeaderTemplateContainer>.Create(
				Settings.HeaderCaptionTemplateContent, Settings.HeaderCaptionTemplateContentMethod,
				typeof(GridViewHeaderTemplateContainer));
			Control.Templates.DataRow = ContentControlTemplate<GridViewDataRowTemplateContainer>.Create(
				Settings.DataRowTemplateContent, Settings.DataRowTemplateContentMethod,
				typeof(GridViewDataRowTemplateContainer));
			Control.Templates.DataItem = ContentControlTemplate<GridViewDataItemTemplateContainer>.Create(
				Settings.DataItemTemplateContent, Settings.DataItemTemplateContentMethod,
				typeof(GridViewDataItemTemplateContainer));
			Control.Templates.GroupRow = ContentControlTemplate<GridViewGroupRowTemplateContainer>.Create(
				Settings.GroupRowTemplateContent, Settings.GroupRowTemplateContentMethod,
				typeof(GridViewGroupRowTemplateContainer));
			Control.Templates.GroupRowContent = ContentControlTemplate<GridViewGroupRowTemplateContainer>.Create(
				Settings.GroupRowContentTemplateContent, Settings.GroupRowContentTemplateContentMethod,
				typeof(GridViewGroupRowTemplateContainer));
			Control.Templates.PreviewRow = ContentControlTemplate<GridViewPreviewRowTemplateContainer>.Create(
				Settings.PreviewRowTemplateContent, Settings.PreviewRowTemplateContentMethod,
				typeof(GridViewPreviewRowTemplateContainer));
			Control.Templates.EmptyDataRow = ContentControlTemplate<GridViewEmptyDataRowTemplateContainer>.Create(
				Settings.EmptyDataRowTemplateContent, Settings.EmptyDataRowTemplateContentMethod,
				typeof(GridViewEmptyDataRowTemplateContainer));
			Control.Templates.FilterRow = ContentControlTemplate<GridViewFilterRowTemplateContainer>.Create(
				Settings.FilterRowTemplateContent, Settings.FilterRowTemplateContentMethod,
				typeof(GridViewFilterRowTemplateContainer));
			Control.Templates.FilterCell = ContentControlTemplate<GridViewFilterCellTemplateContainer>.Create(
				Settings.FilterCellTemplateContent, Settings.FilterCellTemplateContentMethod,
				typeof(GridViewFilterCellTemplateContainer));
			Control.Templates.FooterRow = ContentControlTemplate<GridViewFooterRowTemplateContainer>.Create(
				Settings.FooterRowTemplateContent, Settings.FooterRowTemplateContentMethod,
				typeof(GridViewFooterRowTemplateContainer));
			Control.Templates.FooterCell = ContentControlTemplate<GridViewFooterCellTemplateContainer>.Create(
				Settings.FooterCellTemplateContent, Settings.FooterCellTemplateContentMethod,
				typeof(GridViewFooterCellTemplateContainer));
			Control.Templates.StatusBar = ContentControlTemplate<GridViewStatusBarTemplateContainer>.Create(
				Settings.StatusBarTemplateContent, Settings.StatusBarTemplateContentMethod,
				typeof(GridViewStatusBarTemplateContainer));
			Control.Templates.TitlePanel = ContentControlTemplate<GridViewTitleTemplateContainer>.Create(
				Settings.TitlePanelTemplateContent, Settings.TitlePanelTemplateContentMethod,
				typeof(GridViewTitleTemplateContainer));
			Control.Templates.PagerBar = ContentControlTemplate<GridViewPagerBarTemplateContainer>.Create(
				Settings.PagerBarTemplateContent, Settings.PagerBarTemplateContentMethod,
				typeof(GridViewPagerBarTemplateContainer));
			Control.Templates.DetailRow = ContentControlTemplate<GridViewDetailRowTemplateContainer>.Create(
				Settings.DetailRowTemplateContent, Settings.DetailRowTemplateContentMethod,
				typeof(GridViewDetailRowTemplateContainer));
			Control.Templates.EditForm = ContentControlTemplate<GridViewEditFormTemplateContainer>.Create(
				Settings.EditFormTemplateContent, Settings.EditFormTemplateContentMethod,
				typeof(GridViewEditFormTemplateContainer), Control.RenderHelper.UseEndlessPaging);
			for(int i = 0, j = 0; i < Control.Columns.Count; i++, j++) {
				if(Control.Columns[i] is GridViewCommandColumn) {
					Control.Columns[i].HeaderTemplate = ContentControlTemplate<GridViewHeaderTemplateContainer>.Create(
						Settings.CommandColumn.HeaderTemplateContent, Settings.CommandColumn.HeaderTemplateContentMethod,
						typeof(GridViewHeaderTemplateContainer));
					Control.Columns[i].HeaderCaptionTemplate = ContentControlTemplate<GridViewHeaderTemplateContainer>.Create(
						Settings.CommandColumn.HeaderCaptionTemplateContent, Settings.CommandColumn.HeaderCaptionTemplateContentMethod,
						typeof(GridViewHeaderTemplateContainer));
					Control.Columns[i].FooterTemplate = ContentControlTemplate<GridViewFooterCellTemplateContainer>.Create(
						Settings.CommandColumn.FooterTemplateContent, Settings.CommandColumn.FooterTemplateContentMethod,
						typeof(GridViewFooterCellTemplateContainer));
					Control.Columns[i].FilterTemplate = ContentControlTemplate<GridViewFilterCellTemplateContainer>.Create(
						Settings.CommandColumn.FilterTemplateContent, Settings.CommandColumn.FilterTemplateContentMethod,
						typeof(GridViewFilterCellTemplateContainer));
					if(Settings.Columns.Count > j && !(Settings.Columns[j] is GridViewCommandColumn))
						j--;
					continue;
				}
				GridViewColumn sourceColumn = (j < Settings.Columns.Count) ? Settings.Columns[j] : null;
				if(sourceColumn != null)
					AssignColumnTemplates(sourceColumn, Control.Columns[i]);
			}
			Control.EditFormLayoutProperties.ForEach(item => {
				MVCxGridViewColumnLayoutItem columnLayoutItem = item as MVCxGridViewColumnLayoutItem;
				if(columnLayoutItem == null)
					return;
				columnLayoutItem.Template = ContentControlTemplate<GridViewEditFormLayoutItemTemplateContainer>.Create(
					columnLayoutItem.TemplateContent, columnLayoutItem.TemplateContentMethod,
					typeof(GridViewEditFormLayoutItemTemplateContainer));
			});
		}
		protected void AssignColumnTemplates(GridViewColumn sourceColumn, GridViewColumn destinationColumn) {
			MVCxGridViewBandColumn sourceBandColumn = sourceColumn as MVCxGridViewBandColumn;
			if(sourceBandColumn != null) {
				destinationColumn.HeaderTemplate = ContentControlTemplate<GridViewHeaderTemplateContainer>.Create(
					sourceBandColumn.HeaderTemplateContent, sourceBandColumn.HeaderTemplateContentMethod,
					typeof(GridViewHeaderTemplateContainer));
				destinationColumn.HeaderCaptionTemplate = ContentControlTemplate<GridViewHeaderTemplateContainer>.Create(
					sourceBandColumn.HeaderCaptionTemplateContent, sourceBandColumn.HeaderCaptionTemplateContentMethod,
					typeof(GridViewHeaderTemplateContainer));
				GridViewBandColumn desctinationBandColumn = (GridViewBandColumn)destinationColumn;
				for(int i = 0; i < desctinationBandColumn.Columns.Count; i++) 
					AssignColumnTemplates(sourceBandColumn.Columns[i], desctinationBandColumn.Columns[i]);
			}
			MVCxGridViewColumn sourceDataColumn = sourceColumn as MVCxGridViewColumn;
			if(sourceDataColumn != null) {
				destinationColumn.HeaderTemplate = ContentControlTemplate<GridViewHeaderTemplateContainer>.Create(
					sourceDataColumn.HeaderTemplateContent, sourceDataColumn.HeaderTemplateContentMethod,
					typeof(GridViewHeaderTemplateContainer));
				destinationColumn.HeaderCaptionTemplate = ContentControlTemplate<GridViewHeaderTemplateContainer>.Create(
					sourceDataColumn.HeaderCaptionTemplateContent, sourceDataColumn.HeaderCaptionTemplateContentMethod,
					typeof(GridViewHeaderTemplateContainer));
				destinationColumn.FooterTemplate = ContentControlTemplate<GridViewFooterCellTemplateContainer>.Create(
					sourceDataColumn.FooterTemplateContent, sourceDataColumn.FooterTemplateContentMethod,
					typeof(GridViewFooterCellTemplateContainer));
				destinationColumn.FilterTemplate = ContentControlTemplate<GridViewFilterCellTemplateContainer>.Create(
					sourceDataColumn.FilterTemplateContent, sourceDataColumn.FilterTemplateContentMethod,
					typeof(GridViewFilterCellTemplateContainer));
				((GridViewDataColumn)destinationColumn).DataItemTemplate = ContentControlTemplate<GridViewDataItemTemplateContainer>.Create(
					sourceDataColumn.DataItemTemplateContent, sourceDataColumn.DataItemTemplateContentMethod,
					typeof(GridViewDataItemTemplateContainer), Control.RenderHelper.UseEndlessPaging);
				((GridViewDataColumn)destinationColumn).EditItemTemplate = ContentControlTemplate<GridViewEditItemTemplateContainer>.Create(
					sourceDataColumn.EditItemTemplateContent, sourceDataColumn.EditItemTemplateContentMethod,
					typeof(GridViewEditItemTemplateContainer), Control.RenderHelper.UseEndlessPaging);
				((GridViewDataColumn)destinationColumn).GroupRowTemplate = ContentControlTemplate<GridViewGroupRowTemplateContainer>.Create(
					sourceDataColumn.GroupRowTemplateContent, sourceDataColumn.GroupRowTemplateContentMethod,
					typeof(GridViewGroupRowTemplateContainer));
			}
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			Control.PerformOnLoad();
			FormLayoutItemHelper.ConfigureLayoutItemsByMetadata(Control.EditFormLayoutProperties);
		}
		protected override void ProcessCallback(string callbackArgument) {
			Control.EnsureChildControls();
			base.ProcessCallback(callbackArgument);
		}
		protected override Control GetCallbackResultControl() {
			if (IsExistFilterControl)
				return null; 
			return Control.GetCallbackResultControl();
		}
		protected override void RenderCallbackResult() {
			if(Control.RenderHelper.UseEndlessPaging && !IsExistFilterControl) {
				Control.EndlessPagingHelper.OnBeforeGetCallbackResult();
				if(!Control.RenderHelper.RequireEndlessPagingPartialLoad)
					Control.EnsureChildControls();
			}
			base.RenderCallbackResult();
		}
		protected override void RenderCallbackResultControl() {
			base.RenderCallbackResultControl();
			RenderString(Control.GetCallbackClientObjectScript());
			RenderString(Control.GetPostponeScript());
		}
		protected override MVCxPopupFilterControl FilterControl { get { return Control.FilterControl; } }
		public GridViewExtension Bind(object dataObject) {
			BindInternal(dataObject);
			return this;
		}
		public GridViewExtension BindToXML(string fileName) {
			return BindToXML(fileName, string.Empty, string.Empty);
		}
		public GridViewExtension BindToXML(string fileName, string xPath) {
			return BindToXML(fileName, xPath, string.Empty);
		}
		public GridViewExtension BindToXML(string fileName, string xPath, string transformFileName) {
			BindToXMLInternal(fileName, xPath, transformFileName);
			return this;
		}
		public GridViewExtension BindToLINQ(Type contextType, string tableName) {
			return BindToLINQ(contextType.FullName, tableName);
		}
		public GridViewExtension BindToLINQ(string contextTypeName, string tableName) {
			return BindToLINQ(contextTypeName, tableName, null);
		}
		public GridViewExtension BindToLINQ(Type contextType, string tableName, EventHandler<LinqServerModeDataSourceSelectEventArgs> selectingMethod) {
			return BindToLINQ(contextType.FullName, tableName, selectingMethod);
		}
		public GridViewExtension BindToLINQ(string contextTypeName, string tableName, EventHandler<LinqServerModeDataSourceSelectEventArgs> selectingMethod) {
			return BindToLINQ(contextTypeName, tableName, selectingMethod, null);
		}
		public GridViewExtension BindToLINQ(string contextTypeName, string tableName, EventHandler<LinqServerModeDataSourceSelectEventArgs> selectingMethod,
			EventHandler<DevExpress.Data.ServerModeExceptionThrownEventArgs> exceptionThrownMethod) {
			BindToLINQDataSourceInternal(contextTypeName, tableName, selectingMethod, exceptionThrownMethod);
			return this;
		}
		public GridViewExtension BindToEF(Type contextType, string tableName) {
			return BindToEF(contextType.FullName, tableName);
		}
		public GridViewExtension BindToEF(string contextTypeName, string tableName) {
			return BindToEF(contextTypeName, tableName, null);
		}
		public GridViewExtension BindToEF(Type contextType, string tableName, EventHandler<LinqServerModeDataSourceSelectEventArgs> selectingMethod) {
			return BindToEF(contextType.FullName, tableName, selectingMethod);
		}
		public GridViewExtension BindToEF(string contextTypeName, string tableName, EventHandler<LinqServerModeDataSourceSelectEventArgs> selectingMethod) {
			return BindToEF(contextTypeName, tableName, selectingMethod, null);
		}
		public GridViewExtension BindToEF(string contextTypeName, string tableName, EventHandler<LinqServerModeDataSourceSelectEventArgs> selectingMethod,
			EventHandler<DevExpress.Data.ServerModeExceptionThrownEventArgs> exceptionThrownMethod) {
			BindToEFDataSourceInternal(contextTypeName, tableName, selectingMethod, exceptionThrownMethod);
			return this;
		}
		public GridViewExtension BindToCustomData(GridViewModel viewModel) {
			Control.EnableCustomOperations = true;
			Control.CustomOperationViewModel.Assign(viewModel);
			Control.PageSize = viewModel.Pager.PageSize;
			Control.PageIndex = viewModel.Pager.PageIndex;
			Control.SettingsSearchPanel.GroupOperator = viewModel.SearchPanel.GroupOperator;
			Control.SettingsSearchPanel.ColumnNames = viewModel.SearchPanel.ColumnNames;
			Control.SearchPanelFilter = viewModel.SearchPanel.Filter;
			Control.FilterExpression = viewModel.FilterExpression;
			Control.FilterEnabled = viewModel.IsFilterApplied;
			PrepareColumnsViaViewModel(viewModel, Control.Columns.Count == 0);
			if(!string.IsNullOrEmpty(viewModel.KeyFieldName))
				Control.KeyFieldName = viewModel.KeyFieldName;
			AddSummaryFromState(Control.TotalSummary, viewModel.TotalSummary);
			AddSummaryFromState(Control.GroupSummary, viewModel.GroupSummary);
			Control.ResetFilterHelper();
			return this;
		}
		protected internal override bool IsBindInternalRequired() {
			return !IsCallback() || Control.AutoGenerateColumns && Control.Columns.Count == 0; 
		}
		void PrepareColumnsViaViewModel(GridViewModel viewModel, bool areAddNewColumn) {
			foreach(var columnState in viewModel.Columns) {
				MVCxGridViewColumn column = Control.Columns[columnState.FieldName] as MVCxGridViewColumn;
				if (areAddNewColumn && column == null) {
					column = new MVCxGridViewColumn(columnState.FieldName);
					Control.Columns.Add(column);
				}
				if (column != null)
					column.AssignState(columnState);
			}
		}
		static void AddSummaryFromState(ASPxSummaryItemCollection summaryItems, IGridSummaryItemStateCollection summaryItemStates) {
			foreach(var itemState in summaryItemStates) {
				var item = summaryItems.Where(s => s.FieldName == itemState.FieldName && s.SummaryType == itemState.SummaryType && s.Tag == itemState.Tag).SingleOrDefault();
				if(item != null)
					return;
				item = new ASPxSummaryItem();
				summaryItems.Add(item);
				item.FieldName = itemState.FieldName;
				item.SummaryType = itemState.SummaryType;
				item.Tag = itemState.Tag;
			}
		}
		public GridViewExtension SetEditErrorText(string message) {
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
		public static IPrintable CreatePrintableObject(GridViewSettings settings, object dataObject) {
			return (IPrintable)CreateExporterAndRaiseBeforeExport(settings, dataObject);
		}
		static ActionResult Export(GridViewSettings settings, object dataObject, Action<MVCxGridViewExporter, Stream> write, string fileExtension) {
			return Export(settings, dataObject, write, fileExtension, null);
		}
		static ActionResult Export(GridViewSettings settings, object dataObject, Action<MVCxGridViewExporter, Stream> write, string fileExtension, string fileName) {
			return Export(settings, dataObject, write, fileExtension, fileName, true);
		}
		static ActionResult Export(GridViewSettings settings, object dataObject, Action<MVCxGridViewExporter, Stream> write, string fileExtension, bool saveAsFile) {
			return Export(settings, dataObject, write, fileExtension, null, saveAsFile);
		}
		static ActionResult Export(GridViewSettings settings, object dataObject, Action<MVCxGridViewExporter, Stream> write, string fileExtension, string fileName, bool saveAsFile) {
			GridViewExtension extension = CreateExtension(settings, dataObject);
			MVCxGridViewExporter exporter = CreateExporterAndRaiseBeforeExport(extension);
			return ExportUtils.Export(extension, s => write(exporter, s), fileName ?? exporter.FileName, saveAsFile, fileExtension);
		}
		internal static void RaiseBeforeExportEvent(GridViewExtension extension) {
			if(extension == null || extension.Settings.SettingsExport.BeforeExport == null)
				return;
			extension.Settings.SettingsExport.BeforeExport(extension.Control, EventArgs.Empty);
		}
		#endregion
		#region Pdf Export
		static void WritePdf(MVCxGridViewExporter exporter, Stream stream) {
			exporter.WritePdf(stream);
		}
		static Action<MVCxGridViewExporter, Stream> WritePdf(DevExpress.XtraPrinting.PdfExportOptions options) {
			return (e, s) => e.WritePdf(s, options);
		}
		public static void WritePdf(GridViewSettings settings, object dataObject, Stream stream) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WritePdf(stream);
		}
		[Obsolete("This method is now obsolete. Use the ExportToPdf method instead.")]
		public static void WritePdfToResponse(GridViewSettings settings, object dataObject) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WritePdfToResponse();
		}
		[Obsolete("This method is now obsolete. Use the ExportToPdf method instead.")]
		public static void WritePdfToResponse(GridViewSettings settings, object dataObject, string fileName) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WritePdfToResponse(fileName);
		}
		[Obsolete("This method is now obsolete. Use the ExportToPdf method instead.")]
		public static void WritePdfToResponse(GridViewSettings settings, object dataObject, bool saveAsFile) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WritePdfToResponse(saveAsFile);
		}
		[Obsolete("This method is now obsolete. Use the ExportToPdf method instead.")]
		public static void WritePdfToResponse(GridViewSettings settings, object dataObject, string fileName, bool saveAsFile) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WritePdfToResponse(fileName, saveAsFile);
		}
		public static ActionResult ExportToPdf(GridViewSettings settings, object dataObject) {
			return Export(settings, dataObject, WritePdf, "pdf");
		}
		public static ActionResult ExportToPdf(GridViewSettings settings, object dataObject, string fileName) {
			return Export(settings, dataObject, WritePdf, "pdf", fileName);
		}
		public static ActionResult ExportToPdf(GridViewSettings settings, object dataObject, bool saveAsFile) {
			return Export(settings, dataObject, WritePdf, "pdf", saveAsFile);
		}
		public static ActionResult ExportToPdf(GridViewSettings settings, object dataObject, string fileName, bool saveAsFile) {
			return Export(settings, dataObject, WritePdf, "pdf", fileName, saveAsFile);
		}
		public static void WritePdf(GridViewSettings settings, object dataObject, Stream stream, DevExpress.XtraPrinting.PdfExportOptions exportOptions) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WritePdf(stream, exportOptions);
		}
		[Obsolete("This method is now obsolete. Use the ExportToPdf method instead.")]
		public static void WritePdfToResponse(GridViewSettings settings, object dataObject, DevExpress.XtraPrinting.PdfExportOptions exportOptions) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WritePdfToResponse(exportOptions);
		}
		[Obsolete("This method is now obsolete. Use the ExportToPdf method instead.")]
		public static void WritePdfToResponse(GridViewSettings settings, object dataObject, string fileName, DevExpress.XtraPrinting.PdfExportOptions exportOptions) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WritePdfToResponse(fileName, exportOptions);
		}
		[Obsolete("This method is now obsolete. Use the ExportToPdf method instead.")]
		public static void WritePdfToResponse(GridViewSettings settings, object dataObject, bool saveAsFile, DevExpress.XtraPrinting.PdfExportOptions exportOptions) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WritePdfToResponse(saveAsFile, exportOptions);
		}
		[Obsolete("This method is now obsolete. Use the ExportToPdf method instead.")]
		public static void WritePdfToResponse(GridViewSettings settings, object dataObject, string fileName, bool saveAsFile, DevExpress.XtraPrinting.PdfExportOptions exportOptions) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WritePdfToResponse(fileName, saveAsFile, exportOptions);
		}
		public static ActionResult ExportToPdf(GridViewSettings settings, object dataObject, DevExpress.XtraPrinting.PdfExportOptions exportOptions) {
			return Export(settings, dataObject, WritePdf(exportOptions), "pdf");
		}
		public static ActionResult ExportToPdf(GridViewSettings settings, object dataObject, string fileName, DevExpress.XtraPrinting.PdfExportOptions exportOptions) {
			return Export(settings, dataObject, WritePdf(exportOptions), "pdf", fileName);
		}
		public static ActionResult ExportToPdf(GridViewSettings settings, object dataObject, bool saveAsFile, DevExpress.XtraPrinting.PdfExportOptions exportOptions) {
			return Export(settings, dataObject, WritePdf(exportOptions), "pdf", saveAsFile);
		}
		public static ActionResult ExportToPdf(GridViewSettings settings, object dataObject, string fileName, bool saveAsFile, DevExpress.XtraPrinting.PdfExportOptions exportOptions) {
			return Export(settings, dataObject, WritePdf(exportOptions), "pdf", fileName, saveAsFile);
		}
		#endregion
		#region Xls Export
		static void WriteXls(MVCxGridViewExporter exporter, Stream stream) {
			exporter.WriteXls(stream);
		}
		static Action<MVCxGridViewExporter, Stream> WriteXls(DevExpress.XtraPrinting.XlsExportOptionsEx options) {
			return (e, s) => e.WriteXls(s, options);
		}
		[Obsolete("Use another overload of this method instead.")]
		static Action<MVCxGridViewExporter, Stream> WriteXls(DevExpress.XtraPrinting.XlsExportOptions options) {
			return (e, s) => e.WriteXls(s, options);
		}
		public static void WriteXls(GridViewSettings settings, object dataObject, Stream stream) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteXls(stream);
		}
		[Obsolete("This method is now obsolete. Use the ExportToXls method instead.")]
		public static void WriteXlsToResponse(GridViewSettings settings, object dataObject) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteXlsToResponse();
		}
		[Obsolete("This method is now obsolete. Use the ExportToXls method instead.")]
		public static void WriteXlsToResponse(GridViewSettings settings, object dataObject, string fileName) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteXlsToResponse(fileName);
		}
		[Obsolete("This method is now obsolete. Use the ExportToXls method instead.")]
		public static void WriteXlsToResponse(GridViewSettings settings, object dataObject, bool saveAsFile) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteXlsToResponse(saveAsFile);
		}
		[Obsolete("This method is now obsolete. Use the ExportToXls method instead.")]
		public static void WriteXlsToResponse(GridViewSettings settings, object dataObject, string fileName, bool saveAsFile) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteXlsToResponse(fileName, saveAsFile);
		}
		public static ActionResult ExportToXls(GridViewSettings settings, object dataObject) {
			return Export(settings, dataObject, WriteXls, "xls");
		}
		public static ActionResult ExportToXls(GridViewSettings settings, object dataObject, string fileName) {
			return Export(settings, dataObject, WriteXls, "xls", fileName);
		}
		public static ActionResult ExportToXls(GridViewSettings settings, object dataObject, bool saveAsFile) {
			return Export(settings, dataObject, WriteXls, "xls", saveAsFile);
		}
		public static ActionResult ExportToXls(GridViewSettings settings, object dataObject, string fileName, bool saveAsFile) {
			return Export(settings, dataObject, WriteXls, "xls", fileName, saveAsFile);
		}
		public static void WriteXls(GridViewSettings settings, object dataObject, Stream stream, DevExpress.XtraPrinting.XlsExportOptionsEx exportOptions) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteXls(stream, exportOptions);
		}
		[Obsolete("Use another overload of this method instead.")]
		public static void WriteXls(GridViewSettings settings, object dataObject, Stream stream, DevExpress.XtraPrinting.XlsExportOptions exportOptions) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteXls(stream, exportOptions);
		}
		[Obsolete("This method is now obsolete. Use the ExportToXls method instead.")]
		public static void WriteXlsToResponse(GridViewSettings settings, object dataObject, DevExpress.XtraPrinting.XlsExportOptions exportOptions) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteXlsToResponse(exportOptions);
		}
		[Obsolete("This method is now obsolete. Use the ExportToXls method instead.")]
		public static void WriteXlsToResponse(GridViewSettings settings, object dataObject, string fileName, DevExpress.XtraPrinting.XlsExportOptions exportOptions) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteXlsToResponse(fileName, exportOptions);
		}
		[Obsolete("This method is now obsolete. Use the ExportToXls method instead.")]
		public static void WriteXlsToResponse(GridViewSettings settings, object dataObject, bool saveAsFile, DevExpress.XtraPrinting.XlsExportOptions exportOptions) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteXlsToResponse(saveAsFile, exportOptions);
		}
		[Obsolete("This method is now obsolete. Use the ExportToXls method instead.")]
		public static void WriteXlsToResponse(GridViewSettings settings, object dataObject, string fileName, bool saveAsFile, DevExpress.XtraPrinting.XlsExportOptions exportOptions) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteXlsToResponse(fileName, saveAsFile, exportOptions);
		}
		public static ActionResult ExportToXls(GridViewSettings settings, object dataObject, DevExpress.XtraPrinting.XlsExportOptionsEx exportOptions) {
			return Export(settings, dataObject, WriteXls(exportOptions), "xls");
		}
		public static ActionResult ExportToXls(GridViewSettings settings, object dataObject, string fileName, DevExpress.XtraPrinting.XlsExportOptionsEx exportOptions) {
			return Export(settings, dataObject, WriteXls(exportOptions), "xls", fileName);
		}
		public static ActionResult ExportToXls(GridViewSettings settings, object dataObject, bool saveAsFile, DevExpress.XtraPrinting.XlsExportOptionsEx exportOptions) {
			return Export(settings, dataObject, WriteXls(exportOptions), "xls", saveAsFile);
		}
		public static ActionResult ExportToXls(GridViewSettings settings, object dataObject, string fileName, bool saveAsFile, DevExpress.XtraPrinting.XlsExportOptionsEx exportOptions) {
			return Export(settings, dataObject, WriteXls(exportOptions), "xls", fileName, saveAsFile);
		}
		[Obsolete("Use another overload of this method instead.")]
		public static ActionResult ExportToXls(GridViewSettings settings, object dataObject, DevExpress.XtraPrinting.XlsExportOptions exportOptions) {
			return Export(settings, dataObject, WriteXls(exportOptions), "xls");
		}
		[Obsolete("Use another overload of this method instead.")]
		public static ActionResult ExportToXls(GridViewSettings settings, object dataObject, string fileName, DevExpress.XtraPrinting.XlsExportOptions exportOptions) {
			return Export(settings, dataObject, WriteXls(exportOptions), "xls", fileName);
		}
		[Obsolete("Use another overload of this method instead.")]
		public static ActionResult ExportToXls(GridViewSettings settings, object dataObject, bool saveAsFile, DevExpress.XtraPrinting.XlsExportOptions exportOptions) {
			return Export(settings, dataObject, WriteXls(exportOptions), "xls", saveAsFile);
		}
		[Obsolete("Use another overload of this method instead.")]
		public static ActionResult ExportToXls(GridViewSettings settings, object dataObject, string fileName, bool saveAsFile, DevExpress.XtraPrinting.XlsExportOptions exportOptions) {
			return Export(settings, dataObject, WriteXls(exportOptions), "xls", fileName, saveAsFile);
		}
		#endregion
		#region Xlsx Export
		static void WriteXlsx(MVCxGridViewExporter exporter, Stream stream) {
			exporter.WriteXlsx(stream);
		}
		static Action<MVCxGridViewExporter, Stream> WriteXlsx(DevExpress.XtraPrinting.XlsxExportOptionsEx options) {
			return (e, s) => e.WriteXlsx(s, options);
		}
		[Obsolete("Use another overload of this method instead.")]
		static Action<MVCxGridViewExporter, Stream> WriteXlsx(DevExpress.XtraPrinting.XlsxExportOptions options) {
			return (e, s) => e.WriteXlsx(s, options);
		}
		public static void WriteXlsx(GridViewSettings settings, object dataObject, Stream stream) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteXlsx(stream);
		}
		[Obsolete("This method is now obsolete. Use the ExportToXlsx method instead.")]
		public static void WriteXlsxToResponse(GridViewSettings settings, object dataObject) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteXlsxToResponse();
		}
		[Obsolete("This method is now obsolete. Use the ExportToXlsx method instead.")]
		public static void WriteXlsxToResponse(GridViewSettings settings, object dataObject, string fileName) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteXlsxToResponse(fileName);
		}
		[Obsolete("This method is now obsolete. Use the ExportToXlsx method instead.")]
		public static void WriteXlsxToResponse(GridViewSettings settings, object dataObject, bool saveAsFile) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteXlsxToResponse(saveAsFile);
		}
		[Obsolete("This method is now obsolete. Use the ExportToXlsx method instead.")]
		public static void WriteXlsxToResponse(GridViewSettings settings, object dataObject, string fileName, bool saveAsFile) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteXlsxToResponse(fileName, saveAsFile);
		}
		public static ActionResult ExportToXlsx(GridViewSettings settings, object dataObject) {
			return Export(settings, dataObject, WriteXlsx, "xlsx");
		}
		public static ActionResult ExportToXlsx(GridViewSettings settings, object dataObject, string fileName) {
			return Export(settings, dataObject, WriteXlsx, "xlsx", fileName);
		}
		public static ActionResult ExportToXlsx(GridViewSettings settings, object dataObject, bool saveAsFile) {
			return Export(settings, dataObject, WriteXlsx, "xlsx", saveAsFile);
		}
		public static ActionResult ExportToXlsx(GridViewSettings settings, object dataObject, string fileName, bool saveAsFile) {
			return Export(settings, dataObject, WriteXlsx, "xlsx", fileName, saveAsFile);
		}
		public static void WriteXlsx(GridViewSettings settings, object dataObject, Stream stream, DevExpress.XtraPrinting.XlsxExportOptionsEx exportOptions) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteXlsx(stream, exportOptions);
		}
		[Obsolete("Use another overload of this method instead.")]
		public static void WriteXlsx(GridViewSettings settings, object dataObject, Stream stream, DevExpress.XtraPrinting.XlsxExportOptions exportOptions) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteXlsx(stream, exportOptions);
		}
		[Obsolete("This method is now obsolete. Use the ExportToXlsx method instead.")]
		public static void WriteXlsxToResponse(GridViewSettings settings, object dataObject, DevExpress.XtraPrinting.XlsxExportOptions exportOptions) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteXlsxToResponse(exportOptions);
		}
		[Obsolete("This method is now obsolete. Use the ExportToXlsx method instead.")]
		public static void WriteXlsxToResponse(GridViewSettings settings, object dataObject, string fileName, DevExpress.XtraPrinting.XlsxExportOptions exportOptions) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteXlsxToResponse(fileName, exportOptions);
		}
		[Obsolete("This method is now obsolete. Use the ExportToXlsx method instead.")]
		public static void WriteXlsxToResponse(GridViewSettings settings, object dataObject, bool saveAsFile, DevExpress.XtraPrinting.XlsxExportOptions exportOptions) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteXlsxToResponse(saveAsFile, exportOptions);
		}
		[Obsolete("This method is now obsolete. Use the ExportToXlsx method instead.")]
		public static void WriteXlsxToResponse(GridViewSettings settings, object dataObject, string fileName, bool saveAsFile, DevExpress.XtraPrinting.XlsxExportOptions exportOptions) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteXlsxToResponse(fileName, saveAsFile, exportOptions);
		}
		public static ActionResult ExportToXlsx(GridViewSettings settings, object dataObject, DevExpress.XtraPrinting.XlsxExportOptionsEx exportOptions) {
			return Export(settings, dataObject, WriteXlsx(exportOptions), "xlsx");
		}
		public static ActionResult ExportToXlsx(GridViewSettings settings, object dataObject, string fileName, DevExpress.XtraPrinting.XlsxExportOptionsEx exportOptions) {
			return Export(settings, dataObject, WriteXlsx(exportOptions), "xlsx", fileName);
		}
		public static ActionResult ExportToXlsx(GridViewSettings settings, object dataObject, bool saveAsFile, DevExpress.XtraPrinting.XlsxExportOptionsEx exportOptions) {
			return Export(settings, dataObject, WriteXlsx(exportOptions), "xlsx", saveAsFile);
		}
		public static ActionResult ExportToXlsx(GridViewSettings settings, object dataObject, string fileName, bool saveAsFile, DevExpress.XtraPrinting.XlsxExportOptionsEx exportOptions) {
			return Export(settings, dataObject, WriteXlsx(exportOptions), "xlsx", fileName, saveAsFile);
		}
		[Obsolete("Use another overload of this method instead.")]
		public static ActionResult ExportToXlsx(GridViewSettings settings, object dataObject, DevExpress.XtraPrinting.XlsxExportOptions exportOptions) {
			return Export(settings, dataObject, WriteXlsx(exportOptions), "xlsx");
		}
		[Obsolete("Use another overload of this method instead.")]
		public static ActionResult ExportToXlsx(GridViewSettings settings, object dataObject, string fileName, DevExpress.XtraPrinting.XlsxExportOptions exportOptions) {
			return Export(settings, dataObject, WriteXlsx(exportOptions), "xlsx", fileName);
		}
		[Obsolete("Use another overload of this method instead.")]
		public static ActionResult ExportToXlsx(GridViewSettings settings, object dataObject, bool saveAsFile, DevExpress.XtraPrinting.XlsxExportOptions exportOptions) {
			return Export(settings, dataObject, WriteXlsx(exportOptions), "xlsx", saveAsFile);
		}
		[Obsolete("Use another overload of this method instead.")]
		public static ActionResult ExportToXlsx(GridViewSettings settings, object dataObject, string fileName, bool saveAsFile, DevExpress.XtraPrinting.XlsxExportOptions exportOptions) {
			return Export(settings, dataObject, WriteXlsx(exportOptions), "xlsx", fileName, saveAsFile);
		}
		#endregion
		#region Rtf Export
		static void WriteRtf(MVCxGridViewExporter exporter, Stream stream) {
			exporter.WriteRtf(stream);
		}
		static Action<MVCxGridViewExporter, Stream> WriteRtf(DevExpress.XtraPrinting.RtfExportOptions options) {
			return (e, s) => e.WriteRtf(s, options);
		}
		public static void WriteRtf(GridViewSettings settings, object dataObject, Stream stream) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteRtf(stream);
		}
		[Obsolete("This method is now obsolete. Use the ExportToRtf method instead.")]
		public static void WriteRtfToResponse(GridViewSettings settings, object dataObject) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteRtfToResponse();
		}
		[Obsolete("This method is now obsolete. Use the ExportToRtf method instead.")]
		public static void WriteRtfToResponse(GridViewSettings settings, object dataObject, string fileName) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteRtfToResponse(fileName);
		}
		[Obsolete("This method is now obsolete. Use the ExportToRtf method instead.")]
		public static void WriteRtfToResponse(GridViewSettings settings, object dataObject, bool saveAsFile) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteRtfToResponse(saveAsFile);
		}
		[Obsolete("This method is now obsolete. Use the ExportToRtf method instead.")]
		public static void WriteRtfToResponse(GridViewSettings settings, object dataObject, string fileName, bool saveAsFile) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteRtfToResponse(fileName, saveAsFile);
		}
		public static ActionResult ExportToRtf(GridViewSettings settings, object dataObject) {
			return Export(settings, dataObject, WriteRtf, "rtf");
		}
		public static ActionResult ExportToRtf(GridViewSettings settings, object dataObject, string fileName) {
			return Export(settings, dataObject, WriteRtf, "rtf", fileName);
		}
		public static ActionResult ExportToRtf(GridViewSettings settings, object dataObject, bool saveAsFile) {
			return Export(settings, dataObject, WriteRtf, "rtf", saveAsFile);
		}
		public static ActionResult ExportToRtf(GridViewSettings settings, object dataObject, string fileName, bool saveAsFile) {
			return Export(settings, dataObject, WriteRtf, "rtf", fileName, saveAsFile);
		}
		public static void WriteRtf(GridViewSettings settings, object dataObject, Stream stream, DevExpress.XtraPrinting.RtfExportOptions exportOptions) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteRtf(stream, exportOptions);
		}
		[Obsolete("This method is now obsolete. Use the ExportToRtf method instead.")]
		public static void WriteRtfToResponse(GridViewSettings settings, object dataObject, DevExpress.XtraPrinting.RtfExportOptions exportOptions) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteRtfToResponse(exportOptions);
		}
		[Obsolete("This method is now obsolete. Use the ExportToRtf method instead.")]
		public static void WriteRtfToResponse(GridViewSettings settings, object dataObject, string fileName, DevExpress.XtraPrinting.RtfExportOptions exportOptions) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteRtfToResponse(fileName, exportOptions);
		}
		[Obsolete("This method is now obsolete. Use the ExportToRtf method instead.")]
		public static void WriteRtfToResponse(GridViewSettings settings, object dataObject, bool saveAsFile, DevExpress.XtraPrinting.RtfExportOptions exportOptions) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteRtfToResponse(saveAsFile, exportOptions);
		}
		[Obsolete("This method is now obsolete. Use the ExportToRtf method instead.")]
		public static void WriteRtfToResponse(GridViewSettings settings, object dataObject, string fileName, bool saveAsFile, DevExpress.XtraPrinting.RtfExportOptions exportOptions) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteRtfToResponse(fileName, saveAsFile, exportOptions);
		}
		public static ActionResult ExportToRtf(GridViewSettings settings, object dataObject, DevExpress.XtraPrinting.RtfExportOptions exportOptions) {
			return Export(settings, dataObject, WriteRtf(exportOptions), "rtf");
		}
		public static ActionResult ExportToRtf(GridViewSettings settings, object dataObject, string fileName, DevExpress.XtraPrinting.RtfExportOptions exportOptions) {
			return Export(settings, dataObject, WriteRtf(exportOptions), "rtf", fileName);
		}
		public static ActionResult ExportToRtf(GridViewSettings settings, object dataObject, bool saveAsFile, DevExpress.XtraPrinting.RtfExportOptions exportOptions) {
			return Export(settings, dataObject, WriteRtf(exportOptions), "rtf", saveAsFile);
		}
		public static ActionResult ExportToRtf(GridViewSettings settings, object dataObject, string fileName, bool saveAsFile, DevExpress.XtraPrinting.RtfExportOptions exportOptions) {
			return Export(settings, dataObject, WriteRtf(exportOptions), "rtf", fileName, saveAsFile);
		}
		#endregion
		#region Csv Export
		static void WriteCsv(MVCxGridViewExporter exporter, Stream stream) {
			exporter.WriteCsv(stream);
		}
		static Action<MVCxGridViewExporter, Stream> WriteCsv(DevExpress.XtraPrinting.CsvExportOptionsEx options) {
			return (e, s) => e.WriteCsv(s, options);
		}
		[Obsolete("Use another overload of this method instead.")]
		static Action<MVCxGridViewExporter, Stream> WriteCsv(DevExpress.XtraPrinting.CsvExportOptions options) {
			return (e, s) => e.WriteCsv(s, options);
		}
		public static void WriteCsv(GridViewSettings settings, object dataObject, Stream stream) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteCsv(stream);
		}
		[Obsolete("This method is now obsolete. Use the ExportToCsv method instead.")]
		public static void WriteCsvToResponse(GridViewSettings settings, object dataObject) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteCsvToResponse();
		}
		[Obsolete("This method is now obsolete. Use the ExportToCsv method instead.")]
		public static void WriteCsvToResponse(GridViewSettings settings, object dataObject, string fileName) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteCsvToResponse(fileName);
		}
		[Obsolete("This method is now obsolete. Use the ExportToCsv method instead.")]
		public static void WriteCsvToResponse(GridViewSettings settings, object dataObject, bool saveAsFile) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteCsvToResponse(saveAsFile);
		}
		[Obsolete("This method is now obsolete. Use the ExportToCsv method instead.")]
		public static void WriteCsvToResponse(GridViewSettings settings, object dataObject, string fileName, bool saveAsFile) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteCsvToResponse(fileName, saveAsFile);
		}
		public static ActionResult ExportToCsv(GridViewSettings settings, object dataObject) {
			return Export(settings, dataObject, WriteCsv, "csv");
		}
		public static ActionResult ExportToCsv(GridViewSettings settings, object dataObject, string fileName) {
			return Export(settings, dataObject, WriteCsv, "csv", fileName);
		}
		public static ActionResult ExportToCsv(GridViewSettings settings, object dataObject, bool saveAsFile) {
			return Export(settings, dataObject, WriteCsv, "csv", saveAsFile);
		}
		public static ActionResult ExportToCsv(GridViewSettings settings, object dataObject, string fileName, bool saveAsFile) {
			return Export(settings, dataObject, WriteCsv, "csv", fileName, saveAsFile);
		}
		public static void WriteCsv(GridViewSettings settings, object dataObject, Stream stream, DevExpress.XtraPrinting.CsvExportOptionsEx exportOptions) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteCsv(stream, exportOptions);
		}
		[Obsolete("Use another overload of this method instead.")]
		public static void WriteCsv(GridViewSettings settings, object dataObject, Stream stream, DevExpress.XtraPrinting.CsvExportOptions exportOptions) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteCsv(stream, exportOptions);
		}
		[Obsolete("This method is now obsolete. Use the ExportToCsv method instead.")]
		public static void WriteCsvToResponse(GridViewSettings settings, object dataObject, DevExpress.XtraPrinting.CsvExportOptions exportOptions) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteCsvToResponse(exportOptions);
		}
		[Obsolete("This method is now obsolete. Use the ExportToCsv method instead.")]
		public static void WriteCsvToResponse(GridViewSettings settings, object dataObject, string fileName, DevExpress.XtraPrinting.CsvExportOptions exportOptions) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteCsvToResponse(fileName, exportOptions);
		}
		[Obsolete("This method is now obsolete. Use the ExportToCsv method instead.")]
		public static void WriteCsvToResponse(GridViewSettings settings, object dataObject, bool saveAsFile, DevExpress.XtraPrinting.CsvExportOptions exportOptions) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteCsvToResponse(saveAsFile, exportOptions);
		}
		[Obsolete("This method is now obsolete. Use the ExportToCsv method instead.")]
		public static void WriteCsvToResponse(GridViewSettings settings, object dataObject, string fileName, bool saveAsFile, DevExpress.XtraPrinting.CsvExportOptions exportOptions) {
			CreateExporterAndRaiseBeforeExport(settings, dataObject).WriteCsvToResponse(fileName, saveAsFile, exportOptions);
		}
		public static ActionResult ExportToCsv(GridViewSettings settings, object dataObject, DevExpress.XtraPrinting.CsvExportOptionsEx exportOptions) {
			return Export(settings, dataObject, WriteCsv(exportOptions), "csv");
		}
		public static ActionResult ExportToCsv(GridViewSettings settings, object dataObject, string fileName, DevExpress.XtraPrinting.CsvExportOptionsEx exportOptions) {
			return Export(settings, dataObject, WriteCsv(exportOptions), "csv", fileName);
		}
		public static ActionResult ExportToCsv(GridViewSettings settings, object dataObject, bool saveAsFile, DevExpress.XtraPrinting.CsvExportOptionsEx exportOptions) {
			return Export(settings, dataObject, WriteCsv(exportOptions), "csv", saveAsFile);
		}
		public static ActionResult ExportToCsv(GridViewSettings settings, object dataObject, string fileName, bool saveAsFile, DevExpress.XtraPrinting.CsvExportOptionsEx exportOptions) {
			return Export(settings, dataObject, WriteCsv(exportOptions), "csv", fileName, saveAsFile);
		}
		[Obsolete("Use another overload of this method instead.")]
		public static ActionResult ExportToCsv(GridViewSettings settings, object dataObject, DevExpress.XtraPrinting.CsvExportOptions exportOptions) {
			return Export(settings, dataObject, WriteCsv(exportOptions), "csv");
		}
		[Obsolete("Use another overload of this method instead.")]
		public static ActionResult ExportToCsv(GridViewSettings settings, object dataObject, string fileName, DevExpress.XtraPrinting.CsvExportOptions exportOptions) {
			return Export(settings, dataObject, WriteCsv(exportOptions), "csv", fileName);
		}
		[Obsolete("Use another overload of this method instead.")]
		public static ActionResult ExportToCsv(GridViewSettings settings, object dataObject, bool saveAsFile, DevExpress.XtraPrinting.CsvExportOptions exportOptions) {
			return Export(settings, dataObject, WriteCsv(exportOptions), "csv", saveAsFile);
		}
		[Obsolete("Use another overload of this method instead.")]
		public static ActionResult ExportToCsv(GridViewSettings settings, object dataObject, string fileName, bool saveAsFile, DevExpress.XtraPrinting.CsvExportOptions exportOptions) {
			return Export(settings, dataObject, WriteCsv(exportOptions), "csv", fileName, saveAsFile);
		}
		#endregion
		static GridViewExtension CreateExtension(GridViewSettings settings, object dataObject) {
			GridViewExtension extension = ExtensionsFactory.InstanceInternal.GridView(settings).Bind(dataObject);
			extension.PrepareControl();
			extension.LoadPostData();
			return extension;
		}
		static MVCxGridViewExporter CreateExporterAndRaiseBeforeExport(GridViewExtension extension) {
			MVCxGridViewExporter exporter = new MVCxGridViewExporter(extension.Control);
			AssignExporterSettings(exporter, extension.Settings.SettingsExport);
			RaiseBeforeExportEvent(extension);
			return exporter;
		}
		static MVCxGridViewExporter CreateExporterAndRaiseBeforeExport(GridViewSettings settings, object dataObject) {
			return CreateExporterAndRaiseBeforeExport(CreateExtension(settings, dataObject));
		}
		static void AssignExporterSettings(MVCxGridViewExporter exporter, MVCxGridViewExportSettings settings) {
			string gridID = exporter.GridView.ClientID;
			exporter.GetExportDetailGridViewsHandlerHash[gridID] = settings.GetExportDetailGridViews;
			exporter.RenderBrick += settings.RenderBrick;
			exporter.FileName = settings.FileName;
			exporter.MaxColumnWidth = settings.MaxColumnWidth;
			exporter.PrintSelectCheckBox = settings.PrintSelectCheckBox;
			exporter.PreserveGroupRowStates = settings.PreserveGroupRowStates;
			exporter.ExportedRowType = settings.ExportedRowType;
			exporter.ExportSelectedRowsOnly = settings.ExportSelectedRowsOnly;
			exporter.BottomMargin = settings.BottomMargin;
			exporter.TopMargin = settings.TopMargin;
			exporter.LeftMargin = settings.LeftMargin;
			exporter.RightMargin = settings.RightMargin;
			exporter.Landscape = settings.Landscape;
			exporter.Styles.Assign(settings.Styles);
			AssignExporterHeaderFooter(exporter.PageHeader, settings.PageHeader);
			AssignExporterHeaderFooter(exporter.PageFooter, settings.PageFooter);
			exporter.ReportHeader = settings.ReportHeader;
			exporter.ReportFooter = settings.ReportFooter;
			exporter.DetailVerticalOffset = settings.DetailVerticalOffset;
			exporter.DetailHorizontalOffset = settings.DetailHorizontalOffset;
			exporter.ExportEmptyDetailGrid = settings.ExportEmptyDetailGrid;
			exporter.PaperKind = settings.PaperKind;
			exporter.PaperName = settings.PaperName;
		}
		static void AssignExporterHeaderFooter(GridViewExporterHeaderFooter exporterHeaderFooter, GridViewExporterHeaderFooter settingsHeaderFooter) {
			exporterHeaderFooter.Center = settingsHeaderFooter.Center;
			exporterHeaderFooter.Font.CopyFrom(settingsHeaderFooter.Font);
			exporterHeaderFooter.Left = settingsHeaderFooter.Left;
			exporterHeaderFooter.Right = settingsHeaderFooter.Right;
			exporterHeaderFooter.VerticalAlignment = settingsHeaderFooter.VerticalAlignment;
		}
		protected override DevExpress.Web.ASPxWebControl CreateControl() {
			return new MVCxGridView(ViewContext);
		}
		protected override void PreRender() {
			if(IsMasterGridCallback())
				LoadPostData();
			base.PreRender();
		}
		bool IsMasterGridCallback() {
			return !string.IsNullOrEmpty(MvcUtils.CallbackName) && MvcUtils.CallbackName == Control.SettingsDetail.MasterGridName;
		}
		public static GridViewModel GetViewModel(string name) {
			return GridViewModel.Load(name);
		}
		public new static ContentResult GetCustomDataCallbackResult(object data) {
			if(GridViewCustomOperationCallbackHelper.FunctionalCallbackCommandName != GridViewCallbackCommand.CustomValues) {
				throw new InvalidOperationException(
					"A client MVCxClientGridView.GetValuesOnCustomCallback function must be called in order to execute the GetCustomDataCallbackResult server method.");	 
			}
			GridCallbackArgumentsReader argumentsReader = new GridCallbackArgumentsReader(ProcessedCallbackArgument);
			string result = string.Join("|", new string[] { "FB", argumentsReader.InternalCallbackIndex.ToString(), HtmlConvertor.ToJSON(data) });
			return ExtensionBase.GetCustomDataCallbackResult(result);
		}
	}
}
