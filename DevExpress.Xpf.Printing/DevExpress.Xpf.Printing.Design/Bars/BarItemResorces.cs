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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Platform::DevExpress.Xpf.Printing;
using Platform::DevExpress.XtraPrinting;
namespace DevExpress.Xpf.Printing.Design.Bars {
	public class BarItemResorces {
		#region inner classes
		public class BarItemResource {
			public string IsCheckedString { get; set; }
			public string IsVisibleString { get; set; }
			public string IsEnabled { get; set; }
			public string Glyph { get; private set; }
			public string LargeGlyph { get; set; }
			public string Content { get; private set; }
			public string Hint { get; set; }
			public string Command { get; private set; }
			public string CommandParameter { get; set; }
			public string ItemsSource { get; set; }
			public string[] Items { get; set; }
			public bool IsStatusBarItem { get; set; }
			public bool ShowScreenTip { get; set; }
			public string EditValue { get; set; }
			public bool EditValueOneWay { get; set; }
			public BarItemResource (string command, string content, string glyph) {
				this.IsStatusBarItem = false;
				this.ShowScreenTip = true;
				this.Command = command;
				this.Content = content;
				this.Glyph = glyph;
			}
		}
		#endregion
		#region fileds and properties
		readonly static Dictionary<string, BarItemResource> resources = new Dictionary<string, BarItemResource>();
		public BarItemResource this[string key] {
			get {
				return resources.ContainsKey(key) ? resources[key] : null;
			}
		}
		#endregion
		static BarItemResorces() {
			CollectResources();
		}
		static void CollectResources() {
			resources["Open"] = new BarItemResource("Model.OpenCommand", PrintingStringId.Open.ToString(), "Images/BarItems/Open_16x16.png") { Hint = PrintingStringId.Open_Hint.ToString(), LargeGlyph = "Images/BarItems/Open_32x32.png" };
			resources["Save"] = new BarItemResource("Model.SaveCommand", PrintingStringId.Save.ToString(), "Images/BarItems/Save_16x16.png") { Hint = PrintingStringId.Save_Hint.ToString(), LargeGlyph = "Images/BarItems/Save_32x32.png" };
			resources["Print"] = new BarItemResource("Model.PrintCommand", PrintingStringId.Print.ToString(), "Images/BarItems/PrintDialog_16x16.png") { Hint = PrintingStringId.Print_Hint.ToString(), LargeGlyph = "Images/BarItems/PrintDialog_32x32.png" };
#if !SILVERLIGHT
			resources["PrintDirect"] = new BarItemResource("Model.PrintDirectCommand", PrintingStringId.PrintDirect.ToString(), "Images/BarItems/Print_16x16.png") { Hint = PrintingStringId.PrintDirect_Hint.ToString(), LargeGlyph = "Images/BarItems/Print_32x32.png" };
#else
			resources["PrintSubItem"] = new BarItemResource("Model.PrintCommand", PrintingStringId.Print.ToString(), "Images/BarItems/PrintDialog_16x16.png") { Hint = PrintingStringId.Print_Hint.ToString(), LargeGlyph = "Images/BarItems/PrintDialog_32x32.png" };
			resources["PrintPdf"] = new BarItemResource("Model.PrintPdfCommand", PrintingStringId.PrintPdf.ToString(), "Images/BarItems/PrintPdf_16x16.png") { Hint = PrintingStringId.PrintPdf_Hint.ToString(), LargeGlyph = "Images/BarItems/PrintPdf_32x32.png" };
#endif
			resources["PageSetup"] = new BarItemResource("Model.PageSetupCommand", PrintingStringId.PageSetup.ToString(), "Images/BarItems/PageSetup_16x16.png") { Hint = PrintingStringId.PageSetup_Hint.ToString(), LargeGlyph = "Images/BarItems/PageSetup_32x32.png" };
			resources["Scale"] = new BarItemResource("Model.ScaleCommand", PrintingStringId.Scaling.ToString(), "Images/BarItems/Scaling_16x16.png") { Hint = PrintingStringId.Scaling_Hint.ToString(), LargeGlyph = "Images/BarItems/Scaling_32x32.png", IsVisibleString = "Model.IsScaleVisible" };
			resources["ZoomOut"] = new BarItemResource("Model.ZoomOutCommand", PrintingStringId.DecreaseZoom.ToString(), "Images/BarItems/ZoomOut_16x16.png") { Hint = PrintingStringId.DecreaseZoom_Hint.ToString(), LargeGlyph = "Images/BarItems/ZoomOut_32x32.png" };
			resources["ZoomMode"] = new BarItemResource(null, PrintingStringId.Zoom.ToString(), "Images/BarItems/Zoom_16x16.png") { Hint = PrintingStringId.Zoom_Hint.ToString(), LargeGlyph = "Images/BarItems/Zoom_32x32.png" };
			resources["BarsZoomMode"] = new BarItemResource(null, null, "Images/BarItems/Zoom_16x16.png") { Hint = PrintingStringId.Zoom_Hint.ToString(), LargeGlyph = "Images/BarItems/Zoom_32x32.png" };
			resources["ZoomIn"] = new BarItemResource("Model.ZoomInCommand", PrintingStringId.IncreaseZoom.ToString(), "Images/BarItems/ZoomIn_16x16.png") { Hint = PrintingStringId.IncreaseZoom_Hint.ToString(), LargeGlyph = "Images/BarItems/ZoomIn_32x32.png" };
			resources["ShowFirstPage"] = new BarItemResource("Model.FirstPageCommand", PrintingStringId.FirstPage.ToString(), "Images/BarItems/First_16x16.png") { Hint = PrintingStringId.FirstPage_Hint.ToString(), LargeGlyph = "Images/BarItems/First_32x32.png" };
			resources["ShowLastPage"] = new BarItemResource("Model.LastPageCommand", PrintingStringId.LastPage.ToString(), "Images/BarItems/Last_16x16.png") { Hint = PrintingStringId.LastPage_Hint.ToString(), LargeGlyph = "Images/BarItems/Last_32x32.png" };
			resources["ShowNextPage"] = new BarItemResource("Model.NextPageCommand", PrintingStringId.NextPage.ToString(), "Images/BarItems/Next_16x16.png") { Hint = PrintingStringId.NextPage_Hint.ToString(), LargeGlyph = "Images/BarItems/Next_32x32.png" };
			resources["ShowPrevPage"] = new BarItemResource("Model.PreviousPageCommand", PrintingStringId.PreviousPage.ToString(), "Images/BarItems/Prev_16x16.png") { Hint = PrintingStringId.PreviousPage_Hint.ToString(), LargeGlyph = "Images/BarItems/Prev_32x32.png" };
			resources["Watermark"] = new BarItemResource("Model.WatermarkCommand", PrintingStringId.Watermark.ToString(), "Images/BarItems/Watermark_16x16.png") { Hint = PrintingStringId.Watermark_Hint.ToString(), LargeGlyph = "Images/BarItems/Watermark_32x32.png", IsEnabled = "Model.IsSetWatermarkVisible" };
			resources["ExportPdf"] = new BarItemResource("Model.ExportCommand", PrintingStringId.ExportPdf.ToString(), "Images/BarItems/ExportToPDF_16x16.png") { Hint = PrintingStringId.ExportPdf.ToString(), LargeGlyph = "Images/BarItems/ExportToPDF_32x32.png", CommandParameter = "Pdf" };
			resources["ExportHtm"] = new BarItemResource("Model.ExportCommand", PrintingStringId.ExportHtm.ToString(), "Images/BarItems/ExportToHTML_16x16.png") { Hint = PrintingStringId.ExportHtm.ToString(), LargeGlyph = "Images/BarItems/ExportToHTML_32x32.png", CommandParameter = "Htm" };
			resources["ExportMht"] = new BarItemResource("Model.ExportCommand", PrintingStringId.ExportMht.ToString(), "Images/BarItems/ExportToMHT_16x16.png") { Hint = PrintingStringId.ExportMht.ToString(), LargeGlyph = "Images/BarItems/ExportToMHT_32x32.png", CommandParameter = "Mht" };
			resources["ExportRtf"] = new BarItemResource("Model.ExportCommand", PrintingStringId.ExportRtf.ToString(), "Images/BarItems/ExportToRTF_16x16.png") { Hint = PrintingStringId.ExportRtf.ToString(), LargeGlyph = "Images/BarItems/ExportToRTF_32x32.png", CommandParameter = "Rtf" };
			resources["ExportXls"] = new BarItemResource("Model.ExportCommand", PrintingStringId.ExportXls.ToString(), "Images/BarItems/ExportToXLS_16x16.png") { Hint = PrintingStringId.ExportXls.ToString(), LargeGlyph = "Images/BarItems/ExportToXLS_32x32.png", CommandParameter = "Xls" };
			resources["ExportXlsx"] = new BarItemResource("Model.ExportCommand", PrintingStringId.ExportXlsx.ToString(), "Images/BarItems/ExportToXLSX_16x16.png") { Hint = PrintingStringId.ExportXlsx.ToString(), LargeGlyph = "Images/BarItems/ExportToXLSX_32x32.png", CommandParameter = "Xlsx" };
			resources["ExportCsv"] = new BarItemResource("Model.ExportCommand", PrintingStringId.ExportCsv.ToString(), "Images/BarItems/ExportToCSV_16x16.png") { Hint = PrintingStringId.ExportCsv.ToString(), LargeGlyph = "Images/BarItems/ExportToCSV_32x32.png", CommandParameter = "Csv" };
			resources["ExportTxt"] = new BarItemResource("Model.ExportCommand", PrintingStringId.ExportTxt.ToString(), "Images/BarItems/ExportToText_16x16.png") { Hint = PrintingStringId.ExportTxt.ToString(), LargeGlyph = "Images/BarItems/ExportToText_32x32.png", CommandParameter = "Txt" };
			resources["ExportGraphic"] = new BarItemResource("Model.ExportCommand", PrintingStringId.ExportImage.ToString(), "Images/BarItems/ExportToImage_16x16.png") { Hint = PrintingStringId.ExportImage.ToString(), LargeGlyph = "Images/BarItems/ExportToImage_32x32.png", CommandParameter = "Image" };
			resources["ExportXps"] = new BarItemResource("Model.ExportCommand", PrintingStringId.ExportXps.ToString(), "Images/BarItems/ExportToXPS_16x16.png") { Hint = PrintingStringId.ExportXps.ToString(), LargeGlyph = "Images/BarItems/ExportToXPS_32x32.png", CommandParameter = "Xps" };
			resources["ExportFile"] = new BarItemResource("Model.ExportCommand", PrintingStringId.ExportFile.ToString(), "Images/BarItems/Export_16x16.png") { Hint = PrintingStringId.ExportFile_Hint.ToString(), LargeGlyph = "Images/BarItems/Export_32x32.png" };
#if SILVERLIGHT
			resources["ExportPdfToWindow"] = new BarItemResource("Model.ExportToWindowCommand", PrintingStringId.ExportPdfToWindow.ToString(), "Images/BarItems/ExportToPDF_16x16.png") { Hint = PrintingStringId.ExportPdf.ToString(), LargeGlyph = "Images/BarItems/ExportToPDF_32x32.png", CommandParameter = "Pdf" };
			resources["ExportHtmToWindow"] = new BarItemResource("Model.ExportToWindowCommand", PrintingStringId.ExportHtmToWindow.ToString(), "Images/BarItems/ExportToHTML_16x16.png") { Hint = PrintingStringId.ExportHtm.ToString(), LargeGlyph = "Images/BarItems/ExportToHTML_32x32.png", CommandParameter = "Htm" };
			resources["ExportMhtToWindow"] = new BarItemResource("Model.ExportToWindowCommand", PrintingStringId.ExportMhtToWindow.ToString(), "Images/BarItems/ExportToMHT_16x16.png") { Hint = PrintingStringId.ExportMht.ToString(), LargeGlyph = "Images/BarItems/ExportToMHT_32x32.png", CommandParameter = "Mht" };
			resources["ExportRtfToWindow"] = new BarItemResource("Model.ExportToWindowCommand", PrintingStringId.ExportRtfToWindow.ToString(), "Images/BarItems/ExportToRTF_16x16.png") { Hint = PrintingStringId.ExportRtf.ToString(), LargeGlyph = "Images/BarItems/ExportToRTF_32x32.png", CommandParameter = "Rtf" };
			resources["ExportXlsToWindow"] = new BarItemResource("Model.ExportToWindowCommand", PrintingStringId.ExportXlsToWindow.ToString(), "Images/BarItems/ExportToXLS_16x16.png") { Hint = PrintingStringId.ExportXls.ToString(), LargeGlyph = "Images/BarItems/ExportToXLS_32x32.png", CommandParameter = "Xls" };
			resources["ExportXlsxToWindow"] = new BarItemResource("Model.ExportToWindowCommand", PrintingStringId.ExportXlsxToWindow.ToString(), "Images/BarItems/ExportToXLSX_16x16.png") { Hint = PrintingStringId.ExportXlsx.ToString(), LargeGlyph = "Images/BarItems/ExportToXLSX_32x32.png", CommandParameter = "Xlsx" };
			resources["ExportCsvToWindow"] = new BarItemResource("Model.ExportToWindowCommand", PrintingStringId.ExportCsvToWindow.ToString(), "Images/BarItems/ExportToCSV_16x16.png") { Hint = PrintingStringId.ExportCsv.ToString(), LargeGlyph = "Images/BarItems/ExportToCSV_32x32.png", CommandParameter = "Csv" };
			resources["ExportTxtToWindow"] = new BarItemResource("Model.ExportToWindowCommand", PrintingStringId.ExportTxtToWindow.ToString(), "Images/BarItems/ExportToText_16x16.png") { Hint = PrintingStringId.ExportTxt.ToString(), LargeGlyph = "Images/BarItems/ExportToText_32x32.png", CommandParameter = "Txt" };
			resources["ExportGraphicToWindow"] = new BarItemResource("Model.ExportToWindowCommand", PrintingStringId.ExportImageToWindow.ToString(), "Images/BarItems/ExportToImage_16x16.png") { Hint = PrintingStringId.ExportImage.ToString(), LargeGlyph = "Images/BarItems/ExportToImage_32x32.png", CommandParameter = "Image" };
			resources["ExportXpsToWindow"] = new BarItemResource("Model.ExportToWindowCommand", PrintingStringId.ExportXpsToWindow.ToString(), "Images/BarItems/ExportToXPS_16x16.png") { Hint = PrintingStringId.ExportXps.ToString(), LargeGlyph = "Images/BarItems/ExportToXPS_32x32.png", CommandParameter = "Xps" };
			resources["ExportFileToWindow"] = new BarItemResource("Model.ExportToWindowCommand", PrintingStringId.ExportFileToWindow.ToString(), "Images/BarItems/ExportToWindow_16x16.png") { Hint = PrintingStringId.ExportFile_Hint.ToString(), LargeGlyph = "Images/BarItems/ExportToWindow_32x32.png" };
#else
			resources["SendPdf"] = new BarItemResource("Model.SendCommand", PrintingStringId.SendPdf.ToString(), "Images/BarItems/ExportToPDF_16x16.png") { Hint = PrintingStringId.ExportPdf.ToString(), LargeGlyph = "Images/BarItems/ExportToPDF_32x32.png", CommandParameter = "Pdf" };
			resources["SendMht"] = new BarItemResource("Model.SendCommand", PrintingStringId.SendMht.ToString(), "Images/BarItems/ExportToMHT_16x16.png") { Hint = PrintingStringId.ExportMht.ToString(), LargeGlyph = "Images/BarItems/ExportToMHT_32x32.png", CommandParameter = "Mht" };
			resources["SendRtf"] = new BarItemResource("Model.SendCommand", PrintingStringId.SendRtf.ToString(), "Images/BarItems/ExportToRTF_16x16.png") { Hint = PrintingStringId.ExportRtf.ToString(), LargeGlyph = "Images/BarItems/ExportToRTF_32x32.png", CommandParameter = "Rtf" };
			resources["SendXls"] = new BarItemResource("Model.SendCommand", PrintingStringId.SendXls.ToString(), "Images/BarItems/ExportToXLS_16x16.png") { Hint = PrintingStringId.ExportXls.ToString(), LargeGlyph = "Images/BarItems/ExportToXLS_32x32.png", CommandParameter = "Xls" };
			resources["SendXlsx"] = new BarItemResource("Model.SendCommand", PrintingStringId.SendXlsx.ToString(), "Images/BarItems/ExportToXLSX_16x16.png") { Hint = PrintingStringId.ExportXlsx.ToString(), LargeGlyph = "Images/BarItems/ExportToXLSX_32x32.png", CommandParameter = "Xlsx" };
			resources["SendCsv"] = new BarItemResource("Model.SendCommand", PrintingStringId.SendCsv.ToString(), "Images/BarItems/ExportToCSV_16x16.png") { Hint = PrintingStringId.ExportCsv.ToString(), LargeGlyph = "Images/BarItems/ExportToCSV_32x32.png", CommandParameter = "Csv" };
			resources["SendTxt"] = new BarItemResource("Model.SendCommand", PrintingStringId.SendTxt.ToString(), "Images/BarItems/ExportToText_16x16.png") { Hint = PrintingStringId.ExportTxt.ToString(), LargeGlyph = "Images/BarItems/ExportToText_32x32.png", CommandParameter = "Txt" };
			resources["SendGraphic"] = new BarItemResource("Model.SendCommand", PrintingStringId.SendImage.ToString(), "Images/BarItems/ExportToImage_16x16.png") { Hint = PrintingStringId.ExportImage.ToString(), LargeGlyph = "Images/BarItems/ExportToImage_32x32.png", CommandParameter = "Image" };
			resources["SendXps"] = new BarItemResource("Model.SendCommand", PrintingStringId.SendXps.ToString(), "Images/BarItems/ExportToXPS_16x16.png") { Hint = PrintingStringId.ExportXps.ToString(), LargeGlyph = "Images/BarItems/ExportToXPS_32x32.png", CommandParameter = "Xps" };
			resources["SendFile"] = new BarItemResource("Model.SendCommand", PrintingStringId.SendFile.ToString(), "Images/BarItems/Mail_16x16.png") { Hint = PrintingStringId.ExportFile_Hint.ToString(), LargeGlyph = "Images/BarItems/Mail_32x32.png" };
#endif                
			resources["Parameters"] = new BarItemResource("Model.ToggleParametersPanelCommand", PrintingStringId.Parameters.ToString(), "Images/BarItems/Parameters_16x16.png") { Hint = PrintingStringId.Parameters_Hint.ToString(), LargeGlyph = "Images/BarItems/Parameters_32x32.png", IsCheckedString = "Model.IsParametersPanelVisible" };
			resources["Find"] = new BarItemResource("Model.ToggleSearchPanelCommand", PrintingStringId.Search.ToString(), "Images/BarItems/Find_16x16.png") { Hint = PrintingStringId.Search_Hint.ToString(), LargeGlyph = "Images/BarItems/Find_32x32.png", IsCheckedString = "Model.IsSearchPanelVisible", IsVisibleString = "Model.IsSearchVisible" };
			resources["DocumentMap"] = new BarItemResource("Model.ToggleDocumentMapCommand", PrintingStringId.DocumentMap.ToString(), "Images/BarItems/DocumentMap_16x16.png") { Hint = PrintingStringId.DocumentMap_Hint.ToString(), LargeGlyph = "Images/BarItems/DocumentMap_32x32.png", IsCheckedString = "Model.IsDocumentMapVisible" };
			resources["Zoom"] = new BarItemResource(null, PrintingStringId.Zoom.ToString(), "Images/BarItems/Zoom_16x16.png") { Hint = PrintingStringId.Zoom_Hint.ToString(), LargeGlyph = "Images/BarItems/Zoom_32x32.png", ItemsSource = "Model.ZoomModes" };
			resources["PageNumber"] = new BarItemResource(null, PrintingStringId.GoToPage.ToString(), null) { IsEnabled = "Model.PageCount", ShowScreenTip = false, IsStatusBarItem = true };
			resources["Progress"] = new BarItemResource(null, null, null) { EditValueOneWay = true, EditValue="Model.ProgressValue", IsVisibleString = "Model.ProgressVisibility", ShowScreenTip = false, IsStatusBarItem = true };
			resources["MarqueeProgress"] = new BarItemResource(null, null, null) { IsVisibleString = "Model.ProgressMarqueeVisibility", ShowScreenTip = false, IsStatusBarItem = true };
			resources["StopPageBuilding"] = new BarItemResource("Model.StopCommand", PrintingStringId.StopPageBuilding.ToString(), "Images/BarItems/Stop_16x16.png") { IsStatusBarItem = true };
			resources["ZoomFactor"] = new BarItemResource(null, null, null) { IsStatusBarItem = true };
		}
	}
}
