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
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.Export.Xl;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraExport.Xls {
	#region XLStringEncoder
	public static class XLStringEncoder {
		const int singleByteCodePage = 1252;
		public static Encoding GetSingleByteEncoding() {
			return DXEncoding.GetEncoding(singleByteCodePage);
		}
		public static Encoding GetEncoding(bool highBytes) {
			return (highBytes) ? Encoding.Unicode : DXEncoding.GetEncoding(singleByteCodePage);
		}
		public static byte[] GetBytes(string value, bool highBytes) {
			return GetEncoding(highBytes).GetBytes(value);
		}
		public static bool StringHasHighBytes(string value) {
			if(string.IsNullOrEmpty(value)) return false;
			try {
				for(int i = 0; i < value.Length; i++) {
					uint charCode = (uint)(value[i]);
					if(charCode > 0x00ff)
						return true;
				}
				return false;
			}
			catch { }
			return true;
		}
	}
	#endregion
	#region XLUnicodeStringBase (abstract)
	public abstract class XLUnicodeStringBase {
		#region Fields
		const int baseHeaderSize = 3;
		bool hasHighBytes;
		bool forceHighBytes;
		string value;
		#endregion
		protected XLUnicodeStringBase() {
			this.value = string.Empty;
		}
		#region Properties
		public bool HasHighBytes {
			get { return hasHighBytes; }
		}
		public bool ForceHighBytes {
			get { return this.forceHighBytes; }
			set {
				this.forceHighBytes = value;
				this.hasHighBytes = StringHasHighBytes(this.value);
			}
		}
		public string Value {
			get { return this.value; }
			set {
				if(value == null) value = string.Empty;
				ValidateStringValue(value);
				this.value = value;
				this.hasHighBytes = StringHasHighBytes(this.value);
			}
		}
		public abstract int Length { get; }
		#endregion
		protected virtual void ValidateStringValue(string text) {
			if(text.Length > short.MaxValue)
				throw new ArgumentException("String value too long");
		}
		protected void Read(XlsReader reader) {
			int charCount = ReadCharCount(reader);
			byte flags = ReadFlags(reader);
			ReadExtraHeader(reader, flags);
			if(charCount > 0) {
				int length = (HasHighBytes) ? charCount * 2 : charCount;
				byte[] buffer = reader.ReadBytes(length);
				this.value = XLStringEncoder.GetEncoding(HasHighBytes).GetString(buffer, 0, length);
			}
			else
				this.value = string.Empty;
			ReadExtraData(reader, flags);
		}
		protected void Read(XlsReader reader, int size) {
			long initialPosition = reader.Position;
			int charCount = ReadCharCount(reader);
			byte flags = ReadFlags(reader);
			ReadExtraHeader(reader, flags);
			if(charCount > 0) {
				int maxBytesToRead = size - (int)(reader.Position - initialPosition);
				if(maxBytesToRead < 0)
					throw new Exception("Wrong bytes to read for XLUnicodeString");
				int length = (HasHighBytes) ? charCount * 2 : charCount;
				if(length > maxBytesToRead) {
					if(HasHighBytes)
						length = (maxBytesToRead / 2) * 2;
					else
						length = maxBytesToRead;
				}
				byte[] buffer = reader.ReadBytes(length);
				this.value = XLStringEncoder.GetEncoding(HasHighBytes).GetString(buffer, 0, length);
			}
			else
				this.value = string.Empty;
			ReadExtraData(reader, flags);
		}
		protected void Read(BinaryReader reader) {
			int charCount = ReadCharCount(reader);
			byte flags = ReadFlags(reader);
			ReadExtraHeader(reader, flags);
			if(charCount > 0) {
				int length = (HasHighBytes) ? charCount * 2 : charCount;
				byte[] buffer = reader.ReadBytes(length);
				this.value = XLStringEncoder.GetEncoding(HasHighBytes).GetString(buffer, 0, length);
			}
			else
				this.value = string.Empty;
			ReadExtraData(reader, flags);
		}
		public void Write(BinaryWriter writer) {
			XlsChunkWriter chunkWriter = writer as XlsChunkWriter;
			if(chunkWriter != null) chunkWriter.BeginRecord(GetHeaderSize());
			WriteCharCount(writer);
			writer.Write(GetFlags());
			WriteExtraHeader(writer);
			if(chunkWriter != null) chunkWriter.BeginStringValue(HasHighBytes);
			writer.Write(XLStringEncoder.GetBytes(this.value, HasHighBytes));
			if(chunkWriter != null) chunkWriter.EndStringValue();
			WriteExtraData(writer);
		}
		protected virtual int ReadCharCount(XlsReader reader) {
			return reader.ReadInt16();
		}
		protected virtual int ReadCharCount(BinaryReader reader) {
			return reader.ReadInt16();
		}
		protected virtual void WriteCharCount(BinaryWriter writer) {
			writer.Write((short)Value.Length);
		}
		protected virtual byte GetFlags() {
			byte flags = 0;
			if(HasHighBytes)
				flags |= 0x01;
			return flags;
		}
		protected internal byte ReadFlags(XlsReader reader) {
			byte flags = reader.ReadByte();
			this.hasHighBytes = (flags & 0x01) != 0;
			return flags;
		}
		protected internal byte ReadFlags(BinaryReader reader) {
			byte flags = reader.ReadByte();
			this.hasHighBytes = (flags & 0x01) != 0;
			return flags;
		}
		protected virtual void ReadExtraHeader(XlsReader reader, byte flags) {
		}
		protected virtual void ReadExtraHeader(BinaryReader reader, byte flags) {
		}
		protected virtual void WriteExtraHeader(BinaryWriter writer) {
		}
		protected virtual void ReadExtraData(XlsReader reader, byte flags) {
		}
		protected virtual void ReadExtraData(BinaryReader reader, byte flags) {
		}
		protected virtual void WriteExtraData(BinaryWriter writer) {
		}
		protected abstract int GetHeaderSize();
		bool StringHasHighBytes(string text) {
			if(this.forceHighBytes) return true;
			return XLStringEncoder.StringHasHighBytes(text);
		}
	}
	#endregion
	#region ShortXLUnicodeString
	public class ShortXLUnicodeString : XLUnicodeStringBase {
		const int headerSize = 2;
		public ShortXLUnicodeString()
			: base() {
		}
		public override int Length { get { return (HasHighBytes) ? Value.Length * 2 + headerSize : Value.Length + headerSize; } }
		public static ShortXLUnicodeString FromStream(XlsReader reader) {
			ShortXLUnicodeString result = new ShortXLUnicodeString();
			result.Read(reader);
			return result;
		}
		public static ShortXLUnicodeString FromStream(BinaryReader reader) {
			ShortXLUnicodeString result = new ShortXLUnicodeString();
			result.Read(reader);
			return result;
		}
		protected override void ValidateStringValue(string text) {
			if(text.Length > byte.MaxValue)
				throw new ArgumentException("String value too long");
		}
		protected override int ReadCharCount(XlsReader reader) {
			return reader.ReadByte();
		}
		protected override int ReadCharCount(BinaryReader reader) {
			return reader.ReadByte();
		}
		protected override void WriteCharCount(BinaryWriter writer) {
			writer.Write((byte)Value.Length);
		}
		protected override int GetHeaderSize() {
			int result = headerSize;
			if(Value.Length > 0) {
				int charLen = HasHighBytes ? 2 : 1;
				result += charLen;
			}
			return result;
		}
	}
	#endregion
	#region ISupportPartialReading
	public interface ISupportPartialReading {
		void ReadData(XlsReader reader);
		bool Complete { get; }
	}
	#endregion
	#region XLUnicodeString
	public class XLUnicodeString : XLUnicodeStringBase, ISupportPartialReading {
		const int headerSize = 3;
		int charCount;
		public XLUnicodeString()
			: base() {
		}
		public override int Length { get { return (HasHighBytes) ? Value.Length * 2 + headerSize : Value.Length + headerSize; } }
		public static XLUnicodeString FromStream(XlsReader reader) {
			XLUnicodeString result = new XLUnicodeString();
			result.Read(reader);
			return result;
		}
		public static XLUnicodeString FromStream(XlsReader reader, int size) {
			XLUnicodeString result = new XLUnicodeString();
			result.Read(reader, size);
			return result;
		}
		public static XLUnicodeString FromStream(BinaryReader reader) {
			XLUnicodeString result = new XLUnicodeString();
			result.Read(reader);
			return result;
		}
		protected override int GetHeaderSize() {
			int result = headerSize;
			if(Value.Length > 0) {
				int charLen = HasHighBytes ? 2 : 1;
				result += charLen;
			}
			return result;
		}
		#region ISupportPartialReading Members
		public void ReadData(XlsReader reader) {
			if(Complete) {
				Value = null;
				this.charCount = 0;
			}
			if(charCount == 0 && Value.Length == 0) {
				this.charCount = ReadCharCount(reader);
				byte flags = ReadFlags(reader);
				ReadExtraHeader(reader, flags);
			}
			if(reader.Position == 0)
				ReadFlags(reader);
			long bytesAvailable = reader.StreamLength - reader.Position;
			int bytesToRead = charCount - Value.Length;
			if(HasHighBytes)
				bytesToRead *= 2;
			if(bytesToRead > bytesAvailable)
				bytesToRead = (int)bytesAvailable;
			byte[] buffer = reader.ReadBytes(bytesToRead);
			string part = XLStringEncoder.GetEncoding(HasHighBytes).GetString(buffer, 0, bytesToRead);
			Value = Value + part;
		}
		public bool Complete {
			get { return Value.Length == charCount; }
		}
		#endregion
	}
	#endregion
	#region XlsFormatRun
	public class XlsFormatRun {
		public const int RecordSize = 4;
		public XlsFormatRun() {
		}
		#region Properties
		public int CharIndex { get; set; }
		public int FontIndex { get; set; }
		#endregion
		public static XlsFormatRun FromStream(XlsReader reader) {
			XlsFormatRun result = new XlsFormatRun();
			result.Read(reader);
			return result;
		}
		public static XlsFormatRun FromStream(BinaryReader reader) {
			XlsFormatRun result = new XlsFormatRun();
			result.Read(reader);
			return result;
		}
		protected void Read(XlsReader reader) {
			CharIndex = reader.ReadUInt16();
			FontIndex = reader.ReadUInt16();
		}
		protected void Read(BinaryReader reader) {
			CharIndex = reader.ReadUInt16();
			FontIndex = reader.ReadUInt16();
		}
		public void Write(BinaryWriter writer) {
			XlsChunkWriter chunkWriter = writer as XlsChunkWriter;
			if(chunkWriter != null) chunkWriter.BeginBlock();
			writer.Write((ushort)CharIndex);
			writer.Write((ushort)FontIndex);
			if(chunkWriter != null) chunkWriter.EndBlock();
		}
		public bool IsDefault() {
			return CharIndex == 0 && FontIndex == 0;
		}
		public override bool Equals(object obj) {
			XlsFormatRun other = obj as XlsFormatRun;
			if(other == null) return false;
			return CharIndex == other.CharIndex &&
				FontIndex == other.FontIndex; ;
		}
		public override int GetHashCode() {
			return (int)(CharIndex & 0x0000ffff + ((FontIndex & 0x0000ffff) << 16));
		}
	}
	#endregion
	#region XLUnicodeRichExtendedString
	public class XLUnicodeRichExtendedString : XLUnicodeStringBase, ISupportPartialReading {
		#region Fields
		const int headerSize = 3;
		List<XlsFormatRun> formatRuns = new List<XlsFormatRun>();
		byte[] phoneticData = new byte[0];
		int charCount;
		int formatRunCount;
		int phoneticDataLen;
		#endregion
		public XLUnicodeRichExtendedString()
			: base() {
		}
		#region Properties
		public bool HasPhoneticData { get { return phoneticData.Length > 0; } }
		public bool IsRichString { get { return formatRuns.Count > 0; } }
		public IList<XlsFormatRun> FormatRuns { get { return formatRuns; } }
		public byte[] PhoneticData {
			get { return phoneticData; }
			set {
				if(value == null)
					phoneticData = new byte[0];
				else
					phoneticData = value;
			}
		}
		public override int Length {
			get {
				int result = headerSize + (HasHighBytes ? Value.Length * 2 : Value.Length);
				if(IsRichString)
					result += this.formatRuns.Count * 4 + 2;
				if(HasPhoneticData)
					result += this.phoneticData.Length + 4;
				return result;
			}
		}
		#endregion
		public static XLUnicodeRichExtendedString FromStream(XlsReader reader) {
			XLUnicodeRichExtendedString result = new XLUnicodeRichExtendedString();
			result.Read(reader);
			return result;
		}
		protected override byte GetFlags() {
			byte flags = base.GetFlags();
			if(HasPhoneticData)
				flags |= 0x04;
			if(IsRichString)
				flags |= 0x08;
			return flags;
		}
		protected override void ReadExtraHeader(XlsReader reader, byte flags) {
			bool hasPhonetic = Convert.ToBoolean(flags & 0x04);
			bool isRich = Convert.ToBoolean(flags & 0x08);
			if(isRich)
				formatRunCount = reader.ReadUInt16();
			else
				formatRunCount = 0;
			if(hasPhonetic)
				phoneticDataLen = reader.ReadInt32();
			else
				phoneticDataLen = 0;
		}
		protected override void WriteExtraHeader(BinaryWriter writer) {
			if(IsRichString)
				writer.Write((ushort)FormatRuns.Count);
			if(HasPhoneticData)
				writer.Write((int)PhoneticData.Length);
		}
		protected override void ReadExtraData(XlsReader reader, byte flags) {
			this.formatRuns.Clear();
			if(formatRunCount > 0) {
				for(int i = 0; i < formatRunCount; i++) {
					this.formatRuns.Add(XlsFormatRun.FromStream(reader));
				}
			}
			if(phoneticDataLen > 0)
				PhoneticData = reader.ReadBytes(phoneticDataLen);
			else
				PhoneticData = null;
		}
		protected override void WriteExtraData(BinaryWriter writer) {
			if(IsRichString) {
				for(int i = 0; i < FormatRuns.Count; i++) {
					XlsFormatRun item = FormatRuns[i];
					item.Write(writer);
				}
			}
			if(HasPhoneticData)
				writer.Write(PhoneticData);
		}
		protected override int GetHeaderSize() {
			int result = headerSize;
			if(Value.Length > 0) {
				int charLen = HasHighBytes ? 2 : 1;
				result += charLen;
			}
			if(IsRichString)
				result += 2;
			if(HasPhoneticData)
				result += 4;
			return result;
		}
		#region ISupportPartialReading Members
		public void ReadData(XlsReader reader) {
			if(Complete)
				Cleanup();
			if(IsEmpty()) {
#if DISCOVER
				long position = reader.Position;
#endif
				charCount = ReadCharCount(reader);
				byte flags = ReadFlags(reader);
#if DISCOVER
				Debug.Print(string.Format("CharCount={0}, Flags={1}, Position={2}", charCount, flags, position));
#endif
				ReadExtraHeader(reader, flags);
			}
			if(!ReadValuePart(reader)) return;
			if(!ReadFormatRunsPart(reader)) return;
			ReadPhoneticDataPart(reader);
		}
		public bool Complete {
			get {
				return IsValueCompleted() && IsFormatRunsCompleted() && IsPhoneticDataCompleted();
			}
		}
		#endregion
		void Cleanup() {
			this.charCount = 0;
			Value = null;
			FormatRuns.Clear();
			PhoneticData = null;
		}
		bool IsEmpty() {
			return (this.charCount == 0) && (Value.Length == 0) && (FormatRuns.Count == 0) && (PhoneticData.Length == 0);
		}
		bool IsValueCompleted() {
			return Value.Length == charCount;
		}
		bool IsFormatRunsCompleted() {
			return FormatRuns.Count == formatRunCount;
		}
		bool IsPhoneticDataCompleted() {
			return PhoneticData.Length == phoneticDataLen;
		}
		bool ReadValuePart(XlsReader reader) {
			if(!IsValueCompleted()) {
				if(reader.Position == 0)
					ReadFlags(reader);
				long bytesAvailable = reader.StreamLength - reader.Position;
				int bytesToRead = charCount - Value.Length;
				if(HasHighBytes)
					bytesToRead *= 2;
				if(bytesToRead > bytesAvailable)
					bytesToRead = (int)bytesAvailable;
				byte[] buffer = reader.ReadBytes(bytesToRead);
				string part = XLStringEncoder.GetEncoding(HasHighBytes).GetString(buffer, 0, bytesToRead);
				Value = Value + part;
			}
			return IsValueCompleted();
		}
		bool ReadFormatRunsPart(XlsReader reader) {
			while(!IsFormatRunsCompleted()) {
				long bytesAvailable = reader.StreamLength - reader.Position;
				if(bytesAvailable < XlsFormatRun.RecordSize) break;
				FormatRuns.Add(XlsFormatRun.FromStream(reader));
			}
			return IsFormatRunsCompleted();
		}
		void ReadPhoneticDataPart(XlsReader reader) {
			if(!IsPhoneticDataCompleted()) {
				int bytesToRead = phoneticDataLen - PhoneticData.Length;
				long bytesAvailable = reader.StreamLength - reader.Position;
				if(bytesToRead > bytesAvailable)
					bytesToRead = (int)bytesAvailable;
				byte[] buf = reader.ReadBytes(bytesToRead);
				if(PhoneticData.Length == 0)
					PhoneticData = buf;
				else {
					int initialLength = PhoneticData.Length;
					byte[] data = new byte[initialLength + bytesToRead];
					Array.Copy(PhoneticData, data, initialLength);
					Array.Copy(buf, 0, data, initialLength, bytesToRead);
					PhoneticData = data;
				}
			}
		}
		public void CopyFrom(XLUnicodeRichExtendedString value) {
			Cleanup();
			ForceHighBytes = value.ForceHighBytes;
			Value = value.Value;
			foreach(XlsFormatRun item in value.FormatRuns) {
				XlsFormatRun formatRun = new XlsFormatRun();
				formatRun.CharIndex = item.CharIndex;
				formatRun.FontIndex = item.FontIndex;
				this.FormatRuns.Add(formatRun);
			}
			if(value.PhoneticData.Length > 0) {
				byte[] data = new byte[value.PhoneticData.Length];
				Array.Copy(value.PhoneticData, data, data.Length);
				this.PhoneticData = data;
			}
		}
		public override bool Equals(object obj) {
			XLUnicodeRichExtendedString other = obj as XLUnicodeRichExtendedString;
			if(other == null) return false;
			if(Value != other.Value) return false;
			if(FormatRuns.Count != other.FormatRuns.Count) return false;
			if(FormatRuns.Count > 0) {
				for(int i = 0; i < FormatRuns.Count; i++) {
					if(!FormatRuns[i].Equals(other.FormatRuns[i])) return false;
				}
			}
			if(PhoneticData.Length != other.PhoneticData.Length) return false;
			if(PhoneticData.Length > 0) {
				for(int i = 0; i < PhoneticData.Length; i++) {
					if(PhoneticData[i] != other.PhoneticData[i]) return false;
				}
			}
			return true;
		}
		public override int GetHashCode() {
			return 0;
		}
	}
	#endregion
	#region XLUnicodeCharactersArray
	public class XLUnicodeCharactersArray {
		string value = string.Empty;
		#region Properties
		public string Value {
			get { return this.value; }
			set {
				if(value == null)
					value = string.Empty;
				if(value.Length > ushort.MaxValue)
					throw new ArgumentException("String value too long");
				this.value = value;
			}
		}
		public virtual int Length { get { return this.value.Length * 2; } }
		#endregion
		public static XLUnicodeCharactersArray FromStream(XlsReader reader, int charCount) {
			XLUnicodeCharactersArray result = new XLUnicodeCharactersArray();
			result.Read(reader, charCount);
			return result;
		}
		protected void Read(XlsReader reader, int charCount) {
			byte[] buf = reader.ReadBytes(charCount * 2);
			this.value = XLStringEncoder.GetEncoding(true).GetString(buf, 0, buf.Length);
		}
		public virtual void Write(BinaryWriter writer) {
			if(!String.IsNullOrEmpty(this.value)) {
				byte[] buf = XLStringEncoder.GetEncoding(true).GetBytes(this.value);
				writer.Write(buf);
			}
		}
	}
	#endregion
	#region LPWideString
	public class LPWideString : XLUnicodeCharactersArray {
		public override int Length { get { return 2 + base.Length; } }
		public static LPWideString FromStream(XlsReader reader) {
			LPWideString result = new LPWideString();
			result.Read(reader);
			return result;
		}
		protected void Read(XlsReader reader) {
			int charCount = reader.ReadUInt16();
			base.Read(reader, charCount);
		}
		public override void Write(BinaryWriter writer) {
			int charCount = Value.Length;
			writer.Write((ushort)charCount);
			base.Write(writer);
		}
	}
	#endregion
	#region XLUnicodeStringNoCch
	public class XLUnicodeStringNoCch {
		#region Fields
		bool hasHighBytes;
		string value = string.Empty;
		#endregion
		#region Properties
		public bool HasHighBytes {
			get { return hasHighBytes; }
		}
		public string Value {
			get { return this.value; }
			set {
				if(value == null) value = string.Empty;
				if(value.Length > short.MaxValue)
					throw new ArgumentException("String value too long");
				this.value = value;
				this.hasHighBytes = XLStringEncoder.StringHasHighBytes(this.value);
			}
		}
		public int Length { get { return (HasHighBytes ? this.value.Length * 2 : this.value.Length) + 1; } }
		#endregion
		public static XLUnicodeStringNoCch FromStream(BinaryReader reader, int charCount) {
			XLUnicodeStringNoCch result = new XLUnicodeStringNoCch();
			result.Read(reader, charCount);
			return result;
		}
		public static XLUnicodeStringNoCch FromStream(XlsReader reader, int charCount) {
			XLUnicodeStringNoCch result = new XLUnicodeStringNoCch();
			result.Read(reader, charCount);
			return result;
		}
		protected void Read(BinaryReader reader, int charCount) {
			this.hasHighBytes = Convert.ToBoolean(reader.ReadByte());
			if(charCount > 0) {
				int length = hasHighBytes ? charCount * 2 : charCount;
				byte[] buffer = reader.ReadBytes(length);
				this.value = XLStringEncoder.GetEncoding(HasHighBytes).GetString(buffer, 0, length);
			}
		}
		protected void Read(XlsReader reader, int charCount) {
			this.hasHighBytes = Convert.ToBoolean(reader.ReadByte());
			if(charCount > 0) {
				int length = hasHighBytes ? charCount * 2 : charCount;
				byte[] buffer = reader.ReadBytes(length);
				this.value = XLStringEncoder.GetEncoding(HasHighBytes).GetString(buffer, 0, length);
			}
		}
		public void Write(BinaryWriter writer) {
			if(this.value.Length > 0) {
				writer.Write((byte)(HasHighBytes ? 1 : 0));
				writer.Write(XLStringEncoder.GetBytes(this.value, HasHighBytes));
			}
		}
	}
	#endregion
	#region XlsPalette
	class XlsPalette {
		#region Fields
		public const int BuiltInColorsCount = 8;
		public const int DefaultForegroundColorIndex = 64;
		public const int DefaultBackgroundColorIndex = 65;
		public const int SystemWindowFrameColorIndex = 66;
		public const int System3DFaceColorIndex = 67;
		public const int System3DTextColorIndex = 68;
		public const int System3DHighlightColorIndex = 69;
		public const int System3DShadowColorIndex = 70;
		public const int SystemHighlightColorIndex = 71;
		public const int SystemControlTextColorIndex = 72;
		public const int SystemControlScrollColorIndex = 73;
		public const int SystemControlInverseColorIndex = 74;
		public const int SystemControlBodyColorIndex = 75;
		public const int SystemControlFrameColorIndex = 76;
		public const int DefaultChartForegroundColorIndex = 77;
		public const int DefaultChartBackgroundColorIndex = 78;
		public const int ChartNeutralColorIndex = 79;
		public const int ToolTipFillColorIndex = 80;
		public const int ToolTipTextColorIndex = 81;
		public const int FontAutomaticColorIndex = 32767;
		readonly Dictionary<int, Color> colorTable;
		#endregion
		public XlsPalette() {
			colorTable = CreateDefaultColorTable();
		}
		#region Properties
		public Color this[int index] {
			get { return colorTable[index]; }
			set { colorTable[index] = value; }
		}
		public Color DefaultForegroundColor {
			get { return colorTable[DefaultForegroundColorIndex]; }
			set { colorTable[DefaultForegroundColorIndex] = value; }
		}
		public Color DefaultBackgroundColor {
			get { return colorTable[DefaultBackgroundColorIndex]; }
			set { colorTable[DefaultBackgroundColorIndex] = value; }
		}
		public Color DefaultChartForegroundColor {
			get { return colorTable[DefaultChartForegroundColorIndex]; }
			set { colorTable[DefaultChartForegroundColorIndex] = value; }
		}
		public Color DefaultChartBackgroundColor {
			get { return colorTable[DefaultChartBackgroundColorIndex]; }
			set { colorTable[DefaultChartBackgroundColorIndex] = value; }
		}
		public Color ChartNeutralColor { get { return colorTable[ChartNeutralColorIndex]; } }
		public Color ToolTipTextColor {
			get { return colorTable[ToolTipTextColorIndex]; }
			set { colorTable[ToolTipTextColorIndex] = value; }
		}
		public Color FontAutomaticColor {
			get { return colorTable[FontAutomaticColorIndex]; }
			set { colorTable[FontAutomaticColorIndex] = value; }
		}
		#endregion
		public bool IsValidColorIndex(int index) {
			return colorTable.ContainsKey(index);
		}
		public int GetColorIndex(Color color) {
			int index = GetExactColorIndex(color, 0, FontAutomaticColorIndex);
			if(index != -1)
				return index;
			if(color.A == 0xFF) {
				color = DXColor.FromArgb(0, color.R, color.G, color.B);
				index = GetExactColorIndex(color, 0, FontAutomaticColorIndex);
				if(index != -1)
					return index;
			}
			return DefaultForegroundColorIndex;
		}
		public int GetNearestColorIndex(Color color) {
			int nearest = GetExactColorIndex(color, 0, 63);
			if(nearest != -1) return nearest;
			return GetNearestColorIndexCore(color, 0, 63);
		}
		public int GetPaletteNearestColorIndex(Color color) {
			int nearest = GetExactColorIndex(color, 8, 63);
			if(nearest != -1) return nearest;
			return GetNearestColorIndexCore(color, 8, 63);
		}
		public int GetFontColorIndex(XlColor color, XlDocumentTheme theme) {
			if(color.IsAutoOrEmpty)
				return FontAutomaticColorIndex;
			return GetPaletteNearestColorIndex(color.ConvertToRgb(theme));
		}
		public int GetForegroundColorIndex(XlColor color, XlDocumentTheme theme) {
			if(color.IsAutoOrEmpty)
				return DefaultForegroundColorIndex;
			return GetPaletteNearestColorIndex(color.ConvertToRgb(theme));
		}
		public int GetBackgroundColorIndex(XlColor color, XlDocumentTheme theme) {
			if(color.IsAutoOrEmpty)
				return DefaultBackgroundColorIndex;
			return GetPaletteNearestColorIndex(color.ConvertToRgb(theme));
		}
		#region Default color table
		Dictionary<int, Color> CreateDefaultColorTable() {
			Dictionary<int, Color> result = new Dictionary<int, Color>();
			result.Add(0, DXColor.FromArgb(0, 0, 0, 0));
			result.Add(1, DXColor.FromArgb(0, 255, 255, 255));
			result.Add(2, DXColor.FromArgb(0, 255, 0, 0));
			result.Add(3, DXColor.FromArgb(0, 0, 255, 0));
			result.Add(4, DXColor.FromArgb(0, 0, 0, 255));
			result.Add(5, DXColor.FromArgb(0, 255, 255, 0));
			result.Add(6, DXColor.FromArgb(0, 255, 0, 255));
			result.Add(7, DXColor.FromArgb(0, 0, 255, 255));
			result.Add(8, DXColor.FromArgb(0, 0, 0, 0));
			result.Add(9, DXColor.FromArgb(0, 255, 255, 255));
			result.Add(10, DXColor.FromArgb(0, 255, 0, 0));
			result.Add(11, DXColor.FromArgb(0, 0, 255, 0));
			result.Add(12, DXColor.FromArgb(0, 0, 0, 255));
			result.Add(13, DXColor.FromArgb(0, 255, 255, 0));
			result.Add(14, DXColor.FromArgb(0, 255, 0, 255));
			result.Add(15, DXColor.FromArgb(0, 0, 255, 255));
			result.Add(16, DXColor.FromArgb(0, 128, 0, 0));
			result.Add(17, DXColor.FromArgb(0, 0, 128, 0));
			result.Add(18, DXColor.FromArgb(0, 0, 0, 128));
			result.Add(19, DXColor.FromArgb(0, 128, 128, 0));
			result.Add(20, DXColor.FromArgb(0, 128, 0, 128));
			result.Add(21, DXColor.FromArgb(0, 0, 128, 128));
			result.Add(22, DXColor.FromArgb(0, 192, 192, 192));
			result.Add(23, DXColor.FromArgb(0, 128, 128, 128));
			result.Add(24, DXColor.FromArgb(0, 153, 153, 255));
			result.Add(25, DXColor.FromArgb(0, 153, 51, 102));
			result.Add(26, DXColor.FromArgb(0, 255, 255, 204));
			result.Add(27, DXColor.FromArgb(0, 204, 255, 255));
			result.Add(28, DXColor.FromArgb(0, 102, 0, 102));
			result.Add(29, DXColor.FromArgb(0, 255, 128, 128));
			result.Add(30, DXColor.FromArgb(0, 0, 102, 204));
			result.Add(31, DXColor.FromArgb(0, 204, 204, 255));
			result.Add(32, DXColor.FromArgb(0, 0, 0, 128));
			result.Add(33, DXColor.FromArgb(0, 255, 0, 255));
			result.Add(34, DXColor.FromArgb(0, 255, 255, 0));
			result.Add(35, DXColor.FromArgb(0, 0, 255, 255));
			result.Add(36, DXColor.FromArgb(0, 128, 0, 128));
			result.Add(37, DXColor.FromArgb(0, 128, 0, 0));
			result.Add(38, DXColor.FromArgb(0, 0, 128, 128));
			result.Add(39, DXColor.FromArgb(0, 0, 0, 255));
			result.Add(40, DXColor.FromArgb(0, 0, 204, 255));
			result.Add(41, DXColor.FromArgb(0, 204, 255, 255));
			result.Add(42, DXColor.FromArgb(0, 204, 255, 204));
			result.Add(43, DXColor.FromArgb(0, 255, 255, 153));
			result.Add(44, DXColor.FromArgb(0, 153, 204, 255));
			result.Add(45, DXColor.FromArgb(0, 255, 153, 204));
			result.Add(46, DXColor.FromArgb(0, 204, 153, 255));
			result.Add(47, DXColor.FromArgb(0, 255, 204, 153));
			result.Add(48, DXColor.FromArgb(0, 51, 102, 255));
			result.Add(49, DXColor.FromArgb(0, 51, 204, 204));
			result.Add(50, DXColor.FromArgb(0, 153, 204, 0));
			result.Add(51, DXColor.FromArgb(0, 255, 204, 0));
			result.Add(52, DXColor.FromArgb(0, 255, 153, 0));
			result.Add(53, DXColor.FromArgb(0, 255, 102, 0));
			result.Add(54, DXColor.FromArgb(0, 102, 102, 153));
			result.Add(55, DXColor.FromArgb(0, 150, 150, 150));
			result.Add(56, DXColor.FromArgb(0, 0, 51, 102));
			result.Add(57, DXColor.FromArgb(0, 51, 153, 102));
			result.Add(58, DXColor.FromArgb(0, 0, 51, 0));
			result.Add(59, DXColor.FromArgb(0, 51, 51, 0));
			result.Add(60, DXColor.FromArgb(0, 153, 51, 0));
			result.Add(61, DXColor.FromArgb(0, 153, 51, 102));
			result.Add(62, DXColor.FromArgb(0, 51, 51, 153));
			result.Add(63, DXColor.FromArgb(0, 51, 51, 51));
			result.Add(SystemWindowFrameColorIndex, DXSystemColors.WindowFrame);
			result.Add(System3DFaceColorIndex, DXSystemColors.Control);
			result.Add(System3DTextColorIndex, DXSystemColors.ControlText);
			result.Add(System3DHighlightColorIndex, DXSystemColors.ControlLight);
			result.Add(System3DShadowColorIndex, DXSystemColors.ControlDark);
			result.Add(SystemHighlightColorIndex, DXSystemColors.Highlight);
			result.Add(SystemControlTextColorIndex, DXSystemColors.ControlText);
			Color sbc = DXSystemColors.ScrollBar;
			result.Add(SystemControlScrollColorIndex, sbc);
			result.Add(SystemControlInverseColorIndex, DXColor.FromArgb(0, (byte)~sbc.R, (byte)~sbc.G, (byte)~sbc.B));
			result.Add(SystemControlBodyColorIndex, DXSystemColors.Window);
			result.Add(SystemControlFrameColorIndex, DXSystemColors.WindowFrame);
			result.Add(DefaultForegroundColorIndex, DXSystemColors.WindowText);
			result.Add(DefaultBackgroundColorIndex, DXSystemColors.Window);
			result.Add(DefaultChartForegroundColorIndex, DXColor.FromArgb(0, 0, 0, 0));
			result.Add(DefaultChartBackgroundColorIndex, DXColor.FromArgb(0, 255, 255, 255));
			result.Add(ChartNeutralColorIndex, DXColor.FromArgb(0, 0, 0, 0));
			result.Add(ToolTipFillColorIndex, DXSystemColors.Info);
			result.Add(ToolTipTextColorIndex, DXSystemColors.InfoText);
			result.Add(FontAutomaticColorIndex, DXColor.Empty);
			return result;
		}
		#endregion
		#region Internals
		class ColorDistanceInfo : IComparable<ColorDistanceInfo> {
			public double Distance { get; set; }
			public int ColorIndex { get; set; }
			#region IComparable<ColorDistanceInfo> Members
			public int CompareTo(ColorDistanceInfo other) {
				if(Distance > other.Distance) return 1;
				if(Distance < other.Distance) return -1;
				return 0;
			}
			#endregion
		}
		int GetExactColorIndex(Color color, int minIndex, int maxIndex) {
			foreach(KeyValuePair<int, Color> pair in colorTable) {
				if(pair.Key < minIndex) continue;
				if(pair.Key > maxIndex) continue;
				if(pair.Value == color)
					return pair.Key;
			}
			return -1;
		}
		int GetNearestColorIndexCore(Color color, int minIndex, int maxIndex) {
			List<ColorDistanceInfo> items = new List<ColorDistanceInfo>();
			foreach(KeyValuePair<int, Color> pair in colorTable) {
				if(pair.Key < minIndex) continue;
				if(pair.Key > maxIndex) continue;
				if(IsCompatibleColors(pair.Value, color)) {
					ColorDistanceInfo item = new ColorDistanceInfo();
					item.Distance = GetColorDistance(color, pair.Value, 3.0);
					item.ColorIndex = pair.Key;
					items.Add(item);
				}
			}
			items.Sort();
			const int limit = 5;
			if(items.Count > limit)
				items.RemoveRange(limit, items.Count - limit);
			int nearest = -1;
			double distance = double.MaxValue;
			foreach(ColorDistanceInfo item in items) {
				if(nearest == -1) {
					nearest = item.ColorIndex;
					distance = GetColorDistance(color, colorTable[item.ColorIndex], 1.5);
				}
				else {
					double d = GetColorDistance(color, colorTable[item.ColorIndex], 1.5);
					if(d < distance) {
						nearest = item.ColorIndex;
						distance = d;
					}
				}
			}
			return nearest;
		}
		double GetColorDistance(Color x, Color y, double rgbWeight) {
			double hsbD = ColorDifferenceHSB(x, y);
			double rgbD = ColorDifferenceRGB(x, y) * rgbWeight;
			return hsbD + rgbD;
		}
		bool IsCompatibleColors(Color x, Color y) {
			bool xIsGray = x.R == x.G && x.R == x.B;
			bool yIsGray = y.R == y.G && y.R == y.B;
			return xIsGray == yIsGray;
		}
		double ColorDifferenceRGB(Color x, Color y) {
			double deltaR = ((double)x.R - (double)y.R) / 255;
			double deltaG = ((double)x.G - (double)y.G) / 255;
			double deltaB = ((double)x.B - (double)y.B) / 255;
			return Math.Sqrt(deltaR * deltaR + deltaG * deltaG + deltaB * deltaB);
		}
		double ColorDifferenceHSB(Color x, Color y) {
			double deltaH = Math.Abs(x.GetHue() - y.GetHue());
			if(deltaH > 180.0)
				deltaH = 360.0 - deltaH;
			deltaH /= 57.3;
			double deltaB = Math.Abs(x.GetBrightness() - y.GetBrightness()) * 3.0;
			double deltaS = Math.Abs(x.GetSaturation() - y.GetSaturation()) * 1.5;
			return deltaB + deltaH + deltaS;
		}
		#endregion
	}
	#endregion
	#region FutureRecordHeaderBase
	public abstract class FutureRecordHeaderBase {
		#region Properties
		public virtual short RecordTypeId { get; set; }
		#endregion
		public void Read(XlsReader reader) {
			RecordTypeId = reader.ReadInt16();
			ReadCore(reader);
		}
		public void Write(BinaryWriter writer) {
			writer.Write(RecordTypeId);
			WriteCore(writer);
		}
		protected abstract void ReadCore(XlsReader reader);
		protected abstract void WriteCore(BinaryWriter writer);
		public abstract short GetSize();
	}
	#endregion
	#region FutureRecordHeaderFlagsBase
	public abstract class FutureRecordHeaderFlagsBase : FutureRecordHeaderBase {
		#region Properties
		public bool RangeOfCells { get; set; }
		public bool Alert { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader) {
			ushort bitwiseField = reader.ReadUInt16();
			RangeOfCells = Convert.ToBoolean(bitwiseField & 0x0001);
			Alert = Convert.ToBoolean(bitwiseField & 0x0002);
		}
		protected override void WriteCore(BinaryWriter writer) {
			ushort bitwiseField = 0;
			if(RangeOfCells) bitwiseField |= 0x0001;
			if(Alert) bitwiseField |= 0x0002;
			writer.Write(bitwiseField);
		}
	}
	#endregion
	#region FutureRecordHeader
	public class FutureRecordHeader : FutureRecordHeaderFlagsBase {
		public static FutureRecordHeader FromStream(XlsReader reader) {
			FutureRecordHeader result = new FutureRecordHeader();
			result.Read(reader);
			return result;
		}
		protected override void ReadCore(XlsReader reader) {
			base.ReadCore(reader);
			reader.ReadUInt64(); 
		}
		protected override void WriteCore(BinaryWriter writer) {
			base.WriteCore(writer);
			writer.Write((ulong)0); 
		}
		public override short GetSize() {
			return 12;
		}
	}
	#endregion
	#region FutureRecordHeaderRefNoFlags
	public class FutureRecordHeaderRefNoFlags : FutureRecordHeaderBase {
		XlsRef8 range = new XlsRef8();
		public XlsRef8 Range {
			get { return range; }
			set {
				if(value == null)
					value = new XlsRef8();
				range = value;
			}
		}
		public static FutureRecordHeaderRefNoFlags FromStream(XlsReader reader) {
			FutureRecordHeaderRefNoFlags result = new FutureRecordHeaderRefNoFlags();
			result.Read(reader);
			return result;
		}
		protected override void ReadCore(XlsReader reader) {
			this.range = XlsRef8.FromStream(reader);
		}
		protected override void WriteCore(BinaryWriter writer) {
			this.range.Write(writer);
		}
		public override short GetSize() {
			return 10;
		}
	}
	#endregion
	#region FutureRecordHeaderRefU
	public class FutureRecordHeaderRefU : FutureRecordHeaderFlagsBase {
		#region Static Members
		public static FutureRecordHeaderRefU FromStream(XlsReader reader) {
			FutureRecordHeaderRefU result = new FutureRecordHeaderRefU();
			result.Read(reader);
			return result;
		}
		#endregion
		#region Fields
		XlsRef8 range = new XlsRef8();
		#endregion
		#region Fields
		public XlsRef8 Range {
			get { return range; }
			set {
				if(value != null)
					range = value;
				else
					range = new XlsRef8();
			}
		}
		#endregion
		protected override void ReadCore(XlsReader reader) {
			base.ReadCore(reader);
			this.range = XlsRef8.FromStream(reader);
		}
		protected override void WriteCore(BinaryWriter writer) {
			base.WriteCore(writer);
			this.range.Write(writer);
		}
		public override short GetSize() {
			return 12;
		}
	}
	#endregion
	#region XlStyleCategory
	public enum XlStyleCategory {
		CustomStyle = 0x00,
		GoodBadNeutralStyle = 0x01,
		DataModelStyle = 0x02,
		TitleAndHeadingStyle = 0x03,
		ThemedCellStyle = 0x04,
		NumberFormatStyle = 0x05
	}
	#endregion
	#region XlsCountryCodes
	public static class XlsCountryCodes {
		const int countryCodeUS = 1;
		static Dictionary<int, CultureInfo> codeTable;
		static Dictionary<CultureInfo, int> cultureTable;
		static XlsCountryCodes() {
			codeTable = new Dictionary<int, CultureInfo>();
			cultureTable = new Dictionary<CultureInfo, int>();
			InitializeCodeTable();
			InitializeCultureTable();
		}
		static void InitializeCodeTable() {
			codeTable.Add(1, new CultureInfo("en-US"));   
			codeTable.Add(2, new CultureInfo("en-CA"));   
			codeTable.Add(3, new CultureInfo("es"));	  
			codeTable.Add(7, new CultureInfo("ru-RU"));   
			codeTable.Add(20, new CultureInfo("ar-EG"));  
			codeTable.Add(30, new CultureInfo("el-GR"));  
			codeTable.Add(31, new CultureInfo("nl-NL"));  
			codeTable.Add(32, new CultureInfo("nl-BE"));  
			codeTable.Add(33, new CultureInfo("fr-FR"));  
			codeTable.Add(34, new CultureInfo("es-ES"));  
			codeTable.Add(36, new CultureInfo("hu-HU"));  
			codeTable.Add(39, new CultureInfo("it-IT"));  
			codeTable.Add(41, new CultureInfo("de-CH"));  
			codeTable.Add(43, new CultureInfo("de-AT"));  
			codeTable.Add(44, new CultureInfo("en-GB"));  
			codeTable.Add(45, new CultureInfo("da-DK"));  
			codeTable.Add(46, new CultureInfo("sv-SE"));  
			codeTable.Add(47, new CultureInfo("no"));	 
			codeTable.Add(48, new CultureInfo("pl-PL"));  
			codeTable.Add(49, new CultureInfo("de-DE"));  
			codeTable.Add(52, new CultureInfo("es-MX"));  
			codeTable.Add(55, new CultureInfo("pt-BR"));  
			codeTable.Add(61, new CultureInfo("en-AU"));  
			codeTable.Add(64, new CultureInfo("en-NZ"));  
			codeTable.Add(66, new CultureInfo("th-TH"));  
			codeTable.Add(81, new CultureInfo("ja-JP"));  
			codeTable.Add(82, new CultureInfo("ko-KR"));  
			codeTable.Add(84, new CultureInfo("vi-VN"));  
			codeTable.Add(86, new CultureInfo("zh-CN"));  
			codeTable.Add(90, new CultureInfo("tr-TR"));  
			codeTable.Add(213, new CultureInfo("ar-DZ")); 
			codeTable.Add(216, new CultureInfo("ar-MA")); 
			codeTable.Add(218, new CultureInfo("ar-LY")); 
			codeTable.Add(351, new CultureInfo("pt-PT")); 
			codeTable.Add(354, new CultureInfo("is-IS")); 
			codeTable.Add(358, new CultureInfo("fi-FI")); 
			codeTable.Add(420, new CultureInfo("cs-CZ")); 
			codeTable.Add(886, new CultureInfo("zh-TW")); 
			codeTable.Add(961, new CultureInfo("ar-LB")); 
			codeTable.Add(962, new CultureInfo("ar-JO")); 
			codeTable.Add(963, new CultureInfo("ar-SY")); 
			codeTable.Add(964, new CultureInfo("ar-IQ")); 
			codeTable.Add(965, new CultureInfo("ar-KW")); 
			codeTable.Add(966, new CultureInfo("ar-SA")); 
			codeTable.Add(971, new CultureInfo("ar-AE")); 
			codeTable.Add(972, new CultureInfo("he-IL")); 
			codeTable.Add(974, new CultureInfo("ar-QA")); 
			codeTable.Add(981, new CultureInfo("fa-IR")); 
		}
		static void InitializeCultureTable() {
			foreach(KeyValuePair<int, CultureInfo> item in codeTable) {
				if(!item.Value.IsNeutralCulture)
					cultureTable.Add(item.Value, item.Key);
			}
			cultureTable.Add(new CultureInfo("es-AR"), 3);  
			cultureTable.Add(new CultureInfo("es-BO"), 3);  
			cultureTable.Add(new CultureInfo("es-CL"), 3);  
			cultureTable.Add(new CultureInfo("es-CO"), 3);  
			cultureTable.Add(new CultureInfo("es-CR"), 3);  
			cultureTable.Add(new CultureInfo("es-DO"), 3);  
			cultureTable.Add(new CultureInfo("es-EC"), 3);  
			cultureTable.Add(new CultureInfo("es-SV"), 3);  
			cultureTable.Add(new CultureInfo("es-GT"), 3);  
			cultureTable.Add(new CultureInfo("es-HN"), 3);  
			cultureTable.Add(new CultureInfo("es-NI"), 3);  
			cultureTable.Add(new CultureInfo("es-PA"), 3);  
			cultureTable.Add(new CultureInfo("es-PY"), 3);  
			cultureTable.Add(new CultureInfo("es-PE"), 3);  
			cultureTable.Add(new CultureInfo("es-PR"), 3);  
			cultureTable.Add(new CultureInfo("es-UY"), 3);  
			cultureTable.Add(new CultureInfo("es-VE"), 3);  
			cultureTable.Add(new CultureInfo("nb-NO"), 47); 
			cultureTable.Add(new CultureInfo("nn-NO"), 47); 
		}
		public static CultureInfo GetCultureInfo(int countryCode) {
			CultureInfo currentCulture = CultureInfo.CurrentCulture;
			CultureInfo culture = currentCulture;
			if(codeTable.ContainsKey(countryCode)) {
				culture = codeTable[countryCode];
				if(culture.IsNeutralCulture) {
					if(culture.IsParentOf(currentCulture))
						return currentCulture;
					return CreateSpecificCulture(culture.Name);
				}
			}
			return culture;
		}
		public static int GetCountryCode(CultureInfo cultureInfo) {
			if(cultureTable.ContainsKey(cultureInfo))
				return cultureTable[cultureInfo];
			return countryCodeUS;
		}
		public static bool IsParentOf(this CultureInfo parentCulture, CultureInfo culture) {
			if(string.IsNullOrEmpty(culture.Name))
				return false;
			if(string.IsNullOrEmpty(parentCulture.Name))
				return true;
			CultureInfo parent = culture.Parent;
			while(!string.IsNullOrEmpty(parent.Name)) {
				if(parent.Name == parentCulture.Name)
					return true;
				parent = parent.Parent;
			}
			return false;
		}
#if !SL && !DXPORTABLE
		static CultureInfo CreateSpecificCulture(string name) {
			return CultureInfo.CreateSpecificCulture(name);
		}
#else
		static CultureInfo CreateSpecificCulture(string name) {
			return CultureInfo.CurrentCulture;
		}
#endif
	}
	#endregion
	#region XlsSSTInfo
	public class XlsSSTInfo {
		#region Fields
		int streamPosition;
		int offset;
		#endregion
		#region Properties
		public int StreamPosition {
			get { return streamPosition; }
			set {
				Guard.ArgumentNonNegative(value, "StreamPosition value");
				streamPosition = value;
			}
		}
		public int Offset {
			get { return offset; }
			set {
				if (value < 0 || value > ushort.MaxValue)
					throw new ArgumentOutOfRangeException("Offset value");
				offset = value;
			}
		}
		#endregion
	}
	#endregion
	#region XlsRkRec
	public class XlsRkRec {
		XlsRkNumber rk = new XlsRkNumber(0);
		#region Properties
		public int FormatIndex { get; set; }
		public XlsRkNumber Rk {
			get { return rk; }
			set {
				if(value == null)
					rk = new XlsRkNumber(0);
				else
					rk = value;
			}
		}
		#endregion
		public static XlsRkRec Read(XlsReader reader) {
			XlsRkRec result = new XlsRkRec();
			result.FormatIndex = reader.ReadUInt16();
			result.Rk = new XlsRkNumber(reader.ReadInt32());
			return result;
		}
		public void Write(BinaryWriter writer) {
			writer.Write((ushort)FormatIndex);
			writer.Write(Rk.GetRawValue());
		}
	}
	#endregion
	#region XlsRkNumber
	public class XlsRkNumber {
		#region DoubleInt64Union
		[StructLayout(LayoutKind.Explicit)]
		struct DoubleInt64Union {
			[FieldOffset(0)]
			public double DoubleValue;
			[FieldOffset(0)]
			public Int64 Int64Value;
		}
		#endregion
		#region Fields
		const long valueMask = 0xfffffffc;
		const long low34BitsMask = 0x3ffffffff;
		const int int30MinValue = -536870912; 
		const int int30MaxValue = 536870911; 
		int rkType;
		double value;
		#endregion
		public XlsRkNumber(Int32 rawValue) {
			this.rkType = rawValue & 0x03;
			DoubleInt64Union helper = new DoubleInt64Union();
			if(!IsInt) 
				helper.Int64Value = (rawValue & valueMask) << 32;
			else
				helper.DoubleValue = (double)((int)((rawValue & valueMask)) >> 2);
			this.value = (X100) ? helper.DoubleValue / 100 : helper.DoubleValue;
		}
		#region Properties
		public double Value {
			get { return this.value; }
			set {
				if(!IsRkValue(value))
					throw new ArgumentException("value can't be represented as RkNumber");
				this.value = value;
				this.rkType = GetRkType(value);
			}
		}
		public bool X100 { get { return (rkType & 0x01) != 0; } }
		public bool IsInt { get { return (rkType & 0x02) != 0; } }
		#endregion
		public Int32 GetRawValue() {
			DoubleInt64Union helper = new DoubleInt64Union();
			if(X100)
				helper.DoubleValue = Value * 100;
			else
				helper.DoubleValue = Value;
			int result;
			if(!IsInt) 
				result = (int)((helper.Int64Value >> 32) & valueMask);
			else
				result = ((int)helper.DoubleValue) << 2;
			result |= this.rkType;
			return result;
		}
		public static bool IsRkValue(double value) {
			return GetRkType(value) != -1;
		}
		static int GetRkType(double value) {
			DoubleInt64Union helper = new DoubleInt64Union();
			helper.DoubleValue = value;
			if((helper.Int64Value & low34BitsMask) == 0)
				return 0;
			if(CanBePresentedAsInt30(value))
				return 2;
			try {
				helper.DoubleValue = (double)(value * 100);
				if((helper.Int64Value & low34BitsMask) == 0)
					return 1;
				if(CanBePresentedAsInt30(value * 100))
					return 3;
			}
			catch(OverflowException) { }
			return -1;
		}
		static bool CanBePresentedAsInt30(double value) {
			double truncated = Truncate(value);
			return truncated == value && truncated >= int30MinValue && truncated <= int30MaxValue;
		}
		static double Truncate(double value) {
#if !SL
			return Math.Truncate(value);
#else
			return (int)value;
#endif
		}
	}
	#endregion
	#region XlsFormulaValue
	public class XlsFormulaValue {
		#region FormulaValueUnion
		[StructLayout(LayoutKind.Explicit)]
		struct FormulaValueUnion {
			[FieldOffset(0)]
			public Int64 Int64Value;
			[FieldOffset(0)]
			public double DoubleValue;
			[FieldOffset(0)]
			public byte Byte1;
			[FieldOffset(2)]
			public byte Byte3;
			[FieldOffset(6)]
			public ushort ExprO;
		}
		#endregion
		#region Fields
		const ushort nanExpr = 0xffff;
		FormulaValueUnion value;
		#endregion
		#region Properties
		public bool IsString {
			get { return value.ExprO == nanExpr && value.Byte1 == 0x00; }
			set {
				if(value) {
					this.value.Int64Value = 0;
					this.value.ExprO = nanExpr;
				}
			}
		}
		public bool IsBoolean { get { return value.ExprO == nanExpr && value.Byte1 == 0x01; } }
		public bool IsError { get { return value.ExprO == nanExpr && value.Byte1 == 0x02; } }
		public bool IsBlankString {
			get { return value.ExprO == nanExpr && value.Byte1 == 0x03; }
			set {
				if(value) {
					this.value.Int64Value = 0;
					this.value.Byte1 = 0x03;
					this.value.ExprO = nanExpr;
				}
			}
		}
		public bool IsNumeric { get { return value.ExprO != nanExpr; } }
		public bool BooleanValue {
			get {
				if(IsBoolean)
					return value.Byte3 != 0;
				return false;
			}
			set {
				this.value.Int64Value = 0;
				this.value.Byte1 = 0x01;
				this.value.Byte3 = (byte)(value ? 1 : 0);
				this.value.ExprO = nanExpr;
			}
		}
		public byte ErrorCode {
			get {
				if(IsError)
					return this.value.Byte3;
				return 0;
			}
			set {
				this.value.Int64Value = 0;
				this.value.Byte1 = 0x02;
				this.value.Byte3 = value;
				this.value.ExprO = nanExpr;
			}
		}
		public double NumericValue {
			get {
				if(IsNumeric) return value.DoubleValue;
				return double.NaN;
			}
			set {
				if(XNumChecker.IsNegativeZero(value))
					this.value.DoubleValue = 0.0;
				else {
					XNumChecker.CheckValue(value);
					this.value.DoubleValue = value;
				}
			}
		}
		#endregion
		public void Read(XlsReader reader) {
			this.value.Int64Value = reader.ReadInt64();
		}
		public void Write(BinaryWriter writer) {
			writer.Write(this.value.Int64Value);
		}
	}
	#endregion
	#region XlsDbCellCalculator
	public class XlsDbCellCalculator {
		#region Fields
		List<long> rowPositions = new List<long>();
		List<long> firstCellPositions = new List<long>();
		long dbCellPosition;
		#endregion
		public void RegisterRowPosition(long position) {
			this.rowPositions.Add(position);
		}
		public void RegisterFirstCellPosition(long position) {
			this.firstCellPositions.Add(position);
		}
		public void RegisterDbCellPosition(long position) {
			this.dbCellPosition = position;
		}
		public long CalculateFirstRowOffset() {
			if(this.rowPositions.Count == 0)
				return 0;
			return this.dbCellPosition - this.rowPositions[0];
		}
		public List<int> CalculateStreamOffsets() {
			List<int> result = new List<int>();
			if(this.rowPositions.Count > 0) {
				if(this.rowPositions.Count == 1) {
					result.Add(0);
				}
				else {
					int offsetFromSecondRowToFirstCellOfFirstRow = (int)(this.firstCellPositions[0] - this.rowPositions[1]);
					result.Add(offsetFromSecondRowToFirstCellOfFirstRow);
					int count = this.firstCellPositions.Count;
					for(int i = 1; i < count; i++) {
						int offsetToNextRowFirstCell = (int)(this.firstCellPositions[i] - this.firstCellPositions[i - 1]);
						result.Add(offsetToNextRowFirstCell);
					}
				}
			}
			return result;
		}
		public void Reset() {
			this.rowPositions.Clear();
			this.firstCellPositions.Clear();
		}
	}
	#endregion
	#region XlsRef8
	public class XlsRef8 {
		public int FirstColumnIndex { get; set; }
		public int LastColumnIndex { get; set; }
		public int FirstRowIndex { get; set; }
		public int LastRowIndex { get; set; }
		public static XlsRef8 FromStream(XlsReader reader) {
			XlsRef8 result = new XlsRef8();
			result.Read(reader);
			return result;
		}
		public static XlsRef8 FromRange(XlCellRange range) {
			XlsRef8 result = new XlsRef8();
			result.FirstRowIndex = range.TopLeft.Row == XlCellPosition.InvalidValue.Row ? 0 : range.TopLeft.Row;
			result.LastRowIndex = range.BottomRight.Row == XlCellPosition.InvalidValue.Row ? XlsDefs.MaxRowCount - 1 : range.BottomRight.Row;
			result.FirstColumnIndex = range.TopLeft.Column == XlCellPosition.InvalidValue.Column ? 0 : range.TopLeft.Column;
			result.LastColumnIndex = range.BottomRight.Column == XlCellPosition.InvalidValue.Column ? XlsDefs.MaxColumnCount - 1 : range.BottomRight.Column;
			if(result.FirstRowIndex >= XlsDefs.MaxRowCount)
				return null;
			if(result.FirstColumnIndex >= XlsDefs.MaxColumnCount)
				return null;
			if(result.LastRowIndex >= XlsDefs.MaxRowCount)
				result.LastRowIndex = XlsDefs.MaxRowCount - 1;
			if(result.LastColumnIndex >= XlsDefs.MaxColumnCount)
				result.LastColumnIndex = XlsDefs.MaxColumnCount - 1;
			return result;
		}
		protected void Read(XlsReader reader) {
			FirstRowIndex = reader.ReadUInt16();
			LastRowIndex = reader.ReadUInt16();
			FirstColumnIndex = Math.Min((int)reader.ReadUInt16(), 0x00ff);
			LastColumnIndex = Math.Min((int)reader.ReadUInt16(), 0x00ff);
		}
		public void Write(BinaryWriter writer) {
			writer.Write((ushort)FirstRowIndex);
			writer.Write((ushort)LastRowIndex);
			writer.Write((ushort)FirstColumnIndex);
			writer.Write((ushort)LastColumnIndex);
		}
		public override bool Equals(object obj) {
			XlsRef8 other = obj as XlsRef8;
			if(other == null)
				return false;
			return FirstRowIndex == other.FirstRowIndex &&
				LastRowIndex == other.LastRowIndex &&
				FirstColumnIndex == other.FirstColumnIndex &&
				LastColumnIndex == other.LastColumnIndex;
		}
		public override int GetHashCode() {
			return FirstRowIndex ^ LastRowIndex ^ FirstColumnIndex ^ LastColumnIndex;
		}
		internal void Union(XlsRef8 other) {
			if(other == null)
				return;
			FirstColumnIndex = Math.Min(FirstColumnIndex, other.FirstColumnIndex);
			FirstRowIndex = Math.Min(FirstRowIndex, other.FirstRowIndex);
			LastColumnIndex = Math.Max(LastColumnIndex, other.LastColumnIndex);
			LastRowIndex = Math.Max(LastRowIndex, other.LastRowIndex);
		}
	}
	#endregion
	#region NullTerminatedUnicodeString
	public class NullTerminatedUnicodeString {
		string value = string.Empty;
		#region Properties
		public string Value {
			get { return this.value; }
			set {
				if(value == null)
					value = string.Empty;
				this.value = value;
			}
		}
		public virtual int Length { get { return this.value.Length * 2 + 2; } }
		#endregion
		public static NullTerminatedUnicodeString FromStream(XlsReader reader) {
			NullTerminatedUnicodeString result = new NullTerminatedUnicodeString();
			result.Read(reader);
			return result;
		}
		protected void Read(XlsReader reader) {
			StringBuilder builder = new StringBuilder();
			ushort ch = reader.ReadUInt16();
			while(ch != 0x0000) {
				builder.Append(Convert.ToChar(ch));
				ch = reader.ReadUInt16();
			}
			this.value = builder.ToString();
		}
		public virtual void Write(BinaryWriter writer) {
			if(!String.IsNullOrEmpty(this.value)) {
				byte[] buf = XLStringEncoder.GetEncoding(true).GetBytes(this.value);
				writer.Write(buf);
			}
			writer.Write((ushort)0); 
		}
	}
	#endregion
	#region HyperlinkString
	public class HyperlinkString : XLUnicodeCharactersArray {
		public override int Length { get { return 6 + base.Length; } }
		public static HyperlinkString FromStream(XlsReader reader) {
			HyperlinkString result = new HyperlinkString();
			result.Read(reader);
			return result;
		}
		protected void Read(XlsReader reader) {
			int charCount = reader.ReadInt32();
			if(charCount > 1)
				base.Read(reader, charCount - 1);
			if(charCount > 0)
				reader.ReadUInt16(); 
		}
		public override void Write(BinaryWriter writer) {
			int charCount = Value.Length + 1;
			writer.Write(charCount);
			base.Write(writer);
			writer.Write((ushort)0); 
		}
	}
	#endregion
	#region XNumChecker
	public static class XNumChecker {
		static readonly long negativeZeroBits = BitConverter.DoubleToInt64Bits(-0.0);
		public static void CheckValue(double value) {
			if(double.IsNaN(value))
				throw new ArgumentException("value is not-a-number");
			if(double.IsInfinity(value))
				throw new ArgumentException("value is infinity");
			if(IsNegativeZero(value))
				throw new ArgumentException("value is negative zero");
		}
		public static bool IsNegativeZero(double value) {
			return BitConverter.DoubleToInt64Bits(value) == negativeZeroBits;
		}
	}
	#endregion
	#region CompositeMemoryStream
	public class CompositeMemoryStream : Stream {
		#region Internals
		class SubstreamInfo {
			public SubstreamInfo(Stream stream, long startPosition) {
				StartPosition = startPosition;
				BaseStream = stream;
			}
			public Stream BaseStream { get; private set; }
			public long StartPosition { get; private set; }
			public long InnerPosition { get { return BaseStream.Position; } set { BaseStream.Position = value; } }
			public long Length { get { return BaseStream.Length; } }
		}
		class SubstreamComparable : IComparable<SubstreamInfo> {
			long position;
			public SubstreamComparable(long position) {
				this.position = position;
			}
			#region IComparable<SubstreamInfo> Members
			public int CompareTo(SubstreamInfo other) {
				if(position < other.StartPosition)
					return 1;
				if(position >= (other.StartPosition + other.Length))
					return -1;
				return 0;
			}
			#endregion
		}
		#endregion
		readonly List<SubstreamInfo> substreams = new List<SubstreamInfo>();
		long position;
		bool leaveOpen;
		public CompositeMemoryStream() 
			: this(null, false) {
		}
		public CompositeMemoryStream(bool leaveOpen) 
			: this(null, leaveOpen) {
		}
		public CompositeMemoryStream(Stream stream)
			: this(stream, false) {
		}
		public CompositeMemoryStream(Stream stream, bool leaveOpen) {
			this.leaveOpen = leaveOpen;
			if(stream == null)
				stream = new ChunkedMemoryStream();
			Attach(stream);
		}
		public override bool CanRead { get { return true; } }
		public override bool CanSeek { get { return true; } }
		public override bool CanWrite { get { return true; } }
		public override long Length {
			get {
				long result = 0;
				int count = substreams.Count;
				if(count > 0) {
					SubstreamInfo lastSubstream = substreams[count - 1];
					result = lastSubstream.StartPosition + lastSubstream.Length;
				}
				return result;
			}
		}
		public override long Position {
			get { return position; }
			set {
				if(value < 0)
					value = 0;
				position = value;
			}
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) {
				if(!leaveOpen) {
					foreach(SubstreamInfo item in substreams)
						item.BaseStream.Dispose();
				}
				substreams.Clear();
			}
		}
		public override void Flush() {
			foreach(SubstreamInfo item in substreams)
				item.BaseStream.Flush();
		}
		public override long Seek(long offset, SeekOrigin origin) {
			switch(origin) {
				case SeekOrigin.Begin:
					Position = offset;
					break;
				case SeekOrigin.Current:
					Position = Position + offset;
					break;
				case SeekOrigin.End:
					Position = Length + offset;
					break;
			}
			return Position;
		}
		public override int Read(byte[] buffer, int offset, int count) {
			int bytesRead = 0;
			int bytesToRead = count;
			while(bytesToRead > 0) {
				SubstreamInfo item = FindSubstream(position);
				if(item == null)
					return bytesRead;
				item.InnerPosition = position - item.StartPosition;
				long restOfSubstream = item.Length - item.InnerPosition;
				int bytesCount = (int)Math.Min(bytesToRead, restOfSubstream);
				item.BaseStream.Read(buffer, offset, bytesCount);
				position += bytesCount;
				offset += bytesCount;
				bytesRead += bytesCount;
				bytesToRead -= bytesCount;
			}
			return bytesRead;
		}
		public override void Write(byte[] buffer, int offset, int count) {
			int bytesToWrite = count;
			while(bytesToWrite > 0) {
				SubstreamInfo item = FindSubstream(position);
				if(item == null) {
					item = substreams[substreams.Count - 1];
					item.InnerPosition = position - item.StartPosition;
					item.BaseStream.Write(buffer, offset, bytesToWrite);
					position += bytesToWrite;
					return;
				}
				item.InnerPosition = position - item.StartPosition;
				long restOfSubstream = item.Length - item.InnerPosition;
				int bytesCount = (int)Math.Min(bytesToWrite, restOfSubstream);
				item.BaseStream.Write(buffer, offset, bytesCount);
				position += bytesCount;
				offset += bytesCount;
				bytesToWrite -= bytesCount;
			}
		}
		public override void SetLength(long value) {
			long length = Length;
			if(value == length)
				return;
			if(value < length) {
				int index = Algorithms.BinarySearch<SubstreamInfo>(substreams, new SubstreamComparable(position));
				int count = substreams.Count;
				for(int i = count - 1; i > index; i--) {
					if(!leaveOpen)
						substreams[i].BaseStream.Dispose();
					substreams.RemoveAt(i);
				}
			}
			SubstreamInfo lastSubstream = substreams[substreams.Count - 1];
			lastSubstream.BaseStream.SetLength(value - lastSubstream.StartPosition);
		}
		public void Attach(Stream stream) {
			Guard.ArgumentNotNull(stream, "stream");
			if(stream.CanRead != CanRead || stream.CanWrite != CanWrite || stream.CanSeek != CanSeek)
				throw new ArgumentException("Not compatible stream");
			SubstreamInfo substream = new SubstreamInfo(stream, Length);
			substreams.Add(substream);
		}
		public byte[] ToArray() {
			byte[] result = new byte[Length];
			long savedPosition = Position;
			Position = 0;
			Read(result, 0, result.Length);
			Position = savedPosition;
			return result;
		}
		#region Internals
		SubstreamInfo FindSubstream(long position) {
			int index = Algorithms.BinarySearch<SubstreamInfo>(substreams, new SubstreamComparable(position));
			if(index < 0)
				return null;
			return substreams[index];
		}
		#endregion
	}
	#endregion
	#region XlsRefU
	public class XlsRefU {
		public int FirstColumnIndex { get; set; }
		public int LastColumnIndex { get; set; }
		public int FirstRowIndex { get; set; }
		public int LastRowIndex { get; set; }
		internal int CellCount {
			get {
				return (LastColumnIndex - FirstColumnIndex + 1) * (LastRowIndex - FirstRowIndex + 1);
			}
		}
		public static XlsRefU FromStream(XlsReader reader) {
			XlsRefU result = new XlsRefU();
			result.Read(reader);
			return result;
		}
		public static XlsRefU FromRange(XlCellRange range) {
			XlsRefU result = new XlsRefU();
			result.FirstRowIndex = range.TopLeft.Row == XlCellPosition.InvalidValue.Row ? 0 : range.TopLeft.Row;
			result.LastRowIndex = range.BottomRight.Row == XlCellPosition.InvalidValue.Row ? XlsDefs.MaxRowCount - 1 : range.BottomRight.Row;
			result.FirstColumnIndex = range.TopLeft.Column == XlCellPosition.InvalidValue.Column ? 0 : range.TopLeft.Column;
			result.LastColumnIndex = range.BottomRight.Column == XlCellPosition.InvalidValue.Column ? XlsDefs.MaxColumnCount - 1 : range.BottomRight.Column;
			if(result.FirstRowIndex >= XlsDefs.MaxRowCount)
				return null;
			if(result.FirstColumnIndex >= XlsDefs.MaxColumnCount)
				return null;
			if(result.LastRowIndex >= XlsDefs.MaxRowCount)
				result.LastRowIndex = XlsDefs.MaxRowCount - 1;
			if(result.LastColumnIndex >= XlsDefs.MaxColumnCount)
				result.LastColumnIndex = XlsDefs.MaxColumnCount - 1;
			return result;
		}
		protected void Read(XlsReader reader) {
			FirstRowIndex = reader.ReadUInt16();
			LastRowIndex = reader.ReadUInt16();
			FirstColumnIndex = reader.ReadByte();
			LastColumnIndex = reader.ReadByte();
		}
		public void Write(BinaryWriter writer) {
			writer.Write((ushort)FirstRowIndex);
			writer.Write((ushort)LastRowIndex);
			writer.Write((byte)FirstColumnIndex);
			writer.Write((byte)LastColumnIndex);
		}
		public override bool Equals(object obj) {
			XlsRefU other = obj as XlsRefU;
			if(other == null)
				return false;
			return FirstRowIndex == other.FirstRowIndex &&
				LastRowIndex == other.LastRowIndex &&
				FirstColumnIndex == other.FirstColumnIndex &&
				LastColumnIndex == other.LastColumnIndex;
		}
		public override int GetHashCode() {
			return FirstRowIndex ^ LastRowIndex ^ FirstColumnIndex ^ LastColumnIndex;
		}
	}
	#endregion
	#region XlsXORObfuscation
	public class XlsXORObfuscation {
		const int size = 4;
		public short Key { get; protected set; }
		public short VerificationId { get; protected set; }
		protected XlsXORObfuscation() { }
		public XlsXORObfuscation(short key, short verificationId) {
			Key = key;
			VerificationId = verificationId;
		}
		public static XlsXORObfuscation FromStream(XlsReader reader) {
			XlsXORObfuscation result = new XlsXORObfuscation();
			result.Read(reader);
			return result;
		}
		protected void Read(XlsReader reader) {
			Key = reader.ReadNotCryptedInt16();
			VerificationId = reader.ReadNotCryptedInt16();
		}
		public void Write(BinaryWriter writer) {
			writer.Write(Key);
			writer.Write(VerificationId);
		}
		public int GetSize() {
			return size;
		}
	}
	#endregion
	#region XlsRC4EncryptionHeader
	public abstract class XlsRC4EncryptionHeaderBase {
		#region Properties
		public short VersionMinor { get; protected set; }
		public short VersionMajor { get; protected set; }
		#endregion
		public void Read(XlsReader reader) {
			VersionMajor = reader.ReadNotCryptedInt16();
			VersionMinor = reader.ReadNotCryptedInt16();
			ReadCore(reader);
		}
		public void Write(BinaryWriter writer) {
			writer.Write(VersionMajor);
			writer.Write(VersionMinor);
			WriteCore(writer);
		}
		public virtual int GetSize() {
			return 4;
		}
		protected abstract void ReadCore(XlsReader reader);
		protected abstract void WriteCore(BinaryWriter writer);
	}
	public class XlsRC4EncryptionHeader : XlsRC4EncryptionHeaderBase {
		#region Fields
		const int bytesLength = 16;
		byte[] salt = new byte[bytesLength];
		byte[] encryptedVerifier = new byte[bytesLength];
		byte[] encryptedVerifierHash = new byte[bytesLength];
		#endregion
		public XlsRC4EncryptionHeader() {
			VersionMinor = 1;
			VersionMajor = 1;
		}
		#region Properties
		public byte[] Salt {
			get { return salt; }
			set {
				Guard.ArgumentNotNull(value, "Salt value");
				if(value.Length != bytesLength)
					throw new ArgumentException("Invalid Salt value length");
				salt = value;
			}
		}
		public byte[] EncryptedVerifier {
			get { return encryptedVerifier; }
			set {
				Guard.ArgumentNotNull(value, "EncryptedVerifier value");
				if(value.Length != bytesLength)
					throw new ArgumentException("Invalid EncryptedVerifier value length");
				encryptedVerifier = value;
			}
		}
		public byte[] EncryptedVerifierHash {
			get { return encryptedVerifierHash; }
			set {
				Guard.ArgumentNotNull(value, "EncryptedVerifierHash value");
				if(value.Length != bytesLength)
					throw new ArgumentException("Invalid EncryptedVerifierHash value length");
				encryptedVerifierHash = value;
			}
		}
		#endregion
		protected override void ReadCore(XlsReader reader) {
			this.salt = reader.ReadNotCryptedBytes(bytesLength);
			this.encryptedVerifier = reader.ReadNotCryptedBytes(bytesLength);
			this.encryptedVerifierHash = reader.ReadNotCryptedBytes(bytesLength);
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write(this.salt);
			writer.Write(this.encryptedVerifier);
			writer.Write(this.encryptedVerifierHash);
		}
		public override int GetSize() {
			return base.GetSize() + bytesLength * 3;
		}
	}
	public class XlsRC4CryptoAPIEncryptionHeader : XlsRC4EncryptionHeaderBase {
		public XlsRC4CryptoAPIEncryptionHeader() {
			VersionMajor = 4; 
			VersionMinor = 2;
		}
		#region Properties
		public bool CryptoAPI { get; set; }
		public bool DocumentPropertiesEncrypted { get; set; }
		public bool ExternalEncryption { get; set; }
		public bool AES { get; set; }
		public int AlgorithmId { get; set; }
		public int AlgorithmIdHash { get; set; }
		public int KeySize { get; set; }
		public int ProviderType { get; set; }
		public string CSPName { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader) {
			reader.ReadInt32(); 
			int headerSize = reader.ReadNotCryptedInt32();
			int bitwiseField = reader.ReadNotCryptedInt32();
			CryptoAPI = Convert.ToBoolean(bitwiseField & 0x0004);
			DocumentPropertiesEncrypted = Convert.ToBoolean(bitwiseField & 0x0008);
			ExternalEncryption = Convert.ToBoolean(bitwiseField & 0x0010);
			AES = Convert.ToBoolean(bitwiseField & 0x0020);
			int sizeExtra = reader.ReadNotCryptedInt32();
			if(sizeExtra != 0)
				throw new Exception("Invalid Xls file : XlsRC4CryptoAPIEncryptionHeader.ReadCore: sizeExtra != 0");
			AlgorithmId = reader.ReadNotCryptedInt32();
			AlgorithmIdHash = reader.ReadNotCryptedInt32();
			KeySize = reader.ReadNotCryptedInt32();
			ProviderType = reader.ReadNotCryptedInt32();
			reader.ReadNotCryptedInt32(); 
			reader.ReadNotCryptedInt32(); 
			int bytesToRead = headerSize - 32;
			if(bytesToRead > 0) {
				byte[] buf = reader.ReadNotCryptedBytes(bytesToRead);
				CSPName = Encoding.Unicode.GetString(buf, 0, buf.Length - 2);
			}
		}
		protected override void WriteCore(BinaryWriter writer) {
			int bitwiseField = 0;
			if(CryptoAPI)
				bitwiseField |= 0x0004;
			if(DocumentPropertiesEncrypted)
				bitwiseField |= 0x0008;
			if(ExternalEncryption)
				bitwiseField |= 0x0010;
			if(AES)
				bitwiseField |= 0x0020;
			writer.Write(bitwiseField); 
			writer.Write(GetHeaderSize());
			writer.Write(bitwiseField); 
			writer.Write((int)0); 
			writer.Write(AlgorithmId);
			writer.Write(AlgorithmIdHash);
			writer.Write(KeySize);
			writer.Write(ProviderType);
			writer.Write((int)0); 
			writer.Write((int)0); 
			if(!string.IsNullOrEmpty(CSPName)) {
				byte[] buf = Encoding.Unicode.GetBytes(CSPName + (char)0);
				writer.Write(buf);
			}
		}
		public override int GetSize() {
			return base.GetSize() + GetHeaderSize() + 8;
		}
		int GetHeaderSize() {
			if(string.IsNullOrEmpty(CSPName)) return 32;
			return 32 + (CSPName.Length + 1) * 2;
		}
	}
	#endregion
}
