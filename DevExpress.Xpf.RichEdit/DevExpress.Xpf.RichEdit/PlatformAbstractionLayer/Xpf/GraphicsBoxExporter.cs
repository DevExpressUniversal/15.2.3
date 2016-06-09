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
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office;
using DevExpress.Office.Drawing;
using DevExpress.Office.Model;
using DevExpress.Office.Utils;
using DevExpress.XtraPrinting;
#if SL
using PlatformIndependentColor = System.Windows.Media.Color;
#else
using PlatformIndependentColor = System.Drawing.Color;
#endif
namespace DevExpress.XtraRichEdit.Layout.Export {
	public class XpfGraphicsDocumentLayoutExporterAdapter : GraphicsDocumentLayoutExporterAdapter {
		public override void ExportInlinePictureBox(GraphicsDocumentLayoutExporter exporter, InlinePictureBox box) {
			OfficeImage img = box.GetImage(exporter.PieceTable, exporter.ReadOnly);
			Rectangle imgBounds = exporter.GetDrawingBounds(box.Bounds);
			Size size = box.GetImageActualSizeInLayoutUnits(exporter.PieceTable);
			ImageSizeMode sizing = box.GetSizing(exporter.PieceTable);
			exporter.Painter.DrawImage(img, imgBounds, size, sizing);
		}
		public override void ExportFloatingObjectPicture(GraphicsDocumentLayoutExporter exporter, FloatingObjectBox box, PictureFloatingObjectContent pictureContent) {
			OfficeImage img = pictureContent.Image;
			Rectangle imgBounds = exporter.GetDrawingBounds(box.ContentBounds);
			exporter.Painter.DrawImage(img, imgBounds);
			if (exporter.ShouldGrayContent())
				exporter.Painter.FillRectangle(DXColor.FromArgb(0x80, 0xFF, 0xFF, 0xFF), imgBounds);
		}
		public override void ExportLineBoxCore<T>(GraphicsDocumentLayoutExporter exporter, IPatternLinePainter<T> linePainter, UnderlineBox lineBox, PatternLine<T> line, PlatformIndependentColor lineColor) {
			Rectangle underlineClipBounds = line.CalcLineBounds(lineBox.ClipBounds, lineBox.UnderlineThickness);
			Rectangle actualUnderlineBounds = line.CalcLineBounds(lineBox.UnderlineBounds, lineBox.UnderlineThickness);
			actualUnderlineBounds.Y += (int)line.CalcLinePenVerticalOffset(new RectangleF(actualUnderlineBounds.Location, actualUnderlineBounds.Size));
			Rectangle result = new Rectangle(actualUnderlineBounds.Location, actualUnderlineBounds.Size);
			result.Intersect(new Rectangle(underlineClipBounds.X, actualUnderlineBounds.Y, underlineClipBounds.Width, actualUnderlineBounds.Height));
			line.Draw(linePainter, exporter.GetDrawingBounds(result), lineColor);
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
			FontCache cache = exporter.DocumentModel.FontCache;
			int fontIndex = cache.CalcFontIndex("Arial", 16, false, false, CharacterFormattingScript.Normal, false, false);
			FontInfo fontInfo = cache[fontIndex];
			string text = new String(box.MarkCharacter, 1);
			Size textSize = cache.Measurer.MeasureString(text, fontInfo);
			Rectangle textBounds = boxBounds;
			textBounds.X += (textBounds.Width - textSize.Width) / 2;
			textBounds.Width = textSize.Width;
			textBounds.Y += (textBounds.Height - textSize.Height) / 2;
			PlatformIndependentColor foreColor = box.GetActualForeColor(exporter.PieceTable, exporter.TextColors, exporter.GetBackColor(textBounds));
			exporter.Painter.DrawString(text, fontInfo, foreColor, textBounds);
		}
		public override void ExportLineBreakBoxCore(GraphicsDocumentLayoutExporter exporter, LineBreakBox box) {
			Rectangle boxBounds = exporter.GetDrawingBounds(box.Bounds);
			if (!exporter.IsValidBounds(boxBounds))
				return;
			FontCache cache = exporter.DocumentModel.FontCache;
			int fontIndex = cache.CalcFontIndex("Arial", 16, false, false, CharacterFormattingScript.Normal, false, false);
			FontInfo fontInfo = cache[fontIndex];
			string text = "\u00AC";
			Size textSize = cache.Measurer.MeasureString(text, fontInfo);
			Rectangle textBounds = boxBounds;
			textBounds.Y += (textBounds.Height - textSize.Height) / 2;
			PlatformIndependentColor foreColor = box.GetActualForeColor(exporter.PieceTable, exporter.TextColors, exporter.GetBackColor(textBounds));
			exporter.Painter.DrawString(text, fontInfo, foreColor, textBounds);
		}
		public override void ExportTabLeader(GraphicsDocumentLayoutExporter exporter, TabSpaceBox box) {
			exporter.ExportTabLeaderAsUnderline(box);
		}
	}
}
