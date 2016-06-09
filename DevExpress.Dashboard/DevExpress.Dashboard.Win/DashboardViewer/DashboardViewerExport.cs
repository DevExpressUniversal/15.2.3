#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardWin.Forms.Export;
using DevExpress.DashboardWin.Native;
using DevExpress.DashboardWin.Native.Printing;
using DevExpress.DashboardWin.ServiceModel;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
namespace DevExpress.DashboardWin {
	public partial class DashboardViewer {
		bool allowPrintDashboard = DefaultAllowPrintDashboard;
		bool allowPrintDashboardItems = DefaultAllowPrintDashboardItmes;
		DashboardPrintPreviewType printPreviewType = DefaultPrintPreviewType;
		IDashboardExportUIProvider exportProvider;
		ExportOptionsRepository exportOptionsRepository;
		IDashboardExportService ExportService { get { return serviceContainer.RequestServiceStrictly<IDashboardExportService>(); } }
		internal void ShowPrintPreview(IDashboardExportItem exportItem, IExportOptionsOwner options) {
			ShowPrintPreviewInternal(printPreviewType, exportItem, options);
		}
		internal void ShowPrintPreview(DashboardPrintPreviewType printPreviewType) {
			ShowPrintPreviewInternal(printPreviewType, this, this);
		}
		internal void ShowPrintPreviewBySelectedType() {
			ShowPrintPreview(printPreviewType);
		}
		internal void ShowExportForm(DashboardExportFormat format) {
			ShowExportForm(format, this, this);
		}
		internal void ShowExportForm(DashboardExportFormat format, IDashboardExportItem exportItem, IExportOptionsOwner options) {
			bool shouldExport = true;
			if(ShouldShowExportForm())
				shouldExport = exportProvider.ShowExportForm(GetItemCaption(options), exportItem.ExportItemType, format, options);
			if(shouldExport)
				exportProvider.ShowSaveFileDialogAndExport(format, exportItem, options);
		}
		internal void SetFormProvider(IDashboardExportUIProvider exportProvider) {
			this.exportProvider = exportProvider;
		}
		internal ExtendedReportOptions GetDefaultReportOptions(string title, bool titleVisiblity, string itemType) {
			ExtendedReportOptions opts = printingOptions.GetOptions(itemType);
			if(!String.IsNullOrEmpty(itemType))
				return opts;
			opts.FileName = string.IsNullOrEmpty(title) ? Name : title;
			DefaultBoolean showTitle = printingOptions.DocumentContentOptions.ShowTitle;
			if(showTitle == DocumentContentPrintingOptions.DefaultShowTitle) {
#pragma warning disable 0612, 0618 // Obsolete
				showTitle = printingOptions.DashboardItemOptions.IncludeCaption;
#pragma warning restore 0612, 0618
			}
			opts.DocumentContentOptions.ShowTitle = showTitle.ToBoolean(titleVisiblity);
			opts.DocumentContentOptions.Title = title;
			return opts;
		}
		internal ExtendedReportOptions GetDefaultReportOptions() {
			return GetDefaultReportOptions(null);
		}
		internal ExtendedReportOptions GetDefaultReportOptions(string itemType) {
			return GetDefaultReportOptions(titlePresenter.TitleText, titlePresenter.TitleVisible, itemType);
		}
		internal ExportOptionsRepository GetExportOptionsRepository() {
			return exportOptionsRepository;
		}
		void ShowPrintPreviewInternal(DashboardPrintPreviewType printPreviewType, IDashboardExportItem exportItem, IExportOptionsOwner optionsOwner) {
			ExportInfo exportInfo = exportItem.CreateExportInfo(optionsOwner.GetActual().GetOptions());
			using(IReportHolder reportHolder = ExportService.GetExportReport(exportInfo, GetDashboardClientState())) {
				exportProvider.ShowPrintPreview(reportHolder, exportItem, optionsOwner, printPreviewType);
			}
		}
		string GetItemCaption(IExportOptionsOwner owner) {
			DocumentContentOptions documentContentOptions = owner.GetActual().DocumentContentOptions;
			return documentContentOptions != null ? documentContentOptions.Title : string.Empty;
		}
		void RaiseBeforeExport(DashboardBeforeExportEventArgs e) {
			if(BeforeExport != null)
				BeforeExport(this, e);
		}
		bool ShouldShowExportForm() {
			DashboardBeforeExportEventArgs e = new DashboardBeforeExportEventArgs();
			RaiseBeforeExport(e);
			return e.ShowExportForm;
		}
		ExtendedReportOptions GetActualReportOptions(ExtendedReportOptions defaultReportOptions) {
			return exportOptionsRepository.GetActualOpts(String.Empty, defaultReportOptions);
		}
		void ExportToPdf(Stream stream, PdfExportOptions pdfOpts, IExportOptionsOwner options, IDashboardExportItem exportItem) {
			DashboardReportOptions opts = options.GetDefault().GetOptions();
			opts.FormatOptions.Format = DashboardExportFormat.PDF;
			if (pdfOpts != null)
				opts.FormatOptions.PdfOptions = pdfOpts;
			ExportTo(stream, opts, exportItem);
		}
		void ExportToPdf(string filePath, PdfExportOptions pdfOpts, IExportOptionsOwner options, IDashboardExportItem exportItem) {
			DashboardReportOptions opts = options.GetDefault().GetOptions();
			opts.FormatOptions.Format = DashboardExportFormat.PDF;
			if (pdfOpts != null)
				opts.FormatOptions.PdfOptions = pdfOpts;
			ExportTo(filePath, opts, exportItem);
		}
		void ExportToImage(Stream stream,  ImageExportOptions imgOpts, IExportOptionsOwner options, IDashboardExportItem exportItem) {
			DashboardReportOptions opts = options.GetDefault().GetOptions();
			opts.FormatOptions.Format = DashboardExportFormat.Image;
			if (imgOpts != null)
				opts.FormatOptions.ImageOptions = imgOpts;
			ExportTo(stream, opts, exportItem);
		}
		void ExportToImage(string filePath, ImageExportOptions imgOpts, IExportOptionsOwner options, IDashboardExportItem exportItem) {
			DashboardReportOptions opts = options.GetDefault().GetOptions();
			opts.FormatOptions.Format = DashboardExportFormat.Image;
			if (imgOpts != null)
				opts.FormatOptions.ImageOptions = imgOpts;
			ExportTo(filePath, opts, exportItem);
		}
		void ExportToExcel(string filePath, ExcelExportOptions excelOpts, IExportOptionsOwner options, IDashboardExportItem exportItem) {
			DashboardReportOptions opts = options.GetDefault().GetOptions();
			opts.FormatOptions.Format = DashboardExportFormat.Excel;
			if(excelOpts != null)
				opts.FormatOptions.ExcelOptions = excelOpts;
			ExportTo(filePath, opts, exportItem);
		}
		void ExportToExcel(Stream stream, ExcelExportOptions excelOpts, IExportOptionsOwner options, IDashboardExportItem exportItem) {
			DashboardReportOptions opts = options.GetDefault().GetOptions();
			opts.FormatOptions.Format = DashboardExportFormat.Excel;
			if(excelOpts != null)
				opts.FormatOptions.ExcelOptions = excelOpts;
			ExportTo(stream, opts, exportItem);
		}
		void ExportTo(string filePath, DashboardReportOptions options, IDashboardExportItem exportItem) {
			ExportService.PerformExport(filePath, exportItem.CreateExportInfo(options), GetDashboardClientState());
		}
		void ExportTo(Stream stream, DashboardReportOptions options, IDashboardExportItem exportItem) {
			ExportService.PerformExport(stream, exportItem.CreateExportInfo(options), GetDashboardClientState());
		}
		bool IDashboardExportUIProvider.ShowExportForm(string name, string type, DashboardExportFormat format, IExportOptionsOwner options) {
			using(ExportOptionsForm form = new ExportOptionsForm(name, type, format, options))
				return form.ShowDialog() == DialogResult.OK;
		}
		void IDashboardExportUIProvider.ShowPrintPreview(IReportHolder reportHolder, IDashboardExportItem exportItem, IExportOptionsOwner options, DashboardPrintPreviewType printPreviewType) {
			using(DashboardReportPrintTool tool = new DashboardReportPrintTool(reportHolder, exportItem, options, this)) {
				switch(printPreviewType) {
					case DashboardPrintPreviewType.StandardPreview:
						tool.ShowPreviewDialog(FindForm(), LookAndFeel);
					break;
					case DashboardPrintPreviewType.RibbonPreview:
						tool.ShowRibbonPreviewDialog(FindForm(), LookAndFeel);
					break;
					default:
					throw new NotSupportedException();
				}
			}
		}
		bool IDashboardExportUIProvider.ShowPrintPreviewExportForm(string name, string type, IExportOptionsOwner options) {
			using(PrintPreviewOptionsForm form = new PrintPreviewOptionsForm(name, type, options))
				return form.ShowDialog() == DialogResult.OK;
		}
		void IDashboardExportUIProvider.ShowSaveFileDialogAndExport(DashboardExportFormat format, IDashboardExportItem exportItem, IExportOptionsOwner options) {
			using(SaveFileDialog saveDialog = new SaveFileDialog()) {
				DashboardReportOptions opts = options.GetActual().GetOptions();
				string regexSearch = new string(Path.GetInvalidFileNameChars());
				Regex r = new Regex(string.Format("[{0}]", Regex.Escape(regexSearch)));
				string fileName = r.Replace(GetItemCaption(options), "");
				string filters = FileFilters.AllFiles;
				switch(format) {
					case DashboardExportFormat.Excel:
						switch(opts.FormatOptions.ExcelOptions.Format) {
							case ExcelFormat.Xls:
								filters = FileFilters.XLS;
								break;
							case ExcelFormat.Xlsx:
								filters = FileFilters.XLSX;
								break;
							case ExcelFormat.Csv:
								filters = FileFilters.CSV;
								break;
							default:
								break;
						}
						break;
					case DashboardExportFormat.PDF:
						filters = FileFilters.PDF;
						break;
					default: {
							ImageFormat imageFormat = opts.FormatOptions.ImageOptions.Format;
							if(imageFormat == ImageFormat.Png)
								filters = FileFilters.PNG;
							if(imageFormat == ImageFormat.Gif)
								filters = FileFilters.GIF;
							if(imageFormat == ImageFormat.Jpeg)
								filters = FileFilters.JPEG;
							break;
						}
				}
				saveDialog.Filter = filters;
				saveDialog.FileName = fileName;
				if(!string.IsNullOrEmpty(fileName))
					saveDialog.InitialDirectory = Path.GetDirectoryName(fileName);
				if(saveDialog.ShowDialog() == DialogResult.OK) {
					fileName = saveDialog.FileName;
					if(!string.IsNullOrEmpty(fileName)) {
						opts.FormatOptions.Format = format;
						try {
							ExportService.PerformExport(fileName, exportItem.CreateExportInfo(opts), GetDashboardClientState());
						} catch(Exception ex) {
							DashboardWinHelper.ShowWarningMessage(LookAndFeel, this, ex.Message);
						}
					}
				}
			}
		}
		ExtendedReportOptions IExportOptionsOwner.GetDefault() {
			return GetDefaultReportOptions();
		}
		ExtendedReportOptions IExportOptionsOwner.GetActual() {
			return GetActualReportOptions(GetDefaultReportOptions());
		}
		void IExportOptionsOwner.Set(ExtendedReportOptions opts) {
			exportOptionsRepository.Add(String.Empty, GetDefaultReportOptions(), opts);
		}
		ItemStateCollection IDashboardExportItem.GetItemStateCollection() {
			ItemStateCollection itemStateCollection = new ItemStateCollection();
			foreach(IDashboardExportItem itemViewer in ItemViewers)
				itemStateCollection.AddRange(itemViewer.GetItemStateCollection());
			return itemStateCollection;
		}
		ExportInfo IDashboardExportItem.CreateExportInfo(DashboardReportOptions exportOptions) {
			ExportInfo exportInfo = new ExportInfo();
			exportInfo.ViewerState = new ViewerState();
			exportInfo.ViewerState.Size = GetClientSize();
			exportInfo.ViewerState.ItemsState = ((IDashboardExportItem)this).GetItemStateCollection();
			exportInfo.ViewerState.TitleHeight = TitleBounds.Height;
			exportInfo.FontInfo = printingOptions.FontInfo.GetDashboardFontInfo();
			exportInfo.Mode = DashboardExportMode.EntireDashboard;
			exportInfo.ExportOptions = exportOptions;
			return exportInfo;
		}
		string IDashboardExportItem.ExportItemType { get { return ExportOptionsForm.DashboardType; } }
	}
}
