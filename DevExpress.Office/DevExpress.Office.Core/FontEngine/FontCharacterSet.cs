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
using DevExpress.Office.Utils;
namespace DevExpress.Office.Drawing {
	#region FontCharacterSet
	public class FontCharacterSet {
		public const int panoseLength = 10;
		const int unicodeCharacterCount = 65536;
		BitArray bitArray;
		byte[] panose;
		public FontCharacterSet(List<FontCharacterRange> ranges, byte[] panose) {
			if (panose.Length != panoseLength)
				Exceptions.ThrowArgumentException("panose", panose);
			this.panose = panose;
			this.bitArray = new BitArray(unicodeCharacterCount);
			int count = ranges.Count;
			for (int i = 0; i < count; i++)
				AddRange(ranges[i]);
		}
		internal BitArray BitArray { get { return bitArray; } }
		void AddRange(FontCharacterRange fontCharacterRange) {
			for (int i = fontCharacterRange.Low; i <= fontCharacterRange.Hi; i++)
				bitArray[i] = true;
		}
		public bool ContainsChar(char ch) {
			return bitArray[ch];
		}
		public static int CalculatePanoseDistance(FontCharacterSet font1, FontCharacterSet font2) {
			byte[] panose1 = font1.panose;
			byte[] panose2 = font2.panose;
			int count = panose1.Length;
			int sum = 0;
			for (int i = 0; i < count; i++) {
				int dist = panose1[i] - panose2[i];
				sum = sum + dist * dist;
			}
			return sum;
		}
	}
	#endregion
}
