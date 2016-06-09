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
using System.Text;
using System.IO;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public class PdfType1FontFileCreator  : PdfDisposableObject {
		static Stream CreateStream(string fileName) {
			return new FileStream(fileName, FileMode.Create);
		}
		static short ConvertToShort(double value) {
			return (short)Math.Ceiling(value);
		}
		public static string CreateType1FontFiles(IType1Font font, string pfmFileName, string pfbFileName) {
			using (PdfType1FontFileCreator creator = new PdfType1FontFileCreator(font, pfmFileName, pfbFileName))
				return creator.CreateFiles();
		}
		readonly IType1Font font;
		readonly short top;
		readonly short height;
		readonly short fontWeight;
		readonly byte pitchAndFamily;
		readonly short avgWidth;
		readonly short maxWidth;
		readonly short capHeight;
		readonly short xHeight;
		readonly short ascent;
		readonly short descent;
		readonly Stream pfmStream;
		readonly Stream pfbStream;
		PdfType1FontFileCreator(IType1Font font, string pfmFileName, string pfbFileName) {
			this.font = font;
			PdfFontDescriptor fontDescriptor = font.FontDescriptor;
			if (fontDescriptor == null) {
				top = 0;
				height = 0;
				fontWeight = PdfFontRegistrator.NormalWeight;
				pitchAndFamily = 48;
				avgWidth = 0;
				maxWidth = 0;
				capHeight = 0;
				xHeight = 0;
				ascent = 0;
				descent = 0;
			}
			else {
				PdfRectangle fontBBox = fontDescriptor.FontBBox;
				top = ConvertToShort(fontBBox.Top);
				height = ConvertToShort(Math.Max(0, fontBBox.Height - 1000));
				fontWeight = (short)fontDescriptor.FontWeight;
				PdfFontFlags flags = fontDescriptor.Flags;
				if ((flags & PdfFontFlags.Serif) == PdfFontFlags.Serif)
					pitchAndFamily = 16;
				else if ((flags & PdfFontFlags.Script) == PdfFontFlags.Script)
					pitchAndFamily = 64;
				else
					pitchAndFamily = 48;
				if ((flags & PdfFontFlags.FixedPitch) == PdfFontFlags.FixedPitch)
					pitchAndFamily += 1;
				avgWidth = ConvertToShort(fontDescriptor.AvgWidth);
				maxWidth = ConvertToShort(fontDescriptor.MaxWidth);
				capHeight = ConvertToShort(fontDescriptor.CapHeight);
				xHeight = ConvertToShort(fontDescriptor.XHeight);
				ascent = ConvertToShort(fontDescriptor.Ascent);
				descent = ConvertToShort(Math.Abs(fontDescriptor.Descent));
			}
			pfmStream = CreateStream(pfmFileName);
			pfbStream = CreateStream(pfbFileName);
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				pfmStream.Dispose();
				pfbStream.Dispose();
			}
		}
		void WriteByte(byte value) {
			pfmStream.WriteByte(value);
		}
		void WriteShort(short value) {
			pfmStream.WriteByte((byte)(value & 0xFF));
			pfmStream.WriteByte((byte)((value & 0xFF00) >> 8));
		}
		void WriteInt(Stream stream, int value) {
			stream.WriteByte((byte)(value & 0xFF));
			stream.WriteByte((byte)((value & 0xFF00) >> 8));
			stream.WriteByte((byte)((value & 0xFF0000) >> 16));
			stream.WriteByte((byte)((value & 0xFF000000) >> 24));
		}
		void WriteString(string value) {
			foreach (char c in value)
				pfmStream.WriteByte((byte)c);
			pfmStream.WriteByte((byte)0);
		}
		string CreateFiles() {
			string fontName = font.BaseFont;
			byte[] fontFileData = font.FontFileData;
			int plainTextLength = font.PlainTextLength;
			int cipherTextLength = font.CipherTextLength;
			int nullSegmentLength = font.NullSegmentLength;
			PdfType1FontProgram fontProgram = PdfType1FontProgram.Create(font);
			if (fontProgram != null) {
				if (fontProgram.Validate()) {
					byte[] validFontProgram = Encoding.UTF8.GetBytes(fontProgram.ToPostScript());
					int newPlainTextLength = validFontProgram.Length;
					int remainLength = cipherTextLength + nullSegmentLength;
					byte[] newFontFileData = new byte[newPlainTextLength + remainLength];
					Array.Copy(validFontProgram, newFontFileData, newPlainTextLength);
					Array.Copy(fontFileData, plainTextLength, newFontFileData, newPlainTextLength, remainLength);
					fontFileData = newFontFileData;
					plainTextLength = newPlainTextLength;
				}
				fontName = fontProgram.FontName;
			}
			pfbStream.WriteByte((byte)0x80);
			pfbStream.WriteByte((byte)0x01);
			WriteInt(pfbStream, plainTextLength);
			pfbStream.Write(fontFileData, 0, plainTextLength);
			pfbStream.WriteByte((byte)0x80);
			pfbStream.WriteByte((byte)0x02);
			WriteInt(pfbStream, cipherTextLength);
			pfbStream.Write(fontFileData, plainTextLength, cipherTextLength);
			pfbStream.WriteByte((byte)0x80);
			pfbStream.WriteByte((byte)0x01);
			WriteInt(pfbStream, nullSegmentLength);
			pfbStream.Write(fontFileData, plainTextLength + cipherTextLength, nullSegmentLength);
			pfbStream.WriteByte((byte)0x80);
			pfbStream.WriteByte((byte)0x03);
			WriteShort(256);
			WriteInt(pfmStream, 0);
			for (int i = 0; i < 60; i++) 
				WriteByte(0);
			WriteShort(129);
			WriteShort(10);
			WriteShort(300);
			WriteShort(300);
			WriteShort(top);
			WriteShort(height);
			WriteShort(196);
			WriteByte(0);   
			WriteByte(0);
			WriteByte(0);
			WriteShort(fontWeight);
			WriteByte(0);
			WriteShort(0);
			WriteShort(1000);
			WriteByte(pitchAndFamily);
			WriteShort(avgWidth);
			WriteShort(maxWidth);
			WriteByte((byte)font.FirstChar);
			WriteByte((byte)font.LastChar);
			WriteByte(0);
			WriteByte(0);
			WriteShort(0);
			WriteInt(pfmStream, 199);
			WriteInt(pfmStream, 210);
			WriteInt(pfmStream, 0);
			WriteInt(pfmStream, 0);
			WriteShort(30);
			WriteInt(pfmStream, 147);
			WriteInt(pfmStream, 0);
			WriteInt(pfmStream, 0);
			WriteInt(pfmStream, 0);
			WriteInt(pfmStream, 0);
			WriteInt(pfmStream, 0);
			WriteInt(pfmStream, 0);
			WriteShort(52);
			WriteShort(240);
			WriteShort(0);
			WriteShort(1000);
			WriteShort(3);
			WriteShort(1000);
			WriteShort(1000);
			WriteShort(capHeight);
			WriteShort(xHeight);
			WriteShort(ascent);
			WriteShort(descent);
			WriteShort(0);
			WriteShort(-500);
			WriteShort(250);
			WriteShort(500);
			WriteShort(500);
			WriteShort(0);
			WriteShort(0);
			WriteShort(0);
			WriteShort(0);
			WriteShort(0);
			WriteShort(0);
			WriteShort(405);
			WriteShort(50);
			WriteShort(0);
			WriteShort(0);
			WriteString("PostScript");
			WriteString(fontName);
			long offsetFullyqualifiedName = pfmStream.Position;
			WriteString(fontName);
			long offsetExtentTable = pfmStream.Position;
			foreach (double width in font.Widths)
				WriteShort(ConvertToShort(width));
			pfmStream.Position = 2;
			WriteInt(pfmStream, (int)pfmStream.Length);
			pfmStream.Position = 123;
			WriteInt(pfmStream, (int)offsetExtentTable);
			pfmStream.Position = 139;
			WriteInt(pfmStream, (int)offsetFullyqualifiedName);
			return fontName;
		}
	}
}
