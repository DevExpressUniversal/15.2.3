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
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.Utils.Internal {
	#region TTFontInfo
	public class TTFontInfo {
		#region Fields
		static readonly int ErrorGlyphWidth = 2000;
		TTFHeader header;
		CMAPTable cmap;
		internal ushort[] widths;
		internal KernTable kernTable = new KernTable();
		#endregion
		#region Properties
		public TTFWeightClass WeightClass { get { return header.WeightClass; } }
		public byte[] Panose { get { return header.Panose; } }
		[CLSCompliant(false)]
		public ushort[] Widths { get { return widths; } }
		#endregion
		#region TTFontInfo
		public TTFontInfo(Stream stream) {
			BigEndianStreamReader reader = new BigEndianStreamReader(stream);
			header = new TTFHeader();
			header.Read(reader);
			if(!header.IsInternal) {
				ReadCMAPTable(reader);
				ReadHMetrics(reader);
				ReadKerning(reader);
			} else {
				ReadCMAPTableInternal(reader);
				if(header.Version == TTFHeader.InternalFormatVersion)
					ReadHMetricsInternalV1(reader);	
				else
					ReadHMetricsInternalV2(reader);	
				if(header.IsSupportKerningInternal)
					ReadKerningInternal(reader);
			}
		}
		#endregion
		#region WriteInternal
		public void WriteInternal(Stream stream) {
			BigEndianStreamWriter writer = new BigEndianStreamWriter(stream);
			header.WriteInternal(writer);
			WriteCMAPTableInternal(writer);
			WriteHMetricsInternal(writer);
			WriteKerningInternal(writer);
		}
		#endregion
		#region ReadCMAPTable
		void ReadCMAPTable(BigEndianStreamReader reader) {
			reader.Stream.Position = header.Tables["cmap"].Offset;
			cmap = new CMAPTable();
			cmap.Read(reader);
		}
		#endregion
		#region ReadCMAPTableInternal
		void ReadCMAPTableInternal(BigEndianStreamReader reader) {
			cmap = new CMAPTable();
			cmap.ReadInternal(reader);
		}
		#endregion
		#region WriteCMAPTableInternal
		void WriteCMAPTableInternal(BigEndianStreamWriter writer) {
			cmap.WriteInternal(writer);
		}
		#endregion
		#region ReadHMetrics
		void ReadHMetrics(BigEndianStreamReader reader) {
			reader.Stream.Position = header.Tables["hmtx"].Offset;
			int hmetricsCount = header.HMetricsCount,
				glyphsCount = header.GlyphsCount;
			widths = new ushort[glyphsCount];
			for(int i = 0; i < glyphsCount; i++) {
				if(i >= hmetricsCount)
					widths[i] = widths[0];
				else {
					widths[i] = reader.ReadUShort();
					reader.Stream.Seek(2, SeekOrigin.Current);	
				}
			}
		}
		#endregion
		#region ReadHMetricsInternal
		void ReadHMetricsInternalV2(BigEndianStreamReader reader) {
			int widthsCount = header.GlyphsCount;
			ArrayStreamHelper<ushort> helper = new ArrayStreamHelper<ushort>();
			widths = helper.ReadArray(reader, widthsCount, r => r.ReadUShort());
		}
		void ReadHMetricsInternalV1(BigEndianStreamReader reader) {
			int compressedFormat = reader.Stream.ReadByte();
			if (compressedFormat > 0)
				ReadHMetricsInternalCompressedFormat(reader);
			else
				ReadHMetricsInternalDefaultFormat(reader);
		}
		void ReadHMetricsInternalDefaultFormat(BigEndianStreamReader reader) {
			int widthsCount = header.GlyphsCount;
			widths = new ushort[widthsCount];
			for (int i = 0; i < widthsCount; i++)
				widths[i] = reader.ReadUShort();
		}
		void ReadHMetricsInternalCompressedFormat(BigEndianStreamReader reader) {
			int widthsCount = header.GlyphsCount;
			widths = new ushort[widthsCount];
			int totalWidthsRead = 0;
			while (totalWidthsRead < widthsCount) {
				int equalWidthsCount = reader.ReadUShort();
				ushort width = reader.ReadUShort();
				int count = totalWidthsRead + equalWidthsCount;
				for (int i = totalWidthsRead; i < count; i++)
					widths[i] = width;
				totalWidthsRead = count;
			}
		}
		#endregion
		#region WriteHMetricsInternal
		void WriteHMetricsInternal(BigEndianStreamWriter writer) {
			ArrayStreamHelper<ushort> helper = new ArrayStreamHelper<ushort>();
			helper.WriteArray(writer, widths, (w, value) => w.WriteUShort(value));		  
		}
		#endregion
		#region ReadKerning
		void ReadKerning(BigEndianStreamReader reader) {
			if(header.Tables.ContainsKey("kern")) {
				reader.Stream.Position = header.Tables["kern"].Offset;
				if(reader.ReadUShort() != 0)
					return; 
				int tablesCount = reader.ReadUShort();
				if(tablesCount > 0) {
					kernTable = new KernTable(reader);
					return;
				}
			}
			kernTable = new KernTable();
		}
		#endregion
		#region ReadKerningInternal
		void ReadKerningInternal(BigEndianStreamReader reader) {
			kernTable = new KernTable();
			kernTable.ReadInternal(reader);
		}
		#endregion
		#region WriteKerningInternal
		void WriteKerningInternal(BigEndianStreamWriter writer) {
			kernTable.WriteInternal(writer);
		}
		#endregion
		#region AlternativeMeasureChar
		private int AlternativeMeasureChar(char c, double fontSize) {
			return ErrorGlyphWidth;
		}
		#endregion
		#region MeasureText
		public SizeF MeasureText(string str, double fontSize) {
			if (string.IsNullOrEmpty(str))
				return SizeF.Empty;
			SizeF res = new SizeF(MeasureTextWidth(str, fontSize), header.Ascent + header.Descent + header.LineGap);
			res.Width = (float)(res.Width * fontSize / header.UnitsPerEm);
			res.Height = (float)(res.Height * fontSize / header.UnitsPerEm);
			return res;
		}
		float MeasureTextWidth(string str, double fontSize) {
			if (String.IsNullOrEmpty(str))
				return 0;
			float res = 0;
			int glyphIndex, nextGlyphIndex = GetGlyphIndex(str[0]);
			for (int i = 0; i < str.Length; i++) {
				glyphIndex = nextGlyphIndex;
				if (widths.Length <= glyphIndex || glyphIndex < 0)
					res += AlternativeMeasureChar(str[i], fontSize); 
				else
					res += widths[glyphIndex];
				if (i != str.Length - 1) {
					nextGlyphIndex = GetGlyphIndex(str[i + 1]);
					res += kernTable[glyphIndex, nextGlyphIndex]; 
				}
			}
			return res;
		}
		#endregion
		#region MeasureMultilineText
		public SizeF MeasureMultilineText(string str, double availableWidth, double fontSize) {
			if (string.IsNullOrEmpty(str))
				return new SizeF(0, (float)((header.Ascent + header.Descent + header.LineGap) * fontSize / header.UnitsPerEm));
			availableWidth *= header.UnitsPerEm / fontSize;
			bool hasNewLines = str.IndexOf('\r') >= 0 || str.IndexOf('\n') >= 0;
			if (hasNewLines)
				str = str.Replace("\r\n", "\n").Replace('\r', '\n');
			SizeF res = new SizeF();
			int lineCount = 1;
			string[] lines;
			if (hasNewLines)
				lines = str.Split('\n');
			else
				lines = new string[] { str };
			float lineWidth = 0;
			int count = lines.Length;
			bool decrementLines = false;
			for (int i = 0; i < count; i++) {
				int position = 0;
				lineWidth = 0;
				decrementLines = false;
				if (i > 0) {
					res.Width = Math.Max(res.Width, lineWidth);
					lineCount++;
				}
				for (; ; ) {
					string word = GetNextWord(lines[i], position);
					if (word == null)
						break;
					decrementLines = false;
					float wordWidth = MeasureTextWidth(word, fontSize);
					if (lineWidth + wordWidth > availableWidth) { 
						if (lineWidth == 0) { 
							wordWidth = MeasureTextWidth(word.Trim(), fontSize); 
							res.Width = Math.Max(res.Width, lineWidth + wordWidth);
							lineCount++;
							lineWidth = 0;
							decrementLines = true;
						}
						else {
							wordWidth = MeasureTextWidth(word.Trim(), fontSize); 
							if (lineWidth + wordWidth <= availableWidth) { 
								res.Width = Math.Max(res.Width, lineWidth + wordWidth);
								lineCount++;
								lineWidth = 0;
								decrementLines = true;	
							}
							else { 
								res.Width = Math.Max(res.Width, lineWidth);
								lineCount++;
								lineWidth = wordWidth;
							}
						}
					}
					else
						lineWidth += wordWidth;
					position += word.Length;
				}
			}
			if (decrementLines)
				lineCount = Math.Max(1, lineCount - 1);
			res.Width = Math.Max(res.Width, lineWidth);
			res.Height = lineCount * (header.Ascent + header.Descent + header.LineGap);
			res.Width = (float)(res.Width * fontSize / header.UnitsPerEm);
			res.Height = (float)(res.Height * fontSize / header.UnitsPerEm);
			return res;
		}
		string GetNextWord(string str, int position) {
			int length = str.Length;
			if (position >= str.Length)
				return null;
			int originalPosition = position;
			for (; position < length; position++) {
				if (!IsSpaceOrLineSeparator(str[position]))
					break;
			}
			if (position >= length)
				return str.Substring(originalPosition);
			for (; position < length; position++) {
				if (IsSpaceOrLineSeparator(str[position]))
					break;
			}
			if (position >= length)
				return str.Substring(originalPosition);
			for (; position < length; position++) {
				if (!IsSpaceOrLineSeparator(str[position]))
					break;
			}
			if (position >= length)
				return str.Substring(originalPosition);
			return str.Substring(originalPosition, position - originalPosition);
		}
		bool IsSpaceOrLineSeparator(char ch) {
			return Char.IsWhiteSpace(ch);
		}
		#endregion
		#region MeasureCharacterBounds
		public Rectangle[] MeasureCharacterBounds(string str, double fontSizeInUnits) {
			Rectangle[] res = new Rectangle[str.Length];
			if(string.IsNullOrEmpty(str)) return res;
			int height = FUnitsToFontSizeUnits(header.Ascent + header.Descent + header.LineGap, fontSizeInUnits),
				glyphIndex, nextGlyphIndex = GetGlyphIndex(str[0]), x = 0;
			for(int i = 0; i < str.Length; i++) {
				glyphIndex = nextGlyphIndex;
				int width = (widths.Length <= glyphIndex || glyphIndex < 0) ? ErrorGlyphWidth : widths[glyphIndex];
				if(i != str.Length - 1) {
					nextGlyphIndex = GetGlyphIndex(str[i + 1]);
					width += kernTable[glyphIndex, nextGlyphIndex]; 
				}
				width = FUnitsToFontSizeUnits(width, fontSizeInUnits);
				res[i] = new Rectangle(x, 0, width, height);
				x += width;
			}
			return res;
		}
		#endregion
		#region GetAscent
		public int GetAscent(double fontSizeInUnits) {
			return FUnitsToFontSizeUnits(header.Ascent, fontSizeInUnits);
		}
		#endregion
		#region GetDescent
		public int GetDescent(double fontSizeInUnits) {
			return FUnitsToFontSizeUnits(header.Descent, fontSizeInUnits);
		}
		#endregion
		#region GetLineGap
		public int GetLineGap(double fontSizeInUnits) {
			return FUnitsToFontSizeUnits(header.LineGap, fontSizeInUnits);
		}
		#endregion
		#region GetUnderlinePosition
		public int GetUnderlinePosition(double fontSizeInUnits) {
			return FUnitsToFontSizeUnits(header.UnderlinePosition, fontSizeInUnits);
		}
		#endregion
		#region GetUnderlineSize
		public int GetUnderlineSize(double fontSizeInUnits) {
			return FUnitsToFontSizeUnits(header.UnderlineSize, fontSizeInUnits);
		}
		#endregion
		#region GetStrikeOutPosition
		public int GetStrikeOutPosition(double fontSizeInUnits) {
			return FUnitsToFontSizeUnits(header.StrikeOutPosition, fontSizeInUnits);
		}
		#endregion
		#region GetStrikeOutSize
		public int GetStrikeOutSize(double fontSizeInUnits) {
			return FUnitsToFontSizeUnits(header.StrikeOutSize, fontSizeInUnits);
		}
		#endregion
		#region GetSubscriptXSize
		public int GetSubscriptXSize(double fontSizeInUnits) {
			return FUnitsToFontSizeUnits(header.SubscriptXSize, fontSizeInUnits);
		}
		#endregion
		#region GetSubscriptYSize
		public int GetSubscriptYSize(double fontSizeInUnits) {
			return FUnitsToFontSizeUnits(header.SubscriptYSize, fontSizeInUnits);
		}
		#endregion
		#region GetSubscriptXOffset
		public int GetSubscriptXOffset(double fontSizeInUnits) {
			return FUnitsToFontSizeUnits(header.SubscriptXOffset, fontSizeInUnits);
		}
		#endregion
		#region GetSubscriptYOffset
		public int GetSubscriptYOffset(double fontSizeInUnits) {
			return FUnitsToFontSizeUnits(header.SubscriptYOffset, fontSizeInUnits);
		}
		#endregion
		#region GetSuperscriptXSize
		public int GetSuperscriptXSize(double fontSizeInUnits) {
			return FUnitsToFontSizeUnits(header.SuperscriptXSize, fontSizeInUnits);
		}
		#endregion
		#region GetSuperscriptYSize
		public int GetSuperscriptYSize(double fontSizeInUnits) {
			return FUnitsToFontSizeUnits(header.SuperscriptYSize, fontSizeInUnits);
		}
		#endregion
		#region GetSuperscriptXOffset
		public int GetSuperscriptXOffset(double fontSizeInUnits) {
			return FUnitsToFontSizeUnits(header.SuperscriptXOffset, fontSizeInUnits);
		}
		#endregion
		#region GetSuperscriptYOffset
		public int GetSuperscriptYOffset(double fontSizeInUnits) {
			return FUnitsToFontSizeUnits(header.SuperscriptYOffset, fontSizeInUnits);
		}
		#endregion
		#region GetGlyphIndex
		public int GetGlyphIndex(char chr) {
			return cmap.GetGlyphIndex(chr);
		}
		#endregion
		#region FUnitsToPixels
		public int FUnitsToFontSizeUnits(int valueInFUnits, double fontSizeInUnits) {
			return (int)Math.Round(valueInFUnits * fontSizeInUnits / header.UnitsPerEm);
		}
		#endregion
		protected void SetAscent(int value) {
			header.Ascent = value;
		}
		protected void SetDescent(int value) {
			header.Descent = value;
		}
		protected void SetLineGap(int value) {
			header.LineGap = value;
		}
		public Size[] GetCharSegments() {
			return cmap.GetCharSegments();
		}
	}
	#endregion
	#region KernTable
	class KernTable {
		#region Fields
		uint[] keys;
		int[] values;
		internal bool isHorizontal, isMinimum, isCrossStream, isOverride;
		#endregion
		#region Properties
		public bool IsHorizontal { get { return isHorizontal; } }
		public bool IsMinimum { get { return isMinimum; } }
		public bool IsCrossStream { get { return isCrossStream; } }
		public bool IsOverride { get { return isOverride; } }
		public int Count { get { return keys != null ? keys.Length : 0; } }
		#endregion
		#region KernTable
		public KernTable() { }
		public KernTable(BigEndianStreamReader reader)
			: this() {
			Read(reader);
		}
		#endregion
		#region Read
		public void Read(BigEndianStreamReader reader) {
			if(reader.ReadUShort() != 0)
				throw new NotSupportedException("kern subtable version");
			reader.ReadUShort(); 
			ushort coverage = reader.ReadUShort();
			int format = ReadFlags(coverage);
			if(format != 0)
				throw new NotSupportedException("only format0 kern tables are supported");
			int pairsCount = reader.ReadUShort();
			reader.Stream.Seek(6, SeekOrigin.Current);
			keys = new uint[pairsCount];
			values = new int[pairsCount];
			for(int i = 0; i < pairsCount; i++) {
				keys[i] = reader.ReadUInt32();
				values[i] = reader.ReadShort();
			}
		}
		#endregion
		#region ReadFlags
		int ReadFlags(ushort coverage) {
			isHorizontal = (coverage & 1) == 1;
			isMinimum = (coverage & 2) == 2;
			isCrossStream = (coverage & 4) == 4;
			isOverride = (coverage & 8) == 8;
			return coverage & 0xF0;
		}
		#endregion
		#region WriteFlags
		void WriteFlagsInternal(BigEndianStreamWriter writer) {
			ushort coverage = 0;
			if(isHorizontal)
				coverage |= 1;
			if (isMinimum)
				coverage |= 2;
			if (isCrossStream)
				coverage |= 4;
			if (isOverride)
				coverage |= 8;
			writer.WriteUShort(coverage);
		}
		#endregion
		#region this
		public int this[int left, int right] {
			get {
				return this[GetIndex(left, right)];
			}
		}		
		public int this[uint key] {
			get {
				int index = IndexOfKey(key);
				return index >= 0 ? values[index] : 0;
			}
		}
		#endregion
		#region GetIndex
		uint GetIndex(int left, int right) {
			return (((uint)left & 0xFFFF) << 16) | ((uint)right & 0xFFFF);
		}
		#endregion
		#region IndexOfKey
		int IndexOfKey(uint value) {
			if(keys == null) return -1;
			int left = 0,
				right = keys.Length - 1;
			while(left <= right) {
				int index = left + ((right - left) >> 1);
				if(keys[index] == value)
					return index;
				if(keys[index] < value)
					left = index + 1;
				else
					right = index - 1;
			}
			return -1;
		}
		#endregion
		#region WriteInternal
		public void WriteInternal(BigEndianStreamWriter writer) {
			WriteFlagsInternal(writer);
			writer.WriteInt32(Count);
			if (Count == 0)
				return; 
			uint[] deltaKeys = new uint[Count];
			uint lastKey = 0;
			for (int i = 0; i < Count; i++) {
				deltaKeys[i] = keys[i] - lastKey - 1;
				lastKey = keys[i];
			}
			ArrayStreamHelper<uint> uintHelper = new ArrayStreamHelper<uint>();
			uintHelper.WriteArray(writer, deltaKeys, (w, value) => ArrayStreamHelper<uint>.WriteUIntCompressed(w, value));
			ArrayStreamHelper<int> ushortHelper = new ArrayStreamHelper<int>();
			ushortHelper.WriteArray(writer, values, (w, value) => w.WriteShort((short)value));
		}
		#endregion
		#region ReadInternal
		public void ReadInternal(BigEndianStreamReader reader) {
			ReadFlags(reader.ReadUShort());
			int pairsCount = reader.ReadInt32();
			if (pairsCount == 0) {
				keys = new uint[0];
				values = new int[0];
			}
			ArrayStreamHelper<uint> uintHelper = new ArrayStreamHelper<uint>();
			uint[] deltaKeys = uintHelper.ReadArray(reader, pairsCount, r => ArrayStreamHelper<uint>.ReadUIntCompressed(r));
			ArrayStreamHelper<int> ushortHelper = new ArrayStreamHelper<int>();
			values = ushortHelper.ReadArray(reader, pairsCount, r => r.ReadShort());
			keys = new uint[pairsCount];
			uint lastKey = 0;
			for (int i = 0; i < pairsCount; i++) {				
				lastKey += deltaKeys[i] + 1;
				keys[i] = lastKey;
			}
		}
		#endregion
	}
	#endregion
	#region BigEndianStreamReader
	public class BigEndianStreamReader {
		#region Fields
		Stream stream;
		#endregion
		public BigEndianStreamReader(Stream stream) {
			this.stream = stream;
		}
		#region Properties
		public Stream Stream { get { return stream; } }
		#endregion
		#region ReadFixed32
		public float ReadFixed32() {
			int whole = ReadShort(), dec = ReadUShort();
			if(whole < 0) dec *= -1;
			return Convert.ToSingle(whole) + Convert.ToSingle(dec) / 0xFFFF;
		}
		#endregion
		#region ReadShort
		public short ReadShort() {
			int byte1 = stream.ReadByte(), byte2 = stream.ReadByte();
			return (short)((byte1 << 8) | byte2);
		}
		#endregion
		#region ReadUShort
		[CLSCompliant(false)]
		public ushort ReadUShort() {
			return (ushort)ReadShort();
		}
		[CLSCompliant(false)]
		public ushort ReadUInt16() {
			return (ushort)ReadShort();
		}
		#endregion
		#region ReadInt32
		public int ReadInt32() {
			int byte1 = stream.ReadByte(), byte2 = stream.ReadByte(), byte3 = stream.ReadByte(), byte4 = stream.ReadByte();
			return (byte1 << 24) | (byte2 << 16) | (byte3 << 8) | (byte4);
		}
		#endregion
		#region ReadUInt32
		[CLSCompliant(false)]
		public uint ReadUInt32() {
			return (uint)ReadInt32();
		}
		#endregion
		#region ReadFixedString
		public string ReadFixedString(int length) {
			byte[] buf = new byte[length];
			stream.Read(buf, 0, length);
			return Encoding.UTF8.GetString(buf, 0, length);
		}
		#endregion
		internal byte ReadByte() {
			return (byte)stream.ReadByte();
		}
	}
	#endregion
	public class ArrayStreamHelper<T> where T : struct {
		public T[] ReadArray(BigEndianStreamReader reader, int count, Func<BigEndianStreamReader, T> readItemAction) {
			byte compressed = reader.ReadByte();
			if (compressed != 0)
				return ReadArrayCompressedFormat(reader, count, readItemAction);
			else
				return ReadArrayDefaultFormat(reader, count, readItemAction);
		}
		T[] ReadArrayDefaultFormat(BigEndianStreamReader reader, int count, Func<BigEndianStreamReader, T> readItemAction) {
			T[] result = new T[count];
			for (int i = 0; i < count; i++)
				result[i] = readItemAction(reader);
			return result;
		}
		T[] ReadArrayCompressedFormat(BigEndianStreamReader reader, int count, Func<BigEndianStreamReader, T> readItemAction) {
			T[] result = new T[count];
			int currentIndex = 0;
			while (currentIndex < count) {
				uint equalItemsCount = ReadUIntCompressed(reader);
				T item = readItemAction(reader);
				for (int i = 0; i < equalItemsCount; i++, currentIndex++)
					result[currentIndex] = item;
			}
			return result;
		}
		public void WriteArray(BigEndianStreamWriter writer, T[] items, Action<BigEndianStreamWriter, T> writeItemAction) {
			byte[] compressedBytes;
			using(MemoryStream compressedMemoryStream = new MemoryStream()) {
				BigEndianStreamWriter innerWriter = new BigEndianStreamWriter(compressedMemoryStream);
				WriteArrayCompressedFormat(innerWriter, items, writeItemAction);
				compressedBytes = compressedMemoryStream.ToArray();
			}
			byte[] defaultFormatBytes;
			using (MemoryStream defaultFormatMemoryStream = new MemoryStream()) {
				BigEndianStreamWriter innerWriter = new BigEndianStreamWriter(defaultFormatMemoryStream);
				WriteArrayDefaultFormat(innerWriter, items, writeItemAction);
				defaultFormatBytes = defaultFormatMemoryStream.ToArray();
			}
			if (compressedBytes.Length < defaultFormatBytes.Length) {
				writer.WriteByte(1);
				writer.Stream.Write(compressedBytes, 0, compressedBytes.Length);
			}
			else {
				writer.WriteByte(0);
				writer.Stream.Write(defaultFormatBytes, 0, defaultFormatBytes.Length);
			}
		}
		void WriteArrayDefaultFormat(BigEndianStreamWriter writer, T[] items, Action<BigEndianStreamWriter, T> writeItemAction) {			
			int count = items.Length;			
			for (int i = 0; i < count; i++) {
				writeItemAction(writer, items[i]);
			}
		}
		void WriteArrayCompressedFormat(BigEndianStreamWriter writer, T[] items, Action<BigEndianStreamWriter, T> writeItemAction) {			
			T prevItem = default(T);
			int count = items.Length;
			uint equalItemsCount = 0;
			for (int i = 0; i < count; i++) {
				if (!items[i].Equals(prevItem)) {
					if (equalItemsCount > 0) {
						WriteUIntCompressed(writer, equalItemsCount);
						writeItemAction(writer, prevItem);
					}
					prevItem = items[i];
					equalItemsCount = 1;
				}
				else
					equalItemsCount++;
			}
			if (equalItemsCount > 0) {
				WriteUIntCompressed(writer, equalItemsCount);
				writeItemAction(writer, prevItem);
			}
		}
		internal static uint ReadUIntCompressed(BigEndianStreamReader reader) {
			byte firstByte = reader.ReadByte();
			if (firstByte < 0xFE)
				return firstByte;
			if (firstByte == 0xFE)
				return reader.ReadUShort();
			else
				return reader.ReadUInt32();
		}
		internal static void WriteUIntCompressed(BigEndianStreamWriter writer, uint equalItemsCount) {
			if (equalItemsCount > 0xFFFF) {
				writer.WriteByte(0xFF);
				writer.WriteUInt32(equalItemsCount);
				return;
			}
			if (equalItemsCount >= 0xFE) {
				writer.WriteByte(0xFE);
				writer.WriteUShort((ushort)equalItemsCount);
				return;
			}
			writer.WriteByte((byte)equalItemsCount);
		}
	}
	#region BigEndianStreamWriter
	public class BigEndianStreamWriter {
		#region Fields
		Stream stream;
		byte[] buffer;
		#endregion
		public BigEndianStreamWriter(Stream stream) {
			this.stream = stream;
			this.buffer = new byte[4];
		}
		#region Properties
		public Stream Stream { get { return stream; } }
		#endregion
		#region WriteFixed32
		public void WriteFixed32(float value) {
			short intPart = (short)value;
			float fracPart = Math.Abs(value - intPart);
			WriteShort((short)intPart);
			WriteUShort((ushort)(fracPart * 0xFFFF));
		}
		#endregion
		#region WriteUShort
		[CLSCompliant(false)]
		public void WriteUShort(ushort value) {
			WriteShort((short)value);
		}
		#endregion
		#region WriteShort
		public void WriteShort(short value) {
			buffer[0] = (byte)(value >> 8);
			buffer[1] = (byte)value;
			Stream.Write(buffer, 0, 2);
		}
		#endregion
		#region WriteUInt32
		[CLSCompliant(false)]
		public void WriteUInt32(UInt32 value) {
			WriteInt32((int)value);
		}
		#endregion
		#region WriteInt32
		public void WriteInt32(int value) {
			buffer[0] = (byte)((value >> 24) & 0xFF);
			buffer[1] = (byte)((value >> 16) & 0xFF);
			buffer[2] = (byte)((value >> 8) & 0xFF);
			buffer[3] = (byte)(value & 0xFF);
			Stream.Write(buffer, 0, 4);
		}
		#endregion
		internal void WriteByte(byte value) {
			Stream.WriteByte(value);
		}
	}
	#endregion
	#region TTFIndexToLocFormat
	enum TTFIndexToLocFormat { Short = 0, Long = 1 }
	#endregion
	#region TTFWeightClass
	public enum TTFWeightClass {
		Thin = 100, ExtraLight = 200, Light = 300, Normal = 400,
		Medium = 500, SemiBold = 600, Bold = 700, ExtraBold = 800, Black = 900
	}
	#endregion
	#region TTFWidthClass
	public enum TTFWidthClass {
		UltraCondensed = 1, ExtraCondensed = 2, Condensed = 3, SemiCondensed = 4,
		Medium = 5, SemiExpanded = 6, Expanded = 7, ExtraExpanded = 8, UltraExpanded = 9
	}
	#endregion
	#region TTFHeader
	class TTFHeader {
		#region Constants
		public const float InternalFormatVersion = -1.0f;
		public const float InternalFormatVersion2 = -2.0f;
		#endregion
		#region Fields
		float version;
		Dictionary<string, TTFTableRecord> tables;
		int glyphsCount, unitsPerEm, hMetricsCount, lineGap, ascent, descent,
			strikeOutSize, strikeOutPosition, underlineSize, underlinePosition;
		TTFIndexToLocFormat indexToLocFormat;
		short glyphDataFormat, caretOffset;
		bool isNonLinearScaling;
		int subscriptXSize, subscriptXOffset, subscriptYSize, subscriptYOffset,
			superscriptXSize, superscriptXOffset, superscriptYSize, superscriptYOffset;
		TTFWeightClass weightClass;
		TTFWidthClass widthClass;
		byte[] panose = new byte[10];
		#endregion
		#region Properties
		public bool IsInternal { get { return Version == InternalFormatVersion || Version == InternalFormatVersion2; } }
		public bool IsSupportKerningInternal { get { return Version == InternalFormatVersion2; } }
		public float Version { get { return version; } }
		public int GlyphsCount { get { return glyphsCount; } }
		public int UnitsPerEm { get { return unitsPerEm; } }
		public short GlyphDataFormat { get { return glyphDataFormat; } }
		public TTFIndexToLocFormat IndexToLocTableFormat { get { return indexToLocFormat; } }
		public Dictionary<string, TTFTableRecord> Tables { get { return tables; } }
		public bool IsNonLinearScaling { get { return isNonLinearScaling; } }
		public int Ascent { get { return ascent; } internal set { ascent = value; } }
		public int Descent { get { return descent; } internal set { descent = value; } }
		public int LineGap { get { return lineGap; } internal set { lineGap = value; } }
		public short CaretOffset { get { return caretOffset; } }
		public int HMetricsCount { get { return hMetricsCount; } }
		public int SubscriptXSize { get { return subscriptXSize; } }
		public int SubscriptXOffset { get { return subscriptXOffset; } }
		public int SubscriptYSize { get { return subscriptYSize; } }
		public int SubscriptYOffset { get { return subscriptYOffset; } }
		public int SuperscriptXSize { get { return superscriptXSize; } }
		public int SuperscriptXOffset { get { return superscriptXOffset; } }
		public int SuperscriptYSize { get { return superscriptYSize; } }
		public int SuperscriptYOffset { get { return superscriptYOffset; } }
		public TTFWeightClass WeightClass { get { return weightClass; } }
		public TTFWidthClass WidthClass { get { return widthClass; } }
		public int StrikeOutSize { get { return strikeOutSize; } }
		public int StrikeOutPosition { get { return strikeOutPosition; } }
		public int UnderlineSize { get { return underlineSize; } }
		public int UnderlinePosition { get { return underlinePosition; } }
		public byte[] Panose { get { return panose; } }
		#endregion
		#region Methods
		#region Read
		public void Read(BigEndianStreamReader reader) {
			version = reader.ReadFixed32();
			if(version == 1.0f) {
				ReadV10(reader);
				return;
			}
			if (version == InternalFormatVersion || version == InternalFormatVersion2) {
				ReadInternal(reader);
				return;
			}
			throw new NotSupportedException(String.Format("version={0}, expected versions: {1}, {2}, {3}", version, 1.0, InternalFormatVersion, InternalFormatVersion2));
		}
		#endregion
		#region ReadV10
		void ReadV10(BigEndianStreamReader reader) {
			int numTables = reader.ReadUShort();
			reader.Stream.Seek(6, SeekOrigin.Current);
			ReadTableRecords(reader, numTables);
			ReadMAXPTable(reader);
			ReadHEADTable(reader);
			ReadHHEATable(reader);
			ReadOS2Table(reader);
			ReadPOSTTable(reader);
		}
		#endregion
		#region ReadInternal
		void ReadInternal(BigEndianStreamReader reader) {
			glyphsCount = reader.ReadUShort();
			unitsPerEm = reader.ReadUShort();
			ascent = reader.ReadShort();
			descent = reader.ReadShort();
			lineGap = reader.ReadShort();
			caretOffset = reader.ReadShort();
			subscriptXSize = reader.ReadShort();
			subscriptXOffset = reader.ReadShort();
			subscriptYSize = reader.ReadShort();
			subscriptYOffset = reader.ReadShort();
			superscriptXSize = reader.ReadShort();
			superscriptXOffset = reader.ReadShort();
			superscriptYSize = reader.ReadShort();
			superscriptYOffset = reader.ReadShort();
			strikeOutPosition = reader.ReadShort();
			strikeOutSize = reader.ReadShort();
			underlinePosition = reader.ReadShort();
			underlineSize = reader.ReadShort();
			reader.Stream.Read(panose, 0, 10);
		}
		#endregion
		#region WriteInternal
		public void WriteInternal(BigEndianStreamWriter writer) {
			writer.WriteFixed32(InternalFormatVersion2);
			writer.WriteUShort((ushort)GlyphsCount);
			writer.WriteUShort((ushort)UnitsPerEm);
			writer.WriteShort((short)Ascent);
			writer.WriteShort((short)Descent);
			writer.WriteShort((short)LineGap);
			writer.WriteShort((short)CaretOffset);
			writer.WriteShort((short)SubscriptXSize);
			writer.WriteShort((short)SubscriptXOffset);
			writer.WriteShort((short)SubscriptYSize);
			writer.WriteShort((short)SubscriptYOffset);
			writer.WriteShort((short)SuperscriptXSize);
			writer.WriteShort((short)SuperscriptXOffset);
			writer.WriteShort((short)SuperscriptYSize);
			writer.WriteShort((short)SuperscriptYOffset);
			writer.WriteShort((short)StrikeOutPosition);
			writer.WriteShort((short)StrikeOutSize);
			writer.WriteShort((short)UnderlinePosition);
			writer.WriteShort((short)UnderlineSize);
			writer.Stream.Write(Panose, 0, Panose.Length);
		}
		#endregion
		#region ReadTableRecords
		void ReadTableRecords(BigEndianStreamReader reader, int numTables) {
			tables = new Dictionary<string, TTFTableRecord>(numTables);
			for(int i = 0; i < numTables; i++) {
				TTFTableRecord table = new TTFTableRecord();
				table.Read(reader);
				Tables.Add(table.Tag, table);
			}
		}
		#endregion
		#region ReadMAXPTable
		void ReadMAXPTable(BigEndianStreamReader reader) {
			SeekToTable(reader, "maxp");
			float tableVersion = reader.ReadFixed32();
			if(tableVersion != 1.0)
				throw new NotSupportedException("maxp table version");
			glyphsCount = reader.ReadUShort();
		}
		#endregion
		#region ReadHEADTable
		void ReadHEADTable(BigEndianStreamReader reader) {
			SeekToTable(reader, "head");
			float tableVersion = reader.ReadFixed32();
			if(tableVersion != 1.0)
				throw new NotSupportedException("head table version");
			reader.Stream.Seek(12, SeekOrigin.Current);
			ushort flags = reader.ReadUShort();
			ParseFlags(flags);
			unitsPerEm = reader.ReadUShort();
			reader.Stream.Seek(30, SeekOrigin.Current);
			indexToLocFormat = (TTFIndexToLocFormat)reader.ReadShort();
			glyphDataFormat = reader.ReadShort();
		}
		#endregion
		#region ReadHHEATable
		void ReadHHEATable(BigEndianStreamReader reader) {
			SeekToTable(reader, "hhea");
			float tableVersion = reader.ReadFixed32();
			if(tableVersion != 1.0)
				throw new NotSupportedException("hhea table version");
			ascent = reader.ReadShort();
			descent = -reader.ReadShort();
			lineGap = reader.ReadShort();
			reader.Stream.Seek(12, SeekOrigin.Current);
			caretOffset = reader.ReadShort();
			if(reader.ReadUInt32() != 0)
				throw new ArgumentException("corrupted hhea table: no zero padding");
			if(reader.ReadUInt32() != 0)
				throw new ArgumentException("corrupted hhea table: no zero padding");
			if(reader.ReadShort() != 0)
				throw new NotSupportedException("metric data format");
			hMetricsCount = reader.ReadUShort();
		}
		#endregion
		#region ReadOS2Table
		void ReadOS2Table(BigEndianStreamReader reader) {
			SeekToTable(reader, "OS/2");
			reader.ReadUShort(); 
			reader.Stream.Seek(2, SeekOrigin.Current);
			weightClass = (TTFWeightClass)reader.ReadUShort();
			widthClass = (TTFWidthClass)reader.ReadUShort();
			reader.Stream.Seek(2, SeekOrigin.Current);
			subscriptXSize = reader.ReadShort();
			subscriptYSize = reader.ReadShort();
			subscriptXOffset = reader.ReadShort();
			subscriptYOffset = reader.ReadShort();
			superscriptXSize = reader.ReadShort();
			superscriptYSize = reader.ReadShort();
			superscriptXOffset = reader.ReadShort();
			superscriptYOffset = -reader.ReadShort();
			strikeOutSize = reader.ReadShort();
			strikeOutPosition = reader.ReadShort();
			reader.ReadShort(); 
			reader.Stream.Read(panose, 0, 10);
		}
		#endregion
		#region ReadPOSTTAble
		void ReadPOSTTable(BigEndianStreamReader reader) {
			if(!SeekToTable(reader, "post")) return;
			reader.Stream.Seek(8, SeekOrigin.Current);
			underlinePosition = -reader.ReadShort();
			underlineSize = reader.ReadShort();
		}
		#endregion
		#region SeekToTable
		public bool SeekToTable(BigEndianStreamReader reader, string name) {
			if(!Tables.ContainsKey(name)) return false;
			reader.Stream.Seek(Tables[name].Offset, SeekOrigin.Begin);
			return true;
		}
		#endregion
		#region ParseFlags
		void ParseFlags(ushort flags) {
			isNonLinearScaling = (flags & 16) == 16;
		}
		#endregion
		#endregion
	}
	#endregion
	#region TTFTableRecord
	class TTFTableRecord {
		#region Fields
		string tag;
		uint offset, length;
		#endregion
		#region Properties
		public string Tag { get { return tag; } }
		public uint Offset { get { return offset; } }
		public uint Length { get { return length; } }
		#endregion
		#region Read
		public void Read(BigEndianStreamReader reader) {
			tag = reader.ReadFixedString(4);
			reader.Stream.Seek(4, SeekOrigin.Current);
			offset = reader.ReadUInt32();
			length = reader.ReadUInt32();
		}
		#endregion
		#region ToString
		public override string ToString() {
			return Tag + ":" + Length.ToString() + " bytes at " + Offset.ToString();
		}
		#endregion
	}
	#endregion
	#region CMAPTable
	class CMAPTable {
		#region Fields
		CharMapper4 mapper;
		#endregion
		#region GetGlyphIndex
		public int GetGlyphIndex(char chr) {
			if(mapper == null)
				throw new InvalidOperationException("cmap table isn't loaded");
			return mapper.GetGlyphIndex(chr);
		}
		#endregion
		#region Read
		public void Read(BigEndianStreamReader reader) {
			ushort version = reader.ReadUShort();
			if(version == 0) {
				ReadV0(reader);
				return;
			}
			throw new NotSupportedException("invalid cmap table version");
		}
		#endregion
		#region ReadV0
		void ReadV0(BigEndianStreamReader reader) {
			long startPosition = reader.Stream.Position - 2;
			ushort numRecords = reader.ReadUShort();
			List<EncodingRecord> encodingRecords = ReadEncodingRecords(reader, numRecords);
			EncodingRecord unicodeRecord = FindEncodingRecord(encodingRecords, Platform.Windows, 1);
			if(unicodeRecord == null)
				throw new NotSupportedException("only Windows Unicode cmap tables are supported");
			reader.Stream.Seek(startPosition + unicodeRecord.Offset, SeekOrigin.Begin);
			reader.ReadUShort(); 
			mapper = new CharMapper4();
			mapper.Read(reader);
		}
		#endregion
		#region ReadInternal
		public void ReadInternal(BigEndianStreamReader reader) {
			mapper = new CharMapper4();
			mapper.ReadInternal(reader);
		}
		#endregion
		#region WriteInternal
		public void WriteInternal(BigEndianStreamWriter writer) {
			mapper.WriteInternal(writer);
		}
		#endregion
		#region ReadEncodingRecords
		List<EncodingRecord> ReadEncodingRecords(BigEndianStreamReader reader, ushort numRecords) {
			List<EncodingRecord> res = new List<EncodingRecord>(numRecords);
			for(ushort i = 0; i < numRecords; i++) {
				res.Add(new EncodingRecord(reader));
			}
			return res;
		}
		#endregion
		#region FindEncodingRecord
		EncodingRecord FindEncodingRecord(IEnumerable<EncodingRecord> encodingRecords, Platform platform, ushort encodingID) {
			foreach(EncodingRecord rec in encodingRecords) {
				if(rec.Platform == platform) return rec;
			}
			return null;
		}
		#endregion
		internal Size[] GetCharSegments() {
			CharMapper4Segment[] segments = mapper.Segments;
			int count = segments.Length;
			Size[] result = new Size[count];
			for (int i = 0; i < count; i++)
				result[i] = new Size(segments[i].StartCode, segments[i].EndCode);
			return result;
		}
	}
	#endregion
	#region EncodingRecord
	enum Platform { Unicode = 0, Mac = 1, Windows = 3 }
	class EncodingRecord {
		public Platform Platform;
		public ushort EncodingID;
		public uint Offset;
		public EncodingRecord() { }
		public EncodingRecord(BigEndianStreamReader reader)
			: this() {
			Read(reader);
		}
		public void Read(BigEndianStreamReader reader) {
			Platform = (Platform)reader.ReadUShort();
			EncodingID = reader.ReadUShort();
			Offset = reader.ReadUInt32();
		}
	}
	#endregion
	#region CharMapper4
	class CharMapper4 {
		CharMapper4Segment[] segments;
		public CharMapper4Segment[] Segments { get { return segments; } }
		#region GetGlyphIndex
		public int GetGlyphIndex(char chr) {
			if(segments == null)
				throw new InvalidOperationException("you should load char mapper first");
			int index = IndexOfSegment(chr);
			return index >= 0 ? segments[index].GetGlyphIndex(chr) : 0;
		}
		#endregion
		#region Read
		public void Read(BigEndianStreamReader reader) {
			reader.Stream.Seek(4, SeekOrigin.Current);	
			int segmentsCount = reader.ReadUShort();
			if(segmentsCount % 2 != 0)
				throw new ArgumentException("Invalid data");
			segmentsCount /= 2;
			reader.Stream.Seek(6, SeekOrigin.Current);
			segments = new CharMapper4Segment[segmentsCount];
			for(int i = 0; i < segmentsCount; i++) {
				segments[i] = new CharMapper4Segment();
				segments[i].EndCode = reader.ReadUShort();
				if(i == segmentsCount - 1 && segments[i].EndCode != 0xFFFF)
					throw new ArgumentException("corrupted data: last segment's endCount have to be 0xFFFF");
			}
			ushort pad = reader.ReadUShort();
			if(pad != 0)
				throw new ArgumentException("corrupted data: zero pad is missing");
			for(int i = 0; i < segmentsCount; i++) {
				segments[i].StartCode = reader.ReadUShort();
				if(segments[i].StartCode > segments[i].EndCode)
					throw new ArgumentException("corrupted data: invalid startCount");
			}
			for(int i = 0; i < segmentsCount; i++)
				segments[i].IdDelta = reader.ReadShort();
			for(int i = 0; i < segmentsCount; i++) {
				ushort idRangeOffset = reader.ReadUShort();
				if(idRangeOffset != 0) {
					long oldPos = reader.Stream.Position;
					reader.Stream.Seek(idRangeOffset - 2, SeekOrigin.Current);
					CharMapper4Segment seg = segments[i];
					seg.IndexArray = new ushort[seg.EndCode - seg.StartCode];
					for(int j = 0; j < seg.IndexArray.Length; j++) {
						seg.IndexArray[j] = reader.ReadUShort();
					}
					reader.Stream.Seek(oldPos, SeekOrigin.Begin);
				}
			}
		}
		#endregion
		#region ReadInternal
		public void ReadInternal(BigEndianStreamReader reader) {
			int segmentsCount = reader.ReadUShort();
			segments = new CharMapper4Segment[segmentsCount];
			for(int i = 0; i < segmentsCount; i++) {
				segments[i] = new CharMapper4Segment();
				segments[i].ReadInternal(reader);
			}
		}
		#endregion
		#region WriteInternal
		public void WriteInternal(BigEndianStreamWriter writer) {
			int segmentsCount = segments.Length;
			writer.WriteUShort((ushort)segmentsCount);
			for(int i = 0; i < segmentsCount; i++) {
				segments[i].WriteInternal(writer);
			}
		}
		#endregion
		#region IndexOfSegment
		protected int IndexOfSegment(char chr) {
			if(segments == null) return -1;
			ushort chrIndex = (ushort)chr;
			int left = 0,
				right = segments.Length - 1;
			while(left <= right) {
				int index = left + ((right - left) >> 1);
				CharMapper4Segment segment = segments[index];
				if(segment.StartCode <= chrIndex && chrIndex <= segment.EndCode)
					return index;
				if(chrIndex > segment.EndCode)
					left = index + 1;
				else
					right = index - 1;
			}
			return -1;
		}
		#endregion
	}
	#endregion
	#region CharMapper4Segment
	class CharMapper4Segment {
		#region Fields
		public ushort StartCode, EndCode;
		public short IdDelta;
		public ushort[] IndexArray;
		#endregion
		#region IsCharFallInRange
		public bool IsCharFallInRange(ushort chrIndex) {
			return chrIndex >= StartCode && chrIndex <= EndCode;
		}
		#endregion
		#region GetGlyphIndex
		public int GetGlyphIndex(char chr) {
			int chrIndex = (int)chr;
			if(IndexArray != null) {
				int index = chrIndex - StartCode;
				return (index >= 0 && index < IndexArray.Length) ? IndexArray[chrIndex - StartCode] : -1;
			}
			ushort result = (ushort)chrIndex;
			result += (ushort)IdDelta;
			return result;
		}
		#endregion
		#region WriteInternal
		public void WriteInternal(BigEndianStreamWriter writer) {
			writer.WriteUShort(StartCode);
			writer.WriteUShort(EndCode);
			writer.WriteShort(IdDelta);
			if(IdDelta == 0) {
				writer.WriteUShort((ushort)IndexArray.Length);
				for(int i = 0; i < IndexArray.Length; i++) {
					writer.WriteUShort(IndexArray[i]);
				}
			}
		}
		#endregion
		#region ReadInternal
		public void ReadInternal(BigEndianStreamReader reader) {
			StartCode = reader.ReadUShort();
			EndCode = reader.ReadUShort();
			IdDelta = reader.ReadShort();
			if(IdDelta == 0) {
				int indexArraySize = reader.ReadUShort();
				IndexArray = new ushort[indexArraySize];
				for(int i = 0; i < indexArraySize; i++) {
					IndexArray[i] = reader.ReadUShort();
				}
			} else
				IndexArray = null;
		}
		#endregion
		#region ToString
		public override string ToString() {
			return "[" + StartCode.ToString() + ", " + EndCode.ToString() + "] -> [" +
				(StartCode + IdDelta).ToString() + ", " + (EndCode + IdDelta).ToString() + "]";
		}
		#endregion
	}
	#endregion
}
namespace DevExpress.Office.Services {
	public interface IFontCharacterSetService {
		void BeginProcessing(string fontName);
		bool ContainsChar(char ch);
		void EndProcessing();
	}
}
