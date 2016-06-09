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
using System.Text;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting.Native;
using System.Drawing.Printing;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraReports.Native.Printing {
	public class DetailReportBandManager : IBandManager {
		TypedComponentEnumerator detailReportBandEnumerator;
		XtraReportBase report;
		PrintingSystemBase printingSystem;
		public DetailReportBandManager(XtraReportBase report) {
			this.report = report;
			detailReportBandEnumerator = new TypedComponentEnumerator(report.OrderedBands, typeof(DetailReportBand));
			printingSystem = report.RootReport.GetCreatingPrintingSystem();
		}
		#region IBandManager Members
		PrintingSystemBase IBandManager.PrintingSystem {
			get { return printingSystem; }
		}
		bool IBandManager.IsCompleted {
			get { return printingSystem == null || printingSystem.PrintingDocument.Root.Completed; }
		}
		public void EnsureGroupFooter(DocumentBand documentBand) {
		}
		public void EnsureReportFooterBand(DocumentBand band) {
		}
		public DocumentBand GetBand(DocumentBand rootBand, PageBuildInfo pageBuildInfo) {
			DocumentBand docBand = pageBuildInfo.Index < rootBand.Bands.Count ? rootBand.Bands[pageBuildInfo.Index] : null;
			if(docBand != null)
				return docBand;
			DetailReportBand detailReport = GetNextDetailReport();
			if(detailReport == null)
				return null;
			SelfGeneratedDocumentBand detailReportDocBand = new SelfGeneratedMultiColumnDocumentBand(DocumentBandKind.Detail, detailReport, report.DataBrowser.Position);
			DetailReportDocumentBuilder detailReportDocumentBuilder = new DetailReportDocumentBuilder(detailReport, detailReportDocBand);
			detailReport.WriteToDocument(detailReportDocumentBuilder);
			rootBand.Bands.Add(detailReportDocBand);
			detailReportDocBand.BandManager = detailReportDocumentBuilder;
			int pageBreakIndex = detailReportDocBand.GetPageBreakIndex(0f);
			if(pageBreakIndex >= 0) {
				DocumentBand pageBreakBand = DocumentBand.CreatePageBreakBand(0f);
				rootBand.Bands.Insert(pageBreakBand, rootBand.Bands.Count - 1);
				detailReportDocBand.PageBreaks.RemoveAt(pageBreakIndex);
			}
			pageBreakIndex = detailReportDocBand.GetPageBreakIndex(DocumentBand.MaxBandHeightPix);
			if(pageBreakIndex >= 0) {
				DocumentBand pageBreakBand = DocumentBand.CreatePageBreakBand(0f);
				rootBand.Bands.Add(pageBreakBand);
				detailReportDocBand.PageBreaks.RemoveAt(pageBreakIndex);
			}
			return detailReportDocBand;
		}
		DetailReportBand GetNextDetailReport() {
			while(detailReportBandEnumerator.MoveNext()) {
				DetailReportBand detailReport = (DetailReportBand)detailReportBandEnumerator.Current;
				PrintEventArgs args = new PrintEventArgs();
				detailReport.OnBeforePrint(args);
				if(!args.Cancel)
					return detailReport;
			}
			return null;
		}
		public DocumentBand GetPageFooterBand(DocumentBand band) {
			return null;
		}
		#endregion
	}
}
