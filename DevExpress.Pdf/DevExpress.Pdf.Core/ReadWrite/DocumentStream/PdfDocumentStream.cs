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

using System.Collections.Generic;
using System.IO;
using System.Text;
namespace DevExpress.Pdf.Native {
	public class PdfDocumentStream : PdfDocumentWritableStream {
		const string unicodeStringStart = "FEFF";
		static readonly PdfTokenDescription checkObjToken = new PdfTokenDescription(new byte[] { (byte)' ', (byte)'o', (byte)'b', (byte)'j' });
		static readonly PdfTokenDescription endobjToken = new PdfTokenDescription(new byte[] { (byte)'e', (byte)'n', (byte)'d', (byte)'o', (byte)'b', (byte)'j' });
		static readonly PdfTokenDescription objToken = new PdfTokenDescription(new byte[] { (byte)'o', (byte)'b', (byte)'j' });
		public static string ReadString(Stream stream) {
			StringBuilder sb = new StringBuilder();
			for (; ; ) {
				int symbol = stream.ReadByte();
				switch (symbol) {
					case -1:
					case PdfDocumentReader.LineFeed:
					case PdfDocumentReader.CarriageReturn:
						return sb.ToString();
					default:
						sb.Append((char)symbol);
						break;
				}
			}
		}
		public static byte[] ConvertToArray(string str) {
			if (str == null)
				return null;
			int length = str.Length;
			byte[] result = new byte[length];
			for (int i = 0; i < length; i++)
				result[i] = (byte)str[i];
			return result;
		}
		readonly long startPosition = 0;
		PdfSignatureByteRange signatureByteRange;
		PdfSignatureContents signatureContents;
		long streamLength;
		public long Length { get { return streamLength; } }
		public long Position {
			get { return Stream.Position - startPosition; }
			set { Stream.Position = value + startPosition; }
		}
		public PdfDocumentStream(Stream stream, bool readable) :base(stream) {
			if (readable) {
				streamLength = stream.Length;
				startPosition = stream.Position;
			}
		}
		public PdfDocumentStream(Stream stream)
			: this(stream, true) {
		}
		public void Reset() {
			Stream.SetLength(1);
			streamLength = 1;
		}
		public void UpdateLength() {
			streamLength = Stream.Length;
		}
		public void SetSignatureByteRange(PdfSignatureByteRange value) {
			signatureByteRange = value;
		}
		public void SetSignatureContents(PdfSignatureContents value) {
			signatureContents = value;
		}
		public void SetPositionFromEnd(int value) {
			long length = streamLength;
			long newPosition = length > value ? length - value : length;
			if (Stream.Position != newPosition)
				Stream.Position = newPosition;
		}
		public int SkipSpaces() {
			for (; ; ) {
				int next = Stream.ReadByte();
				if (next == -1)
					return -1;
				byte symbol = (byte)next;
				if (symbol == PdfDocumentReader.Comment)
					ReadString();
				else if (!PdfObjectParser.IsSpaceSymbol(symbol))
					return next;
			}
		}
		public void SkipEmptySpaces() {
			for (int symbol = ReadByte(); ; symbol = ReadByte()) {
				if (symbol < 0)
					PdfDocumentReader.ThrowIncorrectDataException();
				else if (!PdfObjectParser.IsSpaceSymbol((byte)symbol))
					break;
			}
			Stream.Position--;
		}
		public bool FindToken(PdfTokenDescription description) {
			PdfTokenDescription token = PdfTokenDescription.BeginCompare(description);
			for (; ; ) {
				int next = Stream.ReadByte();
				if (next == -1)
					return false;
				if (token.Compare((byte)next))
					return true;
			}
		}
		public long FindLastToken(PdfTokenDescription description) {
			return FindLastToken(description, true);
		}
		public long FindLastToken(PdfTokenDescription description, bool mustBe) {
			long offset = -1;
			while (FindToken(description))
				offset = Stream.Position;
			if (offset == -1 && mustBe)
				PdfDocumentReader.ThrowIncorrectDataException();
			return offset;
		}
		public int ReadByte() {
			return Stream.ReadByte();
		}
		public byte[] ReadBytes(int count) {
			byte[] result = new byte[count];
			Stream.Read(result, 0, count);
			return result;
		}
		public int ReadBytes(byte[] buffer) {
			return Stream.Read(buffer, (int)startPosition, buffer.Length);
		}
		public string ReadString() {
			return ReadString(Stream);
		}
		public int ReadNumber() {
			int next = SkipSpaces();
			if (next == -1)
				PdfDocumentReader.ThrowIncorrectDataException();
			bool isFraction = false;
			int number = PdfObjectParser.ConvertToDigit((byte)next);
			for (; ; ) {
				next = Stream.ReadByte();
				if (next == -1)
					PdfDocumentReader.ThrowIncorrectDataException();
				byte symbol = (byte)next;
				if (PdfObjectParser.IsSpaceSymbol(symbol))
					break;
				if (symbol == '.')
					isFraction = true;
				else if (isFraction) {
					if (symbol != '0')
						PdfDocumentReader.ThrowIncorrectDataException();
				}
				else
					number = number * 10 + PdfObjectParser.ConvertToDigit(symbol);
			}
			return number;
		}
		public long ReadNumber(int digitsCount) {
			int next = SkipSpaces();
			if (next == -1)
				PdfDocumentReader.ThrowIncorrectDataException();
			int number = PdfObjectParser.ConvertToDigit((byte)next);
			for (int i = 1; i < digitsCount; i++) {
				next = Stream.ReadByte();
				if (next == -1)
					PdfDocumentReader.ThrowIncorrectDataException();
				number = number * 10 + PdfObjectParser.ConvertToDigit((byte)next);
			}
			return number;
		}
		public bool ReadToken(PdfTokenDescription description) {
			PdfTokenDescription token = PdfTokenDescription.BeginCompare(description);
			int next = token.IsStartWithComment ? Stream.ReadByte() : SkipSpaces();
			if (next == -1)
				return false;
			token.Compare((byte)next);
			int length = token.Length;
			for (int i = 1; i < length; i++) {
				next = Stream.ReadByte();
				if (next == -1)
					return false;
				if (token.Compare((byte)next))
					return true;
			}
			return false;
		}
		public int ReadIndirectObjectNumber(long offset) {
			Stream.Position = offset + startPosition;
			return ReadNumber();
		}
		public PdfIndirectObject ReadIndirectObject(long offset) {
			int number = ReadIndirectObjectNumber(offset);
			int generation = ReadNumber();
			if (!ReadToken(objToken))
				PdfDocumentReader.ThrowIncorrectDataException();
			return new PdfIndirectObject(number, generation, new PdfStreamData(Stream, Stream.Position, streamLength), offset);
		}
		public PdfIndirectObject ReadPrivateDataIndirectObject(long offset) {
			int number = ReadIndirectObjectNumber(offset);
			int generation = ReadNumber();
			if (!ReadToken(objToken))
				PdfDocumentReader.ThrowIncorrectDataException();
			List<byte> data = new List<byte>();
			PdfTokenDescription beginToken = PdfTokenDescription.BeginCompare(checkObjToken);
			PdfTokenDescription endToken = PdfTokenDescription.BeginCompare(endobjToken);
			bool nested = false;
			for (; ; ) {
				int next = Stream.ReadByte();
				if (next == -1)
					break;
				byte symbol = (byte)next;
				data.Add(symbol);
				if (endToken.Compare(symbol))
					if (nested) {
						endToken = PdfTokenDescription.BeginCompare(endobjToken);
						nested = false;
					}
					else {
						int tokenLength = endToken.Length;
						data.RemoveRange(data.Count - tokenLength, tokenLength);
						break;
					}
				if (beginToken.Compare(symbol)) {
					if (PdfObjectParser.IsDigitSymbol(data[data.Count - checkObjToken.Length - 1]))
						nested = true;
					beginToken = PdfTokenDescription.BeginCompare(checkObjToken);
				}
			}
			return new PdfIndirectObject(number, generation, new PdfArrayData(data.ToArray()), offset);
		}
		public PdfObjectSlot ReadObject(long offset) {
			Position = offset;
			int number = ReadNumber();
			int generation = ReadNumber();
			if (!ReadToken(objToken) || !FindToken(endobjToken))
				PdfDocumentReader.ThrowIncorrectDataException();
			SkipEmptySpaces();
			return new PdfObjectSlot(number, generation, offset);
		}
		public void PatchSignature() {
			if (signatureByteRange != null && signatureContents != null) {
				long startPosition = Stream.Position;
				signatureByteRange.PatchStream(this);
				signatureContents.PatchStream(this);
				Stream.Position = startPosition;
			}
		}
		internal void Flush() {
			Stream.Flush();
		}
		public override void WriteByte(byte b) {
			base.WriteByte(b);
			streamLength++;
		}
		protected override void WriteBytes(byte[] bytes, int count) {
			base.WriteBytes(bytes, count);
			streamLength += count;
		}
	}
}
