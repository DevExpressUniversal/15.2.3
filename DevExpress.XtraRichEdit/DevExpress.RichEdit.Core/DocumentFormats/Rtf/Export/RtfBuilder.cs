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
using System.IO;
using System.Text;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Export.Rtf {
	#region RtfBuilder
	public class RtfBuilder {
		#region Fields
		static readonly Dictionary<char, string> specialMarks;
		static readonly Dictionary<char, string> specialPCDataMarks;
		internal readonly static string[] byteToHexString;
		bool hexMode;
		readonly ChunkedStringBuilder rtfContent;
		bool isPreviousWriteCommand;
		readonly StringBuilder unicodeTextBuilder;
		int rowLength;
		readonly Encoding encoding;
		#endregion
		static RtfBuilder() {
			int count = byte.MaxValue;
			count += 1;
			byteToHexString = new string[256];
			for (int i = 0; i < count; i++)
				byteToHexString[i] = String.Format("{0:x2}", i);
			specialMarks = CreateSpecialMarksTable();
			specialPCDataMarks = CreateSpecialPCDataMarksTable();
		}
		#region CreateSpecialMarksTable
		static Dictionary<char, string> CreateSpecialMarksTable() {
			Dictionary<char, string> result = new Dictionary<char, string>();
			result.Add(Characters.EmSpace, @"\u8195\'3f");
			result.Add(Characters.EnSpace, @"\u8194\'3f");
			result.Add(Characters.Hyphen, String.Empty);
			result.Add(Characters.LineBreak, @"\line ");
			result.Add(Characters.PageBreak, @"\page ");
			result.Add(Characters.ColumnBreak, @"\column ");
			result.Add(Characters.NonBreakingSpace, @"\~");
			result.Add(Characters.QmSpace, @"\u8197\'3f");
			result.Add(Characters.TabMark, @"\tab ");
			return result;
		}
		#endregion        
		#region CreateSpecialMarksTable
		static Dictionary<char, string> CreateSpecialPCDataMarksTable() {
			Dictionary<char, string> result = new Dictionary<char,string>();
			for (int i = 0; i < 31; i++ ) {
				AddHexToMarkTable(result, (char)i);
			}
			AddHexToMarkTable(result, '\\');
			AddHexToMarkTable(result, '{');
			AddHexToMarkTable(result, '}');
			return result;
		}
		static void AddHexToMarkTable(Dictionary<char, string> table, char ch) {
			table.Add(ch, String.Format(@"\'{0}", byteToHexString[(int)ch]));
		}
		#endregion
		public RtfBuilder(Encoding encoding) {
			Guard.ArgumentNotNull(encoding, "encoding");
			this.encoding = encoding;
			this.rtfContent = new ChunkedStringBuilder();
			isPreviousWriteCommand = false;
			this.unicodeTextBuilder = new StringBuilder();
			rowLength = 0;
		}
		#region Properties
		public ChunkedStringBuilder RtfContent { get { return rtfContent; } }
		public virtual int RowLengthBound { get { return 200; } }
		protected Encoding Encoding { get { return encoding; } }
		protected StringBuilder UnicodeTextBuilder { get { return unicodeTextBuilder; } }
		#endregion
		public void Clear() {
			rtfContent.Clear();
		}
		void IncreaseRowLength(int delta) {
			rowLength += delta;
			if (rowLength >= RowLengthBound && hexMode) {
				rtfContent.Append(RtfExportSR.CLRF);
				isPreviousWriteCommand = false;
				rowLength = 0;
			}
		}
		public void WriteCommand(string command) {
			rtfContent.Append(command);
			isPreviousWriteCommand = true;
			IncreaseRowLength(command.Length);
		}
		public void WriteCommand(string command, int param) {
			rtfContent.Append(command);
			rtfContent.Append(param);
			isPreviousWriteCommand = true;
			IncreaseRowLength(command.Length);
		}
		public void WriteCommand(string command, string param) {
			rtfContent.Append(command);
			rtfContent.Append(param);
			isPreviousWriteCommand = true;
			IncreaseRowLength(command.Length);
		}
		public void WriteText(string text) {
			WriteTextCore(text, specialMarks);
		}
		public void WritePCData(string text) {
			WriteTextCore(text, specialPCDataMarks);
		}
		void WriteTextCore(string text, Dictionary<char, string> specialMarks) {
			int count = text.Length;
			for (int i = 0; i < count; i++) {
				char ch = text[i];
				string specialMark;
				if (specialMarks.TryGetValue(ch, out specialMark))
					WriteTextDirect(specialMark);
				else
					WriteChar(ch);
			}
		}
		public void WriteTextDirect(string text, bool makeStringUnicodeCompatible) {
			if (isPreviousWriteCommand)
				rtfContent.Append(RtfExportSR.Space);
			if(makeStringUnicodeCompatible)
				rtfContent.Append(GetUnicodeCompatibleStringDirect(text));
			else
				rtfContent.Append(text);
			isPreviousWriteCommand = false;
			IncreaseRowLength(text.Length);
		}
		public void WriteTextDirect(string text) {
			WriteTextDirect(text, false);
		}
		protected internal void WriteTextDirectUnsafe(ChunkedStringBuilder text) {
			if (isPreviousWriteCommand)
				rtfContent.Append(RtfExportSR.Space);
			int textLength = text.Length;
			rtfContent.AppendExistingBuffersUnsafe(text);
			isPreviousWriteCommand = false;
			IncreaseRowLength(textLength);
		}
		public void WriteChar(char ch) {
			if (isPreviousWriteCommand)
				rtfContent.Append(RtfExportSR.Space);
			string str = GetUnicodeCompatibleString(ch);
			rtfContent.Append(str);
			isPreviousWriteCommand = false;
			IncreaseRowLength(str.Length);
		}
		public void OpenGroup() {
			rtfContent.Append(RtfExportSR.OpenGroup);
			isPreviousWriteCommand = false;
			IncreaseRowLength(RtfExportSR.OpenGroup.Length);
		}
		public void CloseGroup() {
			rtfContent.Append(RtfExportSR.CloseGroup);
			isPreviousWriteCommand = false;
			IncreaseRowLength(RtfExportSR.CloseGroup.Length);
		}
		internal static bool IsSpecialSymbol(char ch) {
			return ch == '{' || ch == '}' || ch == '\\';
		}
		protected virtual string GetUnicodeCompatibleString(char ch) {
			unicodeTextBuilder.Length = 0;
			int code = (short)ch;
			if (code >= 0 && code <= 127) {
				if (IsSpecialSymbol(ch))
					unicodeTextBuilder.Append(@"\");
				unicodeTextBuilder.Append(ch);
			}
			else
				AppendUnicodeCompatibleCharCore(code, ch);
			return unicodeTextBuilder.ToString();
		}
		protected virtual string GetUnicodeCompatibleStringDirect(string text) {
			unicodeTextBuilder.Length = 0;
			int length = text.Length;
			for (int i = 0; i < length; i++) {
				char ch = text[i];
				int code = (short)ch;
				if (code >= 0 && code <= 127)
					unicodeTextBuilder.Append(ch);
				else
					AppendUnicodeCompatibleCharCore(code, ch);
			}
			return unicodeTextBuilder.ToString();
		}
		protected virtual void AppendUnicodeCompatibleCharCore(int code, char ch) {
			byte[] bytes = Encoding.GetBytes(new char[] { ch });
			unicodeTextBuilder.AppendFormat(@"\u{0}\'{1}", code, byteToHexString[bytes[0]]);
		}
		public void WriteHex(byte val) {
			if (!hexMode)
				Exceptions.ThrowInternalException();
			rtfContent.Append(byteToHexString[val]);
		}
		public void WriteHex(int val) {
			if (!hexMode)
				Exceptions.ThrowInternalException();
			rtfContent.Append(byteToHexString[val & 0xFF]);
			rtfContent.Append(byteToHexString[(val >> 8) & 0xFF]);
			rtfContent.Append(byteToHexString[(val >> 16) & 0xFF]);
			rtfContent.Append(byteToHexString[val >> 24]);
		}
		public void WriteStreamAsHex(Stream stream) {
			ChunkedMemoryStream chunkedMemoryStream = stream as ChunkedMemoryStream;
			if (chunkedMemoryStream != null) {
				WriteChunkedMemoryStreamAsHex(chunkedMemoryStream);
				return;
			}
			MemoryStream memoryStream = stream as MemoryStream;
			if (memoryStream != null) {
				WriteMemoryStreamAsHex(memoryStream);
				return;
			}
			WriteStreamAsHexCore(stream);
		}
		void WriteChunkedMemoryStreamAsHex(ChunkedMemoryStream stream) {
			IList<byte[]> buffers = stream.GetBuffers();
			int maxBufferSize = stream.MaxBufferSize;
			int count = buffers.Count - 1;
			for (int i = 0; i < count; i++) {
				WriteByteArrayAsHex(buffers[i], 0, maxBufferSize);
				isPreviousWriteCommand = false;
			}
			int bytesLeft = (int)(stream.Length % maxBufferSize);
			if (bytesLeft > 0)
				WriteByteArrayAsHex(buffers[count], 0, bytesLeft);
		}
		void WriteMemoryStreamAsHex(MemoryStream stream) {
			byte[] buffer = TryGetMemoryStreamBuffer(stream);
			if (buffer != null)
				WriteByteArrayAsHex(buffer, 0, (int)stream.Length);
			else
				WriteStreamAsHexCore(stream);
		}
		byte[] TryGetMemoryStreamBuffer(MemoryStream stream) {
			try {
				return stream.GetBuffer();
			}
			catch {
				return null;
			}
		}
		public void WriteByteArrayAsHex(byte[] array) {
			WriteByteArrayAsHex(array, 0, array.Length);
		}
		public void WriteByteArrayAsHex(byte[] array, int offset, int length) {
			if (isPreviousWriteCommand)
				rtfContent.Append(RtfExportSR.Space);
			hexMode = true;
			int count = offset + length;
			for (int i = offset; i < count; i++) {
				byte val = array[i];
				IncreaseRowLength(2);
				rtfContent.Append(byteToHexString[val]);
			}
			isPreviousWriteCommand = true;
			hexMode = false;
		}
		void WriteStreamAsHexCore(Stream stream) {
			byte[] buffer = new byte[4096];
			int bufferLength = buffer.Length;
			long length = stream.Length - stream.Position;
			while (length >= bufferLength) {
				stream.Read(buffer, 0, bufferLength);
				length -= bufferLength;
				WriteByteArrayAsHex(buffer);
				this.isPreviousWriteCommand = false;
			}
			if (length > 0) {
				stream.Read(buffer, 0, (int)length);
				WriteByteArrayAsHex(buffer, 0, (int)length);
			}
			this.isPreviousWriteCommand = true;
		}
		public void WriteShapeProperty(string propertyName, string propertyValue) {
			OpenGroup();
			try {
				WriteCommand(RtfExportSR.ShapeProperty);
				WriteShapePropertyName(propertyName);
				WriteShapePropertyValue(propertyValue);
			}
			finally {
				CloseGroup();
			}
		}
		public void WriteShapeIntegerProperty(string propertyName, int propertyValue) {
			WriteShapeProperty(propertyName, propertyValue.ToString());
		}
		public void WriteShapeBoolProperty(string propertyName, bool propertyValue) {
			WriteShapeProperty(propertyName, GetBoolParameterValue(propertyValue));
		}
		string GetBoolParameterValue(bool value) {
			return ((value == true) ? 1 : 0).ToString();
		}
		public void WriteShapeColorProperty(string propertyName, Color propertyValue) {
			string color = (propertyValue.R | (propertyValue.G << 8) | (propertyValue.B << 16)).ToString();
			WriteShapeProperty(propertyName, color);
		}
		public void WriteShapePropertyName(string propertyName) {
			OpenGroup();
			try {
				WriteCommand(RtfExportSR.ShapePropertyName);
				WriteTextDirect(propertyName);
			}
			finally {
				CloseGroup();
			}
		}
		public void WriteShapePropertyValue(string propertyValue) {
			OpenGroup();
			try {
				WriteCommand(RtfExportSR.ShapePropertyValue);
				WriteTextDirect(propertyValue);
			}
			finally {
				CloseGroup();
			}
		}
	}
	#endregion
	#region DBCSRtfBuilder
	public class DBCSRtfBuilder : RtfBuilder {
		public DBCSRtfBuilder(Encoding encoding)
			: base(encoding) {
#if !SL
			if (Encoding.IsSingleByte)
				Exceptions.ThrowArgumentException("encoding", encoding);
#endif
		}
		protected override void AppendUnicodeCompatibleCharCore(int code, char ch) {
			if (Encoding.GetByteCount(new char[] { ch }) > 1) {
				UnicodeTextBuilder.AppendFormat(@"\u{0}?", code);
			}
			else
				base.AppendUnicodeCompatibleCharCore(code, ch);
		}
	}
	#endregion
}
