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
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public class PdfASCII85DecodeFilter : PdfFilter {
		internal const string Name = "ASCII85Decode";
		internal const string ShortName = "A85";
		const int bufferSize = 5;
		const long maxAllowedValue = (1L << 32) - 1;
		const long multiplier1 = 85;
		const long multiplier2 = multiplier1 * multiplier1;
		const long multiplier3 = multiplier2 * multiplier1;
		const long multiplier4 = multiplier3 * multiplier1;
		static bool IsSpaceSymbol(byte symbol) {
			return symbol == 0x0a || symbol == 0x0d || symbol == 0 || symbol == 9 || symbol == 0x0c || symbol == 0x20;
		}
		static void DecodeBuffer(List<byte> result, byte[] buffer, int count) {
			long value = (buffer[0] - 0x21) * multiplier4 + (buffer[1] - 0x21) * multiplier3 + (buffer[2] - 0x21) * multiplier2 + (buffer[3] - 0x21) * multiplier1 + buffer[4] - 0x21;
			if (value > maxAllowedValue)
				PdfDocumentReader.ThrowIncorrectDataException();
			result.Add((byte)((value & 0xff000000) >> 24));
			if (--count > 0) {
				result.Add((byte)((value & 0xff0000) >> 16));
				if (--count > 0) {
					result.Add((byte)((value & 0xff00) >> 8));
					if (--count > 0)
						result.Add((byte)(value & 0xff));
				}
			}
		}
		protected internal override string FilterName { get { return Name; } }
		internal PdfASCII85DecodeFilter() {
		}
		protected internal override byte[] Decode(byte[] data) {
			int dataLength = data.Length;
			if (dataLength == 0)
				return data;
			byte[] buffer = new byte[bufferSize];
			List<byte> result = new List<byte>();
			int position = 0;
			int bufferIndex = 0;
			bool possibleEnd = false;
			while (position < dataLength) {
				byte next = data[position++];
				if (!IsSpaceSymbol(next))
					if (next == 0x7e) {
						for (;;) {
							if (position >= dataLength)
								PdfDocumentReader.ThrowIncorrectDataException();
							next = data[position++];
							if (next == 0x3e)
								break;
							if (!IsSpaceSymbol(next))
								PdfDocumentReader.ThrowIncorrectDataException();
						}
						break;
					}
					else if (next == 0x7a) {
						if (bufferIndex != 0)
							PdfDocumentReader.ThrowIncorrectDataException();
						for (int i = 0; i < 4; i++)
							result.Add(0);
					}
					else {
						if (next < 0x21 || next > 0x75 || possibleEnd)
							PdfDocumentReader.ThrowIncorrectDataException();
						buffer[bufferIndex] = next;
						if (++bufferIndex == bufferSize) {
							DecodeBuffer(result, buffer, 4);
							bufferIndex = 0;
						}
					}
			}
			if (bufferIndex > 0) {
				for (int i = bufferIndex; i < bufferSize; i++)
					buffer[i] = 0x75;
				DecodeBuffer(result, buffer, bufferIndex - 1);
			}
			return result.ToArray();
		}
	}
}
