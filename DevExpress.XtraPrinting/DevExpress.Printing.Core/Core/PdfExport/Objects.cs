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
using System.Drawing;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using DevExpress.Utils;
using DevExpress.Utils.Zip;
using DevExpress.Pdf.Common;
namespace DevExpress.XtraPrinting.Export.Pdf {
	public enum PdfObjectType {
		Direct,
		Indirect
	}
	public class PdfIndirectReference {
		public const int Generation = 0;
		int number;
		long byteOffset = -1;
		public int Number { get { return number; } set { number = value; } }
		public long ByteOffset { get { return byteOffset; } }
		public void CalculateByteOffset(StreamWriter writer) {
			writer.Flush();
			this.byteOffset = writer.BaseStream.Position;
		}
		public void WriteToStream(StreamWriter writer) {
			writer.Write("{0} {1} R", this.number, Generation);
		}
	}
	public abstract class PdfObject {
		PdfObject owner;
		PdfObjectType type = PdfObjectType.Direct;
		PdfIndirectReference indirectReference;
		public PdfObjectType Type { get { return type; } }
		public PdfIndirectReference IndirectReference { get { return indirectReference; } }
		public PdfIndirectReference ChainingIndirectReference {
			get {
				if(IndirectReference != null)
					return IndirectReference;
				if(Owner == null)
					return null;
				return Owner.ChainingIndirectReference;
			}
		}
		public PdfObject Owner {
			get { return owner; }
			set { owner = value; }
		}
		protected PdfObject()
			: this(PdfObjectType.Direct) {
		}
		protected PdfObject(PdfObjectType type) {
			this.type = type;
			if(type == PdfObjectType.Indirect)
				this.indirectReference = new PdfIndirectReference();
		}
		protected abstract void WriteContent(StreamWriter writer);
		public void WriteToStream(StreamWriter writer) {
			if(this.indirectReference != null)
				this.indirectReference.WriteToStream(writer);
			else
				WriteContent(writer);
		}
		public void WriteIndirect(StreamWriter writer) {
			if(this.indirectReference == null)
				throw new PdfException("Can't write direct object as indirect");
			this.indirectReference.CalculateByteOffset(writer);
			writer.WriteLine("{0} {1} obj", this.indirectReference.Number, PdfIndirectReference.Generation);
			WriteContent(writer);
			writer.WriteLine();
			writer.WriteLine("endobj");
		}
	}
	public class PdfNull : PdfObject {
		public PdfNull()
			: base() {
		}
		protected override void WriteContent(StreamWriter writer) {
			writer.Write("null");
		}
	}
	public class PdfBoolean : PdfObject {
		bool value;
		public bool Value { get { return value; } }
		public PdfBoolean(bool value)
			: base() {
			this.value = value;
		}
		protected override void WriteContent(StreamWriter writer) {
			writer.Write((value) ? "true" : "false");
		}
	}
	public class PdfNumber : PdfObject {
		int value;
		public int Value { get { return value; } }
		public PdfNumber(int value)
			: base() {
			this.value = value;
		}
		protected override void WriteContent(StreamWriter writer) {
			writer.Write(value);
		}
	}
	public class PdfDouble : PdfObject {
		double value;
		public double Value { get { return value; } }
		public PdfDouble(double value)
			: base() {
			this.value = value;
		}
		protected override void WriteContent(StreamWriter writer) {
			writer.Write(Utils.ToString(value));
		}
	}
#if DEBUGTEST && !SL
	[System.Diagnostics.DebuggerDisplay(@"\{{GetType().FullName,nq}, {Value}}")]
#endif
	public class PdfName : PdfObject {
		const string escape = "%()<>[]{}/#";
		public static bool IsEscapeChar(char ch) {
			return escape.IndexOf(ch) >= 0;
		}
		string value;
		public string Value { get { return value; } }
		public PdfName(string value)
			: base() {
			this.value = value;
		}
		string EscapeName(string name) {
			StringBuilder result = new StringBuilder();
			for(int i = 0; i < name.Length; i++) {
				if(IsEscapeChar(name[i]) || name[i] < '!' || name[i] > '~') {
					result.Append("#");
					result.Append(PdfStringUtils.HexCharAsByte(name[i]));
				}
				else
					result.Append(name[i]);
			}
			return result.ToString();
		}
		protected override void WriteContent(StreamWriter writer) {
			writer.Write("/" + EscapeName(value));
		}
	}
	public class PdfLiteralString : PdfObject {
		string value;
		bool encryption;
		public PdfLiteralString(string value, bool encryption)
			: base() {
			this.value = value;
			this.encryption = encryption;
		}
		public PdfLiteralString(string value)
			: this(value, true) {
		}
		public string Value { get { return value; } }
		protected override void WriteContent(StreamWriter writer) {
			writer.Flush();
			PdfStreamWriter pdfWriter = writer as PdfStreamWriter;
			string str = encryption && pdfWriter != null ? pdfWriter.EncryptString(value, this) : value;
			string escapedString = PdfStringUtils.EscapeString(str);
			string result = String.Format("({0})", escapedString);
			byte[] bytes = PdfStringUtils.GetIsoBytes(result);
			writer.BaseStream.Write(bytes, 0, bytes.Length);
			writer.BaseStream.Flush();
		}
	}
	public class PdfHexadecimalString : PdfObject {
		byte[] value;
		public PdfHexadecimalString(byte[] value)
			: base() {
			this.value = value;
		}
		protected void SetValue(byte[] value) {
			this.value = value;
		}
		protected override void WriteContent(StreamWriter writer) {
			writer.Flush();
			string str = PdfStringUtils.ArrayToHexadecimalString(value);
			string result = String.Format("<{0}>", str);
			byte[] bytes = PdfStringUtils.GetIsoBytes(result);
			writer.BaseStream.Write(bytes, 0, bytes.Length);
			writer.BaseStream.Flush();
		}
	}
	public class PdfTextUnicode : PdfObject {
		string value;
		public string Value { get { return value; } }
		public PdfTextUnicode(string value)
			: base() {
			this.value = value;
		}
		byte[] GetBytes(char ch) {
			byte[] buffer = BitConverter.GetBytes(ch);
			if(BitConverter.IsLittleEndian)
				Array.Reverse(buffer);
			List<byte> resultList = new List<byte>(4);
			resultList.AddRange(buffer);
			return resultList.ToArray();
		}
		protected override void WriteContent(StreamWriter writer) {
			MemoryStream stream = new MemoryStream();
			stream.WriteByte(0xFE);
			stream.WriteByte(0xFF);
			foreach(char ch in value) {
				byte[] buffer = GetBytes(ch);
				stream.Write(buffer, 0, buffer.Length);
			}
			PdfStreamWriter pdfWriter = writer as PdfStreamWriter;
			stream = pdfWriter == null ? stream : pdfWriter.EncryptStream(stream, this);
			string str = PdfStringUtils.GetIsoString(stream.ToArray());
			str = PdfStringUtils.EscapeString(str);
			stream = new MemoryStream(PdfStringUtils.GetIsoBytes(str));
			writer.Write("(");
			writer.Flush();
			stream.WriteTo(writer.BaseStream);
			writer.Write(")");
		}
	}
	public class PdfArray : PdfObject, IEnumerable {
		ArrayList array = new ArrayList();
		int maxRowCount = -1;
		public int MaxRowCount { get { return maxRowCount; } set { maxRowCount = value; } }
		public int Count { get { return array.Count; } }
		public PdfObject this[int index] { get { return array[index] as PdfObject; } }
		public PdfArray()
			: base() {
		}
		public PdfArray(PdfObjectType type)
			: base(type) {
		}
		protected override void WriteContent(StreamWriter writer) {
			writer.Write("[");
			for(int i = 0; i < Count; i++) {
				this[i].WriteToStream(writer);
				if(maxRowCount > 0) {
					if(((i + 1) % maxRowCount) == 0)
						writer.WriteLine();
				}
				if(i < Count - 1)
					writer.Write(" ");
			}
			writer.Write("]");
		}
		public int IndexOf(PdfObject obj) {
			if(obj == null) return -1;
			for(int i = 0; i < Count; i++)
				if(this[i] == obj) return i;
			return -1;
		}
		public void Add(PdfObject obj) {
			if(obj == null) return;
			if(IndexOf(obj) >= 0) return;
			array.Add(obj);
			obj.Owner = this;
		}
		public void Add(string name) {
			Add(new PdfName(name));
		}
		public void Add(int number) {
			Add(new PdfNumber(number));
		}
		public void Add(bool value) {
			Add(new PdfBoolean(value));
		}
		public void Add(float value) {
			Add(new PdfDouble(value));
		}
		public void AddRange(int[] numbers) {
			foreach(int number in numbers)
				Add(number);
		}
		public void Clear() {
			array.Clear();
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return array.GetEnumerator();			
		}
	}
	public class PdfDictionary : PdfObject {
		#region inner classes
		class Entry {
			public readonly PdfName Name;
			public readonly PdfObject Value;
			public Entry(string name, PdfObject value) {
				this.Name = new PdfName(name);
				this.Value = value;
			}
		}
		#endregion
		ArrayList list = new ArrayList();
		public PdfDictionary()
			: base() {
		}
		public PdfDictionary(PdfObjectType type)
			: base(type) {
		}
		protected override void WriteContent(StreamWriter writer) {
			writer.Write("<< ");
			if(this.list.Count > 0)
				writer.WriteLine();
			foreach(Entry entry in this.list) {
				entry.Name.WriteToStream(writer);
				writer.Write(" ");
				entry.Value.WriteToStream(writer);
				writer.WriteLine();
			}
			writer.Write(">>");
		}
		public void Add(string name, PdfObject value) {
			this.list.Add(new Entry(name, value));
			value.Owner = this;
		}
		public void Add(string name, string value) {
			Add(name, new PdfName(value));
		}
		public void Add(string name, int value) {
			Add(name, new PdfNumber(value));
		}
		public void AddIfNotNull(string name, PdfObject value) {
			if(value != null)
				Add(name, value);
		}
	}
	public class PdfStream : PdfObject {
		PdfDictionary attributes = new PdfDictionary();
		Stream data;
		StreamWriter writer;
		bool useLength1;
		protected StreamWriter Writer { get { return writer; } }
		public PdfDictionary Attributes { get { return attributes; } }
		public Stream Data {
			get {
				Writer.Flush();
				return data;
			}
		}
		public PdfStream()
			: this(false) {
		}
		public PdfStream(bool useLength1)
			: base(PdfObjectType.Indirect) {
			this.useLength1 = useLength1;
			this.data = CreateStream();
			this.writer = new StreamWriter(data, DXEncoding.GetEncoding(1252));
			this.writer.NewLine = "\n";
		}
		protected virtual Stream CreateStream() {
			return new MemoryStream();
		}
		protected virtual void FillAttributes(MemoryStream actualData) {
			this.attributes.Add("Length", (int)actualData.Length);
			if(this.useLength1)
				Attributes.Add("Length1", (int)Data.Length);
		}
		protected override void WriteContent(StreamWriter writer) {
			Writer.Flush();
			WriteStream(writer, (MemoryStream)this.data);
		}
		protected void WriteStream(StreamWriter writer, MemoryStream actualData) {
			PdfStreamWriter pdfWriter = writer as PdfStreamWriter;
			actualData = pdfWriter == null ? actualData : pdfWriter.EncryptStream(actualData, this);
			FillAttributes(actualData);
			attributes.WriteToStream(writer);
			writer.WriteLine();
			writer.WriteLine("stream");
			writer.Flush();
			actualData.WriteTo(writer.BaseStream);
			writer.WriteLine();
			writer.Write("endstream");
		}
		public void SetString(string string_) {
			Writer.Write(string_);
		}
		public void SetStringLine(string string_) {
			Writer.WriteLine(string_);
		}
		public void SetByte(byte byte_) {
			data.WriteByte(byte_);
		}
		public void SetBytes(byte[] bytes) {
			SetBytes(bytes, 0);
		}
		public void SetBytes(byte[] bytes, int startIndex) {
			SetBytes(bytes, startIndex, bytes.Length - startIndex);
		}
		public void SetBytes(byte[] bytes, int startIndex, int length) {
			Writer.Flush();
			data.Write(bytes, startIndex, length);
		}
		public void Close() {
			if(this.data != null) {
				this.writer.Dispose();
				this.writer = null;
				this.data.Dispose();
				this.data = null;
			}
		}
	}
	public class PdfFlateStream : PdfStream {
		public PdfFlateStream()
			: base() {
		}
		public PdfFlateStream(bool useLength1)
			: base(useLength1) {
		}
		protected override void FillAttributes(MemoryStream actualData) {
			base.FillAttributes(actualData);
			Attributes.Add("Filter", "FlateDecode");
		}
		protected override void WriteContent(StreamWriter writer) {
			MemoryStream actualData = DevExpress.XtraPrinting.Export.Pdf.Compression.Deflater.DeflateStream((MemoryStream)Data);
			try {
				WriteStream(writer, actualData);
			}
			finally {
				actualData.Close();
			}
		}
	}
	public class PdfDirectFlateStream : PdfStream {
		public PdfDirectFlateStream()
			: base() {
		}
		class ZLibStream : DeflateStream {
			Adler32 adler;
			public ZLibStream()
				: base(new MemoryStream(), CompressionMode.Compress, true) {
				BaseStream.WriteByte(0x58);
				BaseStream.WriteByte(0x85);
				adler = new Adler32();
			}
			public override void Write(byte[] array, int offset, int count) {
				base.Write(array, offset, count);
				adler.Calculate(array, offset, count);
			}
			protected override void Dispose(bool disposing) {
				Stream baseStream = BaseStream;
				base.Dispose(disposing);
				if(baseStream != null) {
					baseStream.WriteByte((byte)(adler.Checksum >> 24));
					baseStream.WriteByte((byte)(adler.Checksum >> 16));
					baseStream.WriteByte((byte)((adler.Checksum & 0xFFFF) >> 8));
					baseStream.WriteByte((byte)(adler.Checksum & 0xFFFF));
				}
			}
		}
		protected override Stream CreateStream() {
			return new ZLibStream();
		}
		protected override void FillAttributes(MemoryStream actualData) {
			base.FillAttributes(actualData);
			Attributes.Add("Filter", "FlateDecode");
		}
		protected override void WriteContent(StreamWriter writer) {
			DeflateStream stream = (DeflateStream)Data;
			MemoryStream actualData = (MemoryStream)stream.BaseStream;
			Writer.Close();
			stream.Close();
			try {
				WriteStream(writer, actualData.Length > 6 ? actualData : new MemoryStream());
			}
			finally {
				actualData.Close();
			}
		}
	}
	public class PdfRectangle : PdfObject {
		float left;
		float bottom;
		float right;
		float top;
		public float Left {
			get { return left; }
			set {
				if(value == left || value < 0) return;
				left = value;
			}
		}
		public float Bottom {
			get { return bottom; }
			set {
				if(value == bottom || value < 0) return;
				bottom = value;
			}
		}
		public float Right {
			get { return right; }
			set {
				if(value == right || value < 0) return;
				right = value;
			}
		}
		public float Top {
			get { return top; }
			set {
				if(value == top || value < 0) return;
				top = value;
			}
		}
		public SizeF Size { get { return new SizeF(right - left, top - bottom); } }
		public PdfRectangle()
			: this(0f, 0f, 0f, 0f) {
		}
		public PdfRectangle(float left, float bottom, float right, float top)
			: base() {
			this.left = left;
			this.bottom = bottom;
			this.right = right;
			this.top = top;
		}
		PdfArray GetArray() {
			PdfArray array = new PdfArray();
			array.Add(new PdfDouble(left));
			array.Add(new PdfDouble(bottom));
			array.Add(new PdfDouble(right));
			array.Add(new PdfDouble(top));
			return array;
		}
		protected override void WriteContent(StreamWriter writer) {
			PdfArray array = GetArray();
			array.WriteToStream(writer);
		}
	}
	public class PdfDate : PdfLiteralString {
		readonly DateTime dateTime = DateTime.MinValue;
		public DateTime DateTime { get { return dateTime; } }
		public PdfDate(DateTime dateTime)
			: base(GetStringValue(dateTime)) {
			this.dateTime = dateTime;
		}
		static string GetStringValue(DateTime dateTime) {
			StringBuilder sb = new StringBuilder();
			sb.Append("D:");
			sb.AppendFormat("{0:D4}", dateTime.Year);
			sb.AppendFormat("{0:D2}", dateTime.Month);
			sb.AppendFormat("{0:D2}", dateTime.Day);
			sb.AppendFormat("{0:D2}", dateTime.Hour);
			sb.AppendFormat("{0:D2}", dateTime.Minute);
			sb.AppendFormat("{0:D2}", dateTime.Second);
			AppendGMTOffset(sb, dateTime);
			return sb.ToString();
		}
		static void AppendGMTOffset(StringBuilder sb, DateTime dateTime) {
			TimeSpan offset = TimeZone.CurrentTimeZone.GetUtcOffset(dateTime);
			if(offset.Ticks == 0) {
				sb.Append("Z");
				return;
			}
			if(offset.Ticks > 0)
				sb.Append("+");
			sb.Append(offset.Hours.ToString("D2"));
			sb.Append("'");
			sb.Append(offset.Minutes.ToString("D2"));
			sb.Append("'");
		}
	}
	public class PdfDestination : PdfObject {
		const string destType = "XYZ";
		PdfArray array;
		public PdfDestination(PdfPage page, float top)
			: base() {
			array = new PdfArray();
			array.Add(page.InnerObject);
			array.Add(destType);
			array.Add(new PdfNull());
			array.Add(new PdfDouble(top));
			array.Add(new PdfNull());
		}
		protected override void WriteContent(StreamWriter writer) {
			array.WriteToStream(writer);
		}
	}
	public class DestinationInfo {
		int destPageIndex = -1;
		float destTop;
		RectangleF linkArea = Rectangle.Empty;
		PdfPage linkPage;
		public int DestPageIndex {
			get { return destPageIndex; }
		}
		public float DestTop {
			get { return destTop; }
			set { destTop = value; }
		}
		public PdfPage LinkPage {
			get { return linkPage; }
		}
		public RectangleF LinkArea {
			get { return linkArea; }
			set { linkArea = value; }
		}
		public DestinationInfo(int destPageIndex, float destTop) {
			Initialize(destPageIndex, destTop, null, RectangleF.Empty);
		}
		public DestinationInfo(int destPageIndex, float destTop, PdfPage linkPage, RectangleF linkArea) {
			Initialize(destPageIndex, destTop, linkPage, linkArea);
		}
		private void Initialize(int destPageIndex, float destTop, PdfPage linkPage, RectangleF linkArea) {
			this.destPageIndex = destPageIndex;
			this.destTop = destTop;
			this.linkPage = linkPage;
			this.linkArea = linkArea;
		}
	}
	public class Utils {
		public static string ToString(double value) {
			return Format(Round(value));
		}
		public static string Format(double value) {
			return value.ToString("G", NumberFormatInfo.InvariantInfo);
		}
		public static double Round(double value) {
			return Math.Round(value, 3);
		}
		public static PdfRectangle ToPdfRectangle(RectangleF bounds) {
			return new PdfRectangle(bounds.X, bounds.Y, bounds.Right, bounds.Bottom);
		}
	}
	public class PdfException : Exception {
		public PdfException() {
		}
		public PdfException(string message)
			: base(message) {
		}
		public PdfException(string message, Exception innerEx)
			: base(message, innerEx) {
		}
	}
}
