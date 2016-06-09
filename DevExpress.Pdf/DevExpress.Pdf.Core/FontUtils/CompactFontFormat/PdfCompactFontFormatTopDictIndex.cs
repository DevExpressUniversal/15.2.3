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
	public class PdfCompactFontFormatTopDictIndex : PdfCompactFontFormatIndex {
		const string operatorCharset = "charset";
		const string operatorCharStrings = "CharStrings";
		const string operatorROS = "ROS";
		static readonly string[] singleByteOperators = new string[] { "version", "Notice", "FullName", "FamilyName", "Weight", "FontBBox", "BlueValues", "OtherBlues", 
																	  "FamilyBlues", "FamilyOtherBlues", "StdHW", "StdVW", "", "UniqueID", "XUID", operatorCharset, 
																	  "Encoding", operatorCharStrings, "Private", "Subrs", "defaultWidthX", "nominalWidthX" };
		static readonly string[] doubleByteOperators = new string[] { "Copyright", "isFixedPitch", "ItalicAngle", "UnderlinePosition", "UnderlineThickness", "PaintType", "CharstringType", "FontMatrix",
																	   "StrokeWidth", "", "", "", "", "", "", "", "", "", "", "", "SyntheticBase", "PostScript", "BaseFontName", "BaseFontBlend", "", "", "", 
																	   "", "", "", operatorROS, "CIDFontVersion", "CIDFontRevision", "CIDFontType", "CIDCount", "UIDBase", "FDArray", "FDSelect", "FontName" };
		readonly Dictionary<string, List<int>> dictionary = new Dictionary<string, List<int>>();
		public int CharSet { get { return GetValue(operatorCharset); } }
		public int CharStrings { get { return GetValue(operatorCharStrings); } }
		public bool IsIdentityEncoding { get { return dictionary.ContainsKey(operatorROS); } }
		public PdfCompactFontFormatTopDictIndex(PdfBinaryStream stream) : base(stream) {
			IList<byte[]> objects = Objects;
			if (objects.Count > 0) {
				byte[] data = objects[0];
				int operatorsCount = singleByteOperators.Length;
				List<int> operands = new List<int>();
				int length = data.Length;
				for (int i = 0; i < length;) {
					int value = data[i++];
					if (value < operatorsCount) {
						string oper = singleByteOperators[value];
						if (String.IsNullOrEmpty(oper))
							oper = doubleByteOperators[data[i++]];
						dictionary[oper] =  operands;
						operands = new List<int>();
					}
					else if (value == 28) {
						int operand = data[i++] << 8;
						operands.Add(operand + data[i++]);
					}
					else if (value == 29) {
						int operand = data[i++] << 24;
						operand += data[i++] << 16;
						operand += data[i++] << 8;
						operands.Add(operand + data[i++]);
					}
					else if (value == 30) {
						int dataLength = data.Length;
						int operand = data[i++];
						for (int j = 0; j < dataLength && (operand & 0x0f) != 0x0f; j++) 
							operand = data[i++];
					}
					else if (value >= 32 && value <= 246)
						operands.Add(value - 139);
					else if (value >= 247 && value <= 250)
						operands.Add((value - 247) * 256 + data[i++] + 108);
					else if (value >= 251 && value <= 254)
						operands.Add((251 - value) * 256 - data[i++] - 108);
				}
			}
		}
		int GetValue(string operatorName) {
			List<int> operands;
			if (!dictionary.TryGetValue(operatorName, out operands))
				return 0;
			int operandsCount = operands.Count;
			return operandsCount == 0 ? 0 : operands[operandsCount - 1];
		}
	}
}
