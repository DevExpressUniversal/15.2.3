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

extern alias Platform;
using System;
using Platform::DevExpress.XtraPrinting;
using Platform::DevExpress.Xpf.Printing;
using Microsoft.Windows.Design.Model;
using Platform::DevExpress.Xpf.Bars;
using Platform::DevExpress.Xpf.Core.Native;
using System.Windows;
using DevExpress.Xpf.Core.Design;
namespace DevExpress.Xpf.Printing.Design.Bars {
	public class BarInfos {
#if !SILVERLIGHT
		#region File
		public static BarInfo FileGroup { get { return fileGroup; } }
		static readonly BarInfo fileGroup = new BarInfo(
			"",
			PrintingLocalizer.GetString(PrintingStringId.RibbonPageCaption),
			PrintingLocalizer.GetString(PrintingStringId.RibbonPageGroup_File),
			new BarInfoItems(
				new string[] { PrintingSystemCommand.Open.ToString(), PrintingSystemCommand.Save.ToString() },
				new BarItemInfo[] { BarItemInfos.BarButtonItem, BarItemInfos.BarButtonItem }),
				"", "", PrintingStringId.RibbonPageCaption.ToString(), "RibbonPageGroup_File");
		#endregion
#endif
		#region Print
		public static BarInfo PrintGroup { get { return printGroup; } }
		static readonly BarInfo printGroup = new BarInfo(
			"",
			PrintingLocalizer.GetString(PrintingStringId.RibbonPageCaption),
			PrintingLocalizer.GetString(PrintingStringId.RibbonPageGroup_Print),
			new BarInfoItems(
				new string[] { 
#if !SILVERLIGHT
					PrintingSystemCommand.Print.ToString(), PrintingSystemCommand.PrintDirect.ToString(), 
#else
					PrintingSystemCommand.Print.ToString(),
#endif
					PrintingSystemCommand.PageSetup.ToString(), PrintingSystemCommand.Scale.ToString() },
				new BarItemInfo[] { 
#if !SILVERLIGHT
					BarItemInfos.BarButtonItem, BarItemInfos.BarButtonItem, 
#else
					new BarSplitButtonSubItemInfo(new BarInfoItems(
						new string[] { "PrintSubItem", PrintingSystemCommand.PrintPdf.ToString() },
						new BarItemInfo[] { BarItemInfos.BarButtonItem, BarItemInfos.BarButtonItem })),
#endif
					BarItemInfos.BarButtonItem, BarItemInfos.BarButtonItem }),
				"", "", PrintingStringId.RibbonPageCaption.ToString(), "RibbonPageGroup_Print");
		#endregion
		#region Navigation
		public static BarInfo NavigationGroup { get { return navigationGroup; } }
		static readonly BarInfo navigationGroup = new BarInfo(
			"",
			PrintingLocalizer.GetString(PrintingStringId.RibbonPageCaption),
			PrintingLocalizer.GetString(PrintingStringId.RibbonPageGroup_Navigation),
			new BarInfoItems(
				new string[] { PrintingSystemCommand.ShowFirstPage.ToString(), PrintingSystemCommand.ShowPrevPage.ToString(), PrintingSystemCommand.ShowNextPage.ToString(), PrintingSystemCommand.ShowLastPage.ToString() },
				new BarItemInfo[] { BarItemInfos.BarButtonItem, BarItemInfos.BarButtonItem, BarItemInfos.BarButtonItem, BarItemInfos.BarButtonItem }),
				"", "", PrintingStringId.RibbonPageCaption.ToString(), "RibbonPageGroup_Navigation");
		#endregion
		#region Zoom Ribbon
		public static BarInfo ZoomGroup { get { return zoomGroup; } }
		static readonly BarInfo zoomGroup = new BarInfo(
			"",
			PrintingLocalizer.GetString(PrintingStringId.RibbonPageCaption),
			PrintingLocalizer.GetString(PrintingStringId.RibbonPageGroup_Zoom),
			new BarInfoItems(
				new string[] { 
					PrintingSystemCommand.ZoomOut.ToString(), 
					"ZoomMode",
					PrintingSystemCommand.ZoomIn.ToString() },
				new BarItemInfo[] { 
					BarItemInfos.BarButtonItem,
					new RibbonZoomModeItemInfo(),
					BarItemInfos.BarButtonItem }),
				"", "", PrintingStringId.RibbonPageCaption.ToString(), "RibbonPageGroup_Zoom");
		#endregion
		#region Zoom bars
		public static BarInfo ZoomBarsGroup { get { return zoomBarsGroup; } }
		static readonly BarInfo zoomBarsGroup = new BarInfo(
			"",
			PrintingLocalizer.GetString(PrintingStringId.RibbonPageCaption),
			PrintingLocalizer.GetString(PrintingStringId.RibbonPageGroup_Zoom),
			new BarInfoItems(
				new string[] { 
					PrintingSystemCommand.ZoomOut.ToString(), 
					"BarsZoomMode",
					PrintingSystemCommand.ZoomIn.ToString() },
				new BarItemInfo[] { 
					BarItemInfos.BarButtonItem,
					new BarsZoomModeItemInfo(),
					BarItemInfos.BarButtonItem }),
				"", "", PrintingStringId.RibbonPageCaption.ToString(), "RibbonPageGroup_Zoom");
		#endregion
		#region Export
		public static BarInfo ExportGroup { get { return exportGroup; } }
		static readonly BarInfo exportGroup = new BarInfo(
			"",
			PrintingLocalizer.GetString(PrintingStringId.RibbonPageCaption),
			PrintingLocalizer.GetString(PrintingStringId.RibbonPageGroup_Export),
			new BarInfoItems(
				new string[] { PrintingSystemCommand.ExportFile.ToString(), 
#if !SILVERLIGHT
					PrintingSystemCommand.SendFile.ToString()
#else
					PrintingSystemCommand.ExportFileToWindow.ToString()
#endif
				},
				new BarItemInfo[] { 
					new BarSplitButtonSubItemInfo(
					new BarInfoItems( 
						new string[] {
						PrintingSystemCommand.ExportPdf.ToString(),
						PrintingSystemCommand.ExportHtm.ToString(),
						PrintingSystemCommand.ExportMht.ToString(),
						PrintingSystemCommand.ExportRtf.ToString(),
						PrintingSystemCommand.ExportXls.ToString(),
						PrintingSystemCommand.ExportXlsx.ToString(),
						PrintingSystemCommand.ExportCsv.ToString(),
						PrintingSystemCommand.ExportTxt.ToString(),
						PrintingSystemCommand.ExportGraphic.ToString(),
						PrintingSystemCommand.ExportXps.ToString(),
					}, new BarItemInfo[] { 
						BarItemInfos.BarButtonItem, BarItemInfos.BarButtonItem, 
						BarItemInfos.BarButtonItem, BarItemInfos.BarButtonItem,
						BarItemInfos.BarButtonItem, BarItemInfos.BarButtonItem, 
						BarItemInfos.BarButtonItem, BarItemInfos.BarButtonItem, 
						BarItemInfos.BarButtonItem, BarItemInfos.BarButtonItem})
					),
					new BarSplitButtonSubItemInfo(
					new BarInfoItems( 
						new string[] {
#if !SILVERLIGHT
						PrintingSystemCommand.SendPdf.ToString(),
						PrintingSystemCommand.SendMht.ToString(),
						PrintingSystemCommand.SendRtf.ToString(),
						PrintingSystemCommand.SendXls.ToString(),
						PrintingSystemCommand.SendXlsx.ToString(),
						PrintingSystemCommand.SendCsv.ToString(),
						PrintingSystemCommand.SendTxt.ToString(),
						PrintingSystemCommand.SendGraphic.ToString(),
						PrintingSystemCommand.SendXps.ToString(),
#else
						"ExportPdfToWindow",
						"ExportHtmToWindow",
						"ExportMhtToWindow",
						"ExportRtfToWindow",
						"ExportXlsToWindow",
						"ExportXlsxToWindow",
						"ExportCsvToWindow",
						"ExportTxtToWindow",
						"ExportGraphicToWindow",
						"ExportXpsToWindow",
#endif
					}, new BarItemInfo[] { 
						BarItemInfos.BarButtonItem, BarItemInfos.BarButtonItem,
						BarItemInfos.BarButtonItem, BarItemInfos.BarButtonItem,
						BarItemInfos.BarButtonItem, BarItemInfos.BarButtonItem, 
						BarItemInfos.BarButtonItem, BarItemInfos.BarButtonItem, 
						BarItemInfos.BarButtonItem, 
#if SILVERLIGHT
						BarItemInfos.BarButtonItem
#endif
					})
					),
				}),
				"", "", PrintingStringId.RibbonPageCaption.ToString(), "RibbonPageGroup_Export");
		#endregion
		#region Document
		public static BarInfo DocumentGroup { get { return documentGroup; } }
		static readonly BarInfo documentGroup = new BarInfo(
			"",
			PrintingLocalizer.GetString(PrintingStringId.RibbonPageCaption),
			PrintingLocalizer.GetString(PrintingStringId.RibbonPageGroup_Document),
			new BarInfoItems(
				new string[] { PrintingSystemCommand.Parameters.ToString(), PrintingSystemCommand.DocumentMap.ToString(), PrintingSystemCommand.Find.ToString() },
				new BarItemInfo[] { BarItemInfos.BarCheckItem, BarItemInfos.BarCheckItem, BarItemInfos.BarCheckItem }),
				"", "", PrintingStringId.RibbonPageCaption.ToString(), "RibbonPageGroup_Document");
		#endregion
		#region Watermark
		public static BarInfo WatermarkGroup { get { return watermarkGroup; } }
		static readonly BarInfo watermarkGroup = new BarInfo(
			"",
			PrintingLocalizer.GetString(PrintingStringId.RibbonPageCaption),
			PrintingLocalizer.GetString(PrintingStringId.RibbonPageGroup_Watermark),
			new BarInfoItems(
				new string[] { PrintingSystemCommand.Watermark.ToString() },
				new BarItemInfo[] { BarItemInfos.BarButtonItem }),
				"", "", PrintingStringId.RibbonPageCaption.ToString(), "RibbonPageGroup_Watermark");
		#endregion
		#region StatusBar
		public static StatusBarInfo StatusBarGroup { get { return statusBarGroup; } }
		static readonly StatusBarInfo statusBarGroup = new StatusBarInfo(
			new BarInfoItems(
				new string[] { "PageNumber", "Progress", "MarqueeProgress", PrintingSystemCommand.StopPageBuilding.ToString() },
				new BarItemInfo[] { new PageNumberItemInfo(), new ProgressItemInfo(ProgressItemInfo.ProgressBarType.Normal), new ProgressItemInfo(ProgressItemInfo.ProgressBarType.Indeterminate), new StopButtonItemInfo() }),
			new BarInfoItems(
				new string[] { "ZoomFactor" },
				new BarItemInfo[] { new ZoomFactorItemInfo() }) 
				);
		#endregion
	}
}
