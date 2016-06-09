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

#if !DXPORTABLE
using System.IO;
using System.Drawing;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.Pdf;
using DevExpress.Pdf.Drawing;
using DevExpress.Pdf.Native;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit {
	public class PdfGraphicsDocumentLayoutExporter : GraphicsDocumentLayoutExporter {
		const float defaultPageDpi = 72f;
		DocumentLayout layout;
		PdfDocument pdfDocument;
		PdfPage currentPage;
		PdfImageCache imageCache;
		PdfEditableFontDataCache editableFontDataCache;
		IPdfLinkUpdater linkUpdater;
		readonly Dictionary<string, string> links = new Dictionary<string, string>();
		readonly Dictionary<string, PdfAnchor> anchors = new Dictionary<string, PdfAnchor>();
		class PdfAnchor {
			readonly PdfPage page;
			PointF location;
			public PdfAnchor(PdfPage page, PointF location) {
				this.page = page;
				this.location = location;
			}
			public PdfPage Page { get { return page; } }
			public PointF Location { get { return location; } }
		}
		public PdfGraphicsDocumentLayoutExporter(PdfDocument pdfDocument, DocumentLayout layout, PdfPainterBase painter, PdfAdapter adapter, Rectangle bounds, TextColors textColors)
			: base(layout.DocumentModel, painter, adapter, bounds, textColors) {
				this.layout = layout;
				this.linkUpdater = layout.DocumentModel.GetService<IPdfLinkUpdater>();
				this.pdfDocument = pdfDocument;
				PdfObjectCollection objects = pdfDocument.DocumentCatalog.Objects;
				this.imageCache = new PdfImageCache(objects);
				this.editableFontDataCache = new PdfEditableFontDataCache(objects);
				DrawInactivePieceTableWithDifferentColor = false;
		}
		public new PdfPainterBase Painter { get { return (PdfPainterBase)base.Painter; } }
		public void ExportToPdf(string fileName) {
			using (FileStream stream = new FileStream(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.Read)) {
				ExportToPdf(stream);
			}
		}
		public void ExportToPdf(Stream stream) {
			Guard.ArgumentNotNull(stream, "stream");
			foreach (var page in this.layout.Pages)
				ExportPage(page);
			FinishPdfExport();
		}
		public override void ExportPage(Page page) {
			float width = DocumentModel.LayoutUnitConverter.LayoutUnitsToPointsF(page.Bounds.Width);
			float height = DocumentModel.LayoutUnitConverter.LayoutUnitsToPointsF(page.Bounds.Height);
			currentPage = pdfDocument.AddPage(new PdfRectangle(0, 0, width, height));
			using (PdfGraphics graphics = new PdfGraphics(imageCache, editableFontDataCache)) {
				PdfPainter pdfPainter = (PdfPainter)Painter;
				pdfPainter.SetGraphics(graphics, page.Bounds);
				base.ExportPage(page);
				graphics.AddToPage(currentPage, pdfPainter.Dpi, pdfPainter.Dpi);
			}
		}
		protected override void ExportRowContentBoxes() {
			base.ExportRowContentBoxes();
			if (Painter.HyperlinksSupported)
				ExportLinks();
		}
		void ExportLinks() {
			BoxCollection boxes = CurrentRow.Boxes;
			int lastIndex = GetLastExportBoxInRowIndex(CurrentRow);
			for (int i = 0; i <= lastIndex; i++)
				if (boxes[i].IsHyperlinkSupported)
					ExportBoxLinks(boxes[i]);
		}
		void ExportBoxLinks(Box box) {
			Rectangle bounds = GetDrawingBounds(box.Bounds);
			string url = GetUrl(box);
			string anchor = GetAnchor(box);
			if (this.linkUpdater != null && !string.IsNullOrEmpty(url)) {
				anchor = linkUpdater.UpdateLinkToPage(url, anchor);
				url = linkUpdater.UpdateLinkToUri(url);
			}
			if (!string.IsNullOrEmpty(url))
				Painter.SetUriArea(url, bounds);
			if (!string.IsNullOrEmpty(anchor)) {
				string link = Painter.SetLink(bounds);
				this.links.Add(link, anchor);
			}
		}
		protected override void ExportRowBookmarkBoxes() {
			BookmarkBoxCollection boxes = CurrentRow.InnerBookmarkBoxes;
			if (boxes == null)
				return;
			int count = boxes.Count;
			for (int i = 0; i < count; i++)
				boxes[i].ExportTo(this);
		}
		public override void ExportBookmarkStartBox(VisitableDocumentIntervalBox box) {
			Bookmark bookmark = box.Interval as Bookmark;
			if (bookmark == null) return;
			Rectangle bounds = new Rectangle(CurrentRow.Bounds.Location, new Size(0, CurrentRow.Bounds.Height));
			PdfAnchor anchor = new PdfAnchor(this.currentPage, GetDrawingBounds(bounds).Location);
			this.anchors[bookmark.Name] = anchor;
		}
		public void FinishPdfExport() {
			ResolveLinks();
			editableFontDataCache.UpdateFonts();
			pdfDocument.FinalizeDocument();
		}
		void ResolveLinks() {
			foreach (var link in this.links) {
				PdfAnchor anchor;
				if (!this.anchors.TryGetValue(link.Value, out anchor))
					continue;
				PdfPage page = anchor.Page;
				float x = anchor.Location.X;
				float y = anchor.Location.Y;
				float factor = Painter.Dpi / defaultPageDpi;
				double left = x / factor;
				double top = page.CropBox.Height - y / factor;
				this.pdfDocument.Names.PageDestinations[link.Key] = new PdfXYZDestination(page, left, top, null);
			}
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (this.editableFontDataCache != null) {
					this.editableFontDataCache.Dispose();
					this.editableFontDataCache = null;
				}
				this.anchors.Clear();
			}
			base.Dispose(disposing);
		}
	}
}
#endif
