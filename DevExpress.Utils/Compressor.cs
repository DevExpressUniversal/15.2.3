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
namespace DevExpress.Utils.Compress {
	public class Compressor {
		const int radix = 128; 
		const int offset = 128;
		static int FindBest(string str, int winStart, int winEnd, string lookAhead, out int matchCount) {
			int matchPos = -1;
			matchCount = 3;
			int lookAheadLength = lookAhead.Length;
			char firstChar = lookAhead[0];
			for (int pos = winStart; pos < winEnd; pos++) {
				if (firstChar == str[pos]) {
					int currentMatchCount = 1;
					while (currentMatchCount < lookAheadLength && pos + currentMatchCount < winEnd && str[pos + currentMatchCount] == lookAhead[currentMatchCount])
						currentMatchCount++;
					if (currentMatchCount > matchCount) {
						matchCount = currentMatchCount;
						matchPos = pos;
						if (currentMatchCount == lookAheadLength)
							break;
					}
				}
			}
			return matchPos;
		}
		public static string Compress(string originalString) {
			StringBuilder compressedString = new StringBuilder(originalString[0]);
			int currPos = 0;
			int bufferSize = radix * radix - 1; 
			int lookAheadBufferSize = radix - 1; 
			string lookAheadString = originalString.Substring(currPos, Math.Min(originalString.Length - currPos, lookAheadBufferSize));
			while (lookAheadString.Length > 0) { 
				int count; 
				int pos = FindBest(originalString, Math.Max(currPos - bufferSize, 0), currPos - 1, lookAheadString, out count); 
				if (pos == -1) { 
					if (originalString[currPos] >= offset)
						compressedString.Append((char)offset);
					compressedString.Append(originalString[currPos]);
					currPos++;
				} 
				else { 
					pos = currPos - pos;
					int ms = pos / radix;
					int ls = pos % radix;
					compressedString.Append(new char[] { (char)(offset + count), (char)(offset + ms), (char)(offset + ls) } );
					currPos += count;
				}
				lookAheadString = originalString.Substring(currPos, Math.Min(originalString.Length - currPos, lookAheadBufferSize));
			}
			return compressedString.ToString();
		}
		public static string Decompress(string i) {
			StringBuilder result = new StringBuilder();
			for(int pos = 0; pos < i.Length; pos++) {
				if(i[pos] >= offset) {
					int l = i[pos] - offset;
					if(l > 0) {
						int p = result.Length - ((i[pos + 1] - offset) * radix + i[pos + 2] - offset);
						if (p >= 0 && p < result.Length && l > 0 && p + l <= result.Length)
							result.Append(result.ToString(p, l));
						else
							throw new ArgumentException("The value is in an unknown format", "compressed");
						pos += 2;
					} else {
						pos++;
						result.Append(i[pos]);
					}
				}
				else
					result.Append(i[pos]);
			}
			return result.ToString();
		}
	}
}
