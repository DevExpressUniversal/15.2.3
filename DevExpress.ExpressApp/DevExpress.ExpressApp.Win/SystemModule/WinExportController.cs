#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Windows.Forms;
using System.ComponentModel;
using System.IO;
using DevExpress.XtraPrinting;
using System.Drawing.Imaging;
using DevExpress.ExpressApp.SystemModule;
using System.Collections.Generic;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Win.SystemModule {
	public struct SaveFileDialogFilterItem {
		public SaveFileDialogFilterItem(string id, string caption, string filter) : this(id, caption, filter, null) {
		}
		public SaveFileDialogFilterItem(string id, string caption, string filter, ImageFormat imageFormat) {
			this.Id = id;
			this.Caption = caption;
			this.Filter = filter;
			this.ImageFormat = imageFormat;
		}
		public string Id;
		public string Caption;
		public string Filter;
		public ImageFormat ImageFormat;
	}
	public class WinExportController : ExportController {
		protected internal static SaveFileDialogFilterItem AllFilesFilterItem = new SaveFileDialogFilterItem("AllFiles", "All files", "*.*");
		protected internal static Dictionary<ExportTarget, SaveFileDialogFilterItem> ExportTargetFilterItems = new Dictionary<ExportTarget, SaveFileDialogFilterItem>() {
			{ExportTarget.Csv, new SaveFileDialogFilterItem("CsvFiles", "Csv files", "*.csv") },
			{ExportTarget.Html, new SaveFileDialogFilterItem("HtmlFiles", "Html files", "*.htm") },
			{ExportTarget.Mht, new SaveFileDialogFilterItem("MhtFiles", "Mht files", "*.mht") },
			{ExportTarget.Pdf, new SaveFileDialogFilterItem("PdfFiles", "Pdf files", "*.pdf") },
			{ExportTarget.Rtf, new SaveFileDialogFilterItem("RtfFiles", "Rtf files", "*.rtf") },
			{ExportTarget.Text, new SaveFileDialogFilterItem("TextFiles", "Text files", "*.txt") },
			{ExportTarget.Xls, new SaveFileDialogFilterItem("XlsFiles", "Xls files", "*.xls") },
			{ExportTarget.Xlsx, new SaveFileDialogFilterItem("XlsxFiles", "Xlsx files", "*.xlsx") }
		};
		protected internal static List<SaveFileDialogFilterItem> ImageFilterItems = new List<SaveFileDialogFilterItem>() { 
			new SaveFileDialogFilterItem("PngFiles", "Png files", "*.png", ImageFormat.Png),
			new SaveFileDialogFilterItem("GifFiles", "GIF files", "*.gif", ImageFormat.Gif),
			new SaveFileDialogFilterItem("JpegFiles", "Jpeg files", "*.jpg", ImageFormat.Jpeg),
			new SaveFileDialogFilterItem("BmpFiles", "Bmp files", "*.bmp", ImageFormat.Bmp),
			new SaveFileDialogFilterItem("TiffFiles", "Tiff files", "*.tiff", ImageFormat.Tiff),
			new SaveFileDialogFilterItem("WmfFiles", "Wmf files", "*.wmf", ImageFormat.Wmf),
			new SaveFileDialogFilterItem("EmfFiles", "Emf files", "*.emf", ImageFormat.Emf)
		};
		private string GetFilterString(ExportTarget exportTarget) {
			string result = "";
			List<SaveFileDialogFilterItem> items = new List<SaveFileDialogFilterItem>();
			if(exportTarget == ExportTarget.Image) {
				items.AddRange(ImageFilterItems);
			}
			else {
				items.Add(ExportTargetFilterItems[exportTarget]);
			}
			items.Add(AllFilesFilterItem);
			foreach(SaveFileDialogFilterItem item in items) {
				result += CaptionHelper.GetLocalizedText(FilterCaptionsLocalizationModelNodesGeneratorUpdater.FilterCaptionsGroup, item.Id, item.Caption) + "|" + item.Filter + "|";
			}
			result = result.TrimEnd('|');
			return result;
		}
		private ImageFormat GetImageFormatByExtension(string extension) {
			string filter = "*." + extension;
			foreach(SaveFileDialogFilterItem item in ImageFilterItems) {
				if(item.Filter == filter) {
					return item.ImageFormat;
				}
			}
			return ImageFormat.Png;
		}
		protected bool SelectTargetFile(ExportTarget exportTarget, out FileStream fileStream, out ImageFormat imageFormat) {
			fileStream = null;
			imageFormat = null;
			using(SaveFileDialog saveFileDialog = new SaveFileDialog()) {
				saveFileDialog.AddExtension = true;
				saveFileDialog.ValidateNames = true;
				saveFileDialog.RestoreDirectory = true;
				saveFileDialog.OverwritePrompt = true;
				saveFileDialog.CreatePrompt = false;
				saveFileDialog.Filter = GetFilterString(exportTarget);
				saveFileDialog.FileName = GetDefaultFileName();
				CustomShowSaveFileDialogEventArgs args = new CustomShowSaveFileDialogEventArgs(saveFileDialog);
				if(CustomShowSaveFileDialog != null)
					CustomShowSaveFileDialog(this, args);
				if(!args.Handled) {
					args.DialogResult = saveFileDialog.ShowDialog(Form.ActiveForm);
				}
				if(args.DialogResult == DialogResult.OK) {
					string ext = Path.GetExtension(saveFileDialog.FileName);
					if(String.IsNullOrEmpty(ext)) {
						ext = GetDefaultExtension(exportTarget);
						saveFileDialog.FileName += "." + ext;
					}
					else {
						ext = ext.Remove(0, 1);
					}
					if(exportTarget == ExportTarget.Image) {
						imageFormat = GetImageFormatByExtension(ext);
					}
					fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
					return true;
				}
			}
			return false;
		}
		protected override void Export(ExportTarget exportTarget) {
			ImageFormat imageFormat;
			FileStream stream;
			if(SelectTargetFile(exportTarget, out stream, out imageFormat)) {
				try {
					ExportCore(exportTarget, stream, imageFormat);
				}
				finally {
					stream.Dispose();
				}
			}
		}
		public event EventHandler<CustomShowSaveFileDialogEventArgs> CustomShowSaveFileDialog;
	}
	public class WinExportAnalysisController : ExportAnalysisController {
		string fileName;
		protected override void OnExporting(CustomExportAnalysisEventArgs args) {
			base.OnExporting(args);
			fileName = args.Stream is FileStream ? ((FileStream)args.Stream).Name : String.Empty;
			DevExpress.XtraGrid.GridControl gridControl =  exportableEditor.Printable as DevExpress.XtraGrid.GridControl;
			if(gridControl != null) {
				gridControl.MainView.ClearDocument();
			}
		}
		protected override void UpdateExportAction() {
			base.UpdateExportAction();
			FileStreamProvider fileStreamProvider = StreamProvider as FileStreamProvider;
			if(fileStreamProvider != null) {
				fileStreamProvider.DefaultFileName = DefaultFileName;
			}
		}
		protected override ImageFormat GetImageFormat() {
			ImageFormat result = base.GetImageFormat();
			if(!String.IsNullOrEmpty(fileName)) {
				string extension = Path.GetExtension(fileName).Remove(0, 1);
				switch(extension) {
					case "png": result = ImageFormat.Png; break;
					case "gif": result = ImageFormat.Gif; break;
					case "jpg": result = ImageFormat.Jpeg; break;
					case "bmp": result = ImageFormat.Bmp; break;
					case "tiff": result = ImageFormat.Tiff; break;
					case "wmf": result = ImageFormat.Wmf; break;
					case "emf": result = ImageFormat.Emf; break;
				}
			}
			return result;
		}
		protected override IStreamProvider CreateStreamProvider() {
			return new FileStreamProvider();
		}
		public WinExportAnalysisController() : base() { }
	}
	public class FileStreamProvider : IStreamProvider {
		private string defaultFileName = String.Empty;
		private string GetFilterString(PrintingSystemCommand exportType) {
			string filter = "";
			switch(exportType) {
				case PrintingSystemCommand.ExportMht:
					filter = "Mht files|*.mht";
					break;
				case PrintingSystemCommand.ExportXls:
					filter = "Excel files|*.xls";
					break;
				case PrintingSystemCommand.ExportXlsx:
					filter = "Xlsx files|*.xlsx";
					break;
				case PrintingSystemCommand.ExportPdf:
					filter = "Pdf files|*.pdf";
					break;
				case PrintingSystemCommand.ExportRtf:
					filter = "Rtf files|*.rtf";
					break;
				case PrintingSystemCommand.ExportTxt:
					filter = "Text files|*.txt";
					break;
				case PrintingSystemCommand.ExportHtm:
					filter = "Html files|*.htm";
					break;
				case PrintingSystemCommand.ExportGraphic:
					filter = "Png files|*.png|GIF files|*.gif|Jpeg files|*.jpg|Bmp files|*.bmp|Tiff files|*.tiff|Wmf files|*.wmf|Emf Files|*.emf";
					break;
				default:
					Tracing.Tracer.LogValue("exportType", exportType);
					Guard.ArgumentNotNullOrEmpty("filter", filter);
					break;
			}
			return String.Format("{0}|All Files|*.*", filter);
		}
		public Stream GetExportStream(PrintingSystemCommand exportType) {
			using(SaveFileDialog saveFileDialog = new SaveFileDialog()) {
				saveFileDialog.AddExtension = true;
				saveFileDialog.ValidateNames = true;
				saveFileDialog.RestoreDirectory = true;
				saveFileDialog.OverwritePrompt = true;
				saveFileDialog.CreatePrompt = false;
				saveFileDialog.Filter = GetFilterString(exportType);
				saveFileDialog.FileName = defaultFileName;
				CustomShowSaveFileDialogEventArgs args = new CustomShowSaveFileDialogEventArgs(saveFileDialog);
				OnCustomShowSaveFileDialog(args);
				if(!args.Handled) {
					args.DialogResult = saveFileDialog.ShowDialog(Form.ActiveForm);
				}
				if(args.DialogResult == DialogResult.OK) {
					if(exportType == PrintingSystemCommand.ExportGraphic) {
						string extension = Path.GetExtension(saveFileDialog.FileName);
						if(String.IsNullOrEmpty(extension)) {
							saveFileDialog.FileName += ".Png";
						}
					}
					return new FileStream(saveFileDialog.FileName, FileMode.Create, FileAccess.Write);
				}
			}
			return null;
		}
		protected virtual void OnCustomShowSaveFileDialog(CustomShowSaveFileDialogEventArgs args) {
			if(CustomShowSaveFileDialog != null) {
				CustomShowSaveFileDialog(this, args);
			}
		}
		public string DefaultFileName {
			get { return defaultFileName; }
			set { defaultFileName = value; }
		}
		public event EventHandler<CustomShowSaveFileDialogEventArgs> CustomShowSaveFileDialog;
	}
	public class CustomShowSaveFileDialogEventArgs : HandledEventArgs {
		private SaveFileDialog dialog;
		private DialogResult dialogResult = DialogResult.None;
		public CustomShowSaveFileDialogEventArgs(SaveFileDialog dialog) {
			this.dialog = dialog;
		}
		public SaveFileDialog Dialog {
			get { return dialog; }
		}
		public DialogResult DialogResult {
			get { return dialogResult; }
			set { dialogResult = value; }
		}
	}
}
