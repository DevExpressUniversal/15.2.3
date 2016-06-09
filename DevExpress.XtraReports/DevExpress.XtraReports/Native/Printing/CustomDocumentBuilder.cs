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
using System.Drawing;
using DevExpress.XtraPrinting;
using System.Windows.Forms;
using DevExpress.Utils;
namespace DevExpress.XtraReports.Native.Printing {
	public class CustomDocumentBuilder : DocumentBuilderBase {
		public CustomDocumentBuilder(CustomReportBase report, PSDocument doc)
			: base(report, doc.Root) {
		}
		CustomReportBase Report {
			get { return (CustomReportBase)report; }
		}
		public override void Build() {
			CreateDocumentBand(BandKind.TopMargin, 0);
			CreateDocumentBand(BandKind.ReportHeader, 0);
			CreateDocumentBand(BandKind.PageHeader, 0);
			DocumentBandContainer documentBandContainer = new DocumentBandContainer();
			documentBandContainer.BandManager = this;
			RootBand.Bands.Add(documentBandContainer);
			CreateDocumentBand(BandKind.ReportFooter, 0);
			CreateDocumentBand(BandKind.PageFooter, 0);
			CreateDocumentBand(BandKind.BottomMargin, 0);
		}
		public override DocumentBand GetBand(DocumentBand band, PageBuildInfo pageBuildInfo) {
			if(pageBuildInfo.Index >= 0 && pageBuildInfo.Index < band.Bands.Count)
				return band.Bands[pageBuildInfo.Index];
			if(band is DocumentBandContainer && Report.ShouldCreateDetail(pageBuildInfo.Index)) {
				Band item = report.Bands[BandKind.Detail];
				DocumentBand docBand = item != null ? item.CreateDocumentBand(PrintingSystem, 0, pageBuildInfo) : 
					BandKind.Detail.ToEmptyDocumentBand();
				band.Bands.Insert(docBand, pageBuildInfo.Index);
				return band.Bands[pageBuildInfo.Index];
			}
			return null;
		}
	}
	public class CustomPSDocument : PSDocument {
		public CustomPSDocument(PrintingSystemBase ps, Action0 afterBuildPages)
			: base(ps, afterBuildPages) {
		}
		protected override PageBuildEngine CreatePageBuildEngine(bool buildPagesInBackground, bool rollPaper) {
			return buildPagesInBackground ? base.CreatePageBuildEngine(true, rollPaper) : new CustomPageBuildEngine(this);
		}
	}
	public class CustomPageBuildEngine : PageBuildEngine {
		public CustomPageBuildEngine(PrintingDocument document)
			: base(document.Root, document) { 
		}
		protected override PageRowBuilder CreatePageRowBuilder(YPageContentEngine psPage) {
			return new PageHeaderFooterRowBuilderBase(psPage);
		}
	}
}
