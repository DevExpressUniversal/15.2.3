#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
using System.ComponentModel.Design;
using System.Drawing.Printing;
using System.IO;
using DevExpress.Utils;
using DevExpress.Web.Internal;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Export.Web;
using DevExpress.XtraPrinting.HtmlExport.Controls;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UI;
namespace DevExpress.XtraReports.Web.Native {
	public class ReportRenderHelper {
		protected ReportViewer ReportViewer { get; private set; }
		protected virtual int PageCount { get { return ReportViewer.PageCount; } }
		protected virtual IServiceContainer ServiceContainer { get { return Report.PrintingSystem; } }
		XtraReport Report { get { return ReportViewer.ActualReport; } }
		public ReportRenderHelper(ReportViewer reportViewer) {
			ReportViewer = reportViewer;
		}
		public string WriteWholeDocument() {
			using(IImageRepository imageRepository = ReportViewer.CreateImageRepository(false)) {
				Document document = XtraReportAccessor.GetReportDocument(Report);
				ReportWebControl webControl = null;
				if(document.IsModified) {
					HtmlExportOptionsBase htmlExportOptions = PrepareOptions(Report.ExportOptions.Html);
					webControl = new ReportMultiplePageWebControl(document, imageRepository, htmlExportOptions);
				} else {
					webControl = new ReportWebControl(document, imageRepository, ReportViewer.TableLayout);
				}
				return WriteToString(webControl, imageRepository);
			}
		}
		HtmlExportOptionsBase PrepareOptions(HtmlExportOptions reportOptions) {
			var viewerOptions = new HtmlExportOptions();
			viewerOptions.Assign(reportOptions);
			viewerOptions.TableLayout = ReportViewer.TableLayout;
			return viewerOptions;
		}
		public string WriteForPrinting(int pageIndex) {
			using(IImageRepository imageRepository = ReportViewer.CreateImageRepository(true)) {
				DXWebControlBase webControl = pageIndex >= 0
					? CreatePageWebControl(imageRepository, pageIndex)
					: CreateMultiPageControl(imageRepository);
				return WriteToString(webControl, !RenderUtils.Browser.IsOpera, imageRepository);
			}
		}
		public string WritePage(int pageIndex) {
			using(IImageRepository imageRepository = ReportViewer.CreateImageRepository(false)) {
				var service = new AspNavigationService(ReportViewer.ClientID);
				ServiceContainer.AddService<INavigationService>(service);
				ServiceContainer.ReplaceService<IBrickPublisher>(new DefaultBrickPublisher());
				try {
					var pageWebControl = CreatePageWebControl(imageRepository, pageIndex);
					return WriteToString(pageWebControl, imageRepository);
				} finally {
					ServiceContainer.RemoveService(typeof(INavigationService));
					ServiceContainer.RemoveService<IBrickPublisher>();
				}
			}
		}
		protected virtual ReportsPageWebControl CreatePageWebControl(IImageRepository imageRepository, int pageIndex) {
			if(Report == null) {
				return null;
			}
			Document document = XtraReportAccessor.GetReportDocument(Report);
			return new ReportsPageWebControl(document, imageRepository, pageIndex, ReportViewer.TableLayout, ReportViewer.EnableReportMargins);
		}
		protected virtual HtmlExportOptionsBase CreateHtmlExportOptionsForDocumentBuilder() {
			var report = Report;
			var exportOptionsHtml = report.ExportOptions.Html;
			var title = StringHelper.GetNonEmptyValue(report.DisplayName, exportOptionsHtml.Title, report.Name);
			return ExportOptionsHelper.ChangeOldHtmlProperties(exportOptionsHtml, exportOptionsHtml.CharacterSet, title, false);
		}
		protected virtual void BeginWriteToString() {
			if(!ReportViewer.IsReportReady(Report))
				throw new InvalidOperationException("Report");
		}
		DXWebControlBase CreateMultiPageControl(IImageRepository imageRepository) {
			var placeHolder = new DXWebControlBase();
			var styles = new WebStyleControl();
			if(PageCount > 0)
				placeHolder.Controls.Add(styles);
			for(int i = 0; i < PageCount; i++) {
				placeHolder.Controls.Add(CreatePageWebControl(imageRepository, i));
				if(i < PageCount - 1)
					placeHolder.Controls.Add(PSWebControl.CreatePageBreaker(styles));
			}
			return placeHolder;
		}
		string WriteToString(DXWebControlBase webControl, IImageRepository imageRepository) {
			return WriteToString(webControl, false, imageRepository);
		}
		string WriteToString(DXWebControlBase webControl, bool generateWholePage, IImageRepository imageRepository) {
			Guard.ArgumentNotNull(webControl, "webControl");
			BeginWriteToString();
			HtmlDocumentBuilderBase builder = CreateHtmlDocumentBuilder(generateWholePage);
			using(var stringWriter = new StringWriter()) {
				bool shouldCompressOutput = RenderUtils.Browser.IsIE && RenderUtils.Browser.MajorVersion >= 9;
				var page = ReportViewer.Page;
				var textWriter = new DXHtmlTextWriterEx(stringWriter, page, shouldCompressOutput);
				builder.CreateDocumentCore(textWriter, webControl, imageRepository);
				return stringWriter.ToString();
			}
		}
		HtmlDocumentBuilderBase CreateHtmlDocumentBuilder(bool generateWholePage) {
			if(!generateWholePage)
				return new HtmlDocumentBuilderBase();
			var exportOptions = CreateHtmlExportOptionsForDocumentBuilder();
			var builder = new HtmlDocumentBuilder(exportOptions) {
				BodyMargins = new Margins(0, 0, 0, 0)
			};
			return builder;
		}
	}
}
