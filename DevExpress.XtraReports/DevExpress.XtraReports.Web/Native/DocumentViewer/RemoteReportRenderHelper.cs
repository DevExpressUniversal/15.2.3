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
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Export.Web;
namespace DevExpress.XtraReports.Web.Native.DocumentViewer {
	class RemoteReportRenderHelper : ReportRenderHelper {
		readonly PrintingSystemBase ps;
		protected override int PageCount {
			get { return ps.PageCount; }
		}
		protected override IServiceContainer ServiceContainer {
			get { return ps; }
		}
		public RemoteReportRenderHelper(ReportViewer reportViewer, PrintingSystemBase ps)
			: base(reportViewer) {
			this.ps = ps;
		}
		protected override ReportsPageWebControl CreatePageWebControl(IImageRepository imageRepository, int pageIndex) {
			return new ReportsPageWebControl(ps.Document, imageRepository, pageIndex, ReportViewer.TableLayout, ReportViewer.EnableReportMargins);
		}
		protected override void BeginWriteToString() {
			if(ps.PageCount == 0) {
				throw new InvalidOperationException("PrintingSystem.PageCount");
			}
		}
		protected override HtmlExportOptionsBase CreateHtmlExportOptionsForDocumentBuilder() {
			return new HtmlExportOptions { RemoveSecondarySymbols = false };
		}
	}
}
