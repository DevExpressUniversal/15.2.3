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
#if SL
using DevExpress.Xpf.Collections;
#endif
namespace DevExpress.Pdf.Common {
	class TTFGlyf : TTFTable {
		TTFGlyphData[] glyphs;
		List<TTFGlyphData> purgedGlyphs = new List<TTFGlyphData>();
		protected internal override string Tag { get { return "glyf"; } }
		public int Count { get { return purgedGlyphs.Count; } }
		public TTFGlyphData this[int index] { get { return purgedGlyphs[index]; } }
		public TTFGlyphData[] Glyphs { get { return glyphs; } }
		public override int Length {
			get {
				int result = 0;
				for(int i = 0; i < Count; i++)
					result += this[i].Size;
				return result;
			}
		}
		public TTFGlyf(TTFFile ttfFile)
			: base(ttfFile) {
		}
		void AssignGlyphData(int index, TTFGlyphData glyphData) {
			glyphs[index] = glyphData;
			if(glyphData != null)
				purgedGlyphs.Add(glyphData);
		}
		protected override void ReadTable(TTFStream ttfStream) {
			glyphs = new TTFGlyphData[Owner.MaxP.NumGlyphs];
			purgedGlyphs.Clear();
			int startPosition = ttfStream.Position;
			for(int i = 0; i < Owner.MaxP.NumGlyphs; i++) {
				uint glyphOffset = Owner.Loca[(ushort)i];
				uint glyphOffsetNext = Owner.Loca[(ushort)(i + 1)];
				if(glyphOffsetNext != glyphOffset) {
					AssignGlyphData(i, new TTFGlyphData((int)(glyphOffsetNext - glyphOffset)));
					ttfStream.Seek(startPosition);
					ttfStream.Move((int)glyphOffset);
					glyphs[i].Read(ttfStream);
				}
			}
		}
		protected override void WriteTable(TTFStream ttfStream) {
			for(int i = 0; i < Count; i++)
				this[i].Write(ttfStream);
		}
		protected override void InitializeTable(TTFTable pattern, TTFInitializeParam param) {
			TTFGlyf p = pattern as TTFGlyf;
			glyphs = new TTFGlyphData[p.Glyphs.Length];
			purgedGlyphs.Clear();
			ushort[] glyphIndices = param.Chars.GlyphIndices;
			foreach(ushort glyphIndex in glyphIndices)
				AssignGlyphData(glyphIndex, p.Glyphs[glyphIndex]);
		}
	}
	class TTFGlyphData {
		const int headerSize = 10;
		short numberOfContours;
		short xMin;
		short yMin;
		short xMax;
		short yMax;
		TTFGlyphDescription description;
		int size;
		public TTFGlyphDescription Description { get { return description; } }
		public int Size { get { return size; } }
		public short XMin { get { return xMin; } }
		public short YMin { get { return yMin; } }
		public short XMax { get { return xMax; } }
		public short YMax { get { return yMax; } }
		public TTFGlyphData(int size) {
			this.size = size;
		}
		void ReadDescription(TTFStream ttfStream) {
			if(numberOfContours >= 0)
				description = new TTFGlyphDescription();
			else
				description = new TTFCompositeGlyphDescription();
			description.Read(ttfStream, size - headerSize);
		}
		public void Read(TTFStream ttfStream) {
			numberOfContours = ttfStream.ReadShort();
			xMin = ttfStream.ReadFWord();
			yMin = ttfStream.ReadFWord();
			xMax = ttfStream.ReadFWord();
			yMax = ttfStream.ReadFWord();
			ReadDescription(ttfStream);
		}
		public void Write(TTFStream ttfStream) {
			ttfStream.WriteShort(numberOfContours);
			ttfStream.WriteFWord(xMin);
			ttfStream.WriteFWord(yMin);
			ttfStream.WriteFWord(xMax);
			ttfStream.WriteFWord(yMax);
			if(description != null)
				description.Write(ttfStream);
		}
	}
	class TTFGlyphDescription {
		byte[] data;
		public byte[] Data { get { return data; } }
		public virtual void Read(TTFStream ttfStream, int size) {
			data = ttfStream.ReadBytes(size);
		}
		public void Write(TTFStream ttfStream) {
			ttfStream.WriteBytes(data);
		}
	}
	class TTFCompositeGlyphDescription : TTFGlyphDescription {
		const ushort ARG_1_AND_2_ARE_WORDS = 1;
		const ushort WE_HAVE_A_SCALE = 8;
		const ushort MORE_COMPONENTS = 32;
		const ushort WE_HAVE_AN_X_AND_Y_SCALE = 64;
		const ushort WE_HAVE_A_TWO_BY_TWO = 128;
		List<ushort> glyphIndexList = new List<ushort>();
		public int Count { get { return glyphIndexList.Count; } }
		public ushort this[int index] { get { return glyphIndexList[index]; } }
		void CreateGlyphIndexList(TTFStream ttfStream) {
			ushort flags = 0;
			do {
				flags = ttfStream.ReadUShort();
				glyphIndexList.Add(ttfStream.ReadUShort());
				if((flags & ARG_1_AND_2_ARE_WORDS) != 0)
					ttfStream.Move(TTFStream.SizeOf_Short * 2);
				else
					ttfStream.Move(TTFStream.SizeOf_UShort);
				if((flags & WE_HAVE_A_SCALE) != 0)
					ttfStream.Move(TTFStream.SizeOf_F2Dot14);
				else if((flags & WE_HAVE_AN_X_AND_Y_SCALE) != 0)
					ttfStream.Move(TTFStream.SizeOf_F2Dot14 * 2);
				else if((flags & WE_HAVE_A_TWO_BY_TWO) != 0)
					ttfStream.Move(TTFStream.SizeOf_F2Dot14 * 4);
			} while((flags & MORE_COMPONENTS) != 0);
		}
		public override void Read(TTFStream ttfStream, int size) {
			int startPosition = ttfStream.Position;
			CreateGlyphIndexList(ttfStream);
			ttfStream.Seek(startPosition);
			base.Read(ttfStream, size);
		}
	}
}
