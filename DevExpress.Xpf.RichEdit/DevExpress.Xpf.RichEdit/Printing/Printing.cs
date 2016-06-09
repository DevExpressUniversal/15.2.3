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
using System.Drawing;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Office.Drawing;
using DevExpress.Office.Layout;
using DevExpress.Office.Model;
using DevExpress.Office.Printing;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraPrinting;
using DevExpress.Xpf.RichEdit.Controls.Internal;
using DevExpress.XtraPrinting.XamlExport;
using RichEditLayoutPage = DevExpress.XtraRichEdit.Layout.Page;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office.Internal;
#if SL
using System.Windows.Printing;
using DevExpress.Xpf.Core.Native;
using PlatformIndependentBeginPrintEventArgs = System.Windows.Printing.BeginPrintEventArgs;
using PlatformIndependentEndPrintEventArgs = System.Windows.Printing.EndPrintEventArgs;
using PlatformIndependentImage = System.Windows.Controls.Image;
using PlatformImage = System.Windows.Controls.Image;
using PlatformIndependentPen = DevExpress.Xpf.Drawing.Pen;
using PlatformIndependentBrush = System.Windows.Media.Brush;
using PlatformIndependentColor = System.Windows.Media.Color;
using PlatformBrush = System.Windows.Media.Brush;
#else
using System.Drawing.Printing;
using System.Linq;
using System.Printing;
using System.Windows.Documents;
using DevExpress.Xpf.Printing;
using PlatformIndependentBeginPrintEventArgs = System.Drawing.Printing.PrintEventArgs;
using PlatformIndependentEndPrintEventArgs = System.Drawing.Printing.PrintEventArgs;
using PlatformImage = System.Windows.Controls.Image;
using PlatformIndependentPen = System.Drawing.Pen;
using PlatformIndependentBrush = System.Drawing.Brush;
using PlatformIndependentColor = System.Drawing.Color;
using PlatformBrush = System.Windows.Media.Brush;
using System.Collections.Generic;
#endif
namespace DevExpress.XtraRichEdit.Printing {
#if SL
	public class DocumentPage {
		System.Windows.Size pageSize;
		Rect bleedBox;
		Rect contentBox;
		SimpleViewPage visual;
		public DocumentPage(SimpleViewPage visual, System.Windows.Size pageSize, Rect bleedBox, Rect contentBox) {
			this.visual = visual;
			this.pageSize = pageSize;
			this.bleedBox = bleedBox;
			this.contentBox = contentBox;
		}
		public virtual SimpleViewPage Visual { get { return this.visual; } }
		public System.Windows.Size PageSize { get { return pageSize; } }
		protected void SetVisual(SimpleViewPage visual) {
			this.visual = visual;
		}
	}
	public interface IDocumentPaginatorSource {
	}
	public abstract class DocumentPaginator {
		public abstract DocumentPage GetPage(int pageNumber);
		public abstract bool IsPageCountValid { get; }
		public abstract int PageCount { get; }
		public abstract System.Windows.Size PageSize { get; set; }
		public abstract IDocumentPaginatorSource Source { get; }
	}
#endif
	public class RichEditViewPageCreatedEventArgs : EventArgs {
		readonly RichEditViewPage page;
		public RichEditViewPageCreatedEventArgs(RichEditViewPage page) {
			Guard.ArgumentNotNull(page, "page");
			this.page = page;
		}
		public RichEditViewPage Page { get { return page; } }
	}
	public delegate void RichEditViewPageCreatedEventHandler(object sender, RichEditViewPageCreatedEventArgs args);
	public class RichEditDocumentLayoutPaginator : DocumentPaginator {
		#region Fields
		readonly RichEditPrinterBase printer;
		int currentPageIndex;
		DocumentLayout documentLayout;
		#endregion
		public RichEditDocumentLayoutPaginator(RichEditPrinterBase printer) {
			Guard.ArgumentNotNull(printer, "printer");
			this.printer = printer;
		}
		#region Properties
		public override bool IsPageCountValid { get { return documentLayout != null; } }
		public override int PageCount { get { return documentLayout != null ? documentLayout.Pages.Count : 0; } }
		public override System.Windows.Size PageSize {
			get {
				if (documentLayout == null)
					return new System.Windows.Size();
				DocumentLayoutUnitConverter unitConverter = documentLayout.UnitConverter;
				RichEditLayoutPage layoutPage = documentLayout.Pages[currentPageIndex];
				float width = unitConverter.LayoutUnitsToPixelsF(layoutPage.Bounds.Width, DpiX);
				float height = unitConverter.LayoutUnitsToPixelsF(layoutPage.Bounds.Height, DpiY);
				return new System.Windows.Size(width, height);
			}
			set {
			}
		}
		public override IDocumentPaginatorSource Source { get { return null; } }
		public int CurrentPageIndex { get { return currentPageIndex; } set { currentPageIndex = value; } }
		protected internal virtual float DpiX { get { return 96; } }
		protected internal virtual float DpiY { get { return 96; } }
		#endregion
		#region Events
		RichEditViewPageCreatedEventHandler onPageCreated;
		public event RichEditViewPageCreatedEventHandler PageCreated { add { onPageCreated += value; } remove { onPageCreated -= value; } }
		protected internal virtual void RaisePageCreated(RichEditViewPage page) {
			if (onPageCreated != null)
				onPageCreated(this, new RichEditViewPageCreatedEventArgs(page));
		}
		#endregion
		public void BeginPagination() {
			this.currentPageIndex = 0;
			this.documentLayout = printer.CalculatePrintDocumentLayout();
		}
		public void EndPagination() {
			this.documentLayout = null;
			this.currentPageIndex = 0;
		}
		public override DocumentPage GetPage(int pageNumber) {
			this.currentPageIndex = pageNumber;
			RichEditLayoutPage layoutPage = documentLayout.Pages[currentPageIndex];
			DocumentLayoutUnitConverter unitConverter = documentLayout.UnitConverter;
			SimpleViewPage page = new SimpleViewPage();
			page.Background = GetPageBackColor(page.Background);
			page.Width = unitConverter.LayoutUnitsToPixelsF(layoutPage.Bounds.Width, DpiX);
			page.Height = unitConverter.LayoutUnitsToPixelsF(layoutPage.Bounds.Height, DpiY);
			RaisePageCreated(page);
			if (page.SuperRoot == null)
				page.ApplyTemplate();
			if (page.SuperRoot == null)
				return null;			
			DocumentLayoutExporter exporter = CreateExporter(page.Children);
			layoutPage.ExportTo(exporter);
			exporter.FinishExport();
			System.Windows.Size size = new System.Windows.Size(page.Width, page.Height);
			Rect rect = new Rect(new System.Windows.Point(), size);
			return new DocumentPage(page, size, rect, rect);
		}
		protected internal virtual PlatformBrush GetPageBackColor(PlatformBrush sourceBrush) {
			DocumentModel documentModel = documentLayout.DocumentModel;
			PlatformIndependentColor pageBackColor = documentModel.DocumentProperties.PageBackColor;
			if (documentModel.PrintingOptions.EnablePageBackgroundOnPrint && !DXColor.IsTransparentOrEmpty(pageBackColor))
				return new SolidColorBrush(XpfTypeConverter.ToPlatformColor(pageBackColor));
			return sourceBrush;
		}
#if !SL
		public FixedPage GetFixedPage(int pageNumber) {
			this.currentPageIndex = pageNumber;
			RichEditLayoutPage layoutPage = documentLayout.Pages[currentPageIndex];
			FixedPage page = new FixedPage();
			page.Background = GetPageBackColor(page.Background);
			DocumentLayoutUnitConverter unitConverter = documentLayout.UnitConverter;
			page.Width = unitConverter.LayoutUnitsToPixelsF(layoutPage.Bounds.Width, DpiX);
			page.Height = unitConverter.LayoutUnitsToPixelsF(layoutPage.Bounds.Height, DpiY);
			DocumentLayoutExporter exporter = CreateExporter(page.Children);
			layoutPage.ExportTo(exporter);
			exporter.FinishExport();
			return page;
		}
#endif
		protected virtual DocumentLayoutExporter CreateExporter(UIElementCollection panelChildren) {
			XpfDrawingSurface surface = new XpfDrawingSurface(panelChildren);
			Painter painter = CreatePainter(documentLayout.UnitConverter, surface);
			GraphicsDocumentLayoutExporterAdapter adapter = new XpfGraphicsDocumentLayoutExporterAdapter();
			GraphicsDocumentLayoutExporter exporter = new PrintingGraphicsDocumentLayoutExporter(documentLayout.DocumentModel, painter, adapter, new Rectangle(0, 0, int.MaxValue, int.MaxValue));
			exporter.MinReadableTextHeight = int.MaxValue;
			exporter.ShowWhitespace = false;
			exporter.DrawInactivePieceTableWithDifferentColor = false;
			return exporter;
		}
		protected virtual Painter CreatePainter(DocumentLayoutUnitConverter unitConverter, XpfDrawingSurface surface) {
			return new XpfPrintingPainter(unitConverter, surface);
		}
	}
#if !SL
	public class RichEditDocumentLayoutServerPaginator : RichEditDocumentLayoutPaginator {
		Dictionary<FontInfo, WpfFontInfo> mapFontInfoToWpfFontInfo;
		public RichEditDocumentLayoutServerPaginator(RichEditPrinterBase printer, Dictionary<FontInfo, WpfFontInfo> mapFontInfoToWpfFontInfo)
			: base(printer) {
				this.mapFontInfoToWpfFontInfo = mapFontInfoToWpfFontInfo;
		}
		protected override Painter CreatePainter(DocumentLayoutUnitConverter unitConverter, XpfDrawingSurface surface) {
			return new XpfServerFixedDocumentPrintingPainter(unitConverter, surface, mapFontInfoToWpfFontInfo);
		}
	}
#endif
	public class PrintingGraphicsDocumentLayoutExporter : GraphicsDocumentLayoutExporter {
		public PrintingGraphicsDocumentLayoutExporter (DocumentModel documentModel, Painter painter, GraphicsDocumentLayoutExporterAdapter adapter, Rectangle bounds)
			: base(documentModel, painter, adapter, bounds, TextColors.Defaults) {
		}
		public PrintingGraphicsDocumentLayoutExporter(DocumentModel documentModel, Painter painter, GraphicsDocumentLayoutExporterAdapter adapter, Rectangle bounds, bool hidePartiallyVisibleRow)
			: base(documentModel, painter, adapter, bounds, hidePartiallyVisibleRow, TextColors.Defaults) {
		}
		protected override void ExportFieldsHighlighting() {
		}
	}
	public class XpfRichEditPrinter : RichEditPrinterBase {
#if !SL
		FontCacheManager fontCacheManager;
#endif
		RichEditDocumentLayoutPaginator paginator;
		static DocumentModel GetDocumentModel(IRichEditControl control) {
			Guard.ArgumentNotNull(control, "control");
			return control.InnerControl.DocumentModel;
		}
		static DocumentModel GetDocumentModel(IRichEditDocumentServer documentServer) {
			Guard.ArgumentNotNull(documentServer, "documentServer");
			return documentServer.Model;
		}		
		public XpfRichEditPrinter(IRichEditControl control)
			: base(GetDocumentModel(control)) {
		}
		public XpfRichEditPrinter(IRichEditDocumentServer documentServer)
			: base(GetDocumentModel(documentServer)) {
		}
		public virtual void Print() {
#if SL
			PrintDialog();
#else
			PrintDialog printDialog = new PrintDialog();
			QuickPrint(printDialog);
#endif
		}
		public virtual void PrintDialog() {
#if SL
			PrintDocument printDocument = new PrintDocument();
			printDocument.BeginPrint += OnBeginPrint;
			printDocument.EndPrint += OnEndPrint;
			printDocument.PrintPage += OnPrintPage;
			printDocument.Print(String.Empty);
#else
			PrintDialog printDialog = new PrintDialog();
			if (printDialog.ShowDialog() == true)
				QuickPrint(printDialog);
#endif
		}
#if !SL
		static bool IsPrinterInstalled() {
			try {
				using (LocalPrintServer printServer = new LocalPrintServer()) {
					using (PrintQueueCollection printQueues = printServer.GetPrintQueues()) {
						return printQueues.Count() > 0;
					}
				}
			}
			catch {
				return false;
			}
		}
		protected internal virtual void QuickPrint(PrintDialog printDialog) {
			if (!IsPrinterInstalled())
				throw new NotSupportedException(PrintingLocalizer.GetString(PrintingStringId.Exception_NoPrinterFound));
			FixedDocument printDocument = CreateFixedDocument();
			printDialog.PrintDocument(((IDocumentPaginatorSource)printDocument).DocumentPaginator, String.Empty);
		}
		protected internal virtual FixedDocument CreateFixedDocument() {
			FixedDocument printDocument = new FixedDocument();
			OnBeginPrint(this, null);
			int count = paginator.PageCount;
			for (int i = 0; i < count; i++) {
				PageContent pageContent = new PageContent();
				pageContent.Child = paginator.GetFixedPage(i);
				printDocument.Pages.Add(pageContent);
			}
			OnEndPrint(this, null);
			return printDocument;
		}
#endif
		void OnBeginPrint(object sender, PlatformIndependentBeginPrintEventArgs e) {
#if !SL
			this.fontCacheManager =  new WpfFontCacheManager(DocumentModel.LayoutUnitConverter);
#endif
			this.paginator = CreatePaginator();
			this.paginator.BeginPagination();
			this.paginator.PageCreated += new RichEditViewPageCreatedEventHandler(OnPageCreated);
		}
		protected virtual RichEditDocumentLayoutPaginator CreatePaginator() {
			return new RichEditDocumentLayoutPaginator(this);
		}
#if !SL
		protected internal override void InitializeEmptyDocumentModel(DocumentModel documentModel) {
			base.InitializeEmptyDocumentModel(documentModel);
			documentModel.SetFontCacheManager(this.fontCacheManager);
		}
#endif
		void OnEndPrint(object sender, PlatformIndependentEndPrintEventArgs e) {
			this.paginator.EndPagination();
			this.paginator = null;
#if !SL
			this.fontCacheManager = null;
#endif
		}
		PrintPageEventArgs printPageEventArgs;
		void OnPageCreated(object sender, RichEditViewPageCreatedEventArgs args) {
#if SL
			if (printPageEventArgs != null)
				printPageEventArgs.PageVisual = args.Page;
#endif
		}
		void OnPrintPage(object sender, PrintPageEventArgs e) {
			this.printPageEventArgs = e;
			DocumentPage page = paginator.GetPage(paginator.CurrentPageIndex);
			paginator.CurrentPageIndex++;
			if (page == null)
				return;
			e.HasMorePages = paginator.CurrentPageIndex < paginator.PageCount;
#if SL
			UIElement pageVisual = e.PageVisual;			
			e.PageVisual = null;
			if (page.Visual != null && page.Visual.SuperRoot != null && (page.Visual.SuperRoot.Parent as UIElement) != null) {
				Transform transform = TransformPrintingPage(page.PageSize, e.PrintableArea, e.PageMargins);;
				if (transform != null)
					((UIElement)page.Visual.SuperRoot.Parent).RenderTransform = transform;
			}
			e.PageVisual = pageVisual;
#endif
		}
#if SL
		Transform TransformPrintingPage(System.Windows.Size pageContentSize, System.Windows.Size printableArea, Thickness pageMargins) {
			if (IsLandscape(pageContentSize) != IsLandscape(printableArea)) {
				var result = new TransformGroup();
				result.Children.Add(new RotateTransform { Angle = -90 });
				result.Children.Add(new TranslateTransform() { X = -pageMargins.Top, Y = pageContentSize.Width - pageMargins.Left });
				return result;
			}
			else {
				if (pageMargins.Left != 0 || pageMargins.Top != 0) {
					var result = new TransformGroup();
					result.Children.Add(new TranslateTransform() { X = -pageMargins.Left, Y = -pageMargins.Top });
					return result;
				}
			}
			return new MatrixTransform();
		}
		bool IsLandscape(System.Windows.Size size) {
			return size.Width > size.Height;
		}		
#endif
		protected internal override DocumentPrinter CreateDocumentPrinter(DocumentModel documentModel) {
			return new XpfDocumentPrinter(documentModel);
		}
	}
#if !SL
	public class XpfServerFixedDocumentPrinter : XpfRichEditPrinter {
		Dictionary<FontInfo, WpfFontInfo> mapFontInfoToWpfFontInfo;
		public XpfServerFixedDocumentPrinter(IRichEditDocumentServer documentServer, Dictionary<FontInfo, WpfFontInfo> mapFontInfoToWpfFontInfo)
			: base(documentServer) {
				this.mapFontInfoToWpfFontInfo = mapFontInfoToWpfFontInfo;
		}
		protected override RichEditDocumentLayoutPaginator CreatePaginator() {
			return new RichEditDocumentLayoutServerPaginator(this, mapFontInfoToWpfFontInfo);
		}
	}
#endif
	#region XpfDocumentPrinter
	public class XpfDocumentPrinter : DocumentPrinter {
		public XpfDocumentPrinter(DocumentModel documentModel)
			: base(documentModel) {
		}
		protected internal override DocumentFormattingController CreateDocumentFormattingController(DocumentLayout documentLayout) {
			return new PrintLayoutViewDocumentFormattingController(documentLayout, PieceTable);
		}
		protected internal override DocumentPrinterController CreateDocumentPrinterController() {
			return new PlatformDocumentPrinterController();
		}
		protected internal override BoxMeasurer CreateMeasurer(Graphics gr) {
#if SL
			return new PrecalculatedMetricsBoxMeasurer(DocumentModel);
#else
			return new WpfBoxMeasurer(DocumentModel, gr);
#endif
		}
	}
	#endregion
	#region XpfPrintingPainter
	public class XpfPrintingPainter : XpfPainterOverwrite {
		public XpfPrintingPainter(DocumentLayoutUnitConverter unitConverter, XpfDrawingSurface surface)
			: base(unitConverter, surface) {
				ZoomFactor = 1.0F;
		}
		public override int DpiX { get { return 96; } }
		public override int DpiY { get { return 96; } }
		public override void DrawImage(OfficeImage img, System.Drawing.Rectangle bounds) {
			PlatformImage image = CreateImage(img);
			if (image == null)
				return;
			image.Stretch = Stretch.Fill;
			SetPosition(image, bounds);
			SetWidthHeight(image, bounds.Width, bounds.Height);
		}
		protected virtual PlatformImage CreateImage(OfficeImage img) {
#if SL
			PlatformImage image = new PlatformImage();
			image.Source = XpfPainter.CreatePlatformImage(img).Source;
#else
			PlatformImage image = XpfPainter.CreatePlatformImage(img);
#endif
			if (image == null)
				return null;
			return Surface.AppendExistingImage(image);
		}
		protected internal override void FillRectangleCore(PlatformBrush brush, Rectangle bounds) {
			PointF[] pointF = new PointF[] {
				new PointF(bounds.X, bounds.Y),
				new PointF(bounds.Right, bounds.Y),
				new PointF(bounds.Right, bounds.Bottom),
				new PointF(bounds.X, bounds.Bottom)
			};
			System.Windows.Shapes.Path line = CreatePolyline(pointF, true, true);
			line.Fill = brush;
		}
		public override void DrawLine(PlatformIndependentPen pen, float x1, float y1, float x2, float y2) {
			PointF[] pointF = new PointF[] {
				new PointF(x1, y1),
				new PointF(x2, y2)
			};
			DrawLines(pen, pointF);
		}
	}
	#endregion
#if !SL
	#region XpfFixedDocumentPrintingPainter
	public class XpfServerFixedDocumentPrintingPainter : XpfPrintingPainter {
		Dictionary<FontInfo, WpfFontInfo> mapFontInfoToWpfFontInfo;
		WpfFontCache fontCache;
		public XpfServerFixedDocumentPrintingPainter(DocumentLayoutUnitConverter unitConverter, XpfDrawingSurface surface, Dictionary<FontInfo, WpfFontInfo> mapFontInfoToWpfFontInfo)
			: base(unitConverter, surface) {
			this.mapFontInfoToWpfFontInfo = mapFontInfoToWpfFontInfo;
			this.fontCache = new WpfFontCache(unitConverter);
		}		
		protected override void DrawStringCore(TextBlock textBlock, string text, FontInfo fontInfo, PlatformIndependentColor foreColor) {
			WpfFontInfo wpfFontInfo = GetWpfFontInfo(fontInfo);
			base.DrawStringCore(textBlock, text, wpfFontInfo, foreColor);
		}
		public WpfFontInfo GetWpfFontInfo(FontInfo fontInfo) {
			WpfFontInfo result;
			if (mapFontInfoToWpfFontInfo.TryGetValue(fontInfo, out result))
				return result;
			GdiFontInfo gdiFontInfo = (GdiFontInfo)fontInfo;
			result = (WpfFontInfo)fontCache.CreateFontInfoCore(fontInfo.Name, (int)Math.Round(gdiFontInfo.Size * 2), fontInfo.Bold, fontInfo.Italic, fontInfo.Font.Strikeout, fontInfo.Underline);
			mapFontInfoToWpfFontInfo.Add(fontInfo, result);
			return result;
		}		
	}
	#endregion
#endif
	#region PatternLineBrickXamlExporter<T> (abstract class)
	public abstract class PatternLineBrickXamlExporter<T> : VisualBrickXamlExporter, IPatternLinePaintingSupport where T : struct {
		static readonly DocumentLayoutUnitConverter pixelLayoutUnitConverter = new DocumentLayoutUnitPixelsConverter(GraphicsDpi.DeviceIndependentPixel);
		static readonly DocumentLayoutUnitConverter documentLayoutUnitConverter = new DocumentLayoutUnitDocumentConverter();
		XamlWriter writer;
		protected override XamlBrickExportMode GetBrickExportMode() {
			return XamlBrickExportMode.Content;
		}
		public override void WriteBrickToXaml(XamlWriter writer, BrickBase brick, XamlExportContext exportContext, RectangleF clipRect, Action<XamlWriter> declareNamespaces, Action<XamlWriter, object> writeCustomProperties) {
			PatternLineBrick<T> lineBrick = brick as PatternLineBrick<T>;
			if (lineBrick == null)
				throw new ArgumentException("brick");
			this.writer = writer;
			try {
				IPatternLinePainter<T> linePainter = CreateLinePainter(this, pixelLayoutUnitConverter);
				PatternLine<T> line = lineBrick.GetPatternLine();
				RectangleF layoutBounds = lineBrick.CalculateLineBounds(null, lineBrick.Rect);
				RectangleF pixelBounds = new RectangleF(
					documentLayoutUnitConverter.LayoutUnitsToPixelsF(layoutBounds.X, GraphicsDpi.DeviceIndependentPixel),
					documentLayoutUnitConverter.LayoutUnitsToPixelsF(layoutBounds.Y, GraphicsDpi.DeviceIndependentPixel),
					documentLayoutUnitConverter.LayoutUnitsToPixelsF(layoutBounds.Width, GraphicsDpi.DeviceIndependentPixel),
					documentLayoutUnitConverter.LayoutUnitsToPixelsF(layoutBounds.Height, GraphicsDpi.DeviceIndependentPixel));
				pixelBounds.X = (int)Math.Round(pixelBounds.X);
				pixelBounds.Y = (int)Math.Round(pixelBounds.Y);
				pixelBounds.Width = Math.Max(1, (int)Math.Ceiling(pixelBounds.Width));
				pixelBounds.Height = Math.Max(1, (int)Math.Ceiling(pixelBounds.Height));
				line.Draw(linePainter, pixelBounds, lineBrick.BorderColor);
			}
			finally {
				this.writer = null;
			}
		}
		protected abstract IPatternLinePainter<T> CreateLinePainter(IPatternLinePaintingSupport painter, DocumentLayoutUnitConverter unitConverter);
		void ApplyPen(PlatformIndependentPen pen) {
			writer.WriteAttribute(XamlAttribute.Stroke, pen.Color);
			float thickness = pen.Width;
			if (thickness == 0)
				thickness = 1;
			writer.WriteAttribute(XamlAttribute.StrokeThickness, thickness);
#if !SL
			if (pen.DashStyle == System.Drawing.Drawing2D.DashStyle.Custom)
#endif
				if (pen.DashPattern != null) {
					writer.WriteAttribute(XamlAttribute.StrokeDashArray, pen.DashPattern);
				}
		}
		#region IPatternLinePaintingSupport
		void IPatternLinePaintingSupport.DrawLine(PlatformIndependentPen pen, float x1, float y1, float x2, float y2) {
			writer.WriteStartElement(XamlTag.Line);
			writer.WriteAttribute(XamlAttribute.X1, x1);
			writer.WriteAttribute(XamlAttribute.Y1, y1);
			writer.WriteAttribute(XamlAttribute.X2, x2);
			writer.WriteAttribute(XamlAttribute.Y2, y2);
			ApplyPen(pen);
			writer.WriteEndElement();
		}
		void AppendPoint(ChunkedStringBuilder pathData, string prefix, PointF point) {
			pathData.Append(prefix);
			pathData.Append(point.X.ToString(CultureInfo.InvariantCulture));
			pathData.Append(',');
			pathData.Append(point.Y.ToString(CultureInfo.InvariantCulture));
		}
		void IPatternLinePaintingSupport.DrawLines(PlatformIndependentPen pen, PointF[] points) {
			int count = points.Length;
			if (count <= 1)
				return;
			writer.WriteStartElement("Path");
			ApplyPen(pen);
			ChunkedStringBuilder pathData = new ChunkedStringBuilder();
			AppendPoint(pathData, "M ", points[0]);
			for (int i = 1; i < count; i++)
				AppendPoint(pathData, " L ", points[i]);
			writer.WriteAttribute("Data", pathData.ToString());
			writer.WriteEndElement();
		}
		PlatformIndependentBrush IPatternLinePaintingSupport.GetBrush(PlatformIndependentColor color) {
#if SL
			return new SolidColorBrush(XpfTypeConverter.ToPlatformColor(color));
#else
			return new SolidBrush(color);
#endif
		}
		PlatformIndependentPen IPatternLinePaintingSupport.GetPen(PlatformIndependentColor color, float thickness) {
			return new PlatformIndependentPen(color, thickness);
		}
		PlatformIndependentPen IPatternLinePaintingSupport.GetPen(PlatformIndependentColor color) {
			return new PlatformIndependentPen(color);
		}
		void IPatternLinePaintingSupport.ReleaseBrush(PlatformIndependentBrush brush) {
		}
		void IPatternLinePaintingSupport.ReleasePen(PlatformIndependentPen pen) {
			pen.Dispose();
		}
		#endregion
	}
	#endregion
	#region UnderlineBrickXamlExporter
	public class UnderlineBrickXamlExporter : PatternLineBrickXamlExporter<UnderlineType> {
		protected override IPatternLinePainter<UnderlineType> CreateLinePainter(IPatternLinePaintingSupport painter, DocumentLayoutUnitConverter unitConverter) {
			return new RichEditHorizontalPatternLinePainter(painter, unitConverter);
		}
	}
	#endregion
	#region VerticalUnderlineBrickXamlExporter
	public class VerticalUnderlineBrickXamlExporter : PatternLineBrickXamlExporter<UnderlineType> {
		protected override IPatternLinePainter<UnderlineType> CreateLinePainter(IPatternLinePaintingSupport painter, DocumentLayoutUnitConverter unitConverter) {
			return new RichEditHorizontalPatternLinePainter(painter, unitConverter);
		}
	}
	#endregion
	#region StrikeoutBrickXamlExporter
	public class StrikeoutBrickXamlExporter : PatternLineBrickXamlExporter<StrikeoutType> {
		protected override IPatternLinePainter<StrikeoutType> CreateLinePainter(IPatternLinePaintingSupport painter, DocumentLayoutUnitConverter unitConverter) {
			return new RichEditHorizontalPatternLinePainter(painter, unitConverter);
		}
	}
	#endregion
}
