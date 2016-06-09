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

using System.IO;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraReports.Web.WebDocumentViewer.Native {
	public class PSExportingStrategy : ExportingStrategy {
		readonly PrintingSystemBase ps;
		readonly Stream stream;
		public PSExportingStrategy(PrintingSystemBase ps, Stream stream) {
			Guard.ArgumentNotNull(ps, "ps");
			Guard.ArgumentNotNull(stream, "stream");
			this.ps = ps;
			this.stream = stream;
		}
		protected override void ExportToCsv(CsvExportOptions exportOptions) {
			ps.ExportToCsv(stream, exportOptions);
		}
		protected override void ExportToHtml(HtmlExportOptions exportOptions) {
			ps.ExportToHtml(stream, exportOptions);
		}
		protected override void ExportToImage(ImageExportOptions exportOptions) {
			ps.ExportToImage(stream, exportOptions);
		}
		protected override void ExportToMht(MhtExportOptions exportOptions) {
			ps.ExportToMht(stream, exportOptions);
		}
		protected override void ExportToPdf(PdfExportOptions exportOptions) {
			ps.ExportToPdf(stream, exportOptions);
		}
		protected override void ExportToRtf(RtfExportOptions exportOptions) {
			ps.ExportToRtf(stream, exportOptions);
		}
		protected override void ExportToText(TextExportOptions exportOptions) {
			ps.ExportToText(stream, exportOptions);
		}
		protected override void ExportToXls(XlsExportOptions exportOptions) {
			ps.ExportToXls(stream, exportOptions);
		}
		protected override void ExportToXlsx(XlsxExportOptions exportOptions) {
			ps.ExportToXlsx(stream, exportOptions);
		}
	}
}
