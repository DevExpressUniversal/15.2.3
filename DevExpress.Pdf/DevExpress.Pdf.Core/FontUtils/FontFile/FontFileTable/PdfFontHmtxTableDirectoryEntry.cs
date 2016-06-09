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
namespace DevExpress.Pdf.Native {
	public class PdfFontHmtxTableDirectoryEntry : PdfFontTableDirectoryEntry {
		public const string EntryTag = "hmtx";
		short[] advanceWidths;
		public short[] AdvanceWidths { get { return advanceWidths; } }
		public PdfFontHmtxTableDirectoryEntry(byte[] tableData) : base(EntryTag, tableData) {
		}
		public PdfFontHmtxTableDirectoryEntry(IFont font) : base(EntryTag) {
			PdfBinaryStream stream = TableStream;
			int count = (font.FontFileEncoding.Count + 1) * 4;
			for (int i = 0; i < count; i++)
				stream.WriteByte(0);
		}
		public short[] FillAdvanceWidths(int hMetricsCount, int glyphsCount) {
			PdfBinaryStream tableStream = TableStream;
			tableStream.Position = 0;
			int size = Math.Max(hMetricsCount, glyphsCount);
			advanceWidths = new short[size];
			if (tableStream.Length >= hMetricsCount * 4) {
				for (int i = 0; i < hMetricsCount; i++) {
					advanceWidths[i] = tableStream.ReadShort();
					tableStream.ReadShort();
				}
				short lastAdvanceWidth = advanceWidths[hMetricsCount - 1];
				for (int i = hMetricsCount; i < size; i++)
					advanceWidths[i] = lastAdvanceWidth;
			}
			return advanceWidths;
		}
	}
}
