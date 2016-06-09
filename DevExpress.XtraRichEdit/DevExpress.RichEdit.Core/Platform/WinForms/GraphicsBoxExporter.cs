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
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.Office.Drawing;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
using DevExpress.Office.Utils;
using DevExpress.Office.Model;
namespace DevExpress.XtraRichEdit.Layout.Export {
	public class WinFormsGraphicsDocumentLayoutExporterAdapter : GraphicsDocumentLayoutExporterAdapter {
		public override void ExportLineBoxCore<T>(GraphicsDocumentLayoutExporter exporter, IPatternLinePainter<T> linePainter, UnderlineBox lineBox, PatternLine<T> line, Color lineColor) {
			Rectangle underlineClipBounds = line.CalcLineBounds(lineBox.ClipBounds, lineBox.UnderlineThickness);
			underlineClipBounds = exporter.GetDrawingBounds(underlineClipBounds);
			Painter painter = exporter.Painter;
			RectangleF oldClip = painter.ClipBounds;
			RectangleF clipRect = new RectangleF(underlineClipBounds.X, oldClip.Y, underlineClipBounds.Width, oldClip.Height);
			try {
				clipRect.Intersect(oldClip);
				painter.ClipBounds = clipRect;
				Rectangle actualUnderlineBounds = line.CalcLineBounds(lineBox.UnderlineBounds, lineBox.UnderlineThickness);
				actualUnderlineBounds.Y += (int)line.CalcLinePenVerticalOffset(actualUnderlineBounds);
				actualUnderlineBounds = exporter.GetDrawingBounds(actualUnderlineBounds);
				line.Draw(linePainter, actualUnderlineBounds, lineColor);
			}
			finally {
				painter.ClipBounds = oldClip;
			}
		}
		public  override void ExportInlinePictureBox(GraphicsDocumentLayoutExporter exporter, InlinePictureBox box) {
			OfficeImage img = box.GetImage(exporter.PieceTable, exporter.ReadOnly);
			Rectangle imgBounds = exporter.GetDrawingBounds(box.Bounds);
			Size size = box.GetImageActualSizeInLayoutUnits(exporter.PieceTable);
			Painter painter = exporter.Painter;
			ImageSizeMode sizing = box.GetSizing(exporter.PieceTable);
			if (exporter.IsValidBounds(imgBounds)) {
				Rectangle rowBounds = exporter.CurrentRow.Bounds;
				if (box.Bounds.Height > rowBounds.Height) {
					RectangleF oldClip = painter.ClipBounds;
					RectangleF clipRect = exporter.GetDrawingBounds(rowBounds);
					try {
						clipRect.Intersect(oldClip);
						painter.ClipBounds = clipRect;
						ExportInlinePictureBoxCore(exporter, box, img, imgBounds, size, sizing);
					}
					finally {
						painter.ClipBounds = oldClip;
					}
				}
				else {
					ExportInlinePictureBoxCore(exporter, box, img, imgBounds, size, sizing);
				}
			}
		}
		void ExportInlinePictureBoxCore(GraphicsDocumentLayoutExporter exporter, InlinePictureBox box, OfficeImage img, Rectangle imgBounds, Size size, ImageSizeMode sizing) {
			Painter painter = exporter.Painter;
			painter.DrawImage(img, imgBounds, size, sizing);
			if (!exporter.ReadOnly)
				box.ExportHotZones(painter);
			if (exporter.ShouldGrayContent())
				painter.FillRectangle(DXColor.FromArgb(0x80, 0xFF, 0xFF, 0xFF), imgBounds);
		}
		public override void ExportFloatingObjectPicture(GraphicsDocumentLayoutExporter exporter, FloatingObjectBox box, PictureFloatingObjectContent pictureContent) {
			Rectangle imgBounds = exporter.GetDrawingBounds(box.ContentBounds);
			if (exporter.IsValidBounds(imgBounds)) {
				if (imgBounds.Width == 0 || imgBounds.Height == 0)
					return;
				exporter.Painter.DrawImage(pictureContent.Image, imgBounds);
				if (exporter.ShouldGrayContent())
					exporter.Painter.FillRectangle(DXColor.FromArgb(0x80, 0xFF, 0xFF, 0xFF), imgBounds);
			}
		}
		public override void ExportTabSpaceBoxCore(GraphicsDocumentLayoutExporter exporter, TabSpaceBox box) {
			ExportSingleCharacterMarkBoxCore(exporter, box);
		}
		public override void ExportSeparatorBoxCore(GraphicsDocumentLayoutExporter exporter, SeparatorBox box) {
			ExportSingleCharacterMarkBoxCore(exporter, box);
		}
		void ExportSingleCharacterMarkBoxCore(GraphicsDocumentLayoutExporter exporter, SingleCharacterMarkBox box) {
			Rectangle boxBounds = exporter.GetDrawingBounds(box.Bounds);
			if (!exporter.IsValidBounds(boxBounds))
				return;
			Color foreColor = exporter.GetActualColor(box.GetActualForeColor(exporter.PieceTable, exporter.TextColors, exporter.GetBackColor(boxBounds)));
			FontInfo fontInfo = box.GetFontInfo(exporter.PieceTable);
			string text = new String(box.MarkCharacter, 1);
			Size textSize = exporter.DocumentModel.FontCache.Measurer.MeasureString(text, fontInfo);
			Rectangle textBounds = boxBounds;
			textBounds.X += (textBounds.Width - textSize.Width) / 2;
			textBounds.Width = textSize.Width;
			textBounds.Y += (textBounds.Height - textSize.Height) / 2;
			exporter.Painter.DrawString(text, fontInfo, foreColor, textBounds);
		}
		public override void ExportLineBreakBoxCore(GraphicsDocumentLayoutExporter exporter, LineBreakBox box) {
			Rectangle characterBounds = exporter.GetDrawingBounds(box.Bounds);
			Rectangle glyphBounds = characterBounds;
			glyphBounds.Height = characterBounds.Width;
			glyphBounds.Offset(0, (characterBounds.Height - glyphBounds.Height) / 2);
			Color foreColor = exporter.GetActualColor(box.GetActualForeColor(exporter.PieceTable, exporter.TextColors, exporter.GetBackColor(glyphBounds)));
			DrawLineBreakArrow(exporter, glyphBounds, foreColor);
		}
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
		public override void ExportTabLeader(GraphicsDocumentLayoutExporter exporter, TabSpaceBox box) {
			exporter.ExportTabLeaderAsCharacterSequence(box);
		}
	}
}
