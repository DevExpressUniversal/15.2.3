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
	class TTFMaxP : TTFTable {
		byte[] tableVersion;
		ushort numGlyphs;
		ushort maxPoints;
		ushort maxContours;
		ushort maxCompositePoints;
		ushort maxCompositeContours;
		ushort maxZones;
		ushort maxTwilightPoints;
		ushort maxStorage;
		ushort maxFunctionDefs;
		ushort maxInstructionDefs;
		ushort maxStackElements;
		ushort maxSizeOfInstructions;
		ushort maxComponentElements;
		ushort maxComponentDepth;
		public int NumGlyphs { get { return Convert.ToInt32(numGlyphs); } }
		public override int Length { get { return TTFMaxP.SizeOf; } }
		public static int SizeOf {
			get {
				return
					TTFStream.SizeOf_Fixed +
					TTFStream.SizeOf_UShort * 14;
			}
		}
		protected internal override string Tag { get { return "maxp"; } }
		public TTFMaxP(TTFFile ttfFile)
			: base(ttfFile) {
		}
		protected override void ReadTable(TTFStream ttfStream) {
			tableVersion = ttfStream.ReadBytes(TTFStream.SizeOf_Fixed);
			numGlyphs = ttfStream.ReadUShort();
			maxPoints = ttfStream.ReadUShort();
			maxContours = ttfStream.ReadUShort();
			maxCompositePoints = ttfStream.ReadUShort();
			maxCompositeContours = ttfStream.ReadUShort();
			maxZones = ttfStream.ReadUShort();
			maxTwilightPoints = ttfStream.ReadUShort();
			maxStorage = ttfStream.ReadUShort();
			maxFunctionDefs = ttfStream.ReadUShort();
			maxInstructionDefs = ttfStream.ReadUShort();
			maxStackElements = ttfStream.ReadUShort();
			maxSizeOfInstructions = ttfStream.ReadUShort();
			maxComponentElements = ttfStream.ReadUShort();
			maxComponentDepth = ttfStream.ReadUShort();
		}
		protected override void WriteTable(TTFStream ttfStream) {
			ttfStream.WriteBytes(tableVersion);
			ttfStream.WriteUShort(numGlyphs);
			ttfStream.WriteUShort(maxPoints);
			ttfStream.WriteUShort(maxContours);
			ttfStream.WriteUShort(maxCompositePoints);
			ttfStream.WriteUShort(maxCompositeContours);
			ttfStream.WriteUShort(maxZones);
			ttfStream.WriteUShort(maxTwilightPoints);
			ttfStream.WriteUShort(maxStorage);
			ttfStream.WriteUShort(maxFunctionDefs);
			ttfStream.WriteUShort(maxInstructionDefs);
			ttfStream.WriteUShort(maxStackElements);
			ttfStream.WriteUShort(maxSizeOfInstructions);
			ttfStream.WriteUShort(maxComponentElements);
			ttfStream.WriteUShort(maxComponentDepth);
		}
		protected override void InitializeTable(TTFTable pattern, TTFInitializeParam param) {
			TTFMaxP p = pattern as TTFMaxP;
			tableVersion = new byte[p.tableVersion.Length];
			p.tableVersion.CopyTo(tableVersion, 0);
			numGlyphs = p.numGlyphs;
			maxPoints = p.maxPoints;
			maxContours = p.maxContours;
			maxCompositePoints = p.maxCompositePoints;
			maxCompositeContours = p.maxCompositeContours;
			maxZones = p.maxZones;
			maxTwilightPoints = p.maxTwilightPoints;
			maxStorage = p.maxStorage;
			maxFunctionDefs = p.maxFunctionDefs;
			maxInstructionDefs = p.maxInstructionDefs;
			maxStackElements = p.maxStackElements;
			maxSizeOfInstructions = p.maxSizeOfInstructions;
			maxComponentElements = p.maxComponentElements;
			maxComponentDepth = p.maxComponentDepth;
		}
	}
}
