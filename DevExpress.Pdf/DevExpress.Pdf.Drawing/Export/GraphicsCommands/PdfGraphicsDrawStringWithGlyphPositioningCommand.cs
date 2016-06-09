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

using System.Drawing;
using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	class PdfGraphicsDrawStringWithGlyphPositioningCommands : PdfGraphicsNonStrokingCommand {
		readonly FontStyle fontStyle;
		readonly string fontFamily;
		readonly string text;
		readonly PointF location;
		readonly PdfGraphicsTextOrigin textOrigin;
		readonly float fontSize;
		readonly float[] glyphPositions;
		readonly ushort[] glyphIndices;
		readonly int[] order;
		public PdfGraphicsDrawStringWithGlyphPositioningCommands(PdfSolidBrushContainer brush, string fontFamily, FontStyle fontStyle, float fontSize, PointF location, PdfGraphicsTextOrigin textOrigin, string text, ushort[] glyphIndices, float[] glyphPositions, int[] order)
			: base(brush) {
			this.fontStyle = fontStyle;
			this.fontFamily = fontFamily;
			this.text = text;
			this.location = location;
			this.textOrigin = textOrigin;
			this.fontSize = fontSize;
			this.glyphPositions = glyphPositions;
			this.glyphIndices = glyphIndices;
			this.order = order;
		}
		public override void Execute(PdfGraphicsPageContentsCommandConstructor constructor) {
			base.Execute(constructor);
			PdfEditableFontData fontData = constructor.FontCache.GetEditableFontData(fontStyle, fontFamily);
			PdfStringGlyphRun textRun;
			double scaleFactor = fontSize / 1000.0 * constructor.DpiX / 72.0;
			if (fontData.Embedded) {
				int count = glyphIndices.Length;
				textRun = new PdfStringGlyphRun();
				double previousWidth = 0;
				for (int i = 0; i < count; i++) {
					int index = order[i];
					ushort glyphIndex = glyphIndices[index];
					double width = fontData.GetCharWidth(glyphIndex);
					double offset = i == 0 ? 0 : previousWidth - glyphPositions[order[i - 1]] / scaleFactor;
					textRun.Append(new PdfStringGlyph(glyphIndex, width, offset));
					previousWidth = width;
				}
				fontData.RegisterString(PdfGDIGlyphMapper.PrepareCharMap(text, order, glyphIndices));
			}
			else {
				int count = text.Length;
				textRun = new PdfNotEmbeddedStringGlyphRun();
				double previousWidth = 0;
				bool kern = count == glyphPositions.Length;
				for (int i = 0; i < count; i++) {
					char ch = text[i];
					double width = fontData.GetCharWidth(ch);
					double offset = i > 0 && kern ? previousWidth - glyphPositions[i - 1] / scaleFactor : 0;
					textRun.Append(new PdfStringGlyph(ch, width, offset));
					previousWidth = width;
				}
			}
			constructor.DrawPrecalculatedString(textRun, location, textOrigin, fontData, fontSize, ((PdfSolidBrushContainer)Brush).Brush.Color);
		}
	}
}
