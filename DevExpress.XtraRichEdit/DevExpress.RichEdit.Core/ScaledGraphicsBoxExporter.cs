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
using DevExpress.Utils;
using DevExpress.Office.Drawing;
using DevExpress.Office.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Layout.TableLayout;
namespace DevExpress.XtraRichEdit.Drawing {
	#region SimpleIGraphicsPainter
	public class SimpleIGraphicsPainter : IGraphicsPainter {
		DevExpress.XtraPrinting.Export.Pdf.PdfGraphics pdfGraphics;
		PointF uriAreaOffset;
		float uriAreaScale;
		public SimpleIGraphicsPainter(IGraphicsBase graphics, PointF uriAreaOffset, float uriAreaScale)
			: base(graphics) {
			pdfGraphics = graphics as DevExpress.XtraPrinting.Export.Pdf.PdfGraphics;
			this.uriAreaOffset = uriAreaOffset;
			this.uriAreaScale = uriAreaScale;
		}
		public override bool HyperlinksSupported { get { return pdfGraphics != null; } }
		public override Pen GetPen(Color color) {
			return new Pen(color);
		}
		public override Pen GetPen(Color color, float thickness) {
			return new Pen(color, (int)thickness);
		}
		public override void ReleasePen(Pen pen) {
			if (pen != null)
				pen.Dispose();
		}
		public override Brush GetBrush(Color color) {
			return new SolidBrush(color);
		}
		public override void ReleaseBrush(Brush brush) {
			if (brush != null)
				brush.Dispose();
		}
		public override void SetUriArea(string uri, RectangleF bounds) {
			if (String.IsNullOrEmpty(uri) || pdfGraphics == null)
				return;
			bounds = new RectangleF(bounds.X * uriAreaScale + uriAreaOffset.X, bounds.Y * uriAreaScale + uriAreaOffset.Y, bounds.Width * uriAreaScale, bounds.Height * uriAreaScale);
			pdfGraphics.SetUriArea(uri, bounds);
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Layout.Export {
	#region ScaledGraphicsDocumentLayoutExporter
	public class ScaledGraphicsDocumentLayoutExporter : GraphicsDocumentLayoutExporter {
		#region static
		public static Rectangle CalcRowContentBoundsByBoxes(Row row) {
			Rectangle contentBounds = Rectangle.Empty;
			int count = row.Boxes.Count;
			if (count > 0) {
				Box lastBox = row.Boxes[count - 1];
				if (lastBox is ParagraphMarkBox || lastBox is SectionMarkBox || lastBox is LineBreakBox)
					count--;
			}
			for (int i = 0; i < count; i++) {
				Box box = row.Boxes[i];
				if (box is TabSpaceBox)
					continue;
				contentBounds = contentBounds.IsEmpty ?
					box.Bounds :
					Rectangle.Union(contentBounds, box.Bounds);
			}
			return contentBounds;
		}
		#endregion
		readonly Painter painter;
		public ScaledGraphicsDocumentLayoutExporter(DocumentModel documentModel, Painter painter, GraphicsDocumentLayoutExporterAdapter adapter, Rectangle bounds, TextColors textColors)
			: base(documentModel, painter, adapter, bounds, textColors) {
			Guard.ArgumentNotNull(painter, "graphics");
			this.painter = painter;
		}
		protected internal override Rectangle CalcRowContentBounds(Row row) {
			return CalcRowContentBoundsByBoxes(row);
		}
		#region IDocumentLayoutExporter implementation
		protected internal override void ExportRowCore() {
			Rectangle rowBounds = CalcRowContentBounds(CurrentRow);
			RectangleF drawingBounds = GetDrawingBounds(rowBounds);
			if (!drawingBounds.IntersectsWith(painter.ClipBounds))
				return;
			ExportBackground(CurrentRow);
			base.ExportRowCore();
		}
		void ExportBackground(Row row) {
			HighlightAreaCollection highlightAreas = CurrentRow.HighlightAreas;
			int count = highlightAreas.Count;
			for (int i = 0; i < count; i++)
				ExportHighlightArea(highlightAreas[i]);
		}
		public override void ExportHighlightArea(HighlightArea area) {
			RectangleF drawingBounds = GetDrawingBounds(area.Bounds);
			if (!drawingBounds.IntersectsWith(painter.ClipBounds))
				return;
			painter.FillRectangle(area.Color, GetDrawingBounds(area.Bounds));
		}
		public override void ExportSpaceBox(Box box) {
			ExportTextBoxCore(box);
		}
		public override void ExportLineNumberBox(LineNumberBox box) {
			Rectangle textBounds = GetDrawingBounds(box.Bounds);
			if (IsValidBounds(textBounds)) {
				string text = GetBoxText(box);
				FontInfo fontInfo = box.GetFontInfo(PieceTable);
				Color backColor = box.GetRun(PieceTable.DocumentModel.MainPieceTable).BackColor;
				if (!DXColor.IsTransparentOrEmpty(backColor))
					ExportHighlightArea(new HighlightArea(backColor, box.Bounds));
				Color foreColor = box.GetActualForeColor(PieceTable, TextColors, GetBackColor(textBounds));
				painter.DrawString(text, fontInfo, foreColor, textBounds);
			}
		}
		protected internal override void ExportNumberingListBoxCore(Box box) {
			Rectangle textBounds = GetDrawingBounds(box.Bounds);
			if (IsValidBounds(textBounds)) {
				string text = GetBoxText(box);
				FontInfo fontInfo = box.GetFontInfo(PieceTable);
				Color foreColor = box.GetActualForeColor(PieceTable, TextColors, GetBackColor(textBounds));
				painter.DrawString(text, fontInfo, foreColor, textBounds);
			}
		}
		internal protected override void ExportTextBoxCore(Box box) {
			string text = GetBoxText(box);
			TextRun run = (TextRun)box.GetRun(PieceTable);
			FontInfo fontInfo = box.GetFontInfo(PieceTable);
			Rectangle textBounds = GetDrawingBounds(box.Bounds);
			if (IsValidBounds(textBounds)) {
				Color foreColor = run.ForeColor;
				if (foreColor == Color.Empty)
					foreColor = Color.Black;
				if (painter.HyperlinksSupported)
					painter.SetUriArea(GetUrl(box), textBounds);
				painter.DrawString(text, fontInfo, foreColor, textBounds);
			}
		}
		public override void ExportHyphenBox(HyphenBox box) {
			Exceptions.ThrowInternalException();
		}
		public override void ExportTableBorderCorner(CornerViewInfoBase corner, int x, int y) {
			TableCornerPainter borderPainter = GetBorderPainter(corner);
			if (borderPainter != null)
				borderPainter.DrawCorner(new GraphicsPainterWrapper(painter, HorizontalLinePainter, VerticalLinePainter), x + Offset.X, y + Offset.Y, corner);
		}
		public override void ExportTableCell(TableCellViewInfo cell) {
			Rectangle rect = cell.TableViewInfo.GetCellBounds(cell);
			int top = cell.TableViewInfo.Anchors[cell.TopAnchorIndex].VerticalPosition;
			int bottom = cell.TableViewInfo.Anchors[cell.BottomAnchorIndex].VerticalPosition;
			rect.Y = top;
			rect.Height = bottom - top;
			painter.FillRectangle(cell.Cell.GetActualBackgroundColor(), GetDrawingBounds(rect));
		}
		public override void ExportTableRow(TableRowViewInfoBase row) {
			Rectangle rect = row.TableViewInfo.GetRowBounds(row);
			painter.FillRectangle(row.TableViewInfo.Table.BackgroundColor, GetDrawingBounds(rect));
		}
		public override void ExportInlinePictureBox(InlinePictureBox box) {
			OfficeImage img = box.GetImage(PieceTable, ReadOnly);
			Size size = box.GetImageActualSizeInLayoutUnits(PieceTable);
			ImageSizeMode sizing = box.GetSizing(PieceTable);
			Rectangle imgBounds = GetDrawingBounds(box.Bounds);
			if (IsValidBounds(imgBounds))
				painter.DrawImage(img, imgBounds, size, sizing);
		}
		#endregion
	}
	#endregion
}
