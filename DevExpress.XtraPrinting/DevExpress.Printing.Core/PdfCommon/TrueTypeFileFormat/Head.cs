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
	class TTFHead : TTFTable {
		byte[] tableVersion;
		byte[] fontRevision;
		uint checkSumAdjustment;
		uint magicNumber;
		ushort flags;
		ushort unitsPerEm;
		byte[] created;
		byte[] modified;
		short xMin;
		short yMin;
		short xMax;
		short yMax;
		ushort macStyle;
		ushort lowestRecPPEM;
		short fontDirectionHint;
		short indexToLocFormat;
		short glyphDataFormat;
		public ushort Flags { get { return flags; } }
		public ushort UnitsPerEm { get { return unitsPerEm; } }
		public short XMin { get { return xMin; } }
		public short YMin { get { return yMin; } }
		public short XMax { get { return xMax; } }
		public short YMax { get { return yMax; } }
		public short IndexToLocFormat { get { return indexToLocFormat; } }
		public override int Length { get { return TTFHead.SizeOf; } }
		public static int SizeOf {
			get {
				return
					TTFStream.SizeOf_Fixed * 2 +
					TTFStream.SizeOf_ULong * 2 +
					TTFStream.SizeOf_UShort * 2 +
					TTFStream.SizeOf_InternationalDate * 2 +
					TTFStream.SizeOf_FWord * 4 +
					TTFStream.SizeOf_UShort * 2 +
					TTFStream.SizeOf_Short * 3;
			}
		}
		public TTFHead(TTFFile ttfFile)
			: base(ttfFile) {
		}
		protected override void ReadTable(TTFStream ttfStream) {
			tableVersion = ttfStream.ReadBytes(TTFStream.SizeOf_Fixed);
			fontRevision = ttfStream.ReadBytes(TTFStream.SizeOf_Fixed);
			checkSumAdjustment = ttfStream.ReadULong();
			magicNumber = ttfStream.ReadULong();
			flags = ttfStream.ReadUShort();
			unitsPerEm = ttfStream.ReadUShort();
			created = ttfStream.ReadBytes(TTFStream.SizeOf_InternationalDate);
			modified = ttfStream.ReadBytes(TTFStream.SizeOf_InternationalDate);
			xMin = ttfStream.ReadFWord();
			yMin = ttfStream.ReadFWord();
			xMax = ttfStream.ReadFWord();
			yMax = ttfStream.ReadFWord();
			macStyle = ttfStream.ReadUShort();
			lowestRecPPEM = ttfStream.ReadUShort();
			fontDirectionHint = ttfStream.ReadShort();
			indexToLocFormat = ttfStream.ReadShort();
			glyphDataFormat = ttfStream.ReadShort();
		}
		protected override void WriteTable(TTFStream ttfStream) {
			ttfStream.WriteBytes(tableVersion);
			ttfStream.WriteBytes(fontRevision);
			ttfStream.WriteULong(checkSumAdjustment);
			ttfStream.WriteULong(magicNumber);
			ttfStream.WriteUShort(flags);
			ttfStream.WriteUShort(unitsPerEm);
			ttfStream.WriteBytes(created);
			ttfStream.WriteBytes(modified);
			ttfStream.WriteFWord(xMin);
			ttfStream.WriteFWord(yMin);
			ttfStream.WriteFWord(xMax);
			ttfStream.WriteFWord(yMax);
			ttfStream.WriteUShort(macStyle);
			ttfStream.WriteUShort(lowestRecPPEM);
			ttfStream.WriteShort(fontDirectionHint);
			ttfStream.WriteShort(indexToLocFormat);
			ttfStream.WriteShort(glyphDataFormat);
		}
		protected override void InitializeTable(TTFTable pattern, TTFInitializeParam param) {
			TTFHead p = pattern as TTFHead;
			tableVersion = new Byte[p.tableVersion.Length];
			p.tableVersion.CopyTo(tableVersion, 0);
			fontRevision = new byte[p.fontRevision.Length];
			p.fontRevision.CopyTo(fontRevision, 0);
			checkSumAdjustment = 0;
			magicNumber = p.magicNumber;
			flags = p.flags;
			unitsPerEm = p.UnitsPerEm;
			created = new byte[p.created.Length];
			p.created.CopyTo(created, 0);
			modified = new byte[p.modified.Length];
			p.modified.CopyTo(modified, 0);
			xMin = p.XMin;
			yMin = p.YMin;
			xMax = p.XMax;
			yMax = p.YMax;
			macStyle = p.macStyle;
			lowestRecPPEM = p.lowestRecPPEM;
			fontDirectionHint = p.fontDirectionHint;
			indexToLocFormat = p.indexToLocFormat;
			glyphDataFormat = p.glyphDataFormat;
		}
		protected internal override string Tag { get { return "head"; } }
		public void WriteCheckSumAdjustment(TTFStream ttfStream) {
			if(Entry == null) return;
			ttfStream.Seek(Entry.Offset);
			ttfStream.Move(TTFStream.SizeOf_Fixed * 2);
			if(checkSumAdjustment == 0) {
				checkSumAdjustment = TTFUtils.CalculateCheckSum(ttfStream, 0, ttfStream.Length);
				checkSumAdjustment = 0xB1B0AFBA - checkSumAdjustment;
			}
			ttfStream.WriteULong(checkSumAdjustment);
		}
	}
}
