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
using System.Collections;
using System.ComponentModel;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Web.Design;
using DevExpress.XtraReports.Web;
using DevExpress.XtraReports.Web.DocumentViewer.Ribbon;
using DevExpress.XtraReports.Web.DocumentViewer.Ribbon.Native;
using DevExpress.XtraReports.Web.Localization;
namespace DevExpress.Web.DocumentViewer.Design {
	public class DocumentViewerRibbonItemsOwner : RibbonItemsOwner {
		public DocumentViewerRibbonItemsOwner(object component, IServiceProvider provider, IList items)
			: base(component, provider, items) {
		}
		public DocumentViewerRibbonItemsOwner(Collection items)
			: base(null, null, items) {
		}
		protected override void FillItemTypes() {
			AddItemType(typeof(DocumentViewerHomeRibbonTab), ASPxReportsLocalizer.GetString(ASPxReportsStringId.DocumentViewer_RibbonHomeTabText) + " Tab", TabControlItemImageResource);
			AddItemType(typeof(DocumentViewerPrintRibbonGroup), ASPxReportsLocalizer.GetString(ASPxReportsStringId.DocumentViewer_RibbonPrintGroupText) + " Group", RibbonGroupItemImageResource);
			AddItemType(typeof(DocumentViewerNavigationRibbonGroup), ASPxReportsLocalizer.GetString(ASPxReportsStringId.DocumentViewer_RibbonNavigationGroupText) + " Group", RibbonGroupItemImageResource);
			AddItemType(typeof(DocumentViewerExportRibbonGroup), ASPxReportsLocalizer.GetString(ASPxReportsStringId.DocumentViewer_RibbonExportGroupText) + " Group", RibbonGroupItemImageResource);
			AddItemType(typeof(DocumentViewerPrintReportRibbonCommand), "Print Report", RibbonButtonItemImageResource);
			AddItemType(typeof(DocumentViewerPrintPageRibbonCommand), "Print Page", RibbonButtonItemImageResource);
			AddItemType(typeof(DocumentViewerSearchRibbonCommand), "Find Text", RibbonButtonItemImageResource);
			AddItemType(typeof(DocumentViewerFirstPageRibbonCommand), "First Page", RibbonButtonItemImageResource);
			AddItemType(typeof(DocumentViewerPreviousPageRibbonCommand), "Previous Page", RibbonButtonItemImageResource);
			AddItemType(typeof(DocumentViewerPageNumbersTemplateRibbonCommand), "Page Numbers", RibbonTemplateItemImageResource);
			AddItemType(typeof(DocumentViewerNextPageRibbonCommand), "Next Page", RibbonButtonItemImageResource);
			AddItemType(typeof(DocumentViewerLastPageRibbonCommand), "Last Page", RibbonButtonItemImageResource);
			AddItemType(typeof(DocumentViewerSaveToDiskDropDownRibbonCommand), "Save To File...", DropDownButtonImageResource);
			AddItemType(typeof(DocumentViewerSaveToWindowDropDownRibbonCommand), "Save To Window...", DropDownButtonImageResource);
			AddItemType(typeof(RibbonPdfFormatCommand), "Pdf", DropDownButtonImageResource);
			AddItemType(typeof(RibbonXlsFormatCommand), "Xls", DropDownButtonImageResource);
			AddItemType(typeof(RibbonXlsxFormatCommand), "Xlsx", DropDownButtonImageResource);
			AddItemType(typeof(RibbonRtfFormatCommand), "Rtf", DropDownButtonImageResource);
			AddItemType(typeof(RibbonMhtFormatCommand), "Mht", DropDownButtonImageResource);
			AddItemType(typeof(RibbonHtmlFormatCommand), "Html", DropDownButtonImageResource);
			AddItemType(typeof(RibbonTextFormatCommand), "Text ", DropDownButtonImageResource);
			AddItemType(typeof(RibbonCsvFormatCommand), "Csv", DropDownButtonImageResource);
			AddItemType(typeof(RibbonPngFormatCommand), "Image", DropDownButtonImageResource);
		}
	}
}
