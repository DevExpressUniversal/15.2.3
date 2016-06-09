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
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.UI;
using System.Collections;
using DevExpress.XtraReports.Native.Data;
using System.Windows.Forms;
using DevExpress.XtraPrinting;
using System.Drawing;
namespace DevExpress.XtraReports.Native.Printing {
	public class DocumentBuilder : DocumentBuilderBase {
		public DocumentBuilder(XtraReportBase report, DocumentBand docBand)
			: base(report, docBand) {
		}
		protected int RowIndex { get { return DetailWriter.RowIndex; } }
		DetailWriterBase detailWriter;
		bool beforeWriteWasCalled;
		bool endOfDataWasReached;
		protected DetailWriterBase DetailWriter {
			get {
				if(detailWriter == null)
					detailWriter = DetailWriterBase.CreateInstance(report, RootBand);
				return detailWriter;
			}
		}
		protected virtual void BuildCore() {
		}
		public override void Build() {
			CreateDocumentBand(BandKind.ReportHeader, RowIndex);
			if(ShouldCreatePageHeader)
				CreateDocumentBand(BandKind.PageHeader, RowIndex);
			BuildCore();
			if(ShouldCreatePageFooter)
				CreateDocumentBand(BandKind.PageFooter, RowIndex);
		}
		public override DocumentBand GetBand(DocumentBand rootBand, PageBuildInfo pageBuildInfo) {
			DocumentBand docBand = pageBuildInfo.Index < rootBand.Bands.Count ? rootBand.Bands[pageBuildInfo.Index] : null;
			if(docBand != null && docBand.IsKindOf(DocumentBandKind.Detail, DocumentBandKind.Header, DocumentBandKind.Footer, DocumentBandKind.ReportHeader) && !docBand.IsPageBand(DocumentBandKind.Footer))
				return docBand;
			if(endOfDataWasReached)
				return docBand != null && docBand.IsKindOf(DocumentBandKind.ReportFooter) ? docBand : null;
			if(!DetailWriter.EndOfData()) {
				if(!beforeWriteWasCalled) {
					beforeWriteWasCalled = true;
					DetailWriter.BeforeWrite();
				}
				DetailWriter.Write(rootBand, pageBuildInfo);
				return pageBuildInfo.Index < rootBand.Bands.Count ? rootBand.Bands[pageBuildInfo.Index] : null;
			}
			EnsureGroupFooter(rootBand);
			if(rootBand.Parent == RootBand) {
				endOfDataWasReached = true;
				DetailWriter.AfterWrite();
				EnsureReportFooterBand(RootBand);
				report.RootReport.Summaries.OnReportFinished(report);
				if(!RootBand.IsRoot)
					report.OnAfterPrint(new System.Drawing.Printing.PrintEventArgs());
			}
			return pageBuildInfo.Index < rootBand.Bands.Count ? rootBand.Bands[pageBuildInfo.Index] : null;
		}
		public override void EnsureGroupFooter(DocumentBand documentBand) {
			if(DetailWriter.ShouldWriteGroupFooters(documentBand))
				DetailWriter.Write(documentBand, null);
		}
		public override DocumentBand GetPageFooterBand(DocumentBand band) {
			DocumentBand docBand = band.GetPageBand(DocumentBandKind.Footer);
			if(docBand == null)
				return DetailWriter.GetPageFooterBand(band);
			return docBand;
		}
	}
}
