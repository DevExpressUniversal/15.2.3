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
using System.IO;
using System.Text;
using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	public class PdfCharacterMapping {
		public static byte[] CreateCharacterMappingData(IDictionary<int, string> charMap, string fontName) {
			using (MemoryStream ms = new MemoryStream()) {
				using (StreamWriter writer = new StreamWriter(ms)) {
					writer.WriteLine("/CIDInit /ProcSet findresource begin");
					writer.WriteLine("12 dict begin");
					writer.WriteLine("begincmap");
					writer.WriteLine("/CIDSystemInfo");
					writer.WriteLine("<<");
					writer.WriteLine("/Registry (Adobe)");
					writer.WriteLine("/Ordering (" + fontName + ")");
					writer.WriteLine("/Supplement 0");
					writer.WriteLine(">> def");
					writer.WriteLine("/CMapName /" + fontName + " def");
					writer.WriteLine("/CMapType 2 def");
					writer.WriteLine("1 begincodespacerange");
					writer.WriteLine("<0000> <FFFF>");
					writer.WriteLine("endcodespacerange");
					List<KeyValuePair<int, string>> chars = new List<KeyValuePair<int, string>>(charMap.Count);
					List<KeyValuePair<int, string>> ranges = new List<KeyValuePair<int, string>>();
					if (charMap.Count > 0) {
						foreach (KeyValuePair<int, string> pair in charMap)
							if (pair.Value.Length == 1)
								chars.Add(pair);
							else
								ranges.Add(pair);
						if (chars.Count > 0) {
							writer.Write(Convert.ToString(chars.Count));
							writer.WriteLine(" beginbfchar");
							foreach (KeyValuePair<int, string> pair in chars)
								writer.WriteLine("<" + pair.Key.ToString("X4") + "> <" + ((ushort)pair.Value[0]).ToString("X4") + ">");
							writer.WriteLine("endbfchar");
						}
						if (ranges.Count > 0) {
							writer.Write(Convert.ToString(ranges.Count));
							writer.WriteLine(" beginbfrange");
							foreach (KeyValuePair<int, string> pair in ranges) {
								string glyph = pair.Key.ToString("X4");
								writer.Write("<" + glyph + "><" + glyph + "><");
								foreach (char c in pair.Value)
									writer.Write(((ushort)c).ToString("X4"));
								writer.WriteLine(">");
							}
							writer.WriteLine("endbfrange");
						}
					}
					writer.WriteLine("endcmap");
					writer.WriteLine("CMapName currentdict /CMap defineresource pop");
					writer.WriteLine("end");
					writer.WriteLine("end");
				}
				return ms.ToArray();
			}
		}
		readonly byte[] data;
		readonly PdfCharacterMappingTreeBranch mappingTree = new PdfCharacterMappingTreeBranch();
		readonly int maxCodeLength;
		public byte[] Data { get { return data; } }
		internal PdfCharacterMapping(byte[] data, IDictionary<byte[], string> map) {
			this.data = data;
			foreach (KeyValuePair<byte[], string> pair in map) {
				byte[] code = pair.Key;
				maxCodeLength = Math.Max(maxCodeLength, code.Length);
				mappingTree.Fill(code, 0, pair.Value);
			}
		}
		internal string MapCode(byte[] code) {
			PdfCharacterMappingFindResult findResult = Find(code, 0);
			if (findResult.CodeLength > 0)
				return findResult.Value;
			return (code.Length == 1 && code[0] == 0) ? String.Empty : Encoding.BigEndianUnicode.GetString(code, 0, code.Length);
		}
		internal PdfStringData GetStringData(byte[] data, double[] glyphOffsets) {
			List<byte[]> codes = new List<byte[]>();
			List<double> actualOffsets = new List<double>();
			List<short> result = new List<short>();
			int length = data.Length;
			for (int position = 0; position < length;) {
				PdfCharacterMappingFindResult findResult = Find(data, position);
				int codeLength = findResult.CodeLength;
				if (codeLength == 0) {
					result.Add((short)32);
					codeLength = 1;
				}
				else
					result.Add((short)findResult.Value[0]);
				byte[] code = new byte[codeLength];
				Array.Copy(data, position, code, 0, codeLength);
				codes.Add(code);
				if (glyphOffsets != null)
					actualOffsets.Add(glyphOffsets[position]);
				position += codeLength;
			}
			double[] locations;			
			if (glyphOffsets != null) {
				actualOffsets.Add(0);
				locations = actualOffsets.ToArray();
			}
			else
				locations = new double[codes.Count + 1];
			return new PdfStringData(codes.ToArray(), result.ToArray(), locations);
		}
		PdfCharacterMappingFindResult Find(byte[] code, int position) {
			PdfCharacterMappingFindResult result = mappingTree.Find(code, position);
			int codeLength = code.Length - position;
			if (result.CodeLength > 0 || codeLength >= maxCodeLength)
				return result;
			int newCodeLength = codeLength + 1;
			byte[] newCode = new byte[newCodeLength];
			Array.Copy(code, position, newCode, 1, codeLength);
			result = Find(newCode, 0);
			return new PdfCharacterMappingFindResult(result.Value, codeLength);
		}
	}
}
