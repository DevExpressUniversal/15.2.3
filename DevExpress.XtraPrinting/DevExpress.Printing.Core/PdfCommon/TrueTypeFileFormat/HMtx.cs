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
	struct TTFHMtxEntry {
		public ushort AdvanceWidth;
		public short LeftSideBearing;
		public static int SizeOf {
			get {
				return
					TTFStream.SizeOf_UFWord +
					TTFStream.SizeOf_FWord;
			}
		}
	}
	class TTFHMtx : TTFTable {
		TTFHMtxEntry[] entries;
		protected internal override string Tag { get { return "hmtx"; } }
		public int Count { get { return entries.Length; } }
		public TTFHMtxEntry this[ushort index] { get { return entries[index]; } }
		public override int Length { get { return TTFHMtxEntry.SizeOf * Count; } }
		public TTFHMtx(TTFFile ttfFile)
			: base(ttfFile) {
		}
		protected override void ReadTable(TTFStream ttfStream) {
			int size = Math.Max(Owner.MaxP.NumGlyphs, Owner.HHea.NumberOfHMetrics);
			entries = new TTFHMtxEntry[size];
			for(int i = 0; i < Owner.HHea.NumberOfHMetrics; i++) {
				entries[i].AdvanceWidth = ttfStream.ReadUFWord();
				entries[i].LeftSideBearing = ttfStream.ReadFWord();
			}
			ushort lastAdvanceWidth = entries[Owner.HHea.NumberOfHMetrics - 1].AdvanceWidth;
			for(int i = Owner.HHea.NumberOfHMetrics; i < size; i++) {
				entries[i].AdvanceWidth = lastAdvanceWidth;
				entries[i].LeftSideBearing = ttfStream.ReadFWord();
			}
		}
		protected override void WriteTable(TTFStream ttfStream) {
			int size = Math.Max(Owner.MaxP.NumGlyphs, Owner.HHea.NumberOfHMetrics);
			for(int i = 0; i < Owner.HHea.NumberOfHMetrics; i++) {
				ttfStream.WriteUFWord(entries[i].AdvanceWidth);
				ttfStream.WriteFWord(entries[i].LeftSideBearing);
			}
			for(int i = Owner.HHea.NumberOfHMetrics; i < size; i++)
				ttfStream.WriteFWord(entries[i].LeftSideBearing);
		}
		protected override void InitializeTable(TTFTable pattern, TTFInitializeParam param) {
			TTFHMtx p = pattern as TTFHMtx;
			entries = new TTFHMtxEntry[p.entries.Length];
			p.entries.CopyTo(entries, 0);
		}
	}
}
