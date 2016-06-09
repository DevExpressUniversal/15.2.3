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

using DevExpress.XtraPrinting.Export.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Pdf.Common {
	abstract class PdfToUnicodeCMapBase : PdfDocumentStreamObject {
		public PdfToUnicodeCMapBase(bool compressed)
			: base(compressed) {
		}
		protected abstract string CreateCMapName();
		protected abstract PdfCharCache GetCharCache();
		public override void FillUp() {
			Stream.SetStringLine("/CIDInit /ProcSet findresource begin");
			Stream.SetStringLine("12 dict begin");
			Stream.SetStringLine("begincmap");
			Stream.SetStringLine("/CIDSystemInfo");
			Stream.SetStringLine("<<");
			Stream.SetStringLine("/Registry (Adobe)");
			Stream.SetStringLine("/Ordering (" + CreateCMapName() + ")");
			Stream.SetStringLine("/Supplement 0");
			Stream.SetStringLine(">> def");
			Stream.SetStringLine("/CMapName /" + CreateCMapName() + " def");
			Stream.SetStringLine("/CMapType 2 def");
			Stream.SetStringLine("1 begincodespacerange");
			Stream.SetStringLine("<0000> <FFFF>");
			Stream.SetStringLine("endcodespacerange");
			if(map == null) {
				FillUpCharMap();
			} else {
				Stream.SetString(map);
			}
			Stream.SetStringLine("endcmap");
			Stream.SetStringLine("CMapName currentdict /CMap defineresource pop");
			Stream.SetStringLine("end");
			Stream.SetStringLine("end");
		}
		public static string map;
		protected void FillUpCharMap() {
			var charMap = GetCharCache().GetCharMap();
			List<KeyValuePair<ushort, char[]>> chars = new List<KeyValuePair<ushort, char[]>>(charMap.Count);
			List<KeyValuePair<ushort, char[]>> ranges = new List<KeyValuePair<ushort, char[]>>();
			if(charMap.Count > 0) {
				foreach(var pair in charMap)
					if(pair.Value.Length == 1)
						chars.Add(pair);
					else
						ranges.Add(pair);
				if(chars.Count > 0) {
					Stream.SetString(Convert.ToString(chars.Count));
					Stream.SetStringLine(" beginbfchar");
					foreach(var pair in chars)
						Stream.SetStringLine("<" + pair.Key.ToString("X4") + "> <" + ((ushort)pair.Value[0]).ToString("X4") + ">");
					Stream.SetStringLine("endbfchar");
				}
				if(ranges.Count > 0) {
					Stream.SetString(Convert.ToString(ranges.Count));
					Stream.SetStringLine(" beginbfrange");
					foreach(var pair in ranges) {
						string glyph = pair.Key.ToString("X4");
						Stream.SetString("<" + glyph + "><" + glyph + "><");
						foreach(char c in pair.Value)
							Stream.SetString(((ushort)c).ToString("X4"));
						Stream.SetStringLine(">");
					}
					Stream.SetStringLine("endbfrange");
				}
			}
		}
	}
}
