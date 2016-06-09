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
	public class PdfFontHheaTableDirectoryEntry : PdfFontTableDirectoryEntry {
		internal const string EntryTag = "hhea";
		readonly int version;
		readonly short ascender;
		readonly short descender;
		readonly short lineGap;
		readonly short advanceWidthMax;
		readonly short minLeftSideBearing;
		readonly short minRightSideBearing;
		readonly short xMaxExtent;
		readonly short caretSlopeRise;
		readonly short caretSlopeRun;
		readonly short metricDataFormat;
		readonly int numberOfHMetrics;
		bool shouldWrite;
		public int Version { get { return version; } }
		public short Ascender { get { return ascender; } }
		public short Descender { get { return descender; } }
		public short LineGap { get { return lineGap; } }
		public short AdvanceWidthMax { get { return advanceWidthMax; } }
		public short MinLeftSideBearing { get { return minLeftSideBearing; } }
		public short MinRightSideBearing { get { return minRightSideBearing; } }
		public short XMaxExtent { get { return xMaxExtent; } }
		public short CaretSlopeRise { get { return caretSlopeRise; } }
		public short CaretSlopeRun { get { return caretSlopeRun; } }
		public short MetricDataFormat { get { return metricDataFormat; } }
		public int NumberOfHMetrics { get { return numberOfHMetrics; } }
		public PdfFontHheaTableDirectoryEntry(byte[] tableData) : base(EntryTag, tableData) {
			PdfBinaryStream stream = TableStream;
			version = stream.ReadInt();
			ascender = stream.ReadShort();
			descender = stream.ReadShort();
			lineGap = stream.ReadShort();
			advanceWidthMax = stream.ReadShort();
			minLeftSideBearing = stream.ReadShort();
			minRightSideBearing = stream.ReadShort();
			xMaxExtent = stream.ReadShort();
			caretSlopeRise = stream.ReadShort();
			caretSlopeRun = stream.ReadShort();
			stream.ReadShort();
			stream.ReadShort();
			stream.ReadShort();
			stream.ReadShort();
			stream.ReadShort();
			metricDataFormat = stream.ReadShort();
			numberOfHMetrics = stream.ReadUshort();
		}
		public PdfFontHheaTableDirectoryEntry(IFont font) : base(EntryTag) {
			version = 0x10000;
			double italicAngle;
			PdfFontDescriptor fontDescriptor = font.FontDescriptor;
			if (fontDescriptor == null) {
				ascender = 0;
				descender = 0;
				italicAngle = 0;
			}
			else {
				ascender = (short)fontDescriptor.ActualAscent;
				descender = (short)fontDescriptor.ActualDescent;
				italicAngle = Math.Abs(fontDescriptor.ItalicAngle);			
			}
			short em = (short)(ascender - descender);
			lineGap = (short)(TypoLineGapRatio * em);
			double minWidth = Double.MaxValue;
			double maxWidth = 0;
			foreach (double width in font.GlyphWidths)
				if (width != 0) {
					minWidth = Math.Min(width, minWidth);
					maxWidth = Math.Max(width, maxWidth);
				}
			advanceWidthMax = (short)maxWidth;
			minLeftSideBearing = 0;
			minRightSideBearing = 0;
			xMaxExtent = (short)minWidth;
			caretSlopeRise = italicAngle == 0 ? (short)1 : Convert.ToInt16(em * Math.Sin((90 - italicAngle) * Ratio));
			caretSlopeRun = Convert.ToInt16(em * Math.Sin(italicAngle * Ratio));
			metricDataFormat = 0;
			numberOfHMetrics = font.FontFileEncoding.Count + 1;
			shouldWrite = true;
		}
		protected override void ApplyChanges() {
			base.ApplyChanges();
			if (shouldWrite) {
				PdfBinaryStream stream = CreateNewStream();
				stream.WriteInt(version);
				stream.WriteShort(ascender);
				stream.WriteShort(descender);
				stream.WriteShort(lineGap);
				stream.WriteShort(advanceWidthMax);
				stream.WriteShort(minLeftSideBearing);
				stream.WriteShort(minRightSideBearing);
				stream.WriteShort(xMaxExtent);
				stream.WriteShort(caretSlopeRise);
				stream.WriteShort(caretSlopeRun);
				stream.WriteShort(0);
				stream.WriteShort(0);
				stream.WriteShort(0);
				stream.WriteShort(0);
				stream.WriteShort(0);
				stream.WriteShort(metricDataFormat);
				stream.WriteShort((short)numberOfHMetrics);
			}
		}
	}
}
