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
using System.Drawing;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraReports.Native.Printing {
	public abstract class DocumentBuilderBase : IBandManager {
		protected XtraReportBase report;
		DocumentBand rootBand;
		public DocumentBand RootBand { 
			get { return rootBand; } 
		}
		public PrintingSystemBase PrintingSystem {
			get;
			private set;
		}
		bool IBandManager.IsCompleted {
			get { return PrintingSystem == null || PrintingSystem.PrintingDocument.Root.Completed; }
		}
		protected DocumentBuilderBase(XtraReportBase report, DocumentBand rootBand) {
			if(report == null || rootBand == null)
				throw new ArgumentException();
			this.rootBand = rootBand;
			this.report = report;
			PrintingSystem = report.RootReport.GetCreatingPrintingSystem();
		}
		protected virtual bool ShouldCreatePageHeader {
			get { return true; }
		}
		protected virtual bool ShouldCreatePageFooter {
			get { return true; }
		}
		public abstract void Build();
		public abstract DocumentBand GetBand(DocumentBand band, PageBuildInfo pageBuildInfo);
		public virtual void EnsureGroupFooter(DocumentBand documentBand) { 
		}
		public virtual DocumentBand GetPageFooterBand(DocumentBand band) {
			return band.GetPageBand(DocumentBandKind.Footer);
		}
		public void EnsureReportFooterBand(DocumentBand documentBand) {
			if(documentBand == RootBand && documentBand.GetBand(DocumentBandKind.ReportFooter) == null) {
				DocumentBand pageFooterBand = documentBand.GetPageBand(DocumentBandKind.Footer);
				Band item = report.Bands[BandKind.ReportFooter];
				DocumentBand reportFooterBand = item != null ? item.CreateDocumentBand(PrintingSystem, report.DataBrowser.Count - 1, PageBuildInfo.Empty) :
					BandKind.ReportFooter.ToEmptyDocumentBand();
				if(pageFooterBand != null)
					documentBand.Bands.Insert(reportFooterBand, pageFooterBand.Index);
				else
					documentBand.Bands.Add(reportFooterBand);
			}
		}
		protected DocumentBand CreateDocumentBand(BandKind bandKind, int rowIndex) {
			Band item = report.Bands[bandKind];
			DocumentBand band = item != null ? item.CreateDocumentBand(PrintingSystem, rowIndex, PageBuildInfo.Empty) :
				bandKind.ToEmptyDocumentBand();
			rootBand.Bands.Add(band);
			return band;
		}
	}
}
