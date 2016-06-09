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

using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
namespace DevExpress.Pdf.Native {
	public class PdfObjectParser {
		const byte plus = (byte)'+';
		const byte minus = (byte)'-';
		const byte period = (byte)'.';
		const byte digitStart = (byte)'0';
		const byte digitEnd = (byte)'9';
		const byte hexadecimalDigitStart = (byte)'A';
		const byte hexadecimalDigitEnd = (byte)'F';
		const byte lowercaseHexadecimalDigitStart = (byte)'a';
		const byte lowercaseHexadecimalDigitEnd = (byte)'f';
		const byte numberSign = (byte)'#';
		const byte escape = (byte)'\\';
		const byte horizontalTab = (byte)'\x09';
		const byte endString = (byte)')';
		protected const byte StartString = (byte)'(';
		protected const byte StartArray = (byte)'[';
		protected const byte EndArray = (byte)']';
		protected const byte CarriageReturn = (byte)'\x0d';
		protected const byte LineFeed = (byte)'\x0a';
		protected const byte Space = (byte)' ';
		protected const byte Comment = (byte)'%';
		protected const byte NameIdentifier = (byte)'/';
		static readonly byte[] nullToken = new byte[] { (byte)'n', (byte)'u', (byte)'l', (byte)'l' };
		static readonly byte[] trueToken = new byte[] { (byte)'t', (byte)'r', (byte)'u', (byte)'e' };
		static readonly byte[] falseToken = new byte[] { (byte)'f', (byte)'a', (byte)'l', (byte)'s', (byte)'e' };
		static readonly PdfTokenDescription startxrefToken =
			new PdfTokenDescription(new byte[] { (byte)'s', (byte)'t', (byte)'a', (byte)'r', (byte)'t', (byte)'x', (byte)'r', (byte)'e', (byte)'f' });
		public static bool IsSpaceSymbol(byte symbol) {
			return symbol == Space || symbol == LineFeed || symbol == CarriageReturn || symbol == horizontalTab || symbol == (byte)'\x0c';
		}
		public static bool IsDigitSymbol(byte symbol) {
			return symbol >= digitStart && symbol <= digitEnd;
		}
		public static byte ConvertToDigit(byte symbol) {
			if (symbol < digitStart || symbol > digitEnd)
				PdfDocumentReader.ThrowIncorrectDataException();
			return (byte)(symbol - digitStart);
		}
		public static bool IsHexadecimalDigitSymbol(byte symbol) {
			return (symbol >= digitStart && symbol <= digitEnd) || 
				(symbol >= hexadecimalDigitStart && symbol <= hexadecimalDigitEnd) || (symbol >= lowercaseHexadecimalDigitStart && symbol <= lowercaseHexadecimalDigitEnd);
		}
		public static byte ConvertToHexadecimalDigit(byte symbol) {
			if (symbol >= digitStart && symbol <= digitEnd)
				return (byte)(symbol - digitStart);
			if (symbol >= hexadecimalDigitStart && symbol <= hexadecimalDigitEnd)
				return (byte)(symbol - hexadecimalDigitStart + 10);
			if (symbol < lowercaseHexadecimalDigitStart || symbol > lowercaseHexadecimalDigitEnd)
				PdfDocumentReader.ThrowIncorrectDataException();
			return (byte)(symbol - lowercaseHexadecimalDigitStart + 10);
		}
		public static string[] ParseNameArray(byte[] data) {
			return new PdfObjectParser(new PdfArrayData(data), 0).ReadNameArray();
		}
		public static int? ParseStartXRef(byte[] data) {
			return new PdfObjectParser(new PdfArrayData(data), 0).ReadStartXRef();
		}
		readonly PdfData data;
		int currentPosition;
		byte current;
		byte Digit { get { return ConvertToDigit(current); } }
		bool IsHexadecimalDigit { get { return IsHexadecimalDigitSymbol(current); } }
		protected PdfData Data { get { return data; } }
		protected int CurrentPosition {
			get { return currentPosition; }
			set {
				currentPosition = value;
				current = (byte)data.Get(currentPosition);
			}
		}
		protected byte Current { get { return current; } }
		protected bool IsSpace { get { return IsSpaceSymbol(current); } }
		protected bool IsDigit { get { return IsDigitSymbol(current); } }
		protected byte HexadecimalDigit { get { return ConvertToHexadecimalDigit(current); } }
		protected virtual bool CanContinueReading {
			get { return !IsSpace && current != Comment && current != NameIdentifier && current != StartString && current != endString && current != StartArray && current != EndArray; }
		}
		protected virtual bool IgnoreIncorrectSymbolsInNames { get { return false; } }
		protected PdfObjectParser(PdfData data, int position) {
			this.data = data;
			currentPosition = position;
			current = (byte)data.Get(position);
		}
		public object ReadObject(bool isHexadecimalStringSeparatedUsingWhiteSpaces, bool isIndirect) {
			if (!CanReadObject())
				return null;
			if (IsDigit)
				return ReadNumericObject();
			switch (current) {
				case Comment:
					StringBuilder sb = new StringBuilder();
					while (ReadNext() && current != CarriageReturn && current != LineFeed)
						sb.Append((char)current);
					return new PdfComment(sb.ToString());
				case plus:
				case minus:
				case period:
					return ReadNumber();
				case NameIdentifier:
					return ReadName();
				case StartString:
					return ReadString();
				case StartArray:
					return ReadArray();
				case (byte)'n':
					if (ReadToken(nullToken))
						return null;
					ReadPrev();
					break;
				case (byte)'t':
					if (ReadToken(trueToken))
						return true;
					ReadPrev();
					break;
				case (byte)'f':
					if (ReadToken(falseToken))
						return false;
					ReadPrev();
					break;
			}
			return ReadAlphabeticalObject(isHexadecimalStringSeparatedUsingWhiteSpaces, isIndirect);
		}
		void SkipLineFeed() {
			if (!ReadNext())
				PdfDocumentReader.ThrowIncorrectDataException();
			if (current != LineFeed)
				ReadPrev();
		}
		byte[] ReadString() {
			if (current != StartString)
				PdfDocumentReader.ThrowIncorrectDataException();
			int nestCount = 1;
			List<byte> result = new List<byte>();
			while (ReadNext()) {
				switch (current) {
					case escape:
						if (!ReadNext())
							PdfDocumentReader.ThrowIncorrectDataException();
						if (IsDigit) {
							byte characterCode = Digit;
							for (int i = 0; i < 2; i++) {
								if (!ReadNext())
									PdfDocumentReader.ThrowIncorrectDataException();
								if (IsDigit)
									characterCode = (byte)(characterCode * 8 + Digit);
								else
									ReadPrev();
							}
							result.Add(characterCode);
							break;
						}
						else {
							switch (current) {
								case (byte)'n':
									result.Add(LineFeed);
									break;
								case (byte)'r':
									result.Add(CarriageReturn);
									break;
								case (byte)'t':
									result.Add(horizontalTab);
									break;
								case (byte)'b':
									result.Add((byte)0x08);
									break;
								case (byte)'f':
									result.Add((byte)0x0c);
									break;
								case CarriageReturn:
									SkipLineFeed();
									break;
								case LineFeed:
									break;
								default:
									result.Add(current);
									break;
							}
						}
						break;
					case CarriageReturn:
						result.Add(LineFeed);
						SkipLineFeed();
						break;
					case endString:
						if (--nestCount == 0) {
							ReadNext();
							return DecryptString(result);
						}
						goto default;
					case StartString:
						nestCount++;
						goto default;
					default:
						result.Add(current);
						break;
				}
			}
			PdfDocumentReader.ThrowIncorrectDataException();
			return null;
		}
		IList<object> ReadArray() {
			if (current != StartArray || !ReadNext())
				PdfDocumentReader.ThrowIncorrectDataException();
			List<object> data = new List<object>();
			while (SkipSpaces()) {
				if (current == EndArray) {
					ReadNext();
					return data;
				}
				data.Add(ReadObject(false, false));
			}
			PdfDocumentReader.ThrowIncorrectDataException();
			return data;
		}
		string[] ReadNameArray() {
			List<string> names = new List<string>();
			while (SkipSpaces())
				names.Add(ReadName().Name);
			return names.ToArray();
		}
		int? ReadStartXRef() {
			PdfTokenDescription token = PdfTokenDescription.BeginCompare(startxrefToken);
			do if (token.Compare(current)) {
				if (!ReadNext() || !SkipSpaces())
					return null;
				object offset = ReadNumber();
				return (offset is int) ? (int?)offset : null;
			} while (ReadNext());
			return null;
		}
		protected bool ReadNext() {
			currentPosition++;
			if (currentPosition >= data.DataLength)
				return false;
			int res = data.Get(currentPosition);
			if (res < 0)
				return false;
			current = (byte)res;
			return true;
		}
		protected bool ReadPrev() {
			if (--currentPosition < 0)
				return false;
			current = (byte)data.Get(currentPosition);
			return true;
		}
		protected bool SkipSpaces() {
			if (currentPosition >= data.DataLength)
				return false;
			for (;;) {
				if (IsSpace) {
					if (!ReadNext())
						return false;
				}
				else if (current != Comment)
					return true;
				else do
					if (!ReadNext())
						return false;
				while (current != CarriageReturn && current != LineFeed);
			}
		}
		protected bool ReadToken(byte[] token) {
			bool end = false;
			foreach (byte symbol in token) {
				if (end || current != symbol)
					return false;
				if (!ReadNext())
					end = true;
			}
			return true;
		}
		protected string ReadToken() {
			StringBuilder sb = new StringBuilder();
			do {
				if (!CanContinueReading)
					break;
				sb.Append((char)current);
			} while (ReadNext());
			return sb.ToString();
		}
		protected object ReadNumber() {
			bool negative;
			bool hasSign;
			switch (current) {
				case minus:
					negative = true;
					hasSign = true;
					if (!ReadNext() || (current == minus && !ReadNext()))
						PdfDocumentReader.ThrowIncorrectDataException();
					break;
				case plus:
					negative = false;
					hasSign = true;
					if (!ReadNext())
						PdfDocumentReader.ThrowIncorrectDataException();
					break;
				default:
					negative = false;
					hasSign = false;
					break;
			}
			int value = 0;
			while (IsDigit) {
				value = value * 10 + Digit;
				if (!ReadNext())
					break;
			}
			if (negative)
				value = -value;
			if (current != period && current != ',')
				return value;
			if (!ReadNext())
				return (double)value;
			if (current == minus && !hasSign)
				if (ReadNext() && IsDigit) {
					negative = true;
					value = -value;
				}
				else {
					ReadPrev();
					return (double)value;
				}
			int position = currentPosition;
			double fraction = 0;
			double pow = 10;
			while (IsDigit) {
				fraction = fraction + Digit / pow;
				pow *= 10;
				if (!ReadNext())
					break;
			}
			if (negative)
				fraction = -fraction;
			return value + fraction;
		}
		protected PdfName ReadName() {
			if (current != NameIdentifier || !ReadNext())
				PdfDocumentReader.ThrowIncorrectDataException();
			StringBuilder sb = new StringBuilder();
			while (CanContinueReading) {
				if (current < (byte)'!') {
					if (!IgnoreIncorrectSymbolsInNames)
						PdfDocumentReader.ThrowIncorrectDataException();
				}
				else if (current == numberSign) {
					if (!ReadNext()) {
						sb.Append((char)numberSign);
						break;
					}
					if (!IsHexadecimalDigit) {
						sb.Append((char)numberSign);
						continue;
					}
					byte first = current;
					byte high = (byte)(HexadecimalDigit * 16);
					if (!ReadNext()) {
						sb.Append((char)numberSign);
						sb.Append((char)first);
						break;
					}
					if (!IsHexadecimalDigit) {
						sb.Append((char)numberSign);
						sb.Append((char)first);
						continue;
					}
					sb.Append((char)(high + HexadecimalDigit));
				}
				else
					sb.Append((char)current);
				if (!ReadNext())
					break;
			}
			return new PdfName(sb.ToString());
		}
		protected virtual byte[] DecryptString(List<byte> list) {
			return list.ToArray();
		}
		protected virtual bool CanReadObject() {
			if (currentPosition >= data.DataLength)
				return false;
			while (IsSpace)
				if (!ReadNext())
					return false;
			return true;
		}
		protected virtual object ReadNumericObject() {
			return ReadNumber();
		}
		protected virtual object ReadAlphabeticalObject(bool isHexadecimalStringSeparatedUsingWhiteSpaces, bool isIndirect) {
			return ReadToken();
		}
	}
}
