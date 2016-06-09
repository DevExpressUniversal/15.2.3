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

using DevExpress.Printing.Core.HtmlExport;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Export;
using DevExpress.XtraPrinting.Export.Web;
using DevExpress.XtraPrinting.InternalAccess;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraReports.Web.Native {
	public class ReportsPageWebControl : SinglePageWebControl {
		readonly bool enableReportMargins;
		public ReportsPageWebControl(Document document, IImageRepository imageRepository, int pageIndex, bool tableLayout, bool enableReportMargins)
			: base(document, imageRepository, pageIndex, tableLayout) {
			this.enableReportMargins = enableReportMargins;
			DocumentAccessor.LoadPage(document, pageIndex);
		}
		protected override bool NeedClipMargins { get { return !enableReportMargins; } }
		protected override HtmlExportContext CreateExportContext(PrintingSystemBase printingSystem, IScriptContainer scriptContainer, IImageRepository imageRepository, HtmlExportMode mode) {
			return new HtmlPageWebExportContext(printingSystem, scriptContainer, imageRepository);
		}
		protected override ScriptBlockControl CreateScriptControl(WebStyleControl styleControl) {
			return new WebScriptBlockControl(styleControl);
		}
		protected override HtmlPageLayoutBuilder CreateHtmlPageLayoutBuilder(Page page, HtmlExportContext htmlExportContext) {
			return enableReportMargins
				? base.CreateHtmlPageLayoutBuilder(page, htmlExportContext)
				: new ReportsHtmlPageLayoutBuilder(page, htmlExportContext);
		}
	}
}
