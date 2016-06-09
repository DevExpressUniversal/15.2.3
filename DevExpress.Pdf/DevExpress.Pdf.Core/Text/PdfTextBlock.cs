#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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
using System.Text;
using System.Collections.Generic;
namespace DevExpress.Pdf.Native { 
	public class PdfTextBlock {
		readonly List<PdfCharacter> characters;
		readonly PdfFontData fontData;
		readonly double fontSize;
		readonly double characterSpacing;
		readonly double textWidthFactor;
		readonly double textHeightFactor;
		readonly PdfPoint location;
		readonly double angle;
		readonly PdfPoint[] charLocations;
		public IList<PdfCharacter> Characters { get { return characters; } }
		public PdfFontData FontData { get { return fontData; } }
		public double FontSize { get { return fontSize; } }
		public PdfPoint Location { get { return location; } }
		public double Angle { get { return angle; } }
		public double FontHeight { 
			get {
				const double maxHeightDifferenceRatio = 2.5;
				double height = fontData.Height * textHeightFactor;
				height = height <= Math.Abs(FontDescent) ? fontData.BBoxHeight * textHeightFactor + FontDescent : height;
				return height / fontSize > maxHeightDifferenceRatio ? fontSize : height;
			}  
		}
		public double FontDescent { get { return fontData.Descent * textHeightFactor; } }
		public PdfPoint[] CharLocations { get { return charLocations; } }
		public PdfTextBlock(PdfStringData data, PdfFontData fontData, double fontSize, double characterSpacing, double textWidthFactor, double textHeightFactor, double[] glyphOffsets, PdfPoint location, double angle) {
			this.fontData = fontData;
			this.fontSize = fontSize;
			this.characterSpacing = characterSpacing;
			this.textWidthFactor = textWidthFactor;
			this.textHeightFactor = textHeightFactor;
			this.location = location;
			this.angle = angle;
			double x = location.X;
			double y = location.Y;
			int length = glyphOffsets.Length;
			charLocations = new PdfPoint[length];
			charLocations[0] = new PdfPoint(x, y);
			double cos = Math.Cos(angle);
			double sin = Math.Sin(angle);
			for (int i = 1, offset = 0; i < length; i++) {
				double glyphOffset = glyphOffsets[offset++];
				charLocations[i] =  new PdfPoint(x + glyphOffset * cos, y + glyphOffset * sin);
			}
			PdfFont font = fontData.Font;
			characters = new List<PdfCharacter>();
			byte[][] charCodes = data.CharCodes;
			short[] str = data.Str;
			int count = charCodes.Length;
			double fontDescent = FontDescent;
			double fontHeight = FontHeight;
			if (fontDescent == 0 && fontData.BBoxHeight != 0) {
				fontDescent = -fontData.BBoxHeight * 0.2 * textHeightFactor;
				fontHeight -= fontDescent;
			}
			double height = fontHeight + fontDescent;
			for (int i = 0; i < count; i++) {
				PdfPoint rotatedLocation = PdfTextUtils.RotatePoint(charLocations[i], -angle);
				PdfOrientedRectangle charRect = new PdfOrientedRectangle(PdfTextUtils.RotatePoint(new PdfPoint(rotatedLocation.X, rotatedLocation.Y + height), angle), CalculateCharacterWidth(str[i]), fontHeight, angle);
				characters.Add(new PdfCharacter(GetActualCharacter(charCodes[i]), font, fontSize, charRect));
			}
		}
		double CalculateCharacterWidth(int character) {
			double[] widths = fontData.Widths;
			int index = character - fontData.FirstChar;
			return ((widths == null || index < 0 || index > widths.Length - 1) ? fontData.MissingWidth : widths[index]) * textWidthFactor + characterSpacing;
		}
		string GetActualCharacter(byte[] character) {
			PdfCharacterMapping toUnicode = fontData.ToUnicode;
			if (toUnicode != null)
				return toUnicode.MapCode(character);
			PdfSimpleFont font = fontData.Font as PdfSimpleFont;
			if (font == null)
				return Encoding.BigEndianUnicode.GetString(character, 0, character.Length);
			short code = 0;
			int codeLength = character.Length;
			for (int i = 0; i < codeLength; i++)
				code = (short)((code << 8) + character[i]);
			return new String((char)PdfUnicodeConverter.GetGlyphCode(code, font.Encoding), 1);
		}
	}
}
