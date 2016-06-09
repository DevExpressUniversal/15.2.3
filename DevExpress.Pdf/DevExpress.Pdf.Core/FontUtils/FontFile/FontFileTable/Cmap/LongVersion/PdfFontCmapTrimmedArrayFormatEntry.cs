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
	public class PdfFontCmapTrimmedArrayFormatEntry : PdfFontCmapLongFormatEntry {
		const int headerLength = 20;
		readonly int characterCount;
		readonly short[] glyphs;
		readonly int startCharacterCode;
		public int CharacterCount { get { return characterCount; } }
		public short[] Glyphs { get { return glyphs; } }
		public override int Length { get { return headerLength + glyphs.Length * 2; } }
		public int StartCharacterCode { get { return startCharacterCode; } }
		protected override PdfFontCmapFormatID Format { get { return PdfFontCmapFormatID.TrimmedArray; } }
		public PdfFontCmapTrimmedArrayFormatEntry(PdfFontPlatformID platformId, PdfFontEncodingID encodingId, PdfBinaryStream stream)
			: base(platformId, encodingId, stream) {
			startCharacterCode = stream.ReadInt();
			characterCount = stream.ReadInt();
			glyphs = stream.ReadShortArray(characterCount);
		}
		public override void Write(PdfBinaryStream tableStream) {
			base.Write(tableStream);
			tableStream.WriteInt(startCharacterCode);
			tableStream.WriteInt(characterCount);
			tableStream.WriteShortArray(glyphs);
		}
	}
}
