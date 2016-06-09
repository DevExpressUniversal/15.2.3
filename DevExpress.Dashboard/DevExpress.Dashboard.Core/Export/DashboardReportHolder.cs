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

using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Export;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.Service;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Export.Pdf;
using DevExpress.XtraReports;
using DevExpress.XtraReports.UI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
namespace DevExpress.DashboardExport {
	public abstract class DashboardReportHolder : IDisposable, IReportHolder {
		readonly XtraReport report;
		readonly DetailBand band;
		protected XtraReport Report { get { return report; } }
		protected DetailBand Band { get { return band; } }
		protected DashboardReportHolder(VerticalContentSplitting verticalContentSplitting) {
			report = new DashboardExportReport { ReportUnit = ReportUnit.Pixels, VerticalContentSplitting = verticalContentSplitting };
			band = new DetailBand();
			report.Bands.Add(band);
		}
		void ApplyScaleMode(IReport report, ScalingOptions scalingOptions) {
			XtraReport xtraReport = (XtraReport)report;
			xtraReport.PrintingSystem.Document.ScaleFactor = scalingOptions.ScaleFactor;
			xtraReport.PrintingSystem.Document.AutoFitToPagesWidth = scalingOptions.AutoFitPageCount;
		}
		void ApplyPaperOptions(IReport report, PaperOptions paperOptions) {
			XtraReport xtraReport = (XtraReport)report;
			xtraReport.PaperKind = paperOptions.PaperKind;
			xtraReport.Landscape = paperOptions.Landscape;
		}
		void ApplyReportName(IReport report, string name) {
			XtraReport xtraReport = (XtraReport)report;
			if (!string.IsNullOrEmpty(name)) {
				string cleanName = name;
				cleanName = Path.GetInvalidFileNameChars().Aggregate(cleanName, (current, invalid) => current.Replace(invalid.ToString(), string.Empty));
				if (!string.IsNullOrWhiteSpace(cleanName))
					xtraReport.Name = cleanName;
			}
		}
		void PrepareDocumentOptions(DashboardReportOptions opts, DashboardExportMode mode, Size clientSize, Margins margins) {
			PageOptions pageOptions = opts.PageOptions;
			PaperOptions paperOptions = pageOptions.PaperOptions;
			ScalingOptions scalingOptions = pageOptions.ScalingOptions;
			AutomaticPageOptions autoOptions = opts.AutoPageOptions;
			if (!(autoOptions.AutoFitToPageSize && autoOptions.AutoRotate))
				return;
			if (mode == DashboardExportMode.SingleItem || opts.FormatOptions.Format == DashboardExportFormat.Image) {
				scalingOptions.ScaleFactor = 1.0f;
				scalingOptions.AutoFitPageCount = 0;
				return;
			}
			pageOptions.PaperOptions.Landscape = clientSize.Width > clientSize.Height;
			Size pageSize = ExportHelper.GetPageClientSize(paperOptions.PaperKind, margins, paperOptions.Landscape);
			if ((float)pageSize.Width / (float)pageSize.Height < (float)clientSize.Width / (float)clientSize.Height) {
				scalingOptions.AutoFitPageCount = 1;
				scalingOptions.ScaleFactor = 1f;
			} else {
				scalingOptions.AutoFitPageCount = 0;
				scalingOptions.ScaleFactor = (float)pageSize.Height / (float)clientSize.Height;
			}
		}
		void PrepareFormatOptions(DashboardReportOptions opts) {
			opts.FormatOptions.PdfOptions.ConvertImagesToJpeg = !ExportHelper.CheckSecurityPermissions();
			PdfGraphics.EnableAzureCompatibility = DashboardExportSettings.CompatibilityMode == DashboardExportCompatibilityMode.Restricted;
		}
		protected void PrepareReport(IReport report, DashboardExportMode mode, Size clientSize, DashboardReportOptions opts) {
			PrepareDocumentOptions(opts, mode, clientSize, ((XtraReport)report).Margins);
			PrepareFormatOptions(opts);
			ApplyPaperOptions(report, opts.PageOptions.PaperOptions);
			ApplyReportName(report, opts.FileName);
			report.CreateDocument();
			ApplyScaleMode(report, opts.PageOptions.ScalingOptions);
		}
		protected void AddSeparateFilterBlock(DetailBand band, PointF location, float width, FilterStatePresentation presentation, IList<DimensionFilterValues> filterValues, DashboardFontInfo fontInfo) {
			if (filterValues != null && filterValues.Count > 0 && presentation != FilterStatePresentation.None) {
				if (presentation == FilterStatePresentation.AfterAndSplitPage)
					band.Controls.Add(new XRPageBreak { LocationF = location });
				XRControl blockControl = new FilterValuesSeparateBlock(filterValues).CreateControl(fontInfo);
				blockControl.BoundsF = new RectangleF(new PointF(location.X, location.Y + 5), new SizeF(width, 20));
				band.Controls.Add(blockControl);
			}
		}
		#region IReportHolder
		IReport IReportHolder.Report { get { return report; } }
		void IReportHolder.Update(ExportInfo exportInfo) {
			UpdateReportState(exportInfo);
		}
		#endregion
		protected abstract void UpdateReportState(ExportInfo exportInfo);
		protected abstract void DisposeExporters();
		public void Dispose() {
			report.Dispose();
			DisposeExporters();
		}
	}
}
