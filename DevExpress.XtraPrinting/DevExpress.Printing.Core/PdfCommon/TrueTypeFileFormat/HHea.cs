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
using System.Text;
namespace DevExpress.Pdf.Common {
	class TTFHHea : TTFTable {
		byte[] tableVersion;
		short ascender;
		short descender;
		short lineGap;
		ushort advanceWidthMax;
		short minLeftSideBearing;
		short minRightSideBearing;
		short xMaxExtent;
		short caretSlopeRise;
		short caretSlopeRun;
		byte[] reserved;
		short metricDataFormat;
		ushort numberOfHMetrics;
		protected internal override string Tag { get { return "hhea"; } }
		public short Ascender { get { return ascender; } }
		public short Descender { get { return descender; } }
		public int NumberOfHMetrics { get { return (int)numberOfHMetrics; } }
		public override int Length { get { return TTFHHea.SizeOf; } }
		public static int SizeOf {
			get {
				return
					TTFStream.SizeOf_Fixed +
					TTFStream.SizeOf_FWord * 3 +
					TTFStream.SizeOf_UFWord +
					TTFStream.SizeOf_FWord * 3 +
					TTFStream.SizeOf_Short * 8 +
					TTFStream.SizeOf_UShort;
			}
		}
		public TTFHHea(TTFFile ttfFile)
			: base(ttfFile) {
		}
		protected override void ReadTable(TTFStream ttfStream) {
			tableVersion = ttfStream.ReadBytes(TTFStream.SizeOf_Fixed);
			ascender = ttfStream.ReadFWord();
			descender = ttfStream.ReadFWord();
			lineGap = ttfStream.ReadFWord();
			advanceWidthMax = ttfStream.ReadUFWord();
			minLeftSideBearing = ttfStream.ReadFWord();
			minRightSideBearing = ttfStream.ReadFWord();
			xMaxExtent = ttfStream.ReadFWord();
			caretSlopeRise = ttfStream.ReadShort();
			caretSlopeRun = ttfStream.ReadShort();
			reserved = ttfStream.ReadBytes(TTFStream.SizeOf_Short * 5);
			metricDataFormat = ttfStream.ReadShort();
			numberOfHMetrics = ttfStream.ReadUShort();
		}
		protected override void WriteTable(TTFStream ttfStream) {
			ttfStream.WriteBytes(tableVersion);
			ttfStream.WriteFWord(ascender);
			ttfStream.WriteFWord(descender);
			ttfStream.WriteFWord(lineGap);
			ttfStream.WriteUFWord(advanceWidthMax);
			ttfStream.WriteFWord(minLeftSideBearing);
			ttfStream.WriteFWord(minRightSideBearing);
			ttfStream.WriteFWord(xMaxExtent);
			ttfStream.WriteShort(caretSlopeRise);
			ttfStream.WriteShort(caretSlopeRun);
			ttfStream.WriteBytes(reserved);
			ttfStream.WriteShort(metricDataFormat);
			ttfStream.WriteUShort(numberOfHMetrics);
		}
		protected override void InitializeTable(TTFTable pattern, TTFInitializeParam param) {
			TTFHHea p = pattern as TTFHHea;
			tableVersion = new byte[p.tableVersion.Length];
			p.tableVersion.CopyTo(tableVersion, 0);
			ascender = p.ascender;
			descender = p.descender;
			lineGap = p.lineGap;
			advanceWidthMax = p.advanceWidthMax;
			minLeftSideBearing = p.minLeftSideBearing;
			minRightSideBearing = p.minRightSideBearing;
			xMaxExtent = p.xMaxExtent;
			caretSlopeRise = p.caretSlopeRise;
			caretSlopeRun = p.caretSlopeRun;
			reserved = new byte[p.reserved.Length];
			p.reserved.CopyTo(reserved, 0);
			metricDataFormat = p.metricDataFormat;
			numberOfHMetrics = p.numberOfHMetrics;
		}
	}
}
