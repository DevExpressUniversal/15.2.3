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
	class TTCFile {
		public TTCFile() {
		}
		public TTFFile Read(byte[] data, string fontName, int fontCodePage) {
			TTFStream ttfStream = new TTFStreamAsByteArray(data);
			TTCHeaderBase tccHeader = TTCHeaderBase.CreateTCCHeader(ttfStream);
			tccHeader.Read(ttfStream);
			TTFFile ttfFile;
			for(int i = 0; i < tccHeader.NumFonts; i++) {
				ttfFile = new TTFFile(tccHeader.OffsetTable[i]);
				ttfFile.ReadNameTable(ttfStream);
				if(ttfFile.Name.FamilyName == fontName) {
					ttfFile.Read(data);
					ttfFile.FontCodePage = fontCodePage;
					return ttfFile;
				}
			}
			return null;
		}
	}
	class TTCHeaderBase {
		public static TTCHeaderBase CreateTCCHeader(TTFStream ttfStream) {
			ttfStream.Seek(4);
			uint version = ttfStream.ReadULong();
			return version == 0x00010000 ? new TTCHeaderVer1() : new TTCHeaderVer2();
		}
		byte[] tag;
		uint version;
		uint numFonts;
		uint[] offsetTable;
		public uint NumFonts { get { return numFonts; } }
		public uint[] OffsetTable { get { return offsetTable; } }
		public virtual void Read(TTFStream ttfStream) {
			ttfStream.Seek(0);
			tag = ttfStream.ReadBytes(4);
			version = ttfStream.ReadULong();
			numFonts = ttfStream.ReadULong();
			offsetTable = new uint[numFonts];
			for(int i = 0; i < numFonts; i++) {
				offsetTable[i] = ttfStream.ReadULong();
			}
		}
	}
	class TTCHeaderVer1 : TTCHeaderBase {
	}
	class TTCHeaderVer2 : TTCHeaderVer1 {
		uint ulDsigTag;
		uint ulDsigLength;
		uint ulDsigOffset;
		public override void Read(TTFStream ttfStream) {
			base.Read(ttfStream);
			ulDsigTag = ttfStream.ReadULong();
			ulDsigLength = ttfStream.ReadULong();
			ulDsigOffset = ttfStream.ReadULong();
		}
	}
}
