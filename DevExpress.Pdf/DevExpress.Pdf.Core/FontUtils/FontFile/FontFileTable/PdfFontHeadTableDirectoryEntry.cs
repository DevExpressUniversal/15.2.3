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
	[Flags]
	public enum PdfFontHeadTableDirectoryEntryFlags { Empty = 0x0000, BaselineForFontAt0 = 0x0001, LeftSidebearingPointAt0 = 0x0002, InstructionsMayDependOnPointSize = 0x0004, 
													  ForcePPEMToIntegerValues = 0x0008, InstructionsMayAlterAdvanceWidth = 0x0008, FontDataIsLossless = 0x0800, 
													  ProduceCompatibleMetrics = 0x1000, OptimizedForClearType = 0x2000, LastResort = 0x4000 }
	[Flags]
	public enum PdfFontHeadTableDirectoryEntryMacStyle { Empty = 0x00, Bold = 0x01, Italic = 0x02, Underline = 0x04, Outline = 0x08, Shadow = 0x10, Condensed = 0x20, Extended = 0x40 }
	public enum PdfFontDirectionHint { FullyMixedDirectionalGlyphs = 0, OnlyStronglyLeftToRight = 1, OnlyStronglyLeftToRightButAlsoContainsNeutrals = 2, 
									   OnlyStronglyRightToLeft = -1, OnlyStronglyRightToLeftButAlsoContainsNeutrals = -2 }
	public enum PdfIndexToLocFormat { Short = 0, Long = 1 }
	public class PdfFontHeadTableDirectoryEntry : PdfFontTableDirectoryEntry {
		internal const string EntryTag = "head";
		const short defaultUnitsPerEm = 2048;
		static readonly DateTime minDateTime = new DateTime(1904, 1, 1);
		readonly int version;
		readonly int fontRevision;
		readonly int checkSumAdjustment;
		readonly int magicNumber;
		PdfFontHeadTableDirectoryEntryFlags flags;
		readonly short unitsPerEm = defaultUnitsPerEm;
		readonly long created;
		readonly long modified;
		readonly short xMin;
		readonly short yMin;
		readonly short xMax;
		readonly short yMax;
		readonly PdfFontHeadTableDirectoryEntryMacStyle macStyle;
		readonly short lowestRecPPEM;
		readonly PdfFontDirectionHint fontDirectionHint;
		readonly PdfIndexToLocFormat indexToLocFormat;
		readonly short glyphDataFormat;
		bool shouldWrite;
		public int Version { get { return version; } }
		public int FontRevision { get { return fontRevision; } }
		public int CheckSumAdjustment { get { return checkSumAdjustment; } }
		public int MagicNumber { get { return magicNumber; } }
		public PdfFontHeadTableDirectoryEntryFlags Flags { get { return flags; } }
		public short UnitsPerEm { get { return unitsPerEm; } }
		public long Created { get { return created; } }
		public long Modified { get { return modified; } }
		public short XMin { get { return xMin; } }
		public short YMin { get { return yMin; } }
		public short XMax { get { return xMax; } }
		public short YMax { get { return yMax; } }
		public PdfFontHeadTableDirectoryEntryMacStyle MacStyle { get { return macStyle; } }
		public short LowestRecPPEM { get { return lowestRecPPEM; } }
		public PdfFontDirectionHint FontDirectionHint { get { return fontDirectionHint; } }
		public PdfIndexToLocFormat IndexToLocFormat { get { return indexToLocFormat; } }
		public short GlyphDataFormat { get { return glyphDataFormat; } }
		public PdfFontHeadTableDirectoryEntry(byte[] tableData) : base(EntryTag, tableData) {
			PdfBinaryStream tableStream = TableStream;
			version = tableStream.ReadInt();
			fontRevision = tableStream.ReadInt();
			checkSumAdjustment = tableStream.ReadInt();
			magicNumber = tableStream.ReadInt();
			flags = (PdfFontHeadTableDirectoryEntryFlags)tableStream.ReadShort();
			unitsPerEm = tableStream.ReadShort();
			created = tableStream.ReadLong();
			modified = tableStream.ReadLong();
			xMin = tableStream.ReadShort();
			yMin = tableStream.ReadShort();
			xMax = tableStream.ReadShort();
			yMax = tableStream.ReadShort();
			macStyle = (PdfFontHeadTableDirectoryEntryMacStyle)tableStream.ReadShort();
			lowestRecPPEM = tableStream.ReadShort();
			fontDirectionHint = (PdfFontDirectionHint)tableStream.ReadShort();
			indexToLocFormat = (PdfIndexToLocFormat)tableStream.ReadShort();
			glyphDataFormat = tableStream.ReadShort();
		}
		public PdfFontHeadTableDirectoryEntry(IFont font) : base(EntryTag) {
			version = 0x00010000;
			fontRevision = 0;
			checkSumAdjustment = 0;
			magicNumber = 0x5F0F3CF5;
			flags = PdfFontHeadTableDirectoryEntryFlags.Empty;
			created = (DateTime.Now - minDateTime).Seconds;
			modified = created;
			PdfFontDescriptor fontDescriptor = font.FontDescriptor;
			if (fontDescriptor != null) {
				unitsPerEm = Convert.ToInt16(fontDescriptor.ActualAscent + fontDescriptor.ActualDescent);
				if (unitsPerEm == 0)
					unitsPerEm = defaultUnitsPerEm;
				PdfRectangle fontBBox = fontDescriptor.FontBBox;
				if (fontBBox != null) {
					xMin = Convert.ToInt16(fontBBox.Left);
					yMin = Convert.ToInt16(fontBBox.Bottom);
					xMax = Convert.ToInt16(fontBBox.Right);
					yMax = Convert.ToInt16(fontBBox.Top);
				}
			}
			if (xMin == xMax)
				xMax = (short)(xMin + CalcAverageGlyphWidth(font));
			lowestRecPPEM = 6;
			shouldWrite = true;
		}
		protected override void ApplyChanges() {
			base.ApplyChanges();
			if (shouldWrite) {
				PdfBinaryStream tableStream = CreateNewStream();
				tableStream.WriteInt(version);
				tableStream.WriteInt(fontRevision);
				tableStream.WriteInt(checkSumAdjustment);
				tableStream.WriteInt(magicNumber);
				tableStream.WriteShort((short)flags);
				tableStream.WriteShort(unitsPerEm);
				tableStream.WriteLong(created);
				tableStream.WriteLong(modified);
				tableStream.WriteShort(xMin);
				tableStream.WriteShort(yMin);
				tableStream.WriteShort(xMax);
				tableStream.WriteShort(yMax);
				tableStream.WriteShort((short)macStyle);
				tableStream.WriteShort(lowestRecPPEM);
				tableStream.WriteShort((short)fontDirectionHint);
				tableStream.WriteShort((short)indexToLocFormat);
				tableStream.WriteShort(glyphDataFormat);
			}
		}
	}
}
