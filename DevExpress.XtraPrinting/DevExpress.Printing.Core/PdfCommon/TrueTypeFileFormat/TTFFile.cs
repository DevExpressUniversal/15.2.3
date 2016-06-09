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
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
#if SL
using DevExpress.Xpf.Collections;
#endif
namespace DevExpress.Pdf.Common {
	class TTFFile {
		TTFTableDirectory tableDirectory;
		TTFCMap cmap;
		TTFGlyf glyf;
		TTFHead head;
		TTFHHea hhea;
		TTFHMtx hmtx;
		TTFLoca loca;
		TTFMaxP maxp;
		TTFPost post;
		TTFOS2 os2;
		TTFName name;
		TTFBinaryTable prep;
		TTFBinaryTable cvt;
		TTFBinaryTable fpgm;
		bool hasPrep = true;
		bool hasCVT = true;
		bool hasFPGM = true;
		uint offset;
		int fontCodePage;
		public TTFTableDirectory TableDirectory { get { return tableDirectory; } }
		public TTFHead Head { get { return head; } }
		public TTFMaxP MaxP { get { return maxp; } }
		public TTFHHea HHea { get { return hhea; } }
		public TTFHMtx HMtx { get { return hmtx; } }
		public TTFPost Post { get { return post; } }
		public TTFOS2 OS2 { get { return os2; } }
		public TTFLoca Loca { get { return loca; } }
		public TTFGlyf Glyf { get { return glyf; } }
		public TTFCMap CMap { get { return cmap; } }
		public TTFName Name { get { return name; } }
		public TTFBinaryTable Prep { get { return prep; } }
		public TTFBinaryTable CVT { get { return cvt; } }
		public TTFBinaryTable FPGM { get { return fpgm; } }
		public bool IsEmbeddableFont { get { return OS2.FsType != 2; } }
		public uint Offset { get { return offset; } }
		public int FontCodePage { get { return fontCodePage; } set { fontCodePage = value; } }
		public TTFFile()
			: this(0, 0) {
		}
		public TTFFile(uint offset)
			: this(offset, 0) {
		}
		public TTFFile(uint offset, int fontCodePage) {
			this.offset = offset;
			this.fontCodePage = fontCodePage;
			tableDirectory = new TTFTableDirectory(this);
			head = new TTFHead(this);
			maxp = new TTFMaxP(this);
			hhea = new TTFHHea(this);
			hmtx = new TTFHMtx(this);
			post = new TTFPost(this);
			os2 = new TTFOS2(this);
			loca = new TTFLoca(this);
			glyf = new TTFGlyf(this);
			cmap = new TTFCMap(this);
			name = new TTFName(this);
			prep = new TTFBinaryTable(this, "prep");
			cvt = new TTFBinaryTable(this, "cvt ");
			fpgm = new TTFBinaryTable(this, "fpgm");
		}
		void AddGlyphIndex(ushort glyphIndex, TTFGlyphIndexCache cache) {
			if(cache.Add(glyphIndex)) {
				TTFGlyphData glyphData = Glyf.Glyphs[(int)glyphIndex];
				if(glyphData != null) {
					TTFCompositeGlyphDescription compositeDescription = glyphData.Description as TTFCompositeGlyphDescription;
					if(compositeDescription != null) {
						for(int i = 0; i < compositeDescription.Count; i++)
							AddGlyphIndex(compositeDescription[i], cache);
					}
				}
			}
		}
		void Initialize(TTFFile pattern, TTFInitializeParam param) {
			Glyf.Initialize(pattern.Glyf, param);
			Head.Initialize(pattern.Head); 
			Loca.Initialize(pattern.Loca);
			MaxP.Initialize(pattern.MaxP);
			HHea.Initialize(pattern.HHea);
			HMtx.Initialize(pattern.HMtx);
			this.hasPrep = pattern.hasPrep;
			this.hasCVT = pattern.hasCVT;
			this.hasFPGM = pattern.hasFPGM;
			if(this.hasPrep)
				Prep.Initialize(pattern.Prep);
			if(this.hasCVT)
				CVT.Initialize(pattern.CVT);
			if(this.hasFPGM)
				FPGM.Initialize(pattern.FPGM);
			tableDirectory.Initialize(pattern.TableDirectory);
		}
		void Write(TTFStream ttfStream) {
			tableDirectory.Write(ttfStream);
			Head.Write(ttfStream);
			HHea.Write(ttfStream);
			MaxP.Write(ttfStream);
			HMtx.Write(ttfStream);
			Loca.Write(ttfStream);
			Glyf.Write(ttfStream);
			if(this.hasPrep)
				Prep.Write(ttfStream);
			if(this.hasCVT)
				CVT.Write(ttfStream);
			if(this.hasFPGM)
				FPGM.Write(ttfStream);
			ttfStream.Pad4();
			tableDirectory.WriteOffsets(ttfStream);
			tableDirectory.WriteCheckSum(ttfStream);
			Head.WriteCheckSumAdjustment(ttfStream);
		}
		bool TryToRead(TTFTable optionalTable, TTFStream ttfStream) {
			try {
				optionalTable.Read(ttfStream);
				return true;
			} catch(TTFFileException) {
				return false;
			}
		}
		public void Read(byte[] data) {
			TTFStream ttfStream = new TTFStreamAsByteArray(data);
			tableDirectory.Read(ttfStream);
			head.Read(ttfStream);
			hhea.Read(ttfStream);
			maxp.Read(ttfStream);
			hmtx.Read(ttfStream);
			loca.Read(ttfStream);
			glyf.Read(ttfStream);
			this.hasPrep = TryToRead(prep, ttfStream);
			this.hasCVT = TryToRead(cvt, ttfStream);
			this.hasFPGM = TryToRead(fpgm, ttfStream);
			os2.Read(ttfStream);
			post.Read(ttfStream);
			cmap.Read(ttfStream);
			name.Read(ttfStream);
		}
		internal void ReadNameTable(TTFStream ttfStream) {
			tableDirectory.Read(ttfStream);
			name.Read(ttfStream);
		}
		public void Write(Stream stream, PdfCharCache charCache, string newFontName) {
			TTFFile subset = new TTFFile();
			TTFInitializeParam param = new TTFInitializeParam();
			param.Chars = charCache;
			param.NewFontName = newFontName;
			subset.Initialize(this, param);
			subset.Write(new TTFStreamAsStream(stream));
		}
		public ushort GetCharWidth(char ch) {
			return GetCharWidth(GetGlyphIndex(ch));
		}
		public ushort GetCharWidth(ushort glyphIndex) {
			return HMtx[glyphIndex].AdvanceWidth;
		}
		public ushort GetGlyphIndex(char ch) {
			return CMap[(ushort)ch];
		}
		public ushort[] CreateGlyphIndices(PdfCharCache charCache) {
			TTFGlyphIndexCache cache = new TTFGlyphIndexCache();
			AddGlyphIndex(0, cache);
			foreach(char char_ in charCache) {
				ushort glyphIndex = CMap[Convert.ToUInt16(char_)];
				AddGlyphIndex(glyphIndex, cache);
				charCache.AddUniqueGlyph(glyphIndex, new char[] { char_ });
			}
			foreach(ushort glyphIndex in charCache.GlyphIndices)
				AddGlyphIndex(glyphIndex, cache);
			return cache.ToArray;
		}
		public static bool IsIdentical(TTFFile ttfFile1, TTFFile ttfFile2) {
			if(ttfFile1 == null || ttfFile2 == null || ttfFile1.tableDirectory.Count != ttfFile2.tableDirectory.Count)
				return false;
			for(int i = 0; i < ttfFile1.tableDirectory.Count; i++) {
				if(ttfFile1.tableDirectory[i].CheckSum != ttfFile2.tableDirectory[i].CheckSum)
					return false;
			}
			return true;
		}
	}
	class TTFGlyphIndexCache {
		HashSet<ushort> innerList = new HashSet<ushort>();
		public int Count { get { return innerList.Count; } }
		public ushort[] ToArray {
			get {
				List<ushort> result = new List<ushort>(innerList);
				result.Sort();
				return result.ToArray();
			}
		}
		public bool Add(ushort glyphIndex) {
			if(!innerList.Contains(glyphIndex)) {
				innerList.Add(glyphIndex);
				return true;
			} else
				return false;
		}
		public void Clear() {
			innerList.Clear();
		}
	}
	class TTFUtils {
		public static uint CalculateCheckSum(TTFStream ttfStream, int start, int length) {
			int remainder = length % 4;
			if(remainder > 0)
				length += 4 - remainder;
			length /= 4;
			int oldPosition = ttfStream.Position;
			try {
				ttfStream.Seek(start);
				uint checkSum = 0;
				for(int i = 0; i < length; i++)
					checkSum += ttfStream.ReadULong();
				return checkSum;
			} finally {
				ttfStream.Seek(oldPosition);
			}
		}
		TTFUtils() {
		}
	}
	public class TTFFileException : Exception {
		public TTFFileException() {
		}
		public TTFFileException(string message)
			: base(message) {
		}
		public TTFFileException(string message, Exception innerEx)
			: base(message, innerEx) {
		}
	}
}
