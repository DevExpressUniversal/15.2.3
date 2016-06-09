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
	public class PdfFontTableDirectoryEntry : PdfDisposableObject {
		public static PdfFontTableDirectoryEntry Create(string tag, byte[] array) {
			if (array.Length == 0)
				return new PdfFontTableDirectoryEntry(tag, array);
			switch (tag) {
				case PdfFontHeadTableDirectoryEntry.EntryTag:
					return new PdfFontHeadTableDirectoryEntry(array);
				case PdfTrueTypeLocaTableDirectoryEntry.EntryTag:
					return new PdfTrueTypeLocaTableDirectoryEntry(array);
				case PdfTrueTypeGlyfTableDirectoryEntry.EntryTag:
					return new PdfTrueTypeGlyfTableDirectoryEntry(array);
				case PdfOpenTypeCFFTableDirectoryEntry.EntryTag:
					return new PdfOpenTypeCFFTableDirectoryEntry(array);
				case PdfFontNameTableDirectoryEntry.EntryTag:
					return new PdfFontNameTableDirectoryEntry(array);
				case PdfFontCmapTableDirectoryEntry.EntryTag:
					return new PdfFontCmapTableDirectoryEntry(array);
				case PdfFontMaxpTableDirectoryEntry.EntryTag:
					return new PdfFontMaxpTableDirectoryEntry(array);
				case PdfFontOS2TableDirectoryEntry.EntryTag:
					return new PdfFontOS2TableDirectoryEntry(array);
				case PdfFontPostTableDirectoryEntry.EntryTag:
					return new PdfFontPostTableDirectoryEntry(array);
				case PdfFontKernTableDirectoryEntry.EntryTag:
					return new PdfFontKernTableDirectoryEntry(array);
				case PdfFontHheaTableDirectoryEntry.EntryTag:
					return new PdfFontHheaTableDirectoryEntry(array);
				case PdfFontHmtxTableDirectoryEntry.EntryTag:
					return new PdfFontHmtxTableDirectoryEntry(array);
				default:
					return new PdfFontTableDirectoryEntry(tag, array);
			}
		}
		protected static short CalcAverageGlyphWidth(IFont font) {
			double sum = 0.0;
			int count = 0;
			foreach (double width in font.GlyphWidths)
				if (width != 0) {
					sum += width;
					count++;
				}
			return count == 0 ? (short)0 : Convert.ToInt16(Math.Ceiling(sum / count));
		}
		protected const double Ratio = Math.PI / 180;
		protected const double TypoLineGapRatio = 1.2;
		readonly string tag;
		PdfBinaryStream tableStream;
		protected PdfBinaryStream TableStream { get { return tableStream; } }
		public string Tag { get { return tag; } }
		public int Length { get { return (int)tableStream.Length; } }
		public byte[] TableData { get { return tableStream.Data; } }
		public PdfFontTableDirectoryEntry(string tag) {
			this.tag = tag;
			this.tableStream = new PdfBinaryStream();
		}
		public PdfFontTableDirectoryEntry(string tag, byte[] tableData) {
			this.tag = tag;
			tableStream = new PdfBinaryStream();
			tableStream.WriteArray(tableData);
			tableStream.Position = 0;
		}
		public int Write(PdfBinaryStream stream, int offset) {
			ApplyChanges();
			int length = Length;
			if (length == 0) {
				tableStream.WriteInt(0);
				length = 4;
			}
			int factor = (int)(length % 4);
			int additionalLength = 0;
			if (factor != 0) {
				additionalLength = 4 - factor;
				tableStream.Position = length;
				tableStream.WriteArray(new byte[additionalLength]);
			}
			stream.WriteString(tag);
			int checkSum = 0;
			byte[] tableData = TableData;
			int count = tableData.Length / 4;
			for (int i = 0, index = 0; i < count; i++) {
				int element = tableData[index++] << 24;
				element += tableData[index++] << 16;
				element += tableData[index++] << 8;
				element += tableData[index++];
				checkSum += element;
			}
			stream.WriteInt(checkSum);
			stream.WriteInt(offset);
			stream.WriteInt(length);
			return length + additionalLength;
		}
		protected PdfBinaryStream CreateNewStream() {
			tableStream.Dispose();
			tableStream = new PdfBinaryStream();
			return tableStream;
		}
		protected virtual void ApplyChanges() {
		}
		protected override void Dispose(bool disposing) {
			if (disposing)
				tableStream.Dispose();
		}
	}
}
