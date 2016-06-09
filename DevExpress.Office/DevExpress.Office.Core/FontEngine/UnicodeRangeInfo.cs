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
using System.Collections;
using DevExpress.Utils;
namespace DevExpress.Office.Utils {
	#region UnicodeSubrange
	public class UnicodeSubrange {
		#region Fields
		readonly char lowValue;
		readonly char hiValue;
		readonly int bit;
		#endregion
		public UnicodeSubrange(char low, char hi, int bit) {
			this.lowValue = low;
			this.hiValue = hi;
			this.bit = bit;
		}
		#region Properties
		public char LowValue { get { return lowValue; } }
		public char HiValue { get { return hiValue; } }
		public int Bit { get { return bit; } }
		#endregion
		public bool ContainsChar(char ch) {
			return ch >= LowValue && ch <= HiValue;
		}
	}
	#endregion
	#region UnicodeRangeInfo
	public class UnicodeRangeInfo {
		readonly List<UnicodeSubrange> ranges;
		readonly Dictionary<int, UnicodeSubrange> bitToSubrangeMap;
		public UnicodeRangeInfo() {
			this.ranges = new List<UnicodeSubrange>();
			this.bitToSubrangeMap = new Dictionary<int, UnicodeSubrange>();
			PopulateSubranges();
		}
		#region PopulateSubranges
		protected internal virtual void PopulateSubranges() {
			AddSubrange('\x0000', '\x007F', 0); 
			AddSubrange('\x0080', '\x00FF', 1); 
			AddSubrange('\x0100', '\x017F', 2); 
			AddSubrange('\x0180', '\x024F', 3); 
			AddSubrange('\x0250', '\x02AF', 4); 
			AddSubrange('\x02B0', '\x02FF', 5); 
			AddSubrange('\x0300', '\x036F', 6); 
			AddSubrange('\x0370', '\x03FF', 7); 
			AddSubrange('\x0400', '\x04FF', 9); 
			AddSubrange('\x0500', '\x052F', 9); 
			AddSubrange('\x0530', '\x058F', 10); 
			AddSubrange('\x0590', '\x05FF', 11); 
			AddSubrange('\x0600', '\x06FF', 13); 
			AddSubrange('\x0700', '\x074F', 71); 
			AddSubrange('\x0750', '\x077F', 13); 
			AddSubrange('\x0780', '\x07BF', 72); 
			AddSubrange('\x07C0', '\x07FF', 14); 
			AddSubrange('\x0900', '\x097F', 15); 
			AddSubrange('\x0980', '\x09FF', 16); 
			AddSubrange('\x0A00', '\x0A7F', 17); 
			AddSubrange('\x0A80', '\x0AFF', 18); 
			AddSubrange('\x0B00', '\x0B7F', 19); 
			AddSubrange('\x0B80', '\x0BFF', 20); 
			AddSubrange('\x0C00', '\x0C7F', 21); 
			AddSubrange('\x0C80', '\x0CFF', 22); 
			AddSubrange('\x0D00', '\x0D7F', 23); 
			AddSubrange('\x0D80', '\x0DFF', 73); 
			AddSubrange('\x0E00', '\x0E7F', 24); 
			AddSubrange('\x0E80', '\x0EFF', 25); 
			AddSubrange('\x0F00', '\x0FFF', 70); 
			AddSubrange('\x1000', '\x109F', 74); 
			AddSubrange('\x10A0', '\x10FF', 26); 
			AddSubrange('\x1100', '\x11FF', 28); 
			AddSubrange('\x1200', '\x137F', 75); 
			AddSubrange('\x1380', '\x139F', 75); 
			AddSubrange('\x13A0', '\x13FF', 76); 
			AddSubrange('\x1400', '\x167F', 77); 
			AddSubrange('\x1680', '\x169F', 78); 
			AddSubrange('\x16A0', '\x16FF', 79); 
			AddSubrange('\x1700', '\x171F', 84); 
			AddSubrange('\x1720', '\x173F', 84); 
			AddSubrange('\x1740', '\x175F', 84); 
			AddSubrange('\x1760', '\x177F', 84); 
			AddSubrange('\x1780', '\x17FF', 80); 
			AddSubrange('\x1800', '\x18AF', 81); 
			AddSubrange('\x1900', '\x194F', 93); 
			AddSubrange('\x1950', '\x197F', 94); 
			AddSubrange('\x1980', '\x19DF', 95); 
			AddSubrange('\x19E0', '\x19FF', 80); 
			AddSubrange('\x1A00', '\x1A1F', 96); 
			AddSubrange('\x1B00', '\x1B7F', 27); 
			AddSubrange('\x1B80', '\x1BBF', 112); 
			AddSubrange('\x1C00', '\x1C4F', 113); 
			AddSubrange('\x1C50', '\x1C7F', 114); 
			AddSubrange('\x1D00', '\x1D7F', 4); 
			AddSubrange('\x1D80', '\x1DBF', 4); 
			AddSubrange('\x1DC0', '\x1DFF', 6); 
			AddSubrange('\x1E00', '\x1EFF', 29); 
			AddSubrange('\x1F00', '\x1FFF', 30); 
			AddSubrange('\x2000', '\x206F', 31); 
			AddSubrange('\x2070', '\x209F', 32); 
			AddSubrange('\x20A0', '\x20CF', 33); 
			AddSubrange('\x20D0', '\x20FF', 34); 
			AddSubrange('\x2100', '\x214F', 35); 
			AddSubrange('\x2150', '\x218F', 36); 
			AddSubrange('\x2190', '\x21FF', 37); 
			AddSubrange('\x2200', '\x22FF', 38); 
			AddSubrange('\x2300', '\x23FF', 39); 
			AddSubrange('\x2400', '\x243F', 40); 
			AddSubrange('\x2440', '\x245F', 41); 
			AddSubrange('\x2460', '\x24FF', 42); 
			AddSubrange('\x2500', '\x257F', 43); 
			AddSubrange('\x2580', '\x259F', 44); 
			AddSubrange('\x25A0', '\x25FF', 45); 
			AddSubrange('\x2600', '\x26FF', 46); 
			AddSubrange('\x2700', '\x27BF', 47); 
			AddSubrange('\x27C0', '\x27EF', 38); 
			AddSubrange('\x27F0', '\x27FF', 37); 
			AddSubrange('\x2800', '\x28FF', 82); 
			AddSubrange('\x2900', '\x297F', 37); 
			AddSubrange('\x2980', '\x29FF', 38); 
			AddSubrange('\x2A00', '\x2AFF', 38); 
			AddSubrange('\x2B00', '\x2BFF', 37); 
			AddSubrange('\x2C00', '\x2C5F', 97); 
			AddSubrange('\x2C60', '\x2C7F', 29); 
			AddSubrange('\x2C80', '\x2CFF', 8); 
			AddSubrange('\x2D00', '\x2D2F', 26); 
			AddSubrange('\x2D30', '\x2D7F', 98); 
			AddSubrange('\x2D80', '\x2DDF', 75); 
			AddSubrange('\x2DE0', '\x2DFF', 9); 
			AddSubrange('\x2E00', '\x2E7F', 31); 
			AddSubrange('\x2E80', '\x2EFF', 59); 
			AddSubrange('\x2F00', '\x2FDF', 59); 
			AddSubrange('\x2FF0', '\x2FFF', 59); 
			AddSubrange('\x3000', '\x303F', 48); 
			AddSubrange('\x3040', '\x309F', 49); 
			AddSubrange('\x30A0', '\x30FF', 50); 
			AddSubrange('\x3100', '\x312F', 51); 
			AddSubrange('\x3130', '\x318F', 52); 
			AddSubrange('\x3190', '\x319F', 59); 
			AddSubrange('\x31A0', '\x31BF', 51); 
			AddSubrange('\x31C0', '\x31EF', 61); 
			AddSubrange('\x31F0', '\x31FF', 50); 
			AddSubrange('\x3200', '\x32FF', 54); 
			AddSubrange('\x3300', '\x33FF', 55); 
			AddSubrange('\x3400', '\x4DBF', 59); 
			AddSubrange('\x4DC0', '\x4DFF', 99); 
			AddSubrange('\x4E00', '\x9FFF', 59); 
			AddSubrange('\xA000', '\xA48F', 83); 
			AddSubrange('\xA490', '\xA4CF', 83); 
			AddSubrange('\xA500', '\xA63F', 12); 
			AddSubrange('\xA640', '\xA69F', 9); 
			AddSubrange('\xA700', '\xA71F', 5); 
			AddSubrange('\xA720', '\xA7FF', 29); 
			AddSubrange('\xA800', '\xA82F', 100); 
			AddSubrange('\xA840', '\xA87F', 53); 
			AddSubrange('\xA880', '\xA8DF', 115); 
			AddSubrange('\xA900', '\xA92F', 116); 
			AddSubrange('\xA930', '\xA95F', 117); 
			AddSubrange('\xAA00', '\xAA5F', 118); 
			AddSubrange('\xAC00', '\xD7AF', 56); 
			AddSubrange('\xD800', '\xDFFF', 57); 
			AddSubrange('\xE000', '\xF8FF', 60); 
			AddSubrange('\xF900', '\xFAFF', 61); 
			AddSubrange('\xFB00', '\xFB4F', 62); 
			AddSubrange('\xFB50', '\xFDFF', 63); 
			AddSubrange('\xFE00', '\xFE0F', 91); 
			AddSubrange('\xFE10', '\xFE1F', 65); 
			AddSubrange('\xFE20', '\xFE2F', 64); 
			AddSubrange('\xFE30', '\xFE4F', 65); 
			AddSubrange('\xFE50', '\xFE6F', 66); 
			AddSubrange('\xFE70', '\xFEFF', 67); 
			AddSubrange('\xFF00', '\xFFEF', 68); 
			AddSubrange('\xFFF0', '\xFFFF', 69); 
		}
		#endregion
		protected internal virtual void AddSubrange(char low, char hi, int bit) {
			UnicodeSubrange subrange = new UnicodeSubrange(low, hi, bit);
			ranges.Add(subrange);
		}
		public UnicodeSubrange LookupSubrange(char ch) {
			int index = Algorithms.BinarySearch(ranges, new UnicodeSubrangeAndCharComparer(ch));
			if (index <= 0)
				return null;
			else
				return ranges[index];
		}
		public List<UnicodeSubrange> GetSubranges(BitArray bitArray) {
			List<UnicodeSubrange> result = new List<UnicodeSubrange>();
			int count = bitArray.Length;
			for (int i = 0; i < count; i++) {
				if(!bitArray[i])
					continue;
				UnicodeSubrange subrange;
				if(bitToSubrangeMap.TryGetValue(i, out subrange))
					result.Add(subrange);
			}
			return result;
		}
	}
	#endregion
	#region UnicodeSubrangeAndCharComparer
	public class UnicodeSubrangeAndCharComparer : IComparable<UnicodeSubrange> {
		readonly char ch;
		public UnicodeSubrangeAndCharComparer(char ch) {
			this.ch = ch;
		}
		#region IComparable<UnicodeSubrange> Members
		public int CompareTo(UnicodeSubrange other) {
			if (ch < other.LowValue)
				return 1;
			else if (ch > other.HiValue)
				return -1;
			else
				return 0;
		}
		#endregion
	}
	#endregion
}
