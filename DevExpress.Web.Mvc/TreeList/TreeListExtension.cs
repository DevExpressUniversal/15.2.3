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
using DevExpress.Web.ASPxTreeList;
using DevExpress.Web.ASPxTreeList.Internal;
using DevExpress.Web.Mvc.Internal;
using DevExpress.Web.Mvc.UI;
using DevExpress.XtraPrinting;
namespace DevExpress.Web.Mvc {
	public delegate void TreeListVirtualModeCreateChildrenMethod(TreeListVirtualModeCreateChildrenEventArgs args);
	public delegate void TreeListVirtualModeNodeCreatingMethod(TreeListVirtualModeNodeCreatingEventArgs args);
	public class TreeListExtension<RowType>: TreeListExtension {
		public TreeListExtension(TreeListSettings<RowType> settings)
			: base(settings) {
		}
		public TreeListExtension(TreeListSettings<RowType> settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
	}
	public class TreeListExtension : ExtensionBase {
		const char CommandSeparator = ' ';
		public TreeListExtension(TreeListSettings settings)
			: base(settings) {
		}
		public TreeListExtension(TreeListSettings settings, ViewContext viewContext)
			: base(settings, viewContext) {
		}
		protected internal new MVCxTreeList Control {
			get { return (MVCxTreeList)base.Control; }
		}
		protected internal new TreeListSettings Settings {
			get { return (TreeListSettings)base.Settings; }
		}
		protected bool HasCommandColumn() {
			return Settings.CommandColumn.Visible && Settings.CommandColumn.VisibleIndex > -1;
		}
		protected bool HasNoCustomizedColumns() {
			return Settings.Columns.IsEmpty;
		}
		protected override void AssignInitialProperties() {
			base.AssignInitialProperties();
			Control.CallbackRouteValues = Settings.CallbackRouteValues;
			Control.CustomActionRouteValues = Settings.CustomActionRouteValues;
			Control.CustomDataActionRouteValues = Settings.CustomDataActionRouteValues;
			Control.AutoGenerateColumns = Settings.AutoGenerateColumns;
			Control.AutoGenerateServiceColumns = Settings.AutoGenerateServiceColumns;
			Control.Caption = Settings.Caption;
			Control.ClientSideEvents.Assign(Settings.ClientSideEvents);
			Control.ClientVisible = Settings.ClientVisible;
			if(!HasNoCustomizedColumns())
				Control.Columns.Assign(Settings.Columns);
			if(HasCommandColumn()) {
				Control.Columns.Add(Settings.CommandColumn);
			}
			Control.DataCacheMode = Settings.DataCacheMode;
			Control.EnableCallbackAnimation = Settings.EnableCallbackAnimation;
			Control.EnablePagingCallbackAnimation = Settings.EnablePagingCallbackAnimation;
			Control.EnablePagingGestures = Settings.EnablePagingGestures;
			Control.Images.CopyFrom(Settings.Images);
			Control.ImagesEditors.Assign(Settings.ImagesEditors);
			Control.KeyboardSupport = Settings.KeyboardSupport;
			Control.KeyFieldName = Settings.KeyFieldName;
			Control.ParentFieldName = Settings.ParentFieldName;
			Control.PreviewFieldName = Settings.PreviewFieldName;
			if(Settings.RootValue != null)
				Control.RootValue = Settings.RootValue;
			Control.Settings.Assign(Settings.Settings);
			Control.SettingsBehavior.Assign(Settings.SettingsBehavior);
			Control.SettingsCookies.Assign(Settings.SettingsCookies);
			Control.SettingsCustomizationWindow.Assign(Settings.SettingsCustomizationWindow);
			Control.SettingsEditing.Assign(Settings.SettingsEditing);
			Control.SettingsLoadingPanel.Assign(Settings.SettingsLoadingPanel);
			Control.SettingsPager.Assign(Settings.SettingsPager);
			Control.SettingsPopupEditForm.Assign(Settings.SettingsPopupEditForm);
			Control.SettingsDataSecurity.Assign(Settings.SettingsDataSecurity);
			Control.SettingsSelection.Assign(Settings.SettingsSelection);
			Control.SettingsText.Assign(Settings.SettingsText);
			Control.Styles.CopyFrom(Settings.Styles);
			Control.Styles.AlternatingNode.CopyFrom(Settings.Styles.AlternatingNode);
			Control.StylesEditors.CopyFrom(Settings.StylesEditors);
			Control.StylesPager.CopyFrom(Settings.StylesPager);
			Control.Summary.Assign(Settings.Summary);
			Control.SummaryText = Settings.SummaryText;
			if(Settings.Nodes.Count > 0) {
				foreach(MVCxTreeListNode item in Settings.Nodes) {
					TreeListNode node = Control.AppendNode(item.KeyObject, null);
					AssignNodeProperties(item, ref node);
					AppendChildNodes(item);
				}
			}
			Control.AccessibilityCompliant = Settings.AccessibilityCompliant;
			Control.RightToLeft = Settings.RightToLeft;
			Control.InitialPageSize = Settings.SettingsPager.PageSize;
			Control.BeforeGetCallbackResult += Settings.BeforeGetCallbackResult;
			Control.CellEditorInitialize += Settings.CellEditorInitialize;
			Control.ClientLayout += Settings.ClientLayout;
			Control.CommandColumnButtonInitialize += Settings.CommandColumnButtonInitialize;
			Control.CustomJSProperties += Settings.CustomJSProperties;
			Control.DataBound += Settings.DataBound;
			Control.DataBinding += Settings.DataBinding;
			Control.HtmlCommandCellPrepared += Settings.HtmlCommandCellPrepared;
			Control.HtmlDataCellPrepared += Settings.HtmlDataCellPrepared;
			Control.HtmlRowPrepared += Settings.HtmlRowPrepared;
			Control.InitNewNode += Settings.InitNewNode;
			Control.NodeValidating += Settings.NodeValidating;
			Control.CustomSummaryCalculate += Settings.CustomSummaryCalculate;
			Control.CustomNodeSort += Settings.CustomNodeSort;
			Control.VirtualModeNodeCreated += Settings.VirtualModeNodeCreated;
		}
		protected override void AssignRenderProperties() {
			base.AssignRenderProperties();
			AssignTemplates();
		}
		protected void AssignTemplates() {
			Control.Templates.HeaderCaption = ContentControlTemplate<TreeListHeaderTemplateContainer>.Create(
				Settings.HeaderCaptionTemplateContent, Settings.HeaderCaptionTemplateContentMethod,
				typeof(TreeListHeaderTemplateContainer));
			Control.Templates.DataCell = ContentControlTemplate<TreeListDataCellTemplateContainer>.Create(
				Settings.DataCellTemplateContent, Settings.DataCellTemplateContentMethod,
				typeof(TreeListDataCellTemplateContainer));
			Control.Templates.Preview = ContentControlTemplate<TreeListPreviewTemplateContainer>.Create(
				Settings.PreviewTemplateContent, Settings.PreviewTemplateContentMethod,
				typeof(TreeListPreviewTemplateContainer));
			Control.Templates.GroupFooterCell = ContentControlTemplate<TreeListFooterCellTemplateContainer>.Create(
				Settings.GroupFooterCellTemplateContent, Settings.GroupFooterCellTemplateContentMethod,
				typeof(TreeListFooterCellTemplateContainer));
			Control.Templates.FooterCell = ContentControlTemplate<TreeListFooterCellTemplateContainer>.Create(
				Settings.FooterCellTemplateContent, Settings.FooterCellTemplateContentMethod,
				typeof(TreeListFooterCellTemplateContainer));
			Control.Templates.EditForm = ContentControlTemplate<TreeListEditFormTemplateContainer>.Create(
				Settings.EditFormTemplateContent, Settings.EditFormTemplateContentMethod,
				typeof(TreeListEditFormTemplateContainer));
			for(int i = 0; i < Control.Columns.Count; i++) {
				if(Control.Columns[i] is TreeListCommandColumn) {
					Control.Columns[i].HeaderCaptionTemplate = ContentControlTemplate<TreeListHeaderTemplateContainer>.Create(
						Settings.CommandColumn.HeaderCaptionTemplateContent, Settings.CommandColumn.HeaderCaptionTemplateContentMethod,
						typeof(TreeListHeaderTemplateContainer));
					Control.Columns[i].GroupFooterCellTemplate = ContentControlTemplate<TreeListFooterCellTemplateContainer>.Create(
						Settings.CommandColumn.GroupFooterCellTemplateContent, Settings.CommandColumn.GroupFooterCellTemplateContentMethod,
						typeof(TreeListFooterCellTemplateContainer));
					Control.Columns[i].FooterCellTemplate = ContentControlTemplate<TreeListFooterCellTemplateContainer>.Create(
						Settings.CommandColumn.FooterCellTemplateContent, Settings.CommandColumn.FooterCellTemplateContentMethod,
						typeof(TreeListFooterCellTemplateContainer));
				}
				else
					AssignColumnTemplates(i);
				}
			}
		protected void AssignColumnTemplates(int columnIndex) {
			if(HasNoCustomizedColumns())
				return;
			MVCxTreeListColumn sourceColumn = Settings.Columns[columnIndex] as MVCxTreeListColumn;
			if(sourceColumn != null) {
				TreeListColumn destinationColumn = Control.Columns[columnIndex];
				destinationColumn.HeaderCaptionTemplate = ContentControlTemplate<TreeListHeaderTemplateContainer>.Create(
					sourceColumn.HeaderCaptionTemplateContent, sourceColumn.HeaderCaptionTemplateContentMethod,
					typeof(TreeListHeaderTemplateContainer));
				destinationColumn.GroupFooterCellTemplate = ContentControlTemplate<TreeListFooterCellTemplateContainer>.Create(
					sourceColumn.GroupFooterCellTemplateContent, sourceColumn.GroupFooterCellTemplateContentMethod,
					typeof(TreeListFooterCellTemplateContainer));
				destinationColumn.FooterCellTemplate = ContentControlTemplate<TreeListFooterCellTemplateContainer>.Create(
					sourceColumn.FooterCellTemplateContent, sourceColumn.FooterCellTemplateContentMethod,
					typeof(TreeListFooterCellTemplateContainer));
				((TreeListDataColumn)destinationColumn).DataCellTemplate = ContentControlTemplate<TreeListDataCellTemplateContainer>.Create(
					sourceColumn.DataCellTemplateContent, sourceColumn.DataCellTemplateContentMethod,
					typeof(TreeListDataCellTemplateContainer));
				((TreeListDataColumn)destinationColumn).EditCellTemplate = ContentControlTemplate<TreeListEditCellTemplateContainer>.Create(
					sourceColumn.EditCellTemplateContent, sourceColumn.EditCellTemplateContentMethod,
					typeof(TreeListDataCellTemplateContainer));
			}
		}
		public TreeListExtension Bind(object dataObject) {
			BindInternal(dataObject);
			return this;
		}
		public TreeListExtension BindToVirtualData(TreeListVirtualModeCreateChildrenMethod createChildrenMethod, TreeListVirtualModeNodeCreatingMethod nodeCreatingMethod) {
			if(createChildrenMethod != null)
				Control.VirtualModeCreateChildren += (sender, args) => {
					createChildrenMethod(args);
				};
			if(nodeCreatingMethod != null)
				Control.VirtualModeNodeCreating += (sender, args) => {
					nodeCreatingMethod(args);
				};
			return this;
		}
		public TreeListExtension BindToSiteMap(string fileName) {
			return BindToSiteMap(fileName, true);
		}
		public TreeListExtension BindToSiteMap(string fileName, bool showStartingNode) {
			BindToSiteMapInternal(fileName, showStartingNode);
			return this;
		}
		public TreeListExtension BindToXML(string fileName) {
			return BindToXML(fileName, string.Empty, string.Empty);
		}
		public TreeListExtension BindToXML(string fileName, string xPath) {
			return BindToXML(fileName, xPath, string.Empty);
		}
		public TreeListExtension BindToXML(string fileName, string xPath, string transformFileName) {
			BindToXMLInternal(fileName, xPath, transformFileName);
			return this;
		}
		public TreeListExtension SetEditErrorText(string message) {
			Control.EditErrorText = message;
			return this;
		}
		public static T GetEditValue<T>(string fieldName) {
			return TreeListValueProvider.GetValue<T>(fieldName);
		}
		#region Export
		public static IPrintable CreatePrintableObject(TreeListSettings settings) {
			return CreatePrintableObject(settings, null);
		}
		public static IPrintable CreatePrintableObject(TreeListSettings settings, object dataObject) {
			return (IPrintable)CreateExporter(settings, dataObject);
		}
		static ActionResult Export(TreeListSettings settings, object dataObject, Action<MVCxTreeListExporter, Stream> write, string fileExtension) {
			return Export(settings, dataObject, write, fileExtension, null);
		}
		static ActionResult Export(TreeListSettings settings, object dataObject, Action<MVCxTreeListExporter, Stream> write, string fileExtension, string fileName) {
			return Export(settings, dataObject, write, fileExtension, fileName, true);
		}
		static ActionResult Export(TreeListSettings settings, object dataObject, Action<MVCxTreeListExporter, Stream> write, string fileExtension, bool saveAsFile) {
			return Export(settings, dataObject, write, fileExtension, null, saveAsFile);
		}
		static ActionResult Export(TreeListSettings settings, object dataObject, Action<MVCxTreeListExporter, Stream> write, string fileExtension, string fileName, bool saveAsFile) {
			TreeListExtension extension = CreateExtension(settings, dataObject);
			MVCxTreeListExporter exporter = CreateExporter(extension);
			return ExportUtils.Export(extension, s => write(exporter, s), fileName ?? exporter.FileName, saveAsFile, fileExtension);
		}
		#endregion
		#region Pdf Export
		static void WritePdf(MVCxTreeListExporter exporter, Stream stream) {
			exporter.WritePdf(stream);
		}
		static Action<MVCxTreeListExporter, Stream> WritePdf(PdfExportOptions options) {
			return (e, s) => e.WritePdf(s, options);
		}
		public static void WritePdf(TreeListSettings settings, Stream stream) {
			WritePdf(settings, null, stream);
		}
		public static void WritePdf(TreeListSettings settings, object dataObject, Stream stream) {
			CreateExporter(settings, dataObject).WritePdf(stream);
		}
		public static ActionResult ExportToPdf(TreeListSettings settings) {
			object dataObject = null;
			return ExportToPdf(settings, dataObject);
		}
		public static ActionResult ExportToPdf(TreeListSettings settings, string fileName) {
			object dataObject = null;
			return ExportToPdf(settings, dataObject, fileName);
		}
		public static ActionResult ExportToPdf(TreeListSettings settings, bool saveAsFile) {
			object dataObject = null;
			return ExportToPdf(settings, dataObject, saveAsFile);
		}
		public static ActionResult ExportToPdf(TreeListSettings settings, string fileName, bool saveAsFile) {
			object dataObject = null;
			return ExportToPdf(settings, dataObject, fileName, saveAsFile);
		}
		public static ActionResult ExportToPdf(TreeListSettings settings, object dataObject) {
			return Export(settings, dataObject, WritePdf, "pdf");
		}
		public static ActionResult ExportToPdf(TreeListSettings settings, object dataObject, string fileName) {
			return Export(settings, dataObject, WritePdf, "pdf", fileName);
		}
		public static ActionResult ExportToPdf(TreeListSettings settings, object dataObject, bool saveAsFile) {
			return Export(settings, dataObject, WritePdf, "pdf", saveAsFile);
		}
		public static ActionResult ExportToPdf(TreeListSettings settings, object dataObject, string fileName, bool saveAsFile) {
			return Export(settings, dataObject, WritePdf, "pdf", fileName, saveAsFile);
		}
		public static void WritePdf(TreeListSettings settings, Stream stream, PdfExportOptions exportOptions) {
			WritePdf(settings, null, stream, exportOptions);
		}
		public static void WritePdf(TreeListSettings settings, object dataObject, Stream stream, PdfExportOptions exportOptions) {
			CreateExporter(settings, dataObject).WritePdf(stream, exportOptions);
		}
		public static ActionResult ExportToPdf(TreeListSettings settings, PdfExportOptions exportOptions) {
			object dataObject = null;
			return ExportToPdf(settings, dataObject, exportOptions);
		}
		public static ActionResult ExportToPdf(TreeListSettings settings, string fileName, PdfExportOptions exportOptions) {
			object dataObject = null;
			return ExportToPdf(settings, dataObject, fileName, exportOptions);
		}
		public static ActionResult ExportToPdf(TreeListSettings settings, bool saveAsFile, PdfExportOptions exportOptions) {
			object dataObject = null;
			return ExportToPdf(settings, dataObject, saveAsFile, exportOptions);
		}
		public static ActionResult ExportToPdf(TreeListSettings settings, string fileName, bool saveAsFile, PdfExportOptions exportOptions) {
			object dataObject = null;
			return ExportToPdf(settings, dataObject, fileName, saveAsFile, exportOptions);
		}
		public static ActionResult ExportToPdf(TreeListSettings settings, object dataObject, PdfExportOptions exportOptions) {
			return Export(settings, dataObject, WritePdf(exportOptions), "pdf");
		}
		public static ActionResult ExportToPdf(TreeListSettings settings, object dataObject, string fileName, PdfExportOptions exportOptions) {
			return Export(settings, dataObject, WritePdf(exportOptions), "pdf", fileName);
		}
		public static ActionResult ExportToPdf(TreeListSettings settings, object dataObject, bool saveAsFile, PdfExportOptions exportOptions) {
			return Export(settings, dataObject, WritePdf(exportOptions), "pdf", saveAsFile);
		}
		public static ActionResult ExportToPdf(TreeListSettings settings, object dataObject, string fileName, bool saveAsFile, PdfExportOptions exportOptions) {
			return Export(settings, dataObject, WritePdf(exportOptions), "pdf", fileName, saveAsFile);
		}
		#endregion
		#region Xls Export
		static void WriteXls(MVCxTreeListExporter exporter, Stream stream) {
			exporter.WriteXls(stream);
		}
		static Action<MVCxTreeListExporter, Stream> WriteXls(XlsExportOptions options) {
			return (e, s) => e.WriteXls(s, options);
		}
		public static void WriteXls(TreeListSettings settings, Stream stream) {
			WriteXls(settings, null, stream);
		}
		public static void WriteXls(TreeListSettings settings, object dataObject, Stream stream) {
			CreateExporter(settings, dataObject).WriteXls(stream);
		}
		public static ActionResult ExportToXls(TreeListSettings settings) {
			object dataObject = null;
			return ExportToXls(settings, dataObject);
		}
		public static ActionResult ExportToXls(TreeListSettings settings, string fileName) {
			object dataObject = null;
			return ExportToXls(settings, dataObject, fileName);
		}
		public static ActionResult ExportToXls(TreeListSettings settings, bool saveAsFile) {
			object dataObject = null;
			return ExportToXls(settings, dataObject, saveAsFile);
		}
		public static ActionResult ExportToXls(TreeListSettings settings, string fileName, bool saveAsFile) {
			object dataObject = null;
			return ExportToXls(settings, dataObject, fileName, saveAsFile);
		}
		public static ActionResult ExportToXls(TreeListSettings settings, object dataObject) {
			return Export(settings, dataObject, WriteXls, "xls");
		}
		public static ActionResult ExportToXls(TreeListSettings settings, object dataObject, string fileName) {
			return Export(settings, dataObject, WriteXls, "xls", fileName);
		}
		public static ActionResult ExportToXls(TreeListSettings settings, object dataObject, bool saveAsFile) {
			return Export(settings, dataObject, WriteXls, "xls", saveAsFile);
		}
		public static ActionResult ExportToXls(TreeListSettings settings, object dataObject, string fileName, bool saveAsFile) {
			return Export(settings, dataObject, WriteXls, "xls", fileName, saveAsFile);
		}
		public static void WriteXls(TreeListSettings settings, Stream stream, XlsExportOptions exportOptions) {
			WriteXls(settings, null, stream, exportOptions);
		}
		public static void WriteXls(TreeListSettings settings, object dataObject, Stream stream, XlsExportOptions exportOptions) {
			CreateExporter(settings, dataObject).WriteXls(stream, exportOptions);
		}
		public static ActionResult ExportToXls(TreeListSettings settings, XlsExportOptions exportOptions) {
			object dataObject = null;
			return ExportToXls(settings, dataObject, exportOptions);
		}
		public static ActionResult ExportToXls(TreeListSettings settings, string fileName, XlsExportOptions exportOptions) {
			object dataObject = null;
			return ExportToXls(settings, dataObject, fileName, exportOptions);
		}
		public static ActionResult ExportToXls(TreeListSettings settings, bool saveAsFile, XlsExportOptions exportOptions) {
			object dataObject = null;
			return ExportToXls(settings, dataObject, saveAsFile, exportOptions);
		}
		public static ActionResult ExportToXls(TreeListSettings settings, string fileName, bool saveAsFile, XlsExportOptions exportOptions) {
			object dataObject = null;
			return ExportToXls(settings, dataObject, fileName, saveAsFile, exportOptions);
		}
		public static ActionResult ExportToXls(TreeListSettings settings, object dataObject, XlsExportOptions exportOptions) {
			return Export(settings, dataObject, WriteXls(exportOptions), "xls");
		}
		public static ActionResult ExportToXls(TreeListSettings settings, object dataObject, string fileName, XlsExportOptions exportOptions) {
			return Export(settings, dataObject, WriteXls(exportOptions), "xls", fileName);
		}
		public static ActionResult ExportToXls(TreeListSettings settings, object dataObject, bool saveAsFile, XlsExportOptions exportOptions) {
			return Export(settings, dataObject, WriteXls(exportOptions), "xls", saveAsFile);
		}
		public static ActionResult ExportToXls(TreeListSettings settings, object dataObject, string fileName, bool saveAsFile, XlsExportOptions exportOptions) {
			return Export(settings, dataObject, WriteXls(exportOptions), "xls", fileName, saveAsFile);
		}
		#endregion
		#region Xlsx Export
		static void WriteXlsx(MVCxTreeListExporter exporter, Stream stream) {
			exporter.WriteXlsx(stream);
		}
		static Action<MVCxTreeListExporter, Stream> WriteXlsx(XlsxExportOptions options) {
			return (e, s) => e.WriteXlsx(s, options);
		}
		public static void WriteXlsx(TreeListSettings settings, Stream stream) {
			WriteXlsx(settings, null, stream);
		}
		public static void WriteXlsx(TreeListSettings settings, object dataObject, Stream stream) {
			CreateExporter(settings, dataObject).WriteXlsx(stream);
		}
		public static ActionResult ExportToXlsx(TreeListSettings settings) {
			object dataObject = null;
			return ExportToXlsx(settings, dataObject);
		}
		public static ActionResult ExportToXlsx(TreeListSettings settings, string fileName) {
			object dataObject = null;
			return ExportToXlsx(settings, dataObject, fileName);
		}
		public static ActionResult ExportToXlsx(TreeListSettings settings, bool saveAsFile) {
			object dataObject = null;
			return ExportToXlsx(settings, dataObject, saveAsFile);
		}
		public static ActionResult ExportToXlsx(TreeListSettings settings, string fileName, bool saveAsFile) {
			object dataObject = null;
			return ExportToXlsx(settings, dataObject, fileName, saveAsFile);
		}
		public static ActionResult ExportToXlsx(TreeListSettings settings, object dataObject) {
			return Export(settings, dataObject, WriteXlsx, "xlsx");
		}
		public static ActionResult ExportToXlsx(TreeListSettings settings, object dataObject, string fileName) {
			return Export(settings, dataObject, WriteXlsx, "xlsx", fileName);
		}
		public static ActionResult ExportToXlsx(TreeListSettings settings, object dataObject, bool saveAsFile) {
			return Export(settings, dataObject, WriteXlsx, "xlsx", saveAsFile);
		}
		public static ActionResult ExportToXlsx(TreeListSettings settings, object dataObject, string fileName, bool saveAsFile) {
			return Export(settings, dataObject, WriteXlsx, "xlsx", fileName, saveAsFile);
		}
		public static void WriteXlsx(TreeListSettings settings, Stream stream, XlsxExportOptions exportOptions) {
			WriteXlsx(settings, null, stream, exportOptions);
		}
		public static void WriteXlsx(TreeListSettings settings, object dataObject, Stream stream, XlsxExportOptions exportOptions) {
			CreateExporter(settings, dataObject).WriteXlsx(stream, exportOptions);
		}
		public static ActionResult ExportToXlsx(TreeListSettings settings, XlsxExportOptions exportOptions) {
			object dataObject = null;
			return ExportToXlsx(settings, dataObject, exportOptions);
		}
		public static ActionResult ExportToXlsx(TreeListSettings settings, string fileName, XlsxExportOptions exportOptions) {
			object dataObject = null;
			return ExportToXlsx(settings, dataObject, fileName, exportOptions);
		}
		public static ActionResult ExportToXlsx(TreeListSettings settings, bool saveAsFile, XlsxExportOptions exportOptions) {
			object dataObject = null;
			return ExportToXlsx(settings, dataObject, saveAsFile, exportOptions);
		}
		public static ActionResult ExportToXlsx(TreeListSettings settings, string fileName, bool saveAsFile, XlsxExportOptions exportOptions) {
			object dataObject = null;
			return ExportToXlsx(settings, dataObject, fileName, saveAsFile, exportOptions);
		}
		public static ActionResult ExportToXlsx(TreeListSettings settings, object dataObject, XlsxExportOptions exportOptions) {
			return Export(settings, dataObject, WriteXlsx(exportOptions), "xlsx");
		}
		public static ActionResult ExportToXlsx(TreeListSettings settings, object dataObject, string fileName, XlsxExportOptions exportOptions) {
			return Export(settings, dataObject, WriteXlsx(exportOptions), "xlsx", fileName);
		}
		public static ActionResult ExportToXlsx(TreeListSettings settings, object dataObject, bool saveAsFile, XlsxExportOptions exportOptions) {
			return Export(settings, dataObject, WriteXlsx(exportOptions), "xlsx", saveAsFile);
		}
		public static ActionResult ExportToXlsx(TreeListSettings settings, object dataObject, string fileName, bool saveAsFile, XlsxExportOptions exportOptions) {
			return Export(settings, dataObject, WriteXlsx(exportOptions), "xlsx", fileName, saveAsFile);
		}
		#endregion
		#region Rtf Export
		static void WriteRtf(MVCxTreeListExporter exporter, Stream stream) {
			exporter.WriteRtf(stream);
		}
		static Action<MVCxTreeListExporter, Stream> WriteRtf(RtfExportOptions options) {
			return (e, s) => e.WriteRtf(s, options);
		}
		public static void WriteRtf(TreeListSettings settings, Stream stream) {
			WriteRtf(settings, null, stream);
		}
		public static void WriteRtf(TreeListSettings settings, object dataObject, Stream stream) {
			CreateExporter(settings, dataObject).WriteRtf(stream);
		}
		public static ActionResult ExportToRtf(TreeListSettings settings) {
			object dataObject = null;
			return ExportToRtf(settings, dataObject);
		}
		public static ActionResult ExportToRtf(TreeListSettings settings, string fileName) {
			object dataObject = null;
			return ExportToRtf(settings, dataObject, fileName);
		}
		public static ActionResult ExportToRtf(TreeListSettings settings, bool saveAsFile) {
			object dataObject = null;
			return ExportToRtf(settings, dataObject, saveAsFile);
		}
		public static ActionResult ExportToRtf(TreeListSettings settings, string fileName, bool saveAsFile) {
			object dataObject = null;
			return ExportToRtf(settings, dataObject, fileName, saveAsFile);
		}
		public static ActionResult ExportToRtf(TreeListSettings settings, object dataObject) {
			return Export(settings, dataObject, WriteRtf, "rtf");
		}
		public static ActionResult ExportToRtf(TreeListSettings settings, object dataObject, string fileName) {
			return Export(settings, dataObject, WriteRtf, "rtf", fileName);
		}
		public static ActionResult ExportToRtf(TreeListSettings settings, object dataObject, bool saveAsFile) {
			return Export(settings, dataObject, WriteRtf, "rtf", saveAsFile);
		}
		public static ActionResult ExportToRtf(TreeListSettings settings, object dataObject, string fileName, bool saveAsFile) {
			return Export(settings, dataObject, WriteRtf, "rtf", fileName, saveAsFile);
		}
		public static void WriteRtf(TreeListSettings settings, Stream stream, RtfExportOptions exportOptions) {
			WriteRtf(settings, null, stream, exportOptions);
		}
		public static void WriteRtf(TreeListSettings settings, object dataObject, Stream stream, RtfExportOptions exportOptions) {
			CreateExporter(settings, dataObject).WriteRtf(stream, exportOptions);
		}
		public static ActionResult ExportToRtf(TreeListSettings settings, RtfExportOptions exportOptions) {
			object dataObject = null;
			return ExportToRtf(settings, dataObject, exportOptions);
		}
		public static ActionResult ExportToRtf(TreeListSettings settings, string fileName, RtfExportOptions exportOptions) {
			object dataObject = null;
			return ExportToRtf(settings, dataObject, fileName, exportOptions);
		}
		public static ActionResult ExportToRtf(TreeListSettings settings, bool saveAsFile, RtfExportOptions exportOptions) {
			object dataObject = null;
			return ExportToRtf(settings, dataObject, saveAsFile, exportOptions);
		}
		public static ActionResult ExportToRtf(TreeListSettings settings, string fileName, bool saveAsFile, RtfExportOptions exportOptions) {
			object dataObject = null;
			return ExportToRtf(settings, dataObject, fileName, saveAsFile, exportOptions);
		}
		public static ActionResult ExportToRtf(TreeListSettings settings, object dataObject, RtfExportOptions exportOptions) {
			return Export(settings, dataObject, WriteRtf(exportOptions), "rtf");
		}
		public static ActionResult ExportToRtf(TreeListSettings settings, object dataObject, string fileName, RtfExportOptions exportOptions) {
			return Export(settings, dataObject, WriteRtf(exportOptions), "rtf", fileName);
		}
		public static ActionResult ExportToRtf(TreeListSettings settings, object dataObject, bool saveAsFile, RtfExportOptions exportOptions) {
			return Export(settings, dataObject, WriteRtf(exportOptions), "rtf", saveAsFile);
		}
		public static ActionResult ExportToRtf(TreeListSettings settings, object dataObject, string fileName, bool saveAsFile, RtfExportOptions exportOptions) {
			return Export(settings, dataObject, WriteRtf(exportOptions), "rtf", fileName, saveAsFile);
		}
		#endregion
		static TreeListExtension CreateExtension(TreeListSettings settings, object dataObject) {
			TreeListExtension extension = ExtensionsFactory.InstanceInternal.TreeList(settings);
			if(dataObject != null)
				extension.Bind(dataObject);
			extension.PrepareControl();
			extension.LoadPostData();
			return extension;
		}
		static MVCxTreeListExporter CreateExporter(TreeListSettings settings, object dataObject) {
			return CreateExporter(CreateExtension(settings, dataObject));
		}
		static MVCxTreeListExporter CreateExporter(TreeListExtension extension) {
			MVCxTreeListExporter exporter = new MVCxTreeListExporter(extension.Control);
			AssignExporterSettings(exporter, extension.Settings.SettingsExport);
			return exporter;
		}
		static void AssignExporterSettings(MVCxTreeListExporter exporter, MVCxTreeListSettingsExport settings) {
			exporter.FileName = settings.FileName;
			exporter.Settings.Assign(settings.PrintSettings);
			exporter.Styles.CopyFrom(settings.Styles);
			exporter.RenderBrick += settings.RenderBrick;
		}
		protected internal override void PrepareControl() {
			base.PrepareControl();
			Control.PerformOnLoad();
			Control.EnsureClientStateLoaded();
		}
		protected override Control GetCallbackResultControl() {
			return Control.GetCallbackResultControl();
		}
		protected override void RenderCallbackResultControl() {
			if(Control.IsPartialUpdatePossible()) {
				List<System.Web.UI.WebControls.TableRow> rows = Control.GetPartialCallbackResult();
				if(rows != null) {
					foreach(System.Web.UI.WebControls.TableRow row in rows)
						RenderControl(row);
				}
			}
			else 
				base.RenderCallbackResultControl();
		}
		protected void AppendChildNodes(MVCxTreeListNode parentNode) {
			if(parentNode.ChildNodes.Count > 0) {
				foreach(MVCxTreeListNode item in parentNode.ChildNodes) {
					TreeListNode node = Control.AppendNode(item.KeyObject, Control.FindNodeByKeyValue(parentNode.Key));
					AssignNodeProperties(item, ref node);
					AppendChildNodes(item);
				}
			}
		}
		protected void AssignNodeProperties(MVCxTreeListNode source, ref TreeListNode node) {
			if(source != null) {
				Dictionary<string, object> fieldsValues = source.NodeFields;
				if(fieldsValues != null && fieldsValues.Count > 0) {
					foreach(KeyValuePair<string, object> field in fieldsValues)
						node[field.Key] = field.Value;
				}
				node.AllowSelect = source.AllowSelect;
				node.Expanded = source.Expanded;
				node.Selected = source.Selected;
			}
		}
		protected override ASPxWebControl CreateControl() {
			return new MVCxTreeList(ViewContext);
		}
		protected override void LoadPostDataInternal() {
			var postDataCollection = ValueProvider != null ? new MvcPostDataCollection(ValueProvider) : Request.Params;
			PostBackDataHandler.LoadPostData("", postDataCollection);
			Control.LayoutChanged();
			Control.DoSort();
			if(Control.IsEditing && CallbackCommandId == TreeListCommandId.UpdateEdit)
				Control.SetEditingValues();
		}
		public new static ContentResult GetCustomDataCallbackResult(object data) {
			if(CallbackCommandId != TreeListCommandId.CustomDataCallback) {
				throw new InvalidOperationException(
					"A client MVCxClientTreeList.PerformCustomDataCallback function must be called in order to execute the GetCustomDataCallbackResult server method.");
			}
			MVCxTreeListCustomDataCallbackCommand command = new MVCxTreeListCustomDataCallbackCommand(
				ProcessedCallbackArgument.Substring(ProcessedCallbackArgument.IndexOf(CommandSeparator) + 1)
			); 
			return ExtensionBase.GetCustomDataCallbackResult(command.GetCustomDataCallbackResult(data));
		}
		static TreeListCommandId CallbackCommandId {
			get {
				TreeListCommandId commandId = TreeListCommandId.Empty;
				int separatorPos = ProcessedCallbackArgument.IndexOf(CommandSeparator);
				int commandIdValue;
				if(Int32.TryParse(separatorPos < 0 ? ProcessedCallbackArgument : ProcessedCallbackArgument.Substring(0, separatorPos), out commandIdValue))
					commandId = (TreeListCommandId)commandIdValue;
				return commandId;
			}
		}
	}
}
