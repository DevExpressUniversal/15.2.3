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
namespace DevExpress.Pdf.Native {
	public class PdfFontKernTableDirectoryEntry : PdfFontTableDirectoryEntry {
		public const string EntryTag = "kern";
		Dictionary<int, short> kerning = new Dictionary<int, short>();
		public PdfFontKernTableDirectoryEntry(byte[] tableData) : base(EntryTag, tableData) {
			PdfBinaryStream stream = TableStream;
			stream.ReadUshort();
			int tablesCount = stream.ReadUshort();
			long tableStart = stream.Position;
			long lastTableLength = 0;
			for (int i = 0; i < tablesCount; i++) {
				tableStart += lastTableLength;
				stream.Position = tableStart + 2;
				lastTableLength = stream.ReadUshort();
				int format = stream.ReadUshort() & 0xFFF7;
				if (format == 1) {
					int pairCount = stream.ReadUshort();
					stream.ReadArray(6);
					for (int j = 0; j < pairCount; j++) {
						int pair = stream.ReadInt();
						if (!kerning.ContainsKey(pair))
							kerning[pair] = stream.ReadShort();
					}
				}
			}
		}
		public short GetKerning(int glyphIndex1, int glyphIndex2) {
			int key = (glyphIndex1 << 16) + glyphIndex2;
			short value = 0;
			if (kerning.TryGetValue(key, out value))
				return value;
			return 0;
		}
	}
}
