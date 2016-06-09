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
using System.ComponentModel.Design;
using System.Drawing;
using DevExpress.Office.Drawing;
using DevExpress.Office.Layout;
using DevExpress.Utils;
using DevExpress.XtraPrinting.BrickExporters;
using DevExpress.XtraPrinting.Export;
using DevExpress.XtraPrinting.Export.Rtf;
using DevExpress.XtraPrinting.Export.Web;
using DevExpress.XtraPrinting.Exports;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.RichText;
using DevExpress.XtraRichEdit.Export;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.XtraRichEdit.Model;
using System.ComponentModel;
namespace DevExpress.XtraPrinting {
	[BrickExporter(typeof(RichTextBrickBaseExporter))]
	public abstract class RichTextBrickBase : VisualBrick {
		GraphicsDocumentPrinter printer;
		bool isFormatted;
		bool isOwnDocumentModelAndPrinter;
		DocumentModel documentModel;
		readonly List<Brick> innerBricks = new List<Brick>();
		internal List<Brick> InnerBricks {
			get {
				if(innerBricks.Count == 0 && documentModel != null) {
					DocumentModelBrickCollector collector = new DocumentModelBrickCollector(documentModel, Printer);
					collector.Collect(PrintingSystem, innerBricks);
				}
				return innerBricks;
			}
		}
		protected virtual DocumentModel DocumentModel { get { return documentModel; } set { documentModel = value; } }
		protected bool IsFormatted { get { return isFormatted; } set { isFormatted = value; } }
		protected bool IsOwnDocumentModelAndPrinter { get { return isOwnDocumentModelAndPrinter; } set { isOwnDocumentModelAndPrinter = value; } }
		internal protected GraphicsDocumentPrinter Printer { 
			get { return printer; }			 
			set { 
				printer = value;
				IsFormatted = false;
			} 
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichTextBrickBaseText")]
#endif
		public override string Text { get { return XtraRichTextEditHelper.GetTextFromDocManager(documentModel); } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("RichTextBrickBaseEffectiveHeight")]
#endif
		public int EffectiveHeight { 
			get {
				PrepareDocumentPrinter(null);
				return Printer.GetEffectiveHeight(); 
			} 
		}
#if DEBUGTEST
		public string PlainText { get { return Text; } }
#endif
		protected RichTextBrickBase(IBrickOwner brickOwner)
			: base(brickOwner) {
			Style.BorderWidth = 0;
		}
		protected RichTextBrickBase(RichTextBrickBase brick)
			: base(brick) {
		}
		protected static string ValidateRtf(string rtf, DocumentModel model) {
			if(RtfTags.IsRtfContent(rtf))
				return rtf;
			XtraRichTextEditHelper.ImportPlainTextToDocManager(rtf, model);
			return XtraRichTextEditHelper.GetRtfFromDocManager(model);
		}
		protected void RecreateDocManagerAndPrinter() {
			DisposeDocManagerAndPrinter();
			isOwnDocumentModelAndPrinter = true;
			DocumentModel = XtraRichTextEditHelper.CreateDocumentModel();
			SetFontCacheManager();
			Printer = CreateDocumentPrinter();
		}
		protected override void OnSetPrintingSystem(bool cacheStyle) {
			base.OnSetPrintingSystem(cacheStyle);
			SetFontCacheManager();
		}
		void SetFontCacheManager() {
			if(this.PrintingSystem == null || documentModel == null)
				return;
			FontCacheManager manager = ((IServiceProvider)this.PrintingSystem).GetService(typeof(FontCacheManager)) as FontCacheManager;
			if(manager == null) {
				if (PrecalculatedMetricsPrintingFontCacheManager.ShouldUse())
					manager = new PrecalculatedMetricsPrintingFontCacheManager(new DocumentLayoutUnitDocumentConverter());
				else
					manager = new PrintingFontCacheManager(new DocumentLayoutUnitDocumentConverter());
				((IServiceContainer)this.PrintingSystem).AddService(typeof(FontCacheManager), manager);
			}
			documentModel.FontCacheManager = manager;
		}
		protected virtual GraphicsDocumentPrinter CreateDocumentPrinter() {
			return new BreaksFreeDocumentPrinter(documentModel);
		}
		protected void DisposeDocManagerAndPrinter() {
			if(!isOwnDocumentModelAndPrinter)
				return;
			if(Printer != null) {
				Printer.Dispose();
				Printer = null;
			}
			if(DocumentModel != null) {
				DocumentModel.Dispose();
				DocumentModel = null;
			}
			foreach(Brick brick in innerBricks)
				brick.Dispose();
			innerBricks.Clear();
		}
		public override void Dispose() {
			DisposeDocManagerAndPrinter();
			base.Dispose();
		}
		internal protected virtual void PrepareDocumentPrinter(IPrintingSystemContext context) { 
		}
		#region export
		protected override float ValidatePageBottomInternal(float pageBottom, RectangleF rect, IPrintingSystemContext context) {
			RectangleF clientRect = CalcClientRectUsingPaddingAndBorders(rect);
			float relativePageBottom = pageBottom - clientRect.Top;
			PrepareDocumentPrinter(context);
			relativePageBottom = ValidateRelativePageBottom(relativePageBottom, Printer.Columns, GetScaleFactor(context));
			if(relativePageBottom == 0)
				return rect.Top;
			return relativePageBottom + clientRect.Top;
		}
		float ValidateRelativePageBottom(float pageBottom, ColumnCollection columns, float scaleFactor) {
			RectangleF rowContentBounds = RectangleF.Empty;
			int columnCount = columns.Count;
			for(int columnIndex = 0; columnIndex < columnCount; columnIndex++) {
				Column column = columns[columnIndex];
				int rowCount = column.Rows.Count;
				for(int rowIndex = 0; rowIndex < rowCount; rowIndex++) {
					rowContentBounds = MathMethods.Scale(ScaledGraphicsDocumentLayoutExporter.CalcRowContentBoundsByBoxes(column.Rows[rowIndex]), scaleFactor);
					if((rowContentBounds.Top <= pageBottom) && (pageBottom < rowContentBounds.Bottom)) {
						return rowContentBounds.Top;
					}
				}
			}
			return pageBottom;
		}
		#endregion
		protected RectangleF CalcClientRectUsingPaddingAndBorders(RectangleF boundsInDocument) {
			return Padding.Deflate(GetClientRectangle(boundsInDocument, GraphicsDpi.Document), GraphicsDpi.Document);
		}
		protected void SetInternalRtf(string rtfText) {
			RecreateDocManagerAndPrinter();
			XtraRichTextEditHelper.ImportRtfTextToDocManager(rtfText, documentModel);
		}
	}
	public class RichTextBrickBaseExporter : VisualBrickExporter {
		RichTextBrickBase RichTextBrickBase { get { return Brick as RichTextBrickBase; } }
		protected override void DrawClientContent(IGraphics gr, RectangleF clientRect) {
			RichTextBrickBase.PrepareDocumentPrinter(gr);
			GdiGraphics gdiGraph = gr as GdiGraphics;
			System.Drawing.Drawing2D.SmoothingMode mode = System.Drawing.Drawing2D.SmoothingMode.None;
			if(gdiGraph != null) {
				mode = gdiGraph.Graphics.SmoothingMode;
				gdiGraph.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			}
			try {
				RichTextBrickBase.Printer.Draw(gr, clientRect, VisualBrick.GetScaleFactor(gr));
			} finally {
				if(gdiGraph != null)
					gdiGraph.Graphics.SmoothingMode = mode;
			}
		}
		protected override void FillHtmlTableCellCore(IHtmlExportProvider exportProvider) {
			exportProvider.CurrentCell.Style["text-decoration"] = "none";
			RichTextBrickBase.PrepareDocumentPrinter(exportProvider.ExportContext);
			HtmlCellRtfContentCreator creator = new HtmlCellRtfContentCreator(exportProvider.CurrentCell);
			IList<bool> patches = PatchParagraphLineSpacing(RichTextBrickBase.Printer.DocumentModel.ActivePieceTable.Paragraphs);
			creator.CreateContent(RichTextBrickBase.Printer.DocumentModel, exportProvider.HtmlExportContext);
			HtmlHelper.SetClip(exportProvider.CurrentCell, Point.Empty, exportProvider.CurrentCellBounds.Size);
			RestoreParagraphLineSpacing(RichTextBrickBase.Printer.DocumentModel.ActivePieceTable.Paragraphs, patches);
		}
		static IList<bool> PatchParagraphLineSpacing(IndexedTree<Paragraph, ParagraphIndex> paragraphs) {
			System.Collections.Generic.List<bool> patches = new System.Collections.Generic.List<bool>(paragraphs.Count);
			int i = 0;
			foreach(Paragraph paragraph in paragraphs) {
				patches.Add(paragraph.LineSpacingType == ParagraphLineSpacing.Single);
				if(patches[i]) {
					paragraph.LineSpacingType = ParagraphLineSpacing.Multiple;
					paragraph.LineSpacing = 1.2f;
				}
				i++;
			}
			return patches;
		}
		static void RestoreParagraphLineSpacing(IndexedTree<Paragraph, ParagraphIndex> paragraphs, IList<bool> patches) {
			int i = 0;
			foreach(Paragraph paragraph in paragraphs) {
				if(patches[i]) {
					paragraph.LineSpacingType = ParagraphLineSpacing.Single;
					paragraph.LineSpacing = 0.0f;
				}
				i++;
			}
		}
		protected internal override void FillRtfTableCellInternal(IRtfExportProvider exportProvider) {
			RichTextBrickBase.PrepareDocumentPrinter(exportProvider.ExportContext);
			exportProvider.SetAnchor(RichTextBrickBase.AnchorName);
			string innerRtf = GetInnerRtf(exportProvider.RtfExportContext, RichTextBrickBase.Printer, ((RtfDocumentProviderBase)exportProvider).GetParagraphAppearance());
			exportProvider.SetContent(innerRtf);
		}
		protected internal override void FillXlsTableCellInternal(IXlsExportProvider exportProvider) {
			RichTextBrickBase.PrepareDocumentPrinter(exportProvider.ExportContext);
			exportProvider.SetCellData(RichTextBrickBase.Printer.GetPlainText());
		}
		static string GetInnerRtf(RtfExportContext context, GraphicsDocumentPrinter printer, DevExpress.XtraPrinting.Export.Rtf.RtfDocumentProviderBase.PrintingParagraphAppearance paragraphAppearance) {
			RtfDocumentExporterOptions options = new RtfDocumentExporterOptions();
			TableFrameRtfExporter exporter = new TableFrameRtfExporter(printer.DocumentModel, options, context.RtfExportHelper) { ParagraphAppearance = paragraphAppearance };
			exporter.ExportAsNestedTable = !context.IsPageByPage;
			exporter.Export();
			return exporter.RtfBuilder.RtfContent.ToString();
		}
		protected internal override void FillTextTableCellInternal(ITableExportProvider exportProvider, bool shouldSplitText) {
			RichTextBrickBase.PrepareDocumentPrinter(exportProvider.ExportContext);
			if(exportProvider.CurrentData is TextBrickViewData)
				((TextBrickViewData)exportProvider.CurrentData).Texts = new SimpleTextLayoutTable(RichTextBrickBase.Printer.GetFormattedPlainText());
		}
		protected internal override BrickViewData[] GetExportData(ExportContext exportContext, RectangleF rect, RectangleF clipRect) {
			if(!RichTextBrickBase.IsVisible)
				return new BrickViewData[0];
			RichTextBrickBase.PrepareDocumentPrinter(exportContext);
			if(exportContext is RtfPageExportContext && !Contains(rect.Height, clipRect.Height)) {
				List<BrickViewData> data = new List<BrickViewData>();
				data.Add(CreateOutFrame(exportContext, rect));
				foreach(Brick brick in RichTextBrickBase.InnerBricks) {
					VisualBrick visualBrick = brick as VisualBrick;
					if(visualBrick == null)
						continue;
					RectangleF brickRect = GraphicsUnitConverter.Convert(visualBrick.Rect, GraphicsDpi.Document, GraphicsDpi.Pixel);
					PaddingInfo parentPadding = RichTextBrickBase.Padding;
					brickRect.Offset(GraphicsUnitConverter.Convert(new PointF(parentPadding.Left, parentPadding.Top), parentPadding.Dpi, GraphicsDpi.Pixel));
					brickRect.Offset(rect.Location);
					if(clipRect.IntersectsWith(brickRect)) {
						BrickViewData[] viewData = brick is DevExpress.Office.Printing.OfficePanelBrick ?
							((PanelBrickExporter)GetExporter(exportContext, brick)).GetExportData(exportContext, brickRect, rect) :
							new BrickViewData[] { exportContext.CreateBrickViewData(visualBrick.Style, brickRect, brick) };
						data.AddRange(viewData);
					}
				}
				return data.ToArray();
			}
			if(exportContext is HtmlExportContext && (Style.Font.Strikeout || Style.Font.Underline)) {
				using(BrickStyle style = (BrickStyle)Style.Clone()) {
					Font font = style.Font;
				using(Font newFont = new Font(font.FontFamily, font.Size, font.Style & ~(FontStyle.Strikeout | FontStyle.Underline), font.Unit, font.GdiCharSet, font.GdiVerticalFont)) {
					style.Font = newFont;
					return exportContext.CreateBrickViewDataArray(style, rect, TableCell);
				}
				}
			}
			return exportContext.CreateBrickViewDataArray(Style, rect, TableCell);
		}
		BrickViewData CreateOutFrame(ExportContext exportContext, RectangleF rect) {
			RectangleF brickRect = GraphicsUnitConverter.Convert(RichTextBrickBase.Rect, GraphicsDpi.Document, GraphicsDpi.Pixel);
			brickRect.X = rect.X;
			brickRect.Y = rect.Y;
			return exportContext.CreateBrickViewData(RichTextBrickBase.Style, brickRect, new PanelBrick());
		}
		static bool Contains(double inner, double outer) {
			return inner - 0.001 < outer;
		}
	}
}
