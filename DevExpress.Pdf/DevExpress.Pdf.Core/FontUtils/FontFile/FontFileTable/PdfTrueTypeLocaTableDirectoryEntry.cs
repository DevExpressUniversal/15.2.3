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

namespace DevExpress.Pdf.Native {
	public class PdfTrueTypeLocaTableDirectoryEntry : PdfFontTableDirectoryEntry {
		public const string EntryTag = "loca";
		bool isShortFormat;
		int[] glyphOffsets;
		bool shouldWrite;
		public int[] GlyphOffsets { 
			get { return glyphOffsets; } 
			set {
				glyphOffsets = value;
				shouldWrite = true;
			}
		}
		public PdfTrueTypeLocaTableDirectoryEntry(byte[] tableData) : base(EntryTag, tableData) {
		}
		public void ReadOffsets(PdfFontFile fontFile) {
			PdfFontHeadTableDirectoryEntry head = fontFile.Head;
			isShortFormat = head != null && head.IndexToLocFormat == PdfIndexToLocFormat.Short;
			int offsetCount = isShortFormat ? TableData.Length / 2 : TableData.Length / 4;
			if (offsetCount > 1) {
				PdfFontMaxpTableDirectoryEntry maxp = fontFile.Maxp;
				if (maxp != null) {
					int maxpCount = maxp.NumGlyphs + 1;
					if (offsetCount > maxpCount)
						offsetCount = maxpCount;
					else if (offsetCount < maxpCount)
						maxp.NumGlyphs = offsetCount - 1;
				}
				PdfBinaryStream tableStream = TableStream;
				tableStream.Position = 0;
				glyphOffsets = new int[offsetCount];
				if (isShortFormat)
					for (int i = 0; i < offsetCount; i++)
						glyphOffsets[i] = tableStream.ReadUshort() * 2;
				else
					for (int i = 0; i < offsetCount; i++)
						glyphOffsets[i] = tableStream.ReadInt();
			}
			else
				glyphOffsets = new int[0];
		}
		protected override void ApplyChanges() {
			base.ApplyChanges();
			if (shouldWrite) {
				PdfBinaryStream tableStream = CreateNewStream();
				int count = glyphOffsets.Length;
				if (isShortFormat)
					for (int i = 0; i < count; i++)
						tableStream.WriteShort((short)(glyphOffsets[i] / 2));
				else
					for (int i = 0; i < count; i++)
						tableStream.WriteInt(glyphOffsets[i]);
			}
		}
	}
}
