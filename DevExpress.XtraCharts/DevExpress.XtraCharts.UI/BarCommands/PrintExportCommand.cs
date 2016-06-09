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

using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using DevExpress.Utils.Commands;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Printing;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraCharts.Commands {
	public class PrintPreviewCommand : ChartCommand {
		public override string ImageName { get { return "PrintPreview"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdPrintPreviewDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdPrintPreviewMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.PrintPreview; } }
		public PrintPreviewCommand(IChartContainer control) : base(control) {
		}
		protected override void ExecuteCore(ICommandUIState state) {
			((ChartControl)Control).ShowRibbonPrintPreview(PrintSizeMode.Zoom);
		}
	}
	public class PrintCommand : ChartCommand {
		public override string ImageName { get { return "Print"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdPrintDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdPrintMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.Print; } }
		public PrintCommand(IChartContainer control) : base(control) {
		}
		protected override void ExecuteCore(ICommandUIState state) {
			if (Chart.Printer != null)
				Chart.Printer.PerformPrintingAction(delegate { ((ComponentPrinterBase)Chart.Printer.ComponentPrinter).PrintDialog(); }, PrintSizeMode.Zoom);
		}
	}
	public class ExportPlaceHolderCommand : ChartCommand {
		public override string ImageName { get { return "Export"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdExportPlaceHolderDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdExportPlaceHolderMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.ExportPlaceHolder; } }
		public ExportPlaceHolderCommand(IChartContainer control)
			: base(control) {
		}
		protected override void ExecuteCore(ICommandUIState state) {
		}
	}
	public class ExportToPDFCommand : ChartCommand {
		public override string ImageName { get { return "ExportToPDF"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdExportToPDFDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdExportToPDFMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.ExportToPDF; } }
		public ExportToPDFCommand(IChartContainer control) : base(control) {
		}
		protected override void ExecuteCore(ICommandUIState state) {
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "PDF documents|*.pdf|All files|*.*";
			saveFileDialog.Title = "Export To PDF";
			saveFileDialog.ShowDialog();
			if (saveFileDialog.FileName != "") {
				FileStream file = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
				PrintSizeMode sizeMode = Chart.OptionsPrint.SizeMode;
				Chart.OptionsPrint.SizeMode = PrintSizeMode.Zoom;
				try {
					Chart.ExportToPdf(file);
				}
				finally {
					Chart.OptionsPrint.SizeMode = sizeMode;
				}
				file.Close();
			}
		}
	}
	public class ExportToHTMLCommand : ChartCommand {
		public override string ImageName { get { return "ExportToHTML"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdExportToHTMLDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdExportToHTMLMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.ExportToHTML; } }
		public ExportToHTMLCommand(IChartContainer control)
			: base(control) {
		}
		protected override void ExecuteCore(ICommandUIState state) {
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "HTML documents|*.html|All files|*.*";
			saveFileDialog.Title = "Export To HTML";
			saveFileDialog.ShowDialog();
			if (saveFileDialog.FileName != "") {
				FileStream file = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
				Chart.ExportToHtml(file);
				file.Close();
			}
		}
	}
	public class ExportToMHTCommand : ChartCommand {
		public override string ImageName { get { return "ExportToMHT"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdExportToMHTDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdExportToMHTMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.ExportToMHT; } }
		public ExportToMHTCommand(IChartContainer control)
			: base(control) {
		}
		protected override void ExecuteCore(ICommandUIState state) {
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "MHT documents|*.mht|All files|*.*";
			saveFileDialog.Title = "Export To MHT";
			saveFileDialog.ShowDialog();
			if (saveFileDialog.FileName != "")
				Chart.ExportToMht(saveFileDialog.FileName);
		}
	}
	public class ExportToXLSCommand : ChartCommand {
		public override string ImageName { get { return "ExportToXLS"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdExportToXLSDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdExportToXLSMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.ExportToXLS; } }
		public ExportToXLSCommand(IChartContainer control)
			: base(control) {
		}
		protected override void ExecuteCore(ICommandUIState state) {
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "XLS documents|*.xls|All files|*.*";
			saveFileDialog.Title = "Export To XLS";
			saveFileDialog.ShowDialog();
			if (saveFileDialog.FileName != "") {
				FileStream file = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
				Chart.ExportToXls(file);
				file.Close();
			}
		}
	}
	public class ExportToXLSXCommand : ChartCommand {
		public override string ImageName { get { return "ExportToXLSX"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdExportToXLSXDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdExportToXLSXMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.ExportToXLSX; } }
		public ExportToXLSXCommand(IChartContainer control)
			: base(control) {
		}
		protected override void ExecuteCore(ICommandUIState state) {
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "XLSX documents|*.xlsx|All files|*.*";
			saveFileDialog.Title = "Export To XLSX";
			saveFileDialog.ShowDialog();
			if (saveFileDialog.FileName != "") {
				FileStream file = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
				Chart.ExportToXlsx(file);
				file.Close();
			}
		}
	}
	public class ExportToRTFCommand : ChartCommand {
		public override string ImageName { get { return "ExportToRTF"; } }
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdExportToRTFDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdExportToRTFMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.ExportToRTF; } }
		public ExportToRTFCommand(IChartContainer control)
			: base(control) {
		}
		protected override void ExecuteCore(ICommandUIState state) {
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "RTF documents|*.rtf|All files|*.*";
			saveFileDialog.Title = "Export To RTF";
			saveFileDialog.ShowDialog();
			if (saveFileDialog.FileName != "") {
				FileStream file = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
				PrintSizeMode sizeMode = Chart.OptionsPrint.SizeMode;
				Chart.OptionsPrint.SizeMode = PrintSizeMode.Zoom;
				try {
					Chart.ExportToRtf(file);
				}
				finally {
					Chart.OptionsPrint.SizeMode = sizeMode;
				}
				file.Close();
			}
		}
	}
	public class ExportToImagePlaceHolderCommand : ChartCommand {
		public override string ImageName { get { return "ExportToImage"; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdExportToImagePlaceHolderMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.ExportToImagePlaceHolder; } }
		public ExportToImagePlaceHolderCommand(IChartContainer control)
			: base(control) {
		}
		protected override void ExecuteCore(ICommandUIState state) {
		}
	}
	public class ExportToBMPCommand : ChartCommand {
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdExportToBMPDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdExportToBMPMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.ExportToBMP; } }
		public ExportToBMPCommand(IChartContainer control)
			: base(control) {
		}
		protected override void ExecuteCore(ICommandUIState state) {
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "BMP image|*.bmp|All files|*.*";
			saveFileDialog.Title = "Export To BMP";
			saveFileDialog.ShowDialog();
			if (saveFileDialog.FileName != "") {
				FileStream file = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
				Chart.ExportToImage(file, ImageFormat.Bmp);
				file.Close();
			}
		}
	}
	public class ExportToGIFCommand : ChartCommand {
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdExportToGIFDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdExportToGIFMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.ExportToGIF; } }
		public ExportToGIFCommand(IChartContainer control)
			: base(control) {
		}
		protected override void ExecuteCore(ICommandUIState state) {
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "GIF image|*.gif|All files|*.*";
			saveFileDialog.Title = "Export To GIF";
			saveFileDialog.ShowDialog();
			if (saveFileDialog.FileName != "") {
				FileStream file = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
				Chart.ExportToImage(file, ImageFormat.Gif);
				file.Close();
			}
		}
	}
	public class ExportToJPEGCommand : ChartCommand {
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdExportToJPEGDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdExportToJPEGMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.ExportToJPEG; } }
		public ExportToJPEGCommand(IChartContainer control)
			: base(control) {
		}
		protected override void ExecuteCore(ICommandUIState state) {
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "JPEG image|*.jpg;*.jpeg|All files|*.*";
			saveFileDialog.Title = "Export To JPEG";
			saveFileDialog.ShowDialog();
			if (saveFileDialog.FileName != "") {
				FileStream file = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
				Chart.ExportToImage(file, ImageFormat.Jpeg);
				file.Close();
			}
		}
	}
	public class ExportToPNGCommand : ChartCommand {
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdExportToPNGDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdExportToPNGMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.ExportToPNG; } }
		public ExportToPNGCommand(IChartContainer control)
			: base(control) {
		}
		protected override void ExecuteCore(ICommandUIState state) {
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "PNG image|*.png|All files|*.*";
			saveFileDialog.Title = "Export To PNG";
			saveFileDialog.ShowDialog();
			if (saveFileDialog.FileName != "") {
				FileStream file = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
				Chart.ExportToImage(file, ImageFormat.Png);
				file.Close();
			}
		}
	}
	public class ExportToTIFFCommand : ChartCommand {
		public override ChartStringId DescriptionStringId { get { return ChartStringId.CmdExportToTIFFDescription; } }
		public override ChartStringId MenuCaptionStringId { get { return ChartStringId.CmdExportToTIFFMenuCaption; } }
		public override ChartCommandId Id { get { return ChartCommandId.ExportToTIFF; } }
		public ExportToTIFFCommand(IChartContainer control)
			: base(control) {
		}
		protected override void ExecuteCore(ICommandUIState state) {
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.Filter = "TIFF image|*.tiff, *.tif|All files|*.*";
			saveFileDialog.Title = "Export To TIFF";
			saveFileDialog.ShowDialog();
			if (saveFileDialog.FileName != "") {
				FileStream file = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
				Chart.ExportToImage(file, ImageFormat.Tiff);
				file.Close();
			}
		}
	}
}
