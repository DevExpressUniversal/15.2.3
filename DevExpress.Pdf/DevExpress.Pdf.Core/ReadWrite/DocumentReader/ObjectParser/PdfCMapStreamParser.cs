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
	public class PdfCMapStreamParser : PdfDocumentParser {
		public static PdfCharacterMapping Parse(byte[] data) {
			return new PdfCMapStreamParser(data).Parse();
		}
		static string GetUnicodeValues(byte[] bytes) {
			if (bytes == null)
				PdfDocumentReader.ThrowIncorrectDataException();
			int count = bytes.Length;
			bool oddCount = count % 2 != 0;
			count = oddCount ? (count + 1) / 2 : count / 2;
			char[] array = new char[count];
			for (int i = 0, index = 0; i < count; i++)
				array[i] = (char)(oddCount && i == 0 ? bytes[index++] : (bytes[index++] << 8) + bytes[index++]);
			return new String(array);
		}
		static bool Increment(byte[] value, byte[] max) {
			bool equals = true;
			int length = value.Length;
			for (int i = 0; i < length; i++)
				if (value[i] != max[i]) {
					equals = false;
					break;
				}
			if (equals)
				return false;
			for (int index = length - 1; index >= 0; index--) {
				byte b = value[index];
				if (b == 0xff) 
					value[index] = 0;
				else {
					value[index] = (byte)(b + 1);
					return true;
				}
			}
			return false;
		}
		readonly Dictionary<byte[], string> cMapTable = new Dictionary<byte[], string>();
		protected override bool IgnoreIncorrectSymbolsInNames { get { return true; } }
		protected override bool ShouldAutoCompleteDictionary { get { return true; } }
		PdfCMapStreamParser(byte[] data) : base (null, 0, 0, new PdfArrayData(data), 0) {
		}
		byte[] ReadHexNumber() {
			int value = 0;
			int count = 0;
			SkipSpaces();
			if (Current == StartObject) {
				ReadNext();
				while (Current != EndObject) {
					if (!IsSpace) {
						value = (value << 4) + HexadecimalDigit;
						count++;
					}
					ReadNext();
				}
			}
			ReadNext();
			int bytesCount = (int)Math.Ceiling(count / 2.0);
			byte[] result = new byte[bytesCount];
			for (int i = bytesCount - 1; i >= 0; i--) {
				result[i] = (byte)(value & 0xff);
				value >>= 8;
			}
			return result;
		}
		PdfCharacterMapping Parse() {
			int valuesCount = 0;
			while (SkipSpaces()) {
				object value = ReadObject(false, false);
				if (value != null) {
					string token = value as string;
					if (token == null) {
						int? number = value as int?;
						if (number.HasValue)
							valuesCount = number.Value;
					}
					else if (token == "endcmap")
						break;
					else if (token == "beginbfchar" || token == "begincidchar")
						for (int i = 0; i < valuesCount; i++) {
							byte[] charCode = ReadHexNumber();
							object charArray = ReadObject(false, false);
							byte[] unicodeCharacters = charArray as byte[];
							if (unicodeCharacters == null) {
								int? cidChar = charArray as int?;
								if (!cidChar.HasValue)
									PdfDocumentReader.ThrowIncorrectDataException();
								cMapTable.Add(charCode, new string((char)cidChar, 1));
							}
							else 
								cMapTable.Add(charCode, GetUnicodeValues(unicodeCharacters));
						}
					else if (token == "beginbfrange" || token == "begincidrange") 
						for (int i = 0; i < valuesCount; i++) {
							byte[] currentCharCode = ReadHexNumber();
							int currentCharCodeLength = currentCharCode.Length;
							byte[] lastCharCode = ReadHexNumber();
							int lastCharCodeLength = lastCharCode.Length;
							int diff = currentCharCodeLength - lastCharCodeLength;
							if (diff < 0) {
								byte[] charCode = new byte[lastCharCodeLength];
								Array.Copy(currentCharCode, 0, charCode, -diff, currentCharCodeLength);
								currentCharCode = charCode;
							}
							else if (diff > 0) {
								byte[] charCode = new byte[currentCharCodeLength];
								Array.Copy(lastCharCode, 0, charCode, diff, lastCharCodeLength);
								lastCharCode = charCode;
							}
							object range = ReadObject(false, false);	  
							IList<object> unicodeRangeArray = range as IList<object>;
							if (unicodeRangeArray == null) {
								byte[] unicodeRangeStart = range as byte[];
								if (unicodeRangeStart == null) {
									int? cidRangeStart = range as int?;
									if (!cidRangeStart.HasValue)
										break;										
									int cidValue = cidRangeStart.Value;
									do {
										cMapTable.Add((byte[])currentCharCode.Clone(), new string((char)cidValue, 1));
										cidValue++;
									} while (Increment(currentCharCode, lastCharCode));
								}
								else {
									char unicodeRangeValue = GetUnicodeValues(unicodeRangeStart)[0];
									do {
										cMapTable.Add((byte[])currentCharCode.Clone(), unicodeRangeValue.ToString());
										unicodeRangeValue++;
									} while (Increment(currentCharCode, lastCharCode));
								}
							}
							else {
								int j = 0;
								int unicodeArrayLength = unicodeRangeArray.Count;
								do {
									if (j >= unicodeArrayLength)
										PdfDocumentReader.ThrowIncorrectDataException();
									byte[] array = unicodeRangeArray[j++] as byte[];
									if (array == null)
										PdfDocumentReader.ThrowIncorrectDataException();
									cMapTable.Add((byte[])currentCharCode.Clone(), GetUnicodeValues(array));
								} while (Increment(currentCharCode, lastCharCode));
							}
						}
				}
			}
			return new PdfCharacterMapping(Data.ToArray(), cMapTable);
		}
		protected override object ReadAlphabeticalObject(bool isHexadecimalStringSeparatedUsingWhiteSpaces, bool isIndirect) {
			do {
				object dictionaryOrStream = ReadDictonaryOrStream(isHexadecimalStringSeparatedUsingWhiteSpaces, isIndirect);
				if (dictionaryOrStream != null)
					return dictionaryOrStream;
				string token = ReadToken();
				if (!String.IsNullOrEmpty(token))
					return token;
			} while (ReadNext());
			return null;
		}
		protected override bool CheckDictionaryAlphabeticalToken(string token) {
			return token == "def";
		}
	}
}
