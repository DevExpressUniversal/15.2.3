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
using DevExpress.Office.Model;
using DevExpress.Office.Utils;
using DevExpress.Office.Drawing;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraRichEdit {
	public class PdfAdapter : GraphicsDocumentLayoutExporterAdapter {
		static PointF[] arrowPointsTemplate = new PointF[] {
			new PointF(0.00f, 0.60f),
			new PointF(0.35f, 0.35f),
			new PointF(0.30f, 0.55f),
			new PointF(0.80f, 0.55f),
			new PointF(0.80f, 0.10f),
			new PointF(0.90f, 0.10f),
			new PointF(0.90f, 0.65f),
			new PointF(0.30f, 0.65f),
			new PointF(0.35f, 0.85f)
		};
		public override void ExportLineBoxCore<T>(GraphicsDocumentLayoutExporter exporter, IPatternLinePainter<T> linePainter, UnderlineBox lineBox, PatternLine<T> line, Color lineColor) {
			Rectangle underlineClipBounds = line.CalcLineBounds(lineBox.ClipBounds, lineBox.UnderlineThickness);
			PdfPainterBase pdfPainter = (PdfPainterBase)exporter.Painter;
			RectangleF oldClip = pdfPainter.GetRectangularClipBounds();
			RectangleF clipRect = new RectangleF(underlineClipBounds.X, oldClip.Y, underlineClipBounds.Width, oldClip.Height);
			pdfPainter.PdfGraphics.SaveGraphicsState();
			try {
				pdfPainter.PdfGraphics.IntersectClip(clipRect);
				Rectangle actualUnderlineBounds = line.CalcLineBounds(lineBox.UnderlineBounds, lineBox.UnderlineThickness);
				actualUnderlineBounds.Y += (int)line.CalcLinePenVerticalOffset(actualUnderlineBounds);
				actualUnderlineBounds = exporter.GetDrawingBounds(actualUnderlineBounds);
				line.Draw(linePainter, actualUnderlineBounds, lineColor);
			}
			finally {
				pdfPainter.PdfGraphics.RestoreGraphicsState();
			}
		}
		public override void ExportInlinePictureBox(GraphicsDocumentLayoutExporter exporter, InlinePictureBox box) {
			OfficeImage img = box.GetImage(exporter.PieceTable, exporter.ReadOnly);
			Rectangle imgBounds = box.Bounds;
			exporter.Painter.DrawImage(img, imgBounds);
		}
		public override void ExportTabSpaceBoxCore(GraphicsDocumentLayoutExporter exporter, TabSpaceBox box) {
			ExportSingleCharacterMarkBoxCore(exporter, box);
		}
		public override void ExportSeparatorBoxCore(GraphicsDocumentLayoutExporter exporter, SeparatorBox box) {
			ExportSingleCharacterMarkBoxCore(exporter, box);
		}
		void ExportSingleCharacterMarkBoxCore(GraphicsDocumentLayoutExporter exporter, SingleCharacterMarkBox box) {
			Rectangle bounds = box.Bounds;
			if (!exporter.IsValidBounds(bounds))
				return;
			string text = new String(box.MarkCharacter, 1);
			FontInfo fontInfo = box.GetFontInfo(exporter.PieceTable);
			Color foreColor = exporter.GetActualColor(box.GetActualForeColor(exporter.PieceTable, exporter.TextColors, exporter.GetBackColor(bounds)));
			exporter.Painter.DrawString(text, fontInfo, foreColor, bounds);
		}
		public override void ExportLineBreakBoxCore(GraphicsDocumentLayoutExporter exporter, LineBreakBox box) {
			Rectangle characterBounds = exporter.GetDrawingBounds(box.Bounds);
			Rectangle glyphBounds = characterBounds;
			glyphBounds.Height = characterBounds.Width;
			glyphBounds.Offset(0, (characterBounds.Height - glyphBounds.Height) / 2);
			Color foreColor = exporter.GetActualColor(box.GetActualForeColor(exporter.PieceTable, exporter.TextColors, exporter.GetBackColor(glyphBounds)));
			DrawLineBreakArrow(exporter, glyphBounds, foreColor);
		}
		public override void ExportTabLeader(GraphicsDocumentLayoutExporter exporter, TabSpaceBox box) {
			exporter.ExportTabLeaderAsCharacterSequence(box);
		}
		public override void ExportFloatingObjectPicture(GraphicsDocumentLayoutExporter exporter, FloatingObjectBox box, PictureFloatingObjectContent pictureContent) {
			Rectangle imgBounds = box.ContentBounds;
			if (!exporter.IsValidBounds(imgBounds))
				return;
			if (imgBounds.Width == 0 || imgBounds.Height == 0)
				return;
			exporter.Painter.DrawImage(pictureContent.Image, imgBounds);
			if (exporter.ShouldGrayContent())
				exporter.Painter.FillRectangle(DXColor.FromArgb(0x80, 0xFF, 0xFF, 0xFF), imgBounds);
		}
		protected internal virtual void DrawLineBreakArrow(GraphicsDocumentLayoutExporter exporter, Rectangle glyphBounds, Color foreColor) {
			int count = arrowPointsTemplate.Length;
			PointF[] arrowPoints = new PointF[count];
			for (int i = 0; i < count; i++) {
				PointF pt = arrowPointsTemplate[i];
				arrowPoints[i] = new PointF(glyphBounds.X + glyphBounds.Width * pt.X, glyphBounds.Y + glyphBounds.Height * pt.Y);
			}
			using (Brush brush = new SolidBrush(foreColor)) {
				exporter.Painter.FillPolygon(brush, arrowPoints);
			}
		}
	}
}
