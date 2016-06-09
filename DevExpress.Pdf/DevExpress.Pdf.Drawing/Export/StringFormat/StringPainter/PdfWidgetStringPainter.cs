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

using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	class PdfWidgetStringPainter : PdfStringPainter {
		readonly int spaceGlyphIndex;
		double horizontalScaling = 100;
		double charSpacing;
		double wordSpacing;
		public PdfWidgetStringPainter(PdfCommandConstructor constructor, int spaceGlyphIndex)
			: base(constructor) {
			this.spaceGlyphIndex = spaceGlyphIndex;
		}
		public void SetCharSpacing(double value) {
			charSpacing = value;
		}
		public void SetWordSpacing(double value) {
			wordSpacing = value;
		}
		public void SetHorizontalScaling(double value) {
			horizontalScaling = value;
		}
		protected override double GetGlyphRunWidth(PdfStringGlyphRun glyphRun, double emFactor) {
			double width = 0;
			foreach (PdfStringGlyph glyph in glyphRun.Glyphs) {
				double glyphWidth = (glyph.Width - glyph.GlyphOffset) * emFactor;
				if (horizontalScaling != 100)
					glyphWidth *= horizontalScaling / 100;
				if (glyphWidth > 0)
					glyphWidth += charSpacing;
				else
					glyphWidth = 0;
				if (glyph.GlyphIndex == spaceGlyphIndex)
					glyphWidth += wordSpacing;
				width += glyphWidth;
			}
			return width;
		}
		protected override void BeginTextDrawing() {
		}
		protected override void EndTextDrawing() {
		}
	}
}
