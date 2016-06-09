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

using System.IO;
using System.Drawing;
namespace DevExpress.Pdf.Drawing {
	public class EmfLogFont {
		static readonly string[] fontNames = {
								"Courier", "Courier-Bold", "Courier-Oblique", "Courier-BoldOblique",
								"Helvetica", "Helvetica-Bold", "Helvetica-Oblique", "Helvetica-BoldOblique",
								"Times-Roman", "Times-Bold", "Times-Italic", "Times-BoldItalic",
								"Symbol", "ZapfDingbats"};
		static readonly string[] fontTokens = {
								"courier","terminal", "fixedsys", "ms sans serif", "arial", "system",
								"arial black", "times", "ms serif", "roman", "symbol"};
		readonly int height;
		readonly int width;
		readonly int escapement;
		readonly int orientation;
		readonly int weight;
		readonly bool isItalic;
		readonly bool isUnderline;
		readonly bool isStrikeOut;
		readonly EmfCharacterSet characterSet;
		readonly EmfFamilyFont family;
		readonly EmfPitchFont pitch;
		readonly string faceName;
		public int Width { get { return width; } }
		public int Escapement { get { return escapement; } }
		public int Orientation { get { return orientation; } }
		public EmfCharacterSet CharacterSet { get { return characterSet; } }
		public EmfLogFont(BinaryReader reader) {
			height = reader.ReadInt32();
			width = reader.ReadInt32();
			escapement = reader.ReadInt32();
			orientation = reader.ReadInt32();
			weight = reader.ReadInt32();
			isItalic = reader.ReadBoolean();
			isUnderline = reader.ReadBoolean();
			isStrikeOut = reader.ReadBoolean();
			characterSet = EmfEnumToValueConverter.ParseEmfEnum<EmfCharacterSet>(reader.ReadByte());
			reader.ReadBytes(3);
			byte fontPitchAndFamilyByte = reader.ReadByte();
			family = EmfEnumToValueConverter.Parse<EmfFamilyFont>(fontPitchAndFamilyByte >> 4);
			pitch = (EmfPitchFont)(fontPitchAndFamilyByte & 0x03);
			char[] faceNameChars = new char[32];
			int i = 0;
			for (; i < 32; i++) {
				char c = reader.ReadChar();
				if (c == 0)
					break;
				faceNameChars[i] = c;
			}
			faceName = new string(faceNameChars);
		}
		public Font GetGdiFont(EmfMetafileGraphics context) {
			float size = height;
			FontStyle style = (isItalic ? FontStyle.Italic : 0) | (isUnderline ? FontStyle.Underline : 0) |
				(isStrikeOut ? FontStyle.Strikeout : 0) | (weight > 500 ? FontStyle.Bold : 0);
			string fontName = null;
			int styleValue = ((style & FontStyle.Bold) != 0 ? 1 : 0) + ((style & FontStyle.Italic) != 0 ? 2 : 0);
			int[][] tokensIndices = new int[][] { new[] { 0, 1, 2 }, new[] { 3, 4, 5 }, new[] { 7 }, new[] { 8, 9, 10 }, new[] { 11 } };
			int k = 0;
			for (int i = 0; i < tokensIndices.Length; i++) {
				for (int j = 0; j < tokensIndices[i].Length; j++) {
					if (faceName.Contains(fontTokens[tokensIndices[i][j]])) {
						fontName = fontNames[k + styleValue];
						return new Font(fontName, size, style);
					}
				}
				k += tokensIndices[i].Length;
			}
			switch (family) {
				case EmfFamilyFont.FF_DECORATIVE:
					fontName = fontNames[4 + styleValue];
					break;
				case EmfFamilyFont.FF_ROMAN:
					fontName = fontNames[8 + styleValue];
					break;
				case EmfFamilyFont.FF_SCRIPT:
				case EmfFamilyFont.FF_SWISS:
				case EmfFamilyFont.FF_MODERN:
					fontName = fontNames[styleValue];
					break;
				default:
					break;
			}
			switch (pitch) {
				case EmfPitchFont.FIXED_PITCH:
					fontName = fontNames[styleValue];
					break;
				default:
					fontName = fontNames[4 + styleValue];
					break;
			}
			return new Font(fontName, size, style);
		}
	}
}
