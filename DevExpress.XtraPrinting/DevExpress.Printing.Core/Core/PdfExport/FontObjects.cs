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
using System.Collections;
using System.Drawing;
using System.IO;
using System.Text;
using DevExpress.Utils;
using System.Collections.Generic;
using DevExpress.Pdf.Common;
namespace DevExpress.XtraPrinting.Export.Pdf {
	public abstract class PdfFontBase : PdfDocumentDictionaryObject, IDisposable {
		PdfFont owner;
		internal PdfFont Owner { get { return owner; } }
		internal Font Font { get { return Owner.Font; } }
		internal string Name { get { return Owner.Name; } }
		internal TTFFile TTFFile { get { return Owner.TTFFile; } }
		public abstract string Subtype { get; }
		public abstract string BaseFont { get; }
		protected PdfFontBase(PdfFont owner, bool compressed)
			: base(compressed) {
			this.owner = owner;
		}
		public virtual string ProcessText(TextRun info) {
			return "(" + PdfStringUtils.EscapeString(info.Text) + ")";
		}
		public override void FillUp() {
			Dictionary.Add("Type", "Font");
			Dictionary.Add("Subtype", Subtype);
			if(Name != null)
				Dictionary.Add("Name", Name);
			Dictionary.Add("BaseFont", BaseFont);
		}
		public abstract void Dispose();
	}
	class PdfTrueTypeFont : PdfFontBase {
		public const int FirstChar = 32;
		public const int LastChar = 255;
		PdfFontDescriptor fontDescriptor;
		protected PdfArray widths;
		public override string Subtype { get { return "TrueType"; } }
		public override string BaseFont { 
			get {
				return Owner.TTFFile != null && FamilyNamesAreDifferent(Owner.TTFFile.Name) ?
					PdfStringUtils.ClearSpaces(Owner.TTFFile.Name.PostScriptName) :
					PdfFontUtils.GetTrueTypeFontName(Font, false);
			}
		}
		static bool FamilyNamesAreDifferent(TTFName name) {
			return !string.IsNullOrEmpty(name.MacintoshFamilyName) &&
					name.MacintoshFamilyName != name.FamilyName &&
					!string.IsNullOrEmpty(name.PostScriptName);
		}
		public PdfFontDescriptor FontDescriptor { get { return fontDescriptor; } }
		public PdfTrueTypeFont(PdfFont owner, bool compressed) : base(owner, compressed) {
			this.fontDescriptor = CreateFontDescriptor();
			this.widths = new PdfArray(PdfObjectType.Indirect);
			FillWidths();
		}
		protected virtual PdfFontDescriptor CreateFontDescriptor() {
			return new PdfFontDescriptor(this, Compressed);
		}
		protected virtual void FillWidths() {
			widths.MaxRowCount = 8;
			for(int i = FirstChar; i <= LastChar; i++) {
				byte[] ansiBytes = BitConverter.GetBytes(i);
				byte ansi = BitConverter.IsLittleEndian ? ansiBytes[0] : ansiBytes[ansiBytes.Length - 1];
				byte[] unicodeBytes = Encoding.Convert(DXEncoding.GetEncoding(1252), Encoding.Unicode, new byte[] { ansi });
				char ch = Encoding.Unicode.GetChars(unicodeBytes)[0];
				widths.Add((int)Owner.GetCharWidth(ch));				
			}
		}
		protected override void RegisterContent(PdfXRef xRef) {
			base.RegisterContent(xRef);
			xRef.RegisterObject(widths);
			fontDescriptor.Register(xRef);
		}
		protected override void WriteContent(StreamWriter writer) {
			base.WriteContent(writer);
			widths.WriteIndirect(writer);
			fontDescriptor.Write(writer);
		}
		public override void FillUp() {
			base.FillUp();
			Dictionary.Add("FirstChar", FirstChar);
			Dictionary.Add("LastChar", LastChar);
			Dictionary.Add("Widths", widths);
			Dictionary.Add("Encoding", "WinAnsiEncoding");			
			Dictionary.Add("FontDescriptor", fontDescriptor.Dictionary);
			fontDescriptor.FillUp();
		}
		public override void Dispose() {
			if(this.fontDescriptor != null) {
				this.fontDescriptor.Dispose();
				this.fontDescriptor = null;
			}
		}
	}
	class PdfType0Font : PdfFontBase {
		PdfCIDFont cidFont;
		PdfToUnicodeCMap toUnicode;
		public override string Subtype { get { return "Type0"; } }
		public override string BaseFont { get { return PdfFontUtils.GetTrueTypeFontName(Font, true); } }	
		public PdfCIDFont CIDFont { get { return cidFont; } }
		public PdfType0Font(PdfFont owner, bool compressed) : base(owner, compressed) {
			cidFont = new PdfCIDFont(this, Compressed);
			toUnicode = new PdfToUnicodeCMap(this, Compressed);
		}
		protected override void RegisterContent(PdfXRef xRef) {
			base.RegisterContent(xRef);
			cidFont.Register(xRef);
			toUnicode.Register(xRef);
		}
		protected override void WriteContent(StreamWriter writer) {
			base.WriteContent(writer);
			cidFont.Write(writer);
			toUnicode.Write(writer);
		}
		public override string ProcessText(TextRun text) {
			StringBuilder result = new StringBuilder(2 + 4 * (text.HasGlyphs ? text.Glyphs.Length : text.Text.Length));
			result.Append('<');
			if(text.HasGlyphs) {
				foreach(short glyph in text.Glyphs)
					result.Append(((ushort)glyph).ToString("X4"));
			} else {
				foreach(char ch in text.Text)
				if(ch != '\0')
					result.Append(TTFFile.GetGlyphIndex(ch).ToString("X4"));
			}
			result.Append('>');
			return result.ToString();
		}
		public override void FillUp() {
			base.FillUp();
			PdfArray descendantFonts = new PdfArray();
			descendantFonts.Add(cidFont.Dictionary);
			Dictionary.Add("DescendantFonts", descendantFonts);
			Dictionary.Add("Encoding", "Identity-H");
			cidFont.FillUp();
			Dictionary.Add("ToUnicode", toUnicode.Stream);
			toUnicode.FillUp();
		}
		public override void Dispose() {
			if(this.cidFont != null) {
				this.cidFont.Dispose();
				this.cidFont = null;
			}
			if(this.toUnicode != null) {
				this.toUnicode.Close();
				this.toUnicode = null;
			}
		}
	}
	class PdfCIDFont : PdfDocumentDictionaryObject, IDisposable {
		PdfFontBase ownerFont;
		PdfFontDescriptor fontDescriptor;
		PdfArray widths;
		public PdfFontBase OwnerFont { get { return ownerFont; } } 
		public PdfFontDescriptor FontDescriptor { get { return fontDescriptor; } } 
		public PdfCIDFont(PdfFontBase ownerFont, bool compressed) : base(compressed) {
			this.ownerFont = ownerFont;
			fontDescriptor = new PdfFontDescriptor(ownerFont, Compressed);
			widths = new PdfArray();
			widths.MaxRowCount = 2;
		}
		void FillConsecutiveWidths(ArrayList list) {
			if(list.Count == 0) return;
			PdfArray temp = new PdfArray();
			for(int i = 0; i < list.Count; i++)
				temp.Add((int)OwnerFont.Owner.GetCharWidth((ushort)list[i]));
			widths.Add((ushort)list[0]);
			widths.Add(temp);
			list.Clear();
		}
		void FillWidths() {
			PdfCharCache charCache = OwnerFont.Owner.CharCache;
			ArrayList consecutiveGlyphIndeces = new ArrayList();
			foreach(ushort glyphIndex in charCache.GlyphIndices) {
				if(consecutiveGlyphIndeces.Count > 0) 
					if(((ushort)consecutiveGlyphIndeces[consecutiveGlyphIndeces.Count - 1] + 1) != glyphIndex)
						FillConsecutiveWidths(consecutiveGlyphIndeces);
				consecutiveGlyphIndeces.Add(glyphIndex);
			}
			FillConsecutiveWidths(consecutiveGlyphIndeces);
		}
		protected override void RegisterContent(PdfXRef xRef) {
			base.RegisterContent(xRef);
			fontDescriptor.Register(xRef);
		}
		protected override void WriteContent(StreamWriter writer) {
			base.WriteContent(writer);
			fontDescriptor.Write(writer);
		}
		public override void FillUp() {
			Dictionary.Add("Type", "Font");
			Dictionary.Add("Subtype", "CIDFontType2");
			Dictionary.Add("BaseFont", OwnerFont.BaseFont);
			PdfDictionary CIDSystemInfo = new PdfDictionary();
			CIDSystemInfo.Add("Registry", new PdfLiteralString("Adobe"));
			CIDSystemInfo.Add("Ordering", new PdfLiteralString("Identity"));
			CIDSystemInfo.Add("Supplement", 0);
			Dictionary.Add("CIDSystemInfo", CIDSystemInfo);
			Dictionary.Add("FontDescriptor", fontDescriptor.Dictionary);
			fontDescriptor.FillUp();
			Dictionary.Add("CIDToGIDMap", "Identity");
			Dictionary.Add("W", widths);
			FillWidths();
		}
		public void Dispose() {
			if(this.fontDescriptor != null) {
				this.fontDescriptor.Dispose();
				this.fontDescriptor = null;
			}
		}
	}
	class PdfFontDescriptor : PdfDocumentDictionaryObject, IDisposable {
		protected PdfFontBase ownerFont;
		PdfStream fontFile;
		internal TTFFile TTFFile { get { return OwnerFont.TTFFile; } }
		internal PdfFont Owner { get { return OwnerFont.Owner; } }
		public PdfFontBase OwnerFont { get { return ownerFont; } }
		public PdfFontDescriptor(PdfFontBase ownerFont, bool compressed) : base(compressed) {
			this.ownerFont = ownerFont;
			if(ownerFont is PdfType0Font)
				fontFile = Compressed ? new PdfFlateStream(true) : new PdfStream(true);
		}
		protected byte[] SetBit(byte[] value, int bitNumber) {
			bitNumber--;
			if(bitNumber < 0 || bitNumber >= value.Length * 8) return value;
			int byteN = (int)(bitNumber / 8);
			int bitN = bitNumber % 8;
			byte mask = (byte)(0x01 << bitN); 
			value[byteN] |= mask;
			return value;
		}
		protected virtual int GetFlags() {
			byte[] flags = new byte[4] { 0, 0, 0, 0 };
			flags = SetBit(flags, (int)PdfFontFlags.UsesTheAdobeStandardRomanCharacterSet);
			if(TTFFile.OS2.Panose.bProportion == 9) 
				flags = SetBit(flags, (int)PdfFontFlags.FixedWidthFont);
			if(TTFFile.OS2.Panose.bSerifType != 11 && 
				TTFFile.OS2.Panose.bSerifType != 12 && 
				TTFFile.OS2.Panose.bSerifType != 13) 
				flags = SetBit(flags, (int)PdfFontFlags.SerifFont);
			if(TTFFile.OS2.Panose.bFamilyType == 3) 
				flags = SetBit(flags, (int)PdfFontFlags.ScriptFont);
			return BitConverter.ToInt32(flags, 0);
		}
		protected override void WriteContent(StreamWriter writer) {
			if(fontFile != null) 
				fontFile.WriteIndirect(writer);
		}
		protected override void RegisterContent(PdfXRef xRef) {
			if(fontFile != null)
				xRef.RegisterObject(fontFile);
		}
		public override void FillUp() {
			if(ownerFont == null) return;
			Dictionary.Add("Type", "FontDescriptor");
			Dictionary.Add("FontName", OwnerFont.BaseFont);
			Dictionary.Add("Ascent", (int)Math.Round(TTFFile.HHea.Ascender * Owner.ScaleFactor));
			Dictionary.Add("CapHeight", 500);
			Dictionary.Add("Descent", (int)Math.Round(TTFFile.HHea.Descender * Owner.ScaleFactor));
			Dictionary.Add("Flags", GetFlags());
			int llx = (int)Math.Round(TTFFile.Head.XMin * Owner.ScaleFactor);
			int lly = (int)Math.Round(TTFFile.Head.YMin * Owner.ScaleFactor);
			int urx = (int)Math.Round(TTFFile.Head.XMax * Owner.ScaleFactor);
			int ury = (int)Math.Round(TTFFile.Head.YMax * Owner.ScaleFactor);
			Dictionary.Add("FontBBox", new PdfRectangle(llx, lly, urx, ury));
			Dictionary.Add("ItalicAngle", (int)TTFFile.Post.ItalicAngle);
			Dictionary.Add("StemV", 0);
			if(fontFile != null) {
				if(!TTFFile.IsEmbeddableFont)
					throw new PdfException(string.Format("The {0} font cannot be embedded", Owner.Font.Name));
				Dictionary.Add("FontFile2", fontFile);
				Owner.WriteFontSubset(fontFile.Data);
			}
		}
		public void Dispose() {
			if(this.fontFile != null) {
				this.fontFile.Close();
				this.fontFile = null;
			}
		}
	}
	class PdfToUnicodeCMap : PdfToUnicodeCMapBase {
		PdfFontBase ownerFont;
		public PdfFontBase OwnerFont { get { return ownerFont; } }
		public PdfToUnicodeCMap(PdfFontBase ownerFont, bool compressed) : base(compressed) {
			this.ownerFont = ownerFont;
		}
		protected override string CreateCMapName() {
			return PdfFontUtils.Subname + "+" + OwnerFont.Name;
		}
		protected override PdfCharCache GetCharCache() {
			return OwnerFont.Owner.CharCache;
		}
	}
}
