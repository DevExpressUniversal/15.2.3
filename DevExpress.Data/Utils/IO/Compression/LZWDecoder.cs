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
namespace DevExpress.Utils {
	public class LZWDecoder {
		public static byte[] Decode(byte[] data, int initialSequenceLength) {
			return new LZWDecoder(data, initialSequenceLength).Decode();
		}
		readonly Dictionary<int, byte[]> dictionary = new Dictionary<int, byte[]>();
		readonly byte[] data;
		readonly int initialSequenceLength;
		readonly int dataSize;
		readonly int clearTable;
		readonly int endOfData;
		int currentEntryLength;
		int currentMaxEntryLength;
		int currentDictionarySize;
		int currentPosition = 0;
		byte currentSymbol;
		int remainBits = 8;
		LZWDecoder(byte[] data, int initialSequenceLength) {
			if (initialSequenceLength < 3 || initialSequenceLength > 9)
				throw new ArgumentException("initialSequenceLength");
			this.data = data;
			this.initialSequenceLength = initialSequenceLength;
			dataSize = data.Length;
			if (dataSize > 0)
				currentSymbol = data[currentPosition];
			clearTable = 1 << (initialSequenceLength - 1);
			endOfData = clearTable + 1;
			InitializeTable();
		}
		void InitializeTable() {
			currentEntryLength = initialSequenceLength;
			currentMaxEntryLength = (1 << initialSequenceLength) - 1;
			currentDictionarySize = endOfData + 1;
		}
		byte[] Decode() {
			List<byte> result = new List<byte>();
			byte[] currentSequence = new byte[0];
			for (;;) {
				int nextCode = ReadNext();
				if (nextCode == endOfData) 
					break;
				if (nextCode == clearTable) {
					InitializeTable();
					dictionary.Clear();
					currentSequence = new byte[0];
				}
				else {
					int currentSequenceLength = currentSequence.Length;
					int nextSequenceLength = currentSequenceLength + 1;
					byte[] sequence;
					if (nextCode < clearTable)
						sequence = new byte[] { (byte)nextCode };
					else if (nextCode < currentDictionarySize) 
						sequence = dictionary[nextCode];
					else {
						sequence = new byte[nextSequenceLength];
						currentSequence.CopyTo(sequence, 0);
						sequence[currentSequenceLength] = currentSequence[0];
					}
					for(int n = 0; n < sequence.Length; n++) result.Add(sequence[n]);
					if (nextSequenceLength > 1) {
						byte[] dictionaryEntry = new byte[nextSequenceLength];
						currentSequence.CopyTo(dictionaryEntry, 0);
						dictionaryEntry[currentSequenceLength] = sequence[0];
						dictionary.Add(currentDictionarySize, dictionaryEntry);
						if (++currentDictionarySize == currentMaxEntryLength) {
							if (currentEntryLength < 12) {
								currentEntryLength++;
								currentMaxEntryLength = ((currentMaxEntryLength + 1) << 1) - 1;
							}
						}
					}
					currentSequence = sequence;
				}
			}
			return result.ToArray();
		}
		int ReadNext() {
			int result = 0;
			int toRead = currentEntryLength - remainBits;
			while (toRead > 0) {
				result += currentSymbol << toRead;
				if (++currentPosition >= dataSize)
					throw new ArgumentException("data");
				currentSymbol = data[currentPosition];
				remainBits = 8;
				toRead -= 8;
			}
			remainBits = -toRead;
			result += currentSymbol >> remainBits;
			currentSymbol = (byte)(currentSymbol & ((1 << remainBits) - 1));
			return result;
		}
	}
}
