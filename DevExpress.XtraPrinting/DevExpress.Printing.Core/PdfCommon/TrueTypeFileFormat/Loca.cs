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
	class TTFLoca : TTFTable {
		uint[] offsets;
		protected internal override string Tag { get { return "loca"; } }
		public int Count { get { return offsets.Length; } }
		public uint this[ushort index] { get { return offsets[index]; } }
		public override int Length {
			get {
				int itemSize = (Owner.Head.IndexToLocFormat == 1) ? TTFStream.SizeOf_ULong : TTFStream.SizeOf_UShort;
				return itemSize * Count;
			}
		}
		public TTFLoca(TTFFile ttfFile)
			: base(ttfFile) {
		}
		protected override void ReadTable(TTFStream ttfStream) {
			offsets = new uint[Owner.MaxP.NumGlyphs + 1];
			for(int i = 0; i < Owner.MaxP.NumGlyphs + 1; i++)
				offsets[i] = (Owner.Head.IndexToLocFormat == 1) ? ttfStream.ReadULong() : Convert.ToUInt32(ttfStream.ReadUShort()) << 1;
		}
		protected override void WriteTable(TTFStream ttfStream) {
			for(int i = 0; i < Owner.MaxP.NumGlyphs + 1; i++) {
				if(Owner.Head.IndexToLocFormat == 1)
					ttfStream.WriteULong(offsets[i]);
				else
					ttfStream.WriteUShort(Convert.ToUInt16(offsets[i] >> 1));
			}
		}
		protected override void InitializeTable(TTFTable pattern, TTFInitializeParam param) {
			TTFLoca p = pattern as TTFLoca;
			offsets = new uint[p.Count];
			uint offset = 0;
			for(int i = 0; i < Owner.Glyf.Glyphs.Length; i++) {
				offsets[i] = offset;
				if(Owner.Glyf.Glyphs[i] != null)
					offset += (uint)Owner.Glyf.Glyphs[i].Size;
			}
			offsets[Owner.Glyf.Glyphs.Length] = offset;
		}
	}
}
