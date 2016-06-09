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
	public class PdfFontCmapTrimmedMappingFormatEntry : PdfFontCmapShortFormatEntry {
		readonly short firstCode;
		readonly short entryCount;
		readonly short[] glyphIdArray;
		public short FirstCode { get { return firstCode; } }
		public short EntryCount { get { return entryCount; } }
		public short[] GlyphIdArray { get { return glyphIdArray; } }
		public override int Length { get { return HeaderLength + 4 + glyphIdArray.Length * 2; } }
		protected override PdfFontCmapFormatID Format { get { return PdfFontCmapFormatID.TrimmedMapping; } }
		public PdfFontCmapTrimmedMappingFormatEntry(PdfFontPlatformID platformId, PdfFontEncodingID encodingId, PdfBinaryStream stream) : base(platformId, encodingId, stream) {
			firstCode = stream.ReadShort();
			entryCount = stream.ReadShort();
			glyphIdArray = stream.ReadShortArray(entryCount);
		}
		public override void Write(PdfBinaryStream tableStream) {
			base.Write(tableStream);
			tableStream.WriteShort(firstCode);
			tableStream.WriteShort(entryCount);
			tableStream.WriteShortArray(glyphIdArray);
		}
		public override int MapCode(char character) {
			int code = character - firstCode;
			return code >= 0 && code < entryCount ? (ushort)glyphIdArray[code] : MissingGlyphIndex;
		}
	}
}
