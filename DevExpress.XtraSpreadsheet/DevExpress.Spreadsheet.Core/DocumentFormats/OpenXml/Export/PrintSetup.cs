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
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Office;
using System.Globalization;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		protected internal virtual void GeneratePrintSetupContent(PrintSetup printSetup) {
			if (printSetup.IsDefault())
				return;
			WriteShStartElement("pageSetup");
			try {
				PrintSetupInfo defaultInfo = Workbook.Cache.PrintSetupInfoCache.DefaultItem;
				if (defaultInfo.PaperKind != printSetup.PaperKind)
					WriteIntValue("paperSize", (int)printSetup.PaperKind);
				if (defaultInfo.CommentsPrintMode != printSetup.CommentsPrintMode)
					WriteStringValue("cellComments", ConvertCommentsPrintMode(printSetup.CommentsPrintMode));
				if (defaultInfo.ErrorsPrintMode != printSetup.ErrorsPrintMode)
					WriteStringValue("errors", ConvertErrorsPrintMode(printSetup.ErrorsPrintMode));
				if (defaultInfo.PagePrintOrder != printSetup.PagePrintOrder)
					WriteStringValue("pageOrder", ConvertPagePrintOrder(printSetup.PagePrintOrder));
				if (defaultInfo.Orientation != printSetup.Orientation)
					WriteStringValue("orientation", ConvertPageOrientation(printSetup.Orientation));
				if (defaultInfo.Scale != printSetup.Scale)
					WriteIntValue("scale", printSetup.Scale);
				if (defaultInfo.BlackAndWhite != printSetup.BlackAndWhite)
					WriteBoolValue("blackAndWhite", printSetup.BlackAndWhite);
				if (defaultInfo.Draft != printSetup.Draft)
					WriteBoolValue("draft", printSetup.Draft);
				if (defaultInfo.UseFirstPageNumber != printSetup.UseFirstPageNumber)
					WriteBoolValue("useFirstPageNumber", printSetup.UseFirstPageNumber);
				if (defaultInfo.UsePrinterDefaults != printSetup.UsePrinterDefaults)
					WriteBoolValue("usePrinterDefaults", printSetup.UsePrinterDefaults);
				if (defaultInfo.Copies != printSetup.Copies)
					WriteIntValue("copies", printSetup.Copies);
				if (defaultInfo.FirstPageNumber != printSetup.FirstPageNumber)
					WriteIntValue("firstPageNumber", printSetup.FirstPageNumber);
				if (defaultInfo.FitToWidth != printSetup.FitToWidth)
					WriteIntValue("fitToWidth", printSetup.FitToWidth);
				if (defaultInfo.FitToHeight != printSetup.FitToHeight)
					WriteIntValue("fitToHeight", printSetup.FitToHeight);
				if (defaultInfo.HorizontalDpi != printSetup.HorizontalDpi)
					WriteIntValue("horizontalDpi", printSetup.HorizontalDpi);
				if (defaultInfo.VerticalDpi != printSetup.VerticalDpi)
					WriteIntValue("verticalDpi", printSetup.VerticalDpi);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected bool ShouldExportPrintOptionsContent(PrintSetup printSetup) {
			PrintSetupInfo defaultInfo = Workbook.Cache.PrintSetupInfoCache.DefaultItem;
			return !(printSetup.CenterHorizontally == defaultInfo.CenterHorizontally &&
				printSetup.CenterVertically == defaultInfo.CenterVertically &&
				printSetup.PrintHeadings == defaultInfo.PrintHeadings &&
				printSetup.PrintGridLines == defaultInfo.PrintGridLines &&
				printSetup.PrintGridLinesSet == defaultInfo.PrintGridLinesSet);
		}
		protected internal void GeneratePrintOptionsContent(PrintSetup printSetup) {
			if (!ShouldExportPrintOptionsContent(printSetup))
				return;
			PrintSetupInfo defaultInfo = Workbook.Cache.PrintSetupInfoCache.DefaultItem;
			WriteShStartElement("printOptions");
			try {
				if (defaultInfo.CenterHorizontally != printSetup.CenterHorizontally)
					WriteBoolValue("horizontalCentered", printSetup.CenterHorizontally);
				if (defaultInfo.CenterVertically != printSetup.CenterVertically)
					WriteBoolValue("verticalCentered", printSetup.CenterVertically);
				if (defaultInfo.PrintHeadings != printSetup.PrintHeadings)
					WriteBoolValue("headings", printSetup.PrintHeadings);
				if (defaultInfo.PrintGridLines != printSetup.PrintGridLines)
					WriteBoolValue("gridLines", printSetup.PrintGridLines);
				if (defaultInfo.PrintGridLinesSet != printSetup.PrintGridLinesSet)
					WriteBoolValue("gridLinesSet", printSetup.PrintGridLinesSet);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected string ConvertCommentsPrintMode(ModelCommentsPrintMode value) {
			return commentsPrintModeTable[value];
		}
		protected string ConvertErrorsPrintMode(ModelErrorsPrintMode value) {
			return errorsPrintModeTable[value];
		}
		protected string ConvertPagePrintOrder(PagePrintOrder value) {
			return pagePrintOrderTable[value];
		}
		protected string ConvertPageOrientation(ModelPageOrientation value) {
			return pageOrientationTable[value];
		}
		#region Translation tables
		internal static readonly Dictionary<ModelCommentsPrintMode, string> commentsPrintModeTable = CreateCommentsPrintModeTable();
		internal static readonly Dictionary<ModelErrorsPrintMode, string> errorsPrintModeTable = CreateErrorsPrintModeTable();
		internal static readonly Dictionary<PagePrintOrder, string> pagePrintOrderTable = CreatePagePrintOrderTable();
		internal static readonly Dictionary<ModelPageOrientation, string> pageOrientationTable = CreatePageOrientationTable();
		static Dictionary<ModelCommentsPrintMode, string> CreateCommentsPrintModeTable() {
			Dictionary<ModelCommentsPrintMode, string> result = new Dictionary<ModelCommentsPrintMode, string>();
			result.Add(ModelCommentsPrintMode.None, "none");
			result.Add(ModelCommentsPrintMode.AtEnd, "atEnd");
			result.Add(ModelCommentsPrintMode.AsDisplayed, "asDisplayed");
			return result;
		}
		static Dictionary<ModelErrorsPrintMode, string> CreateErrorsPrintModeTable() {
			Dictionary<ModelErrorsPrintMode, string> result = new Dictionary<ModelErrorsPrintMode, string>();
			result.Add(ModelErrorsPrintMode.Displayed, "displayed");
			result.Add(ModelErrorsPrintMode.Dash, "dash");
			result.Add(ModelErrorsPrintMode.Blank, "blank");
			result.Add(ModelErrorsPrintMode.NA, "NA");
			return result;
		}
		static Dictionary<PagePrintOrder, string> CreatePagePrintOrderTable() {
			Dictionary<PagePrintOrder, string> result = new Dictionary<PagePrintOrder, string>();
			result.Add(PagePrintOrder.DownThenOver, "downThenOver");
			result.Add(PagePrintOrder.OverThenDown, "overThenDown");
			return result;
		}
		static Dictionary<ModelPageOrientation, string> CreatePageOrientationTable() {
			Dictionary<ModelPageOrientation, string> result = new Dictionary<ModelPageOrientation, string>();
			result.Add(ModelPageOrientation.Default, "default");
			result.Add(ModelPageOrientation.Portrait, "portrait");
			result.Add(ModelPageOrientation.Landscape, "landscape");
			return result;
		}
		#endregion
	}
}
