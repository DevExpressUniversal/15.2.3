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
	class TTFOS2 : TTFTable {
		public static int SizeOf {
			get {
				return
					TTFStream.SizeOf_UShort +
					TTFStream.SizeOf_Short +
					TTFStream.SizeOf_UShort * 2 +
					TTFStream.SizeOf_Short * 12 +
					TTFStream.SizeOf_PANOSE +
					TTFStream.SizeOf_ULong * 4 +
					TTFStream.SizeOf_Char * 4 +
					TTFStream.SizeOf_UShort * 8 +
					TTFStream.SizeOf_ULong * 2;
			}
		}
		short fsType;
		TTFPanose panose;
		public TTFPanose Panose { get { return panose; } }
		public short FsType { get { return fsType; } }
		public override int Length { get { return SizeOf; } }
		protected internal override string Tag { get { return "OS/2"; } }
		public TTFOS2(TTFFile ttfFile)
			: base(ttfFile) {
		}
		protected override void ReadTable(TTFStream ttfStream) {
			ttfStream.Move(TTFStream.SizeOf_UShort * 3 + TTFStream.SizeOf_Short);
			this.fsType = ttfStream.ReadShort();
			ttfStream.Move(TTFStream.SizeOf_Short * 11);
			this.panose = ttfStream.ReadPanose();
		}
		protected override void WriteTable(TTFStream ttfStream) {
			throw new TTFFileException("Not supported");
		}
		protected override void InitializeTable(TTFTable pattern, TTFInitializeParam param) {
			throw new TTFFileException("Not supported");
		}
	}
	public struct TTFPanose {
		public byte bFamilyType;
		public byte bSerifType;
		public byte bWeight;
		public byte bProportion;
		public byte bContrast;
		public byte bStrokeVariation;
		public byte bArmStyle;
		public byte bLetterForm;
		public byte bMidline;
		public byte bXHeight;
	}
}
