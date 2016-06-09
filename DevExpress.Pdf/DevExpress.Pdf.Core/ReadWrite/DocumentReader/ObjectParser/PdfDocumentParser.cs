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
using System.Collections.Generic;
namespace DevExpress.Pdf.Native {
	public class PdfDocumentParser : PdfObjectParser {
		protected const byte StartObject = (byte)'<';
		protected const byte EndObject = (byte)'>';
		static readonly byte[] streamToken = new byte[] { (byte)'s', (byte)'t', (byte)'r', (byte)'e', (byte)'a', (byte)'m' };
		static readonly byte[] endstreamToken = new byte[] { (byte)'e', (byte)'n', (byte)'d', (byte)'s', (byte)'t', (byte)'r', (byte)'e', (byte)'a', (byte)'m' };
		static readonly byte[] endobjToken = new byte[] { (byte)'e', (byte)'n', (byte)'d', (byte)'o', (byte)'b', (byte)'j' };
		public static PdfReaderDictionary ParseDictionary(PdfObjectCollection objects, int number, int generation, PdfData data) {
			return new PdfDocumentParser(objects, number, generation, data).ReadDictionary(true);
		}
		public static PdfReaderStream ParseStream(PdfObjectCollection objects, int number, int generation, PdfData data) {
			return new PdfDocumentParser(objects, number, generation, data).ReadStream();
		}
		public static object ParseObject(PdfObjectCollection objects, int number, int generation, PdfData data, bool applyEncryption) {
			if (applyEncryption && objects.EncryptionInfo != null)
				return new PdfEncryptedDocumentParser(objects, number, generation, data).ReadObject(false, true);
			else
				return new PdfDocumentParser(objects, number, generation, data).ReadObject(false, true);
		}
		readonly PdfObjectCollection objects;
		readonly int number;
		readonly int generation;
		protected PdfObjectCollection Objects { get { return objects; } }
		protected int Number { get { return number; } }
		protected int Generation { get { return generation; } }
		protected override bool CanContinueReading {
			get {
				byte current = Current;
				return base.CanContinueReading && current != StartObject && current != EndObject;
			}
		}
		protected virtual bool ShouldAutoCompleteDictionary { get { return false; } }
		public PdfDocumentParser(PdfObjectCollection objects, int number, int generation, PdfData data, int position) : base(data, position) {
			this.objects = objects;
			this.number = number;
			this.generation = generation;
			SkipSpaces();
		}
		PdfDocumentParser(PdfObjectCollection objects, int number, int generation, PdfData data) : this(objects, number, generation, data, 0) {
		}
		byte[] ReadHexadecimalString(bool isSeparatedUsingWhiteSpaces) {
			if (Current != StartObject || !ReadNext())
				PdfDocumentReader.ThrowIncorrectDataException();
			List<byte> result = new List<byte>();
			while (SkipSpaces()) {
				if (Current == EndObject) {
					ReadNext();
					return DecryptString(result);
				}
				byte symbol = HexadecimalDigit;
				if (!ReadNext())
					PdfDocumentReader.ThrowIncorrectDataException();
				if (isSeparatedUsingWhiteSpaces)
					if (IsSpace) {
						result.Add(symbol);
						if (!ReadNext())
							PdfDocumentReader.ThrowIncorrectDataException();
						continue;
					}
					else
						symbol *= 16;
				else {
					if (!SkipSpaces())
						PdfDocumentReader.ThrowIncorrectDataException();
					symbol *= 16;
				}
				if (Current == EndObject)
					result.Add(symbol);
				else {
					result.Add((byte)(symbol + HexadecimalDigit));
					if (!ReadNext())
						PdfDocumentReader.ThrowIncorrectDataException();
				}
			}
			PdfDocumentReader.ThrowIncorrectDataException();
			return null;
		}
		PdfReaderDictionary ReadDictionary(bool isIndirect) {
			if (Current != StartObject || !ReadNext() || Current != StartObject || !ReadNext())
				PdfDocumentReader.ThrowIncorrectDataException();
			PdfReaderDictionary dictionary = new PdfReaderDictionary(objects, isIndirect ? number : -1, generation);
			while (SkipSpaces())
				switch (Current) {
					case NameIdentifier:
						string name = ReadName().Name;
						if (!SkipSpaces())
							PdfDocumentReader.ThrowIncorrectDataException();
						try {
							dictionary[name] = ReadObject(name == PdfCompositeFontDescriptor.PanoseDictionaryKey, false);
						}
						catch {
							return dictionary;
						}
						break;
					case EndObject:
						if (!ReadNext() && Current != EndObject)
							PdfDocumentReader.ThrowIncorrectDataException();
						ReadNext();
						return dictionary;
					default:
						if (!ShouldAutoCompleteDictionary)
							PdfDocumentReader.ThrowIncorrectDataException();
						int position = CurrentPosition;
						if (!CheckDictionaryAlphabeticalToken(ReadToken())) {
							CurrentPosition = position;
							return dictionary;
						}
						break;
				}
			PdfDocumentReader.ThrowIncorrectDataException();
			return dictionary;
		}
		bool ReadStreamToken() {
			int position = CurrentPosition;
			SkipSpaces();
			if (ReadToken(streamToken))
				return true;
			CurrentPosition = position;
			return false;
		}
		PdfReaderStream ReadStream(PdfReaderDictionary dictionary) {
			while (Current == Space)
				if (!ReadNext())
					PdfDocumentReader.ThrowIncorrectDataException();
			switch (Current) {
				case CarriageReturn:
					if (!ReadNext())
						PdfDocumentReader.ThrowIncorrectDataException();
					if (Current != LineFeed)
						ReadPrev();
					break;
				case LineFeed:
					break;
				default:
					PdfDocumentReader.ThrowIncorrectDataException();
					break;
			}
			Data.SavePosition();
			int? lengthValue = dictionary.GetInteger(PdfStream.DictionaryLengthKey);
			Data.RestorePosition();
			if (!lengthValue.HasValue) {
				if (!SkipSpaces())
					PdfDocumentReader.ThrowIncorrectDataException();
				return CreateStream(dictionary, ReadData(endstreamToken, true));
			}
			int length = lengthValue.Value;
			if (length < 0)
				PdfDocumentReader.ThrowIncorrectDataException();
			return CreateStream(dictionary, ReadData(length, endstreamToken, endobjToken, true));
		}
		protected virtual PdfReaderStream CreateStream(PdfReaderDictionary dictionary, byte[] data) {
			return new PdfReaderStream(dictionary, data, null);
		}
		PdfReaderStream ReadStream() {
			PdfReaderDictionary dictionary = ReadDictionary(true);
			if (!ReadStreamToken())
				PdfDocumentReader.ThrowIncorrectDataException();
			return ReadStream(dictionary);
		}
		protected byte[] ReadData(byte[] token, bool ignoreWhiteSpace) {
			int tokenLength = token.Length;
			int fullTokenLength = tokenLength + 1;
			int endMarkerPosition = 0;
			List<byte> buffer = new List<byte>();
			for (int remain = -fullTokenLength; ; remain++) {
				byte current = Current;
				buffer.Add(current);
				if (endMarkerPosition < tokenLength && current == token[endMarkerPosition])
					endMarkerPosition++;
				else {
					if (IsSpace && endMarkerPosition == tokenLength)
						if (ignoreWhiteSpace)
							break;
						else {
							byte previous = buffer[remain];
							if (previous == Space || previous == LineFeed || previous == CarriageReturn)
								break;
						}
					endMarkerPosition = 0;
				}
				if (!ReadNext()) {
					if (endMarkerPosition == tokenLength) {
						buffer.Add(Current);
						break;
					}
					PdfDocumentReader.ThrowIncorrectDataException();
				}
			}
			fullTokenLength++;
			buffer.RemoveRange(buffer.Count - fullTokenLength, fullTokenLength);
			return buffer.ToArray();
		}
		protected byte[] ReadData(int length, byte[] token, byte[] alternativeToken, bool ignoreWhiteSpace) {
			if (!ReadNext()) {
				if (length == 0)
					return new byte[0];
				PdfDocumentReader.ThrowIncorrectDataException();
			}
			int currentPosition = CurrentPosition;
			try {
				byte[] result = null;
				if (length > 0) {
					int nextPosition = currentPosition + length;
					result = Data.Read(currentPosition, length);
					CurrentPosition = nextPosition;
				}
				int position = CurrentPosition;
				if (!SkipSpaces() || !ReadToken(token)) {
					CurrentPosition = position;
					if (alternativeToken == null || !ReadToken(alternativeToken))
						PdfDocumentReader.ThrowIncorrectDataException();
				}
				return result ?? new byte[0];
			}
			catch {
				CurrentPosition = currentPosition;
				return ReadData(token, ignoreWhiteSpace);
			}
		}
		protected object ReadDictonaryOrStream(bool isHexadecimalStringSeparatedUsingWhiteSpaces, bool isIndirect) {
			if (Current != StartObject || !ReadNext())
				return null;
			bool isDictionary = Current == StartObject;
			ReadPrev();
			if (!isDictionary)
				return ReadHexadecimalString(isHexadecimalStringSeparatedUsingWhiteSpaces);
			PdfReaderDictionary dictionary = ReadDictionary(isIndirect);
			return ReadStreamToken() ? (object)ReadStream(dictionary) : (object)dictionary;
		}
		protected override bool CanReadObject() {
			return SkipSpaces();
		}
		protected override object ReadNumericObject() {
			object number = ReadNumber();
			if (!(number is int))
				return number;
			int value = (int)number;
			if (!SkipSpaces() || !IsDigit)
				return value;
			int position = CurrentPosition;
			number = ReadNumber();
			if ((number is int) && SkipSpaces() && Current == (byte)'R' && (!ReadNext() || !CanContinueReading))
				return new PdfObjectReference(value, (int)number);
			CurrentPosition = position;
			return value;
		}
		protected override object ReadAlphabeticalObject(bool isHexadecimalStringSeparatedUsingWhiteSpaces, bool isIndirect) {
			object result = ReadDictonaryOrStream(isHexadecimalStringSeparatedUsingWhiteSpaces, isIndirect);
			if (result == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			return result;
		}
		protected virtual bool CheckDictionaryAlphabeticalToken(string token) {
			return false;
		}
	}
}
