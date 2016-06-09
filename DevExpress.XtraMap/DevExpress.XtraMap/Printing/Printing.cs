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

using DevExpress.Utils;
using DevExpress.XtraMap.Drawing;
using DevExpress.XtraMap.Native;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using System.Diagnostics.CodeAnalysis;
namespace DevExpress.XtraMap {
	public enum MapPrintSizeMode {
		Normal = 0,
		Zoom = 1,
		Stretch = 2
	}
}
namespace DevExpress.XtraMap.Printing {
	public enum PrintImageFormat {
		Bitmap,
		Metafile
	}
	public class MapPrinter : IPrintable, IDisposable {
		readonly InnerMap map;
		ComponentPrinter componentPrinter;
		IPrintingSystem printingSystem;
		ILink link;
		BrickGraphics graphics;
		Rectangle printBounds;
		PrintingOptionsEditor printingOptionsEditor;
		PrintImageFormat imageFormat;
		protected InnerMap Map { get { return map; } }
		protected ComponentPrinter ComponentPrinter {
			get {
				if(componentPrinter == null)
					componentPrinter = CreateComponentPrinter();
				return componentPrinter;
			}
		}
		protected PrintingSystemBase PrintingSystemBase { get { return ComponentPrinter.PrintingSystemBase; } }
		public PrintImageFormat ImageFormat { get { return imageFormat; } set { imageFormat = value; } }
		public MapPrinter(InnerMap map) {
			Guard.ArgumentNotNull(map, "map");
			this.map = map;
		}
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			if(disposing) {
				if(componentPrinter != null) {
					componentPrinter.Dispose();
					componentPrinter = null;
				}
				if (printingOptionsEditor != null) {
					printingOptionsEditor.Dispose();
					componentPrinter = null;
				}
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~MapPrinter() {
			Dispose(false);
		}
		#endregion
		PrintingSystemCommand[] GetSupportedCommands() {
			return new PrintingSystemCommand[] { PrintingSystemCommand.ExportGraphic, 
				PrintingSystemCommand.ExportHtm, PrintingSystemCommand.ExportMht,
				PrintingSystemCommand.ExportPdf, PrintingSystemCommand.ExportRtf, 
				PrintingSystemCommand.ExportXls, PrintingSystemCommand.ExportXlsx, 
				PrintingSystemCommand.SendGraphic, PrintingSystemCommand.SendMht, 
				PrintingSystemCommand.SendPdf, PrintingSystemCommand.SendRtf, 
				PrintingSystemCommand.SendXls, PrintingSystemCommand.SendXlsx };
		}
		void SetPrintingSystem(IPrintingSystem ps) {
			if(printingSystem != null)
				printingSystem.AfterChange -= OnPrintingSystemAfterChange;
			this.printingSystem = ps;
			if(printingSystem != null) {
				printingSystem.AfterChange += OnPrintingSystemAfterChange;
			}
		}
		void SetCommandVisibility(PrintingSystemCommand[] commands) {
			foreach(PrintingSystemCommand item in commands)
				printingSystem.SetCommandVisibility(item, true);
		}
		void OnPrintingSystemAfterChange(object sender, ChangeEventArgs e) {
			if(printingSystem == null || link == null)
				return;
			switch(e.EventName) {
				case DevExpress.XtraPrinting.SR.PageSettingsChanged:
				case DevExpress.XtraPrinting.SR.AfterMarginsChange:
					((LinkBase)this.link).Margins = ((PrintingSystemBase)printingSystem).PageMargins;
					this.link.CreateDocument();
					break;
			}
		}
		internal Bitmap CreateDetailBitmap(Size size) {
			Bitmap image = CreateBitmapInstance(size);
			RenderToImageInternal(image, size);
			return image;
		}
		internal Metafile CreateDetailMetafile(Size size, MetafileFrameUnit units) {
			Rectangle metafileBounds = new Rectangle(Point.Empty, new Size(size.Width + 1, size.Height + 1));
			Rectangle drawingBounds = new Rectangle(Point.Empty, size);
			Metafile metafile = CreateMetafileInstance(null, metafileBounds, units, EmfType.EmfPlusOnly);
			try {
				DrawMetafile(metafile, drawingBounds);
			} catch {
				metafile.Dispose();
				throw;
			}
			return metafile;
		}
		Bitmap CreateBitmapInstance(Size size) {
			return new Bitmap(size.Width, size.Height);
		}
		[SuppressMessage("Microsoft.Security", "CA2141:Transparent methods must not satisfy LinkDemands")]
		Metafile CreateMetafileInstance(Stream stream, Rectangle bounds, MetafileFrameUnit units, EmfType emfType) {
			using(Graphics gr = Graphics.FromHwnd(IntPtr.Zero)) {
				IntPtr hdc = gr.GetHdc();
				try {
					return stream == null ? new Metafile(hdc, bounds, units, emfType) : new Metafile(stream, hdc, bounds, units, emfType);
				}
				finally {
					gr.ReleaseHdc(hdc);
				}
			}
		}
		void PrintDetailArea(IBrickGraphics graphics) {
			if(map == null)
				return;
			this.graphics = graphics as BrickGraphics;
			Size pageSize = Size.Truncate(this.graphics.ClientPageSize);
			if(pageSize.Width <= 0 || pageSize.Height <= 0)
				return;
			Rectangle clientRect = Map.ClientRectangle;
			this.printBounds = CalculatePrintBounds(clientRect, pageSize, Map.PrintOptions.SizeMode);
			if(printBounds.Width == 0 && printBounds.Height == 0)
				return;
			Image image = imageFormat == PrintImageFormat.Metafile ? (Image)CreateDetailMetafile(clientRect.Size, MetafileFrameUnit.Pixel) : 
				(Image)CreateDetailBitmap(clientRect.Size);
			DrawToBrickGraphics(this.graphics, printBounds, image);
		}
		void DrawToBrickGraphics(BrickGraphics brickGraph, Rectangle bounds, Image image) {
			if(image == null || brickGraph == null | bounds.Width == 0 && bounds.Height == 0)
				return;
			GraphicsUnit oldUnits = brickGraph.PageUnit;
			try {
				brickGraph.PageUnit = GraphicsUnit.Pixel;
				ImageBrick brick = printingSystem.CreateBrick("ImageBrick") as ImageBrick;
				if(brick != null) {
					brick.Image = image;
					brick.Sides = BorderSide.None;
					brick.SizeMode = ConvertPrintSizeMode(Map.PrintOptions.SizeMode);
					brick.DisposeImage = true;
					brickGraph.DrawBrick(brick, bounds);
				}
			}
			finally {
				brickGraph.PageUnit = oldUnits;
			}
		}
		public static ImageSizeMode ConvertPrintSizeMode(MapPrintSizeMode mode){
		   switch(mode){
			case MapPrintSizeMode.Normal:
					return ImageSizeMode.Normal;
				case MapPrintSizeMode.Stretch:
					return ImageSizeMode.StretchImage;
				case MapPrintSizeMode.Zoom:
					return ImageSizeMode.ZoomImage;
			   default:
				   return ImageSizeMode.Normal;
		   }
		}
		ImageCodecInfo FindImageEncoder(ImageFormat format) {
			foreach(ImageCodecInfo item in ImageCodecInfo.GetImageEncoders())
				if(item.FormatID.Equals(format.Guid))
					return item;
			return null;
		}
		protected virtual ComponentPrinter CreateComponentPrinter() {
			return new ComponentPrinter(this);
		}
		protected virtual void ExecuteAction(Action action) {
			ComponentPrinter.ClearDocument();
			action();
		}
		protected void Export(ExportTarget exportTarget, Stream stream) {
			ExecuteAction(() => { ComponentPrinter.Export(exportTarget, stream); });
		}
		protected virtual void Export(ExportTarget exportTarget, Stream stream, ExportOptionsBase options) {
			ExecuteAction(() => { ComponentPrinter.Export(exportTarget, stream, options); });
		}
		protected void Export(ExportTarget exportTarget, string fileName) {
			ExecuteAction(() => { ComponentPrinter.Export(exportTarget, fileName); });
		}
		protected void Export(ExportTarget exportTarget, string fileName, ExportOptionsBase options) {
			ExecuteAction(() => { ComponentPrinter.Export(exportTarget, fileName, options); });
		}
		protected virtual void ExportToMetafile(Stream stream, Rectangle clientRectangle) {
			using(Metafile metafile = CreateMetafileInstance(stream, clientRectangle, MetafileFrameUnit.Pixel, EmfType.EmfPlusDual)) {
				DrawMetafile(metafile, clientRectangle);
			}
		}
		void DrawMetafile(Metafile metafile, Rectangle clientRectangle) {
			using(Image img = CreateDetailBitmap(clientRectangle.Size)) {
				RenderToImageInternal(img, clientRectangle.Size);
				using(Graphics imageGraphics = Graphics.FromImage(metafile))
					imageGraphics.DrawImageUnscaled(img, clientRectangle);
			}
		}
		void RenderToImageInternal(Image image, Size imageSize) {
			Map.RenderController.RenderToImage(image, imageSize, Map.PrintOptions);
		}
		protected virtual void ExportToBitmap(Stream stream, ImageFormat format, Rectangle clientRectangle) {
			ImageCodecInfo encoder = FindImageEncoder(format);
			EncoderParameters encoderParameters = null;
			if(encoder == null)
				throw new ArgumentException("Incorrect Image Format");
			using(Image image = CreateBitmapInstance(clientRectangle.Size)) {
				if(format == System.Drawing.Imaging.ImageFormat.Jpeg) {
					encoderParameters = new EncoderParameters(1);
					encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
				}
				RenderToImageInternal(image, clientRectangle.Size);
				image.Save(stream, encoder, encoderParameters);
			}
		}
		protected internal Rectangle CalculatePrintBounds(Rectangle imageRect, Size pageSize, MapPrintSizeMode sizeMode) {
			RectangleF imageBounds = new RectangleF(PointF.Empty, imageRect.Size);
			RectangleF printBounds = imageBounds;
			RectangleF pageBounds = new RectangleF(PointF.Empty, pageSize);
			switch(sizeMode) {
				case MapPrintSizeMode.Normal:
					break;
				case MapPrintSizeMode.Zoom:
					printBounds.Size = MathMethods.ZoomInto(pageSize, imageBounds.Size);
					break;
				case MapPrintSizeMode.Stretch:
					printBounds = new Rectangle(Point.Empty, pageSize);
					break;
			}
			return Rectangle.Truncate(printBounds);
		}
		public void Initialize(IPrintingSystem ps, ILink link) {
			SetPrintingSystem(ps);
			this.link = link;
			if(printingSystem != null)
				SetCommandVisibility(GetSupportedCommands());
		}
		public void Finalize(IPrintingSystem ps, ILink link) {
			SetPrintingSystem(null);
			link = null;
		}
		public void CreateArea(string areaName, IBrickGraphics graphics) {
			switch(areaName) {
				case DevExpress.XtraPrinting.SR.Detail:
					PrintDetailArea(graphics);
					break;
			}
		}
		public void RejectChanges() {
		}
		public void ShowHelp() {
		}
		public bool CreatesIntersectedBricks {
			get { return true; }
		}
		public bool SupportsHelp() {
			return false;
		}
		public bool HasPropertyEditor() {
			return true;
		}
		public Form ShowPreview() {
			Form form = null;
			if(ComponentPrinterBase.IsPrintingAvailable(false))
				ExecuteAction(() => form = ComponentPrinter.ShowPreview(null));
			return form;
		}
		public Form ShowRibbonPreview() {
			Form form = null;
			if(ComponentPrinterBase.IsPrintingAvailable(false))
				ExecuteAction(() => form = ComponentPrinter.ShowRibbonPreview(null));
			return form;
		}
		public System.Windows.Forms.UserControl PropertyEditorControl {
			get {
				UserControl ctrl = new UserControl();
				if(printingOptionsEditor == null)
					printingOptionsEditor = new PrintingOptionsEditor();
				printingOptionsEditor.Initialize(map.PrintOptions);
				printingOptionsEditor.Dock = DockStyle.Fill;
				ctrl.Size = printingOptionsEditor.Size;
				ctrl.Controls.Add(printingOptionsEditor);
				ctrl.HandleDestroyed += delegate(object sender, EventArgs e) { ctrl.Controls.Remove(printingOptionsEditor); };
				return ctrl;
			}
		}
		public void AcceptChanges() {
			if(printingOptionsEditor != null)
				printingOptionsEditor.ApplyOptions();
		}
		public void Print() {
			if(ComponentPrinterBase.IsPrintingAvailable(false))
				ComponentPrinter.Print();
		}
		public void PrintDialog() {
			if(ComponentPrinterBase.IsPrintingAvailable(false))
				ComponentPrinter.PrintDialog();
		}
		public void ExportToHtml(string filePath) {
			Export(ExportTarget.Html, filePath);
		}
		public void ExportToHtml(string filePath, HtmlExportOptions options) {
			Export(ExportTarget.Html, filePath, options);
		}
		public void ExportToHtml(Stream stream) {
			Export(ExportTarget.Html, stream);
		}
		public void ExportToHtml(Stream stream, HtmlExportOptions options) {
			Export(ExportTarget.Html, stream, options);
		}
		public void ExportToMht(string filePath) {
			Export(ExportTarget.Mht, filePath);
		}
		public void ExportToMht(string filePath, MhtExportOptions options) {
			Export(ExportTarget.Mht, filePath, options);
		}
		public void ExportToMht(Stream stream, MhtExportOptions options) {
			Export(ExportTarget.Mht, stream, options);
		}
		public void ExportToPdf(string filePath) {
			Export(ExportTarget.Pdf, filePath);
		}
		public void ExportToPdf(string filePath, PdfExportOptions options) {
			Export(ExportTarget.Pdf, filePath, options);
		}
		public void ExportToPdf(Stream stream) {
			Export(ExportTarget.Pdf, stream);
		}
		public void ExportToPdf(Stream stream, PdfExportOptions options) {
			Export(ExportTarget.Pdf, stream, options);
		}
		public void ExportToRtf(string filePath) {
			Export(ExportTarget.Rtf, filePath);
		}
		public void ExportToRtf(Stream stream) {
			Export(ExportTarget.Rtf, stream);
		}
		public void ExportToXls(string filePath) {
			Export(ExportTarget.Xls, filePath);
		}
		public void ExportToXls(string filePath, XlsExportOptions options) {
			Export(ExportTarget.Xls, filePath, options);
		}
		public void ExportToXls(Stream stream) {
			Export(ExportTarget.Xls, stream);
		}
		public void ExportToXls(Stream stream, XlsExportOptions options) {
			Export(ExportTarget.Xls, stream, options);
		}
		public void ExportToXlsx(string filePath) {
			Export(ExportTarget.Xlsx, filePath);
		}
		public void ExportToXlsx(string filePath, XlsxExportOptions options) {
			Export(ExportTarget.Xlsx, filePath, options);
		}
		public void ExportToXlsx(Stream stream) {
			Export(ExportTarget.Xlsx, stream);
		}
		public void ExportToImage(Stream stream, ImageFormat format) {
			if(format == System.Drawing.Imaging.ImageFormat.Emf || 
				format == System.Drawing.Imaging.ImageFormat.Wmf)
				ExportToMetafile(stream, Map.ClientRectangle);
			else
				ExportToBitmap(stream, format, Map.ClientRectangle);
		}
		public void ExportToImage(string filePath, ImageFormat format) {
			using(FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite)) {
				ExportToImage(fs, format);
				fs.Close();
			}
		}
	}
}
